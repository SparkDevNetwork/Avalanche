using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Lava;
using ReverseMarkdown;

namespace Avalanche.Lava
{
    public static class CustomFilters
    {
        public static string HtmlToMarkdown( string input )
        {
            var converter = new Converter();
            var markdown = converter.Convert( input );
            return markdown;
        }
    }
}
