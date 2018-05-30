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
using System.Text;
using Avalanche.Components.FormElements;
using Avalanche.Interfaces;
using Xamarin.Forms;

namespace Avalanche.Models
{
    public class FormElementItem
    {
        public string Label { get; set; }
        public string Key { get; set; }
        public int HeightRequest { get; set; } = 50;
        public string Type { get; set; }
        public string Keyboard { get; set; }
        public string Value { get; set; }
        public List<string> Options { get; set; }
        public bool Required { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public IFormElement Render()
        {
            IFormElement element;
            switch ( Type )
            {
                case FormElementType.Entry:
                    element = new EntryElement();
                    break;
                case FormElementType.DatePicker:
                    element = new DatePickerElement();
                    break;
                case FormElementType.Picker:
                    element = new PickerElement();
                    break;
                case FormElementType.Editor:
                    element = new EditorElement();
                    break;
                case FormElementType.Checkbox:
                    element = new CheckboxElement();
                    break;
                case FormElementType.Hidden:
                    element = new HiddenElement();
                    break;
                case FormElementType.Button:
                    element = new ButtonElement();
                    break;
                default:
                    element = new EntryElement();
                    break;
            }
            element.Label = this.Label;
            element.Key = Key;
            element.Keyboard = Keyboard;
            element.HeightRequest = HeightRequest;
            element.Options = Options;
            element.Required = Required;

            if ( !string.IsNullOrWhiteSpace( BackgroundColor ) )
            {
                element.BackgroundColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( BackgroundColor );
            }

            if ( !string.IsNullOrWhiteSpace( TextColor ) )
            {
                element.TextColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( TextColor );
            }

            element.Render();
            if ( !string.IsNullOrWhiteSpace( Value ) )
            {
                element.Value = Value;
            }

            return element;
        }
    }
    public class FormElementType
    {
        public const string Entry = "Entry";
        public const string DatePicker = "DatePicker";
        public const string Picker = "Picker";
        public const string Editor = "Editor";
        public const string Hidden = "Hidden";
        public const string Checkbox = "Checkbox";
        public const string Button = "Button";
    }
}