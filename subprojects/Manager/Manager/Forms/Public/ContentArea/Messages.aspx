<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_Public_ContentArea_Messages" Codebehind="Messages.aspx.cs" MasterPageFile="~/Forms/Master/MasterPage.master" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <div>
        <table width="100%" class="PagesContentTable">
            <tr>
                <td>
                    <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                     <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%">
                        <tr align="center">
                            <td><br/>
                                <asp:Label runat="server" ID="MessageLabel" Width="100%" CssClass="TitleLabel"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td><br/>
                               <input type="button" runat="server" id="GoBackButton" class="ToolbarButton"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>