<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ViewDocument.aspx.cs" Inherits="DocManagerWeb.Forms.Public.ContentArea.ViewDocument"
    MasterPageFile="~/Forms/Master/DocManager.Master" %>

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
<%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<script language="javascript" type="text/javascript">
    function coe_clearForm(formId) 
    {        
        var form, elements, i, elm;
        
        form = document.getElementById ? document.getElementById(formId) : document.forms[formId];
        
        if (document.getElementsByTagName) {
            elements = form.getElementsByTagName('input');
            for (i = 0, elm; elm = elements.item(i++); ) {
                if (elm.getAttribute('type') == "text" && elm.readOnly == false)
                    elm.value = '';
                if (elm.getAttribute('type') == "checkbox" && elm.disabled == false)
                    elm.checked = false;
            }
            elements = form.getElementsByTagName('select');
            for (i = 0, elm; elm = elements.item(i++); ) {
                if (elm.disabled == false)
                    elm.options.selectedIndex = 0;
            }

            elements = form.getElementsByTagName('textarea');
            for (i = 0, elm; elm = elements.item(i++); ) {
                elm.value = '';
            }
        }
        else {
            elements = form.elements;
            for (i = 0, elm; elm = elements[i++]; ) {
                if (elm.type == "text" || elm.type == "textarea" && elm.readOnly == false) {
                    elm.value = '';
                }
                else if (elm.type == "select" && elm.disabled == false) {
                    elm.options.selectedIndex = 0;
                }
            }
        }
        return false;
    }
    function ConfirmCancel() 
    {
        return window.confirm('Are you sure you want to cancel? You will lose all the changes.');
    }
</script>
    <table class="PagesContentTable" cellspacing="1" cellpadding="2">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left" style="white-space: nowrap; padding-top: 10px;" class="MainButtonContainer">
                <asp:Button SkinID="ButtonBig" ID="CancelButton" runat="server" OnClientClick="return ConfirmCancel();"
                    CausesValidation="false" OnClick="CancelButton_Click" />
                <asp:Button SkinID="ButtonBig" ID="ClearFormButton" runat="server" CausesValidation="false"
                    UseSubmitBehavior="false" OnClientClick="var result = coe_clearForm('aspnetForm');return result;" />
                <asp:Button SkinID="ButtonBig" ID="BackButton" runat="server" OnClick="BackButton_Click" 
                    Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
                <asp:Button SkinID="ButtonBig" ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" />
                <asp:Button SkinID="ButtonBig" ID="EditButton" runat="server" OnClick="EditButton_Click"
                    Visible="false" CausesValidation="false" UseSubmitBehavior="false" />
                <asp:Button SkinID="ButtonBig" ID="DoneButton" runat="server" OnClick="DoneButton_Click"
                    Visible="false" CausesValidation="false" UseSubmitBehavior="false" />
            </td>
            <td align="right">
                <asp:Label SkinID="PageTitleLabel" ID="PageTitleLabel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="height: 21px; background-color: #CFD8E6;">
                <asp:PlaceHolder ID="viewDocInformationHolder" runat="server"></asp:PlaceHolder>
                <uc1:MessageBox ID="ConfirmationMessageBox" runat="server" OnBtn1Clicked="ConfirmationMessageBox_BtnYesClicked"
                    OnBtn2Clicked="ConfirmationMessageBox_BtnNoClicked" />
                <cc1:CslaDataSource ID="documentCslaDataSource" runat="server" TypeAssemblyName="CambridgeSoft.COE.DocumentManager.Services"
                    TypeName="CambridgeSoft.COE.DocumentManager.Services.Types.Document" OnSelectObject="documentCslaDataSource_SelectObject"
                    OnUpdateObject="documentCslaDataSource_UpdateObject">
                </cc1:CslaDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
