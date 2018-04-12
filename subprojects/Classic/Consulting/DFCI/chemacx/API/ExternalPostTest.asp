<html>
<head>
<script LANGUAGE="javascript">
<!--
	window.focus();
	function getStrucData(){
		document.form1["Substance.Structure"].value = cd_getData("CDX","chemical/x-cdx");
		//alert(document.form1["Substance.Structure"].value)
		document.form1.action = "/chemacx/chemacx/chemacx_action.asp";
		document.form1.submit();
	}
	
	function ValidateQuery(){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// SubstanceName is required
		//if (document.form1.SubstanceName.value.length == 0) {
			//errmsg = errmsg + "- SubstanceName is required.\r";
			//bWriteError = true;
		//}
		// Report problems
		//if (bWriteError){
			//alert(errmsg);
		//}
		//else{
			document.form1.submit();
		//}
	}
//-->
</script>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<title>Post Data to ChemACX</title>
</head>
<body>
<br><br><br>
<center>
<form name="form1" method="POST">
	<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="2">
		
			</td>
			<td rowspan="3">
			<table cellspacing="0" cellpadding="0" border="1">
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
						<!--- <BR><BR>						<input type="checkbox" name="crossRef" value="1">Cross Reference<BR> --->
					</td>
				</tr>
			</table>
			</td>		
			
			
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td width="300">
				<select name="struc_search_type" size="1"><option selected value="Substructure">Substructure</option><option value="Exact">Exact Structure</option><option value="Similarity">Tanimoto Similarity</option></select>
				<br>
				<table border="1" cellpadding="0" cellspacing="0">
					<tr>
						<td>
							
							<!---<embed src="/CFWTEMP/chemacx/chemacxTemp/mt.cdx" border="0" width="280" height="200" id="5" name="CDX" type="chemical/x-cdx">-->
							<script language="javascript">cd_insertObjectStr("<embed src='/CFWTEMP/chemacx/chemacxTemp/mt.cdx' border='0' width='280' height='200' id='5' name='CDX' type='chemical/x-cdx'>");</script> 
							<!--Hidden field used to pass structure data is populated by getStrucData function above--->
							<input type="hidden" name="Substance.Structure" value>
						</td>
					</tr>
				</table>
			</td>
			<td valign="top">
				<table border="0">
					<tr>
			        	<td>
							<span title="(eg. Acetonitrile)">Substance Name:</span><br>
							<input type="text" name="Synonym.Name" SIZE="30" value maxlength="255">  
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)">CAS Registry#:</span><br>
							<input type="text" name="Substance.CAS" SIZE="30" value maxlength="15"> 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<input type="text" name="Substance.ACX_ID" SIZE="30" value maxlength="15">
						</td>
					</tr>
					<tr>
			        	<td>
							<span title="(eg. C6F1-3)">Molecular Formula:</span><br>
							<input type="text" name="Substance.Formula" SIZE="30" value maxlength="255">  
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(&lt;110, &lt;600, 110-150)">MolWeight Range:</span><br>
							<input type="text" name="Substance.MolWeight" SIZE="30" value maxlength="15"> 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. 1003)">Catalog Number<br>
							<input type="text" name="Product.CatalogNum" SIZE="30" value maxlength="15">
						</td>
					</tr>
		    	</table>
			</td>
		  </tr>
		  <tr>
			<td colspan="3">
				Return structure data as: 
				<input TYPE="radio" name="structype" value="mol">Mol String
				<input TYPE="radio" name="structype" value="base64cdx">base64cdx
				<input TYPE="radio" name="structype" value="cdx" checked>cdx
				<input TYPE="radio" name="structype" value="gif">gif
			</td>
		  </tr>
		 
		  <tr>
			<td align="right" colspan="3">
				<a HREF="../Post%20Data%20to%20ChemACX" onclick="getStrucData(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Post query to ChemACX server"></a>
			</td>
		  </tr>
		</table>
	<input TYPE="hidden" name="return_location" value="/chemacx/chemacx/acxxml.asp">
	<input TYPE="hidden" name="dataaction" value="search">
	<input TYPE="hidden" name="dbname" value="chemacx">
	<input TYPE="hidden" name="formgroup" value="base_form_group">
	<input TYPE="hidden" name="metadata_directive" value="blind">
	<input TYPE="hidden" name="return_data" value="redirect">
	<input TYPE="hidden" name="store_request" value="true">
	<input TYPE="hidden" name="reload_basers" value="false">
	
	
</form>
</center>
</body>
</html>

