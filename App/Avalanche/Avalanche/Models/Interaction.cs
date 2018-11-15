// <copyright>

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
using Avalanche.Utilities;

namespace Avalanche.Models
{
    public class Interaction
    {
        public string Operation { get; set; }
        public string InteractionSummary { get; set; }
        public string InteractionData { get; set; }
        public int? PageId { get; set; }
        public string PageTitle { get; set; }

        public void Send()
        {
            ObservableResource<Dictionary<string, string>> observableResource = new ObservableResource<Dictionary<string, string>>();
            var body = new Dictionary<string, string>()
            {
                { "Operation",Operation },
                {"InteractionSummary", InteractionSummary},
                {"InteractionData", InteractionData},
                {"PageTitle", PageTitle}
            };

            if ( PageId != null )
            {
                body["PageId"] = PageId.ToString();
            }

            RockClient.PostResource( observableResource, "/api/avalanche/interaction", body );
        }
    }
}
