using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace Avalanche.Utilities
{
    public static class RockClient
    {
        public static bool Login(string username, string password)
        {
            var client = new RestClient( Constants.serverUrl );
            var values = new Dictionary<string, string>
            {
                { "client_id", Constants.client_id },
                { "client_secret", Constants.client_secret },
                { "grant_type", "password" },
                { "username", username},
                { "password", password },
            };

            var request = new RestRequest( Constants.tokenEndpoint, Method.POST )
            { RequestFormat = RestSharp.DataFormat.Json }
                .AddBody( values );

            var response = client.Execute( request );

            return false;
        }
    }
}
