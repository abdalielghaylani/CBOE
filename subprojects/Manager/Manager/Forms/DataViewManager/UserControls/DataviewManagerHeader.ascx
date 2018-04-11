<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataviewManagerHeader.ascx.cs" Inherits="Manager.Forms.DataViewManager.UserControls.DataviewManagerHeader" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:Panel runat="server" ID="HeaderContainer" CssClass="DataviewManagerContainer">
            <div runat="server" id="LogoCell" class="DataviewManagerHeader">
                <div class="DataViewManagerBrand" id="LogoContainer" runat="server"></div>
                <div><asp:Button BorderStyle="none" runat="server" CssClass="DataviewManagerLoginButton" ID="LogOffButton" onClick="DoLogOff" CausesValidation="false" /></div>
                <div>    
                    <span class="DataviewManagerUserStatus"><asp:Literal runat="server" ID="WelcomeLiteral" />
                    <strong><asp:Literal runat="server" ID="UserLiteral" ></asp:Literal></strong>
                    </span>
                </div>
                <div id="mainnav">
                    <span><asp:LinkButton ID="HomeLink" runat="server" CausesValidation="false" /></span>
                    <span><asp:LinkButton ID="MainLink" runat="server" CausesValidation="false" /></span>
	                <span><a href="" id="HelpLink" runat="server" target="_blank" /></span>
                    <span class="last"><asp:LinkButton ID="AboutLink" runat="server" CausesValidation="false" /></span>
                </div>
                <div class="Goto">
                    <ignav:UltraWebMenu ID="GoToUltraWebMenu" runat="server" SkinID="GoToUltraWebMenu"
                        OnMenuItemClicked="GoToUltraWebMenu_MenuItemClicked" Enabled="true">
                        <ItemStyle CssClass="Item"></ItemStyle>
                        <HoverItemStyle CssClass="ItemHover">
                        </HoverItemStyle>
                    </ignav:UltraWebMenu>
                </div>
                <div class="DataviewManagerPageTitleLabel">
                    <asp:Label runat="server" ID="PageTitleLabel"></asp:Label>
                </div>
                <div>
                    <igtbar:UltraWebToolbar ID="UltraWebToolbarControl" runat="server" OnButtonClicked="UltraWebToolbarControl_ButtonClicked"
                        CssClass="MenuHeader">
                        <Items>
                            <igtbar:TBLabel Text="">
                            </igtbar:TBLabel>
                        </Items>
                    </igtbar:UltraWebToolbar>
                </div>
            </div>
</asp:Panel>