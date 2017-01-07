<%@ Control Language="C#" AutoEventWireup="true" Inherits="SecurityUC" Codebehind="Security.ascx.cs" %>
<table runat="server" id="PublicDvTable">
    <tr>
        <td align="left">
            <asp:RadioButtonList ID="DataViewUserRolesSelectionRadioList" runat="server" SkinID="UsersRadioItems">
            </asp:RadioButtonList>
        </td>
    </tr>
</table>
<table cellpadding="0" id="UsersSelectionTable" runat="server" class="RolesUsersSelectionTable">
    <tr>
        <th colspan="3" class="RolesUsersSelectionTableCell" style="width: 100%;" align="center"
            id="UsersHeaderCell" runat="server">
        </th>
    </tr>
    <tr valign="top">
        <th id="AvailableUsersHeaderCell" class="RolesUsersSelectionTableCell" runat="server">
        </th>
        <th id="UserActionsHeaderCell" style="width: 50px;" class="RolesUsersSelectionTableCell"
            runat="server" align="center">
        </th>
        <th id="CurrentUsersHeaderCell" class="RolesUsersSelectionTableCell" runat="server">
        </th>
    </tr>
    <tr valign="top">
        <td valign="top" align="left" class="RolesUsersSelectionTableCell">
            <asp:ListBox runat="server" ID="AllUsersList" SkinID="ListItems" DataValueField="PersonID"
                DataTextField="UserID" Rows="20" SelectionMode="Multiple" CssClass="UserListBox"
                Width="95%"></asp:ListBox>
        </td>
        <td align="center" valign="middle" style="width: 50px;" class="RolesUsersSelectionTableCell">
            <table style="margin-bottom: 30px; border: none;">
                <tr>
                    <td>
                        <asp:Button ID="AddUserImageButton" Text=">>" runat="server" BackColor="#000063"
                            ForeColor="#ffffff" Font-Bold="true" Width="50px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="RemoveUserImageButton" Text="<<" runat="server" BackColor="#000063"
                            ForeColor="#ffffff" Font-Bold="true" Width="50px" />
                    </td>
                </tr>
            </table>
        </td>
        <td valign="top" align="left" class="RolesUsersSelectionTableCell">
            <asp:ListBox runat="server" ID="SelectedUsersList" SkinID="ListItems" DataValueField="PersonID"
                DataTextField="UserID" Rows="20" SelectionMode="Multiple" CssClass="UserListBox"
                Width="95%"></asp:ListBox>
        </td>
    </tr>
</table>
<table cellpadding="0" id="RolesSelectionTable" runat="server" class="RolesUsersSelectionTable">
    <tr>
        <th colspan="3" class="RolesUsersSelectionTableCell" style="width: 100%;" align="center"
            id="RolesHeaderCell" runat="server">
        </th>
    </tr>
    <tr valign="top">
        <th id="AvailableRolesHeaderCell" class="RolesUsersSelectionTableCell" runat="server">
        </th>
        <th id="RoleActionsHeaderCell" style="width: 50px;" class="RolesUsersSelectionTableCell"
            runat="server" align="center">
        </th>
        <th id="CurrentRolesHeaderCell" class="RolesUsersSelectionTableCell" runat="server">
        </th>
    </tr>
    <tr valign="top">
        <td valign="top" align="left" class="RolesUsersSelectionTableCell">
            <asp:ListBox runat="server" ID="AllRolesList" SkinID="ListItems" DataValueField="RoleID"
                DataTextField="RoleName" Rows="20" SelectionMode="Multiple" CssClass="UserListBox"
                Width="95%"></asp:ListBox>
        </td>
        <td align="center" valign="middle" style="width: 50px;" class="RolesUsersSelectionTableCell">
            <table style="margin-bottom: 30px; border: none;">
                <tr>
                    <td>
                        <asp:Button ID="AddRoleImageButton" Text=">>" runat="server" BackColor="#000063"
                            ForeColor="#ffffff" Font-Bold="true" Width="50px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="RemoveRoleImageButton" Text="<<" runat="server" BackColor="#000063"
                            ForeColor="#ffffff" Font-Bold="true" Width="50px" />
                    </td>
                </tr>
            </table>
        </td>
        <td valign="top" align="left" class="RolesUsersSelectionTableCell">
            <asp:ListBox runat="server" ID="SelectedRolesList" SkinID="ListItems" DataValueField="RoleID"
                DataTextField="RoleName" Rows="20" SelectionMode="Multiple" CssClass="UserListBox"
                Width="95%"></asp:ListBox>
        </td>
    </tr>
</table>
