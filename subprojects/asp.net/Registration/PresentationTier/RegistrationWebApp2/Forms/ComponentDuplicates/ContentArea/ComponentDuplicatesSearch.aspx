<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="ComponentDuplicatesSearch.aspx.cs" Inherits="RegistrationWebApp2.Forms.ComponentDuplicates.ContentArea.ComponentDuplicatesSearch"
    Title="Untitled Page" %>

<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc2" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.ChemDraw"
    TagPrefix="cc1" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" style="margin-left: 2px;">
        <tr class="PagesToolBar">
            <td align="left">
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClick="GoHomeButton_Click" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="ComponentToolBar">
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <iframe id="ComponentDuplicatesSearchFrame" runat="server" style="width: 100%;" scrolling="no">
                </iframe>
            </td>
        </tr>
    </table>    
</asp:Content>
