using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class PipelineStep<TInput, TOutput>
    {
        // The input we will take items from
        private readonly BlockingCollection<TInput> _input;
        // Name of the step, for logging purposes
        private readonly string _name;
        // A function to get the pipeline contructor
        private readonly Func<IPipeline<TInput, TOutput>> _pipelineConstructor;
        // Semaphore to limit concurrency
        private readonly SemaphoreSlim _semaphoreSlim;

        // Most of the variables are just assigned from parameters
        public PipelineStep(string name, BlockingCollection<TInput> input,
            Func<IPipeline<TInput, TOutput>> pipelineConstructor,
            byte concurrency)
        {
            _name = name;
            _input = input;
            _pipelineConstructor = pipelineConstructor;
            // We define how many concurrent items we can process
            _semaphoreSlim = new SemaphoreSlim(concurrency, concurrency);
        }

        // The output collection, public, so the next step can take items
        public BlockingCollection<TOutput> Output { get; } = new BlockingCollection<TOutput>();

        // The method that handles the work
        public async Task Process()
        {
            // We want to make sure all the tasks finished before we exit
            var alltasks = new List<Task>();

            // If the blocking collection has not finished
            while (!_input.IsAddingCompleted)
            {
                // Wait for an item from the collection
                var item = await Task.Run(() => _input.Take());

                // Start a new Task to process the item, don't wait, add to alltasks
                alltasks.Add(Task.Run(async () =>
                {
                    // Wait for the semaphore to be available
                    await _semaphoreSlim.WaitAsync();
                    Console.WriteLine($"{DateTime.Now.Ticks}\t{_name}:\tStart\t{item}");
                    try
                    {
                        // Do the actual work
                        var result = await _pipelineConstructor().Process(item);
                        // Output the result
                        Output.Add(result);
                        Console.WriteLine($"{DateTime.Now.Ticks}\t{_name}:\tFinish\t{item}");
                    }
                    catch (Exception e)
                    {
                        // If there's an exception, the result is not added to the output, so that item is no longer processed
                        System.Console.WriteLine($"{DateTime.Now.Ticks}\t{_name}:\tError\t{item}\n\n{e}\n\n");
                    }
                    finally
                    {
                        // Even if there was an exception in processing, release the semaphore, so the next Task can start
                        _semaphoreSlim.Release();
                    }
                    
                }));
            }
            
            // When there are no more items in the collection, wait for any remaining Tasks that are still running
            await Task.WhenAll(alltasks);
        }
    }
}
