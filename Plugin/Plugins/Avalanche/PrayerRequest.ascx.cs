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
using Avalanche.Field;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Prayer Request" )]
    [Category( "Avalanche" )]
    [Description( "Basic form for the purpose of requesting prayer." )]
    [ActionItemField( "Action Item" )]
    public partial class PrayerRequest : AvalancheBlock
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
            var elements = new List<FormElementItem>();
            var firstName = new FormElementItem()
            {
                Type = FormElementType.Entry,
                Key = "firstName",
                Keyboard = Keyboard.Text,
                Label = "First Name",
                BackgroundColor = "White",
                TextColor = "Red",
                Required = true
            };
            elements.Add( firstName );

            var lastName = new FormElementItem()
            {
                Type = FormElementType.Entry,
                Key = "lastName",
                Keyboard = Keyboard.Text,
                BackgroundColor = "White",
                TextColor = "Red",
                Label = "Last Name",
            };
            elements.Add( lastName );

            var prayerType = new FormElementItem()
            {
                Type = FormElementType.Picker,
                Key = "prayerType",
                Label = "Prayer Type",
                BackgroundColor = "White",
                TextColor = "Red",
                Options = new List<string>() { "Urgent", "Private", "Please Share" }
            };
            elements.Add( prayerType );

            var checkbox = new FormElementItem()
            {
                Type = FormElementType.Checkbox,
                Key = "checkBox",
                Label = "Would you like a phone call?",
                Value = "True",
                BackgroundColor = "White",
                TextColor = "Red",
            };
            elements.Add( checkbox );

            var hidden = new FormElementItem()
            {
                Type = FormElementType.Hidden,
                Key = "hiddenField",
                Value = "Hidden Field Value",
                BackgroundColor = "White",
                TextColor = "Red",
            };
            elements.Add( hidden );

            var prayerRequest = new FormElementItem()
            {
                Type = FormElementType.Editor,
                Key = "prayerRequest",
                Keyboard = Keyboard.Text,
                Label = "Prayer Request",
                Required = true,
                HeightRequest = 100,
                BackgroundColor = "White",
                TextColor = "Red",
            };
            elements.Add( prayerRequest );

            var submitButton = new FormElementItem()
            {
                Type = FormElementType.Button,
                Key = "submitButton",
                Keyboard = Keyboard.Text,
                Label = "Submit Prayer Request",
                BackgroundColor = "Green",
                TextColor = "White",
            };
            elements.Add( submitButton );

            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( elements ) );

            AvalancheUtilities.SetActionItems( GetAttributeValue( "SuccessAction" ), CustomAttributes, CurrentPerson );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };


        }


        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            var response = new FormResponse();
            var body = Body;
            if ( !body.ContainsKey( "lastName" ) || string.IsNullOrWhiteSpace( body["lastName"] ) )
            {
                response.Success = false;
                response.ErrorMessage = "Last name is required.";
            }
            else
            {
                RockContext rockContext = new RockContext();
                PrayerRequestService prayerRequestService = new PrayerRequestService( rockContext );
                var prayer = new Rock.Model.PrayerRequest
                {
                    FirstName = body["firstName"],
                    LastName = body["lastName"],
                    Text = body["prayerRequest"],
                    IsActive = true,
                    EnteredDateTime = Rock.RockDateTime.Now
                };

                if ( body.ContainsKey( "prayerType" ) )
                {
                    switch ( body["prayerType"] )
                    {
                        case "Urgent":
                            prayer.IsUrgent = true;
                            break;
                        case "Please Share":
                            prayer.IsPublic = true;
                            break;
                        default:
                            break;
                    }
                }

                if ( CurrentPerson != null )
                {
                    prayer.CreatedByPersonAliasId = CurrentPerson.PrimaryAliasId;
                }
                prayerRequestService.Add( prayer );
                rockContext.SaveChanges();

                response.Success = true;
                response.SetResponse( GetAttributeValue( "ActionItem" ) );
            }

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };
        }
    }
}