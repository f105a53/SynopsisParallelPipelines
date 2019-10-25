using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class LoadImageStep : IPipeline<ImageInfo, ImageInfo>
    {
        public async Task<ImageInfo> Process(ImageInfo input)
        {
            // Sometimes throw to illustrate exception handling
            var r = new Random();
            if (r.Next(0,4) == 0)
            {
                throw new Exception("Example to showcase exception handling");
            }
            // Load normally
            var image = await Task.Run(() => Image.Load(input.Path));
            input.ImageNotResized = image;
            return input;
        }
    }
}
