<%@ Page Language="C#" MasterPageFile="~/Forms/Master/SecurityManager.Master" AutoEventWireup="true" CodeBehind="TableEditor.aspx.cs" Inherits="Manager.Forms.TableEditor.ContentArea.TableEditor" %>
<%@ MasterType VirtualPath="~/Forms/Master/SecurityManager.master" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COETableManager"
    TagPrefix="COECntrl" %>
    
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" width="100%">
        <tr class="PagesToolBar">
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="Title"></asp:Label>
            </td>   
        </tr>
        <tr>
            <td colspan="2" align="center">
                <coecntrl:coetablemanager id="COETableManager1" runat="server" pagesize="10" Width="100%"></coecntrl:coetablemanager>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
