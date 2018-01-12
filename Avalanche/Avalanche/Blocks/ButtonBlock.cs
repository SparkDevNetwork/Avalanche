using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Avalanche.Models;
using Avalanche.Utilities;
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

        private void MessageHandler_Response( object sender, MobileBlockResponse e )
        {
            if ( !string.IsNullOrWhiteSpace( e?.Response ) )
            {
                Text = e.Response;
            }
        }

        private void ButtonBlock_Clicked( object sender, EventArgs e )
        {
            AttributeHelper.HandleActionItem( Attributes );
        }
    }
}