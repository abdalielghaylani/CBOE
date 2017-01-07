<%@ Control Language="C#" AutoEventWireup="true" Inherits="AddSchemas" Codebehind="AddSchemas.ascx.cs" %>
<style type="text/css">
.DropdownlistHeight
{
	height: 23px;
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
    
    if(isPageValid) 
        parent.SetProgressLayerVisibility(true);
    
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
        </th>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="InstanceTitleLabel" SkinID="Title" Text="Data source"></asp:Label>
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
        <td class="DropdownlistHeight"><asp:DropDownList runat="server" ID="SchemaDropDownList" AppendDataBoundItems="true" SkinID="SchemaDropDownList" /></td>
        <td><asp:RequiredFieldValidator runat="server" ID="SchemaRequiredFieldValidator" SkinID="ReqField" ControlToValidate="SchemaDropDownList" InitialValue="-1" ></asp:RequiredFieldValidator></td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="PasswordTitleLabel" SkinID="Title"></asp:Label>
        </td>
        <td class="DropdownlistHeight"><asp:TextBox runat="server" TextMode="Password" ID="PasswordTextBox" SkinID="TextBox"></asp:TextBox></td>
        <td><asp:RequiredFieldValidator runat="server" ID="PasswordRequiredField" SkinID="ReqField" ControlToValidate="PasswordTextBox" ></asp:RequiredFieldValidator></td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="PublishRelationshipsLabel" SkinID="Title"></asp:Label>
        </td>
        <td colspan="2"><asp:CheckBox runat="server" ID="PublishRelationshipsCheckBox" Checked="true" /></td>
    </tr>
    <tr>
        <td colspan="3">
        <%--Jira ID: CBOE-404 : Bug fixing--%>
            <COEManager:ImageButton runat="server" ID="CancelImageButton" OnButtonClicked="CancelSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="window.parent.CloseModal(false);" />
            <COEManager:ImageButton runat="server" ID="PublishSchemaButton" OnButtonClicked="PublishSchemaButton_ButtonClicked" ButtonMode="ImgAndTxt" TypeOfButton="Publish" CausesValidation="true" OnClientClick="return DisableBackground();"/>
            <asp:ValidationSummary ID="SchemaValidationSummary" runat="server" DisplayMode="BulletList" />
        </td>
    </tr>
</table>


