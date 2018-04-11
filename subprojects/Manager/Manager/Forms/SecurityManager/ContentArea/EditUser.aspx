<%@ Page Language="C#" AutoEventWireup="true"  EnableEventValidation="false" Inherits="Forms_ContentArea_EditUser" Codebehind="EditUser.aspx.cs" MasterPageFile="~/Forms/Master/SecurityManager.Master"%>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.Master" %>
<%@ Register Src="../UserControls/EditUser.ascx" TagName="EditUserUC" TagPrefix="uc1" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server" >  
    <table class="PagesContentTable">
        <tr>
            <td align="center">
               <COEManager:ConfirmationArea id="ConfirmationAreaUserControl" runat="server" visible="false" />
          </td>
        </tr>
        <tr>
            <td >
            <asp:UpdatePanel ID="ErrorAreaUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                <COEManager:ErrorArea id="ErrorAreaUserControl" runat="server" visible="false"/>
                </ContentTemplate></asp:UpdatePanel>
            </td>
        </tr>
       <tr>
            <td>
                <uc1:EditUserUC id="EditUserControl" runat="server"></uc1:EditUserUC>
            </td>
       </tr>
       <tr>
            <td>
               
            </td>
       </tr>
    </table>
</asp:Content>


