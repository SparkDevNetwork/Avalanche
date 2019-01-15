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

        public List<ListElement> ItemsSource { get; set; }

        public bool CanRefresh
        {
            get
            {
                return lvListView.IsPullToRefreshEnabled;
            }
            set
            {
                lvListView.IsPullToRefreshEnabled = value;
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
            ItemsSource = new List<ListElement>();
            lvListView.ItemsSource = new ObservableCollection<ListElement>();

            lvListView.Refreshing += LvListView_Refreshing;
            lvListView.ItemSelected += LvListView_ItemSelected;
            lvListView.ItemAppearing += LvListView_ItemAppearing;
        }

        public void Draw()
        {
            var source = ((ObservableCollection<ListElement>)lvListView.ItemsSource);
            source.Clear();
            foreach( var item in ItemsSource)
            {
                source.Add(item);
            }
        }

    }
}