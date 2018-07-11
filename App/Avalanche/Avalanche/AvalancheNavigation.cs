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
using System.Threading.Tasks;
using Avalanche;
using Avalanche.Models;
using Avalanche.Utilities;
using Avalanche.Views;
using Xamarin.Forms;

namespace Avalanche
{
    static class AvalancheNavigation
    {
        public static double YOffSet = 0;
        public static MenuPage Footer = null;
        public static bool AllowResize = false;
        public static Thickness SafeInset = new Thickness( 0 );
        private static ObservableResource<RckipidToken> rckipidResource;

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

        public static void HandleActionItem( Dictionary<string, string> Attributes )
        {
            if ( !Attributes.ContainsKey( "ActionType" ) || Attributes["ActionType"] == "0" )
            {
                return;
            }

            var resource = "";
            if ( Attributes.ContainsKey( "Resource" ) )
            {
                resource = Attributes["Resource"];
            }

            var parameter = "";
            if ( Attributes.ContainsKey( "Parameter" ) )
            {
                parameter = Attributes["Parameter"];
            }

            if ( Attributes["ActionType"] == "1" && !string.IsNullOrWhiteSpace( resource ) ) //push new page
            {
                AvalancheNavigation.GetPage( Attributes["Resource"], parameter );
            }
            else if ( Attributes["ActionType"] == "2" && !string.IsNullOrWhiteSpace( resource ) ) //replace
            {
                AvalancheNavigation.ReplacePage( Attributes["Resource"], parameter );
            }
            else if ( Attributes["ActionType"] == "3" ) //pop page
            {
                AvalancheNavigation.RemovePage();
            }
            else if ( Attributes["ActionType"] == "4" && !string.IsNullOrWhiteSpace( resource ) )
            {
                if ( !string.IsNullOrWhiteSpace( parameter ) && parameter == "1" )
                {
                    if ( resource.Contains( "?" ) )
                    {
                        resource += "&rckipid=" + GetRckipid();
                    }
                    else
                    {
                        resource += "?rckipid=" + GetRckipid();
                    }
                }
                else if ( resource.Contains( "{{rckipid}}" ) )
                {
                    resource.Replace( "{{rckipid}}", GetRckipid() );
                }
                Device.OpenUri( new Uri( resource ) );
            }
        }

        public static string GetRckipid()
        {
            var token = "";
            if ( App.Current.Properties.ContainsKey( "rckipid" ) )
            {
                token = App.Current.Properties["rckipid"] as string;
            }
            RequestNewRckipid();
            return token;
        }

        public async static void UpdateRckipid()
        {
            if ( App.Current.Properties.ContainsKey( "rckipid_expiration" ) )
            {
                DateTime? expiration = App.Current.Properties["rckipid_expiration"] as DateTime?;
                var now = TimeZoneInfo.ConvertTime( DateTime.UtcNow, TimeZoneInfo.Local );
                if ( expiration != null && expiration > now )
                {
                    RequestNewRckipid();
                }
            }
            else if ( !string.IsNullOrWhiteSpace( await RockClient.GetAccessToken() ) )
            {
                RequestNewRckipid();
            }
        }

        public static void RequestNewRckipid()
        {
            rckipidResource = new ObservableResource<RckipidToken>();
            rckipidResource.PropertyChanged += RckipidResource_HandleResponse;
            RockClient.GetResource( rckipidResource, "/api/avalanche/token", true );
        }

        private static void RckipidResource_HandleResponse( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            if ( rckipidResource.Resource != null )
            {
                App.Current.Properties["rckipid"] = rckipidResource.Resource.Token;
                App.Current.Properties["rckipid_expiration"] = rckipidResource.Resource.Expiration.AddHours( -12 );
                App.Current.SavePropertiesAsync();
            }
        }
    }
}
