<%@ Page ValidateRequest="false" Language="C#" AutoEventWireup="true" CodeFile="LoginRequest.aspx.cs" Inherits="LoginRequest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
   
        <asp:LinkButton ID="lbtnLoginRequest" runat="server" OnClick="lbtnLoginRequest_Click">Login Request</asp:LinkButton>
        <asp:Panel runat="server" Visible="false" ID="panelURLResponse">
            <b>Response&nbsp;</b><h1>URL</h1>
            <br />
            <asp:Label ID="lbtnResponseURL" runat="server">Go to Shopping Cart</asp:Label>
        </asp:Panel>
    
    </form>
</body>
</html>
