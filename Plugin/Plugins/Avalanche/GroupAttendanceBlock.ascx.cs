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
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Security;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Group Attendance Block" )]
    [Category( "Avalanche" )]
    [Description( "Mobile block to take group attendance." )]


    public partial class GroupAttendanceBlock : AvalancheBlock
    {
        private List<GroupMember> groupMembers;
        private Group group;
        private RockContext rockContext;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
        }

        public override MobileBlock GetMobile( string parameter )
        {
            GetGroupMembers( parameter );

            if ( groupMembers == null )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.LabelBlock",
                    Attributes = new Dictionary<string, string>
                    {
                        { "Text", "Sorry, you are not authorized to update the attendance for the selected group."},
                        { "BackgroundColor", "#f8d7da"},
                        { "TextColor", "#941c24" },
                        { "Margin", "5,10" }
                    }
                };
            }

            List<FormElementItem> elements = GetForm();
            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( elements ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };
        }

        private List<FormElementItem> GetForm( string currentOccurance = "", bool? didNotMeetOverride = null )
        {

            List<FormElementItem> form = new List<FormElementItem>();

            if ( group.Schedule != null )
            {
                AttendanceService attendanceService = new AttendanceService( rockContext );
                var occurances = attendanceService.Queryable().Where( a => a.Occurrence.GroupId == group.Id )
                    .DistinctBy( s => s.StartDateTime )
                    .Select( s => s.StartDateTime )
                    .Take( 50 )
                    .ToList()
                    .ToDictionary( s => ( s - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds.ToString(),
                                   s => s.ToString( "MMM d, yyyy -  h:mmtt" ) );

                if ( group.Schedule.ScheduleType == ScheduleType.Named
                    || group.Schedule.ScheduleType == ScheduleType.Custom )
                {

                    var prevSchedules = group.Schedule
                        .GetScheduledStartTimes( Rock.RockDateTime.Today.AddYears( -1 ), Rock.RockDateTime.Today.AddDays( 1 ) )
                        .OrderByDescending( o => o )
                        .Take( 10 )
                        .ToDictionary( s => ( s - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds.ToString(),
                                         s => s.ToString( "MMM d, yyyy -  h:mmtt" ) );
                    foreach ( var prev in prevSchedules )
                    {
                        occurances.AddOrIgnore( prev.Key, prev.Value );
                    }
                }
                else if ( group.Schedule.ScheduleType == ScheduleType.Weekly )
                {
                    var schedules = new List<DateTime>();

                    DateTime lastSchedule = Rock.RockDateTime.Today;
                    //Crawl backward to find the last time this occured
                    while ( lastSchedule.DayOfWeek != group.Schedule.WeeklyDayOfWeek )
                    {
                        lastSchedule = lastSchedule.AddDays( -1 );
                    }
                    lastSchedule = lastSchedule.AddMinutes( group.Schedule.WeeklyTimeOfDay.Value.TotalMinutes );
                    schedules.Add( lastSchedule );
                    for ( int i = 1; i < 10; i++ )
                    {
                        schedules.Add( lastSchedule.AddDays( i * -7 ) );
                    }

                    foreach ( var schedule in schedules )
                    {
                        occurances.AddOrIgnore( ( schedule - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds.ToString(), schedule.ToString( "MMM d, yyyy -  h:mmtt" ) );
                    }
                }
                if ( occurances.Any() && string.IsNullOrWhiteSpace( currentOccurance ) )
                {
                    currentOccurance = occurances.Last().Key;
                }

                FormElementItem datePicker = new FormElementItem
                {
                    Type = FormElementType.Picker,
                    Options = occurances,
                    Value = currentOccurance,
                    Key = "schedule",
                    Label = "Schedule",
                    AutoPostBack = true
                };

                form.Add( datePicker );


                var didNotMeet = false;


                if ( !string.IsNullOrWhiteSpace( currentOccurance ) )
                {
                    //The drop down stores the time in unix time
                    var occurenceDate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Local )
                         .AddSeconds( currentOccurance.AsInteger() );

                    var attendanceData = new AttendanceService( rockContext )
                        .Queryable()
                        .Where( a => a.Occurrence.GroupId == group.Id && a.StartDateTime == occurenceDate )
                        .ToList();

                    if ( didNotMeetOverride == null )
                    {
                        didNotMeet = ( attendanceData.Where( a => a.DidAttend == true ).Count() <= 0
                                       && attendanceData.Where( a => a.Occurrence.DidNotOccur == true ).Count() > 0 );
                    }
                    else
                    {
                        didNotMeet = didNotMeetOverride.Value;
                    }

                    if ( !didNotMeet )
                    {

                        var items = groupMembers
                            .Where( gm => gm.GroupMemberStatus == GroupMemberStatus.Active )
                            .OrderBy( gm => gm.Person.LastName )
                            .ThenBy( gm => gm.Person.NickName )
                            .Select( m => new
                            {
                                PersonId = m.PersonId.ToString(),
                                FullName = m.ToString(),
                                Attended = ( attendanceData.Where( a => a.PersonAlias.PersonId == m.PersonId ).Any()
                                    && ( attendanceData.Where( a => a.PersonAlias.PersonId == m.PersonId ).FirstOrDefault().DidAttend ?? false ) )
                            }
                            )
                            .ToList();

                        var checkBoxOptions = new Dictionary<string, string>();
                        var selectedPeople = new List<string>();
                        foreach ( var item in items )
                        {
                            checkBoxOptions.Add( item.PersonId, item.FullName );
                            if ( item.Attended )
                            {
                                selectedPeople.Add( item.PersonId );
                            }
                        }
                        var checkBoxList = new FormElementItem
                        {
                            Type = FormElementType.CheckboxList,
                            Key = "groupMembers",
                            Label = "Group Members",
                            Options = checkBoxOptions,
                            Value = string.Join( ",", selectedPeople )
                        };

                        form.Add( checkBoxList );
                    }
                }

                var didNotMeetSwitch = new FormElementItem
                {
                    Type = FormElementType.Switch,
                    Key = "didNotMeet",
                    Label = "Did Not Meet",
                    Value = didNotMeet.ToString(),
                    AutoPostBack = true
                };
                form.Add( didNotMeetSwitch );

                var button = new FormElementItem
                {
                    Label = "Save",
                    Key = "save",
                    Type = FormElementType.Button
                };
                form.Add( button );

                var groupId = new FormElementItem
                {
                    Type = FormElementType.Hidden,
                    Key = "groupId",
                    Value = group.Id.ToString()
                };
                form.Add( groupId );

            }
            return form;
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> body )
        {

            if ( request == "save" )
            {
                return SaveAttendance( body );
            }

            GetGroupMembers( body["groupId"] );

            if ( body.ContainsKey( "schedule" ) )
            {
                bool? didNotMeetOverride = null;
                if ( request == "didNotMeet" )
                {
                    didNotMeetOverride = body["didNotMeet"].AsBoolean();
                }

                var response = new FormResponse
                {
                    Success = true,
                    FormElementItems = GetForm( body["schedule"], didNotMeetOverride: didNotMeetOverride )
                };

                return new MobileBlockResponse()
                {
                    Request = "save",
                    Response = JsonConvert.SerializeObject( response ),
                    TTL = 0
                };
            }

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( "" ),
                TTL = 0
            };
        }

        private MobileBlockResponse SaveAttendance( Dictionary<string, string> body )
        {
            GetGroupMembers( body["groupId"] );

            if ( group != null )
            {
                if ( body["schedule"].AsInteger() != 0 )
                {
                    //The drop down stores the time in unix time
                    var occurenceDate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Local )
                         .AddSeconds( body["schedule"].AsInteger() );

                    var attendanceService = new AttendanceService( rockContext );
                    var attendanceData = attendanceService
                        .Queryable( "PersonAlias" )
                        .Where( a => a.Occurrence.GroupId == group.Id && a.StartDateTime == occurenceDate );

                    var personAliasService = new PersonAliasService( rockContext );

                    foreach ( var item in groupMembers )
                    {
                        var personId = item.PersonId;
                        var attendanceItem = attendanceData.Where( a => a.PersonAlias.PersonId == personId )
                            .FirstOrDefault();
                        if ( attendanceItem == null )
                        {
                            var attendancePerson = new PersonService( rockContext ).Get( personId );
                            if ( attendancePerson != null )
                            {
                                attendanceService.AddOrUpdate( attendancePerson.PrimaryAliasId ?? 0, occurenceDate, group.Id, null, group.ScheduleId, group.CampusId );
                            }
                        }

                        if ( body["didNotMeet"].AsBoolean() )
                        {
                            attendanceItem.DidAttend = false;
                            attendanceItem.Occurrence.DidNotOccur = true;
                        }
                        else
                        {
                            List<string> groupMemberAttendedIds = groupMemberAttendedIds = body["groupMembers"].SplitDelimitedValues( false ).ToList();
                            attendanceItem.Occurrence.DidNotOccur = false;
                            attendanceItem.DidAttend = groupMemberAttendedIds.Contains( personId.ToString() );
                        }
                    }
                }

                rockContext.SaveChanges();

                var response = new FormResponse
                {
                    Success = true,
                    Message = "Attendance Saved",
                    FormElementItems = GetForm( body["schedule"] )
                };

                return new MobileBlockResponse()
                {
                    Request = "save",
                    Response = JsonConvert.SerializeObject( response ),
                    TTL = 0
                };
            }

            var errorResponse = new FormResponse
            {
                Success = false,
                Message = "There was an errror saving your attendance."
            };

            return new MobileBlockResponse()
            {
                Request = "save",
                Response = JsonConvert.SerializeObject( errorResponse ),
                TTL = 0
            };
        }

        private void GetGroupMembers( string parameter )
        {
            if ( CurrentPerson == null )
            {
                return;
            }

            var groupId = parameter.AsInteger();
            rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            group = groupService.Get( groupId );

            if ( group == null )
            {
                return;
            }

            if ( group.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
            {
                groupMembers = group.Members.ToList();
            }
            return;
        }
    }
}