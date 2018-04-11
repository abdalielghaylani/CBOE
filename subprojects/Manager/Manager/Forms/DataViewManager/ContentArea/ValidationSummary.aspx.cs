using System;
using System.Data;
using System.Configuration;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using System.Web.UI;
using CambridgeSoft.COE.Framework.COEDataViewService;

public partial class Forms_DataViewManger_ContentArea_ValidationSummary : GUIShellPage
{
    #region Properties

    private string PreviousPageURL
    {
        get
        {
            return ViewState["PreviousPageURL"] != null ? (string)ViewState["PreviousPageURL"] : string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState["PreviousPageURL"] = value;
        }
    }
    #endregion

    #region Events Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.DisplayValidationSummary();
            this.SetControlsAttributtes();
        }
        if (Page.PreviousPage != null)
        {
            this.PreviousPageURL = Page.PreviousPage.AppRelativeVirtualPath;
            if (!string.IsNullOrEmpty(this.Request.QueryString[Constants.ParamCaller]))
                this.PreviousPageURL += "?" + Constants.ParamCaller + "=" + this.Request.QueryString[Constants.ParamCaller];
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Button Event Handlers
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.SubmitImageButton.ButtonClicked += new EventHandler<EventArgs>(SubmitImageButton_ButtonClicked);
        this.CancelImageButton1.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.SubmitImageButton1.ButtonClicked += new EventHandler<EventArgs>(SubmitImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void SubmitImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsValid)
            this.SubmitDataView();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void BackImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("Security.aspx", true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string transferUrl = this.PreviousPageURL;
        if (string.IsNullOrEmpty(transferUrl))
            transferUrl = "DataviewBoard.aspx";
        Server.Transfer(transferUrl, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region GUIShell Methods

    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    /// <remarks>You have to expand the group of your interest in the accordion</remarks>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.ValidationSummary_Page_Title);
        //Read the isValid property in the UC to enable or not the Submit button.
        if (this.ValidationSummaryUserControl != null)
            this.SubmitImageButton.Enabled = this.SubmitImageButton1.Enabled = this.ValidationSummaryUserControl.IsValid;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private void SubmitDataView()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (dataViewBO != null)
        {
            bool errors = false;
            try
            {
                dataViewBO.SaveFromDataViewManager();
                //Clean Session variables.
                Session.Remove(Constants.COEDataViewBO);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                errors = true;
                if (ex is Csla.DataPortalException)
                    message = ((Csla.DataPortalException)ex).BusinessException.Message;
                this.DisplayErrorMessage(message);
            }
            if (!errors)
            {
                Session["IsBaseEnable"] = Convert.ToBoolean(0);
                Session["MasterSchema"] = null;
                Session["SchemaChange"] = null;
                this.Master.DisplayMessagesPage(Constants.MessagesCode.SubmittedDataView, GUIShellTypes.MessagesButtonType.Close);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Go to home page
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Bind dataview datasource to the UC in charge to display the information.
    /// </summary>
    private void DisplayValidationSummary()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.ValidationSummaryUserControl != null && dataViewBO != null)
            this.ValidationSummaryUserControl.DataBind(dataViewBO);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel)((Forms_DataViewManger_ContentArea_ValidationSummary)this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
        if (string.IsNullOrEmpty(message))
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = false;
        else
        {
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Text = message;
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = true;
        }
        errorAreaUpatePanel.Update();
    }
    #endregion
}
