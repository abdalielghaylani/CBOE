using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;


public partial class Forms_UserControls_Footer : System.Web.UI.UserControl
{
    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!Page.IsPostBack)
        //{
        //    this.SetControlsAttributes();
        //}
    }

    #endregion

    //#region Methods

    ///// <summary>
    ///// Method to set all the controls attributtes as Text, tooltip, etc...
    ///// </summary>
    //private void SetControlsAttributes()
    //{
    //    this.HomeHyperLink.Text = Resources.Resource.Home_Label_Text;
    //    this.HomeHyperLink.NavigateUrl = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Home.aspx";

    //    this.MainHyperLink.Text = Resources.Resource.Main_HyperLink_Text;
    //    this.MainHyperLink.NavigateUrl = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Home.aspx";

    //    this.HelpHyperLink.Text = Resources.Resource.Help_Label_Text;
    //    this.HelpHyperLink.NavigateUrl = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "Help.aspx";

    //    this.AboutHyperLink.Text = Resources.Resource.About_Label_Text;
    //    this.AboutHyperLink.NavigateUrl = this.Page.ResolveUrl(Constants.GetPublicContentAreaFolder()) + "About.aspx";

    //    this.Separator1Label.Text = this.Separator2Label.Text = this.Separator3Label.Text = Resources.Resource.FooterSeparator_Label_Text;

    //    this.RightsLabel.Text = string.Format(Resources.Resource.SiteRights_Label_Text, DateTime.Now.Year);

    //}

    //#endregion


}
