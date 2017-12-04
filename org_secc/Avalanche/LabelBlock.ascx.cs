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
    [DisplayName( "Label Block" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A button." )]

    [TextField( "Text", "The text of the label to be displayed.", false )]
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public partial class LabelBlock : RockBlock, IMobileResource
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbLabel.Text = GetAttributeValue( "Text" );
            var fontsize = GetAttributeValue( "FontSize" ).AsInteger();
            if ( fontsize != 0 )
            {
                lbLabel.Style.Add( "font-size", GetAttributeValue( "FontSize" ) + "px" );
            }
            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "TextColor" ) ) )
            {
                lbLabel.Style.Add( "color", GetAttributeValue( "TextColor" ) );
            }
        }

        public MobileBlock GetMobile()
        {
            var attributes = new Dictionary<string, string>
            {
                { "Text", GetAttributeValue("Text") }
            };

            var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
            foreach ( var item in customs )
            {
                attributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.LabelBlock",
                Body = attributes
            };
        }

        public Dictionary<string, string> HandlePostback( Dictionary<string, string> Body )
        {
            return Body;
        }
    }
}