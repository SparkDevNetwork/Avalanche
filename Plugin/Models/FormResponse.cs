using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalanche.Models
{
    public class FormResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string Resource { get; set; }
        public string Parameter { get; set; }
        public List<FormElementItem> FormElementItems { get; set; } = new List<FormElementItem>();

        public void SetResponse( string attributeValue )
        {
            char[] splitchar = { };
            if ( attributeValue.Contains( "^" ) )
            {
                splitchar = new char[] { '^' };
            }
            else
            {
                splitchar = new char[] { '|' };
            }
            var actionItems = ( attributeValue ?? "" ).Split( splitchar );

            if ( actionItems.Length > 0 && !string.IsNullOrWhiteSpace( actionItems[0] ) )
            {
                var actionType = actionItems[0];
                ActionType = actionType;
                if ( actionType != "0" && actionType != "3" ) //Not Do Nothing or Pop Current Page
                {
                    if ( actionItems.Length > 1 )
                    {
                        Resource = actionItems[1];
                    }
                    if ( actionItems.Length > 2 )
                    {
                        Parameter = actionItems[2];
                    }
                }
            }
            else
            {
                ActionType = "3"; //If no action specified pop page.
            }
        }
    }
}
