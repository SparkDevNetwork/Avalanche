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
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;

namespace Avalanche.Security.Authentication
{
    /// <summary>
    /// Authenticates a username/password using a phone number and pin
    /// </summary>
    [Description( "PhoneNumber Authentication Provider" )]
    [Export( typeof( AuthenticationComponent ) )]
    [ExportMetadata( "ComponentName", "PhoneNumber" )]
    public class PhoneNumber : AuthenticationComponent
    {
        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public override AuthenticationServiceType ServiceType
        {
            get { return AuthenticationServiceType.Internal; }
        }

        /// <summary>
        /// Determines if user is directed to another site (i.e. Facebook, Gmail, Twitter, etc) to confirm approval of using
        /// that site's credentials for authentication.
        /// </summary>
        /// <value>
        /// The requires remote authentication.
        /// </value>
        public override bool RequiresRemoteAuthentication
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes the <see cref="Database" /> class.
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Authentication requires a 'PasswordKey' app setting</exception>
        static PhoneNumber()
        {
        }

        /// <summary>
        /// Authenticates the specified user name.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Boolean Authenticate( UserLogin user, string password )
        {
            try
            {
                if ( user.LastPasswordChangedDateTime == null
                    || user.LastPasswordChangedDateTime < Rock.RockDateTime.Now.AddMinutes( -30 ) )
                {
                    return false;
                }

                if ( user.FailedPasswordAttemptCount > 5 )
                {
                    return false;
                }
                if ( string.IsNullOrWhiteSpace( user.Password ) || user.Password != password )
                {
                    using ( RockContext rockContext = new RockContext() )
                    {
                        UserLoginService userLoginService = new UserLoginService( rockContext );
                        var userLogin = userLoginService.Get( user.Id );
                        if ( userLogin != null )
                        {
                            userLogin.FailedPasswordAttemptCount++;
                            rockContext.SaveChanges();
                        }
                    }

                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Encodes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override String EncodePassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates the user based on a request from a third-party provider.  Will set the username and returnUrl values.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Authenticate( HttpRequest request, out string userName, out string returnUrl )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Uri GenerateLoginUrl( HttpRequest request )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests the Http Request to determine if authentication should be tested by this
        /// authentication provider.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsReturningFromAuthentication( HttpRequest request )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the URL of an image that should be displayed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ImageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether [supports change password].
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports change password]; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsChangePassword
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="warningMessage">The warning message.</param>
        /// <returns>
        /// A <see cref="System.Boolean" /> value that indicates if the password change was successful. <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.Exception">Cannot change password on external service type</exception>
        public override bool ChangePassword( UserLogin user, string oldPassword, string newPassword, out string warningMessage )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        public override void SetPassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }
    }
}