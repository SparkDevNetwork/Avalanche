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
using Rock.Security.ExternalAuthentication;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Phone Number Login" )]
    [Category( "Avalanche" )]
    [Description( "Block to log in with your phone number" )]

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

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> body )
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
                return "0|Please enter a 10 digit phone number.";
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

            var smsAuthentication = new SMSAuthentication();
            var success = smsAuthentication.SendSMSAuthentication( resource );

            if ( success )
            {
                return "1|Success!";
            }
            return "0|We are sorry, we could not send the login request at this time. Please login using a different method.";

        }
    }
}