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
using System.Text;
using Avalanche;
using Avalanche.Models;
using Avalanche.Views;
using Xamarin.Forms;

namespace Avalanche
{
    static class AvalancheNavigation
    {
        public static double YOffSet = 0;
        public static MenuPage Footer = null;

        public static void GetPage( string resource, string parameter = "" )
        {
            App.Navigation.Navigation.PushAsync( new MainPage( "page/" + resource, parameter ) );
        }

        public static void RemovePage()
        {
            App.Navigation.Navigation.PopAsync();
        }

        public async static void ReplacePage( string resource, string argument = "" )
        {
            var page = App.Navigation.Navigation.NavigationStack[App.Navigation.Navigation.NavigationStack.Count - 1];
            await App.Navigation.Navigation.PushAsync( new MainPage( "page/" + resource, argument ) );
            App.Navigation.Navigation.RemovePage( page );
        }
    }
}
