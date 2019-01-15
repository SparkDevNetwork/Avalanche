// <copyright>
// Copyright Southeast Christian Church

//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Avalanche;
using Avalanche.Blocks;
using Avalanche.Utilities;
using Avalanche.Models;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Avalanche.CustomControls;
using Avalanche.Interfaces;
using Newtonsoft.Json;

namespace Avalanche.Views
{
    public partial class MainPage : ContentPage
    {
        public string Resource { get; set; }
        public string Parameter { get; set; }

        public ObservableResource<MobilePage> observableResource = new ObservableResource<MobilePage>();
        private List<IHasMedia> mediaBlocks = new List<IHasMedia>();
        private List<INotify> notifyBlock = new List<INotify>();
        private List<View> blocks = new List<View>();
        private StackLayout nav;
        private LayoutManager layoutManager;

        private string _backgroundImage;
        public new string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                AddBackgroundImage();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            Task.Run( () => { Handle_Timeout(); } );
        }

        public MainPage( string resource, string parameter = "" )
        {
            InitializeComponent();
            Resource = resource;
            Parameter = parameter;
            observableResource.PropertyChanged += ObservableResource_PropertyChanged;
            Task.Run( () => { Handle_Timeout(); } );
            if ( !string.IsNullOrWhiteSpace( parameter ) )
            {
                resource += "/" + parameter;
            }
            RockClient.GetResource<MobilePage>( observableResource, "/api/avalanche/" + resource );
            Content.Margin = new Thickness(
                AvalancheNavigation.SafeInset.Left,
                AvalancheNavigation.SafeInset.Top + AvalancheNavigation.YOffSet,
                AvalancheNavigation.SafeInset.Right,
                AvalancheNavigation.SafeInset.Bottom );
        }

