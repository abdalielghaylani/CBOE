<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_Public_UserControls_ImageButton" Codebehind="ImageButton.ascx.cs" %>
<div id="TxtAndImgContainer" runat="server">
    <asp:ImageButton ID="ActionImageButton" runat="server" OnClick="ActionImageButton_Click" CssClass="ActionImageButton"/>
    <asp:Button ID="ActionButton" runat="server" OnClick="ActionButton_Click" CssClass="ActionButton"/>
</div>


<div id="OnlyTextContainer" runat="server" class="Button2">
    <div class="Left">
    </div>
    <asp:LinkButton ID="TxtActionButton" runat="server" OnClick="ActionButton_Click" CssClass="LinkButton" />
    <div class="Right">
    </div>
</div>  