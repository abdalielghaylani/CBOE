<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistryRecordNavigation.ascx.cs" Inherits="RegistrationWebApp.Forms.Public.UserControls.RegistryRecordNavigation" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<asp:HiddenField runat="server" ID="CompoundIDHidden" />
<asp:HiddenField runat="server" ID="BatchIDHidden" />
<asp:HiddenField runat="server" ID="ShowButtonsHidden" Value="true" />
<table class="LeftPanelUC">
    <tr valign="top">
        <td valign="top">
            <table id="MessagesInfoTable" runat="server">
                <tr>
                    <td align="center"><asp:Label CssClass="MessageLabel" id="MessageLabel" runat="server"/></td>
                </tr>
                <tr>
                    <td align="center" style="height:35px">
                        <asp:Button CssClass="ToolbarButton" ID="DoneButton" runat="server" Visible="false" CommandName="Done" OnClick="DoneButton_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="AcordeonLine" align="center"></td>
                </tr>
            </table>
            <br />
            <table id="CompundInfoTable" runat="server">
                <tr>
                    <td>
                        <ignav:UltraWebTree ID="UltraWebTreeControl" runat="server" OnNodeClicked="UltraWebTreeControl_NodeClicked" Cursor="Hand">
                            <ClientSideEvents NodeClick="CheckPageValidator" />
                        </ignav:UltraWebTree>
                    </td>
                </tr>
            </table>
            <br />
            <table id="CompoundActionsTable">
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="UpdateButton" runat="server" CommandName="Update" OnClick="UpdateButton_Click"/>
                    </td>
                    <td align="left">
                        <asp:Button ID="AddBatchButton" runat="server" CommandName="AddBatch" OnClick="AddBatchButton_Click"/>
                    </td>
                </tr>  
            </table>
        </td>
    </tr>
</table>

