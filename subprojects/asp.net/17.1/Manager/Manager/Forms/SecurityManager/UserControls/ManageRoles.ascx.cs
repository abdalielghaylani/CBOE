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

public partial class ManageRoles : System.Web.UI.UserControl
{
    #region General Event handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["CurrentRoleList"] = null;
            Session["CurrentRole"] = null;
            
           
        }
        else
        {
            string eventTarget = (this.Request["__EVENTTARGET"] == null) ? string.Empty : this.Request["__EVENTTARGET"];
            //string eventArgument = (this.Request["__EVENTARGUMENT"] == null) ? string.Empty : this.Request["__EVENTARGUMENT"];
            switch (eventTarget)
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

    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events
        #endregion
        base.OnInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        this.RoleListControl.Attributes.Add("onclick", "handleWisely('click','ChangeButtonState')");
        this.RoleListControl.Attributes.Add("ondblclick", "handleWisely('dblclick','EditButtonAction')");

        //Coverity Fixes: CBOE-313
        base.OnPreRender(e);
    }

 

    /// <summary>
    /// Deletes the selected user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DeleteRole()
    {

        DeleteThisRole(RoleListControl.SelectedValue);
        //Session["CurrentRoleList"] = RoleList.GetListByApplication(ApplicationListControl.SelectedValue);
        //ManageRoleUpdatePanel.Update();

    }

   
    
   

   
    #endregion

    #region Application list events and methods

    /// <summary>
    /// get list for specifiction application
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelectApplication(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string applicationName = Request.QueryString["appName"].ToString();
            GetRoleList(applicationName);
            ApplicationListControl.SelectedValue = applicationName;
            DisableManageRoleEditAndDeleteButtons();
        }
        else
        {
            GetRoleList(ApplicationListControl.SelectedValue);
            if (ApplicationListControl.SelectedValue == "All CS Applications")
            {
                DisableManageRoleButtons();
            }
            else
            {
                DisableManageRoleEditAndDeleteButtons();
            }
        }
        RoleListControl.DataBind();

    }


    /// <summary>
    /// databind applicationlist object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ApplicationNamesDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        try
        {

            e.BusinessObject = GetAppList();
            if (!IsPostBack)
            {
                ApplicationListControl.SelectedValue = Request.QueryString["appName"].ToString();
                GetRoleList(Request.QueryString["appName"].ToString());
            }
        }
        catch (Exception) { }
    }

    /// <summary>
    /// get current applist
    /// </summary>
    /// <returns></returns>
    private ApplicationList GetAppList()
    {
        object businessObject = Session["ApplicationNameList"];
        try
        {
            if (businessObject == null || !(businessObject is ApplicationList))
            {
                businessObject = ApplicationList.GetList(false);
                Session["ApplicationNameList"] = businessObject;
            }
        }
        catch (Exception)
        {

            throw;
        }
        return (ApplicationList)businessObject;
    }

    #endregion

    #region Role methods


    protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        try
        {

            e.BusinessObject = (RoleList)Session["CurrentRoleList"];

        }
        catch (Exception) { }
    }

    private void  GetRoleList(string applicationName)
    {
        Dictionary<string, RoleList> currentRolesLists = null;

        try
        {
            currentRolesLists = (Dictionary<string, RoleList>)Session["ApplicationNameList"];
        }
        catch
        {
            currentRolesLists = new Dictionary<string, RoleList>(); ;
        }

        if (currentRolesLists.Count == 0)
        {
            RoleList businessObject = null;
            if (applicationName == "All CS Applications")
            {
                businessObject = RoleList.GetList();
            }
            else
            {
                businessObject = RoleList.GetListByApplication(applicationName);
            }

            currentRolesLists.Add(applicationName, businessObject);
            Session["ApplicationNameList"] = currentRolesLists;
            Session["CurrentRoleList"] = businessObject;
           
        }
        else
        {
            RoleList businessObject = null;
            currentRolesLists.TryGetValue(applicationName, out businessObject);
            if (businessObject == null || !(businessObject is RoleList))
            {

                businessObject = RoleList.GetListByApplication(applicationName);

                currentRolesLists.Add(applicationName, (RoleList)businessObject);
            }
            Session["ApplicationNameList"] = currentRolesLists;
            Session["CurrentRoleList"] = businessObject;
        }
       
    }



    protected void DeleteThisRole(string roleName)
    {
        try
        {
            COERoleBO role = COERoleBO.Get(roleName);
            role.Delete();
            Session["ApplicationNameList"] = null;
            Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);

        }
        catch (Csla.DataPortalException ex)
        {
            this.DisplayErrorMessage(ex.BusinessException.Message);
        }
        catch (Exception ex)
        {
            this.DisplayErrorMessage(ex.Message);
        }
    }

    #endregion

    #region role buttons

    protected void ChangeButtonState(object sender, EventArgs e)
    {
        EnableManageRoleButtons();
    }

    protected void ChangeButtonState()
    {
        EnableManageRoleButtons();
    }



    protected void AddButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditRole.aspx" + "?appName=" + ApplicationListControl.SelectedValue, false);

    }

    protected void DeleteButtonAction(object sender, EventArgs e)
    {
        DeleteRole();
    }

    protected void EditButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditRole.aspx?RoleName=" + RoleListControl.SelectedValue + "&appName=" + ApplicationListControl.SelectedValue, false);
    }

    protected void EditButtonAction()
    {
        Response.Redirect("../ContentArea/EditRole.aspx?RoleName=" + RoleListControl.SelectedValue + "&appName=" + ApplicationListControl.SelectedValue, false);
    }
    protected void EditUsersButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditRoleUsers.aspx?RoleName=" + RoleListControl.SelectedValue + "&appName=" + ApplicationListControl.SelectedValue, false);
    }
    protected void EditRolesButtonAction(object sender, EventArgs e)
    {
        Response.Redirect("../ContentArea/EditRoleRoles.aspx?RoleName=" + RoleListControl.SelectedValue + "&appName=" + ApplicationListControl.SelectedValue, false);
    }

   

    private void EnableManageRoleButtons()
    {

        this.AddButton.Enabled = true;
        this.EditButton.Enabled = true;
        this.EditUsersButton.Enabled = true;
        this.EditRolesButton.Enabled = true;
        this.DeleteButton.Enabled = true;
       

       
    }
    private void DisableManageRoleButtons()
    {
        this.AddButton.Enabled = false;
        this.EditButton.Enabled = false;
        this.EditUsersButton.Enabled = false;
        this.EditRolesButton.Enabled = false;
        this.DeleteButton.Enabled = false;

    }

    private void DisableManageRoleEditAndDeleteButtons()
    {

        this.AddButton.Enabled = true;
        this.EditButton.Enabled = false;
        this.EditUsersButton.Enabled = false;
        this.EditRolesButton.Enabled = false;
        this.DeleteButton.Enabled = false;
    }

    #endregion

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel)((Forms_ContentArea_ManageRoles)this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
        if (string.IsNullOrEmpty(message))
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = false;
        else
        {
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Text = message;
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = true;
        }
        errorAreaUpatePanel.Update();
    }
}
