<%
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
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<%
' Clears the supplier selection from a previous call via chemacx_pricepage.asp
Session("supplierSelect")=""
%>
<head>
<script language="javascript">
function ifCAS() {

var maybeCAS = document.npSearchForm.npSearchText.value;
if ((isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-1,maybeCAS.length)))==false) 
	&& (maybeCAS.substring(maybeCAS.length-2,maybeCAS.length-1)=="-")
	&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-4,maybeCAS.length-2)))==false) 
	&& (maybeCAS.substring(maybeCAS.length-5,maybeCAS.length-4))=="-"
	&& (isNaN(parseFloat(maybeCAS.substring(maybeCAS.length-6,maybeCAS.length-5)))==false)) {
	//alert("1");
	var firstdash=maybeCAS.indexOf("-", 1);


	if ((firstdash<=6) && (isNaN(maybeCAS.substring(1,firstdash))==false)){
		//alert("2");
		var seconddash=maybeCAS.indexOf("-", firstdash+1);

		var isCAS=false;

		dashfree_maybeCAS=maybeCAS.substring(0,firstdash)+maybeCAS.substring(firstdash+1,firstdash+3)+maybeCAS.substring(seconddash+1,maybeCAS.length)
		//alert("dashfree="+ dashfree_maybeCAS);
		CASsum=0;
		i=dashfree_maybeCAS.length-1; 
		while (i>=1) {
		//alert("i="+i);
		CASsum=CASsum + i*(dashfree_maybeCAS.substring(dashfree_maybeCAS.length-i-1,dashfree_maybeCAS.length-i));
		//alert("CASsum=" + CASsum);
		i--;
		}

	if ((CASsum%10)==((dashfree_maybeCAS.substring(dashfree_maybeCAS.length-1,dashfree_maybeCAS.length))%10)) {
		isCAS=true;
		document.npSearchForm.action= "/ChemACX/default.asp?formgroup=basenp_form_group&dbname=chemacx&dataaction=query_string&field_type=TEXT&full_field_name=Substance.CAS&field_value=" + maybeCAS
		
		return(true);
		
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
	
	document.npSearchForm.action= "/ChemACX/default.asp?formgroup=basenp_form_group&dbname=chemacx&dataaction=query_string&field_criteria=" + fcriteria + "&field_type=TEXT&full_field_name=Synonym.Name&field_value=" + escape(maybeCAS)
	return(true);
	}
}



</script>
<title>CambridgeSoft's ChemACX.com - Search, find and buy chemicals online.</title>


<script LANGUAGE="JavaScript" SRC="cheminv/wddx.js"></script>
<script LANGUAGE="JavaScript" SRC="cheminv/wddxRsEx.js"></script>
<script LANGUAGE="JavaScript" SRC="cheminv/shopcart_functions.js"></script>

<script language="JavaScript">
var productsShown= false;
var theMainFrame = top;
//FillWDDXShopCartFromCookie();
</script>

</head>


<body>
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top"><img SRC="<%=Application("NavButtonGifPath")%>chemacx_banner_narrow.gif" BORDER="0" ALT="ChemACX"></td>
		<td valign="top" align="right">
		<a href="shop" onclick="ViewCart(); return false"><img src="graphics/shopping_cart_2.gif" alt="View Shopping cart" border="0"></a>
		</td>
	</tr>
</table>

<br><br>

<table border="0" cellpadding="0" cellspacing="0">
<tr>
	<td width="20"></td>
	<td valign="top" width="500" bgcolor="#FFFFD6">
	<br>
<form name="npSearchForm" onsubmit="ifCAS()" method="POST" target="_top">
&nbsp;&nbsp;<font size="-1">Enter a chemical name or a CAS Number</font><br>
&nbsp;&nbsp;<input type="text" name="npSearchText" size="40" value>
<input type="submit" value="Search">
<br>
&nbsp;&nbsp;Or choose: <a target="_top" href="<%=Application("AppPathHTTP")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=chemacx">Advanced Query with Plugin</a><br>
<input type="hidden" name="Substance.CAS" value>
<input type="hidden" name="Synonym.Name" value>
</form>
<br Clear="All">
</td>
	<td valign="middle" width="100" align="center">
	
	</td>
</tr>
</table>

<% 
if (NOT request.QueryString("shopcartID")="") then
scReqNum = request.QueryString("shopcartID")
Set fs = Server.CreateObject("Scripting.FileSystemObject")
tempDirPath =  Application("TempFileDirectory" & "chemacx") & "tempshopcart" & scReqNum  & ".txt"
Set oTextStream = fs.OpenTextFile(tempDirPath)
wddxPacket = oTextStream.ReadAll
oTextStream.Close
Set oTextStream = nothing
Set fs = nothing
'response.write "<BR>" & wddxPacket

Set MyDeser = Server.CreateObject("WDDX.Deserializer.1")
Set qty_ST = MyDeser.deserialize(wddxPacket)
packid_AR = qty_ST.getPropNames()
packid_list=""
For Each packid In packid_AR
packid_list = packid_list & packid & ","
Next
packid_list = left(packid_list,len(packid_list)-1)
'response.write packid_list

'get connection string from application variable
connection_array = Application( "base_connection" & "chemacx")
ConnStr = connection_array(0) & "="  & connection_array(1)
'response.write ConnStr
Set AdoConn = Server.CreateObject("ADODB.Connection")

SQLQuery = "SELECT [Package].[PackageID] AS packid, [Product].[SupplierID] AS supid, [Supplier].[ShortName] AS supname,[Product].[CatalogNum] AS catnum, [Product].[ProdName] AS descript, [Package].[Size] AS thesize, [Package].[" & Application("listPricefield") &  "] AS listprice, [Package].[" & Application("pricefield") & "] AS price, [Package].[CSYMBOL] AS thecsymbol FROM (Package INNER JOIN Product ON Package.ProductID = Product.ProductID) INNER JOIN Supplier ON Product.SupplierID = Supplier.SupplierID WHERE Package.PackageID IN (" & packid_list & ")" 



AdoConn.Open ConnStr
set ShopCartRS = AdoConn.Execute(SQLQuery)
rowsarray = ShopCartRS.GetRows()
numItems=UBound(rowsarray, 2)+1
set AdoConn= nothing
'response.write numItems
sq= "'"
jj = "','"
qq = Chr(34)

ShopCartRS.MoveFirst
if NOT (ShopCartRS.EOF AND ShopCartRS.BOF) then
response.write "<SCRIPT language=JavaScript>"
	While Not ShopCartRS.EOF

	
'escape text fields
supname = Replace(ShopCartRS("supname"),"'" ,"@@")
supname = Replace(supname,qq,"~~")
descript = Replace(ShopCartRS("descript"),"'" ,"@@")
descript = Replace(descript,qq,"~~")

addtocart = "AddtoShopcartWRSRemote(" & qty_ST.getProp(ShopCartRS("packid")) & "," & sq & ShopCartRS("packid") & jj &  ShopCartRS("supid") & jj & supname & jj & ShopCartRS("catnum") & jj & descript & jj & ShopCartRS("thesize") & jj & ShopCartRS("price") & jj & ShopCartRS("listprice") & jj & ShopCartRS("thecsymbol") & sq & ");"
	response.write addtoCart
	ShopCartRS.MoveNext
	Wend
response.write "ViewCart();"	
response.write "</script>"
end if


end if%>


</body>
</html>



