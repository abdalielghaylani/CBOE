<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetContainerAttributes.asp"-->

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
' This lines must be before GetEHSAttributes but after GetContainerAttributes!
CompoundID = Request("CompoundID")
ContainerID = Request("ContainerID")
TrackBy = Request("TrackBy")
%>
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/GetEHSAttributes.asp"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Create or Edit EH&amp;S Information</title>

<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
	var DialogWindow;
	window.focus();

	// Validates container attributes
	function ValidateContainer(){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		//Populate hidden variables
		document.form1.SubstanceName.value = document.form1.iSubstanceName.value
		<%if TrackBy = "" then%>
		document.form1.SupplierName.value = document.form1.iSupplierName.value
		document.form1.SupplierCatNum.value = document.form1.iSupplierCatNum.value
		<%end if%>
		document.form1.EHSGroup1.value = document.form1.iEHSGroup1.value
		document.form1.EHSGroup2.value = document.form1.iEHSGroup2.value
		document.form1.EHSGroup3.value = document.form1.iEHSGroup3.value
		document.form1.EHSHealth.value = document.form1.iEHSHealth.value
		document.form1.EHSFlammability.value = document.form1.iEHSFlammability.value
		document.form1.EHSReactivity.value = document.form1.iEHSReactivity.value
		document.form1.EHSPackingGroup.value = document.form1.iEHSPackingGroup.value
		document.form1.EHSUNNumber.value = document.form1.iEHSUNNumber.value
		document.form1.EHSACGIHCarcinogenCategory.value = document.form1.iEHSACGIHCarcinogenCategory.value
		document.form1.EHSIARCCarcinogen.value = document.form1.iEHSIARCCarcinogen.value
		document.form1.EHSEUCarcinogen.value = document.form1.iEHSEUCarcinogen.value

		//SubstanceName is required

		if (document.form1.SubstanceName.value.length == 0){
			errmsg = errmsg + "- Substance Name is required.\r";
			bWriteError = true;
		}	
		// UN Number should be Numeric 
		if (!isNumber(document.form1.iEHSUNNumber.value)){
		errmsg = errmsg + "- UN Number should be numeric.\r";
		bWriteError = true;
		}	
		//Health must be between 0 and 4
		if ((document.form1.EHSHealth.value.length > 0) && ((document.form1.EHSHealth.value.length > 1) || (document.form1.EHSHealth.value < "0") || (document.form1.EHSHealth.value > "4"))){
			errmsg = errmsg + "- Health must be between 0 and 4.\r";
			bWriteError = true;
		}

		//Flammability must be between 0 and 4
		if ((document.form1.EHSFlammability.value.length > 0) && ((document.form1.EHSFlammability.value.length > 1) || (document.form1.EHSFlammability.value < "0") || (document.form1.EHSFlammability.value > "4"))){
			errmsg = errmsg + "- Flammability must be between 0 and 4.\r";
			bWriteError = true;
		}

		//Reactivity must be between 0 and 4
		if ((document.form1.EHSReactivity.value.length > 0) && ((document.form1.EHSReactivity.value.length > 1) || (document.form1.EHSReactivity.value < "0") || (document.form1.EHSReactivity.value > "4"))){
			errmsg = errmsg + "- Reactivity must be between 0 and 4.\r";
			bWriteError = true;
		}
		//Packing group  must be between 0 and 3
		if ((document.form1.EHSPackingGroup.value.length > 0) && ((document.form1.EHSPackingGroup.value.length > 1) || (document.form1.EHSPackingGroup.value < "0") || (document.form1.EHSPackingGroup.value > "3"))){
			errmsg = errmsg + "- Packing Group must be between 0 and 3.\r";
			bWriteError = true;
		}
		//ACGIH Carcinogen Category must be either A1 or A2
		if ((document.form1.EHSACGIHCarcinogenCategory.value.length > 0) && ((document.form1.EHSACGIHCarcinogenCategory.value.length > 2) || ((document.form1.EHSACGIHCarcinogenCategory.value != "A1") && (document.form1.EHSACGIHCarcinogenCategory.value != "A2")))){
			errmsg = errmsg + "- ACGIH Carcinogen Category must be either 'A1' or 'A2'.\r";
			bWriteError = true;
		}
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var bcontinue = true;
			
			if (bcontinue) document.form1.submit();
		}
	}
</script>

