using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class MobileContentItem : ContentView, IRenderable
    {
        public MobileContentItem()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            if ( Attributes.ContainsKey( "Image" ) )
            {
                ffImage.Source = Attributes["Image"];
            }
            if ( Attributes.ContainsKey( "Markdown" ) )
            {
                mdMarkdown.Markdown = Attributes["Markdown"];
            }

            Task.Run( async () =>
            {
                await Task.Delay( 200 );
                if ( Attributes.ContainsKey( "Title" ) )
                {
                    App.Current.MainPage.Navigation.NavigationStack[App.Current.MainPage.Navigation.NavigationStack.Count - 1].Title = Attributes["Title"];
                }
            }
            );

            return this;
        }
    }
}