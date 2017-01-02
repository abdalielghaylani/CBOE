<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmationPanel.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls.ConfirmationPanel" %>
<div id="ConfirmationYUIPanel" style="text-align:center" runat="server">
    <div class="hd" id="header" runat="server"></div>
    <div class="bd" id="body" runat="server">
        <div><asp:Label ID="ConfirmationLabel" runat="server" /></div>
        <div><asp:Button ID="OKButton" runat="server" /></div>
        <asp:HiddenField ID="ActionHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitlistIDHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitListTypeHiddenField" Value="" runat="server" />
    </div>
    <div class="ft" id="footer" runat="server"></div>
</div>
