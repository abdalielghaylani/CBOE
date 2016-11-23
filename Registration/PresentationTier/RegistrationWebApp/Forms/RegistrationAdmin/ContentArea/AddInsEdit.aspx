<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.master" AutoEventWireup="true"
    ValidateRequest="false" Codebehind="AddInsEdit.aspx.cs" Inherits="CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.ContentArea.AddInsEdit"
    Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader">AddIns Edition</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <div class="Clear">
        </div>
        <div class="TabsContent">        
            <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click"
                Style="margin-right: 50px;margin-top:10px">Back</asp:LinkButton>
                <div class="Clear">
        </div>
            <fieldset class="FieldsetTabsContent" style="margin-top: 20px">
                <legend class="LegendTabsContent">AddIn Information</legend>
                <div class="DivFieldset">
                    <asp:Label ID="LblFriendlyName" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:Label ID="LblFiendlyNameSelected" runat="server" CssClass="PTabsContent"></asp:Label>
                    <asp:Label ID="LblAddInAssembly" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:Label ID="LblAddInAssemblySelected" runat="server" CssClass="PTabsContent"></asp:Label>
                    <asp:Label ID="LblAddInClass" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:Label ID="LblAddInClassSelected" runat="server" CssClass="PTabsContent"></asp:Label>
                </div>
                <div style="margin-left: 20%; margin-bottom: 2%; margin-top: 1%">
                    <asp:Label ID="LabelEnabled" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:CheckBox ID="CheckBoxEnable" runat="server" OnCheckedChanged="CheckBox_Click"
                        AutoPostBack="true" />
                    <asp:Label ID="LabelRequired" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:CheckBox ID="CheckBoxRequired" runat="server" Enabled="false" />
                </div>
            </fieldset>
            <fieldset class="FieldsetTabsContent" id="EventsPanel" runat="server">
                <legend class="LegendTabsContent">Event Subscription</legend>
                <div class="DivFieldset2">
                    <asp:Label ID="LblEvent" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:DropDownList ID="DdlEvent" runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="LblHandler" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <asp:DropDownList ID="DdlHandler" runat="server">
                    </asp:DropDownList>
                    <br />
                    <br />
                    <asp:Label ID="LabelEventList" runat="server" CssClass="PTabsSubTittle"></asp:Label>
                    <ignav:UltraWebTree ID="UltraWebTreeEvents" runat="server">
                    </ignav:UltraWebTree>
                </div>
                <div style="text-align: right; margin-right: 20px; margin-bottom: 20px">
                    <asp:Button ID="BtnAddEvent" runat="server" OnClick="BtnAddEvent_Click" SkinID="ButtonBigRegAdmin" />
                    <asp:Button ID="BtnDeleteEvent" runat="server" OnClick="BtnDeleteEvent_Click" SkinID="ButtonBigRegAdmin" />
                </div>
            </fieldset>
            <fieldset class="FieldsetTabsContent">
                <legend class="LegendTabsContent">Configuration Edition</legend>
                <br />
                <asp:Xml ID="XmlControl" runat="server" Visible="true" EnableViewState="false"></asp:Xml>
                <asp:TextBox ID="TextBoxConf" runat="server" TextMode="MultiLine" Visible="false"
                    CssClass="AddInConfig" Style="width: 770px"></asp:TextBox>
                <div style="text-align: right; margin-right: 20px; margin-bottom: 20px">
                    <asp:Button ID="ButtonConf" runat="server" OnClick="ButtonConf_Click" SkinID="ButtonBigRegAdmin" />
                    <asp:Button ID="ButtonCancelConfig" runat="server" OnClick="ButtonCancelConfig_Click"
                        SkinID="ButtonBigRegAdmin" />
                </div>
            </fieldset>
            <div style="text-align: right; margin-right: 60px; margin-bottom: 20px">
                <asp:Button ID="BtnSave" runat="server" OnClick="BtnSave_Click" SkinID="ButtonBigRegAdmin" />
                <asp:Button ID="BtnCancel" runat="server" OnClick="BtnCancel_Click" SkinID="ButtonBigRegAdmin" />
            </div>
            <div class="Clear">
            </div>
            <div class="TabsFooter">
                <div class="DivFooter">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
