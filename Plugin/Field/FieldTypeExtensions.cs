using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;
using Rock.Web.Cache;

namespace Avalanche.Field
{
    public static class FieldTypeExtensions
    {
        public static FormElementItem GetMobileElement( this Rock.Field.FieldType fieldType )
        {
            return new FormElementItem();
        }

        public static FormElementItem GetMobileElement( this Rock.Field.Types.TextFieldType fieldType )
        {
            
            var formElement = new FormElementItem()
            {
                Type = FormElementType.Entry,
                Keyboard = Keyboard.Text,
            };

            return formElement;
        }
    }
}
