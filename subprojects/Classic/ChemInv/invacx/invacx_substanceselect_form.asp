<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%response.expires = 0%>
<%' Copyright 2001-2002, CambridgeSoft Corp., All Rights Reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<script LANGUAGE="javascript" src="../gui/CalculateFromPlugin.js"></script>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title><%=Application("appTitle")%> Substances -- Form View</title>
<script LANGUAGE="javascript">
<!--
	// This flag tells the onload function called from recordset_footer to process the AfterOnLoad() function
	DoAfterOnLoad = true;
	
	function PostToCreateSubstace(BaseID){
		<%
		if  detectModernBrowser = false then 
		%>
		document.form1["inv_compounds.Structure"].value = cd_getData("Substance.Structure" + BaseID + "_ctrl", "chemical/x-cdx");
		<%
		else
		%>
		if (document.getElementById('SubstanceBASE64_CDX_' + BaseID + '_orig'))
			document.form1["inv_compounds.Structure"].value = document.getElementById('SubstanceBASE64_CDX_' + BaseID + '_orig').value;
		<%
		end if
		%>		
		document.form1.action = "/cheminv/gui/CreateSubstance2.asp";
		//alert(document.form1.Structure.value);
		//alert(document.form1.CAS.value);
		//alert(document.form1.ACX_ID.value);
		//alert(document.form1.SubstanceName.value);
		document.form1.submit();
	}
	
	// This function is called by recordset footer at the end of onload event
	function AfterOnLoad(){
	}		
//-->
</script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>

</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cheminv/invacx/GetACXSubstanceAttributes.asp"-->
<%
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value And detectModernBrowser = false then
	displayType = "cdx"
else
	displayType = "GIF"
end if
%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GUIFeedback">Add this ChemACX Substance to ChemInventory</span>
			
		</td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</span>
	</tr>
	<tr>
		<td>
			<table border="1">
				<tr>
					<!-- Structure -->
					<td>
						<%ShowCFWChemResult dbkey, formgroup, "Structure", "Substance.Structure",BaseID, displayType, 185, 130%>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
				<tr><!-- Header Row -->
					<td colspan="4" align="center">
						&nbsp;<em><b><%=TruncateInSpan(SubstanceName, 50, "")%></b></em>
					</td>
				</tr>
				<tr>
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>
					<!-- Row 1 Col 2-->
					<%=ShowField("CSNum:", "BaseID", 15, "")%>
					
				</tr>
				<tr>
					<!-- Row 2 Col 1-->
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
					<!-- Row 2 Col 2-->
					<%=ShowField("CAS Number:", "CAS", 15, "")%>
				</tr>
				<tr>
					<!-- Row 3 Col 1-->
					<%=ShowField("ACX ID:", "ACX_ID", 15, "")%>
					<!-- Row 3 Col 2-->
					<td></td><td><a class="MenuLink" href="View%20synonyms%20for%20this%20substance" onclick="OpenDialog('/cheminv/invacx/Synlookup.asp?CSNum=<%=BaseID%>&amp;recordNum=<%baseRuningIndex%>', 'SynDiag', 3); return false;" title="Look up synonyms for this substance">ChemACX Synonyms</a></td>
				</tr>
			</table>
		</td>
	</tr>
	<%if Session("IsPopUP") then%>
	<tr>
		<td colspan="2" align="right">
			<input type="hidden" name="inv_compounds.Substance_Name" value="<%=SubstanceName%>"> 
			<a HREF="#" onclick="top.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="PostToCreateSubstace(<%=BaseID%>); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
	<%end if%>
</table>


<%CloseRS(BaseRS)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<form name="form1" method="post" target="_top">
	<input type="hidden" name="add_order" value="inv_compounds">
	<input type="hidden" name="add_record_action" value="Duplicate_Search_No_Add">	
	<input type="hidden" name="commit_type" value="full_commit">	
	<input type="hidden" name="return_location" value>
	<input type="hidden" name="inv_compounds.Structure.sstype" value="1">
	<input type="hidden" name="ExactSearchFields" value="inv_compounds.Structure">
	<input type="hidden" name="RelationalSearchFields" value="inv_compounds.Substance_Name;0,inv_compounds.CAS;0,inv_compounds.ACX_ID;0,inv_compounds.Conflicting_Fields;0">
	<input type="hidden" name="inv_compounds.Structure" value>
	<input type="hidden" name="inv_compounds.CAS" value="<%=CAS%>">
	<input type="hidden" name="inv_compounds.ACX_ID" value="<%=ACX_ID%>">
	<input type="hidden" name="inv_compounds.Substance_Name" value="<%=SubstanceName%>">
	<input type="hidden" name="return_location" value>
	<input type="hidden" name="no_gui" value="true">
</form>
</body>
</html>
