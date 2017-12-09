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

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Image Block" )]
    [Category( "SECC > Avalanche" )]
    [Description( "A button." )]
    [BinaryFileField( Rock.SystemGuid.BinaryFiletype.CONTENT_CHANNEL_ITEM_IMAGE, "Image", "Image to be displayed" )]

    [IntegerField( "Aspect", "Aspect to use. AspectFit: 0, AspectFill:1, Fit:2", false )]
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

            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "Image" ) ) )
            {
                CustomAttributes.Add( "Source", string.Format( "{0}/GetImage.ashx?guid={1}", GlobalAttributesCache.Value( "InternalApplicationRoot" ), GetAttributeValue( "Image" ) ) );
            }
            if ( new List<string> { "0", "1", "2" }.Contains( GetAttributeValue( "Aspect" ) ) )
            {
                CustomAttributes.Add( "Aspect", GetAttributeValue( "Aspect" ) );
            }

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