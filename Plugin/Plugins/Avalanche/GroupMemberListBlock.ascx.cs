// <copyright>
// Copyright Southeast Christian Church

//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
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
    [DisplayName( "Group Member List Block" )]
    [Category( "Avalanche" )]
    [Description( "Mobile block to show group members of a group." )]

    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [IntegerField( "Members Per Request", "The number of members to get per request. All group members will be loaded, but in multiple requests.", true, 20 )]
    public partial class GroupMemberListBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {

        }

        public override MobileBlock GetMobile( string parameter )
        {
            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Read( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            var groupMembers = GetGroupMembers( parameter, 0 );
            if ( groupMembers == null || !groupMembers.Any() )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.Null",
                    Attributes = CustomAttributes
                };
            }

            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                   CustomAttributes,
                                   CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                   GetAttributeValue( "EnabledLavaCommands" ),
                                   parameter );

            CustomAttributes["InitialRequest"] = parameter + "|0";
            CustomAttributes["NextRequest"] = parameter + "|1";
            CustomAttributes["Content"] = JsonConvert.SerializeObject( groupMembers );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var splitRequest = request.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );
            if ( splitRequest.Length < 2 )
            {
                return new MobileBlockResponse() // send nothing if bad request
                {
                    Request = request,
                    Response = JsonConvert.SerializeObject( new List<ListViewResponse>() ),
                    TTL = 0
                };
            }

            var parameter = splitRequest[0];
            var page = splitRequest[1].AsInteger();

            var groupMembers = GetGroupMembers( parameter, page );
            if ( groupMembers == null )
            {
                return new MobileBlockResponse() // send nothing if no members
                {
                    Request = request,
                    Response = JsonConvert.SerializeObject( new List<ListViewResponse>() ),
                    TTL = 0
                };
            }

            var response = new ListViewResponse
            {
                Content = groupMembers,
                NextRequest = string.Format( "{0}|{1}", parameter, ( page + 1 ) )
            };

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };
        }

        private List<ListElement> GetGroupMembers( string parameter, int page )
        {
            if ( CurrentPerson == null )
            {
                return null;
            }

            var groupId = parameter.AsInteger();
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            var group = groupService.Get( groupId );

            if ( group == null )
            {
                return null;
            }

            if ( !group.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
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
            var count = GetAttributeValue( "MembersPerRequest" ).AsInteger();
            if ( count < 1 )
            {
                count = 20;
            }

            return members
                .OrderByDescending( m => m.GroupRole.IsLeader )
                .ThenBy( m => m.Person.FirstName )
                .ThenBy( m => m.Person.LastName )
                .Skip( page * count )
                .Take( count )
                .ToList()
                .Where( m => m.GroupMemberStatus != GroupMemberStatus.Inactive )
                .Select( m => new ListElement
                {
                    Id = m.Guid.ToString(),
                    Title = m.Person.FullName,
                    Image = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + m.Person.PhotoUrl + "&width=100",
                    Description = m.GroupRole.Name
                } )
                .ToList();
        }
    }
}