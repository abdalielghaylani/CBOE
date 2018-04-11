<%@ Control Language="C#" AutoEventWireup="true" Inherits="SelectTables" Codebehind="SelectTables.ascx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebToolbar.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebToolbar" TagPrefix="igtbar" %>
<table>
    <tr>
        <td colspan="1">
            <asp:Label ID="SummaryTitleLabel" runat="server" SkinID="DataViewTitle"></asp:Label>
         </td>
         <td colspan="1" align="right">
            <asp:Label ID="DataBaseLabel" runat="server" SkinID="DataViewTitle"></asp:Label>
            <asp:Label ID="IDLabel" runat="server" SkinID="DataViewTitle"></asp:Label>
         </td>
    </tr>
    <tr valign="top">
        <td colspan="2">
            <table>
                <tr valign="top">
                    <td align="left">
                        <ignav:UltraWebMenu ID="SortByWebMenu" runat="server" SkinID="SortByWebMenu" OnMenuItemClicked="SortByWebMenu_MenuItemClicked">
                            <ItemStyle CssClass="SortByItem"/>
                            <HoverItemStyle CssClass="SortByItemHover"></HoverItemStyle>
                        </ignav:UltraWebMenu>
                    </td>
                    <td align="right">
                        <asp:Label ID="SelectedSortLabel" runat="server" SkinID="Text"></asp:Label>
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="2" align="left">
                      <ignav:UltraWebTree ID="TablesUltraWebTree" SingleBranchExpand="true" runat="server" DataKeyOnClient="false" SkinID="TablesUltraWebTree" OnNodeClicked="TablesUltraWebTree_NodeClicked" OnNodeChecked="TablesUltraWebTree_NodeChecked" OnNodeExpanded="TablesUltraWebTree_NodeExpanded" OnNodeCollapsed="TablesUltraWebTree_NodeCollapsed" CompactRendering="true" LoadOnDemand="AutomaticSmartCallbacks" AllowDrag="false" AllowDrop="false" Section508Compliant="true" WebTreeTarget="HierarchicalTree"></ignav:UltraWebTree>  
                    </td>
                </tr>
            </table>
        </td>
     </tr>
</table>
