using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    class SaveImageStep : IPipeline<ImageInfo,ImageInfo>
    {
        public async Task<ImageInfo> Process(ImageInfo input)
        {
            await Task.Run(() =>
            input.ImageResized.Save($"../../../Images/{input.Name} Edited.jpg")
            );
            input.ImageSaved = true;
            return input;
        }
    }
}
