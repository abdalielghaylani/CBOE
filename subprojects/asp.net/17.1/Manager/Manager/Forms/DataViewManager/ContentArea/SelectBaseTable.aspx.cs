using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

public partial class Forms_DataViewManager_ContentArea_SelectBaseTable : GUIShellPage
{
    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.BindInfo();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Button EventHandlers
        this.NextImageButton.ButtonClicked += new EventHandler<EventArgs>(NextImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void NextImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsValid)
            this.SetSelectedBasePage();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("DataviewBoard.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets user selected base table.
    /// </summary>
    private void SetSelectedBasePage()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int baseTableID = int.MinValue;
        if (this.SelectBaseTableUserControl != null)
            baseTableID = this.SelectBaseTableUserControl.Unbind().ID;
        if (baseTableID != int.MinValue)
        {
            COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
            if (dataViewBO != null)
                dataViewBO.DataViewManager.SetBaseTable(baseTableID);
            Session[Constants.COEDataViewBO] = dataViewBO;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("DataviewBoard.aspx", false);
    }

    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.SelectBaseTable_Label_Text);
        this.NextImageButton.ButtonText = this.NextImageButton.ButtonToolTip = Resources.Resource.OK_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Bind the info into the UC.
    /// </summary>
    private void BindInfo()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.SelectBaseTableUserControl != null)
            this.SelectBaseTableUserControl.DataBind();
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Gets selected base table in the current DV.
    /// </summary>
    /// <returns></returns>
    private int GetBaseTableId()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (dataViewBO != null)
            retVal = dataViewBO.DataViewManager.BaseTableId;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    #endregion
}
