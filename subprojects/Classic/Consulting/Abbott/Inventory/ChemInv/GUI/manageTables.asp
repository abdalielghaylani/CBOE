<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim rsTable
if not isEmpty(Request.Form("TableName")) then
	TableName = Request.Form("TableName")
else
	TableName = Request.QueryString("TableName")
end if
%>
<html>
<head>
	<title><%=Application("appTitle")%> -- Manage Tables</title>
		<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
</head>
<body>

<center>
<form name="form1" method="GET" action="ManageTables.asp">
<INPUT TYPE="hidden" NAME="currTableName" VALUE="<%=TableName%>">
<table border="0">
	<tr>
		<td align="right" valign="top" nowrap>Select table:</td>
		<td><%=BuildSelectBoxFromDict(editable_tables_dict,"TableName",TableName,0,RepeatString(25, "&nbsp;"),"","",false,"document.form1.submit();return false;")%>
	</tr>
	<TR>
		<TD COLSPAN="2">
<%
if TableName <> "" then

	sortByField = Request("sortByField")
	fieldsNotShown = "RID,CREATOR,TIMESTAMP"
	img = ""
	if instr(TableName,"(") > 0 then
		openParenPos = instr(TableName,"(")
		closeParenPos = instr(TableName,")")
		semiColPos = instr(TableName,";")
		IDColumnName = ucase(mid(TableName,openParenPos+1,semiColPos-openParenPos-1))
		IDValue = mid(TableName,semiColPos+1,closeParenPos-semiColPos-1)
		dbTableName = left(TableName,openParenPos-1)
		SQL = "SELECT * FROM " & dbTableName & " WHERE " & IDColumnName & " = " & IDValue
		fieldsNotShown = fieldsNotShown & IDColumnName & ","
	else
		dbTableName = TableName
		SQL = "SELECT * FROM " & dbTableName
	end if

	if len(sortByField) > 0 then
		direction = " asc"
		img = "/cheminv/graphics/asc_arrow.gif"
		if not isempty(Session("lastSort")) then
			lastSort = Session("lastSort")
			lastField = left(lastSort,instr(lastSort," ")-1)
			if lastField = sortByField then
				lastDirection = mid(lastSort,instr(lastSort," "))
				if lastDirection = " asc" then 
					direction = " desc"
					img = "/cheminv/graphics/desc_arrow.gif"
				end if
			end if
		end if
		sortBy = sortByField & direction
		'Response.Write lastField & "=lastField<BR>"
		'Response.Write lastDirection & "=lastDirection<BR>"
		'Response.Write sortBy & "=sortBy<BR>"	
		Session("lastSort") = sortBy
		SQL = SQL & " ORDER BY " & sortBy
	end if
	
	Call getInvConnection()
	set Cmd = Server.CreateObject("adodb.command")
	Cmd.ActiveConnection = Conn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = SQL
	'Response.Write cmd.CommandText
	'Response.End
	
	Set rsTable = Server.CreateObject("adodb.recordset")
	Set rsTable = cmd.Execute
	
	' look for PK contraint
	SQL = "SELECT * FROM user_cons_columns WHERE table_name = '" & ucase(dbTableName) & "'"
	
	' Set up an ADO command
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GETPKCOLUMN", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PTABLENAME", 200, 1, 4000, ucase(dbTableName))

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
	Else
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GETPKCOLUMN")
	End if

	' Return the Update Status
	pkColumnName = Trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
	if instr("inv_owners",lcase(dbTableName)) = 0 then fieldsNotShown = fieldsNotShown & pkColumnName & ","
	'Response.Write pkColumnName
	'Response.End
	
	' get column information
	SQL = "SELECT * FROM all_tab_columns where table_name = upper('" & dbTableName & "')"
	Set rsSchema = Conn.Execute(SQL)
	
	isFirst = true
	QS = "TableName=" & TableName 
	QS = QS & "&dbTableName=" & dbTableName
	QS = QS & "&IDColumnName=" & IDColumnName
	QS = QS & "&pkColumnName=" & trim(pkColumnName)
	while not rsSchema.EOF
		colName = rsSchema("column_name")
		dataType = rsSchema("data_type")
		'Response.Write IDColumnName & ":" & colName & ":" & dataType & "<BR>"
		if instr(IDColumnName,colName) > 0 then
			QS = QS & "&" & colName & "=" & IDValue & ":" & dataType
		else
			QS = QS & "&" & colName & "=:" & dataType
		end if
		rsSchema.movenext
	wend	
	Response.Write "<TABLE BORDER=""0"" CELLPADDING=""2"" CELLSPACING=""0""><TR>"
	Response.Write "<TD ALIGN=""right""><A CLASS=""MenuLink"" HREF=""#"" ONCLICK=""OpenDialog('/Cheminv/GUI/EditOrDeleteTableRow.asp?" & QS & "&action=create', 'Diag',1); return false"">New Row</A></TD></TR>"
	Response.Write "<TR><TD><TABLE BORDER=""1"" CELLPADDING=""2"" CELLSPACING=""0"">"
	while not rsTable.EOF
		if isFirst then
			Response.Write ShowHeaderRow(rsTable, fieldsNotShown, sortByField, img)
			isFirst = false
		end if
		Response.Write ShowDataRow(rsTable, fieldsNotShown) & chr(13)
		rsTable.MoveNext
	wend
	Response.Write "</TABLE></TD></TR></TABLE>"

