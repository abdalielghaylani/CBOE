<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.master" AutoEventWireup="true"
    Codebehind="ParamEdit.aspx.cs" Inherits="PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea.ParamEdit"
    Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="~/Forms/RegistrationAdmin/UserControls/NavigationPanel.ascx" TagName="NavPanel" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <uc:NavPanel ID="PropertiesPanel" runat="server" />
    <asp:Panel ID="PanelParamEdit" runat="server" Width="700px" SkinID="ValidationSummaryPanel">
        <div id="Parameters" style="margin-bottom: 20px">
            <table>
                <tr>
                    <td style="width: 300px; height: 69px;">
                        <div>
                            <div style="display: inline; margin-top: 10px; margin-right: 8px">
                                <asp:Label ID="LblName" runat="server" Width="80px" SkinID="Title"></asp:Label><asp:TextBox
                                    ID="TxtName" runat="server" Width="80px"></asp:TextBox>
                            </div>
                            <div style="display: inline">
                                <asp:Button ID="BtnAddParameter" runat="server" OnClick="BtnAddParameter_Click" />
                            </div>
                        </div>
                        <div>
                            <div style="display: inline; margin-right: 8px">
                                <asp:Label ID="LblValue" runat="server" Width="80px" SkinID="Title"></asp:Label><asp:TextBox
                                    ID="TxtValue" runat="server" Width="80px"></asp:TextBox>
                            </div>
                            <div style="display: inline">
                                <asp:Button ID="BtnDeleteParameter" runat="server"  OnClick="BtnDeleteParameter_Click" />
                            </div>
                        </div>
                    </td>
                    <td style="width: 211px; height: 69px;">
                        <div>
                            <ignav:UltraWebTree ID="UltraWebTreeParams" runat="server" Width="209px" SkinID="TablesUltraWebTree">
                            </ignav:UltraWebTree>
                        </div>
                    </td>
                </tr>
            </table>
            <div style="text-align: right;margin-right:20px">
                <asp:Button ID="BtnSave" runat="server" OnClick="BtnSave_Click" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
