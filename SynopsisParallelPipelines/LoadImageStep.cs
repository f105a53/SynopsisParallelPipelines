using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class LoadImageStep<T> : IPipeline<T, T>
    {
        public async Task<T> Process(T input)
        {
            ImageInfo imageInfo = input as ImageInfo;
            var image = Image.Load(imageInfo.Path);

            imageInfo.ImageNotResized = image;


            return (T) Convert.ChangeType(imageInfo, typeof(T));
        }
    }
}
