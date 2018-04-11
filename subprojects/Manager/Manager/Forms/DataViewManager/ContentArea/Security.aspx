<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_Security" Codebehind="Security.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/Security.ascx" TagName="SecurityUC" TagPrefix="uc1" %>
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
                <uc1:SecurityUC id="SecurityUserControl" runat="server"></uc1:SecurityUC>
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


