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
        public ObservableCollection<MobileListViewItem> ItemsSource { get; set; }
        public object SelectedItem { get; set; }
        private double _fontSize = 15;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
            }
        }
        private double? _iconSize;
        public double IconSize
        {
            get
            {
                return _iconSize ?? FontSize * 6;
            }
            set
            {
                _iconSize = value;
            }
        }

        private Color _textColor = Color.Black;
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
            }
        }
        private Color? _iconColor;
        public Color IconColor
        {
            get
            {
                return _iconColor ?? _textColor;
            }
            set
            {
                _iconColor = value;
            }
        }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        public ColumnListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<MobileListViewItem>();
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

            foreach ( MobileListViewItem item in ItemsSource )
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

            foreach ( MobileListViewItem item in e.NewItems )
            {
                AddCell( item,
                        ( ItemsSource.Count - 1 ) % Convert.ToInt32( Columns ),
                        Convert.ToInt32( Math.Floor( ( ItemsSource.Count - 1 ) / Columns ) ) );
            }
        }

        private void AddCell( MobileListViewItem item, int x, int y )
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
                    FontSize = IconSize,
                    InputTransparent = true,
                    TextColor = IconColor
                };
                sl.Children.Add( icon );
            }

            Label label = new Label()
            {
                Text = item.Title,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                InputTransparent = true,
                TextColor = TextColor
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