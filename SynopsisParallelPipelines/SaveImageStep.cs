using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    class SaveImageStep<T> : IPipeline<T, T>
    {
        public async Task<T> Process(T input)
        {
            ImageInfo imageInfo = input as ImageInfo;

            imageInfo.ImageResized.Save($"../../../Images/{imageInfo.Name} Edited.jpg");
            imageInfo.ImageSaved = true;

            return (T)Convert.ChangeType(imageInfo, typeof(T));
        }
    }
}
