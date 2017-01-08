<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="DeleteMarked.aspx.cs" Inherits="PerkinElmer.CBOE.Registration.Client.Forms.BulkRegisterMarked.ContentArea.DeleteMarked"
    Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td align="left">
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClick="GoHomeButton_Click" />
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel" />
                <asp:Button ID="AcceptButton" runat="server" OnClick="AcceptButton_Click" />
                <asp:Button ID="ExitButton" runat="server" OnClick="ExitButton_Click" BorderColor="red" />
            </td>
            <td align="right">
            </td>
        </tr>
        <tr runat="server" id="ComponentToolBar">
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="2" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LogDescriptionLabel" runat="server" Visible="false"></asp:Label>
                <asp:TextBox ID="LogDescriptionTextBox" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <cc2:COEGridView ID="RegistriesCOEGridView" runat="server">
                </cc2:COEGridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="StatusLabel" runat="server"></asp:Label>
                <asp:LinkButton ID="LogLinkButton" runat="server" OnClick="LogLinkButton_Click" Visible="false"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <iframe id="ChemBioVizFrame" runat="server" enableviewstate="true" height="800px"
                    width="1024px" visible="false"></iframe>
            </td>
        </tr>
    </table>
</asp:Content>
