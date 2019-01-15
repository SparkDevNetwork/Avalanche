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
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avalanche;
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Field.Types;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Content Channel Mobile List" )]
    [Category( "Avalanche" )]
    [Description( "Block to display a list of content itmes." )]

    // Block Properties
    [LinkedPage( "Detail Page", "The page to navigate to for details.", false, "", "", 1 )]

    // Custom Settings
    [ContentChannelField( "Channel", "The channel to display items from.", false, "", "CustomSetting" )]
    [EnumsField( "Status", "Include items with the following status.", typeof( ContentChannelItemStatus ), false, "2", "CustomSetting" )]
    [IntegerField( "Count", "The maximum number of items to display.", false, 10, "CustomSetting" )]
    [IntegerField( "Item Cache Duration", "Number of seconds to cache the content items returned by the selected filter.", false, 3600, "CustomSetting", 0, "CacheDuration" )]
    [IntegerField( "Output Cache Duration", "Number of seconds to cache the resolved output. Only cache the output if you are not personalizing the output based on current user, current page, or any other merge field value.", false, 0, "CustomSetting", 0, "OutputCacheDuration" )]
    [IntegerField( "Filter Id", "The data filter that is used to filter items", false, 0, "CustomSetting" )]
    [BooleanField( "Query Parameter Filtering", "Determines if block should evaluate the query string parameters for additional filter criteria.", false, "CustomSetting" )]
    [TextField( "Order", "The specifics of how items should be ordered. This value is set through configuration and should not be modified here.", false, "", "CustomSetting" )]
    [TextField( "Title Lava", "Lava to display the details of each {{Item}}", true, "{{Item.Title}}", "CustomSetting" )]
    [TextField( "Description Lava", "The attribute key of the descriptionch formatted for mobile.", false, "", "CustomSetting" )]
    [TextField( "Image Lava", "The attribute key of the image to show on the list view will hide icon if not blank.", false, "", "CustomSetting" )]
    [TextField( "Icon Lava", "The attribute key of the icon to show on the list view.", false, "", "CustomSetting" )]
    [TextField( "Order Lava", "Lava to help order the items in the list {{Item}}", true, "", "CustomSetting" )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public partial class ContentChannelMobileList : RockBlockCustomSettings, IMobileResource
    {
        #region Fields

        private readonly string ITEM_TYPE_NAME = "Rock.Model.ContentChannelItem";
        private readonly string CONTENT_CACHE_KEY = "Content";
        private readonly string TEMPLATE_CACHE_KEY = "Template";
        private readonly string OUTPUT_CACHE_KEY = "Output";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the duration of the item cache.
        /// </summary>
        /// <value>
        /// The duration of the item cache.
        /// </value>
        public int? ItemCacheDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the output cache.
        /// </summary>
        /// <value>
        /// The duration of the output cache.
        /// </value>
        public int? OutputCacheDuration { get; set; }

        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        public Guid? ChannelGuid { get; set; }

        /// <summary>
        /// Gets the settings tool tip.
        /// </summary>
        /// <value>
        /// The settings tool tip.
        /// </value>
        public override string SettingsToolTip
        {
            get
            {
                return "Edit Criteria";
            }
        }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            ChannelGuid = ViewState["ChannelGuid"] as Guid?;

            var rockContext = new RockContext();

            var channel = new ContentChannelService( rockContext ).Queryable( "ContentChannelType" )
                .FirstOrDefault( c => c.Guid.Equals( ChannelGuid.Value ) );
            if ( channel != null )
            {
                CreateFilterControl( channel, DataViewFilter.FromJson( ViewState["DataViewFilter"].ToString() ), false, rockContext );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            ItemCacheDuration = GetAttributeValue( "CacheDuration" ).AsIntegerOrNull();
            OutputCacheDuration = GetAttributeValue( "OutputCacheDuration" ).AsIntegerOrNull();

            this.BlockUpdated += ContentDynamic_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            Button btnTrigger = new Button();
            btnTrigger.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            btnTrigger.ID = "rock-config-cancel-trigger";
            btnTrigger.Click += btnTrigger_Click;
            pnlEditModal.Controls.Add( btnTrigger );

            AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
            trigger.ControlID = "rock-config-cancel-trigger";
            trigger.EventName = "Click";
            upnlContent.Triggers.Add( trigger );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState["ChannelGuid"] = ChannelGuid;
            ViewState["DataViewFilter"] = GetFilterControl().ToJson();

            return base.SaveViewState();
        }

        #endregion

        #region Events

        void ContentDynamic_BlockUpdated( object sender, EventArgs e )
        {
            FlushCacheItem( CONTENT_CACHE_KEY );
            FlushCacheItem( TEMPLATE_CACHE_KEY );
            FlushCacheItem( OUTPUT_CACHE_KEY );
        }

        void btnTrigger_Click( object sender, EventArgs e )
        {
            mdEdit.Hide();
            pnlEditModal.Visible = false;
        }

        protected void ddlChannel_SelectedIndexChanged( object sender, EventArgs e )
        {
            ChannelGuid = ddlChannel.SelectedValue.AsGuidOrNull();
            ShowEdit();
        }

        protected void lbSave_Click( object sender, EventArgs e )
        {

            var dataViewFilter = GetFilterControl();

            // update Guids since we are creating a new dataFilter and children and deleting the old one
            SetNewDataFilterGuids( dataViewFilter );

            if ( !Page.IsValid )
            {
                return;
            }

            if ( !dataViewFilter.IsValid )
            {
                // Controls will render the error messages                    
                return;
            }

            var rockContext = new RockContext();
            DataViewFilterService dataViewFilterService = new DataViewFilterService( rockContext );

            int? dataViewFilterId = hfDataFilterId.Value.AsIntegerOrNull();
            if ( dataViewFilterId.HasValue )
            {
                var oldDataViewFilter = dataViewFilterService.Get( dataViewFilterId.Value );
                DeleteDataViewFilter( oldDataViewFilter, dataViewFilterService );
            }

            dataViewFilterService.Add( dataViewFilter );

            rockContext.SaveChanges();

            SetAttributeValue( "Status", cblStatus.SelectedValuesAsInt.AsDelimited( "," ) );
            SetAttributeValue( "Channel", ddlChannel.SelectedValue );
            SetAttributeValue( "Count", ( nbCount.Text.AsIntegerOrNull() ?? 5 ).ToString() );
            SetAttributeValue( "CacheDuration", ( nbItemCacheDuration.Text.AsIntegerOrNull() ?? 0 ).ToString() );
            SetAttributeValue( "OutputCacheDuration", ( nbOutputCacheDuration.Text.AsIntegerOrNull() ?? 0 ).ToString() );
            SetAttributeValue( "FilterId", dataViewFilter.Id.ToString() );
            SetAttributeValue( "Order", kvlOrder.Value );
            SetAttributeValue( "TitleLava", tbTitleLava.Text );
            SetAttributeValue( "IconLava", tbIconLava.Text );
            SetAttributeValue( "ImageLava", tbImageLava.Text );
            SetAttributeValue( "SubtitleLava", tbSubtitleLava.Text );
            SetAttributeValue( "OrderLava", tbOrder.Text );

            var ppFieldType = new PageReferenceFieldType();
            SetAttributeValue( "DetailPage", ppFieldType.GetEditValue( ppDetailPage, null ) );

            SaveAttributeValues();

            FlushCacheItem( CONTENT_CACHE_KEY );
            FlushCacheItem( TEMPLATE_CACHE_KEY );
            FlushCacheItem( OUTPUT_CACHE_KEY );

            mdEdit.Hide();
            pnlEditModal.Visible = false;
            upnlContent.Update();
        }

        /// <summary>
        /// Handles the AddFilterClick event of the groupControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void groupControl_AddFilterClick( object sender, EventArgs e )
        {
            FilterGroup groupControl = sender as FilterGroup;
            FilterField filterField = new FilterField();
            filterField.DataViewFilterGuid = Guid.NewGuid();
            groupControl.Controls.Add( filterField );
            filterField.ID = string.Format( "ff_{0}", filterField.DataViewFilterGuid.ToString( "N" ) );

            // Remove the 'Other Data View' Filter as it doesn't really make sense to have it available in this scenario
            filterField.ExcludedFilterTypes = new string[] { typeof( Rock.Reporting.DataFilter.OtherDataViewFilter ).FullName };
            filterField.FilteredEntityTypeName = groupControl.FilteredEntityTypeName;
            filterField.Expanded = true;
        }

        /// <summary>
        /// Handles the AddGroupClick event of the groupControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void groupControl_AddGroupClick( object sender, EventArgs e )
        {
            FilterGroup groupControl = sender as FilterGroup;
            FilterGroup childGroupControl = new FilterGroup();
            childGroupControl.DataViewFilterGuid = Guid.NewGuid();
            groupControl.Controls.Add( childGroupControl );
            childGroupControl.ID = string.Format( "fg_{0}", childGroupControl.DataViewFilterGuid.ToString( "N" ) );
            childGroupControl.FilteredEntityTypeName = groupControl.FilteredEntityTypeName;
            childGroupControl.FilterType = FilterExpressionType.GroupAll;
        }

        /// <summary>
        /// Handles the DeleteClick event of the filterControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void filterControl_DeleteClick( object sender, EventArgs e )
        {
            FilterField fieldControl = sender as FilterField;
            fieldControl.Parent.Controls.Remove( fieldControl );
        }

        /// <summary>
        /// Handles the DeleteGroupClick event of the groupControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void groupControl_DeleteGroupClick( object sender, EventArgs e )
        {
            FilterGroup groupControl = sender as FilterGroup;
            groupControl.Parent.Controls.Remove( groupControl );
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Shows the settings.
        /// </summary>
        protected override void ShowSettings()
        {
            // Switch does not automatically initialize again after a partial-postback.  This script 
            // looks for any switch elements that have not been initialized and re-intializes them.
            string script = @"
$(document).ready(function() {
    $('.switch > input').each( function () {
        $(this).parent().switch('init');
    });
});
";
            ScriptManager.RegisterStartupScript( this.Page, this.Page.GetType(), "toggle-switch-init", script, true );

            pnlEditModal.Visible = true;
            upnlContent.Update();
            mdEdit.Show();

            var rockContext = new RockContext();
            ddlChannel.DataSource = new ContentChannelService( rockContext ).Queryable()
                .OrderBy( c => c.Name )
                .Select( c => new { c.Guid, c.Name } )
                .ToList();
            ddlChannel.DataBind();
            ddlChannel.Items.Insert( 0, new ListItem( "", "" ) );
            ddlChannel.SetValue( GetAttributeValue( "Channel" ) );
            ChannelGuid = ddlChannel.SelectedValue.AsGuidOrNull();

            cblStatus.BindToEnum<ContentChannelItemStatus>();
            foreach ( string status in GetAttributeValue( "Status" ).SplitDelimitedValues() )
            {
                var li = cblStatus.Items.FindByValue( status );
                if ( li != null )
                {
                    li.Selected = true;
                }
            }

            nbCount.Text = GetAttributeValue( "Count" );
            nbItemCacheDuration.Text = GetAttributeValue( "CacheDuration" );
            nbOutputCacheDuration.Text = GetAttributeValue( "OutputCacheDuration" );
            hfDataFilterId.Value = GetAttributeValue( "FilterId" );

            tbTitleLava.Text = GetAttributeValue( "TitleLava" );
            tbIconLava.Text = GetAttributeValue( "IconLava" );
            tbImageLava.Text = GetAttributeValue( "ImageLava" );
            tbSubtitleLava.Text = GetAttributeValue( "SubtitleLava" );
            tbOrder.Text = GetAttributeValue( "OrderLava" );

            var ppFieldType = new PageReferenceFieldType();
            ppFieldType.SetEditValue( ppDetailPage, null, GetAttributeValue( "DetailPage" ) );

            var directions = new Dictionary<string, string>();
            directions.Add( "", "" );
            directions.Add( SortDirection.Ascending.ConvertToInt().ToString(), "Ascending" );
            directions.Add( SortDirection.Descending.ConvertToInt().ToString(), "Descending" );
            kvlOrder.CustomValues = directions;
            kvlOrder.Value = GetAttributeValue( "Order" );
            kvlOrder.Required = true;

            ShowEdit();

            upnlContent.Update();
        }

        private List<ListElement> ShowView( int page )
        {
            var errorMessages = new List<string>();
            List<ContentChannelItem> content;
            try
            {
                content = GetContent( errorMessages ) ?? new List<ContentChannelItem>();
            }
            catch ( Exception ex )
            {
                this.LogException( ex );
                Exception exception = ex;
                while ( exception != null )
                {
                    errorMessages.Add( exception.Message );
                    exception = exception.InnerException;
                }

                content = new List<ContentChannelItem>();
            }
            var count = GetAttributeValue( "Count" ).AsInteger();

            content = content.Skip( page * count ).Take( count ).ToList();

            var titleLava = GetAttributeValue( "TitleLava" );
            var descriptionLava = GetAttributeValue( "DescriptionLava" );
            var imageLava = GetAttributeValue( "ImageLava" );
            var iconLava = GetAttributeValue( "IconLava" );
            var orderLava = GetAttributeValue( "OrderLava" );


            List<ListElement> listViews = new List<ListElement>();
            foreach ( var item in content )
            {
                var mlv = new ListElement()
                {
                    Id = item.Id.ToString(),
                    Description = "",
                    Image = "",
                    Icon = ""
                };

                mlv.Title = ProcessLava( titleLava, item );

                if ( !string.IsNullOrWhiteSpace( descriptionLava ) )
                {
                    mlv.Description = ProcessLava( descriptionLava, item );
                }
                if ( !string.IsNullOrWhiteSpace( imageLava ) )
                {
                    mlv.Image = ProcessLava( imageLava, item );
                }
                if ( !string.IsNullOrWhiteSpace( iconLava ) )
                {
                    mlv.Icon = ProcessLava( iconLava, item );
                }
                if ( !string.IsNullOrWhiteSpace( orderLava ) )
                {
                    mlv.Order = ProcessLava( orderLava, item );
                }
                listViews.Add( mlv );
            }

            return listViews;
        }

        private List<ContentChannelItem> GetContent( List<string> errorMessages )
        {
            List<ContentChannelItem> items = null;

            // only load from the cache if a cacheDuration was specified
            if ( ItemCacheDuration.HasValue && ItemCacheDuration.Value > 0 )
            {
                items = GetCacheItem( CONTENT_CACHE_KEY ) as List<ContentChannelItem>;
            }


            if ( items == null )
            {
                Guid? channelGuid = GetAttributeValue( "Channel" ).AsGuidOrNull();
                if ( channelGuid.HasValue )
                {
                    var rockContext = new RockContext();
                    var service = new ContentChannelItemService( rockContext );
                    var itemType = typeof( Rock.Model.ContentChannelItem );

                    ParameterExpression paramExpression = service.ParameterExpression;

                    var contentChannel = new ContentChannelService( rockContext ).Get( channelGuid.Value );
                    if ( contentChannel != null )
                    {
                        var entityFields = HackEntityFields( contentChannel, rockContext );

                        items = new List<ContentChannelItem>();

                        var qry = service
                            .Queryable( "ContentChannel,ContentChannelType" )
                            .Where( i => i.ContentChannelId == contentChannel.Id );

                        if ( contentChannel.RequiresApproval && !contentChannel.ContentChannelType.DisableStatus )
                        {
                            // Check for the configured status and limit query to those
                            var statuses = new List<ContentChannelItemStatus>();
                            foreach ( string statusVal in ( GetAttributeValue( "Status" ) ?? "2" ).Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
                            {
                                var status = statusVal.ConvertToEnumOrNull<ContentChannelItemStatus>();
                                if ( status != null )
                                {
                                    statuses.Add( status.Value );
                                }
                            }
                            if ( statuses.Any() )
                            {
                                qry = qry.Where( i => statuses.Contains( i.Status ) );
                            }
                        }

                        int? dataFilterId = GetAttributeValue( "FilterId" ).AsIntegerOrNull();
                        if ( dataFilterId.HasValue )
                        {
                            var dataFilterService = new DataViewFilterService( rockContext );
                            var dataFilter = dataFilterService.Queryable( "ChildFilters" ).FirstOrDefault( a => a.Id == dataFilterId.Value );
                            Expression whereExpression = dataFilter != null ? dataFilter.GetExpression( itemType, service, paramExpression, errorMessages ) : null;

                            qry = qry.Where( paramExpression, whereExpression, null );
                        }

                        // All filtering has been added, now run query, check security and load attributes
                        foreach ( var item in qry.ToList() )
                        {
                            if ( item.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
                            {
                                item.LoadAttributes( rockContext );
                                items.Add( item );
                            }
                        }

                        // Order the items
                        SortProperty sortProperty = null;

                        string orderBy = GetAttributeValue( "Order" );
                        if ( !string.IsNullOrWhiteSpace( orderBy ) )
                        {
                            var fieldDirection = new List<string>();
                            foreach ( var itemPair in orderBy.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ).Select( a => a.Split( '^' ) ) )
                            {
                                if ( itemPair.Length == 2 && !string.IsNullOrWhiteSpace( itemPair[0] ) )
                                {
                                    var sortDirection = SortDirection.Ascending;
                                    if ( !string.IsNullOrWhiteSpace( itemPair[1] ) )
                                    {
                                        sortDirection = itemPair[1].ConvertToEnum<SortDirection>( SortDirection.Ascending );
                                    }
                                    fieldDirection.Add( itemPair[0] + ( sortDirection == SortDirection.Descending ? " desc" : "" ) );
                                }
                            }

                            sortProperty = new SortProperty();
                            sortProperty.Direction = SortDirection.Ascending;
                            sortProperty.Property = fieldDirection.AsDelimited( "," );

                            string[] columns = sortProperty.Property.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

                            var itemQry = items.AsQueryable();
                            IOrderedQueryable<ContentChannelItem> orderedQry = null;

                            for ( int columnIndex = 0; columnIndex < columns.Length; columnIndex++ )
                            {
                                string column = columns[columnIndex].Trim();

                                var direction = sortProperty.Direction;
                                if ( column.ToLower().EndsWith( " desc" ) )
                                {
                                    column = column.Left( column.Length - 5 );
                                    direction = sortProperty.Direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                                }

                                try
                                {
                                    if ( column.StartsWith( "Attribute:" ) )
                                    {
                                        string attributeKey = column.Substring( 10 );

                                        if ( direction == SortDirection.Ascending )
                                        {
                                            orderedQry = ( columnIndex == 0 ) ?
                                                itemQry.OrderBy( i => i.AttributeValues.Where( v => v.Key == attributeKey ).FirstOrDefault().Value.SortValue ) :
                                                orderedQry.ThenBy( i => i.AttributeValues.Where( v => v.Key == attributeKey ).FirstOrDefault().Value.SortValue );
                                        }
                                        else
                                        {
                                            orderedQry = ( columnIndex == 0 ) ?
                                                itemQry.OrderByDescending( i => i.AttributeValues.Where( v => v.Key == attributeKey ).FirstOrDefault().Value.SortValue ) :
                                                orderedQry.ThenByDescending( i => i.AttributeValues.Where( v => v.Key == attributeKey ).FirstOrDefault().Value.SortValue );
                                        }
                                    }
                                    else
                                    {
                                        if ( direction == SortDirection.Ascending )
                                        {
                                            orderedQry = ( columnIndex == 0 ) ? itemQry.OrderBy( column ) : orderedQry.ThenBy( column );
                                        }
                                        else
                                        {
                                            orderedQry = ( columnIndex == 0 ) ? itemQry.OrderByDescending( column ) : orderedQry.ThenByDescending( column );
                                        }
                                    }
                                }
                                catch { }

                            }

                            try
                            {
                                if ( orderedQry != null )
                                {
                                    items = orderedQry.ToList();
                                }
                            }
                            catch { }

                        }

                        if ( ItemCacheDuration.HasValue && ItemCacheDuration.Value > 0 )
                        {
                            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds( ItemCacheDuration.Value ) };
                            AddCacheItem( CONTENT_CACHE_KEY, items, cacheItemPolicy );
                        }
                    }
                }
            }

            return items;

        }

        /// <summary>
        /// Shows the edit.
        /// </summary>
        public void ShowEdit()
        {
            int? filterId = hfDataFilterId.Value.AsIntegerOrNull();

            if ( ChannelGuid.HasValue )
            {
                var rockContext = new RockContext();
                var channel = new ContentChannelService( rockContext ).Queryable( "ContentChannelType" )
                    .FirstOrDefault( c => c.Guid.Equals( ChannelGuid.Value ) );
                if ( channel != null )
                {

                    cblStatus.Visible = channel.RequiresApproval && !channel.ContentChannelType.DisableStatus;

                    var filterService = new DataViewFilterService( rockContext );
                    DataViewFilter filter = null;

                    if ( filterId.HasValue )
                    {
                        filter = filterService.Get( filterId.Value );
                    }

                    if ( filter == null || filter.ExpressionType == FilterExpressionType.Filter )
                    {
                        filter = new DataViewFilter();
                        filter.Guid = new Guid();
                        filter.ExpressionType = FilterExpressionType.GroupAll;
                    }

                    CreateFilterControl( channel, filter, true, rockContext );

                    kvlOrder.CustomKeys = new Dictionary<string, string>();
                    kvlOrder.CustomKeys.Add( "", "" );
                    kvlOrder.CustomKeys.Add( "Title", "Title" );
                    kvlOrder.CustomKeys.Add( "Priority", "Priority" );
                    kvlOrder.CustomKeys.Add( "Status", "Status" );
                    kvlOrder.CustomKeys.Add( "StartDateTime", "Start" );
                    kvlOrder.CustomKeys.Add( "ExpireDateTime", "Expire" );
                    kvlOrder.CustomKeys.Add( "Order", "Order" );

                    string currentMetaDescriptionAttribute = GetAttributeValue( "MetaDescriptionAttribute" ) ?? string.Empty;
                    string currentMetaImageAttribute = GetAttributeValue( "MetaImageAttribute" ) ?? string.Empty;

                    // add channel attributes
                    channel.LoadAttributes();
                    foreach ( var attribute in channel.Attributes )
                    {
                        var field = attribute.Value.FieldType.Field;
                        string computedKey = "C^" + attribute.Key;
                    }

                    // add item attributes
                    AttributeService attributeService = new AttributeService( rockContext );
                    var itemAttributes = attributeService.GetByEntityTypeId( new ContentChannelItem().TypeId ).AsQueryable()
                                            .Where( a => (
                                                    a.EntityTypeQualifierColumn.Equals( "ContentChannelTypeId", StringComparison.OrdinalIgnoreCase ) &&
                                                    a.EntityTypeQualifierValue.Equals( channel.ContentChannelTypeId.ToString() )
                                                ) || (
                                                    a.EntityTypeQualifierColumn.Equals( "ContentChannelId", StringComparison.OrdinalIgnoreCase ) &&
                                                    a.EntityTypeQualifierValue.Equals( channel.Id.ToString() )
                                                ) )
                                            .OrderByDescending( a => a.EntityTypeQualifierColumn )
                                            .ThenBy( a => a.Order )
                                            .ToList();

                    foreach ( var attribute in itemAttributes )
                    {
                        string attrKey = "Attribute:" + attribute.Key;
                        if ( !kvlOrder.CustomKeys.ContainsKey( attrKey ) )
                        {
                            kvlOrder.CustomKeys.Add( "Attribute:" + attribute.Key, attribute.Name );

                            string computedKey = "I^" + attribute.Key;

                            var field = attribute.FieldType.Name;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the list value.
        /// </summary>
        /// <param name="listControl">The list control.</param>
        /// <param name="value">The value.</param>
        private void SetListValue( ListControl listControl, string value )
        {
            foreach ( ListItem item in listControl.Items )
            {
                item.Selected = ( item.Value == value );
            }
        }

        /// <summary>
        /// **The PropertyFilter checks for it's property/attribute list in a cached items object before recreating 
        /// them using reflection and loading of generic attributes. Because of this, we're going to load them here
        /// and exclude some properties and add additional attributes specific to the channel type, and then save
        /// list to same cached object so that property filter lists our collection of properties/attributes
        /// instead.
        /// </summary>
        private List<Rock.Reporting.EntityField> HackEntityFields( ContentChannel channel, RockContext rockContext )
        {
            if ( channel != null )
            {
                var entityTypeCache = EntityTypeCache.Read( ITEM_TYPE_NAME );
                if ( entityTypeCache != null )
                {
                    var entityType = entityTypeCache.GetEntityType();

                    /// See above comments on HackEntityFields** to see why we are doing this
                    HttpContext.Current.Items.Remove( Rock.Reporting.EntityHelper.GetCacheKey( entityType ) );

                    var entityFields = Rock.Reporting.EntityHelper.GetEntityFields( entityType );
                    foreach ( var entityField in entityFields
                        .Where( f =>
                            f.FieldKind == Rock.Reporting.FieldKind.Attribute &&
                            f.AttributeGuid.HasValue )
                        .ToList() )
                    {
                        // remove EntityFields that aren't attributes for this ContentChannelType or ChannelChannel (to avoid duplicate Attribute Keys)
                        var attribute = AttributeCache.Get( entityField.AttributeGuid.Value );
                        if ( attribute != null &&
                            attribute.EntityTypeQualifierColumn == "ContentChannelTypeId" &&
                            attribute.EntityTypeQualifierValue.AsInteger() != channel.ContentChannelTypeId )
                        {
                            entityFields.Remove( entityField );
                        }

                        if ( attribute != null &&
                            attribute.EntityTypeQualifierColumn == "ContentChannelId" &&
                            attribute.EntityTypeQualifierValue.AsInteger() != channel.Id )
                        {
                            entityFields.Remove( entityField );
                        }
                    }

                    if ( entityFields != null )
                    {
                        // Remove the status field
                        var ignoreFields = new List<string>();
                        ignoreFields.Add( "ContentChannelId" );
                        ignoreFields.Add( "Status" );

                        entityFields = entityFields.Where( f => !ignoreFields.Contains( f.Name ) ).ToList();

                        // Add any additional attributes that are specific to channel/type
                        var item = new ContentChannelItem();
                        item.ContentChannel = channel;
                        item.ContentChannelId = channel.Id;
                        item.ContentChannelType = channel.ContentChannelType;
                        item.ContentChannelTypeId = channel.ContentChannelTypeId;
                        item.LoadAttributes( rockContext );
                        foreach ( var attribute in item.Attributes
                            .Where( a =>
                                a.Value.EntityTypeQualifierColumn != "" &&
                                a.Value.EntityTypeQualifierValue != "" )
                            .Select( a => a.Value ) )
                        {
                            if ( !entityFields.Any( f => f.AttributeGuid.Equals( attribute.Guid ) ) )
                            {
                                Rock.Reporting.EntityHelper.AddEntityFieldForAttribute( entityFields, attribute );
                            }
                        }

                        // Re-sort fields
                        int index = 0;
                        var sortedFields = new List<Rock.Reporting.EntityField>();
                        foreach ( var entityProperty in entityFields.OrderBy( p => p.Title ).ThenBy( p => p.Name ) )
                        {
                            entityProperty.Index = index;
                            index++;
                            sortedFields.Add( entityProperty );
                        }

                        // Save new fields to cache ( which report field will use instead of reading them again )
                        HttpContext.Current.Items[Rock.Reporting.EntityHelper.GetCacheKey( entityType )] = sortedFields;
                    }

                    return entityFields;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the filter control.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="setSelection">if set to <c>true</c> [set selection].</param>
        /// <param name="rockContext">The rock context.</param>
        private void CreateFilterControl( ContentChannel channel, DataViewFilter filter, bool setSelection, RockContext rockContext )
        {
            HackEntityFields( channel, rockContext );

            phFilters.Controls.Clear();
            if ( filter != null )
            {
                CreateFilterControl( phFilters, filter, setSelection, rockContext );
            }
        }

        /// <summary>
        /// Creates the filter control.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="setSelection">if set to <c>true</c> [set selection].</param>
        /// <param name="rockContext">The rock context.</param>
        private void CreateFilterControl( Control parentControl, DataViewFilter filter, bool setSelection, RockContext rockContext )
        {
            try
            {
                if ( filter.ExpressionType == FilterExpressionType.Filter )
                {
                    var filterControl = new FilterField();

                    parentControl.Controls.Add( filterControl );
                    filterControl.DataViewFilterGuid = filter.Guid;
                    filterControl.ID = string.Format( "ff_{0}", filterControl.DataViewFilterGuid.ToString( "N" ) );

                    // Remove the 'Other Data View' Filter as it doesn't really make sense to have it available in this scenario
                    filterControl.ExcludedFilterTypes = new string[] { typeof( Rock.Reporting.DataFilter.OtherDataViewFilter ).FullName };
                    filterControl.FilteredEntityTypeName = ITEM_TYPE_NAME;

                    if ( filter.EntityTypeId.HasValue )
                    {
                        var entityTypeCache = Rock.Web.Cache.EntityTypeCache.Read( filter.EntityTypeId.Value, rockContext );
                        if ( entityTypeCache != null )
                        {
                            filterControl.FilterEntityTypeName = entityTypeCache.Name;
                        }
                    }

                    filterControl.Expanded = filter.Expanded;
                    if ( setSelection )
                    {
                        try
                        {
                            filterControl.SetSelection( filter.Selection );
                        }
                        catch ( Exception ex )
                        {
                            this.LogException( new Exception( "Exception setting selection for DataViewFilter: " + filter.Guid, ex ) );
                        }
                    }

                    filterControl.DeleteClick += filterControl_DeleteClick;
                }
                else
                {
                    var groupControl = new FilterGroup();
                    parentControl.Controls.Add( groupControl );
                    groupControl.DataViewFilterGuid = filter.Guid;
                    groupControl.ID = string.Format( "fg_{0}", groupControl.DataViewFilterGuid.ToString( "N" ) );
                    groupControl.FilteredEntityTypeName = ITEM_TYPE_NAME;
                    groupControl.IsDeleteEnabled = parentControl is FilterGroup;
                    if ( setSelection )
                    {
                        groupControl.FilterType = filter.ExpressionType;
                    }

                    groupControl.AddFilterClick += groupControl_AddFilterClick;
                    groupControl.AddGroupClick += groupControl_AddGroupClick;
                    groupControl.DeleteGroupClick += groupControl_DeleteGroupClick;
                    foreach ( var childFilter in filter.ChildFilters )
                    {
                        CreateFilterControl( groupControl, childFilter, setSelection, rockContext );
                    }
                }
            }
            catch ( Exception ex )
            {
                this.LogException( new Exception( "Exception creating FilterControl for DataViewFilter: " + filter.Guid, ex ) );
            }
        }

        private DataViewFilter GetFilterControl()
        {
            if ( phFilters.Controls.Count > 0 )
            {
                return GetFilterControl( phFilters.Controls[0] );
            }

            return null;
        }

        private DataViewFilter GetFilterControl( Control control )
        {
            FilterGroup groupControl = control as FilterGroup;
            if ( groupControl != null )
            {
                return GetFilterGroupControl( groupControl );
            }

            FilterField filterControl = control as FilterField;
            if ( filterControl != null )
            {
                return GetFilterFieldControl( filterControl );
            }

            return null;
        }

        private DataViewFilter GetFilterGroupControl( FilterGroup filterGroup )
        {
            DataViewFilter filter = new DataViewFilter();
            filter.Guid = filterGroup.DataViewFilterGuid;
            filter.ExpressionType = filterGroup.FilterType;
            foreach ( Control control in filterGroup.Controls )
            {
                DataViewFilter childFilter = GetFilterControl( control );
                if ( childFilter != null )
                {
                    filter.ChildFilters.Add( childFilter );
                }
            }

            return filter;
        }

        private DataViewFilter GetFilterFieldControl( FilterField filterField )
        {
            DataViewFilter filter = new DataViewFilter();
            filter.Guid = filterField.DataViewFilterGuid;
            filter.ExpressionType = FilterExpressionType.Filter;
            filter.Expanded = filterField.Expanded;
            if ( filterField.FilterEntityTypeName != null )
            {
                filter.EntityTypeId = Rock.Web.Cache.EntityTypeCache.Read( filterField.FilterEntityTypeName ).Id;
                filter.Selection = filterField.GetSelection();
            }

            return filter;
        }

        private void SetNewDataFilterGuids( DataViewFilter dataViewFilter )
        {
            if ( dataViewFilter != null )
            {
                dataViewFilter.Guid = Guid.NewGuid();
                foreach ( var childFilter in dataViewFilter.ChildFilters )
                {
                    SetNewDataFilterGuids( childFilter );
                }
            }
        }

        private void DeleteDataViewFilter( DataViewFilter dataViewFilter, DataViewFilterService service )
        {
            if ( dataViewFilter != null )
            {
                foreach ( var childFilter in dataViewFilter.ChildFilters.ToList() )
                {
                    DeleteDataViewFilter( childFilter, service );
                }

                service.Delete( dataViewFilter );
            }
        }

        private Dictionary<string, string> _customAtributes;
        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                if ( _customAtributes == null )
                {
                    _customAtributes = new Dictionary<string, string>();
                    var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
                    foreach ( var item in customs )
                    {
                        _customAtributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
                    }
                }
                return _customAtributes;
            }
        }
        public MobileBlock GetMobile( string parameter )
        {
            var pageGuid = GetAttributeValue( "DetailPage" );
            CustomAttributes["Resource"] = PageCache.Read( pageGuid.AsGuid() ).Id.ToString();
            CustomAttributes["ActionType"] = "1";
            CustomAttributes["InitialRequest"] = "0";
            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Read( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            return new MobileBlock()
            {
                Attributes = CustomAttributes,
                BlockType = "Avalanche.Blocks.ListViewBlock"
            };
        }

        public MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var page = request.AsInteger();

            var listElements = ShowView( page );

            var response = new ListViewResponse
            {
                Content = listElements,
                NextRequest = ( page + 1 ).ToString()
            };

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = GetAttributeValue( "OutputCacheDuration" ).AsInteger()
            };
        }

        private string ProcessLava( string lava, ContentChannelItem item )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, CurrentPerson );
            mergeObjects["Item"] = item;
            return lava.ResolveMergeFields( mergeObjects );
        }

    }
    #endregion

}