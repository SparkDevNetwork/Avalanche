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
using Rock.Plugin;

namespace Avalanche.Migrations
{
    [MigrationNumber( 2, "1.8.0" )]
    class Interactions : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateDefinedValue( Rock.SystemGuid.DefinedType.INTERACTION_CHANNEL_MEDIUM, "Avalanche App", "Used for tracking requests from an Avalanche app.", AvalancheUtilities.AppMediumValue );
        }

        public override void Down()
        {
        }
    }
}
