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
using Rock.Web.UI.Controls;
using System.IO;
using System.Text;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Person Card" )]
    [Category( "Avalanche" )]
    [Description( "Card to display person's information from guid." )]

    [TextField( "Accent Color", "Optional color to accent the member detail.", false )]
    [CustomDropdownListField( "EntityType", "The entity type to get the person from.", "Person^Person,GroupMember^Group Member" )]

    public partial class PersonCard : AvalancheBlock
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
            RockContext rockContext = new RockContext();

            Person person = null;
            if ( GetAttributeValue( "EntityType" ) == "GroupMember" )
            {
                GroupMemberService groupMemberService = new GroupMemberService( rockContext );
                var groupMember = groupMemberService.Get( parameter.AsGuid() );
                if ( groupMember != null )
                {
                    person = groupMember.Person;
                }
            }
            else
            {
                PersonService personService = new PersonService( rockContext );
                person = personService.Get( parameter.AsGuid() );
            }


            if ( person != null )
            {
                CustomAttributes["PersonGuid"] = person.Guid.ToString();
                CustomAttributes["Name"] = person.FullName;
                CustomAttributes["Image"] = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + person.PhotoUrl;

                CustomAttributes["AccentColor"] = GetAttributeValue( "AccentColor" );

                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.PersonCard",
                    Attributes = CustomAttributes
                };
            }

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Null",
                Attributes = CustomAttributes
            };
        }

        public override MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body )
        {
            RockContext rockContext = new RockContext();
            PersonService personService = new PersonService( rockContext );
            BinaryFileService binaryFileService = new BinaryFileService( rockContext );

            var person = personService.Get( resource.AsGuid() );
            if ( person == null )
            {
                return base.HandleRequest( resource, Body );
            }

            var data = new BinaryFileData()
            {
                Content = Convert.FromBase64String( Body["Photo"] )
            };

            var file = new BinaryFile()
            {
                MimeType = "image/jpg",
                DatabaseData = data,
                FileName = person.FullName,
                BinaryFileType = new BinaryFileTypeService( rockContext ).Get( Rock.SystemGuid.BinaryFiletype.PERSON_IMAGE.AsGuid() ),
            };

            binaryFileService.Add( file );
            rockContext.SaveChanges();

            AddOrUpdatePersonInPhotoRequestGroup( person, rockContext );
            person.PhotoId = file.Id;
            rockContext.SaveChanges();

            return base.HandleRequest( resource, Body );
        }

        private void AddOrUpdatePersonInPhotoRequestGroup( Person person, RockContext rockContext )
        {
            GroupService service = new GroupService( rockContext );
            var _photoRequestGroup = service.GetByGuid( Rock.SystemGuid.Group.GROUP_PHOTO_REQUEST.AsGuid() );

            var groupMember = _photoRequestGroup.Members.Where( m => m.PersonId == person.Id ).FirstOrDefault();
            if ( groupMember == null )
            {
                groupMember = new GroupMember();
                groupMember.GroupId = _photoRequestGroup.Id;
                groupMember.PersonId = person.Id;
                groupMember.GroupRoleId = _photoRequestGroup.GroupType.DefaultGroupRoleId ?? -1;
                _photoRequestGroup.Members.Add( groupMember );
            }

            groupMember.GroupMemberStatus = GroupMemberStatus.Pending;
        }

        private string ProcessLava( string lava, GroupMember groupMember )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, CurrentPerson );
            mergeObjects["GroupMember"] = groupMember;
            return lava.ResolveMergeFields( mergeObjects );
        }
    }
}