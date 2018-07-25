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
using Avalanche.Attribute;
using Newtonsoft.Json;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Prayer Request Entry" )]
    [Category( "Avalanche" )]
    [Description( "Block to add prayer requests to the prayer system via mobile." )]

    // Category Selection
    [CategoryField( "Category Selection", "A top level category. This controls which categories the person can choose from when entering their prayer request.", false, "Rock.Model.PrayerRequest", "", "", false, "", "Category Selection", 1 )]
    [CategoryField( "Default Category", "If categories are not being shown, choose a default category to use for all new prayer requests.", false, "Rock.Model.PrayerRequest", "", "", false, "4B2D88F5-6E45-4B4B-8776-11118C8E8269", "Category Selection", 2, "DefaultCategory" )]

    // Features
    [BooleanField( "Enable Auto Approve", "If enabled, prayer requests are automatically approved; otherwise they must be approved by an admin before they can be seen by the prayer team.", true, "Features", 3 )]
    [IntegerField( "Expires After (Days)", "Number of days until the request will expire (only applies when auto-approved is enabled).", false, 14, "Features", 4, "ExpireDays" )]
    [BooleanField( "Default Allow Comments Setting", "This is the default setting for the 'Allow Comments' on prayer requests. If you enable the 'Comments Flag' below, the requestor can override this default setting.", true, "Features", 5 )]
    [BooleanField( "Enable Urgent Flag", "If enabled, requestors will be able to flag prayer requests as urgent.", false, "Features", 6 )]
    [BooleanField( "Enable Comments Flag", "If enabled, requestors will be able set whether or not they want to allow comments on their requests.", false, "Features", 7 )]
    [BooleanField( "Enable Public Display Flag", "If enabled, requestors will be able set whether or not they want their request displayed on the public website.", false, "Features", 8 )]
    [BooleanField( "Default To Public", "If enabled, all prayers will be set to public by default", false, "Features", 9 )]
    [BooleanField( "Require Last Name", "Require that a last name be entered", true, "Features", 11 )]
    [BooleanField( "Show Campus", "Show a campus picker", true, "Features", 12 )]
    [BooleanField( "Require Campus", "Require that a campus be selected", false, "Features", 13 )]

    // On Save Behavior
    [ActionItemField( "Action Item", "Action to take upon submittin the prayer request.", false )]

    [TextField( "Save Success Text", "Text the user sees when the prayer is saved.", defaultValue: "Thank you for allowing us to pray for you.", order: 16 )]
    [WorkflowTypeField( "Workflow", "An optional workflow to start when prayer request is created. The PrayerRequest will be set as the workflow 'Entity' attribute when processing is started.", false, false, "", "On Save Behavior", 17 )]

    public partial class PrayerRequestEntry : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbLabel.Text = "Prayer Request Entry";
        }

        public override MobileBlock GetMobile( string parameter )
        {
            var form = new List<FormElementItem>();

            var firstName = new FormElementItem
            {
                Type = FormElementType.Entry,
                Key = "firstName",
                Label = "First Name",
                Required = true
            };
            if ( CurrentPerson != null )
            {
                firstName.Value = CurrentPerson.NickName;
            }
            form.Add( firstName );

            var lastName = new FormElementItem
            {
                Type = FormElementType.Entry,
                Key = "lastName",
                Label = "Last Name",
                Required = GetAttributeValue( "RequireLastName" ).AsBoolean()
            };
            if ( CurrentPerson != null )
            {
                lastName.Value = CurrentPerson.LastName;
            }
            form.Add( lastName );

            var email = new FormElementItem
            {
                Type = FormElementType.Entry,
                Key = "email",
                Label = "Email",
                Required = false,
                Keyboard = Keyboard.Email
            };
            if ( CurrentPerson != null )
            {
                email.Value = CurrentPerson.Email;
            }
            form.Add( email );

            if ( GetAttributeValue( "ShowCampus" ).AsBoolean() )
            {
                var campus = new FormElementItem
                {
                    Type = FormElementType.Picker,
                    Key = "campus",
                    Label = "Campus",
                    Options = CampusCache.All().ToDictionary( c => c.Id.ToString(), c => c.Name ),
                    Required = GetAttributeValue( "RequireCampus" ).AsBoolean()
                };
                if ( CurrentPerson != null )
                {
                    campus.Value = CurrentPerson.GetCampus().Id.ToString();
                }

                form.Add( campus );
            }

            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "CategorySelection" ) ) )
            {
                var categoryGuid = GetAttributeValue( "CategorySelection" ).AsGuid();
                var categoryList = CategoryCache.Read( categoryGuid ).Categories.ToDictionary( c => c.Id.ToString(), c => c.Name );

                var category = new FormElementItem
                {
                    Type = FormElementType.Picker,
                    Key = "category",
                    Label = "Category",
                    Options = categoryList,
                    Required = true
                };
                form.Add( category );
            }

            var request = new FormElementItem
            {
                Type = FormElementType.Editor,
                Label = "Request",
                Key = "request",
                HeightRequest = 100,
                Required = true
            };
            form.Add( request );

            if ( GetAttributeValue( "EnableUrgentFlag" ).AsBoolean() )
            {
                var urgent = new FormElementItem
                {
                    Type = FormElementType.Switch,
                    Key = "urgent",
                    Label = "Urgent?"
                };
                form.Add( urgent );
            }

            if ( GetAttributeValue( "DefaultAllowCommentsSetting" ).AsBoolean() )
            {
                var allowComments = new FormElementItem
                {
                    Type = FormElementType.Switch,
                    Key = "allowComments",
                    Label = "Allow Encouraging Comments?"
                };
                form.Add( allowComments );
            }

            if ( GetAttributeValue( "EnablePublicDisplayFlag" ).AsBoolean() )
            {
                var allowPublication = new FormElementItem
                {
                    Type = FormElementType.Switch,
                    Key = "allowPublication",
                    Label = "Allow Publication?"
                };
                form.Add( allowPublication );
            }

            var button = new FormElementItem
            {
                Type = FormElementType.Button,
                Label = "Save Request",
                Key = "save"
            };
            form.Add( button );

            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( form ) );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };
        }
        Dictionary<string, string> body;
        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            body = Body;
            RockContext rockContext = new RockContext();
            PrayerRequestService prayerRequestService = new PrayerRequestService( rockContext );
            PrayerRequest prayerRequest = new PrayerRequest();

            prayerRequest.EnteredDateTime = RockDateTime.Now;
            prayerRequest.FirstName = GetItem( "firstName" );
            prayerRequest.LastName = GetItem( "lastName" );
            prayerRequest.Text = GetItem( "request" );
            prayerRequest.RequestedByPersonAliasId = CurrentPersonAliasId;

            if ( !string.IsNullOrWhiteSpace( GetItem( "campus" ) ) )
            {
                prayerRequest.CampusId = GetItem( "campus" ).AsInteger();
            }

            bool isAutoApproved = GetAttributeValue( "EnableAutoApprove" ).AsBoolean();
            if ( isAutoApproved )
            {
                prayerRequest.ApprovedByPersonAliasId = CurrentPersonAliasId;
                prayerRequest.ApprovedOnDateTime = RockDateTime.Now;
                var expireDays = GetAttributeValue( "ExpireDays" ).AsDouble();
                prayerRequest.ExpirationDate = RockDateTime.Now.AddDays( expireDays );
            }

            //Category
            if ( GetItem( "category" ).AsInteger() != 0 )
            {
                prayerRequest.CategoryId = GetItem( "category" ).AsInteger();
            }
            else
            {
                Guid defaultCategoryGuid = GetAttributeValue( "DefaultCategory" ).AsGuid();
                var defaultCategory = CategoryCache.Read( defaultCategoryGuid );
                if ( defaultCategory != null )
                {
                    prayerRequest.CategoryId = defaultCategory.Id;
                }
            }

            if ( GetItem( "urgent" ).AsBoolean() )
            {
                prayerRequest.IsUrgent = true;
            }
            else
            {
                prayerRequest.IsUrgent = false;
            }

            if ( GetItem( "allowComments" ).AsBoolean() )
            {
                prayerRequest.AllowComments = true;
            }
            else
            {
                prayerRequest.AllowComments = false;
            }

            if ( GetItem( "allowPublication" ).AsBoolean() )
            {
                prayerRequest.IsPublic = true;
            }
            else
            {
                prayerRequest.IsPublic = false;
            }
            prayerRequestService.Add( prayerRequest );
            rockContext.SaveChanges();

            Guid? workflowTypeGuid = GetAttributeValue( "Workflow" ).AsGuidOrNull();

            if ( workflowTypeGuid.HasValue )
            {
                prayerRequest.LaunchWorkflow( workflowTypeGuid, prayerRequest.Name );
            }

            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                               CustomAttributes,
                                               CurrentPerson,
                                               AvalancheUtilities.GetMergeFields( CurrentPerson ) );

            var response = new FormResponse
            {
                Success = true,
                Message = GetAttributeValue( "SaveSuccessText" )
            };

            if ( CustomAttributes.ContainsKey( "ActionType" ) && CustomAttributes["ActionType"] != "0" )
            {
                response.ActionType = CustomAttributes["ActionType"];
            }

            if ( CustomAttributes.ContainsKey( "Resource" ) )
            {
                response.Resource = CustomAttributes["Resource"];
            }

            if ( CustomAttributes.ContainsKey( "Parameter" ) )
            {
                response.Parameter = CustomAttributes["Parameter"];
            }


            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };
        }

        private string GetItem( string item )
        {
            if ( body.ContainsKey( item ) && body[item] != null )
            {
                return body[item];
            }
            return "";
        }
    }
}