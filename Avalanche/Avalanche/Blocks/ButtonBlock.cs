using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    class ButtonBlock : Button, IRenderable
    {
        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            Clicked += ButtonBlock_Clicked;
            return this;
        }

        private void ButtonBlock_Clicked( object sender, EventArgs e )
        {
            AvalancheNavigation.GetPage( "page/" + Attributes["PageNumber"] );
        }
    }
}