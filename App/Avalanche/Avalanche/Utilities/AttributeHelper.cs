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
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public static class AttributeHelper
    {
        private static string[] trueStrings = new string[] { "true", "yes", "t", "y", "1" };

        public static bool IsTrue( string s )
        {
            return ( trueStrings.Contains( s.ToLower() ) );
        }

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
            // because of the FFLoadingCache has a slightly different converter
            // {typeof(ImageSource).Name, new ImageSourceConverter() }, 
            {typeof(List<string>).Name, new ListStringTypeConverter() },
            {typeof(LayoutOptions).Name, new LayoutOptionsConverter() },
            {typeof(Point).Name, new PointTypeConverter() },
            {typeof(double).Name, new FontSizeConverter() },
            {typeof(Thickness).Name, new ThicknessTypeConverter() },
            {typeof(Type).Name, new TypeTypeConverter() },
            {typeof(Uri).Name, new Xamarin.Forms.UriTypeConverter() },
            {typeof(WebViewSource).Name, new  WebViewSourceTypeConverter() },
            { "CustomFontFamily", new CustomFontTypeConverter() }
        };


        public static void ApplyTranslation( object obj, Dictionary<string, string> attributes )
        {
            if ( attributes == null )
            {
                return;
            }

            foreach ( var attribute in attributes )
            {
                if ( string.IsNullOrWhiteSpace( attribute.Value ) )
                {
                    continue;
                }

                var property = obj.GetType().GetProperty( attribute.Key );
                if ( property == null || !property.CanWrite )
                {
                    continue;
                }

                if ( property.PropertyType == typeof( string ) )
                {
                    if ( property.Name == "FontFamily" )
                    {
                        property.SetValue( obj, typeConverters["CustomFontFamily"].ConvertFromInvariantString( attribute.Value ) );
                    }
                    else
                    {
                        property.SetValue( obj, attribute.Value );
                    }
                }
                else if ( property.PropertyType == typeof( int ) )
                {
                    int.TryParse( attribute.Value, out int value );
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
                else if ( property.PropertyType == typeof( bool ) )
                {
                    if ( trueStrings.Contains( attribute.Value.ToLower() ) )
                    {
                        property.SetValue( obj, true );
                    }
                    else
                    {
                        property.SetValue( obj, false );
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
