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
using Avalanche.Utilities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class AddressElement : IFormElement
    {
        private Entry Street1;
        private Entry Street2;
        private Entry City;
        private Picker State;
        private Entry PostalCode;

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
        public string Value
        {
            get
            {
                if ( View != null )
                {
                    var locationData = new Dictionary<string, string>
                    {
                        {"Street1", Street1.Text },
                        {"Street2", Street2.Text },
                        {"City", City.Text },
                        {"State", State.SelectedItem.ToString() },
                        {"PostalCode", PostalCode.Text }
                    };

                    return JsonConvert.SerializeObject( locationData );
                }
                var blankData = new Dictionary<string, string>
                    {
                        {"Street1", "" },
                        {"Street2", "" },
                        {"City", "" },
                        {"State", "" },
                        {"PostalCode", "" }
                    };
                return JsonConvert.SerializeObject( blankData );
            }
            set
            {
                if ( View != null )
                {
                    try
                    {
                        var locationData = JsonConvert.DeserializeObject<Dictionary<string, string>>( value );

                        if ( locationData.ContainsKey( "Street1" ) )
                        {
                            Street1.Text = locationData["Street1"];
                        }

                        if ( locationData.ContainsKey( "Street2" ) )
                        {
                            Street2.Text = locationData["Street2"];
                        }

                        if ( locationData.ContainsKey( "City" ) )
                        {
                            City.Text = locationData["City"];
                        }

                        if ( locationData.ContainsKey( "State" ) && State.SelectedItem.ToString() == locationData["State"] )
                        {
                            State.SelectedItem = locationData["State"];
                        }

                        if ( locationData.ContainsKey( "PostalCode" ) )
                        {
                            PostalCode.Text = locationData["PostalCode"];
                        }
                    }
                    catch
                    {

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
                    if ( !string.IsNullOrWhiteSpace( Street1.Text ) && !string.IsNullOrWhiteSpace( PostalCode.Text ) )
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
            StackLayout addressStack = new StackLayout()
            {
                Spacing = 0,
                Margin = new Thickness( 5 )
            };

            if ( !string.IsNullOrWhiteSpace( Label ) )
            {
                Label label = new Label
                {
                    Text = Label,
                    Margin = new Thickness( 5, 0, 0, 5 ),
                    FontAttributes = FontAttributes.Bold
                };

                if ( ElementTextColor != null )
                {
                    label.TextColor = ElementTextColor;
                }
                addressStack.Children.Add( label );
            }

            Street1 = new Entry()
            {
                Placeholder = "Street"
            };
            addressStack.Children.Add( Street1 );

            Street2 = new Entry()
            {
                Placeholder = "Street 2 (Optional)"
            };
            addressStack.Children.Add( Street2 );

            City = new Entry()
            {
                Placeholder = "City"
            };
            addressStack.Children.Add( City );

            List<string> states = new List<string>();

            if ( Attributes.ContainsKey( "States" ) )
            {
                states = Attributes["States"].Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            }

            State = new Picker()
            {
                ItemsSource = states,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            if ( Attributes.ContainsKey( "DefaultState" ) && states.Contains( Attributes["DefaultState"] ) )
            {
                State.SelectedItem = Attributes["DefaultState"];
            }

            PostalCode = new Entry()
            {
                Placeholder = "Postal Code",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var horizontalStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };

            if ( ElementTextColor != null )
            {
                Street1.TextColor = ElementTextColor;
                Street2.TextColor = ElementTextColor;
                City.TextColor = ElementTextColor;
                State.TextColor = ElementTextColor;
                PostalCode.TextColor = ElementTextColor;
            }

            if ( ElementBackgroundColor != null )
            {
                Street1.BackgroundColor = ElementBackgroundColor;
                Street2.BackgroundColor = ElementBackgroundColor;
                City.BackgroundColor = ElementBackgroundColor;
                State.BackgroundColor = ElementBackgroundColor;
                PostalCode.BackgroundColor = ElementBackgroundColor;
            }

            horizontalStack.Children.Add( State );
            horizontalStack.Children.Add( PostalCode );
            addressStack.Children.Add( horizontalStack );

            View = addressStack;
            return addressStack;
        }
    }
}