</head>
<body>
<form name="form1" method="POST" action="EditEHSInfo_action.asp">
<input TYPE="hidden" NAME="ContainerID" Value="<%=ContainerID%>">
<input TYPE="hidden" NAME="CompoundID" Value="<%=CompoundID%>">
<input TYPE="hidden" NAME="action" Value="<%=Request("action")%>">
<input TYPE="hidden" NAME="TrackBy" Value="<%=TrackBy%>">
<input TYPE="hidden" NAME="EHSFoundData" Value="<%=EHSFoundData%>">
<input TYPE="hidden" NAME="EHSIsDefaultSource" Value="<%=EHSIsDefaultSource%>">
<input TYPE="hidden" NAME="SubstanceName" Value>
<input TYPE="hidden" NAME="SupplierName" Value>
<input TYPE="hidden" NAME="SupplierCatNum" Value>
<input TYPE="hidden" NAME="EHSGroup1" Value>
<input TYPE="hidden" NAME="EHSGroup2" Value>
<input TYPE="hidden" NAME="EHSGroup3" Value>
<input TYPE="hidden" NAME="EHSHealth" Value>
<input TYPE="hidden" NAME="EHSFlammability" Value>
<input TYPE="hidden" NAME="EHSReactivity" Value>
<input TYPE="hidden" NAME="EHSPackingGroup" Value>
<input TYPE="hidden" NAME="EHSUNNumber" Value>
<input TYPE="hidden" NAME="EHSIsOSHACarcinogen" Value="<%=EHSIsOSHACarcinogen%>">
<input TYPE="hidden" NAME="EHSACGIHCarcinogenCategory" Value="<%=EHSACGIHCarcinogenCategory%>">
<input TYPE="hidden" NAME="EHSIsSensitizer" Value="<%=EHSIsSensitizer%>">
<input TYPE="hidden" NAME="EHSIsRefrigerated" Value="<%=EHSIsRefrigerated%>">
<input TYPE="hidden" NAME="EHSIARCCarcinogen" Value>
<input TYPE="hidden" NAME="EHSEUCarcinogen" Value>

<table border="0" cellspacing="0" cellpadding="0" width="500">
	<tr height="25">
		<%=ShowInputBox("Substance Name:", "SubstanceName", 50, "", true, true)%>
	</tr>
	<%if TrackBy = "" then%>
	<tr height="25">
		<%=ShowInputBox("Supplier Name:", "SupplierName", 50, "", true, true)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Supplier Cat Num:", "SupplierCatNum", 20, "", true, true)%>
	</tr>
	<%else%>
	<tr height="25">
		<%=ShowInputBox("CAS Num:", "CAS", 15, "", true, true)%>
	</tr>
	<%end if%>
	<tr height="25">
		<%=ShowInputBox("EHS Group 1:", "EHSGroup1", 10, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("EHS Group 2:", "EHSGroup2", 10, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("EHS Group 3:", "EHSGroup3", 10, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("UN Number:", "EHSUNNumber", 4, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Packing Group:", "EHSPackingGroup", 1, "", False, False)%>
	</tr>
	<tr height="25">
		<td>&nbsp;</td><td><input type="checkbox" name="isRefrigerated_cb" onclick="document.form1.EHSIsRefrigerated.value = (this.checked?1:0);">Is Refrigerated<br></td>
		<script language="Javascript">if (document.form1.EHSIsRefrigerated.value == "1") document.form1.isRefrigerated_cb.click(); </script>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Health:", "EHSHealth", 1, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Flammability:", "EHSFlammability", 1, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Reactivity:", "EHSReactivity", 1, "", False, False)%>
	</tr>
	<tr height="25">		
		<td>&nbsp;</td>
		<td><input type="checkbox" name="isOSHACarcinogen_cb" onclick="document.form1.EHSIsOSHACarcinogen.value = (this.checked?1:0);">Is OSHA Carcinogen<br></td>		
		<script language="Javascript">if (document.form1.EHSIsOSHACarcinogen.value == "1") document.form1.isOSHACarcinogen_cb.click(); </script>	
	</tr>		
	<tr height="25"><%=ShowInputBox("ACGIH Carcinogen Cat.:", "EHSACGIHCarcinogenCategory", 2, "", False, False)%>	</tr>
	<tr height="25">
		<%=ShowInputBox("IARC Carcinogen:", "EHSIARCCarcinogen", 10, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("EU Carcinogen:", "EHSEUCarcinogen", 50, "", False, False)%>
	</tr>
	<tr height="25">
		<td>&nbsp;</td><td><input type="checkbox" name="isSensitizer_cb" onclick="document.form1.EHSIsSensitizer.value = (this.checked?1:0);">Is Sensitizer<br></td>
		<script language="Javascript">if (document.form1.EHSIsSensitizer.value == "1") document.form1.isSensitizer_cb.click(); </script>
	</tr>

	<tr>
		<td colspan="2" align="right" height="20" valign="bottom"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/ChemInv/graphics/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<a HREF="#" onclick="ValidateContainer(); return false;"><img SRC="/ChemInv/graphics/ok_dialog_btn.gif" border="0"></a>
				<br><br>
		</td>
	</tr>
	<tr>
		<%If EHSIsDefaultSource = "2" Then%>
		<td colspan="2" align="center">(Note: displayed values are the defaults for this substance)</td>
		<%ElseIf EHSIsDefaultSource = "0" Then%>
		<td colspan="2" align="center">(Note: displayed values are for this particular supplier &amp; product, not the defaults for this substance)</td>
		<%Else%>
		<td colspan="2" align="center">(Note: displayed values are from another product with the same substance)</td>
		<%End If%>
	</tr>
</table>	
</form>
</body>
</html>
