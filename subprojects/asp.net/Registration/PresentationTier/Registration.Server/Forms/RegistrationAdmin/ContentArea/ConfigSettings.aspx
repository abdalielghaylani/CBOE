<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" CodeBehind="ConfigSettings.aspx.cs" Inherits="PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea.ConfigSettings" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEConfigSettingManager"
    TagPrefix="COECntrl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
     <div class="Content" >
        <div style="padding:6px;">
            <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click" CausesValidation="false">Back</asp:LinkButton>
        </div>
        <div>
            <COECntrl:COEConfigSettingManager id="COEConfigSettingManager" runat="server" Height="100%" Width="100%" SkinID="AppSettingsUltraWebTabSkin" CancelButtonURL="~/Forms/RegistrationAdmin/ContentArea/Default.aspx">
            </COECntrl:COEConfigSettingManager>
        </div>
    </div>
</asp:Content>

