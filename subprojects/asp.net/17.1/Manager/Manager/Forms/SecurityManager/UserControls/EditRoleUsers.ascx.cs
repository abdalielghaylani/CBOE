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
using System.Reflection;
using Csla.Web;
using Csla.Properties;

public partial class EditRoleUsersUC : System.Web.UI.UserControl
{
    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            Session["CurrentRole"] = null;
        }
    }

    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events

        //capture the click of all commands in the grid.
        this.DetailsView1.ItemCommand += new DetailsViewCommandEventHandler(DetailsViewCommand_ButtonClicked);
      
        #endregion
        base.OnInit(e);
    }

    protected void DetailsView_Load(object sender, EventArgs e)
    {
        if (this.DetailsView1.DefaultMode == DetailsViewMode.Insert)
        {

            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Insert";
            saveButton.CommandArgument = "Insert";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";


        }

        if (this.DetailsView1.DefaultMode == DetailsViewMode.Edit)
        {

            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Update";
            saveButton.CommandArgument = "Update";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";

        }
    }

    /// <summary>
    /// Here we intercept command events of coming from the detail view control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DetailsViewCommand_ButtonClicked(object sender, DetailsViewCommandEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        try
        {
            switch (e.CommandName)
            {

                case "Cancel":
                    CancelEdit();
                    break;

                default:

                    switch (((Button)e.CommandSource).ID)
                    {
                        case "MoveFromLeftListToRightList":
                            ShuttleListsLeftToRight(sender);
                            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
                            saveButton.Enabled = true;
                            break;

                        case "MoveFromRightListToLeftList":
                            ShuttleListsRightToLeft(sender);
                            Forms_Public_UserControls_ImageButton saveButton2 = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
                            saveButton2.Enabled = true;
                            break;
                    }
                    break;
            }
        }
        catch
        {

        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }



    /// <summary>
    /// capture the cancel button click and go back to the users list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void CancelEdit()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }



    #endregion

    #region User List Methods

    /// <summary>
    /// Move list items from Right side to left
    /// </summary>
    protected void ShuttleListsRightToLeft(object sender)
    {
        int itemCountRight = ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Count;
        ListBox workingListRight = (ListBox)((DetailsView)sender).Controls[0].FindControl("RightList");
        List<ListItem> finalRightList = new List<ListItem>();
        for (int i = 0; i < itemCountRight; i++)
        {
            if (workingListRight.Items[i].Selected)
            {
                finalRightList.Add(workingListRight.Items[i]);

            }
        }

        foreach (ListItem item in finalRightList)
        {
            ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Add(item);
            ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Remove(item);
        }
        //update session variable with final outcome
        Session["RoleUsersGranted"] = ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items;


    }


    /// <summary>
    /// Move list items from left side to right
    /// </summary>
    protected void ShuttleListsLeftToRight(object sender)
    {
        int itemCountLeft = ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Count;
        ListBox workingListLeft = (ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList");
        List<ListItem> finalList = new List<ListItem>();
        for (int i = 0; i < itemCountLeft; i++)
        {
            if (workingListLeft.Items[i].Selected)
            {
                finalList.Add(workingListLeft.Items[i]);

            }
        }

        foreach (ListItem item in finalList)
        {
            ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Add(item);
            ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Remove(item);
        }
        //update session variable with final outcome
        Session["RoleUsersGranted"] = ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items;

    }


    /// <summary>
    /// Get the the COERolesBOList for the roles picklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COEUsersBOListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = GetRoleAvailableUsers();
    }

    #endregion

    #region role methods
    /// <summary>
    /// Get the the COERoleBO for the selected role
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COERoleBODataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = GetRole();
    }

    private COEUserBOList GetRoleAvailableUsers()
    {
        object businessObject = Session["RoleUsersNotGranted"];
        if (businessObject == null || !(businessObject is COERoleBOList))
        {
            COERoleBO role = GetRole();
            //Coverity Fixes: CBOE-313 : CID-11782
            if (role != null)
                businessObject = COEUserBOList.GetRoleAvailableUsers(role);
        }
        return (COEUserBOList)businessObject;

    }

    protected List<string> ConvertListItemCollectionToList(ListItemCollection listCollection)
    {
        List<string> myList = new List<string>();
        try
        {
            foreach (ListItem item in listCollection)
            {

                myList.Add(item.Text);
            }
        }
        catch (System.Exception)
        {
        }

        return myList;
    }
    /// <summary>
    /// Update the user with changes values and added/removed roles
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COERoleBODataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
        COERoleBO role = GetRole();
        //Coverity Fixes: CBOE-313 : CID-11783
        if (role != null)
        {
            //roleUsers is coming from alist taht will not be picked up by csla so it need to be mapped manually
            Csla.Data.DataMapper.Map(e.Values, role);
            role.RoleUsers = COEUserBOList.NewList(ConvertListItemCollectionToList((ListItemCollection)Session["RoleUsersGranted"]));
            e.RowsAffected = SaveRole(role);
        }
        if (e.RowsAffected == 1)
        {
            Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }
    }

    private int SaveRole(COERoleBO role)
    {
        int rowsAffected;
        try
        {
            //role.RoleUsers = null;
            role.RoleRoles = null;
            role.Privileges = null;
            Session["CurrentRole"] = role.Save();
            rowsAffected = 1;
        }
        catch (Csla.Validation.ValidationException ex)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            message.AppendFormat("{0}<br/>", ex.Message);
            if (role.BrokenRulesCollection.Count == 1)
                message.AppendFormat("* {0}: {1}",
                  role.BrokenRulesCollection[0].Property,
                  role.BrokenRulesCollection[0].Description);
            else
                foreach (Csla.Validation.BrokenRule rule in role.BrokenRulesCollection)
                    message.AppendFormat("* {0}: {1}<br/>", rule.Property, rule.Description);
            this.DisplayErrorMessage(message.ToString());
            rowsAffected = 0;
        }
        catch (Csla.DataPortalException ex)
        {
            this.DisplayErrorMessage(ex.BusinessException.Message);
            rowsAffected = 0;
        }
        catch (Exception ex)
        {
            this.DisplayErrorMessage(ex.Message);
            rowsAffected = 0;
        }
        return rowsAffected;
    }


    private COERoleBO GetRole()
    {
        object businessObject = Session["CurrentRole"];
        if (businessObject == null ||
          !(businessObject is COERoleBO))
        {
            try
            {
                string roleName = Request.QueryString["RoleName"];
                if (!string.IsNullOrEmpty(roleName))
                {
                    //this gets the COERole with out the privileges or roleRoles. It only gets roleUsers
                    businessObject = COERoleBO.Get(roleName, false, false, true);
                }
                
                Session["CurrentRole"] = businessObject;
            }
            catch (System.Security.SecurityException)
            {
                Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
            }
        }
        return (COERoleBO)businessObject;
    }


    #endregion

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel)((Forms_ContentArea_EditRoleUsers)this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
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
