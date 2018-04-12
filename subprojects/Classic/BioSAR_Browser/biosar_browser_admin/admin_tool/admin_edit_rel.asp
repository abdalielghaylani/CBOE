<%@ Language=VBScript %>




<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
</HEAD>

<%

	
	
	
	
	Dim oRSCol
	Dim lColumnId
	Dim sTableName
	Dim sColumnName
	Dim sOwnerName
	dim lCurrRelId
	dim lcurrlookupval
	dim currOwner
	dim currColumn
	
	lColumnId = Request("column_id")
	Set oConn = SysConnection
	
	Set oRSCol = oConn.Execute(SQLColumnTableById(lColumnId))
	sOwnerName = oRSCol("OWNER")
	sTableName = orsCol("TABLE_NAME")
	sColumnName = orsCol("COLUMN_NAME")
	sDataType = orsCol("DATATYPE")
	
	Dim oRSSelectedCols
	Set oRSSelectedCols = oConn.Execute(SQLRelationshipsByChildColumnId(lColumnId))
	bNoLookupText = true 'this is set to remove the option of settting no parent. this option shouls only show up if there is a preexisting parent table.
	if not (request("lookup_table") <> "" or  request("lookup_owner") <> "") then
		if Not (oRSSelectedCols.EOF and oRSSelectedCols.BOF) then
			lCurrRelId = oRSSelectedCols("COLUMN_ID")
			if  Not(lCurrRelId  = "" or lCurrRelId  = "-1") then
					Set oRSCol2 = oConn.Execute(SQLColumnTableById(lCurrRelId))
					currOwner =  oRSCol2("OWNER")
					currName =  oRSCol2("TABLE_ID")
			end if
		else
			currOwner = request("lookup_owner")
			currName = request("lookup_table")
			lCurrRelId =-1
		end if
	
	else
		currOwner = request("lookup_owner")
		currName = request("lookup_table")
		lCurrRelId =-1
	end if
	on error resume next
	lCurrRelId = CLng(lCurrRelId)
	on error goto 0
	if  lCurrRelId > 0 then
		bNoLookupText= false
	end if

	'Response.Write currOwner & ":" & currName  & ":" & lCurrRelId 
	'Response.Write Request.servervariables("query_string")
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Parent Column For:  </td><td class=form_purpose valign = "bottom"><%=stablename & "." & scolumnname & " <b>Datatype: </b>" & sDataType%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

<br>Select a parent schema, parent table and parent column. <br>


<form name=cancel_form method=post action="admin_table_view.asp?<%=session("admin_table_view_cancel_url")%>">
<input type = "hidden" name="cancel_form">

</form>

<form name=admin_form method=post action="admin_table_view.asp?formgroup=base_form_group&dbname=biosar_browser&action=edit_rels&child_col_id=<%=lColumnId%>&table_id=<%=orscol("table_id")%>&base_col_id=<%=lColumnId%>&user_name=<%=request("user_name")%>" id=form1 >
<table border = 0><td class="form_purpose_header"  valign = "bottom">
<%if bNoLookupText = false then%>

<tr>
<td colspan = 3><b>This column has no parent column</b></td>
<td align=center><input type=radio name=no_parent_col_id value=-1 ID="Radio1"></td>	 
</tr>

<%end if%>

</table><br>


<%
Dim sCheckedId


dim dispOwner
dim dispName

outputSchemasAndTablesAndColumns()%>

</table>

<%Sub outputSchemasAndTablesAndColumns()
	Response.Write "<table border = 0><tr>"
	Response.Write "<td class=table_cell_header>Parent Schema</td>"
	Response.Write "<td class=table_cell_header>Parent Table</td>"
	
	
	Response.Write "</tr><tr>"
	Response.Write "<td class=table_cell>" 
	currentOwner = WriteSchemaLookupSelector(currOwner)
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	
	currentTable=WriteTableLookupSelector(currentOwner, currName)
	Response.Write "</td></tr></table>"
	Response.Write "<br>"
	Response.Write "<table border = 0><tr>"
	Response.Write "<td class=table_cell_header>Parent Column (data type)</td>"
	Response.Write "<td class=table_cell_header>Join Type</td>"
	Response.Write "</tr><tr>"
	Response.Write "<td class=table_cell>" 
	WriteRelLookupIDColumnSelector currentOwner, currentTable, lCurrRelId,sDataType
	Response.Write "</td>"
	Response.Write "<td valign = ""top"" class=table_cell>" 
	WriteJoinTypeSelecter currentOwner, currentTable, lColumnId
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	

End Sub



