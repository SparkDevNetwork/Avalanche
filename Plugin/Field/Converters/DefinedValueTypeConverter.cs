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
using System.Linq;
using Avalanche.Attribute;
using Avalanche.Models;
using Rock;
using Rock.Field;
using Rock.Field.Types;
using Rock.Web.Cache;

namespace Avalanche.Field.Converters
{
    [ConvertForFieldType( typeof( DefinedValueFieldType ) )]

    public class DefinedValueTypeConverter : FieldTypeConverter
    {
        public override FormElementItem Convert( IFieldType fieldType, AttributeCache attribute )
        {

            string INCLUDE_INACTIVE_KEY = "includeInactive";




            var element = new FormElementItem()
            {
                Type = FormElementType.Picker,
                Keyboard = Keyboard.Text,
            };

            var definedTypeValue = attribute.QualifierValues.GetValueOrNull( "definedtype" );
            var definedType = DefinedTypeCache.Get( definedTypeValue.AsInteger() );
            if ( definedType != null )
            {
                var values = definedType.DefinedValues;
                if ( !attribute.QualifierValues.GetValueOrNull( "includeInactive" ).AsBoolean() )
                {
                    values = values.Where( a => a.IsActive ).ToList();
                }

                if ( definedTypeValue != null )
                {
                    element.Options = values.ToDictionary( dv => dv.Guid.ToString(), dv => dv.Value );
                }

                var allowmultiple = attribute.QualifierValues.GetValueOrNull( "allowmultiple" );
                if ( allowmultiple != null && allowmultiple.AsBoolean() == true )
                {
                    element.Type = FormElementType.CheckboxList;
                }
            }

            return element;
        }
    }
}
