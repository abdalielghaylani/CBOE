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
	dim lcurrlookupid
	dim lcurrlookupval
	dim currOwner
	dim currColumn
	bNoLookupText = true 'this is set to remove the option of settting no lookup. this option shouls only show up if there is a preexisting lookup table.
	lColumnId = Request("column_id")
	
	Set oConn = SysConnection
	Set oRSCol = oConn.Execute(SQLColumnTableById(lColumnId))
	sOwnerName = oRSCol("OWNER")
	sTableName = orsCol("TABLE_NAME")
	sColumnName = orsCol("COLUMN_NAME")
	sDataType = orsCol("DATATYPE")
	if not (request("lookup_table") <> "" or  request("lookup_owner") <> "") then
		lCurrLookupId = CnvLong(orsCol("LOOKUP_COLUMN_ID"), "DB_TO_VB")
		
		lCurrLookupVal = CnvLong(orscol("LOOKUP_COLUMN_DISPLAY"), "DB_TO_VB")
		lJoinTYpe =  CnvLong(orscol("LOOKUP_JOIN_TYPE"), "DB_TO_VB")
		lSortDesc =  orscol("LOOKUP_SORT_DIRECT")
		if  Not(lCurrLookupId  = "" or lCurrLookupId  = "-1") then
				Set oRSCol2 = oConn.Execute(SQLColumnTableById(lCurrLookupID))
				currOwner =  oRSCol2("OWNER")
				currName =  oRSCol2("TABLE_ID")
				
		end if
	else
		currOwner = request("lookup_owner")
		currName = request("lookup_table")
		lJoinType = ""
		lSortSesc = ""
		lCurrLookupId =-1
		lCurrLookupVal =-1
	end if
	if  lCurrLookupId > 0 then
		bNoLookupText= false
	end if
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Lookup Table For: </td><td class=form_purpose valign = "bottom"><%=stablename & "." & scolumnname & " <b>Datatype: </b>" & sDataType%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

Select a Schema and Table then select a  column to mark as the ID column for lookup and a column as the Value for the lookup<br>


<form name=cancel_form method=post action="admin_table_view.asp?<%=session("admin_table_view_cancel_url")%>">
<input type = "hidden" name="cancel_form">

</form>

<form name=admin_form method=post action="admin_table_view.asp?formgroup=base_form_group&dbname=biosar_browser&action=edit_lookup&table_id=<%=orscol("table_id")%>&base_col_id=<%=lColumnId%>&user_name=<%=request("user_name")%>" id=form1>



<table  cellspacing=0 cellpadding=5>
<%if bNoLookupText = false then%>
<tr>
<td colspan = 4><b>Do not use a lookup table for this column</b></td>
<td align=center><input type=radio name=no_lookup_id_col value=-1 ID="Radio1"></td>	 
</tr>
<%end if%>

<%
Dim sCheckedId
Dim sCheckedVal

dim dispOwner
dim dispName

outputSchemasAndTablesAndColumns()

%>
</table>

</form>

<%Sub outputSchemasAndTablesAndColumns()
	Response.Write "<table border = 0><tr>"
	Response.Write "<td class=table_cell_header>Lookup Schema</td>"
	Response.Write "<td class=table_cell_header>Lookup Table</td>"
	
	Response.Write "</tr><tr>"
	Response.Write "<td class=table_cell>" 
	currentOwner = WriteSchemaLookupSelector(currOwner)
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	
	currentTable=WriteTableLookupSelector(currentOwner, currName)
	Response.Write "</td></tr></table>"
	Response.Write "<br>"
	Response.Write "<table border = 0><tr>"
	Response.Write "<td class=table_cell_header>Linking Column (data type)</td>"
	Response.Write "<td class=table_cell_header>Display Column</td>"
	Response.Write "<td class=table_cell_header>Join Type</td>"
	Response.Write "<td class=table_cell_header>Sort Direction</td>"
	Response.Write "</tr><tr>"
	Response.Write "<td class=table_cell>" 
	WriteLookupIDColumnSelector currentOwner, currentTable, lCurrLookupId, sDataType
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	WriteLookupValueColumnSelector currentOwner, currentTable, lCurrLookupVal
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	WriteJoinTypeSelecter currentOwner, currentTable, lColumnId
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	WriteSortDirectionSelecter currentOwner, currentTable, lColumnId
	Response.Write "</td>"
	Response.Write "</tr></table>"
End Sub


Function WriteSchemaLookupSelector(currOwner)
	if Not len(trim(currOwner)) > 0 then
		
		currOwner = request("lookup_owner")
	end if
	Set oRSSchema = oConn.Execute(SQLAllExposedSchemas)
	
	Response.Write "<SELECT size=10  class=""SelectBox"" name=""schema_selector"" onChange=""doSwitchLookupSchema()"" >" & vblf
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
	
	Response.Write "		<SELECT size=10  class=""SelectBox"" name=""table_selector""  onChange=""doSwitchSchemaLookupTables()"" >" 
	
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
							
						else
							scheckedid = ""
						end if
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

