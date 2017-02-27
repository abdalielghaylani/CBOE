<%@ Control Language="C#" AutoEventWireup="true" Inherits="AddSchemas" Codebehind="AddSchemas.ascx.cs" %>
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
    if (typeof(Page_ClientValidate) != "undefined")
    {
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

function onInstanceChange() {
    //add this to remove validation error message.
    document.form1.submit();
    return true;
}
</script>
<table id="tableContainer">
    <tr>
        <th colspan="3">
            <asp:Label runat="server" ID="AddSchemaTitleLabel" SkinID="Title"></asp:Label>
            <br/><br/>
        </th>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="InstanceTitleLabel" SkinID="Title" Text="Data source"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
        <td class="DropdownlistHeight">
            <asp:DropDownList runat="server" ID="InstanceDropDownList" AutoPostBack="true" CausesValidation="false" onchange="return onInstanceChange();"
                AppendDataBoundItems="true" SkinID="SchemaDropDownList" OnSelectedIndexChanged="InstanceDropDownList_SelectedIndexChanged" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="SchemaNameTitleLabel" SkinID="Title"></asp:Label>
        </td>
        <td class="DropdownlistHeight">
            <asp:DropDownList runat="server" ID="SchemaDropDownList" AppendDataBoundItems="true" SkinID="SchemaDropDownList" />
        </td>
        <td><asp:RequiredFieldValidator runat="server" ID="SchemaRequiredFieldValidator" SkinID="ReqField" ControlToValidate="SchemaDropDownList" InitialValue="-1" ></asp:RequiredFieldValidator></td>
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
            <asp:RequiredFieldValidator runat="server" ID="GranterUserRequiredField" SkinID="ReqField" Enabled = "false" ControlToValidate="GranterUserTextBox" ></asp:RequiredFieldValidator>
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
            <asp:RequiredFieldValidator runat="server" ID="PasswordRequiredField" SkinID="ReqField" Enabled = "false" ControlToValidate="PasswordTextBox" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr><td></br></td></tr>
    <tr>
        <td></td>
        <td colspan="2" class="DropdownlistHeight"><asp:CheckBox runat="server" ID="PublishRelationshipsCheckBox" Text = "Publish relationships?" Checked="true" /></td>
    </tr>
    <tr>
        <td></td>
        <td colspan="2" >
            <br/>
            <%--Jira ID: CBOE-404 : Bug fixing--%>
            <COEManager:ImageButton runat="server" ID="CancelImageButton" OnButtonClicked="CancelSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="window.parent.CloseModal(false);" />
            <COEManager:ImageButton runat="server" ID="PublishSchemaButton" OnButtonClicked="PublishSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Publish" CausesValidation="true" OnClientClick="return DisableBackground();"/>
            <asp:ValidationSummary ID="SchemaValidationSummary" runat="server" DisplayMode="BulletList" />
        </td>
    </tr>
</table>


