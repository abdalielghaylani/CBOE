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
using CambridgeSoft.COE.RegistrationAdmin.Services;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea
{
    public partial class EditFormsXml : GUIShellPage
    {
        #region Variables
        RegistrationMaster _masterPage = null;
        private const string LITABSHEADERCLASSNAME = "LiTabsHeader";
        private const string SELECTEDTABSHEADERCLASSNAME = "SelectedTabsHeader";
        private const string CONFIGURATIONBOVARIABLENAME = "ConfigurationRegistryRecord";
        #endregion

        #region Properties
        private ConfigurationRegistryRecord ConfigurationRegistryRecord
        {
            get
            {
                if (Session[CONFIGURATIONBOVARIABLENAME] == null)
                {
                    Session[CONFIGURATIONBOVARIABLENAME] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                    return (ConfigurationRegistryRecord)Session[CONFIGURATIONBOVARIABLENAME];
                }
                else
                    return (ConfigurationRegistryRecord)Session[CONFIGURATIONBOVARIABLENAME];
            }
            set
            {
                Session[CONFIGURATIONBOVARIABLENAME] = value;
            }
        }
        #endregion

        #region LyfeCycle events
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.ClientScript.RegisterStartupScript(typeof(EditFormsXml), "copytoclipboard", @"
            function copyToClipboard() {
                if(!window.clipboardData.setData('Text', document.getElementById('" + this.XmlTextBox.ClientID + @"').value)) {
                     alert('" + Resource.ErrorTextNotCoppiedToClipboard_Alert_Text + @"');
                    return false;
                }
                return true;
            }

            function pasteFromClipboard() {
                var clipBoardData = window.clipboardData.getData('Text');
                if(!clipBoardData) {
                    alert('" + Resource.ErrorTextNotCoppiedFromClipboard_Alert_Text + @"');
                    return false;
                }
                
                document.getElementById('" + this.XmlTextBox.ClientID + @"').value = clipBoardData;
                
                return true;
            }", true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.SaveButton.Click += new EventHandler(SaveViewButton_Click);
                this.LinkButtonSubmitMixture.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonReviewRegister.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonViewMixture.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonSearchTemporary.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonSearchPermanent.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonELNSearchPerm.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonELNSearchTemp.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonDataLoader.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonRegistryDuplicates.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonComponentDuplicates.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonDeleteLog.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonSearchComponentsToAdd.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonSearchComponentsToAddRR.Click += new EventHandler(LinkButton_Click);
                this.LinkButtonSendToRegitration.Click += new EventHandler(LinkButton_Click);
                this._masterPage.SetDefaultAction(this.LinkButtonGoToMain.UniqueID);

                this.XmlControl.TransformSource = Resource.XmlToHtml_FilePath;

                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Event Handlers
        void LinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveButton.Text == Resource.SaveXml_Button_Text)
                    CancelEditMode();
                SelectFormTab(((LinkButton)sender).ID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        private void SaveViewButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetControlsVisibility();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        public void ButtonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                CancelEditMode();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        protected void LinkButtonGoToMain_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer(Resource.RegAdmin_URL);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }
        #endregion

        #region Private Methods
        private void CancelEditMode()
        {
            this.SaveButton.Text = Resource.Cancel_Button_Text;
            SetControlsVisibility();
        }
        protected override void SetControlsAttributtes()
        {
            this.Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.AdvancedCustomizeForms_Page_Title;
            this.LinkButtonSubmitMixture.Text = Resource.SubmitNewRecord_LinkButton_Text;
            this.LinkButtonReviewRegister.Text = Resource.ReviewRegister_LinkButton_Text;
            this.LinkButtonViewMixture.Text = Resource.ViewRegistered_LinkButton_Text;
            this.LinkButtonSearchTemporary.Text = Resource.SearchTemporary_LinkButton_Text;
            this.LinkButtonSearchPermanent.Text = Resource.SearchRegistry_LinkButton_Text;
            this.LinkButtonELNSearchTemp.Text = Resource.ELNSearchTempForm_LinkButton_Text;
            this.LinkButtonELNSearchPerm.Text = Resource.ELNSearchPermForm_LinkButton_Text;
            this.LinkButtonDataLoader.Text = Resource.DataLoader_LinkButton_Text;
            this.LinkButtonRegistryDuplicates.Text = Resource.RegistryDuplicatesForm_LinkButton_Text;
            this.LinkButtonComponentDuplicates.Text = Resource.ComponentDuplicatesForm_LinkButton_Text;
            this.LinkButtonSendToRegitration.Text = Resource.SendToRegitration_LinkButton_Text;
            this.LinkButtonDeleteLog.Text = Resource.DeleteLog_LinkButton_Text;
            this.LinkButtonSearchComponentsToAdd.Text = Resource.SearchComponentsToAdd_LinkButton_Text;
            this.LinkButtonSearchComponentsToAddRR.Text = Resource.SearchComponentsToAddRR_LinkButton_Text;
            this.SelectFormTab(this.LinkButtonSearchTemporary.ID);
        }
        private void SetControlsVisibility()
        {
            if (this.SaveButton.Text == Resource.EditXml_Button_Text)
            {
                this.MessagesAreaUserControl.Visible = false;
                this.LinkButtonCancelDiv.Visible = true;
                this.XmlControlDiv.Visible = false;
                this.XmlControl.Visible = false;
                this.XmlTextBox.Visible = this.PasteFromClipBoardButton.Visible = this.CopyToClipBoardButton.Visible = true;
                this.SaveButton.Text = Resource.SaveXml_Button_Text;
                this.XmlTextBox.InnerHtml = this.Server.HtmlEncode(this.ConfigurationRegistryRecord.FormGroup.ToString());
            }
            else if (this.SaveButton.Text == Resource.SaveXml_Button_Text)
            {
                this.LinkButtonCancelDiv.Visible = false;
                this.XmlControlDiv.Visible = true;
                this.XmlControl.Visible = true;
                this.XmlTextBox.Visible = this.PasteFromClipBoardButton.Visible = this.CopyToClipBoardButton.Visible = false;
                this.SaveButton.Text = Resource.EditXml_Button_Text;
                try
                {
                    this.ConfigurationRegistryRecord.COEFormHelper.SaveFormGroup(this.Server.HtmlDecode(this.XmlTextBox.InnerHtml));
                    this.ShowConfirmationMessage(Resource.FormGroupSavedSuccessfuly_Message_Text);
                }
                catch (Exception)
                {
                    this._masterPage.DisplayErrorMessage(Resource.Entered_XML_ErroMessage, false);
                }

                this.XmlControl.DocumentContent = this.ConfigurationRegistryRecord.FormGroup.ToString();
            }
            else
            {
                this.LinkButtonCancelDiv.Visible = false;
                this.XmlControlDiv.Visible = true;
                this.XmlControl.Visible = true;
                this.XmlTextBox.Visible = this.PasteFromClipBoardButton.Visible = this.CopyToClipBoardButton.Visible = false;
                this.SaveButton.Text = Resource.EditXml_Button_Text;
                this.XmlControl.DocumentContent = this.ConfigurationRegistryRecord.FormGroup.ToString();
            }
        }
        private void SelectFormTab(string linkButtonId)
        {
            this.LiSubmitMixture.Attributes.Remove("class");
            this.LiSubmitMixture.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiSearchTemporary.Attributes.Remove("class");
            this.LiSearchTemporary.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiSearchPermanent.Attributes.Remove("class");
            this.LiSearchPermanent.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiRegisterMixture.Attributes.Remove("class");
            this.LiRegisterMixture.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiViewMixture.Attributes.Remove("class");
            this.LiViewMixture.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiELNSearchPermForm.Attributes.Remove("class");
            this.LiELNSearchPermForm.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiELNSearchTempForm.Attributes.Remove("class");
            this.LiELNSearchTempForm.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiDataLoader.Attributes.Remove("class");
            this.LiDataLoader.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiRegistryDuplicatesForm.Attributes.Remove("class");
            this.LiRegistryDuplicatesForm.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiComponentDuplicates.Attributes.Remove("class");
            this.LiComponentDuplicates.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiSendToRegitration.Attributes.Remove("class");
            this.LiSendToRegitration.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiDeleteLog.Attributes.Remove("class");
            this.LiDeleteLog.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiSearchComponentsToAdd.Attributes.Remove("class");
            this.LiSearchComponentsToAdd.Attributes.Add("class", LITABSHEADERCLASSNAME);
            this.LiSearchComponentsToAddRR.Attributes.Remove("class");
            this.LiSearchComponentsToAddRR.Attributes.Add("class", LITABSHEADERCLASSNAME);

            if (linkButtonId == this.LinkButtonSubmitMixture.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SubmitMixture);
                this.LiSubmitMixture.Attributes.Remove("class");
                this.LiSubmitMixture.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonReviewRegister.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ReviewRegisterMixture);
                this.LiRegisterMixture.Attributes.Remove("class");
                this.LiRegisterMixture.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonViewMixture.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ViewMixture);
                this.LiViewMixture.Attributes.Remove("class");
                this.LiViewMixture.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonSearchTemporary.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchTemporary);
                this.LiSearchTemporary.Attributes.Remove("class");
                this.LiSearchTemporary.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonSearchPermanent.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchPermanent);
                this.LiSearchPermanent.Attributes.Remove("class");
                this.LiSearchPermanent.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonELNSearchTemp.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ELNSearchTempForm);
                this.LiELNSearchTempForm.Attributes.Remove("class");
                this.LiELNSearchTempForm.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonELNSearchPerm.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ELNSearchPermForm);
                this.LiELNSearchPermForm.Attributes.Remove("class");
                this.LiELNSearchPermForm.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonDataLoader.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.DataLoaderForm);
                this.LiDataLoader.Attributes.Remove("class");
                this.LiDataLoader.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonRegistryDuplicates.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.RegistryDuplicatesForm);
                this.LiRegistryDuplicatesForm.Attributes.Remove("class");
                this.LiRegistryDuplicatesForm.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonComponentDuplicates.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.ComponentDuplicatesForm);
                this.LiComponentDuplicates.Attributes.Remove("class");
                this.LiComponentDuplicates.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonSendToRegitration.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SendToRegistrationForm);
                this.LiSendToRegitration.Attributes.Remove("class");
                this.LiSendToRegitration.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonDeleteLog.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.DeleteLogFrom);
                this.LiDeleteLog.Attributes.Remove("class");
                this.LiDeleteLog.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonSearchComponentsToAdd.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchComponentToAddForm);
                this.LiSearchComponentsToAdd.Attributes.Remove("class");
                this.LiSearchComponentsToAdd.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }
            else if (linkButtonId == this.LinkButtonSearchComponentsToAddRR.ID)
            {
                this.ConfigurationRegistryRecord.COEFormHelper.Load(COEFormHelper.COEFormGroups.SearchComponentToAddFormRR);
                this.LiSearchComponentsToAddRR.Attributes.Remove("class");
                this.LiSearchComponentsToAddRR.Attributes.Add("class", SELECTEDTABSHEADERCLASSNAME);
            }

            this.XmlControl.DocumentContent = this.ConfigurationRegistryRecord.FormGroup.ToString();
            this.XmlControl.DataBind();
            this.MessagesAreaUserControl.Visible = false;
        }
        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.AreaText = messageToDisplay;
            this.MessagesAreaUserControl.Visible = true;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion
    }
}
