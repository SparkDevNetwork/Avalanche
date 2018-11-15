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
        public bool AutoPostBack { get; set; } = false;
        public Dictionary<string, string> Options { get; set; }
        public bool Required { get; set; }
        public string ElementBackgroundColor { get; set; }
        public string ElementTextColor { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public IFormElement Render()
        {
            IFormElement element;
            switch ( Type )
            {
                case FormElementType.Entry:
                    element = new EntryElement();
                    break;
                case FormElementType.Address:
                    element = new AddressElement();
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
                case FormElementType.Switch:
                    element = new SwitchElement();
                    break;
                case FormElementType.CheckboxList:
                    element = new CheckboxListElement();
                    break;
                case FormElementType.Hidden:
                    element = new HiddenElement();
                    break;
                case FormElementType.Button:
                    element = new ButtonElement();
                    break;
                case FormElementType.Label:
                    element = new LabelElement();
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
            element.Attributes = Attributes;
            element.AutoPostBack = AutoPostBack;

            if ( !string.IsNullOrWhiteSpace( ElementBackgroundColor ) )
            {
                element.ElementBackgroundColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( ElementBackgroundColor );
            }

            if ( !string.IsNullOrWhiteSpace( ElementTextColor ) )
            {
                element.ElementTextColor = ( Color ) new ColorTypeConverter().ConvertFromInvariantString( ElementTextColor );
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
        public const string Address = "Address";
        public const string DatePicker = "DatePicker";
        public const string Picker = "Picker";
        public const string Editor = "Editor";
        public const string Hidden = "Hidden";
        public const string Switch = "Switch";
        public const string CheckboxList = "CheckboxList";
        public const string Button = "Button";
        public const string Label = "Label";
    }
}