<%@ LANGUAGE=VBScript %>
<%'SYAN modified on 3/31/2004 to support parameterized SQL%>
<!--#INCLUDE FILE = "AnonymousSearchCounterFunctions.asp"-->
<!--#INCLUDE FILE = "API/guiutils.asp"-->
<%
' CSWebUser Authentication
Session("ShowContent") = Session("okOnService_1")

' Allows anonymous searches before forcing CSWebUsers authentication
NumSearchesAllowed =  5 ' Number of anonymous searches allowed
if Not Session("ShowContent") then
	Session("ShowContent") = AllowAnonymousSearch(ServiceID, NumSearchesAllowed)	
	if bdebugPrint then Response.Write "ShowContent= " & Session("ShowContent") &"<BR>"
End if
' Determines whether the user is logged into ChemACX Pro or ChemACX Net
Session("IsNet") = Not Session("okOnService_10")
%>
<%
' This next little bit checks for "special" types of accesses, such as from ChemDraw for Excel
' It's critically important that if we get into one of these bits, that *no* other text be
' exported.  One way to do that is to make sure that no output happens at any point prior to this.
userID = request.form("userID")
if NOT userID ="" then
	if NOT userID = "166" then
		response.write "OK"
	else 
		response.write "http://www.chemacx.com/chemacx/accessdenied.asp?userid=" & userID
	end if
	response.end
end if
if (NOT request("checkPriceLessOK")="") then
	response.write Application("addToCartWithoutPrice")
	response.end
end if
if (NOT request("addBatchToCart")="") then
	Application.Lock
	Application("TempShopcartConter")=Application("TempShopcartConter") + 1
	scCounter= Application("TempShopcartConter")
	Application.Unlock
	if scCounter > 9 then 
		scCounter = 1
		Application.Lock
			Application("TempShopcartConter") = 1
		Application.Unlock
	End if
	
	wddxPacket = Request("addBatchToCart")

	Set fs = Server.CreateObject("Scripting.FileSystemObject")
	tempDirPath =  Application("TempFileDirectory" & "chemacx") & "tempshopcart" & scCounter & ".txt"
	
	Set oTextStream = fs.CreateTextFile(tempDirPath,True,False)
	oTextStream.Write(wddxPacket)
	oTextStream.Close
	Set oTextStream = nothing
	Set fs = nothing
	response.write scCounter
	response.end
End if
%>
<style type="text/css">{  }
body, p, h1, h2, h3, h4, h5, td, li, a, dl { font-family: Verdana, arial, helvetica, sans-serif; font-size: x-small; }
tt, pre { font-family: monospace; }
sup, sub { font-size: 60%; }
</style>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<!--COWS1-->
<%
' Clears the supplier selection from a previous call via chemacx_pricepage.asp
Session("supplierSelect")=""
%>
<head>
<script language="JavaScript">
if(parent.location.href != window.location.href) parent.location.href = window.location.href;

var needToAuthenticate = <%=lcase(Not Session("ShowContent"))%>
var serverName = "http://<%=Request.ServerVariables("HTTP_HOST")%>";
function ifCAS() {
	var maybeCAS = document.npSearchForm.npSearchText.value;
	if ((isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-1,maybeCAS.length)))==false) 
	&& (maybeCAS.substring(maybeCAS.length-2,maybeCAS.length-1)=="-")
	&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-4,maybeCAS.length-2)))==false) 
	&& (maybeCAS.substring(maybeCAS.length-5,maybeCAS.length-4))=="-"
	&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-6,maybeCAS.length-5)))==false)){

		var firstdash=maybeCAS.indexOf("-", 1);
		if ((firstdash<=6) && (isNaN(maybeCAS.substring(1,firstdash))==false)){		
			var seconddash=maybeCAS.indexOf("-", firstdash+1);
			var isCAS=false;
			dashfree_maybeCAS=maybeCAS.substring(0,firstdash)+maybeCAS.substring(firstdash+1,firstdash+3)+maybeCAS.substring(seconddash+1,maybeCAS.length)
			CASsum=0;
			i=dashfree_maybeCAS.length-1; 
			while (i>=1) {
				CASsum=CASsum + i*(dashfree_maybeCAS.substring(dashfree_maybeCAS.length-i-1,dashfree_maybeCAS.length-i));
				i--;
			}
			if ((CASsum%10)==((dashfree_maybeCAS.substring(dashfree_maybeCAS.length-1,dashfree_maybeCAS.length))%10)) {
				isCAS=true;
				qs= "formgroup=basenp_form_group&dbname=chemacx&dataaction=query_string&field_type=TEXT&full_field_name=Substance.CAS&field_value=" + maybeCAS	
			}
			else {
				alert("The number you entered is not a valid CAS RN!");
				return(false);
			}
		}
	}
	else 
	{
		var fcriteria = "LIKE";
		if (maybeCAS.indexOf("=")==0){
			maybeCAS = maybeCAS.substring(1,maybeCAS.length);
			fcriteria = "=";
		}
		var dbquote = String.fromCharCode(34);
		if (maybeCAS.indexOf(dbquote)!= -1){
			maybeCAS = maybeCAS.replace(/"/g, "")
			fcriteria = "=";
		}
			<%if Application("DBMS")= "ORACLE"  then
			response.write "SynTableName =""ACX_SYNONYM"";" 
			else
			response.write "SynTableName =""SYNONYM"";"
			End if
			%>
			qs= "formgroup=basenp_form_group&dbname=chemacx&dataaction=query_string&field_criteria=" + fcriteria + "&field_type=TEXT&full_field_name=" + SynTableName + ".Name&field_value=" + escape(maybeCAS) 
	}
		
	if (needToAuthenticate){
		postURL = "postrelay.asp?"
	}
	else{
		postURL = "/ChemACX/default.asp?"
	}
		
	document.npSearchForm.action = postURL + qs
	document.npSearchForm.submit();
}
</script>
<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/DBShoppingCart.asp"-->	


<script LANGUAGE="JavaScript" SRC="chemacx/wddx.js"></script>
<script LANGUAGE="JavaScript" SRC="chemacx/wddxDes.js"></script>
<script LANGUAGE="JavaScript" SRC="chemacx/wddxRsEx.js"></script>
<script LANGUAGE="JavaScript" SRC="chemacx/shopcart_functions.js"></script>

<script language="JavaScript">
var productsShown= false;
var theMainFrame = top;
FillWDDXShopCart();


</script>


<% if NOT Application("UseCSWebUserAccounts") then%>
	<!--#include file="intranet_np_form.asp"-->
<%else%>
	<!--#include file="acxcom_np_form.asp"-->
<%end if%>




<!--#INCLUDE FILE = "purchasingforexcel.asp"-->


</body>
</html>



