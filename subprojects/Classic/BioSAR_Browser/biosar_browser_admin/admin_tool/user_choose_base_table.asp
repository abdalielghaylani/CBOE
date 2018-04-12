<%@ Language=VBScript %>


<HTML>
<HEAD>


<!-- #INCLUDE FILE="header.asp" -->
</HEAD>
<body>

<%
dim lgroupId
lgroupId = Request("formgroup_id")
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Choose Base Table</td>
</table><br>
<form name=cows_input_form  method="post" ID="Form2">
</form>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>
	<form name=cancel_form method=post action="user_tables.asp?<%=session("user_columns_cancel_url")%>">
	<input type="hidden" name = "cancel_form">
	</form>
Select the schema and base table for this form:<p>
<form action="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lgroupid%>&action=change_base_table" method=post id=form1 name=admin_form>


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

	Set oConn = SysConnection
	if not (request("lookup_table") <> "" or  request("lookup_owner") <> "") then
		if request("curr_id") <> "" then
		
			lCurrRelId = request("curr_id")
			if  Not(lCurrRelId  = "" or lCurrRelId  = "-1") then
					Set oRSCol2 = oConn.Execute(SQLExposedTableById(lCurrRelId))
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
	

	'Response.Write currOwner & ":" & currName  & ":" & lCurrRelId 
	'Response.Write Request.servervariables("query_string")

Dim sCheckedId


dim dispOwner
dim dispName

outputSchemasAndTables()%>

</table>
</form>
<%Sub outputSchemasAndTables()
	Response.Write "<table border = 0><tr>"
	Response.Write "<td class=table_cell_header>Schema</td>"
	Response.Write "<td class=table_cell_header>Base Table </td>"
	
	Response.Write "</tr><tr>"
	Response.Write "<td class=table_cell>" 
	currentOwner = WriteSchemaBaseTableSelector(currOwner)
	Response.Write "</td>"
	Response.Write "<td class=table_cell>" 
	
	currentTable=WriteBaseTableSelector(currentOwner, currName)
	Response.Write "</td></tr></table>"


End Sub



Function WriteSchemaBaseTableSelector(currOwner)
	if Not len(trim(currOwner)) > 0 then
		
		currOwner = request("lookup_owner")
	end if
	Set oRSSchema = oConn.Execute(SQLAllExposedSchemas)
	
	Response.Write "<SELECT size =""20"" class=""SelectBox"" name=""schema_selector"" onChange=""doSwitchBaseTableSchema()"" >" & vblf
		if not currOwner <> "" then
			checked = "selected"
			'Response.Write "		<option " & checked & ">" & "Select a Schema" &"</option>"
		end if
		if  NOT (oRSSchema.EOF AND oRSSchema.BOF) then 
			Do While NOT oRSSchema.EOF
					if currOwner = oRSSchema("OWNER")  then
						checked = "selected"
					Else
						checked = ""
					End if	
				
				Response.Write "		<option " & checked & " value=""" & oRSSchema("OWNER") & """>" & server.HTMLEncode(oRSSchema("DISPLAY_NAME"))&"</option>"
				oRSSchema.MoveNext
			Loop 
		end if
	Response.Write "</SELECT>" 
	WriteSchemaBaseTableSelector = currOwner
End Function	 




Function WriteBaseTableSelector(theOwner, theTable)
	if not len(trim(theTable))> 0  then
		theTable = request("lookup_table")
	end if
	if not len(trim(theOwner))> 0  then
		theOwner = request("lookup_owner")
	end if
	
	Response.Write "		<SELECT size =""20"" class=""SelectBox"" name=""table_id""  >" 
	
	if  theOwner <> "" then

		Set oRSTable = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'N' and owner = '" & theOwner & "' and is_exposed = 'Y' and Base_column_id is not null order by table_short_name asc")
		Set oRSView = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'Y' and owner = '" & theOwner & "' and is_exposed = 'Y' and Base_column_id is not null order by table_short_name asc")
		Set oDict = AuthorizedTables(theOwner)
		if not theTable <> "" then
			scheckedid = " checked"
			'Response.Write "		<option " & scheckedid & ">" & "Select a table" &"</option>"
		end if
		if err.number = 0 then
			if  NOT (oRSTable.EOF AND oRSTable.BOF) then 
			
				Do While NOT oRSTable.EOF
					if oDict.Exists(cstr(oRSTable("TABLE_NAME"))) Or IGNORE_ADMIN_PERMISSIONS Then
						scheckedid = ""
						
						if clng(theTable) = clng(oRSTable("TABLE_ID")) then	
							scheckedid = " selected"
						End if
					
						Response.Write "		<option " & scheckedid & " value=""" & oRSTable.Fields("TABLE_ID") & """>" & server.HTMLEncode(oRSTable.Fields("DISPLAY_NAME")) & " " & addText &"</option>"
					end if
					oRSTable.MoveNext
				Loop 
			end if
			if  NOT (oRSView.EOF AND oRSView.BOF) then 
				Do While NOT oRSView.EOF
					if oDict.Exists(cstr(oRSView("TABLE_NAME"))) Or IGNORE_ADMIN_PERMISSIONS Then
					
						 scheckedid = ""
						
						if clng(theTable) = clng(oRSView("TABLE_ID")) then	
							scheckedid = " selected"
						End if
					
						Response.Write "<option " & scheckedid & " value=""" & oRSView.Fields("TABLE_ID") & """>" & server.HTMLEncode(oRSView.Fields("DISPLAY_NAME")) & " " & addText &"</option>"
					end if
					oRSView.MoveNext
				Loop 
				
			end if
		end if
	
	else
		Response.Write "		<option >" & "Please select a schema"  & "</option>"

	end if
	Response.Write "</SELECT>" 
	WriteBaseTableSelector = theTable
End Function	 %>


<!-- #INCLUDE FILE="footer.asp" -->