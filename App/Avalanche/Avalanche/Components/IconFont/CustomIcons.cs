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

namespace Avalanche.Components.IconFont
{
    public class CustomIcons : IIconFont
    {
        public string iOSFont { get { return "CustomIcons"; } }
        public string AndroidFont { get { return "CustomIcons.ttf#CustomIcons"; } }
        public Dictionary<string, string> LookupTable
        {
            get
            {
                return new Dictionary<string, string> {
                     { "se se-rock","\ue90c"} ,
                     { "se se-kids", "\ue90b" },
                     { "se se-c920", "\ue900" },
                     { "se se-bb", "\ue901"} ,
                     { "se se-cw", "\ue902"} ,
                     { "se se-et", "\ue903"} ,
                     { "se se-hsm", "\ue904"},
                     { "se se-in", "\ue905"} ,
                     { "se se-la", "\ue906"} ,
                     { "se se-lwya", "\ue907" },
                     { "se se-msm", "\ue908"},
                     { "se se-se", "\ue909"} ,
                     { "se se-sw" , "\ue90a"},
        };
            }
        }
    }
}
