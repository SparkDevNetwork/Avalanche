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
using Rock.Plugin;

namespace Avalanche.Migrations
{
    [MigrationNumber( 4, "1.7.0" )]
    class CarouselListView : Migration
    {
        public override void Up()
        {
            //CarouselListView ListView
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.MobileListViewComponent, "Carousel ListView", "A one at a time slide show. Set the Custom Attribute 'ScrollInterval' with a number in seconds to have it auto scroll.", "77B69D91-5FF2-405E-96DB-ED24BF797529" );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "77B69D91-5FF2-405E-96DB-ED24BF797529", "9107CE0D-3EC8-4C4F-8BB1-A45EFD4DB2E9", "Avalanche.Components.ListView.CarouselListView" );
        }

        public override void Down()
        {
        }
    }
}
