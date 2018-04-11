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

public partial class Forms_UserControls_Header : System.Web.UI.UserControl, ICOEHeaderUC
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.HomeLink.Click += new EventHandler(HomeLink_Click);
        this.MainLink.Click += new EventHandler(MainLink_Click);
        this.AboutLink.Click += new EventHandler(AboutLink_Click);
    }

    void AboutLink_Click(object sender, EventArgs e)
    {
        Server.Transfer(this.ResolveUrl("~/Forms/Public/ContentArea/About.aspx"));
    }

    void MainLink_Click(object sender, EventArgs e)
    {
        Server.Transfer(this.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx"));
    }

    void HomeLink_Click(object sender, EventArgs e)
    {
        Response.Redirect("/COEManager/Forms/Public/ContentArea/Home.aspx");
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
        this.HelpLink.InnerText = Resources.Resource.Help_Label_Text;
        this.HelpLink.HRef = "/CBOEHelp/CBOEContextHelp/ChemBioViz Webhelp/Default.htm";
        this.LogOffButton.Text = Resources.Resource.LogOff_Label_Text;
        this.AboutLink.Text = Resources.Resource.About_Label_Text;
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

    public void DataBind(COELogo coeLogoObj, COEMenu coeMenuObj, COEMenu coeToolBarObj)
    {
        #region Logo

        //Set Logo information from COELogoObj
        this.LogoContainer.Attributes.Add("onclick", "window.location.href=" + "'" + coeLogoObj.LogoItem[0].URL + "'");
        this.LogoContainer.Attributes.Add("title", coeLogoObj.LogoItem[0].ToolTip);
        #endregion

        #region Menu
        //Set Menu info from COEMenuObj
        //Get root node.
        int parentLevel = -1;
        string filterExpresion = "ParentLevel = {0} AND ParentKey = '{1}'";

        DataRow[] rootLevelRow = coeMenuObj.MenuItem.Select(string.Format(filterExpresion, parentLevel, String.Empty));
        if(rootLevelRow.Length == 1)
        {
            Item rootLevelItem = new Item();
            rootLevelItem.Text = ((COEMenu.MenuItemRow) rootLevelRow[0]).TitleText;
            if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) rootLevelRow[0]).URL))
                rootLevelItem.TargetUrl = ((COEMenu.MenuItemRow) rootLevelRow[0]).URL;

            if(parentLevel != 0)
                parentLevel = 0;
            //First Level nodes
            foreach(DataRow firstLevelRow in coeMenuObj.MenuItem.Select(string.Format(filterExpresion, parentLevel, ((COEMenu.MenuItemRow) rootLevelRow[0]).Key)))
            {
                if(parentLevel != 1)
                    parentLevel = 1;
                Item firstLevelItem = new Item();
                firstLevelItem.Text = ((COEMenu.MenuItemRow) firstLevelRow).TitleText;
                if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) firstLevelRow).URL))
                    firstLevelItem.TargetUrl = ((COEMenu.MenuItemRow) firstLevelRow).URL;
                foreach(DataRow secondLevelRow in coeMenuObj.MenuItem.Select(string.Format(filterExpresion, parentLevel, ((COEMenu.MenuItemRow) firstLevelRow).Key)))
                {
                    //Second Level nodes
                    Item secondLevelItem = new Item();
                    if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) secondLevelRow).TitleText))
                        secondLevelItem.Text = ((COEMenu.MenuItemRow) secondLevelRow).TitleText;
                    if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) secondLevelRow).URL))
                        secondLevelItem.TargetUrl = ((COEMenu.MenuItemRow) secondLevelRow).URL;
                    firstLevelItem.Items.Add(secondLevelItem);
                }
                rootLevelItem.Items.Add(firstLevelItem);
            }
            this.GoToUltraWebMenu.Items.Add(rootLevelItem);
        }
        else
        {
            throw new Exception("Invalid information in coeMenuObj. Just one item can be root(in this kind of menu).");
        }
        #endregion

        //#region Toolbar

        //parentLevel = -1;
        //foreach(DataRow topLevelRow in coeToolBarObj.MenuItem.Select(string.Format(filterExpresion, parentLevel, String.Empty)))
        //{
        //    Item rootLevelItem = new Item();
        //    rootLevelItem.Text = ((COEMenu.MenuItemRow) topLevelRow).TitleText;
        //    if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) topLevelRow).URL))
        //        rootLevelItem.TargetUrl = ((COEMenu.MenuItemRow) topLevelRow).URL;
        //    //First Level nodes
        //    foreach(DataRow firstLevelRow in coeToolBarObj.MenuItem.Select(string.Format(filterExpresion, ++parentLevel, ((COEMenu.MenuItemRow) topLevelRow).Key)))
        //    {
        //        Item firstLevelItem = new Item();
        //        firstLevelItem.Text = ((COEMenu.MenuItemRow) firstLevelRow).TitleText;
        //        if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) firstLevelRow).URL))
        //            firstLevelItem.TargetUrl = ((COEMenu.MenuItemRow) firstLevelRow).URL;
        //        foreach(DataRow secondLevelRow in coeToolBarObj.MenuItem.Select(string.Format(filterExpresion, ++parentLevel, ((COEMenu.MenuItemRow) firstLevelRow).Key)))
        //        {
        //            //Second Level nodes
        //            Item secondLevelItem = new Item();
        //            secondLevelItem.Text = ((COEMenu.MenuItemRow) secondLevelRow).TitleText;
        //            if(!string.IsNullOrEmpty(((COEMenu.MenuItemRow) secondLevelRow).URL))
        //                secondLevelItem.TargetUrl = ((COEMenu.MenuItemRow) secondLevelRow).URL;
        //            secondLevelItem.Separator = true;
        //            firstLevelItem.Items.Add(secondLevelItem);
        //        }
        //        rootLevelItem.Items.Add(firstLevelItem);
        //    }
        //    this.UltraWebToolbarControl.Items.Add(rootLevelItem);
        //}

        //#endregion
    }

    #endregion


    public void SetPageTitle(string pageName)
    {
        this.PageTitleLabel.Text = pageName;
    }
}


