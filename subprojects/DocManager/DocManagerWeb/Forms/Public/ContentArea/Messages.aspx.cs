using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

public partial class Forms_ContentArea_Messages : GUIShellPage
{
    #region Variables

    MasterPage _masterPage = null;

    #endregion

    #region Event Handlers

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

        #region Page GUIShell Settings
        // To make easier to read the source code.
        _masterPage = (MasterPage)this.Master;
        #endregion

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
            case Constants.MessagesCode.Unknown:
                this.MessageLabel.Text = Resources.Resource.UnknownErrorMessage_Label_Text
                    + "<br>" + (this.Server.GetLastError() == null ? string.Empty : HttpUtility.HtmlEncode(this.Server.GetLastError().GetBaseException().Message));
                break;
            case Constants.MessagesCode.InvalidHttpRequest:
                this.MessageLabel.Text = Resources.Resource.InvalidHttpRequest_Label_Text;
                break;
            case Constants.MessagesCode.InvalidEnteredInput:
                this.MessageLabel.Text = Resources.Resource.InvalidEnteredInput_Lable_Text;
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
            case GUIShellTypes.MessagesButtonType.None:
                this.GoBackButton.Visible = false;
                break;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
}
