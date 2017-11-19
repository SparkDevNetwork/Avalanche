using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Xamarin.Forms;
using org.secc.Avalanche.Models;
using org.secc.Avalanche;
using Avalanche.Blocks;
using Avalanche.Utilities;

namespace Avalanche
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            throw new NotImplementedException();
        }

        public MainPage( string resource )
        {
            InitializeComponent();
            var client = new RestClient( Constants.serverUrl );
            var request = new RestRequest( "/api/org.secc/avalanche/" + resource, Method.GET );
            request.AddHeader( "Accept", "application/json" );
            var response = client.Execute<MobilePage>( request );
            var page = response.Data;
            var layoutType = Type.GetType( "Avalanche.Layouts." + page.LayoutType.Replace( " ", "" ) );
            var layout = ( ContentView ) Activator.CreateInstance( layoutType );

            foreach ( var child in page.Blocks )
            {
                IRenderable mobileBlock = ( IRenderable ) Activator.CreateInstance( Type.GetType( child.BlockType ) );
                mobileBlock.Attributes = child.Body;
                var zone = layout.FindByName<Layout<View>>( child.Zone );
                zone.Children.Add( mobileBlock.Render() );
            }
            Content = layout;
        }


    }
}
