<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" CodeBehind="LockMarked.aspx.cs" Inherits="RegistrationWebApp2.Forms.BulkRegisterMarked.ContentArea.LockMarked"  Title="Untitled Page" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table cellspacing="0" class="PagesContentTable">
        <tr class="PagesToolBar">
            <td align="left">
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClick="GoHomeButton_Click" />
                <asp:Button ID="ExitButton" runat="server" OnClick="ExitButton_Click" BorderColor="red" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>
        </tr>    
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td align="center" colspan="2">
                <uc2:messagesarea id="MessagesAreaUserControl" runat="server" visible="false"></uc2:messagesarea>
            </td>
        </tr>
    </table>
</asp:Content>
