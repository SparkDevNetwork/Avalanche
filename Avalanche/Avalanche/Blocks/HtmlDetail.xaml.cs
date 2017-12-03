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
    public partial class HtmlDetail : ContentView, IRenderable
    {
        public HtmlDetail()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            var html = Attributes["Content"];
            var htmlSource = new HtmlWebViewSource() { Html = html };
            wvContent.Source = html;
            return this;
        }
    }
}