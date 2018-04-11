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
using System.Xml;
using Csla.Data;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.WebUI.UltraWebToolbar;
using System.Text;

public partial class ManageGroups : System.Web.UI.UserControl
{
    #region variables
    private int _globalPersonID = 0;
    Forms_Master_MasterPage _masterPage = null;
    #endregion
    //*****************************Notes about page events
    //order of page events:
    //PreInit - use to check isPostBack, create dynamic controls, set master page , set theme, read/set profile values. viewstate has not been restored for controls so changes to controls might be wiped out
    //Init - raised after all controls are initialized. used to read or initialize control properties
    //InitComplete - all initialization for all controls is complete
    //PreLoad - use for processing 
    //Load - onLoad of page is called and onLoad for each control
    //Control Events - individaul control events are triggered
    //LoadComplete - all controls are loaded
    //databinding occurs here before prerender
    //PreRender - page calls ensurechildcontrols. each  databound control calls databind, final control and page changes can be made. Note databinding on control is called first.
    //SaveStateComplete -  viewstate saved- no changes to controls.
    //Render-not an event, the page calls this on each control
    //UnLoad- Response Stream is complete. No changes can be made. Use for cleanup only
    //******************************
    //The code in this page is organized based on the abovve page events ordering. It helps to undertand how it works. It is not totally straightforward as shown, but the general flow can be seen

