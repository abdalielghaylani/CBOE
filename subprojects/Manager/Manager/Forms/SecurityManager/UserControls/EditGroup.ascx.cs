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
public partial class EditGroupUC : System.Web.UI.UserControl
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
        if (!Page.IsPostBack)
        {

        }
        else
        {

        }
    }

    /// <summary>
    /// Does post databinding work
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DetailsView_Load(object sender, EventArgs e)
    {
        this.EditGroupDetailView.ChangeMode(DetailsViewMode.Edit);
        COEGroupBO group = GetGroup();
        //Coverity Fixes: CBOE-313 : CID-11856
        if (group != null)
        {
            int parentGroupId = group.ParentGroupID;
            COEGroupBO parentGroup = GetParentGroup(parentGroupId);

            TextBox val = ((TextBox)((DetailsView)sender).Controls[0].FindControl("GroupMaster"));
            ListBox leftList = ((ListBox)((DetailsView)sender).Controls[0].FindControl("EditLeftList"));
            Button leftListButton = ((Button)((DetailsView)sender).Controls[0].FindControl("EditLeftToRightButton"));
            ListBox rightList = ((ListBox)((DetailsView)sender).Controls[0].FindControl("EditRightList"));
            Button rightListButton = ((Button)((DetailsView)sender).Controls[0].FindControl("EditRightToLeftButton"));
            HiddenField listHiddenField = ((HiddenField)((DetailsView)sender).Controls[0].FindControl("EditHidden"));
            listHiddenField.Value = group.PrivSetsString;
            //set onClickEvents for move buttons
            leftListButton.OnClientClick = "MoveItemEdit" + "('" + leftList.ClientID + "','" + rightList.ClientID + "')";
            rightListButton.OnClientClick = "MoveItemEdit" + "('" + rightList.ClientID + "','" + leftList.ClientID + "')";
            _hiddenFieldName = listHiddenField.ClientID;

            if (parentGroup.GroupID == 0)
            {
                val.Text = "Inventory Masters";
            }
            else
            {
                val.Text = parentGroup.GroupName;
            }
            //these loops are needed to load privileges. The shuttle list should be fixed so that the datasource loads properly
            if (group.AvailablePrivSets != null)
            {
                foreach (Csla.NameValueListBase<string, string>.NameValuePair role in group.AvailablePrivSets)
                {
                    leftList.Items.Add(new ListItem(role.Value, role.Key));
                }
            }
            if (group.PrivSets != null)
            {
                foreach (Csla.NameValueListBase<string, string>.NameValuePair role in group.PrivSets)
                {
                    rightList.Items.Add(new ListItem(role.Value, role.Key));
                }

            }
            Forms_Public_UserControls_ImageButton saveButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("SaveButton");
            saveButton.CommandName = "Update";
            saveButton.CommandArgument = "Update";
            Forms_Public_UserControls_ImageButton cancelButton = (Forms_Public_UserControls_ImageButton)((DetailsView)sender).Controls[0].FindControl("CancelButton");
            cancelButton.CommandArgument = "Cancel";
            cancelButton.CommandName = "Cancel";
        }
    }



    protected override void OnInit(EventArgs e)
    {
        #region Buttons Events
        _masterPage = (Forms_Master_MasterPage)this.Page.Master;
        //capture the click of all commands in the grid.
        #endregion
        base.OnInit(e);
    }
    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        HiddenField privValuesField = (HiddenField)this.EditGroupDetailView.Controls[0].FindControl("EditHidden");
        string key = "ShuttleListEdit";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
        {   //Shuttle List javascript
            string script = "<script type=\"text/javascript\">";
            script += "function MoveItemEdit";
            script += "(ctrlSource, ctrlTarget){";
            script += "var Source = document.getElementById(ctrlSource);";
            script += "var Target = document.getElementById(ctrlTarget); ";
            script += "if ((Source != null) && (Target != null)) {";
            script += "while ( Source.options.selectedIndex >= 0 ) {";
            script += "var HiddenList = document.getElementById('" + privValuesField.ClientID + "');";
            script += " var SelectedValue = Source.options[Source.options.selectedIndex].value + ','; ";
            script += " var newOption = new Option(); ";
            script += "newOption.text = Source.options[Source.options.selectedIndex].text;";
            script += "newOption.value = Source.options[Source.options.selectedIndex].value;";

            script += "Target.options[Target.length] = newOption;";
            script += "Source.remove(Source.options.selectedIndex);";
            script += " if (HiddenList.value.indexOf(SelectedValue) == -1){";
            script += "HiddenList.value += SelectedValue;}";
            script += " else";
            script += "{HiddenList.value = HiddenList.value.replace(SelectedValue,'');}}}}";


            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(EditGroupUC), key, script);
        }
        base.OnPreRender(e);
    }


    #region group methods



    /// <summary>
    /// Get the current group based on node selected in tree. 
    /// </summary>
    /// <returns></returns>
    private COEGroupBO GetGroup()
    {
        int groupID = -1;
        COEGroupBO businessObject = null;
        try
        {
            if ((Session["LastGroupNode"] != null) && (_groupOperation == GroupOperationType.Edit))
            {
                groupID = (int)Session["LastGroupNode"];
            }
            if (groupID > 0)
            {

                businessObject = COEGroupBO.Get(groupID);
            }
            else
            {
                businessObject = COEGroupBO.New();
            }

        }
        catch (System.Security.SecurityException)
        {
            //where to redirect?
            //Response.Redirect("Managegroups.aspx");
        }

        return (COEGroupBO)businessObject;
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
    /// Update the group with changes values and added/removed roles
    /// </summary>
    /// <param name="sender">the sender is the CSLA GroupDataSource</param>
    /// <param name="e">csla update object arguments which contain the values entered int he form</param>
    protected void GroupBODataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
    {
        ManageGroups manageGroupsControlshowbtn = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");
        manageGroupsControlshowbtn.ShowGroupEditBtn();
        COEGroupBO group = GetGroup();
        int origParentID = group.ParentGroupID;
        Csla.Data.DataMapper.Map(e.Values, group);
        string errMessage = string.Empty;
        string origName = group.GroupName;
        if (origName.Trim() == string.Empty)
        {
            errMessage = " - Please enter a valid Group Name<BR>";
        }
        if (errMessage != string.Empty)
        {
            ManageGroups manageGroupsControl = (ManageGroups)_masterPage.FindControlInPage("ManageGroupsControl");
            manageGroupsControl.SetGroupEditPanel(errMessage, "error");
            manageGroupsControlshowbtn.ShowGroupEditokBtn();
            return;
        }
        HiddenField privValuesField = (HiddenField)this.EditGroupDetailView.Controls[0].FindControl("EditHidden");
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
            manageGroupsControl.SetGroupEditPanel("Group updated sucessfully", "success");
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
            Session["LastGroupNode"] = group.GroupID;
            rowsAffected = 1;


        }


        catch (Csla.Validation.ValidationException ex)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            message.AppendFormat("{0}<br/>", ex.Message);
            if (group.BrokenRulesCollection.Count == 1)
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
            else
                foreach (Csla.Validation.BrokenRule rule in group.BrokenRulesCollection)
                    if (rule.Property == "LeaderPersonID")
                    {
                        message.AppendFormat("* {0}: {1}<br/>", rule.Property, "The LeaderPersonID required");
                    }
                    else
                    {
                        message.AppendFormat("* {0}: {1}<br/>", rule.Property, rule.Description);
                    }
            manageGroupsControl.SetGroupEditPanel(message.ToString(), "error");
            rowsAffected = 0;
        }
        catch (Csla.DataPortalException ex)
        {
            if (ex.BusinessException.Message.IndexOf("(COEDB.COEGROUP_AK)") > 0)
                manageGroupsControl.SetGroupEditPanel("A group named " + group.GroupName + " already exists in the system", "error");
            else
                manageGroupsControl.SetGroupEditPanel(ex.BusinessException.Message, "error");
            rowsAffected = 0;
        }
        catch (Exception ex)
        {
            manageGroupsControl.SetGroupEditPanel(ex.Message, "error");
            rowsAffected = 0;
        }

        return rowsAffected;
    }
    #endregion

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
    private COEGroupBO GetParentGroup(int groupID)
    {
        object coeGroup = null;
        if (coeGroup == null ||
          !(coeGroup is COEGroupBO))
        {
            try
            {
                if (groupID > 0)
                {
                    COEGroupBO myGroup = COEGroupBO.Get(groupID);
                    coeGroup = myGroup;
                    Session["CurrentGroup"] = coeGroup;
                    Session["GroupOrgID"] = myGroup.GroupOrgID;
                }
                else
                    coeGroup = COEGroupBO.New();
                Session["NewGroup"] = coeGroup;
            }
            catch (System.Security.SecurityException)
            {
            }
        }
        return (COEGroupBO)coeGroup;
    }
}
