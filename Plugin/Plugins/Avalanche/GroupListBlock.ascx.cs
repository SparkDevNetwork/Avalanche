// <copyright>
// Copyright Southeast Christian Church
// Copyright Mark Lee
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

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Group List" )]
    [Category( "Avalanche" )]
    [Description( "Way to show list of groups" )]

    [LinkedPage( "Detail Page", "The page to navigate to for details.", false, "", "", 1 )]
    [TextField( "Parent Group Ids", "Comma separated list of id's of parent groups to look into." )]
    public partial class GroupListBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                RockContext rockContext = new RockContext();
                GroupService groupService = new GroupService( rockContext );

                var ids = new List<int>();
                var strings = GetAttributeValue( "ParentGroupIds" ).Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                foreach ( var s in strings )
                {
                    var id = s.AsInteger();
                    if ( id != 0 )
                    {
                        ids.Add( id );
                    }
                }

                var parentGroups = groupService.Queryable().Where( g => ids.Contains( g.Id ) ).Select( g => g.Name ).ToList();
                lbGroups.Text = String.Join( "<br>", parentGroups );
            }
        }

        public override MobileBlock GetMobile( string parameter )
        {
            var pageGuid = GetAttributeValue( "DetailPage" );
            PageCache page = PageCache.Read( pageGuid.AsGuid() );
            if ( page != null && page.IsAuthorized( "View", CurrentPerson ) )
            {
                CustomAttributes["ActionType"] = "1";
            }

            CustomAttributes["InitialRequest"] = "0"; //Request for pull to refresh
            var groups = GetGroupElements( 0 );
            CustomAttributes["Content"] = JsonConvert.SerializeObject( groups );
            if ( groups.Any() )
            {
                CustomAttributes["NextReqest"] = "1"; //Next request
            }

            CustomAttributes["Component"] = "Avalanche.Components.ListView.ColumnListView";

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            if ( request == "" )
            {
                return new MobileBlockResponse()
                {
                    Request = request,
                    Response = JsonConvert.SerializeObject( new ListViewResponse() ),
                    TTL = GetAttributeValue( "OutputCacheDuration" ).AsInteger()
                };
            }

            var start = request.AsInteger();
            List<ListElement> groups = GetGroupElements( start );

            var response = new ListViewResponse
            {
                Content = groups,
            };
            if ( groups.Any() )
            {
                response.NextRequest = ( start+1 ).ToString();
            }

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = GetAttributeValue( "OutputCacheDuration" ).AsInteger(),
            };
        }

        private List<ListElement> GetGroupElements( int start )
        {
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );

            var ids = new List<int>();
            var strings = GetAttributeValue( "ParentGroupIds" ).Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            foreach ( var s in strings )
            {
                var id = s.AsInteger();
                if ( id != 0 )
                {
                    ids.Add( id );
                }
            }
            var groupMemberService = new GroupMemberService( rockContext );

            int personId = 0;
            if ( CurrentPerson != null )
            {
                personId = CurrentPerson.Id;
            }

            var groups = groupService.Queryable()
                .Where( g => ids.Contains( g.Id ) )
                .SelectMany( g => g.Groups )
                .Join(
                    groupMemberService.Queryable(),
                    g => g.Id,
                    m => m.GroupId,
                    ( g, m ) => new { Group = g, Member = m }
                ).Where( m => m.Member.PersonId == personId && m.Member.GroupRole.IsLeader && m.Member.GroupMemberStatus == GroupMemberStatus.Active )
                .OrderBy( m => m.Group.Name )
                .Skip( start * 50 )
                .Take( 50 )
                .ToList() // leave sql server
                .Select( m => new ListElement
                {
                    Id = m.Group.Guid.ToString(),
                    Title = m.Group.Name,
                    Icon = m.Group.GroupType.IconCssClass,
                    Image = "",
                    Description = ""
                } )
                .DistinctBy( g => g.Id )
                .ToList();

            return groups;
        }
    }
}