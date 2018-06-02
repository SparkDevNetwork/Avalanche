using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Security;
using Rock.Workflow;
using Avalanche;
using Avalanche.Models;
using Avalanche.Field;
using Newtonsoft.Json;
using Rock.Field.Types;

namespace RockWeb.Plugins.Avalanche
{
    /// <summary>
    /// Used to enter information for a workflow form entry action.
    /// </summary>
    [DisplayName( "Mobile Workflow" )]
    [Category( "Avalanche" )]
    [Description( "Mobile block to allow workflow to be accessed from app" )]

    [WorkflowTypeField( "Workflow Type", "Type of workflow to start." )]
    public partial class MobileWorkflow : AvalancheBlock
    {
        #region Fields

        private RockContext _rockContext = null;
        private WorkflowService _workflowService = null;

        private WorkflowTypeCache _workflowType = null;
        private WorkflowActionTypeCache _actionType = null;
        private Workflow _workflow = null;
        private WorkflowActivity _activity = null;
        private WorkflowAction _action = null;
        private int? WorkflowTypeId = null;
        private int? WorkflowId = null;
        private int? ActionTypeId = null;
        private string parameter;

        #endregion


        #region Base Control Methods


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                if ( HydrateObjects() )
                {
                    BuildForm( true );
                    ProcessActionRequest();
                }
            }
        }

        public override MobileBlock GetMobile( string parameter )
        {
            if ( HydrateObjects() )
            {
                List<FormElementItem> elements = BuildForm( true );
                CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( elements ) );
                AvalancheUtilities.SetActionItems( GetAttributeValue( "SuccessAction" ), CustomAttributes, CurrentPerson );

                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.FormBlock",
                    Attributes = CustomAttributes
                };
            }
            return new MobileBlock();
        }

        #endregion


        #region Methods

        private bool HydrateObjects()
        {
            LoadWorkflowType();

            if ( _workflowType == null )
            {
                nbError.NotificationBoxType = NotificationBoxType.Danger;
                nbError.Text = "Configuration Error, Workflow type was not configured or specified correctly.";
                return false;
            }

            if ( !_workflowType.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
            {
                return false;
            }

            if ( !( _workflowType.IsActive ?? true ) )
            {
                nbError.NotificationBoxType = NotificationBoxType.Danger;
                nbError.Text = "This type of workflow is not active.";

                return false;
            }

            // If operating against an existing workflow, get the workflow and load attributes
            if ( !WorkflowId.HasValue )
            {
                WorkflowId = parameter.AsIntegerOrNull();
                if ( !WorkflowId.HasValue )
                {
                    Guid guid = parameter.AsGuid();
                    if ( !guid.IsEmpty() )
                    {
                        _workflow = _workflowService.Queryable()
                            .Where( w => w.Guid.Equals( guid ) && w.WorkflowTypeId == _workflowType.Id )
                            .FirstOrDefault();
                        if ( _workflow != null )
                        {
                            WorkflowId = _workflow.Id;
                        }
                    }
                }
            }

            if ( WorkflowId.HasValue )
            {
                if ( _workflow == null )
                {
                    _workflow = _workflowService.Queryable()
                        .Where( w => w.Id == WorkflowId.Value && w.WorkflowTypeId == _workflowType.Id )
                        .FirstOrDefault();
                }
                if ( _workflow != null )
                {
                    _workflow.LoadAttributes();
                    foreach ( var activity in _workflow.Activities )
                    {
                        activity.LoadAttributes();
                    }
                }

            }

            // If an existing workflow was not specified, activate a new instance of workflow and start processing
            if ( _workflow == null )
            {

                var workflowName = "New " + _workflowType.WorkTerm;

                _workflow = Rock.Model.Workflow.Activate( _workflowType, workflowName );
                if ( _workflow != null )
                {
                    // If a PersonId or GroupId parameter was included, load the corresponding
                    // object and pass that to the actions for processing
                    object entity = null;

                    List<string> errorMessages;
                    if ( !_workflowService.Process( _workflow, entity, out errorMessages ) )
                    {
                        //ShowMessage( NotificationBoxType.Danger, "Workflow Processing Error(s):",
                        //    "<ul><li>" + errorMessages.AsDelimited( "</li><li>" ) + "</li></ul>" );
                        return false;
                    }
                    if ( _workflow.Id != 0 )
                    {
                        WorkflowId = _workflow.Id;
                    }
                }
            }

            if ( _workflow == null )
            {
                //ShowNotes( false );
                //ShowMessage( NotificationBoxType.Danger, "Workflow Activation Error", "Workflow could not be activated." );
                return false;
            }

            var canEdit = UserCanEdit || _workflow.IsAuthorized( Authorization.EDIT, CurrentPerson );

            if ( _workflow.IsActive )
            {
                if ( ActionTypeId.HasValue )
                {
                    foreach ( var activity in _workflow.ActiveActivities )
                    {
                        _action = activity.ActiveActions.Where( a => a.ActionTypeId == ActionTypeId.Value ).FirstOrDefault();
                        if ( _action != null )
                        {
                            _activity = activity;
                            _activity.LoadAttributes();

                            _actionType = _action.ActionTypeCache;
                            ActionTypeId = _actionType.Id;
                            return true;
                        }
                    }
                }

                // Find first active action form
                int personId = CurrentPerson != null ? CurrentPerson.Id : 0;
                int? actionId = null;
                foreach ( var activity in _workflow.Activities
                    .Where( a =>
                        a.IsActive &&
                        ( !actionId.HasValue || a.Actions.Any( ac => ac.Id == actionId.Value ) ) &&
                        (
                            ( canEdit ) ||
                            ( !a.AssignedGroupId.HasValue && !a.AssignedPersonAliasId.HasValue ) ||
                            ( a.AssignedPersonAlias != null && a.AssignedPersonAlias.PersonId == personId ) ||
                            ( a.AssignedGroup != null && a.AssignedGroup.Members.Any( m => m.PersonId == personId ) )
                        )
                    )
                    .ToList()
                    .OrderBy( a => a.ActivityTypeCache.Order ) )
                {
                    if ( canEdit || ( activity.ActivityTypeCache.IsAuthorized( Authorization.VIEW, CurrentPerson ) ) )
                    {
                        foreach ( var action in activity.ActiveActions
                            .Where( a => ( !actionId.HasValue || a.Id == actionId.Value ) ) )
                        {
                            if ( action.ActionTypeCache.WorkflowForm != null && action.IsCriteriaValid )
                            {
                                _activity = activity;
                                _activity.LoadAttributes();

                                _action = action;
                                _actionType = _action.ActionTypeCache;
                                ActionTypeId = _actionType.Id;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }



        private void LoadWorkflowType()
        {
            if ( _rockContext == null )
            {
                _rockContext = new RockContext();
            }

            if ( _workflowService == null )
            {
                _workflowService = new WorkflowService( _rockContext );
            }

            // Get the workflow type id (initial page request)
            if ( !WorkflowTypeId.HasValue )
            {
                // Get workflow type set by attribute value
                Guid workflowTypeguid = GetAttributeValue( "WorkflowType" ).AsGuid();
                if ( !workflowTypeguid.IsEmpty() )
                {
                    _workflowType = WorkflowTypeCache.Read( workflowTypeguid );
                }

                // If an attribute value was not provided, check for query/route value
                if ( _workflowType != null )
                {
                    WorkflowTypeId = _workflowType.Id;
                }
            }

            // Get the workflow type 
            if ( _workflowType == null && WorkflowTypeId.HasValue )
            {
                _workflowType = WorkflowTypeCache.Read( WorkflowTypeId.Value );
            }
        }

        private void ProcessActionRequest()
        {
            string action = PageParameter( "Command" );
            if ( !string.IsNullOrWhiteSpace( action ) )
            {
                CompleteFormAction( action );
            }
        }

        private List<FormElementItem> BuildForm( bool setValues )
        {
            var formElements = new List<FormElementItem>();

            var mergeFields = AvalancheUtilities.GetMergeFields( CurrentPerson );
            mergeFields.Add( "Action", _action );
            mergeFields.Add( "Activity", _activity );
            mergeFields.Add( "Workflow", _workflow );

            var form = _actionType.WorkflowForm;

            foreach ( var formAttribute in form.FormAttributes.OrderBy( a => a.Order ) )
            {
                if ( formAttribute.IsVisible )
                {
                    var attribute = AttributeCache.Read( formAttribute.AttributeId );

                    string value = attribute.DefaultValue;
                    if ( _workflow != null && _workflow.AttributeValues.ContainsKey( attribute.Key ) && _workflow.AttributeValues[attribute.Key] != null )
                    {
                        value = _workflow.AttributeValues[attribute.Key].Value;
                    }
                    // Now see if the key is in the activity attributes so we can get it's value
                    else if ( _activity != null && _activity.AttributeValues.ContainsKey( attribute.Key ) && _activity.AttributeValues[attribute.Key] != null )
                    {
                        value = _activity.AttributeValues[attribute.Key].Value;
                    }

                    if ( !string.IsNullOrWhiteSpace( formAttribute.PreHtml ) )
                    {
                        formElements.Add( new FormElementItem
                        {
                            Type = FormElementType.Label,
                            Value = formAttribute.PreHtml.ResolveMergeFields( mergeFields )
                        } );
                    }

                    if ( formAttribute.IsReadOnly )
                    {

                        formElements.Add( new FormElementItem
                        {
                            Type = FormElementType.Label,
                            Value = value,
                            Label = formAttribute.HideLabel ? string.Empty : attribute.Name
                        } );
                    }
                    else
                    {

                        var formElement = attribute.FieldType.Field.GetMobileElement( attribute );
                        formElement.Key = attribute.Key;
                        formElement.Label = formAttribute.HideLabel ? string.Empty : attribute.Name;
                        formElement.Required = formAttribute.IsRequired;
                        formElement.Value = value;

                        formElements.Add( formElement );
                    }
                    if ( !string.IsNullOrWhiteSpace( formAttribute.PostHtml ) )
                    {
                        formElements.Add( new FormElementItem
                        {
                            Type = FormElementType.Label,
                            Value = formAttribute.PostHtml.ResolveMergeFields( mergeFields )
                        } );
                    }
                }
            }


            foreach ( var action in form.Actions.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
            {
                var details = action.Split( new char[] { '^' } );
                if ( details.Length > 0 )
                {
                    formElements.Add( new FormElementItem
                    {
                        Type = FormElementType.Button,
                        Value = details[0].EscapeQuotes(),
                        Key = details[0].EscapeQuotes()
                    } );
                }
            }
            return formElements;
        }





        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var response = new FormResponse();

            if ( _workflow != null && _actionType != null )
            {
                var form = _actionType.WorkflowForm;

                var values = new Dictionary<int, string>();
                foreach ( var formAttribute in form.FormAttributes.OrderBy( a => a.Order ) )
                {
                    if ( formAttribute.IsVisible && !formAttribute.IsReadOnly )
                    {
                        var attribute = AttributeCache.Read( formAttribute.AttributeId );

                        if ( attribute != null && Body.ContainsKey( attribute.Key ) )
                        {
                            IHasAttributes item = null;
                            if ( attribute.EntityTypeId == _workflow.TypeId )
                            {
                                item = _workflow;
                            }
                            else if ( attribute.EntityTypeId == _activity.TypeId )
                            {
                                item = _activity;
                            }

                            if ( item != null )
                            {
                                item.SetAttributeValue( attribute.Key, Body[attribute.Key] );
                            }
                        }
                    }
                }
            }


            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };
        }

        private void CompleteFormAction( string formAction )
        {
            //    if ( !string.IsNullOrWhiteSpace( formAction ) &&
            //        _workflow != null &&
            //        _actionType != null &&
            //        _actionType.WorkflowForm != null &&
            //        _activity != null &&
            //        _action != null )
            //    {
            //        var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
            //        mergeFields.Add( "Action", _action );
            //        mergeFields.Add( "Activity", _activity );
            //        mergeFields.Add( "Workflow", _workflow );

            //        Guid activityTypeGuid = Guid.Empty;
            //        string responseText = "Your information has been submitted successfully.";

            //        foreach ( var action in _actionType.WorkflowForm.Actions.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
            //        {
            //            var actionDetails = action.Split( new char[] { '^' } );
            //            if ( actionDetails.Length > 0 && actionDetails[0] == formAction )
            //            {
            //                if ( actionDetails.Length > 2 )
            //                {
            //                    activityTypeGuid = actionDetails[2].AsGuid();
            //                }

            //                if ( actionDetails.Length > 3 && !string.IsNullOrWhiteSpace( actionDetails[3] ) )
            //                {
            //                    responseText = actionDetails[3].ResolveMergeFields( mergeFields );
            //                }
            //                break;
            //            }
            //        }

            //        _action.MarkComplete();
            //        _action.FormAction = formAction;
            //        _action.AddLogEntry( "Form Action Selected: " + _action.FormAction );

            //        if ( _action.ActionTypeCache.IsActivityCompletedOnSuccess )
            //        {
            //            _action.Activity.MarkComplete();
            //        }

            //        if ( _actionType.WorkflowForm.ActionAttributeGuid.HasValue )
            //        {
            //            var attribute = AttributeCache.Read( _actionType.WorkflowForm.ActionAttributeGuid.Value );
            //            if ( attribute != null )
            //            {
            //                IHasAttributes item = null;
            //                if ( attribute.EntityTypeId == _workflow.TypeId )
            //                {
            //                    item = _workflow;
            //                }
            //                else if ( attribute.EntityTypeId == _activity.TypeId )
            //                {
            //                    item = _activity;
            //                }

            //                if ( item != null )
            //                {
            //                    item.SetAttributeValue( attribute.Key, formAction );
            //                }
            //            }
            //        }

            //        if ( !activityTypeGuid.IsEmpty() )
            //        {
            //            var activityType = _workflowType.ActivityTypes.Where( a => a.Guid.Equals( activityTypeGuid ) ).FirstOrDefault();
            //            if ( activityType != null )
            //            {
            //                WorkflowActivity.Activate( activityType, _workflow );
            //            }
            //        }

            //        List<string> errorMessages;
            //        if ( _workflowService.Process( _workflow, out errorMessages ) )
            //        {
            //            int? previousActionId = null;

            //            if ( _action != null )
            //            {
            //                previousActionId = _action.Id;
            //            }

            //            ActionTypeId = null;
            //            _action = null;
            //            _actionType = null;
            //            _activity = null;

            //            if ( HydrateObjects() && _action != null && _action.Id != previousActionId )
            //            {
            //                // If we are already being directed (presumably from the Redirect Action), don't redirect again.
            //                if ( !Response.IsRequestBeingRedirected )
            //                {
            //                    var cb = CurrentPageReference;
            //                    cb.Parameters.AddOrReplace( "WorkflowId", _workflow.Id.ToString() );
            //                    Response.Redirect( cb.BuildUrl(), false );
            //                    Context.ApplicationInstance.CompleteRequest();
            //                }
            //            }
            //            else
            //            {
            //                if ( lSummary.Text.IsNullOrWhiteSpace() )
            //                {
            //                    ShowMessage( NotificationBoxType.Success, string.Empty, responseText, ( _action == null || _action.Id != previousActionId ) );
            //                }
            //                else
            //                {
            //                    pnlForm.Visible = false;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            ShowMessage( NotificationBoxType.Danger, "Workflow Processing Error(s):",
            //                "<ul><li>" + errorMessages.AsDelimited( "</li><li>", null, true ) + "</li></ul>" );
            //        }
            //        if ( _workflow.Id != 0 )
            //        {
            //            WorkflowId = _workflow.Id;
            //        }
            //    }
            //}

            #endregion

        }
    }
}
