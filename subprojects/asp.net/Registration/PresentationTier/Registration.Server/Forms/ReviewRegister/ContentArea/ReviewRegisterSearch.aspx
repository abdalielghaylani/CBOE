<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" CodeBehind="ReviewRegisterSearch.aspx.cs" 
Inherits="PerkinElmer.COE.Registration.Server.Forms.ReviewRegister.ContentArea.ReviewRegisterSearch"%>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox" TagPrefix="uc1" %> 
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea" TagPrefix="uc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30" Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="SearchPagesContentTable"  cellspacing="1" cellpadding="2">
        <tr class="PagesToolBar">
           <td class="ColumnContentHeader">
           </td>
           <td align="left">
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClientClick="return goBackInFrame()" OnClick="GoHomeButton_Click" />
           </td> 
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>   
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false"/>
            </td>   
        </tr>
        <tr>
            <td colspan="3">
                <iframe id="SearchTempFrame" runat="server" frameborder="0" scrolling="no" style="width:100%;"></iframe>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
         var originalReferrer = frames['<%= SearchTempFrame.ClientID %>'].document.referrer;

         //fix for CBOE-1733     
         function goBackInFrame() {
             try {
                 if (frames['<%= SearchTempFrame.ClientID %>'].history.length > 0 && frames['<%= SearchTempFrame.ClientID %>'].document.referrer != originalReferrer) {
                                     
                     window.location = "ReviewRegisterSearch.aspx?Caller=ST";
                     return false;
                 }
             } catch (err) {
                 window.location = "ReviewRegisterSearch.aspx?Caller=ST";
             }
             return true;
         }
    </script>
</asp:Content>
