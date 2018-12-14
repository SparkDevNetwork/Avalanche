// <copyright>
// Southeast Christian Church
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
    [MigrationNumber( 3, "1.8.0" )]
    class Layouts : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Tools", "Avalanche Layouts", "Layouts for mobile Avalanche app.", AvalancheUtilities.LayoutsDefinedType );
            RockMigrationHelper.AddDefinedTypeAttribute( AvalancheUtilities.LayoutsDefinedType, Rock.SystemGuid.FieldType.MEMO, "Content", "Content", "Content to be loaded a as layout.", 0, "", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91" );

            var simple = @"[
  {
    ""Name"": ""Featured"",
    ""Type"": ""StackLayout"",
    ""Orientation"": ""Vertical"",
    ""Attributes"": {
                ""VerticalOptions"": ""Start""
    }
        },
  {
    ""Name"": ""Container"",
    ""Type"": ""StackLayout"",
    ""Orientation"": ""Vertical"",
    ""ScrollY"": true,
    ""Attributes"": {
      ""VerticalOptions"": ""FillAndExpand""
    },
    ""Children"": [
      {
        ""Name"": ""Main"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical""
      },
      {
        ""Name"": ""Horizontal"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Horizontal"",
        ""Children"": [
          {
            ""Name"": ""Left"",
            ""Type"": ""StackLayout"",
            ""Orientation"": ""Vertical""
          },
          {
            ""Name"": ""Center"",
            ""Type"": ""StackLayout"",
            ""Orientation"": ""Vertical""
          },
          {
            ""Name"": ""Right"",
            ""Type"": ""StackLayout"",
            ""Orientation"": ""Vertical""
          }
        ]
      }
    ]
  },
  {
    ""Name"": ""Footer"",
    ""Type"": ""StackLayout"",
    ""Orientation"": ""Vertical""
  }
]";

            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.LayoutsDefinedType, "Simple", "Simple layout with a featured, main, three sub sections, and a footer.", "BB5006D8-7F51-43D5-B977-5E07F5ACA8C2", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "BB5006D8-7F51-43D5-B977-5E07F5ACA8C2", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", simple );

            var noscroll = @"[
{
  ""Name"": ""Featured"",
  ""Type"": ""StackLayout"",
  ""Orientation"": ""Vertical"",
  ""Attributes"": {
                ""VerticalOptions"": ""Start""
  }
        },
{
  ""Name"": ""Main"",
  ""Type"": ""StackLayout"",
  ""Orientation"": ""Vertical"",
  ""Attributes"": {
    ""VerticalOptions"": ""FillAndExpand""
  }
},
{
  ""Name"": ""Footer"",
  ""Type"": ""StackLayout"",
  ""Orientation"": ""Vertical"",
}
]";
            RockMigrationHelper.AddDefinedValue( AvalancheUtilities.LayoutsDefinedType, "No Scroll", "Layout with no scrolling elements. Good for list views.", "3812C543-8B80-4A7C-BBD0-4DEFEABBA7DC", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3812C543-8B80-4A7C-BBD0-4DEFEABBA7DC", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", noscroll );
        }
        public override void Down()
        {
        }
    }
}
