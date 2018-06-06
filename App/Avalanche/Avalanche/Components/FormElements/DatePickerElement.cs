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
using Avalanche.Interfaces;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class DatePickerElement : IFormElement
    {
        private DatePicker datePicker;
        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public bool IsVisualOnly { get; } = false;
        public bool AutoPostBack { get; set; } = false;
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public View View { get; private set; }
        public string Value
        {
            get
            {
                if ( datePicker != null )
                {
                    return datePicker.Date.ToString();
                }
                return "";
            }
            set
            {
                if ( datePicker != null )
                {
                    DateTime dateTime;
                    var success = DateTime.TryParse( value, out dateTime );
                    if ( success )
                    {
                        datePicker.Date = dateTime;
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
                    if ( datePicker != null && datePicker.Date != DateTime.MinValue )
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
            StackLayout stackLayout = new StackLayout()
            {
                Margin = new Thickness( 5 )
            };

            Label label = new Label()
            {
                Text = Label
            };

            stackLayout.Children.Add( label );
            datePicker = new DatePicker();
            stackLayout.Children.Add( datePicker );

            if ( BackgroundColor != null )
            {
                datePicker.BackgroundColor = BackgroundColor;
            }

            if ( TextColor != null )
            {
                datePicker.TextColor = TextColor;
            }

            datePicker.DateSelected += ( s, e ) =>
            {
                if ( AutoPostBack )
                {
                    PostBack?.Invoke( s, Key );
                }
            };

            View = stackLayout;
            return View;
        }
    }
}
