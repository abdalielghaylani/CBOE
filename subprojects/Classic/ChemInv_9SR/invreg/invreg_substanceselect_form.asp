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
						&nbsp;<em><b><%=TruncateInSpan(SubstanceName, 50, "")%></b></em>
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
	notebookSQL = " batches.notebook_text AS NoteBook_text, "
end if
sql = "SELECT to_char(batch_number, '09') Batch_Number, batch_reg_date, last_mod_date," & notebookSQL & "notebook_page, people.user_id, amount_units, amount" &_
		 " FROM batches, cs_security.people" &_ 
		 " WHERE batches.scientist_id = cs_security.people.person_id(+)" &_
		 " AND batches.reg_internal_id=" & Reg_ID
'Response.Write sql
'Response.end
Set BatchRS = DataConn.Execute(sql)
if Not(BatchRS.BOF and BatchRS.EOF) then
%>
<span class="GuiFeedback">Select a registry batch to associate to the container</span>
<br><br>
<table border="0">
	<tr>
		<th>Batch Number</th>
		<th>Chemist</th>
		<th>Notebook</th>
		<th>Page</th>
		<th>Amount</th>
		<th>Reg Date</th>
		<th>Last Modified</th>
		<th>&nbsp;</th>
	</tr>
<%		
Do While Not BatchRS.EOF
	Response.Write "<tr>"
	Response.Write "	<td>" & htmlNull(BatchRS("batch_number")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("user_id")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("notebook_text")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("notebook_page")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("amount")) & "&nbsp;" & htmlNull(BatchRS("amount_Units")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("batch_reg_date")) & "</td>"
	Response.Write "	<td>" & htmlNull(BatchRS("last_mod_date")) & "</td>"
	Response.Write "    <td><a class=""MenuLink"" HREF=""#"" onclick=""AssignRegCompound(" & Reg_ID & "," & BatchRS("batch_number") & ",'" & baseRegNumber & "'); return false;"">select</a></td>"
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
			<a HREF="#" onclick="top.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
<%end if%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->

</body>

</html>
