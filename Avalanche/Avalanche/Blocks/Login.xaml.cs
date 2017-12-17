using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
using Avalanche;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
                    App.Current.MainPage = new NavigationPage( new Avalanche.MainPage( "home" ) );
                    break;
                default:
                    break;
            }
        }

        private void btnLogout_Clicked( object sender, EventArgs e )
        {
            RockClient.Logout();
        }

        private void btnForgot_Clicked( object sender, EventArgs e )
        {
            Device.OpenUri( new Uri( "http://secc.org/login" ) );
        }
    }
}