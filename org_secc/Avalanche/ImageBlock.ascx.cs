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
    public partial class ImageBlock : RockBlock, IMobileResource
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "Image" ) ) )
            {
                iImage.ImageUrl = string.Format( "{0}/GetImage.ashx?guid={1}", GlobalAttributesCache.Value( "InternalApplicationRoot" ), GetAttributeValue( "Image" ) );
            }
        }

        public MobileBlock GetMobile()
        {
            var attributes = new Dictionary<string, string>();

            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "Image" ) ) )
            {
                attributes.Add( "Source", string.Format( "{0}/GetImage.ashx?guid={1}", GlobalAttributesCache.Value( "InternalApplicationRoot" ), GetAttributeValue( "Image" ) ) );
            }
            if ( new List<string> { "0", "1", "2" }.Contains( GetAttributeValue( "Aspect" ) ) )
            {
                attributes.Add( "Aspect", GetAttributeValue( "Aspect" ) );
            }


            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ImageBlock",
                Body = attributes
            };
        }

        public Dictionary<string, string> HandlePostback( Dictionary<string, string> Body )
        {
            return Body;
        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
            Response.Redirect( "/page/" + GetAttributeValue( "PageNumber" ) );
        }
    }
}