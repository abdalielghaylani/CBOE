<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AddInList.ascx.cs" Inherits="PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls.AddInList" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<div>
    <div class="TabsContent">        
        <table class="MessageTableRegAdmin">
            <tr>
                <td colspan="4" align="center">
                    <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
                </td>
            </tr>
        </table>
        <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click">Back</asp:LinkButton>
        <div class="Clear">
        </div>
        <fieldset class="FieldsetTabsContent" style="margin-top: 20px">
            <legend class="LegendTabsContent">Current Addins</legend>
            <div class="AddInList">
                <asp:Label Style="margin-top: 20px" ID="LabelAddInList" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                <br />
                <br />
                <ignav:UltraWebTree ID="AddInsUltraWebTree" runat="server">
                </ignav:UltraWebTree>
            </div>
            <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                <asp:Button ID="ButtonEditAddIn" runat="server" OnClick="ButtonEditAddIn_Click" SkinID="ButtonBigRegAdmin" />
                <asp:Button ID="BtnDeleteAddIn" runat="server" OnClick="BtnDeleteAddIn_Click" SkinID="ButtonBigRegAdmin" />
            </div>
        </fieldset>
        <fieldset class="FieldsetTabsContent" style="margin-top: 50px" id="NewAddInPanel"
            runat="server">
            <legend class="LegendTabsContent">New AddIn</legend>
            <div style="margin-top: 20px">
                <asp:Label ID="LabelFrienlyName" runat="server" CssClass="LabelFieldsetAddIn"></asp:Label>
                <asp:TextBox ID="TexBoxFriendlyName" runat="server" CssClass="TextBoxSubTittle"></asp:TextBox>
            </div>
            <div>
                <asp:Label ID="LblAssembly" runat="server" CssClass="LabelFieldsetAddIn"></asp:Label>
                <asp:DropDownList ID="DdlAssemblies" runat="server" OnSelectedIndexChanged="DdlAssemblies_SelectedIndexChanged"
                    CssClass="AddinProp">
                </asp:DropDownList>
                <asp:Label ID="LblClass" runat="server" CssClass="LabelFieldsetAddIn"></asp:Label>
                <asp:DropDownList ID="DdlClass" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlClass_SelectedIndexChanged"
                    CssClass="AddinProp">
                </asp:DropDownList>
                <div class="Clear">
                </div>
            </div>
            <div>
                <asp:Label ID="LblHandler" runat="server" CssClass="LabelFieldsetAddIn"></asp:Label>
                <asp:DropDownList ID="DdlHandler" runat="server" CssClass="AddinEvent">
                </asp:DropDownList>
                <div class="Clear">
                </div>
                <asp:Label ID="LblEvent" runat="server" CssClass="LabelFieldsetAddIn"></asp:Label>
                <asp:DropDownList ID="DdlEvent" runat="server" CssClass="AddinEvent">
                </asp:DropDownList>
            </div>
            <div class="EventList">
                <br />
                <br />
                <asp:Label ID="LabelEventList" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                <ignav:UltraWebTree ID="EventsUltraWebTree" runat="server">
                </ignav:UltraWebTree>
            </div>
            <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                <asp:Button ID="BtnAddEvent" runat="server" OnClick="BtnAddEvent_Click" SkinID="ButtonBigRegAdmin" />
                <asp:Button ID="BtnDeleteEvent" runat="server" OnClick="BtnDeleteEvent_Click" SkinID="ButtonBigRegAdmin" />
            </div>
            <br />
            <br />
            <div class="DivFieldset">
                <asp:Label ID="LabelAddInConfiguration" runat="server" CssClass="PTabsSubTittle"></asp:Label>
            </div>
            <br />
            <br />
            <asp:TextBox ID="TextBoxConf" runat="server" TextMode="MultiLine" CssClass="MultilineInput"></asp:TextBox>
            <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                <asp:Button ID="BtnAddAddIn" runat="server" OnClick="BtnAddAddIn_Click" SkinID="ButtonBigRegAdmin" />
            </div>
        </fieldset>
        <div style="text-align: right; margin-right: 60px; margin-top: 20px; margin-bottom: 20px">
            <asp:Button ID="Btn_Save" runat="server" OnClick="Btn_Save_Click" SkinID="ButtonBigRegAdmin" />
        </div>
        <div class="Clear">
        </div>
        <div class="TabsFooter">
            <div class="DivFooter">
            </div>
        </div>
    </div>
</div>
