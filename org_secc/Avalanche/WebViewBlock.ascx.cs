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
    [DisplayName( "WebViewBlock" )]
    [Category( "Avalanche" )]
    [Description( "A control to display Markdown." )]
    [TextField( "Url", "Webpage to display." )]
    [TextField( "Regex Limit", "If a page is opened that doesn't match this Regex. A request to open in external browser will appear." )]
    public partial class WebViewBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbHtml.Text = GetAttributeValue( "Url" );
        }

        public override MobileBlock GetMobile( string parameter )
        {
            CustomAttributes["Url"] = GetAttributeValue( "Url" );
            CustomAttributes["Domain"] = GetAttributeValue( "RegexLimit" );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.WebViewBlock",
                Attributes = CustomAttributes
            };
        }
    }
}