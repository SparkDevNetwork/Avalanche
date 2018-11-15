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
using Rock;
using Rock.Field;
using Rock.Field.Types;
using Rock.Web.Cache;

namespace Avalanche.Field.Converters
{
    [ConvertForFieldType( typeof( TextFieldType ) )]
    [ConvertForFieldType( typeof( EmailFieldType ) )]
    [ConvertForFieldType( typeof( UrlLinkFieldType ) )]
    [ConvertForFieldType( typeof( CurrencyFieldType ) )]
    [ConvertForFieldType( typeof( DecimalFieldType ) )]
    [ConvertForFieldType( typeof( PhoneNumberFieldType ) )]
    public class TextFieldTypeConverter : FieldTypeConverter
    {
        public override FormElementItem Convert( IFieldType fieldType, AttributeCache attribute )
        {
            var element = new FormElementItem()
            {
                Type = FormElementType.Entry,
                Keyboard = Keyboard.Text,
                Options = new Dictionary<string, string>()
            };

            if ( fieldType is TextFieldType )
            {
                var qualifiers = attribute.QualifierValues;
                if ( qualifiers.ContainsKey( "ispassword" ) && qualifiers["ispassword"].Value.AsBoolean() == true )
                {
                    element.Attributes.Add( "IsPassword", "true" );
                }
            }
            else if ( fieldType is EmailFieldType )
            {
                element.Keyboard = Keyboard.Email;
            }
            else if ( fieldType is UrlLinkFieldType )
            {
                element.Keyboard = Keyboard.Url;
            }
            else if ( fieldType is CurrencyFieldType || fieldType is DecimalFieldType )
            {
                element.Keyboard = Keyboard.Numeric;
            }
            else if (fieldType is PhoneNumberFieldType )
            {
                element.Keyboard = Keyboard.Telephone;
            }

            return element;
        }
    }
}
