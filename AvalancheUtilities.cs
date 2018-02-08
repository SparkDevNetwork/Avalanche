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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock;
using Rock.Model;

namespace Avalanche
{
    public static class AvalancheUtilities
    {
        //Constansts
        public const string MobileListViewComponent = "657FDF2F-FB7B-44C4-BAB0-A370893FDFB8";


        //Methods
        public static string GetShortAssembly( Type type )
        {
            return string.Format(
                "{0}, {1}",
                type.FullName,
                type.Assembly.GetName().Name
                );
        }

        public static void SetActionItems( string ActionItemValue, Dictionary<string, string> CustomAttributes, Person CurrentPerson )
        {
            var actionItems = ( ActionItemValue ?? "" ).Split( new char[] { '|' } );

            if ( actionItems.Length > 0 && !string.IsNullOrWhiteSpace( actionItems[0] ) )
            {
                var actionType = actionItems[0];
                CustomAttributes.Add( "ActionType", actionType );
                if ( actionType != "0" && actionType != "3" ) //Not Do Nothing or Pop Current Page
                {
                    if ( actionItems.Length > 1 )
                    {
                        CustomAttributes.Add( "Resource", actionItems[1] );
                    }
                    if ( actionItems.Length > 2 )
                    {
                        CustomAttributes.Add( "Parameter", actionItems[2] );
                    }
                }
            }
            else
            {
                CustomAttributes.Add( "ActionType", "0" );
            }
        }

        public static string ProcessLava( string lava, Person currentPerson, string parameter = "", string enabledLavaCommands = "" )
        {
            var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, currentPerson );
            mergeObjects["parameter"] = parameter;
            return lava.ResolveMergeFields( mergeObjects, null, enabledLavaCommands );
        }
    }
}
