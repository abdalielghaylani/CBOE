


<SCRIPT LANGUAGE=vbscript RUNAT=Server>



Sub CreateXMLTemplates(formgroup_id)
	if not isObject(uConn) then
		Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
	end if

	xmlTemplateBasePath = "/biosar_browser/config/xml_templates/"

	
	Set RS = Server.CreateObject("adodb.recordset")
	
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  uConn
	cmd.CommandType = adCmdText
	Call DeleteXMLTemplates(cmd,formgroup_id)
	call RestTableOrder(formgroup_id)

	'Create List View Columns Across anad Column Down
	'get the max table order for looping in the following function. This is conditional because if maxTableOrder = 0 it means the formgroup didn't have any formitems
	'so we don't want to go any further since errors will occur.
	maxTableOrder_L = getMaxTableOrder(RS, cmd, formgroup_id,FORM_LIST)
	maxTableOrder_D = getMaxTableOrder(RS, cmd, formgroup_id,FORM_DETAIL)
	maxTableOrder_Q= getMaxTableOrder(RS, cmd, formgroup_id,FORM_QUERY)
	
	
	if  maxTableOrder_L > 0 then
	
		Call CreateListView_CD(RS, cmd,formgroup_id,maxTableOrder_L)
		Call CreateListView_CA(RS, cmd,formgroup_id,maxTableOrder_L)
	end if
	
	
	if maxTableOrder_D > 0 then
		
		Call CreateDetailView(RS, cmd,formgroup_id,maxTableOrder_D)
		'Call CreateExcelTemplate(RS, cmd,formgroup_id)
		Call CreateDetailView_ChildOnly(RS, cmd,formgroup_id,maxTableOrder_D)
	end if
	
	Call CreateDisplaySQL(formgroup_id,maxTableOrder_L,maxTableOrder_D,maxTableOrder_Q)
	
	
End sub

Function getMaxTableOrder(RS, cmd, formgroup_id,formtype_id)
	
	'DGB because the cmd is reused we need to make sure we delete the parameters before adding them
	DeleteParameters(cmd)
	sql = "Select Max(table_order) as max_table_order from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ?"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
	cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1, 0, formtype_id)
	
	Set RS = cmd.Execute
	max_table_order = RS("max_table_order")
	if isNull(max_table_order) or max_table_order="" then 
		getMaxTableOrder = 0
	else
		getMaxTableOrder = CLng(max_table_order)
	end if
	RS.Close
	'cmd.Parameters.Delete "pFormgroupID"
	'cmd.Parameters.Delete "pFormTypeID"
End Function


