<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HitlistOperationPanel.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.UserControls.HitlistOperationPanel" %>
<div id="HitListOperationYUIPanel" style="text-align:center" runat="server">
    <div class="hd" id="header" runat="server"></div>
    <div class="bd" id="body" runat="server">
        <div><asp:Label ID="HitListOperandALabel" runat="server" /><asp:Label ID="HitListOperationLabel" runat="server" /><asp:Label ID="HitListOperandBLabel" runat="server" /></div>
        <div><asp:Button ID="ExchangeOperandsButton" runat="server" style="visibility:hidden;display:none;"/><asp:Button ID="OKButton" runat="server" /></div>
        <asp:HiddenField ID="ActionHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitlistIDAHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitlistIDBHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitListTypeAHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="HitListTypeBHiddenField" Value="" runat="server" />
        <asp:HiddenField ID="DatabaseHiddenField" Value="" runat="server" />
    </div>
    <div class="ft" id="footer" runat="server"></div>
</div>