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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Views
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class LaunchPage : ContentPage
    {
        public LaunchPage()
        {
            InitializeComponent();
            var items = new List<CarouselItem>
            {
                new CarouselItem
                {
                    Image = "Splash1.jpg",
                },
                new CarouselItem
                {
                    Image = "Splash2.jpg",
                },
                new CarouselItem
                {
                    Image = "Splash3.jpg",
                },
                new CarouselItem
                {
                }
            };

            Carousel.ItemsSource = items;

            Carousel.PositionSelected += Carousel_PositionSelected;
        }

        private void Carousel_PositionSelected( object sender, SelectedPositionChangedEventArgs e )
        {
            if ( Carousel.Position == 3 )
            {
                Navigation.PopModalAsync();
            }
        }

        private class CarouselItem
        {
            public string Image { get; set; }
        }
    }
}