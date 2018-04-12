<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShoppingCartConfirmation.aspx.cs" Inherits="ShoppingCartConfirmation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ChemACX <> iProcurment Link</title>
    <link rel='stylesheet' type='text/css' media='print' href='/cheminv/cheminv_ie.css' />
    <script type="text/javascript" language="javascript">
 function JsHTTPGet(strURL){
	var objXML = new ActiveXObject("Msxml2.XMLHTTP"); 
	objXML.open("GET", strURL, false);
	objXML.send(); 
	strResponse = objXML.responseText;
	return strResponse;
	}
    function submitCart(cartid, cartsessionvars) {
    var HTTPResponse = JsHTTPGet("SCManagement.aspx?cartid=" + cartid + "&" + cartsessionvars + "&status=1");
    }
    
    function clearCart() {
        document.form1.oracleCart.value=""; 
        
    }
        function cancelCart(cartid, cartsessionvars) {
    var HTTPResponse = JsHTTPGet("SCManagement.aspx?cartid=" + cartid + "&" + cartsessionvars + "&status=2");
    }    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:PlaceHolder ID="ButtonsSection" runat="server"></asp:PlaceHolder>
        &nbsp;
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:HyperLinkField DataTextField="cartid" HeaderText="Cart Id" />
                <asp:BoundField DataField="userid" HeaderText="User" />
                <asp:BoundField DataField="createddate" HeaderText="Created On" />
                <asp:HyperLinkField DataNavigateUrlFields="cartid" DataNavigateUrlFormatString="ShoppingCartConfirmation.aspx?cartid={0}" Text="&nbsp;&nbsp;&nbsp;View&nbsp;&nbsp;&nbsp;" NavigateUrl="ShoppingCartConfirmation.aspx" />               
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
