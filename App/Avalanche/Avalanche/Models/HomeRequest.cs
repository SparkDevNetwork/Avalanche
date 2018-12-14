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
using System.Collections.Generic;

namespace Avalanche.Models
{
    public class HomeRequest
    {
        public MobilePage Header { get; set; }
        public MobilePage Footer { get; set; }
        public MobilePage Page { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }
}