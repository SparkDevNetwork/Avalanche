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
using FFImageLoading.Transformations;
using Xam.Forms.Markdown;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class CardListView : ContentView, IListViewComponent
    {

        private double _columns = 2;
        public double Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;

                gGrid.ColumnDefinitions.Clear();
                for ( var i = 0; i < Columns; i++ )
                {
                    gGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );
                }
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
                aiLoading.IsRunning = value;
            }
        }
        public List<ListElement> ItemsSource { get; set; }
        public object SelectedItem { get; set; }

        public bool CanRefresh { get; set; }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public CardListView()
        {
            InitializeComponent();
            ItemsSource = new List<ListElement>();

            for ( var i = 0; i < Columns; i++ )
            {
                gGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );
            }

            svScrollView.Scrolled += SvScrollView_Scrolled;

        }

        private void SvScrollView_Scrolled( object sender, ScrolledEventArgs e )
        {
            double scrollingSpace = svScrollView.ContentSize.Height - svScrollView.Height - 20;

            if ( scrollingSpace <= e.ScrollY && !IsRefreshing )
            {
                if ( ItemsSource.Any() )
                {
                    ItemAppearing?.Invoke( this, new ItemVisibilityEventArgs( ItemsSource[ItemsSource.Count - 1] ) );
                }
            }
        }


        public void Draw()
         {
            gGrid.Children.Clear();
            gGrid.RowDefinitions.Clear();
            gGrid.ColumnDefinitions.Clear();

            for ( var i = 0; i < Columns; i++ )
            {
                gGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );
            }
            gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );

            while ( gGrid.RowDefinitions.Count < ItemsSource.Count / Columns )
            {
                gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
            }

            int itemNumber = 0;
            foreach ( ListElement item in ItemsSource )
            {
                AddCell( item,
                         ( itemNumber ) % Convert.ToInt32( Columns ),
                         Convert.ToInt32( Math.Floor( ( itemNumber ) / Columns ) ) );
                itemNumber++;
            }
        }

        private void AddItems( NotifyCollectionChangedEventArgs e )
        {
            while ( gGrid.RowDefinitions.Count < ItemsSource.Count / Columns )
            {
                gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
            }

            foreach ( ListElement item in e.NewItems )
            {
                AddCell( item,
                        ( ItemsSource.Count - 1 ) % Convert.ToInt32( Columns ),
                        Convert.ToInt32( Math.Floor( ( ItemsSource.Count - 1 ) / Columns ) ) );
            }
        }

        private void AddCell( ListElement item, int x, int y )
        {
            var frame = new Frame()
            {
                Padding = new Thickness( 0, 0, 0, 10 ),
                HasShadow = true
            };

            StackLayout sl = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = ( App.Current.MainPage.Width / Columns ) - 10
            };
            frame.Content = sl;

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
                    Margin = new Thickness( 0, 15, 0, 0 ),
                    TextColor = item.IconTextColor
                };
                sl.Children.Add( icon );
            }

            Label title = new Label()
            {
                Text = item.Title,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = item.FontSize,
                TextColor = item.TextColor,
                Margin = new Thickness( 10, 0 )
            };
            sl.Children.Add( title );

            MarkdownView description = new MarkdownView()
            {
                Markdown = item.Description,
                Margin = new Thickness( 10, 0 )
            };
            sl.Children.Add( description );

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
            gGrid.Children.Add( frame, x, y );
        }
    }
}