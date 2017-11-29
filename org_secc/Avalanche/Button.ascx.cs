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

namespace RockWeb.Plugins.org_secc.Avalanche
{
    [DisplayName( "Button" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A button." )]
    [TextField( "Text", "Text which will appear on the button." )]
    [TextField( "Page Number", "Number of the page to navigate to." )]
    public partial class Button : RockBlock, IAvalanche
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            btnButton.Text = GetAttributeValue( "Text" );
        }

        public MobileBlock GetMobile()
        {
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Button",
                Body = new Dictionary<string, string>
                {
                    { "Text", GetAttributeValue("Text") },
                    { "PageNumber", GetAttributeValue("PageNumber") }
                }
            };
        }

        public Dictionary<string, string> HandlePostback( Dictionary<string, string> Body )
        {
            return Body;
        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
            Response.Redirect( "/page/" + GetAttributeValue( "PageNumber" ) );
        }
    }
}