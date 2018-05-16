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

namespace Avalanche
{
    public class AvalanchePage : MultiPage<Page>
    {
        ObservableResource<HomeRequest> observableResource = new ObservableResource<HomeRequest>();
        MainPage mainPage;
        private bool resize = false;
        private bool isPortrait = true;

        public AvalanchePage()
        {
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            RockClient.GetResource( observableResource, "/api/avalanche/home" );
            mainPage = new MainPage();
            App.Navigation = new NavigationPage( mainPage );
            Children.Add( App.Navigation );
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
            resize = true;
        }



        protected override void OnSizeAllocated( double width, double height )
        {
            bool localPortrait = true;

            localPortrait = height > width;

            base.OnSizeAllocated( width, height );
            if ( resize || localPortrait != isPortrait )
            {
                isPortrait = localPortrait;
                if ( AvalancheNavigation.Footer != null )
                {
                    AvalancheNavigation.YOffSet = AvalancheNavigation.Footer.Menu.Height;
                    AvalancheNavigation.Footer.Menu.Margin = new Thickness( 0, App.Current.MainPage.Height - AvalancheNavigation.YOffSet, 0, 0 );
                }

                mainPage.Content.Margin = new Thickness( 0, AvalancheNavigation.YOffSet, 0, 0 );
                mainPage.TranslationY = AvalancheNavigation.YOffSet * -1;
            }
            resize = false;
        }

        protected override Page CreateDefault( object item )
        {
            return new Page();
        }
    }
}