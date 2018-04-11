using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COESecurityService;

public partial class Forms_DataViewManager_ContentArea_Security : GUIShellPage
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
        this.NextImageButton.ButtonClicked      += new EventHandler<EventArgs>(NextImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked    += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("DataviewBoard.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void NextImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsValid)
            this.UnBindInfo();
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
        this.Master.SetPageTitle(Resources.Resource.Secutiry_Page_Title);
        this.NextImageButton.ButtonText = Resources.Resource.OK_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Extract control information and return a COEAccessRights object
    /// </summary>
    private void UnBindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        COEAccessRightsBO accessRights = null;
        if (this.SecurityUserControl != null)
            accessRights = this.SecurityUserControl.UnBind();
        dataViewBO.COEAccessRights = accessRights;
        dataViewBO.IsPublic = dataViewBO.COEAccessRights == null ? true : false;
        this.Master.SetDataViewBO(dataViewBO);
        Server.Transfer("DataviewBoard.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Displays the controls to select the DV access settings (roles & users).
    /// </summary>
    private void ShowSecuritySettings()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        this.SecurityUserControl.DataBind(this.MergeLists(COEUserReadOnlyBOList.GetList(), dataViewBO.COEAccessRights != null && !dataViewBO.IsPublic ? dataViewBO.COEAccessRights.Users : null),
                                    dataViewBO.COEAccessRights != null && !dataViewBO.IsPublic ? dataViewBO.COEAccessRights.Users : null,
                                    this.MergeLists(COERoleReadOnlyBOList.GetList(), dataViewBO.COEAccessRights != null && !dataViewBO.IsPublic ? dataViewBO.COEAccessRights.Roles : null),
                                    dataViewBO.COEAccessRights != null && !dataViewBO.IsPublic ? dataViewBO.COEAccessRights.Roles : null); 

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Merge to just display two unique lists of users. We remove the users that the DV has already set in its COEAccessRight obj.
    /// </summary>
    /// <param name="fullList">All the system users</param>
    /// <param name="partialList">Selected users in the current DV</param>
    /// <returns></returns>
    private COEUserReadOnlyBOList MergeLists(COEUserReadOnlyBOList fullList, COEUserReadOnlyBOList partialList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (partialList != null)
        {
            foreach (COEUserReadOnlyBO user in partialList)
            {
                if (fullList.Contains(user.PersonID))
                    fullList.RemoveUser(user);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return fullList;
    }

    /// <summary>
    /// Merge to just display two unique lists of users. We remove the users that the DV has already set in its COEAccessRight obj.
    /// </summary>
    /// <param name="fullList">All the system roles</param>
    /// <param name="partialList">Selected roles in the current DV</param>
    /// <returns></returns>
    private COERoleReadOnlyBOList MergeLists(COERoleReadOnlyBOList fullList, COERoleReadOnlyBOList partialList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (partialList != null)
        {
            foreach (COERoleReadOnlyBO role in partialList)
            {
                if (fullList.Contains(role.RoleID))
                    fullList.RemoveRole(role);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return fullList;
    }

    #endregion
}
