<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>ChemACX.Com-- Access Dennied</title>
<script LANGUAGE="JavaScript" SRC="chemacx/wddx.js"></script>
<script LANGUAGE="JavaScript" SRC="chemacx/wddxRsEx.js"></script>
<script LANGUAGE="JavaScript" SRC="chemacx/shopcart_functions.js"></script>

<script language="JavaScript">
var productsShown= false;
var theMainFrame = top;
FillWDDXShopCartFromCookie();

function ACXLogin(){
SalesWindow = window.open("http://sales.chemacx.com/chemsales/corporate/CS2_corplogin.cfm","ChemACX_Sales",
 'toolbar=0,location=0,directories=0,status=0,menubar=0,width=650,height=600,xpos=60,ypos=60,left=60,top=60,scrollbars=1,resizable=1');
}

function ACXYourAccount(){ 
SalesWindow = window.open("http://sales.chemacx.com/chemsales/CS2_login.cfm","ChemACX_Sales",
 'height=500,width=700,toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,xpos=60,ypos=60,left=60,top=60');
}

function ACXRegister(){
SalesWindow = window.open("http://sales.chemacx.com/chemsales/CS2_regform.cfm","ChemACX_Sales",
 'height=500,width=700,toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,xpos=60,ypos=60,left=60,top=60');
}

function ACXInstructions(){
SalesWindow = window.open("/chemacx/instruct.asp","ChemACX_Sales",
 'height=500,width=700,toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,xpos=60,ypos=60,left=60,top=60');
}
 
function ACXVendorLogin(){
SalesWindow = window.open("http://sales.chemacx.com/chemsales/CS2_vendorlogin.cfm","ChemACX_Sales",
 'toolbar=0,location=0,directories=0,status=0,menubar=0,width=650,height=600,xpos=60,ypos=60,left=60,top=60,scrollbars=1,resizable=1');
}


</script>

</head>

<body>
<script language="JavaScript" src="http://graphics.camsoft.com/js/header.js"></script>
<table border="0" cellspacing="0" cellpadding="0" height="67"  width=600>
	<tr>
		<TD width="8"></td>
		<td valign="top"><IMG BORDER=0 WIDTH=100 HEIGHT=67 ALT="CS ChemFinder" SRC="http://graphics.camsoft.com/chemfinder/cfcomleftcorner.gif"><IMG WIDTH=291 HEIGHT=67 SRC="http://graphics.chemclub.com/chemacx/caban.gif" BORDER=0 ALT="CS ChemACX"></TD>
		<TD valign="center" align="center">
			<TABLE cellpadding="0" cellspacing="0" border="0">
				<TR>
					<TD align="right"><a href="shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart.gif" width="111" height="18" alt="" border="0"></a></td>
					<TD align="left"><a href="login" onclick="ACXLogin();return false"><img src="<%=Application("NavButtonGifPath")%>login.gif" width="58" height="18" alt="" border="0"></a></td>
				</tr>
				<TR>
					<TD align="right"><a href="Your Account" onclick="ACXYourAccount();return false"><img src="<%=Application("NavButtonGifPath")%>your_account.gif" width="111" height="16" alt="" border="0"></a></td>
					<TD align="left"><a href="Help" onclick="ACXInstructions();return false"><img src="<%=Application("NavButtonGifPath")%>help.gif" width="46" height="16" alt="" border="0"></a></td>
				</tr>
			</table>		
		</td>
	</tr>
</table>

<table border="0" cellpadding="0" cellspacing="0">

<tr>
<td VALIGN=TOP ALIGN=LEFT width="125"><font size="-1">
<BR>
<FONT size=-1>
	<!--#include virtual="/chemacx/callbutton.asp"-->
<BR clear=all><BR>
<a target="_top" href="http://www.freechem.com">FREE!</A>
</FONT>
<BR>

