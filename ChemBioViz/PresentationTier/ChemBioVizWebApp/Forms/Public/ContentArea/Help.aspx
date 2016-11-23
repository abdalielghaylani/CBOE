<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_ContentArea_Help" Codebehind="Help.aspx.cs" MasterPageFile="~/Forms/Master/MasterPage.master" %>
<%@ MasterType VirtualPath="~/Forms/Master/MasterPage.master" %>
<%@ Register Src="../UserControls/ErrorArea.ascx" TagName="ErrorArea" TagPrefix="ChemBioViz" %>
<%@ Register Src="../UserControls/ConfirmationArea.ascx" TagName="ConfirmationArea" TagPrefix="ChemBioViz" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <table width="100%" class="PagesContentTable">
        <tr>
            <td align="center">
                <ChemBioViz:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <ChemBioViz:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
    </table>
</asp:Content>
