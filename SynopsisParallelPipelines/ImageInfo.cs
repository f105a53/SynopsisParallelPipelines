using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynopsisParallelPipelines
{
    public class ImageInfo
    {
        public string Path { set; get; }
        public string Name { set; get; }
        public Image ImageNotResized { set; get; }
        public Image ImageResized { set; get; }
        public bool ImageSaved { set; get; }
        public override string ToString()
        {
            return this.Name;
        }
}
}
