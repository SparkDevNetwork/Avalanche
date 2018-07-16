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
using Avalanche.Utilities;
using Avalanche;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Avalanche.Interfaces;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class Login : ContentView, IRenderable
    {
        public Login()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {

            if ( Attributes.ContainsKey( "FontSize" ) && !string.IsNullOrWhiteSpace( Attributes["FontSize"] ) )
            {
                username.FontSize = Convert.ToDouble( Attributes["FontSize"] );
                password.FontSize = Convert.ToDouble( Attributes["FontSize"] );
                btnSubmit.FontSize = Convert.ToDouble( Attributes["FontSize"] );
            }

            return this;
        }

        private async void btnSubmit_Clicked( object sender, EventArgs e )
        {
            slForm.IsVisible = false;
            aiActivity.IsRunning = true;
            var response = await RockClient.LogIn( username.Text, password.Text );

            switch ( response )
            {
                case LoginResponse.Error:
                    aiActivity.IsRunning = false;
                    slForm.IsVisible = true;
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "There was an issue with your log-in attempt. Please try again later. (Sorry)", "OK" );
                    break;
                case LoginResponse.Failure:
                    aiActivity.IsRunning = false;
                    slForm.IsVisible = true;
                    App.Current.MainPage.DisplayAlert( "Log-in Error", "Your username or password was incorrect.", "OK" );
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