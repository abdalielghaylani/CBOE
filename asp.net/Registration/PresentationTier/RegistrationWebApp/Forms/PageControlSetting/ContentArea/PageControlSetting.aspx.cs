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
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace RegistrationWebApp.Forms.PageControlSetting.ContentArea
{
    public partial class PageControlSetting : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.CheckCOEPageSettingsIsEnable())
                {
                    if (!Page.IsPostBack)
                        this.SetControlsAttributtes();
                }
                else
                    _masterPage.DisplayMessagesPage(GUIShellTypes.MessagesCode.PageSettingsDisable, GUIShellTypes.MessagesButtonType.Back);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected override void SetControlsAttributtes()
        {
            string url = this.Page.ResolveUrl(@"~/Forms/Public/ContentArea/Home.aspx");
            this.COEPageCtrlSettingManager1.PreviousPage = this.ResolveUrl(url); 
        }

        private bool CheckCOEPageSettingsIsEnable()
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(RegUtilities.GetConfigSetting("MISC", "PageControlsManager")))
                if (RegUtilities.GetConfigSetting("MISC", "PageControlsManager").ToUpper() == "ENABLE")
                    retVal = true;
            return retVal;
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                if (this.Master is RegistrationMaster)
                {
                    _masterPage = (RegistrationMaster)this.Master;
                    _masterPage.ShowLeftPanel = false;
                }
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        #endregion
    }
}
