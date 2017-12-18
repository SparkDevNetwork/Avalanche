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
    public partial class IconButton : ContentView, IRenderable
    {
        public string Icon
        {
            get
            {
                return iIcon.Text;
            }
            set
            {
                iIcon.Text = value;
            }
        }

        public string Text
        {
            get
            {
                return lbLabel.Text;
            }
            set
            {
                lbLabel.Text = value;
            }
        }

        public double FontSize
        {
            get
            {
                return lbLabel.FontSize;
            }
            set
            {
                iIcon.FontSize = value * 1.2;
                lbLabel.FontSize = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return iIcon.TextColor;
            }
            set
            {
                lbLabel.TextColor = value;
                iIcon.TextColor = value;
            }
        }

        public FontAttributes FontAttributes
        {
            get
            {
                return iIcon.FontAttributes;
            }
            set
            {
                iIcon.FontAttributes = value;
                lbLabel.FontAttributes = value;
            }
        }

        public IconButton()
        {
            InitializeComponent();
            btnButton.Clicked += BtnButton_Clicked;
            btnButton.Pressed += BtnButton_Pressed;
            btnButton.Released += BtnButton_Released;
        }

        private void BtnButton_Released( object sender, EventArgs e )
        {
            this.TranslationY = 0;
        }

        private void BtnButton_Pressed( object sender, EventArgs e )
        {
            this.TranslationY = 0.5;
        }

        private void BtnButton_Clicked( object sender, EventArgs e )
        {
            if ( Attributes.ContainsKey( "PageNumber" ) )
            {
                if ( Attributes.ContainsKey( "Argument" ) )
                {
                    AvalancheNavigation.GetPage( Attributes["PageNumber"], Attributes["Argument"] );
                }
                else
                {
                    AvalancheNavigation.GetPage( Attributes["PageNumber"] );
                }
            }
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return this;
        }
    }
}