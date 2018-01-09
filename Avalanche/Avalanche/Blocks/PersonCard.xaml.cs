using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class PersonCard : ContentView, IRenderable, IHasBlockMessenger
    {
        private string personGuid;
        public string Image
        {
            get
            {
                return ffImage.Source.ToString();
            }
            set
            {
                ffImage.Source = value;
            }
        }

        public string Name
        {
            get
            {
                return lName.Text;
            }
            set
            {
                lName.Text = value;
            }
        }

        private Color _prevBarColor;
        public Color AccentColor
        {
            get
            {
                return slAccent.BackgroundColor;
            }
            set
            {
                if ( App.Current.MainPage is NavigationPage )
                {
                    ( ( NavigationPage ) App.Current.MainPage ).BarBackgroundColor = value;
                }
                slAccent.BackgroundColor = value;
            }
        }

        public PersonCard()
        {
            InitializeComponent();

            TapGestureRecognizer tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            tgr.Tapped += Tgr_Tapped;

            ffImage.GestureRecognizers.Add( tgr );

            Task.Run( async () =>
            {
                await Task.Delay( 250 );
                App.Current.MainPage.Navigation.NavigationStack[App.Current.MainPage.Navigation.NavigationStack.Count - 1].Disappearing += GroupMemberDetail_Disappearing;
            } );
        }

        private async void Tgr_Tapped( object sender, EventArgs e )
        {
            await CrossMedia.Current.Initialize();

            if ( !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported )
            {
                App.Current.MainPage.DisplayAlert( "No Camera", ":( No camera available.", "OK" );
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync( new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "AvalanchePhotos",
                Name = "person.jpg",
                MaxWidthHeight = 800,
                PhotoSize = PhotoSize.MaxWidthHeight,
                CompressionQuality = 90
            } );

            if ( file == null )
                return;


            ffImage.Source = ImageSource.FromStream( () =>
            {
                var stream = file.GetStream();
                return stream;
            } );

            Task.Run( async () =>
            {
                await Task.Delay( 100 );
                byte[] imgArray;
                using ( var fileStream = file.GetStream() )
                {
                    using ( System.IO.MemoryStream ms = new System.IO.MemoryStream() )
                    {
                        fileStream.CopyTo( ms );
                        file.Dispose();
                        imgArray = ms.ToArray();
                    }
                }

                var body = new Dictionary<string, string>()
            {
                { "Photo", Convert.ToBase64String(imgArray) }
            };
                MessageHandler.Post( personGuid, body );
            } );
        }

        private void GroupMemberDetail_Disappearing( object sender, EventArgs e )
        {
            if ( _prevBarColor != null )
            {
                ( ( NavigationPage ) App.Current.MainPage ).BarBackgroundColor = _prevBarColor;
            }
        }

        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        public View Render()
        {
            if ( Attributes.ContainsKey( "PersonGuid" ) )
            {
                personGuid = Attributes["PersonGuid"];
            }
            return this;
        }
    }
}