<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_ChangePassword" Codebehind="ChangePassword.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/ChangePassword.ascx" TagName="ChangePasswordUC" TagPrefix="uc1" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
            </td>
        </tr>
       <tr>
            <td>
                <uc1:ChangePasswordUC id="ChangePasswordControl" runat="server"></uc1:ChangePasswordUC>
            </td>
       </tr>
      
    </table>
    
</asp:Content>
