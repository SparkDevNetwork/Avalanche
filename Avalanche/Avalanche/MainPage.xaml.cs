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

        public MainPage( string resource, string parameter = "" )
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar( this, false );
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            if ( !string.IsNullOrWhiteSpace( parameter ) )
            {
                resource += "/" + parameter;
            }
            Task.Run( () => { Handle_Timeout(); } );
            RockClient.GetResource<MobilePage>( observableResource, "/api/avalanche/" + resource );
        }

        private void ObservableResource_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            HandleResponse();
        }

        private void HandleResponse()
        {
            var page = observableResource.Resource as MobilePage;
            this.Title = page.Title;

            NavigationPage.SetHasNavigationBar( this, page.ShowTitle );
            if ( !page.ShowTitle && Device.RuntimePlatform == Device.iOS )
            {
                this.Padding = new Thickness( 0, 12, 0, 0 );
            }

            var layoutType = Type.GetType( "Avalanche.Layouts." + page.LayoutType.Replace( " ", "" ) );
            if ( layoutType == null )
            {
                AvalancheNavigation.RemovePage();
                return;
            }
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
                    mobileBlock.Attributes = block.Attributes;

                    //Setup postback handler if required
                    if ( mobileBlock is IHasBlockMessenger )
                    {
                        var hasPostbackBlock = mobileBlock as IHasBlockMessenger;
                        hasPostbackBlock.MessageHandler = new BlockMessenger( block.BlockId );
                    }

                    var zone = layout.FindByName<Layout<View>>( block.Zone );
                    if ( zone != null )
                    {
                        try
                        {
                            var renderedBlock = mobileBlock.Render();
                            AttributeHelper.ApplyTranslation( renderedBlock, mobileBlock.Attributes );
                            zone.Children.Add( renderedBlock );
                        }
                        catch ( Exception e )
                        {
                            //
                        }
                    }
                }
            }
            if ( ActivityIndicator.IsRunning == false )
            {
                layout.Opacity = 0;
                MainGrid.Children.Clear();
                MainGrid.Children.Add( layout );
                layout.FadeTo( 1, 500, Easing.CubicInOut );
            }
            else
            {
                MainGrid.Children.Add( layout );
                ActivityIndicator.IsRunning = false;
            }
        }

        private void btnBack_Clicked( object sender, EventArgs e )
        {
            Navigation.PopAsync();
        }

        private async void Handle_Timeout()
        {
            await Task.Delay( Constants.timeout * 1000 );
            if ( ActivityIndicator.IsRunning )
            {
                lTimeout.FadeTo( 1 );
                if ( Navigation.NavigationStack.Count > 1 )
                {
                    btnBack.FadeTo( 1 );
                }
            }
        }
    }
}
