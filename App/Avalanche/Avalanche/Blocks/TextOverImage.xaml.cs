using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Interfaces;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class TextOverImage : ContentView, IRenderable
    {
        public string Text { get => lLabel.Text; set => lLabel.Text = value; }
        public double FontSize { get => lLabel.FontSize; set => lLabel.FontSize = value; }
        public string FontFamily { get => lLabel.FontFamily; set => lLabel.FontFamily = value; }
        public Color TextColor { get => lLabel.TextColor; set => lLabel.TextColor = value; }
        public ImageSource Source { get => ffImage.Source; set => ffImage.Source = value; }
        public ImageSource LoadingPlaceholder { get => ffImage.LoadingPlaceholder; set => ffImage.LoadingPlaceholder = value; }

        public TextOverImage()
        {
            InitializeComponent();

        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            SetAspectRatio();
            ffImage.Finish += FfImage_Finish;
            ffImage.DownloadStarted += FfImage_DownloadStarted;



            TapGestureRecognizer tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            tgr.Tapped += Tgr_Tapped;
            this.GestureRecognizers.Add( tgr );

            return this;
        }

        private void FfImage_DownloadStarted( object sender, CachedImageEvents.DownloadStartedEventArgs e )
        {
            SetAspectRatio();
        }

        private void SetAspectRatio()
        {
            try
            {
                double aspect = 0.5;
                if ( Attributes.ContainsKey( "AspectRatio" ) && !string.IsNullOrWhiteSpace( Attributes["AspectRatio"] ) )
                {
                    aspect = Convert.ToDouble( Attributes["AspectRatio"] );
                }
                ffImage.HeightRequest = App.Current.MainPage.Width * aspect;
            }
            catch { }
        }

        private void FfImage_Finish( object sender, CachedImageEvents.FinishEventArgs e )
        {
            SetAspectRatio();
        }

        private void Tgr_Tapped( object sender, EventArgs e )
        {
            AvalancheNavigation.HandleActionItem( Attributes );
        }

    }
}