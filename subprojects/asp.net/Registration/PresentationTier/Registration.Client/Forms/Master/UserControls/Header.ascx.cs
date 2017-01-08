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

namespace PerkinElmer.CBOE.Registration.Client.Forms.Master.UserControls
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
        
        /// <summary>
        /// Method to customize the changes required for User Preference in header user control (for the user preference page)
        /// </summary>
        public void CustomizeUserPreference()
        {
            this.GoToUltraWebMenu.Visible = false;
            this.UltraWebToolbarControl.Visible = false;
            this.LogOffButton.Visible = false;
            this.LogoContainer.Attributes.Add("OnClick", "");
            this.LogoContainer.Style.Add("margin-left", "0px");
            this.TopContainer.Style.Add("width", "730px");
            this.MenuContainer.Style.Add("width", "100%");
            this.UserWelcomeContainer.Style.Add("width", "250px");
            this.LogOffContainer.Visible=false;
            this.CommonHeaderTable.Style.Add("width", "730px");
        }

        protected void UltraWebToolbarControl_ButtonClicked(object sender, ButtonEvent e)
        {
            try
            {
                Server.Transfer(e.Button.TargetURL + "?", false);
                //if (e.Button.Key.ToUpper() == "LOGOFF")
                //{
                //    GUIShellUtilities.DoLogout();
                //    if (!CambridgeSoft.COE.Registration.Services.Common.Utilities.SimulationMode())
                //        Response.Redirect(ConfigurationManager.AppSettings["LogoutUrl"]);
                //}
                //else
                //{
                //    Server.Transfer(e.Button.TargetURL + "?", false);
                //}
            }
            catch (Exception exception)
            {
                //if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    Server.Transfer("~/Forms/ContentArea/Messages.aspx?MessageCode=" + CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString() + "&");
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