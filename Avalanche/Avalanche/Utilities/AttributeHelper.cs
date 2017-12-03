using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public static class AttributeHelper
    {
        private static Dictionary<string, TypeConverter> typeConverters = new Dictionary<string, TypeConverter>
        {
            {typeof(Accelerator).Name, new AcceleratorTypeConverter() },
            {typeof(Binding).Name, new BindingTypeConverter() },
            {typeof(Rectangle).Name, new BoundsTypeConverter() },
            {typeof(Color).Name, new ColorTypeConverter()},
            {typeof(Constraint).Name, new  ConstraintTypeConverter()},
            {typeof(Font).Name, new FontTypeConverter() },
            {typeof(GridLength).Name, new GridLengthTypeConverter() },
            {typeof(Keyboard).Name, new KeyboardTypeConverter() },
            // Image source converter added as a comment to say this conversion is handled specially
            // because of the FFLoadingCache has a slightly different image source type
            // {typeof(ImageSource).Name, new ImageSourceConverter() }, 
            {typeof(List<string>).Name, new ListStringTypeConverter() },
            {typeof(LayoutOptions).Name, new LayoutOptionsConverter() },
            {typeof(Point).Name, new PointTypeConverter() },
            {typeof(double).Name, new FontSizeConverter() },
            {typeof(Thickness).Name, new ThicknessTypeConverter() },
            {typeof(Type).Name, new TypeTypeConverter() },
            {typeof(Uri).Name, new Xamarin.Forms.UriTypeConverter() },
            {typeof(WebViewSource).Name, new  WebViewSourceTypeConverter()}
        };


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
                else if ( property.PropertyType == typeof( ImageSource ) )
                {
                    if ( obj is FFImageLoading.Forms.CachedImage )
                    {
                        property.SetValue( obj, new FFImageLoading.Forms.ImageSourceConverter().ConvertFromInvariantString( attribute.Value ) );
                    }
                    else
                    {
                        property.SetValue( obj, new ImageSourceConverter().ConvertFromInvariantString( attribute.Value ) );
                    }
                }
                else if ( typeConverters.ContainsKey( property.PropertyType.Name ) )
                {
                    property.SetValue( obj, typeConverters[property.PropertyType.Name].ConvertFromInvariantString( attribute.Value ) );
                }
            }
        }
    }
}
