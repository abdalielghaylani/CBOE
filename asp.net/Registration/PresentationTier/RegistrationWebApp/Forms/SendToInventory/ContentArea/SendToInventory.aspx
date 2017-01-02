<%@ Page AutoEventWireup="true" Codebehind="SendToInventory.aspx.cs" Inherits="RegistrationWebApp.Forms.SendToInventory.SendToInventory"
    Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" %>


<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="SendToInventoryContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder">
        <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td align="left">
                <asp:Button ID="HomeButton" runat="server" CausesValidation="false" OnClick="HomeButton_Click" />
                <asp:Button ID="DoneButton" runat="server" CausesValidation="false" OnClick="DoneButton_Click" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td align="center" colspan="3">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>    
        </table>
</asp:Content>

