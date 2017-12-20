using System;
using System.Collections.Generic;
using System.Text;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    class ImageBlock : FFImageLoading.Forms.CachedImage, IRenderable
    {
        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            TapGestureRecognizer tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            tgr.Tapped += Tgr_Tapped;
            this.GestureRecognizers.Add( tgr );


            return this;
        }

        private void Tgr_Tapped( object sender, EventArgs e )
        {
            AttributeHelper.HandleActionItem( Attributes );
        }
    }
}
