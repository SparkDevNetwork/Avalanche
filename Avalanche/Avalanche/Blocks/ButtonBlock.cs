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