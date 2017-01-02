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

public partial class leftPane : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["Selected_Item"] = null;
    }

    protected void wb_NodeClicked(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
    {
        if (wb.SelectedNode.Tag != null)
        {
            Session["Selected_Item"] = wb.SelectedNode.Tag.ToString();
        }

    }

    protected void wb_NodeSelectionChanged(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
    {
        if (wb.SelectedNode.Tag != null)
        {
            Session["Selected_Item"] = wb.SelectedNode.Tag.ToString();
        }

    }
}
