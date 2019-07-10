<%@ Control Language="C#" AutoEventWireup="true" Inherits="ManageRoles" Codebehind="ManageRoles.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

  <script type="text/javascript">

var userListToolBarID;




   function Confirmation() {
        if (confirm("Confirm Delete \n\n\n\n Are you sure you want to delete this role?")) {
            return true;
        } else {
            return false;
        }
    }
    
    var dcTime=250;    // doubleclick time
 var dcDelay=100;   // no clicks after doubleclick
 var dcAt=0;        // time of doubleclick
 var savEvent=null; // save Event for handling doClick().
 var savEvtTime=0;  // save time of click event.
 var savTO=null;    // handle of click setTimeOut
 
 
 
 function hadDoubleClick() {
   var d = new Date();
   var now = d.getTime();
   if ((now - dcAt) < dcDelay) {
     return true;
   }
   return false;
 }
 
 function handleWisely(which,eventName) {
   switch (which) {
     case "click": 
       // If we've just had a doubleclick then ignore it
       if (hadDoubleClick()) return false;
       
       var selectBox = document.getElementById('<%= this.RoleListControl.ClientID %>');
       if (!selectBox || selectBox.selectedIndex == -1) {
         return false;
       }       
       // Otherwise set timer to act.  It may be preempted by a doubleclick.
       savEvent = eventName;
       d = new Date();
       savEvtTime = d.getTime();
       if(eventName == 'ChangeButtonState')
         savTO = setTimeout('EnableManageRoleButtons()', dcTime);
       else
         savTO = setTimeout("doClick(savEvent)", dcTime);
       break;
     case "dblclick":
       doDoubleClick(eventName);
       break;
     default:
   }
 }
 
 function doClick(eventName) {
   // preempt if DC occurred after original click.
   if (savEvtTime - dcAt <= 0) {
     return false;
   }
   
   __doPostBack(eventName,'');
 }
 
 function doDoubleClick(eventName) {
   var d = new Date();
   dcAt = d.getTime();
   if (savTO != null) {
     clearTimeout( savTO );          // Clear pending Click  
     savTO = null;
   }
   __doPostBack(eventName,'');
 }

function EnableManageRoleButtons()
 {
    document.getElementById('<%= this.AddButton.GetButtonClientID() %>').disabled = false;
    document.getElementById('<%= this.EditButton.GetButtonClientID() %>').disabled = false;
    document.getElementById('<%= this.EditUsersButton.GetButtonClientID() %>').disabled = false;
    document.getElementById('<%= this.EditRolesButton.GetButtonClientID() %>').disabled = false;
    document.getElementById('<%= this.DeleteButton.GetButtonClientID() %>').classList.remove("btn-disable");
    document.getElementById('<%= this.DeleteButton.GetButtonClientID() %>').classList.add("ImageButton");
 }
</script>
 <style type="text/css">
    .btn-disable
        {
        cursor: not-allowed;
        pointer-events: none;
        border:none;
        color: #c0c0c0;
        background-color: #ffffff;
        margin-top: 5px;
        }
</style>

<asp:UpdatePanel ID="ManageRoleUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
<ContentTemplate>

<asp:Panel ID="ButtonPanel" SkinID="ButtonPanel" runat="server" HorizontalAlign="Center">
<div class="ToolbarRoles">
    <ul>
        <li><COEManager:ImageButton enabled="true" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Add_Role.png" HoverImageURL="../../../App_Themes/Common/Images/Add_Role.png" ImageCssClass="SecurityImage" TypeOfButton="AddRole"  ID="AddButton" OnButtonClicked="AddButtonAction" runat="server" /></li>
        <li><COEManager:ImageButton enabled="false" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Edit.png" HoverImageURL="../../../App_Themes/Common/Images/Edit.png" ImageCssClass="SecurityImage"  TypeOfButton="Edit" ID="EditButton" OnButtonClicked="EditButtonAction" runat="server" /></li>
        <li><COEManager:ImageButton enabled="false" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Edit_User.png" HoverImageURL="../../../App_Themes/Common/Images/Edit_User.png" ImageCssClass="SecurityImage"  TypeOfButton="EditRoleUsers" ID="EditUsersButton" OnButtonClicked="EditUsersButtonAction" runat="server" /><br /></li>
        <li><COEManager:ImageButton enabled="false" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Edit_Role.png" HoverImageURL="../../../App_Themes/Common/Images/Edit_Role.png" ImageCssClass="SecurityImage"  TypeOfButton="EditRoleRoles" ID="EditRolesButton" OnButtonClicked="EditRolesButtonAction" runat="server" /></li>
        <li><COEManager:ImageButton ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Delete.png" HoverImageURL="../../../App_Themes/Common/Images/Delete.png" ImageCssClass="SecurityImage"   TypeOfButton="Delete"  ID="DeleteButton" OnClientClick="return Confirmation();" OnButtonClicked="DeleteButtonAction" runat="server" ButtonCssClass="btn-disable" /></li> 
     </ul>   
</div>        
</asp:Panel>

<asp:Table ID="Table1"  runat="server" SkinID="SecurityTable">
<asp:TableRow ID="TableRow1" SkinID="SecurityRow"  runat="server">
<asp:TableCell ID="TableCell1" SkinID="SecurityCellLeft"  runat="server">
View Roles From:
</asp:TableCell>
<asp:TableCell ID="TableCell2" SkinID="SecurityCellRight" runat="server">
    <asp:DropDownList ID="ApplicationListControl" SkinID="SecurityDropDownList" DataSourceID="ApplicationListDataSource" DataTextField="Key" DataValueField="Value"  OnSelectedIndexChanged="SelectApplication" runat="server" AutoPostBack="true">
    </asp:DropDownList>
</asp:TableCell>
</asp:TableRow>
<asp:TableRow ID="TableRow2"  SkinID="SecurityRow" runat="server">
<asp:TableCell ID="TableCell3" SkinID="SecurityCellLeft2" runat="server">
Select a Role:
</asp:TableCell>
<asp:TableCell ID="TableCell4" SkinID="SecurityCellRight" runat="server">
<asp:ListBox ID="RoleListControl"   SkinID="SecurityListBox" Rows="15" DataTextFormatString="" DataSourceID="RoleListDataSource" runat="server" DataTextField="Key" DataValueField="Value" ></asp:ListBox>
</asp:TableCell>
</asp:TableRow>

<asp:TableFooterRow runat="server"><asp:TableCell SkinID="SecurityCellFooter" runat="server" ColumnSpan="2">

</asp:TableCell></asp:TableFooterRow>


</asp:Table>
</ContentTemplate></asp:UpdatePanel>



 <!--dataSources-->
<cc1:csladatasource id="RoleListDataSource" runat="server" OnSelectObject="RoleListDataSource_SelectObject" typeassemblyname="CambridgeSoft.COE.Framework" typename="CambridgeSoft.COE.Framework.COESecurityService.RoleList" ></cc1:csladatasource>
<cc1:csladatasource Id="ApplicationListDataSource" runat="server" typeassemblyname="CambridgeSoft.COE.Framework" typename="CambridgeSoft.COE.Framework.COESecurityService.ApplicationList"  OnSelectObject="ApplicationNamesDataSource_SelectObject"></cc1:csladatasource>

