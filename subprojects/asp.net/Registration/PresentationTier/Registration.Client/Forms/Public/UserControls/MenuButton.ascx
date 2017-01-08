<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuButton.ascx.cs" Inherits="PerkinElmer.CBOE.Registration.Client.Forms.Public.UserControls.MenuButton" %>

<div style="float:left">
    <div class="MenuButton" id="MainButtonContainer" runat="server">
        <img id="LeftImage" src="../../../App_Themes/Blue/Images/ButBk.gif" runat="server" alt=""/>
        <asp:LinkButton ID="SplitButtonLink" runat="server" Text="Split Button" />
        <img id="DropDownImage" src="../../../App_Themes/Blue/Images/DropdownArrow.gif" runat="server" alt=""/>
    </div>
    <div style="display: none;" class="MenuItemContainer" id="MenuItems" runat="server"/>
</div>
