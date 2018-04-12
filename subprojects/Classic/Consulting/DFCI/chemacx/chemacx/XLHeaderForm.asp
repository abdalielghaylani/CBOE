

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
	<title>Untitled</title>
</head>

<body>
<form name="ReqHeader" action="FillXLTemplate.asp" method="POST" target="_new">
<h2 align="center">Requisitioner Information</h2>
<div align="center">

<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
	<tr>
		<td rowspan="3">
		<b>Ship to:</b>
		</td>
		<td>
			<input type="text" name="E4" value="<%=Request.Cookies("XLHeader")("E4")%>" size="50">
		</td>
	</tr>
	<tr>
		<td>
			<input type="text" name="E5" value="<%=Request.Cookies("XLHeader")("E5")%>" size="50">
		</td>
	</tr>
	<tr>
		<td>
			<input type="text" name="E6" value="<%=Request.Cookies("XLHeader")("E6")%>" size="50">
		</td>
	</tr>	
</table>
<br><br>
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
	<tr>
    	<td>
			<b>Requested By:</b>
		</td>
    	<td>
			<input type="text" size="30" name="E15" value="<%=Request.Cookies("XLHeader")("E15")%>">
		</td>
	</tr>
	<tr>
    	<td>
			<b>Phone:</b>
		</td>
    	<td>
			<input type="text" size="15" name="E7" value="<%=Request.Cookies("XLHeader")("E7")%>">
		</td>
	</tr>
	<tr>
    	<td>
			<b>Requested date:</b>
		</td>
    	<td>
			<input type="text" size="10" name="I15" value="<%=Request.Cookies("XLHeader")("I15")%>">
		</td>
	</tr>
	<tr>
    	<td>
			<b>Required date:</b>
		</td>
    	<td>
			<input type="text" size="10" name="I20" value="<%=Request.Cookies("XLHeader")("I20")%>">
		</td>
	</tr>
	<tr>
		<td>
			<b>Expense Account Code:</b>
		</td>
    	<td>
			<input type="text" size="10" name="I24" value="<%=Request.Cookies("XLHeader")("I24")%>">
		</td>
	</tr>
</table>
<br><br>
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
	<tr>
		<td>
		<b>Comments:</b>
		</td>
		<td>
			<textarea rows="4" cols="50" name="E27"><%=Request.Cookies("XLHeader")("E27")%></textarea>
		</td>
	</tr>
</table>
<br><br>
<input type="hidden" name="_wddxitems" value="<%=Request.Form("WDDXContent")%>">

<!--- Mappings for Fields from Shopping cart --->
<!--- Line items valid fields: qty,unitmeas,cas,catnum,description,unitprice,totalprice,prodid,vendorcode,vendorName,catnum2,listprice,csymnbol --->
<input type="hidden" name="_lineitems" Value="B33:qty,C33:unitmeas,D33:catnum,E33:cas,F33:description,I33:unitprice">

<!--- Mappings for Vendor Contact Fields --->
<!--- vendormail valid fields:SupplierID,SupplierFullName,SupplierShortName,SupplierCodeName,Addr1,Addr2,City,State,ZipCode,Country --->
<input type="hidden" name="_VendorMailRS" Value="E18:supplierfullname,E19:Addr1,E20:Addr2,E21:City~State~ZipCode,E22:Country">


<!--- Mappings for Vendor Phone Fields --->
<!--- vendor phone valid fields:SupplierID,SupplierFullName,SupplierShortName,SupplierCodeName,NumberType,CountryCode,AreaCode,PhoneNum,Location --->
<input type="hidden" name="_VendorPhoneRS" Value="E24:CountryCode~AreaCode~PhoneNum">


<!--- Mappings for Vendor Fax Fields --->
<!--- vendor phone valid fields:SupplierID,SupplierFullName,SupplierShortName,SupplierCodeName,NumberType,CountryCode,AreaCode,PhoneNum,Location --->
<input type="hidden" name="_VendorFaxRS" Value="E25:CountryCode~AreaCode~PhoneNum">


<a href="Close" onclick="parent.opener.focus(); parent.close(); return false"><img src="<%=Application("NavButtonGifPath")%>close_btn.gif" alt="Close the shopping cart window" border="0"></a>
<a href="Next" onclick="submit();return false"><img src="<%=Application("NavButtonGifPath")%>export_excel_btn.gif" alt="Export to Excel Template" border="0"></a>
</form> 
</div>
</body>
</html>

