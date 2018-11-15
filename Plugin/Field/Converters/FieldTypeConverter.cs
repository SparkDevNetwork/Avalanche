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
using Avalanche.Models;
using Rock.Field;
using Rock.Web.Cache;

namespace Avalanche.Field.Converters
{
    public abstract class FieldTypeConverter
    {
        public abstract FormElementItem Convert( IFieldType fieldType, AttributeCache attribute );


        /// <summary>
        /// Encodes the attribute value from a format that Rock understands to a format for Avalanche.
        /// Useful for more complex values such as an address.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string EncodeValue( IFieldType fieldType, AttributeCache attribute, string value, bool isReadOnly = false )
        {
            //Default is to passthrough the value
            return value;
        }


        /// <summary>
        /// Decodes the attribute value from Avalanche into a format Rock can use.
        /// Useful for more complex values such as an address.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string DecodeValue( IFieldType fieldType, AttributeCache attribute, string value )
        {
            //Default is to passthrough the value
            return value;
        }
    }
}