sub CreateListView_CA(ByRef RS, ByRef cmd, formgroup_id, max_table_order)
	'get max table order for looping

	XML_OUTPUT = ""
	running_total_width=0
	isBaseTable = false
	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select db_vw_formitems_compact.* from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ? and table_order = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1,0, FORM_LIST)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 5, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		'on error resume next 
		'loop through recordset and get elements
		tableHeaderDone = false
		
		if not (RS.BOF and RS.EOF) then
		  Do WHile Not RS.EOF
			if isBaseTable then
				baseTableID = RS("TABLE_ID")
			end if
			
			currentTable = RS("TABLE_NAME")
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			if lookup_display_column <> "" then
				column_name = lookup_display_column
				column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				'the column_display for lookups should be the normal display name NOT the lookup_display_name
				column_display_name = RS("DISPLAY_NAME")
				'column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				lookup_data_type = RS("lookup_display_datatype")
				lookup_tablename = RS("lookup_table_name")
				temp = split(lookup_tablename, ".", -1)
				lookup_tablename_short = temp(1)
				table_name = lookup_tablename_short
				full_table_name = lookup_tablename
				testDataType = lookup_data_type
				if  ((lookup_data_type = "CLOB") or (lookup_data_type = "LOB") or (lookup_data_type = "LONG") or (lookup_data_type = "RAW") or (lookup_data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if	
				
				if HasParentRelationship(cmd,RS("COLUMN_ID"),baseTableID) then
					column_name =  "a_" & column_name
				end if
			else
				
				temp = split(currentTable, ".", -1)
				table_name_short = temp(1)
				table_name = table_name_short
				full_table_name = currentTable
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				data_type = RS("DATA_TYPE")
				testDataType = data_type
				if  ((data_type = "CLOB") or (data_type = "LOB") or (data_type = "LONG") or (data_type = "RAW") or (data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if
			end if
			height_temp = RS("HEIGHT")
			
			if not (height_temp = "-1")   then
				var_height = height_temp
			else
				var_height = ""
			end if
			width_temp = RS("WIDTH")
			if not (width_temp = "-1")   then
				var_width = width_temp
			else
				var_width = "100"
			end if
			running_total_width = running_total_width + Clng(var_width)
			baseId = RS("base_column_name")
			if fieldisLob = true then
				dbname2 = "dbname=biosar_browser"
				formgroup2= "&formgroup=" & formgroup_id
				table_name2 = "&table_name=" & currentTable
				field_name2 = "&field_name=" & column_name
				mime_type2 = "&mime_type=" & lcase(getmimetype(RS("COLUMN_ID")))
				base_column2= "&base_column=" & baseId
				lob_display2 = "&lob_display=" & RS("display_option")
				unique_id2 = "&unique_id=" & "#" & baseid & "#"
				get_data2 = "&get_data=true"
				fullGetLOBString = dbname2 & formgroup2 & table_name2 & field_name2 & mime_type2 & base_column2 & lob_display2  & unique_id2 & get_data2
			end if
			if isBaseTable = true then 'basetable
				'create xml header and javascript button functions
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""1"" CLASS=""main_table"" REPEAT_HEADER=""true"">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					
					XML_OUTPUT = XML_OUTPUT & "<TABLE_ELEMENT  NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""parent_table""  COLUMNS="""" VALUE_CLASS=""parent_table_cell_value""  HEADER_CLASS=""parent_table_cell_header"" WIDTH=""#TABLE_WIDTH#""  HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value)& """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					tableHeaderDone = true
				end if
				
					select Case UCase(RS("display_type_name"))
			
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetParentChemistryString(RS, column_name, full_table_name, var_width, var_height, 1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getParentMW (var_width, var_height,"MW", "",1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getParentFormula (var_width, var_height,"MF", "",1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getparentColumn(RS, column_display_name,"", table_name, column_name, baseid, var_width, var_height,testDataType,fullGetLOBString,1)

					end select
					
			else 'is Subtable
				RS_Name =  replace(Replace(RS("TABLE_NAME") & "_RS", ".", "_"),"$","_D_")
				if tableHeaderDone = false then
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT  NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""child_table""  COLUMNS=""""  VALUE_CLASS=""child_table_cell_value"" HEADER_CLASS=""child_table_cell_header"" WIDTH=""#TABLE_WIDTH#""  HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & """  HEADER_TIP=""" & protectXMLReserved(RS("TABLE_DESCRIPTION").value) & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<SUB_RS NAME=""" & RS_Name & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetChildChemistryString(RS, rs_name,column_name, full_table_name, var_width, var_height, 1,fullGetLOBString, baseid)
							
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getChildMW(rs_name, var_width, var_height,"MW", "", 1)
							
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getChildFormula (rs_name,var_width, var_height,"MF", "", 1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getChildColumn(RS,RS_NAME, column_display_name, "", table_name, column_name, baseid, var_width, var_height,testDataType, fullGetLOBString,1)
					end select
			end if
			RS.MoveNext
			
			loop
		
		
			if isBaseTable = true then
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				XML_OUTPUT = XML_OUTPUT &  "</SUB_RS>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
			tableHeaderDone = false
			RS.Close
			'replace TABLE_WIDTH with running total for this table and then clear the variable
			XML_OUTPUT = Replace(XML_OUTPUT, "#TABLE_WIDTH#", running_total_width)
			running_total_width=0
		else
			if isBaseTable=true then
				XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""1"" CLASS=""main_table"" REPEAT_HEADER=""true"">"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
		end if
	next
	'end display and add asp variables
	XML_OUTPUT = XML_OUTPUT &  "</DISPLAY>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""sortURL"" CALL=""getSortURL()"" TYPE=""Function""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""Appkey"" TYPE=""Application""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	XML_OUTPUT = XML_OUTPUT &  "</ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DOCUMENT>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	
	Call InsertIntoXMLTemplates(cmd, formgroup_id, LIST_COLUMN_ACROSS, "LIST_COLUMN_ACROSS",XML_OUTPUT)
End Sub

sub CreateListView_CD(ByRef RS, ByRef cmd, formgroup_id,max_table_order)
	
	XML_OUTPUT = ""
	
	isBaseTable = false
	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select db_vw_formitems_compact.* from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ? and table_order = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1,0, FORM_LIST)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 5, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		'on error resume next 
		'loop through recordset and get elements
		tableHeaderDone = false
		if not (RS.BOF and RS.EOF) then
		  Do WHile Not RS.EOF
			if isBaseTable then
				baseTableID = RS("TABLE_ID")
			end if
			currentTable = RS("TABLE_NAME")
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			if lookup_display_column <> "" then
				column_name = lookup_display_column
				column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				'the column_display for lookups should be the normal display name NOT the lookup_display_name
				column_display_name = RS("DISPLAY_NAME")
				'column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				lookup_data_type = RS("lookup_display_datatype")
				lookup_tablename = RS("lookup_table_name")
				temp = split(lookup_tablename, ".", -1)
				lookup_tablename_short = temp(1)
				table_name = lookup_tablename_short
				full_table_name = lookup_tablename
				testDataType = lookup_data_type
				if  ((lookup_data_type = "CLOB") or (lookup_data_type = "LOB") or (lookup_data_type = "LONG") or (lookup_data_type = "RAW") or (lookup_data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if	
				if HasParentRelationship(cmd,RS("COLUMN_ID"),baseTableID) then
					column_name =  "a_" & column_name
				end if
			else
				
				temp = split(currentTable, ".", -1)
				full_table_name = currentTable
				table_name_short = temp(1)
				table_name = table_name_short
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				data_type = RS("DATA_TYPE")
				testDataType = data_type
				if  ((data_type = "CLOB") or (data_type = "LOB") or (data_type = "LONG") or (data_type = "RAW") or (data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if
			end if
			height_temp = RS("HEIGHT")
			
			if not (height_temp = "-1")   then
				var_height = height_temp
			else
				var_height = ""
			end if
			width_temp = RS("WIDTH")
			if not (width_temp = "-1")   then
				var_width = width_temp
			else
				var_width = "100"
			end if
			running_total_width = running_total_width + Clng(var_width)
			baseId = RS("base_column_name")
			if fieldisLob = true then
				dbname2 = "dbname=biosar_browser"
				formgroup2= "&formgroup=" & formgroup_id
				table_name2 = "&table_name=" & currentTable
				field_name2 = "&field_name=" & column_name
				mime_type2 = "&mime_type=" & lcase(getmimetype(RS("COLUMN_ID")))
				base_column2= "&base_column=" & baseId
				lob_display2 = "&lob_display=" & RS("display_option")
				unique_id2 = "&unique_id=" & "#" & baseid & "#"
				get_data2 = "&get_data=true"
				fullGetLOBString = dbname2 & formgroup2 & table_name2 & field_name2 & mime_type2 & base_column2 & lob_display2  & unique_id2 & get_data2
			end if
			if isBaseTable = true then 'basetable
				'create xml header and javascript button functions
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""1"" CLASS=""main_table"" REPEAT_HEADER=""true"">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT & "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""parent_table""  COLUMNS=""1"" VALUE_CLASS=""parent_table_cell_value""  HEADER_CLASS=""parent_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value)& """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
								XML_OUTPUT = XML_OUTPUT & GetParentChemistryString(RS, column_name, full_table_name, var_width, var_height, 1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getParentMW (var_width, var_height,"", "MW",1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getParentFormula (var_width, var_height,"", "MF",1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getparentColumn(RS, "",column_display_name, table_name, column_name, baseid, var_width, var_height,testDataType,fullGetLOBString,1)
						
					end select
			else 'is Subtable
				RS_Name =  replace(Replace(RS("TABLE_NAME") & "_RS", ".", "_"),"$","_D_")
				if tableHeaderDone = false then
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""child_table""  COLUMNS=""1""  VALUE_CLASS=""child_table_cell_value"" HEADER_CLASS=""child_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & """  HEADER_TIP=""" & protectXMLReserved(RS("TABLE_DESCRIPTION").value) & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<SUB_RS NAME=""" & RS_Name & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetChildChemistryString(RS,  rs_name,column_name, full_table_name, var_width, var_height, 1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getChildMW(rs_name, var_width, var_height,"", "MW", 1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getChildFormula (rs_name,var_width, var_height,"", "MF", 1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getChildColumn(RS,RS_NAME, "", column_display_name, table_name, column_name, baseid, var_width, var_height,testDataType,  fullGetLOBString, 1)
					end select

			end if
			RS.MoveNext
			loop
		
			if isBaseTable = true then
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				XML_OUTPUT = XML_OUTPUT &  "</SUB_RS>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
			tableHeaderDone = false
			RS.Close
		end if
	next
	'end display and add asp variables
	XML_OUTPUT = XML_OUTPUT &  "</DISPLAY>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""sortURL"" CALL=""getSortURL()"" TYPE=""Function""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""Appkey"" TYPE=""Application""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	XML_OUTPUT = XML_OUTPUT &  "</ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DOCUMENT>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	
	Call InsertIntoXMLTemplates(cmd, formgroup_id, LIST_COLUMN_DOWN, "LIST_COLUMN_DOWN",XML_OUTPUT)
End Sub

sub CreateExcelTemplate(ByRef RS, ByRef cmd, formgroup_id,max_table_order)
	

	
	XML_OUTPUT = ""
	
	isBaseTable = false
	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select db_vw_formitems_compact.* from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ? and table_order = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1,0, FORM_LIST)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 5, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		'on error resume next 
		'loop through recordset and get elements
		tableHeaderDone = false
		if not (RS.BOF and RS.EOF) then
		  Do WHile Not RS.EOF
			currentTable = RS("TABLE_NAME")
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			if lookup_display_column <> "" then
				column_name = lookup_display_column
				column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				'the column_display for lookups should be the normal display name NOT the lookup_display_name
				column_display_name = RS("DISPLAY_NAME")
				'column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				lookup_data_type = RS("lookup_display_datatype")
				lookup_tablename = RS("lookup_table_name")
				
				temp = split(lookup_tablename, ".", -1)
				lookup_tablename_short = temp(1)
				table_name = lookup_tablename_short
				full_table_name=lookup_tablename
				testDataType = lookup_data_type
				if  ((lookup_data_type = "CLOB") or (lookup_data_type = "LOB") or (lookup_data_type = "LONG") or (lookup_data_type = "RAW") or (lookup_data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if	
				if HasParentRelationship(cmd,RS("COLUMN_ID"),baseTableID) then
					column_name =  "a_" & column_name
				end if
			else
				
				temp = split(currentTable, ".", -1)
				full_table_name=currentTable
				table_name_short = temp(1)
				table_name = table_name_short
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				data_type = RS("DATA_TYPE")
				testDataType = data_type
				if  ((data_type = "CLOB") or (data_type = "LOB") or (data_type = "LONG") or (data_type = "RAW") or (data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if
			end if
			height_temp = RS("HEIGHT")
			
			if not (height_temp = "-1")   then
				var_height = height_temp
			else
				var_height = ""
			end if
			width_temp = RS("WIDTH")
			if not (width_temp = "-1")   then
				var_width = width_temp
			else
				var_width = "100"
			end if
			running_total_width = running_total_width + Clng(var_width)
			baseId = RS("base_column_name")
			if fieldisLob = true then
				dbname2 = "dbname=biosar_browser"
				formgroup2= "&formgroup=" & formgroup_id
				table_name2 = "&table_name=" & currentTable
				field_name2 = "&field_name=" & column_name
				mime_type2 = "&mime_type=" & lcase(getmimetype(RS("COLUMN_ID")))
				base_column2= "&base_column=" & baseId
				lob_display2 = "&lob_display=" & RS("display_option")
				unique_id2 = "&unique_id=" & "#" & baseid & "#"
				get_data2 = "&get_data=true"
				fullGetLOBString = dbname2 & formgroup2 & table_name2 & field_name2 & mime_type2 & base_column2 & lob_display2  & unique_id2 & get_data2
			end if
			if isBaseTable = true then 'basetable
				'create xml header and javascript button functions
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""1"" CLASS=""main_table"" REPEAT_HEADER=""true"">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					'XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
					'XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT & "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""parent_table""  COLUMNS="""" VALUE_CLASS=""parent_table_cell_value""  HEADER_CLASS=""parent_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value)& """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetParentChemistryString(RS, column_name, full_table_name, var_width, var_height,1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getParentMW (var_width, var_height,"MW", "",1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getParentFormula (var_width, var_height,"MF", "",1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getparentColumn(RS, column_display_name,"", table_name, column_name, baseid, var_width, var_height,testDataType, 1,fullGetLOBString, baseid)
		
					end select
			else 'is Subtable
				RS_Name =  replace(Replace(RS("TABLE_NAME") & "_RS", ".", "_"),"$","_D_")
				if tableHeaderDone = false then
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""child_table""  COLUMNS=""""  VALUE_CLASS=""child_table_cell_value"" HEADER_CLASS=""child_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & """  HEADER_TIP=""" & protectXMLReserved(RS("TABLE_DESCRIPTION").value) & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<SUB_RS NAME=""" & RS_Name & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetChildChemistryString(RS, rs_name, column_name, full_table_name, var_width, var_height,fullGetLOBString,baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getChildMW(rs_name, var_width, var_height,"MW", "", 1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getChildFormula (rs_name,var_width, var_height,"MF", "", 1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getChildColumn(RS,RS_NAME, column_display_name, "", table_name, column_name, baseid, var_width, var_height,testDataType, fullGetLOBString,1)
	
					end select

			end if
			RS.MoveNext
			loop
		
			if isBaseTable = true then
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				XML_OUTPUT = XML_OUTPUT &  "</SUB_RS>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
			tableHeaderDone = false
			RS.Close
		end if
	next
	'end display and add asp variables
	XML_OUTPUT = XML_OUTPUT &  "</DISPLAY>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""Appkey"" TYPE=""Application""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	XML_OUTPUT = XML_OUTPUT &  "</ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DOCUMENT>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	

	
	Call InsertIntoXMLTemplates(cmd, formgroup_id, EXCEL_TEMPLATE, "EXCEL_TEMPLATE",XML_OUTPUT)
End Sub

sub CreateDetailView(ByRef RS, ByRef cmd, formgroup_id,max_table_order)
	
	on error resume next

	XML_OUTPUT = ""
	bSubExists =false
	isBaseTable = false
	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select db_vw_formitems_compact.* from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ? and table_order = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1,0, FORM_DETAIL)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 5, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		'on error resume next 
		'loop through recordset and get elements
		tableHeaderDone = false
		subtableHeaderDone = false
		if not (RS.BOF and RS.EOF) then
		  Do WHile Not RS.EOF
			if isBaseTable then
				baseTableID = RS("TABLE_ID")
			end if
			currentTable = RS("TABLE_NAME")
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			if lookup_display_column <> "" then
				column_name = lookup_display_column
				column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				'the column_display for lookups should be the normal display name NOT the lookup_display_name
				column_display_name = RS("DISPLAY_NAME")
				'column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				lookup_data_type = RS("lookup_display_datatype")
				lookup_tablename = RS("lookup_table_name")
				temp = split(lookup_tablename, ".", -1)
				lookup_tablename_short = temp(1)
				table_name = lookup_tablename_short
				full_table_name=lookup_tablename
				testDataType = lookup_data_type
				if  ((lookup_data_type = "CLOB") or (lookup_data_type = "LOB") or (lookup_data_type = "LONG") or (lookup_data_type = "RAW") or (lookup_data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if	
				if HasParentRelationship(cmd,RS("COLUMN_ID"),baseTableID) then
					column_name =  "a_" & column_name
				end if
			else
				
				temp = split(currentTable, ".", -1)
				full_table_name=currentTable
				table_name_short = temp(1)
				table_name = table_name_short
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				data_type = RS("DATA_TYPE")
				testDataType = data_type
				if  ((data_type = "CLOB") or (data_type = "LOB") or (data_type = "LONG") or (data_type = "RAW") or (data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if
			end if
			height_temp = RS("HEIGHT")
			
			if not (height_temp = "-1")   then
				var_height = height_temp
			else
				var_height = ""
			end if
			width_temp = RS("WIDTH")
			if not (width_temp = "-1")   then
				var_width = width_temp
			else
				var_width = "100"
			end if
		
			running_total_width = running_total_width + Clng(var_width)
			baseId = RS("base_column_name")
			if fieldisLob = true then
				dbname2 = "dbname=biosar_browser"
				formgroup2= "&formgroup=" & formgroup_id
				table_name2 = "&table_name=" & currentTable
				field_name2 = "&field_name=" & column_name
				mime_type2 = "&mime_type=" & lcase(getmimetype(RS("COLUMN_ID")))
				base_column2= "&base_column=" & baseId
				lob_display2 = "&lob_display=" & RS("display_option")
				unique_id2 = "&unique_id=" & "#" & baseid & "#"
				get_data2 = "&get_data=true"
				fullGetLOBString = dbname2 & formgroup2 & table_name2 & field_name2 & mime_type2 & base_column2 & lob_display2  & unique_id2 & get_data2
			end if
			if isBaseTable = true then 'basetable
				'create xml header and javascript button functions
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""0"" CLASS=""main_table_detail"" REPEAT_HEADER=""true"">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					'XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
					'XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					XML_OUTPUT = XML_OUTPUT & "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""parent_table_detail""  COLUMNS=""1"" VALUE_CLASS=""parent_table_cell_value""  HEADER_CLASS=""parent_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value)& """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine

					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
								XML_OUTPUT = XML_OUTPUT & GetParentChemistryString(RS, column_name, full_table_name, var_width, var_height, 1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getParentMW (var_width, var_height,"", "MW",1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getParentFormula (var_width, var_height,"", "MF",1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getparentColumn(RS,"" ,column_display_name, table_name, column_name, baseid, var_width, var_height,testDataType,fullGetLOBString,1)
					end select
			else 'is Subtable
				bSubExists = true
				RS_Name =  replace(Replace(RS("TABLE_NAME") & "_RS", ".", "_"),"$","_D_")
				if mainsubtableHeaderDone = false then
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT SHOW=""1"" BORDER=""0"" COLUMNS=""1"" HEADER_CLASS="""" HEADER_NAME="""" CLASS="""" REPEAT_HEADER=""false"">"
					mainsubtableHeaderDone=true
				end if
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""child_table_detail""  COLUMNS=""""  VALUE_CLASS=""child_table_cell_value"" COLSPAN=""1"" HEADER_CLASS=""child_table_cell_header"" HEADER_NAME=""" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & """  HEADER_TIP=""" & protectXMLReserved(RS("TABLE_DESCRIPTION").value) & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<CAPTION_ELEMENT SHOW=""1""  CLASS=""child_table_detail"" COLSPAN=""2"">" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & "</CAPTION_ELEMENT>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<SUB_RS NAME=""" & RS_Name & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetChildChemistryString(RS, rs_name, column_name, full_table_name, var_width, var_height,1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getChildMW(rs_name, var_width, var_height,"MW", "", 1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getChildFormula (rs_name,var_width, var_height,"MF", "", 1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getChildColumn(RS,RS_NAME, column_display_name, "", table_name, column_name, baseid, var_width, var_height,testDataType, fullGetLOBString, 1)
					end select

			end if
			RS.MoveNext
			loop
			if isBaseTable = true then
				'XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				'XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				XML_OUTPUT = XML_OUTPUT &  "</SUB_RS>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
		tableHeaderDone = false
		RS.Close
		end if
	next
	
	'end display and add asp variables
	XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	if bSubExists = true then
		XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
	end if
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DISPLAY>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""sortURL"" CALL=""getSortURL()"" TYPE=""Function""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""Appkey"" TYPE=""Application""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	XML_OUTPUT = XML_OUTPUT &  "</ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DOCUMENT>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	 
	Call InsertIntoXMLTemplates(cmd, formgroup_id, DETAIL, "DETAIL",XML_OUTPUT)
End Sub
sub CreateDetailView_ChildOnly(ByRef RS, ByRef cmd, formgroup_id,max_table_order)
	
	on error resume next
	
	
	
	XML_OUTPUT = ""
	
	isBaseTable = false
	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select db_vw_formitems_compact.* from biosardb.db_vw_formitems_compact where formgroup_id = ? and formtype_id= ? and table_order = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1,0, FORM_DETAIL)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 5, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		'on error resume next 
		'loop through recordset and get elements
		tableHeaderDone = false
		subtableHeaderDone = false
		if not (RS.BOF and RS.EOF) then
		  Do WHile Not RS.EOF
			if isBaseTable then
				baseTableID = RS("TABLE_ID")
			end if
			currentTable = RS("TABLE_NAME")
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			if lookup_display_column <> "" then
				column_name = lookup_display_column
				column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				'the column_display for lookups should be the normal display name NOT the lookup_display_name
				column_display_name = RS("DISPLAY_NAME")
				'column_display_column = RS("LOOKUP_DISPLAY_COLUMN")
				lookup_data_type = RS("lookup_display_datatype")
				lookup_tablename = RS("lookup_table_name")
				full_table_name=lookup_tablename
				temp = split(lookup_tablename, ".", -1)
				lookup_tablename_short = temp(1)
				table_name = lookup_tablename_short
				testDataType = lookup_data_type
				if  ((lookup_data_type = "CLOB") or (lookup_data_type = "LOB") or (lookup_data_type = "LONG") or (lookup_data_type = "RAW") or (lookup_data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if	
				if HasParentRelationship(cmd,RS("COLUMN_ID"),baseTableID) then
					column_name =  "a_" & column_name
				end if
			else
				
				temp = split(currentTable, ".", -1)
				full_table_name=currentTable
				table_name_short = temp(1)
				table_name = table_name_short
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				data_type = RS("DATA_TYPE")
				testDataType = data_type
				if  ((data_type = "CLOB") or (data_type = "LOB") or (data_type = "LONG") or (data_type = "RAW") or (data_type = "BLOB")) then
					fieldisLob = true
				else
					fieldisLob = false
				end if
			end if
			height_temp = RS("HEIGHT")
			
			if not (height_temp = "-1")   then
				var_height = height_temp
			else
				var_height = ""
			end if
			width_temp = RS("WIDTH")
			if not (width_temp = "-1")   then
				var_width = width_temp
			else
				var_width = "100"
			end if
			running_total_width = running_total_width + Clng(var_width)
			baseId = RS("base_column_name")
			if fieldisLob = true then
				dbname2 = "dbname=biosar_browser"
				formgroup2= "&formgroup=" & formgroup_id
				table_name2 = "&table_name=" & currentTable
				field_name2 = "&field_name=" & column_name
				mime_type2 = "&mime_type=" & lcase(getmimetype(RS("COLUMN_ID")))
				base_column2= "&base_column=" & baseId
				lob_display2 = "&lob_display=" & RS("display_option")
				unique_id2 = "&unique_id=" & "#" & baseid & "#"
				get_data2 = "&get_data=true"
				fullGetLOBString = dbname2 & formgroup2 & table_name2 & field_name2 & mime_type2 & base_column2 & lob_display2  & unique_id2 & get_data2
			end if
			if isBaseTable = true then 'basetable
				'create xml header and javascript button functions
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<?xml version=""1.0""?>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &   "<DOCUMENT xmlns:s=""uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882"" xmlns:dt=""uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"" xmlns:rs=""urn:schemas-microsoft-com:rowset"" xmlns:z=""#RowsetSchema""><DISPLAY BORDER=""0"" CLASS=""main_table_detail"" REPEAT_HEADER=""false"">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					'XML_OUTPUT = XML_OUTPUT  & "<FIELD VALUE_COLUMNS=""INDEXCOUNTER"" VALUE_CLASS=""main_table_c1_value""  NAME_CLASS=""main_table_c1_name"" COLSPAN=""1"" SHOW=""1"" HEADER_NAME="" "" WIDTH=""100"" POST_PROCESS=""true"">&lt;!getJscriptButtons(""#INDEXCOUNTER#"")!&gt;</FIELD>"
					'XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					XML_OUTPUT = XML_OUTPUT & "<TABLE_ELEMENT NAME=""NO_HIDE"" SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""parent_table_detail""  COLUMNS=""1"" VALUE_CLASS=""parent_table_cell_value""  HEADER_CLASS=""parent_table_cell_header"" HEADER_NAME="""">"
					XML_OUTPUT = XML_OUTPUT & vbNewLine

					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
					case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetParentChemistryString(RS, column_name, full_table_name, var_width, var_height, 0,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getParentMW (var_width, var_height,"MW", "",0)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getParentFormula (var_width, var_height,"MF", "",0)
						case Else
							XML_OUTPUT = XML_OUTPUT & getparentColumn(RS, column_display_name,"", table_name, column_name, baseid, var_width, var_height,testDataType,fullGetLOBString,0)
						
					end select
			else 'is Subtable
			
				RS_Name =  replace(Replace(RS("TABLE_NAME") & "_RS", ".", "_"),"$","_D_")
				if mainsubtableHeaderDone = false then
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT NAME=""NO_HIDE"" SHOW=""1"" BORDER=""0"" COLUMNS=""1"" HEADER_CLASS="""" HEADER_NAME="""" CLASS="""" REPEAT_HEADER=""false"">"
					mainsubtableHeaderDone=true
				end if
				if tableHeaderDone = false then
					
					XML_OUTPUT = XML_OUTPUT &  "<TABLE_ELEMENT NAME=""" & UCase(RS("TABLE_NAME")) & """ SHOW=""1"" BORDER=""0"" REPEAT_HEADER=""false"" CLASS=""child_table_detail""  COLUMNS=""""  VALUE_CLASS=""child_table_cell_value"" COLSPAN=""1"" HEADER_CLASS=""child_table_cell_header"" HEADER_NAME=""""  HEADER_TIP=""" & protectXMLReserved(RS("TABLE_DESCRIPTION")) & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<CAPTION_ELEMENT CLASS=""child_cell_header"" SHOW=""1"" COLSPAN=""2"">" & protectXMLReserved(RS("TABLE_DISPLAY_NAME").value) & "</CAPTION_ELEMENT>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					XML_OUTPUT = XML_OUTPUT &  "<SUB_RS NAME=""" & RS_Name & """>"
					XML_OUTPUT = XML_OUTPUT & vbNewLine
					
					tableHeaderDone = true
				end if
					select Case UCase(RS("display_type_name"))
						case "STRUCTURE"
							XML_OUTPUT = XML_OUTPUT & GetChildChemistryString(RS, rs_name, column_name, full_table_name, var_width, var_height,1,fullGetLOBString, baseid)
						case "MOLWEIGHT"
							XML_OUTPUT = XML_OUTPUT &  getChildMW(rs_name, var_width, var_height,"MW", "", 1)
						case "FORMULA"
							XML_OUTPUT = XML_OUTPUT &  getChildFormula (rs_name,var_width, var_height,"MF", "", 1)
						case Else
							XML_OUTPUT = XML_OUTPUT & getChildColumn(RS,RS_NAME,column_display_name, "", table_name, column_name, baseid, var_width, var_height,testDataType,fullGetLOBString,1)
					end select

			end if
			RS.MoveNext
			loop
			if isBaseTable = true then
			'XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
			'XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				XML_OUTPUT = XML_OUTPUT &  "</SUB_RS>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
				
				
			end if
		
		
		tableHeaderDone = false
		RS.Close
		end if
	next
	
	'end display and add asp variables
	XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "</TABLE_ELEMENT >"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DISPLAY>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""sortURL"" CALL=""getSortURL()"" TYPE=""Function""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT & "<VARIABLE NAME=""Appkey"" TYPE=""Application""/>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	XML_OUTPUT = XML_OUTPUT &  "</ASP_VARIABLES>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	XML_OUTPUT = XML_OUTPUT &  "</DOCUMENT>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine

	 
	Call InsertIntoXMLTemplates(cmd, formgroup_id, DETAIL_CHILD, "DETAIL_CHILD",XML_OUTPUT)
End Sub

Sub InsertIntoXMLTemplates(byRef cmd, formgroup_id, template_id, template_desc, template_def)
	'on error resume next
	sql = "INSERT INTO biosardb.DB_XML_TEMPL_DEF (formgroup_id, template_id, template_desc)Values(?,?,'" & template_desc &"')"
	cmd.CommandText = sql
	DeleteParameters(cmd)
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
	cmd.Parameters.Append cmd.CreateParameter("pTemplateID", 5, 1,0,template_id )
	cmd.Execute
	
	
	sql = "SELECT template_def from biosardb.DB_XML_TEMPL_DEF where formgroup_id = ? and template_id = ?"
	cmd.CommandText = sql
	Set RS = Server.CreateObject("adodb.recordset")
	'RS.CursorLocation = adUseClient
	RS.LockType= 3
	RS.CursorType = 3 
	RS.Open cmd
	RS("template_def") = template_def
	RS.Update
	RS.Close
	'cmd.Parameters.Delete "pFormgroupID"
	'cmd.Parameters.Delete "pTemplateID"
End Sub

Sub InsertIntoCSS(cmd, formgroup_id, css_def)


End sub

Sub DeleteXMLTemplates(cmd,formgroup_id)

	numTemplateTypes = 4
	for i = 1 to numTemplateTypes
		sql = "DELETE FROM biosardb.DB_XML_TEMPL_DEF WHERE formgroup_id = ? AND  template_id = ?"
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pTemplateID", 5, 1,0,CLng(i) )
		'on error resume next
		cmd.Execute , ,adExecuteNoRecords
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pTemplateID"
	next
End sub

function getmimetype(colID)
	if not isObject(uConn) then
		Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
	end if
	Set RS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  uConn
	cmd.CommandType = adCmdText
	sql = "select mime_type from  BIOSARDB.db_http_content_type where content_type_id=(select content_type_id from db_column where column_id=?)"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pColID", 5, 1, 0, colID) 
	RS.Open cmd
	if not (RS.BOF and RS.EOF) then
		mime_type = RS("mime_type")
	else
		mime_type=""
	end if
	RS.Close
	getmimetype = mime_type
end function 

Function GetParentChemistryString(byRef RS, column_name,table_name, var_width, var_height,bShow,fullLobString,baseid)
		
		if RS("LOOKUP_DISPLAY_COLUMN") <> "" then
			base_column_name = RS("lookup_column_name")
			value_column=RS("column_name")
			column_id = RS("LOOKUP_DISPLAY_COLUMN__ID").value
			datatype = UCase(RS("LOOKUP_DISPLAY_DATATYPE").value)
		else
			value_column=baseid
			base_column_name=baseid
			column_id = RS("COLUMN_ID").value
			datatype = UCase(RS("DATA_TYPE").value)
		end if
		mimetype = lcase(getmimetype(column_id))
		
		if RS("display_option")="GIF" then
			XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & "STRUCTURE" & ",INDEXCOUNTER"" VALUE_CLASS=""parent_cell_value""   HEADER_NAME="" "" SORT_COLUMN="" "" DISPLAY_NAME="""" NAME_CLASS=""parent_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """  WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!getSizedGifDisplay(""#STRUCTURE#"",""#INDEXCOUNTER#"",""" & var_width & """,""" & var_height & """,""" & table_name & """,""" & column_name & """)!&gt;</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		else
			
			select case mimetype
				case "text/xml"
					file_ext=".xml"
				case "chemical/x-cdx"
					select case datatype
						case "CLOB"
							mimetype = mimetype & ";base64"
							file_ext=""
						case "BLOB"
							file_ext=".cdx"
					end select
				case else
			end select
			'add both types of structure display in-line and not in-line. The templates are adjusted at display time depening on the number of structures displayed in list view.
			data_location="recordset"
			bShow = 0
				XML_OUTPUT = XML_OUTPUT & "<FIELD VALUE_COLUMNS=""STRUCTURE""  VALUE_CLASS=""parent_structure_cell_value"" HEADER_NAME="" "" SORT_COLUMN=""" & column_name & """ DISPLAY_NAME="""" NAME_CLASS=""parent_structure_cell_header"" COLSPAN=""1"" SHOW=""" & bshow & """   HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """ IS_STRUCTURE=""1"" BORDER=""0""  DATA_LOCATION=""" & data_location & """ FILE_EXT=""" & file_ext & """ MIMETYPE=""" & mimetype & """ FORM_NAME=""cows_input_form"">#" & "STRUCTURE" & "#</FIELD>"
			data_location="table"
			bShow = 0
				XML_OUTPUT = XML_OUTPUT & "<FIELD VALUE_COLUMNS=""" & value_column & """ COLUMN_NAME=""" & column_name & """ BASE_COLUMN_NAME=""" & base_column_name & """  TABLE_NAME=""" & table_name & """ VALUE_CLASS=""parent_structure_cell_value"" HEADER_NAME="" "" SORT_COLUMN=""" & column_name & """ DISPLAY_NAME="""" NAME_CLASS=""parent_structure_cell_header"" COLSPAN=""1"" SHOW=""" & bshow & """   HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """ IS_STRUCTURE=""1"" BORDER=""0""  DATA_LOCATION=""" & data_location & """ MIMETYPE=""" & mimetype & """ FORM_NAME=""cows_input_form"">#" & value_column & "#</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		end if
		GetParentChemistryString = XML_OUTPUT
End Function

Function GetChildChemistryString(byRef RS, RS_Name,column_name,table_name, var_width, var_height, bShow,fullLOBString,baseid)
	
		if RS("LOOKUP_DISPLAY_COLUMN") <> "" then
			base_column_name = RS("lookup_column_name")
			value_column=RS("column_name")
			column_id = RS("LOOKUP_DISPLAY_COLUMN__ID").value
			datatype = UCase(RS("LOOKUP_DISPLAY_DATATYPE").value)
		else
			value_column=baseid
			base_column_name=baseid
			column_id = RS("COLUMN_ID").value
			datatype = UCase(RS("DATA_TYPE").value)
		end if
		mimetype = lcase(getmimetype(column_id))
	
		if RS("display_option")="GIF" then
			XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & "STRUCTURE" & ",INDEXCOUNTER"" VALUE_CLASS=""child_structure_cell_value""   HEADER_NAME="" "" SORT_COLUMN="" "" DISPLAY_NAME="""" NAME_CLASS=""child_cell_header"" SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """  WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!getSizedGifDisplay(""#STRUCTURE#"",""#INDEXCOUNTER#"",""" & var_width & """,""" & var_height & """,""" & table_name & """,""" & column_name & """)!&gt;</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		else
			
			select case mimetype
				case "text/xml"
					file_ext=".xml"
				case "chemical/x-cdx"
					select case datatype
						case "CLOB"
							mimetype = mimetype & ";base64"
							file_ext=""
						case "BLOB"
							file_ext=".cdx"
					end select
				case else
			end select
			'add both types of structure display in-line and not in-line. The templates are adjusted at display time depening on the number of structures displayed in list view.
			data_location="recordset"
			bShow = 1
			XML_OUTPUT = XML_OUTPUT & "<FIELD VALUE_COLUMNS=""STRUCTURE"" VALUE_CLASS=""child_structure_cell_value"" HEADER_NAME="" "" SORT_COLUMN=""" & column_name & """ DISPLAY_NAME="""" NAME_CLASS=""child_cell_header"" SUB_RS_NAME=""" & RS_Name & """  COLSPAN=""1"" SHOW=""" & bshow & """   HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """ IS_STRUCTURE=""1"" DATA_LOCATION=""" & data_location & """ FILE_EXT=""" & file_ext & """ MIMETYPE=""" & mimetype & """  BORDER=""0"" DATA_TYPE=""base64"" FORM_NAME=""cows_input_form"">#" & "STRUCTURE" & "#</FIELD>"
			data_location="table"
			bShow = 0
			XML_OUTPUT = XML_OUTPUT & "<FIELD VALUE_COLUMNS=""" & value_column & """ COLUMN_NAME=""" & column_name & """ BASE_COLUMN_NAME=""" & base_column_name & """  TABLE_NAME=""" & table_name & """ VALUE_CLASS=""child_structure_cell_value"" HEADER_NAME="" "" SORT_COLUMN=""" & column_name & """ DISPLAY_NAME="""" NAME_CLASS=""child_cell_header"" SUB_RS_NAME=""" & RS_Name & """  COLSPAN=""1"" SHOW=""" & bshow & """   HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """ IS_STRUCTURE=""1"" DATA_LOCATION=""" & data_location & """ MIMETYPE=""" & mimetype & """  BORDER=""0""  FORM_NAME=""cows_input_form"">#" & value_column & "#</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		end if
		
	
		GetChildChemistryString = XML_OUTPUT
End Function

Function getparentColumn(byRef RS, header_name, display_name, table_name, column_name, baseid, var_width, var_height,testDataType,fullgetLOBString, bShow)
		
		if Application("TRUNCATE_CELL_DATA")="1" then
			fontF = Application("FONT_FACTOR")
			truncLength = Int(CInt(var_width)/fontF)
		else
			truncLength = ""
		end if
		header_name=protectXMLReserved(header_name)
		display_name=protectXMLReserved(display_name)
		display_type_name = UCASE(RS("DISPLAY_TYPE_NAME").value)
		XML_OUTPUT=""
		if  Not((testDataType = "CLOB") or (testDataType = "LOB") or (testDataType = "LONG") or (testDataType = "RAW") or (testDataType = "BLOB")) then
			'support alias used in subselects
			if RS("LOOKUP_DISPLAY_COLUMN") <> "" then
				column_name = RS("COLUMN_NAME")
			end if
			if testDataType = "DATE" then
				XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""parent_cell_value"" DATA_TYPE=""date"" HEADER_CLASS=""parent_cell_header""   HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""parent_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """ MAX_LENGTH=""" & truncLength & """ WIDTH=""" & var_width & """>#" &  column_name & "#</FIELD>"
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			else
				' Implement view as hyperlink option
				'jhs hyperlink edit 07/30/2007
				'if display_type_name = "HYPERLINK" then
				'	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""parent_cell_value"" HEADER_CLASS=""parent_cell_header""   HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""parent_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """  WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!ShowAsHyperlink(""#" & column_name & "#"")!&gt;</FIELD>"
				'else
					XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""parent_cell_value"" HEADER_CLASS=""parent_cell_header""   HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""parent_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """ MAX_LENGTH=""" & truncLength & """ WIDTH=""" & var_width & """>#" &  column_name & "#</FIELD>"
				'end if
				
				XML_OUTPUT = XML_OUTPUT & vbNewLine
			end if
		else
			
			' DGB This is where we handle the BLOB image columns
			' We need to handle the case when the BLOB is null to
			' avoid putting in broken images or links.
			' This requires postprocessing so we can make decisions based on the 
			' column value.
			' comment out the old code
			'XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ASP:Appkey," & UCase(baseId) & """   VALUE_CLASS="" child_cell_value""  HEADER_NAME=""" & header_name & """ HEADER_CLASS=""child_cell_header"" SORT_COLUMN="""" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name"" COLSPAN=""1""  SHOW=""" & bshow & """ SUB_RS_NAME=""" & RS_Name & """ HEIGHT=""" & var_height & """  WIDTH=""" & var_width & """><![CDATA["
			'	
			'select case Ucase(RS("display_option"))
			'	case "IN_LINE"
			'		XML_OUTPUT = XML_OUTPUT  & "<img src=""\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString & """>"
			'	case "NEW_WINDOW"
			'		XML_OUTPUT = XML_OUTPUT  & "<a href=""\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString & """ target='_new'>image link</a>"
			'End select
			'XML_OUTPUT = XML_OUTPUT &  "]]></FIELD>"
			
			' DGB Postprocess instead using new function called ShowBlobColumn()
			dim url
			url = "\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString
			url =replace(url, "&", "&amp;")
			XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ISNULLBLOB"" VALUE_CLASS=""parent_cell_value"" HEADER_CLASS=""parent_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN="""" DISPLAY_NAME=""Is Null Blob"" NAME_CLASS=""parent_cell_name""  SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""0""  HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """>#ISNULLBLOB#</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
			XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ASP:Appkey," & UCase(baseId) & ",ISNULLBLOB""   VALUE_CLASS=""parent_cell_value""  HEADER_NAME=""" & header_name & """ HEADER_CLASS=""parent_cell_header"" SORT_COLUMN="""" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""parent_cell_name"" COLSPAN=""1""  SHOW=""" & bshow & """ SUB_RS_NAME=""" & RS_Name & """ HEIGHT=""" & var_height & """  WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!ShowBlobColumn(""" & url & """,""" & RS("display_option") & """,""#ISNULLBLOB#"")!&gt;</FIELD>"

		end if
	getparentColumn= XML_OUTPUT
End Function

Function getChildColumn(byRef RS, RS_NAME,header_name,display_name, table_name, column_name, baseid, var_width, var_height,testDataType, fullgetLOBString,bShow)
	if Application("TRUNCATE_CELL_DATA")="1" then
		fontF = Application("FONT_FACTOR")
		truncLength = Int(CInt(var_width)/fontF)
	else
		truncLength = ""
	end if
	XML_OUTPUT=""
	header_name=protectXMLReserved(header_name)
	display_name=protectXMLReserved(display_name)
	display_type_name = UCASE(RS("DISPLAY_TYPE_NAME").value)
	if  Not((testDataType = "CLOB") or (testDataType = "LOB") or (testDataType = "LONG") or (testDataType = "RAW") or (testDataType = "BLOB")) then
		'support alias used in subselects
		if RS("LOOKUP_DISPLAY_COLUMN") <> "" then
			column_name = RS("COLUMN_NAME")
		end if
		if testDataType = "DATE" then
			XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""child_cell_value"" DATA_TYPE=""date"" HEADER_CLASS=""child_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name""  SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height  & """  MAX_LENGTH=""" & truncLength & """ WIDTH=""" & var_width & """>#" & column_name & "#</FIELD>"
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		else				
			' Implement view as hyperlink option
			'jhs hyperlink edit 07/30/2007
			'if display_type_name = "HYPERLINK" then
			'	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""child_cell_value"" HEADER_CLASS=""child_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name""  SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!ShowAsHyperlink(""#" & column_name & "#"")!&gt;</FIELD>"
			'else
				XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""" & column_name & """ VALUE_CLASS=""child_cell_value"" HEADER_CLASS=""child_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN=""" & column_name & """ DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name""  SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height  & """   MAX_LENGTH=""" & truncLength & """ WIDTH=""" & var_width & """>#" & column_name & "#</FIELD>"
			'end if
			
			
			XML_OUTPUT = XML_OUTPUT & vbNewLine
		end if
	else
		' DGB This is where we handle the BLOB image columns
		' We need to handle the case when the BLOB is null to
		' avoid putting in broken images or links.
		' This requires postprocessing so we can make decisions based on the 
		' column value.
		' comment out the old code
		'XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ASP:Appkey," & baseid & """  VALUE_CLASS=""child_cell_value""  HEADER_NAME=""" & header_name & """ HEADER_CLASS=""child_cell_header"" SORT_COLUMN="""" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name"" COLSPAN=""1""  SHOW=""" & bshow & """ SUB_RS_NAME=""" & RS_Name & """ HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """><![CDATA["
		'select case Ucase(RS("display_option"))
		'	case "IN_LINE"
		'		XML_OUTPUT = XML_OUTPUT  & "<img src=""\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString & """>"
		'	case "NEW_WINDOW"
		'		XML_OUTPUT = XML_OUTPUT  & "<a href=""\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString & """ target='_new'>image link</a>"
		'End select	
		'XML_OUTPUT = XML_OUTPUT &  "]]></FIELD>"
		
		' DGB Postprocess instead using new function called ShowBlobColumn()
		dim url
		url = "\#ASP:Appkey#\show_picture.asp?" & fullgetLOBString
		url =replace(url, "&", "&amp;")
		XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ISNULLBLOB"" VALUE_CLASS=""child_cell_value"" HEADER_CLASS=""child_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN="""" DISPLAY_NAME=""Is Null Blob"" NAME_CLASS=""child_cell_name""  SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""0""  HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """>#ISNULLBLOB#</FIELD>"
		XML_OUTPUT = XML_OUTPUT & vbNewLine
		XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""ASP:Appkey," & baseid & ",ISNULLBLOB""  VALUE_CLASS=""child_cell_value""  HEADER_NAME=""" & header_name & """ HEADER_CLASS=""child_cell_header"" SORT_COLUMN="""" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_cell_name"" COLSPAN=""1""  SHOW=""" & bshow & """ SUB_RS_NAME=""" & RS_Name & """ HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!ShowBlobColumn(""" & url & """,""" & RS("display_option") & """,""#ISNULLBLOB#"")!&gt;</FIELD>"
		
	end if
	getChildColumn = XML_OUTPUT
end function

Function getParentMW (var_width, var_height,header_name, display_name, bShow)
	XML_OUTPUT=""
	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""MOLWEIGHT"" VALUE_CLASS=""parent_mw_cell_value"" HEADER_CLASS=""parent_mw_cell_header""   DISPLAY_NAME=""" & display_name & """ SORT_COLUMN=""MOLWEIGHT"" HEADER_NAME=""" & header_name & """ NAME_CLASS=""parent_mw_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """>#MOLWEIGHT#</FIELD>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	getParentMW = XML_OUTPUT 
End Function



Function getParentFormula(var_width, var_height,header_name, display_name, bShow)
	XML_OUTPUT=""
	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""FORMULA"" VALUE_CLASS=""parent_formula_cell_value"" HEADER_CLASS=""parent_formula_cell_header""   DISPLAY_NAME=""" & display_name & """ SORT_COLUMN=""FORMULA"" HEADER_NAME=""" & header_name & """ NAME_CLASS=""parent_formula_cell_name"" COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height & """ WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!GetHTMLStringForFormula(""#FORMULA#"")!&gt;</FIELD>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	getParentFormula = XML_OUTPUT 
End Function



Function getChildMW (rs_name, var_width, var_height,header_name, display_name, bShow)
	XML_OUTPUT=""
	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""MOLWEIGHT"" VALUE_CLASS=""child_mw_cell_value"" HEADER_CLASS=""child_mw_cell_header""  HEADER_NAME=""" & header_name & """ SORT_COLUMN=""MOLWEIGHT"" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_mw_cell_name"" SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """ HEIGHT=""" & var_height  & """   WIDTH=""" & var_width & """>#MOLWEIGHT#</FIELD>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	getChildMW = XML_OUTPUT 
End Function

Function getChildFormula (rs_name, var_width, var_height,header_name, display_name, bShow)
	XML_OUTPUT=""
	XML_OUTPUT = XML_OUTPUT &  "<FIELD VALUE_COLUMNS=""FORMULA"" VALUE_CLASS=""child_formula_cell_value"" HEADER_CLASS=""child_formula_cell_header""   HEADER_NAME="""& header_name & """ SORT_COLUMN=""FORMULA"" DISPLAY_NAME=""" & display_name & """ NAME_CLASS=""child_formula_cell_name"" SUB_RS_NAME=""" & RS_Name & """ COLSPAN=""1"" SHOW=""" & bshow & """  HEIGHT=""" & var_height  & """  WIDTH=""" & var_width & """ POST_PROCESS=""true"">&lt;!GetHTMLStringForFormula(""#FORMULA#"")!&gt;</FIELD>"
	XML_OUTPUT = XML_OUTPUT & vbNewLine
	getChildFormula = XML_OUTPUT 
End Function

Sub RestTableOrder(formgroup_id)
	if not isObject(uConn) then
		Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
	end if
	Set tablesRS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  uConn
	cmd.CommandType = adCmdText
	sql = "select table_order from biosardb.db_formgroup_tables where formgroup_id = ? order by table_order asc"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
	on error resume next
	tablesRS.LockType= 3
	tablesRS.CursorType = 3 
	tablesRS.Open cmd
	counter = 1
	If not (tablesRS.bof and tablesRS.eof) then
		 do while not tablesRS.eof
			tablesRS("table_order") = counter
			counter = counter + 1
			tablesRS.MoveNext
		 loop
		 tablesRS.BatchUpdate
		 tablesRS.close
	end if
	
end sub
</SCRIPT>

