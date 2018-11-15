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
using Avalanche.CustomControls;
using Avalanche.Interfaces;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class SwitchElement : IFormElement
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public bool IsVisualOnly { get; } = false;
        public bool AutoPostBack { get; set; } = false;
        public View View { get; private set; }
        public Color ElementBackgroundColor { get; set; }
        public Color ElementTextColor { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        private Switch toggle;
        private string _value;

        public event EventHandler<string> PostBack;

        public string Value
        {
            get => toggle.IsToggled.ToString();
            set
            {
                if ( AttributeHelper.IsTrue( value ) )
                {
                    toggle.IsToggled = true;
                }
                else
                {
                    toggle.IsToggled = false;
                }
            }
        }
        public bool IsValid
        {
            get => true;
        }


        public View Render()
        {
            var stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            View = stackLayout;

            toggle = new Switch()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness( 5 ),
            };
            Value = "False";
            stackLayout.Children.Add( toggle );

            toggle.Toggled += Toggle_Toggled;

            TapGestureRecognizer tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1
            };
            tgr.Tapped += Tgr_Tapped;
            stackLayout.GestureRecognizers.Add( tgr );

            Label label = new Label
            {
                Text = Label,
                FontSize = 20,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            if ( ElementTextColor != null )
            {
                label.TextColor = ElementTextColor;
            }

            stackLayout.Children.Add( label );

            return View;
        }

        private void Toggle_Toggled( object sender, ToggledEventArgs e )
        {
            if ( AutoPostBack )
            {
                PostBack?.Invoke( sender, Key );
            }
        }

        private void Tgr_Tapped( object sender, System.EventArgs e )
        {
            toggle.IsToggled = !toggle.IsToggled;
        }
    }
}
