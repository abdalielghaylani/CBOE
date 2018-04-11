<%@ Control Language="C#" AutoEventWireup="true" Inherits="NameDescriptionUC" Codebehind="NameDescription.ascx.cs" %>
<table>
    <tr>
        <td colspan="5" align="center" style="padding:5px;">
            <asp:Label runat="server" ID="TitleLabel" SkinID="DataViewTitle"></asp:Label>
        </td>
    </tr>
    <tr valign="top" style="height: 100px;vertical-align:middle;">
        <td align="left" style="width: 100px;">
            <asp:Label runat="server" ID="NameLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left" style="width: 200px;">
            <asp:TextBox runat="server" ID="NameTextBox" TextMode="MultiLine" SkinID="TextBoxMultiline" Height="50px" Width="200px" MaxLength="50" ></asp:TextBox>
        </td>
        <td align="left" style="width: 15px;">
            <asp:RequiredFieldValidator runat="server" ID="NameRequiredField" SkinID="ReqField" ControlToValidate="NameTextBox" ></asp:RequiredFieldValidator>
        </td>
        <td align="left" style="width: 15px;">
            <asp:RegularExpressionValidator runat="server" ID="NameRegExpValidator" SkinID="ReqField" EnableClientScript="true" ControlToValidate="NameTextBox" ValidationExpression="^[a-zA-Z0-9_'=()!?,.* /+:%-.\s]{1,50}$" />
        </td>
        <td align="left" style="width: 15px;">
            <asp:CustomValidator runat="server" ID="UniqueNameValidator" SkinID="ReqField" EnableClientScript="false" ControlToValidate="NameTextBox" OnServerValidate="UniqueNameValidator_ServerValidate" />
        </td>
    </tr>
    <tr valign="top" style="height: 100px;vertical-align:middle;">
         <td align="left" style="width: 100px;">
            <asp:Label runat="server" ID="DescriptionLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left" style="width: 20px;">
            <asp:TextBox runat="server" ID="DescriptionTextBox" TextMode="MultiLine" SkinID="TextBoxMultiline" Height="50px" Width="200px" MaxLength="255"></asp:TextBox>
        </td>
        <td align="left" style="width: 15px;">
            <asp:RequiredFieldValidator runat="server" ID="DescriptionRequiredFieldValidator" SkinID="ReqField" ControlToValidate="DescriptionTextBox"></asp:RequiredFieldValidator>
        </td>
        <td align="left" style="width: 15px;">
            <asp:RegularExpressionValidator runat="server" ID="DescRegExpValidator" SkinID="ReqField" ControlToValidate="DescriptionTextBox" ValidationExpression="^[a-zA-Z0-9_'=()!?,.:* /+%-.\s]{1,255}$" ></asp:RegularExpressionValidator>
        </td>
    </tr>
</table>