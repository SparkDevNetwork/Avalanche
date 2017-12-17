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

        public int Columns { get; set; }

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
        public ObservableCollection<MobileListView> ItemsSource { get; set; }
        public object SelectedItem { get; set; }
        public double FontSize { get; set; }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public ColumnListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<MobileListView>();
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            Columns = 2;

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
            gGrid.Children.Clear();
            gGrid.RowDefinitions.Clear();

            gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );

            var rowCounter = 0;
            var columnCounter = 0;

            foreach ( var item in ItemsSource )
            {
                if ( columnCounter > Columns )
                {
                    columnCounter = 0;
                    rowCounter++;
                    gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
                }

                AddCell( item, rowCounter, columnCounter );
                columnCounter++;
            }
        }

        private void AddItems( NotifyCollectionChangedEventArgs e )
        {
            while ( gGrid.RowDefinitions.Count < ItemsSource.Count / Columns )
            {
                gGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
            }

            foreach ( MobileListView item in e.NewItems )
            {
                AddCell( item,
                        ( ItemsSource.Count - 1 ) % Columns,
                        ( ( ItemsSource.Count + 1 ) / Columns ) - 1 );
            }
        }

        private void AddCell( MobileListView item, int x, int y )
        {
            StackLayout sl = new StackLayout() { HorizontalOptions = LayoutOptions.Center };
            if ( !string.IsNullOrWhiteSpace( item.Image ) )
            {
                CachedImage img = new CachedImage() { Source = item.Image, Aspect = Aspect.AspectFit, WidthRequest = App.Current.MainPage.Width / Columns };
                sl.Children.Add( img );
            }
            else
            {
                FontAwesomeIcon fai = new FontAwesomeIcon() { Text = item.Icon, HorizontalOptions = LayoutOptions.Center, FontSize = 60 };
                sl.Children.Add( fai );
            }

            Label label = new Label() { Text = item.Title, HorizontalOptions = LayoutOptions.Center, FontSize = 20 };
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