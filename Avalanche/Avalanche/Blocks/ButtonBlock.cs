using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    class ButtonBlock : Button, IRenderable, IHasBlockMessenger
    {
        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        public View Render()
        {
            MessageHandler.Response += MessageHandler_Response;
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
            var body = new Dictionary<string, string> { { "Test", "OK" }, { "Second", "Yes" } };
            MessageHandler.Post( "", body );
            //AvalancheNavigation.GetPage( Attributes["PageNumber"] );
        }
    }
}