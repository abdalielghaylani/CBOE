<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_EditRole" Codebehind="EditRole.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/EditRole.ascx" TagName="EditRoleUC" TagPrefix="uc1" %>
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
                <uc1:EditRoleUC id="EditRoleControl" runat="server"></uc1:EditRoleUC>
            </td>
       </tr>
       <tr>
            <td>
            </td>
       </tr>
    </table>
</asp:Content>


