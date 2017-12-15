using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class WebViewBlock : ContentView, IRenderable
    {
        public string Domain { get; set; }
        public WebViewBlock()
        {
            InitializeComponent();
            //HeightRequest = App.Current.MainPage.Height;
            //WidthRequest = App.Current.MainPage.Width;
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            wvWebView.Source = Attributes["Url"];
            wvWebView.Navigated += WvWebView_Navigated;
            wvWebView.Navigating += WvWebView_Navigating;

            return this;
        }

        private async void WvWebView_Navigating( object sender, WebNavigatingEventArgs e )
        {
            if ( !Regex.IsMatch( e.Url, Domain ) )
            {
                e.Cancel = true;
                if ( await App.Current.MainPage.DisplayAlert( "Open Web Page", "Would you like to open this link in your browser?", "Yes", "No" ) )
                {
                    Device.OpenUri( new Uri( e.Url ) );
                }
            }
        }

        private void WvWebView_Navigated( object sender, WebNavigatedEventArgs e )
        {
            aiActivity.IsRunning = false;
            aiActivity.IsVisible = false;
        }
    }
}