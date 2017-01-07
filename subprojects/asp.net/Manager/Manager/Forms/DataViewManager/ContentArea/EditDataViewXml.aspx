<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_EditDataViewXml"
    ValidateRequest="false" Codebehind="EditDataViewXml.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>

<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebToolbar" TagPrefix="igtbar" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"
        ScriptMode="Release" />
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table class="DataViewBigTable" border="0">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="SummaryTitleLabel" runat="server" SkinID="DataViewTitle"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td style="padding-left: 60px;" align="left">
                            <table id="tblTreeview" runat="server">
                                <tr valign="top">
                                    <td align="left">
                                        <ignav:UltraWebMenu ID="SortByWebMenu" runat="server" SkinID="SortByWebMenu" OnMenuItemClicked="SortByWebMenu_MenuItemClicked">
                                            <ItemStyle CssClass="SortByItem" />
                                            <HoverItemStyle CssClass="SortByItemHover">
                                            </HoverItemStyle>
                                        </ignav:UltraWebMenu>
                                    </td>
                                    <td>
                                        <asp:Label ID="SelectedSortLabel" runat="server" SkinID="Text"></asp:Label>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td colspan="2">
                                        <ignav:UltraWebTree ID="ExistingDataViewsUltraWebTree" runat="server" SkinID="DataViewSummaryTree"
                                            OnNodeClicked="DataViewsTree_NodeClicked" Height="400px">
                                        </ignav:UltraWebTree>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="DvDetails" runat="server" visible="false">
                                <table id="DataViewTableDetails" class="DataViewTable" runat="server">
                                    <tr class="DataViewTable" valign="top">
                                        <td align="left">
                                            <asp:Label runat="server" ID="DataViewNameLabel" SkinID="DataViewTitle"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td colspan="2">
                                            <center>
                                                <asp:Label runat="server" Font-Bold="true" ID="DisplayNameTextBoxWithPopUp"></asp:Label>
                                            </center>
                                            <table>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label runat="server" ID="DescriptionLabel" SkinID="Text"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <COEManager:TextBoxWithPopUp ID="DescriptionTextBoxWithPopUp" runat="server" />
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label runat="server" ID="UserLabel" SkinID="Text" />
                                                    </td>
                                                    <td align="left">
                                                        <COEManager:TextBoxWithPopUp ID="UserTextBoxWithPopUp" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label runat="server" ID="FormGroupLabel" SkinID="Text" />
                                                    </td>
                                                    <td align="left">
                                                        <COEManager:TextBoxWithPopUp ID="FormGroupTextBoxWithPopUp" runat="server" />
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label runat="server" ID="DateCreatedLabel" SkinID="Text" />
                                                    </td>
                                                    <td align="left">
                                                        <COEManager:TextBoxWithPopUp ID="DateCreatedTextBoxWithPopUp" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table id="DataViewXmlDetails" class="DataViewTable" runat="server" style="height: 200px">
                                    <tr class="DataViewTable" valign="top">
                                        <td align="left">
                                            <asp:TextBox ID="DataViewXmlText" runat="server" Height="300px" Width="100%" TextMode="MultiLine">
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="EditXmlButton" runat="server" Text="Edit Xml" OnClick="EditXmlButton_Click" />
                                            <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="BackButton_Click" />
                                            <asp:Button ID="SaveXmlButton" runat="server" Text="Save Xml" OnClick="SaveXmlButton_Click" />
                                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click"
                                                OnClientClick="return ConfirmCancel();" CausesValidation="false" />
                                            <div>
                                                <asp:Button ID="CopyToClipBoardButton" runat="server" OnClientClick="copyToClipboard();return false;"
                                                    CssClass="Goto2" Visible="false" Text="Copy to Clipboard" />
                                                <asp:Button ID="PasteFromClipBoardButton" runat="server" OnClientClick="pasteFromClipboard();return false;"
                                                    CssClass="Goto2" Visible="false" Text="Paste from Clipboard" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
