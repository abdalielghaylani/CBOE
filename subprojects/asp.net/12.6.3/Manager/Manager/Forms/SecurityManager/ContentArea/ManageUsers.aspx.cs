using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;

public partial class Forms_ContentArea_ManageUsers : GUIShellPage
{
    #region Events Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.ShowSecuritySettings();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Buttons Handlers
        this.DoneImageButton.ButtonClicked    += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DoneImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoHome();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region GUIShell Methods
    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.Manager_Users_Title);
        this.Page.Title = Resources.Resource.COESecurity_Page_Title;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer(Constants.PublicContentAreaFolder + Resources.Resource.COE_HOME, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void UnBindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Dictionary<int, string> selectedUsers = null;
        //if (UControl != null)
        //    selectedUsers = ((ManageUsersUC)UControl).UnBind();
        Server.Transfer("ValidationSummary.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ShowSecuritySettings()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
}