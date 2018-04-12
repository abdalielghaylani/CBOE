<%@ LANGUAGE=VBScript  %>

<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%response.expires = 0%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
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
	
	var holdTime = 3000;
	<%if Session("isCDP") = "TRUE" then%>
	if (cd_getBrowserVersion() >= 6) holdTime = 1;
	window.onload = function(){setTimeout("GetFormula();GetMolWeight();",holdTime)}
	<%end if%>
	// This function is called by recordset footer at the end of onload event
	function AfterOnLoad(){
		// Calculate the formula and molw from the plugin data
		var holdTime = 3000;
		<%if Session("isCDP") = "TRUE" then%>
		if (cd_getBrowserVersion() >= 6) holdTime = 1;
		setTimeout("GetFormula();GetMolWeight();",holdTime);
		<%end if%>
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
<!--#INCLUDE VIRTUAL ="/cheminv/invreg/GetRegSubstanceAttributes.asp"-->
<table border="0">
	<tr>
		<td>
			<table border="1">
				<tr>
					<!-- Structure -->
					<td>
						<%
						if Session("isCDP") = "TRUE" then
							specifier = 185
						else
							specifier = "185:gif"
						end if
						Base64DecodeDirect "invreg", "base_form_group", StructuresRS("BASE64_CDX"), "Structures.BASE64_CDX", cpdDBCounter, cpdDBCounter, specifier, 130
						%>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
				<tr><!-- Header Row -->
					<td colspan="4" align="center">
						<%
						if SubstanceName <> "" then
							Response.Write("&nbsp;<em><b>" & TruncateInSpan(SubstanceName, 50, "") & "</b></em>")
						else
							Response.Write("&nbsp;<em><b>No Substance Name</b></em>")
						end if
						%>
					</td>
				</tr>
				<tr>
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>
					<!-- Row 1 Col 2-->
					<%=ShowField("Reg Number:", "baseRegNumber", 15, "")%>
					
				</tr>
				<tr>
					<!-- Row 2 Col 1-->
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
					<!-- Row 2 Col 2-->
					<%=ShowField("CAS Number:", "CAS", 15, "")%>
				</tr>
				<tr>
					<!-- Row 3 Col 1-->
					<%=ShowField("Registered:", "RegDate", 15, "")%>
					<!-- Row 3 Col 2-->
					<%=ShowField("Registered by:", "UserID", 15, "")%>
				</tr>
				<tr>
					<td></td><td></td>
					<td></td><td><a class="MenuLink" href="Lookup%20alternate%20identifiers%20for%20this%20ChemReg%20substance" onclick="OpenDialog('/cheminv/invreg/altids.asp?RegID=<%=reg_ID%>&amp;recordNum=1', 'Synonyms_Window', 1); return false;" title="Lookup alternate ID's for this registry substance">Alternate ID's</a></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
	<p>&nbsp;</p>
	  <%CloseRS(BaseRS)%>
<%
if CBool(Application("UseNotebookTable")) then
	notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS NoteBook_text, " 
else
	notebookSQL = " RegNotebook AS NoteBook_text, "
end if
sql = "SELECT * " &_
		 " FROM inv_vw_reg_batches, inv_vw_reg_structures" &_ 
		 " WHERE inv_vw_reg_batches.regid = inv_vw_reg_structures.regid " &_
		 " AND inv_vw_reg_batches.regid = " & Reg_ID
		 
'Response.Write sql
'Response.end
Set BatchRS = DataConn.Execute(sql)
if Not(BatchRS.BOF and BatchRS.EOF) then
%>
<input TYPE="hidden" NAME="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<span class="GuiFeedback">Select a registry batch to associate to the container</span>
<br><br>
<table border="0">
	<tr>
	<%
		i = 0
		for each key in reg_fields_dict
			if key <> "BASE64_CDX" and i < 6 then
				Response.Write("<th>" & reg_fields_dict.item(key) & "</th>")
				i = i + 1
			end if
		next
	%>
		<th>&nbsp;</th>
	</tr>
	
<%		
Do While Not BatchRS.EOF
	Response.Write "<tr>"
	j = 0
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" and j < 6 then
			Response.Write("<td align=""right"">" & BatchRS(key) & "</td>")
			j = j + 1
		end if
	next
	Response.Write "    <td><a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewAllRegFields.asp?RegID=" & BatchRS("RegID") & "&amp;BatchNumber=" & BatchRS("BatchNumber") & "', 'RegDetails', 1); return false;"" title=""View all Registration data for this batch"">view all fields</a>&nbsp;|&nbsp;<a class=""MenuLink"" HREF=""#"" onclick=""AssignRegCompound(" & Reg_ID & "," & BatchRS("BatchNumber") & ",'" & baseRegNumber & "'); return false;"">select</a></td>"
	Response.Write "</tr>"
	BatchRS.MoveNext
Loop
end if
%>
<%if Session("IsPopUP") then%>
	<tr>
		<td colspan="8" align="right">
			<br>
			<input type="hidden" name="SubstanceName" value="<%=SubstanceName%>"> 
			<a HREF="#" onclick="top.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
		</td>
	</tr>
<%end if%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->

</body>

</html>
