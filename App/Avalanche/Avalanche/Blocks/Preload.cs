// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
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
