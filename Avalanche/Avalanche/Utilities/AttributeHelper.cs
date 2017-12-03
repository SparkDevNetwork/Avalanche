using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public static class AttributeHelper
    {
        public static void ApplyTranslation( object obj, Dictionary<string, string> attributes )
        {
            foreach ( var attribute in attributes )
            {
                if ( string.IsNullOrWhiteSpace( attribute.Value ) )
                {
                    continue;
                }

                var property = obj.GetType().GetProperty( attribute.Key );
                if ( property == null )
                {
                    continue;
                }

                if ( property.PropertyType == typeof( string ) )
                {
                    property.SetValue( obj, attribute.Value );
                }
                else if ( property.PropertyType == typeof( int ) )
                {
                    int value = 0;
                    int.TryParse( attribute.Value, out value );
                    property.SetValue( obj, value );
                }
                else if ( property.PropertyType == typeof( Thickness ) )
                {
                    property.SetValue( obj, new ThicknessTypeConverter().ConvertFromInvariantString( attribute.Value ) );
                }
                else if ( property.PropertyType == typeof( Color ) )
                {
                    property.SetValue( obj, new ColorTypeConverter().ConvertFromInvariantString( attribute.Value ) );
                }
            }
        }
    }
}
