using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    class Preload : ContentView, IRenderable
    {
        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            Task.Factory.StartNew( new Action( BackgroundAction ) );
            return this;
        }

        private void BackgroundAction()
        {
            if ( Attributes.ContainsKey( "Resources" ) && !string.IsNullOrWhiteSpace( Attributes["Resources"] ) )
            {
                ObservableResource<MobilePage> observableResource = new ObservableResource<MobilePage>();
                var resources = Attributes["Resources"].Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );
                foreach (var resource in resources )
                {
                    RockClient.GetResource( observableResource, resource );
                }
            }
        }
    }
}
