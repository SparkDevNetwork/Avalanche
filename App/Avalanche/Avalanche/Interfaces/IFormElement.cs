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
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Interfaces
{
    public interface IFormElement
    {
        string Key { get; set; }
        string Label { get; set; }
        string Value { get; set; }
        Dictionary<string, string> Options { get; set; }
        int HeightRequest { get; set; }
        string Keyboard { get; set; }
        bool IsValid { get; }
        bool Required { get; set; }
        bool IsVisualOnly { get; }
        bool AutoPostBack { get; set; }
        Color ElementBackgroundColor { get; set; }
        Color ElementTextColor { get; set; }
        Dictionary<string, string> Attributes { get; set; }
        event EventHandler<string> PostBack;
        View View { get; }
        View Render();
    }
}
