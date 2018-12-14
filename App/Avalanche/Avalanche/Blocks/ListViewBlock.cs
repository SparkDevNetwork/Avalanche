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
using Avalanche.Interfaces;
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
        private bool _endOfList = false;
        private string _nextRequest;
        private string _initialRequest;
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

            // An initial request may have Content or a Request target.
            if ( Attributes.ContainsKey( "Content" ) && !string.IsNullOrWhiteSpace( Attributes["Content"] ) )
            {
                AddRenderContent();
            }

            if ( Attributes.ContainsKey( "NextRequest" ) && !string.IsNullOrWhiteSpace( Attributes["NextRequest"] ) )
            {
                _nextRequest = Attributes["NextRequest"];
            }

            if ( Attributes.ContainsKey( "InitialRequest" ) && !string.IsNullOrWhiteSpace( Attributes["InitialRequest"] ) )
            {
                listViewComponent.CanRefresh = true;
                _initialRequest = Attributes["InitialRequest"];
            }
            else
            {
                listViewComponent.CanRefresh = false;
            }

            if ( !Attributes.ContainsKey( "Content" ) )
            {
                MessageHandler.Get( _initialRequest );
            }

            var view = ( View ) listViewComponent;
            return view;
        }

        private void PreConfigureStyles()
        {
            AttributeHelper.ApplyTranslation( listViewComponent, Attributes );

            if ( Attributes.ContainsKey( "Columns" ) )
            {
                Attributes.Remove( "Columns" );
            }
        }

        private void AddRenderContent()
        {
            List<Dictionary<string, string>> mlv = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>( Attributes["Content"] );
            foreach ( var element in mlv )
            {
                AddElement( element );
            }
            listViewComponent.ItemsSource = listViewComponent.ItemsSource.OrderBy(e => e.Order).ToList();
            listViewComponent.Draw();
            listViewComponent.IsRefreshing = false;
        }

        private void AddElement( Dictionary<string, string> template )
        {
            var keys = new List<string> { "Resource", "ActionType" };
            var clonedAttributes = Attributes
                .Where( a => !keys.Contains( a.Key ) )
                .ToDictionary( a => a.Key, a => a.Value );

            foreach ( var item in template )
            {
                clonedAttributes[item.Key] = item.Value;
            }

            var element = new ListElement();

            AttributeHelper.ApplyTranslation( element, clonedAttributes );
            foreach ( var i in listViewComponent.ItemsSource )
            {
                if ( !string.IsNullOrEmpty( i.Id ) && i.Id == element.Id )
                {
                    listViewComponent.ItemsSource.Remove( i );
                    break;
                }
            }
            listViewComponent.ItemsSource.Add(element);
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
                ListViewResponse listViewResponse = JsonConvert.DeserializeObject<ListViewResponse>( response );
                if ( listViewResponse?.Content == null || !listViewResponse.Content.Any() )
                {
                    _endOfList = true;
                    listViewComponent.IsRefreshing = false;
                    return;
                }

                if ( !string.IsNullOrWhiteSpace( listViewResponse.NextRequest ) )
                {
                    _nextRequest = listViewResponse.NextRequest;
                }
                else
                {
                    _nextRequest = null;
                }

                foreach ( var listElement in listViewResponse.Content )
                {
                    try
                    {
                        AddElement( listElement );
                    }
                    catch
                    {
                    }
                }
                listViewComponent.IsRefreshing = false;
                listViewComponent.ItemsSource = listViewComponent.ItemsSource.OrderBy(i => i.Order).ToList();
                listViewComponent.Draw();
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
            listViewComponent.IsRefreshing = true;
            if ( !string.IsNullOrWhiteSpace( _initialRequest ) )
            {
                MessageHandler.Get( _initialRequest );
            }
            else
            {
                listViewComponent.IsRefreshing = false;
            }
        }

        private void ListView_ItemAppearing( object sender, ItemVisibilityEventArgs e )
        {
            if ( listViewComponent.IsRefreshing || listViewComponent.ItemsSource.Count == 0 || _endOfList )
                return;

            //hit bottom!
            if ( ( ( ListElement ) e.Item ).Id == listViewComponent.ItemsSource[listViewComponent.ItemsSource.Count - 1].Id )
            {
                if ( !string.IsNullOrWhiteSpace( _nextRequest ) )
                {
                    listViewComponent.IsRefreshing = true;
                }
                MessageHandler.Get( _nextRequest );
            }
        }

        private void ListView_ItemSelected( object sender, SelectedItemChangedEventArgs e )
        {
            if ( e.SelectedItem == null )
            {
                return;
            }

            var item = listViewComponent.SelectedItem as ListElement;
            if ( item == null )
            {
                return;
            }

            //see if a parameter has been provided
            //if not use the item's id
            var parameter = item.Id;
            if ( !string.IsNullOrWhiteSpace( item.Parameter ) )
            {
                parameter = item.Parameter;
            }

            //if the individual item determins it's action type and resource
            if ( !string.IsNullOrWhiteSpace( item.Resource ) && !string.IsNullOrWhiteSpace( item.ActionType ) )
            {
                var actionDictionary = new Dictionary<string, string> {
                    { "Resource", item.Resource },
                    { "ActionType", item.ActionType },
                    { "Parameter",  parameter}
                };
                AvalancheNavigation.HandleActionItem( actionDictionary );
                return;
            }

            //default to the block is supplying the action type and resource
            listViewComponent.SelectedItem = null;
            Attributes["Parameter"] = parameter;
            AvalancheNavigation.HandleActionItem( Attributes );
        }
        #endregion
    }
}
