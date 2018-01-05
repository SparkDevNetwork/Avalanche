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
            //listViewComponent.IsRefreshing = true;

            listViewComponent.Refreshing += ListView_Refreshing;
            listViewComponent.ItemAppearing += ListView_ItemAppearing;

            if ( Attributes.ContainsKey( "DetailPage" ) && !string.IsNullOrWhiteSpace( Attributes["DetailPage"] ) )
            {
                DetailPage = Attributes["DetailPage"];
            }
            listViewComponent.ItemSelected += ListView_ItemSelected;

            MessageHandler.Response += MessageHandler_Response;

            if ( Attributes.ContainsKey( "Resource" ) && !string.IsNullOrWhiteSpace( Attributes["Resource"] ) )
            {
                MessageHandler.Get( Attributes["Resource"] );
            }
            else
            {
                MessageHandler.Get( "" );
            }

            if ( Attributes.ContainsKey( "Columns" ) && !string.IsNullOrWhiteSpace( Attributes["Columns"] ) )
            {
                listViewComponent.Columns = Convert.ToDouble( Attributes["Columns"] );
            }

            var view = ( View ) listViewComponent;
            //view.HeightRequest = App.Current.MainPage.Height;

            return view;
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
            catch (Exception ex)
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
            if ( Attributes.ContainsKey( "Resource" ) && !string.IsNullOrWhiteSpace( Attributes["Resource"] ) )
            {
                MessageHandler.Get( Attributes["Resource"], true );

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

            AttributeHelper.HandleActionItem( Attributes );
        }
        #endregion
    }
}
