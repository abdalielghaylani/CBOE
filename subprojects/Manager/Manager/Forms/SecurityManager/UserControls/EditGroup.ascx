<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditGroupUC" Codebehind="EditGroup.ascx.cs"  %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebListbar.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebListbar" TagPrefix="iglbar" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:panel id="Panel1" runat="server" >
    <asp:DetailsView ID="EditGroupDetailView" SkinID="SecurityDetailView2" DataKeyNames="GroupID"  OnDataBound="DetailsView_Load" runat="server" AutoGenerateRows="False" DataSourceID="GroupDataSource">
        <Fields>
            <asp:TemplateField HeaderText="ParentGroup"><ItemTemplate>
            <asp:TextBox ID="GroupMaster" runat="server" ReadOnly="true" BackColor="lightgray"></asp:TextBox></ItemTemplate></asp:TemplateField>
            <asp:BoundField Visible="False" DataField="GroupID" HeaderText="GroupID" SortExpression="GroupID" />
            <asp:BoundField Visible="False" DataField="GroupOrgID" HeaderText="GroupOrgID" SortExpression="GroupOrgID" />
            <asp:BoundField Visible="True" DataField="GroupName" HeaderText=
            "GroupName" SortExpression="GroupName" />
            <asp:BoundField Visible="False" DataField="ParentGroupID" HeaderText="ParentGroupID" SortExpression="ParentGroupID" />
            <asp:TemplateField HeaderText="Leader">
            <EditItemTemplate >
              <asp:DropDownList   ID="UserListDropDown" runat="server" 
                DataSourceID="UserListDataSource"
                DataTextField="Value" DataValueField="Key" 
                SelectedValue='<%# Bind("LeaderPersonID") %>'>
              </asp:DropDownList>
            </EditItemTemplate>
            <ItemTemplate>
              <asp:DropDownList ID="UserListDropDown2" runat="server" 
                DataSourceID="UserListDataSource"
                 DataTextField="Value" DataValueField="Key" 
                Enabled="False" SelectedValue='<%# Bind("LeaderPersonID") %>'>
              </asp:DropDownList>
            </ItemTemplate>
          </asp:TemplateField>
            <asp:TemplateField HeaderText="Roles" >
            <ItemTemplate >
            <table  BackColor="#E2DED6" Font-Bold="True">
                    <tr>
                    <td style="width: 158px">
                            <asp:Label ID="EditLeftListLabel"  runat="server"></asp:Label>
                            <asp:ListBox Enabled="true"  ID="EditLeftList" runat="server" SelectionMode="Multiple" AutoPostBack="true" AppendDataBoundItems="false" Rows="5"></asp:ListBox>
                    </td>
                    <td style="width: 120px" align="center">
                        &nbsp;<table>
                            <tr>
                                <td style="width: 65px"  >
                                <asp:Button ID="EditLeftToRightButton" Text=">>" runat="server" BackColor="#000063" ForeColor="#ffffff" Font-Bold="true" Width="50px" OnClientClick="ShowMsg()" /></td>
                           </tr>
                           
                            <tr>
                                <td style="width: 65px">
                                  <asp:Button  ID="EditRightToLeftButton" UseSubmitBehavior="false" Text="<<" runat="server" BackColor="#000063" ForeColor="#ffffff" Font-Bold="true" Width="50px"/></td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 158px">
                     <asp:Label ID="EditRightListLabel"  runat="server"></asp:Label>
                     <asp:ListBox  Enabled="true" ID="EditRightList" runat="server" SelectionMode="Multiple" AutoPostBack="False" AppendDataBoundItems="false" Rows="5"></asp:ListBox>
                    </td>
                    <asp:HiddenField ID="EditHidden" runat="server" />
                    </tr>
                </table>            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <div style="margin-bottom:3px;">
                        <COEManager:ImageButton ID="SaveButton"   runat="server"  ButtonMode="ImgAndTxt" ButtonText="Save Group" TypeOfButton="Save" ButtonCssClass="EditRoleSave" ImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" HoverImageURL="../../../App_Themes/BLUE/Images/IconLibrary/Vista_Business_Collection/PNG/24/Save.png" />
                        <COEManager:ImageButton ID="CancelButton"  runat="server"   ButtonMode="ImgAndTxt" OnClientClick="hideEditGroupPanel(); return false;"   TypeOfButton="Cancel" ButtonCssClass="EditRoleCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" />
                    </div>
                 </ItemTemplate>
             </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <cc1:CslaDataSource id="EditGroupDataSource" OnSelectObject="GroupBODataSource_SelectObject"  OnUpdateObject="GroupBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COEGroupBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
</asp:panel>
<cc1:CslaDataSource id="GroupDataSource" OnSelectObject="GroupBODataSource_SelectObject"  OnUpdateObject="GroupBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COEGroupBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="UserListDataSource" OnSelectObject="UserListDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.PersonList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 

    
    
    
    
    
    


