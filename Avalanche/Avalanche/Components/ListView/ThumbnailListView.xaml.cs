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