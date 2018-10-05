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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;

namespace Avalanche.Controls

{
    /// <summary>
    /// 
    /// </summary>
    public class ActionItem : CompositeControl, IRockControl
    {

        #region IRockControl implementation

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The text for the label." )
        ]
        public string Label
        {
            get { return ViewState["Label"] as string ?? string.Empty; }
            set { ViewState["Label"] = value; }
        }

        /// <summary>
        /// Gets or sets the form group class.
        /// </summary>
        /// <value>
        /// The form group class.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        Description( "The CSS class to add to the form-group div." )
        ]
        public string FormGroupCssClass
        {
            get { return ViewState["FormGroupCssClass"] as string ?? string.Empty; }
            set { ViewState["FormGroupCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>
        /// The help text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The help block." )
        ]
        public string Help
        {
            get
            {
                return HelpBlock != null ? HelpBlock.Text : string.Empty;
            }
            set
            {
                if ( HelpBlock != null )
                {
                    HelpBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the warning text.
        /// </summary>
        /// <value>
        /// The warning text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The warning block." )
        ]
        public string Warning
        {
            get
            {
                return WarningBlock != null ? WarningBlock.Text : string.Empty;
            }
            set
            {
                if ( WarningBlock != null )
                {
                    WarningBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Is the value required?" )
        ]
        public bool Required
        {
            get { return ViewState["Required"] as bool? ?? false; }
            set { ViewState["Required"] = value; }
        }

        /// <summary>
        /// Gets or sets the required error message.  If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The required error message.
        /// </value>
        public string RequiredErrorMessage
        {
            get
            {
                return RequiredFieldValidator != null ? RequiredFieldValidator.ErrorMessage : string.Empty;
            }
            set
            {
                if ( RequiredFieldValidator != null )
                {
                    RequiredFieldValidator.ErrorMessage = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid
        {
            get
            {
                return !Required || RequiredFieldValidator == null || RequiredFieldValidator.IsValid;
            }
        }

        /// <summary>
        /// Gets or sets the help block.
        /// </summary>
        /// <value>
        /// The help block.
        /// </value>
        public HelpBlock HelpBlock { get; set; }

        /// <summary>
        /// Gets or sets the warning block.
        /// </summary>
        /// <value>
        /// The warning block.
        /// </value>
        public WarningBlock WarningBlock { get; set; }

        /// <summary>
        /// Gets or sets the required field validator.
        /// </summary>
        /// <value>
        /// The required field validator.
        /// </value>
        public RequiredFieldValidator RequiredFieldValidator { get; set; }

        /// <summary>
        /// Gets or sets the group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server.
        /// </summary>
        /// <returns>The group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server. The default value is an empty string ("").</returns>
        public string ValidationGroup { get; set; }

        #endregion

        #region Controls

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the defined type id.  If a defined type id is used, the value control
        /// will render as a DropDownList of values from that defined type.  If a DefinedTypeId is not specified
        /// the values will be rendered as free-form text fields.
        /// </summary>
        /// <value>
        /// The defined type id.
        /// </value>
        public int? DefinedTypeId
        {
            get { return ViewState["DefinedTypeId"] as int?; }
            set { ViewState["DefinedTypeId"] = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get
            {
                if ( ddlActionList.SelectedValue == "1" || ddlActionList.SelectedValue == "2" )
                {
                    return string.Format( "{0}^{1}^{2}", ddlActionList.SelectedValue, ppPage.SelectedValue, tbParameter.Text );
                }

                if ( ddlActionList.SelectedValue == "4" )
                {
                    return string.Format( "{0}^{1}^{2}", ddlActionList.SelectedValue, tbTarget.Text, ddlRckipid.SelectedValue );
                }

                return ddlActionList.SelectedValue;
            }
            set
            {
                EnsureChildControls();
                string[] values = null;
                if ( value.Contains( '^' ) )
                {
                    values = value.Split( '^' );
                }
                else
                {
                    values = value.Split( '|' );
                }
                ddlActionList.SelectedValue = values[0];

                if ( ddlActionList.SelectedValue == "1" || ddlActionList.SelectedValue == "2" )
                {
                    if ( values.Length > 1 )
                    {
                        ppPage.SetValue( values[1].AsInteger() );
                    }
                    if ( values.Length > 2 )
                    {
                        tbParameter.Text = values[2];
                    }
                }

                if ( ddlActionList.SelectedValue == "4" )
                {
                    if ( values.Length > 1 )
                    {
                        tbTarget.Text = values[1];
                    }
                    if ( values.Length > 2 )
                    {
                        ddlRckipid.SelectedValue = values[2];
                    }
                }
            }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="ValueList"/> class.
        /// </summary>
        public ActionItem() : base()
        {
            this.HelpBlock = new HelpBlock();
            this.WarningBlock = new WarningBlock();
        }

        private RockDropDownList ddlActionList;
        private PagePicker ppPage;
        private RockTextBox tbTarget;
        private RockTextBox tbParameter;
        private RockDropDownList ddlRckipid;

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Clear();
            RockControlHelper.CreateChildControls( this, Controls );
            EnsureChildControls();

            ddlActionList = new RockDropDownList();
            ddlActionList.Label = "Action";
            ddlActionList.AutoPostBack = true;
            ddlActionList.Items.Add( new ListItem( "Do Nothing", "0" ) );
            ddlActionList.Items.Add( new ListItem( "Push New Page", "1" ) );
            ddlActionList.Items.Add( new ListItem( "Replace Current Page", "2" ) );
            ddlActionList.Items.Add( new ListItem( "Pop Curent Page", "3" ) );
            ddlActionList.Items.Add( new ListItem( "Open Browser", "4" ) );
            ddlActionList.SelectedIndexChanged += DdlActionList_SelectedIndexChanged;
            ddlActionList.CssClass = "col-md-12";
            ddlActionList.ID = "ddlActionList_" + this.ID;

            ppPage = new PagePicker()
            {
                Label = "Page",
                ID = "ppPage_" + this.ID
            };

            tbTarget = new RockTextBox()
            {
                Label = "Target",
                ID = "tbTarget_" + this.ID
            };

            tbParameter = new RockTextBox()
            {
                Label = "Parameter",
                ID = "tbParameter_" + this.ID
            };

            ddlRckipid = new RockDropDownList();
            ddlRckipid.ID = "ddlRckipid_" + this.ID;
            ddlRckipid.Label = "Send Impersonation Id";
            ddlRckipid.Items.Add( new ListItem( "No", "0" ) );
            ddlRckipid.Items.Add( new ListItem( "Yes", "1" ) );

            Controls.Add( ddlActionList );
            Controls.Add( ppPage );
            Controls.Add( tbTarget );
            Controls.Add( tbParameter );
            Controls.Add( ddlRckipid );
            EnsureChildControls();
        }

        private void DdlActionList_SelectedIndexChanged( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                RockControlHelper.RenderControl( this, writer );
            }
        }

        /// <summary>
        /// Renders the base control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void RenderBaseControl( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "well" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            ddlActionList.RenderControl( writer );

            if ( ddlActionList.SelectedValue == "1" || ddlActionList.SelectedValue == "2" )
            {
                ppPage.RenderControl( writer );
                tbParameter.RenderControl( writer );
            }
            else if ( ddlActionList.SelectedValue == "4" )
            {
                tbTarget.RenderControl( writer );
                ddlRckipid.RenderControl( writer );
            }

            writer.RenderEndTag();
        }
    }
}