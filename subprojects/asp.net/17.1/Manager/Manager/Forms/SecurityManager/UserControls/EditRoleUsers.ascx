<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditRoleUsersUC" Codebehind="EditRoleUsers.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:panel id="Panel1" runat="server">
    <asp:UpdatePanel ID="EditRoleUsersUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DetailsView SkinID="SecurityDetailView4" id="DetailsView1" runat="server"  OnDataBound="DetailsView_Load" DataSourceID="COERoleBODataSource" DataKeyNames="RoleName" AutoGenerateRows="False" DefaultMode="Edit">
                <Fields>
                    <asp:BoundField DataField="RoleID" HeaderText="RoleID" ReadOnly="True" SortExpression="RoleID" Visible= "False" />
                    <asp:BoundField DataField="RoleName" ReadOnly="true" HeaderText="Role Name" SortExpression="RoleName" />
                    <asp:TemplateField HeaderStyle-CssClass="HiddenHeader">
                        <ItemTemplate>
                            <table class="EditRoleUsersTable">
                                <tr>
                                    <td>
                                        <asp:Label ID="List1Label" Text="Available Users" runat="server" CssClass="ListLabel"></asp:Label>
                                        <asp:ListBox ID="LeftList" Rows="7" DataSourceID="COEUsersBOListDataSource" DataTextField= "UserID" DataValueField= "UserID" SelectionMode="Multiple"  runat="server" CssClass="UserListBox"></asp:ListBox>
                                    </td>
                                    <td>
                                        <table style="vertical-align:middle;">
                                            <tr style="height:50px">
                                                <td><asp:Button ID="MoveFromLeftListToRightList" Text=">>" runat="server" CssClass="RightButton" /></td>
                                           </tr>
                                            <tr style="height:50px">
                                                <td><asp:Button ID="MoveFromRightListToLeftList" Text="<<" runat="server" CssClass="LeftButton"/></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Label ID="List2Label" Text="Current Users" runat="server" CssClass="ListLabel"></asp:Label>
                                        <asp:ListBox ID="RightList" Rows="7" DataSource='<%# Bind("RoleUsers")%>'  DataTextField= "UserID" DataValueField= "UserID" SelectionMode="Multiple"  runat="server" CssClass="UserListBox"></asp:ListBox>
                                    </td>
                                 </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="ManageRolesButtonsHolder">
                                <COEManager:ImageButton ID="SaveButton"  enabled="false" runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonCssClass="EditRoleUsersSave"></COEManager:ImageButton>
                                <COEManager:ImageButton ID="CancelButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" ButtonCssClass="EditRoleUsersCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"></COEManager:ImageButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:panel>
<cc1:CslaDataSource id="COERoleBODataSource"  OnSelectObject="COERoleBODataSource_SelectObject"  OnUpdateObject="COERoleBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COERoleBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="COEUsersBOListDataSource"  OnSelectObject="COEUsersBOListDataSource_SelectObject"  runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COEUserBOList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 

 
    
    
    
    
    
    


