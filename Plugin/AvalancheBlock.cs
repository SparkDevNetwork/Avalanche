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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Avalanche.Models;
using Rock;
using Rock.Attribute;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace Avalanche
{
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public abstract class AvalancheBlock : RockBlock, IMobileResource
    {
        private UpdatePanel updatePanel;
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

        public abstract MobileBlock GetMobile( string parameter );
        public virtual MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            return new MobileBlockResponse()
            {
                Request = request,
                Response = "",
                TTL = 0
            };
        }

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );

            if ( CurrentUser != null && UserCanAdministrate )
            {
                updatePanel = new UpdatePanel();
                this.Controls.AddAt( 0, updatePanel );

                Panel panel = new Panel
                {
                    CssClass = "avalanche-header"
                };
                updatePanel.ContentTemplateContainer.Controls.Add( panel );

                Literal literal = new Literal
                {
                    Text = BlockCache.Name
                };
                panel.Controls.Add( literal );

                BootstrapButton btnDetails = new BootstrapButton
                {
                    Text = "Show Details",
                    CssClass = "btn btn-default btn-xs pull-right"
                };

                panel.Controls.Add( btnDetails );

                var target = Page.Request.Params["__EVENTTARGET"];
                if ( target != null && target == btnDetails.UniqueID
                    && ( ViewState["ShowDetails"] != null && ( bool ) ViewState["ShowDetails"] == true ) )
                {
                    ViewState["ShowDetails"] = false;
                }
                else if ( ( target != null && target == btnDetails.UniqueID && ( ViewState["ShowDetails"] == null || ( bool ) ViewState["ShowDetails"] == false ) )
                    || ViewState["ShowDetails"] != null && ( bool ) ViewState["ShowDetails"] == true )
                {
                    ShowDetails();
                    btnDetails.Text = "Hide Details";
                }

            }
            else
            {
                this.Visible = false;
            }
        }

        private void ShowDetails()
        {
            ViewState["ShowDetails"] = true;
            Panel details = new Panel()
            {
                CssClass = "avalanche-details form-inline",
            };
            updatePanel.ContentTemplateContainer.Controls.Add( details );

            TextBox tbParameter = new TextBox
            {
                CssClass = "form-control",
            };
            tbParameter.Attributes.Add( "placeholder", "Parameter" );
            details.Controls.Add( tbParameter );
            var parameterValue = Request.Form.GetValues( tbParameter.UniqueID );

            if ( parameterValue != null && parameterValue.Length > 0 )
            {
                tbParameter.Text = parameterValue[0];
            }

            BootstrapButton btnParameter = new BootstrapButton
            {
                Text = "Change Parameter",
                CssClass = "btn btn-default"
            };
            details.Controls.Add( btnParameter );


            BootstrapButton btnRequest = new BootstrapButton
            {
                Text = "Do Postback",
                CssClass = "btn btn-default pull-right"
            };
            details.Controls.Add( btnRequest );

            TextBox tbRequest = new TextBox
            {
                CssClass = "form-control pull-right",
            };
            tbRequest.Attributes.Add( "placeholder", "Request" );
            details.Controls.Add( tbRequest );
            var requestValue = Request.Form.GetValues( tbRequest.UniqueID );

            if ( requestValue != null && requestValue.Length > 0 )
            {
                tbRequest.Text = requestValue[0];
            }


            var target = Page.Request.Params["__EVENTTARGET"];
            if ( target == btnRequest.UniqueID )
            {
                GetPostbackResponse( tbRequest.Text );
            }
            else
            {
                GetMobileBlockData( tbParameter.Text );
            }
        }

        private void GetMobileBlockData( string parameter )
        {
            var mobileBlock = this.GetMobile( parameter );

            Panel pnlMobileBlock = new Panel()
            {
                CssClass = "avalanche-block-details"
            };
            updatePanel.ContentTemplateContainer.Controls.Add( pnlMobileBlock );

            Literal ltBlockType = new Literal()
            {
                Text = "<h4>" + mobileBlock.BlockType + "</h4>"
            };
            pnlMobileBlock.Controls.Add( ltBlockType );

            foreach ( var att in mobileBlock.Attributes )
            {
                Literal ltAttribute = new Literal()
                {
                    Text = string.Format( "<hr><b>{0}:</b><br>{1}", att.Key, att.Value.ScrubHtmlAndConvertCrLfToBr() )
                };
                pnlMobileBlock.Controls.Add( ltAttribute );
            }
        }

        private void GetPostbackResponse( string request )
        {
            var response = HandleRequest( request, new Dictionary<string, string>() );
            Panel pnlMobileBlock = new Panel()
            {
                CssClass = "avalanche-block-details"
            };
            updatePanel.ContentTemplateContainer.Controls.Add( pnlMobileBlock );

            Literal ltRequest = new Literal()
            {
                Text = "<h4> Request: " + response.Request + "</h4><hr>"
            };
            pnlMobileBlock.Controls.Add( ltRequest );

            Literal ltResponse = new Literal()
            {
                Text = response.Response.ScrubHtmlAndConvertCrLfToBr()
            };
            pnlMobileBlock.Controls.Add( ltResponse );
        }
    }
}