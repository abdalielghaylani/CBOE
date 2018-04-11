<%@ Control Language="C#" AutoEventWireup="true" Inherits="RefreshSchema" Codebehind="RefreshSchema.ascx.cs" %>
<style type="text/css">
.DropdownlistHeight
{
	height: 23px;
}
.textDisabled[disabled="disabled"]
{
    background-color:#F1F1F1;
    border-top: #C0C0C0 1px solid;
    border-left: #C0C0C0 1px solid;
    border-right: #C0C0C0 1px solid;
    border-bottom: #C0C0C0 1px solid;
}
</style>
<script type="text/javascript" language="javascript">
function DisableBackground()
{
    var isPageValid = true;

    if (typeof(Page_ClientValidate) != "undefined") {
        isPageValid = Page_ClientValidate();
    }

    if (isPageValid) {
        parent.SetProgressLayerVisibility(true);
    } else {
        document.form1.onsubmit = null;
    }

    return isPageValid;
}

parent.SetProgressLayerVisibility(false);

</script>
<table id="tableContainer">
    <tr>
        <th colspan="3">
            <asp:Label runat="server" ID="RefreshSchemaTitleLabel" SkinID="Title"></asp:Label>
            <br/><br/>
        </th>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="InstanceTitleLabel" SkinID="Title" Text="Data source"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
        <td class="DropdownlistHeight">
            <asp:TextBox runat="server" ID="InstanceTextBox" SkinID="TextBox" Enabled="False"/>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="SchemaNameTitleLabel" SkinID="Title"></asp:Label>
        </td>
        <td class="DropdownlistHeight">
            <asp:TextBox runat="server" ID="SchemaTextBox" SkinID="TextBox" Enabled="False"/>
        </td>
    </tr>
    <tr><td></br></td></tr>
    <tr>
        <td></td>
        <td colspan="2" class="DropdownlistHeight">
            <asp:CheckBox runat="server" ID="AuthorizeCheckBox" AutoPostBack="true" 
                Text = "Authorize privilege" Checked="false" 
                oncheckedchanged="AuthorizeCheckBox_CheckedChanged" />
        </td>
    </tr>
    <tr id="GranterUserRow" runat = "server">
        <td>
            <asp:Label runat="server" ID="GranterUserLabel" SkinID="Title"></asp:Label>
        </td>
        <td class="DropdownlistHeight">
            <asp:TextBox runat="server" ID="GranterUserTextBox" Enabled = "false" SkinID="TextBox" CssClass="textDisabled"></asp:TextBox>
        </td>
        <td>
            <asp:RequiredFieldValidator runat="server" ID="GranterUserRequiredField" Enabled = "false" SkinID="ReqField" ControlToValidate="GranterUserTextBox" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr id="PasswordRow" runat = "server">
        <td>
            <asp:Label runat="server" ID="PasswordTitleLabel" SkinID="Title"></asp:Label>
        </td>
        <td class="DropdownlistHeight">
            <asp:TextBox runat="server" TextMode="Password" ID="PasswordTextBox" Enabled = "false" SkinID="TextBox" CssClass="textDisabled"></asp:TextBox>
        </td>
        <td>
            <asp:RequiredFieldValidator runat="server" ID="PasswordRequiredField" Enabled = "false" SkinID="ReqField" ControlToValidate="PasswordTextBox" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td></td>
        <td colspan="2" >
            <br/>
            <COEManager:ImageButton runat="server" ID="CancelImageButton" OnButtonClicked="CancelSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="window.parent.CloseModal(false);" />
            <COEManager:ImageButton runat="server" ID="RefreshSchemaButton" OnButtonClicked="RefreshSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Publish" CausesValidation="true" OnClientClick="return DisableBackground();"/>
            <asp:ValidationSummary ID="SchemaValidationSummary" runat="server" DisplayMode="BulletList" />
        </td>
    </tr>
</table>


