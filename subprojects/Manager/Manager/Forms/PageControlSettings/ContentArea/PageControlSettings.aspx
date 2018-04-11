<%@ Page Language="C#" MasterPageFile="~/Forms/Master/SecurityManager.Master" AutoEventWireup="true" CodeBehind="PageControlSettings.aspx.cs" Inherits="Manager.Forms.PagerControlSettings.PageControlSettings" Title="Untitled Page" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEPageCtrlSettingManager"
    TagPrefix="COECntrl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div runat="server" id="CtrlContainer">
        <coecntrl:COEPageCtrlSettingManager id="COEPageCtrlSettingManager1" runat="server" Height="100%" Width="100%">
        </coecntrl:COEPageCtrlSettingManager>   
     </div> 
</asp:Content>
