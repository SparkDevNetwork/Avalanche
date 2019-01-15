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
    [MigrationNumber( 1, "1.8.0" )]
    class Init : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Global", "Mobile ListView Components", "List of components for available for Mobile List Views", AvalancheUtilities.MobileListViewComponent );
            RockMigrationHelper.AddDefinedTypeAttribute( AvalancheUtilities.MobileListViewComponent, Rock.SystemGuid.FieldType.TEXT, "ComponentType", "ComponentType", "Components that can be used to display a list of mobile list view items in Avalanche.", 0, "", "ACAE178E-E804-4F32-9BE3-2F020E7314CF" );

            //Thumbnail ListView
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.MobileListViewComponent, "Thumbnail ListView", "Default list view. Supports images, icons, title and description.", "D9EA2C97-68E1-4D94-B881-F3AC4F2883A3" );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "D9EA2C97-68E1-4D94-B881-F3AC4F2883A3", "ACAE178E-E804-4F32-9BE3-2F020E7314CF", "Avalanche.Components.ListView.ThumbnailListView" );

            //Column ListView
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.MobileListViewComponent, "Column ListView", "List view in columns. Supports images, icons, and title.", "1A637B48-35FB-43B2-9822-88AF2FD1D333" );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "1A637B48-35FB-43B2-9822-88AF2FD1D333", "ACAE178E-E804-4F32-9BE3-2F020E7314CF", "Avalanche.Components.ListView.ColumnListView" );

            //Card ListView
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.MobileListViewComponent, "Card ListView", "Card based list view. Supports titles, images, icons, and descriptions in Markdown", "A6EFB571-56C8-44C2-8F87-B7F4DB4E1991" );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "A6EFB571-56C8-44C2-8F87-B7F4DB4E1991", "ACAE178E-E804-4F32-9BE3-2F020E7314CF", "Avalanche.Components.ListView.CardListView" );

            //Horizontal ListView
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.MobileListViewComponent, "Horizontal ListView", "Horizontal layed out list view. Supports titles, images, icons.", "673B7DB5-2200-41D6-8857-9A7663B56C47" );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "673B7DB5-2200-41D6-8857-9A7663B56C47", "ACAE178E-E804-4F32-9BE3-2F020E7314CF", "Avalanche.Components.ListView.HorizontalListView" );

            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.TEXT, "", "", "Avalanche Home Page", "Page which starts the Avalanche App", 0, "", "5FEFE20F-742E-4204-8A1C-7E400F802288" );

        }

        public override void Down()
        {
        }
    }
}
