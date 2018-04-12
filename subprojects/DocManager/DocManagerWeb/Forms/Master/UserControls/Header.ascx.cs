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
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Infragistics.WebUI.UltraWebToolbar;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using Infragistics.WebUI.UltraWebNavigator;
using Resources;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.DocManagerWeb.Forms.Master.UserControls
{
    public partial class Header : System.Web.UI.UserControl, ICOEHeaderUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributes();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);                
            }
        }

        private void SetControlsAttributes()
        {
            this.WelcomeLiteral.Text = Resource.HomeTitle_Label_Text;
            this.UserLiteral.Text = HttpContext.Current.User.Identity.Name.ToUpper();
            this.LogOffButton.Text = Resource.LogOff_Label_Text;
        }

        protected void UltraWebToolbarControl_ButtonClicked(object sender, ButtonEvent e)
        {
            try
            {
                Server.Transfer(e.Button.TargetURL + "?", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
            }
        }

        protected void GoToUltraWebMenu_MenuItemClicked(object sender, WebMenuItemEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Item.TargetUrl))
                    Server.Transfer(e.Item.TargetUrl, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
            }
        }

        protected void DoLogOff(object sender, EventArgs e)
        {
            GUIShellUtilities.DoLogout();
        }

        #region ICOEHeaderUC Members

        public void DataBind(COELogo logo, COEMenu menu, COEMenu toolbar)
        {
            try
            {
                if (this.Page.Master is GUIShellMaster)
                {
                    ((GUIShellMaster)this.Page.Master).SetLogoAttributes(logo, this.LogoContainer);
                    ((GUIShellMaster)this.Page.Master).SetMenuAttributes(menu, this.GoToUltraWebMenu);
                    ((GUIShellMaster)this.Page.Master).SetToolBarAttributtes(toolbar, this.UltraWebToolbarControl);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
            }
        }

        #endregion
    }
}