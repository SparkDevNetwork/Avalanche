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
using Avalanche.CustomControls;
using Avalanche.Interfaces;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class CheckboxListElement : IFormElement
    {
        private string check_true = "fa fa-check-square-o";
        private string check_false = "fa fa-square-o";
        private List<string> selectedKeys = new List<string>();


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

        public event EventHandler<string> PostBack;

        public string Value
        {
            get => string.Join( ",", selectedKeys );
            set
            {
                //The simplest way to update a value change is to redraw the field
                selectedKeys = value.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).ToList();
                Render();
            }
        }
        public bool IsValid
        {
            get => true;
        }


        public View Render()
        {
            var stackLayout = new StackLayout
            {
                Spacing = 0
            };
            View = stackLayout;

            if ( !string.IsNullOrWhiteSpace( Label ) )
            {
                Label label = new Label
                {
                    Text = Label,
                    Margin = new Thickness( 10, 0, 0, 0 ),
                    FontAttributes = FontAttributes.Bold
                };

                if ( ElementTextColor != null )
                {
                    label.TextColor = ElementTextColor;
                }
                stackLayout.Children.Add( label );
            }

            foreach ( var pair in Options )
            {
                var itemStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };
                stackLayout.Children.Add( itemStack );

                var icon = new IconLabel()
                {
                    FontSize = 30,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    WidthRequest = 30,
                    Margin = new Thickness( 5 )
                };

                if ( selectedKeys.Contains( pair.Key ) )
                {
                    icon.Text = check_true;
                }
                else
                {
                    icon.Text = check_false;
                }
                itemStack.Children.Add( icon );

                Label label = new Label
                {
                    Text = pair.Value,
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };

                if ( ElementTextColor != null )
                {
                    label.TextColor = ElementTextColor;
                    icon.TextColor = ElementTextColor;
                }

                itemStack.Children.Add( label );

                if ( ElementTextColor != null )
                {
                    label.TextColor = ElementTextColor;
                    icon.TextColor = ElementTextColor;
                }

                TapGestureRecognizer tgr = new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1
                };
                tgr.Tapped += ( s, e ) =>
                {
                    if ( selectedKeys.Contains( pair.Key ) )
                    {
                        icon.Text = check_false;
                        selectedKeys.Remove( pair.Key );
                    }
                    else
                    {
                        icon.Text = check_true;
                        selectedKeys.Add( pair.Key );
                    }

                    if ( AutoPostBack )
                    {
                        PostBack?.Invoke( s, Key );
                    }
                };
                itemStack.GestureRecognizers.Add( tgr );
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
