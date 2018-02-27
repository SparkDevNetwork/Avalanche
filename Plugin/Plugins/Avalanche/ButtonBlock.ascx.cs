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
using System.ComponentModel;
using Rock.Model;
using Rock.Security;
using System.Web.UI;
using Rock.Web.Cache;
using Rock.Web.UI;
using System.Web;
using Rock.Data;
using System.Linq;
using System.Collections.Generic;
using Rock;
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;
using Avalanche.Attribute;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Button" )]
    [Category( "Avalanche" )]
    [Description( "A button." )]

    [ActionItemField( "Action Item", "", false )]
    [TextField( "Text", "The text of the label to be displayed. Lava enabled with the {{parameter}} available.", false )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    public partial class ButtonBlock : AvalancheBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            btnButton.Text = GetAttributeValue( "Text" );
        }

        public override MobileBlock GetMobile( string parameter )
        {

            CustomAttributes.Add( "Text", AvalancheUtilities.ProcessLava( GetAttributeValue( "Text" ),
                                                                          CurrentPerson,
                                                                          parameter,
                                                                          GetAttributeValue( "EnabledLavaCommands" ) ) );

            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ), CustomAttributes, CurrentPerson );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ButtonBlock",
                Attributes = CustomAttributes
            };
        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
        }
    }
}