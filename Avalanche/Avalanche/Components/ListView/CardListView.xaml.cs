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
using FFImageLoading.Transformations;
using Xam.Forms.Markdown;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class CardListView : ContentView, IListViewComponent
    {

        private int _columns = 1;
        public int Columns
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
                aiLoading.IsRunning = value;
            }
        }
        public ObservableCollection<MobileListViewItem> ItemsSource { get; set; }
        public object SelectedItem { get; set; }
        public double FontSize { get; set; }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public CardListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<MobileListViewItem>();
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

            foreach ( MobileListViewItem item in e.NewItems )
            {
                AddCell( item,
                        ( ItemsSource.Count - 1 ) % Columns,
                        ( ( ItemsSource.Count + 1 ) / Columns ) - 1 );
            }
        }

        private void AddCell( MobileListViewItem item, int x, int y )
        {
            var frame = new Frame()
            {
                Padding = new Thickness( 0,0,0,10),
                HasShadow = true
            };

            StackLayout sl = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = ( App.Current.MainPage.Width / Columns ) - 10,
            };
            frame.Content = sl;

            if ( !string.IsNullOrWhiteSpace( item.Image ) )
            {
                CachedImage img = new CachedImage()
                {
                    Source = item.Image,
                    Aspect = Aspect.AspectFit,
                    WidthRequest = App.Current.MainPage.Width / Columns,
                };
                //img.Transformations = new List<FFImageLoading.Work.ITransformation> { new CircleTransformation() };
                sl.Children.Add( img );
            }
            else
            {
                IconLabel icon = new IconLabel()
                {
                    Text = item.Icon,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 60
                };
                sl.Children.Add( icon );
            }

            Label title = new Label()
            {
                Text = item.Title,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20,
                Margin = new Thickness(10,0)
                
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