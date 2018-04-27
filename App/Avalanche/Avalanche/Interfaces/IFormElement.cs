using System;
using System.Collections.Generic;
using System.Text;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Interfaces
{
    public interface IFormElement
    {
        string Key { get; set; }
        string Label { get; set; }
        string Value { get; set; }
        List<string> Options { get; set; }
        int HeightRequest { get; set; }
        string Keyboard { get; set; }
        bool IsValid { get; }
        bool Required { get; set; }
        Color BackgroundColor { get; set; }
        Color TextColor { get; set; }
        event EventHandler<string> PostBack;
        View View { get; }
        View Render();
    }
}
