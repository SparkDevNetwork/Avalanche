using System;
using System.Collections.Generic;
using System.Text;
using Avalanche;
using Avalanche.Models;
using RestSharp;
using Xamarin.Forms;

namespace Avalanche
{
    static class AvalancheNavigation
    {
        public static void GetPage( string resource, string argument = "" )
        {
            App.Current.MainPage.Navigation.PushAsync( new MainPage( "page/" + resource, argument ) );
        }

        public static void ReplacePage( string resource, string argument = "" )
        {
            if ( App.Current.MainPage.Navigation.NavigationStack.Count > 0 )
            {
                App.Current.MainPage.Navigation.PopAsync();
            }
            App.Current.MainPage.Navigation.PushAsync( new MainPage( "page/" + resource, argument ) );
        }
    }
}
