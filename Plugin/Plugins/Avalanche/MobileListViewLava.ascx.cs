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
using Newtonsoft.Json;
using Avalanche.Attribute;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Mobile ListView Lava" )]
    [Category( "Avalanche" )]
    [Description( "Displays mobile list view from lava" )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [CodeEditorField( "Lava", "Lava to display list items.", Rock.Web.UI.Controls.CodeEditorMode.Lava, required: false )]
    public partial class MobileListViewLava : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lLava.Text = GetAttributeValue( "Lava" );
            lLavaRendered.Text = AvalancheUtilities.ProcessLava( GetAttributeValue( "Lava" ),
                                                                 CurrentPerson,
                                                                 enabledLavaCommands: GetAttributeValue( "EnabledLavaCommands" ) )
                                                   .ConvertMarkdownToHtml();
        }

        public override MobileBlock GetMobile( string parameter )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                   CustomAttributes,
                                   CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                   GetAttributeValue( "EnabledLavaCommands" ),
                                   parameter );

            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Read( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            CustomAttributes["Content"] = GetContent( parameter );
            CustomAttributes["InitialRequest"] = parameter;

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }
        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var content = GetContent( request );
            var response = "{\"Content\": " + content + "}";

            return new MobileBlockResponse()
            {
                Request = request,
                Response = response,
                TTL = PageCache.OutputCacheDuration
            };
        }

        private string GetContent( string parameter = "" )
        {
            return AvalancheUtilities.ProcessLava( GetAttributeValue( "Lava" ),
                                                               CurrentPerson,
                                                               parameter,
                                                               GetAttributeValue( "EnabledLavaCommands" ) );
        }
    }
}