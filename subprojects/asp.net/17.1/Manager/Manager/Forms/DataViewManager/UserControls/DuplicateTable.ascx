<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateTable.ascx.cs" Inherits="DuplicateTable" %>
<asp:ValidationSummary ID="ValidationSummary" runat="server" EnableClientScript="true" ShowSummary="true" DisplayMode="List" CssClass="NameDescUCTable" />
<asp:HiddenField ID="SelectedTableIDHidden" Value="-1" runat="server" />
<table>
    <tr>
        <td colspan="4" align="center">
            <asp:Label runat="server" ID="TitleLabel" SkinID="DataViewTitle"></asp:Label>
        </td>
    </tr>
    <tr valign="top">
        <td align="left" style="height: 50px;">
            <asp:Label runat="server" ID="NameLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left" style="height: 50px;">
            <asp:TextBox runat="server" ID="NameTextBox" SkinID="TextBoxRO" ReadOnly="true" Height="50px" MaxLength="255" ></asp:TextBox>
        </td>
    </tr>
    <tr valign="top">
        <td align="left" style="height: 50px;">
            <asp:Label runat="server" ID="AliasLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left" style="height: 50px;">
            <asp:TextBox runat="server" ID="AliasTextBox" SkinID="TextBox" Height="50px" MaxLength="30" ></asp:TextBox>
        </td>
        <td align="left" style="height: 50px;">
            <asp:RequiredFieldValidator runat="server" ID="AliasRequiredField" SkinID="ReqField" ControlToValidate="AliasTextBox" Enabled="false" Display="dynamic" ValidationGroup="DuplicateTable" ></asp:RequiredFieldValidator>
        </td>
        <td align="left" style="height: 50px;">
            <asp:RegularExpressionValidator runat="server" ID="AliasRegExpValidator" SkinID="ReqField" EnableClientScript="true" ControlToValidate="AliasTextBox" ValidationExpression="^.{0,30}$" Enabled="false" Display="dynamic" ValidationGroup="DuplicateTable" />
        </td>
    </tr>
    <tr>
        <td colspan="4" align="center">
            <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="ShowDuplicateTable(false); return false;" />
            <COEManager:ImageButton ID="DuplicateImageButton" runat="server" TypeOfButton="CreateAlias" CausesValidation="true" OnClientClick="if(ValidateDuplicateTable()==1){DoDuplicateTable();} return false;" />
        </td>
    </tr>
</table>