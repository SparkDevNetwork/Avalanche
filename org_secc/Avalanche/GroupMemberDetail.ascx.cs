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
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Group Member Detail" )]
    [Category( "Avalanche" )]
    [Description( "Group member detail block." )]

    [TextField( "Accent Color", "Optional color to accent the member detail.", false )]
    [CodeEditorField( "Markdown Lava", "Markdown to display as group member details.", CodeEditorMode.Markdown )]
    public partial class GroupMemberDetail : AvalancheBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
        }

        public override MobileBlock GetMobile( string arg )
        {
            RockContext rockContext = new RockContext();
            GroupMemberService groupMemberService = new GroupMemberService( rockContext );
            var groupMember = groupMemberService.Get( arg.AsGuid() );

            if ( groupMember != null )
            {
                CustomAttributes["Name"] = groupMember.Person.FullName;
                CustomAttributes["Image"] = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + groupMember.Person.PhotoUrl;
                CustomAttributes["Markdown"] = ProcessLava( GetAttributeValue( "MarkdownLava" ), groupMember );

                CustomAttributes["AccentColor"] = GetAttributeValue( "AccentColor" );

                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.GroupMemberDetail",
                    Attributes = CustomAttributes
                };
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Null",
                Attributes = CustomAttributes
            };

        }

        private string ProcessLava( string lava, GroupMember groupMember )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, CurrentPerson );
            mergeObjects["GroupMember"] = groupMember;
            return lava.ResolveMergeFields( mergeObjects );
        }
    }
}