Function WriteSchemaLookupSelector(currOwner)
	if Not len(trim(currOwner)) > 0 then
		
		currOwner = request("lookup_owner")
	end if
	Set oRSSchema = oConn.Execute(SQLAllExposedSchemas)
	
	Response.Write "<SELECT   size=10 class=""SelectBox"" name=""schema_selector"" onChange=""doSwitchLookupSchema()"" >" & vblf
		if not currOwner <> "" then
			checked = "selected"
			Response.Write "		<option " & checked & ">" & "Select a Schema" &"</option>"
		end if
		if  NOT (oRSSchema.EOF AND oRSSchema.BOF) then 
			Do While NOT oRSSchema.EOF
					if currOwner = oRSSchema("OWNER")  then
						checked = "selected"
					Else
						checked = ""
					End if	
				
				Response.Write "		<option " & checked & " value=""" & oRSSchema("OWNER") & """>" & oRSSchema("OWNER") &"</option>"
				oRSSchema.MoveNext
			Loop 
		end if
	Response.Write "</SELECT>" 
	WriteSchemaLookupSelector = currOwner
End Function	 




Function WriteTableLookupSelector(theOwner, theTable)

	if not len(trim(theTable))> 0  then
		theTable = request("lookup_table")
	end if
	if not len(trim(theOwner))> 0  then
		theOwner = request("lookup_owner")
	end if
	
	Response.Write "		<SELECT  size=10 class=""SelectBox"" name=""table_selector""  onChange=""doSwitchSchemaLookupTables()"" >" 
	
	if  theOwner <> "" then
	
		Set oRSTable = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'N' and owner = '" & theOwner & "' and is_exposed = 'Y' order by table_short_name asc")
		Set oRSView = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'Y' and owner = '" & theOwner & "' and is_exposed = 'Y' order by table_short_name asc")
		Set oDict = AuthorizedTables(theOwner)
		if not theTable <> "" then
			scheckedid = " checked"
			Response.Write "		<option " & scheckedid & ">" & "Select a table" &"</option>"
		end if
		if err.number = 0 then
			if  NOT ((oRSTable.EOF AND oRSTable.BOF) AND (oRSView.EOF AND oRSView.BOF))  then 
				on error resume next
				Do While NOT oRSTable.EOF
					if oDict.Exists(cstr(oRSTable("TABLE_NAME_SHORT"))) Or IGNORE_ADMIN_PERMISSIONS Then
						scheckedid = ""
						
						if clng(theTable) = clng(oRSTable("TABLE_ID")) then	
							scheckedid = " selected"
							
						End if
					Response.Write "		<option " & scheckedid & " value=""" & oRSTable.Fields("TABLE_ID") & """>" & oRSTable.Fields("TABLE_SHORT_NAME") & " " & addText &"</option>"
					end if
					oRSTable.MoveNext
				Loop 
				Do While NOT oRSView.EOF
					if oDict.Exists(cstr(oRSView("TABLE_NAME_SHORT"))) Or IGNORE_ADMIN_PERMISSIONS Then
					
						 scheckedid = ""
						
						if clng(theTable) = clng(oRSView("TABLE_ID")) then	
							scheckedid = " selected"
							
						End if
					Response.Write "<option " & scheckedid & " value=""" & oRSView.Fields("TABLE_ID") & """>" & oRSView.Fields("TABLE_SHORT_NAME") & " " & addText &"</option>"
					end if
					oRSView.MoveNext
				Loop 
				
			end if
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema"  & "</option>"

	end if
	Response.Write "</SELECT>" 
	WriteTableLookupSelector = theTable
End Function	 

Sub WriteRelLookupIDColumnSelector(theOwner, thetable, lCurrRelId, sDataType)
	Response.Write "		<SELECT size=10 class=""SelectBox"" name=""parent_col_id""  >" & vblf
	if  theOwner <> "" and theTable <> "" then
		on error resume next
		Set oRSCols = oConn.EXecute(SQLColumnByTableIDandDataType(thetable,sDataType))
		scheckedid = ""
		scheckedval = ""

		if err.number = 0 then
			if  lCurrRelId  = "" or lCurrRelId  = "-1" then
				checked = " selected"
				if  theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a parent table " &"</option>"
				end if 
				if Not theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a schema and parent table " &"</option>"
				end if
				if  theOwner <> "" and  theTable <> "" and not lCurrRelID <> "" then
					Response.Write "		<option " & checked & ">" & "Select  a parent ID column" &"</option>"
				end if
			end if
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				
				
				Do While NOT oRSCols.EOF
					scheckedid=""
					if clng(lCurrRelId) = clng(oRSCols("COLUMN_ID")) then	
						scheckedid = " selected"
					End if
					
					Response.Write "		<option " & scheckedid & " value=""" & oRSCols.Fields("COLUMN_ID") &""">" & oRSCols.Fields("COLUMN_NAME") & " (" & sDataType & ")" & "</option>"
					oRSCols.MoveNext
				Loop 
				
				
			end if
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema and table"  & "</option>"

	end if
	Response.Write "</SELECT>" 
End sub	 

Sub WriteJoinTypeSelecter(theOwner, thetable, columnID)
	Response.Write "		<SELECT size=4   class=""SelectBox"" name=""join_type""  >" & vblf
	if  theOwner <> "" and theTable <> "" then
		sql = "select join_type from biosardb.db_relationship where child_column_id = " & columnID
		Set oRSCols = oConn.Execute(sql)
		scheckedid = ""
		scheckedval = ""
		if err.number = 0 then
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				currentValue = oRSCols("join_type")	
			end if
			if not currentValue <> "" or isNull(currentValue) then
				currentValue = "INNER"
			end if
			supported_joins = "INNER,INNER:Case Insensitive,OUTER,OUTER:Case Insensitive"
			supported_joins_array = split(supported_joins, ",", -1)
			for i = 0 to UBound(supported_joins_array)
				scheckedid=""
				if UCase(supported_joins_array(i)) = UCase(currentValue) then	
					scheckedid = " selected"
				End if
					
				Response.Write "		<option " & scheckedid & " value=""" & supported_joins_array(i) & """>" & supported_joins_array(i) &"</option>"
			next
		else
			Response.Write "error occurred"
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema and table"  & "</option>"

	end if
	Response.Write "</SELECT>" 
End sub	 

%>
<!-- #INCLUDE FILE="footer.asp" -->

</body>
</html>