<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
	CompoundID = Request("CompoundID")
	Dim Cmd
	Dim Conn
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".COMPOUNDS.GetSynonyms(?,?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PCOMPOUNDID",131, 1, 0, CompoundID)
	Cmd.Parameters("PCOMPOUNDID").Precision = 9	
	Cmd.Parameters.Append Cmd.CreateParameter("O_COMPOUNDNAME", 200, adParamOutput, 255, NULL)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
%>
<html>
<head>
	<title>Manage Substance Synonyms</title>
	<script LANGUAGE="javascript">
		window.focus();
	</script>

	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
	<center>
	<%=Cmd.Parameters("O_COMPOUNDNAME")%>
	<br><br>
	<table border="1">
	<tr>
		<th>
			Synonym
		</th>
		<th align="center">
			<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Synonyms.asp?CompoundID=<%=CompoundID%>', 'Diag', 1); return false">New</a>
		</td>
	</tr>
<%
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No synonyms found for this compound</Span></TD></tr></table>")
	Else
		While (Not RS.EOF)
			SynonymID = RS("Synonym_ID")
			SubstanceName = RS("Substance_Name")
			editData = "CompoundID=" & CompoundID & "&SynonymID=" & SynonymID & "&SubstanceName=" & server.URLEncode(SubstanceName)
%>
			<tr>
				<td align="center">
					<%=SubstanceName%>
				</td>
				<td align="center">
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Synonyms.asp?<%=editData%>', 'Diag', 1); return false">&nbsp;Edit</a>
					|
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Synonyms.asp?action=delete&amp;<%=editData%>', 'Diag',1); return false">Delete&nbsp;</a>
				</td>
			</tr>
			<%rs.MoveNext
		Wend
		Response.Write "</table>"
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing		
%>
<br><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
</center>
</body>
</html>
