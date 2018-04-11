<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTags.aspx.cs" Inherits="EditTags" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/EditTags.ascx" TagName="EditTags" TagPrefix="uc1" %>
<asp:Content Id="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server" >
    <table class="PagesContentTable">
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
                <uc1:EditTags id="EditTagsUserControl" runat="server">
                </uc1:EditTags>
            </td>
        </tr>
    </table>
</asp:Content>