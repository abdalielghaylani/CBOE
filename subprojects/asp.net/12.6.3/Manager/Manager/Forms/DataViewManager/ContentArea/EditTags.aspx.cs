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
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;


public partial class EditTags : GUIShellPage
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

    #region Page Life Cycle Events
    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            this.ShowTags();
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods
    /// <summary>
    /// Displays all the tags for the table
    /// </summary>
    private void ShowTags()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.EditTagsUserControl != null && dataViewBO != null)
        {
            try
            {
                this.EditTagsUserControl.DataBind(dataViewBO, this.CurrentTableID);
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
        this.Master.SetPageTitle(Resources.Resource.EditTags_Page_Title);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

