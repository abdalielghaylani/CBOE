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
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Reflection;
using Infragistics.WebUI.UltraWebToolbar;

public partial class ManageUsers : System.Web.UI.UserControl
{
    #region General Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            Session["CurrentUserList"] = null;
        }
        else
        {
            string eventTarget = (this.Request["__EVENTTARGET"] == null) ? string.Empty : this.Request["__EVENTTARGET"];
            //string eventArgument = (this.Request["__EVENTARGUMENT"] == null) ? string.Empty : this.Request["__EVENTARGUMENT"];
            switch(eventTarget)
            {
                case "ChangeButtonState":
                    ChangeButtonState();
                    break;
                case "EditButtonAction":
                    EditButtonAction();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    protected void ChangeButtonState()
    {
        EnableUserListButtons();
    }

    protected void ChangeButtonState(object sender, EventArgs e)
    {
        EnableUserListButtons();
    }

    protected override void OnPreRender(EventArgs e)
    {
        this.UserListControl.Attributes.Add("onclick", "handleWisely('click','ChangeButtonState')");
        this.UserListControl.Attributes.Add("ondblclick", "handleWisely('dblclick','EditButtonAction')");
        base.OnPreRender(e);
    }

    protected void AddButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditUser.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
    }

    protected void DeleteButtonAction(object sender, EventArgs e)
    {
        DeleteUserButton_Click();
    }

    protected void EditButtonAction()
    {
        Response.Redirect("../ContentArea/EditUser.aspx?UserID=" + UserListControl.SelectedValue + "&appName=" + Request.QueryString["appName"].ToString(), false);
    }

    protected void EditButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditUser.aspx?UserID=" + UserListControl.SelectedValue + "&appName=" + Request.QueryString["appName"].ToString(), false);
    }

    private void EnableUserListButtons()
    {
        this.AddButton.Enabled = true;
        this.EditButton.Enabled = true;
        this.DeleteButton.ButtonCssClass = "btn-disable";
    }

    private void DisableUserEditAndDeleteButtons()
    {
        this.AddButton.Enabled = true;
        this.EditButton.Enabled = false;
        this.DeleteButton.ButtonCssClass = "ImageButton";
    }


    /// <summary>
    /// Deletes the selected user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DeleteUserButton_Click()
    {
        DeleteThisUser(UserListControl.SelectedValue.ToString());
    }
    #endregion

    #region Application list events and methods
    /// <summary>
    /// Get list specific to an applicaiton
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelectApplication(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            string applicationName = Request.QueryString["appName"].ToString();
            if(applicationName == string.Empty) { applicationName = "All CS Applications"; }
            GetUserList(applicationName);
            ApplicationListControl.SelectedValue = applicationName;

        }
        else
        {
            GetUserList(ApplicationListControl.SelectedValue);

        }

        DisableUserEditAndDeleteButtons();

        UserListControl.DataBind();

    }


    /// <summary>
    /// Bind application lsit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ApplicationNamesDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        try
        {
            e.BusinessObject = GetAppList();
            if(!IsPostBack)
            {
                ApplicationListControl.SelectedValue = Request.QueryString["appName"].ToString();
                GetUserList(Request.QueryString["appName"].ToString());
            }
        }
        catch(Exception) { }
    }

    /// <summary>
    /// Get the app list.
    /// </summary>
    /// <returns></returns>
    private ApplicationList GetAppList()
    {
        object businessObject = Session["ApplicationNameList"];
        try
        {
            if(businessObject == null || !(businessObject is ApplicationList))
            {
                businessObject = ApplicationList.GetList(true);
                Session["ApplicationNameList"] = businessObject;
            }
        }
        catch(Exception)
        {
            throw;
        }
        return (ApplicationList) businessObject;
    }

    #endregion

    #region User list events and methods
    private void GetUserList(string ApplicationName)
    {
        Dictionary<string, UserList> currentUsersLists = null;

        try
        {
            currentUsersLists = (Dictionary<string, UserList>) Session["ApplicationNameList"];
        }
        catch
        {
            currentUsersLists = new Dictionary<string, UserList>();
        }

        try
        {
            if(currentUsersLists.Count == 0)
            {
                UserList businessObject = null;
                if(ApplicationName == "All CS Applications")
                {
                    businessObject = UserList.GetCOEList();
                }
                else
                {
                    businessObject = UserList.GetCOEList(ApplicationName);
                }

                currentUsersLists.Add(ApplicationName, businessObject);
                Session["ApplicationNameList"] = currentUsersLists;
                Session["CurrentUserList"] = businessObject;
            }
            else
            {
                UserList businessObject = null;
                currentUsersLists.TryGetValue(ApplicationName, out businessObject);
                if(businessObject == null || !(businessObject is UserList))
                {
                    if(ApplicationName == "All CS Applications")
                    {
                        businessObject = UserList.GetCOEList();
                    }
                    else
                    {
                        businessObject = UserList.GetCOEList(ApplicationName);
                    }
                    currentUsersLists.Add(ApplicationName, (UserList) businessObject);
                }
                Session["ApplicationNameList"] = currentUsersLists;
                Session["CurrentUserList"] = businessObject;
            }
        }
        catch(Csla.DataPortalException ex)
        {
            this.DisplayErrorMessage(ex.BusinessException.Message);
        }
        catch(Exception ex)
        {
            this.DisplayErrorMessage(ex.Message);
        }
    }

    protected void UserListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        try
        {
            e.BusinessObject = (UserList) Session["CurrentUserList"];
        }
        catch(Exception) { }
    }

    protected void DeleteThisUser(string UserName)
    {
        try
        {
            COEUserBO.Delete(UserName);
            Session["ApplicationNameList"] = null;
            Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }
        catch(Csla.DataPortalException ex)
        {
            this.DisplayErrorMessage(ex.BusinessException.Message);
        }
        catch(Exception ex)
        {
            this.DisplayErrorMessage(ex.Message);
        }
    }

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel) ((Forms_ContentArea_ManageUsers) this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
        if(string.IsNullOrEmpty(message))
            ((Forms_Public_UserControls_ErrorArea) errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = false;
        else
        {
            ((Forms_Public_UserControls_ErrorArea) errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Text = message;
            ((Forms_Public_UserControls_ErrorArea) errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = true;
        }
        errorAreaUpatePanel.Update();
    }
    #endregion

}
