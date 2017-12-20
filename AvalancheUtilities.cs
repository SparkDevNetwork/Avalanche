using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;

namespace Avalanche
{
    public static class AvalancheUtilities
    {
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
                        CustomAttributes.Add( "Argument", actionItems[2] );
                    }
                    if ( actionType == "4" && actionItems.Length > 3 ) //Webpage
                    {
                        if ( actionItems[3] == "1" && CurrentPerson != null )
                        {
                            CustomAttributes["Argument"] = CurrentPerson.UrlEncodedKey;
                        }
                        else
                        {
                            CustomAttributes["Argument"] = "";
                        }
                    }
                }
            }
            else
            {
                CustomAttributes.Add( "ActionType", "0" );
            }
        }

    }
}
