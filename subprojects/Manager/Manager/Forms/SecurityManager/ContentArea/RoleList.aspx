<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_RoleList" Codebehind="RoleList.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master"%>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/RoleList.ascx" TagName="RoleListUC" TagPrefix="uc1" %>

<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
            </td>
        </tr>
       <tr>
            <td>
                <uc1:RoleListUC id="ManageUsersControl" runat="server"></uc1:RoleListUC>
            </td>
       </tr>
       <tr>
            <td>
                <COEManager:ImageButton ID="CancelImageButton" runat="server" OnClientClick="return ConfirmCancel();" CausesValidation="false" ButtonMode="ImgAndTxt" TypeOfButton="Cancel"/>&nbsp;&nbsp;
            </td>
       </tr>
    </table>
</asp:Content>


