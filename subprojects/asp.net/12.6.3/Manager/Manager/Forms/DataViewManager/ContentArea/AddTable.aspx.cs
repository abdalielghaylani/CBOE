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
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;

namespace Manager.Forms.DataViewManager.ContentArea
{
    public partial class AddTable : GUIShellPage
    {
        #region Page Life Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!Page.IsPostBack)
            {
                this.SetControlsAttributtes();
                this.ShowTables();
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays the tables in the master dv
        /// </summary>
        private void ShowTables()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.AddTableUserControl.Bind();
            }
            catch (Exception ex)
            {
                this.Master.DisplayErrorMessage(ex.Message);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.SetPageTitle(Resources.Resource.AddTable_Page_Title);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

    }
}
