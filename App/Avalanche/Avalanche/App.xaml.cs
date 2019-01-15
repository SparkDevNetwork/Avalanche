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
using System.Threading.Tasks;
using Avalanche.Models;
using Avalanche.Utilities;
using Plugin.MediaManager;
using Xamarin.Forms;

namespace Avalanche
{
    public partial class App : Application
    {
        private bool active = true;

        public static NavigationPage Navigation = null;
        public App()
        {
            if ( App.Current.Properties.ContainsKey( "SecondRun" ) )
            {
                DoAppStartup();
            }
            else
            {
                MainPage = new AvalanchePage();
            }
            InitializeComponent();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            RockClient.CreateDatabase();
            MainPage = new AvalanchePage();
            Task.Factory.StartNew( new Action( backgroundAction ) );
        }

        private void DoAppStartup()
        {
            //The purpose of this loading section is to improve image load time on the home page
            //Getting the images into memory can take some time, and if other things start
            //Running first, such as the database or HTTP requests, this can get started very late
            //Causing an unpleasent first experience.
            var sl = new StackLayout();
            MainPage = new ContentPage()
            {
                Content = sl,
                BackgroundColor = Color.Black
            };
            sl.Children.Add( new ActivityIndicator
            {
                IsRunning = true,
                Color = Color.White,
                Margin = new Thickness( 0, 30, 0, 0 )
            } );

            if ( App.Current.Properties.ContainsKey( "PreloadImages" )
                && !string.IsNullOrWhiteSpace( App.Current.Properties["PreloadImages"].ToString() ) )
            {
                foreach ( var imageSource in App.Current.Properties["PreloadImages"].ToString().Split( "," ) )
                {
                    sl.Children.Add( new FFImageLoading.Forms.CachedImage { Source = imageSource, Opacity = 0 } );
                }
            }

            var finalImage = new FFImageLoading.Forms.CachedImage { Opacity = 0 };
            finalImage.Finish += Startup_Finish;
            finalImage.Source = "pixel.png";
            sl.Children.Add( finalImage );
        }

        private void Startup_Finish( object sender, FFImageLoading.Forms.CachedImageEvents.FinishEventArgs e )
        {
            Device.BeginInvokeOnMainThread( () =>
            {
                MainPage = new AvalanchePage();
            } );
        }

        private async void backgroundAction()
        {
            active = true;
            while ( active )
            {
                AvalancheNavigation.UpdateRckipid();
                await Task.Delay( 60000 );
            }
        }

        protected override void OnStart()
        {
            var interaction = new Interaction
            {
                Operation = "AppLaunch"
            };
            interaction.Send();
        }

        protected override void OnSleep()
        {
            if ( CrossMediaManager.Current.Status != Plugin.MediaManager.Abstractions.Enums.MediaPlayerStatus.Playing )
            {
                try
                {
                    CrossMediaManager.Current.MediaNotificationManager.StopNotifications();
                }
                catch { }
            }
        }

        protected override void OnResume()
        {
            Task.Factory.StartNew( new Action( backgroundAction ) );
        }
    }
}
