<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DataViewManager.Master" AutoEventWireup="true" CodeBehind="AddTable.aspx.cs" Inherits="Manager.Forms.DataViewManager.ContentArea.AddTable" Title="Add Table" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/AddTable.ascx" TagName="AddTable" TagPrefix="uc1" %>

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
                <uc1:AddTable id="AddTableUserControl" runat="server">
                </uc1:AddTable>
            </td>
        </tr>
    </table>
</asp:Content>