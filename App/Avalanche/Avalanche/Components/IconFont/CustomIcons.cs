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
                    {"se se-920",  "\ue911"},
                    {"se se-bb",  "\ue912"},
                    {"se se-cw",  "\ue913"},
                    {"se se-et",  "\ue914"},
                    {"se se-hsm",  "\ue915"},
                    {"se se-in",  "\ue916"},
                    {"se se-kids",  "\ue917"},
                    {"se se-la",  "\ue918"},
                    {"se se-lwya",  "\ue919"},
                    {"se se-msm",  "\ue91a"},
                    {"se se-rock",  "\ue91c"},
                    {"se se-se",  "\ue91d"},
                    {"se se-sw",  "\ue91e"},
                    {"se se-piggybank",  "\ue900"},
                    {"se se-shoe",  "\ue901"},
                    {"se se-connect",  "\ue902"},
                    {"se se-contact",  "\ue903"},
                    {"se se-dental",  "\ue904"},
                    {"se se-mind",  "\ue905"},
                    {"se se-give",  "\ue906"},
                    {"se se-missions",  "\ue907"},
                    {"se se-calendar",  "\ue908"},
                    {"se se-debate",  "\ue909"},
                    {"se se-heart-hands",  "\ue90a"},
                    {"se se-money",  "\ue90b"},
                    {"se se-step1",  "\ue90c"},
                    {"se se-step2",  "\ue90d"},
                    {"se se-visit",  "\ue90e"},
                    {"se se-watch",  "\ue90f"},
                    {"se se-barbell",  "\ue910"}
        };
            }
        }
    }
}
