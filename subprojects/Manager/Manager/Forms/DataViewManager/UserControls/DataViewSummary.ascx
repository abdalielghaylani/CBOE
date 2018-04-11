<%@ Control Language="C#" AutoEventWireup="true" Inherits="DataViewSummary" Codebehind="DataViewSummary.ascx.cs" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
    <%@ Register Assembly="Infragistics2.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebToolbar" TagPrefix="igtbar" %>    
<table class="DataViewBigTable">
    <tr>
        <td colspan="2">
            <asp:Label ID="SummaryTitleLabel" runat="server" SkinID="DataViewTitle"></asp:Label>
         </td>
    </tr>
    <tr valign="top">
        <td style="padding-left:60px;" align="left">
            <table>
                <tr valign="top">
                    <td align="left">
                        <ignav:UltraWebMenu ID="SortByWebMenu" runat="server" SkinID="SortByWebMenu" OnMenuItemClicked="SortByWebMenu_MenuItemClicked">
                            <ItemStyle CssClass="SortByItem"/>
                            <HoverItemStyle CssClass="SortByItemHover"></HoverItemStyle>
                        </ignav:UltraWebMenu>
                      </td>
                    <td>
                        <asp:Label ID="SelectedSortLabel" runat="server" SkinID="Text"></asp:Label>
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="2">
                        <ignav:UltraWebTree ID="ExistingDataViewsUltraWebTree" runat="server" SkinID="DataViewSummaryTree" OnNodeClicked="DataViewsTree_NodeClicked" Height="400px" >
                        </ignav:UltraWebTree>  
                    </td>
                </tr>
            </table>
        </td>
        <td>
            <asp:UpdatePanel ID="UpdatePanelControl" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="DataBaseLabel" runat="server" SkinID="DataViewTitle"></asp:Label>&nbsp;&nbsp;
                <table id="DataViewTableDetails" class="DataViewTable" runat="server" style="height: 200px">
                    <tr class="DataViewTable" valign="top">
                        <td align="left">
                            <asp:Label runat="server" ID="DataViewNameLabel" SkinID="DataViewTitle"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td colspan="2">
                            <table >
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="DisplayNameLabel" runat="server" SkinID="Text"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <COEManager:TextBoxWithPopUp ID="DisplayNameTextBoxWithPopUp" runat="server" />
                                    </td>
                                </tr>
                                <tr> 
                                    <td  align="left">
                                        <asp:Label runat="server" ID="DescriptionLabel" SkinID="Text"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <COEManager:TextBoxWithPopUp ID="DescriptionTextBoxWithPopUp" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="UserLabel" SkinID="Text"/>
                                    </td>
                                    <td align="left">
                                        <COEManager:TextBoxWithPopUp ID="UserTextBoxWithPopUp" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="FormGroupLabel" SkinID="Text"/>
                                    </td>
                                    <td align="left">
                                        <COEManager:TextBoxWithPopUp ID="FormGroupTextBoxWithPopUp" runat="server" />
                                    </td>
                                </tr>
                                <tr>
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
           </ContentTemplate>
           <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ExistingDataViewsUltraWebTree" EventName="NodeClicked" />
           </Triggers>
         </asp:UpdatePanel>
        </td>
    </tr>
</table>



