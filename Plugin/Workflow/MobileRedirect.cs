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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Web;
using Avalanche.Attribute;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;

namespace Avalanche.Workflow
{
    /// <summary>
    /// Redirects the user to a different page.
    /// </summary>
    [ActionCategory( "Avalanche" )]
    [Description( "Redirects the user to the appropriate page or website." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Mobile Redirect" )]

    [ActionItemField( "Action Type", "The action to take to redirect the user.", false )]
    [CustomDropdownListField( "Processing Options", "How should workflow continue processing?", "0^Always continue,1^Only continue on redirect,2^Never continue", true, "0", "", 1 )]
    public class Redirect : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            var actionItemValue = GetAttributeValue( action, "ActionType" );

            var actionDictionary = new Dictionary<string, string>();
            AvalancheUtilities.SetActionItems( actionItemValue, actionDictionary, action.Activity?.AssignedPersonAlias?.Person, GetMergeFields( action ) );


            var processOpt = GetAttributeValue( action, "ProcessingOptions" );
            bool canSendRedirect = actionDictionary.ContainsKey( "ActionType" ) && actionDictionary["ActionType"] != "0" && HttpContext.Current != null;

            if ( canSendRedirect )
            {
                HttpContext.Current.Response.AddHeader( "ActionType", actionDictionary["ActionType"] );

                if ( actionDictionary.ContainsKey( "Resource" ) )
                {
                    HttpContext.Current.Response.AddHeader( "Resource", actionDictionary["Resource"] );
                }

                if ( actionDictionary.ContainsKey( "Parameter" ) )
                {
                    HttpContext.Current.Response.AddHeader( "Parameter", actionDictionary["Parameter"] );
                }

            }

            if ( processOpt == "1" )
            {
                // 1) if HttpContext.Current is null, this workflow action might be running as a job (there is no browser session associated), so Redirect couldn't have been sent to a browser
                // 2) if there was no url specified, the redirect wouldn't have executed either
                return canSendRedirect;
            }
            else
            {
                return processOpt != "2";
            }
        }
    }
}
