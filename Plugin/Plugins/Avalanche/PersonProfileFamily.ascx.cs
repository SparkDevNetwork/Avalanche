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
using System.Web.UI;
using Rock.Web.Cache;
using Rock.Data;
using System.Linq;
using System.Collections.Generic;
using Rock;
using Avalanche;
using Avalanche.Models;
using Rock.Attribute;
using Avalanche.Attribute;
using Newtonsoft.Json;
using System.Text;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Person Profile Family" )]
    [Category( "Avalanche" )]
    [Description( "Block to show a list of Family members." )]

    [ActionItemField( "Action Item", "Action to take upon press of item in list." )]
    [DefinedValueField( AvalancheUtilities.MobileListViewComponent, "Component", "Different components will display your list in different ways." )]
    [LinkedPage( "Additional Changes Link", "Optional page to request aditional changes not permitted here.", false )]
    public partial class PersonProfileFamily : AvalancheBlock
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

            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                   CustomAttributes,
                                   CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                   GetAttributeValue( "EnabledLavaCommands" ),
                                   parameter );

            var valueGuid = GetAttributeValue( "Component" );
            var value = DefinedValueCache.Get( valueGuid );
            if ( value != null )
            {
                CustomAttributes["Component"] = value.GetAttributeValue( "ComponentType" );
            }

            CustomAttributes["InitialRequest"] = parameter; //Request for pull to refresh

            var family = new List<ListElement>();

            family.Add(
                new ListElement
                {
                    Image = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + CurrentPerson.PhotoUrl,
                    Id = CurrentPersonAlias.Guid.ToString(),
                    Title = CurrentPerson.FullName,
                    Description = GetInfo( CurrentPerson, true )
                } );

            foreach ( var member in CurrentPerson.GetFamilyMembers() )
            {
                family.Add( new ListElement
                {
                    Image = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + member.Person.PhotoUrl,
                    Id = member.Person.PrimaryAlias.Guid.ToString(),
                    Title = member.Person.FullName,
                    Description = GetInfo( member.Person, false )
                } );
            }

            family.Add( new ListElement
            {
                Id = "0",
                Title = "Add New Family Member"
            } );

            var additionalChanges = GetAttributeValue( "AdditionalChangesLink" );

            var additionalChangesPage = PageCache.Get( additionalChanges );
            if ( additionalChangesPage != null )
            {
                family.Add( new ListElement
                {
                    ActionType = "1",
                    Resource = additionalChangesPage.Id.ToString(),
                    Title = "Request Additional Changes",
                    Id = "1"
                } );


            }



            CustomAttributes["Content"] = JsonConvert.SerializeObject( family );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ListViewBlock",
                Attributes = CustomAttributes
            };
        }

        private string GetInfo( Person person, bool completeInfo )
        {
            var markdown = new StringBuilder();

            if ( person.BirthDate != null )
            {
                markdown.Append( string.Format( "{0} *({1})* \n", person.FormatAge(), person.BirthDate.Value.ToString( "MM/dd/yyyy" ) ) );
            }
            markdown.Append( person.Gender != Gender.Unknown ? person.Gender.ToString() + "\n" : string.Empty );
            markdown.Append( person.MaritalStatusValueId.DefinedValue() );
            markdown.Append( "\n" );
            if ( !string.IsNullOrWhiteSpace( person.GradeFormatted ) )
            {
                markdown.Append( person.GradeFormatted );
                markdown.Append( "\n" );
            }
            markdown.Append( "\n" );

            foreach ( var phone in person.PhoneNumbers )
            {
                markdown.Append( string.Format( "{0}: {1}\n", ( ( int? ) phone.NumberTypeValueId ).DefinedValue(), phone.NumberFormatted ) );
            }
            markdown.Append( person.Email );
            markdown.Append( "\n" );

            if ( completeInfo )
            {
                var location = person.GetHomeLocation();
                if ( location != null )
                {
                    markdown.Append( string.Format( "\n**Home Address**\n {0} {1}\n {2}, {3} {4}",
                        location.Street1, location.Street2, location.City, location.State, location.PostalCode ) );
                }
            }

            return markdown.ToString();
        }
    }
}