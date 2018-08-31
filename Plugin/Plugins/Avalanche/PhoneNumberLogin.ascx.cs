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
using Rock.Communication;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Phone Number Login" )]
    [Category( "Avalanche" )]
    [Description( "Block to log in with your phone number" )]

    [DefinedValueField( Rock.SystemGuid.DefinedType.COMMUNICATION_SMS_FROM, "From", "The number to originate message from (configured under Admin Tools > General Settings > Defined Types > SMS From Values).", true, false, "", "", 0 )]
    [TextField( "Message", "Message that will be sent along with the login code.", true, "Use {{ password }} to log into.  If you recieved this message by mistake please disregard." )]
    [TextField( "Help Url", "Page to send the user to if their phonenumber could not be resolved.", false )]
    public partial class PhoneNumberLogin : AvalancheBlock
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
            CustomAttributes["HelpUrl"] = GetAttributeValue( "HelpUrl" );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.PhoneNumberLogin",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            string response = GenerateCode( request );


            return new MobileBlockResponse()
            {
                Request = request,
                Response = response,
                TTL = 0
            };
        }

        private string GenerateCode( string resource )
        {
            if ( resource == null || resource.Length != 10 )
            {
                return "Please enter a 10 digit phone number.";
            }

            RockContext rockContext = new RockContext();
            PhoneNumberService phoneNumberService = new PhoneNumberService( rockContext );
            var numberOwners = phoneNumberService.Queryable()
                .Where( pn => pn.Number == resource )
                .Select( pn => pn.Person )
                .DistinctBy( p => p.Id )
                .ToList();

            if ( numberOwners.Count == 0 )
            {
                return "0|We are sorry, we could not find your phone number in our records.";
            }

            if ( numberOwners.Count > 1 )
            {
                return "2|We are sorry, we dected more than one person with your number in our records.";
            }

            var person = numberOwners.FirstOrDefault();

            UserLoginService userLoginService = new UserLoginService( rockContext );
            var userLogin = userLoginService.Queryable()
                .Where( u => u.UserName == ( "__PHONENUMBER__+1" + resource ) )
                .FirstOrDefault();

            if ( userLogin == null )
            {
                var entityTypeId = EntityTypeCache.Read( "Avalanche.Security.Authentication.PhoneNumber" ).Id;

                userLogin = new UserLogin()
                {
                    UserName = "__PHONENUMBER__+1" + resource,
                    EntityTypeId = entityTypeId,
                };
                userLoginService.Add( userLogin );
            }

            userLogin.PersonId = person.Id;
            userLogin.LastPasswordChangedDateTime = Rock.RockDateTime.Now;
            userLogin.FailedPasswordAttemptWindowStartDateTime = Rock.RockDateTime.Now;
            userLogin.FailedPasswordAttemptCount = 0;
            userLogin.IsConfirmed = true;
            userLogin.Password = new Random().Next( 100000, 999999 ).ToString();

            rockContext.SaveChanges();

            var workflowName = string.Format( "{0} ({1})", person.FullName, resource );
            var atts = new Dictionary<string, string>
            {
                { "PhoneNumber",resource },
                { "Password" , userLogin.Password }
            };

            var recipients = new List<RecipientData>();
            recipients.Add( new RecipientData( resource ) );

            var smsMessage = new RockSMSMessage();
            smsMessage.SetRecipients( recipients );

            // Get the From value
            Guid? fromGuid = GetAttributeValue( "From" ).AsGuidOrNull();
            if ( fromGuid.HasValue )
            {
                var fromValue = DefinedValueCache.Read( fromGuid.Value, rockContext );
                if ( fromValue != null )
                {
                    smsMessage.FromNumber = DefinedValueCache.Read( fromValue.Id );
                }
            }

            var mergeObjects = new Dictionary<string, object> { { "password", userLogin.Password } };
            var message = AvalancheUtilities.ProcessLava( GetAttributeValue( "Message" ), CurrentPerson, mergeObjects: mergeObjects );

            smsMessage.Message = message;
            smsMessage.Send();

            return "1|Success!";
        }
    }
}