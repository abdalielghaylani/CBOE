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
using Infragistics.WebUI.WebDataInput;
using System.Xml;

/// <summary>
/// Class for loading EditGroup UserControl in two different modes allowing for preloading in YUI panels.Putting the detailview directly in the parent page
/// cases rendering issues.  This method allows to simulataneously load a COEGroupBO in Insert and one in Edit mode. 
/// </summary>
public partial class AddGroupUC : System.Web.UI.UserControl
{
    #region Member Variables
    GroupOperationType _groupOperation = GroupOperationType.Add;
    Forms_Master_MasterPage _masterPage = null;
    string _hiddenFieldName = string.Empty;
    #endregion

    #region Properties


    //property for user control that indicates if it should load and edit or insert version of COEGroupBO
    public GroupOperationType GroupOperation
   
    {
        set
        {
            _groupOperation = value;
        }
        get
        {
            return _groupOperation;
        }
        
    }

    /// <summary>
    /// Group Operations
    /// </summary>
    public enum GroupOperationType
    {
        Edit,
        Add,
        
    }
    #endregion
    
    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
                  
    }

     /// <summary>
     /// Does post databinding work
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
    protected void DetailsView_Load(object sender, EventArgs e)
    {
        if (this.GroupOperation == GroupOperationType.Add)
        {
            COEGroupBO group = GetGroup();
            this.AddGroupDetailView.ChangeMode(DetailsViewMode.Insert);
            if (group != null)
            {

                COEGroupBO ParentGroup = GetParentGroup();
                DropDownList leader = ((DropDownList)((DetailsView)sender).Controls[0].FindControl("UserListDropDown2"));
                DropDownList leader2 = ((DropDownList)((DetailsView)sender).Controls[0].FindControl("UserListDropDown"));
                TextBox groupName = ((TextBox)((DetailsView)sender).Controls[0].FindControl("GroupName"));
                TextBox val = ((TextBox)((DetailsView)sender).Controls[0].FindControl("GroupMaster")); 
                ListBox leftList = ((ListBox)((DetailsView)sender).Controls[0].FindControl("InsertLeftList"));
                Button leftListButton = ((Button)((DetailsView)sender).Controls[0].FindControl("InsertLeftToRightButton"));
                ListBox rightList = ((ListBox)((DetailsView)sender).Controls[0].FindControl("InsertRightList"));
                Button rightListButton = ((Button)((DetailsView)sender).Controls[0].FindControl("InsertRightToLeftButton"));
                HiddenField listHiddenField = ((HiddenField)((DetailsView)sender).Controls[0].FindControl("InsertHidden"));
                HiddenField ListTextField = ((HiddenField)((DetailsView)sender).Controls[0].FindControl("InsertHiddenText"));
                if (group.AvailablePrivSets != null)
                {
                    foreach (Csla.NameValueListBase<string, string>.NameValuePair role in group.AvailablePrivSets)
                    {
                        leftList.Items.Add(new ListItem(role.Value, role.Key));
                    }
                }
                if (Session["RoleTxt"]!=null)
                {
                    string roleListText = Session["RoleTxt"].ToString();
                    string roleListValue = Session["RoleValue"].ToString();

                    if (roleListText.Length > 0)
                    {
                        roleListText = roleListText.Remove(roleListText.Length - 1);
                        string[] arrRoleListText = roleListText.Split(',');
                        string[] arrRoleListValue = roleListValue.Split(',');
                        for (int i = 0; i < arrRoleListText.Length; i++)
                        {
                            rightList.Items.Insert(i, new ListItem(arrRoleListText[i], arrRoleListValue[i]));
                            leftList.Items.Remove(new ListItem(arrRoleListText[i], arrRoleListValue[i]));
                        }
                    }
                }
                listHiddenField.Value=group.PrivSetsString;
                //set onClickEvents for move buttons
                leftListButton.OnClientClick = "MoveItemInsert" + "('" + leftList.ClientID + "','" + rightList.ClientID + "')";
                rightListButton.OnClientClick = "MoveItemInsert" + "('" + rightList.ClientID + "','" + leftList.ClientID + "')";
                _hiddenFieldName = listHiddenField.ClientID;
                if (Session["Role"]!=null)
                {
                    rightList = (ListBox)Session["Role"]; 
                }
                 groupName.Text = Session["groupname"] == null ? string.Empty : Session["groupname"].ToString();
                if ((Session["LeaderPersonID"] != null))
                {
                    if (leader != null)
                    {
                        leader.SelectedValue = Session["LeaderPersonID"].ToString();
                    }
                    else
                    {
                        leader2.SelectedValue = Session["LeaderPersonID"].ToString();
                    }
                }

                if (ParentGroup.GroupID == 0)
                {
                    val.Text =  "Inventory Masters";
                }
                else
                {
                    val.Text =  ParentGroup.GroupName;
                }

                if (group.PrivSets != null)
                {
                    foreach (Csla.NameValueListBase<string, string>.NameValuePair role in group.PrivSets)
                    {
                        rightList.Items.Add(new ListItem(role.Value, role.Key));
                    }

                }
                Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
                saveButton.CommandName = "Insert";
                saveButton.CommandArgument = "Insert";
                Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
                cancelButton.CommandArgument = "Cancel";
                cancelButton.CommandName = "Cancel";
            }
            else
            {
                //prevent loading problems with their is no group object
                this.AddGroupDetailView.ChangeMode(DetailsViewMode.Insert);
            }
        }
    }
    
    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events
        _masterPage = (Forms_Master_MasterPage)this.Page.Master;
        #endregion
        base.OnInit(e);
    }
    protected override void OnPreRender(EventArgs e)
    {

        HiddenField privValuesField = (HiddenField)this.AddGroupDetailView.Controls[0].FindControl("InsertHidden");
        HiddenField privTextField = (HiddenField)this.AddGroupDetailView.Controls[0].FindControl("InsertHiddenText");
        string privTextClientID = string.Empty;
        string privValueClientID = string.Empty;
        try
        {
            privValueClientID = privValuesField.ClientID;
            privTextClientID = privTextField.ClientID;
        }
        catch { } //don't do anything here

        string key = "ShuttleListInsert";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
        {   //Shuttle List javascript
            string script = "<script type=\"text/javascript\">";
            script += "function MoveItemInsert";
            script += "(ctrlSource, ctrlTarget){";
            script += "var Source = document.getElementById(ctrlSource);";
            script += "var Target = document.getElementById(ctrlTarget); ";
            script += "if ((Source != null) && (Target != null)) {";
            script += "while ( Source.options.selectedIndex >= 0 ) {";
            script += "var HiddenList = document.getElementById('" + privValueClientID + "');";
            script += "var HiddenText= document.getElementById('" + privTextClientID + "');";
            script += " var SelectedValue = Source.options[Source.options.selectedIndex].value + ','; ";
            script += " var SelectedText = Source.options[Source.options.selectedIndex].text + ','; ";
            script += " var newOption = new Option(); ";
            script += "newOption.text = Source.options[Source.options.selectedIndex].text;";
            script += "newOption.value = Source.options[Source.options.selectedIndex].value;";

            script += "Target.options[Target.length] = newOption;";
            script += "Source.remove(Source.options.selectedIndex);";
            script += " if (HiddenList.value.indexOf(SelectedValue) == -1){";
            script += "HiddenList.value += SelectedValue; HiddenText.value += SelectedText;}";
           // script += " else";
            script += "}}}";
            //{HiddenList.value = HiddenList.value.replace(SelectedValue,'');}
            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(AddGroupUC), key, script);
        }
        base.OnPreRender(e);
    }

    public string GetHiddenFieldValueString()
        {
            //string returnValue = string.Empty;
            //string hiddenFieldValue = this.HiddenField1.Value;
            //if (hiddenFieldValue.Length > 0)
            //{
            //    returnValue = hiddenFieldValue.Remove(hiddenFieldValue.Length - 1);
            //}
            //return returnValue;
            return string.Empty;
        }
    
    
    
    #endregion

    #region group methods

    /// <summary>
    /// Get the current group based on node selected in tree. 
    /// </summary>
    /// <returns></returns>
    private COEGroupBO GetGroup()
    {
        int groupID = -1;
        COEGroupBO coeGroup = null;
        try
        {
            {
                coeGroup = COEGroupBO.New();
            }

        }
        catch (System.Security.SecurityException)
        {
            //where to redirect?
            //Response.Redirect("Managegroups.aspx");
        }

        return (COEGroupBO)coeGroup;
    }
   
    /// <summary>
    /// Get the the COEgroupBO for the selected group node in the tree
    /// </summary>
    /// <param name="sender">the sender is the CSLA GroupDataSource</param>
    /// <param name="e">csla select object arguments which will be used to add the business object to bind to the form</param>
    protected void GroupBODataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        COEGroupBO group = GetGroup();
        e.BusinessObject = group;
    }
    /// <summary>
    /// Add  a new group
    /// </summary>
    /// <param name="sender">the sender is the CSLA GroupDataSource</param>
    /// <param name="e">csla insert object arguments which contain the values entered int he form</param>
    protected void GroupBODataSource_InsertObject(object sender, Csla.Web.InsertObjectArgs e)
    {
        ManageGroups manageGroupsControladd = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");
        manageGroupsControladd.ShowGroupAddBtn();
        TextBox groupName = (TextBox)this.AddGroupDetailView.Controls[0].FindControl("GroupName");
        ListBox rightList = (ListBox)this.AddGroupDetailView.Rows[0].FindControl("InsertRightList");
        HiddenField privValuesField = (HiddenField)this.AddGroupDetailView.Controls[0].FindControl("InsertHidden");
        HiddenField privTextField = (HiddenField)this.AddGroupDetailView.Controls[0].FindControl("InsertHiddenText");
        Session["NewGroupNode"] = null;
        COEGroupBO group = GetGroup();
        Csla.Data.DataMapper.Map(e.Values, group, true, "GroupID");
        group.GroupName = groupName.Text;
        string errMessage = string.Empty;
        if (Session["RoleTxt"] == null)
        {
            Session["RoleTxt"] = privTextField.Value;
            Session["RoleValue"] = privValuesField.Value;
        }
        if (group.GroupName == "") 
        {
            errMessage = " - Group Name can not be empty<BR>";
            Session["groupname"] = null;
        }
        else if (group.GroupName.Trim() == string.Empty)
        {
            errMessage = " - Please enter a valid Group Name<BR>";
            groupName.Text = "";
            Session["groupname"] = null;
        }
        else
        {
            Session["GroupName"] = group.GroupName;
        }

        if ((group.LeaderPersonID == 0))
        {
            errMessage += " - Group Leader can not be empty<BR>";
            Session["LeaderPersonID"] = null;
        }
        else
        {
            Session["LeaderPersonID"] = group.LeaderPersonID;
        }
        if (errMessage != string.Empty)
        {
           ManageGroups manageGroupsControl = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");
            manageGroupsControl.SetGroupAddPanel(errMessage, "error");
            manageGroupsControladd.ShowGroupAddokBtn();
            return;
        }
        group.ParentGroupID = (int)Session["LastGroupNode"];
        group.GroupOrgID = (int)Session["GroupOrgID"];
        
        string privValues = privValuesField.Value;
        if (privValues.Length > 0)
        {
            group.PrivSetsString = privValues.Remove(privValues.Length - 1);
        }
        e.RowsAffected = SaveGroup(group);
        
        //reget group so the object is updated properly with usrs
        Session["CurrentGroup"] = null;
        if (e.RowsAffected == 1)
        {
            
                Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
                UpdatePanel webPanel = (UpdatePanel)_masterPage.FindControlInPage("ManageGroupsControl").Controls[2].Controls[1];
                webPanel.Update();
                ManageGroups manageGroupsControl = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");
                manageGroupsControl.SetGroupAddPanel("Group added sucessfully", "Success");
                Session["LeaderPersonID"] = null;
                Session["groupname"] = null;
                Session["RoleValue"] = null;
                Session["RoleTxt"] = null;
        }
        else
        {
            UpdatePanel webPanel = (UpdatePanel)_masterPage.FindControlInPage("ManageGroupsControl").Controls[2].Controls[1];
            webPanel.Update();
        }
    }
   
    /// <summary>
    /// persis the new group settings to the database.
    /// </summary>
    /// <param name="group">COEGroupBO object</param>
    /// <returns>a COEGroupBO with saved values</returns>
    internal int SaveGroup(COEGroupBO group)
    {
        int rowsAffected;
        ManageGroups manageGroupsControl = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");

        try
        {
            Session["CurrentGroup"] = group.Save();
            Session["NewGroupNode"] = group.GroupID;
            rowsAffected = 1;
        }
        catch (Csla.Validation.ValidationException ex)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            message.AppendFormat("{0}<br/>", ex.Message);
            if (group.BrokenRulesCollection.Count == 1)
            {
                if (group.BrokenRulesCollection[0].Property == "LeaderPersonID")
                {
                    message.AppendFormat("* {0}: {1}",
                group.BrokenRulesCollection[0].Property,
                 "The LeaderPersonID required");
                }
                else
                {
                    message.AppendFormat("* {0}: {1}",
                      group.BrokenRulesCollection[0].Property,
                       group.BrokenRulesCollection[0].Description);
                }
            }
            else
            {
                foreach (Csla.Validation.BrokenRule rule in group.BrokenRulesCollection)
                    if (rule.Property == "LeaderPersonID")
                    {
                        message.AppendFormat("* {0}: {1}<br/>", rule.Property, "The LeaderPersonID required");
                    }
                    else
                    {
                        message.AppendFormat("* {0}: {1}<br/>", rule.Property, rule.Description);
                    }
            }
            manageGroupsControl.SetGroupAddPanel(message.ToString(), "Error");
            rowsAffected = 0;
        }
        catch (Csla.DataPortalException ex)
        {
            if (ex.BusinessException.Message.IndexOf("(COEDB.COEGROUP_AK)") > 0)
            {
                manageGroupsControl.SetGroupAddPanel("A group named " + group.GroupName + " already exists in the system", "Error");
            }
            else
            {
                manageGroupsControl.SetGroupAddPanel(ex.BusinessException.Message, "Error");
            }
            rowsAffected = 0;
        }
        catch (Exception ex)
        {
            manageGroupsControl.SetGroupAddPanel(ex.Message, "Error");
            rowsAffected = 0;
        }

   
        return rowsAffected;
    }

   
    
    #endregion

    private COEGroupBO GetParentGroup()
    {
        int groupID = -1;
        COEGroupBO coeGroup = null;
        try
        {
            if ((Session["LastGroupNode"] != null) && (_groupOperation == GroupOperationType.Add))
            {
                groupID = (int)Session["LastGroupNode"];
            }
            if (groupID > 0)
            {

                coeGroup = COEGroupBO.Get(groupID);
            }
            else
            {
                coeGroup = COEGroupBO.New();
            }
        }
        catch
        { //don't do anything here
        }
        return (COEGroupBO)coeGroup;
    }

  
    #region User List methods

    /// <summary>
    /// Get the user list for the current user supervior id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserListDataSource_SelectObject(
      object sender, Csla.Web.SelectObjectArgs e)
    {
        COEGroupBO group = GetGroup();
        if (group != null)
        {
            e.BusinessObject = PersonList.GetCOEList();
        }
    }
    #endregion
   
}
