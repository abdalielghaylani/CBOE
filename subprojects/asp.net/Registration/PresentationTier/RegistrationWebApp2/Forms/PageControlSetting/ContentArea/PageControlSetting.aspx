<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Forms/Master/Registration.Master" CodeBehind="PageControlSetting.aspx.cs" Inherits="RegistrationWebApp2.Forms.PageControlSetting.ContentArea.PageControlSetting" EnableSessionState="True" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEPageCtrlSettingManager"
    TagPrefix="COECntrl" %>
<asp:Content ID="CompoundDetails" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
     <div>
        <coecntrl:COEPageCtrlSettingManager id="COEPageCtrlSettingManager1" runat="server" Height="100%" Width="100%">
        </coecntrl:COEPageCtrlSettingManager>
      </div>   
</asp:Content>


