<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuItem.ascx.cs" Inherits="Forms_Public_UserControls_MenuItem" %>
<div id="MenuItem" runat="server" class="MenuItemContainer">
    <table align="left" width="100%">
        <tr align="left" class="MenuItemHeaderRow">
            <td colspan="2" align="left">
                <asp:HyperLink runat="server" ID="ItemTitleLink" SkinID="MenuTitleItem" ></asp:HyperLink>
            </td>
        </tr>
        <tr align="left">
            <td align="left">
                <asp:ImageButton runat="server" ID="MenuItemImageButton" />
            </td>
            <td valign="top" align="left">
                <asp:HyperLink runat="server" ID="ItemDescriptionLink" SkinID="MenuItem" ></asp:HyperLink>
            </td>
        </tr>
    </table>
</div>
