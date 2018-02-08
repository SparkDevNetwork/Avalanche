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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
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

        private string _backgroundImage;
        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                iBackgroundImage.Source = value;
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
            if ( Device.RuntimePlatform == Device.Android )
            {
                btnButton.Opacity = 0;
            }
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
            AttributeHelper.HandleActionItem( Attributes );
        }

        public Dictionary<string, string> Attributes { get; set; }

        public View Render()
        {
            return this;
        }
    }
}