        private void ObservableResource_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            HandleResponse();
        }


        private void HandleResponse()
        {
            foreach ( var block in notifyBlock )
            {
                block.OnDisappearing();
            }
            notifyBlock.Clear();
            mediaBlocks.Clear();

            var page = observableResource.Resource as MobilePage;
            this.Title = page.Title;

            layoutManager = new Utilities.LayoutManager( page.Layout );
            var layout = layoutManager.Content;

            if ( page.ShowTitle )
            {
                AddTitleBar( layout, page );
            }

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
                            //Get blocks that need notfication of appearing and disappearing
                            if ( renderedBlock is INotify )
                            {
                                notifyBlock.Add( ( INotify ) renderedBlock );
                            }

                            //Setup media if needed
                            if ( renderedBlock is IHasMedia )
                            {
                                var mediaBlock = ( IHasMedia ) renderedBlock;
                                mediaBlocks.Add( mediaBlock );
                                mediaBlock.FullScreenChanged += MediaBlock_FullScreenChanged;
                            }
                            blocks.Add( renderedBlock );
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
                AddBackgroundImage();
                MainGrid.Children.Add( layout );
                layout.FadeTo( 1, 500, Easing.CubicInOut );
            }
            else
            {
                MainGrid.Children.Add( layout );
                ActivityIndicator.IsRunning = false;
            }
            btnBack.IsVisible = false;
            lTimeout.IsVisible = false;
            OnAppearing(); //iOS fires OnAppearing before the page can be rendered.
        }
        public void AddBackgroundImage()
        {
            var mainGrid = this.FindByName<Grid>( "MainGrid" );
            if ( mainGrid != null )
            {
                var image = new FFImageLoading.Forms.CachedImage()
                {
                    Aspect = Aspect.AspectFill,
                    Source = _backgroundImage

                };
                mainGrid.Children.Add( image );
            }
        }


        MenuPage footer;
        double yOffset;
        private void MediaBlock_FullScreenChanged( object sender, bool isFullScreen )
        {
            foreach ( var block in blocks )
            {
                if ( isFullScreen )
                {
                    HideView( block );
                }
                else
                {
                    ShowView( block );
                }
                ShowView( ( View ) ( sender ) );
            }


            if ( nav != null )
            {
                nav.IsVisible = !isFullScreen;
            }

            if ( isFullScreen )
            {
                ( ( View ) sender ).VerticalOptions = LayoutOptions.FillAndExpand;
            }

            if ( AvalancheNavigation.Footer != null || footer != null )
            {
                if ( isFullScreen )
                {
                    footer = AvalancheNavigation.Footer;
                    yOffset = AvalancheNavigation.YOffSet;
                    AvalancheNavigation.Footer = null;
                    Content.Margin = new Thickness(
                        AvalancheNavigation.SafeInset.Left,
                        AvalancheNavigation.SafeInset.Top,
                        AvalancheNavigation.SafeInset.Right,
                        AvalancheNavigation.SafeInset.Bottom );
                    ( ( View ) sender ).HeightRequest = Content.Width;
                }
                else
                {
                    AvalancheNavigation.Footer = footer;
                    footer = null;
                    Content.Margin = new Thickness(
                        AvalancheNavigation.SafeInset.Left,
                        AvalancheNavigation.SafeInset.Top + yOffset,
                        AvalancheNavigation.SafeInset.Right,
                        AvalancheNavigation.SafeInset.Bottom );
                    ( ( View ) sender ).HeightRequest = -1;
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            bool back = false;
            foreach ( var mediaBlock in mediaBlocks )
            {
                if ( mediaBlock.IsFullScreen )
                {
                    mediaBlock.BackButtonPressed();
                    back = true;
                }
            }
            return back;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            foreach ( var block in notifyBlock )
            {
                block.OnDisappearing();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach ( var block in notifyBlock )
            {
                block.OnAppearing();
            }
        }

        private void AddTitleBar( ContentView layout, MobilePage page )
        {
            nav = ( StackLayout ) layoutManager.GetElement( "NavBar" );
            if ( nav != null )
            {
                nav.IsVisible = true;
                if ( page.Attributes.ContainsKey( "AccentColor" ) && !string.IsNullOrEmpty( page.Attributes["AccentColor"] ) )
                {
                    nav.BackgroundColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( page.Attributes["AccentColor"] );
                }

                StackLayout stackLayout = new StackLayout
                {
                    HeightRequest = 44,
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center
                };

                nav.Children.Add( stackLayout );

                if ( App.Navigation.Navigation.NavigationStack.Count > 1 )
                {
                    IconLabel icon = new IconLabel
                    {
                        Text = "fa fa-chevron-left",
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 30,
                        Margin = new Thickness( 10, 7, 0, 0 ),
                        TextColor = Color.Black
                    };

                    TapGestureRecognizer tgr = new TapGestureRecognizer()
                    {
                        NumberOfTapsRequired = 1
                    };
                    tgr.Tapped += ( s, ee ) =>
                    {
                        AvalancheNavigation.RemovePage();
                    };
                    nav.GestureRecognizers.Add( tgr );

                    stackLayout.Children.Add( icon );
                }

                Label label = new Label
                {
                    Text = page.Title,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 20,
                    Margin = new Thickness( 0, 6, 0, 0 ),
                    TextColor = Color.Black

                };
                stackLayout.Children.Add( label );

                BoxView boxview = new BoxView
                {
                    HeightRequest = 0.5,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Black
                };
                nav.Children.Add( boxview );

            }
        }

        private void HideView( Xamarin.Forms.VisualElement view )
        {
            if ( view.Parent is Xamarin.Forms.VisualElement )
            {
                view.IsVisible = false;
                var parent = ( Xamarin.Forms.VisualElement ) view.Parent;
                if ( parent != null )
                {
                    HideView( parent );
                }
            }
        }

        private void ShowView( Xamarin.Forms.VisualElement view )
        {
            if ( view.Parent is Xamarin.Forms.VisualElement )
            {
                view.IsVisible = true;
                var parent = ( Xamarin.Forms.VisualElement ) view.Parent;
                if ( parent != null )
                {
                    ShowView( parent );
                }
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
