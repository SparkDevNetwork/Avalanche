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
    [DisplayName( "Image Block" )]
    [Category( "Avalanche" )]
    [Description( "A button." )]

    [TextField( "Image", "Image to be displayed. Data is parsed through Lava with the request {{parameter}}.", false )]
    [CustomDropdownListField( "Aspect", "Aspect type", "0^AspectFit,1^AspectFill,2^Fit" )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [ActionItemField( "Action Item", "Action to take on touch.", false )]

    public partial class ImageBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            imgImage.ImageUrl = AvalancheUtilities.ProcessLava( GetAttributeValue( "Image" ),
                                                                CurrentPerson,
                                                                "",
                                                                GetAttributeValue( "EnabledLavaCommands" ) );
            lLava.Text = GetAttributeValue( "Image" );
        }

        public override MobileBlock GetMobile( string parameter )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ), CustomAttributes, CurrentPerson );

            CustomAttributes.Add( "Source", AvalancheUtilities.ProcessLava( GetAttributeValue( "Image" ),
                                                                            CurrentPerson,
                                                                            parameter,
                                                                            GetAttributeValue( "EnabledLavaCommands" )
                                                                            ) );
            CustomAttributes.Add( "Aspect", GetAttributeValue( "Aspect" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ImageBlock",
                Attributes = CustomAttributes
            };
        }
    }
}