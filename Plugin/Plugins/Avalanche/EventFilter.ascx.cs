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
using Avalanche.Attribute;
using Newtonsoft.Json;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Event Filter" )]
    [Category( "Avalanche" )]
    [Description( "Creates a form for event filtering " )]

    [ActionItemField( "Action Item", "Action to take upon changing the filter", false )]

    public partial class EventFilter : AvalancheBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbLabel.Text = "Event Filter";
        }

        public override MobileBlock GetMobile( string parameter )
        {
            var form = new List<FormElementItem>();
            var selectedCampus = "0";
            var selectedMinistry = "0";
            if ( !string.IsNullOrWhiteSpace( parameter ) )
            {
                var split = parameter.Split( '|' );
                if ( split.Length > 0 )
                {
                    selectedCampus = split[0];
                }
                if ( split.Length > 1 )
                {
                    selectedMinistry = split[1];
                }
            }


            var campuses = CampusCache.All().ToDictionary( c => c.Id.ToString(), c => c.Name );
            campuses.Add( "0", "All Campuses" );

            var campusDdl = new FormElementItem
            {
                Type = FormElementType.Picker,
                Key = "campus",
                Label = "Events From",
                Options = campuses,
                Value = selectedCampus,
                AutoPostBack = true
            };
            form.Add( campusDdl );

            var dv = new Dictionary<string, string>();
            var dvCache = DefinedTypeCache.Read( 16 ).DefinedValues;
            foreach ( var value in dvCache )
            {
                if ( value.GetAttributeValue( "PublicFilter" ).AsBoolean() )
                {
                    dv[value.Id.ToString()] = value.Value;
                }
            }
            dv.Add( "0", "All Ministries" );

            var ministryDdl = new FormElementItem
            {
                Type = FormElementType.Picker,
                Key = "ministry",
                Label = "Select A Ministry",
                Options = dv,
                Value = selectedMinistry,
                AutoPostBack = true
            };
            form.Add( ministryDdl );


            CustomAttributes.Add( "FormElementItems", JsonConvert.SerializeObject( form ) );
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.FormBlock",
                Attributes = CustomAttributes
            };
        }
        Dictionary<string, string> body;
        public override MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                               CustomAttributes,
                                               CurrentPerson,
                                               AvalancheUtilities.GetMergeFields( CurrentPerson ) );

            var response = new FormResponse
            {
                Success = true,
            };

            if ( CustomAttributes.ContainsKey( "ActionType" ) && CustomAttributes["ActionType"] != "0" )
            {
                response.ActionType = CustomAttributes["ActionType"];
            }

            if ( CustomAttributes.ContainsKey( "Resource" ) )
            {
                response.Resource = CustomAttributes["Resource"];
            }

            response.Parameter = Body["campus"] + "|" + Body["ministry"];

            var form = new List<FormElementItem>();

            var campuses = CampusCache.All().ToDictionary( c => c.Id.ToString(), c => c.Name );
            campuses.Add( "0", "All Campuses" );

            var campusDdl = new FormElementItem
            {
                Type = FormElementType.Picker,
                Key = "campus",
                Label = "Events From",
                Options = campuses,
                Value = Body["campus"],
                AutoPostBack = true
            };
            form.Add( campusDdl );

            var dv = new Dictionary<string, string>();
            var dvCache = DefinedTypeCache.Read( 16 ).DefinedValues;
            foreach ( var value in dvCache )
            {
                if ( value.GetAttributeValue( "PublicFilter" ).AsBoolean() )
                {
                    dv[value.Id.ToString()] = value.Value;
                }
            }
            dv.Add( "0", "All Ministries" );

            var ministryDdl = new FormElementItem
            {
                Type = FormElementType.Picker,
                Key = "ministry",
                Label = "Select A Ministry",
                Options = dv,
                Value = Body["ministry"],
                AutoPostBack = true
            };
            form.Add( ministryDdl );

            response.FormElementItems = form;

            return new MobileBlockResponse()
            {
                Request = request,
                Response = JsonConvert.SerializeObject( response ),
                TTL = 0
            };
        }

        private string GetItem( string item )
        {
            if ( body.ContainsKey( item ) && body[item] != null )
            {
                return body[item];
            }
            return "";
        }
    }
}