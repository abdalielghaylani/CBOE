<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" Inherits="RegistrationWebApp.Forms.Public.ContentArea.Messages" EnableTheming="true" Codebehind="Messages.aspx.cs" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="CompoundDetails" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" CssClass="ContentPageTitle"></asp:Label>
            </td>
        </tr>
        <tr align="center">
            <td>
                <br/>
                <asp:Label runat="server" ID="MessageLabel" Width="100%"></asp:Label>
            </td>
        </tr>
        <tr align="center">
            <td>
                <br/>
               <input type="button" runat="server" ID="GoBackButton" class="ButtonBig"/>
            </td>
        </tr>
     </table>
</asp:Content>
