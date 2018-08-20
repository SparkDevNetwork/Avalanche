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
using Avalanche.Models;
using Avalanche.Utilities;
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
                var startup = new FFImageLoading.Forms.CachedImage { Opacity = 0 };
                startup.Finish += Startup_Finish;
                startup.Source = "https://rock.secc.org/Content/SEApp/Sermons.jpg";
                var sl = new StackLayout();
                MainPage = new ContentPage()
                {
                    Content = sl,
                    BackgroundColor = Color.Black
                };
                sl.Children.Add( new ActivityIndicator { IsRunning = true, Color = Color.White } );
                sl.Children.Add( new FFImageLoading.Forms.CachedImage { Source = "https://rock.secc.org/Content/SEApp/Stories.jpg", Opacity = 0 } );
                sl.Children.Add( new FFImageLoading.Forms.CachedImage { Source = "https://rock.secc.org/Content/SEApp/Events.jpg", Opacity = 0 } );
                sl.Children.Add( new FFImageLoading.Forms.CachedImage { Source = "https://rock.secc.org/Content/SEApp/Serve.jpg", Opacity = 0 } );

                sl.Children.Add( startup );
            }
            else
            {
                MainPage = new AvalanchePage();
            }
            InitializeComponent();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            RockClient.CreateDatabase();
            Task.Factory.StartNew( new Action( backgroundAction ) );
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

        }

        protected override void OnResume()
        {
            Task.Factory.StartNew( new Action( backgroundAction ) );
        }
    }
}
