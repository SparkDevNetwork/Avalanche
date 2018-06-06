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
using Avalanche.Interfaces;
using Avalanche.Models;
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class EntryElement : IFormElement
    {
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
                    return ( ( Entry ) View ).Text;
                }
                return "";
            }
            set
            {
                if ( View != null )
                {
                    ( ( Entry ) View ).Text = value;
                }
            }
        }
        public bool IsValid
        {
            get
            {
                if ( Required )
                {
                    if ( !string.IsNullOrWhiteSpace( ( ( Entry ) View ).Text ) )
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
            View = new Entry()
            {
                Text = Value,
                Keyboard = ( Keyboard ) new KeyboardTypeConverter().ConvertFromInvariantString( Keyboard ?? "Default" ),
                Placeholder = Label,
                Margin = new Thickness( 5 )
            };

            if ( BackgroundColor != null )
            {
                View.BackgroundColor = BackgroundColor;
            }

            if ( TextColor != null )
            {
                ( ( Entry ) View ).TextColor = TextColor;
            }

            AttributeHelper.ApplyTranslation( ( Entry ) View, Attributes );

            return View;
        }
    }
}
