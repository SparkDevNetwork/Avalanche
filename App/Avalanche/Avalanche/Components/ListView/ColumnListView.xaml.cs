// <copyright>
// Copyright Southeast Christian Church
// Copyright Mark Lee
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class ColumnListView : ContentView, IListViewComponent
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
                _resetItems();
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
        public ObservableCollection<ListElement> ItemsSource { get; set; }
        public object SelectedItem { get; set; }
        public bool CanRefresh { get; set; }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public ColumnListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<ListElement>();
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

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

        private void ItemsSource_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            if ( e.Action == NotifyCollectionChangedAction.Add )
            {
                AddItems( e );
            }
            else
            {
                ResetItems( e );
            }
        }

        private void ResetItems( NotifyCollectionChangedEventArgs e )
        {
            _resetItems();
        }

        private void _resetItems()
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

            foreach ( ListElement item in ItemsSource )
            {
                AddCell( item,
                         ( ItemsSource.Count - 1 ) % Convert.ToInt32( Columns ),
                         Convert.ToInt32( Math.Floor( ( ItemsSource.Count - 1 ) / Columns ) ) );
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
            StackLayout sl = new StackLayout() { HorizontalOptions = LayoutOptions.Center };
            if ( !string.IsNullOrWhiteSpace( item.Image ) )
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
            else
            {
                IconLabel icon = new IconLabel()
                {
                    Text = item.Icon,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = item.IconFontSize,
                    InputTransparent = true,
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
                InputTransparent = true,
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
            gGrid.Children.Add( sl, x, y );
        }

    }
}