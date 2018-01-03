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
    [Category( "SECC > Avalanche" )]
    [Description( "A control to display Markdown." )]
    [CodeEditorField( "Markdown", "Markdown code to be rendered in app using the {{resource}}.", Rock.Web.UI.Controls.CodeEditorMode.Markdown,
        Rock.Web.UI.Controls.CodeEditorTheme.Rock, 600, false )]
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
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.MarkdownDetail",
                Attributes = new Dictionary<string, string>
                {
                    { "Content", AvalancheUtilities.ProcessLava( GetAttributeValue("Markdown"), CurrentPerson, parameter )},

                }
            };
        }
    }
}