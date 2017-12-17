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

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Group Member List Block" )]
    [Category( "Avalanche" )]
    [Description( "Mobile block to show group members of a group." )]

    [LinkedPage( "Detail Page", "The page to navigate to for details.", false, "", "", 1 )]
    public partial class GroupMemberListBlock : AvalancheBlock
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
            if ( GetGroupMembers( arg ) == null )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.Null",
                    Attributes = CustomAttributes
                };
            }

            CustomAttributes["Resource"] = arg;

            var pageGuid = GetAttributeValue( "DetailPage" );
            PageCache page = PageCache.Read( pageGuid.AsGuid() );
            if ( page != null && page.IsAuthorized( "View", CurrentPerson ) )
            {
                CustomAttributes["DetailPage"] = page.Id.ToString();
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body )
        {
            var groupMembers = GetGroupMembers( resource );
            if ( groupMembers == null )
            {
                return new MobileBlockResponse()
                {
                    Arg = resource,
                    Response = JsonConvert.SerializeObject( new List<MobileListView>() ),
                    TTL = GetAttributeValue( "OutputCacheDuration" ).AsInteger()
                };
            }

            var members = groupMembers.Select( m => new
            {
                Id = m.Guid.ToString(),
                Title = m.Person.FullName,
                Image = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + m.Person.PhotoUrl,
                Subtitle = m.GroupRole.Name
            } ).ToList();

            return new MobileBlockResponse()
            {
                Arg = resource,
                Response = JsonConvert.SerializeObject( members ),
                TTL = GetAttributeValue( "OutputCacheDuration" ).AsInteger()
            };
        }

        private List<GroupMember> GetGroupMembers( string arg )
        {
            if ( CurrentPerson == null )
            {
                return null;
            }

            var groupGuid = arg.AsGuid();
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            var group = groupService.Get( groupGuid );

            if ( group == null )
            {
                return null;
            }

            var personId = CurrentPerson.Id;
            var members = group.Members;
            var leaders = members.Where( m => m.PersonId == personId && m.GroupRole.IsLeader );
            if ( !leaders.Any() )
            {
                return null;
            }

            return members.ToList();
        }

    }
}