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
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.WebUI.UltraWebToolbar;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;

public partial class Forms_Master_UserControls_Header : System.Web.UI.UserControl, ICOEHeaderUC
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.HomeLink.Click += new EventHandler(HomeLink_Click);
        // Commented Link button functionality, Because added new Linkbutton.
        //this.AboutLink.Click += new EventHandler(AboutLink_Click);
    }

    void AboutLink_Click(object sender, EventArgs e)
    {
        Server.Transfer(this.ResolveUrl("~/Forms/Public/ContentArea/about.aspx"));
    }

    void HomeLink_Click(object sender, EventArgs e)
    {
        Server.Transfer(this.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx"));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Code to open new window for About.aspx
        GetAboutLinkUrl();
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
        this.HelpLink.InnerText = Resources.Resource.Help_Label_Text;
        this.HelpLink.HRef = "/CBOEHelp/CBOEUserHelp/";
        this.AdminHelpLink.InnerText = Resources.Resource.AdminGuide_Label_Text;
        this.AdminHelpLink.HRef = "/CBOEHelp/CBOEAdminHelp/";
        // Commented Link button functionality,Because added new Linkbutton.
        //this.AboutLink.Text = Resources.Resource.About_Label_Text;
        this.LogOffButton.Text = Resources.Resource.LogOff_Label_Text;
        this.LogoContainer.Attributes.Add("onclick", "jscript: self.location.href='" + this.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx") + "'");
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
        //    ((GUIShellMaster) this.Page.Master).SetToolBarAttributtes(toolbar, this.Toolbar);
        //}
    }

    #endregion


    public void SetPageTitle(string pageName)
    {
        this.PageTitleLabel.Text = pageName;
    }

    protected string GetAboutLinkUrl()
    {
        string urlPref = "http";
        if (Request.Url.AbsoluteUri.Contains("https"))
            urlPref = "https";
        AboutPageLink.OnClientClick = string.Format(@"window.open('" + urlPref + "://{0}/COEManager/Forms/Public/ContentArea/About.aspx','','scrollbars=no,height=600,width=1000,menubar=no,toolbar=no,location=no,status=no'); return false;", Request.Url.Host.ToString());

        return AboutPageLink.OnClientClick;
    }
}


