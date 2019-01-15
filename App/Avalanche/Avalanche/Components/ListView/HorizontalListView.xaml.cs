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
using Avalanche.Models;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class HorizontalListView : ContentView, IListViewComponent
    {
        private double _columns = 4.5;
        public double Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;
            }
        }
        public List<ListElement> ItemsSource { get; set; }
        public object SelectedItem { get; set; }
        public bool CanRefresh { get; set; }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public HorizontalListView()
        {
            InitializeComponent();
            ItemsSource = new List<ListElement>();

            svScrollView.Scrolled += SvScrollView_Scrolled;

        }

        private void SvScrollView_Scrolled( object sender, ScrolledEventArgs e )
        {
            double scrollingSpace = svScrollView.ContentSize.Width - svScrollView.Width - 20;

            if ( scrollingSpace <= e.ScrollY && !IsRefreshing )
            {
                if ( ItemsSource.Any() )
                {
                    ItemAppearing?.Invoke( this, new ItemVisibilityEventArgs( ItemsSource[ItemsSource.Count - 1] ) );
                }
            }
        }


        public void Draw( )
        {
            slStackLayout.Children.Clear();
            foreach ( var item in ItemsSource )
            {
                AddCell( item );
            }
        }


        protected override void OnSizeAllocated( double width, double height )
        {
            base.OnSizeAllocated( width, height );
            foreach ( var child in slStackLayout.Children )
            {
                child.WidthRequest = ( App.Current.MainPage.Width / Columns ) - ( slStackLayout.Spacing * ( Columns - 1 ) );
            }

        }

        private void AddCell( ListElement item )
        {
            var widthRequest = ( App.Current.MainPage.Width / Columns ) - ( slStackLayout.Spacing * ( Columns - 1 ) );
            StackLayout sl = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = widthRequest,
                Padding = new Thickness( 4 ),
            };
            if ( !string.IsNullOrWhiteSpace( item.Image ) )
            {
                if ( item.Image.Contains( ".svg" ) )
                {
                    SvgCachedImage img = new SvgCachedImage()
                    {
                        Source = item.Image,
                        Aspect = Aspect.AspectFit,
                        WidthRequest = App.Current.MainPage.Width / Columns,
                        InputTransparent = true
                    };
                    sl.Children.Add( img );
                }
                else
                {
                    CachedImage img = new CachedImage()
                    {
                        Source = item.Image,
                        Aspect = Aspect.AspectFit,
                        WidthRequest = App.Current.MainPage.Width / Columns,
                        InputTransparent = true
                    };
                    sl.Children.Add( img );
                }
            }
            else
            {
                IconLabel icon = new IconLabel()
                {
                    Text = item.Icon,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = item.IconFontSize,
                    TextColor = item.IconTextColor
                };
                sl.Children.Add( icon );
            }

            Label label = new Label()
            {
                Text = item.Title,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = item.FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = item.TextColor
            };
            sl.Children.Add( label );

            TapGestureRecognizer tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            tgr.Tapped += ( s, ee ) =>
            {
                SelectedItem = item;
                ItemSelected?.Invoke( sl, new SelectedItemChangedEventArgs( item ) );
            };
            sl.GestureRecognizers.Add( tgr );
            slStackLayout.Children.Add( sl );
        }
    }
}