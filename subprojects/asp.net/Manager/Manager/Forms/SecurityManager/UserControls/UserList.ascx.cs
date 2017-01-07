using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using Csla.Web;
using Csla.Properties;

public partial class UserListUC : System.Web.UI.UserControl
{
    #region Properties
    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["UserList"] = null;
            //ApplyAuthorizationRules();
            
        }
    }

    /// <summary>
    /// Redirects user to EditUser.aspx page is called wiht an empty userID in the url. This singles a new COEUserBO should be created
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelectOracleUserButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditUser.aspx?isDBMSUser=true&userID=" + UserListBoxControl.SelectedValue + "&appName=" + Request.QueryString["appName"].ToString());
    }

    protected void CloseButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditUser.aspx?appName=" + Request.QueryString["appName"].ToString());
    }

    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events

      
        #endregion
        base.OnInit(e);
    }

    /// <summary>
    /// capture the cancel button click and go back to the users list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void CancelEdit()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
  
    #region UserListDataSource

    protected void UserListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        if (Request.QueryString["UserType"].ToString().ToUpper() == "DBMS")
        {
            e.BusinessObject = UserList.GetDBMSList();
        }
        else
        {
            e.BusinessObject = UserList.GetCOEList(Request.QueryString["AppName"]);
        }
    }
    #endregion

}
