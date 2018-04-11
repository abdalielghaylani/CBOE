<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_EditRoleRoles" Codebehind="EditRoleRoles.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/EditRoleRoles.ascx" TagName="EditRoleRolesUC" TagPrefix="uc1" %>

<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" > 
    <table class="PagesContentTable">
        <tr>
            <td align="center">
               <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
          </td>
        </tr>
        <tr>
            <td >
               <asp:UpdatePanel ID="ErrorAreaUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
                </ContentTemplate></asp:UpdatePanel>
            </td>
        </tr>
       <tr>
            <td>
                <uc1:EditRoleRolesUC id="EditRoleRolesControl" runat="server"></uc1:EditRoleRolesUC>
            </td>
       </tr>
       <tr>
            <td>
            </td>
       </tr>
    </table>
</asp:Content>


