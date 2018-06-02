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
    public class SoutheastIcons : IIconFont
    {
        public string iOSFont { get { return "CustomIcons"; } }
        public string AndroidFont { get { return "CustomIcons.ttf#CustomIcons"; } }
        public Dictionary<string, string> LookupTable
        {
            get
            {
                return new Dictionary<string, string> {
            {"fa fa-500px", "\uf26e"},
            {"se se-rock", "\uf90c"},
            {    "se se-kids",  "\uf90b"},
            {   "se se-c920",  "\uf900"},
            {  "se se-bb", "\uf901"},
            {  "se se-cw",  "\uf902"},
            {  "se se-et",  "\uf903"},
            {  "se se-hsm",  "\uf904"},
            {  "se se -in", "\uf905"},
            {  "se se-la",  "\uf906"},
            {  "se se-lwya", "\uf907"},
            {  "se se se-msm",  "\uf908"},
            {  "se-se", " \uf909"},
            {  "se se-sw",  "\uf90a"}
            };
            }
        }
    }
}
