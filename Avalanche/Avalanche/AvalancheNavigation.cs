using System;
using System.Collections.Generic;
using System.Text;
using org.secc.Avalanche;
using org.secc.Avalanche.Models;
using RestSharp;
using Xamarin.Forms;

namespace Avalanche
{
    static class AvalancheNavigation
    {
        public static void GetPage( string resource )
        {
            App.Current.MainPage = new MainPage(resource);
        }
    }
}
