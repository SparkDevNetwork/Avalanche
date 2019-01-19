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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalanche;
using Avalanche.Attribute;
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Camp Member Leader List Block" )]
    [Category( "Avalanche" )]
    [Description( "Mobile block to show group members of a group for camp." )]

    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [KeyValueListField( "Group Ids", "Map the volunteer group ids to the student group ids", true, "", "Volunteer Group Id", "Student Group Id" )]
    [TextField( "Volunteer Group Key", "Key for the group attribute for the volunteer's group" )]
    [TextField( "Student Group Key", "Key for the group attribute for the students's group" )]
    [TextField( "Base URL", "Base url for loading images", defaultValue: "https://secc.org" )]
    public partial class CampMemberLeaderBlock : AvalancheBlock
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
            var value = DefinedValueCache.Get( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            var pairs = GetAttributeValue( "GroupIds" ).ToKeyValuePairList();
            RockContext rockContext = new RockContext();
            GroupMemberService groupMemberService = new GroupMemberService( rockContext );
            AttributeValueService attributeValueService = new AttributeValueService( rockContext );
            List<GroupMember> members = new List<GroupMember>();

            foreach ( var pair in pairs )
            {
                var volunteerId = pair.Key.AsInteger();
                var volunteers = groupMemberService.GetByGroupIdAndPersonId( volunteerId, CurrentPerson.Id );
                foreach ( var volunteer in volunteers )
                {
                    //Add the volunteer and any other volunteers
                    members.Add( volunteer );
                    volunteer.LoadAttributes();
                    var campGroupName = volunteer.GetAttributeValue( GetAttributeValue( "VolunteerGroupKey" ) );
                    var otherVolunteers = groupMemberService.GetByGroupId( volunteer.GroupId );
                    var volunterGroupAttributeValues = attributeValueService.GetByAttributeId( volunteer.Attributes[GetAttributeValue( "VolunteerGroupKey" )].Id );

                    foreach ( var otherVolunteer in otherVolunteers )
                    {
                        if ( volunterGroupAttributeValues.Where( av => av.EntityId == otherVolunteer.Id && av.Value == campGroupName ).Any() )
                        {
                            members.Add( otherVolunteer );
                        }
                    }

                    //Now add matching students
                    var students = groupMemberService.GetByGroupId( ( ( string ) pair.Value ).AsInteger() );
                    if ( students.Any() )
                    {
                        var firstStudent = students.FirstOrDefault();
                        firstStudent.LoadAttributes();
                        var studentGroupAttributeValues = attributeValueService.GetByAttributeId( firstStudent.Attributes[GetAttributeValue( "StudentGroupKey" )].Id );
                        foreach ( var student in students )
                        {
                            if ( studentGroupAttributeValues.Where( av => av.EntityId == student.Id && av.Value == campGroupName ).Any() )
                            {
                                members.Add( student );
                            }
                        }
                    }
                }
            }
            var url = GetAttributeValue( "BaseURL" );
            members = members.DistinctBy( gm => gm.PersonId ).ToList();
            var groupMembers = new List<ListElement>();
            foreach ( var member in members )
            {
                var groupMemeber = new ListElement
                {
                    Id = member.Guid.ToString(),
                    Title = member.Person.FullName,
                    Description = member.GroupRole.IsLeader ? "Leader" : "Student"
                };

                if ( member.Person.PhotoUrl.Contains( ".svg" ) )
                {
                    groupMemeber.Image = url + "/Content/SEApp/GroupMember.png";
                }
                else
                {
                    groupMemeber.Image = url + member.Person.PhotoUrl + "&mode=crop&scale=both&w=90&h=90&s.roundcorners=100&margin=5";
                }

                groupMembers.Add( groupMemeber );
            }

            if ( !members.Any() )
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
            CustomAttributes["Content"] = JsonConvert.SerializeObject( groupMembers );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }
    }
}