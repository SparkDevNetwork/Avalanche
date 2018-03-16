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
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            RockClient.CreateDatabase();
            MainPage = new NavigationPage( new Avalanche.MainPage( "home" ) );
            if ( !App.Current.Properties.ContainsKey( "SecondRun" ) )
            {
                MainPage.Navigation.PushModalAsync( new LaunchPage() );
                App.Current.Properties["SecondRun"] = true;
            }
        }


        protected override void OnStart()
        {
            var interaction = new Interaction
            {
                Operation="AppLaunch"
            };
            interaction.Send();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
