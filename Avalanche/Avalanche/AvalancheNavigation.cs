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
        public static void GetPage( string resource )
        {
            App.Current.MainPage.Navigation.PushAsync( new MainPage( "page/" + resource ) );
        }
    }
}
