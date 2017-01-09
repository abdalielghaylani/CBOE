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
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Resources;
using CambridgeSoft.COE.Framework.COESearchService;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration;

namespace PerkinElmer.CBOE.Registration.Client.Forms.BulkRegisterMarked.ContentArea
{
    public partial class BulkRegister : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;


        #endregion

        #region Properties
        private int LogId
        {
            get
            {
                if (ViewState["LogId"] == null)
                    ViewState["LogId"] = 0;

                return (int)ViewState["LogId"];
            }
            set
            {
                ViewState["LogId"] = value;
            }
        }

        private int SavedHitlistID
        {
            get
            {
                try
                {
                    if (ViewState["saveHitlistID"] == null)
                        ViewState["saveHitlistID"] = Request.QueryString["COESavedHitListID"];

                    return int.Parse(ViewState["saveHitlistID"].ToString());
                }
                catch (Exception)
                {
                    throw new Csla.Validation.ValidationException(Resource.InvalidOrNoSavedHitListID_Exception);
                }
            }
            set
            {
                ViewState["saveHitlistID"] = value;
            }
        }

        private int RegistriesCount
        {
            get
            {
                if (ViewState["Registries"] == null)
                {
                    ViewState["Registries"] = RegistryRecordList.GetTempRecordCount(SavedHitlistID);
                }

                return (int)ViewState["Registries"];
            }
            set
            {
                ViewState["Registries"] = value;
            }
        }

        private int MaxRegisterMarked
        {
            get
            {
                if (Session["MaxRegisterMarked"] == null)
                {
                    string configSetting = RegUtilities.GetConfigSetting(RegUtilities.Groups.Misc, "MaxRegisterMarked");
                    Session["MaxRegisterMarked"] = string.IsNullOrEmpty(configSetting) ? int.MaxValue : int.Parse(configSetting);
                }

                return (int)Session["MaxRegisterMarked"];
            }
        }
        #endregion

        #region Events Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();

