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
using Avalanche.CustomControls;
using Avalanche.Interfaces;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    public class IconBlock : IconLabel, IRenderable
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

            if ( Attributes.ContainsKey( "Text" ) )
            {
                base.Text = Attributes["Text"];
            }

            return this;
        }

        private void Tgr_Tapped( object sender, EventArgs e )
        {
            AttributeHelper.HandleActionItem( Attributes );
        }
    }
}
