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
using Avalanche.Models;
using Avalanche.Utilities;
using Avalanche.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Avalanche
{
    public class AvalanchePage : MultiPage<Xamarin.Forms.Page>
    {
        ObservableResource<HomeRequest> observableResource = new ObservableResource<HomeRequest>();
        MainPage mainPage;

        private bool isPortrait = true;

        public AvalanchePage()
        {
            Children.Clear();
            AvalancheNavigation.Footer = null;
            mainPage = new MainPage();
            App.Navigation = new Xamarin.Forms.NavigationPage( mainPage );
            Children.Add( App.Navigation );
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            RockClient.GetResource( observableResource, "/api/avalanche/home" );
            if ( !App.Current.Properties.ContainsKey( "SecondRun" ) )
            {
                App.Navigation.Navigation.PushModalAsync( new LaunchPage() );
                App.Current.Properties["SecondRun"] = true;
            }
        }

        private void ObservableResource_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            HandleResponse();
        }

        private void HandleResponse()
        {
            if ( observableResource.Resource.Page != null )
            {
                mainPage.observableResource.Resource = observableResource.Resource.Page;
            }
            bool addFooter = AvalancheNavigation.Footer == null;
            if ( observableResource.Resource.Footer != null )
            {
                AvalancheNavigation.Footer = new MenuPage( observableResource.Resource.Footer );
                if ( addFooter )
                {
                    Children.Insert( 0, AvalancheNavigation.Footer );
                }
            }
            AvalancheNavigation.AllowResize = true;
        }

        protected override void OnSizeAllocated( double width, double height )
        {
            base.OnSizeAllocated( width, height );
            bool localPortrait = true;

            localPortrait = height > width;

            if ( AvalancheNavigation.AllowResize || localPortrait != isPortrait )
            {
                AvalancheNavigation.SafeInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();

                if ( AvalancheNavigation.Footer != null )
                {
                    if ( AvalancheNavigation.Footer.Menu.Height != -1 )
                    {
                        AvalancheNavigation.YOffSet = AvalancheNavigation.Footer.Menu.Height + AvalancheNavigation.SafeInset.Bottom;
                    }
                    else
                    {
                        return;
                    }
                    isPortrait = localPortrait;
                    AvalancheNavigation.Footer.Menu.Margin = new Thickness( AvalancheNavigation.SafeInset.Left, 0, AvalancheNavigation.SafeInset.Right, 0 );
                    AvalancheNavigation.Footer.TranslationY = mainPage.Content.Height - AvalancheNavigation.YOffSet;
                    AvalancheNavigation.SafeInset.Bottom = 0;
                }
                else
                {
                    AvalancheNavigation.YOffSet = 0;
                }

                mainPage.Content.Margin = new Thickness(
                    AvalancheNavigation.SafeInset.Left,
                    AvalancheNavigation.YOffSet + AvalancheNavigation.SafeInset.Top,
                    AvalancheNavigation.SafeInset.Right,
                    AvalancheNavigation.SafeInset.Bottom );

                App.Navigation.TranslationY = AvalancheNavigation.YOffSet * -1;
            }
            AvalancheNavigation.AllowResize = false;
        }

        protected override Xamarin.Forms.Page CreateDefault( object item )
        {
            return new Xamarin.Forms.Page();
        }
    }
}