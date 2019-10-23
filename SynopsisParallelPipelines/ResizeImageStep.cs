using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class ResizeImageStep<T> : IPipeline<T, T>
    {
        public async Task<T> Process(T input)
        {
            ImageInfo imageInfo = input as ImageInfo;

            Image i = imageInfo.ImageNotResized;
            i.Mutate(x => x
                    .Grayscale()
                    .Resize(i.Width / 2, i.Height / 2));
            imageInfo.ImageResized = i;

            return (T)Convert.ChangeType(imageInfo, typeof(T));
        }
    }
}
