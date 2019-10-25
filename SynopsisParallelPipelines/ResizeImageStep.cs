using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class ResizeImageStep : IPipeline<ImageInfo, ImageInfo>
    {
        public async Task<ImageInfo> Process(ImageInfo input)
        {
            Image imageResized = input.ImageNotResized;
            await Task.Run(() =>
            imageResized.Mutate(x => x
                    .Grayscale()
                    .Resize(imageResized.Width / 2, imageResized.Height / 2))
            );

            input.ImageResized = imageResized;
            return input;
        }
    }
}
