using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Avalanche.Models;
using Rock;
using Rock.Attribute;
using Rock.Web.UI;

namespace Avalanche
{
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public abstract class AvalancheBlock : RockBlock, IMobileResource
    {
        private Dictionary<string, string> _customAtributes;
        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                if ( _customAtributes == null )
                {
                    _customAtributes = new Dictionary<string, string>();
                    var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
                    foreach ( var item in customs )
                    {
                        _customAtributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
                    }
                }
                return _customAtributes;
            }
        }

        public abstract MobileBlock GetMobile( string arg );
        public virtual MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body )
        {
            return new MobileBlockResponse()
            {
                Arg = resource,
                Response = "",
                TTL = 0
            };
        }

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
            var mobileBlock = this.GetMobile( "" );
            var atts = string.Join( "<br>", mobileBlock.Attributes.Select( x => x.Key + ": " + x.Value ) );
            HtmlGenericControl div = new HtmlGenericControl( "div" );
            div.InnerHtml = string.Format( @"
            <details style=""margin:0px 0px -18px -18px"">
                <summary><i class='fa fa-info-circle'></i></summary>
                <div class=""mobileBlockInformation"">
                    <div class=""mobileBlockInformationHeader"">
                        <b>{0}</b>
                    </div>
                    <div style=""padding:3px 20px"">
                        {1}
                    </div>
                </div>
            </details>",
            mobileBlock.BlockType,
            atts
            );
            this.Controls.AddAt( 0, div );
        }
    }
}