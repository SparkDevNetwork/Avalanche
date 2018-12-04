using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Attribute;
using Avalanche.Field;
using Avalanche.Field.Converters;

namespace Avalanche.Startup
{
    public partial class LoadFieldTypeConverters : Rock.Utility.IRockOwinStartup
    {
        public int StartupOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnStartup( global::Owin.IAppBuilder app )
        {

            foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                try
                {

                    foreach ( var type in assembly.GetTypes() )
                    {
                        try
                        {
                            var customAttributes = type.GetCustomAttributes( typeof( ConvertForFieldType ), true );
                            foreach ( var attribute in customAttributes as ConvertForFieldType[] )
                            {
                                FieldTypeExtensions.FieldTypeConverters[attribute.FieldTypeName] = Activator.CreateInstance( type ) as FieldTypeConverter;
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
    }
}
