<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="PropertyEdit.aspx.cs" Inherits="RegistrationWebApp.Forms.RegistrationAdmin.ContentArea.PropertyEdit"
    Title="Untitled Page" %>

<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content" style="width: 800px; text-align: center">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader">Edit Property</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <div class="TabsContent">
            <fieldset class="FieldsetTabsContent" style="margin-top: 20px">
                <div>
                    <table class="MessageTable">
                        <tr>
                            <td colspan="4" align="center">
                                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="DivFieldset" style="margin-top:30px">
                    <div class="Div">
                        <asp:Label ID="LabelName" runat="server" CssClass="LabelFieldset"></asp:Label>
                        <asp:TextBox ID="TextBoxName" runat="server" CssClass="ControlElements"></asp:TextBox>
                    </div>
                    <div class="Div">
                        <asp:Label ID="LabelType" runat="server" CssClass="LabelFieldset"></asp:Label>
                        <asp:TextBox ID="TextBoxType" runat="server" CssClass="ControlElements" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="Div">
                        <asp:Label ID="LabelPrecision" runat="server" CssClass="LabelFieldset"></asp:Label>
                        <asp:TextBox ID="TextBoxPrecision" runat="server" CssClass="ControlElements"></asp:TextBox>
                        <asp:CustomValidator ID="CheckPrecisionChangeValidator" runat= "server" ControlToValidate="TextBoxPrecision" OnServerValidate="CheckPrecisionChange"></asp:CustomValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorPrecision" runat="server"></asp:RegularExpressionValidator>
                    </div>
                    <div style="margin-right: 90px; margin-top: 20px; margin-bottom:20px">
                        <asp:Button ID="ButtonCancelEdit" runat="server" OnClick="ButtonCancelEdit_Click"
                            SkinID="ButtonBigRegAdmin" />
                        <asp:Button ID="ButtonSaveProperty" runat="server" OnClick="ButtonSaveProperty_Click"
                            SkinID="ButtonBigRegAdmin" />
                    </div>
                </div>
            </fieldset>
            <div class="Clear">
                <div class="TabsFooter">
                    <div class="DivFooter">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
