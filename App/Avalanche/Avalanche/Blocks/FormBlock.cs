// <copyright>
// Copyright Southeast Christian Church
// Copyright Mark Lee
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
using System.Text;
using Avalanche.Interfaces;
using Avalanche.Models;
using Avalanche.Utilities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Avalanche.Blocks
{
    public class FormBlock : IRenderable, IHasBlockMessenger
    {
        public Dictionary<string, string> Attributes { get; set; }
        public BlockMessenger MessageHandler { get; set; }

        private List<IFormElement> formElements = new List<IFormElement>();
        private Dictionary<string, string> formValues;
        private Label lbValidationMessage;
        private ActivityIndicator activityIndicator;
        private StackLayout formLayout;


        public View Render()
        {
            MessageHandler.Response += MessageHandler_Response;

            List<FormElementItem> formElementItems = GetFormElementItems();
            StackLayout stackLayout = new StackLayout();

            activityIndicator = new ActivityIndicator();
            stackLayout.Children.Add( activityIndicator );

            lbValidationMessage = new Label()
            {
                BackgroundColor = Color.FromHex( "#f8d7da" ),
                TextColor = Color.FromHex( "#941c24" ),
                IsVisible = false
            };
            stackLayout.Children.Add( lbValidationMessage );

            formLayout = new StackLayout();
            stackLayout.Children.Add( formLayout );

            foreach ( var formElementItem in formElementItems )
            {
                IFormElement formElement = formElementItem.Render();
                formElements.Add( formElement );
                formLayout.Children.Add( formElement.View );
                formElement.PostBack += FormElement_PostBack;
            }

            return stackLayout;
        }

        private void FormElement_PostBack( object sender, string e )
        {
            bool isValid = GetFormValues();
            if ( isValid )
            {
                activityIndicator.IsRunning = true;
                formLayout.IsVisible = false;
                MessageHandler.Post( e, formValues );
            }
        }

        private void MessageHandler_Response( object sender, MobileBlockResponse e )
        {

            FormResponse formResponse = null;

            var response = e.Response;
            try
            {
                formResponse = JsonConvert.DeserializeObject<FormResponse>( response );
            }
            catch
            {
                activityIndicator.IsRunning = false;
                formLayout.IsVisible = true;
                lbValidationMessage.Text = "There was a problem with your request.";
                lbValidationMessage.IsVisible = true;
                return;
            }

            if ( formResponse == null )
            {
                activityIndicator.IsRunning = false;
                formLayout.IsVisible = true;
                lbValidationMessage.Text = "There was a problem with your request.";
                lbValidationMessage.IsVisible = true;
                return;
            }

            if ( !formResponse.Success )
            {
                activityIndicator.IsRunning = false;
                formLayout.IsVisible = true;
                lbValidationMessage.Text = formResponse.ErrorMessage;
                lbValidationMessage.IsVisible = true;
                return;
            }
            AttributeHelper.HandleActionItem( new Dictionary<string, string> {
                { "ActionType", formResponse.ActionType },
                { "Resource", formResponse.Resource },
                { "Parameter", formResponse.Parameter } } );

        }

        private bool GetFormValues()
        {
            formValues = new Dictionary<string, string>();
            var errorMessages = new StringBuilder( "Please correct the following:" );
            bool isValid = true;

            foreach ( var element in formElements )
            {
                if ( element.IsValid )
                {
                    formValues.Add( element.Key, element.Value );
                }
                else
                {
                    isValid = false;
                    errorMessages.Append( string.Format( "\n {0} is required.", element.Label ) );
                }
            }

            if ( isValid )
            {
                lbValidationMessage.IsVisible = false;
            }
            else
            {
                lbValidationMessage.IsVisible = true;
                lbValidationMessage.Text = errorMessages.ToString();
            }

            return isValid;
        }

        private List<FormElementItem> GetFormElementItems()
        {
            if ( Attributes.ContainsKey( "FormElementItems" ) && !string.IsNullOrWhiteSpace( Attributes["FormElementItems"] ) )
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<FormElementItem>>( Attributes["FormElementItems"] );
                }
                catch
                {
                    return new List<FormElementItem>();
                }
            }
            return new List<FormElementItem>();
        }
    }
}
