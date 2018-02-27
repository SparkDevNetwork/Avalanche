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

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Preload Block" )]
    [Category( "Avalanche" )]
    [Description( "Block to preload pages in Avalanche." )]

    [KeyValueListField( "Pages To Preload", "Pages with parameters to preload upon launching current page.", false, keyPrompt: "Page Id", valuePrompt: "Parameter" )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    public partial class PreloadBlock : AvalancheBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {

        }

        public override MobileBlock GetMobile( string parameter )
        {
            var pageList = new List<string>();
            var pages = GetAttributeValue( "PagesToPreload" ).ToKeyValuePairList();
            foreach ( var item in pages )
            {
                var page = "/api/avalanche/page/" +
                    item.Key + "/" +
                    AvalancheUtilities.ProcessLava( ( string ) item.Value,
                                                    CurrentPerson,
                                                    parameter,
                                                    GetAttributeValue( "EnabledLavaCommands" )
                                                    );
                pageList.Add( page );
            }

            CustomAttributes.Add( "Resources", string.Join( "|", pageList ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Preload",
                Attributes = CustomAttributes
            };
        }
    }
}