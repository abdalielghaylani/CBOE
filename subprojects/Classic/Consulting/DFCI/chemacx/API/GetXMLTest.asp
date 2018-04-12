<html>
<head>
<script LANGUAGE="javascript">
	window.focus();
</script>
<title>GetXML Test Page</title>
</head>
<body>
<br>
<center>
<form name="form1" action="/chemacx/api/getXMLdata.asp" method="POST">
	
			<table cellspacing="0" cellpadding="0" border="1">
				<tr>
					<td>
						FieldName
						<select name="fieldName">
							<option>CsNum
							<option>ProductID
							<option>PackageID
							<option>CAS
							<option>ACX_ID
						</select>
					</td>
				</tr>
				<tr>
					<td>
						ValueList: <input type="text" name="valueList" size="50">
					</td>
				</tr>
				<tr>
					<td>Tags to include in ACXXML:
						<br>
						<input type="checkbox" name="substance" value="1" checked="1" onClick="substance.checked=1">&lt;substance&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="synonym" value="1">&lt;synonym&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="product" value="1" onClick="if ((prodProperty.checked)||(package.checked)) {product.checked = 1;}">&lt;product&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="prodProperty" value="1" onClick="product.checked=1">&lt;prodProperty&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="package" value="1" onClick="product.checked=1">&lt;package&gt;<br>
						<input type="checkbox" name="supplier" value="1" onClick="if ((supplierAddress.checked)||(supplierEmail.checked)||(supplierURL.checked)||(supplierPhone.checked)) {supplier.checked = 1;}">&lt;supplier&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="supplierAddress" value="1" onClick="supplier.checked=1">&lt;supplierAddress&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="supplierEmail" value="1" onClick="supplier.checked=1">&lt;supplierEmail&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="supplierURL" value="1" onClick="supplier.checked=1">&lt;supplierURL&gt;<br>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="supplierPhone" value="1" onClick="supplier.checked=1">&lt;supplierPhone&gt;<br>
					</td>
				</tr>
			
		  <tr>
			<td colspan="3">
				Return structure data as: 
				<input TYPE="radio" name="structType" value="mol">Mol String
				<input TYPE="radio" name="structType" value="base64cdx">base64cdx
				<input TYPE="radio" name="structType" value="cdx" checked>cdx
				<input TYPE="radio" name="structType" value="gif">gif
			</td>
		  </tr>
		  <tr>
			<td>
				CrossRef: <input type="checkbox" name="crossRef" value="1">
			</td>
		  </tr>
		  <tr>
			<td align="right" colspan="3">
				<input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21">
			</td>
		  </tr>
		</table>
	<input TYPE="hidden" name="return_location" value>
	<input TYPE="hidden" name="dataaction" value="search">
	<input TYPE="hidden" name="dbname" value="chemacx">
	<input TYPE="hidden" name="formgroup" value="base_form_group">
	<input TYPE="hidden" name="metadata_directive" value="blind">
	<input TYPE="hidden" name="return_data" value="redirect">
	<input TYPE="hidden" name="store_request" value="false">
	<input TYPE="hidden" name="reload_basers" value="false">
	<input TYPE="hidden" name="CSUSERNAME" value="<%=Session("UserName" & "chemacx")%>">
	<input TYPE="hidden" name="CSUSERID" value="<%=Session("UserID" & "chemacx")%>">
</form>
</center>
<br><br>
<p>GetXMLData</p>
<p>Returns ChemACX data in ACXML format</p>
<p>USAGE:  http://<servername>/chemacx/api/GetACXData.asp?FieldName=varChar&amp;ValueList=CSV&amp;StructType=varChar</p>
<p>Parameters:<p>
<p>FieldName: (CsNum|ProductID|PackageID|CAS|ACX_ID) The query field
<p>ValueList: A comma delited list of query values corresponding to FieldName
<p>StuctType: (cdx|base64cdx|gif|mol) The format of the structure data used in the datagram
<p>Additonally the following optional boolean parameters control the contents of the XML datagram:</p>
<ul>
	<li> Substance
	<li> Synonym
	<li> Product
	<li> ProdProperty
	<li> Package
	<li> Supplier
	<li> SupplierAddress
	<li> SupplierEmail
	<li> SupplierURL
	<li> SupplierPhone
</ul>
<p>Crossref is a boolean that causes product and or package ID to be crossreferenced against other suppliers.</p>
</body>
</html>

