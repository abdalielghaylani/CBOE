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
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.BulkRegisterMarked.ContentArea
{
    public partial class ApproveMarked : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        #endregion

        #region Properties
        private int COEHitlistID
        {
            get
            {
                try
                {
                    if(ViewState["saveHitlistID"] == null)
                        ViewState["saveHitlistID"] = Request.QueryString["COEHitListID"];

                    return int.Parse(ViewState["saveHitlistID"].ToString());
                }
                catch(Exception)
                {
                    throw new Csla.Validation.ValidationException(Resource.InvalidOrNoSavedHitListID_Exception);
                }
            }
            set
            {
                ViewState["saveHitlistID"] = value;
            }
        }
        #endregion

        #region Page Events
        protected override void OnInit(EventArgs e)
        {
            try
            {
                if(this.Master is RegistrationMaster)
                {
                    _masterPage = (RegistrationMaster)this.Master;
                    _masterPage.ShowLeftPanel = false;
                }
                base.OnInit(e);
            }
            catch(Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();

                    this.ApproveMarkedRegistries();
                }
            }
            catch(Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

        }
        #endregion

        #region Controls Events
        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(Resource.RegisterMarked_LeavePage_Url_Text, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(Resources.Resource.ReviewRegisterSearch_URL + "?Caller=LASTSEARCH", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Methods

        private void ApproveMarkedRegistries()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if(!BulkApprove.Execute(this.COEHitlistID))
                ShowConfirmationMessage(Resource.OperationProblem_Label_Text);
            else
                ShowConfirmationMessage(Resource.OperationSucceeded_Label_Text);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.PageTitleLabel.Text = Resource.ApproveMarked_Page_Title;
            this.ExitButton.Text = Resource.Done_Button_Text;
            this.ExitButton.ToolTip = Resource.ReturnToSearchList_Button_ToolTip;
            this.GoHomeButton.Text = Resource.Home_Button_Text;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if(!string.IsNullOrEmpty(messageToDisplay))
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
