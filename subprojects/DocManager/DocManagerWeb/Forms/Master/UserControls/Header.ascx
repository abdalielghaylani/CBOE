<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Header.ascx.cs" Inherits="CambridgeSoft.COE.DocManagerWeb.Forms.Master.UserControls.Header" %>
<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics2.WebUI.UltraWebToolbar.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:Panel runat="server" ID="HeaderContainer">
    <table cellpadding="0" cellspacing="0" id="CommonHeaderTable" class="Header">
        <tr>
            <td runat="server" id="LogoCell" align="right" valign="middle">
                <div class="Brand" id="LogoContainer" runat="server"></div>
                <div><asp:Button BorderStyle="none" runat="server" CssClass="logingbut" SkinId="LogOffButton" ID="LogOffButton" onClick="DoLogOff" CausesValidation=false /></div>
                <div>    
                    <span class="userstatus"><asp:Literal runat="server" ID="WelcomeLiteral" />
                    <strong><asp:Literal runat="server" ID="UserLiteral" ></asp:Literal></strong>
                    </span>
                </div>    
                <div class="GoTo">
                    <ignav:UltraWebMenu ID="GoToUltraWebMenu" runat="server" SkinID="GoToUltraWebMenu"
                        OnMenuItemClicked="GoToUltraWebMenu_MenuItemClicked" Enabled="true">
                        
                    </ignav:UltraWebMenu>
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
            </td>
        </tr>
    </table>
</asp:Panel>