                    if (string.IsNullOrEmpty(Request.QueryString["LogId"]))
                    {
                        this.SetPageMode(PageState.Start);
                        this.CheckMarkedHitlistSize();
                    }
                    else
                    {
                        this.LogId = int.Parse(Request.QueryString["LogId"]);
                        SetPageMode(PageState.End);
                        GoToLog();
                    }
                }
                this.SetPageTitle();
                _masterPage.MakeCtrlShowProgressModal(this.RegisterMarkedButton.ClientID, Resource.Registering_Label_Text, string.Empty, true);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //Fix for CSBR- 157540
            //Hitting Cancel button while Bulk registration ends up with error message
            string url = Resource.ReviewRegisterSearch_URL + "?Caller=LASTSEARCH";
            Server.Transfer(url, false);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            base.OnInit(e);
        }

        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DoneButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Response.Redirect(string.Format("{0}?Caller=RR", Resources.Resource.ReviewRegisterSearch_URL));
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
        protected override void SetControlsAttributtes()
        {
            this.GoHomeButton.Text = Resource.Cancel_Button_Text;
            this.DoneButton.Text = Resource.Done_Button_Text;
            this.DoneButton.ToolTip = Resource.ReturnToSearchList_Button_ToolTip;


            this.DuplicateActionLabel.Text = Resource.RegisterMarked_ChooseDupliAction_Label_Text;
            this.RegisterMarkedButton.Text = Resource.RegisterMarked_Button_Text;

            /*this.LogIdLabel.Text = Resource.RegisterMarked_LogId_Label_Text;*/
            this.DescriptionLabel.Text = Resource.RegisterMarked_Description_Label_Text;

            this.DuplicateRadioButton.Text = Resource.RegisterMarked_Duplicate_RadioButton_Text;
            this.BatchRadioButton.Text = Resource.RegisterMarked_Batch_RadioButton_Text;
            this.TempRadioButton.Text = Resource.RegisterMarked_Temporary_RadioButton_Text;
            this.NewRegisterRadioButton.Text = Resource.RegisterMarked_None_RadioButton_Text;
            this.LogLinkButton.Text = "View Log";

            DuplicateRadioButton.Enabled = true;
            BatchRadioButton.Enabled = true;
            TempRadioButton.Enabled = true;
            NewRegisterRadioButton.Checked = true;

            RegisterPanel.Visible = true;
            PageTitleLabel.Text = Resource.Bulk_Register_Marked_Text;
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.Bulk_Register_Marked_Text;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Events

        private DuplicateAction GetDuplicateAction()
        {
            DuplicateAction duplicateAction = DuplicateAction.None;
            if (DuplicateRadioButton.Checked)
            {
                duplicateAction = DuplicateAction.Duplicate;
            }
            if (BatchRadioButton.Checked)
            {
                duplicateAction = DuplicateAction.Batch;
            }
            if (TempRadioButton.Checked)
            {
                duplicateAction = DuplicateAction.Temporary;
            }
            if (NewRegisterRadioButton.Checked)
            {
                duplicateAction = DuplicateAction.None;
            }
            return duplicateAction;
        }

        protected void RegisterMarkedButton_Click(object sender, EventArgs e)
        {
            try
            {
                DoRegisterMarked();
                //DataTable dtLogInfo = RegistryRecordList.GetLogInfoTable(LogId);
                this.PromptMsg1Label.Text = "Operation Succeded";
                string queryLessUrl = string.IsNullOrEmpty(Request.Url.Query) ? Request.Url.AbsoluteUri : Request.Url.AbsoluteUri.Remove(Request.Url.AbsoluteUri.IndexOf("?"));
                this._masterPage.CurrentPage = new Uri(queryLessUrl + "?LogId=" + this.LogId);
                //Server.Transfer("RegisterMarked.aspx?LogId=" + this.LogId);
                SetPageMode(PageState.End);
                GoToLog();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
                _masterPage.DisplayErrorMessage(ex, false);
            }
        }

        protected void LogLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                GoToLog();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Private Methods

        protected enum PageState
        {
            Start,
            End,
            Log,
            Error,
        }

        /// <summary>
        /// Provides an abstraction for the GUI event; also allows us to bypass
        /// user-interaction as desired.
        /// </summary>
        private void GoToLog()
        {
            //string strUrl = Request.ServerVariables.Get("Server_Name").ToString();
            string strParam = RegistryRecordList.chemVizUrl("LOG_BULKREGISTRATION");
            string[] strParams;
            strParams = strParam.Split(char.Parse(","));
            string strUrl = GUIShellUtilities.GetSearchEngineURL() + "?FormGroupId=" + strParams[0] + "&SearchCriteriaId=" + strParams[1] + "&SearchCriteriaValue=" + LogId + "&embed=ChemBioViz&AppName=REGISTRATION&KeepRecordCountSynchronized=false";
            EmbedCBVForm(strUrl);
        }

        /// <summary>
        /// Performs temp-to-perm migration for the marked 'hitlist' (here, the list of temp. registry records).
        /// Also eliminates the now-orphaned MARKED records.
        /// </summary>
        private void DoRegisterMarked()
        {
            DuplicateAction duplicateAction = GetDuplicateAction();
            RegistryRecordList.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
            RegRecordListInfo _reglistInfo = RegistryRecordList.LoadRegistryRecordList(duplicateAction, SavedHitlistID, RegUtilities.GetApprovalsEnabled(), CambridgeSoft.COE.Framework.Common.COEUser.Name, DescriptionTextBox.Text);
            if (_reglistInfo != null)
            {
                LogId = _reglistInfo.IntLogId;
            }
        }

        private void SetPageMode(PageState state)
        {
            switch (state)
            {
                case PageState.Start:
                    RegisterPanel.Visible = true;
                    ShowLogInfoPanel.Visible = false;
                    SearchTempFrame.Visible = false;
                    DoneButton.Visible = false;
                    break;
                case PageState.End:
                    RegisterPanel.Visible = false;
                    ShowLogInfoPanel.Visible = true;
                    SearchTempFrame.Visible = false;
                    DoneButton.Visible = true;
                    /*CSBR # - 123646
                     *Date - 26-Jun-2010
                     *Changed by - Soorya Anwar
                     *Purpose - To make the Cancel button on the page invisible once the record has been registered
                     */
                    GoHomeButton.Visible = false;
                    //End of Change
                    break;
                case PageState.Log:
                    RegisterPanel.Visible = false;
                    ShowLogInfoPanel.Visible = false;
                    SearchTempFrame.Visible = true;
                    DoneButton.Visible = true;
                    break;
                case PageState.Error:
                    this.RegisterPanel.Visible = false;
                    this.ShowLogInfoPanel.Visible = false;
                    this.SearchTempFrame.Visible = false;
                    this.DoneButton.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Displays the search frame and provides hands-off parameters for pulling search results.
        /// </summary>
        /// <param name="strUrl">The URL from hwich to fetch the CBV results grid</param>
        private void EmbedCBVForm(string strUrl)
        {
            try
            {
                RegisterPanel.Visible = false;
                ShowLogInfoPanel.Visible = false;

                ((GUIShellMaster)this.Master).ShowLeftPanel = false;

                SearchTempFrame.Visible = true;
                this.SearchTempFrame.Attributes["src"] = strUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CheckMarkedHitlistSize()
        {
            if (RegistriesCount < 1)
            {
                this.SetPageMode(PageState.Error);
                ShowConfirmationMessage(Resource.NoRegistriesMarked_Error_Text);
            }
            else if (RegistriesCount > this.MaxRegisterMarked)
            {
                this.SetPageMode(PageState.Error);
                ShowConfirmationMessage(string.Format(Resource.MaxRegistriesMarkedExeeded_Error_Text, this.MaxRegisterMarked));
            }
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!string.IsNullOrEmpty(messageToDisplay))
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}
