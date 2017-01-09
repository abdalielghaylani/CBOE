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
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Csla;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using PerkinElmer.COE.Registration.Server.Forms.Master;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Forms.SubmitRecord.ContentArea
{
    public partial class SaveMixtureForm : System.Web.UI.Page
    {
        #region Page Variables

        int _formGroup = (int)FormGroups.SingleCompound;

        #endregion

        #region GUIShell Variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.ClearErrorMessage();

                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                }
                this.SetPageTitle();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            
            #region Page Settings
            if (Request[PerkinElmer.COE.Registration.Server.Constants.FormGroup_UrlParameter] != null)
                int.TryParse(Request[PerkinElmer.COE.Registration.Server.Constants.FormGroup_UrlParameter], out _formGroup);
            // To make easier to read the code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            
            #endregion
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                string savedObjectId = this.SaveCompoundForm();

                Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.SubmitMixture_URL, PerkinElmer.COE.Registration.Server.Constants.SavedObjectId_UrlParameter, savedObjectId, PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter, Request[PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter]), false);
            }
            //catch (DataPortalException nae)
            //{
            //    ShowConfirmationMessage(nae.BusinessException.Message);
            //}
            catch (Exception exception)
            {
                //if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.SubmitMixture_URL, PerkinElmer.COE.Registration.Server.Constants.SavedObjectId_UrlParameter, -1, PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter, Request[PerkinElmer.COE.Registration.Server.Constants.RegistryTypeParameter]), false);
                //Server.Transfer(Resource.SubmitMixture_URL, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Page.Form.DefaultButton = this.SaveButton.UniqueID;
            Page.Form.DefaultFocus = this.SaveButton.UniqueID;

            this.PageTitleLabel.Text = Resource.SaveCompoundForm_Page_Title;
            this.SaveButton.Text = Resource.SaveTemplate_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.FormNameLabel.Text = Resource.FormName_Label_Text;
            this.FormDescriptionLabel.Text = Resource.FormDescription_Label_Text;
            this.PublicFormCheckBox.Text = Resource.PublicForm_CheckBox_Text;
           
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.SaveCompoundForm_Page_Title;
            this.Header.Title = Resource.SaveCompoundForm_Page_Title;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Clear all the Messages in the Error Panel.
        /// </summary>
        private void ClearErrorMessage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.ErrorMessagesRow.Visible = false;
            this.ErrorMessageLabel.Text = string.Empty;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Page Methods

        /// <summary>
        /// When saving the RegistryRecord xml as a CLOB, take action on it as if it were being saved to temp.
        /// </summary>
        /// <returns>The GenericObject ID assigned to the CLOB record.</returns>
        private string SaveCompoundForm()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COEGenericObjectStorageBO storageBO = null;

            try
            {
                RegistryRecord currentRecord = (RegistryRecord)Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session];
                string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                string name = this.FormNameTextBox.Text;
                string description = this.FormDescriptionTextBox.Text;
                bool isPublic = this.PublicFormCheckBox.Checked;

                if (currentRecord != null)
                    storageBO = currentRecord.SaveTemplate(name, description, isPublic, _formGroup);
                else
                    throw new NoRecordToSaveException();
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            return storageBO.ID.ToString();
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
