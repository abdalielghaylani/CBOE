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
using Infragistics.WebUI.UltraWebToolbar;
using Infragistics.WebUI.UltraWebNavigator;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;

namespace Manager.Forms.SecurityManager.UserControls
{
    public partial class SecurityManagerHeader : System.Web.UI.UserControl, ICOEHeaderUC
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.HomeLink.Click += new EventHandler(HomeLink_Click);
            this.MainLink.Click += new EventHandler(MainLink_Click);
            this.HelpLink.Click += new EventHandler(HelpLink_Click);
            this.AboutLink.Click += new EventHandler(AboutLink_Click);
        }

        void AboutLink_Click(object sender, EventArgs e)
        {
            Server.Transfer(this.ResolveUrl("~/Forms/SecurityManager/ContentArea/About.aspx"));
        }

        void HelpLink_Click(object sender, EventArgs e)
        {
            //Server.Transfer(this.ResolveUrl("~/Forms/SecurityManager/ContentArea/Help.aspx"));
            Response.Write("<script>");
            Response.Write("window.open('/cboehelp/CBOEAdminHelp/','_blank')");
            Response.Write("</script>");

        }

        void MainLink_Click(object sender, EventArgs e)
        {
            Server.Transfer(this.ResolveUrl("~/Forms/SecurityManager/HomePage/Home.aspx"));
        }

        void HomeLink_Click(object sender, EventArgs e)
        {
            Server.Transfer(this.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.SetControlsAttributes();
            }
        }

        private void SetControlsAttributes()
        {
            this.WelcomeLiteral.Text = Resources.Resource.Welcome_Label_Text;
            this.UserLiteral.Text = HttpContext.Current.User.Identity.Name.ToUpper();
            this.HomeLink.Text = Resources.Resource.Home_Label_Text;
            this.MainLink.Text = Resources.Resource.Main_HyperLink_Text;
            this.HelpLink.Text = Resources.Resource.Help_Label_Text;
            this.AboutLink.Text = Resources.Resource.About_Label_Text;
            this.LogOffButton.Text = Resources.Resource.LogOff_Label_Text;
            this.LogoContainer.Attributes.Add("onclick", "jscript: self.location.href='" + this.ResolveUrl("~/Forms/SecurityManager/HomePage/Home.aspx") + "'");
        }

        protected void UltraWebToolbarControl_ButtonClicked(object sender, ButtonEvent e)
        {
            try
            {
                Server.Transfer(e.Button.TargetURL + "?", false);
            }
            catch(Exception)
            {
                Server.Transfer("~/Forms/Public/ContentArea/Messages.aspx?MessageCode=" + CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString() + "&");
            }
        }

        protected void GoToUltraWebMenu_MenuItemClicked(object sender, WebMenuItemEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Item.TargetUrl))
                Server.Transfer(e.Item.TargetUrl, false);
        }

        protected void DoLogOff(object sender, EventArgs e)
        {
            GUIShellUtilities.DoLogout();
        }

        #region ICOEHeaderUC Members

        public void DataBind(COELogo logo, COEMenu menu, COEMenu toolbar)
        {
            //if(this.Page.Master is GUIShellMaster)
            //{
            //    ((GUIShellMaster) this.Page.Master).SetLogoAttributes(logo, this.LogoContainer);
            //    ((GUIShellMaster) this.Page.Master).SetMenuAttributes(menu, this.GoToUltraWebMenu);
            //    ((GUIShellMaster) this.Page.Master).SetToolBarAttributtes(toolbar, this.UltraWebToolbarControl);
            //}
        }

        #endregion


        public void SetPageTitle(string pageName)
        {
            this.PageTitleLabel.Text = pageName;
            this.Page.Title = pageName;
        }
    }
}