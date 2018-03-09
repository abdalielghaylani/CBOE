<%@ Control Language="C#" AutoEventWireup="true" Inherits="ChangePasswordUC" CodeBehind="ChangePassword.ascx.cs" %>
<asp:ChangePassword SkinID="ChangePassword" ID="ChangePassword1" runat="server" OnCancelButtonClick="CancelClicked"
    OnContinueButtonClick="ContinueClicked">
    <ChangePasswordTemplate>
        <table cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
            <tr>
                <td>
                    <table cellpadding="0">
                        <tr>
                            <td align="center" class="ChangePasswordTitleText" colspan="2">
                                Change Your Password
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="ChangePasswordLabel">
                                <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="CurrentPassword" runat="server" CssClass="ChangePasswordTextBox"
                                    Font-Size="0.8em" TextMode="Password" Width="150px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                    ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="ChangePasswordLabel">
                                <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="NewPassword" runat="server" CssClass="ChangePasswordTextBox" Font-Size="0.8em"
                                    TextMode="Password" Width="150px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                    ErrorMessage="New Password is required." ToolTip="New Password is required."
                                    ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="ChangePasswordLabel">
                                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="ChangePasswordTextBox"
                                    Font-Size="0.8em" TextMode="Password" Width="150px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                    ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                                    ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                    ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                    ValidationGroup="ctl00$ChangePassword1"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2" style="color: Red;">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                                                CssClass="ChangePasswordButton" Text="Change Password" ValidationGroup="ctl00$ChangePassword1" />
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                CssClass="ChangePasswordCancelButton" Text="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                    </table>
                </td>
            </tr>
        </table>
    </ChangePasswordTemplate>
</asp:ChangePassword>
<asp:Panel ID="LDAPMessagePanel" runat="server">
    <asp:Label SkinID="ChangePassword" ID="LDAPEnabledMessage" runat="server">Password cannot be changed here because LDAP authentication is enabled on the server.<br />
				    Please contact the system Adminstrator for instructions on how to change the LDAP password.</asp:Label>
    <COEManager:ImageButton ID="LDAPMessageDoneButton" runat="server" ButtonMode="ImgAndTxt"
        TypeOfButton="Done"></COEManager:ImageButton>
</asp:Panel>
