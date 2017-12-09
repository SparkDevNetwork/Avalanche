using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Avalanche.Models;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Avalanche.Rest.Controllers
{
    public class AvalancheController : ApiControllerBase
    {
        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/page/{id}" )]
        [System.Web.Http.Route( "api/avalanche/page/{id}/{*arg}" )]
        public MobilePage GetPage( int id, string arg = "" )
        {
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );

            var pageCache = PageCache.Read( id );
            if ( !pageCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                return new MobilePage();
            }
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            MobilePage mobilePage = new MobilePage();
            mobilePage.LayoutType = pageCache.Layout.Name;
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
                    var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                    if ( control is RockBlock && control is IMobileResource )
                    {
                        control.SetBlock( pageCache, blockCache );
                        var mobileResource = control as IMobileResource;
                        var mobileBlock = mobileResource.GetMobile( arg );
                        mobileBlock.BlockId = blockCache.Id;
                        mobileBlock.Zone = blockCache.Zone;
                        mobilePage.Blocks.Add( mobileBlock );
                    }
                }
            }
            HttpContext.Current.Response.Headers.Set( "TTL", pageCache.OutputCacheDuration.ToString() );
            return mobilePage;
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*arg}" )]
        public MobileBlockResponse BlockGetRequest( int id, string arg = "" )
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
                    var mobileBlockResponse = mobileResource.HandleRequest( arg, new Dictionary<string, string>() );
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
            var collection = HttpUtility.ParseQueryString( content );
            var body = new Dictionary<string, string>();
            foreach ( var k in collection.AllKeys )
            {
                body.Add( k, collection[k] );
            }

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
    }
}