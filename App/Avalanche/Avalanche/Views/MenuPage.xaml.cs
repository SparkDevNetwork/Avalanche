using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Interfaces;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Views
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class MenuPage : ContentPage
    {

        public View Menu { get; set; }
        public MenuPage( MobilePage page )
        {
            InitializeComponent();

            var layoutManager = new Utilities.LayoutManager( page.Layout );
            var layout = layoutManager.Content;

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

                    var zone = layoutManager.GetElement( block.Zone );

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
            MainStackLayout.Children.Add( layout );
            Menu = layout;
        }

        protected override void OnSizeAllocated( double width, double height )
        {
            base.OnSizeAllocated( width, height );
            AvalancheNavigation.AllowResize = true;
        }
    }
}