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
    public class ButtonElement : IFormElement
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public Dictionary<string,string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public bool IsVisualOnly { get; } = false;
        public bool AutoPostBack { get; set; }
        public Color ElementBackgroundColor { get; set; }
        public Color ElementTextColor { get; set; }
        public View View { get; private set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Value
        {
            get => Key;
            set
            {
                //Do Nothing.
            }
        }
        public bool IsValid
        {
            get => true;
        }

        public event EventHandler<string> PostBack;

        public View Render()
        {

            Button button = new Button()
            {
                Text = Label,
                Margin = new Thickness( 5 )
            };

            View = button;

            if ( ElementBackgroundColor != null )
            {
                View.BackgroundColor = ElementBackgroundColor;
            }

            if ( ElementTextColor != null )
            {
                button.TextColor = ElementTextColor;
            }

            button.Clicked += ( s, e ) => { PostBack?.Invoke( s, Key ); };

            return View;
        }
    }
}
