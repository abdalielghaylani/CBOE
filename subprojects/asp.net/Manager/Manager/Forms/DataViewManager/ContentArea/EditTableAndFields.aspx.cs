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
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;

public partial class Forms_DataViewManager_ContentArea_EditTableAndFields : GUIShellPage
{
    #region Properties
    private string CurrentTableID
    {
        get
        {
            return Request[Constants.ParamCaller];
        }
    }
    #endregion

    #region Variables
    private const string _goBackToMasterDVEditing = "MDVEditing";
    #endregion

    #region Page Life Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.ShowTableAndFields();         
        }
        
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    
    #endregion

    #region Methods
    /// <summary>
    /// Displays all the tables and fields information
    /// </summary>
    private void ShowTableAndFields()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.EditTableAndFieldsUserControl != null && dataViewBO != null)
        {
            try
            {
                this.EditTableAndFieldsUserControl.DataBind(dataViewBO, this.CurrentTableID);
            }
            catch (Exception ex)
            {
                this.Master.DisplayErrorMessage(ex.Message);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.EditTableAndFields_Page_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
