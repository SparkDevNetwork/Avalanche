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
    [DisplayName( "Button" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A button." )]
    [TextField( "Text", "Text which will appear on the button." )]
    [TextField( "Page Number", "Number of the page to navigate to." )]
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public partial class ButtonBlock : RockBlock, IMobileResource
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            btnButton.Text = GetAttributeValue( "Text" );
        }

        public MobileBlock GetMobile( string arg )
        {
            var attributes = new Dictionary<string, string> {
                { "Text", GetAttributeValue("Text") },
                { "PageNumber", GetAttributeValue("PageNumber") }
            };

            var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
            foreach ( var item in customs )
            {
                attributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ButtonBlock",
                Body = attributes
            };
        }
        public MobileBlockResponse HandleRequest( string arg, Dictionary<string, string> Body )
        {
            var cu = CurrentUser;
            return new MobileBlockResponse()
            {
                Arg = arg,
                Response = Rock.RockDateTime.Now.ToString( "h:mm ss" ),
                TTL = 30
            };
        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
            Response.Redirect( "/page/" + GetAttributeValue( "PageNumber" ) );
        }
    }
}