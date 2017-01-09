<%@ Page Language="C#" ValidateRequest="false" MasterPageFile="~/Forms/Master/Registration.Master"
    AutoEventWireup="true" Codebehind="AddIns.aspx.cs" Inherits="PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea.AddIns"
    Title="Untitled Page" %>

<%@ Register Src="../UserControls/AddInList.ascx" TagName="AddInList" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader">Manage AddIns</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <uc1:AddInList ID="AddInList1" runat="server"></uc1:AddInList>
    </div>
</asp:Content>
