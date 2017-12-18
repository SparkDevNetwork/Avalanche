using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SQLite;

namespace Avalanche.Utilities
{
    public static class RockClient
    {
        private static string path = Path.Combine( System.Environment.GetFolderPath( System.Environment.SpecialFolder.Personal ), "Resources.db" );

        private static HttpClient authClient;

        public async static void GetResource<T>( ObservableResource<T> resource, string url, bool refresh = false )
        {
            var webResource = await GetDBResource( url );
            if ( webResource != null && refresh == false )
            {
                var resourceObject = Deserialize<T>( webResource.Response );
                resource.Resource = resourceObject;
                if ( webResource.EOL < DateTime.Now )
                {
                    var newWebResource = await DownloadResource( url );
                    if ( newWebResource != null && webResource.Response != newWebResource.Response )
                    {
                        resource.Resource = Deserialize<T>( newWebResource.Response );
                    }
                }
            }
            else
            {
                webResource = await DownloadResource( url );
                if ( webResource != null )
                {
                    resource.Resource = Deserialize<T>( webResource.Response );
                }
            }

        }

        public async static void PostResource<T>( ObservableResource<T> resource, string url, Dictionary<string, string> body )
        {
            try
            {
                using ( var client = new HttpClient() )
                {
                    client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

                    string token = await GetAccessToken();
                    if ( !string.IsNullOrWhiteSpace( token ) )
                    {
                        client.DefaultRequestHeaders.Add( "client_id", Constants.client_id );
                        client.DefaultRequestHeaders.Add( "client_id", Constants.client_secret );
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", token );
                    }
                    var content = new StringContent( JsonConvert.SerializeObject( body ) );
                    using ( var r = await client.PostAsync( new Uri( Constants.serverUrl + url ), content ) )
                    {
                        string result = await r.Content.ReadAsStringAsync();
                        if ( string.IsNullOrWhiteSpace( result ) )
                        {
                            return;
                        }
                        resource.Resource = Deserialize<T>( result );
                    }
                }
            }
            catch ( Exception e )
            {
                return;
            } //Eat network issues
        }

        private static T Deserialize<T>( string response )
        {
            if ( typeof( T ) == typeof( string ) )
            {
                return ( T ) Convert.ChangeType( response, typeof( T ) );
            }
            return JsonConvert.DeserializeObject<T>( response );
        }

        private async static Task<WebResource> DownloadResource( string url )
        {
            try
            {
                using ( var client = new HttpClient() )
                {
                    client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

                    string token = await GetAccessToken();
                    if ( !string.IsNullOrWhiteSpace( token ) )
                    {
                        client.DefaultRequestHeaders.Add( "client_id", Constants.client_id );
                        client.DefaultRequestHeaders.Add( "client_id", Constants.client_secret );
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", token );
                    }

                    using ( var response = await client.GetAsync( new Uri( Constants.serverUrl + url ) ) )
                    {
                        if ( response.IsSuccessStatusCode )
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            if ( string.IsNullOrWhiteSpace( result ) )
                            {
                                return null;
                            }
                            var ttl = 0;
                            if ( response.Headers.Contains( "TTL" ) )
                            {
                                var ttlString = response.Headers.GetValues( "TTL" ).FirstOrDefault();
                                if ( !string.IsNullOrWhiteSpace( ttlString ) )
                                {
                                    int.TryParse( ttlString, out ttl );
                                }
                            }
                            var webResource = new WebResource()
                            {
                                Url = url,
                                Response = result,
                                EOL = DateTime.Now.AddSeconds( ttl )
                            };

                            var conn = GetConnection();
                            if ( ( await conn.UpdateAsync( webResource ) ) == 0 )
                            {
                                await conn.InsertAsync( webResource );
                            }
                            return webResource;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            } //Eat network issues
        }


        private async static Task<string> GetAccessToken()
        {
            var appProp = App.Current.Properties;

            if ( appProp.ContainsKey( "client_expiration" )
                && ( DateTime ) appProp["client_expiration"] > DateTime.Now
                && appProp.ContainsKey( "client_bearer" ) )
            {
                return ( string ) appProp["client_bearer"];
            }
            try
            {
                return ( string ) await RefreshAuth();
            }
            catch ( Exception e )
            {
                return "";
            }
        }

        private async static Task<string> RefreshAuth()
        {
            var appProp = App.Current.Properties;

            if ( appProp.ContainsKey( "client_refresh_token" ) )
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", Constants.client_id },
                    { "client_secret", Constants.client_secret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", (string) appProp["client_refresh_token"] },
                };
                var content = new FormUrlEncodedContent( values );
                var response = await GetAuthClient().PostAsync( Constants.tokenEndpoint, content );

                if ( response.IsSuccessStatusCode )
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>( responseString );
                    appProp["client_bearer"] = tokenResponse.access_token; //Bear token
                    appProp["client_expiration"] = DateTime.Now.AddSeconds( ( tokenResponse.expires_in - 60 ) ); //Take away a minute for lag
                    appProp["client_refresh_token"] = tokenResponse.refresh_token;
                    await App.Current.SavePropertiesAsync();
                    return tokenResponse.access_token;
                }
                else
                {
                    appProp.Remove( "client_refresh_token" );
                    if ( appProp.ContainsKey( "client_bearer" ) )
                    {
                        appProp.Remove( "client_bearer" );
                        await App.Current.SavePropertiesAsync();
                    }
                }
            }
            return "";
        }