end if

%>
		</TD>
	</TR>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.location='/cheminv/gui/menu.asp'; return false;"><img SRC="../graphics/close.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>
	
</table>	
</form>
</center>
</body>
</html>
<%
Function ShowHeaderRow(ByRef RS, fieldsNotShown, sortByField, img)
	theRow = "<TR>"
	for each Field in RS.Fields
		if instr(fieldsNotShown,Field.Name) = 0 then
			if (Field.Name = sortByField) then
				theRow = theRow & "<TD><A CLASS=""MenuLink"" HREF=""ManageTables.asp?TableName=" & TableName & "&sortByField=" & Field.Name & """>" & Field.Name & "&nbsp;<IMG border=""0"" src=""" & img & """></A>&nbsp;</TD>"
			else
				theRow = theRow & "<TD><A CLASS=""MenuLink"" HREF=""ManageTables.asp?TableName=" & TableName & "&sortByField=" & Field.Name & """>" & Field.Name & "&nbsp;</A>&nbsp;</TD>"
			end if
		end if
	next
	theRow = theRow & "<TD>&nbsp;</TD></TR>" & chr(13)
	ShowHeaderRow = theRow

end function

Function ShowDataRow(ByRef RS, fieldsNotShown)
	QS = "TableName=" & TableName 
	QS = QS & "&dbTableName=" & dbTableName
	QS = QS & "&IDColumnName=" & IDColumnName
	QS = QS & "&pkColumnName=" & trim(pkColumnName)
	theRow = "<TR><TD>"
	for each Field in RS.Fields
		if instr(fieldsNotShown,Field.Name) = 0 then
			'write address link
			if instr(lcase(Field.Name),"address_id_fk") then
				if len(Field.value) > 0 then
					AddressText = "Edit Address"
				else
					AddressText = "Add Address"
					'AddressText = len(Field.value)
				end if
				'<a class="MenuLink" HREF="Admin Menu" onclick="OpenDialog('../gui/menu.asp', 'MenuDiag', 2); return false" target="_top">Tasks</a>

				theRow = theRow & "<A CLASS=""MenuLink"" HREF=""" & AddressText & """ onclick=""OpenDialog('/cheminv/gui/EditAddress.asp?TableName=" & TableName & "&TablePKID=" & RS(pkColumnName) & "&AddressID=" & Field.Value & "','AddressDiag', 4); return false;"">" & AddressText & "</a></TD><TD>"			
				'theRow = theRow & "test</TD><TD>"
			'all other columns
			else
				'theRow = theRow & "<INPUT TYPE=""hidden"" NAME=""" & Field.Name & """ VALUE=""" & Field.Value & """>" & Field.Value & "&nbsp;</TD><TD>"
				theRow = theRow & Field.Value & "&nbsp;</TD><TD>"
			end if
		'else
		'	theRow = theRow & "<INPUT TYPE=""hidden"" NAME=""" & Field.Name & """ VALUE=""" & Field.Value & """>"
		end if
		value = ""
		if not isNull(Field.value) then value = Field.value
		QS = QS & "&" & Field.Name & "=" & server.URLEncode(cstr(value)) & ":" & Field.Type
	next
	theRow = theRow & "<A CLASS=""MenuLink"" HREF=""#"" ONCLICK=""OpenDialog('/Cheminv/GUI/EditOrDeleteTableRow.asp?" & QS & "&action=edit', 'Diag',1); return false"">Edit</A>&nbsp;|&nbsp;<A CLASS=""MenuLink"" HREF=""#"" ONCLICK=""OpenDialog('/Cheminv/GUI/EditOrDeleteTableRow.asp?" & QS & "&action=delete', 'Diag',1); return false"">Delete</A></TD>"
	theRow = theRow & "</TR>" & chr(13)
	ShowDataRow = theRow
end function

Function BuildSelectBoxFromDict(valueDict, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismultiple, ChangeScript)
	Dim str
	Dim DisplayText
	Dim multiple
	
	If ismultiple then 
		multiple = "MULTIPLE"
	Else
		multiple = ""
	End if
	 
	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT size=""" & size & """ name=""" & name & """" & multiple & " onchange='" & ChangeScript & "'>"
	if valueDict.count = 0 AND Len(FirstOptionText) = 0 then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!" 
	Else
		
		If Len(FirstOptionText) > 0 then
			If isValueInList(SelectedValue,FirstOptionValue,",") then
				strSelected = "Selected=""True"""
			End if
			str = str &  "<OPTION " & strSelected & " value=""" & FirstOptionValue &  """>" & FirstOptionText
		End if
		For each key in valueDict
			strSelected = ""
			theValue = key
			'If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then 
			If isValueInList(SelectedValue,theValue,",") then
				strSelected = "Selected=""True"""
			End if
			DisplayText = valueDict.Item(key)
			
			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..." 
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<OPTION " & strSelected & " value=""" & theValue & """ id=""" & DisplayText &  """>" & DisplayText
		Next 
	End if
	str = str & "</SELECT>"
	BuildSelectBoxFromDict = str
End function

%>