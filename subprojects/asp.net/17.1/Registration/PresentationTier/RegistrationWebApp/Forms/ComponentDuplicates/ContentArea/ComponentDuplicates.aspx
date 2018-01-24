<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="ComponentDuplicates.aspx.cs" Inherits="RegistrationWebApp.Forms.ComponentDuplicates.ContentArea.ComponentDuplicates"
    Title="Untitled Page" EnableViewState="true" ValidateRequest="false" %>

<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="1" cellpadding="2">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left" style="white-space: nowrap; padding-top: 10px;" class="MainButtonContainer">
                <asp:Button ID="CancelButton" SkinID="ButtonBig" runat="server" OnClick="CancelButton_Click"
                    UseSubmitBehavior="false" />
                <asp:Button ID="AddBatchButton" runat="server" SkinID="ButtonBig" Visible="false"
                    OnClick="AddBatchButton_Click" />
                <asp:Button ID="DuplicateRegistryButton" runat="server" SkinID="ButtonBig" Visible="false"
                    OnClick="DuplicateRegistryButton_Click" />
                <asp:Button  ID="CreateCompoundFormButton" runat="server" SkinID="ButtonBig2" OnClick="CreateCompoundFormButton_Click" />
                <asp:Button ID="UseStructureButton" runat="server" Visible="false" SkinID="ButtonBig" style="z-index:1"
                    OnClick="UseStructureButton_Click" />
            </td>
            <td >
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel" CssClass="ComponentDuplicatePageTitle"></asp:Label>
            </td>
        </tr>
        <tr style="height: 13px;">
            <td colspan="3" align="center" style="padding-top:10px;padding-bottom:10px">
                <uc2:MessagesArea ID="MessagesAreaUserControl"   runat="server" />
            </td>
        </tr>
        <tr class="MessageAreaTable" runat="server" visible="false" id="ExplanationContainerRow">
            <td colspan="3" align="center">
                <asp:Label runat="server" ID="ExplanationMessageLabel" SkinID="PageTitleLabelSmall"
                    Visible="false"></asp:Label>
            </td>
        </tr>
        <tr><td  colspan="3" align="left" style="padding-bottom:10px;">
                <asp:Label ID="SubmitterCommentsLabel"  runat="server" ForeColor="red" Width="150px"></asp:Label>
        <asp:TextBox ID="SubmitterComments"  runat="server" Width="550px" ></asp:TextBox><asp:HiddenField ID="SaltandbatchSuffixhid" runat="server" /></td></tr>
        <tr>
            <td colspan="3" align="center">
                <asp:LinkButton ID="BackButtonTop" runat="server" OnClick="BackButton_Click" SkinID="BackDupResolution"
                    Height="20px"></asp:LinkButton>&nbsp;
                <asp:LinkButton ID="NextButtonTop" runat="server" OnClick="NextButton_Click" SkinID="NextDupResolution"
                    Height="20px"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:PlaceHolder ID="duplicateRepeaterFormHolder" runat="server"></asp:PlaceHolder>
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <asp:LinkButton ID="BackLinkButton" runat="server" OnClick="BackButton_Click" SkinID="BackDupResolution"></asp:LinkButton>&nbsp;&nbsp;
                <asp:LinkButton ID="NextLinkButton" runat="server" OnClick="NextButton_Click" SkinID="NextDupResolution"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <cc1:CslaDataSource ID="DuplicatesCslaDataSource" runat="server"
                    OnSelectObject="DuplicatesCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.RegistrationWebApp"
                    TypeName="CambridgeSoft.COE.RegistrationWebApp.Forms.ComponentDuplicates.ContentArea.DuplicatesResolver">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="DuplicateCompoundsCslaDataSource" runat="server"
                    OnSelectObject="DuplicateCompoundsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ComponentList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="CurrentDuplicateCslaDataSource" runat="server"
                    OnSelectObject="CurrentDuplicateCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.Component">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="CompoundListCslaDataSource" runat="server"
                    OnSelectObject="CompoundListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.CompoundList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="DuplicateBatchesCslaDataSource" runat="server" OnSelectObject="DuplicateBatchesCslaDataSource_SelectObject" />
                <cc1:CslaDataSource ID="RootCslaDataSource" runat="server" OnSelectObject="RootCslaDataSource_SelectObject" />
                <cc1:CslaDataSource ID="StatusCslaDataSource" runat="server" OnSelectObject="StatusCslaDataSource_SelectObject" />
                <cc1:CslaDataSource ID="ChemistsCslaDataSource" runat="server" OnSelectObject="ChemistsCslaDataSource_SelectObject" />
                <cc1:CslaDataSource ID="FragmentTypesCslaDataSource" runat="server" OnSelectObject="FragmentTypesCslaDataSource_SelectObject" />
                <cc1:CslaDataSource ID="FragmentNamesCslaDataSource" runat="server" OnSelectObject="FragmentNamesCslaDataSource_SelectObject" />
                <!-- CSBR 141834 : Adding Datasource that fetches all project IDs - active, inactive, registry,batch for use in View Mode -->
                <cc1:CslaDataSource ID="AllProjectsCslaDataSource" runat="server" OnSelectObject="AllProjectsCslaDataSource_SelectObject" 
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Only return the active projects sorted by userID and registry type -->
                <cc1:CslaDataSource ID="RegistryProjectsCslaDataSource" runat="server"
                    OnSelectObject="RegistryProjectsCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Only return the active projects sorted by userID and batch type -->
                <cc1:CslaDataSource ID="BatchProjectsCslaDataSource" runat="server"
                    OnSelectObject="BatchProjectsCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Mixture Identifiers grid -->
                <cc1:CslaDataSource ID="RegistryIdentifiersCslaDataSource" runat="server"
                    OnSelectObject="RegistryIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Compound Identifiers grid -->
                <cc1:CslaDataSource ID="CompoundIdentifiersCslaDataSource" runat="server"
                    OnSelectObject="CompoundIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Structure Identifiers grid -->
                <cc1:CslaDataSource ID="StructureIdentifiersCslaDataSource" runat="server"
                    OnSelectObject="StructureIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Batch Identifiers grid -->
                <cc1:CslaDataSource ID="BatchIdentifiersCslaDataSource" runat="server"
                    OnSelectObject="BatchIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that displays the list of availables sequences -->
                <cc1:CslaDataSource ID="SequenceListCslaDataSource" runat="server"
                    OnSelectObject="SequenceListCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.SecuenceList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Project in the Mixture Projetcs grid -->
                <cc1:CslaDataSource ID="IdentifiersCslaDataSource" runat="server"
                    OnSelectObject="IdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchListCslaDataSource" runat="server"
                    OnSelectObject="BatchListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="CurrentBatchCslaDataSource" runat="server"
                    OnSelectObject="CurrentBatchCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.Batch">
                </cc1:CslaDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
