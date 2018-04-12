<%@ Page Language="C#" MasterPageFile="~/Forms/Master/MasterPage.Master" AutoEventWireup="true" Inherits="Forms_ContentArea_Messages" Codebehind="Messages.aspx.cs" ValidateRequest="false" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <div>
        <table class="PagesContentTable">
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>

                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr align="center">
                            <td><br/>
                                <asp:Label runat="server" ID="MessageLabel" SkinID="title"></asp:Label>
                            </td>
                        </tr>
                        <tr align="center">
                            <td><br/>
                               <input type="button" runat="server" id="GoBackButton" class="ToolbarButton"/>
                            </td>
                        </tr>
                    </table>
                    <br />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
