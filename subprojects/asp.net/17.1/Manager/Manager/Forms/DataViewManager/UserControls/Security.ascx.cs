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
using System.Reflection;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Text;

public partial class SecurityUC : System.Web.UI.UserControl
{
    #region Properties

    /// <summary>
    /// All selectable users
    /// </summary>
    private COEUserReadOnlyBOList AllUsers
    {
        get
        {
            return Session[Constants.AllUsers] != null ? (COEUserReadOnlyBOList)Session[Constants.AllUsers] : null;
        }
        set
        {
            Session[Constants.AllUsers] = value;
        }
    }

    /// <summary>
    /// Selected users for the current dataview.
    /// </summary>
    private COEUserReadOnlyBOList SelectedUsers
    {
        get
        {
            return Session[Constants.SelectedUsers] != null ? (COEUserReadOnlyBOList)Session[Constants.SelectedUsers] : null;
        }
        set
        {
            Session[Constants.SelectedUsers] = value;
        }
    }

    /// <summary>
    /// All selectable roles
    /// </summary>
    private COERoleReadOnlyBOList AllRoles
    {
        get
        {
            return Session[Constants.AllRoles] != null ? (COERoleReadOnlyBOList)Session[Constants.AllRoles] : null;
        }
        set
        {
            Session[Constants.AllRoles] = value;
        }
    }

    /// <summary>
    /// Selected roles for the currect dataview.
    /// </summary>
    private COERoleReadOnlyBOList SelectedRoles
    {
        get
        {
            return Session[Constants.SelectedRoles] != null ? (COERoleReadOnlyBOList)Session[Constants.SelectedRoles] : null;
        }
        set
        {
            Session[Constants.SelectedRoles] = value;
        }
    }

    #endregion

    #region Variables