<br>
<STRONG>ChemACX.Com</STRONG>
<br>
<a target="_top" href="http://sales.chemacx.com/index.cfm?demo=/chemacxnet">About</a><br>
<!--- <br>&nbsp;&nbsp;<a target="_top" href="http://www.chemacx.com/">ChemACX Net</a>
<br>&nbsp;&nbsp;<a target="_top" href="https://accounts.camsoft.com/cheminfo/">ChemACX Pro</a> --->
<a href="Help" onclick="ACXInstructions();return false">Instructions</a><br>
<!--- <a target="_top" href="http://sales.chemacx.com/chemsales/corporate/CS2_logwindow.cfm">Corporate Users </a><br>
<a target="_top" href="http://sales.chemacx.com/chemsales/CS2_checkorder.cfm?demo=/chemacxnet">Order Tracking</a><br> --->
<a href="Register" onclick="ACXRegister();return false" onmouseover="status='';return true;">Register</a><br>


<p>
<b></b><STRONG>
Suppliers<BR></STRONG>
<a target="_top" href="http://sales.chemacx.com/join.cfm?demo=/chemacxnet">Join ACX</a><BR>
<!--- <a target="_top" href="http://sales.chemacx.com/joinfaq.cfm?demo=/chemacxnet">FAQs</a><BR> --->
<a target="_top" href="#" onclick="ACXVendorLogin();return false;" onmouseover="status='';return true;">Vendor Login</a><br>


<BR>
<B>ChemNews</B><BR>
<a target="_top" href="http://www.chemnews.com/bytype.cfm?Type=MM">Software</A><BR>
<a target="_top" href="http://www.chemnews.com/bytype.cfm?Type=CI">Databases</A><BR>
<a target="_top" href="http://www.chemnews.com/bytype.cfm?Type=IN">Chemicals</A><BR>

<P>
<a target="_top" href="http://www.chemacx.com">ChemACX</a><BR>
<a target="_top" href="http://scistore.chemstore.com">SciStore</a><BR>
<a target="_top" href="http://labeqwip.chemstore.com">LabEqwipStore</a>
           
<p>
<a target="_top" href="http://www.cheMarket.com">ChemQuote</a><BR>
<a target="_top" href="http://www.cheMarket.com">ChemSell</a><BR>
</font>
</td>

<td width=500 align="center">
<BR><BR>
<table border="1" width="75%" bordercolor="#ff3333" cellpadding="4" cellspacing="0">
	<tr>
		<td>
			<font face="Arial">
			You are trying to access ChemACX.Com from the ChemFinder for Excel Online menu with UserID <%=request.querystring("userid")%>
			<BR>
			That UserID has not been authorized to access this site from Excel.
			<BR>
			Please contact <a href="mailto:chemsales@camsoft.com">chemsales@camsoft.com</a> to subscribe to this service or try accessing the site via your browser by clicking <a href="http://www.chemacx.com">www.chemacx.com</a>
			</font>
		</td>
	</tr>
</table>
<BR Clear="All">
<script langauge=javascript src="engage.js"></script>
<SCRIPT SRC="http://b2b-js.flycast.com/FlycastUniversal/Version3.6/" LANGUAGE="JAVASCRIPT"></SCRIPT>
<script langauge=javascript src="engageb.js"></script>
     

<!--#include virtual="/chemacx/specials.asp"-->
<TABLE width="500" border="0" cellpadding="8" cellspacing="0">
        <TR><TD width="100%">
        <FONT size="2">
        Individual access to ChemACX.Com is provided as a free of charge
        service to the scientific community. Access from corporations, academic
        institutions and government organizations  is charged on an annual
        enterprise subscription basis. Please contact CambridgeSoft sales for
        enterprise subscriptions at <a href="mailto:chemsales@camsoft.com">chemsales@camsoft.com</a>.
        </FONT>
        </TD></TR>
</TABLE>
</td></tr></table>
<BR>
<script language="JavaScript" src="http://graphics.camsoft.com/js/footer.js"></script>
</body>
</html>
