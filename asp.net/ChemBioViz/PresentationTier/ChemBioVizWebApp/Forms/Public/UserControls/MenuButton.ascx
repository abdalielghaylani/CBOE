<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuButton.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.MenuButton" %>
<%@ Register Src="~/Forms/Public/UserControls/SimpleSavePanel.ascx" TagPrefix="CBV" TagName="SimpleSavePanel" %>
<div style="float:left;">
    <div class="MenuButton" id="MainButtonContainer" runat="server">
        <img id="LeftImage" src="../../../App_Themes/Blue/Images/ButBk.gif" runat="server" alt="" height="16" width="16"/>
        <asp:LinkButton ID="SplitButtonLink" runat="server" Text="Split Button" />
        <img id="DropDownImage" src="../../../App_Themes/Blue/Images/DropdownArrow.gif" runat="server" alt="Show More" height="5" width="5"/>
    </div>
    <div style="display: none;" class="MenuItemContainer" id="MenuItems" runat="server"/>
    <CBV:SimpleSavePanel ID="SimpleSavePanel" runat="server" />
</div>