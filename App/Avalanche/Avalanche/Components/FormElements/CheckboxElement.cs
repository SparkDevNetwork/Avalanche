// <copyright>
// Copyright Southeast Christian Church
// Copyright Mark Lee
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
    public class CheckboxElement : IFormElement
    {
        private string check_true = "fa fa-check-square-o";
        private string check_false = "fa fa-square-o";
        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public View View { get; private set; }
        public Color BackgroundColor { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public Color TextColor { get; set; }

        private IconLabel icon;
        private string _value;

        public event EventHandler<string> PostBack;

        public string Value
        {
            get => _value;
            set
            {
                if ( AttributeHelper.IsTrue( value ) )
                {
                    _value = "True";
                    icon.Text = check_true;
                }
                else
                {
                    _value = "False";
                    icon.Text = check_false;
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

            icon = new IconLabel()
            {
                FontSize = 30,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 30,
                Margin = new Thickness( 5 )


            };
            Value = "False";
            stackLayout.Children.Add( icon );

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
            stackLayout.Children.Add( label );

            if ( TextColor != null )
            {
                label.TextColor = TextColor;
                icon.TextColor = TextColor;
            }

            return View;
        }

        private void Tgr_Tapped( object sender, System.EventArgs e )
        {
            if ( AttributeHelper.IsTrue( Value ) )
            {
                Value = "False";
            }
            else
            {
                Value = "True";
            }
        }
    }
}
