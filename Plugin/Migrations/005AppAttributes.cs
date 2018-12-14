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
    [MigrationNumber( 5, "1.8.0" )]
    class AppAttributes : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.KEY_VALUE_LIST, "", "", "Avalanche Attributes", "Attributes to send to Avalanche to change it's behaviour.", 0, "", "C4FE37BA-1504-4D45-974A-06E472CE2780" );
        }

        public override void Down()
        {
        }
    }
}
