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
    [DisplayName( "Mobile Content Item" )]
    [Category( "Avalanche" )]
    [Description( "Block to show a mobile content item." )]

    [ContentChannelField( "Content Channel", "Content channel to limit content items to. " )]
    [TextField( "Title Lava", "Lava to dispay {{Item}} title.", false, "{{Item.Title}}" )]
    [TextField( "Markdown Lava", "Lava to display markdown {{Item}} content.", false )]
    [TextField( "Image Lava", "Lava to display image from the {{Item}}", false )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]

    public partial class MobileContentItem : AvalancheBlock
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
            RockContext rockContext = new RockContext();
            ContentChannelItemService contentChannelItemService = new ContentChannelItemService( rockContext );
            var item = contentChannelItemService.Get( parameter.AsInteger() );
            if ( item != null && item.IsAuthorized( "View", CurrentPerson )
                && ( string.IsNullOrWhiteSpace( GetAttributeValue( "ContentChannel" ) ) || item.ContentChannel.Guid == GetAttributeValue( "ContentChannel" ).AsGuid() ) )
            {
                CustomAttributes["Success"] = "true";
                CustomAttributes["Title"] = ProcessLava( GetAttributeValue( "TitleLava" ), item );
                CustomAttributes["Markdown"] = ProcessLava( GetAttributeValue( "MarkdownLava" ), item );
                CustomAttributes["Image"] = ProcessLava( GetAttributeValue( "ImageLava" ), item );
            }
            else
            {
                CustomAttributes["Success"] = "false";
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.MobileContentItem",
                Attributes = CustomAttributes
            };
        }

        private string ProcessLava( string lava, ContentChannelItem item )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, CurrentPerson );
            mergeObjects["Item"] = item;
            return lava.ResolveMergeFields( mergeObjects, null, GetAttributeValue( "EnabledLavaCommands" ) );
        }
    }
}