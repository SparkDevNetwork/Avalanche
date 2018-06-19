using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool Required { get; set; }
        public bool AutoPostBack { get; set; } = false;
        public Dictionary<string, string> Options { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
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

    public class Keyboard
    {
        public static string Plain = "Plain";
        public static string Chat = "Chat";
        public static string Default = "Default";
        public static string Email = "Email";
        public static string Numeric = "Numeric";
        public static string Telephone = "Telephone";
        public static string Text = "Text";
        public static string Url = "Url";

    }
}
