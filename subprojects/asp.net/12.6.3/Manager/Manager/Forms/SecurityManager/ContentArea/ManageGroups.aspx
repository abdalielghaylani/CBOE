<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_ManageGroups" Codebehind="ManageGroups.aspx.cs" EnableEventValidation="false" %>
<%@ Register Src="../UserControls/ManageGroups.ascx" TagName="ManageGroupsUC" TagPrefix="uc1" %>
    
    
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
                <uc1:ManageGroupsUC id="ManageGroupsControl" runat="server"></uc1:ManageGroupsUC>
            </td>
       </tr>
       <tr>
            <td>
               <COEManager:ImageButton ID="DoneImageButton" runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Done"></COEManager:ImageButton>
            </td>
       </tr>
    </table>
</asp:Content>
