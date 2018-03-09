<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DataViewManager.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Manager.Forms.DataViewManager.ContentArea.About" Title="Untitled Page" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="PublicContentContainer">
        <table width="100%" class="PagesContentTable">
            <tr>
                <td align="center" colspan="2">
                    <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
                </td> 
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false"/>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <div>About ChemBioOffice Enterprise</div>
                    <p class="version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
                    <p class="securitymode">Single SignOn URL:  <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetSingleSignOnURL()%></p>
                    <p class="securitymode">Single SignOn Default Provider:  <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDefaultSingleSignOnProvider()%></p>
                    <p class="copyright"><%=Resources.Resource.CopyrightInfo%></p>
                </td>
                <td align="right" valign="bottom" >
                    <p class="copyright"><%=Resources.Resource.PKI_ABOUT_TEXT%></p>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
