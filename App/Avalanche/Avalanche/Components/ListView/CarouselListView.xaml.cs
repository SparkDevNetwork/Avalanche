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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.CustomControls;
using Avalanche.Interfaces;
using Avalanche.Models;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class CarouselListView : ContentView, IListViewComponent, INotify
    {
        public double Columns { get; set; } //does nothing
        public double ImageWidth { get; set; } = 0;
        public bool IsRefreshing { get; set; }
        public List<ListElement> ItemsSource { get; set; }
        //iOS version of this plugin crashes when we remove items from our itemsource so we proxy as a workaround
        public ObservableCollection<ListElement> ItemsSourceProxy { get; set; }
        public object SelectedItem { get; set; }
        public bool CanRefresh { get; set; }
        public int ScrollInterval { get; set; } = 0;
        private bool autoscrollRunning = false;
        private bool autoscrollUpdated = false;

        public new double HeightRequest
        {
            get
            {
                return Carousel.HeightRequest;
            }
            set
            {
                Carousel.HeightRequest = value;
            }
        }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public CarouselListView()
        {
            InitializeComponent();
            ItemsSource = new List<ListElement>();
            ItemsSourceProxy = new ObservableCollection<ListElement>();

            Carousel.PositionSelected += Carousel_PositionSelected;
            Carousel.ItemsSource = ItemsSourceProxy;
        }

        public void Draw( )
        {
            foreach (ListElement item in ItemsSource)
            {
                var add = true;
                foreach (var i in ItemsSourceProxy)
                {
                    if (!string.IsNullOrEmpty(i.Id) && i.Id == item.Id)
                    {
                        i.ActionType = item.ActionType;
                        i.Description = item.Description;
                        i.DescriptionFontSize = item.DescriptionFontSize;
                        i.DescriptionTextColor = item.DescriptionTextColor;
                        i.FontSize = item.FontSize;
                        i.Image = item.Image;
                        i.Resource = item.Resource;
                        i.TextColor = item.TextColor;
                        i.Title = item.Title;

                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    ItemsSourceProxy.Add(item);
                }
            }
        }

        private void Carousel_PositionSelected( object sender, SelectedPositionChangedEventArgs e )
        {
            if ( Carousel.Position == ItemsSourceProxy.Count - 1 && !IsRefreshing && CanRefresh )
            {
                ItemAppearing?.Invoke( this, new ItemVisibilityEventArgs( ItemsSourceProxy[Carousel.Position] ) );
            }

            if ( autoscrollUpdated && autoscrollRunning )
            {
                //If the auto scroller updated the item ignore
                autoscrollUpdated = false;
            }
            else
            {
                autoscrollRunning = false;
            }
        }

        private void TapGestureRecognizer_Tapped( object sender, EventArgs e )
        {
            SelectedItem = ItemsSourceProxy[Carousel.Position];
            ItemSelected?.Invoke( sender, new SelectedItemChangedEventArgs( ItemsSourceProxy[Carousel.Position] ) );
        }

        public void OnAppearing()
        {
            if ( ScrollInterval > 0 && !autoscrollRunning )
            {
                Task.Factory.StartNew( AutoScroll );
            }
            else
            {
                autoscrollRunning = true;
            }
        }

        private async void AutoScroll()
        {
            autoscrollRunning = true;
            while ( autoscrollRunning )
            {
                await Task.Delay( ScrollInterval * 1000 );
                Device.BeginInvokeOnMainThread( () =>
               {
                   if ( autoscrollRunning )
                   {
                       autoscrollUpdated = true;
                       if ( Carousel.Position == ItemsSource.Count - 1 )
                       {
                           Carousel.Position = 0;
                       }
                       else
                       {
                           Carousel.Position++;
                       }
                   }
               } );

            }
        }

        public void OnDisappearing()
        {
            autoscrollRunning = false;
        }
    }
}