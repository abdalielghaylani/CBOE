


<SCRIPT LANGUAGE=vbscript RUNAT=Server>
Sub CreateDisplaySQL(formgroup_id,maxTableOrder_L, maxTableOrder_D,maxTableOrder_Q)
	if not isObject(uConn) then
		Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
	end if
	Session("VIEWS_NEEDING_GRANTS") = ""
	Set RS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  uConn
	cmd.CommandType = adCmdText
	'Create List View Columns Across anad Column Down
	if maxTableOrder_Q > 0 then
	
		Call CreateFormGroupSQL(RS,cmd,formgroup_id, "QUERY",maxTableOrder_Q)
		if maxTableOrder_L> 0 then
	
			Call CreateFormGroupSQL(RS,cmd,formgroup_id, "LIST",maxTableOrder_L)
		end if
		if maxTableOrder_D> 0 then
		
			Call CreateFormGroupSQL(RS,cmd,formgroup_id, "DETAIL",maxTableOrder_D )
		end if
	end if
	
	
End sub

sub CreateFormGroupSQL(ByRef RS, ByRef cmd, formgroup_id, display_view,max_table_order)
	GET_MW_FORMULA_METHOD = Application("GET_MW_FORMULA_METHOD")
	Select Case UCase(display_view)
		case "QUERY"
			form_type_id = FORM_QUERY
		case "LIST"
			form_type_id = FORM_LIST
		case "DETAIL"
		
			form_type_id = FORM_DETAIL
	end select
	XML_OUTPUT = ""
	
	isBaseTable = false

	'get all form items for a particular table order and form xml output
	for i = 1 to CLng(max_table_order)
		dim currentTable
		dim table_list
		dim baseID
		dim table_id
		dim table_name
		dim column_name
		dim datatype
		dim lookup_display_name
		dim lookup_table_name
		dim lookup_column_name
		dim  column_jointype
		dim join_type
		dim field_item
		dim view_field_item
		dim lookup_join
		dim table_joins
		dim alias_item
		dim links
		dim running_width
		dim running_height
		running_width=""
		running_height = ""
		currentTable=""
		table_list=""
		baseID=""
		table_id=""
		table_name=""
		column_name=""
		datatype=""
		lookup_display_name=""
		lookup_table_name=""
		lookup_column_name=""
		column_jointype=""
		join_type=""
		field_item=""
		view_field_item=""
		lookup_join=""
		table_joins=""
		alias_item=""
		links=""
		table_alias_item=""

		if i = 1 then
			isBaseTable = true
		else
			isBaseTable = false
		end if
		
		sql = "Select dbvwfic.* from biosardb.db_vw_formitems_compact dbvwfic where formgroup_id = ? and formtype_id= ? and table_order = ?"
		
		cmd.CommandText = sql
		DeleteParameters(cmd)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 139, 1,0, form_type_id)
		cmd.Parameters.Append cmd.CreateParameter("pTableOrder", 139, 1, 0, CLng(i))
		'on error resume next
		Set RS = cmd.Execute
		
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pFormgroupID"
		'cmd.Parameters.Delete "pFormTypeID"
		'cmd.Parameters.Delete "pTableOrder"
		
		'loop through recordset and get elements
		hasfields = false
		if not (RS.BOF and RS.EOF) then ' if this is empty for table_order 1 it means there are no basetable fields
		hasfields = true
		  Do WHile Not RS.EOF
			currentTable = RS("TABLE_NAME")
			table_list =  addto_string(table_list, currentTable, ",")
			baseId =  RS("BASE_COLUMN_NAME")
			table_id = RS("TABLE_ID")
			table_name = RS("TABLE_NAME")
			temp= split(table_name, ".", -1)
			tschema_name = temp(0)
			table_name_short = temp(1)
			table_display_name= RS("TABLE_DISPLAY_NAME")
			column_name = RS("COLUMN_NAME")
			datatype = RS("DATA_TYPE")
			formatMask = RS("FORMAT_MASK")
			'JHS to add hyperlink functionality
			link = RS("LINK")
			linktext = RS("LINKTEXT")
			disptypename = RS("DISPLAY_TYPE_NAME")
			if isBaseTable = true then
				baseColumnName = baseid
				basetableName = table_name
				basetable_display_name = table_display_name
				basetableid=table_id
			end if
		
			lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")
			
			width = RS("WIDTH")
			height = RS("HEIGHT")
			if UCase(RS("DISPLAY_TYPE_NAME")) = "DATEPICKER"  then
					height ="-1"
					width = "50"
			end if
			if lookup_display_column <> "" then
				
				
			
				currentTable = RS("TABLE_NAME")
				'the lookup display name should be the column display name not the lookup columns' display Name!!!
				'lookup_display_name = RS("LOOKUP_DISPLAY_NAME")
				lookup_display_name = RS("COLUMN_NAME")
				lookup_column_display_name  = RS("DISPLAY_NAME")
				lookup_table_name = RS("LOOKUP_TABLE_NAME")
				temp= split(lookup_table_name, ".", -1)
				lookup_schema_name = temp(0)
				lookup_table_name_short = temp(1)
				lookup_column_name = RS("LOOKUP_COLUMN_NAME")
				
				column_jointype = RS("lookup_join_type")
				if inStr(column_jointype,"INNER")> 0 then
					join_type = ""
				else
					join_type = "(+)"
				end if
				'lookup_select = "(select lookup_table_name & "." & lookup_display_column  from 
				'field_item = addto_string(field_item, lookup_table_name & "." & lookup_display_column & " as " &  lookup_display_name)

					select Case UCase(RS("display_type_name"))
					case "STRUCTURE"
						if inStr(lcase(column_jointype),"case insensitive")> 0 then
							lookup_join = "lower(" & table_name & "." & column_name & ")=lower(" &  lookup_table_name & "." & lookup_column_name & ")" & join_type 
						else
							lookup_join = table_name & "." & column_name & "=" &  lookup_table_name & "." & lookup_column_name & join_type
						end if
						table_joins = addto_string(table_joins, lookup_join, " AND " )
						table_list =  addto_string(table_list, lookup_table_name, ",")
						if width <> "" and Not isNull(width) then
							width = CLng(width) * 0.5
						end if
						if height <> "" and Not isNull(height) then
							height = CLng(height) * 0.5
						end if
						'temp change:
						field_item = addto_string(field_item,lookup_table_name & "." & lookup_display_column& " as STRUCTURE", "," )
						field_item = addto_string(field_item,table_name & "." & column_name, "," )
						'add to view string for creating view
						view_field_item = addto_string(view_field_item,lookup_table_name & "." & lookup_display_column& " as STRUCTURE", "," )
						view_field_item = addto_string(view_field_item,table_name & "." & column_name & " as """ & column_name & """", "," )
						'NOT COMMENTED OUTfield_item = addto_string(field_item,lookup_table_name & "." & lookup_display_column, ",")
						alias_item = addto_string(alias_item,"STRUCTURE" & ":;:" & "STRUCTURE" & ":;:" & width & ":;:" & height , ",")
						
						query_alias_item = addto_string(query_alias_item,lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN & ":;:" & LOOKUP_DISPLAY_NAME, ",")

					case "MOLWEIGHT"
						if inStr(lcase(column_jointype),"case insensitive")> 0 then
							lookup_join = "lower(" & table_name & "." & column_name & ")=lower(" &  lookup_table_name & "." & lookup_column_name & ")" & join_type 
						else
							lookup_join = table_name & "." & column_name & "=" &  lookup_table_name & "." & lookup_column_name & join_type
						end if
						table_joins = addto_string(table_joins, lookup_join, " AND " )
						table_list =  addto_string(table_list, lookup_table_name, ",")
						indexName = GetCartIndexName(uConn, lookup_schema_name,lookup_table_name_short)
						if indexName <> "" then
							Select case UCase(GET_MW_FORMULA_METHOD)
								case "STANDARD" 'use shipping cartridge functions
									field_item = addto_string(field_item,ApplyFormatMask("CsCartridge.MolWeight(" & lookup_table_name & "." & lookup_display_column &")",formatMask) & " as " & " MOLWEIGHT" , ",")
									'add to view string for creating view
									'view_field_item =  addto_string(view_field_item,"CsCartridge.MolWeight(" & lookup_table_name & "." & lookup_display_column &")" & " as " & " MOLWEIGHT" , ",")
							
								case "PLSQL" 'use special function written for fast access through biosar
									mwTableName = GetCartMWTableName(uConn, lookup_schema_name,indexName)
									mwSelectText = "Cscartridge.FastIndexAccess.RowIDtoMolWeight(" & lookup_table_name & ".rowid,'" & mwTableName & "')" & " as " & " MOLWEIGHT"
									field_item = addto_string(field_item,mwSelectText, ",")
									'add to view string for creating view
									'view_field_item = addto_string(view_field_item,mwSelectText, ",")
								case "JOIN" 'use direct join method - requires grants on internal cartridge index tables
									'join to cscartridge tables. tables most be granted to pubic or individual roles
									mwTableName = GetCartMWTableName(uConn, lookup_schema_name,indexName)
									lookup_join = mwTableName & ".RID=" & lookup_table_name  & ".ROWID"
									table_joins = addto_string(table_joins, lookup_join, " AND " )
									table_list =  addto_string(table_list, mwTableName, ",")
									mwSelectText = "round(" & mwTableName & ".molweight,4) as MOLWEIGHT"
									field_item = addto_string(field_item,mwSelectText, ",")
									'add to view string for creating view
									'view_field_item = addto_string(view_field_item,mwSelectText, ",")
							End Select
								
						
							alias_item = addto_string(alias_item,"MOLWEIGHT" & ":;:" & "MOLWEIGHT"& ":;:" & width & ":;:" & height , ",")
							query_alias_item = alias_item
						end if
				
					case "FORMULA"
						if inStr(lcase(column_jointype),"case insensitive")> 0 then
							lookup_join = "lower(" & table_name & "." & column_name & ")=lower(" &  lookup_table_name & "." & lookup_column_name & ")" & join_type 
						else
							lookup_join = table_name & "." & column_name & "=" &  lookup_table_name & "." & lookup_column_name & join_type
						end if
						table_joins = addto_string(table_joins, lookup_join, " AND " )
						table_list =  addto_string(table_list, lookup_table_name, ",")
						indexName = GetCartIndexName(uConn, lookup_schema_name,lookup_table_name_short)
						if indexName <> "" then
							Select case UCase(GET_MW_FORMULA_METHOD)
								case "STANDARD" 'use shipping cartridge functions
									field_item = addto_string(field_item,"CsCartridge.Formula(" & lookup_table_name & "." & lookup_display_column & ",'')" & " as " & " FORMULA", ",")
									'add to view string for creating view
									'view_field_item =addto_string(view_field_item,"CsCartridge.Formula(" & lookup_table_name & "." & lookup_display_column & ",'')" & " as " & " FORMULA", ",")
								case "PLSQL" 'use special function written for fast access through biosar
									'join to cscartridge tables. tables most be granted to pubic or individual roles
									formulaTableName = GetCartFormulaTableName(uConn, lookup_schema_name,indexName)
									formulaSelectText = "Cscartridge.FastIndexAccess.RowIDtoFormula(" & lookup_table_name & ".rowid,'" & formulaTableName & "')" & " as " & " FORMULA"
									field_item = addto_string(field_item,formulaSelectText, ",")
									'add to view string for creating view
									'view_field_item = addto_string(view_field_item,formulaSelectText, ",")
								case "JOIN" 'use direct join method - requires grants on internal cartridge index tables
									'join to cscartridge tables. tables most be granted to pubic or individual roles
									'join to cscartridge tables. tables most be granted to pubic or individual roles
									formulaTableName = GetCartFormulaTableName(uConn, lookup_schema_name,indexName)
									lookup_join = formulaTableName & ".RID=" & lookup_table_name  & ".ROWID"
									table_joins = addto_string(table_joins, lookup_join, " AND " )
									table_list =  addto_string(table_list, formulaTableName, ",")
									formulaSelectText = formulaTableName & ".Formula as FORMULA"									
									field_item = addto_string(field_item,formulaSelectText, ",")
									'add to view string for creating view
									'view_field_item= addto_string(view_field_item,formulaSelectText, ",")
							End Select
							
							
								
							alias_item = addto_string(alias_item,"FORMULA" & ":;:" & "FORMULA"& ":;:" & width & ":;:" & height , ",")
							query_alias_item = alias_item
						end if
					case Else
						
					
						orig_lookup_table_name = lookup_table_name
						temp = split(orig_lookup_table_name, ".", -1)
						just_table_name = temp(1)
						lookup_table_name = just_table_name & column_name
						lookup_table_name = "a" & Right(lookup_table_name, 19)
						
						if inStr(lcase(column_jointype),"case insensitive")> 0 then
							
							lookup_join = "lower(" & table_name & "." & column_name & ")=lower(" &  lookup_table_name & "." & lookup_column_name & ")" & join_type 
						else
							lookup_join = table_name & "." & column_name & "=" &  lookup_table_name & "." & lookup_column_name & join_type
						end if
						table_joins = addto_string(table_joins, lookup_join, " AND " )
						table_list =  addto_string(table_list, orig_lookup_table_name & " " & lookup_table_name, ",")
						'don't put and LOB columns into display recordset - other then those for structures
						if Not ((datatype = "CLOB") or (datatype = "LOB") or (datatype = "LONG") or (datatype = "RAW") or (datatype = "BLOB")) then
							
							if datatype="DATE" then
							'date_sql is a subselect
								date_sql = formatDate(formatMask, lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN,column_name,"" ,"", "")
								field_item = addto_string(field_item,date_sql, ",")
								'add to view string for creating view
								date_sql_view = formatDate("-1", lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN,column_name,"" ,"", RS("DISPLAY_NAME"))
								view_field_item addto_string(view_field_item,date_sql_view, ",")
								
								alias_item = addto_string(alias_item,column_name & ":;:" &  RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height , ",")
								query_alias_item = addto_string(query_alias_item,lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN & ":;:" & RS("DISPLAY_NAME"), ",")
								
							else
								
								if HasParentRelationship(cmd, RS("COLUMN_ID"),basetableid) then
									final_column_name = "a_" & column_name
								else
									final_column_name=column_name
								end if
								
								'jhs hyperlink
								'if Not(link <> "" and ((datatype = "VARCHAR2") or (datatype = "NUMBER")))  then
								if Not (disptypename = "HYPERLINK") then
									if isOracleReserverdWord(LOOKUP_DISPLAY_COLUMN) then
										temp_string = ApplyFormatMask(lookup_table_name & "." & quotedString(LOOKUP_DISPLAY_COLUMN), formatMask) & " as " & final_column_name
									else
										temp_string = ApplyFormatMask(lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN, formatMask) & " as " & final_column_name
									end if
								else
									if isOracleReserverdWord(LOOKUP_DISPLAY_COLUMN) then
										temp_string = ApplyLinkMask(lookup_table_name & "." & quotedString(LOOKUP_DISPLAY_COLUMN),  link,linktext) & " as " & final_column_name
									else
										temp_string = ApplyLinkMask(lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN, link,linktext) & " as " & final_column_name
									end if
								end if
								
								field_item = addto_string(field_item,temp_string, ",")
								'add to view string for creating view
								temp_string_view = lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN & " as """ & RS("DISPLAY_NAME") & """"
								view_field_item = addto_string(view_field_item,temp_string_view, ",")
								'if this is the basetable and the field is a linking field to a child table then the unaliased form must also be in the select list
								if isBaseTable = true then
									if isChildLinkingField(cmd, RS("COLUMN_ID"),basetableid,formgroup_id) then
										temp_string_view = lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN
										view_field_item = addto_string(view_field_item,temp_string_view, ",")
							
									end if
								end if
								alias_item = addto_string(alias_item,column_name & ":;:" & RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height , ",")
								query_alias_item = addto_string(query_alias_item,lookup_table_name & "." & LOOKUP_DISPLAY_COLUMN & ":;:" & RS("DISPLAY_NAME"), ",")
							end if
						else
							if baseid <> "" then
								field_item = addto_string(field_item,table_name & "." & baseId , ",")
								field_item = addto_string(field_item,"nvl2(" & table_name & "." & column_name & ",'false','true') As ISNULLBLOB ", ",")

							end if
						end if

				end select
			else
			
				table_name = RS("table_name")
				
				temp = split(table_name, ".", -1)
				tschema_name=temp(0) 
				table_name_short=temp(1) 
				column_display_name = RS("DISPLAY_NAME")
				column_name =RS("COLUMN_NAME") 
				
				select Case UCase(RS("display_type_name"))
					case "STRUCTURE"
						'field_item = addto_string(field_item,column_name & " as " & column_display_name, ",")
						if width <> "" and Not isNull(width) then
							width = CLng(width) * 0.5
						end if
						if height <> "" and Not isNull(height) then
							height = CLng(height) * 0.5
						end if
						'temp change
						field_item = addto_string(field_item,table_name & "." & column_name & " as STRUCTURE", ",")
						'add to view string for creating view
						view_field_item=addto_string(view_field_item,table_name & "." & column_name & " as STRUCTURE", ",")
						
						alias_item = addto_string(alias_item,"STRUCTURE"& ":;:" & "STRUCTURE" & ":;:" & width & ":;:" & height , ",")
						query_alias_item = addto_string(query_alias_item,table_name & "." & COLUMN_NAME & ":;:" & column_display_name, ",")
					case "MOLWEIGHT"
				
						indexName = GetCartIndexName(uConn, tschema_name,table_name_short)
							Select case UCase(GET_MW_FORMULA_METHOD)
								case "STANDARD" 'use shipping cartridge functions
									field_item = addto_string(field_item,ApplyFormatMask("CsCartridge.MolWeight(" & table_name & "." & column_name &")",formatMask) & " as " & " MOLWEIGHT", ",")
									'add to view string for creating view
									'view_field_item=addto_string(view_field_item,"CsCartridge.MolWeight(" & table_name & "." & column_name &")" & " as " & " MOLWEIGHT", ",")
								case "PLSQL" 'use special function written for fast access through biosar
                            '-- CSBR ID: 134883
                            '-- Change Done by : Manoj Unnikrishnan
                            '-- Purpose: Moving the indexName check for each methods separately; Since STANDARD method doesn;t need the indexName
                            '-- Date: 03/06/2010
						            if indexName <> "" then
									'join to cscartridge tables. tables most be granted to pubic or individual roles
										mwTableName = GetCartMWTableName(uConn, tschema_name,indexName)
										mwSelectText = "Cscartridge.FastIndexAccess.RowIDtoMolWeight(" & table_name & ".rowid,'" & mwTableName & "')" & " as " & " MOLWEIGHT"
										field_item = addto_string(field_item,mwSelectText, ",")
										'add to view string for creating view
										'view_field_item= addto_string(view_field_item,mwSelectText, ",")
						            end if
								case "JOIN" 'use direct join method - requires grants on internal cartridge index tables
						            if indexName <> "" then
									'join to cscartridge tables. tables most be granted to pubic or individual roles
										mwTableName = GetCartMWTableName(uConn, tschema_name,indexName)
										lookup_join = mwTableName & ".RID=" & table_name  & ".ROWID"
										table_joins = addto_string(table_joins, lookup_join, " AND " )
										table_list =  addto_string(table_list, mwTableName, ",")
										mwSelectText = "round(" &  mwTableName & ".molweight,4) as MOLWEIGHT"
										field_item = addto_string(field_item,mwSelectText, ",")
										'add to view string for creating view
										'view_field_item= addto_string(view_field_item,mwSelectText, ",")
						            end if
                    		'-- End of Change #134883#
							End Select
						
							alias_item = addto_string(alias_item,"MOLWEIGHT" & ":;:" & "MOLWEIGHT" & ":;:" & width & ":;:" & height , ",")
							query_alias_item = alias_item
					case "FORMULA"
					
						indexName = GetCartIndexName(uConn, tschema_name,table_name_short)
							Select case UCase(GET_MW_FORMULA_METHOD)
								case "STANDARD" 'use shipping cartridge functions
									field_item = addto_string(field_item,"CsCartridge.Formula(" & table_name & "." & column_name & ",'')" & " as " & " FORMULA", ",")
									'add to view string for creating view
									'view_field_item=addto_string(view_field_item,"CsCartridge.Formula(" & table_name & "." & column_name & ",'')" & " as " & " FORMULA", ",")
								case "PLSQL" 'use special function written for fast access through biosar
                            '-- CSBR ID: 134883
                            '-- Change Done by : Manoj Unnikrishnan
                            '-- Purpose: Moving the indexName check for each methods separately; Since STANDARD method doesn;t need the indexName
                            '-- Date: 03/06/2010
						            if indexName <> "" then
									    'join to cscartridge tables. tables most be granted to pubic or individual roles
									    formulaTableName =  GetCartFormulaTableName(uConn, tschema_name,indexName)
									    formulaSelectText = "Cscartridge.FastIndexAccess.RowIDtoFormula(" & table_name & ".rowid,'" & formulaTableName & "')" & " as " & " FORMULA"
									    field_item = addto_string(field_item,formulaSelectText, ",")
									    'add to view string for creating view
									    'view_field_item=addto_string(view_field_item,formulaSelectText, ",")
						            end if
								case "JOIN" 'use direct join method - requires grants on internal cartridge index tables
                            '-- CSBR ID: 134883
                            '-- Change Done by : Manoj Unnikrishnan
                            '-- Purpose: Moving the indexName check for each methods separately; Since STANDARD method doesn;t need the indexName
                            '-- Date: 03/06/2010
						            if indexName <> "" then
									    'join to cscartridge tables. tables most be granted to pubic or individual roles
									    formulaTableName = GetCartFormulaTableName(uConn, tschema_name,indexName)
									    lookup_join = formulaTableName & ".RID=" & table_name  & ".ROWID"
									    table_joins = addto_string(table_joins, lookup_join, " AND " )
									    table_list =  addto_string(table_list, formulaTableName, ",")
									    formulaSelectText = formulaTableName & ".Formula as FORMULA"
									    field_item = addto_string(field_item,formulaSelectText, ",")
									    'add to view string for creating view
									    'view_field_item= addto_string(view_field_item,formulaSelectText, ",")
						            end if
                    		'-- End of Change #134883#
							End Select
							
								
							alias_item = addto_string(alias_item,"FORMULA" & ":;:" & "FORMULA" & ":;:" & width & ":;:" & height , ",")
							query_alias_item = alias_item
					case Else
				
						'don't put and LOB columns into display recordset - other then those for structures
						if Not ((datatype = "CLOB") or (datatype = "LOB") or (datatype = "LONG") or (datatype = "RAW") or (datatype = "BLOB")) then
							
							if datatype="DATE" then
								date_sql = formatDate(formatMask, table_name & "." & column_name,column_name,"","","")
								field_item = addto_string(field_item,date_sql, ",")
								
								'add to view string for creating view
								date_sql_view = formatDate("-1", table_name & "." & column_name,column_name,"","",column_display_name)
								view_field_item = addto_string(view_field_item,date_sql_view, ",")
							
								alias_item = addto_string(alias_item,column_name & ":;:" & column_display_name & ":;:" & width & ":;:" & height , ",")
								query_alias_item = addto_string(query_alias_item,table_name & "." & COLUMN_NAME & ":;:" & column_display_name, ",")

							else
								
								'jhs trying to get hypelinks in
							
								'if Not(link <> "" and ((datatype = "VARCHAR2") or (datatype = "NUMBER")))  then
								if Not (disptypename = "HYPERLINK") then
									if isOracleReserverdWord(column_name) then
										field_item = addto_string(field_item,ApplyFormatMask(table_name & "." & quotedString(column_name), formatMask)& " as " & quotedString(column_name) , ",")
									else
										field_item = addto_string(field_item,ApplyFormatMask(table_name & "." & column_name, formatMask) & " as " & column_name , ",")
									end if
								else
									if isOracleReserverdWord(column_name) then
										field_item = addto_string(field_item,ApplyLinkMask(table_name & "." & quotedString(column_name), link, linktext)& " as " & quotedString(column_name) , ",")
									else
										field_item = addto_string(field_item,ApplyLinkMask(table_name & "." & column_name, link, linktext) & " as " & column_name , ",")
									end if
								end if
								
								
								'add to view string for creating view
								temp_string_view = table_name & "." & column_name & " as """ & RS("DISPLAY_NAME") & """"
								view_field_item = addto_string(view_field_item,temp_string_view, ",")
								
								'if this is the basetable field is a linking field then the unaliased form must also be in the select list							
								if isBaseTable = true then
									if isChildLinkingField(cmd,RS("COLUMN_ID"),basetableid,formgroup_id)then
										temp_string_view = table_name & "." & column_name & " as " & column_name
										view_field_item = addto_string(view_field_item,temp_string_view, ",")
									end if
								end if
							
								alias_item = addto_string(alias_item,column_name & ":;:" & column_display_name & ":;:" & width & ":;:" & height , ",")
								query_alias_item = addto_string(query_alias_item,table_name & "." & COLUMN_NAME & ":;:" & column_display_name, ",")

							end if
							
						else
							if baseid <> "" then
									field_item = addto_string(field_item,table_name & "." & baseId , ",")
									field_item = addto_string(field_item,"nvl2(" & table_name & "." & column_name & ",'false','true') As ISNULLBLOB ", ",")

							end if
						end if
				end select
			
			end if
			
			if width <> "" then
				if CInt(width) > 0 then
					if running_width <> "" then
						running_width = CInt(running_width) + CInt(width)
					else
						running_width =  CInt(width)
					end if
				end if
			end if
			if height <> "" then
				if CInt(height) > 0  then
					on error resume next
					if CInt(running_height) > 0 then
						running_height = CInt(running_height) + CInt(height)
					else
						running_height =  CInt(height)
					end if
				end if
			end if
			RS.MoveNext
			loop
		else
				if isBaseTable = true then
				
					sql = "Select table_name, table_id, base_column_name from biosardb.db_vw_formgroup_tables where formgroup_id = ? and table_order= 1"
					DeleteParameters(cmd)
					cmd.CommandText = sql
					cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
					'on error resume next
					Set RS = cmd.Execute
					'uConn.Errors.clear
					'cmd.Parameters.Delete "pFormgroupID"
					basetableName = RS("table_name")
					if not Trim(basetableName) <> "" or isNull(basetableName) then
						
						RS.Close	
						exit sub			
					end if
					basetableid = RS("table_id")
					baseColumnName = RS("base_column_name")
					if not trim(baseColumnName) <> ""   or isNull(baseColumnName) then
						
						RS.Close
						exit sub					
					end if
					table_list = basetableName
					RS.Close
				else
					
					sql = "Select table_name, table_id, base_column_name from biosardb.db_vw_formgroup_tables where formgroup_id = ? and table_order=" & clng(i)
					DeleteParameters(cmd)
					cmd.CommandText = sql
					cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
					'on error resume next
					Set RS = cmd.Execute
					If Not (RS.BOF and RS.EOF)then
						'uConn.Errors.clear
						'cmd.Parameters.Delete "pFormgroupID"
						table_id = RS("table_id")
						RS.Close
					else
						table_id =""
						hasfields=false
					end if
				end if
		end if
		
		if hasfields=true then
			
			if Not running_height <> "" then
				running_height = "NULL"
			end if
			alias_item = alias_item  & ":CS_SIZES:" & running_width & "," & running_height
			if isBaseTable = true then
				basetable_alias_item = alias_item
				basetable_query_alias_item = query_alias_item
				basetable_field_items = field_item
				basetable_joins = table_joins
				basetable_table_list = table_list
				
				basetable_field_items = addto_string(basetable_field_items, basetableName & "." & baseColumnName & " as " & baseColumnName, ",")
				view_basetable_field_items = addto_string(view_field_item, basetableName & "." & baseColumnName & " as " & baseColumnName, ",")
				view_basetable_field_items = addto_string(view_basetable_field_items, basetableName & "." & baseColumnName & " as ""PK""", ",")
				view_basetable_field_items = addto_string(view_basetable_field_items, basetableName & ".ROWID" & " as ""ROWID_COL""", ",")

			else
				
				rel_join_temp = getRelationToBaseTable(uConn, basetableid,table_id, field_item, basetable_field_items,view_field_item)
			
				rel_join_array = split(rel_join_temp, "|", -1)
				field_item= rel_join_array(0)
				'get fields for view
				view_field_item= rel_join_array(5)
				
				basetable_field_items  = rel_join_array(1)
				rel_join = rel_join_array(2)
				all_link_fields=rel_join_array(3) & ";;;" & rel_join_array(4)
				
				table_list = addto_string(table_list, basetableName, "," )
				'hitlist_join =  basetableName & "." &  baseColumnName & " IN(select BIOSARDB.csdohitlist.ID from BIOSARDB.csdohitlist where BIOSARDB.csdohitlist.hitlistid =" & "?" & ")"
				'10/12 update to new hitlist handling
				hitlist_join =  basetableName & ".rowid"& "#=ROWID#"
				'hitlist_join =  basetableName & "." &  baseColumnName & "#=ROWID#"
				view_display_tag=""
				view_name = ""
				if UCase(Display_view) = "LIST" then
					view_display_tag = "_L"
				end if
				if UCase(Display_view) = "DETAIL" then
					view_display_tag = "_D"
				end if
						
				if table_joins <> "" then
					subtableSQL= "SELECT " & field_item & " FROM " & table_list & " WHERE " & rel_join &  " AND " & table_joins & " AND " & hitlist_join
					if view_display_tag <> "" then
						'DGB We need to ensure that the joining fields are present even if not displayed
						view_field_item = AddJoinFields(cmd.Activeconnection, view_field_item, formgroup_id, i)
						view_name=TruncateViewName(table_name_short,i-1,"_" & formgroup_id & view_display_tag, 26)
						createViewSQl = "CREATE OR REPLACE VIEW """ & view_name & """ as SELECT " & view_field_item & " FROM " & table_list & " WHERE " & rel_join &  " AND " & table_joins 
					end if
					'logaction("childtable" & createViewSQl)
				else
					subtableSQL= "SELECT " & field_item & " FROM " & table_list & " WHERE " & rel_join & " AND " & hitlist_join
					if view_display_tag <> "" then
						'DGB We need to ensure that the joining fields are present even if not displayed
						view_field_item = AddJoinFields(cmd.Activeconnection, view_field_item, formgroup_id, i)
						view_name=TruncateViewName(table_name_short,i-1,"_" & formgroup_id & view_display_tag, 26)
						createViewSQl = "CREATE OR REPLACE VIEW """ & view_name & """ as  SELECT " & view_field_item & " FROM " & table_list & " WHERE " & rel_join
					end if'logaction("childtable" & createViewSQl)
				end if
				if Display_view = "QUERY" then
					alias_item = query_alias_item
				end if
				Call UpdateDB_FormgroupSQL(cmd, formgroup_id, subtableSQL,table_id, display_view,alias_item,all_link_fields,table_name, table_display_name)
				'DGB createViews is never declared
				'if createViews = 1  and( UCase(display_view) = "LIST" or UCase(display_view) = "DETAIL") then
				if ( UCase(display_view) = "LIST" or UCase(display_view) = "DETAIL") then
					if view_display_tag <> "" then
						Call CreateViews(createViewSQl, formgroup_id,view_name )
					end if
				end if
			end if
			RS.Close
		else
		
			if table_id <> "" then
				Call RemoveDB_FormgroupSQL(cmd, formgroup_id, table_id, display_view)
			end if
		end if
	next
	
	'Create BaseTable Sql from  subtable information stored while looping
	'hitlist_join =  basetableName & "." &  baseColumnName & " IN(select BIOSARDB.csdohitlist.ID from BIOSARDB.csdohitlist where BIOSARDB.csdohitlist.hitlistid =" & "?" & ")"
	'10/12 update to new hitlist handling
	hitlist_join =  basetableName & ".rowid"& "#=ROWID#"
	'hitlist_join =  basetableName & "." &  baseColumnName & "#=ROWID#"
	temp_basetableNameShort = split(basetableName, ".", -1)
	basetableNameShort=temp_basetableNameShort(UBound(temp_basetableNameShort))
	if basetable_field_items <> "" then
		basetable_field_items = "," & basetable_field_items
	end if
	view_display_tag=""
	view_name=""
	if UCase(Display_view) = "LIST" then
		view_display_tag = "_L"
	end if
	if UCase(Display_view) = "DETAIL" then
		view_display_tag = "_D"
	end if
	if not basetable_table_list <> "" then
		basetable_table_list=basetableName
	end if

	if basetable_joins <> "" then
		baseSQL ="SELECT " & basetableName & "." & baseColumnName & " as IndexCounter, " & basetableName & "." & baseColumnName & " as pageRowNum" & basetable_field_items & "#S_PLACEHOLDER# FROM " & basetable_table_list & "#F_PLACEHOLDER# WHERE " & basetable_joins &  " AND " & hitlist_join & "#J_PLACEHOLDER#"
		if view_display_tag <> "" then
			view_name=TruncateViewName(basetableNameShort,0,"_" & formgroup_id & view_display_tag, 26)
			'DGB We need to ensure that the joining fields are present even if not displayed
			view_basetable_field_items = AddJoinFields(cmd.Activeconnection, view_basetable_field_items, formgroup_id, 1)
			baseSQLView= "CREATE OR REPLACE VIEW """ & view_name & """ as  SELECT " & view_basetable_field_items & " FROM " & basetable_table_list & " WHERE " & basetable_joins
		end if
		'logaction("basetable" & baseSQLView)
	else
		baseSQL ="SELECT " & basetableName & "." & baseColumnName & " as IndexCounter, " & basetableName & "." & baseColumnName & " as pageRowNum" & basetable_field_items & "#S_PLACEHOLDER# FROM " & basetable_table_list & "#F_PLACEHOLDER# WHERE " & hitlist_join & "#J_PLACEHOLDER#"
		if view_display_tag <> "" then
			view_name=TruncateViewName(basetableNameShort,0,"_" & formgroup_id & view_display_tag, 26)
			'DGB We need to ensure that the joining fields are present even if not displayed
			view_basetable_field_items = AddJoinFields(cmd.Activeconnection, view_basetable_field_items, formgroup_id, 1)
			baseSQLView= "CREATE OR REPLACE VIEW """ & view_name  & """ as  SELECT " & view_basetable_field_items & " FROM " & basetable_table_list
		end if
		'logaction("basetable" & baseSQLView)
	end if
	if Display_view = "QUERY" then
		basetable_alias_item = basetable_query_alias_item
	end if
	field_links = baseColumnName & ";;;" & baseColumnName
	Call UpdateDB_FormgroupSQL(cmd, formgroup_id, baseSQL,basetableid, display_view,basetable_alias_item,field_links,basetableName, basetable_display_name)
		'DGB createViews is never declared
		'if createViews = 1  and( UCase(display_view) = "LIST" or UCase(display_view) = "DETAIL") then
		if ( UCase(display_view) = "LIST" or UCase(display_view) = "DETAIL") then
			if view_display_tag <> "" then
				Call CreateViews(baseSQLView, formgroup_id,view_name )
			end if
		end if
	'CreateXMLTemplateFromRS RS, "c:\temp\test.xml", ""
	'RS2HTML RS, "c:\temp\test.xml", "", "", "", "", ""

End Sub

Sub UpdateDB_FormgroupSQL(byRef cmd, formgroup_id, sql_to_insert,table_id, display_view,field_aliases, field_links,table_name,table_display_name)
	
	dim sql_column_name
	alias_column_name = display_view & "_ALIASES"
	if Not display_view = "QUERY" then
	
		sql_column_name = "DISPLAY_SQL_" & display_view
		on error resume next
		sql = "Select " & sql_column_name & "," & alias_column_name & ",links" & ",table_name,table_display_name," & alias_column_name & " from biosardb.DB_FORMGROUP_TABLES  WHERE formgroup_id=? AND table_id=?"
	else
		sql = "Select table_name,table_display_name," & alias_column_name & " from biosardb.DB_FORMGROUP_TABLES  WHERE formgroup_id=? AND table_id=?"

	end if
	
	cmd.CommandText = sql
	DeleteParameters(cmd)
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
	cmd.Parameters.Append cmd.CreateParameter("pTableID", 139, 1,0,table_id )
	Set RS = Server.CreateObject("adodb.recordset")
	RS.LockType= 3
	RS.CursorType = 3  
	RS.Open cmd
	if Not display_view = "QUERY" then
		rs(sql_column_name) = sql_to_insert
		if Not field_links = "" then
			rs("links") = field_links
		end if
	end if
	rs(alias_column_name) = field_aliases
	if Not table_name = "" then
		rs("table_name") = table_name
	end if
	if not table_display_name = "" then
		rs("table_display_name") = table_display_name
	end if
	RS.update
	RS.close
	'cmd.Parameters.Delete "pFormgroupID"
	'cmd.Parameters.Delete "pTableID"
	'Response.Write err.number & err.description & sql

End Sub

Sub RemoveDB_FormgroupSQL(byRef cmd, formgroup_id, table_id, display_view)

	dim sql_column_name
	alias_column_name = "LIST_ALIASES,QUERY_ALIASES,DETAIL_ALIASES"
	if Not display_view = "QUERY" then
	
		sql_column_name = "DISPLAY_SQL_LIST,DISPLAY_SQL_DETAIL"
		on error resume next
		sql = "Select " & sql_column_name & "," & alias_column_name & ",links,table_name,table_display_name from biosardb.DB_FORMGROUP_TABLES  WHERE formgroup_id=? AND table_id=?"
	else
		sql = "Select table_name,table_display_name,QUERY_ALIASES from biosardb.DB_FORMGROUP_TABLES  WHERE formgroup_id=? AND table_id=?"

	end if
	cmd.CommandText = sql
	DeleteParameters(cmd)
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
	cmd.Parameters.Append cmd.CreateParameter("pTableID", 139, 1,0,table_id )
	Set RS = Server.CreateObject("adodb.recordset")
	RS.LockType= 3
	RS.CursorType = 3  
	RS.Open cmd

	Select case display_view
		Case "QUERY"
			rs("QUERY_ALIASES").value = " "
			rs("table_name").value = " "
			rs("table_display_name").value = " "
		Case "DETAIL"
			'make sure that links and table names are not wiped out if fields in list view exist. 
			if Not Trim(rs("DISPLAY_SQL_LIST").value) <> "" then
				rs("DISPLAY_SQL_DETAIL").value = " "
				rs("DETAIL_ALIASES").value = " "
				rs("links").value = " "
				rs("table_name").value = " "
				rs("table_display_name").value = " "
			else
				rs("DISPLAY_SQL_DETAIL").value = " "
				rs("DETAIL_ALIASES").value = " "
			end if
		'if it is list view and there are no fields then it is okay to set all the field to empty strings.
		Case "LIST"
				rs("DISPLAY_SQL_LIST").value = " "
				rs("links").value = " "
				rs("LIST_ALIASES").value = " "
				rs("table_name").value = " "
				rs("table_display_name").value = " "
	end select
	RS.update
	RS.close
	'cmd.Parameters.Delete "pFormgroupID"
	'cmd.Parameters.Delete "pTableID"
	'Response.Write err.number & err.description & sql

End Sub




Function getRelationToBaseTable(byRef uConn,base_table_id, child_table_id, field_items, base_table_items,view_field_item)

	Set RS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  uConn
	cmd.CommandType = adCmdText

	sql= "SELECT  " &_
	" (SELECT table_name FROM db_table bt WHERE bt.table_id = r.table_id) AS parent_table_name, " &_
	" (SELECT column_name FROM db_column bc WHERE bc.column_id = r.column_id) AS parent_column_name, " &_
	" (SELECT table_name FROM db_table ct WHERE ct.table_id = r.child_table_id) AS child_table_name, " &_ 
	" (SELECT column_name FROM db_column cc WHERE cc.column_id = r.child_column_id) AS child_column_name, " &_
	" join_type FROM db_relationship r WHERE  r.child_table_id = ? AND r.table_id = ?"


	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pChildTableID", 139, 1, 0, child_table_id) 
	cmd.Parameters.Append cmd.CreateParameter("pParentTableID", 139, 1, 0, base_table_id) 
	on error resume next
	Set RS = cmd.Execute
	if Not (RS.BOF and RS.EOF) then
	Do while not RS.EOF
		
		if instr(RS("join_type"),"OUTER") > 0 then
			jointype = "(+)"
		else
			jointype = ""
		end if
		
		if instr(lCase(RS("join_type")),"case insensitive") > 0 then
			if join_list <> "" then
				join_list = join_list & " AND lower(" &  RS("parent_table_name") & "." & RS("parent_column_name") & jointype & ")=lower(" & RS("child_table_name") & "." & RS("child_column_name") & ")"
			else
				join_list = " lower(" & RS("parent_table_name") & "." & RS("parent_column_name") & jointype & ")=lower(" & RS("child_table_name")& "." &  RS("child_column_name") & ")" &jointype
			end if
		else
			if join_list <> "" then
				join_list = join_list & " AND " &  RS("parent_table_name") & "." & RS("parent_column_name") &  jointype &"=" & RS("child_table_name") & "." & RS("child_column_name") 
			else
				join_list = RS("parent_table_name") & "." & RS("parent_column_name")&  jointype & "=" & RS("child_table_name")& "." &  RS("child_column_name") 
			end if
		end if
		view_field_item = addto_String(view_field_item, RS("child_table_name") & "." & RS("child_column_name"), ",")
		
		' Added column alias and defend against reserved words
		if isOracleReserverdWord(RS("child_column_name").value) then
			field_items = addto_String(field_items, RS("child_table_name") & "." & quotedString(RS("child_column_name").value) & " as " & quotedString(RS("child_column_name").value), ",")
			base_table_items= addto_String(base_table_items, RS("parent_table_name") & "." & quotedstring(RS("parent_column_name").value) &  " as " & quotedString(RS("parent_column_name").value), ",")
		else
			field_items = addto_String(field_items, RS("child_table_name") & "." & RS("child_column_name").value & " as " & RS("child_column_name").value, ",")
			base_table_items= addto_String(base_table_items, RS("parent_table_name") & "." & RS("parent_column_name").value &  " as " & RS("parent_column_name").value, ",")
		end if
		
		child_table_array = split(RS("child_table_name"), ".", -1)
		child_short_table = child_table_array(1)
		parent_table_array = split(RS("parent_table_name"), ".", -1)
		parent_short_table = parent_table_array(1)
		link_fields = addto_string(link_fields, RS("child_column_name"), ",")
		base_link_fields = addto_string(base_link_fields, RS("parent_column_name"), ",")
		rs.movenext
	loop
	RS.close
	end if
	getRelationToBaseTable = field_items & "|" & base_table_items & "|" & join_list & "|" & link_fields & "|" & base_link_fields & "|" & view_field_item
End Function

'******************************Support Functions**************************
Function GetCartIndexName(byRef DataConn, schema_name, tablename)

	Dim s 
	Dim r 
	on error resume next
	Set r = Server.CreateObject("ADODB.RECORDSET")
	
	s = "SELECT INDEX_NAME FROM ALL_INDEXES WHERE TABLE_OWNER = '" & UCase(schema_name) & "' AND TABLE_NAME = '" & UCase(tablename) & "' AND ITYP_OWNER = 'CSCARTRIDGE' AND ITYP_NAME = 'MOLECULEINDEXTYPE'"
	
	Set r = DataConn.Execute(s)
	if Not (r.EOF and r.BOF) then
		theReturn =  r.Fields("INDEX_NAME").Value
		r.close
	else
		theReturn = ""
	end if
	if err.number <> 0 then
		logaction(err.number & err.Description)
	end if
	
	GetCartIndexName = theReturn
End Function

Function GetCartMWTableName(byRef DataConn, schema_name, index_Name)
	Dim s 
	Dim r 
	on error resume next
	Set r = Server.CreateObject("ADODB.RECORDSET")
	
	s = "SELECT CSCartridge.Aux.MWTabName('" & UCase(schema_name) & "','" & UCase(index_name) & "') as table_name FROM DUAL"
	
	Set r = DataConn.Execute(s)
	if Not (r.EOF and r.BOF) then
		theReturn = r.Fields("TABLE_NAME").Value
		r.close
	else
		theReturn = ""
	end if
	
	if err.number <> 0 then
		logaction(err.number & err.Description)
	end if
	
	GetCartMWTableName = theReturn
End Function

Function GetCartFormulaTableName(byRef DataConn, schema_name, index_Name)

	Dim s 
	Dim r 
	on error resume next
	Set r = Server.CreateObject("ADODB.RECORDSET")
	
	s = "SELECT CSCartridge.Aux.FMTabName('" & schema_name & "','" & index_name & "') as table_name FROM DUAL"
	
	Set r = DataConn.Execute(s)
	if Not (r.EOF and r.BOF) then
		theReturn = r.Fields("TABLE_NAME").Value
		r.close
	else
		theReturn = ""
	end if
	if err.number <> 0 then
		logaction(err.number & err.Description)
	end if
	
	GetCartFormulaTableName = theReturn
End Function

function getIndexType(byRef dataconn, col_id)
	set rs = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  dataconn
	cmd.CommandType = adCmdText
	sql = "select index_type from biosardb.db_index_type,biosardb.db_column where db_index_type.index_type_id = db_column.index_type_id and db_column.column_id=?"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pcol_id", 139, 1, 0, col_id) 
	rs.open cmd
	if not (rs.bof and rs.eof) then
		indextype = rs("index_type")
	else
		indextype = ""
	end if
	getIndexType=indextype
End function
function getContentType(byRef dataconn, col_id)
	set rs = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  dataconn
	cmd.CommandType = adCmdText
	sql = "select mime_type from biosardb.DB_HTTP_CONTENT_TYPE, biosardb.db_column where DB_HTTP_CONTENT_TYPE.content_type_id = db_column.content_type_id  and db_column.column_id=?"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pcol_id", 139, 1, 0, col_id) 
	rs.open cmd
	if not (rs.bof and rs.eof) then
		contenttype = rs("mime_type")
	else
		contenttype = ""
	end if
	getContentType=contenttype
End function

function isLOB(datatype)
	if  ((datatype = "CLOB") or (datatype = "LOB") or (datatype = "LONG") or (datatype = "RAW") or (datatype = "BLOB")) then
		theReturn = true
	else
		theReturn = false
	end if
	isLob = theReturn
end function
			
function formatDate(formatMask, fullfieldname, fieldname, lookup_join, lookup_table_name, field_display_name)
	dateFormat = Application("DATE_FORMAT")
	if not dateFormat <> "" then
		dateFormat = "8"
	end if
	
	'DGB Added support for Custom Format Masks to override the application based dateFormat
	
	if formatMask <> "" then
		dateFormat = 0
	End if
	
	' DGB allow for no format mask to be used in view creation.
	if formatMask = "-1" then
		dateFormat = "-1"
	end if
	
	select case dateFormat
		case "-1"
			date_sql = fullfieldname
		case "0" ' Custom format mask
			date_sql = "TO_CHAR(" & fullfieldname & ",'" & formatMask & "')"		
		case "8"
			date_sql = "TO_CHAR(" & fullfieldname & ",'MM/DD/YYYY')"
		case "9"
			date_sql = "TO_CHAR(" & fullfieldname & ",'DD/MM/YYYY')"
		case "10"
			date_sql = "TO_CHAR(" & fullfieldname & ",'YYYY/MM/DD')"
	end select
	if lookup_join <> "" then
		if field_display_name <> "" then
			final_date_sql = "( select " & date_sql & " FROM " & lookup_table_name & "WHERE " & lookup_join & ") as """ & field_display_name & """"
		else
			final_date_sql = "( select " & date_sql & " FROM " & lookup_table_name & "WHERE " & lookup_join & ") as " & fieldname
		end if
	else
		if field_display_name <> "" then
			final_date_sql = date_sql & " as """ & field_display_name & """"
		else
			final_date_sql = date_sql & " as " & fieldname
		end if
	end if
	formatDate=final_date_sql
End function

Sub CreateViews(view_sql,formgroupid, view_name)
	if Application("BIOVIZ_INTEGRATION")=1   then
		if Application("FORMGROUP_BIOVIZ_INTEGRATION"& formgroupid)=1 then
			if Session("VIEWS_NEEDING_GRANTS") <> "" then
				Session("VIEWS_NEEDING_GRANTS") = Session("VIEWS_NEEDING_GRANTS") &  "," & view_name 
			else
				Session("VIEWS_NEEDING_GRANTS") = view_name
			end if
			test = Session("VIEWS_NEEDING_GRANTS")
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =   schemaConnection("BIOSARDB")
			cmd.CommandType = adCmdText
			cmd.CommandText = view_sql
			cmd.execute
		
			
			' Need to grant to the formgroup owner
			sOwner = getFGOwner(cmd.ActiveConnection, formgroupid)
			
			if sOwner <> "" then
				cmd.CommandText = "grant select on " &  view_name & " to " & sOwner
				cmd.execute
			end if
			
			'JHS 5/3/2007  - This will fix a bug and hopefully not create new ones.
			sFGRole = getFGRole(cmd.ActiveConnection, formgroupid)
			
			if 	sFGRole <> "" then
				cmd.CommandText = "grant select on " &  view_name & " to " & sFGRole
				cmd.execute
			end if	
			'  TODO:  May Need to grant to ChemFinder, but we are not sure if CHEMFINDER is there!
			'cmd.CommandText = "grant select on " &  view_name & " to CHEMFINDER with grant option"
			'logaction("grant select on " &  view_name & " to CHEMFINDER with grant option")
			'cmd.execute	
		end if	
	end if
End Sub

Function getFGOwner(oConn, formgroup_id)
	Dim cmd
	Dim RS
	if isObJect(oConn) then
		if oConn.state <> 1 then
			Set oConn = schemaConnection("BIOSARDB")
		end if
	else
		Set oConn = schemaConnection("BIOSARDB")	
	end if
	
	Set cmd = server.CreateObject("ADODB.Command")
	cmd.ActiveConnection = oConn
	cmd.commandText = "Select user_id from BIOSARDB.DB_FORMGROUP Where db_formgroup.formgroup_id = ?"
	cmd.Parameters.Append cmd.CreateParameter("formgroupid", 5, 1, 0, formgroup_id) 
	Set RS = cmd.Execute
	if not (RS.EOF and RS.BOF) then
		getFGOwner = RS("user_id").value
	else
		getFGOwner = ""	
	end if
	RS.close
	Set RS = nothing
	Set cmd = nothing 			
End function
		
Function getFGRole(oConn, formgroup_id)
	Dim cmd
	Dim RS
	if isObJect(oConn) then
		if oConn.state <> 1 then
			Set oConn = schemaConnection("BIOSARDB")
		end if
	else
		Set oConn = schemaConnection("BIOSARDB")	
	end if
	
	Set cmd = server.CreateObject("ADODB.Command")
	cmd.ActiveConnection = oConn
	cmd.commandText = "Select role from DBA_ROLES Where ROLE like '%_" & formgroup_id & "'"
	'cmd.Parameters.Append cmd.CreateParameter("formgroupid", 5, 1, 0, formgroup_id) 
	Set RS = cmd.Execute
	if not (RS.EOF and RS.BOF) then
		getFGRole = RS("role").value
	else
		getFGRole = ""	
	end if
	RS.close
	Set RS = nothing
	Set cmd = nothing 			
End function

Function ApplyFormatMask(columnName, mask)
	Dim sOut
	sOut = ""
	if mask <> "" then
		sOut = "to_char(" & columnName & ", '" & mask & "')" 	
	else
		sOut = columnName
	end if
	ApplyFormatMask = sOut
end function

Function ApplyLinkMask(columnName, link, linktext)
	Dim sOut
	sOut = ""
	if link <> "" then
		link = Replace(link,"<", "||<")
		link = Replace(link,">" , ">||")

			myArr = split(link,"||")
			
			newlink = ""
			for iLoop=0 to Ubound(myArr)
				
				if instr(1,myArr(iLoop),"<") >0  then
					newlink = "CONCAT(" & newlink & "," & Replace(Replace(myArr(iLoop),"<",""),">",")")
				else
					newlink = "CONCAT(" & newlink & ",'" & myArr(iLoop) & "')"
				end if
			
			next
			
			'for iLoop=0 to Ubound(myArr)
			'	newlink = newlink & ")"
			'next
			newlink = replace(newlink,"CONCAT(,","CONCAT('',")
		
			if linktext <> "" then		
				sOut = "CONCAT(CONCAT('<a href=""'," & newlink & "),'"" target=""_blank"">" & linktext & "</a>')"
				'sOut = "'<a href=""" & link & """ target=""_blank"">" & linktext & "</a>" & "'"
			else
			
				sOut= "CONCAT(CONCAT(CONCAT(CONCAT('<a href=""'," & newlink & "),'"" target=""_blank"">')," & columnName & "),'</a>')"
			
			
				'sOut = "'<a href=""" & link & """ target=""_blank"">' || " & columnName & " || '</a>" & "'"
			end if
	else
		'if
			sOut= "CONCAT(CONCAT('<a href=""'," & columnName & "),'"" target=""_blank"">Link</a>')"

			'sOut = "'<a href=""' || " & columnName & " || '"" target=""_blank""\>Link</a\>" & "'"
		'end if
	
		'sOut = columnName
	end if
	ApplyLinkMask = sOut
end function


' Adds any fields that are part of a join to the tables select
' fields list, if the field is not already there.  The field
' is added without alising.
Function AddJoinFields(oConn, field_list, formgroup_id, table_order)
	Dim conn
	Dim cmd
	Dim RS
	Dim links
	Dim new_item
	Dim i
	
	links = ""	
	'if isObJect(oConn) then
	'	if oConn.state <> 1 then
			Set oConn = schemaConnection("BIOSARDB")
	'	end if
	'else
	'	Set oConn = schemaConnection("BIOSARDB")	
	'end if
	
	if table_order = 1 then 
		table_order = 2
		isBase = true
	end if
	on error goto 0
	Set cmd = server.CreateObject("ADODB.Command")
	cmd.ActiveConnection = oConn
	cmd.commandText =	"select pt.table_name||'.'||pc.column_name||' as '||pc.column_name as parentField, ct.table_name||'.'||cc.column_name as childField" &_ 
						"		from db_formgroup_tables ft, db_formgroup_tables fbt, " &_
						"		db_relationship r, " &_
						"	    db_column cc," &_
						"	    db_column pc," &_
						"	    db_table pt," &_
						"	    db_table ct" &_
						"	where pt.table_id = pc.table_id" &_
						"	and ct.table_id = cc.table_id " &_
						"	and pc.column_id = r.column_id" &_
						"	and cc.column_id = r.child_column_id" &_
						"	and ft.table_id = r.child_table_id" &_
						"	and ft.formgroup_id = fbt.formgroup_id" &_
						"	and fbt.table_id = r.table_id" &_ 
						"	and ft.formgroup_id = ?" &_
						"	and ft.table_order = ?" 

	cmd.Parameters.Append cmd.CreateParameter("formgroupid", 5, 1, 0, formgroup_id)
	cmd.Parameters.Append cmd.CreateParameter("tableorder", 5, 1, 0, table_order) 
	Set RS = cmd.Execute
	if NOT (RS.BOF and RS.EOF) then
		do while not rs.eof
			if isBase then 
				link = rs("parentField").value
			else
				link = rs("childField").value
			end if
			field_list =  addto_string(field_list,link,",")	
			rs.movenext
		loop
	end if
	AddJoinFields = field_list
End function		
</script>
