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
    [ConvertForFieldType( typeof( SelectMultiFieldType ) )]

    public class SelectMultiFieldTypeConverter : FieldTypeConverter
    {
        public override FormElementItem Convert( IFieldType fieldType, AttributeCache attribute )
        {
            var element = new FormElementItem()
            {
                Type = FormElementType.CheckboxList,
                Keyboard = Keyboard.Text,
                Options = Helper.GetConfiguredValues( attribute.QualifierValues )
            };

            return element;
        }
    }
}
