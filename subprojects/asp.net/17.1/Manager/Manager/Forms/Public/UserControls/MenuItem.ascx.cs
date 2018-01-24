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
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;

public partial class Forms_Public_UserControls_MenuItem : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Methods

    internal void DataBind(string title, string description, string URL, string tooltip, string imageURL)
    {
        this.ItemTitleLink.Text = title;
        this.ItemDescriptionLink.Text = description;
        this.ItemTitleLink.NavigateUrl = this.MenuItemImageButton.PostBackUrl = this.ItemDescriptionLink.NavigateUrl = URL;
        this.ItemDescriptionLink.ToolTip = this.MenuItemImageButton.ToolTip = this.ItemDescriptionLink.ToolTip = tooltip;
        this.MenuItemImageButton.ImageUrl = imageURL;
    }
    #endregion
}
