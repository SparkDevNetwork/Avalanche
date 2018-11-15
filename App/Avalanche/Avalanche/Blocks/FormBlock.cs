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
        public Color ErrorBackgroundColor { get; set; } = Color.FromHex( "#f8d7da" );
        public Color InfoBackgroundColor { get; set; } = Color.FromHex( "#d4edda" );
        public Color ErrorTextColor { get; set; } = Color.FromHex( "#941c24" );
        public Color InfoTextColor { get; set; } = Color.FromHex( "#155743" );

        private List<IFormElement> formElements = new List<IFormElement>();
        private Dictionary<string, string> formValues;
        private StackLayout slValidationMessage;
        private Label lbValidationMessage;
        private ActivityIndicator activityIndicator;
        private StackLayout formLayout;

        public View Render()
        {
            MessageHandler.Response += MessageHandler_Response;

            StackLayout stackLayout = new StackLayout();

            activityIndicator = new ActivityIndicator()
            {
                IsVisible = false
            };
            stackLayout.Children.Add( activityIndicator );

            slValidationMessage = new StackLayout()
            {
                IsVisible = false
            };

            stackLayout.Children.Add( slValidationMessage );

            lbValidationMessage = new Label()
            {
                Margin = new Thickness( 10, 10 )
            };
            slValidationMessage.Children.Add( lbValidationMessage );

            formLayout = new StackLayout();
            stackLayout.Children.Add( formLayout );

            List<FormElementItem> formElementItems = GetFormElementItems();
            RenderForm( formElementItems );

            return stackLayout;
        }

        private void RenderForm( List<FormElementItem> formElementItems )
        {
            foreach ( var formElementItem in formElementItems )
            {
                if ( Attributes.ContainsKey( "ElementBackgroundColor" ) )
                {
                    formElementItem.ElementBackgroundColor = Attributes["ElementBackgroundColor"];
                }

                if ( Attributes.ContainsKey( "ElementTextColor" ) )
                {
                    formElementItem.ElementTextColor = Attributes["ElementTextColor"];
                }

                IFormElement formElement = formElementItem.Render();
                formElements.Add( formElement );
                formLayout.Children.Add( formElement.View );
                formElement.PostBack += FormElement_PostBack;
            }

        }

        private void FormElement_PostBack( object sender, string e )
        {
            bool isValid = GetFormValues();
            if ( isValid )
            {
                activityIndicator.IsRunning = true;
                activityIndicator.IsVisible = true;
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
                activityIndicator.IsVisible = false;
                formLayout.IsVisible = true;
                slValidationMessage.IsVisible = true;
                lbValidationMessage.Text = "There was a problem with your request.";
                lbValidationMessage.TextColor = ErrorTextColor;
                slValidationMessage.BackgroundColor = ErrorBackgroundColor;
                return;
            }
            activityIndicator.IsRunning = false;
            if ( formResponse == null )
            {
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                formLayout.IsVisible = true;
                slValidationMessage.IsVisible = true;
                lbValidationMessage.Text = "There was a problem with your request.";
                lbValidationMessage.TextColor = ErrorTextColor;
                slValidationMessage.BackgroundColor = ErrorBackgroundColor;
                return;
            }

            //Handle a failed form
            if ( !formResponse.Success )
            {
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                formLayout.IsVisible = true;
                slValidationMessage.IsVisible = true;
                lbValidationMessage.Text = formResponse.Message;
                slValidationMessage.BackgroundColor = ErrorBackgroundColor;
                lbValidationMessage.TextColor = ErrorTextColor;
                return;
            }

            //Rebuild form if needed
            if ( formResponse.FormElementItems.Any() )
            {
                formElements.Clear();
                formLayout.Children.Clear();
                formLayout.IsVisible = true;
                RenderForm( formResponse.FormElementItems );
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
            }

            if ( !string.IsNullOrWhiteSpace( formResponse.Message ) )
            {
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                slValidationMessage.IsVisible = true;
                lbValidationMessage.Text = formResponse.Message;
                lbValidationMessage.TextColor = InfoTextColor;
                slValidationMessage.BackgroundColor = InfoBackgroundColor;
            }

            AvalancheNavigation.HandleActionItem( new Dictionary<string, string> {
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
                    if ( !element.IsVisualOnly )
                    {
                        formValues.Add( element.Key, element.Value );
                    }
                }
                else
                {
                    isValid = false;
                    errorMessages.Append( string.Format( "\n {0} is required.", element.Label ) );
                }
            }

            if ( isValid )
            {
                slValidationMessage.IsVisible = false;
            }
            else
            {
                slValidationMessage.IsVisible = true;
                lbValidationMessage.Text = errorMessages.ToString();
                lbValidationMessage.TextColor = ErrorTextColor;
                slValidationMessage.BackgroundColor = ErrorBackgroundColor;
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
