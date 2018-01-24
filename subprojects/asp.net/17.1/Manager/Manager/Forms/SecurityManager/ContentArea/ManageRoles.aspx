<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_ManageRoles" Codebehind="ManageRoles.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master"%>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/ManageRoles.ascx" TagName="ManageRolesUC" TagPrefix="uc1" %>

<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
            <asp:UpdatePanel ID="ErrorAreaUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
                </ContentTemplate></asp:UpdatePanel>
            </td>
        </tr>
       <tr>
            <td>
                <uc1:ManageRolesUC id="ManageRolesControl" runat="server"></uc1:ManageRolesUC>
               
            </td>
       </tr>
       <tr>
            <td align="right">
               <div id="FooterButton">
               <COEManager:ImageButton ID="DoneImageButton" runat="server" ImageCssClass="Footer" ButtonCssClass="Footer2" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png"  TypeOfButton="Done"/>
               </div>
            </td>
       </tr>
    </table>
</asp:Content>
