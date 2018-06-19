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
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System.Linq;
using org.secc.OAuth.Model;
using org.secc.OAuth.Data;

namespace RockWeb.Plugins.Avalanche
{
    /// <summary>
    /// Avalanche configuration
    /// </summary>
    [DisplayName( "Avalanche Configuration" )]
    [Category( "Avalanche > Settings" )]
    [Description( "Configuration settings for Avalanche." )]

    public partial class AvalancheConfiguration : Rock.Web.UI.RockBlock
    {

        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                RockContext rockContext = new RockContext();
                AttributeValueService attributeService = new AttributeValueService( rockContext );
                var homePageAttributeCache = GlobalAttributesCache.Read( rockContext ).Attributes.Where( a => a.Key == "AvalancheHomePage" ).FirstOrDefault();
                var homePageAttribue = attributeService.GetByAttributeIdAndEntityId( homePageAttributeCache.Id, null );
                if ( homePageAttribue != null )
                {
                    var homePageAttribueValue = homePageAttribue.Value;
                    ppHome.SetValue( homePageAttribueValue.AsInteger() );
                }

                var menuPageAttributeCache = GlobalAttributesCache.Read( rockContext ).Attributes.Where( a => a.Key == "AvalancheMenuPage" ).FirstOrDefault();
                var menuPageAttribute = attributeService.GetByAttributeIdAndEntityId( menuPageAttributeCache.Id, null );
                if ( menuPageAttribute != null )
                {
                    var menupageAttributeValue = menuPageAttribute.Value;
                    ppMenu.SetValue( menupageAttributeValue.AsInteger() );
                }
            }
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            RockContext rockContext = new RockContext();
            AttributeValueService attributeValueService = new AttributeValueService( rockContext );

            var homePageAttributeCache = GlobalAttributesCache.Read( rockContext ).Attributes.Where( a => a.Key == "AvalancheHomePage" ).FirstOrDefault();
            var homePageAttribute = attributeValueService.GetByAttributeIdAndEntityId( homePageAttributeCache.Id, null );
            if ( homePageAttribute == null )
            {
                homePageAttribute = new AttributeValue()
                {
                    AttributeId = homePageAttributeCache.Id
                };
                attributeValueService.Add( homePageAttribute );
            }

            homePageAttribute.Value = ppHome.SelectedValue;


            var menuPageAttributeCache = GlobalAttributesCache.Read( rockContext ).Attributes.Where( a => a.Key == "AvalancheMenuPage" ).FirstOrDefault();
            var menuPageAttribute = attributeValueService.GetByAttributeIdAndEntityId( menuPageAttributeCache.Id, null );
            if ( menuPageAttribute == null )
            {
                menuPageAttribute = new AttributeValue()
                {
                    AttributeId = menuPageAttributeCache.Id
                };
                attributeValueService.Add( menuPageAttribute );
            }
            menuPageAttribute.Value = ppMenu.SelectedValue;

            rockContext.SaveChanges();
            NavigateToParentPage();
        }

        protected void btnBack_Click( object sender, EventArgs e )
        {
            NavigateToParentPage();
        }
    }
}
