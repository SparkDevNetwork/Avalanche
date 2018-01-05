using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;
using Avalanche.Utilities;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Components.ListView
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class ThumbnailListView : ContentView, IListViewComponent
    {
        private double _columns = 1;
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

        public bool IsRefreshing
        {
            get
            {
                return lvListView.IsRefreshing;
            }
            set
            {
                lvListView.IsRefreshing = value;
            }
        }
        public object SelectedItem
        {
            get
            {
                return lvListView.SelectedItem;
            }
            set
            {
                lvListView.SelectedItem = value;
            }
        }

        public ObservableCollection<MobileListViewItem> ItemsSource { get; set; }

        private double _fontSize;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;

                UpdateFontSize();
            }
        }

        private void UpdateFontSize()
        {
            if ( ItemsSource != null )
            {
                foreach ( MobileListViewItem item in ItemsSource )
                {
                    item.FontSize = _fontSize;
                }
            }
        }

        public event EventHandler Refreshing;
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;
        private void LvListView_Refreshing( object sender, EventArgs e )
        {
            Refreshing?.Invoke( this, e );
        }

        private void LvListView_ItemSelected( object sender, SelectedItemChangedEventArgs e )
        {
            ItemSelected?.Invoke( this, e );
        }
        private void LvListView_ItemAppearing( object sender, ItemVisibilityEventArgs e )
        {
            ItemAppearing?.Invoke( this, e );
        }

        public ThumbnailListView()
        {
            InitializeComponent();
            ItemsSource = new ObservableCollection<MobileListViewItem>();
            lvListView.ItemsSource = ItemsSource;

            lvListView.Refreshing += LvListView_Refreshing;
            lvListView.ItemSelected += LvListView_ItemSelected;
            lvListView.ItemAppearing += LvListView_ItemAppearing;
        }

    }
}