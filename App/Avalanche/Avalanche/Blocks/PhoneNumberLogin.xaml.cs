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
using System.Threading.Tasks;
using Avalanche.Interfaces;
using Avalanche.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class PhoneNumberLogin : ContentView, IRenderable, IHasBlockMessenger
    {
        private string phoneNumber = "";
        public PhoneNumberLogin()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        public View Render()
        {
            if ( Attributes.ContainsKey( "FontSize" ) && !string.IsNullOrWhiteSpace( Attributes["FontSize"] ) )
            {
                ePhoneNumber.FontSize = Convert.ToDouble( Attributes["FontSize"] );
                btnPhoneNumber.FontSize = Convert.ToDouble( Attributes["FontSize"] );
                ePin.FontSize = Convert.ToDouble( Attributes["FontSize"] );
                btnPin.FontSize = Convert.ToDouble( Attributes["FontSize"] );
            }


            MessageHandler.Response += MessageHandler_Response;
            return this;
        }

        private async void MessageHandler_Response( object sender, Models.MobileBlockResponse e )
        {
            var r = e.Response.Split( new char[] { '|' } );
            if ( r[0] == "1" )
            {
                slLoading.IsVisible = false;
                slPin.IsVisible = true;
            }
            else
            {
                slLoading.IsVisible = false;
                slPin.IsVisible = false;
                slPhoneNumber.IsVisible = true;
                if ( Attributes.ContainsKey( "HelpUrl" ) && !string.IsNullOrWhiteSpace( Attributes["HelpUrl"] ) )
                {
                    if ( await App.Current.MainPage.DisplayAlert( "We're sorry.", r[1], "Resolve Problem", "Ok" ) )
                    {
                        Device.OpenUri( new Uri( Attributes["HelpUrl"] ) );
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert( "We're sorry.", r[1], "Ok" );
                }
            }
        }

        private void btnPhoneNumber_Clicked( object sender, EventArgs e )
        {
            if ( ePhoneNumber.Text.Length != 10 )
            {
                App.Current.MainPage.DisplayAlert( "Uh Oh", "Please enter a 10 digit phone number.", "Ok" );
                return;
            }
            phoneNumber = ePhoneNumber.Text;
            slPhoneNumber.IsVisible = false;
            slLoading.IsVisible = true;
            MessageHandler.Get( phoneNumber );
        }

        private async void btnPin_Clicked( object sender, EventArgs e )
        {
            slPin.IsVisible = false;
            slLoading.IsVisible = true;

            var response = await RockClient.LogIn( "__PHONENUMBER__+1" + phoneNumber, ePin.Text );
            switch ( response )
            {
                case LoginResponse.Error:
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "There was an issue with your log-in attempt. Please try again later. (Sorry)", "OK" );
                    slPin.IsVisible = true;
                    slLoading.IsVisible = false;
                    break;
                case LoginResponse.Failure:
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "Phone number and pin did not match. Please try again.", "OK" );
                    slPin.IsVisible = true;
                    slLoading.IsVisible = false;
                    break;
                case LoginResponse.Success:
                    AvalancheNavigation.Footer = null;
                    App.Current.MainPage = new AvalanchePage();
                    AvalancheNavigation.RequestNewRckipid();
                    FCMHelper.RegisterFCMToken();
                    break;
                default:
                    break;
            }
        }
    }
}