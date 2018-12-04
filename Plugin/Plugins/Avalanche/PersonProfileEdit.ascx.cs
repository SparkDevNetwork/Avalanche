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
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Public Profile Edit Block" )]
    [Category( "Avalanche" )]
    [Description( "Allows the user to update their personal information" )]

    [LinkedPage( "Next Page", "Page to forward to after completion." )]
    public partial class PersonProfileEdit : AvalancheBlock
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

            if ( CurrentPerson == null )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.Null",
                    Attributes = CustomAttributes
                };
            }
            Person person = null;

            if ( parameter == "0" )
            {
                person = new Person();
            }
            else
            {
                RockContext rockContext = new RockContext();
                PersonAliasService personAliasService = new PersonAliasService( rockContext );
                var personAlias = personAliasService.Get( parameter.AsGuid() );
                if ( personAlias == null )
                {
                    return new MobileBlock()
                    {
                        BlockType = "Avalanche.Blocks.Null",
                        Attributes = CustomAttributes
                    };
                }
                person = personAlias.Person;
            }
            if ( person.Id != 0 && !CanEdit( person ) )
            {
                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.Null",
                    Attributes = CustomAttributes
                };
            }


            var form = GetForm( person, parameter );
            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( form ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };
        }

        private bool CanEdit( Person person )
        {
            return CurrentPerson
                .GetFamilies()
                .SelectMany( f => f.Members )
                .Select( m => m.PersonId ).Contains( person.Id );
        }

        private List<FormElementItem> GetForm( Person person, string parameter )
        {
            var form = new List<FormElementItem>();

            var hidden = new FormElementItem
            {
                Type = FormElementType.Hidden,
                Key = "parameter",
                Value = parameter
            };
            form.Add( hidden );

            var title = new FormElementItem
            {
                Label = "Title",
                Type = FormElementType.Picker,
                Key = "title",
                Value = person.TitleValueId.ToString(),
                Options = DefinedTypeCache.Get( new Guid( Rock.SystemGuid.DefinedType.PERSON_TITLE ) ).DefinedValues.ToDictionary( k => k.Id.ToString(), k => k.Value )
            };
            form.Add( title );

            if ( string.IsNullOrEmpty( person.FirstName ) )
            {
                var firstName = new FormElementItem
                {
                    Label = "First Name",
                    Type = FormElementType.Entry,
                    Key = "firstName",
                    Value = person.FirstName,
                    Required = true
                };
                form.Add( firstName );

            }
            else
            {
                var firstName = new FormElementItem
                {
                    Label = "First Name",
                    Type = FormElementType.Label,
                    Key = "firstName",
                    Value = person.FirstName,
                };
                form.Add( firstName );
            }

            var nickName = new FormElementItem
            {
                Label = "Nick Name",
                Type = FormElementType.Entry,
                Key = "nickName",
                Value = person.NickName,
            };
            form.Add( nickName );

            var lastName = new FormElementItem
            {
                Label = "Last Name",
                Type = FormElementType.Entry,
                Key = "lastName",
                Value = person.LastName,
                Required = true
            };
            form.Add( lastName );

            var suffixOptions = new Dictionary<string, string> { { "", "" } };
            DefinedTypeCache.Get( new Guid( Rock.SystemGuid.DefinedType.PERSON_SUFFIX ) )
                .DefinedValues
                .ForEach( v => suffixOptions.Add( v.Id.ToString(), v.Value ) );
            var suffix = new FormElementItem
            {
                Label = "Suffix",
                Type = FormElementType.Picker,
                Key = "suffix",
                Value = person.SuffixValueId.ToString(),
                Options = suffixOptions
            };
            form.Add( suffix );

            var birthdateString = "";
            if ( person.BirthDate.HasValue )
            {
                birthdateString = person.BirthDate.Value.ToString( "MM/dd/yyyy" );
            }


            var birthdate = new FormElementItem
            {
                Label = "Birthday",
                Type = FormElementType.DatePicker,
                Key = "birthdate",
                Value = birthdateString,
                Required = true
            };
            form.Add( birthdate );

            var gender = new FormElementItem
            {
                Label = "Gender",
                Type = FormElementType.Picker,
                Key = "gender",
                Value = ( ( int ) person.Gender ).ToString(),
                Options = new Dictionary<string, string> { { "1", "Male" }, { "2", "Female" }, { "0", "Unknown" } }
            };
            form.Add( gender );

            var contactInfo = new FormElementItem
            {
                Type = FormElementType.Label,
                Key = "contactInfo",
                Value = "Contact Info",
            };
            form.Add( contactInfo );

            var mobileNumber = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
            var mobilePhoneNumber = new FormElementItem
            {
                Label = "Mobile Phone",
                Type = FormElementType.Entry,
                Key = "mobilePhone",
                Keyboard = Keyboard.Telephone,
                Value = mobileNumber != null ? mobileNumber.NumberFormatted : ""
            };
            form.Add( mobilePhoneNumber );

            var homeNumber = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() );
            var homePhoneNumber = new FormElementItem
            {
                Label = "Home Phone",
                Type = FormElementType.Entry,
                Key = "homePhone",
                Keyboard = Keyboard.Telephone,
                Value = homeNumber != null ? homeNumber.NumberFormatted : ""
            };
            form.Add( homePhoneNumber );

            var workPhone = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK.AsGuid() );
            var workPhoneNumber = new FormElementItem
            {
                Label = "Work Phone",
                Type = FormElementType.Entry,
                Key = "workPhone",
                Keyboard = Keyboard.Telephone,
                Value = workPhone != null ? workPhone.NumberFormatted : ""
            };
            form.Add( workPhoneNumber );

            var email = new FormElementItem
            {
                Label = "Email",
                Type = FormElementType.Entry,
                Key = "email",
                Keyboard = Keyboard.Email,
                Value = person.Email
            };
            form.Add( email );

            var emailPerference = new FormElementItem
            {
                Label = "Email Preference",
                Type = FormElementType.Picker,
                Key = "emailPreference",
                Value = ( ( int ) person.EmailPreference ).ToString(),
                Options = new Dictionary<string, string> { { "0", "Email Allowed" }, { "1", " No Mass Emails" }, { "2", "Do Not Email" } }
            };
            form.Add( emailPerference );

            var save = new FormElementItem
            {
                Key = "save",
                Type = FormElementType.Button,
                Label = "Save",
                Value = "save"
            };
            form.Add( save );

            var cancel = new FormElementItem
            {
                Key = "cancel",
                Type = FormElementType.Button,
                Label = "Cancel",
                Value = "cancel"
            };
            form.Add( cancel );



            return form;
        }

        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> body )
        {
            if ( request == "cancel" )
            {
                var cancelResponse = new FormResponse
                {
                    Success = true,
                    ActionType = "3",
                };

                return new MobileBlockResponse()
                {
                    Request = "cancel",
                    Response = JsonConvert.SerializeObject( cancelResponse ),
                    TTL = 0
                };
            }

            RockContext rockContext = new RockContext();
            var parameter = body["parameter"];

            Person person = null;

            if ( parameter == "0" )
            {
                person = new Person();
            }
            else
            {
                PersonAliasService personAliasService = new PersonAliasService( rockContext );
                var personAlias = personAliasService.Get( parameter.AsGuid() );
                if ( personAlias == null )
                {
                    return base.HandleRequest( request, body );
                }
                person = personAlias.Person;
            }

            if ( person.Id != 0 && !CanEdit( person ) )
            {
                return base.HandleRequest( request, body );
            }

            if ( body.ContainsKey( "firstName" ) )
            {
                person.FirstName = body["firstName"];
            }

            if ( !string.IsNullOrWhiteSpace( body["nickName"] ) )
            {
                person.NickName = body["nickName"];
            }
            person.LastName = body["lastName"];
            person.TitleValueId = body["title"].AsIntegerOrNull();
            person.SuffixValueId = body["suffix"].AsIntegerOrNull();
            person.Gender = ( Gender ) body["gender"].AsInteger();
            person.Email = body["email"];
            person.EmailPreference = ( EmailPreference ) body["emailPreference"].AsInteger();
            person.SetBirthDate( body["birthdate"].AsDateTime() );

            if ( parameter == "0" )
            {
                GroupTypeRoleService groupTypeRoleService = new GroupTypeRoleService( rockContext );
                var groupTypeRoleId = groupTypeRoleService.Get( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;

                {
                    if ( person.Age < 18 )
                    {
                        groupTypeRoleId = groupTypeRoleService.Get( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;
                    }
                }
                PersonService.AddPersonToFamily( person, true, CurrentPerson.PrimaryFamilyId ?? 0, groupTypeRoleId, rockContext );

            }
            rockContext.SaveChanges();

            person.UpdatePhoneNumber( DefinedValueCache.Get(
                Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() ).Id,
                PhoneNumber.DefaultCountryCode(),
                body["mobilePhone"],
                true,
                false,
                rockContext
                );

            person.UpdatePhoneNumber( DefinedValueCache.Get(
                Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() ).Id,
                PhoneNumber.DefaultCountryCode(),
                body["homePhone"],
                false,
                false,
                rockContext
                );

            person.UpdatePhoneNumber( DefinedValueCache.Get(
                Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK.AsGuid() ).Id,
                PhoneNumber.DefaultCountryCode(),
                body["workPhone"],
                false,
                false,
                rockContext
                );

            var response = new FormResponse
            {
                Success = true,
                ActionType = "3"
            };

            var nextPage = PageCache.Get( GetAttributeValue( "NextPage" ) );
            if ( nextPage != null )
            {
                response.ActionType = "2"; // replace page
                response.Resource = nextPage.Id.ToString();

                DateTime epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
                long ms = ( long ) ( DateTime.UtcNow - epoch ).TotalMilliseconds;
                response.Parameter = ms.ToString();//Cache buster
            }


            return new MobileBlockResponse()
            {
                Request = "save",
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };

        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
        }
    }
}