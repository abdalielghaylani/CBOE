<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Forms/Master/Registration.Master" Inherits="RegistrationWebApp.Forms.Public.ContentArea.About" Codebehind="About.aspx.cs" %>
<asp:Content ID="CompoundDetails" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td colspan="2" align="right">
                <asp:Label ID="PageTitleLabel" runat="server" CssClass="ContentPageTitle"></asp:Label>
            </td>
        </tr>
     </table>
     <div class="AboutTopContainer">
        <table border="0" cellpadding="10" cellspacing="10">
            <tr>
                <td colspan="2">
                    <asp:Image runat="server" ImageUrl="~/App_Themes/Common/Images/CSoft_logo.jpg" />
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label runat="server" ID="SupportTitleLabel"></asp:Label>
                </td>
                <td align="left">
                    <asp:Label runat="server" ID="SupportContentLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                     <asp:Label runat="server" ID="OrdersTitleLabel"></asp:Label>
                </td>
                <td>
                     <asp:Label runat="server" ID="OrdersContentLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="VersionTitileLabel"></asp:Label>
                </td>
                <td>
                    <asp:Label runat="server" ID="VersionContentLabel"></asp:Label>
                </td>
            </tr>
        </table>
     </div>
     <br />
</asp:Content>