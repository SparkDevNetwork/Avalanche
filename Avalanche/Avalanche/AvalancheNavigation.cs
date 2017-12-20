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

        public static void RemovePage()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        public async static void ReplacePage( string resource, string argument = "" )
        {
            var page = App.Current.MainPage.Navigation.NavigationStack[App.Current.MainPage.Navigation.NavigationStack.Count - 1];
            await App.Current.MainPage.Navigation.PushAsync( new MainPage( "page/" + resource, argument ) );
            App.Current.MainPage.Navigation.RemovePage( page );
        }
    }
}
