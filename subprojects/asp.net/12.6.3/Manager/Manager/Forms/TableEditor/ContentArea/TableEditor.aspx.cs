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
using Resources;
using System.Reflection;

namespace Manager.Forms.TableEditor.ContentArea
{
    public partial class TableEditor : GUIShellPage
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if(!Page.IsPostBack)
                this.SetControlsAttributtes();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Master.SetPageTitle(Resource.TableEditor_Page_Title);
            this.PageTitleLabel.Text = Resource.TableEditor_Title_Text;
            //this.GoHomeButton.Text = Resource.Home_Label_Text;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            base.OnPreRender(e);
            string buttonID = this.COETableManager1.GetDefaultButtonID();
            if(!string.IsNullOrEmpty(buttonID))
                this.Master.SetDefaultButton(buttonID);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /*protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.LeavePage();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }*/

        #endregion

        #region Methods

        private void LeavePage()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string url = "../../Public/ContentArea/Home.aspx";
            Server.Transfer(url, false);
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        #endregion
    }
}
