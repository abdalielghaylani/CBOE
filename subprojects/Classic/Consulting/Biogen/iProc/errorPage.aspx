<%@ Page Language="C#" AutoEventWireup="true" CodeFile="errorPage.aspx.cs" Inherits="errorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label ID="msg1" runat="server" Visible="false">
      <h3>Unknown Shopping Cart Session Error</h3> <br />
    <p>Please log into iProcurment to initiate a new punchout session.</p> 
    </asp:Label>
    <asp:Label ID="msg2" runat="server" Visible="false">
    <h3>Invalid Shopping Cart Session</h3> <br />
    <p>Please log into iProcurment to initiate a new punchout session.</p>
    
    </asp:Label>
    
    </div>
    </form>
</body>
</html>
