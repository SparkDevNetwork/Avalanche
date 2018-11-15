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
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    class CustomFontTypeConverter : TypeConverter
    {
        private Dictionary<string, string> fontMapping;
        public CustomFontTypeConverter()
        {
            fontMapping = new Dictionary<string, string>
            {   //ios                  //android
                {"Comic Sans MS", "Comic-Sans-MS.ttf#Comic Sans MS" },
                {"Slabo 27px", "slabo-27px-v4-latin-regular.ttf#Slabo 27px" },
                { "Open Sans","OpenSans-Regular.ttf#Open Sans" }
            };
        }

        public override object ConvertFromInvariantString( string value )
        {
            if ( fontMapping.ContainsKey( value ) )
            {
                return Device.RuntimePlatform == Device.iOS ? value : fontMapping[value];
            }
            return "";
        }

    }
}
