<%@ Control Language="C#" AutoEventWireup="True"   Inherits="ManageGroups" Codebehind="ManageGroups.ascx.cs" %> 
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register TagPrefix="igmisc" Namespace="Infragistics.WebUI.Misc" Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

 <%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Src="../UserControls/EditGroup.ascx" TagName="EditGroupUC" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/AddGroup.ascx" TagName="AddGroupUC" TagPrefix="uc2" %>

<script type="text/javascript">
var groupToolBarID;
var userListToolBarID;
var groupWebTreeID;
var userWebGridID;
var TotalChkBx;
var Counter;

function ToggleSelectAll(CheckBox)
{   
   //Get target base & child control.
   var TargetBaseControl = 
       document.getElementById('<%= this.UserListGrid.ClientID %>');
   var TargetChildControl = "chkSelect";

   //Get all the control of the type INPUT in the base control.
   var Inputs = TargetBaseControl.getElementsByTagName("input");

   //Checked/Unchecked all the checkBoxes in side the GridView.
   for(var n = 0; n < Inputs.length; ++n)
      if(Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0 && !Inputs[n].disabled)
         Inputs[n].checked = CheckBox.checked;

   //Reset Counter
   Counter = CheckBox.checked ? TotalChkBx : 0;
}

function ResetHeaderCheckBox(CheckBox)
{
   var TargetBaseControl = 
       document.getElementById('<%= this.UserListGrid.ClientID %>');
   var TargetChildControl = "chkBxHeader";
   //Get all the control of the type INPUT in the base control.
   var Inputs = TargetBaseControl.getElementsByTagName("input");

   //Checked/Unchecked all the checkBoxes in side the GridView.
   for(var n = 0; n < Inputs.length; ++n)
      if(Inputs[n].type == 'checkbox' && 
                Inputs[n].id.indexOf(TargetChildControl,0) >= 0)
         Inputs[n].checked = false;
}

function EnableMoveUserBtn(CheckBox) {
    var TargetBaseControl =
        document.getElementById('<%= this.UserListGrid.ClientID %>');
    var TargetChildControl = "chkBxHeader";
    //Get all the control of the type INPUT in the base control.
    var Inputs = TargetBaseControl.getElementsByTagName("input");
    var utb = igtbar_getToolbarById(userListToolBarID);
    var item5 = utb.Items.fromKey("UsersMove");
    var item7 = utb.Items.fromKey("UsersRemove");
    if (CheckBox.checked) {
        item5.setEnabled(true);
        item7.setEnabled(true);
        return;
    }
    //Checked/Unchecked all the checkBoxes in side the GridView.
    for (var n = 0; n < Inputs.length; ++n)
        if (Inputs[n].type == 'checkbox' && Inputs[n].checked == true) {
            item5.setEnabled(true);
            item7.setEnabled(true);
            return;
        }
    item5.setEnabled(false);
    item7.setEnabled(false);
}

function OnInitializeGroupToolBar(oToolBar){
    groupToolBarID = oToolBar.Id;
}

function OnInitializeUserListToolBar(oToolBar){
  userListToolBarID = oToolBar.Id;
}

function OnInitializeGroupWebTree(oWebTree){
    groupWebTreeID = oWebTree.Id;
}

function UpdateNodeText(){
        var tree = igtree_getTreeById("ctl00ContentPlaceHolderManageGroupsControlGroupHeirarchyWebPanelGroupHeirarchyWebTree");
        var textbox = document.getElementById("ctl00_ContentPlaceHolder_ManageGroupsControl_GroupHeirarchyWebPanel_EditGroupControl_DetailsView1_GroupNameEdit");
        var node = tree.getSelectedNode();
        node.setText(textbox.value);
}

