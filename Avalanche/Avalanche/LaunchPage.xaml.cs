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

namespace Avalanche
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
                    Title = "Welcome To Avalanche",
                    Description = "This is the demonstration app for Avalanche. Avalanche is a framework for building apps coupled to your instance of Rock.",
                    Next = true
                },
                new CarouselItem
                {
                    Image = "Splash2.png",
                    Title = "Your Church, Your App",
                    Description = "Avalanche allows churches to build high quality apps without the need for special development or generic templates.",
                    Next = true
                },
                new CarouselItem
                {
                    Image = "Splash3.png",
                    Title = "Bit of a Trick.",
                    Description = "This into carousel is actually here to give the app time to preload. Once you click lets go, the app should already be loaded.",
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
            Carousel.Position += 1;
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