        private async static Task<WebResource> GetDBResource( string url )
        {
            try
            {
                var conn = GetConnection();
                var element = await conn.QueryAsync<WebResource>( "SELECT * FROM WebResource WHERE Url = ?", url );
                var resource = element.FirstOrDefault();
                if ( resource == null )
                {
                    return null;
                }
                return resource;
            }
            catch
            {
                return null;
            }
        }

        private static SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection( path );
        }

        public async static Task<bool> CreateDatabase()
        {
            try
            {
                await GetConnection().CreateTableAsync<WebResource>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async static Task<bool> ClearDatabase()
        {
            try
            {
                await GetConnection().ExecuteAsync( "DELETE FROM WebResource" );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<LoginResponse> LogIn( string username, string password )
        {
            try
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", Constants.client_id },
                    { "client_secret", Constants.client_secret },
                    { "grant_type", "password" },
                    { "username", username},
                    { "password", password },
                };
                var content = new FormUrlEncodedContent( values );
                var response = await GetAuthClient().PostAsync( Constants.tokenEndpoint, content );

                if ( response.IsSuccessStatusCode )
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>( responseString );
                    var appProp = App.Current.Properties;
                    appProp["client_bearer"] = tokenResponse.access_token; //Bear token
                    appProp["client_expiration"] = DateTime.Now.AddSeconds( ( tokenResponse.expires_in - 60 ) ); //Take away a minute for lag
                    appProp["client_refresh_token"] = tokenResponse.refresh_token;
                    await App.Current.SavePropertiesAsync();
                    await ClearDatabase();
                    return LoginResponse.Success;
                }
                else
                {
                    var output = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine( output );
                }
                return LoginResponse.Failure;
            }
            catch ( Exception ex )
            {
                return LoginResponse.Error;
            }
        }

        public static void Logout()
        {
            var appProp = App.Current.Properties;
            var keys = new List<string>() { "client_bearer", "client_expiration", "client_refresh_token" };
            foreach ( var key in keys )
            {
                if ( appProp.ContainsKey( key ) )
                {
                    appProp.Remove( key );
                }
            }
            App.Current.SavePropertiesAsync();
            ClearDatabase();
        }

        private static HttpClient GetAuthClient()
        {
            if ( authClient == null )
            {
                authClient = new HttpClient();
                authClient.BaseAddress = new Uri( Constants.serverUrl );
            }

            return authClient;
        }

    }
    class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }

    public enum LoginResponse
    {
        Error,
        Success,
        Failure
    }
}