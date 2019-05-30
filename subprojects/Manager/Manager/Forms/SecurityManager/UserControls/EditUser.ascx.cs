using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
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
using CambridgeSoft.COE.Framework.Controls;


public partial class EditUserUC : System.Web.UI.UserControl
{
    #region Member Variables
    private List<string> userAvailableRoles = null;
    private List<string> userCurrentRoles = null;
    private const string DEFAULT_ROLE = "CSS_USER";
    private string ErrorMsg = string.Empty;
    int siteIndex = 0;
    int SupervisorIndex = 0;
    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Session["CurrentUser"] = null;
            Session["CurrentUserList"] = null;
            ApplyAuthorizationRules();
        }
        this.SetControlsAttributes();
        this.OKActivationButton.Click += new EventHandler(OKActivationButton_Click);
        this.CancelActivationButton.Click += new EventHandler(CancelActivationButton_Click);

    }

    void CancelActivationButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(this.Request.Url.ToString());
    }

    void OKActivationButton_Click(object sender, EventArgs e)
    {
        COEUserBO user = GetUser();
        int recsAffected = SaveUser("ACTIVATE", user, user.IsLDAP);
        if (recsAffected == 1)
        {
            Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }
    }

    protected void DetailsView_Load(object sender, EventArgs e)
    {

        COEUserBO user = GetUser();
        if (this.DetailsView1.DefaultMode == DetailsViewMode.Insert)
        {
            foreach (COERoleBO role in user.AvailableRoles)
            {
                if (role.RoleName != DEFAULT_ROLE)
                {
                    ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Add(new ListItem(role.RoleName));
                }
            }

            //Add DEFAULT_ROLE to Right List. Every User needs this:
            ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Add(new ListItem(DEFAULT_ROLE));
            TableItemStyle tableStyle = new TableItemStyle();
            tableStyle.ForeColor = System.Drawing.Color.LightGray;
            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Insert";
            saveButton.CommandArgument = "Insert";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";
            // used this command name while button click events differentiation 
            // code modified by praveen on 30th Nov 2010
            LinkButton SelectOracleUserLinkButton = (LinkButton)((DetailsView)sender).Controls[0].FindControl("SelectOracleUserLinkButton");
            SelectOracleUserLinkButton.CommandArgument = "SelectOracleUser";
            SelectOracleUserLinkButton.CommandName = "SelectOracleUser";

            //Bug ID: csbr-133690 and csbr-132972
            //Dated: 18-11-2010
            // added if condition for checking whether value is inserted into database or not
            if (ViewState["RowsCount"] != null && ViewState["RowsCount"].ToString() == "0")
            {
                FillUserDetails();

                //Bug ID: csbr-
                //Dated: 03-21-2011
                //To activate LDAP Link
                LinkButton selectOracleUserLinkButton = (LinkButton)((DetailsView)sender).Controls[0].FindControl("SelectOracleUserLinkButton");
                Button getLDAPUserButton = (Button)((DetailsView)sender).Controls[0].FindControl("GetLDAPUser");
                TextBox password = (TextBox)((DetailsView)sender).Controls[0].FindControl("Password");
                TextBox confirmPassword = (TextBox)((DetailsView)sender).Controls[0].FindControl("ConfirmPassword");
                //Fixed Bug with ID:141850
                //Checking wheather the mode is LDAP or Oracle.
                //if oracle then selectOracleUserLinkButton is made visible true else getLDAPUserButton visible true
                if (user.IsLDAP)
                {
                    getLDAPUserButton.Text = Resources.Resource.GetLDAPUser_Button_Text;
                    selectOracleUserLinkButton.Visible = false;
                    getLDAPUserButton.Visible = true;
                    getLDAPUserButton.Enabled = true;

                    /*CBOE-1375 COE Security:User cannot type in Password and throws ldap error once wrong confirm paswword entered - Start*/
                    //disable password, confirm password and save button
                    password.BackColor = System.Drawing.Color.LightGray;
                    confirmPassword.BackColor = System.Drawing.Color.LightGray;
                    password.Enabled = false;
                    confirmPassword.Enabled = false;
                    password.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                    confirmPassword.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                    saveButton.Enabled = false;
                    /*CBOE-1375 COE Security:User cannot type in Password and throws ldap error once wrong confirm paswword entered - End*/
                }
                else
                {
                    getLDAPUserButton.Visible = false;
                    selectOracleUserLinkButton.Visible = true;
                    selectOracleUserLinkButton.Enabled = true;
                    /*CBOE-1375 COE Security:User cannot type in Password and throws ldap error once wrong confirm paswword entered - Start*/
                    //enable password, confirm password and save button
                    password.Enabled = true;
                    confirmPassword.Enabled = true;
                    saveButton.Enabled = true;
                    /*CBOE-1375 COE Security:User cannot type in Password and throws ldap error once wrong confirm paswword entered - End*/
                }
            }
            else
            {

                LinkButton selectOracleUserLinkButton = (LinkButton)((DetailsView)sender).Controls[0].FindControl("SelectOracleUserLinkButton");
                Button getLDAPUserButton = (Button)((DetailsView)sender).Controls[0].FindControl("GetLDAPUser");
                getLDAPUserButton.Text = Resources.Resource.GetLDAPUser_Button_Text;
                TemplateField passwordTemplate = (TemplateField)((DetailsView)sender).Fields[3];
                TemplateField confirmPasswordTemplate = (TemplateField)((DetailsView)sender).Fields[4];
                TextBox password = (TextBox)((DetailsView)sender).Controls[0].FindControl("Password");
                TextBox confirmPassword = (TextBox)((DetailsView)sender).Controls[0].FindControl("ConfirmPassword");
                CheckBox activeUser = (CheckBox)((DetailsView)sender).Controls[0].FindControl("Active");
                activeUser.Checked = true;

                if (user.IsLDAP == false)
                {
                    selectOracleUserLinkButton.Visible = true;
                    selectOracleUserLinkButton.Enabled = true;
                    getLDAPUserButton.Visible = false;
                    CustomValidator LDAPUserValidator = (CustomValidator)((DetailsView)sender).Controls[0].FindControl("LDAPUserCustomValidator");
                    LDAPUserValidator.Enabled = false;
                    ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Add User");
                    ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                    ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                    password.BackColor = System.Drawing.Color.White;
                    confirmPassword.BackColor = System.Drawing.Color.White;
                    password.Enabled = true;
                    confirmPassword.Enabled = true;
                }
                else
                {
                    if (user.IsExempt == true)
                    {
                        ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Add Exempt User");
                        selectOracleUserLinkButton.Visible = true;
                        selectOracleUserLinkButton.Enabled = true;
                        getLDAPUserButton.Enabled = false;
                        ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                        ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                        password.BackColor = System.Drawing.Color.White;
                        confirmPassword.BackColor = System.Drawing.Color.White;
                        password.Enabled = true;
                        confirmPassword.Enabled = true;
                    }
                    else
                    {
                        ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Add LDAP User");

                        selectOracleUserLinkButton.Visible = false;
                        getLDAPUserButton.Visible = true;
                        getLDAPUserButton.Enabled = true;
                        saveButton.Enabled = false;
                        TextBox UserID = (TextBox)((DetailsView)sender).Controls[0].FindControl("UserID");
                        UserID.Enabled = true;
                        UserID.Text = Resources.Resource.EnterLDAPUserName_TextBox_Text;
                        UserID.Font.Italic = true;
                        UserID.Attributes.Add("onclick", "this.select()");

                        //disable password
                        ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                        ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                        password.BackColor = System.Drawing.Color.LightGray;
                        confirmPassword.BackColor = System.Drawing.Color.LightGray;
                        password.Enabled = false;
                        confirmPassword.Enabled = false;
                        password.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                        confirmPassword.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                        RequiredFieldValidator passwordValidator = (RequiredFieldValidator)((DetailsView)sender).Controls[0].FindControl("PasswordRequiredField");
                        passwordValidator.Enabled = false;
                        RequiredFieldValidator confirmPasswordValidator = (RequiredFieldValidator)((DetailsView)sender).Controls[0].FindControl("ConfirmPasswordRequiredField");
                        confirmPasswordValidator.Enabled = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                if (Session["UserAvailableRoles"] != null)
                {
                    List<string> lst = (List<string>)Session["UserAvailableRoles"];
                    ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Clear();
                    foreach (string role in lst)
                    {
                        if (role != DEFAULT_ROLE)
                        {
                            ((ListBox)((DetailsView)sender).Controls[0].FindControl("LeftList")).Items.Add(new ListItem(role));
                        }

                    }
                }
                if (Session["UserCurrentRoles"] != null)
                {
                    List<string> lst = (List<string>)Session["UserCurrentRoles"];
                    ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Clear();
                    foreach (string role in lst)
                    {
                        ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Add(new ListItem(role));

                    }
                }
                if (Session["CurrentUser"] != null)
                {
                    COEUserBO usr = (COEUserBO)Session["CurrentUser"];
                    if (usr != null)
                    {
                        TextBox UserID = (TextBox)((DetailsView)sender).Controls[0].FindControl("UserID");
                        UserID.Enabled = true;
                        UserID.Text = usr.UserID;

                        TextBox UserCode = (TextBox)((DetailsView)sender).Controls[0].FindControl("UserCode");
                        UserCode.Text = usr.UserCode;

                        TextBox UsrEmail = (TextBox)((DetailsView)sender).Controls[0].FindControl("Email");
                        UsrEmail.Text = usr.Email;

                        TextBox usrFirstName = (TextBox)((DetailsView)sender).Controls[0].FindControl("FirstName");
                        usrFirstName.Text = usr.FirstName;

                        TextBox usrLastName = (TextBox)((DetailsView)sender).Controls[0].FindControl("LastName");
                        usrLastName.Text = usr.LastName;

                        TextBox usrMiddleName = (TextBox)((DetailsView)sender).Controls[0].FindControl("MiddleName");
                        usrMiddleName.Text = usr.MiddleName;

                        DropDownList usrSiteID = (DropDownList)((DetailsView)sender).Controls[0].FindControl("SiteDropDownList1");
                        usrSiteID.SelectedIndex = siteIndex;

                        DropDownList usrSupervisorID = (DropDownList)((DetailsView)sender).Controls[0].FindControl("SupervisorDropDownList1");
                        usrSupervisorID.SelectedIndex = SupervisorIndex;

                        TextBox usrTelephone = (TextBox)((DetailsView)sender).Controls[0].FindControl("Telephone");
                        usrTelephone.Text = usr.Telephone.ToString();

                        TextBox usrAddress = (TextBox)((DetailsView)sender).Controls[0].FindControl("Address");
                        usrAddress.Text = usr.Address.ToString();

                        CheckBox usrActive = (CheckBox)((DetailsView)sender).Controls[0].FindControl("Active");
                        usrActive.Checked = usr.Active;
                    }
                }
                ErrorMsg = string.Empty;
                siteIndex = 0;
                SupervisorIndex = 0;
            }
        }

        if (this.DetailsView1.DefaultMode == DetailsViewMode.Edit)
        {
            LinkButton selectOracleUserLinkButton = (LinkButton)((DetailsView)sender).Controls[0].FindControl("SelectOracleUserLinkButton");
            selectOracleUserLinkButton.Visible = false;
            Button getLDAPUserButton = (Button)((DetailsView)sender).Controls[0].FindControl("GetLDAPUser");
            getLDAPUserButton.Visible = false;
            TextBox password = (TextBox)((DetailsView)sender).Controls[0].FindControl("Password");
            TextBox confirmPassword = (TextBox)((DetailsView)sender).Controls[0].FindControl("ConfirmPassword");
            //password.Attributes["value"] = password.Text;
            //confirmPassword.Attributes["value"] = confirmPassword.Text;
            //Fixed Bug with ID:132972
            //passing the dummy password, when user edits a particular user
            password.Attributes["value"] = "@@@@@@@@@@";
            confirmPassword.Attributes["value"] = "@@@@@@@@@@";

            TemplateField passwordTemplate = (TemplateField)((DetailsView)sender).Fields[3];
            TemplateField confirmPasswordTemplate = (TemplateField)((DetailsView)sender).Fields[4];

            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Update";
            saveButton.CommandArgument = "Update";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";
            TextBox UserID = (TextBox)((DetailsView)sender).Controls[0].FindControl("UserID");
            UserID.Enabled = false;

            if (user.IsLDAP == false)
            {
                CustomValidator LDAPUserValidator = (CustomValidator)((DetailsView)sender).Controls[0].FindControl("LDAPUserCustomValidator");
                LDAPUserValidator.Enabled = false;
                ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Edit User");
                ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                password.BackColor = System.Drawing.Color.White;
                confirmPassword.BackColor = System.Drawing.Color.White;
                password.Enabled = true;
                confirmPassword.Enabled = true;
            }
            else
            {
                if (user.IsLDAP == true && user.IsExempt == true)
                {
                    ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Edit Exempt User");
                    ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                    ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.Red;
                    password.BackColor = System.Drawing.Color.White;
                    confirmPassword.BackColor = System.Drawing.Color.White;
                    password.Enabled = true;
                    confirmPassword.Enabled = true;
                }
                else
                {
                    ((Forms_ContentArea_EditUser)this.Page).Master.SetPageTitle("Edit LDAP User");
                    //disable password
                    ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                    ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                    password.BackColor = System.Drawing.Color.LightGray;
                    confirmPassword.BackColor = System.Drawing.Color.LightGray;
                    password.Enabled = false;
                    confirmPassword.Enabled = false;
                    password.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                    confirmPassword.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                }
            }
        }
        
    }

    //Bug ID: csbr-133690 and csbr-132972
    //Dated: 18-11-2010
    // this function fill's entered user details 
    void FillUserDetails()
    {
        if (ViewState["RowsCount"] != null && ViewState["RowsCount"].ToString() == "0")
        {
            COEUserBO user = (COEUserBO)ViewState["user"];
            TextBox mytext = (TextBox)DetailsView1.Controls[0].FindControl("UserID");
            mytext.Text = user.UserID;
            TextBox mytext2 = (TextBox)DetailsView1.Controls[0].FindControl("UserCode");
            mytext2.Text = user.UserCode;
            TextBox mytext3 = (TextBox)DetailsView1.Controls[0].FindControl("Email");
            mytext3.Text = user.Email;
            TextBox mytext4 = (TextBox)DetailsView1.Controls[0].FindControl("FirstName");
            mytext4.Text = user.FirstName;
            TextBox mytext5 = (TextBox)DetailsView1.Controls[0].FindControl("LastName");
            mytext5.Text = user.LastName;
            TextBox mytext6 = (TextBox)DetailsView1.Controls[0].FindControl("MiddleName");
            mytext6.Text = user.MiddleName;
            TextBox mytext7 = (TextBox)DetailsView1.Controls[0].FindControl("Telephone");
            mytext7.Text = user.Telephone;
            TextBox mytext8 = (TextBox)DetailsView1.Controls[0].FindControl("Address");
            mytext8.Text = user.Address;
            TextBox password = (TextBox)DetailsView1.Controls[0].FindControl("Password");
            password.Text = "          ";
            TextBox confirmPassword = (TextBox)DetailsView1.Controls[0].FindControl("ConfirmPassword");
            confirmPassword.Text = "          ";

            CheckBox MyCheckBox = (CheckBox)DetailsView1.Controls[0].FindControl("Active");
            if (user.Active)
                MyCheckBox.Checked = true;
            else
                MyCheckBox.Checked = false;

        }
    }
    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events
        this.DetailsView1.ItemCommand += new DetailsViewCommandEventHandler(DetailsViewCommand_ButtonClicked);
        #endregion
        base.OnInit(e);
    }

    public void LDAPUser_ServerValidate(object source, ServerValidateEventArgs args)
    {
        TextBox userID = (TextBox)DetailsView1.Controls[0].FindControl("UserID");
        Button getLDAPButton = (Button)DetailsView1.Controls[0].FindControl("GetLDAPUser");
        TextBox password = (TextBox)DetailsView1.Controls[0].FindControl("Password");
        TextBox confirmPassword = (TextBox)DetailsView1.Controls[0].FindControl("ConfirmPassword");
        TemplateField passwordTemplate = (TemplateField)DetailsView1.Fields[3];
        TemplateField confirmPasswordTemplate = (TemplateField)DetailsView1.Fields[4];

        try
        {
            if (getLDAPButton.Text == Resources.Resource.GetLDAPUser_Button_Text)
            {
                COEUserBO user = COEUserBO.GetLDAPUser(userID.Text);
                if (user != null)
                {
                    //Session["CurrentUser"] = (COEUserBO)user;
                    userID.Text = user.UserID;
                    getLDAPButton.Text = Resources.Resource.Accept_Button_Text;
                    Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)DetailsView1.Controls[0].FindControl("SaveButton");
                    saveButton.Enabled = false;
                    TextBox userCode = (TextBox)DetailsView1.Controls[0].FindControl("UserCode");
                    userCode.Text = user.UserCode;
                    TextBox lastName = (TextBox)DetailsView1.Controls[0].FindControl("LastName");
                    lastName.Text = user.LastName;
                    TextBox firstName = (TextBox)DetailsView1.Controls[0].FindControl("FirstName");
                    firstName.Text = user.FirstName;
                    TextBox middleName = (TextBox)DetailsView1.Controls[0].FindControl("MiddleName");
                    middleName.Text = user.MiddleName;
                    TextBox email = (TextBox)DetailsView1.Controls[0].FindControl("Email");
                    email.Text = user.Email;
                    ((System.Web.UI.WebControls.DataControlField)(passwordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                    ((System.Web.UI.WebControls.DataControlField)(confirmPasswordTemplate)).HeaderStyle.ForeColor = System.Drawing.Color.LightGray;
                    password.BackColor = System.Drawing.Color.LightGray;
                    confirmPassword.BackColor = System.Drawing.Color.LightGray;
                    password.Enabled = false;
                    confirmPassword.Enabled = false;
                    password.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                    confirmPassword.ToolTip = Resources.Resource.PasswordCannotBeChangedLDAP_Tooptip_Text;
                    this.UserDetailsViewUpdatePanel.Update();
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = false;
                }
            }
            else if (getLDAPButton.Text == Resources.Resource.Accept_Button_Text)
            {
                userID.Enabled = false;
                Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)DetailsView1.Controls[0].FindControl("SaveButton");
                saveButton.Enabled = true;
                getLDAPButton.Text = Resources.Resource.Cancel_Button_Text;
                args.IsValid = true;
                this.UserDetailsViewUpdatePanel.Update();

            }
            else
            {
                string userCode = this.ResetLDAPInsert();
                this.DetailsView1.DataBind();
                ((TextBox)DetailsView1.Controls[0].FindControl("UserID")).Text = userCode;
            }
        }
        catch (Exception)
        {
            CustomValidator LDAPUserValidator = ((CustomValidator)DetailsView1.Controls[0].FindControl("LDAPUserCustomValidator"));
            LDAPUserValidator.ErrorMessage = Resources.Resource.LDAPUserValidation_Error_Message;
            args.IsValid = false;
            this.UserDetailsViewUpdatePanel.Update();
        }

    }


    /// <summary>
    /// add client script, add style sheets etc
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        //create the javascript to show the YUI panel for each button in the button bars for groups and group users.
        string key = "AddExternalUserPanel";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
        {
            string script = "<script type=\"text/javascript\">";
            script += "var validationActive=true;";
            script += "YAHOO.namespace(\"coemanager.security\");";
            script += "function init() {";
            script += "YAHOO.coemanager.security.AddOracleUserPanel = new YAHOO.widget.Panel(\"addOracleUserPanel\",";
            script += "{width:\"600px\",fixedcenter:true,visible:false,modal:true,draggable:true,iframe:true,close:true,modal:false,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.AddOracleUserPanel.render();}";
            script += "YAHOO.util.Event.addListener(window, \"load\", init);";
            script += "function showAddOracleUserPanel(){";
            //Fixed CSBR-140178
            //script += "DisableValidators();";
            script += "YAHOO.coemanager.security.AddOracleUserPanel.show();";
            //script += "YAHOO.coemanager.security.AddOracleUserPanel.hideEvent.subscribe(EnableValidators);";
            script += "}";
            script += "function EnableValidators() { ValidationSummaryOnSubmit = ValidationSummaryOnSubmitEx; validationActive=true; Page_ValidationActive=true;}";
            script += "function DisableValidators() { ValidationSummaryOnSubmit = original_ValidationSummaryOnSubmit; Page_ValidationActive=false; validationActive=false;}";
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(EditUserUC), key, script);
        }
        base.OnPreRender(e);
    }

    protected void SelectOracleUserButton_Click(object sender, EventArgs e)
    {
        UsersAddListBox.Visible = true;
        OracleUserAddUpdatePanel.Update();
    }

    /// <summary>
    /// Here we intercept command events of coming from the detail view control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DetailsViewCommand_ButtonClicked(object sender, DetailsViewCommandEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        // used 'key' parameter as there is a problem in conversion from Button and LinkButton
        // code modified by praveen on 28th Nov 2010
        string key = string.Empty;
        try
        {
            switch (e.CommandName)
            {
                case "Cancel":
                    CancelEdit();
                    key = ((Button)e.CommandSource).ID;
                    break;
                case "SelectOracleUser":
                    key = ((LinkButton)e.CommandSource).ID;
                    break;
                default:
                    switch (((Button)e.CommandSource).ID)
                    {
                        case "MoveFromLeftListToRightList":
                            ShuttleListsLeftToRight(sender);
                            break;
                        case "MoveFromRightListToLeftList":
                            ShuttleListsRightToLeft(sender);
                            break;
                    }
                    key = ((Button)e.CommandSource).ID;
                    break;
            }
        }
        catch (Exception) { }
        if (key == "MoveFromLeftListToRightList" || key == "MoveFromRightListToLeftList")
        {
            ((UpdatePanel)(this.DetailsView1).Controls[0].FindControl("EditUserUpdatePanel")).Update();
        }
        else
        {
            UserDetailsViewUpdatePanel.Update();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void SelectOracleUser_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Response.Redirect("UserList.aspx?userType=dbms&appName=" + Request.QueryString["appName"].ToString(), false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region User methods
    /// <summary>
    /// Get the the COEUserBO for the selected user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COEUserBODataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        COEUserBO user = GetUser();
        Session["CurrentUser"] = user;
        e.BusinessObject = user;
    }

    /// <summary>
    /// Update the user with changes values and added/removed roles
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COEUserBODataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
        COEUserBO user = GetUser();
        COERoleBOList userRoles = user.Roles;
        COERoleBOList userAvailableRoles = user.AvailableRoles;
        Csla.Data.DataMapper.Map(e.Values, user, "isDBMSUser");
        //template fields do not map correctly so they need to be added
        user.Roles = userRoles;
        user.AvailableRoles = userAvailableRoles;
        user.UserCode = (string)e.Values["UserCode"];
        user.Email = (string)e.Values["Email"];
        //user.Department = (string)e.Values["Department"];
        user.FirstName = (string)e.Values["FirstName"];
        user.LastName = (string)e.Values["LastName"];
        user.MiddleName = (string)e.Values["MiddleName"];
        user.Password = (string)e.Values["Password"];
        user.ConfirmPassword = (string)e.Values["ConfirmPassword"];
        user.SiteID = Convert.ToInt32(e.Values["SiteID"]);
        user.SupervisorID = Convert.ToInt32(e.Values["SupervisorID"]);
        user.Telephone = (string)e.Values["Telephone"];
        //user.Title = (string)e.Values["Title"];
        user.Address = (string)e.Values["Address"];
        user.Active = (bool)e.Values["Active"];

        e.RowsAffected = SaveUser("UPDATE", user, user.IsLDAP);

        if (e.RowsAffected == 1)
        {
            Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }

    }

    /// <summary>
    /// Add  a new user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void COEUserBODataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
    {
        //dbms user has been set previously and stored in the session["currentuser"] object
        COEUserBO user = GetUser();
        Csla.Data.DataMapper.Map(e.Values, user, true, "UserID");
        //template fields do not map correctly so they need to be added
        user.UserID = (string)e.Values["UserID"];
        user.Roles = COERoleBOList.NewList((List<string>)Session["UserCurrentRoles"]);
        user.UserCode = (string)e.Values["UserCode"];
        user.Email = (string)e.Values["Email"];
        //user.Department = (string)e.Values["Department"];
        user.FirstName = (string)e.Values["FirstName"];
        user.LastName = (string)e.Values["LastName"];
        user.MiddleName = (string)e.Values["MiddleName"];
        user.Password = (string)e.Values["Password"];
        user.ConfirmPassword = (string)e.Values["ConfirmPassword"];
        user.SiteID = Convert.ToInt32(e.Values["SiteID"]);
        user.SupervisorID = Convert.ToInt32(e.Values["SupervisorID"]);
        user.Telephone = (string)e.Values["Telephone"];
        //user.Title = (string)e.Values["Title"];
        user.Address = (string)e.Values["Address"];
        user.Active = (bool)e.Values["Active"];

        e.RowsAffected = SaveUser("INSERT", user, user.IsLDAP);
        if (e.RowsAffected == 1)
        {
            Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        }
        //Bug ID: csbr-133690
        //Dated: 18-11-2010
        // added elseif condition for checking whether value is inserted into database or not 
        else if (e.RowsAffected == 0)
        {
            ViewState["user"] = user;
            ViewState["RowsCount"] = 0;

        }
        //end comments
    }
  
    private int SaveUser(string saveType, COEUserBO user, bool isLDAP)
    {

        int rowsAffected;
        userCurrentRoles = new List<string>();
        ListBox workingListRight = (ListBox)((DetailsView)DetailsView1).Controls[0].FindControl("RightList");
        userCurrentRoles.Clear();
        foreach (ListItem lt in workingListRight.Items)
            if (!userCurrentRoles.Contains(lt.Text))    //CBOE-1793 COE Security-Add User-Observed available roles duplication for multiple times  ASV 05SEP13
            userCurrentRoles.Add(lt.Text);
        Session["UserCurrentRoles"] = userCurrentRoles;
        user.Roles = COERoleBOList.NewList(userCurrentRoles);

        DropDownList SiteDropDown = (DropDownList)((DetailsView)DetailsView1).Controls[0].FindControl("SiteDropDownList1");
        if (SiteDropDown != null)
            siteIndex = SiteDropDown.SelectedIndex;
        else
            siteIndex = 0;
        DropDownList SupervisorDropDown = (DropDownList)((DetailsView)DetailsView1).Controls[0].FindControl("SupervisorDropDownList1");

        if (SupervisorDropDown != null)
            SupervisorIndex = SupervisorDropDown.SelectedIndex;
        else
            SupervisorIndex = 0;
        try
        {
            if (saveType.ToUpper() == "ACTIVATE")
            {
                Session["CurrentUser"] = user.Activate();
            }
            else
            {
                Session["CurrentUser"] = user.Save();
            }
            rowsAffected = 1;
        }
        catch (Csla.Validation.ValidationException)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();

            if (user.BrokenRulesCollection.Count == 1)
                message.AppendFormat("* {0}",
                    this.FormatError(user.BrokenRulesCollection[0].Description));
            else
                foreach (Csla.Validation.BrokenRule rule in user.BrokenRulesCollection)
                {
                    message.AppendFormat("* {0}<br/>", this.FormatError(rule.Description));
                }
            this.DisplayErrorMessage(message.ToString());
            rowsAffected = 0;
            if (isLDAP && saveType == "INSERT")
            {
                ResetLDAPInsert();
            }
            Session["CurrentUser"] = null;
            user = GetUser();
        }
        catch (Csla.DataPortalException ex)
        {
            rowsAffected = 0;
            if (ex.BusinessException.Message.Contains("is or has been already taken by another user"))
            {
                this.EnableUserPanel.Visible = true;
                this.ActivateUserConfimationUP.Update();
            }
            else if (ex.BusinessException.Message.Contains("conflicts with another user"))
            {
                ErrorMsg = ex.BusinessException.Message;
                this.DisplayErrorMessage(ex.BusinessException.Message);

            }
            else
            {
                this.DisplayErrorMessage(ex.BusinessException.Message);

                if (isLDAP && saveType == "INSERT")
                {
                    ResetLDAPInsert();
                }
                Session["CurrentUser"] = null;
                user = GetUser();
            }
        }
        catch (Exception ex)
        {
            this.DisplayErrorMessage(ex.Message);
            rowsAffected = 0;
            if (isLDAP && saveType == "INSERT")
            {
                ResetLDAPInsert();
            }
            Session["CurrentUser"] = null;
             user = GetUser();
        }
        return rowsAffected;
    }

    private string FormatError(string description)
    {
        return description.Replace("UserID", "User Name").Replace("FirstName", "First Name").Replace("MiddleName", "Middle Name").Replace("LastName", "Last Name");
    }

    private void DisplayErrorMessage(string message)
    {
        ErrorMessage.Text = message;

    }

    /// <summary>
    /// Get the current user
    /// </summary>
    /// <returns></returns>
    private COEUserBO GetUser()
    {
        object businessObject = Session["CurrentUser"];
        if (businessObject == null ||
          !(businessObject is COEUserBO))
        {
            try
            {

                string userID = Request.QueryString["userID"];

                if (!string.IsNullOrEmpty(userID))
                {

                    businessObject = COEUserBO.Get(userID);
                    SupervisorList supList = SupervisorList.GetList();
                    if (!supList.ContainsKey(((COEUserBO)businessObject).SupervisorID) && supList.Count > 0)
                        ((COEUserBO)businessObject).SupervisorID = supList[0].Key;
                }
                else
                    businessObject = COEUserBO.New();
                Session["CurrentUser"] = businessObject;
            }
            catch (System.Security.SecurityException)
            {
                Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
            }
        }
        return (COEUserBO)businessObject;
    }

    /// <summary>
    /// capture the cancel button click and go back to the users list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void CancelEdit()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Response.Redirect("ManageUsers.aspx?appName=" + Request.QueryString["appName"].ToString(), false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void ApplyAuthorizationRules()
    {
        COEUserBO user = GetUser();
        Session["UserAvailableRoles"] = COERoleBOList.GetRoleNameList(user.AvailableRoles);
        Session["UserCurrentRoles"] = COERoleBOList.GetRoleNameList(user.Roles);
        Session["OriginalAvailableRoles"] = Session["UserAvailableRoles"];
        Session["OriginalCurrentRoles"] = Session["UserCurrentRoles"];
        if (COEUserBO.CanEditObject())
        {
            if (user.IsNew)
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

    private void SetControlsAttributes()
    {
        this.UserAlreadyExistLabel.Text = Resources.Resource.ActivateUserConfirmation_Label_Text;
        this.OKActivationButton.Text = Resources.Resource.OK_Button_Text;
        this.CancelActivationButton.Text = Resources.Resource.Cancel_Button_Text;
    }
    #endregion

    #region Role List Methods
    /// <summary>
    /// Move list items from Right side to left
    /// </summary>
    protected void ShuttleListsRightToLeft(object sender)
    {
        int itemCountRight = ((ListBox)((DetailsView)sender).Controls[0].FindControl("RightList")).Items.Count;
        ListBox workingListRight = (ListBox)((DetailsView)sender).Controls[0].FindControl("RightList");
        List<string> lstworkingRight = new List<string>();
        foreach (ListItem lt in workingListRight.Items)
            lstworkingRight.Add(lt.Text);
        Session["UserCurrentRoles"] = lstworkingRight;
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
            MoveCurrentToAvailable(item.Text);
        }
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
            MoveAvailableToCurrent(item.Text);
        }
    }

    /// <summary>
    /// move items from current roles list to available
    /// </summary>
    /// <param name="itemName"></param>
    private void MoveCurrentToAvailable(string itemName)
    {
        COEUserBO user = (COEUserBO)Session["CurrentUser"];
        userAvailableRoles = (List<string>)Session["UserAvailableRoles"];
        userAvailableRoles.Add(itemName);
        Session["UserAvailableRoles"] = userAvailableRoles;
        userCurrentRoles = (List<string>)Session["UserCurrentRoles"];
        userCurrentRoles.Remove(itemName);
        Session["UserCurrentRoles"] = userCurrentRoles;
        user.Roles = COERoleBOList.NewList(userCurrentRoles);
        user.AvailableRoles = COERoleBOList.NewList(userAvailableRoles);
        Session["CurrentUser"] = user;
    }

    /// <summary>
    /// move items from available roles list to current
    /// </summary>
    /// <param name="itemName"></param>
    private void MoveAvailableToCurrent(string itemName)
    {
        COEUserBO user = (COEUserBO)Session["CurrentUser"];
        userAvailableRoles = (List<string>)Session["UserAvailableRoles"];
        userAvailableRoles.Remove(itemName);
        Session["UserAvailableRoles"] = userAvailableRoles;
        userCurrentRoles = (List<string>)Session["UserCurrentRoles"];
        userCurrentRoles.Add(itemName);
        Session["UserCurrentRoles"] = userCurrentRoles;
        user.Roles = COERoleBOList.NewList(userCurrentRoles);
        user.AvailableRoles = COERoleBOList.NewList(userAvailableRoles);
        Session["CurrentUser"] = user;
    }
    #endregion

    #region Supervisor List methods
    /// <summary>
    /// Get the supervisor list for the current user supervior id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SupervisorDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = SupervisorList.GetList();
    }
    #endregion

    #region Site list Methods
    /// <summary>
    /// Get the site list for the current user supervior id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SiteDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = SiteList.GetList();
    }
    #endregion

    #region DBMSUserList for add oracle users YUI panel
    /// <summary>
    /// databinding of csla DBMSUserList to the PersonListBox control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DBMSUserList_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        e.BusinessObject = UserList.GetDBMSList();
    }

    /// <summary>
    /// event handler when the add users button is clicked in the userlistwebgrid tool bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UsersAddListBox_IndexChange(object sender, EventArgs e)
    {
        COEUserBO user = GetUser();
        user.IsDBMSUser = true;
        Session["CurrentUser"] = (COEUserBO)user;
        string UID = UsersAddListBox.SelectedValue;
        TextBox mytext = (TextBox)DetailsView1.Controls[0].FindControl("UserID");
        mytext.Text = UID;
        mytext.Enabled = false;
        TextBox mytext2 = (TextBox)DetailsView1.Controls[0].FindControl("UserCode");
        mytext2.Text = UID;
        TextBox mytext3 = (TextBox)DetailsView1.Controls[0].FindControl("LastName");
        mytext3.Text = UID;
        //Bug ID: csbr-125537 
        //Dated: 02-01-2011
        // Passing the default value i.e "@@@@@@@@@@" to Password and Confirm password fields
        //as we can not retrive the original password from db.
        TextBox password = (TextBox)DetailsView1.Controls[0].FindControl("Password");
        password.Attributes["value"] = "@@@@@@@@@@";
        password.Enabled = false;
        TextBox confirmPassword = (TextBox)DetailsView1.Controls[0].FindControl("ConfirmPassword");
        confirmPassword.Attributes["value"] = "@@@@@@@@@@";
        confirmPassword.Enabled = false;
        this.UserDetailsViewUpdatePanel.Update();
    }
    #endregion

    #region LDAP
    protected void LDAPButton_Clicked(object sender, EventArgs e)
    {

    }

    private string ResetLDAPInsert()
    {
        TextBox userID = (TextBox)DetailsView1.Controls[0].FindControl("UserID");
        Button getLDAPButton = (Button)DetailsView1.Controls[0].FindControl("GetLDAPUser");
        userID.Enabled = true;
        Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)DetailsView1.Controls[0].FindControl("SaveButton");
        saveButton.Enabled = false;
        getLDAPButton.Enabled = true;
        getLDAPButton.Text = Resources.Resource.GetLDAPUser_Button_Text;
        this.UserDetailsViewUpdatePanel.Update();
        return userID.Text;
    }
    #endregion
}
