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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Components;
using Avalanche.Components.ListView;
using Avalanche.Models;
using Avalanche.Utilities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    public class ListViewBlock : IRenderable, IHasBlockMessenger
    {
        private IListViewComponent listViewComponent;
        private bool _manualRefresh = false;
        private int _pageNumber = 1;
        private bool _useFresh = false;
        private bool _endOfList = false;
        public string DetailPage { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        public View Render()
        {
            if ( Attributes.ContainsKey( "Component" ) && !string.IsNullOrWhiteSpace( Attributes["Component"] ) )
            {
                var component = Type.GetType( Attributes["Component"] );
                if ( component != null )
                {
                    listViewComponent = ( IListViewComponent ) Activator.CreateInstance( component );
                }
                else
                {
                    listViewComponent = new ThumbnailListView();
                }
            }
            else
            {
                listViewComponent = new ThumbnailListView();
            }

            PreConfigureStyles();

            listViewComponent.Refreshing += ListView_Refreshing;
            listViewComponent.ItemAppearing += ListView_ItemAppearing;
            listViewComponent.ItemSelected += ListView_ItemSelected;
            MessageHandler.Response += MessageHandler_Response;




            if ( Attributes.ContainsKey( "Content" ) && !string.IsNullOrWhiteSpace( Attributes["Content"] ) )
            {
                AddRenderContent();

            }

            if ( Attributes.ContainsKey( "Request" ) && !string.IsNullOrWhiteSpace( Attributes["Request"] ) )
            {
                MessageHandler.Get( Attributes["Request"] );
            }
            else
            {
                MessageHandler.Get( "" );
            }


            var view = ( View ) listViewComponent;
            return view;
        }

        private void PreConfigureStyles()
        {
            if ( Attributes.ContainsKey( "Columns" ) && !string.IsNullOrWhiteSpace( Attributes["Columns"] ) )
            {
                listViewComponent.Columns = Convert.ToDouble( Attributes["Columns"] );
                Attributes.Remove( "Columns" );
            }

            if ( Attributes.ContainsKey( "FontSize" ) && !string.IsNullOrWhiteSpace( Attributes["FontSize"] ) )
            {
                listViewComponent.FontSize = Convert.ToDouble( Attributes["FontSize"] );
            }

            if ( Attributes.ContainsKey( "IconSize" ) && !string.IsNullOrWhiteSpace( Attributes["IconSize"] ) )
            {
                listViewComponent.IconSize = Convert.ToDouble( Attributes["IconSize"] );
            }

            if ( Attributes.ContainsKey( "TextColor" ) && !string.IsNullOrWhiteSpace( Attributes["TextColor"] ) )
            {
                listViewComponent.TextColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( Attributes["TextColor"] );
            }

            if ( Attributes.ContainsKey( "IconColor" ) && !string.IsNullOrWhiteSpace( Attributes["IconColor"] ) )
            {
                listViewComponent.IconColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( Attributes["IconColor"] );
            }
        }

        private void AddRenderContent()
        {
            List<MobileListViewItem> mlv = JsonConvert.DeserializeObject<List<MobileListViewItem>>( Attributes["Content"] );
            foreach ( var item in mlv )
            {
                item.FontSize = listViewComponent.FontSize;
                foreach ( var i in listViewComponent.ItemsSource )
                {
                    if ( !string.IsNullOrEmpty( i.Id ) && i.Id == item.Id )
                    {
                        listViewComponent.ItemsSource.Remove( i );
                        break;
                    }
                }
                listViewComponent.ItemsSource.Add( item );
            }
            listViewComponent.IsRefreshing = false;
        }

        #region Events
        private void MessageHandler_Response( object sender, MobileBlockResponse e )
        {
            var response = e.Response;
            if ( _manualRefresh )
            {
                listViewComponent.ItemsSource.Clear();
                _manualRefresh = false;
            }
            try
            {

                List<MobileListViewItem> mlv = JsonConvert.DeserializeObject<List<MobileListViewItem>>( response );
                if ( mlv == null || !mlv.Any() )
                {
                    _endOfList = true;
                    listViewComponent.IsRefreshing = false;
                    return;
                }
                foreach ( var item in mlv )
                {
                    item.FontSize = listViewComponent.FontSize;
                    foreach ( var i in listViewComponent.ItemsSource )
                    {
                        if ( !string.IsNullOrEmpty( i.Id ) && i.Id == item.Id )
                        {
                            listViewComponent.ItemsSource.Remove( i );
                            break;
                        }
                    }
                    listViewComponent.ItemsSource.Add( item );
                }
                listViewComponent.IsRefreshing = false;
            }
            catch ( Exception ex )
            {
                _endOfList = true;
                listViewComponent.IsRefreshing = false;
            }
        }

        private void ListView_Refreshing( object sender, EventArgs e )
        {
            _endOfList = false;
            _manualRefresh = true;
            _useFresh = true;
            listViewComponent.IsRefreshing = true;
            _pageNumber = 1;
            if ( Attributes.ContainsKey( "Request" ) && !string.IsNullOrWhiteSpace( Attributes["Resource"] ) )
            {
                MessageHandler.Get( Attributes["Request"], true );

            }
            else
            {
                MessageHandler.Get( "", true );
            }
        }

        private void ListView_ItemAppearing( object sender, ItemVisibilityEventArgs e )
        {
            if ( listViewComponent.IsRefreshing || listViewComponent.ItemsSource.Count == 0 || _endOfList )
                return;

            //hit bottom!
            if ( ( ( MobileListViewItem ) e.Item ).Id == listViewComponent.ItemsSource[listViewComponent.ItemsSource.Count - 1].Id )
            {
                _pageNumber++;
                listViewComponent.IsRefreshing = true;
                MessageHandler.Get( _pageNumber.ToString(), _useFresh );
            }
        }

        private void ListView_ItemSelected( object sender, SelectedItemChangedEventArgs e )
        {
            if ( e.SelectedItem == null )
            {
                return;
            }

            var item = listViewComponent.SelectedItem as MobileListViewItem;
            if ( !string.IsNullOrWhiteSpace( item.Resource ) && !string.IsNullOrWhiteSpace( item.ActionType ) )
            {
                AttributeHelper.HandleActionItem( new Dictionary<string, string> { { "Resource", item.Resource }, { "ActionType", item.ActionType } } );
            }


            listViewComponent.SelectedItem = null;
            Attributes["Parameter"] = item.Id;
            AttributeHelper.HandleActionItem( Attributes );
        }
        #endregion
    }
}
