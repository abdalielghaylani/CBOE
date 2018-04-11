<%@ Page Language="C#" MasterPageFile="~/Forms/Master/MasterPage.master" AutoEventWireup="true" CodeBehind="ManageHitList.aspx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.ContentArea.ManageHitList" Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="~/Forms/Search/UserControls/SavePanel.ascx" TagPrefix="SP" TagName="SavePanel" %>
<%@ Register Src="~/Forms/Search/UserControls/ConfirmationPanel.ascx" TagPrefix="CP" TagName="ConfirmationPanel" %>
<%@ Register Src="~/Forms/Search/UserControls/HitlistOperationPanel.ascx" TagPrefix="OP" TagName="HitlistOperationPanel" %>

<asp:Content ID="ManageHitListPlaceHolder" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div style="width:100%;text-align:center;">
        <div style="display:inline;overflow:hidden;width:600px;float:left;">
            <ignav:UltraWebTree ID="HitListUltraWebTree" runat="server" />
        </div>
        <div style="display:inline;text-align:center;width:150px;">
            <div style="margin-top:50px;">
                <div>
                    <asp:Label ID="HitListInfoLabel" runat="server" />
                </div>
                <div>
                    <table style="text-align:left;">
                        <tr><td><asp:Label ID="IDLabel" runat="server" /></td><td><asp:TextBox ID="IDTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="TypeLabel" runat="server" /></td><td><asp:TextBox ID="TypeTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="NameLabel" runat="server" /></td><td><asp:TextBox ID="NameTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="DescriptionLabel" runat="server" /></td><td><asp:TextBox ID="DescriptionTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="DatabaseLabel" runat="server" /></td><td><asp:TextBox ID="DatabaseTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="UserLabel" runat="server" /></td><td><asp:TextBox ID="UserTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="DateCreatedLabel" runat="server" /></td><td><asp:TextBox ID="DateCreatedTextBox" runat="server" Enabled="false" /></td></tr>
                        <tr><td><asp:Label ID="NumHitsLabel" runat="server" /></td><td><asp:TextBox ID="NumHitsTextBox" runat="server" Enabled="false" /></td></tr>
                    </table>
                </div>
            </div>
            <div style="margin-top:100px;">
                <div>
                    <asp:Label ID="OperationsLabel" runat="server" />
                </div>
                <div>
                    <asp:Button ID="SaveHitListButton" runat="server" UseSubmitBehavior="false" />
                    <asp:Button ID="DeleteHitListButton" runat="server" UseSubmitBehavior="false" />
                    <asp:Button ID="EditHitListButton" runat="server" UseSubmitBehavior="false" />
                    <asp:Button ID="RestoreHitListButton" runat="server" UseSubmitBehavior="false" />
                </div>
                <div>
                    <asp:Button ID="UnionButton" runat="server" UseSubmitBehavior="false" />
                    <asp:Button ID="SubtractButton" runat="server" UseSubmitBehavior="false" />
                    <asp:Button ID="IntersectButton" runat="server" UseSubmitBehavior="false" />
                </div>
            </div>
        </div>
    </div>
    <SP:SavePanel ID="SavePanel" runat="server" />
    <CP:ConfirmationPanel ID="ConfirmationPanel" runat="server" />
    <OP:HitlistOperationPanel ID="HitlistOperationPanel" runat="server" />
</asp:Content>
