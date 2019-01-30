using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Blocks;
using Avalanche.Interfaces;
using Xamarin.Forms;

namespace AutoDocumenter
{
    class Program
    {
        static void Main( string[] args )
        {

            var inteface = typeof( IRenderable );
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( s => s.GetTypes() )
                .Where( p => inteface.IsAssignableFrom( p )
                            && p.Name != typeof( IRenderable ).Name )
                .ToList();

            foreach ( var type in types )
            {
                Console.WriteLine( "**" + type.FullName + "**" );
                var properties = Type.GetType( type.FullName ).GetProperties();
                foreach ( var property in properties.Where( p => p.CanWrite ) )
                {
                    var info = GetInformation( property.PropertyType );
                    if ( !string.IsNullOrWhiteSpace( info ) )
                    {
                        Console.WriteLine( "* " + property.Name + " " + info );
                    }
                }
                Console.WriteLine( "" );
            }
            Console.ReadKey();
        }

        public static Dictionary<string, string> typeInformation = new Dictionary<string, string>
        {
            { typeof (string).Name, "[string](https://github.com/SparkDevNetwork/Avalanche/wiki/String)"},
            { typeof (int).Name, "[int](https://github.com/SparkDevNetwork/Avalanche/wiki/Int)"},
            { typeof (ImageSource).Name, "[ImageSource](https://github.com/SparkDevNetwork/Avalanche/wiki/ImageSource)"},
            { typeof (bool).Name,"[bool](https://github.com/SparkDevNetwork/Avalanche/wiki/Bool)"},
            {typeof(Accelerator).Name,"[Accelerator](https://github.com/SparkDevNetwork/Avalanche/wiki/Accelerator)"},
            {typeof(Binding).Name,"[Binding](https://github.com/SparkDevNetwork/Avalanche/wiki/Binding)"},
            {typeof(Rectangle).Name, "[Rectangle](https://github.com/SparkDevNetwork/Avalanche/wiki/Rectangle)"},
            {typeof(Color).Name, "[Color](https://github.com/SparkDevNetwork/Avalanche/wiki/Color)"},
            {typeof(Constraint).Name, "[Constraint](https://github.com/SparkDevNetwork/Avalanche/wiki/Constraint)"},
            {typeof(Font).Name, "[Font](https://github.com/SparkDevNetwork/Avalanche/wiki/Font)"},
            {typeof(GridLength).Name,"[GridLength](https://github.com/SparkDevNetwork/Avalanche/wiki/GridLength)"},
            {typeof(Keyboard).Name, "[Keyboard](https://github.com/SparkDevNetwork/Avalanche/wiki/Keyboard)"},
            {typeof(List<string>).Name, "[List of Strings](https://github.com/SparkDevNetwork/Avalanche/wiki/String)"},
            {typeof(LayoutOptions).Name, "[LayoutOptions](https://github.com/SparkDevNetwork/Avalanche/wiki/LayoutOptions)"},
            {typeof(Point).Name,"[Point](https://github.com/SparkDevNetwork/Avalanche/wiki/Point)"},
            {typeof(double).Name, "[double](https://github.com/SparkDevNetwork/Avalanche/wiki/Double)"},
            {typeof(Thickness).Name, "[Thickness](https://github.com/SparkDevNetwork/Avalanche/wiki/Thickness)"},
            {typeof(Type).Name,"[Type](https://github.com/SparkDevNetwork/Avalanche/wiki/Type)"},
            {typeof(Uri).Name,"[Uri](https://github.com/SparkDevNetwork/Avalanche/wiki/Uri)"},
            {typeof(WebViewSource).Name,"[WebViewSource](https://github.com/SparkDevNetwork/Avalanche/wiki/WebViewSource)"},
        };

        private static string GetInformation( Type propertyType )
        {
            if ( typeInformation.ContainsKey( propertyType.Name ) )
            {
                return typeInformation[propertyType.Name];
            }
            return null;
        }
    }
}
