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
using CambridgeSoft.COE.Framework.Controls.COEPageCtrlSettingManager;

namespace Manager.Forms.PagerControlSettings
{
    public partial class PageControlSettings : GUIShellPage
    {

        #region Variables

        private enum Caller
        {
            DVMGR,
            SEC,
            EDITRR,
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.CheckCOEPageSettingsIsEnable())
            {
                if (!Page.IsPostBack)
                    this.SetControlsAttributtes();
            }
            else
                Server.Transfer("~/Forms/Public/ContentArea/Messages.aspx?MessageCode=" + GUIShellTypes.MessagesCode.PageSettingsDisable, false);
        }

        #endregion

        #region Methods

        protected override void SetControlsAttributtes()
        {
            string url = string.Empty;
            if (Request[GUIShellTypes.PageSettings_AppName] != null)
            {
                if (!string.IsNullOrEmpty(Request[GUIShellTypes.PageSettings_AppName].ToUpper()))
                    this.COEPageCtrlSettingManager1.AppName = Request[GUIShellTypes.PageSettings_AppName].ToUpper();
                url = "~/Forms/SecurityManager/ContentArea/EditRoleRoles.aspx?appName=" + Request[GUIShellTypes.PageSettings_AppName].ToString();
                if (Request.QueryString["RoleName"] != null)
                    url += "&RoleName=" + Request.QueryString["RoleName"].ToString();
            }
            //Find out who is calling this page, so later the control remembers where to go back.
            if (Request[GUIShellTypes.PageSettings_Caller] != null)
            {
                if (!string.IsNullOrEmpty(Request[GUIShellTypes.PageSettings_Caller].ToUpper()))
                {
                    Caller temp = Caller.EDITRR;
                    if (Enum.IsDefined(typeof(Caller), Request[GUIShellTypes.PageSettings_Caller].ToUpper()))
                        temp = (Caller)Enum.Parse(typeof(Caller), Request[GUIShellTypes.PageSettings_Caller].ToUpper());
                    switch (temp)
                    {
                        case Caller.DVMGR:
                            url = "~/Forms/DataViewManager/HomePage/Home.aspx";
                            break;
                        case Caller.SEC:
                            url = "~/Forms/SecurityManager/HomePage/Home.aspx";
                            break;
                    }
                }
            }
            if(!string.IsNullOrEmpty(url))
                this.COEPageCtrlSettingManager1.PreviousPage = this.ResolveUrl(url); 
        }

        private bool CheckCOEPageSettingsIsEnable()
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting("MANAGER", "MISC", "PageControlsManager")))
                if (CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting("MANAGER", "MISC", "PageControlsManager").ToUpper() == "ENABLE")
                    retVal = true;
            return retVal;
        }

        #endregion
    }
}