    private enum DvVisibility
    {
        Public,
        Customized,
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsPostBack)
        {
            this.SetControlsAttributes();
            //CSBR-133820
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "AddUsers")
            {
                AddUserImageButton_ButtonClicked(AddUserImageButton, EventArgs.Empty);
            }
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "AddRoles")
            {
                AddRoleImageButton_ButtonClicked(AddRoleImageButton, EventArgs.Empty);
            }
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "RemoveUsers")
            {
                RemoveUserImageButton_ButtonClicked(RemoveUserImageButton, EventArgs.Empty);
            }
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "RemoveRoles")
            {
                RemoveRoleImageButton_ButtonClicked(RemoveRoleImageButton, EventArgs.Empty);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Buttons Events

        //CSBR-133820
        this.AddRoleImageButton.Click += new EventHandler(AddRoleImageButton_ButtonClicked);
        this.RemoveRoleImageButton.Click += new EventHandler(RemoveRoleImageButton_ButtonClicked);
        this.AddUserImageButton.Click += new EventHandler(AddUserImageButton_ButtonClicked);
        this.RemoveUserImageButton.Click += new EventHandler(RemoveUserImageButton_ButtonClicked);
        AllUsersList.Attributes.Add("ondblclick", this.Page.ClientScript.GetPostBackEventReference(AllUsersList, "AddUsers"));
        AllRolesList.Attributes.Add("ondblclick", this.Page.ClientScript.GetPostBackEventReference(AllRolesList, "AddRoles"));
        SelectedUsersList.Attributes.Add("ondblclick", this.Page.ClientScript.GetPostBackEventReference(SelectedUsersList, "RemoveUsers"));
        SelectedRolesList.Attributes.Add("ondblclick", this.Page.ClientScript.GetPostBackEventReference(SelectedRolesList, "RemoveRoles"));

        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void RemoveUserImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RemoveUsers();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void AddUserImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AddUsers();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void RemoveRoleImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.RemoveRoles();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void AddRoleImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AddRoles();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Add jscript code to disable/enable the Users/Roles table selection.
        StringBuilder jscriptText = new StringBuilder();
        string jscriptKey = "UsersRolesTableSelection";
        jscriptText.Append(@"<script language='javascript'>");
        jscriptText.Append(@"function DisableTables(status, usersTableId, rolesTableId)");
        jscriptText.Append(@"{");
        jscriptText.Append(@"var usersTable = document.getElementById(usersTableId);");
        jscriptText.Append(@"var rolesTable = document.getElementById(rolesTableId);");
        jscriptText.Append(@"if (usersTable != null && rolesTable != null) {");
        jscriptText.Append(@"if (status) { usersTable.style.visibility = 'hidden';  rolesTable.style.visibility = 'hidden'; ");
        jscriptText.Append(@"usersTable.style.display = 'none';  rolesTable.style.display = 'none'; }");
        jscriptText.Append(@"else { usersTable.style.visibility = 'visible'; rolesTable.style.visibility = 'visible';");
        jscriptText.Append(@"usersTable.style.display = 'inline'; rolesTable.style.display = 'inline';}");
        jscriptText.Append(@"}");
        jscriptText.Append(@"}");
        jscriptText.Append(@"</script>");

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(jscriptKey))
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), jscriptKey, jscriptText.ToString());
        // Add jscripts calls.
        this.DataViewUserRolesSelectionRadioList.Items.FindByValue(DvVisibility.Public.ToString()).Attributes.Add("OnClick", "jscript: DisableTables(true,'" + this.UsersSelectionTable.ClientID + "','" + this.RolesSelectionTable.ClientID + "');");
        this.DataViewUserRolesSelectionRadioList.Items.FindByValue(DvVisibility.Customized.ToString()).Attributes.Add("OnClick", "jscript: DisableTables(false,'" + this.UsersSelectionTable.ClientID + "','" + this.RolesSelectionTable.ClientID + "');");

        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.DataViewUserRolesSelectionRadioList.Items.Count == 0)
        {
            ListItem publicDV = new ListItem(Resources.Resource.PublicDV_Label_Text, DvVisibility.Public.ToString(), true);
            this.DataViewUserRolesSelectionRadioList.Items.Add(publicDV);

            ListItem customizedDV = new ListItem(Resources.Resource.CustomizedDV_Label_Text, DvVisibility.Customized.ToString(), true);
            this.DataViewUserRolesSelectionRadioList.Items.Add(customizedDV);
        }
        this.UsersHeaderCell.InnerText = Resources.Resource.Users_ColHeader_Caption;
        this.AvailableUsersHeaderCell.InnerText = Resources.Resource.AvailableUsers_ColHeader_Caption;
        this.CurrentUsersHeaderCell.InnerText = Resources.Resource.CurrentUsers_ColHeader_Caption;
        this.UserActionsHeaderCell.InnerText = Resources.Resource.Action_ColHeader_Caption;

        this.RolesHeaderCell.InnerText = Resources.Resource.Roles_ColHeader_Caption;
        this.AvailableRolesHeaderCell.InnerText = Resources.Resource.AvailableRoles_ColHeader_Caption;
        this.CurrentRolesHeaderCell.InnerText = Resources.Resource.CurrentRoles_ColHeader_Caption;
        this.RoleActionsHeaderCell.InnerText = Resources.Resource.Action_ColHeader_Caption;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void RemoveUsers()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool haveToBind = false;
        foreach (ListItem checkbox in this.SelectedUsersList.Items)
        {
            if (checkbox.Selected)
            {
                COEUserReadOnlyBO user = this.SelectedUsers.GetUserByID(Convert.ToInt32(checkbox.Value));
                if (this.AllUsers == null)
                    this.AllUsers = new COEUserReadOnlyBOList();
                if (!this.AllUsers.Contains(user))
                    this.AllUsers.AddUser(user);
                if (this.SelectedUsers.Contains(user))
                    this.SelectedUsers.RemoveUser(user);
                haveToBind = true;
            }
        }
        if (haveToBind)
            this.BindUsers(this.AllUsers, this.SelectedUsers);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void AddUsers()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool haveToBind = false;
        foreach (ListItem checkbox in this.AllUsersList.Items)
        {
            if (checkbox.Selected)
            {
                COEUserReadOnlyBO user = this.AllUsers.GetUserByID(Convert.ToInt32(checkbox.Value));
                if (this.AllUsers.Contains(user))
                    this.AllUsers.RemoveUser(user);
                if (this.SelectedUsers == null)
                    this.SelectedUsers = new COEUserReadOnlyBOList();
                if (!this.SelectedUsers.Contains(user))
                    this.SelectedUsers.AddUser(user);
                haveToBind = true;
            }
        }
        if (haveToBind)
            this.BindUsers(this.AllUsers, this.SelectedUsers);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void RemoveRoles()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool haveToBind = false;
        foreach (ListItem checkbox in this.SelectedRolesList.Items)
        {
            if (checkbox.Selected)
            {
                COERoleReadOnlyBO role = this.SelectedRoles.GetRoleByID((Convert.ToInt32(checkbox.Value)));
                if (this.AllRoles == null)
                    this.AllRoles = new COERoleReadOnlyBOList();
                if (!this.AllRoles.Contains(role))
                    this.AllRoles.AddRole(role);
                if (this.SelectedRoles.Contains(role))
                    this.SelectedRoles.RemoveRole(role);
                haveToBind = true;
            }
        }
        if (haveToBind)
            this.BindRoles(this.AllRoles, this.SelectedRoles);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void AddRoles()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool haveToBind = false;
        foreach (ListItem checkbox in this.AllRolesList.Items)
        {
            if (checkbox.Selected)
            {
                COERoleReadOnlyBO role = this.AllRoles.GetRoleByID((Convert.ToInt32(checkbox.Value)));
                if (this.AllRoles.Contains(role))
                    this.AllRoles.RemoveRole(role);
                if (this.SelectedRoles == null)
                    this.SelectedRoles = new COERoleReadOnlyBOList();
                if (!this.SelectedRoles.Contains(role))
                    this.SelectedRoles.AddRole(role);
                haveToBind = true;
            }
        }
        if (haveToBind)
            this.BindRoles(this.AllRoles, this.SelectedRoles);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public void DataBind(COEUserReadOnlyBOList allUsersList, COEUserReadOnlyBOList selectedUsersList,
                            COERoleReadOnlyBOList allRolesList, COERoleReadOnlyBOList selectedRolesList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.SetControlsAttributes();
        //Save datasources in variables in the current class.
        this.AllUsers = allUsersList;
        this.SelectedUsers = selectedUsersList;
        this.AllRoles = allRolesList;
        this.SelectedRoles = selectedRolesList;
        if (Page.IsPostBack)
        {
            this.AllRolesList.Items.Clear();
            this.SelectedRolesList.Items.Clear();
            this.AllUsersList.Items.Clear();
            this.SelectedUsersList.Items.Clear();
        }
        this.BindUsers(allUsersList, selectedUsersList);
        this.BindRoles(allRolesList, selectedRolesList);
        //this.SetTablesToDisplay();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetTablesToDisplay()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewUserRolesSelectionRadioList.ClearSelection();
        if (this.SelectedRoles != null && this.SelectedUsers != null)
        {
            if (this.SelectedRoles.Count == 0 && this.SelectedUsers.Count == 0)
            {
                this.DataViewUserRolesSelectionRadioList.SelectedValue = DvVisibility.Public.ToString();
                this.UsersSelectionTable.Style.Value = this.RolesSelectionTable.Style.Value = "visibility:hidden;display:none;";
            }
            else
            {
                this.DataViewUserRolesSelectionRadioList.SelectedValue = DvVisibility.Customized.ToString();
                this.UsersSelectionTable.Style.Value = this.RolesSelectionTable.Style.Value = "visibility:visible;display:inline;";
            }
        }
        else
        {
            if (this.SelectedRoles != null || this.SelectedUsers != null)
            {
                this.DataViewUserRolesSelectionRadioList.SelectedValue = DvVisibility.Customized.ToString();
                this.UsersSelectionTable.Style.Value = this.RolesSelectionTable.Style.Value = "visibility:visible;display:inline;";
            }
            else
            {
                this.DataViewUserRolesSelectionRadioList.SelectedValue = DvVisibility.Public.ToString();
                this.UsersSelectionTable.Style.Value = this.RolesSelectionTable.Style.Value = "visibility:hidden;display:none;";
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void BindUsers(COEUserReadOnlyBOList allUsersList, COEUserReadOnlyBOList selectedUsersList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Populate CheckBoxLists
        this.AllUsersList.DataSource = allUsersList;
        this.AllUsersList.DataBind();
        this.SelectedUsersList.DataSource = selectedUsersList;
        this.SelectedUsersList.DataBind();
        this.SetTablesToDisplay();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void BindRoles(COERoleReadOnlyBOList allRolesList, COERoleReadOnlyBOList selectedRolesList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AllRolesList.DataSource = allRolesList;
        this.AllRolesList.DataBind();
        this.SelectedRolesList.DataSource = selectedRolesList;
        this.SelectedRolesList.DataBind();
        this.SetTablesToDisplay();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    public COEAccessRightsBO UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEAccessRightsBO retVal = null;
        COEUserReadOnlyBOList usersParam = null;
        COERoleReadOnlyBOList rolesParam = null;
        if (this.DataViewUserRolesSelectionRadioList.SelectedValue == DvVisibility.Customized.ToString())
        {
            if (this.SelectedUsers != null)
                usersParam = this.SelectedUsers.Count > 0 ? this.SelectedUsers : null;
            if (this.SelectedRoles != null)
                rolesParam = this.SelectedRoles.Count > 0 ? this.SelectedRoles : null;
            if (usersParam != null || rolesParam != null)
                retVal = new COEAccessRightsBO(usersParam, rolesParam);
        }
        //Remove temp Session vars
        this.RemoveSessionVars();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    private void RemoveSessionVars()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Session[Constants.AllUsers] != null)
            Session.Remove(Constants.AllUsers);
        if (Session[Constants.SelectedUsers] != null)
            Session.Remove(Constants.SelectedUsers);
        if (Session[Constants.AllRoles] != null)
            Session.Remove(Constants.AllRoles);
        if (Session[Constants.SelectedRoles] != null)
            Session.Remove(Constants.SelectedRoles);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
