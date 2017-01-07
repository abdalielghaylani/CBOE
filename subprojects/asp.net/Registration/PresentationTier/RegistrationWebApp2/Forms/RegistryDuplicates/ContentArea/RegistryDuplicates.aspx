<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="RegistryDuplicates.aspx.cs" Inherits="RegistrationWebApp2.Forms.RegistryDuplicates.RegistryDuplicates"
    Title="Untitled Page" ValidateRequest="false" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left">
                <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" UseSubmitBehavior="false" />
                <asp:Button ID="AddBatchButton" runat="server" OnClick="AddBatchButton_Click" />
                <asp:Button ID="DuplicateButton" runat="server" OnClick="DuplicateButton_Click" />
                <asp:Button ID="SkipButton" runat="server" Visible="False" />
                <asp:Button ID="DoneButton" runat="server" Visible="False" OnClick="DoneButton_Click" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" CssClass="ContentPageTitle" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center" style="padding-top:10px;padding-bottom:10px">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" />
            </td>
        </tr>
        <tr style="height:15px;"></tr>
        <tr><td  colspan="3" align="left" style="padding-bottom:10px;">
                <asp:Label ID="SubmitterCommentsLabel"  runat="server" ForeColor="red" Width="150px"></asp:Label>
        <asp:TextBox ID="SubmitterComments"  runat="server" Width="550px" ></asp:TextBox><asp:HiddenField ID="SaltandbatchSuffixhid" runat="server" /></td></tr>
        
        <tr>
            <td colspan="3" align="center">
                <asp:LinkButton ID="PreviousLinkButton" runat="server" OnClick="PreviousLinkButton_Click"
                    SkinID="BackDupResolution"></asp:LinkButton>&nbsp;&nbsp;
                <asp:LinkButton ID="NextLinkButton" runat="server" OnClick="NextLinkButton_Click"
                    SkinID="NextDupResolution"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:PlaceHolder ID="duplicateFormHolder" runat="server"></asp:PlaceHolder>
                <cc1:CslaDataSource ID="mixtureCslaDataSource" runat="server" OnSelectObject="mixtureCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.RegistryRecord">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchListCslaDataSource" runat="server" OnSelectObject="BatchListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchCslaDataSource" runat="server" OnSelectObject="BatchCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.Batch">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ChemistsCslaDataSource" runat="server" OnSelectObject="ChemistsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.ChemistNameValueList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ProjectsCslaDataSource" runat="server" OnSelectObject="ProjectsCslaDataSource_SelectObject">
                </cc1:CslaDataSource>
                <!-- Only return the active projects sorted by userID and registry type -->
                <cc1:CslaDataSource ID="RegistryProjectsCslaDataSource" runat="server" OnSelectObject="RegistryProjectsCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Only return the active projects sorted by userID and batch type -->
                <cc1:CslaDataSource ID="BatchProjectsCslaDataSource" runat="server" OnSelectObject="BatchProjectsCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Mixture Identifiers grid -->
                <cc1:CslaDataSource ID="RegistryIdentifiersCslaDataSource" runat="server" OnSelectObject="RegistryIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Compound Identifiers grid -->
                <cc1:CslaDataSource ID="CompoundIdentifiersCslaDataSource" runat="server" OnSelectObject="CompoundIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Batch Identifiers grid -->
                <cc1:CslaDataSource ID="BatchIdentifiersCslaDataSource" runat="server" OnSelectObject="BatchIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ComponentListCslaDataSource" runat="server" OnSelectObject="ComponentListCslaDataSource_SelectObject">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that displays the list of availables sequences -->
                <cc1:CslaDataSource ID="SequenceListCslaDataSource" runat="server" OnSelectObject="SequenceListCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.SequenceList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Project in the Mixture Projetcs grid -->
                <cc1:CslaDataSource ID="IdentifiersCslaDataSource" runat="server" OnSelectObject="IdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="StructureIdentifiersCslaDataSource" runat="server" OnSelectObject="StructureIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
