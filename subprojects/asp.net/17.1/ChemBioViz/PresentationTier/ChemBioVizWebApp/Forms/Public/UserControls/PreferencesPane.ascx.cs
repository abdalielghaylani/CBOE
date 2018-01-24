using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls
{
    public partial class PreferencesPane : System.Web.UI.UserControl, ICOENavigationPanelControl
    {
        #region Properties
        private int DefaultGridPageSize
        {
            get
            {
                return ((ChemBioVizSearch)Page).DefaultGridPageSize;
            }
        }

        private FormGroup.CurrentFormEnum SearchAction
        {
            get
            {
                return ((ChemBioVizSearch) Page).SearchAction;
            }
        }

        private FormGroup.CurrentFormEnum CurrentMode
        {
            get
            {
                return ((ChemBioVizSearch)Page).CurrentMode;
            }
        }

        private int DefaultListViewFormIndex
        {
            get
            {
                return ((ChemBioVizSearch) Page).DefaultListViewFormIndex;
            }
        }

        private int DefaultDetailsViewFormIndex
        {
            get
            {
                return ((ChemBioVizSearch)Page).DefaultDetailsViewFormIndex;
            }
        }

        private bool PreviousFilterChildDataOption
        {
            get
            {
                return ((ChemBioVizSearch)Page).FilterChildData;
            }
        }

        private bool PreviousHighlightSubStructureOption
        {
            get
            {
                return ((ChemBioVizSearch)Page).HighlightSubStructures;
            }
        }

        public int HitsPerPage
        {
            get { return this.HitsPerPageTextBox.Text != string.Empty ? int.Parse(this.HitsPerPageTextBox.Text) : 10; }
        }

        public bool IsListSearchAction
        {
            get { return !this.SkipListCheckBox.Checked; }
        }

        public int ListViewFormIndex
        {
            get
            {
                int selectedValue;

                if (this.ListsFormDropDown.Items.Count > 1 && int.TryParse(this.ListsFormDropDown.SelectedValue, out selectedValue))
                {
                    return selectedValue;
                }
                else
                    return 0;
            }
        }

        public int DetailsViewFormIndex
        {
            get
            {
                int selectedValue;

                if (this.DetailsFormDropDown.Items.Count > 1 && int.TryParse(this.DetailsFormDropDown.SelectedValue, out selectedValue))
                {
                    return selectedValue;
                }
                else
                    return 0;
            }
        }

        public bool FilterChildData
        {
            get
            {
                return this.FilterChildDataCheckBox.Checked;
            }
        }

        public bool HighlightSubStructures
        {
            get
            {
                return this.HighlightStructuresCheckBox.Checked;
            }
        }

        private string[] ShowingControlsID
        {
            get
            {
                return new string[1] { this.DefaultsLinkButton.ClientID };
            }
        }
        #endregion

        #region Lyfe Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetControlsAttributtes();
            if(!Page.IsPostBack)
                this.SetPreviousValues();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string jsInit = @"
        YAHOO.namespace('ConfirmationPanelSearchPrefUC');
        function initConfirmationPanelSearchPref() {
            YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref = new YAHOO.widget.Panel('" + this.ConfirmationYUIPanel.ClientID + @"', { width:'380px', close:true, modal:true, draggable:true, visible:false, constraintoviewport:true } );
            YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref.render(document.body.form);

            YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref.beforeHideEvent.subscribe(ShowChemDraws);
            YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref.beforeShowEvent.subscribe(HideChemDraws);
        }
        YAHOO.util.Event.addListener(window, 'load', initConfirmationPanelSearchPref);

        function save()
        {
            if(ValidateHitsPerPage('" + this.HitsPerPageTextBox.ClientID + @"'))
            { 
                document.getElementById('" + this.ActionField.ClientID + @"').value = 'Save';
                " + (this.CurrentMode != FormGroup.CurrentFormEnum.QueryForm ? @"YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref.show(); 
                return false;" : "return true;") + @" 
            } 
            return false;
        }

        function defaults()
        {
            document.getElementById('" + this.ActionField.ClientID + @"').value = 'RestoreDefaults'; 
            " + (this.CurrentMode != FormGroup.CurrentFormEnum.QueryForm ? @"YAHOO.ConfirmationPanelSearchPrefUC.confirmationPanelSearchPref.show();
            return false;" : "return true;") + @"
        }
        ";

            writer.Write("<script language='javascript'>" + jsInit + "</script>");
        }
        #endregion

        #region Event Handlers
        protected void NoButton_Click(object sender, EventArgs e)
        {
            this.Save(false);
        }

        protected void YesButton_Click(object sender, EventArgs e)
        {
            this.Save(true);
        }
        #endregion

        #region Private Methods
        private void SetControlsAttributtes()
        {
            this.HitsPerPageTD.InnerText = Resources.Resource.HitsPerPage_Label_Text + ":";
            this.SkipListTD.InnerText = Resources.Resource.SkipListView_Label_Text;
            this.HighlightStructuresTD.InnerText = Resources.Resource.HighlightSubstructures_Label_Text;
            this.DetailsTD.InnerText = Resources.Resource.DefaultDetailView_Label_Text;
            this.ListsTD.InnerText = Resources.Resource.DefaultListView_Label_Text;
            this.FilterChildDataTD.InnerText = Resources.Resource.FilterChildData_LabelText;
            this.ConfirmationLabel.Text = Resources.Resource.ApplyChangesToCurrentSearch;
            this.YesButton.Text = Resources.Resource.Yes_Button_Text;
            this.NoButton.Text = Resources.Resource.No_Button_Text;
            this.SaveLinkButton.OnClientClick = "return save();";
            this.DefaultsLinkButton.OnClientClick = "return defaults()";
        }

        public void Save(bool applyChanges)
        {
            if (this.ActionField.Value == "RestoreDefaults")
            {
                this.SetDefaultValues();
            }

            if (CommandRaised != null)
            {
                CommandRaised(this, new COENavigationPanelControlEventArgs(this.ID, "SetPreferences", new string[1] { applyChanges ? "yes" : "no" }));
            }
            
        }

        internal void BindData(FormGroup formGroup)
        {
            this.ListsFormDropDown.Items.Clear();
            this.DetailsFormDropDown.Items.Clear();
            foreach(FormGroup.Display display in formGroup.DetailsForms.Displays)
            {
                this.DetailsFormDropDown.Items.Add(new ListItem(display.Name == null ? "detail_" + display.Id.ToString() : display.Name, display.Id.ToString()));
            }
            foreach(FormGroup.Display display in formGroup.ListForms.Displays)
            {
                this.ListsFormDropDown.Items.Add(new ListItem(display.Name == null ? "list_" + display.Id.ToString() : display.Name, display.Id.ToString()));
            }
            if(this.ListsFormDropDown.Items.Count <= 1)
                this.ListsTD.Visible = this.ListsFormDropDown.Visible = false;
            if(this.DetailsFormDropDown.Items.Count <= 1)
                this.DetailsTD.Visible = this.DetailsFormDropDown.Visible = false;

            if (formGroup.ListForms.Displays.Count == 0)
            {
                this.SkipListCheckBox.Checked = true;
                this.SkipListCheckBox.Enabled = false;
                this.SkipListCheckBox.ToolTip = Resources.Resource.NoListsReason_ToolTip_Text;
                this.SkipListTD.Visible = this.SkipListCheckBox.Visible = false;
            }
            if (formGroup.DetailsForms.Displays.Count == 0)
            {
                this.SkipListCheckBox.Checked = false;
                this.SkipListCheckBox.Enabled = false;
                this.SkipListCheckBox.ToolTip = Resources.Resource.NoDisplaysReason_ToolTip_Text;
                this.SkipListTD.Visible = this.SkipListCheckBox.Visible = false;
            }

            Int32 cartMajor = Convert.ToInt32(CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetCartridgeMajorVersion(CambridgeSoft.COE.Framework.Common.DBMSType.ORACLE));
            if (cartMajor < 13)
            {
                this.HighlightStructuresCheckBox.Checked = false;
                this.HighlightStructuresCheckBox.Enabled = false;
                this.HighlightStructuresCheckBox.ToolTip = Resources.Resource.Not13OrHigherReason_Tooltip_Text;
            }
            
            SetPreviousValues();
        }

        private void SetPreviousValues()
        {
            this.HitsPerPageTextBox.Text = this.DefaultGridPageSize.ToString();
            if(this.SkipListCheckBox.Visible)
                this.SkipListCheckBox.Checked = this.SearchAction == FormGroup.CurrentFormEnum.DetailForm;
            if(this.ListsFormDropDown.Visible)
                this.ListsFormDropDown.SelectedIndex = this.DefaultListViewFormIndex;
            if(this.DetailsFormDropDown.Visible)
                this.DetailsFormDropDown.SelectedIndex = this.DefaultDetailsViewFormIndex;
            this.FilterChildDataCheckBox.Checked = this.PreviousFilterChildDataOption;
            if(this.HighlightStructuresCheckBox.Enabled)
                this.HighlightStructuresCheckBox.Checked = this.PreviousHighlightSubStructureOption;
        }

        private void SetDefaultValues()
        {
            this.HitsPerPageTextBox.Text = "10";
            if (this.SkipListCheckBox.Visible)
                this.SkipListCheckBox.Checked = false;
            if (this.ListsFormDropDown.Visible)
                this.ListsFormDropDown.SelectedIndex = this.DefaultListViewFormIndex;
            if (this.DetailsFormDropDown.Visible)
                this.DetailsFormDropDown.SelectedIndex = this.DefaultDetailsViewFormIndex;
            this.FilterChildDataCheckBox.Checked = false;
            if (this.HighlightStructuresCheckBox.Enabled)
                this.HighlightStructuresCheckBox.Checked = false;
        }
        #endregion

        #region ICOENavigationPanelControl Members
        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;
        #endregion
    }
}
