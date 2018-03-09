<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="Export" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Exporting</title>
        <link rel="STYLESHEET" type="text/css" href="../../../App_Themes/Blue/StyleSheet.css">
    </head>
    <body>
        <form runat="server">
            <div style="text-align:center;margin:10px;">
                <div style="margin:10px;">
                    <asp:Label class="Title" ID="ConfirmationMessage" runat="server"></asp:Label>
                </div>
                <span style="text-align:center;margin:10px;">
                    <asp:Button id="OKButton" runat="server" Text="Yes" CssClass="ShortActionButton" OnClientClick="window.open('ContentTransfer.aspx', 'export', 'menubar=no,toolbar=no,scrollbars=no,width=5,height=5,resizable=yes'); window.close();"/>
                </span>
                <span style="text-align:center;margin:10px;">
                    <asp:Button id="CancelButton" runat="server" Text="No" CssClass="ShortActionButton" OnClientClick="window.close();" />
                </span>
            </div>
        </form>
    </body>
</html>
