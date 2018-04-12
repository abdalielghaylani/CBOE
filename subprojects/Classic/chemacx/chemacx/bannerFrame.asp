<%
' Determines whether the user is logged into ChemACX Pro or ChemACX Net
Session("IsNet") = Not Session("okOnService_10")
%>
<script language="JavaScript">
			productsShown = false;
			MainFrame = <%=Application("mainwindow")%>;
		
			function ACXYourAccount(){ 
			SalesWindow = window.open("http://sales.chemacx.com/chemsales/CS2_login.cfm","ChemACX_Sales",
			 'height=500,width=700,toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,xpos=60,ypos=60,left=60,top=60');
			}
			function ACXInstructions(){
			SalesWindow = window.open("http://chemstore.cambridgesoft.com/help/help_acx.cfm","ChemACX_Sales",
			 'height=500,width=700,toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,xpos=60,ypos=60,left=60,top=60');
			}	 
</script>
<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/DBShoppingCart.asp"-->		
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<form name="form1" action="/chemacx/chemacx/bannerframe.asp" method="POST">
		<input type="Hidden" name="SaveCart">
		<input type="Hidden" name="ShoppingCartWDDXPacket">
	</form>
	<head>
		<title>Banner Frame</title>
		<script LANGUAGE="JavaScript" SRC="wddx.js"></script>
		<script LANGUAGE="JavaScript" SRC="wddxDes.js"></script>
		<script LANGUAGE="JavaScript" SRC="wddxRsEx.js"></script>
		<script LANGUAGE="JavaScript" SRC="shopcart_functions.js"></script>
		<script language="JavaScript">
			FillWDDXShopCart();
		</script>
	</head>
	<body bgcolor="#FFFFFF">
		<% if NOT Application("UseCSWebUserAccounts") then%>
			<!--#include file="../intranetBanner.asp"-->
		<%else%>
			<!--#include file="../banner.asp"-->
		<%end if%>
	</body>
</html>

