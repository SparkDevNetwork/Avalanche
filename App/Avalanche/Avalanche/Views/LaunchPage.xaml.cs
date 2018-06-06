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
                    Image = "Splash1.png",
                    Title = "Southeast Christian",
                    Description = "Southeast Christian Church is a multi-campus located throughout the Louisville area.",
                    Next = true
                },
                new CarouselItem
                {
                    Image = "Splash2.png",
                    Title = "Your Church, Your App",
                    Description = "This app is here to help you get the most out of your church.",
                    Next = true
                },
                new CarouselItem
                {
                    Image = "Splash3.png",
                    Title = "Always Welcome",
                    Description = "If you have never attended a service at Southeast Christian Church, know you are welcome here.",
                    End = true
                },
            };

            Carousel.ItemsSource = items;
        }


        private void btnGo_Clicked( object sender, EventArgs e )
        {
            Navigation.PopModalAsync();
        }

        private void btnNext_Clicked( object sender, EventArgs e )
        {
            if ( ( ( List<CarouselItem> ) Carousel.ItemsSource ).Count > Carousel.Position - 1 )
            {
                Carousel.Position += 1;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        private class CarouselItem
        {
            public string Image { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Next { get; set; }
            public bool End { get; set; }
        }
    }
}