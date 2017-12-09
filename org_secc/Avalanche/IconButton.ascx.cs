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
    [DisplayName( "Icon Button" )]
    [Category( "SECC > Avalanche" )]
    [Description( "An icon button" )]

    [TextField( "Text", "The text of the label to be displayed.", false )]
    [TextField( "Icon", "Icon to use on the button." )]
    [IntegerField( "PageNumber", "The rock page number to link to." )]
    public partial class IconButton : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbLabel.Text = "[Icon Button]";
        }

        public override MobileBlock GetMobile( string arg )
        {
            var attributes = new Dictionary<string, string>
            {
            };

            var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
            foreach ( var item in customs )
            {
                attributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
            }

            attributes.Add( "Text", GetAttributeValue( "Text" ) );
            attributes.Add( "PageNumber", GetAttributeValue( "PageNumber" ) );
            attributes.Add( "Icon", GetAttributeValue( "Icon" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.IconButton",
                Attributes = attributes
            };
        }
    }
}