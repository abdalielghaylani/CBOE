<%@ Control Language="C#" AutoEventWireup="true" Inherits="Forms_Master_UserControls_Header"  Codebehind="Header.ascx.cs" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:Panel runat="server" ID="HeaderContainer" CssClass="PublicHeader">
    <div id="bar"></div>
    <div class="PublicBrand" id="LogoContainer" runat="server"></div>
    <div><asp:Button BorderStyle="none" runat="server" CssClass="PublicLoginButton" ID="LogOffButton" onClick="DoLogOff" CausesValidation="false" /></div>
    <div>    
        <span class="PublicUserStatus"><asp:Literal runat="server" ID="WelcomeLiteral" />
        <strong><asp:Literal runat="server" ID="UserLiteral" ></asp:Literal></strong>
        </span>
    </div>
    <div id="mainnav">
        <span><asp:LinkButton ID="HomeLink" runat="server" CausesValidation="false" /></span>
        <span><a href="" id="HelpLink" runat="server" target="Help" /></span>
        <!-- Changed Link button 'ID' to add as new Link button control -->
        <span><asp:LinkButton ID="AboutPageLink" runat="server" >About</asp:LinkButton></span>
        <span class="last"><a href="" id="AdminHelpLink" runat="server" target="Help"></a></span>
    </div>
    <div class="Goto">
        <ignav:UltraWebMenu ID="GoToUltraWebMenu" runat="server" SkinID="GoToUltraWebMenu"
            OnMenuItemClicked="GoToUltraWebMenu_MenuItemClicked" Enabled="true">
            <ItemStyle CssClass="Item"></ItemStyle>
            <HoverItemStyle CssClass="ItemHover">
            </HoverItemStyle>
        </ignav:UltraWebMenu>
    </div>
    <div class="PublicPageTitleLabel">
        <asp:Label runat="server" ID="PageTitleLabel"></asp:Label>
    </div>
    <div>
        <igtbar:UltraWebToolbar ID="Toolbar" runat="server" OnButtonClicked="UltraWebToolbarControl_ButtonClicked"
            CssClass="MenuHeader">
            <Items>
                <igtbar:TBLabel Text="">
                </igtbar:TBLabel>
            </Items>
        </igtbar:UltraWebToolbar>
    </div>
</asp:Panel>