    #region PreInit - use to check isPostBack, create dynamic controls, set master page , set theme, read/set profile values. viewstate has not been restored for controls so changes to controls might be wiped out

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["LastGroupNode"] = -1;
            Session["UserList"] = null;
            Session["GroupList"] = null;
            Session["CurrentGroup"] = null;
            Session["GroupOrgID"] = -1;
            Session["LeaderPersonID"] = null;
            Session["groupname"] = null;
            Session["RoleValue"] = null;
            Session["RoleTxt"] = null;
        }
        else
        {
            Session["CurrentGroup"] = null;
            Session["NewGroup"] = null;
        }

    }
    #endregion

    #region Init -raised after all controls are initialized. used to read or initialize control properties
    protected override void OnInit(EventArgs e)
    {
        _masterPage = (Forms_Master_MasterPage)this.Page.Master;

        //build the tree
        //this.BuildTree();

        //set tree events\
        this.GroupHeirarchyWebTree.NodeClicked += new Infragistics.WebUI.UltraWebNavigator.NodeClickedEventHandler(this.GroupHeirarchyWebTree_NodeClicked);
        //   this.GroupHeirarchyWebTree.PreRender += new System.EventHandler(this.GroupHeirarchyWebTree_PreRender);

        //initialize the user list grid
        this.UserListGrid.SelectedIndexChanged += new System.EventHandler(this.GridView1_SelectedIndexChanged);
        // this.UserListGrid.DataBinding += new  GridViewRowEventHandler(this.UserListGrid_Databinding); 
        //add triggers for UserListUpdatePanel. 
        System.Web.UI.AsyncPostBackTrigger NodeClickedTrigger = new System.Web.UI.AsyncPostBackTrigger();
        NodeClickedTrigger.EventName = "NodeClicked";
        NodeClickedTrigger.ControlID = GroupHeirarchyWebTree.UniqueID.ToString();
        UserListUpdatePanel.Triggers.Add(NodeClickedTrigger);

        //Add this trigger to all other update panels that need to be updated when a new groupID is set
        GroupAddUpdatePanel.Triggers.Add(NodeClickedTrigger);
        GroupEditUpdatePanel.Triggers.Add(NodeClickedTrigger);
        GroupDeleteUpdatePanel.Triggers.Add(NodeClickedTrigger);
        GroupMoveUpdatePanel.Triggers.Add(NodeClickedTrigger);

        base.OnInit(e);
    }



    #endregion

    #region Control- individual control events are triggered

    #region GroupHeirarcicalTree events

    ///VERY important!! In order to get the tree to update inside the MS update panel, I need to make adjustments to the infragistics
    /// ig_webtree.js file and reference it from the tree control (declarively).  with  out this change, you were unable to click a node
    /// that was modified or added. it is something with the styles getting screwed up.  The modfied file is in the security_manager/content/ directory    

    /// <summary>
    /// Called when a node in the tree is clicked. In particular this is the trigger for many of the update panels in the page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GroupHeirarchyWebTree_NodeClicked(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
    {

        //determine what group to bind. 

        int lastGroupID = (int)Session["LastGroupNode"];
        int currentGroupID = GetSelectedGroupID();
        Session["LastGroupNode"] = currentGroupID;
        int currentGroupOrgID = GetSelectedGroupOrgID(e);
        //set the _globalPersonID variable based on the clicked node
        SetPersonID();
        //if user clicked on a different group or user in a different group then rebind the grid
        if (lastGroupID != currentGroupID)
        {

            UpdateAllPanels(currentGroupID);
        }
        //if a user in the same group is clicked don't rebind the grid
        else
        {

            ResetActionPanels();

            //this means user is clicking around within a group;
            //set the other panels correctly
            UserDetailView.DataBind();
            UserDetailUpdatePanel.Update();

            ClearSelectedRow();
            SetSelectedRow();

        }

    }

    #endregion

    #region UserListGrid events


    /// <summary>
    /// event handlder for a row selection is change. This is a trigger for the user detail veiw panel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.GetSelectedRowCount() > 1)
        {
            //DoNothing for now. Not sure what to do in tree?
            UserDetailView.Visible = false;
            UserDetailWebPanel.BackColor = System.Drawing.Color.LightGray;

        }
        else
        {

            SetPersonID(e);
            SetSelectedRow();
            UserDetailView.DataBind();
            UserDetailUpdatePanel.Update();
            UserListUpdatePanel.Update();
            ResetActionPanels();
            //need to select treenode to stay in sync
            SetSelectedNodeToRow();

        }


    }



    /// <summary>
    /// change Image Group leader person
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserListGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton IB = (ImageButton)e.Row.FindControl("ImgButton");
            CheckBox CK = (CheckBox)e.Row.FindControl("chkSelect");
            //we may have to forgo using this because of tree update panel issues. 
            if (e.Row.Cells[7].Text.ToUpper() == "TRUE")
            {
                IB.ImageUrl = "/coecommonresources/infragistics/20111CLR20/Images/MSNExplorer/Person.gif";
                IB.ToolTip = "Leader (Please Click for More Details)";
                CK.Enabled = false;
            }
            else
            {
                IB.ImageUrl = "/coecommonresources/infragistics/20111CLR20/Images/MSNExplorer/People.gif";
                IB.ToolTip = "Member (Please Click for More Details)";
            }

        }
        e.Row.Cells[7].Visible = false;


    }
    /// <summary>
    /// onclick of Image button user details updated.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    protected void ImgButton_Click(object sender, ImageClickEventArgs e)
    {
        UserListGrid.SelectedIndex = ((GridViewRow)((ImageButton)sender).Parent.Parent).DataItemIndex;

        if (this.GetSelectedRowCount() > 1)
        {
            UserDetailView.Visible = false;
            UserDetailWebPanel.BackColor = System.Drawing.Color.LightGray;

        }
        else
        {
            //Coverity Fixes: CBOE-313 : CID-11784
            int personID;
            bool bValue = Int32.TryParse(UserListGrid.SelectedDataKey.Value.ToString(), out personID);
            if (bValue)
                _globalPersonID = personID;
            SetSelectedRow();
            UserDetailView.DataBind();
            UserDetailUpdatePanel.Update();
            UserListUpdatePanel.Update();
            ResetActionPanels();
            //need to select treenode to stay in sync
            // SetSelectedNodeToRow();

        }


    }
    #endregion

    #region Group Events

    //add group and edit group are handled  by the EditGroupUC control in the EditGroup.cs and EditGroup.ascx pages

    /// <summary>
    /// Move a group node to another group node
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void MoveGroupButton_Click(object sender, EventArgs e)
    {
        int groupID = GetSelectedGroupID();

        try
        {
            if (GroupMoveListBox.SelectedValue == "")
            {
                SetGroupMovePanel("Please select at least one Group", "error");
                return;
            }
            COEGroupBO group = GetGroup(groupID);
            int newParent = Convert.ToInt32(GroupMoveListBox.SelectedValue);
            group.ParentGroupID = (int)newParent;
            group = group.Save();
            Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
            SetGroupMovePanel("Group successfully moved", "success");

        }
        catch (Csla.DataPortalException ex)
        {
            if (ex.BusinessException.Message.IndexOf("Group parent cannot be itself or its children") > 0)
                SetGroupMovePanel("Group parent cannot be itself or its children", "error");
            else
                SetGroupMovePanel(ex.BusinessException.Message, "error");

        }
        catch (System.Exception ex)
        {
            SetGroupMovePanel(ex.Message, "error");
        }
        finally
        {
            GroupHeirarchyUpdatePanel.Update();
        }
    }

    /// <summary>
    /// Delete a group node and all associaed users
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DeleteGroupButton_Click(object sender, EventArgs e)
    {
        int groupID = GetSelectedGroupID();
        try
        {
            COEGroupBO.Delete(groupID);
            Session["LastGroupNode"] = GetParentSelectedGroupID();
            Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
            Session["CurrentGroup"] = null;
            Session["UserList"] = null;
            GroupHeirarchyUpdatePanel.Update();
            SetGroupDeletePanel("Group successfully deleted", "success");
            GroupHeirarchyUpdatePanel.Update();
            UserListUpdatePanel.Update();
        }

        catch (Csla.DataPortalException ex)
        {
            if (ex.BusinessException.Message.IndexOf("(COEDB.COEGROUP_PARENT_FK)") > 0)
                SetGroupDeletePanel("Please delete the child group first", "error");
            else
                SetGroupDeletePanel("Cannot delete group because existing Containers/Plates/Locations refer to it.", "error");
        }
        catch (System.Exception ex)
        {
            SetGroupDeletePanel(ex.Message, "error");
        }
    }
    #endregion

    #region User Events

    /// <summary>
    /// Move users from one group to another.  Can either duplicate the users in both group or remove them from the source.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void MoveUsersButton_Click(object sender, EventArgs e)
    {
        string newList = string.Empty;
        string errMessage = string.Empty;
        try
        {
            string personIDS = GetSelectedRows();
            if (personIDS.Length == 0)
            {
                errMessage = " - Please select at least one user<BR>";
            }
            if (UsersMoveListBox.SelectedValue == "")
            {
                errMessage += " - Please select at least one Group<BR>";
            }
            if (errMessage != string.Empty)
            {
                GroupHeirarchyUpdatePanel.Update();
                SetUsersMovePanel(errMessage, "error");
                return;
            }
            if (this.UsersMoveDuplicateCheckBox.Checked == false)
            {
                if (!CheckGroupLeader())
                {
                    GroupHeirarchyUpdatePanel.Update();
                    UserListGrid.DataBind();
                    UserListUpdatePanel.Update();
                    UserDetailUpdatePanel.Update();
                    SetUsersMovePanel("Group Leader cannot be moved until another Group Leader is assigned", "error");
                    return;
                }
            }

            int targetGroupID = Convert.ToInt32(UsersMoveListBox.SelectedValue);
            COEGroupBO targetGroup = GetGroup(targetGroupID);
            string targetUserList = targetGroup.UserListString;
            string[] myArrayTarget = targetUserList.Split(',');
            string targetList = string.Empty;
            string adduserID = string.Empty;
            List<GroupUser> duplicateusers = new List<GroupUser>();
            int countDuplicateUser = 0;
            if (targetUserList == string.Empty)
            {
                targetList = personIDS;
            }
            else
            {
                for (int i = 0; i < myArrayTarget.Length; i++)
                {
                    GroupUserList UL = targetGroup.UserList;
                    GroupUser GU1 = UL.GetUserByID(System.Convert.ToInt16(myArrayTarget[i]));
                    string[] myArrayPerson = personIDS.Split(',');
                    for (int j = 0; j < myArrayPerson.Length; j++)
                    {
                        if (GU1.PersonID == System.Convert.ToInt16(myArrayPerson[j]))
                        {
                            duplicateusers.Add(GU1);
                            countDuplicateUser++;
                            break;
                        }
                    }
                    targetList = targetUserList + "," + personIDS;
                }
            }

            if (countDuplicateUser >= 1)
            {
                targetGroup = COEGroupBO.SetGroupUsers(targetGroupID, targetList);
                string duplicateUserName = string.Empty;
                foreach (GroupUser duplicateuser in duplicateusers)
                {
                    personIDS = RemoveUserFromList(personIDS, duplicateuser.PersonID.ToString());
                }
                foreach (GroupUser duplicateuser in duplicateusers)
                {
                    duplicateUserName += duplicateuser.FirstName.ToString() + duplicateuser.LastName.ToString() + ",";
                }
                duplicateUserName = duplicateUserName.Trim(',');
                string[] adduserArray = personIDS.Split(',');
                string adduserName = string.Empty;
                for (int i = 0; i < adduserArray.Length; i++)
                {
                    COEGroupBO getGroup = GetGroup(targetGroupID);
                    GroupUserList userList = getGroup.UserList;
                    if (adduserArray[i] != "")
                    {
                        GroupUser groupUser = userList.GetUserByID(System.Convert.ToInt16(adduserArray[i]));
                        adduserName += groupUser.FirstName + groupUser.LastName + ",";
                        adduserID += groupUser.PersonID + ",";
                    }
                   //adduserName = adduserName.Trim(',');
                }
                adduserName = adduserName.Trim(',');
                MoveUserCheckDuplicate(adduserID);
                MoveUserCommonCode();
                if (adduserName.Length != 0)
                {

                    string adduser = SplitUserList(adduserName);
                    string duplicateUser = SplitUserList(duplicateUserName);
                    SetUsersMovePanel("Following user(s) Moved successfully :" + adduser + "</BR> " + " Following user(s) already exists in the group :" + duplicateUser, "error");

                }
                else
                {
                    string duplicateUser = SplitUserList(duplicateUserName);
                    SetUsersMovePanel(" Following user(s) already exists in the group :" + duplicateUser, "error");
                }
            }
            else
            {
                targetGroup = COEGroupBO.SetGroupUsers(targetGroupID, targetList);
                MoveUserCheckDuplicate(adduserID);
                Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
                MoveUserCommonCode();
                SetUsersMovePanel("User(s) moved successfully", "success");
            }
        }
        catch (Exception ex)
        {
            SetUsersMovePanel(ex.Message, "error");
        }
    }
    private string SplitUserList(string param)
    {
        StringBuilder str = new StringBuilder();
        try
        {
            string[] UserList = param.Split(',');
            int maxlimit = 35;
            int singleEntityLength = UserList[0].Length + 1;
            int ar = maxlimit / singleEntityLength;
            int arraySize = singleEntityLength * ar;
            String[] resultArray = new String[UserList.Length];

            string temp = string.Empty;
            int j = 0;
            bool continueloop = true;
            if (param.Length > maxlimit)
            {
                for (int i = 0; i < UserList.Length; i++)
                {
                    continueloop = false;
                    if ((temp + "," + UserList[i].ToString()).Length < maxlimit)
                    {
                        if (!string.IsNullOrEmpty(temp))
                            temp = temp + ",";
                        temp = temp + UserList[i].ToString();
                        continueloop = true;
                    }

                    if (!continueloop)
                    {
                        // resultArray[j] = temp + "<br/>";
                        if (!string.IsNullOrEmpty(temp))
                        {
                            str.Append(temp);
                            str.Append("<br/>");
                        }
                        temp = "";
                        j++;
                        i--;
                    }
                }
                if (!string.IsNullOrEmpty(temp)) resultArray[j] = temp;
            }
            else
            {
                str.Append(param);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return str.ToString();
    }
    public void MoveUserCheckDuplicate(string adduserID)
    {
        string personIDS = GetSelectedRows();
        int groupID = GetSelectedGroupID();
        COEGroupBO sourceGroup = GetGroup(groupID);
        string currentUserList = sourceGroup.UserListString;
        string[] myArray = personIDS.Split(',');
        string newSourceList = currentUserList;
        if (this.UsersMoveDuplicateCheckBox.Checked == false)
        {
            for (int i = 0; i < myArray.Length; i++)
            {
                newSourceList = RemoveUserFromList(newSourceList, myArray[i]);
            }
            sourceGroup = COEGroupBO.SetGroupUsers(groupID, newSourceList);
            Session["CurrentGroup"] = null;
        }
        else
        {
            string[] addUserArray = adduserID.Split(',');
            for (int j = 0; j < addUserArray.Length; j++)
            {
                newSourceList = RemoveUserFromList(newSourceList, addUserArray[j]);
            }
            sourceGroup = COEGroupBO.SetGroupUsers(groupID, newSourceList);
            Session["CurrentGroup"] = null;
        }

    }

    public void MoveUserCommonCode()
    {
        GroupHeirarchyUpdatePanel.Update();
        UserListGrid.DataBind();
        UserListUpdatePanel.Update();
        UserDetailUpdatePanel.Update();
    }

    protected void OkButtonOnCreateUser_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void OKButtonOnMoveUser_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void OkButtonOnRemoveUser_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }

    protected void OkButtonOnCreateGroup_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void OkButtonOnRemoveGroup_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void OkButtonOnMoveGroup_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void OkButtonOnEditGroup_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
    }
    protected void CancelButtonOnCreateGroup_onClick(object sender, EventArgs e)
    {
        ShowCloseButton();
        Session["LeaderPersonID"] = null;
        Session["groupname"] = null;
        Session["RoleValue"] = null;
        Session["RoleTxt"] = null;
    }

    public void ShowCloseButton()
    {
        ResetActionPanels();
        UserDetailView.DataBind();
        UserDetailUpdatePanel.Update();
        ClearSelectedRow();
        SetSelectedRow();
    }

    /// <summary>
    /// event handler when the remove users button is clicked in the userlistwebgrid tool bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RemoveUsersFromGroupButton_Click(object sender, EventArgs e)
    {
        try
        {
            int groupID = GetSelectedGroupID();
            string personIDS = GetSelectedRows();
            if (personIDS.Length == 0)
            {
                GroupHeirarchyUpdatePanel.Update();
                SetUsersRemovePanel("Please select at least one user", "error");
                RemoveUserOkButton.Visible = true;
                return;
            }
            if (!CheckGroupLeader())
            {
                GroupHeirarchyUpdatePanel.Update();
                UserListGrid.DataBind();
                UserListUpdatePanel.Update();
                UserDetailUpdatePanel.Update();
                SetUsersRemovePanel("Group Leader cannot be removed until another Group Leader is assigned", "error");
                return;
            }
            COEGroupBO group = GetGroup(groupID);
            string currentUserList = group.UserListString;
            string newList = currentUserList;
            string[] myArray = personIDS.Split(',');
            for (int i = 0; i < myArray.Length; i++)
            {

                newList = RemoveUserFromList(newList, myArray[i]);
            }
            group = COEGroupBO.SetGroupUsers(groupID, newList);
            //reget group so the object is updated properly with usrs
            Session["CurrentGroup"] = null;
            _globalPersonID = 0;//there are no users selected so this needs to be wiped out
            Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
            GroupHeirarchyUpdatePanel.Update();

            UserListGrid.DataBind();
            UserListUpdatePanel.Update();
            RemoveUserOkButton.Visible = true;
            SetUsersRemovePanel("User(s) successfully removed", "success");
        }
        catch (System.Exception ex)
        {

            SetUsersRemovePanel(ex.Message, "error");
        }
    }

    /// <summary>
    /// event handler when the add users button is clicked in the userlistwebgrid tool bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AddUsers_ButtonClick(object sender, EventArgs e)
    {
        CreateUserOkButton.Visible = true;
        ResetGroupAddPanel();
        int groupID = GetSelectedGroupID();
        try
        {
            COEGroupBO group = GetGroup(groupID);
            string currentUserList = group.UserListString;
            string[] myArraycurrentUserList = currentUserList.Split(',');
            string addUserList = GetIDListFromListBoxMultiSelect(UsersAddListBox);
            if (addUserList.Length == 0)
            {
                GroupHeirarchyUpdatePanel.Update();
                SetUsersAddPanel("Please select at least one user", "error");
                CreateUserOkButton.Visible = true;
                return;
            }
            string addedUserList = string.Empty;
            List<GroupUser> duplicateusers = new List<GroupUser>();
            int countDuplicateUser = 0;
            if (currentUserList.Length > 0)
            {
                for (int i = 0; i < myArraycurrentUserList.Length; i++)
                {
                    GroupUserList UL = group.UserList;
                    GroupUser GU1 = UL.GetUserByID(System.Convert.ToInt16(myArraycurrentUserList[i]));
                    string[] myArrayAddUserList = addUserList.Split(',');
                    for (int j = 0; j < myArrayAddUserList.Length; j++)
                    {
                        if (GU1.PersonID == System.Convert.ToInt16(myArrayAddUserList[j]))
                        {
                            duplicateusers.Add(GU1);
                            countDuplicateUser++;
                            break;
                        }
                    }
                }

                foreach (GroupUser duplicateuser in duplicateusers)
                {
                    addUserList = RemoveUserFromList(addUserList, duplicateuser.PersonID.ToString());
                }
            }
            if (countDuplicateUser >= 1)
            {
                string adduserName = string.Empty;
                if (!string.IsNullOrEmpty(addUserList))
                {
                    group = COEGroupBO.SetGroupUsers(groupID, addUserList + "," + currentUserList);
                    GroupHeirarchyUpdatePanel.Update();
                    UserListGrid.DataBind();
                    UsersAddUpdatePanel.Update();
                    string[] adduserArray = addUserList.Split(',');
                    for (int i = 0; i < adduserArray.Length; i++)
                    {
                        COEGroupBO getGroup = GetGroup(groupID);
                        GroupUserList userList = getGroup.UserList;
                        if (adduserArray[i] != "")
                        {
                            GroupUser groupUser = userList.GetUserByID(System.Convert.ToInt16(adduserArray[i]));
                            adduserName += groupUser.FirstName + groupUser.LastName + ",";
                        }

                    }
                    adduserName = adduserName.Trim(',');
                }
                string duplicateUserlist = string.Empty;
                foreach (GroupUser duplicateusername in duplicateusers)
                {
                    duplicateUserlist += duplicateusername.FirstName + duplicateusername.LastName + ",";
                }
                duplicateUserlist = duplicateUserlist.Trim(',');

                if (adduserName.Length != 0)
                {
                    string addeduser = SplitUserList(adduserName);
                    string duplicateUses = SplitUserList(duplicateUserlist);
                    SetUsersAddPanel("Following user(s) added successfully :" + addeduser + "</BR> " + " Following user(s) already exists in the group :" + duplicateUses, "error");
                   
                }
                else
                {

                    string duplicateUses = SplitUserList(duplicateUserlist);
                    SetUsersAddPanel(" Following user(s) already exists in the group :" + duplicateUses, "error");
                }
            }
            else
            {
                //CSBR:165020  Getting an error while adding user to Inventory Masters. 
                if (currentUserList.Length == 0)
                {
                    group = COEGroupBO.SetGroupUsers(groupID, addUserList);
                }
                else
                {
                    group = COEGroupBO.SetGroupUsers(groupID, addUserList + "," + currentUserList);
                }
                //force a reget of the group so the object is updated properly with new usrrs
                Session["CurrentGroup"] = null;
                Session["UserList"] = null;
                _globalPersonID = 0;//there are no users selected so this needs to be wiped out
                Session["GroupHeirarchy"] = GroupHeirarchy.GetXML();
                GroupHeirarchyUpdatePanel.Update();
                UserListGrid.DataBind();
                UserListUpdatePanel.Update();
                UsersAddListBox.Visible = false;
                UsersAddButton.Visible = false;
                CreateUserOkButton.Visible = false;
                UsersAddMessage.Text = "User(s) successfully added";
                UserLabel.Visible = false;
                CreateUserOkButton.Visible = false;
                CreateUserCloseButton.Visible = true;
                UsersAddUpdatePanel.Update();
            }
        }
        catch (System.Exception ex)
        {
            SetUsersAddPanel(ex.Message, "error");
        }
    }

    #endregion

    #endregion

    #region Databinding - called for each control

    #region UserDetailView

    /// <summary>
    /// databinding of csla userdetaildatasource to the userdetailview control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserDetailDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        if (_globalPersonID > 0)
        {
            UserDetailView.Visible = true;

            GroupUserList userList = (GroupUserList)Session["UserList"];
            if (userList != null && userList.Count > 0)
            {

                GroupUser user = userList.GetUserByID(_globalPersonID);
                if (user != null)
                {
                    UserDetailWebPanel.Text = "Details for user: " + user.UserID;
                    e.BusinessObject = user;
                }
                else
                {
                    UserDetailView.Visible = false;
                    UserDetailWebPanel.BackColor = System.Drawing.Color.LightGray;
                }

            }
        }
        else
        {
            UserDetailView.Visible = false;
            UserDetailWebPanel.BackColor = System.Drawing.Color.LightGray;
        }

    }
    #endregion

    #region UserListGrid
    /// <summary>
    /// databinding of csla UserListDataSource to the UserListGrid control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void UserListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        int groupID;
        bool selectUser = false;
        if (GroupHeirarchyWebTree.SelectedNode != null)
        {
            if (GroupHeirarchyWebTree.SelectedNode.ToolTip == "person")  //selected node is a user
            {
                selectUser = true;
                SetPersonID();
            }
            else
            {
                if (GroupHeirarchyWebTree.SelectedNode.ToolTip == "group")
                {
                    _globalPersonID = 0;
                    UserDetailView.Visible = false;
                }
            }

            //get the list of users in the group 
            groupID = GetSelectedGroupID();
            COEGroupBO group = GetGroup(groupID);
            this.UserListWebPanel.Text = "Users in group:" + group.GroupName;
            Session["UserList"] = group.UserList;
            e.BusinessObject = (GroupUserList)Session["UserList"];

            int rowCount = UserListGrid.Rows.Count;

            //set the selected row to the user clicked in the tree
            if (selectUser == true)
            {
                SetSelectedRow();
            }
        }
    }

    #endregion

    #region GroupMoveListBox and UserMoveListBox in YUI panel
    /// <summary>
    /// databinding of csla GroupListDataSource to the GroupListBox control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GroupListDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        int currentGroupID = (int)Session["LastGroupNode"];
        if (Session["GroupOrgID"] != null)
        {
            if ((int)Session["GroupOrgID"] > 0)
            {

                e.BusinessObject = GroupList.GetList((int)Session["GroupOrgID"], currentGroupID);
            }
        }

    }

    #endregion

    #region PersonListBoxControl for addusers YUI panel

    /// <summary>
    /// databinding of csla DBMSUserList to the PersonListBox control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DBMSUserList_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
    {
        PersonList userList = PersonList.GetCOEList();

        //Coverity Fixes: CBOE-313 : CID-11857
        if (userList != null && userList.Count > 0)
        {
            userList.Remove(0);
            e.BusinessObject = userList;
        }
    }

    #endregion

    #endregion

    #region PreRender -page calls ensurechildcontrols. each  databound control calls databind, final control and page changes can be made. Note databinding on control is called first.


    /// <summary>
    /// add client script, add style sheets etc
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        //add scripts at prerender level so they are added to a point in the page that can be globally accessed (outside of <body> tag

        //Load required js for YUI to work
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("yuiloader-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("yuiloader-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "yuiloader-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("dom-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("dom-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "dom-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("event-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("event-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "event-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("dragdrop-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("dragdrop-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "dragdrop-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("container-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("container-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "container-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("element-beta-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("element-beta-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "element-beta-min.js"));
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("button-min"))
            this.Page.ClientScript.RegisterClientScriptInclude("button-min", ResolveClientUrl(Utilities.YUIJsRelativeFolder() + "button-min.js"));

        //Load required css for YUI.  This style sheet has been modified from the standar container.css so that the panel appears int the correct color.
        //for other themes, a duplicate of this modified style sheet should be added and the color change.
        HtmlLink yuiContainer = new HtmlLink();
        yuiContainer.Attributes.Add("rel", "stylesheet");
        yuiContainer.Href = Utilities.YUICssRelativeFolder(this.Page.StyleSheetTheme) + "container2.css";
        yuiContainer.Attributes.Add("type", "text/css");
        this.Page.Header.Controls.Add(yuiContainer);

        //Add style to the body to know how to handle the background color change.
        ((HtmlGenericControl)this.Page.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes.Add("class", "yui-skin-sam");

        //creat the javascript to show the YUI panel for each button in the button bars for groups and group users.
        string key = "GroupPanel";
        if (!this.Page.ClientScript.IsClientScriptBlockRegistered(key))
        {   //AddGroupPanel
            string script = "<script type=\"text/javascript\">";
            script += "YAHOO.namespace(\"coemanager.security\");";
            script += "function init() {";

            //AddGroupPanel
            script += "YAHOO.coemanager.security.AddGroupPanel = new YAHOO.widget.Panel(\"addGroupPanel\",";
            script += "{width:\"600px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.AddGroupPanel.render();";

            //EditGroupPanel
            script += "YAHOO.coemanager.security.EditGroupPanel = new YAHOO.widget.Panel(\"editGroupPanel\",";
            script += "{width:\"600px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.EditGroupPanel.render();";

            //MoveGroupPanel
            script += "YAHOO.coemanager.security.MoveGroupPanel = new YAHOO.widget.Panel(\"moveGroupPanel\",";
            script += "{width:\"277px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.MoveGroupPanel.render();";

            //DeleteGroupPanel
            script += "YAHOO.coemanager.security.DeleteGroupPanel = new YAHOO.widget.Panel(\"deleteGroupPanel\",";
            script += "{width:\"400px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.DeleteGroupPanel.render();";

            //AddUsersPanel
            script += "YAHOO.coemanager.security.AddUsersPanel = new YAHOO.widget.Panel(\"addUsersPanel\",";
            script += "{width:\"400px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.AddUsersPanel.render();";

            //RemoveUsersPanel
            script += "YAHOO.coemanager.security.RemoveUsersPanel = new YAHOO.widget.Panel(\"removeUsersPanel\",";
            script += "{width:\"400px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.RemoveUsersPanel.render();";

            //MoveUsersPanel
            script += "YAHOO.coemanager.security.MoveUsersPanel = new YAHOO.widget.Panel(\"moveUsersPanel\",";
            script += "{width:\"400px\",fixedcenter:true,visible:false,draggable:true,close:false,modal:true,constraintoviewport:true});";
            script += "YAHOO.coemanager.security.MoveUsersPanel.render();}";


            script += "YAHOO.util.Event.addListener(window, \"load\", init);";
            script += "function showDeleteGroupPanel(){";
            script += "YAHOO.coemanager.security.DeleteGroupPanel.show();}";
            script += "function hideDeleteGroupPanel(){";
            script += "YAHOO.coemanager.security.DeleteGroupPanel.hide();}";
            script += "function showMoveGroupPanel(){";
            script += "YAHOO.coemanager.security.MoveGroupPanel.show();}";
            script += "function hideMoveGroupPanel(){";
            script += "YAHOO.coemanager.security.MoveGroupPanel.hide();}";
            script += "function showAddGroupPanel(){";
            script += "YAHOO.coemanager.security.EditGroupPanel.show();";
            script += "YAHOO.coemanager.security.EditGroupPanel.hide();";
            script += "YAHOO.coemanager.security.AddGroupPanel.show();}";
            script += "function hideAddGroupPanel(){";
            script += "YAHOO.coemanager.security.AddGroupPanel.hide();}";
            script += "function ShowOkAddGroupPanel(){";
            script += "YAHOO.coemanager.security.AddGroupPanel.show();}";

            script += "function showEditGroupPanel(){";
            script += "YAHOO.coemanager.security.EditGroupPanel.show();}";
            script += "function ShowOkEditGroupPanel(){";
            script += "YAHOO.coemanager.security.EditGroupPanel.show();}";
            script += "function hideEditGroupPanel(){";
            script += "YAHOO.coemanager.security.EditGroupPanel.hide();}";
            script += "function showAddUsersPanel(){";
            script += "YAHOO.coemanager.security.AddUsersPanel.show();}";
            script += "function hideAddUsersPanel(){";
            script += "YAHOO.coemanager.security.AddUsersPanel.hide();}";
            script += "function ReopenAddUsersPanel(){";
            script += "YAHOO.coemanager.security.AddUsersPanel.show();}";
            script += "function showRemoveUsersPanel(){";
            script += "YAHOO.coemanager.security.RemoveUsersPanel.show();}";
            script += "function hideRemoveUsersPanel(){";
            script += "YAHOO.coemanager.security.RemoveUsersPanel.hide();}";
            script += "function showMoveUsersPanel(){";
            script += "YAHOO.coemanager.security.MoveUsersPanel.show();}";
            script += "function ReopenMoveUsersPanel(){";
            script += "YAHOO.coemanager.security.MoveUsersPanel.show();}";
            script += "function hideMoveUsersPanel(){";
            script += "YAHOO.coemanager.security.MoveUsersPanel.hide();}";

            script += "</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(ManageGroups), key, script);
        }
        BuildTree();
        base.OnPreRender(e);
    }

    #endregion

    #region  supporting methods

    #region update panels




    private void EnableUserListButtons()
    {

        UserListToolbar.Enabled = true;
        // UserListToolbar.Attributes.Add("disabled", "false");
        //TBarButton userMoveButton = (TBarButton)UserListToolbar.Items.FromKey("UsersMove");
        //userMoveButton.Enabled = true;

        //userMoveButton.TargetURL = "javascript:showMoveUsersPanel()";
        //TBarButton userRemoveButton = (TBarButton)UserListToolbar.Items.FromKey("UsersRemove");
        //userRemoveButton.TargetURL = "javascript:showRemoveUsersPanel()";
        //userRemoveButton.Enabled = true;
        //TBarButton userAddButton = (TBarButton)UserListToolbar.Items.FromKey("UsersAdd");
        //userAddButton.TargetURL = "javascript:alert('here');showAddUsersPanel()";
        //userAddButton.Enabled = true;

    }

    private void DisableUserListButtons()
    {

        //UserListToolbar.Enabled = false;
        //TBarButton userMoveButton = (TBarButton)UserListToolbar.Items.FromKey("UsersMove");
        //userMoveButton.Enabled = false;
        //userMoveButton.TargetURL = "javascript:showMoveUsersPanel()";
        //TBarButton userRemoveButton = (TBarButton)UserListToolbar.Items.FromKey("UsersRemove");
        //userRemoveButton.TargetURL = "javascript:showRemoveUsersPanel()";
        //userRemoveButton.Enabled = false;
        //TBarButton userAddButton = (TBarButton)UserListToolbar.Items.FromKey("UsersAdd");
        //userAddButton.TargetURL = "javascript:alert('here');showAddUsersPanel()";
        //userAddButton.Enabled = false;


    }


    //overload
    private void UpdateAllPanelsNotAdd(int currentGroupID)
    {
        UserListGrid.DataBind();
        UserListUpdatePanel.Update();
        UserDetailView.DataBind();
        UserDetailUpdatePanel.Update();
        //set selected row in userlistcontrol
        ClearSelectedRow();
        SetSelectedRow();
        Session["LastGroupNode"] = currentGroupID;
        ResetActionPanelsNotAdd();

    }

    //overload
    private void UpdateAllPanels(int currentGroupID)
    {
        UserListGrid.DataBind();
        UserListUpdatePanel.Update();
        UserDetailView.DataBind();
        UserDetailUpdatePanel.Update();
        //set selected row in userlistcontrol
        ClearSelectedRow();
        SetSelectedRow();
        Session["LastGroupNode"] = currentGroupID;


        ResetActionPanels();

    }


    //resset group and user action panels

    private void ResetActionPanels()
    {
        //YUI Group panels
        ResetGroupMovePanel();
        ResetGroupDeletePanel();
        ResetGroupEditPanel();
        ResetGroupAddPanel();

        //YUI User panels
        ResetUsersAddPanel();
        ResetUsersMovePanel();
        ResetUsersRemovePanel();
    }

    private void ResetActionPanelsNotAdd()
    {
        //YUI Group panels
        ResetGroupMovePanel();
        ResetGroupDeletePanel();
        ResetGroupEditPanel();
        // ResetGroupAddPanel();

        //YUI User panels
        ResetUsersAddPanel();
        ResetUsersMovePanel();
        ResetUsersRemovePanel();
    }

    //move group panel
    private void SetGroupMovePanel(string message, string messageType)
    {

        this.GroupMoveButton.Visible = false;
        this.GroupMoveListBox.Visible = false;
        this.GroupMoveInstructionMessage.Visible = false;
        this.GroupMoveMessage.Text = message;
        if (messageType.ToUpper() == "SUCCESS")
        {
            this.OkMoveGroup.Visible = false;
            this.OkMoveGroupClose.Visible = true;
        }
        else
        {
            this.OkMoveGroup.Visible = true;
            this.OkMoveGroupClose.Visible = false;
            this.GroupMoveBackButton.Visible = true;
        }
        this.GroupAddUpdatePanel.Update();
        this.GroupMoveUpdatePanel.Update();
    }

    private void ResetGroupMovePanel()
    {
        GroupMoveListBox.DataBind();
        //GroupMoveListBox.Items();
        this.GroupMoveButton.Visible = true;
        this.OkMoveGroup.Visible = true;
        this.OkMoveGroupClose.Visible = false;
        this.GroupMoveInstructionMessage.Visible = true;
        this.GroupMoveListBox.Visible = true;
        this.GroupMoveMessage.Text = "";
        GroupMoveUpdatePanel.Update();
    }

    //delete group panel
    private void SetGroupDeletePanel(string message, string messageType)
    {
        this.GroupDeleteButton.Visible = false;
        if (messageType.ToUpper() == "SUCCESS")
        {
            this.OkDeleteGroup.Visible = false;
            this.GroupDeleteButtonClose.Visible = true;
        }
        else
        {
            this.OkDeleteGroup.Visible = true;
            this.GroupDeleteButtonClose.Visible = false;
        }
        this.GroupDeleteLabel.Visible = false;
        this.GroupDeleteMessage.Text = message;
        this.GroupDeleteUpdatePanel.Update();
    }

    private void ResetGroupDeletePanel()
    {

        this.GroupDeleteButton.Visible = true;
        this.GroupDeleteButtonClose.Visible = false;
        this.GroupDeleteLabel.Visible = true;
        this.OkDeleteGroup.Visible = true;
        this.GroupDeleteMessage.Text = "";
        GroupDeleteUpdatePanel.Update();
    }

    //add group panel
    internal void SetGroupAddPanel(string message, string messageType)
    {
        this.GroupAddControl.Visible = false;
        OkBtnAddgroup.Visible = false;
        this.GroupAddMessage.Text = message;
        if (messageType.ToUpper() == "SUCCESS")
        {
            this.AddGroupOk.Visible = false;
            this.AddGroupClose.Visible = true;
        }
        else
        {
            this.AddGroupOk.Visible = true;
            this.AddGroupClose.Visible = false;
        }
        this.GroupAddUpdatePanel.Update();
    }
    internal void ShowGroupAddBtn()
    {
        AddGroupOk.Visible = true;
        this.AddGroupClose.Visible = false;
    }
    internal void ShowGroupEditBtn()
    {
        EditGroupOk.Visible = true;
        EditGroupClose.Visible = false;
    }
    internal void ShowGroupAddokBtn()
    {
        OkBtnAddgroup.Visible = true;
    }
    internal void hideGroupAddokBtn()
    {
        OkBtnAddgroup.Visible = false;
    }
    internal void ShowGroupEditokBtn()
    {
        OkButtonEditGroup.Visible = true;
    }
    internal void hideGroupEditokBtn()
    {
        OkButtonEditGroup.Visible = false;
    }
    private void ResetGroupAddPanel()
    {
        this.OkButtonEditGroup.Visible = false;
        this.OkBtnAddgroup.Visible = false;
        this.AddGroupOk.Visible = false;
        this.AddGroupClose.Visible = false;
        this.EditGroupOk.Visible = false;
        this.EditGroupClose.Visible = false;
        this.GroupMoveBackButton.Visible = false;
        GroupAddControl.DataBind();
        this.GroupAddControl.Visible = true;
        this.GroupAddMessage.Text = "";
        GroupAddUpdatePanel.Update();
    }

    //add edit panel
    internal void SetGroupEditPanel(string message, string messageType)
    {
        this.GroupEditControl.Visible = false;
        this.GroupEditMessage.Text = message;
        if (messageType.ToUpper() == "SUCCESS")
        {
            EditGroupOk.Visible = false;
            EditGroupClose.Visible = true;
        }
        else
        {
            EditGroupOk.Visible = true;
            EditGroupClose.Visible = false;
        }
        this.GroupEditUpdatePanel.Update();
        this.UserListGrid.DataBind();
        this.UserListUpdatePanel.Update();
    }

    private void ResetGroupEditPanel()
    {

        GroupEditControl.DataBind();
        this.GroupEditControl.Visible = true;
        this.GroupEditMessage.Text = "";
        GroupEditUpdatePanel.Update();
    }

    //user add panel

    private void SetUsersAddPanel(string message, string messageType)
    {
        UsersAddListBox.Visible = false;
        UsersAddButton.Visible = false;
        UserLabel.Visible = false;
        CreateUserOkButton.Visible = false;
        OkReopen.Visible = true;
        if (messageType.ToUpper() == "SUCCESS")
        {
            CreateUserCloseButton.Visible = true;
            CreateUserOkButton.Visible = false;
        }
        else
        {
            CreateUserCloseButton.Visible = false;
            CreateUserOkButton.Visible = true;
        }
        UsersAddMessage.Text = message;
        UsersAddUpdatePanel.Update();
    }

    private void ResetUsersAddPanel()
    {
        UsersAddListBox.DataBind();
        UsersAddListBox.Visible = true;
        UsersAddButton.Visible = true;
        UserLabel.Visible = true;
        CreateUserCloseButton.Visible = false;
        CreateUserOkButton.Visible = true;
        CreateUserOkButton.Visible = true;
        RemoveUserOkButton.Visible = true;
        MoveUserOkButton.Visible = true;
        OkReopen.Visible = false;
        OkReopenMovePanel.Visible = false;
        UsersAddMessage.Visible = true;
        UsersAddMessage.Text = "";
        UsersAddUpdatePanel.Update();
    }

    //user move panel
    private void SetUsersMovePanel(string message, string messageType)
    {
        UsersMoveListBox.Visible = false;
        UserMoveLabel.Visible = false;
        UsersMoveButton.Visible = false;
        UsersMoveDuplicateCheckBox.Visible = false;
        UsersMoveMessage.Visible = true;
        OkReopenMovePanel.Visible = true;
        if (messageType.ToUpper() == "SUCCESS")
        {
            MoveUserCloseButton.Visible = true;
            MoveUserOkButton.Visible = false;
        }
        else
        {
            MoveUserCloseButton.Visible = false;
            MoveUserOkButton.Visible = true;
        }
        UsersMoveMessage.Text = message;
        UsersMoveUpdatePanel.Update();
    }

    private void ResetUsersMovePanel()
    {
        UsersMoveListBox.DataBind();
        UsersMoveListBox.Visible = true;
        UserMoveLabel.Visible = true;
        UsersMoveButton.Visible = true;
        MoveUserCloseButton.Visible = false;
        MoveUserOkButton.Visible = true;
        UsersMoveMessage.Visible = true;
        UsersMoveDuplicateCheckBox.Visible = true;
        UsersMoveMessage.Text = "";
        UsersMoveDuplicateCheckBox.Checked = false;
        UsersMoveUpdatePanel.Update();

    }

    //user remove panel

    private void SetUsersRemovePanel(string message, string messageType)
    {
        UsersRemoveLabel.Visible = false;
        UsersRemoveButton.Visible = false;
        //Remove.Visible = false;
        UsersRemoveMessage.Visible = true;
        if (messageType.ToUpper() == "SUCCESS")
        {
            RemoveUserOkButton.Visible = false;
            RemoveUserCloseButton.Visible = true;
        }
        else
        {
            RemoveUserOkButton.Visible = true;
            RemoveUserCloseButton.Visible = false;
        }
        UsersRemoveMessage.Text = message;
        UsersRemoveUpdatePanel.Update();
    }

    private void ResetUsersRemovePanel()
    {

        UsersRemoveLabel.Visible = true;
        UsersRemoveButton.Visible = true;
        RemoveUserOkButton.Visible = true;
        RemoveUserCloseButton.Visible = false;
        UsersRemoveMessage.Visible = false;
        UsersRemoveMessage.Text = "";
        UsersRemoveUpdatePanel.Update();
    }
    #endregion
    #region  general
    /// <summary>
    /// get the _globalPersonId using the the UserListGrid as source
    /// </summary>
    /// <param name="e"></param>
    private void SetPersonID(EventArgs e)
    {
        //Coverity Fixes: CBOE-313 : CID-11785
        int personID;
        bool bValue = Int32.TryParse(UserListGrid.SelectedDataKey.Value.ToString(), out personID);
        if (bValue)
            _globalPersonID = personID;
    }

    /// <summary>
    /// get the _globalPersonId using the the GroupHeirarchyWebTree as source
    /// </summary>
    /// <param name="e"></param>
    private void SetPersonID()
    {
        if (GroupHeirarchyWebTree.SelectedNode.Nodes.Count == 0)  //means selected node is a user
        {

            _globalPersonID = System.Convert.ToInt32(GroupHeirarchyWebTree.SelectedNode.Tag.ToString());

        }
        else   //selected node is a  group
        {
            _globalPersonID = -1;

        }


    }

    /// <summary>
    /// get the groupID using the the GroupHeirarchyWebTree as source
    /// </summary>
    /// <param name="e"></param>
    private int GetSelectedGroupID()
    {
        int groupID = -1;
        switch (GroupHeirarchyWebTree.SelectedNode.ToolTip)
        {
            case "person":
                groupID = System.Convert.ToInt16(GroupHeirarchyWebTree.SelectedNode.Parent.Tag.ToString());

                break;
            case "group":
                groupID = System.Convert.ToInt16(GroupHeirarchyWebTree.SelectedNode.Tag.ToString());
                break;

        }
        //store the selected node for use in deselection when node changes.


        return groupID;
    }



    /// <summary>
    /// get the groupID using the the GroupHeirarchyWebTree as source
    /// </summary>
    /// <param name="e"></param>
    private int GetParentSelectedGroupID()
    {
        int parentID = -1;
        parentID = System.Convert.ToInt16(GroupHeirarchyWebTree.SelectedNode.Parent.Tag.ToString());
        return parentID;
    }

    /// <summary>
    /// get the groupOrgID using the the GroupHeirarchyWebTree as source
    /// </summary>
    /// <param name="e"></param>
    private int GetSelectedGroupOrgID(Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
    {
        int groupOrgID = -1;
        Infragistics.WebUI.UltraWebNavigator.Node myNode = e.Node;
        if (myNode.Parent != null)
        {
            while (myNode.Parent.Level != 0)
            {
                myNode = myNode.Parent;
                groupOrgID = System.Convert.ToInt32(myNode.Tag);
            }
            Session["GroupOrgID"] = groupOrgID;
        }

        return groupOrgID;
    }

    /// <summary>
    /// get a comma delimited list of ids from a list box
    /// </summary>
    /// <param name="listBox"></param>
    /// <returns></returns>
    private string GetIDListFromListBoxMultiSelect(ListBox listBox)
    {
        string newList = string.Empty;
        for (int i = 0; i < listBox.Items.Count; i++)
        {
            if (listBox.Items[i].Selected == true)
            {
                ;
                if (newList.Length == 0)
                {
                    newList = listBox.Items[i].Value;
                }
                else
                {
                    newList = newList + "," + listBox.Items[i].Value;
                }
            }
        }
        return newList;
    }


    /// <summary>
    /// remove an id from a comma delimeted list of ids
    /// </summary>
    /// <param name="e"></param>
    private string RemoveUserFromList(string currentList, string personID)
    {
        string[] currentListArray = currentList.Split(',');
        string newList = string.Empty;
        for (int i = 0; i < currentListArray.Length; i++)
        {
            if (currentListArray[i] != personID)
            {
                if (newList.Length > 0)
                {
                    newList = newList + "," + currentListArray[i];
                }
                else
                {
                    newList = currentListArray[i];
                }
            }
        }
        return newList;
    }

    /// <summary>
    /// check user is group leader in userlist.
    /// </summary>

    protected bool CheckGroupLeader()
    {
        string personIDS = GetSelectedRows();
        int groupID = GetSelectedGroupID();
        COEGroupBO sourceGroup = GetGroup(groupID);
        string currentUserList = sourceGroup.UserListString;
        GroupUserList UL = sourceGroup.UserList;
        string[] myArray = personIDS.Split(',');
        for (int i = 0; i < myArray.Length; i++)
        {
            GroupUser GU = UL.GetUserByID(System.Convert.ToInt16(myArray[i]));
            if (GU.isleader)
            {
                return false;
            }

        }
        return true;
    }



    #endregion

    #region tree methods

    /// <summary>
    /// Get the groupheirary and set the datasource for the GroupHeirarcyTree
    /// </summary>
    protected void BuildTree()
    {
        XmlDocument xmlDoc = null;
        GroupHeirarchy groupHeirarchy = new GroupHeirarchy();
        if ((XmlDocument)Session["GroupHeirarchy"] == null)
        {
            xmlDoc = GroupHeirarchy.GetXML();
            Session["GroupHeirarchy"] = xmlDoc;
        }
        else
        {
            xmlDoc = (XmlDocument)Session["GroupHeirarchy"];
        }

        XmlDataSource dataSource = new XmlDataSource();
        dataSource.Data = xmlDoc.OuterXml;
        dataSource.EnableCaching = false;
        // GroupHeirarchyWebTree.Nodes.Clear();
        GroupHeirarchyWebTree.DataSource = dataSource;
        GroupHeirarchyWebTree.DataBind();
        GroupHeirarchyWebTree.InitialExpandDepth = 2;
        GroupHeirarchyWebTree.ExpandAnimation = ExpandAnimation.None;
        if (Session["NewGroupNode"] != null && (int)Session["NewGroupNode"] > 0)
        {

            foreach (Infragistics.WebUI.UltraWebNavigator.Node node in GroupHeirarchyWebTree)
            {
                if (node.Tag.ToString() == Session["NewGroupNode"].ToString())
                {
                    node.Selected = true;
                    GroupHeirarchyWebTree.SelectedNode = node;
                }
            }
            UpdateAllPanelsNotAdd((int)Session["NewGroupNode"]);
            Session["NewGroupNode"] = null;

        }
        else
        {
            if (Session["LastGroupNode"] != null && (int)Session["LastGroupNode"] > 0)
            {
                foreach (Infragistics.WebUI.UltraWebNavigator.Node node in GroupHeirarchyWebTree)
                {
                    if (node.Tag.ToString() == Session["LastGroupNode"].ToString())
                    {
                        node.Selected = true;
                        GroupHeirarchyWebTree.SelectedNode = node;
                    }
                }
            }
        }
        if (_globalPersonID == 0)
        {
            DisableUserListButtons();
        }
    }


    /// <summary>
    ///  select the node in the tree that corresponds to a selected row
    /// </summary>
    private void SetSelectedNodeToRow()
    {

        foreach (Infragistics.WebUI.UltraWebNavigator.Node node in this.GroupHeirarchyWebTree.Nodes)
        {
            node.Selected = false;
        }
        Infragistics.WebUI.UltraWebNavigator.Node node2 = this.GroupHeirarchyWebTree.Find(_globalPersonID.ToString());
        if (node2 != null)
        {
            node2.Selected = true;
        }


    }
    #endregion

    #region grid methods

    /// <summary>
    /// Get the selected row in the UltraListGrid
    /// </summary>
    private void SetSelectedRow()
    {

        if (_globalPersonID > 0)
        {
            //GridView1.SelectedIndex != -1
            //Coverity Fixes : CBOE-313 : CID-11786
            DataKeyArray userListGridDataKeys = this.UserListGrid.DataKeys;
            if (userListGridDataKeys != null)
            {
                for (int i = 0; i < userListGridDataKeys.Count; i++)
                {
                    //the only way you can see the datakey value for a row is to look at the datakeys. 
                    if (Convert.ToString(userListGridDataKeys[i].Value) == _globalPersonID.ToString())
                    {
                        //select the row with the same index as the datakay.
                        this.UserListGrid.Rows[i].RowState = DataControlRowState.Selected;
                    }
                    else
                    {
                        this.UserListGrid.Rows[i].RowState = DataControlRowState.Normal;
                    }
                }
                UserDetailView.Visible = true;
                UserDetailView.DataSourceID = "UserDetailDataSource";
                UserDetailView.DataBind();
                EnableUserListButtons();
            }
        }
        else
        {
            DisableUserListButtons();
            UserDetailView.Visible = false;
            UserDetailWebPanel.BackColor = System.Drawing.Color.LightGray;
        }

    }

    /// <summary>
    /// Clear selected row in UserListGrid
    /// </summary>
    private void ClearSelectedRow()
    {
        for (int i = 0; i < this.UserListGrid.DataKeys.Count; i++)
        {
            this.UserListGrid.Rows[i].RowState = DataControlRowState.Normal;

        }
    }

    /// <summary>
    /// get number of rows selected in UserListGrid
    /// </summary>
    /// <returns></returns>
    private int GetSelectedRowCount()
    {
        int count = 0;
        for (int i = 0; i < this.UserListGrid.Rows.Count; i++)
        {

            //the only way you can see the datakey value for a row is to look at the datakeys. 
            if (this.UserListGrid.Rows[i].RowState == DataControlRowState.Selected)
            {
                count = count + 1;
            }
        }
        return count;
    }


    /// <summary>
    /// get number of rows selected in UserListGrid
    /// </summary>
    /// <returns></returns>
    private string GetSelectedRows()
    {
        string personIDs = string.Empty;
        foreach (GridViewRow gvr in this.UserListGrid.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkSelect")).Checked == true)
            {
                if (personIDs == string.Empty)
                {
                    personIDs = ((HiddenField)gvr.FindControl("personIDHidden")).Value;
                }
                else
                {
                    personIDs = personIDs + "," + ((HiddenField)gvr.FindControl("personIDHidden")).Value;
                }
            }
        }
        return personIDs;
    }





    #endregion

    #region group business object methods



    /// <summary>
    /// Get the group BO for the selected group
    /// </summary>
    /// <param name="groupID"></param>
    /// <returns></returns>
    private COEGroupBO GetGroup(int groupID)
    {
        object businessObject = null;// Session["CurrentGroup"];
        if (businessObject == null ||
          !(businessObject is COEGroupBO))
        {
            try
            {

                if (groupID > 0)
                {
                    COEGroupBO myGroup = COEGroupBO.Get(groupID);
                    businessObject = myGroup;
                    Session["CurrentGroup"] = businessObject;
                    Session["GroupOrgID"] = myGroup.GroupOrgID;

                }
                else
                    businessObject = COEGroupBO.New();
                Session["NewGroup"] = businessObject;
            }
            catch (System.Security.SecurityException)
            {
                //where to redirect?
                //Response.Redirect("Managegroups.aspx");
            }
        }
        return (COEGroupBO)businessObject;
    }

    #endregion
    #endregion





}


