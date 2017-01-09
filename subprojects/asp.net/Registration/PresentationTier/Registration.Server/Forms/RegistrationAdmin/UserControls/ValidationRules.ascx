<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValidationRules.ascx.cs"
    Inherits="PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls.ValidationRules" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDateChooser.v11.1" Namespace="Infragistics.WebUI.WebSchedule"
    TagPrefix="igsch" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="COECntrl" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
    <div class="Content">
    <ul class="UlTabsHeader">
        <li class="LiFirstTabsHeader"></li>
        <li class="SelectedTabsHeader">
            <div class="LeftCornerTabsHeader">
            </div>
            <a href="#" class="ATabsHeader">Validations Rules</a>
            <div class="RigthCornerTabsHeader">
            </div>
        </li>
        <li class="LiLastTabsHeader"></li>
    </ul>
    <div class="TabsContent">
        <asp:LinkButton ID="LinkButtonBack" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click"
            CausesValidation="false">Back</asp:LinkButton>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" Style="margin-top: 10px;
            text-align: center" />
        <fieldset class="FieldsetTabsContent" style="margin-top: 20px">
            <legend class="LegendTabsContent">Property</legend>
            <div class="DivFieldset">
                <div class="Div">
                    <asp:Label ID="LlbPropName" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox ID="TxtPropName" runat="server" CssClass="ControlElements" Enabled="false"></asp:TextBox>
                </div>
                <div class="Div">
                    <asp:Label ID="LblPropType" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox ID="TxtPropType" runat="server" CssClass="ControlElements" Enabled="false"></asp:TextBox>
                </div>
                <div class="Div">
                </div>
            </div>
        </fieldset>
        <fieldset class="FieldsetTabsContent">
            <legend class="LegendTabsContent">New Validation Rule</legend>
            <div class="DivFieldset">
                <div class="Div">
                    <asp:Label ID="LblType" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:DropDownList ID="DdlType" runat="server" onchange="SetParameterDiv(this.id)"
                        CssClass="ControlElements">
                    </asp:DropDownList>
                </div>
                <div class="Div">
                    <asp:Label ID="LblError" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox ID="TxtError" runat="server" CssClass="ControlElements"></asp:TextBox>
                </div>
                <div class="Div" id="divDefaultValue" style="display: none" visible="false" runat="server">
                    <asp:Label ID="lblDefault" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox ID="TxtDefaultValue" runat="server" CssClass="ControlElements" Enabled="true"></asp:TextBox>
                    <asp:DropDownList ID="DdlDefaultValue" runat="server" AutoPostBack="false">
                    </asp:DropDownList>
                    <igsch:WebDateChooser ID="dateDefaultValue" runat="server" CssClass="ControlElements">
                    <ClientSideEvents CalendarDateClicked="ValidateDefalutValue(this)" CalendarValueChanged="ValidateDefalutValue(this)" InitializeDateChooser="ValidateDefalutValue(this)">
                    </ClientSideEvents>
                    </igsch:WebDateChooser>
                    <asp:RequiredFieldValidator ID="ReqDefaultValue" runat="server"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegDefaultValue" runat="server"></asp:RegularExpressionValidator>

                    <asp:HiddenField ID="HFDVToolTip" runat="server" />
                </div>
            </div>
            <div class="Div">
            </div>
            <div id="ParametersDiv" runat="server" class="DivFieldset">
                <h1 class="H1Fieldset">
                    Parameters:</h1>
                <div class="Div">
                    <asp:Label ID="LblName" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:Label ID="LabelName" runat="server" CssClass="ControlElements"></asp:Label>
                </div>
                <div class="Div">
                    <asp:Label ID="LblValue" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox ID="TxtValue" runat="server" CssClass="ControlElements"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="TextBoxValueValidator" ControlToValidate="TxtValue"
                        runat="server"></asp:RegularExpressionValidator>
                </div>
                <div class="Div">
                    <h1 class="H1Fieldset">
                        Parameter List:</h1>
                    <div class="DivFieldset">
                        <asp:RadioButtonList ID="Rbl_Event" runat="server" SkinID="UsersRadioItems">
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div style="text-align: right; margin-right: 210px; margin-top: 20px">
                    <asp:Button ID="BtnDeleteParameter" runat="server" OnClick="BtnDeleteParameter_Click"
                        SkinID="ButtonBigRegAdmin" CausesValidation="false" />
                    <asp:Button ID="BtnAddParameter" runat="server" OnClick="BtnAddParameter_Click" SkinID="ButtonBigRegAdmin" />
                </div>
                <div class="Clear">
                </div>
            </div>
            <div id="ClientScriptDiv" runat="server" class="DivFieldset">
                <div class="Div">
                    <asp:Label ID="LblClientScript" runat="server" CssClass="LabelFieldset"></asp:Label>
                    <asp:TextBox TextMode="MultiLine" ID="TextAreaClientScript" runat="server" CssClass="ControlElements"></asp:TextBox>
                </div>
                <div class="Clear">
                </div>
            </div>
            <div style="text-align: right; margin-right: 20px; margin-bottom: 20px">
                <asp:Button ID="BtnAddRule" runat="server" OnClick="BtnAddRule_Click" SkinID="ButtonBigRegAdmin"
                    CausesValidation="false" />
                <asp:Button ID="BtnCancelParameter" runat="server" OnClick="BtnCancelParameter_Click"
                     SkinID="ButtonBigRegAdmin" CausesValidation="false" />
            </div>
        </fieldset>
        <fieldset class="FieldsetTabsContent">
            <legend class="LegendTabsContent">Validation Rules List</legend>
            <div class="Clear">
            </div>
            <ignav:UltraWebTree ID="UltraWebTree_Rules" runat="server">
            </ignav:UltraWebTree>
            <div style="text-align: right; margin-right: 20px; margin-bottom: 20px">
                <asp:Button ID="BtnDeleteRule" runat="server" OnClick="BtnDeleteRule_Click" SkinID="ButtonBigRegAdmin"
                    CausesValidation="false" />
            </div>
        </fieldset>
        <div style="text-align: right; margin-right: 60px; margin-bottom: 20px">
            <asp:Button ID="BtnSave" runat="server" OnClick="BtnSave_Click" SkinID="ButtonBigRegAdmin"
                CausesValidation="false" />
        </div>
        <div class="Clear">
        </div>
        <div class="TabsFooter">
            <div class="DivFooter">
            </div>
        </div>
    </div>
    <div class="ClearProperties">
    </div>
</div>
