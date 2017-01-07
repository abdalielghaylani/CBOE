<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditRoleUC" Codebehind="EditRole.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>


<asp:panel id="Panel1" runat="server">
    <asp:UpdatePanel ID="EditRoleUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DetailsView id="DetailsView1" SkinID="SecurityDetailView2" runat="server" OnDataBound="DetailsView_Load" DataSourceID="COERoleBODataSource" DataKeyNames="RoleName" AutoGenerateRows="False"  DefaultMode="Edit" >
                <Fields>
                    <asp:BoundField DataField="RoleID" HeaderText="RoleID" ReadOnly="True" SortExpression="RoleID" Visible= "False"/>
                    <asp:TemplateField>    
                        <ItemTemplate>
                            <asp:HiddenField  runat="server" ID="IsDBMSRole"  Value='<%# Bind("isDBMSRole")%>'  visible="false" ></asp:HiddenField>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Role Name" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                          <asp:TextBox runat="server" ID="RoleName" Text='<%# Bind("RoleName")%>'  CssClass="UserTextBox"></asp:TextBox>
                          <asp:LinkButton ID="SelectOracleRoleLinkButton"  runat="server" OnClick="SelectOracleRoleButton_Click" OnClientClick="javascript:showAddOracleRolePanel()" CssClass="OracleUserLink">Select Oracle Role</asp:LinkButton>
                          <asp:RegularExpressionValidator runat="server" ID="RoleNameRegExpValidator" SkinID="ReqField" EnableClientScript="true" ControlToValidate="RoleName" ValidationExpression="^[a-zA-Z0-9_]*$" Display="dynamic"  />
                          <asp:RegularExpressionValidator runat="server" ID="RoleNameLengthRegExpValidator" SkinID="ReqField" EnableClientScript="true" ControlToValidate="RoleName" ValidationExpression="^.{1,100}$" Display="dynamic" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:panel id="Panel1" runat="server" height="400px" width="250px" ScrollBars="Auto">
                                <asp:CheckBoxList AutoPostBack="false" DataSource='<%# Bind("Privileges") %>' DataTextField="PrivilegeName" DataValueField="PrivilegeName" OnSelectedIndexChanged="PrivilegeListChange" runat="server" ID="PrivilegeListControl"></asp:CheckBoxList>
                            </asp:panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <p runat="server" id="OracleRolWarning" style="height:450px;width:350px;" visible="false"><strong>Note:</strong><br />This Role will be added and will appear when adding or editing users. However, it will not appear in the manage roles list as it is not an editable role.</p>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div style="margin-bottom:3px;">
                                <COEManager:ImageButton ID="SaveButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                                <COEManager:ImageButton ID="CancelButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
                <FooterTemplate>
                </FooterTemplate>
            </asp:DetailsView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:panel> 

 <div id="container_rolelist" >
    <div visible="false" id="addOracleRolePanel">
        <div visible="false" class="hd">
            Add Role From Oracle
        </div>
        <div visible="false" class="bd">
            <asp:UpdatePanel ID="OracleRolesAddUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td align="center">
                                <asp:ListBox SkinID="YUIPanelListBox" AutoPostBack="true" OnSelectedIndexChanged="RoleAddListBox_IndexChange" visible="false" runat="server" Rows="20" Width="280px" ID="RolesAddListBox" SelectionMode="Single" DataSourceID="RoleListDataSource" DataTextField="Value" DataValueField="Key" />
                                <br />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
   </div>
</div>

<cc1:CslaDataSource id="COERoleBODataSource"  OnInsertObject="COERoleBODataSource_InsertObject"   OnSelectObject="COERoleBODataSource_SelectObject"  OnUpdateObject="COERoleBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COERoleBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="RoleListDataSource" OnSelectObject="RoleListDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.RoleList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 


    
    
    
    
    
    


