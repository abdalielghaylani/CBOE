<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.master" AutoEventWireup="true"
    Codebehind="Default.aspx.cs" Inherits="CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.ContentArea.Default"
    Title="Cambridge Soft - Registration Admin" ValidateRequest="false" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">   
        <asp:Image ID="ImageCSoftLogo" runat="server" SkinID="CustomBrand" />
        <div class="AplMenu">
            <asp:Label ID="LabelRegAdminTitle" runat="server" SkinID="Registration"></asp:Label>
            <a class="HelpButton" href="#more" onmouseover="HelpIconTextVisibility(true);" onmouseout="HelpIconTextVisibility(false);">
            </a>
            <div id="DivHelpLink" class="HelpInfo" runat="server">
                <asp:Label ID="LabelHideInfo" runat="server" Text="Label"></asp:Label>
            </div>
            <p class="PAplMenu">
                <asp:Label ID="LabelRegAdminDescription" runat="server"></asp:Label></p>
            
            <asp:Panel ID="ImportExportItem" runat="server" CssClass="Items">
                <asp:Image ID="ImageImpExpIcon" runat="server" SkinID="I24" />
                <asp:LinkButton ID="LinkButtonImpExpPropList" runat="server" SkinID="MenuItem" OnClick="LinkButtonImpExpPropList_Click"></asp:LinkButton>
            </asp:Panel>
            <asp:Panel ID="TableEditorItem" runat="server" CssClass="Items">
                <asp:Image ID="TableEditorImage" runat="server" SkinID="I24" />
                <asp:LinkButton ID="TableEditorLink" runat="server" SkinID="MenuItem" OnClick="TableEditorLink_Click"></asp:LinkButton>
            </asp:Panel>
            <asp:Panel ID="ManageSystemBehaviors" runat="server" class="Items">
                <asp:Image ID="ImagePropertyList" runat="server" SkinID="I24" />
                <asp:Label ID="LabelPropListAddInTitle" runat="server" SkinID="MenuItem"></asp:Label>
            </asp:Panel>
            <ul class="UlAplMenu">
                <li  class="LiAplMenu">
                    <asp:LinkButton ID="LinkButtonSetUpPropList" runat="server" SkinID="MenuItem" OnClick="LinkButtonSetUpPropList_Click"></asp:LinkButton></li>
                <li class="LiAplMenu">
                    <asp:LinkButton ID="CustomizeFormsLinkButton" runat="server" SkinID="MenuItem" OnClick="CustomizeFormsLinkButton_Click"></asp:LinkButton></li>
                <li  class="LiAplMenu">
                    <asp:LinkButton ID="LinkButtonAddInsConf" runat="server" SkinID="MenuItem" OnClick="LinkButtonAddInsConf_Click"></asp:LinkButton></li>
                <li class="LiAplMenu">
                    <asp:LinkButton ID="LinkButtonFormElEdit" runat="server" SkinID="MenuItem" OnClick="LinkButtonFormElEdit_Click"></asp:LinkButton></li>
                <li  class="LiAplMenu">
                    <asp:LinkButton ID="ConfigSettingsLinkButton" runat="server" SkinID="MenuItem" OnClick="ConfigSettingsLinkButton_Click"></asp:LinkButton>
                </li>
                <div class="Clear">
                </div>
            </ul>
            <div class="Clear">
            </div>
        </div>
    </div>
</asp:Content>
