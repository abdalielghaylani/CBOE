<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="BatchComponent.aspx.cs" Inherits="CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.ContentArea.BatchComponent"
    Title="Untitled Page" %>

<%@ Register Src="../UserControls/PropertyListUC.ascx" TagName="PropertyList" TagPrefix="uc1" %>
<%@ Register Src="~/Forms/RegistrationAdmin/UserControls/NavigationPanel.ascx" TagName="NavPanel" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
       <div class="Content">
        <uc:NavPanel ID="PropertiesPanel" runat="server" />        
        <uc1:PropertyList ID="PropertyList1" runat="server"></uc1:PropertyList>
         <div class="ClearProperties"></div>
    </div> 
</asp:Content>
