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
    [DisplayName( "Html Detail" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A control to display Markdown." )]
    [CodeEditorField( "Html", "Html code to be rendered in app.", Rock.Web.UI.Controls.CodeEditorMode.Html,
        Rock.Web.UI.Controls.CodeEditorTheme.Rock, 600, false )]
    public partial class HtmlDetail : RockBlock, IMobileResource
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbHtml.Text = GetAttributeValue( "Html" );
        }

        public MobileBlock GetMobile( string arg )
        {
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.HtmlDetail",
                Body = new Dictionary<string, string>
                {
                    { "Content", GetAttributeValue("Html") }
                }
            };
        }
        public MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body )
        {
            throw new NotImplementedException();
        }
    }
}