Sub WriteLookupIDColumnSelector(theOwner, thetable, lcurrlookupid, sDataType)

	Response.Write "		<SELECT size=10 class=""SelectBox"" name=""lookup_id_col""  >" & vblf
	if  theOwner <> "" and theTable <> "" then
	
on error resume next
		Set oRSCols = oConn.EXecute(SQLColumnByTableIDandDataType(thetable, sDataType))
		scheckedid = ""
		scheckedval = ""
	
		if err.number = 0 then
			if  lcurrlookupid  = "" or lcurrlookupid  = "-1" then
				checked = " selected"
				if  theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a lookup table " &"</option>"
				end if 
				if Not theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a schema and lookup table " &"</option>"
				end if
				if  theOwner <> "" and  theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select  a lookup ID column" &"</option>"
				end if
			end if
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				
				
				Do While NOT oRSCols.EOF
					scheckedid=""
					if clng(lcurrlookupid) = clng(oRSCols("COLUMN_ID")) then	
						scheckedid = " selected"
					End if
					
					Response.Write "		<option " & scheckedid & " value=""" & oRSCols.Fields("COLUMN_ID") & """>" & oRSCols.Fields("COLUMN_NAME") & " (" & sDataType & ")" & "</option>"
					oRSCols.MoveNext
				Loop 
				
				
			end if
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema and table"  & "</option>"

	end if
	Response.Write "</SELECT>" 
End sub	 


Sub WriteLookupValueColumnSelector(theOwner, thetable, lcurrlookupvalue)
	
	Response.Write "		<SELECT size=10  class=""SelectBox"" name=""lookup_id_val"" >" & vblf
	if  theOwner <> "" and theTable <> "" then
		Set oRSCols = oConn.EXecute(SQLColumnByTableID(thetable))
		scheckedid = ""
		scheckedval = ""
	
		
		if err.number = 0 then
			if  lcurrlookupvalue  = "" or lcurrlookupvalue  = "-1" then
				checked = " selected"
				if  theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a lookup table " &"</option>"
				end if 
				if Not theOwner <> "" and Not theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select a schema and lookup table " &"</option>"
				end if
				if  theOwner <> "" and  theTable <> "" then
					Response.Write "		<option " & checked & ">" & "Select  a lookup value column" &"</option>"
				end if
			end if
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				Do While NOT oRSCols.EOF
					scheckedval=""
					if clng(lcurrlookupval) = clng(oRSCols("COLUMN_ID")) then	
						scheckedval = " selected"
					end if
					Response.Write "		<option " & scheckedval & " value=""" & oRSCols.Fields("COLUMN_iD") & """>" & oRSCols.Fields("COLUMN_NAME") &"</option>"
					oRSCols.MoveNext
				Loop 
				
				
			end if
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema and table"  & "</option>"

	end if
	Response.Write "</SELECT>" 
End sub	 

Sub WriteSortDirectionSelecter(theOwner, thetable, columnID)
	Response.Write "		<SELECT size=10  class=""SelectBox"" name=""lookup_sort_direct""  >" & vblf
	if  theOwner <> "" and theTable <> "" then
		on error resume next
		sql = "select lookup_sort_direct from biosardb.db_column where column_id = " & columnID
		Set oRSCols = oConn.Execute(sql)
		
		scheckedid = ""
		scheckedval = ""
	
		if err.number = 0 then
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				currentValue = oRSCols("lookup_sort_direct")
				
				if Not currentValue <> "" or isNull(currentValue) then
					currentValue = "ASC"
				end if	
			end if
			supported_sorts = "ASC,DESC"
			supported_sorts_array = split(supported_sorts, ",", -1)
			full_name = "ascending,descending"
			full_name_array = split(full_name, ",", -1)
			for i = 0 to UBound(supported_sorts_array)
				scheckedid=""
				if Ucase(supported_sorts_array(i)) = UCase(currentValue)then	
					
					scheckedid = " selected"
				End if
				
				Response.Write "		<option " & scheckedid & " value=""" & supported_sorts_array(i) & """>" & full_name_array(i) &"</option>"
			next
			
		else
			Response.Write "error occurred"
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema and table"  & "</option>"

	end if
	Response.Write "</SELECT>" 

End sub	 


Sub WriteJoinTypeSelecter(theOwner, thetable, columnID)
	Response.Write "		<SELECT size=10 class=""SelectBox"" name=""lookup_join_type""  >" & vblf
	if  theOwner <> "" and theTable <> "" then
		sql = "select lookup_join_type from biosardb.db_column where column_id = " & columnID
		Set oRSCols = oConn.Execute(sql)
		scheckedid = ""
		scheckedval = ""
		if err.number = 0 then
		
			if  NOT (oRSCols.EOF AND oRSCols.BOF) then 
				currentValue = oRSCols("lookup_join_type")	
			end if
			if not currentValue <> "" or isNull(currentValue) then
				currentValue = "OUTER"
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
<P>&nbsp;</P>

<!-- #INCLUDE FILE="footer.asp" -->

</body>
</html>