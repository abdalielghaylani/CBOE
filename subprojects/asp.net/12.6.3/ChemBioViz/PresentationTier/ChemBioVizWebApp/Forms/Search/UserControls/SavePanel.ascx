<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SavePanel.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls.SavePanel" %>
<div id="SavePanelYUIPanel" style="text-align:center" runat="server">
    <div class="hd" id="header" runat="server"></div>
    <div class="bd" id="body" runat="server">
        <table style="text-align:left;">
            <tr><td><asp:Label ID="IDLabel" runat="server" /></td><td><asp:TextBox ID="IDTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="TypeLabel" runat="server" /></td><td><asp:TextBox ID="TypeTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="NameLabel" runat="server" /></td><td><asp:TextBox ID="NameTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="DescriptionLabel" runat="server" /></td><td><asp:TextBox ID="DescriptionTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="DatabaseLabel" runat="server" /></td><td><asp:TextBox ID="DatabaseTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="UserLabel" runat="server" /></td><td><asp:TextBox ID="UserTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="DateCreatedLabel" runat="server" /></td><td><asp:TextBox ID="DateCreatedTextBox" runat="server" /></td></tr>
            <tr><td><asp:Label ID="NumHitsLabel" runat="server" /></td><td><asp:TextBox ID="NumHitsTextBox" runat="server" /></td></tr>
        </table>
        <div><asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" UseSubmitBehavior="false" /></div>
    </div>
    <div class="ft" id="footer" runat="server"></div>
</div>