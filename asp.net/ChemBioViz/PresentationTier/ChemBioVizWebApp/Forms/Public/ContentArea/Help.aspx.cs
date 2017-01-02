using System;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

public partial class Forms_ContentArea_Help : GUIShellPage
{
    #region Variables

    ChemBioVizMasterPage masterPage = null;
    
    #endregion

    #region Events Handlers

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

        #region Page GUIShell Settings
        // To make easier to read the source code.
        masterPage = (ChemBioVizMasterPage)this.Master;
        #endregion

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
        masterPage.SetPageTitle(Resources.Resource.Help_Page_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
