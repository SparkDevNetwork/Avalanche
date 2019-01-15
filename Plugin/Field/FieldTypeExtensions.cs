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
using Avalanche.Field.Converters;
using Avalanche.Models;
using Rock.Attribute;
using Rock.Field;
using Rock.Field.Types;
using Rock.Web.Cache;

namespace Avalanche.Field
{
    public static class FieldTypeExtensions
    {
        public static Dictionary<string, FieldTypeConverter> FieldTypeConverters = new Dictionary<string, FieldTypeConverter>();

        public static FormElementItem GetMobileElement( this IFieldType fieldType, AttributeCache attribute )
        {
            var fieldTypeName = fieldType.GetType().FullName;
            if ( FieldTypeConverters.ContainsKey( fieldTypeName ) )
            {
                return FieldTypeConverters[fieldTypeName].Convert( fieldType, attribute );
            }

            return new FormElementItem()
            {
                Type = FormElementType.Entry,
                Keyboard = Keyboard.Text,
            };
        }

        public static string EncodeAttributeValue( this IFieldType fieldType, AttributeCache attribute, string value, bool isReadOnly = true )
        {
            var fieldTypeName = fieldType.GetType().FullName;
            if ( FieldTypeConverters.ContainsKey( fieldTypeName ) )
            {
                return FieldTypeConverters[fieldTypeName].EncodeValue( fieldType, attribute, value, isReadOnly );
            }
            return "";
        }

        public static string DecodeAttributeValue( this IFieldType fieldType, AttributeCache attribute, string value )
        {
            var fieldTypeName = fieldType.GetType().FullName;
            if ( FieldTypeConverters.ContainsKey( fieldTypeName ) )
            {
                return FieldTypeConverters[fieldTypeName].DecodeValue( fieldType, attribute, value );
            }
            return "";
        }

    }
}
