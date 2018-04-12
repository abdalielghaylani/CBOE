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
//
using Util;
using System.IO;
using System.Xml;
using System.Xml.XPath;
public partial class SCManagement : System.Web.UI.Page
{

    ShoppingSessionDAO dao = new ShoppingSessionDAO();

    protected void Page_Load(object sender, EventArgs e)
    {
        int ReturnVal;
        try
        {

            ReturnVal = dao.CheckShoppingSession2(Request.QueryString["userid"].ToString(), Request.QueryString["hashcode"].ToString(), Request.QueryString["sessionid"].ToString(), Request.QueryString["cartid"].ToString(), Request.QueryString["status"].ToString());        

        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }

    }
}
