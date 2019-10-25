using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace SynopsisParallelPipelines
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            await ProcessImage();
        }

        private static async Task TestExample()
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

        private static IEnumerable<FileInfo> Crawl(DirectoryInfo dir)
        {
            foreach (var file in dir.EnumerateFiles())
            {
               yield return file;
            }

            foreach (var subdir in dir.EnumerateDirectories())
            {
                foreach (var file in Crawl(subdir))
                {
                    yield return file;
                }
            }
        }

        private static async Task ProcessImage()
        {
            var alltasks = new List<Task>();
            var input = new BlockingCollection<ImageInfo>();
            var step1 = new PipelineStep<ImageInfo, ImageInfo>("stepLoad", input, () => new LoadImageStep(), 1);
            var step2 = new PipelineStep<ImageInfo, ImageInfo>("stepResize", step1.Output, () => new ResizeImageStep(),2);
            var step3 = new PipelineStep<ImageInfo, ImageInfo>("stepSave", step2.Output, () => new SaveImageStep(), 1);
            alltasks.Add(step1.Process());
            alltasks.Add(step2.Process());
            alltasks.Add(step3.Process());
            alltasks.Add(Task.Run(() =>
            {
                while (step3.Output.IsAddingCompleted == false)
                {
                    var take = step3.Output.Take();
                    //Console.WriteLine($"Output: {take}");
                }
            }));
            Console.WriteLine("Setup finished, starting processing");

            var images = Crawl(new DirectoryInfo(@"C:\Users\Martynas\Desktop\pic")).Select(path => new ImageInfo {Path = path.FullName, Name = path.Name}).ToList();

            foreach (var image in images)
            {
                input.Add(image);
            }

            //input.CompleteAdding();
            await Task.WhenAll(alltasks);
        }
    }
}