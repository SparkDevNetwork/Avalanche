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
        private Dictionary<string, string> reversedOptions = new Dictionary<string, string>();
        private List<string> optionList = new List<string>();

        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public bool IsVisualOnly { get; } = false;
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public View View { get; private set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Value
        {
            get
            {
                if ( View != null )
                {
                    var key = ( string ) ( ( Picker ) View ).SelectedItem;
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
                if ( View != null )
                {
                    if ( reversedOptions.ContainsValue( value ) )
                    {
                        var optionKey = reversedOptions.Where( ro => ro.Value == value ).FirstOrDefault();

                        ( ( Picker ) View ).SelectedItem = optionKey.Key;
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
                    if ( !string.IsNullOrWhiteSpace( ( string ) ( ( Picker ) View ).SelectedItem ) )
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


            View = new Picker()
            {
                ItemsSource = optionList,
                Title = Label,
                Margin = new Thickness( 5 )
            };

            if ( BackgroundColor != null )
            {
                View.BackgroundColor = BackgroundColor;
            }

            if ( TextColor != null )
            {
                ( ( Picker ) View ).TextColor = TextColor;
            }

            return View;
        }
    }
}
