<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="CustomizeForms.aspx.cs" Inherits="RegistrationWebApp2.Forms.RegistrationAdmin.ContentArea.CustomizeForms"
    Title="Cambridge Soft - Registration Admin" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="Content">
        <ul class="UlTabsHeader">
            <li class="LiFirstTabsHeader"></li>
            <li class="SelectedTabsHeader">
                <div class="LeftCornerTabsHeader">
                </div>
                <a href="#" class="ATabsHeader" style="width: 150px">Forms Customization</a>
                <div class="RigthCornerTabsHeader">
                </div>
            </li>
            <li class="LiLastTabsHeader"></li>
        </ul>
        <div class="TabsContent">
            <table class="MessageTableRegAdmin">
                <tr>
                    <td colspan="4" align="center">
                        <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
                    </td>
                </tr>
            </table>
            <div style="margin-right: 40px">
                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="Goto2" OnClick="LinkButtonBack_Click">Back</asp:LinkButton>
                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="Goto2" OnClick="LinkButtonGoToMain_Click"
                    Visible="false">Home</asp:LinkButton>
            </div>
            <div class="Clear">
            </div>
            <div style="margin-left: 20px; width: 900px">
                <asp:DataList ID="MixturePropertiesDataList" runat="server">
                    <HeaderStyle HorizontalAlign="left" />
                    <HeaderTemplate>
                        <table class="DataListTable">
                            <tr style="background-color: #C8C8C8; color: Black; font-weight: bolder;">
                                <td colspan="6">
                                    <asp:Label ID="MixtureTitleLabel" runat="server" Text="Registry Properties" Width="100%" /></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="NameTitleLabel" runat="server" Text="Name" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="ControlTypeTitleLabel" runat="server" Text="Control Type" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="TitleLabel" runat="server" Text="Label" />
                                </td>
                                <td>
                                    <asp:Label ID="CSSClassLabel" runat="server" Text="CSS Class" />
                                </td>
                                <td>
                                    <asp:Label ID="VisibleLabel" Text="Visible" runat="server" />
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="left" CssClass="CustomizePropertyItem" />
                    <AlternatingItemStyle CssClass="CustomizePropertyAltItem" />
                    <ItemTemplate>
                        <tr>
                            <td align="left">
                                <asp:Label ID="NameLabel" runat="server" Width="150px" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ControlTypeDropDownList" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="LabelTextBox" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="CSSClassDropDown" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="VisibleCheckBox" runat="server" />
                            </td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="LabelTextBox"
                                    ErrorMessage="<%#SetErrorMessage()%>" ValidationExpression="<%#SetLabelValidationExpression()%>" EnableClientScript="true" Text="<%#SetErrorMessage()%>">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:DataList>
                <asp:DataList ID="CompoundPropertiesDataList" runat="server">
                    <HeaderStyle HorizontalAlign="left" />
                    <HeaderTemplate>
                        <table style="font-size: 11px; font-family: Verdana; color: #000066; text-align: left;
                            background-color: #fff; border: none; width: 100%; height: 15px; margin-top: 7px;
                            margin-bottom: 7px;">
                            <tr style="background-color: #C8C8C8; color: Black; font-weight: bolder;">
                                <td colspan="6">
                                    <asp:Label ID="CompoundTitleLabel" runat="server" Text="Compound Properties" /></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="NameTitleLabel" runat="server" Text="Name" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="ControlTypeTitleLabel" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="TitleLabel" runat="server" Text="Label" />
                                </td>
                                <td>
                                    <asp:Label ID="CSSClassLabel" runat="server" Text="CSS Class" />
                                </td>
                                <td>
                                    <asp:Label ID="VisibleLabel" Text="Visible" runat="server" />
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                        <tr>
                            <td align="left">
                                <asp:Label ID="NameLabel" runat="server" Width="150px" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ControlTypeDropDownList" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="LabelTextBox" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="CSSClassDropDown" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="VisibleCheckBox" runat="server" />
                            </td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="LabelTextBox"
                                    ErrorMessage="<%#SetErrorMessage()%>" ValidationExpression="<%#SetLabelValidationExpression()%>" EnableClientScript="true" Text="<%#SetErrorMessage()%>">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                    </asp:DataList>
                <asp:DataList ID="StructurePropertiesDataList" runat="server">
                    <HeaderStyle HorizontalAlign="left" />
                    <HeaderTemplate>
                        <table style="font-size: 11px; font-family: Verdana; color: #000066; text-align: left;
                            background-color: #fff; border: none; width: 100%; height: 15px; margin-top: 7px;
                            margin-bottom: 7px;">
                            <tr style="background-color: #C8C8C8; color: Black; font-weight: bolder;">
                                <td colspan="6">
                                    <asp:Label ID="StructureTitleLabel" runat="server" Text="Base Fragment Properties" /></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="NameTitleLabel" runat="server" Text="Name" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="ControlTypeTitleLabel" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="TitleLabel" runat="server" Text="Label" />
                                </td>
                                <td>
                                    <asp:Label ID="CSSClassLabel" runat="server" Text="CSS Class" />
                                </td>
                                <td>
                                    <asp:Label ID="VisibleLabel" Text="Visible" runat="server" />
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                        <tr>
                            <td align="left">
                                <asp:Label ID="NameLabel" runat="server" Width="150px" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ControlTypeDropDownList" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="LabelTextBox" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="CSSClassDropDown" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="VisibleCheckBox" runat="server" />
                            </td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="LabelTextBox"
                                    ErrorMessage="<%#SetErrorMessage()%>" ValidationExpression="<%#SetLabelValidationExpression()%>" EnableClientScript="true" Text="<%#SetErrorMessage()%>">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:DataList>
                <asp:DataList ID="BatchPropertiesDataList" runat="server">
                    <HeaderStyle HorizontalAlign="left" />
                    <HeaderTemplate>
                        <table style="font-size: 11px; font-family: Verdana; color: #000066; text-align: left;
                            background-color: #fff; border: none; width: 100%; height: 15px; margin-top: 7px;
                            margin-bottom: 7px;">
                            <tr style="background-color: #C8C8C8; color: Black; font-weight: bolder;">
                                <td colspan="6">
                                    <asp:Label ID="BatchTitleLabel" runat="server" Text="Batch Properties" /></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="NameTitleLabel" runat="server" Text="Name" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="ControlTypeTitleLabel" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="TitleLabel" runat="server" Text="Label" />
                                </td>
                                <td>
                                    <asp:Label ID="CSSClassLabel" runat="server" Text="CSS Class" />
                                </td>
                                <td>
                                    <asp:Label ID="VisibleLabel" Text="Visible" runat="server" />
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                        <tr>
                            <td align="left">
                                <asp:Label ID="NameLabel" runat="server" Width="150px" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ControlTypeDropDownList" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="LabelTextBox" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="CSSClassDropDown" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="VisibleCheckBox" runat="server" />
                            </td>
                            <td>
                             <asp:RegularExpressionValidator ID="RegularExpressionValidatorLabel" runat="server" ControlToValidate="LabelTextBox"
                                 ErrorMessage="<%#SetErrorMessage()%>" ValidationExpression="<%#SetLabelValidationExpression()%>" EnableClientScript="true" Text="<%#SetErrorMessage()%>">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:DataList>
                <asp:DataList ID="BatchComponentPropertiesDataList" runat="server">
                    <HeaderTemplate>
                        <table style="font-size: 11px; font-family: Verdana; color: #000066; text-align: left;
                            background-color: #fff; border: none; width: 100%; height: 15px; margin-top: 7px;
                            margin-bottom: 7px;">
                            <tr style="background-color: #C8C8C8; color: Black; font-weight: bolder;">
                                <td colspan="6">
                                    <asp:Label ID="BatchComponentTitleLabel" runat="server" Text="Batch Component Properties" /></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Label ID="NameTitleLabel" runat="server" Text="Name" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="ControlTypeTitleLabel" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:Label ID="TitleLabel" runat="server" Text="Label" />
                                </td>
                                <td>
                                    <asp:Label ID="CSSClassLabel" runat="server" Text="CSS Class" />
                                </td>
                                <td>
                                    <asp:Label ID="VisibleLabel" Text="Visible" runat="server" />
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                        <tr>
                            <td align="left">
                                <asp:Label ID="NameLabel" runat="server" Width="150px" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ControlTypeDropDownList" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="LabelTextBox" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="CSSClassDropDown" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="VisibleCheckBox" runat="server" />
                            </td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="LabelTextBox"
                                    ErrorMessage="<%#SetErrorMessage()%>" ValidationExpression="<%#SetLabelValidationExpression()%>" EnableClientScript="true" Text="<%#SetErrorMessage()%>">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:DataList>
            </div>
            <div class="PTabsContent" style="text-align: right; margin-right: 40px">
                <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save"
                    SkinID="ButtonBig" />
            </div>
            <div class="Clear">
                <div class="TabsFooter">
                    <div class="DivFooter">
                    </div>
                </div>
            </div>
        </div>
        <div class="ClearProperties">
        </div>
    </div>
</asp:Content>