function SetUserActionButtons(toolBar){
    var utb = igtbar_getToolbarById(toolBar);
   		if(utb == null) return;
		var item = utb.Items.fromKey("UsersMove");
		var item2 = utb.Items.fromKey("UsersAdd");
		var item3 = utb.Items.fromKey("UsersRemove");
		item.setEnabled(false);
		item2.setEnabled(false);
		item3.setEnabled(false);
		
}
function AfterNodeSelectionChange(treeId, nodeId) {
	    var node = igtree_getNodeById(nodeId);
	    var gtb = igtbar_getToolbarById(groupToolBarID);
	    var utb=igtbar_getToolbarById(userListToolBarID);
		if(gtb == null) return;
		
		//reset buttons
		var item = gtb.Items.fromKey("GroupMove");
		var item2 = gtb.Items.fromKey("GroupAdd");
		var item3 = gtb.Items.fromKey("GroupDelete");
		var item4 = gtb.Items.fromKey("GroupEdit");
		var item5 = utb.Items.fromKey("UsersMove");
		var item6 = utb.Items.fromKey("UsersAdd");
		var item7 = utb.Items.fromKey("UsersRemove");
	
        if(node.getElement().title=='group'){
            if(node.getLevel()<3){
			    item.setEnabled(false);
			    item2.setEnabled(true);
			    item3.setEnabled(false);
			    item4.setEnabled(false);
			    item5.setEnabled(false);
			    item6.setEnabled(true);
			    item7.setEnabled(false);
			}else{
			item.setEnabled(true);
			item2.setEnabled(true);
			item3.setEnabled(true);
			item4.setEnabled(true);
			item5.setEnabled(false);
			item6.setEnabled(true);
			item7.setEnabled(false);
			}
		}else{
			item.setEnabled(false);
			item2.setEnabled(false);
			item3.setEnabled(false);
			item4.setEnabled(false);
			item5.setEnabled(false);
			item6.setEnabled(false);
			item7.setEnabled(false);
		}
}
	   
 function SelectNodeFromSelectedRowValue(personID, groupID) {
 
 //alert(document.forms[0]['UserListBoxControl'].item)
        var tree = igtree_getTreeById("ctl00ContentPlaceHolderManageGroupsControlGroupHeirarchyWebPanelGroupHeirarchyWebTree");
        var nodes = tree.getNodes(true);
        
        for (var i = 0; i < nodes.length; i++) 
        {
            if (nodes[i].getTag() == personID && nodes[i].getParent().getTag()==groupID) 
            {
                 tree.setSelectedNode(nodes[i]);

            }
        }  
             
}
</script>

      <asp:ScriptManager ID="PageScriptManager" runat="server" EnablePartialRendering="true"></asp:ScriptManager> 

