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
using System.ComponentModel;
using System.Text;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public class BlockMessenger
    {
        private int _blockId;

        public event EventHandler<MobileBlockResponse> Response;
        private ObservableResource<MobileBlockResponse> _observableResource;

        public BlockMessenger( int blockId )
        {
            _blockId = blockId;
            _observableResource = new ObservableResource<MobileBlockResponse>();
            _observableResource.PropertyChanged += _observableResource_PropertyChanged;
        }
        private void _observableResource_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Response?.Invoke( this, _observableResource.Resource );
        }

        internal void Get( string resource, bool refresh = false )
        {
            resource = string.Format( "/api/avalanche/block/{0}/{1}", _blockId, resource );
            RockClient.GetResource( _observableResource, resource, refresh );
        }

        internal void Post( string resource, Dictionary<string, string> body )
        {
            resource = string.Format( "/api/avalanche/block/{0}/{1}", _blockId, resource );
            RockClient.PostResource( _observableResource, resource, body );
        }
    }
}
