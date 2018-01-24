<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_SelectBaseTable" Codebehind="SelectBaseTable.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/SelectBaseTable.ascx" TagName="SelectBaseTable"
    TagPrefix="uc1" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" ScriptMode="Release"/>
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
        <tr valign="top">
            <td align="center">
                <uc1:SelectBaseTable ID="SelectBaseTableUserControl" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ImageButton ID="CancelImageButton" runat="server" OnClientClick="return ConfirmCancel();" CausesValidation="false" ButtonMode="ImgAndTxt" TypeOfButton="Cancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                <COEManager:ImageButton ID="NextImageButton" runat="server" TypeOfButton="Next" ButtonMode="ImgAndTxt" />
            </td>
        </tr>
    </table>
</asp:Content>


