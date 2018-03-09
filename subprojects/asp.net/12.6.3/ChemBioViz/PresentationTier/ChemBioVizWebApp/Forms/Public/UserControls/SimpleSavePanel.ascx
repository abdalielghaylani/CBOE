<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpleSavePanel.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.SimpleSavePanel" %>
<div id="SimpleSavePanelYUIPanel" style="text-align:center; display:none;" runat="server">
    <div class="MenuItemContainer">
        <table style="text-align:left;">
            <tr>
                <td><asp:Label ID="NameLabel" runat="server" /></td>
                <td><asp:TextBox ID="NameTextBox" runat="server" onfocus="this.select();"/></td>
            </tr>
            <tr>
                <td><asp:Label ID="DescriptionLabel" runat="server" /></td>
                <td><asp:TextBox ID="DescriptionTextBox" runat="server" onfocus="this.select();"/></td>
            </tr>
            <tr>
                <td colspan="2" class="check_popup" ><asp:CheckBox ID="IsPublicCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:RequiredFieldValidator ID="NameRequiredField" ControlToValidate="NameTextBox" runat="server"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td colspan="2"><asp:RequiredFieldValidator ID="DescriptionRequiredField" ControlToValidate="DescriptionTextBox" runat="server"></asp:RequiredFieldValidator></td>
            </tr>
        </table>
        <div style="width:130px; margin-left: 40px;">
            <div class="Button2">
                <div class="Left">
                </div>
                <asp:LinkButton ID="SaveLinkButton" runat="server" OnClick="SaveButton_Click" CssClass="LinkButton" Text="Save"/>
                <div class="Right">
                </div>
            </div>
            <div class="Button2">
                <div class="Left">
                </div>
                <asp:LinkButton ID="CancelLinkButton" runat="server" CssClass="LinkButton" Text="Cancel" CausesValidation="false" />
                <div class="Right">
                </div>
            </div>
        </div>
    </div>
</div>