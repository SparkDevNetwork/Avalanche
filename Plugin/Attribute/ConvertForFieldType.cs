using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Attribute
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
    public class ConvertForFieldType : System.Attribute
    {
        public string FieldTypeName { get; private set; }
        public ConvertForFieldType( Type fieldType ) => FieldTypeName = fieldType.FullName;
    }
}