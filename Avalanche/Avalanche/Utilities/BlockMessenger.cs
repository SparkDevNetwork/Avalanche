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
