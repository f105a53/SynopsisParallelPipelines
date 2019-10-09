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
            var step1 = new PipelineStep<string, string>(input, () => new DelayReturnPipeline<string>(), 2);
            alltasks.Add(step1.Process());
            alltasks.Add(Task.Run(() =>
            {
                while (step1.Output.IsAddingCompleted == false)
                {
                    var take = step1.Output.Take();
                    Console.WriteLine($"Output {take}");
                }
            }));
            Console.WriteLine("Setup finished");

            await Task.Delay(500);

            for (int i = 0; i >= 0; i++)
            {
                await Task.Delay(500);
                input.Add(i.ToString());
                
            }

            await Task.WhenAll(alltasks);
        }
    }
}