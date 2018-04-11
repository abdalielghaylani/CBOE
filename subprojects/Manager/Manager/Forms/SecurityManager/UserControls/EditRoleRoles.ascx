<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditRoleRolesUC" Codebehind="EditRoleRoles.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:panel id="Panel2" runat="server">
    <asp:UpdatePanel ID="EditRoleUsersUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DetailsView OnDataBound="DetailsView_Load" SkinID="SecurityDetailView4" GridLines="none" id="DetailsView1" runat="server" DataSourceID="COERoleBODataSource" DataKeyNames="RoleName" AutoGenerateRows="False"  DefaultMode="Edit"  >
                <Fields>
                    <asp:BoundField DataField="RoleID" HeaderText="RoleID" ReadOnly="True" SortExpression="RoleID" Visible= "False" />
                    <asp:BoundField DataField="RoleName" ReadOnly="true" HeaderText="Role Name" SortExpression="RoleName" />
                    <asp:TemplateField HeaderStyle-CssClass="HiddenHeader">
                        <ItemTemplate>
                            <table class="EditRoleUsersTable">
                                <tr>
                                    <td>
                                        <asp:Label ID="List1Label" Text="Available Roles" runat="server" CssClass="ListLabel"></asp:Label>
                                        <asp:ListBox ID="LeftList" Rows="7" DataSourceID="COERoleBOListDataSource" DataTextField= "RoleName" DataValueField= "RoleName" SelectionMode="Multiple"  runat="server" CssClass="UserListBox"></asp:ListBox>
                                    </td>
                                    <td>
                                        <table style="vertical-align:middle;">
                                            <tr>
                                                <td style="height:50px;"><asp:Button ID="MoveFromLeftListToRightList" Text=">>" runat="server" CssClass="RightButton"/></td>
                                           </tr>
                                            <tr>
                                                <td style="height:50px;"><asp:Button ID="MoveFromRightListToLeftList" Text="<<" runat="server" CssClass="LeftButton"/></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Label ID="List2Label" Text="Current Roles" runat="server" CssClass="ListLabel"></asp:Label>
                                        <asp:ListBox ID="RightList" Rows="7" DataSource='<%# Bind("RoleRoles")%>'  DataTextField= "RoleName" DataValueField= "RoleName" SelectionMode="Multiple" runat="server" CssClass="UserListBox"></asp:ListBox>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="ManageRolesButtonsHolder">
                                <COEManager:ImageButton ID="SaveButton" enabled="false" runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonCssClass="EditRoleUsersSave"></COEManager:ImageButton>
                                <COEManager:ImageButton ID="CancelButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" ButtonCssClass="EditRoleUsersCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"></COEManager:ImageButton>
                            </div>
                            <asp:HyperLink runat="server" ID="PageSettingsLink" CssClass="PageSettingsLink"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:panel>
<cc1:CslaDataSource id="COERoleBODataSource"  OnSelectObject="COERoleBODataSource_SelectObject"  OnUpdateObject="COERoleBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COERoleBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="COERoleBOListDataSource"  OnSelectObject="COERoleBOListDataSource_SelectObject"  runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COERoleBOList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 

 
    
    
    
    
    
    


