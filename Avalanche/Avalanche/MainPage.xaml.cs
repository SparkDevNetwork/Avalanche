using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Xamarin.Forms;
using Avalanche;
using Avalanche.Blocks;
using Avalanche.Utilities;
using Avalanche.Models;

namespace Avalanche
{
    public partial class MainPage : AvalanchePage
    {
        ObservableResource<MobilePage> observableResource = new ObservableResource<MobilePage>();

        public MainPage()
        {
            throw new NotImplementedException();
        }

        public MainPage( string resource )
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar( this, false );
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            RockClient.GetResource<MobilePage>( observableResource, "/api/avalanche/" + resource );
        }

        private void ObservableResource_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            HandleResponse();
        }

        private void HandleResponse()
        {
            var page = observableResource.Resource as MobilePage;
            var layoutType = Type.GetType( "Avalanche.Layouts." + page.LayoutType.Replace( " ", "" ) );
            var layout = ( ContentView ) Activator.CreateInstance( layoutType );

            //Modify the page with attributes
            AttributeHelper.ApplyTranslation( this, page.Attributes );

            //Put blocks into the layout
            foreach ( var block in page.Blocks )
            {
                var blockType = Type.GetType( block.BlockType );
                if ( blockType != null )
                {
                    IRenderable mobileBlock = ( IRenderable ) Activator.CreateInstance( blockType );
                    mobileBlock.Attributes = block.Body;

                    //Setup postback handler if required
                    if ( mobileBlock is IHasBlockMessenger )
                    {
                        var hasPostbackBlock = mobileBlock as IHasBlockMessenger;
                        hasPostbackBlock.MessageHandler = new BlockMessenger( block.BlockId );
                    }

                    var zone = layout.FindByName<Layout<View>>( block.Zone );
                    if ( zone != null )
                    {
                        var renderedBlock = mobileBlock.Render();
                        AttributeHelper.ApplyTranslation( renderedBlock, mobileBlock.Attributes );
                        zone.Children.Add( renderedBlock );
                    }
                }
            }
            if ( ActivityIndicator.IsRunning == false )
            {
                layout.Opacity = 0;
                MainGrid.Children.Add( layout );
                layout.FadeTo( 1, 500, Easing.CubicInOut );
            }
            else
            {
                MainGrid.Children.Add( layout );
                ActivityIndicator.IsRunning = false;
            }
        }


    }
}
