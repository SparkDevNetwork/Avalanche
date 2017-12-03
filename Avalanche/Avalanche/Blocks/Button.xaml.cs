using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class Button : ContentView, IRenderable
    {
        private string resource;
        public Button()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            btnButton.Text = Attributes["Text"];
            resource = "page/" + Attributes["PageNumber"];
            return this;
        }

        private void btnButton_Clicked( object sender, EventArgs e )
        {
            AvalancheNavigation.GetPage( resource );
        }   
    }
}