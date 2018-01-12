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
using Newtonsoft.Json;
using Avalanche.Attribute;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Mobile ListView Lava" )]
    [Category( "Avalanche" )]
    [Description( "Displays mobile list view from lava" )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [CodeEditorField( "Lava", "Lava to display list items.", Rock.Web.UI.Controls.CodeEditorMode.Lava, required: false )]
    public partial class MobileListViewLava : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            tbLava.Text = GetAttributeValue( "Lava" );
        }

        public override MobileBlock GetMobile( string paramater )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ), CustomAttributes, CurrentPerson );
            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Read( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            CustomAttributes["Content"] = AvalancheUtilities.ProcessLava( GetAttributeValue( "Lava" ),
                                                           CurrentPerson,
                                                           enabledLavaCommands: GetAttributeValue( "EnabledLavaCommands" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }
    }
}