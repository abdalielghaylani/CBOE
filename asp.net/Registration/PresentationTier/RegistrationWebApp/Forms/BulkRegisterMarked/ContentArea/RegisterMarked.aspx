<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Forms/Master/Registration.Master"
    Codebehind="RegisterMarked.aspx.cs" Inherits="RegistrationWebApp.Forms.BulkRegisterMarked.ContentArea.BulkRegister" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="BulkRegisterContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td align="left">
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClick="GoHomeButton_Click" />
                <asp:Button ID="DoneButton" runat="server" CausesValidation="false" OnClick="DoneButton_Click" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel" />
            </td>
        </tr>
        <tr runat="server" id="ComponentToolBar">
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="RegisterPanel" runat="server" HorizontalAlign="Center" Width="690px">
                    <table class="PagesContentTable" cellspacing="0" width="690px">
                        <tr>
                            <td style="width: 100%; height: 40px">
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="center">
                                <asp:Label ID="GoBackLinkButton" Visible="false" runat="server" SkinID="LabelButtonXL"
                                    Style="cursor: pointer" /><asp:Label ID="DuplicateActionLabel" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="width: 100%; height: 25px">
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="center">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:RadioButton ID="DuplicateRadioButton" runat="server" GroupName="1" Width="90px" /></td>
                                        <td>
                                            <asp:RadioButton ID="BatchRadioButton" runat="server" GroupName="1" Width="90px" /></td>
                                        <td>
                                            <asp:RadioButton ID="TempRadioButton" runat="server" GroupName="1" Width="90px" /></td>
                                        <td>
                                            <asp:RadioButton ID="NewRegisterRadioButton" runat="server" GroupName="1" Width="90px" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="width: 100%; height: 15px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="width: 100%; height: 15px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="DescriptionLabel" runat="server" Width="70px" /></td>
                                        <td>
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100%; height: 55px">
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="center">
                                <asp:Button ID="RegisterMarkedButton" runat="server" OnClick="RegisterMarkedButton_Click"
                                    SkinID="ButtonXL" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="ShowLogInfoPanel" runat="server" HorizontalAlign="Center" Width="690px">
                    <table class="PagesContentTable" cellspacing="0" width="690px">
                        <tr>
                            <td style="width: 100%; height: 40px">
                            </td>
                        </tr>
                        <tr align="center">
                            <td align="center">
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="PromptMsg1Label" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:LinkButton ID="LogLinkButton" runat="server" OnClick="LogLinkButton_Click"></asp:LinkButton>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <iframe id="SearchTempFrame" runat="server" enableviewstate="true" height="800" width="100%"
                    visible="false"></iframe>
            </td>
        </tr>
    </table>
</asp:Content>
