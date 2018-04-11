<%@ Page Language="C#" MasterPageFile="~/Forms/Master/Registration.Master" AutoEventWireup="true"
    Codebehind="MoveDelete.aspx.cs" Inherits="RegistrationWebApp.Forms.ViewMixture.ContentArea.MoveDelete"
    Title="Untitled Page" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <table class="PagesContentTable" cellspacing="0">
        <tr class="PagesToolBar">
            <td align="left">
                <asp:Button ID="CancelButton" runat="server" CausesValidation="false" OnClick="CancelButton_Click" />
                <asp:Button ID="AcceptButton" runat="server" Visible="false" CausesValidation="false"
                    OnClick="AcceptButton_Click" />
            </td>
            <td colspan="1" align="right">
                <asp:Label ID="PageTitleLabel" runat="server" CssClass="ContentPageTitle"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="center">
                <uc2:MessagesArea ID="MessagesAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
    </table>
    <table cellspacing="0" width="80%" style="margin-left:100px;">
        <%--<tr>
            <td colspan="3" id="ActionRow" runat="server">
                <asp:Label runat="server" ID="ActionTitleLabel"></asp:Label>
                <asp:RadioButtonList runat="server" ID="ActionRadioList" CausesValidation="true">
                </asp:RadioButtonList>
            </td>
        </tr>--%>
        <tr>
            <td runat="server" id="FromRow" width="30%">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="SourceBatchIDLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="SourceBatchIDTextBox" ReadOnly="true" CausesValidation="true"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="SourceBatchIDRequiredFieldValidator" runat="server"
                                ErrorMessage="*" ControlToValidate="SourceBatchIDTextBox"></asp:RequiredFieldValidator>
                        </td>
                        
                    </tr>
                </table>
            </td>
            <td runat="server" id="ToImageRow" width="5%">
                <asp:Image runat="server" ID="ToImage" />
            </td>
            <td runat="server" id="ToRegistryRow">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="DestinyRegistryIDLabel"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="DestinyRegistryIDTextBox"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="MoveButton" OnClick="SubmitButton_Click" />
                        </td> 
                    </tr>
                </table>
            </td>       
        <td><asp:Button runat="server" ID="SubmitButton" OnClick="SubmitButton_Click" /></td>
     </tr>
    </table>
    <table width="100%" >
        <tr align="center">
            <td runat="server" id="ToDeleteRegistry">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="ToDeleteRegistryIDLabel"></asp:Label>
                        </td>
                        <td>
                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="ToDeleteRegistryIDTextBox"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;&nbsp;<asp:Button runat="server" ID="DeleteRegButton" OnClick="DeleteRegButton_Click"/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
   
       
</asp:Content>
