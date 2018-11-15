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
using System.Web.UI;
using Rock.Web.Cache;
using Rock.Data;
using System.Linq;
using System.Collections.Generic;
using Rock;
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;
using Avalanche.Attribute;
using Newtonsoft.Json;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Group List" )]
    [Category( "Avalanche" )]
    [Description( "Block to show a list of groups." )]

    [TextField( "Parent Group Ids", "Comma separated list of id's of parent groups to display." )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [CodeEditorField( "Lava", "Lava to display list items.", Rock.Web.UI.Controls.CodeEditorMode.Lava, defaultValue: defaultLava )]
    [BooleanField( "Only Show If Leader", "Should groups be hidden from all users except leaders?", true )]
    public partial class GroupListBlock : AvalancheBlock
    {
        public const string defaultLava = @"[
{% for group in Groups -%}
  { ""Id"":""{{group.Id}}"", ""Title"":""{{group.Name}}"", ""Description"":""{{group.Description}}"", ""Icon"":""{{group.GroupType.IconCssClass}}"" },
{% endfor -%}
]";

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

                var gr = new Group();
                var parentGroups = groupService.Queryable().Where( g => ids.Contains( g.Id ) ).Select( g => g.Name ).ToList();
                lbGroups.Text = String.Join( "<br>", parentGroups );
            }
        }

        public override MobileBlock GetMobile( string parameter )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                   CustomAttributes,
                                   CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                   GetAttributeValue( "EnabledLavaCommands" ),
                                   parameter );

            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Read( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            CustomAttributes["InitialRequest"] = parameter; //Request for pull to refresh
            var groups = GetGroups();
            Dictionary<string, object> mergeObjects = new Dictionary<string, object>
            {
                { "Groups", groups }
            };

            var content = AvalancheUtilities.ProcessLava( GetAttributeValue( "Lava" ),
                                                              CurrentPerson,
                                                              parameter,
                                                              GetAttributeValue( "EnabledLavaCommands" ),
                                                              mergeObjects );
            content = content.Replace("\\", "\\\\");
            CustomAttributes["Content"] = content;

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var groups = GetGroups();
            Dictionary<string, object> mergeObjects = new Dictionary<string, object>
            {
                { "Groups", groups }
            };
            var content = AvalancheUtilities.ProcessLava( GetAttributeValue( "Lava" ),
                                                              CurrentPerson,
                                                              request,
                                                              GetAttributeValue( "EnabledLavaCommands" ),
                                                              mergeObjects );
            content = content.Replace( "\\", "\\\\" );
            var response = "{\"Content\": " + content + "}";

            return new MobileBlockResponse()
            {
                Request = request,
                Response = response,
                TTL = PageCache.OutputCacheDuration
            };
        }

        private List<Group> GetGroups()
        {
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );

            var ids = new List<int>();
            var strings = GetAttributeValue( "ParentGroupIds" ).SplitDelimitedValues( false );
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

            var qry = groupService.Queryable()
                .Where( g => ids.Contains( g.Id ) )
                .SelectMany( g => g.Groups )
                .Join(
                    groupMemberService.Queryable(),
                    g => g.Id,
                    m => m.GroupId,
                    ( g, m ) => new { Group = g, Member = m } );
            if ( GetAttributeValue( "OnlyShowIfLeader" ).AsBoolean() )
            {
                qry = qry.Where( m => m.Member.PersonId == personId && m.Member.GroupRole.IsLeader && m.Member.GroupMemberStatus == GroupMemberStatus.Active );
            }
            var groups = qry
                .Select( m => m.Group )
                .DistinctBy( g => g.Id )
                .OrderBy( g => g.Name )
                .ToList();
            return groups;
        }
    }
}