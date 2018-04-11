<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="ImportExportCustom.aspx.cs" Inherits="RegistrationWebApp.Forms.RegistrationAdmin.ContentArea.ImportExportCustom"
    Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader">Import/Export</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <div class="TabsContent">
            <table class="MessageTableRegAdmin">
                <tr>
                    <td>
                        <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
                    </td>
                </tr>
            </table>
            <asp:LinkButton ID="LinkButtonGoToMain" runat="server" CssClass="Goto2" OnClick="LinkButtonGoToMain_Click">Back</asp:LinkButton>
            <asp:Label ID="LabelImporExportInfo" runat="server" CssClass="PTabsContent" Visible="false"></asp:Label>
            <div class="Clear">
                </div>
            <fieldset class="FieldsetTabsContent" id="ImportGroup" runat="server">
                <legend class="LegendTabsContent">
                    <asp:Label ID="LabelLocalImportLegend" runat="server"></asp:Label></legend>
                <asp:Label ID="LabelLocalImportInfo" runat="server" CssClass="PTabsContent"></asp:Label>
                <asp:Label ID="LabelLocalImportUrl" runat="server" CssClass="PTabsContent2"></asp:Label>
                <asp:TextBox ID="TextBoxLocalImport" CssClass="ControlElements" Width="500px" runat="server"></asp:TextBox>
                <span style="margin-top:3px;float:left;margin:2px;"><asp:CheckBox ID="ForceImportCheckBox" runat="server" Checked="false" style="color:#2a28d1;" /></span>
                <asp:RequiredFieldValidator ID="TextBoxLocalImportValidator" ControlToValidate="TextBoxLocalImport"
                    runat="server" EnableClientScript="true" ValidationGroup="Group1" ErrorMessage="The path cannot be empty." />
                <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                    <asp:Button ID="ButtonLocalImport" runat="server" OnClick="ButtonLocalImport_Click"
                        ValidationGroup="Group1" CausesValidation="true" SkinID="ButtonBigRegAdmin" />
                </div>
            </fieldset>
            <fieldset class="FieldsetTabsContent" id="ExportGroup" runat="server">
                <legend class="LegendTabsContent">
                    <asp:Label ID="LabelLegen2" runat="server"></asp:Label></legend>
                <asp:Label ID="LabelInfo2" runat="server" CssClass="PTabsContent"></asp:Label>
                <asp:Label ID="LabelLocalExportUrl" runat="server" CssClass="PTabsContent2"></asp:Label>
                <asp:TextBox ID="TextBoxLocalExport" CssClass="ControlElements" Width="500px" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="TextBoxLocalExportValidator" ControlToValidate="TextBoxLocalExport"
                    runat="server" EnableClientScript="true" ValidationGroup="Group2" ErrorMessage="The path cannot be empty." />
                <div style="margin-top: 50px">
                    <fieldset class="FieldsetTabsContent">
                        <legend class="LegendTabsContent">
                            <asp:Label ID="LabelLegen3" runat="server"></asp:Label></legend>
                        <div style="margin-top: 15px; margin-left: 7px">
                            <asp:RadioButtonList ID="RadioButtonListTables" runat="server" onchange="selectTables()">
                            </asp:RadioButtonList>
                        </div>
                        <div style="margin-top: 20px">
                            <asp:CheckBoxList ID="CheckBoxListTables" runat="server">
                            </asp:CheckBoxList>
                        </div>
                    </fieldset>
                </div>
                <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                    <asp:Button ID="ButtonExport" runat="server" OnClick="ButtonExport_Click" ValidationGroup="Group2"
                        SkinID="ButtonBigRegAdmin" />
                </div>
            </fieldset>
            <%-- <fieldset class="FieldsetTabsContent" id="INIGroup" runat="server">
               <legend class="LegendTabsContent">Import Registration 10 Configuration</legend>
                <asp:Label ID="LabelImportIni" runat="server" CssClass="PTabsContent"></asp:Label>
                <asp:Label ID="LabelIniServerPath" runat="server" CssClass="PTabsContent2"></asp:Label>
                <asp:TextBox ID="TextBoxIniServerPath" CssClass="ControlElements" Width="500px" runat="server"></asp:TextBox>
               <asp:RequiredFieldValidator ID="TextBoxIniServerPathValidator" ControlToValidate="TextBoxIniServerPath"
                    runat="server" EnableClientScript="true" ValidationGroup="Group3" ErrorMessage="The path cannot be empty." />
                <div style="text-align: right; margin-right: 20px; margin-top: 20px; margin-bottom: 20px">
                    <asp:Button ID="ButtonIniFile" runat="server" OnClick="ButtonIniFile_Click" ValidationGroup="Group3"
                        SkinID="ButtonBigRegAdmin" />
                </div>
            </fieldset>--%>
            <div class="TabsFooter">
                <div class="DivFooter" />
            </div>
        </div>
    </div>
</asp:Content>
