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
    [DisplayName( "Markdown Detail" )]
    [Category( "Avalanche" )]
    [Description( "A control to display Markdown." )]
    [CodeEditorField( "Markdown", "Markdown code to be rendered in app using the {{parameter}}.", Rock.Web.UI.Controls.CodeEditorMode.Markdown,
        Rock.Web.UI.Controls.CodeEditorTheme.Rock, 600, false )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    public partial class MarkdownDetail : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbMarkdown.Text = GetAttributeValue( "Markdown" );
        }

        public override MobileBlock GetMobile( string parameter )
        {

            CustomAttributes["Content"] = AvalancheUtilities.ProcessLava(
                GetAttributeValue( "Markdown" ),
                CurrentPerson,
                parameter,
                GetAttributeValue( "EnabledLavaCommands" ) );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.MarkdownDetail",
                Attributes = CustomAttributes
            };
        }
    }
}