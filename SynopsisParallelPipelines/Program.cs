using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var alltasks = new List<Task>();
            var input = new BlockingCollection<string>();
            var step1 = new PipelineStep<string, string>("step1", input, () => new DelayReturnPipeline<string>(), 2);
            var step2 = new PipelineStep<string, string>("step2", step1.Output, () => new DelayReturnPipeline<string>(),
                2);
            alltasks.Add(step1.Process());
            alltasks.Add(step2.Process());
            alltasks.Add(Task.Run(() =>
            {
                while (step2.Output.IsAddingCompleted == false)
                {
                    var take = step2.Output.Take();
                    Console.WriteLine($"Output {take}");
                }
            }));
            Console.WriteLine("Setup finished");

            for (var i = 0; i < 10; i++)
            {
                //await Task.Delay(500);
                Console.WriteLine($"Input {i}");
                input.Add(i.ToString());
            }

            await Task.WhenAll(alltasks);
        }
    }
}