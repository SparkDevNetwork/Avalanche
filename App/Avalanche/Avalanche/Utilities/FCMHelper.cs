using System;
using System.Collections.Generic;
using System.Text;
using Avalanche.Models;

namespace Avalanche.Utilities
{
    public static class FCMHelper
    {
        public static void RegisterFCMToken( string token = null )
        {
            if ( string.IsNullOrWhiteSpace( token ) )
            {
                if ( App.Current.Properties.ContainsKey( "FCMToken" ) )
                {
                    token = App.Current.Properties["FCMToken"] as string;
                }
            }
            else
            {
                App.Current.Properties["FCMToken"] = token;
            }

            if ( !string.IsNullOrWhiteSpace( token ) )
            {
                ObservableResource<Dictionary<string, string>> observableResource = new ObservableResource<Dictionary<string, string>>();
                var body = new Dictionary<string, string>()
                {
                    { "Token",token }
                };
                RockClient.PostResource( observableResource, "/api/avalanche/registerfcm", body );
            }
        }
    }
}
