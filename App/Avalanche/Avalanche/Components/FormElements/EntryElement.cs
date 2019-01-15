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
using Avalanche.Utilities;
using Xamarin.Forms;

namespace Avalanche.Components.FormElements
{
    public class EntryElement : IFormElement
    {
        Entry entry;
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
                if ( entry != null )
                {
                    return entry.Text;
                }
                return "";
            }
            set
            {
                if ( entry != null )
                {
                    entry.Text = value;
                }
            }
        }
        public bool IsValid
        {
            get
            {
                if ( Required )
                {
                    if ( !string.IsNullOrWhiteSpace( entry.Text ) )
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

            entry = new Entry()
            {
                Text = Value,
                Keyboard = ( Keyboard ) new KeyboardTypeConverter().ConvertFromInvariantString( Keyboard ?? "Default" )
            };
            if ( Required )
            {
                entry.Placeholder = "(Required)";
            }

            stackLayout.Children.Add( entry );

            if ( ElementBackgroundColor != null )
            {
                entry.BackgroundColor = ElementBackgroundColor;
            }

            if ( ElementTextColor != null )
            {
                entry.TextColor = ElementTextColor;
            }

            entry.TextChanged += ( s, e ) =>
            {
                if ( AutoPostBack )
                {
                    PostBack?.Invoke( s, Key );
                }
            };

            AttributeHelper.ApplyTranslation( entry, Attributes );

            return View;
        }
    }
}
