// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Store;
using System.Text;
using Rock.Security;
using System.Runtime.Caching;
using Avalanche;
using Avalanche.Models;
using Avalanche.Attribute;

namespace RockWeb.Plugins.Avalanche.Event
{
    /// <summary>
    /// Renders a particular calendar using Lava.
    /// </summary>
    [DisplayName( "Avalanche Event Calendar Lava" )]
    [Category( "Avalanche" )]
    [Description( "Renders a particular calendar using Lava." )]

    [EventCalendarField( "Event Calendar", "The event calendar to be displayed", true, "1", order: 0 )]
    [CustomDropdownListField( "Default View Option", "Determines the default view option", "Day,Week,Month,Year", true, "Week", order: 1 )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this HTML block.", false, order: 3 )]

    [CustomRadioListField( "Campus Filter Display Mode", "", "1^Hidden, 2^Plain, 3^Panel Open, 4^Panel Closed", true, "1", order: 4 )]
    [CustomRadioListField( "Audience Filter Display Mode", "", "1^Hidden, 2^Plain, 3^Panel Open, 4^Panel Closed", true, "1", key: "CategoryFilterDisplayMode", order: 5 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.MARKETING_CAMPAIGN_AUDIENCE_TYPE, "Filter Audiences", "Determines which audiences should be displayed in the filter.", false, true, key: "FilterCategories", order: 6 )]
    [BooleanField( "Show Date Range Filter", "Determines whether the date range filters are shown", false, order: 7 )]

    [BooleanField( "Show Small Calendar", "Determines whether the calendar widget is shown", true, order: 8 )]
    [BooleanField( "Show Day View", "Determines whether the day view option is shown", false, order: 9 )]
    [BooleanField( "Show Week View", "Determines whether the week view option is shown", true, order: 10 )]
    [BooleanField( "Show Month View", "Determines whether the month view option is shown", true, order: 11 )]
    [BooleanField( "Show Year View", "Determines whether the year view option is shown", true, order: 12 )]

    [BooleanField( "Enable Campus Context", "If the page has a campus context it's value will be used as a filter", order: 13 )]
    [TextField( "Cache Duration", "Ammount of time in minutes to cache the output of this block.", true, "3600", order: 14 )]
    [CodeEditorField( "Lava Template", "Lava template to use to display the list of events.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 400, true, @"{% include '~~/Assets/Lava/Calendar.lava' %}", "", 15 )]

    [DayOfWeekField( "Start of Week Day", "Determines what day is the start of a week.", true, DayOfWeek.Sunday, order: 16 )]

    [BooleanField( "Set Page Title", "Determines if the block should set the page title with the calendar name.", false, order: 17 )]

    [TextField( "Campus Parameter Name", "The page parameter name that contains the id of the campus entity.", false, "campusId", order: 18 )]
    [TextField( "Category Parameter Name", "The page parameter name that contains the id of the category entity.", false, "categoryId", order: 19 )]

    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    public partial class CalendarLava : AvalancheBlock
    {
        #region Fields

        private int _calendarId = 0;
        private string _calendarName = string.Empty;
        private DayOfWeek _firstDayOfWeek = DayOfWeek.Sunday;

        protected bool CampusPanelOpen { get; set; }
        protected bool CampusPanelClosed { get; set; }
        protected bool CategoryPanelOpen { get; set; }
        protected bool CategoryPanelClosed { get; set; }

        #endregion

        #region Properties

        private String ViewMode { get; set; }
        private DateTime? SelectedDate { get; set; }
        private DateTime? FilterStartDate { get; set; }
        private DateTime? FilterEndDate { get; set; }
        private List<DateTime> CalendarEventDates { get; set; }

        #endregion

        #region Base ControlMethods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        public override MobileBlock GetMobile( string parameter )
        {
            _firstDayOfWeek = GetAttributeValue( "StartofWeekDay" ).ConvertToEnum<DayOfWeek>();

            var eventCalendar = new EventCalendarService( new RockContext() ).Get( GetAttributeValue( "EventCalendar" ).AsGuid() );
            if ( eventCalendar != null )
            {
                _calendarId = eventCalendar.Id;
                _calendarName = eventCalendar.Name;
            }

            CampusPanelOpen = GetAttributeValue( "CampusFilterDisplayMode" ) == "3";
            CampusPanelClosed = GetAttributeValue( "CampusFilterDisplayMode" ) == "4";
            CategoryPanelOpen = !String.IsNullOrWhiteSpace( GetAttributeValue( "FilterCategories" ) ) && GetAttributeValue( "CategoryFilterDisplayMode" ) == "3";
            CategoryPanelClosed = !String.IsNullOrWhiteSpace( GetAttributeValue( "FilterCategories" ) ) && GetAttributeValue( "CategoryFilterDisplayMode" ) == "4";

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


            var data = BindData();
            CustomAttributes["Content"] = data;
            CustomAttributes["InitialRequest"] = parameter;

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }
        #endregion

        #region Events

        #endregion

        #region Methods

        /// <summary>
        /// Loads and displays the event item occurrences
        /// </summary>
        private string BindData()
        {
            var cacheKey = string.Format( "{0}CalendarLava", BlockCache.Id.ToString() );

            var cache = RockMemoryCache.Default;
            var content = ( string ) cache.Get( cacheKey );

            var rockContext = new RockContext();
            var eventItemOccurrenceService = new EventItemOccurrenceService( rockContext );
            var attributeService = new AttributeService( rockContext );
            var attributeValueService = new AttributeValueService( rockContext );

            int calendarItemEntityTypeId = EntityTypeCache.GetId( typeof( EventCalendarItem ) ).Value;

            // Grab events
            var qry = eventItemOccurrenceService
                    .Queryable( "EventItem, EventItem.EventItemAudiences,Schedule" )
                    .GroupJoin(
                        attributeService.Queryable(),
                        p => calendarItemEntityTypeId,
                        a => a.EntityTypeId,
                        ( eio, a ) => new { EventItemOccurrence = eio, EventCalendarItemAttributes = a }
                    )
                    .GroupJoin(
                        attributeValueService.Queryable().Where( av => av.Attribute.EntityTypeId == ( int? ) calendarItemEntityTypeId ),
                        obj => obj.EventItemOccurrence.EventItem.EventCalendarItems.Where( i => i.EventCalendarId == _calendarId ).Select( i => i.Id ).FirstOrDefault(),
                        av => av.EntityId,
                        ( obj, av ) => new
                        {
                            EventItemOccurrence = obj.EventItemOccurrence,
                            EventCalendarItemAttributes = obj.EventCalendarItemAttributes,
                            EventCalendarItemAttributeValues = av,
                        }
                    )
                    .Where( m =>
                        m.EventItemOccurrence.EventItem.EventCalendarItems.Any( i => i.EventCalendarId == _calendarId ) &&
                        m.EventItemOccurrence.EventItem.IsActive &&
                        m.EventItemOccurrence.EventItem.IsApproved );


            // Get the beginning and end dates
            var today = RockDateTime.Now;
            var filterStart =today;
            var monthStart = new DateTime( filterStart.Year, filterStart.Month, 1 );
            var rangeStart = monthStart.AddMonths( -1 );
            var rangeEnd = monthStart.AddMonths( 2 );
            var beginDate = FilterStartDate.HasValue ? FilterStartDate.Value : rangeStart;
            var endDate = FilterEndDate.HasValue ? FilterEndDate.Value : rangeEnd;

            endDate = endDate.AddDays( 1 ).AddMilliseconds( -1 );

            // Get the occurrences
            var occurrences = qry.ToList();
            foreach ( var occ in occurrences )
            {
                occ.EventItemOccurrence.EventItem.LoadAttributes();
            }

            var occurrencesWithDates = occurrences
                .Select( o => new EventOccurrenceDate
                {
                    EventItemOccurrence = o.EventItemOccurrence,
                    EventCalendarItemAttributes = o.EventCalendarItemAttributes,
                    EventCalendarItemAttributeValues = o.EventCalendarItemAttributeValues,
                    Dates = o.EventItemOccurrence.GetStartTimes( beginDate, endDate ).ToList()
                } )
                .Where( d => d.Dates.Any() )
                .ToList();

            CalendarEventDates = new List<DateTime>();

            var eventOccurrenceSummaries = new List<EventOccurrenceSummary>();
            foreach ( var occurrenceDates in occurrencesWithDates )
            {
                var eventItemOccurrence = occurrenceDates.EventItemOccurrence;
                foreach ( var datetime in occurrenceDates.Dates )
                {
                    if ( eventItemOccurrence.Schedule.EffectiveEndDate.HasValue && ( eventItemOccurrence.Schedule.EffectiveStartDate != eventItemOccurrence.Schedule.EffectiveEndDate ) )
                    {
                        var multiDate = eventItemOccurrence.Schedule.EffectiveStartDate;
                        while ( multiDate.HasValue && ( multiDate.Value < eventItemOccurrence.Schedule.EffectiveEndDate.Value ) )
                        {
                            CalendarEventDates.Add( multiDate.Value.Date );
                            multiDate = multiDate.Value.AddDays( 1 );
                        }
                    }
                    else
                    {
                        CalendarEventDates.Add( datetime.Date );
                    }

                    if ( datetime >= beginDate && datetime < endDate )
                    {
                        eventOccurrenceSummaries.Add( new EventOccurrenceSummary
                        {
                            EventItemOccurrence = eventItemOccurrence,
                            Name = eventItemOccurrence.EventItem.Name,
                            DateTime = datetime,
                            Date = datetime.ToShortDateString(),
                            Time = datetime.ToShortTimeString(),
                            Campus = eventItemOccurrence.Campus != null ? eventItemOccurrence.Campus.Name : "All Campuses",
                            Location = eventItemOccurrence.Campus != null ? eventItemOccurrence.Campus.Name : "All Campuses",
                            LocationDescription = eventItemOccurrence.Location,
                            Description = eventItemOccurrence.EventItem.Description,
                            Summary = eventItemOccurrence.EventItem.Summary,
                            URLSlugs = occurrenceDates.EventCalendarItemAttributeValues.Where( av => av.AttributeKey == "URLSlugs" ).Select( av => av.Value ).FirstOrDefault(),
                            OccurrenceNote = eventItemOccurrence.Note.SanitizeHtml(),
                            DetailPage = String.IsNullOrWhiteSpace( eventItemOccurrence.EventItem.DetailsUrl ) ? null : eventItemOccurrence.EventItem.DetailsUrl,
                            Priority = occurrenceDates.EventCalendarItemAttributeValues.Where( av => av.AttributeKey == "EventPriority" ).Select( av => av.Value ).FirstOrDefault().AsIntegerOrNull() ?? int.MaxValue
                        } );
                    }
                }
            }

            var eventSummaries = eventOccurrenceSummaries
                .OrderBy( e => e.DateTime )
                .GroupBy( e => e.Name )
                .OrderBy( e => e.First().Priority )
                .Select( e => e.ToList() )
                .ToList();

            eventOccurrenceSummaries = eventOccurrenceSummaries
                .OrderBy( e => e.DateTime )
                .ThenBy( e => e.Name )
                .ToList();

            var mergeFields = new Dictionary<string, object>();
            mergeFields.Add( "TimeFrame", ViewMode );
            mergeFields.Add( "DetailsPage", LinkedPageRoute( "DetailsPage" ) );
            mergeFields.Add( "EventItems", eventSummaries );
            mergeFields.Add( "EventItemOccurrences", eventOccurrenceSummaries );
            mergeFields.Add( "CurrentPerson", CurrentPerson );

            content = ( string ) GetAttributeValue( "LavaTemplate" ).ResolveMergeFields( mergeFields, GetAttributeValue( "EnabledLavaCommands" ) );

            var minutes = GetAttributeValue( "CacheDuration" ).AsInteger();
            if ( minutes > 0 )
            {
                var cachePolicy = new CacheItemPolicy();
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes( minutes );
                cache.Set( cacheKey, content, cachePolicy );
            }

            return content;
        }


        #endregion

        #region Helper Classes

        /// <summary>
        /// A class to store event item occurrence data for liquid
        /// </summary>
        [DotLiquid.LiquidType( "EventItemOccurrence", "DateTime", "Name", "Date", "Time", "Campus", "Location", "LocationDescription", "Description", "Summary", "OccurrenceNote", "DetailPage" )]
        public class EventOccurrenceSummary
        {
            public EventItemOccurrence EventItemOccurrence { get; set; }
            public DateTime DateTime { get; set; }
            public String Name { get; set; }
            public String Date { get; set; }
            public String Time { get; set; }
            public String Campus { get; set; }
            public String Location { get; set; }
            public String LocationDescription { get; set; }
            public String Summary { get; set; }
            public String Description { get; set; }
            public String OccurrenceNote { get; set; }
            public String DetailPage { get; set; }
            public int Priority { get; set; }
            public String URLSlugs { get; set; }
        }

        /// <summary>
        /// A class to store the event item occurrences dates
        /// </summary>
        public class EventOccurrenceDate
        {
            public EventItemOccurrence EventItemOccurrence { get; set; }
            public IEnumerable<Rock.Model.Attribute> EventCalendarItemAttributes { get; set; }
            public IEnumerable<AttributeValue> EventCalendarItemAttributeValues { get; set; }
            public List<DateTime> Dates { get; set; }
        }

        #endregion

    }
}
