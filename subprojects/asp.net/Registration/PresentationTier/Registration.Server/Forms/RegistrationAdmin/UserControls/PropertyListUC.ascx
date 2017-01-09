<%@ Control Language="C#" AutoEventWireup="True" Codebehind="PropertyListUC.ascx.cs"
    Inherits="PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls.PropertyListUC" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="ValidationRules.ascx" TagName="ValidationRules" TagPrefix="uc1" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<div class="TabsContent">
    <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click"
        CausesValidation="false" Style="margin-right: 40px; margin-bottom: 20px">Back</asp:LinkButton>
    <asp:LinkButton ID="LinkButtonCustomForms" runat="server" CssClass="Goto2" OnClick="LinkButtonCustomForms_Click"
        CausesValidation="false" Style="margin-right: 40px; margin-bottom: 20px">Customize Form</asp:LinkButton>
    <div style="margin-top: 20px; text-align: center">
        <asp:ValidationSummary ID="ValidationSummary" runat="server" />
    </div>
    <table class="MessageTableRegAdmin">
        <tr>
            <td colspan="4" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
    </table>
    <fieldset class="FieldsetTabsContent" style="margin-top: 20px">
        <legend class="LegendTabsContent">New Property</legend>
        <div class="DivFieldset">
            <div class="Div">
                <asp:Label ID="LblName" runat="server" CssClass="LabelFieldset"></asp:Label>
                <asp:TextBox ID="TxtName" runat="server" CssClass="ControlElements"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorName" runat="server"
                    ControlToValidate="TxtName"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredTxtName" ControlToValidate="TxtName" runat="server"
                    EnableClientScript="true"></asp:RequiredFieldValidator>
            </div>
            <div class="Div">
                <asp:Label ID="LabelLabel" runat="server" CssClass="LabelFieldset"></asp:Label>
                <asp:TextBox ID="TextBoxLabel" runat="server" CssClass="ControlElements"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorLabel" runat="server"
                    ControlToValidate="TextBoxLabel"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredTxtLabel" ControlToValidate="TextBoxLabel" runat="server"
                    EnableClientScript="true"></asp:RequiredFieldValidator>
            </div>
            <div class="Div">
                <asp:Label ID="LblType" runat="server" CssClass="LabelFieldset"></asp:Label>
                <asp:DropDownList ID="DdlType" runat="server" CssClass="ControlElements" onchange="ShowHidePrecisionTextBox(this.value)">
                </asp:DropDownList>
            </div>
            <div id="PrecisionDiv" runat="server" class="Div">
                <asp:Label ID="LblPrecision" runat="server" CssClass="LabelFieldset"></asp:Label>
                <asp:TextBox ID="TextBoxIntegerPrecision" runat="server" CssClass="ControlElements"></asp:TextBox>
                <asp:Label ID="LabelDecimalDot" runat="server">
                </asp:Label>
                <asp:TextBox ID="TextBoxDecimalPrecision" runat="server" CssClass="NumericInput"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorInteger" runat="server"
                    ControlToValidate="TextBoxIntegerPrecision"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredIntegerPrecision" ControlToValidate="TextBoxIntegerPrecision"
                    runat="server" EnableClientScript="true"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorDecimal" runat="server"
                    ControlToValidate="TextBoxDecimalPrecision"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredDecimalPrecision" Enabled="false" ControlToValidate="TextBoxDecimalPrecision"
                    runat="server" EnableClientScript="true"></asp:RequiredFieldValidator>
            </div>
            <div id="PickListDomainsDiv" runat="server" class="Div">
                <asp:Label ID="LabelPickListDomains" runat="server" CssClass="LabelFieldset"></asp:Label>
                <asp:DropDownList ID="DropDownListPickListDomains" runat="server" CssClass="ControlElements">
                </asp:DropDownList>
            </div>
            <div class="Clear">
            </div>
        </div>
        <div style="text-align: right; margin-top: 20px; margin-bottom: 20px; margin-right: 20px">
            <asp:Button ID="BtnAddProperty" runat="server" SkinID="ButtonBigRegAdmin" OnClick="BtnAddProperty_Click" />
        </div>
    </fieldset>
    <p class="PTabsContent">
    </p>
    <fieldset class="FieldsetTabsContent">
        <legend class="LegendTabsContent">Property List</legend>
        <div>
            <div style="float: right; margin-right: 10px">
                <asp:LinkButton ID="UpPropertyButton" runat="server" CausesValidation="false" OnClick="UpPropertyButton_Click">
                    <img src="../../../App_Themes/Blue/Images/Arrow_Up.PNG" id="UpImg" runat="server" /></asp:LinkButton></div>
            <asp:RadioButtonList ID="Rbl_Properties" runat="server" Style="margin-top: 20px;
                margin-top: 20px; margin-left: 20px">
            </asp:RadioButtonList>
            <div style="float: right; margin-right: 10px">
                <asp:LinkButton ID="DownPropertyButton" CausesValidation="false" runat="server" OnClick="DownPropertyButton_Click">
                    <img src="../../../App_Themes/Blue/Images/Arrow_Down.PNG" id="DownImg" runat="server" /></asp:LinkButton></div>
        </div>
        <div style="text-align: right; margin-top: 20px; margin-bottom: 20px; margin-right: 20px">
            <asp:Button ID="Button_ValidationRules" runat="server" SkinID="ButtonBigRegAdmin"
                CausesValidation="false" OnClick="Button_ValidationRules_Click" />
            <asp:Button ID="ButtonEditProperty" runat="server" SkinID="ButtonBigRegAdmin" CausesValidation="false"
                OnClick="ButtonEditProperty_Click" />
            <asp:Button ID="BtnDeleteProp" runat="server" SkinID="ButtonBigRegAdmin" CausesValidation="false"
                OnClick="BtnDeleteProp_Click" />
        </div>
    </fieldset>
    <div class="PTabsContent">
        <div style="text-align: right; margin-right: 40px">
            <asp:Button ID="BtnSave" CausesValidation="false" runat="server" OnClick="BtnSave_Click"
                SkinID="ButtonBigRegAdmin" />
        </div>
    </div>
    <div class="Clear">
    </div>
    <div class="TabsFooter">
        <div class="DivFooter">
        </div>
    </div>
</div>
