﻿// <copyright>
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
    public class EditorElement : IFormElement
    {
        private Editor editor;
        public string Key { get; set; }
        public string Label { get; set; }
        public List<string> Options { get; set; }
        public int HeightRequest { get; set; }
        public string Keyboard { get; set; }
        public bool Required { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public View View { get; private set; }
        public string Value
        {
            get
            {
                if ( editor != null )
                {
                    return editor.Text;
                }
                return "";
            }
            set
            {
                if ( editor != null )
                {
                    editor.Text = value;
                }
            }
        }
        public bool IsValid
        {
            get
            {
                if ( Required )
                {
                    if ( !string.IsNullOrWhiteSpace( editor.Text ) )
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

            Label label = new Label
            {
                Text = this.Label,
                FontAttributes = FontAttributes.Bold
            };
            stackLayout.Children.Add( label );

            editor = new Editor()
            {
                Keyboard = ( Keyboard ) new KeyboardTypeConverter().ConvertFromInvariantString( Keyboard ?? "Default" ),
                HeightRequest = HeightRequest
            };
            stackLayout.Children.Add( editor );

            if ( BackgroundColor != null )
            {
                editor.BackgroundColor = BackgroundColor;
            }

            if ( TextColor != null )
            {
                label.TextColor = TextColor;
                editor.TextColor = TextColor;
            }

            View = stackLayout;
            return View;
        }
    }
}