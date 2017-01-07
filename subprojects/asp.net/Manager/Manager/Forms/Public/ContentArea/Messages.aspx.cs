using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

public partial class Forms_Public_ContentArea_Messages : GUIShellPage
{
    #region Event Handlers

    protected override void OnPreInit(EventArgs e)
    {
        if(Page.PreviousPage != null && Page.PreviousPage.Master != null)
        {
            this.MasterPageFile = Page.PreviousPage.MasterPageFile;
            this.Page.Session["MasterPageFile"] = Page.PreviousPage.MasterPageFile;
        }
        else if(this.Page.Session["MasterPageFile"] != null)
        {
            this.MasterPageFile = (string) this.Page.Session["MasterPageFile"];
        }
								else
            this.Page.Session["MasterPageFile"] = null;
        base.OnPreInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.ReadMessageToDisplay();
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    #endregion

    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    /// <remarks>You have to expand the group of your interest in the accordion</remarks>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //this.Master.SetPageTitle(Resources.Resource.Messages_Page_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to display in the messages in the page. 
    /// </summary>
    private void ReadMessageToDisplay()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string textToDisplay = String.Empty;
        Constants.MessagesCode messageCode = Constants.MessagesCode.None;
        GUIShellTypes.MessagesButtonType buttonType = GUIShellTypes.MessagesButtonType.None;
        if (!string.IsNullOrEmpty(Request[GUIShellTypes.RequestVars.MessageCode.ToString()]))
            messageCode = (Constants.MessagesCode)Enum.Parse(typeof(Constants.MessagesCode), Request[GUIShellTypes.RequestVars.MessageCode.ToString()].ToString());
        if (!string.IsNullOrEmpty(Request[GUIShellTypes.RequestVars.MessagesButtonType.ToString()]))
            buttonType = (GUIShellTypes.MessagesButtonType)Enum.Parse(typeof(GUIShellTypes.MessagesButtonType), Request[GUIShellTypes.RequestVars.MessagesButtonType.ToString()].ToString());
        this.SetLabelText(messageCode);
        this.SetButtonAttributes(buttonType);
    }

    /// <summary>
    /// Sets the test to displau according the message code
    /// </summary>
    /// <param name="messageCode"></param>
    private void SetLabelText(Constants.MessagesCode messageCode)
    {        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        switch (messageCode)
        {
            case Constants.MessagesCode.InvalidDataView:
                this.MessageLabel.Text = Resources.Resource.InvalidDataView_Label_Text;
                break;
            case Constants.MessagesCode.SubmittedDataView:
                this.MessageLabel.Text = Resources.Resource.SuccessfullySaveDataView_Label_Text;
                break;
            case Constants.MessagesCode.DeletedDataView:
                this.MessageLabel.Text = Resources.Resource.SuccessfullyDeletedDataView_Label_Text;
                break;
            case Constants.MessagesCode.MasterDataViewExists:
                this.MessageLabel.Text = Resources.Resource.MasterDVExists_Label_Text;
                break;
            case Constants.MessagesCode.NoDataViews:
                this.MessageLabel.Text = Resources.Resource.NoDataViews_Label_Text;
                break;
            case Constants.MessagesCode.NoMasterDataView:
                this.MessageLabel.Text = Resources.Resource.NoMasterDataView_Label_Text;
                break;
            case Constants.MessagesCode.SessionTimeOut:
                this.MessageLabel.Text = Resources.Resource.SessiontimeOut_Label_Text;
                break;
            case Constants.MessagesCode.PageSettingsDisable:
                this.MessageLabel.Text = Resources.Resource.PageSettingsDisable_Label_Text;
                break;
            case Constants.MessagesCode.Unknown:
                // Coverity Fixes: CBOE-313
                Exception lastError = this.Server.GetLastError();
                if (lastError != null)
                    this.MessageLabel.Text = Resources.Resource.UnknownErrorMessage_Label_Text + "<br>" + lastError.GetBaseException().Message;
                break;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Sets the button attributes.
    /// </summary>
    /// <param name="buttonType"></param>
    private void SetButtonAttributes(GUIShellTypes.MessagesButtonType buttonType)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        switch (buttonType)
        {
            case GUIShellTypes.MessagesButtonType.Back:
                this.GoBackButton.Value = Resources.Resource.Back_Button_Text;
                this.GoBackButton.Attributes.Add("onclick", "jscript: history.back(-1);");
                break;
            case GUIShellTypes.MessagesButtonType.Close:
                this.GoBackButton.Value = Resources.Resource.Close_Button_Text;
                this.GoBackButton.Attributes.Add("onclick", "jscript: window.close();");
                break;
            case GUIShellTypes.MessagesButtonType.Login:
                this.GoBackButton.Value = Resources.Resource.Close_Button_Text;
                this.GoBackButton.Attributes.Add("onclick", "jscript: self.location.href='" + this.ResolveClientUrl("Login.aspx") + "';");
                break;
            case GUIShellTypes.MessagesButtonType.Home:
                this.GoBackButton.Value = Resources.Resource.Home_Button_Text;
                this.GoBackButton.Attributes.Add("onclick", "jscript: self.location.href='" + this.ResolveClientUrl(Resources.Resource.COE_HOME) + "';");
                break;
            case GUIShellTypes.MessagesButtonType.None:
                this.GoBackButton.Visible = false;
                break;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
}
