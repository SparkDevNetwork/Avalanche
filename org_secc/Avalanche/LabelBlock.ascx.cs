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
    [Category( " Avalanche" )]
    [Description( "A button." )]

    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [TextField( "Text", "The text of the label to be displayed.", false )]
    public partial class LabelBlock : AvalancheBlock
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

        public override MobileBlock GetMobile( string parameter )
        {
            CustomAttributes["Text"] = AvalancheUtilities.ProcessLava( GetAttributeValue( "Text" ),
                                                                       CurrentPerson,
                                                                       parameter,
                                                                       GetAttributeValue( "EnabledLavaCommands" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.LabelBlock",
                Attributes = CustomAttributes
            };
        }
    }
}