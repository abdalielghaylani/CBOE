<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" CodeBehind="ViewMixtureSearch.aspx.cs" Inherits="RegistrationWebApp2.Forms.ReviewRegister.ContentArea.ViewMixtureSearch" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" style="margin-left:2px;">
        <tr class="PagesToolBar">
            <!--<td class="ColumnContentHeader"></td> this pushes the back button over and is not useful-->
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
                <iframe id="SearchPermFrame" runat="server" frameborder="0" scrolling="no" style="width:100%;"></iframe>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
    var originalReferrer = frames['<%= SearchPermFrame.ClientID %>'].document.referrer;
    function goBackInFrame()
    {
        try{
       
        if(frames['<%= SearchPermFrame.ClientID %>'].history.length > 0 && frames['<%= SearchPermFrame.ClientID %>'].document.referrer != originalReferrer)
        {
            //frames['<%= SearchPermFrame.ClientID %>'].history.back(0);
            //fix for 155698
             window.location="ViewMixtureSearch.aspx";
            return false;
        }
        }catch(err){
            window.location="ViewMixtureSearch.aspx";
        }

        return true;
    }
    </script>
</asp:Content>
