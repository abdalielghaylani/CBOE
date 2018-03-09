using System;
using System.Data;
using System.Configuration;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Reflection;

public partial class Forms_ContentArea_About : GUIShellPage
{
    #region Variables
    public string _listOfApps = null;
    public string _datasource = null;
    Forms_Master_MasterPage _masterPage = null;
    #endregion
    #region Events Handlers
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if(Page.PreviousPage != null && Page.PreviousPage.MasterPageFile != null)
            this.MasterPageFile = Page.PreviousPage.MasterPageFile;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
        }

        foreach (string sapp in CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetAllAppNamesInConfig())
        {
            _listOfApps += sapp + "<br />";   
        }
        
        _datasource = ConfigurationUtilities.GetDefaultDataSource();

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    
    protected override void OnInit(EventArgs e)
    {
        if (this.Master is Forms_Master_MasterPage)
        {
            _masterPage = (Forms_Master_MasterPage)this.Master;

            if (this._masterPage.FindControl("HeaderContainer") != null)
                this._masterPage.FindControl("HeaderContainer").Visible = false;
            
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
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
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
