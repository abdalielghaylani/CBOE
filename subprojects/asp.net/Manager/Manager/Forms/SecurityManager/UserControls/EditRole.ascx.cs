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

public partial class EditRoleUC : System.Web.UI.UserControl
{
    #region General Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["CurrentRole"] = null;
            Session["CurrentRoleList"] = null;
            ApplyAuthorizationRules();
         }
    }

    /// <summary>
    /// Get the privilege from the list and apply it to role.privileges
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void PrivilegeListChange(object sender, EventArgs e)
    {
        COERoleBO role = GetRole();
        //COEPrivilegeBOList privileges = role.Privileges;
        //COEPrivilegeBO privilege = privileges.GetPrivilegeByName(((CheckBoxList)(sender)).SelectedValue);
        //privilege.Enabled = ((CheckBoxList)(sender)).SelectedItem.Enabled;
        foreach (ListItem checkbox in ((CheckBoxList)(sender)).Items)
        {
            role.Privileges.GetPrivilegeByName(checkbox.Text).Enabled = checkbox.Selected;
        }
        Session["CurrentRole"] = role;
    }
   
    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events
        //capture the click of all commands in the grid.
        this.DetailsView1.ItemCommand += new DetailsViewCommandEventHandler(DetailsViewCommand_ButtonClicked);
       // this.CancelButton.ButtonClicked += new EventHandler<EventArgs>(CancelEdit());
        #endregion
        base.OnInit(e);
    }

    /// <summary>
    /// add client script, add style sheets etc
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        //creat the javascript to show the YUI panel for each button in the button bars for groups and group users.
        string key = "AddRolePanel";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
        {   
            string script = "<script type=\"text/javascript\">";
            script += "YAHOO.namespace(\"coemanager.security\");";
            script += "function init() {";

            //AddOracleRolePanel
            script += "YAHOO.coemanager.security.AddOracleRolePanel = new YAHOO.widget.Panel(\"addOracleRolePanel\",";
            script += "{width:\"300px\",fixedcenter:true,visible:false,modal:true,draggable:true,iframe:true,close:true,modal:false,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.AddOracleRolePanel.render();}";

     

            script += "YAHOO.util.Event.addListener(window, \"load\", init);";
            script += "function showAddOracleRolePanel(){";
            script += "YAHOO.coemanager.security.AddOracleRolePanel.show();}";
            
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(EditRoleUC), key, script);
        }
        base.OnPreRender(e);
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
            switch(e.CommandName)
            {
                case "Cancel":
                     CancelEdit();
                    break;
                default:
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

    #region Role Methods

    private void ApplyAuthorizationRules()
    {
        COERoleBO role = GetRole();

        if (COERoleBO.CanEditObject())
        {
            if (role.IsNew)
            {
                this.DetailsView1.DefaultMode = DetailsViewMode.Insert;
            }
            else
            {
                this.DetailsView1.DefaultMode = DetailsViewMode.Edit;
            }
        }
        else
        {
            this.DetailsView1.DefaultMode = DetailsViewMode.ReadOnly;
        }
    }

    /// <summary>
    /// Get the the COERoleBO for the selected role
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COERoleBODataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = GetRole();
    }

    protected void DetailsView_Load(object sender, EventArgs e)
    {
        RegularExpressionValidator regexp = (RegularExpressionValidator) ((DetailsView) sender).Controls[0].FindControl("RoleNameRegExpValidator");
        regexp.Text = regexp.ToolTip = regexp.ErrorMessage = Resources.Resource.InvalidCharsInRoleName_Error_Text;
        RegularExpressionValidator regexp2 = (RegularExpressionValidator)((DetailsView)sender).Controls[0].FindControl("RoleNameLengthRegExpValidator");
        regexp2.Text = regexp2.ToolTip = regexp2.ErrorMessage = Resources.Resource.RoleName_Length_Error_Text;
        if (this.DetailsView1.DefaultMode == DetailsViewMode.Insert)
        {
            //for insert mode if you don't expilicity apply the list you don't get one.
            COERoleBO role = GetRole();
            foreach (COEPrivilegeBO privilege in role.Privileges)
            {
                if(privilege.NonPrivilegeName.Length <= 0)
                {
                    ListItem checkBox = new ListItem(privilege.PrivilegeName);
                    checkBox.Selected = privilege.Enabled;
                    ((CheckBoxList) ((DetailsView) sender).Controls[0].FindControl("PrivilegeListControl")).Items.Add(checkBox);
                }
            }

            //we don't want css_login to be enabled, but it should be checked
            foreach (ListItem checkbox in ((CheckBoxList)((DetailsView)sender).Controls[0].FindControl("PrivilegeListControl")).Items)
            {
                    if (checkbox.Text == "CSS_LOGIN")
                    {
                        checkbox.Selected = true;
                        checkbox.Enabled = false;
                    }
                    if (checkbox.Text == "IS_FORMGROUP_ROLE")
                    {
                        checkbox.Enabled = false;
                    }
            }
        
            if (Request.QueryString["isDBMSRole"] == "true")
            {
                TextBox mytext = (TextBox)((DetailsView)sender).Controls[0].FindControl("RoleName");
                mytext.Text = Request.QueryString["DBMSRoleID"];
                HiddenField mytext2 = (HiddenField)((DetailsView)sender).Controls[0].FindControl("IsDBMSRole");
                mytext2.Value = "true";
            }
            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Insert";
            saveButton.CommandArgument = "Insert";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";
        }

        if (this.DetailsView1.DefaultMode == DetailsViewMode.Edit)
        {
            LinkButton myLink = (LinkButton)((DetailsView)sender).Controls[0].FindControl("SelectOracleRoleLinkButton");
            myLink.Visible = false;

            TextBox mytext = (TextBox)((DetailsView)sender).Controls[0].FindControl("RoleName");
            mytext.Enabled = false;

            COERoleBO role = GetRole();
            
            CheckBoxList checkboxList = ((CheckBoxList)((DetailsView)sender).Controls[0].FindControl("PrivilegeListControl"));

            for(int i = checkboxList.Items.Count - 1; i >= 0; i--)
            {
                if(string.IsNullOrEmpty(checkboxList.Items[i].Text))
                    checkboxList.Items.RemoveAt(i);
            }

            foreach (COEPrivilegeBO privilege in role.Privileges)
            {
                foreach(ListItem checkbox in checkboxList.Items)
                {
                    if (checkbox.Text == privilege.PrivilegeName)
                    {
                        checkbox.Selected = privilege.Enabled;
                        checkbox.Text = privilege.PrivilegeName;
                        if (checkbox.Text == "CSS_LOGIN")
                        {
                            checkbox.Enabled = false;
                        }

                        if (checkbox.Text == "IS_FORMGROUP_ROLE")
                        {
                            checkbox.Enabled = false;
                        }
                    }
                }
            }

            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName="Update";
            saveButton.CommandArgument = "Update";

            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";
        }
    }

    protected void COERoleBODataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
    {

        COERoleBO role = GetRole();

        //the datamapper wipes out the privlist so you need to store it and apply after the mapping
        COEPrivilegeBOList privList = role.Privileges;
        Csla.Data.DataMapper.Map(e.Values, role, true, "RoleName,isDBMSRole,Privileges");
        role.RoleName = (string)e.Values["RoleName"];
        role.COEIdentifier = Request.QueryString["appName"].ToString();

        role.Privileges = privList;
        e.RowsAffected = SaveRole(role);
        if (e.RowsAffected == 1)
        {
            Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }
    }

    /// <summary>
    /// Update the user with changes values and added/removed roles
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COERoleBODataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
        COERoleBO role = GetRole();
        //the privilege listgets nulled out for some reason so you need to preserve it and do that mapping mannually
        COEPrivilegeBOList privList = role.Privileges;
        Csla.Data.DataMapper.Map(e.Values, role, true, "isDBMSRole,Privileges");
        role.Privileges = privList;
        e.RowsAffected = SaveRole(role);
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
            role.RoleUsers = null;
            role.RoleRoles = null;
            //role.Privileges = null;
            Session["CurrentRole"] = role.Save();
            rowsAffected = 1;
        }
        catch (Csla.Validation.ValidationException)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            //message.AppendFormat("{0}<br/>", ex.Message);
            if (role.BrokenRulesCollection.Count == 1)
            {
                message.AppendFormat("* {0}", role.BrokenRulesCollection[0].Description);
                this.DisplayErrorMessage(message.ToString());
            }
            else
            {
                foreach (Csla.Validation.BrokenRule rule in role.BrokenRulesCollection)
                    message.AppendFormat("* {0}<br/>",rule.Description);
                this.DisplayErrorMessage(message.ToString());
            }
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
                    businessObject = COERoleBO.Get(roleName, true, false, false);
                else
                    businessObject = COERoleBO.New(Request.QueryString["AppName"]);

                Session["CurrentRole"] = (COERoleBO)businessObject;
            }
            catch (System.Security.SecurityException)
            {
                Response.Redirect("ManageRoles.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
            }
        }
       
        return (COERoleBO)businessObject;
    }

    //private COERoleBO RemoveEmptyPrivileges(COERoleBO cOERoleBO)
    //{
    //    int i = cOERoleBO.Privileges.Count;
    //    while(i > 0)
    //    {
    //        if(string.IsNullOrEmpty(cOERoleBO.Privileges[i - 1].PrivilegeName))
    //            cOERoleBO.Privileges.RemoveAt(i - 1);
    //        i--;
    //    }

    //    return cOERoleBO;
    //}
    #endregion

    #region DBMSUserList for add oracle users YUI panel

    /// <summary>
    /// databinding of csla DBMSUserList to the PersonListBox control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DBMSRoleList_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = RoleList.GetDBMSList();
    }

    protected void SelectOracleRoleButton_Click(object sender, EventArgs e)
    {
        RolesAddListBox.Visible = true;
        OracleRolesAddUpdatePanel.Update();
    }

    /// <summary>
    /// event handler when the item in dbms role list is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RoleAddListBox_IndexChange(object sender, EventArgs e)
    {
        COERoleBO role = GetRole();
        role.IsDBMSRole = true;
        Session["CurrentRole"] = (COERoleBO)role;
        string RID = RolesAddListBox.SelectedValue;
        TextBox mytext = (TextBox)DetailsView1.Controls[0].FindControl("RoleName");
        mytext.Text = RID;
        mytext.Enabled = false;
        DetailsView1.Controls[0].FindControl("OracleRolWarning").Visible = true;
        DetailsView1.Controls[0].FindControl("Panel1").Visible = false;
        
        this.EditRoleUpdatePanel.Update();
    }

    #region RoleListDataSource
    protected void RoleListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
            e.BusinessObject = RoleList.GetDBMSList();
    }
    #endregion
    #endregion

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel)((Forms_ContentArea_EditRole)this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
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
