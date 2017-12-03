using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    class ImageBlock : FFImageLoading.Forms.CachedImage, IRenderable
    {
        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return this;
        }
    }
}
