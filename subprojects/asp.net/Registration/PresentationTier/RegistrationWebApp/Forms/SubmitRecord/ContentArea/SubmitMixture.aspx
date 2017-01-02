<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="SubmitMixture.aspx.cs" Inherits="RegistrationWebApp.Forms.SubmitRecord.ContentArea.SubmitMixturePage" ValidateRequest="false" %>

<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuButton.ascx" TagName="MenuButton"
    TagPrefix="uc4" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem"
    TagPrefix="uc4" %>    
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<script language="javascript" type="text/javascript">
    var gridsArray = new Array();
    function clearGridsIfDefined()
    {   
        if(typeof(clearWebGrids) != 'undefined')
            clearWebGrids();            
        return false;
    }
</script>
    <table class="PagesContentTable" cellspacing="1" cellpadding="2">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left"  style="white-space:nowrap; padding-top: 10px;" class="MainButtonContainer">
                <asp:Button SkinID="ButtonBig" ID="CancelButton" runat="server" OnClientClick="return ConfirmCancel();"
                    CausesValidation="false" OnClick="CancelButton_Click" />
                <asp:Button SkinID="ButtonBig" ID="ClearFormButton" runat="server" CausesValidation="false" UseSubmitBehavior="false"
                    OnClientClick="var result = coe_clearForm();clearGridsIfDefined();return result;" />
                <asp:Button SkinID="ButtonBig" ID="BackButton" runat="server" class="ToolbarButton"
                    OnClick="BackButton_Click" Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
                
                <asp:Button SkinID="ButtonBig" ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" />
                <asp:Button SkinID="ButtonBig" ID="RegisterButton" runat="server" OnClick="RegisterButton_Click" />
                <asp:Button SkinID="ButtonBig" ID="DoneButton" runat="server" OnClick="DoneButton_Click"
                    Visible="false" CausesValidation="false" UseSubmitBehavior="false" />
                <asp:Button SkinID="ButtonBig" ID="SavePreferenceButton" runat="server" OnClick="SavePreferenceButton_Click"
                    Visible="false" CausesValidation="false" />
            </td>
            <td align="right">
                <asp:Label SkinID="PageTitleLabel" ID="PageTitleLabel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="ComponentToolBar" class="MenuToolBar">
            <td colspan="3">
                <div style="float:right;">
                    <uc4:MenuButton ID="SubmissionTemplateImageMenuButton" runat="server" CausesValidation="false" Text="Submission Templates" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/table.png">
                        <MenuItemList>
                            <uc4:MenuItem CommandName="SaveTemplate" CausesValidation="true" />
                            <uc4:MenuItem CommandName="LoadTemplate" CausesValidation="false" />
                        </MenuItemList>
                    </uc4:MenuButton>
                    <uc4:MenuButton ID="DefineMixtureImageMenuButton" runat="server" CausesValidation="false" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/Generators_2.png">
                        <MenuItemList>
                            <uc4:MenuItem CommandName="AddComponent" />
                            <uc4:MenuItem CommandName="SearchComponent" />
                            <uc4:MenuItem CommandName="DeleteComponent" />
                            <uc4:MenuItem CommandName="ContinueToBatch" />
                        </MenuItemList>
                    </uc4:MenuButton>
                    <asp:HiddenField ID="ActionToDoHiddenField" runat="server" Value="null" />
                    <asp:HiddenField ID="ComponentIndexHiddenField" runat="server" Value="null" />
                </div>
            </td>
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="height: 21px;background-color: #CFD8E6;">
                <!--<br />
                <br />-->
                <asp:PlaceHolder ID="mixtureInformationHolder" runat="server"></asp:PlaceHolder>
                <uc1:MessageBox ID="ConfirmationMessageBox" runat="server" OnBtn1Clicked="ConfirmationMessageBox_BtnYesClicked"
                    OnBtn2Clicked="ConfirmationMessageBox_BtnNoClicked" />
                <cc1:CslaDataSource ID="mixtureCslaDataSource" runat="server" OnSelectObject="mixtureCslaDataSource_SelectObject"
                    OnUpdateObject="mixtureCslaDataSource_UpdateObject" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.RegistryRecord">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchListCslaDataSource" runat="server" OnSelectObject="BatchListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchList"
                    OnUpdateObject="BatchListCslaDataSource_UpdateObject">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="BatchComponentListCslaDataSource" runat="server" OnSelectObject="BatchComponentListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchComponentList"
                    OnUpdateObject="BatchComponentListCslaDataSource_UpdateObject">
                </cc1:CslaDataSource>
                <cc1:CslaDataSource ID="ComponentListCslaDataSource" runat="server" OnSelectObject="ComponentListCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.ComponentList"
                    OnUpdateObject="ComponentListCslaDataSource_UpdateObject" TypeSupportsPaging="true">
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
                <!-- Only return the active projects sorted by userID -->
                <cc1:CslaDataSource ID="ProjectsCslaDataSource" runat="server" OnSelectObject="ProjectsCslaDataSource_SelectObject" />
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
                <cc1:CslaDataSource ID="BatchComponentFragmentsCslaDataSource" runat="server" OnSelectObject="BatchComponentFragmentsCslaDataSource_SelectObject"
                    TypeAssemblyName="CambridgeSoft.COE.Registration.Services" TypeName="CambridgeSoft.COE.Registration.Services.Types.BatchComponent">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Project in the Mixture Projetcs grid -->
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
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Structure Identifiers grid -->
                <cc1:CslaDataSource ID="StructureIdentifiersCslaDataSource" runat="server" OnSelectObject="StructureIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that is a edit control for the column Identifiers in the Batch Identifiers grid -->
                <cc1:CslaDataSource ID="BatchIdentifiersCslaDataSource" runat="server" OnSelectObject="BatchIdentifiersCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.IdentifiersList">
                </cc1:CslaDataSource>
                <!-- Datasource for the dropdown that displays the list of availables sequences -->
                <cc1:CslaDataSource ID="SequenceListCslaDataSource" runat="server" OnSelectObject="SequenceListCslaDataSource_SelectObject"
                    TypeSupportsSorting="true" TypeAssemblyName="CambridgeSoft.COE.Registration.Services"
                    TypeName="CambridgeSoft.COE.Registration.Services.Types.SecuenceList">
                </cc1:CslaDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="structureToolbarOption" runat="server" ClientIDMode="Static" />
</asp:Content>
