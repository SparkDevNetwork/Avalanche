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

using Xamarin.Forms;

namespace Avalanche.Models
{
    public class ListElement
    {
        public string Id { get; set; }
        public int Order { get; set; }

        public string Title { get; set; }
        public double FontSize { get; set; } = 20;
        public Color TextColor { get; set; }

        public string Description { get; set; }
        public double DescriptionFontSize { get; set; } = 12;
        public Color DescriptionTextColor { get; set; }


        public string Icon { get; set; }
        public double IconFontSize { get; set; } = 40;
        public Color IconTextColor { get; set; }

        public string Image { get; set; }
        public string ActionType { get; set; }
        public string Resource { get; set; }
        public string Parameter { get; set; }
    }
}
