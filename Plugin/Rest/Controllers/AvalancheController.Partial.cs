// <copyright>
// Copyright Southeast Christian Church
// Mark Lee
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Avalanche.Models;
using Avalanche.Transactions;
using Newtonsoft.Json;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Security;
using Rock.Transactions;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Avalanche.Rest.Controllers
{
    public class AvalancheController : ApiControllerBase
    {
        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/home" )]
        public HomeRequest GetHome()
        {
            var homeRequest = new HomeRequest();

            var footer = GlobalAttributesCache.Value( "AvalancheFooterPage" ).AsIntegerOrNull();
            if ( footer != null )
            {
                homeRequest.Footer = GetPage( footer.Value );
            }

            var header = GlobalAttributesCache.Value( "AvalancheHeaderPage" ).AsIntegerOrNull();
            if ( header != null )
            {
                homeRequest.Header = GetPage( header.Value );
            }

            homeRequest.Page = GetPage( GlobalAttributesCache.Value( "AvalancheHomePage" ).AsInteger() );
            return homeRequest;
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/page/{id}" )]
        [System.Web.Http.Route( "api/avalanche/page/{id}/{*parameter}" )]
        public MobilePage GetPage( int id, string parameter = "" )
        {
            var person = GetPerson();
            if ( !HttpContext.Current.Items.Contains( "CurrentPerson" ) )
            {
                HttpContext.Current.Items.Add( "CurrentPerson", person );
            }

            var pageCache = PageCache.Read( id );
            if ( !pageCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                return new MobilePage();
            }

            SavePageViewInteraction( pageCache, person );

            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            MobilePage mobilePage = new MobilePage();
            mobilePage.Layout = AvalancheUtilities.GetLayout( pageCache.Layout.Name );
            mobilePage.Title = pageCache.PageTitle;
            mobilePage.ShowTitle = pageCache.PageDisplayTitle;
            foreach ( var attribute in pageCache.AttributeValues )
            {
                mobilePage.Attributes.Add( attribute.Key, attribute.Value.ValueFormatted );
            }
            foreach ( var block in pageCache.Blocks )
            {
                if ( block.IsAuthorized( Authorization.VIEW, person ) )
                {
                    var blockCache = BlockCache.Read( block.Id );
                    try
                    {
                        var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );
                        if ( control is RockBlock && control is IMobileResource )
                        {
                            control.SetBlock( pageCache, blockCache );
                            var mobileResource = control as IMobileResource;
                            var mobileBlock = mobileResource.GetMobile( parameter );
                            mobileBlock.BlockId = blockCache.Id;
                            mobileBlock.Zone = blockCache.Zone;
                            mobilePage.Blocks.Add( mobileBlock );
                        }
                    }
                    catch ( Exception e )
                    {
                        ExceptionLogService.LogException( e, HttpContext.Current );
                    }
                }
            }
            HttpContext.Current.Response.Headers.Set( "TTL", pageCache.OutputCacheDuration.ToString() );
            return mobilePage;
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*request}" )]
        public MobileBlockResponse BlockGetRequest( int id, string request = "" )
        {
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );
            var blockCache = BlockCache.Read( id );
            var pageCache = PageCache.Read( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                if ( control is RockBlock && control is IMobileResource )
                {
                    control.SetBlock( pageCache, blockCache );
                    var mobileResource = control as IMobileResource;
                    var mobileBlockResponse = mobileResource.HandleRequest( request, new Dictionary<string, string>() );
                    HttpContext.Current.Response.Headers.Set( "TTL", mobileBlockResponse.TTL.ToString() );
                    return mobileBlockResponse;
                }
            }
            HttpContext.Current.Response.Headers.Set( "TTL", "0" );
            return new MobileBlockResponse();
        }


        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*arg}" )]
        public MobileBlockResponse BlockPostRequest( int id, string arg = "" )
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>( content );
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );
            var blockCache = BlockCache.Read( id );
            var pageCache = PageCache.Read( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                if ( control is RockBlock && control is IMobileResource )
                {
                    control.SetBlock( pageCache, blockCache );
                    var mobileResource = control as IMobileResource;
                    var mobileBlockResponse = mobileResource.HandleRequest( arg, body );
                    mobileBlockResponse.TTL = 0;
                    return mobileBlockResponse;
                }
            }
            return new MobileBlockResponse();
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/token" )]
        public RckipidToken GetToken()
        {
            var person = GetPerson();
            if ( person == null )
            {
                return null;
            }
            var expiration = Rock.RockDateTime.Now.AddDays( 7 );
            var token = PersonToken.CreateNew( person.PrimaryAlias, expiration, 1, null );
            return new RckipidToken
            {
                Expiration = expiration,
                Token = token
            };
        }

        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/interaction" )]
        public Dictionary<string, string> PostInteraction()
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            InteractionInformation interactionInformation = JsonConvert.DeserializeObject<InteractionInformation>( content );

            var homePageId = GlobalAttributesCache.Value( "AvalancheHomePage" ).AsInteger();
            var pageCache = PageCache.Read( homePageId );
            var siteId = pageCache.SiteId;
            var person = GetPerson();

            AppInteractionTransaction transaction = new AppInteractionTransaction()
            {
                ComponentName = "Mobile App",
                SiteId = siteId,
                PageId = interactionInformation.PageId.AsIntegerOrNull(),
                PageTitle = interactionInformation.PageTitle,
                DateViewed = Rock.RockDateTime.Now,
                Operation = interactionInformation.Operation,
                PersonAliasId = person?.PrimaryAliasId,
                InteractionData = interactionInformation.InteractionData,
                InteractionSummary = interactionInformation.InteractionSummary,
                IPAddress = GetClientIp( Request ),
                UserAgent = Request.Headers.UserAgent.ToString()
            };
            RockQueue.TransactionQueue.Enqueue( transaction );
            return new Dictionary<string, string> { { "Status", "Ok" } };
        }

        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/registerfcm" )]
        public Dictionary<string, string> RegisterFCM()
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>( content );
            var person = GetPerson();

            if ( tokenDictionary.ContainsKey( "Token" )
                 && !string.IsNullOrWhiteSpace( tokenDictionary["Token"] )
                 && person != null )
            {
                var userAgent = Request.Headers.UserAgent.ToString();
                var deviceId = Regex.Match( userAgent, "(?<=-).+(?=\\))" ).Value.Trim();
                if ( deviceId.Length > 20 )
                {
                    deviceId = deviceId.Substring( 0, 20 );
                }
                RockContext rockContext = new RockContext();
                PersonalDevice personalDevice = AvalancheUtilities.GetPersonalDevice( deviceId, person.PrimaryAliasId, rockContext );
                if ( personalDevice != null && personalDevice.DeviceRegistrationId != tokenDictionary["Token"] )
                {
                    personalDevice.DeviceRegistrationId = tokenDictionary["Token"];
                    personalDevice.NotificationsEnabled = true;
                    rockContext.SaveChanges();
                }

            }
            return new Dictionary<string, string> { { "Status", "Ok" } };
        }

        private void SavePageViewInteraction( PageCache Page, Person CurrentPerson )
        {
            AppInteractionTransaction transaction = new AppInteractionTransaction()
            {
                PageId = Page.Id,
                SiteId = Page.SiteId,
                DateViewed = Rock.RockDateTime.Now,
                PageTitle = Page.PageTitle,
                Operation = "View",
                PersonAliasId = CurrentPerson?.PrimaryAliasId,
                InteractionData = Request.RequestUri.ToString(),
                IPAddress = GetClientIp( Request ),
                UserAgent = Request.Headers.UserAgent.ToString()
            };
            RockQueue.TransactionQueue.Enqueue( transaction );
        }

        private string GetClientIp( HttpRequestMessage request )
        {
            if ( request.Properties.ContainsKey( "MS_HttpContext" ) )
            {
                return ( ( HttpContextWrapper ) request.Properties["MS_HttpContext"] ).Request.UserHostAddress;
            }

            if ( request.Properties.ContainsKey( RemoteEndpointMessageProperty.Name ) )
            {
                RemoteEndpointMessageProperty prop;
                prop = ( RemoteEndpointMessageProperty ) request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }
    }
}