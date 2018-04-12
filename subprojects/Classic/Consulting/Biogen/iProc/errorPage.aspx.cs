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

public partial class errorPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string str = "1";

        if (this.Request.QueryString["msgid"] != null)
        {
            str = this.Request.QueryString["msgid"].ToString();
        }
       
        if (this.FindControl("msg" + str) != null)
        {
            this.FindControl("msg" + str).Visible = true;
        }

    }
}
