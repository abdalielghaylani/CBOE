using System;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

public partial class Forms_ContentArea_Help : GUIShellPage
{
    #region Events Handlers
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if(Page.PreviousPage != null && Page.PreviousPage.Master != null)
            this.MasterPageFile = Page.PreviousPage.MasterPageFile;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();   
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
