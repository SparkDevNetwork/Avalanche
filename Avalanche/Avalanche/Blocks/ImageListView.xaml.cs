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

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class ImageListView : ContentView, IRenderable, IHasBlockMessenger, INotifyPropertyChanged
    {
        private bool _manualRefresh = false;
        private int _pageNumber = 1;
        private bool _useFresh = false;
        private bool _endOfList = false;
        public string DetailPage { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        private ObservableCollection<MobileListView> _mobileListview = new ObservableCollection<MobileListView>();
        public ObservableCollection<MobileListView> mobileListView
        {
            get
            {
                return _mobileListview;
            }
            set
            {
                _mobileListview = value;
                OnPropertyChanged( "mobileListView" );
            }
        }


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


        public ImageListView()
        {
            InitializeComponent();
            lvListView.Refreshing += LvListView_Refreshing;
            lvListView.ItemSelected += LvListView_ItemSelected;
            lvListView.ItemAppearing += LvListView_ItemAppearing;
        }

        private void LvListView_ItemAppearing( object sender, ItemVisibilityEventArgs e )
        {
            if ( lvListView.IsRefreshing || mobileListView.Count == 0 || _endOfList )
                return;

            //hit bottom!
            if ( ( ( MobileListView ) e.Item ).Id == mobileListView[mobileListView.Count - 1].Id )
            {
                _pageNumber++;
                lvListView.IsRefreshing = true;
                MessageHandler.Get( _pageNumber.ToString(), _useFresh );
            }
        }

        private void LvListView_ItemSelected( object sender, SelectedItemChangedEventArgs e )
        {

        }

        private void LvListView_Refreshing( object sender, EventArgs e )
        {
            _endOfList = false;
            _manualRefresh = true;
            _useFresh = true;
            lvListView.IsRefreshing = true;
            _pageNumber = 1;
            MessageHandler.Get( _pageNumber.ToString(), true );
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            lvListView.ItemsSource = mobileListView;
            MessageHandler.Response += MessageHandler_Response;
            MessageHandler.Get( "" );
            return this;
        }

        private void MessageHandler_Response( object sender, MobileBlockResponse e )
        {
            var response = e.Response;
            if ( _manualRefresh )
            {
                mobileListView.Clear();
                _manualRefresh = false;
            }

            List<MobileListView> mlv = JsonConvert.DeserializeObject<List<MobileListView>>( response );
            if ( !mlv.Any() )
            {
                _endOfList = false;
            }
            foreach ( var item in mlv )
            {
                item.FontSize = _fontSize;
                foreach ( var i in mobileListView )
                {
                    if ( i.Id != null && i.Id == item.Id )
                    {
                        mobileListView.Remove( i );
                        break;
                    }
                }
                mobileListView.Add( item );
            }
            lvListView.IsRefreshing = false;

        }

        private void UpdateFontSize()
        {
            foreach ( MobileListView item in lvListView.ItemsSource )
            {
                item.FontSize = _fontSize;
            }
        }

    }
}