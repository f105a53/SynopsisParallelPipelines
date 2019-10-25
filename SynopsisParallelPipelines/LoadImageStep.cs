using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class LoadImageStep : IPipeline<ImageInfo, ImageInfo>
    {
        public async Task<ImageInfo> Process(ImageInfo input)
        {
            var image = await Task.Run(() => Image.Load(input.Path));
            input.ImageNotResized = image;
            return input;
        }
    }
}
