<%@ Page Language="C#" AutoEventWireup="true"  Inherits="Manager.Forms.ConfigControlSettings.ConfigControlSettings" Codebehind="ConfigControlSettings.aspx.cs" MasterPageFile="~/Forms/Master/MasterPage.Master" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEConfigSettingManager"
    TagPrefix="COECntrl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div runat="server" id="CtrlContainer">
        <COECntrl:COEConfigSettingManager id="COEConfigSettingManager" runat="server" Height="100%" Width="100%" CurrentApplicationName="<%= Utilities.GetApplicationName() %>" SkinID="AppSettingsUltraWebTabSkin">
        </COECntrl:COEConfigSettingManager>   
     </div> 
</asp:Content>