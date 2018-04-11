using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;

public partial class Forms_ContentArea_EditRoleUsers: GUIShellPage
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
        #endregion
        base.OnInit(e);
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
        this.Master.SetPageTitle(Resources.Resource.Edit_Role_User_Title);
        this.Page.Title = Resources.Resource.COESecurity_Page_Title;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Methods
    private void UnBindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Dictionary<int, string> selectedUsers = null;
        //if (UControl != null)
        //    selectedUsers = ((EditUserUC)UControl).UnBind();
        //Server.Transfer("ValidationSummary.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ShowSecuritySettings()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ////Just for testing purpose:
        //Dictionary<int, string> allUsers = new Dictionary<int, string>();
        //allUsers.Add(1, "T5_85");
        //allUsers.Add(2, "BIOSAR_ADMIN");
        //allUsers.Add(3, "ULISES");
        //allUsers.Add(4, "T4_85");
        //allUsers.Add(5, "T5_86");
        //allUsers.Add(6, "T4_86");
        //allUsers.Add(7, "BIOSAR_ADMIN_7");
        //allUsers.Add(8, "BIOSAR_ADMIN_8");
        //allUsers.Add(9, "BIOSAR_ADMIN_9");
        //allUsers.Add(10, "BIOSAR_ADMIN_10");

        //Dictionary<int, string> selectedUsers = new Dictionary<int, string>();
        //selectedUsers.Add(11, "BIOSAR_ADMIN_11");
        //selectedUsers.Add(12, "BIOSAR_ADMIN_12");
        ////End testing purpose code.

        //if (allUsers != null && UControl != null && selectedUsers != null)
        //    ((EditUserUC)UControl).DataBind(allUsers, selectedUsers);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        Forms_ContentArea_ManageUsers mngUsers = new Forms_ContentArea_ManageUsers();
        mngUsers.CheckPrivilege("ManageRoles");
    }
    #endregion
}
