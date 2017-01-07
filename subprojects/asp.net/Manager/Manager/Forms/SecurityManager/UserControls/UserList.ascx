<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserListUC" Codebehind="UserList.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>



<asp:panel id="Panel1" runat="server" height="650px" width="670px">
<asp:ListBox SkinID"SecurityListBox" runat="server" Rows="30" ID="UserListBoxControl" DataSourceID="UserListDataSource" DataTextField="Key" DataValueField = "Value"></asp:ListBox>
<asp:Button ID="SelectOracleUserButton" Text="OK" OnClick="SelectOracleUserButton_Click" runat="server" />
<asp:Button ID="Close" Text="Close" OnClick="CloseButton_Click" runat="server" />

</asp:panel>
<cc1:CslaDataSource id="UserListDataSource" OnSelectObject="UserListDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.UserList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 

 
    
    
    
    
    
    


