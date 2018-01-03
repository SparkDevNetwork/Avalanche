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
using Avalanche.Attribute;
using Rock.Attribute;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Icon Button" )]
    [Category( "Avalanche" )]
    [Description( "An icon button" )]

    [TextField( "Text", "The text of the label to be displayed.", false )]
    [TextField( "Icon", "Icon to use on the button." )]
    [ActionItemField( "Action Item", "", false )]
    public partial class IconButton : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            var text = string.Format( "<i class='{0}'></i> {1}", GetAttributeValue( "Icon" ), GetAttributeValue( "Text" ) );
            btnButton.Text = text;

        }

        public override MobileBlock GetMobile( string parameter )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ), CustomAttributes, CurrentPerson );

            CustomAttributes.Add( "Text", AvalancheUtilities.ProcessLava( GetAttributeValue( "Text" ), CurrentPerson, parameter ) );
            CustomAttributes.Add( "Icon", AvalancheUtilities.ProcessLava( GetAttributeValue( "Icon" ), CurrentPerson, parameter ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.IconButton",
                Attributes = CustomAttributes
            };
        }
    }
}