<table >
    <tr>
        <td valign="top" align="left" rowspan="2" >
         <asp:Panel ID="Panel1" runat="server" width="310px">
            <igtbar:UltraWebToolbar ImageDirectory="../../../App_Themes/Common/Images/"  ID="GroupToolBar" runat="server" SkinID="ToolBar_Group" >
                <ClientSideEvents InitializeToolbar="OnInitializeGroupToolBar" ></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="GroupAdd"  Enabled="false" Text="Add"  ToolTip="add a new group below this group"  TargetURL="javascript:showAddGroupPanel()" DefaultStyle-Padding-Left="15" HoverStyle-Padding-Left="15" SelectedStyle-Padding-Left="15">
                        <images  DisabledImage-Url="../../../App_Themes/Common/Images/User_Add_d.png" HoverImage-Url="../../../App_Themes/Common/Images/User_Add_h.png" DefaultImage-Url="../../../App_Themes/Common/Images/User_Add.png"></images>
                    </igtbar:TBarButton>
                    <igtbar:TBarButton  Key="GroupMove" Enabled="false" Text="Move"  ToolTip="move this group" TargetURL="javascript:showMoveGroupPanel()" DefaultStyle-Padding-Left="15"  HoverStyle-Padding-Left="15" SelectedStyle-Padding-Left="15">
                       <images   DisabledImage-Url="../../../App_Themes/Common/Images/User_Refresh_d.png" HoverImage-Url="../../../App_Themes/Common/Images/User_Refresh_h.png" DefaultImage-Url="../../../App_Themes/Common/Images/User_Refresh.png"></images></igtbar:TBarButton>
                    <igtbar:TBarButton Key="GroupEdit" Enabled="false" Text="Edit" ToolTip="edit this group" TargetURL="javascript:showEditGroupPanel()" DefaultStyle-Padding-Left="15"  HoverStyle-Padding-Left="15" SelectedStyle-Padding-Left="15">
                        <images  DisabledImage-Url="../../../App_Themes/Common/Images/User_Edit_d.png" HoverImage-Url="../../../App_Themes/Common/Images/User_Edit_h.png" DefaultImage-Url="../../../App_Themes/Common/Images/User_Edit.png"></images>
                    </igtbar:TBarButton>
                    <igtbar:TBarButton Key="GroupDelete" Enabled="false" Text="Delete" ToolTip="delete this group"  TargetURL="javascript:showDeleteGroupPanel()" DefaultStyle-Padding-Left="15"  HoverStyle-Padding-Left="15" SelectedStyle-Padding-Left="15">
                        <images   DisabledImage-Url="../../../App_Themes/Common/Images/User_Delete_d.png" HoverImage-Url="../../../App_Themes/Common/Images/User_Delete_h.png" DefaultImage-Url="../../../App_Themes/Common/Images/User_Delete.png"></images>
                     </igtbar:TBarButton>
                </Items>
            </igtbar:UltraWebToolbar>
            </asp:Panel>
            <igmisc:WebPanel  SkinID="WebPanel_Group" ID="GroupHeirarchyWebPanel"   runat="server"  Width="300px" Height="617px" >
	            <Header Text="Groups" TextAlignment="Left">
                </Header>
                <Template>
                  <asp:UpdatePanel runat="server" ID="GroupHeirarchyUpdatePanel"  ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline"  >
        	            <ContentTemplate>
        	           
                   <ignav:UltraWebTree FullNodeSelect="false"  ID="GroupHeirarchyWebTree" JavaScriptFilename="./ig_webtreeModified.js"  SkinID="WebTree_Group" WebTreeTarget="ClassicTree" runat="server"  CompactRendering="false"  Height="580px" Width="300px" Font-Names="Verdana" Font-Size="Small" >
                   <ClientSideEvents AfterNodeSelectionChange="AfterNodeSelectionChange"   />
                    <DataBindings  >
                    <ignav:NodeBinding   ToolTipField="GROUPORGLIST" DataMember="GROUPORGLIST"  Tag="Applications" Text="Applications" />
                   <ignav:NodeBinding ToolTipField="GROUPORG" DataMember="GROUPORG"  Tag="GROUPORG" TagField="GROUPORG_ID" TextField="GROUPORG_NAME" />
                    <ignav:NodeBinding TargetFrame="group" ToolTip="group" ImageUrl="/coecommonresources/infragistics/20111CLR20/Images/ig_treefolder.gif"  ToolTipField="GROUP" DataMember="GROUP"  Tag="GROUP" TagField="GROUP_ID" TextField="GROUP_NAME" />
                    <ignav:NodeBinding ToolTip="person" ImageUrl="/coecommonresources/infragistics/20111CLR20/Images/OutlookSidebar/contact24.gif" ToolTipField="PERSON" DataMember="PERSON" Tag="PERSON" TagField="PERSON_ID" TextField="NAME" />
                   </DataBindings>
                
                    <Images>
                        <ExpandImage Url="ig_treePlus.gif" />
                        <CollapseImage Url="ig_treeMinus.gif" />
                    </Images>
                    
                    

                 </ignav:UltraWebTree>
                 
                 </ContentTemplate></asp:UpdatePanel>
                 <div class="containerParentClass">
                   <div id="container" >
                    <div visible="false" id="addGroupPanel">
                            <div visible="false" class="hd" style="font-size: 15px;">
                                Add Group   
                            </div>
                            <div visible="false"  class="bd">
                                <asp:UpdatePanel ID="GroupAddUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                                   <table><tr><td align="center"><uc2:AddGroupUC id="GroupAddControl" GroupOperation="Add"  runat="server"></uc2:AddGroupUC>
                                    </td></tr><tr> <td align="center"><asp:Label SkinID="PostActionMessage"  ID="GroupAddMessage"  runat="server" class= "Message" /> 
                                    </td></tr><tr> <td align="center"> <br />
                                     <COEManager:ImageButton ID="OkBtnAddgroup"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Back" visible="false" OnButtonClicked="OkButtonOnCreateGroup_onClick" OnClientClick="ShowOkAddGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" HoverImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" CausesValidation="false" />
                                    <COEManager:ImageButton ID="AddGroupOk"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" visible="false" OnButtonClicked="CancelButtonOnCreateGroup_onClick" OnClientClick="hideAddGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                    <COEManager:ImageButton ID="AddGroupClose"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OkButtonOnCreateGroup_onClick" OnClientClick="hideAddGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                                   </td></tr></table>
                                 </ContentTemplate></asp:UpdatePanel>
                            </div>
                       </div>
                         <div visible="false" id="editGroupPanel" style="font-size: 15px;">
                            <div visible="false"  class="hd">
                                Edit Group
                            </div>
                            <div visible="false"  class="bd">
                                <asp:UpdatePanel ID="GroupEditUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                                    <table><tr><td align="center"><uc1:EditGroupUC   id="GroupEditControl" GroupOperation="Edit" runat="server"></uc1:EditGroupUC>
                                    </td></tr><tr><td align="center"><asp:Label SkinID="PostActionMessage"  ID="GroupEditMessage"  runat="server" class= "Message"/>
                                    <br /><br />
                                   </td></tr><tr><td align="center">
                                   <COEManager:ImageButton ID="OkButtonEditGroup"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Back" visible="false" OnButtonClicked="OkButtonOnCreateGroup_onClick" OnClientClick="ShowOkEditGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" HoverImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" CausesValidation="false" />
                                     <COEManager:ImageButton ID="EditGroupOk"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" visible="false" OnButtonClicked="OkButtonOnEditGroup_onClick" OnClientClick="hideEditGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                    <COEManager:ImageButton ID="EditGroupClose"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OkButtonOnEditGroup_onClick" OnClientClick="hideEditGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                                    </td></tr></table>
                                 </ContentTemplate></asp:UpdatePanel>
                            </div>
                       </div>
                       
                       <div visible="false" id="deleteGroupPanel" style="font-size: 15px;">
                        <div class="hd">Delete Group</div>
                        <div visible="false" class="bd">
                           <asp:UpdatePanel ID="GroupDeleteUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                            <table><tr><td align="center"><asp:Label SkinID="GroupYUIPanelLabel" ID="GroupDeleteLabel" runat="server" Font-Bold="true" ForeColor="Navy" Font-Size="Smaller">Are you sure you want to delete this group?</asp:Label>
                            </td></tr><tr><td align="center"><asp:Label SkinID="PostActionMessage"  ID="GroupDeleteMessage"  runat="server" class="Message"/><br /><br />
                            </td></tr>
                            <tr><td align="center">
                            <COEManager:ImageButton ID="GroupDeleteButton"   runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonText="Delete Group" OnButtonClicked="DeleteGroupButton_Click" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                            <COEManager:ImageButton ID="OkDeleteGroup"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" visible="false" OnClientClick="hideDeleteGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                            <COEManager:ImageButton ID="GroupDeleteButtonClose"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnClientClick="hideDeleteGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                        </td></tr></table>
                            </ContentTemplate></asp:UpdatePanel>
                        </div>
                        
                       <div visible="false" id="moveGroupPanel" style="font-size: 15px;">
                        <div class="hd" >Move Group</div>
                        <div visible="false" class="bd">
                           <asp:UpdatePanel ID="GroupMoveUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                            <table><tr><td> <asp:Label SkinID="PostActionMessage"  ID="GroupMoveInstructionMessage"  runat="server" class= "LabelMessage">Choose a parent group from the list.</asp:Label>
                            </td></tr><tr><td align="center"><asp:ListBox SkinID="YUIPanelListBox" runat="server" Rows="30" ID="GroupMoveListBox" Width="250"  DataSourceID="GroupMoveDataSource" DataTextField="Value" DataValueField = "Key" Height="158px"></asp:ListBox>
                            </td></tr>
                               <tr><td align="center"><asp:Label SkinID="PostActionMessage"  ID="GroupMoveMessage"  runat="server" class= "Message" /><br /><br />
                               </td></tr>
                                <tr><td align="center">
                                 <COEManager:ImageButton ID="GroupMoveBackButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Back" visible="false" OnButtonClicked="OkButtonOnMoveGroup_onClick" OnClientClick="showMoveGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" HoverImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" CausesValidation="false" />
                                <COEManager:ImageButton ID="GroupMoveButton"   runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonText="Move Group" OnButtonClicked="MoveGroupButton_Click" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                                <COEManager:ImageButton ID="OkMoveGroup"  runat="server"   ButtonMode="ImgAndTxt" OnClientClick="hideMoveGroupPanel();" OnButtonClicked="OkButtonOnMoveGroup_onClick"   TypeOfButton="Cancel" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                <COEManager:ImageButton ID="OkMoveGroupClose"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OkButtonOnMoveGroup_onClick" OnClientClick="hideMoveGroupPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                            </td></tr></table>
                            </ContentTemplate></asp:UpdatePanel>
                        </div>
                        
                       </div>
                    </div>
                </div>
            
            </Template> 
            </igmisc:WebPanel>
        
        </td>
        <td valign="top" align="left"  >
        <asp:UpdatePanel runat="server" ID="UserListUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional"  >
        	            <ContentTemplate>
                <igmisc:WebPanel  SkinID="WebPanel_Group2" ID="UserListWebPanel" runat="server" Width="700px" Height="190px">
                
			            <Header Text="Group Users"  TextAlignment="Left">
                       
                        </Header>
        	            <Template>
        	            
        	            <table><tr><td valign="top">
        	            <asp:Panel id="Panel11" runat="server" ScrollBars="Auto" Width="680px" Height="180px">
        	           <asp:GridView ID="UserListGrid"  SkinID="UserListGridView_Group" DataKeyNames="PersonID" Width="680px"  runat="server" AutoGenerateColumns="False" 
                                BorderWidth="1px"  DataSourceID="UserListDataSource" Font-Names="Verdana" Font-Size="X-Small" OnRowDataBound="UserListGrid_RowDataBound">
                                <Columns >
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImgButton" runat="server" ImageUrl="/coecommonresources/infragistics/20111CLR20/Images/OutlookSidebar/contact24.gif"
                                                OnClick="ImgButton_Click" ToolTip="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField >
                                    <ItemTemplate>
                                    <asp:CheckBox   ID="chkSelect" runat="server" onclick="javascript: if (this.checked == false) {ResetHeaderCheckBox(this)}EnableMoveUserBtn(this);"/>
                                    <asp:HiddenField   ID="personIDHidden"  Value='<%# DataBinder.Eval(Container.DataItem, "PersonID") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                    <asp:CheckBox ID="chkBxHeader" onclick="javascript:ToggleSelectAll(this);EnableMoveUserBtn(this);" runat="server" />
                                    </HeaderTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="PersonID"   HeaderText="PersonID" ReadOnly="True" Visible="False" SortExpression="PersonID"  />
                                    <asp:BoundField DataField="UserID" HeaderText="User ID" ReadOnly="True" SortExpression="UserID" />
                                    <asp:BoundField DataField="UserCode" HeaderText="User Code" ReadOnly="True" SortExpression="UserCode" />
                                    <asp:BoundField DataField="FirstName" HeaderText="First Name" ReadOnly="True" SortExpression="FirstName" />
                                    <asp:BoundField DataField="LastName" HeaderText="Last Name" ReadOnly="True" SortExpression="LastName" />
                                    <asp:BoundField DataField="ISLEADER" HeaderText="ISLEADER" />
                                    

                                </Columns>
				<RowStyle HorizontalAlign="Center" />
                            </asp:GridView>
        	            </asp:Panel>
                        </td></tr></table>
         </td>
         <tr><td valign="bottom" >
                         <igtbar:UltraWebToolbar ImageDirectory="/coecommonresources/icon_library/core_collection/core_aqua/png/small/" SkinID="ToolBar_Group" ID="UserListToolbar"  runat="server" > 
                            <ClientSideEvents InitializeToolbar="OnInitializeUserListToolBar" ></ClientSideEvents>
                            <Items>
                               
                                <igtbar:TBarButton Key="UsersAdd"  Enabled="false"   Text="Add User" ToolTip="Add users to this group" TargetURL="javascript:showAddUsersPanel()" DefaultStyle-Padding-Left="50"  HoverStyle-Padding-Left="50" SelectedStyle-Padding-Left="50">
                                        <images   DisabledImage-Url="../../../App_Themes/Common/Images/Add_User_d.png" HoverImage-Url="../../../App_Themes/Common/Images/Add_User.png" DefaultImage-Url="../../../App_Themes/Common/Images/Add_User.png" ></images>
                                </igtbar:TBarButton>
                             
                                <igtbar:TBarButton Key="UsersMove" Enabled="false"   Text="Move User" ToolTip="Move or duplicate users this displayed group" TargetURL="javascript:showMoveUsersPanel()" DefaultStyle-Padding-Left="50"  HoverStyle-Padding-Left="50" SelectedStyle-Padding-Left="50">
                                    <images   DisabledImage-Url="../../../App_Themes/Common/Images/Edit_Role_d.png" HoverImage-Url="../../../App_Themes/Common/Images/Edit_Role.png" DefaultImage-Url="../../../App_Themes/Common/Images/Edit_Role.png" ></images>
                                </igtbar:TBarButton>
                                 <igtbar:TBarButton Key="UsersRemove" Enabled="false"  Text="Remove User" ToolTip="Remove users this displayed group" TargetURL="javascript:showRemoveUsersPanel()" DefaultStyle-Padding-Left="50"  HoverStyle-Padding-Left="50" SelectedStyle-Padding-Left="50">
                                    <images   DisabledImage-Url="../../../App_Themes/Common/Images/Delete_d.png" HoverImage-Url="../../../App_Themes/Common/Images/Delete.png" DefaultImage-Url="../../../App_Themes/Common/Images/Delete.png" ></images>
                                </igtbar:TBarButton>
                            </Items>
                        </igtbar:UltraWebToolbar>
                        
         </td></tr>
                    </Template>
                    </igmisc:WebPanel>
                    </ContentTemplate>
        	            </asp:UpdatePanel>
        	            
        	            
                       <div id="container_userlist" >
                            <div visible="false" id="addUsersPanel">
                                <div visible="false" class="hd">
                                    Add Users 
                                </div>
                                <div visible="false" class="bd">
                                  <asp:UpdatePanel ID="UsersAddUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                                    <table><tr><td> <asp:Label SkinID="PostActionMessage"  ID="UserLabel"  runat="server" class= "LabelMessage">Please select one more users (ctrl + click) and click Save.</asp:Label>
                            </td></tr><tr><td align="center"><asp:ListBox  SkinID="YUIPanelListBox" visible="false" runat="server" Rows="30" Width="250" ID="UsersAddListBox" SelectionMode="Multiple" DataSourceID="DBMSUserListDataSource" DataTextField="Value" DataValueField = "Key" Height="345px"></asp:ListBox>
                              
                                    </td></tr>
                                    <tr>
                                    <td align="left" >
                                    <asp:Label SkinID="PostActionMessage"  visible="false" ID="UsersAddMessage"  runat="server" class="Message"   /><br /><br />
                                    </td>
                                    </tr>
                                    <tr><td align="center">
                                     <COEManager:ImageButton ID="UsersAddButton"   runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonText="Add User" OnButtonClicked="AddUsers_ButtonClick" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                                    <COEManager:ImageButton ID="OkReopen"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Back" visible="false" OnButtonClicked="OkButtonOnCreateUser_onClick" OnClientClick="ReopenAddUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" HoverImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" CausesValidation="false" />
                                    <COEManager:ImageButton ID="CreateUserOkButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" visible="false" OnButtonClicked="OkButtonOnCreateUser_onClick" OnClientClick="hideAddUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                    <COEManager:ImageButton ID="CreateUserCloseButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OkButtonOnCreateUser_onClick" OnClientClick="hideAddUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                                   </td></tr></table>
                                </ContentTemplate></asp:UpdatePanel>
                                </div>
                           </div>
                          <div visible="false" id="removeUsersPanel">
                                <div visible="false" class="hd">
                                    Remove Users
                                </div>
                                <div visible="false" class="bd">
                                   <asp:UpdatePanel ID="UsersRemoveUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                                    <table><tr><td align="center"><asp:Label SkinID="GroupYUIPanelLabel" visible="false" ID="UsersRemoveLabel" runat="server" CssClass="Message">Are you sure you want to remove the selected user(s) from this group?</asp:Label>
                                    </td></tr> <tr><td align="center"><asp:Label SkinID="PostActionMessage" visible="false" ID="UsersRemoveMessage"  runat="server" class="Message"/>
                                    </td></tr>
                                     <tr><td align="center">
                                     <br /><br />
                                     <COEManager:ImageButton ID="UsersRemoveButton"   runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonText="Remove User" OnButtonClicked="RemoveUsersFromGroupButton_Click" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                                     <COEManager:ImageButton ID="RemoveUserOkButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" visible="false" OnButtonClicked="OkButtonOnRemoveUser_onClick" OnClientClick="hideRemoveUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                     <COEManager:ImageButton ID="RemoveUserCloseButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OkButtonOnRemoveUser_onClick" OnClientClick="hideRemoveUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                                     </td></tr></table>
                                    </ContentTemplate></asp:UpdatePanel>
                                </div>
                           </div>
                           <div visible="false" id="moveUsersPanel">
                                <div class="hd" >
                                    Move Users
                                </div>
                                <div visible="false" class="bd">
                                    <asp:UpdatePanel ID="UsersMoveUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate>
                                    
                                    <table><tr><td> <asp:Label SkinID="PostActionMessage"  ID="UserMoveLabel"  runat="server" class= "LabelMessage">Choose a group to move a user(s).</asp:Label>
                                      </td></tr>
                                      <tr>
                                            <td align="center">
                                                <asp:Label ID="UsersMoveMessage" runat="server" SkinID="PostActionMessage" 
                                                    visible="false" class="Message" />
                                            </td>
                                        </tr>
                                      <tr>
                                            <td align="center">
                                                <asp:ListBox ID="UsersMoveListBox" runat="server" 
                                                    DataSourceID="UserMoveDataSource" DataTextField="Value" DataValueField="Key" 
                                                    Height="220px" Rows="30" SelectionMode="Single" SkinID="YUIPanelListBox" 
                                                    visible="false" Width="250"></asp:ListBox>
                                                <br />
                                            </td>
                                        <tr>
                                            <td align="center">
                                                <asp:CheckBox ID="UsersMoveDuplicateCheckBox" runat="server" 
                                                    SkinID="YUIPanelListBox" Text="Duplicate users in select group" 
                                                    visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center"><br />
                                               <COEManager:ImageButton ID="UsersMoveButton"   runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonText="Move User" OnButtonClicked="MoveUsersButton_Click" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                                               <COEManager:ImageButton ID="OkReopenMovePanel"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Back" visible="false" OnButtonClicked="OkButtonOnCreateUser_onClick" OnClientClick="ReopenMoveUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" HoverImageURL="../../../App_Themes/Common/Images/Arrow_Left_B.png" CausesValidation="false" />
                                               <COEManager:ImageButton ID="MoveUserOkButton"  runat="server"   ButtonMode="ImgAndTxt" OnClientClick="hideMoveUsersPanel();" OnButtonClicked="OKButtonOnMoveUser_onClick"   TypeOfButton="Cancel" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                                               <COEManager:ImageButton ID="MoveUserCloseButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Close" visible="false" OnButtonClicked="OKButtonOnMoveUser_onClick" OnClientClick="hideMoveUsersPanel();" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" CausesValidation="false" />
                                            </td>
                                         </tr>
                                        </table>
                                    </ContentTemplate></asp:UpdatePanel>
                                </div>
                           </div>
                       </div>
        </td>
    </tr>
    <tr>
        <td valign="top" align="left">
        <asp:UpdatePanel ID="UserDetailUpdatePanel" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                          <ContentTemplate>
                            <igmisc:WebPanel  SkinID="WebPanel_Group" ID="UserDetailWebPanel" runat="server" Width="685px" Height="390px" PanelStyle-BackColor="White">
			                <Header Text="Group User Details" TextAlignment="Left" >
                            </Header>
			                <Template>
			                <table ><tr><td >
			                   <asp:Panel ID="Panel2" runat="server" Scrollbars="Auto" Width="685px" Height="390px">
                               <asp:DetailsView SkinID="UserDetailView_Group"  Width="685px" DataSourceID="UserDetailDataSource" ID="UserDetailView"  runat="server" AutoGenerateRows="False" DefaultMode="ReadOnly" Font-Names="Verdana" Font-Size="Small">
                                       <Fields>
                                        <asp:BoundField  Visible="false"    DataField="PersonID" HeaderText="PersonID:" SortExpression="PersonID" />
                                        <asp:BoundField Visible="false"  DataField="UserID" HeaderText="UserID:" SortExpression="UserID" />
                                        <asp:BoundField Visible="false" DataField="UserCode" HeaderText="UserCode:" SortExpression="UserCode" />
                                        <asp:BoundField Visible="false"  DataField="FirstName" HeaderText="FirstName:" SortExpression="FirstName" />
                                        <asp:BoundField Visible="false"  DataField="LastName" HeaderText="LastName:" SortExpression="LastName" />
                                        <asp:BoundField Visible="false"   DataField="MiddleName" HeaderText="MiddleName:" SortExpression="MiddleName" />
                                        <asp:BoundField DataField="Address"  HeaderText="Address:" ReadOnly="true" SortExpression="Address" />
                                        <asp:BoundField DataField="Telephone" HeaderText="Telephone:" ReadOnly="true" SortExpression="Telephone" />
                                        <asp:BoundField DataField="Email"  HeaderText="Email:" ReadOnly="true" SortExpression="Email" />
                                        <asp:CheckBoxField DataField="Active"   HeaderText="Active:" ReadOnly="true" SortExpression="Active"  ></asp:CheckBoxField>
                                        <asp:TemplateField HeaderText="Group Granted Roles:" >
                                        <ItemTemplate >
                                        <table BackColor="#E2DED6" Font-Bold="True">
                                            <tr>
                                                <td >
                                                <asp:Repeater  ID="GroupUserRoles" DataSource='<%# Bind("GroupGrantedRoles")%>'   runat="server">
                                                <ItemTemplate><asp:Label ID="GroupRepeater" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>' runat="server"></asp:Label><br /></ItemTemplate>
                                                </asp:Repeater>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Other Granted Roles:" >
                                        <ItemTemplate >
                                        <table  BackColor="#E2DED6" Font-Bold="True">
                                            <tr>
                                                <td>
                                                <asp:Repeater  ID="DirectUserRoles" DataSource='<%# Bind("DirectGrantedRoles")%>'   runat="server">
                                                <ItemTemplate><asp:Label ID="DirectRepeater" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>' runat="server"></asp:Label><br /></ItemTemplate>
                                                </asp:Repeater>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                    </Fields>
                                    
                                </asp:DetailsView>
                                 </asp:Panel>
                          </td></tr></table>
                          
                          </Template>
                        </igmisc:WebPanel>
                        </ContentTemplate>
                                 </asp:UpdatePanel> 
                        
        </td>
    </tr>
</table>

<cc1:csladatasource id="UserListDataSource" runat="server" OnSelectObject="UserListDataSource_SelectObject" typeassemblyname="CambridgeSoft.COE.Framework" typename="CambridgeSoft.COE.Framework.COESecurityService.GroupUserList" TypeSupportsSorting="True" ></cc1:csladatasource>
<cc1:csladatasource id="UserDetailDataSource" runat="server"   OnSelectObject="UserDetailDataSource_SelectObject" typeassemblyname="CambridgeSoft.COE.Framework" typename="CambridgeSoft.COE.Framework.COESecurityService.GroupUser" ></cc1:csladatasource>
<cc1:CslaDataSource id="DBMSUserListDataSource" OnSelectObject="DBMSUserList_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.PersonList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="GroupMoveDataSource" OnSelectObject="GroupListDataSource_SelectObject"  runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.GroupList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="UserMoveDataSource" OnSelectObject="GroupListDataSource_SelectObject"  runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.GroupList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 





 <!--dataSources-->

