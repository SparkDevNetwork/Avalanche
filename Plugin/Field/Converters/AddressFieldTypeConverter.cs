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
using System.Text;
using System.Threading.Tasks;
using Avalanche.Attribute;
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Data;
using Rock.Field;
using Rock.Field.Types;
using Rock.Model;
using Rock.Web.Cache;

namespace Avalanche.Field.Converters
{
    [ConvertForFieldType( typeof( AddressFieldType ) )]

    public class AddressFieldTypeConverter : FieldTypeConverter
    {
        public override FormElementItem Convert( IFieldType fieldType, AttributeCache attribute )
        {
            var dfState = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.LOCATION_ADDRESS_STATE.AsGuid() );
            var statesList = dfState.DefinedValues.Select( dv => dv.Value ).OrderBy( dv => dv ).ToList();
            var states = string.Join( ",", statesList );
            var globalAttributesCache = GlobalAttributesCache.Get();
            var defaultState = globalAttributesCache.OrganizationState;
            var attributes = new Dictionary<string, string>
            {
                { "States", states },
                { "DefaultState", defaultState}
            };

            var element = new FormElementItem()
            {
                Type = FormElementType.Address,
                Keyboard = Keyboard.Text,
                Attributes = attributes
            };

            return element;
        }

        public override string EncodeValue( IFieldType fieldType, AttributeCache attribute, string value, bool isReadOnly )
        {
            RockContext rockContext = new RockContext();
            LocationService locationService = new LocationService( rockContext );
            var location = locationService.Get( value.AsGuid() );
            if ( location == null )
            {
                return JsonConvert.SerializeObject( new Dictionary<string, string>() );
            }

            if ( isReadOnly )
            {
                return location.FormattedAddress;
            }

            var locationData = new Dictionary<string, string>
            {
                {"Street1", location.Street1 },
                { "Street2", location.Street2 },
                { "City", location.City },
                { "State", location.State },
                { "PostalCode", location.PostalCode }
            };
            return JsonConvert.SerializeObject( locationData );
        }

        public override string DecodeValue( IFieldType fieldType, AttributeCache attribute, string value )
        {
            Dictionary<string, string> locationData = new Dictionary<string, string>();
            try
            {
                locationData = JsonConvert.DeserializeObject<Dictionary<string, string>>( value );
            }
            catch
            {
                return "";
            }
            RockContext rockContext = new RockContext();
            var globalAttributesCache = GlobalAttributesCache.Get();
            LocationService locationService = new LocationService( rockContext );
            var location = locationService.Get( locationData["Street1"], locationData["Street2"], locationData["City"], locationData["State"], locationData["PostalCode"], globalAttributesCache.OrganizationCountry );
            if ( location != null )
            {
                return location.Guid.ToString();
            }
            return "";
        }
    }
}
