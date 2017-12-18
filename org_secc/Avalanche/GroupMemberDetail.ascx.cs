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
    [DisplayName( "Group Member Detail" )]
    [Category( "Avalanche" )]
    [Description( "Group member detail block." )]

    [TextField( "Accent Color", "Optional color to accent the member detail.", false )]
    [CodeEditorField( "Markdown Lava", "Markdown to display as group member details.", CodeEditorMode.Markdown )]
    public partial class GroupMemberDetail : AvalancheBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
        }

        public override MobileBlock GetMobile( string arg )
        {
            RockContext rockContext = new RockContext();
            GroupMemberService groupMemberService = new GroupMemberService( rockContext );
            var groupMember = groupMemberService.Get( arg.AsGuid() );

            if ( groupMember != null )
            {
                CustomAttributes["PersonGuid"] = groupMember.Person.Guid.ToString();
                CustomAttributes["Name"] = groupMember.Person.FullName;
                CustomAttributes["Image"] = GlobalAttributesCache.Value( "InternalApplicationRoot" ) + groupMember.Person.PhotoUrl;
                CustomAttributes["Markdown"] = ProcessLava( GetAttributeValue( "MarkdownLava" ), groupMember );

                CustomAttributes["AccentColor"] = GetAttributeValue( "AccentColor" );

                return new MobileBlock()
                {
                    BlockType = "Avalanche.Blocks.GroupMemberDetail",
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