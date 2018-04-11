using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;

public partial class Forms_ContentArea_ManageGroups : GUIShellPage
{
    #region Variables

    Forms_Master_MasterPage _masterPage = null;
    private const string _userControlID = "ManageGroupsControl";

    #endregion

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
        #region Page GUIShell Settings
        // To make easier to read the source code.
        _masterPage = (Forms_Master_MasterPage)this.Master;
        #endregion
        #region Buttons Handlers
        this.DoneImageButton.ButtonClicked += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);
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
        _masterPage.SetPageTitle("Manage Roles");
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer(this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + Resources.Resource.COE_HOME, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void UnBindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control UControl = _masterPage.FindControlInPage(_userControlID);
        //Dictionary<int, string> selectedUsers = null;
        //if (UControl != null)
        //    selectedUsers = ((ManageUsersUC)UControl).UnBind();
        Server.Transfer("ValidationSummary.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ShowSecuritySettings()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Control UControl = _masterPage.FindControlInPage(_userControlID);
        Forms_ContentArea_ManageUsers mngUsers = new Forms_ContentArea_ManageUsers();
        mngUsers.CheckPrivilege("ManageGroups");
     }
    
    #endregion
}
