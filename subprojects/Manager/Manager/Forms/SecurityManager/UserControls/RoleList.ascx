<%@ Control Language="C#" AutoEventWireup="true" Inherits="RoleListUC" Codebehind="RoleList.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>



<asp:panel id="Panel1" runat="server" height="650px" width="670px">
<asp:ListBox SkinID"SecurityListBox" runat="server" Rows="15" ID="RoleListBoxControl" DataSourceID="RoleListDataSource" DataTextField="Key" DataValueField = "Value"></asp:ListBox>
<asp:Button ID="SelectOracleRoleButton" Text="OK" OnClick="SelectOracleRoleButton_Click" runat="server" />
<asp:Button ID="Close" Text="Cancel" OnClick="CloseButton_Click" runat="server" />

</asp:panel>
<cc1:CslaDataSource id="RoleListDataSource" OnSelectObject="RoleListDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.RoleList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 

 
    
    
    
    
    
    


