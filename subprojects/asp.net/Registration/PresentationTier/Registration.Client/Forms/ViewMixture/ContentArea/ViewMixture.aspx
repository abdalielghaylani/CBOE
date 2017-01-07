<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="ViewMixture.aspx.cs" Inherits="RegistrationWebApp2.Forms.RegisteredRecord.ContentArea.ViewMixture"
    Title="Untitled Page" ValidateRequest="false" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls"
    TagPrefix="cc3" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuButton.ascx" TagName="MenuButton"
    TagPrefix="uc4" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem" TagPrefix="uc4" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:Content ID="CompoundDetails" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="1" cellpadding="2" style="600">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left">
                <asp:Button ID="GoHomeButton" runat="server" Visible="False" CausesValidation="False"
                    OnClick="GoHomeButton_Click" />
                <asp:Button ID="CancelButton" runat="server" Visible="False" CausesValidation="False"
                    OnClick="CancelButton_Click" />
                <asp:Button ID="EditButton" runat="server" CausesValidation="False" OnClick="EditButton_Click" />
                <asp:Button ID="AddBatchButton" runat="server" OnClick="AddBatchButton_Click" />
                <asp:Button ID="SubmitButton" runat="server" Visible="False" OnClick="SubmitButton_Click" />
                <asp:Button ID="MoveBatchButton" runat="server" Visible="true" OnClick="MoveBatchButton_Click" />
                <asp:Button ID="DeleteBatchButton" runat="server" Visible="true" OnClick="DeleteBatchButton_Click" />
            </td>
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" CssClass="ContentPageTitle"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="ComponentToolBar" class="MenuToolBar">
            <td colspan="3">
                <div style="float: right;" id="MenuDiv">
                    <uc4:MenuButton ID="DeleteRecordImageMenuButton" runat="server" CausesValidation="false"
                        EnableMenu="false" LeftImageURL="~/App_Themes/Common/Images/Generators_2.png">
                    </uc4:MenuButton>
                    <uc4:MenuButton ID="DefineMixtureImageMenuButton" runat="server" CausesValidation="false"
                        EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/Generators_2.png">
                        <MenuItemList>
                            <uc4:MenuItem CommandName="AddComponent" />
                            <uc4:MenuItem CommandName="SearchComponent" />
                            <uc4:MenuItem CommandName="DeleteComponent" />
                        </MenuItemList>
                    </uc4:MenuButton>
                    <uc4:MenuButton ID="SendToInventoryImageMenuButton" runat="server" CausesValidation="false"
                        EnableMenu="false" LeftImageURL="/COECommonResources/ChemInv/flask_closed_icon_16.gif">
                    </uc4:MenuButton>
                    <asp:HiddenField ID="ActionToDoHiddenField" runat="server" Value="null" />
                    <asp:HiddenField ID="ComponentIndexHiddenField" runat="server" Value="null" />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr id="ChemBioPluginContainer" style="display:block;">
            <td colspan="3">
                <br />
                <br />
                <asp:PlaceHolder ID="mixtureInformationHolder" runat="server"></asp:PlaceHolder>
                <uc1:MessageBox ID="ConfirmationMessageBox" runat="server" OnBtn1Clicked="ConfirmationMessageBox_YesClicked"
                    OnBtn2Clicked="ConfirmationMessageBox_NoClicked"></uc1:MessageBox>
                <uc1:MessageBox ID="DuplicatesMessageBox" runat="server" OnBtn1Clicked="DuplicatesMessageBox_OkClicked"
                    OnBtn2Clicked="DuplicatesMessageBox_CancelClicked"></uc1:MessageBox>
                <uc1:MessageBox ID="StoppingMessageBox" runat="server" OnBtn1Clicked="StoppingMessageBox_OkClicked">
                </uc1:MessageBox>
                <uc1:MessageBox ID="CopiesMessageBox" runat="server" OnBtn1Clicked="CopiesMessageBox_ContinueClicked"
                    OnBtn2Clicked="CopiesMessageBox_CopyClicked" OnBtn3Clicked="CopiesMessageBox_CancelClicked">
                </uc1:MessageBox>
                <cc1:CslaDataSource ID="mixtureCslaDataSource" runat="server" OnSelectObject="MixtureCslaDataSource_SelectObject"
                    OnUpdateObject="MixtureCslaDataSource_UpdateObject" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.RegistryRecord">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchListCslaDataSource" runat="server" OnSelectObject="BatchListCslaDataSource_SelectObject"
                    OnUpdateObject="BatchListCslaDataSource_UpdateObject" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchComponentListCslaDataSource" runat="server" OnSelectObject="BatchComponentListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchComponentList"
                    OnUpdateObject="BatchComponentListCslaDataSource_UpdateObject">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ComponentListCslaDataSource" runat="server" OnSelectObject="ComponentListCslaDataSource_SelectObject"
                    OnUpdateObject="ComponentListCslaDataSource_UpdateObject" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ComponentList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ChemistsCslaDataSource" runat="server" OnSelectObject="ChemistsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.ChemistNameValueList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="FragmentsCslaDataSource" runat="server" OnSelectObject="FragmentsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchComponent"
                    OnUpdateObject="FragmentsCslaDataSource_UpdateObject">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="FragmentTypesCslaDataSource" runat="server" OnSelectObject="FragmentTypesCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.FragmentNameValueList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="FragmentNamesCslaDataSource" runat="server" OnSelectObject="FragmentNamesCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.FragmentList">
                </cc1:CslaDataSource>
                <!-- CSBR 141834 : Adding Datasource that fetches all project IDs - active, inactive, registry,batch for use in View Mode -->
                <cc1:CslaDataSource ID="AllProjectsCslaDataSource" runat="server" OnSelectObject="AllProjectsCslaDataSource_SelectObject" 
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Project in the Mixture Projetcs grid -->
                <cc1:CslaDataSource ID="ProjectsCslaDataSource" runat="server" OnSelectObject="ProjectsCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.ProjectsList">
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
                <cc1:CslaDataSource ID="IdentifiersCslaDataSource" runat="server" OnSelectObject="IdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
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
                <cc1:CslaDataSource ID="BatchComponentFragmentsCslaDataSource" runat="server" OnSelectObject="BatchComponentFragmentsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchComponent">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="DocMgrDataSource" runat="server" OnSelectObject="DocMgrDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.DocumentList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that displays the list of availables sequences -->
                <cc1:CslaDataSource ID="SequenceListCslaDataSource" runat="server" OnSelectObject="SequenceListCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.SecuenceList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="StructureIdentifiersCslaDataSource" runat="server" OnSelectObject="StructureIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="InvContainersDataSource" runat="server" OnSelectObject="InvContainersDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.InvContainerList">
                </cc1:CslaDataSource>
            </td>
        </tr>
    </table>
    <cc3:COEModalIframe runat="server" BodyURL="" ID="ModalFrame" ControlIDToBind="" HeaderText="" PaneInsideHeight="670" PaneInsideWidth="790" ModalPanelSettings="modal: true, width: '810px', height: '720px', context: ['MenuDiv', 'tr', 'br'] "></cc3:COEModalIframe>
    <asp:HiddenField ID="structureToolbarOption" runat="server" ClientIDMode="Static" />
</asp:Content>
