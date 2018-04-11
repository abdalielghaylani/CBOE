<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_Help" Codebehind="Help.aspx.cs" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <div class="PublicContentContainer">
        <table width="100%" class="PagesContentTable">
            <tr>
                <td align="center">
                    <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td align="center">
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
