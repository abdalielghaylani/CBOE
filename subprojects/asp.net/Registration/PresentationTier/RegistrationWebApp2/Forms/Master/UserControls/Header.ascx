<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="RegistrationWebApp2.Forms.Master.UserControls.Header" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:Panel runat="server" ID="HeaderContainer">
    <table cellpadding="0" cellspacing="0" id="CommonHeaderTable" class="Header" runat="server">
        <tr>
            <td runat="server" id="LogoCell" align="right">
                <div style="width: 980px; height: 57px;" id="TopContainer" runat="server">
                    <div class="Brand" id="LogoContainer" runat="server">
                    </div>
                    <div style="width: 420px; float: left; padding-top: 10px; height: 39px;" id="UserWelcomeContainer" runat="server">
                        <span class="userstatus">
                            <asp:Literal runat="server" ID="WelcomeLiteral" />
                            <strong>
                                <asp:Literal runat="server" ID="UserLiteral"></asp:Literal></strong> </span>
                    </div>
                    <div style="width: 80px; float: left; padding-top: 6px; height: 39px;" id="LogOffContainer" runat="server">
                        <asp:Button BorderStyle="none" runat="server" CssClass="logingbut" SkinID="LogOffButton"
                            ID="LogOffButton" OnClick="DoLogOff" CausesValidation="false" /></div>
                </div>
                <div style="width: 925px; height: 25px; padding-left: 55px; margin-top: 7px;" id="MenuContainer" runat="server">
                    <div class="GoTo">
                        <ignav:UltraWebMenu ID="GoToUltraWebMenu" runat="server" SkinID="GoToUltraWebMenu"
                            OnMenuItemClicked="GoToUltraWebMenu_MenuItemClicked" Enabled="true" CssClass="GoToList">
                        </ignav:UltraWebMenu>
                    </div>
                    <div style="height: 20px; padding-top: 5px; float: left; padding-left: 250px;">
                        <igtbar:UltraWebToolbar ID="UltraWebToolbarControl" runat="server" OnButtonClicked="UltraWebToolbarControl_ButtonClicked"
                            CssClass="MenuHeader">
                            <Items>
                                <igtbar:TBLabel Text="">
                                </igtbar:TBLabel>
                            </Items>
                        </igtbar:UltraWebToolbar>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
