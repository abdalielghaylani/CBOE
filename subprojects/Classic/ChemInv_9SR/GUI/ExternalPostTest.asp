<html>
<head>
<script LANGUAGE="javascript">
<!--
	window.focus();
	function getStrucData(){
		document.form1["inv_compounds.Structure"].value = cd_getData("CDX","chemical/x-cdx");
		//alert(document.form1["inv_compounds.Structure"].value)
		document.form1.action = "/cheminv/cheminv/cheminv_action.asp";
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
<title>Post Data to ChemInv</title>
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
		  	<td rowspan="2">
		  		<img src="../../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td width="300">
				<select name="struc_search_type" size="1"><option selected value="Substructure">Substructure</option><option value="Exact">Exact Structure</option><option value="Similarity">Tanimoto Similarity</option></select>
				<br>
				<table border="1" cellpadding="0" cellspacing="0">
					<tr>
						<td>
							
							<!---<embed src="/CFWTEMP/ChemInv/ChemInvTemp/mt.cdx" border="0" width="280" height="200" id="5" name="CDX" type="chemical/x-cdx">-->
							<%
							if session("DrawPref") = "ISIS" then
							            ISISDraw = " isisdraw=" & "'" & "True" &  "'"
							else
							            ISISDraw = " isisdraw=" & "'" & "False" &  "'"
							end if							
							%>
							<script language="javascript">cd_insertObjectStr("<embed src='/CFWTEMP/ChemInv/ChemInvTemp/mt.cdx' border='0' width='280' height='200' id='5' name='CDX' type='chemical/x-cdx'>"<%=ISISDraw%>);</script> 
							<!--Hidden field used to pass structure data is populated by getStrucData function above--->
							<input type="hidden" name="inv_compounds.Structure" value>
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
							<input type="text" name="inv_compounds.CAS" SIZE="30" value maxlength="15"> 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<input type="text" name="inv_compounds.ACX_ID" SIZE="30" value maxlength="15">
						</td>
					</tr>
					<tr>
			        	<td>
							<span title="(eg. C6F1-3)">Molecular Formula:</span><br>
							<input type="text" name="inv_compounds.Formula" SIZE="30" value maxlength="255">  
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(&lt;110, &lt;600, 110-150)">MolWeight Range:</span><br>
							<input type="text" name="inv_compounds.MolWeight" SIZE="30" value maxlength="15"> 
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
			<td align="right" colspan="3">
				<a HREF="../Post%20Data%20to%20ChemInv" onclick="getStrucData(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt="Post query to ChemInv server" WIDTH="61" HEIGHT="21"></a>
			</td>
		  </tr>
		</table>
	
	<input TYPE="hidden" name="dataaction" value="search">
	<input TYPE="hidden" name="dbname" value="ChemInv">
	<input TYPE="hidden" name="formgroup" value="substances_form_group">
	<input TYPE="hidden" name="metadata_directive" value="blind">
	<input TYPE="hidden" name="return_data" value="CSV">
	<input TYPE="hidden" name="store_request" value="false">
	<input TYPE="hidden" name="reload_basers" value="false">
	
	
</form>
</center>
</body>
</html>

