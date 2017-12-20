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

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Image Block" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A button." )]
    [TextField( "Image", "Image to be displayed", false )]
    [CustomDropdownListField( "Aspect", "Aspect type", "0^AspectFit,1^AspectFill,2^Fit" )]
    [ActionItemField( "Action Item", "Action to take on touch.", false )]

    public partial class ImageBlock : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            img.BinaryFileTypeGuid = Rock.SystemGuid.BinaryFiletype.MEDIA_FILE.AsGuid();
            if ( !Page.IsPostBack )
            {
                RockContext rockContext = new RockContext();
                BinaryFile file = new BinaryFileService( rockContext ).Get( GetAttributeValue( "Image" ).AsGuid() );
                if ( file != null )
                {
                    img.BinaryFileId = file.Id;
                }
            }
        }

        public override MobileBlock GetMobile( string arg )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ), CustomAttributes, CurrentPerson );

            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "Image" ) ) )
            {
                CustomAttributes.Add( "Source", string.Format( "{0}/GetImage.ashx?guid={1}", GlobalAttributesCache.Value( "InternalApplicationRoot" ), GetAttributeValue( "Image" ) ) );
            }
            CustomAttributes.Add( "Aspect", GetAttributeValue( "Aspect" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ImageBlock",
                Attributes = CustomAttributes
            };
        }

        protected void img_ImageUploaded( object sender, Rock.Web.UI.Controls.ImageUploaderEventArgs e )
        {
            RockContext rockContext = new RockContext();
            BinaryFile file = new BinaryFileService( rockContext ).Get( img.BinaryFileId ?? 0 );
            if ( file != null )
            {
                file.IsTemporary = false;
                SetAttributeValue( "Image", file.Guid.ToString() );
                SaveAttributeValues();
            }
        }

        protected void img_ImageRemoved( object sender, Rock.Web.UI.Controls.ImageUploaderEventArgs e )
        {
            SetAttributeValue( "Image", "" );
        }
    }
}