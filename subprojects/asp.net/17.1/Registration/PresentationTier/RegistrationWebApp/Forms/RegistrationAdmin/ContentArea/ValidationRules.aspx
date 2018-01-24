<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.master" AutoEventWireup="true"
    Codebehind="ValidationRules.aspx.cs" Inherits="CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.ContentArea.ValidationRules"
    Title="Untitled Page" %>

<%@ Register Src="../UserControls/ValidationRules.ascx" TagName="ValidationRules"
    TagPrefix="uc1" %>
<%@ Register Src="~/Forms/RegistrationAdmin/UserControls/NavigationPanel.ascx" TagName="NavPanel" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
  
    <uc1:ValidationRules ID="ValidationRules1" runat="server" />

</asp:Content>
