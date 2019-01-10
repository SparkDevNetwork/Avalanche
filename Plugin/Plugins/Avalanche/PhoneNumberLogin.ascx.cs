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
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
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

            var smsAuthentication = new SMSAuthentication();
            RockContext rockContext = new RockContext();
            string error;
            var person = smsAuthentication.GetNumberOwner( resource, rockContext, out error );

            if ( person == null )
            {
                return "0|" + error;
            }

            var success = smsAuthentication.SendSMSAuthentication( resource );

            if ( success )
            {
                return "1|Success!";
            }
            return "0|We are sorry, we could not send the login request at this time. Please login using a different method.";

        }
    }
}