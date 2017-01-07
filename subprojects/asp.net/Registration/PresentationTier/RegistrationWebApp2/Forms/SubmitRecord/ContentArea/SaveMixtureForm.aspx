<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true" CodeBehind="SaveMixtureForm.aspx.cs" Inherits="RegistrationWebApp2.Forms.SubmitRecord.ContentArea.SaveMixtureForm" Title="Untitled Page" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
           <td class="ColumnContentHeader"></td>
           <td align="left">
               <asp:Button ID="CancelButton" runat="server" OnClick="CancelButton_Click" OnClientClick="return ConfirmCancel();"/>
               <asp:Button CssClass="ToolbarSaveTemplateButton" ID="SaveButton" runat="server" OnClick="SaveButton_Click" CausesValidation="true" />
            </td> 
            <td align="right">
                <asp:Label ID="PageTitleLabel" runat="server" SkinID="PageTitleLabel"></asp:Label>
            </td>   
        </tr>
        <tr>
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false"/>
            </td>   
        </tr>
         <tr class="ErrorMessagesRow" runat="server" id="ErrorMessagesRow" visible="false">
            <td colspan="2" align="center">
                <igmisc:WebPanel ID="ErrorsWebPanel" runat="server" Width="90%">
                    <Header>
                    </Header>
                    <Template>
                        <asp:Label ID="ErrorMessageLabel" runat="server" SkinID="ErrorMessageLabel"></asp:Label>
                        <br/>
                        <asp:Label ID="GoBackLinkLabel" runat="server" SkinID="LinkLabel" Visible="false"></asp:Label>
                    </Template>
                  </igmisc:WebPanel>
            </td>
        </tr>  
        <tr>
            <td style="width:50px;"></td>
            <td><br />
                <asp:Label ID="FormNameLabel" runat="server"></asp:Label><br />
                <asp:TextBox ID="FormNameTextBox" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="LengthForNameValidator" Display="Dynamic" ControlToValidate="FormNameTextBox" EnableClientScript="true" ValidationExpression=".{0,255}" ErrorMessage="The length of the Name must be at last 255 chars long" runat="server" Enabled="true"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="FormNameRequiredFieldValidator" runat="server" ControlToValidate="FormNameTextBox">*The field is required</asp:RequiredFieldValidator><br />
                <br />
                <asp:Label ID="FormDescriptionLabel" runat="server" Text="Label"></asp:Label><br />
                <asp:TextBox ID="FormDescriptionTextBox" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="LengthForDescriptionValidator" Display="Dynamic" ControlToValidate="FormDescriptionTextBox" EnableClientScript="true" ValidationExpression=".{0,255}" ErrorMessage="The length of the Description must be at last 255 chars long" runat="server" Enabled="true"></asp:RegularExpressionValidator><br />
                <br />
                <asp:CheckBox ID="PublicFormCheckBox" runat="server" /><br /><br />
           </td>
        </tr>
    </table>
</asp:Content>
