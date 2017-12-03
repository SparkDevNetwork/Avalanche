using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace Avalanche.PageAttributes
{
    public class BackgroundColor : IPageAttribute
    {
        public void Modify( ContentPage contentPage, string value )
        {
            if ( Regex.IsMatch( value, "^#(?:[0-9a-fA-F]{3}){1,2}$" ) )
            {
                contentPage.BackgroundColor = Color.FromHex( value );
            }
        }
    }
}
