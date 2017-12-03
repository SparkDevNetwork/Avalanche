using System;
using System.Collections.Generic;
using System.Text;
using Avalanche;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    public class GenericBlock : IRenderable
    {
        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return new Label()
            {
                Text = Attributes["Content"]
            };
        }
    }
}
