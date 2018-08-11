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
using Avalanche.Interfaces;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class PickerElement : IFormElement
    {
        private Picker picker;

        private Dictionary<string, string> reversedOptions = new Dictionary<string, string>();
        private List<string> optionList = new List<string>();

        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public bool IsVisualOnly { get; } = false;
        public bool AutoPostBack { get; set; } = false;
        public Color ElementBackgroundColor { get; set; }
        public Color ElementTextColor { get; set; }
        public View View { get; private set; }
        public Dictionary<string, string> Attributes { get; set; }
        private string originalValue = "";
        public string Value
        {
            get
            {
                if ( picker != null )
                {
                    var key = ( string ) picker.SelectedItem;
                    if ( key == null )
                    {
                        return "";
                    }
                    if ( reversedOptions.ContainsKey( key ) )
                    {
                        return reversedOptions[key];
                    }
                    else
                    {
                        return key;
                    }
                }
                return "";
            }
            set
            {
                if ( picker != null )
                {
                    if ( reversedOptions.ContainsValue( value ) )
                    {
                        var optionKey = reversedOptions.Where( ro => ro.Value == value ).FirstOrDefault();
                        picker.SelectedItem = optionKey.Key;
                        originalValue = optionKey.Key;
                    }
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if ( Required )
                {
                    if ( !string.IsNullOrWhiteSpace( ( string ) picker.SelectedItem ) )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public event EventHandler<string> PostBack;

        public View Render()
        {
            // In Xamarin pickers (aka dropdown list) are just a list of options
            // We need to have a key value type list to work with Rock
            // So the strat is to swap keys and values
            // and insure no two values are the same

            foreach ( var pair in Options )
            {
                var optionKey = pair.Value;
                var counter = 1;
                while ( optionList.Contains( optionKey ) )
                {
                    optionKey = string.Format( "{0} ({1})", pair.Value, counter );
                    counter++;
                }
                optionList.Add( optionKey );
                reversedOptions.Add( optionKey, pair.Key );
            }

            StackLayout stackLayout = new StackLayout()
            {
                Margin = new Thickness( 5 )
            };
            View = stackLayout;

            if ( !string.IsNullOrWhiteSpace( Label ) )
            {
                Label label = new Label
                {
                    Text = Label,
                    Margin = new Thickness( 5, 0, 0, 0 ),
                    FontAttributes = FontAttributes.Bold
                };

                if ( ElementTextColor != null )
                {
                    label.TextColor = ElementTextColor;
                }
                stackLayout.Children.Add( label );
            }

            picker = new Picker()
            {
                ItemsSource = optionList,
            };
            if ( Required )
            {
                picker.Title = "(Required)";
            }
            stackLayout.Children.Add( picker );

            if ( ElementBackgroundColor != null )
            {
                picker.BackgroundColor = ElementBackgroundColor;
            }

            if ( ElementTextColor != null )
            {
                picker.TextColor = ElementTextColor;
            }

            picker.Unfocused += ( s, e ) =>
                    {
                        if ( AutoPostBack  &&( (string) picker.SelectedItem) != originalValue)
                        {
                            PostBack?.Invoke( s, Key );
                        }
                    };
            return View;
        }
    }
}