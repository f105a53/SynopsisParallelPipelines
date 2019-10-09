using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class PipelineStep<TInput, TOutput>
    {
        private readonly BlockingCollection<TInput> _input;
        private readonly Func<IPipeline<TInput, TOutput>> _pipelineConstructor;
        private readonly SemaphoreSlim _semaphoreSlim;

        public PipelineStep(BlockingCollection<TInput> input, Func<IPipeline<TInput, TOutput>> pipelineConstructor,
            byte concurrency)
        {
            _input = input;
            _pipelineConstructor = pipelineConstructor;
            _semaphoreSlim = new SemaphoreSlim(concurrency, concurrency);
        }

        public BlockingCollection<TOutput> Output { get; } = new BlockingCollection<TOutput>();

        public async Task Process()
        {
            var alltasks = new List<Task>();
            //FIX: handle finished
            while (!_input.IsAddingCompleted)
            {
                var item = await Task.Run(() => _input.Take());
                Console.WriteLine($"Received {item}, waiting for semaphore");

                alltasks.Add(Task.Run(async () =>
                {
                    await _semaphoreSlim.WaitAsync();
                    Console.WriteLine($"Processing {item}");
                    var result = await _pipelineConstructor().Process(item);
                    Output.Add(result);
                    _semaphoreSlim.Release();

                    Console.WriteLine($"Processed {item}");
                }));
            }

            await Task.WhenAll(alltasks);
        }
    }
}