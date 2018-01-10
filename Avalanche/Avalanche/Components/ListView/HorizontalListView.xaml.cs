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

        public HorizontalListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<MobileListViewItem>();
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

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
            slStackLayout.Children.Clear();
            foreach ( var item in ItemsSource )
            {
                AddCell( item );
            }
        }

        private void AddItems( NotifyCollectionChangedEventArgs e )
        {
            foreach ( MobileListViewItem item in e.NewItems )
            {
                AddCell( item );
            }
        }

        private void AddCell( MobileListViewItem item )
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
                CachedImage img = new CachedImage()
                {
                    Source = item.Image,
                    Aspect = Aspect.AspectFit,
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
                    TextColor = IconColor
                };
                sl.Children.Add( icon );
            }

            Label label = new Label()
            {
                Text = item.Title,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = item.FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
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
            slStackLayout.Children.Add( sl );
        }
    }
}