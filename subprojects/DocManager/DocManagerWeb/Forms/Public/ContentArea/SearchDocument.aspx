<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DocManager.Master" AutoEventWireup="true"
    Codebehind="SearchDocument.aspx.cs" Inherits="DocManagerWeb.Forms.Public.ContentArea.SearchDocument" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v6.3, Version=6.3.20063.53, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" style="margin-left:2px;">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader"></td>
            <td align="left">
                <asp:Button ID="GoHomeButton" runat="server" CausesValidation="False" OnClick="GoHomeButton_Click" UseSubmitBehavior="true" OnClientClick="return goBackInFrame()" />
           </td> 
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinId="PageTitleLabel" />
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false"/>
            </td>   
        </tr>
         <tr>
            <td colspan="3">
                <iframe id="SearchFrame" runat="server" frameborder="0" scrolling="no" style="width:100%;"></iframe>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
    var originalReferrer = frames['<%= SearchFrame.ClientID %>'].document.referrer;
    function goBackInFrame()
    {
        if(frames['<%= SearchFrame.ClientID %>'].history.length > 0 && frames['<%= SearchFrame.ClientID %>'].document.referrer != originalReferrer)
        {
            frames['<%= SearchFrame.ClientID %>'].history.back();
            return false;
        }

        return true;
    }
    </script>
</asp:Content>
