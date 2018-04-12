


<!-- #INCLUDE VIRTUAL="/biosar_browser/biosar_browser_admin/admin_tool/xmltemplate_func.asp" -->
<!-- #INCLUDE VIRTUAL="/biosar_browser/biosar_browser_admin/admin_tool/display_sql_func.asp" -->

<script runat="server" language="vbscript">
CDD_DEBUG = false
CDD_RESULT_DEBUG=false
CDD_RS_DEBUG=false
CDD_RS_RESULT_DEBUG=false
CDD_TIMER = false
formgroup = request("formgroup")
Dim counter
counter = 0




'Function that is called from all forms to populate necessary arrays for searching and displaying results
'The base_connection MUST be gathered from the ini file though.

Function populateFormDefArrays(ByVal dbkey, ByVal formgroup)
	if CDD_TIMER = true then
		t0 = timer
	end if
	Session("dbkey_admin_tools")="biosar_browser"
	if (UCase(request("formgroup")="BASE_FORM_GROUP")or (Not formgroup <> "")) then exit function
	Session("bypass_ini" & dbkey & formgroup) = true
	
	'used to populate some of the app variables then wiped
	Session("LOOKUP_NAMES" & dbkey & formgroup)="" 
	
	'wipe out all variables for this loaded formgroup
	Application.Lock
		'fields for general cow functionality
		Application("FORMGROUP_FULL_NAME" & dbkey & formgroup)=""
		Application("FORMGROUP_OWNER" & dbkey & formgroup)=""
		Application("QUERY_INPUT_FORM" &  dbkey  & formgroup) = ""
		Application("FORM_GROUP" & dbkey & formgroup) = ""
		Application("LOOKUP_NAMES" & dbkey & formgroup)=""
		Application("TABLE_GROUP" & dbkey & formgroup)=""
		ClearAppFormgroupTables()
			Application("CHEMconnections_str" & dbkey & formgroup)=""
		'fields for export hits
		Application("TABLE_LABELS" & formgroup & "detail") =""
		Application("TABLE_LABELS" & formgroup & "list") =""
		Application("TABLE_LABELS" & formgroup & "query") =""
		Application("FIELD_LABELS" & formgroup & "DETAIL")=""
		Application("FIELD_LABELS" & formgroup & "LIST")=""
		Application("LOOKUP_NAMES_SELECT" & dbkey & formgroup)=""
		'field list for sorting from search screen
		Application("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup) = ""
	
		'fields for storing xml templates
		ClearAppFormgroupXMLTemps 'stored in Application(
		
	Application.UnLock
	
	Session("LIST_RS") = ""
	Session("DETAIL_RS") = ""
	Session("DETAIL_RS_FULL") = ""
	'myArray = Application("base_connection" & dbkey)
	'Session("base_connection") = Application("base_connection" & dbkey)
	on error resume next
	Set DataConn = getNewConnection(dbkey, formgroup, "base_connection")
	if err.number > 0 then
		Response.Write "Your system username is incorrect. Please check cfserver.ini"
		Response.End
	end if
	if DataConn.State=0 then ' assume user has been logged out
		'DoLoggedOutMsg()
	end if
	
	checkFGRole dbkey, formgroup, DataConn
	ModifyMasterRole
	Session("LOOKUP_NAMES" & dbkey & formgroup) = ""
	Set RS = Server.CreateObject("ADODB.RECORDSET")

	'get Table Gorup definitions
	baseTableId = GetTableGroupValues(formgroup, dbkey, DataConn, RS)
	if baseTableID = "NO_TABLES" then
		
		showmessagedialog("There are no tables defined for this formgroup")
		path="/biosar_browser/biosar_browser/mainpage.asp?bypass_ini=true&dbname=biosar_browser&formgroup=base_form_group&refreshPath=/biosar_browser/biosar_browser/mainpage.asp"&_
			"&formmode=search&TreeID=1&TreeTypeID=" & Session("PRIVATE_CATEGORY_TREE_TYPE_ID") &_
			",2&ItemURL=/biosar_browser/biosar_browser/biosar_browser_action.asp%3Fdataaction=db%26bypass_ini=true" &_
			"&ItemQSId=formgroup&ItemTarget=main&ItemTypeID=2&TreeMode=open_items" &_
			"&ClearNodes=0&QsRelay=formgroup,bypass_ini,formmode,refreshPath,refreshFormgroup,showItems,qshelp,ItemQSId" & "&showitems=true&QSHelp=Click a link to select a query form."
		redirectpath = path
		doredirect dbkey, redirectpath
	else
		'get Form Group Definitions
		theReturn = GetFormGroupValues(formgroup, dbkey, DataConn, RS)
		setFormGroupInfo formgroup, dbkey, DataConn, RS 
		
		if theReturn <> "" then
			showmessagedialog("There are no fields defined for the base table of the formgroup")
			path="/biosar_browser/biosar_browser/mainpage.asp?bypass_ini=true&dbname=biosar_browser&formgroup=base_form_group&refreshPath=/biosar_browser/biosar_browser/mainpage.asp"&_
			"&formmode=search&TreeID=1&TreeTypeID=" & Session("PRIVATE_CATEGORY_TREE_TYPE_ID") &_
			",2&ItemURL=/biosar_browser/biosar_browser/biosar_browser_action.asp%3Fdataaction=db%26bypass_ini=true" &_
			"&ItemQSId=formgroup&ItemTarget=main&ItemTypeID=2&TreeMode=open_items" &_
			"&ClearNodes=0&QsRelay=formgroup,bypass_ini,formmode,refreshPath,refreshFormgroup,showItems,qshelp,ItemQSId" & "&showitems=true&QSHelp=Click a link to select a query form."
	
		
			redirectpath = path
			doredirect dbkey, redirectpath
		else
			'get Table Group Definitions
			Call getXMLTemplates(dbkey, formgroup, DataConn)
			GetTableValues "", dbkey, DataConn, RS,baseTableId,formgroup
			'add information about lookup tables  
			GetLookupTableValues Session("LOOKUP_NAMES" & dbkey & formgroup), dbkey, DataConn, RS, baseTableId, formgroup
			'get recordset and field definitions for output in form
			
			
			Call GetDisplayDefs(dbkey, formgroup, DataConn, "QUERY")
			Call GetDisplayDefs(dbkey, formgroup, DataConn, "LIST")
			Call GetDisplayDefs(dbkey, formgroup, DataConn, "DETAIL")
			Session("CurrentReportHitList_ID" & dbkey & formgroup)= ""
			CloseConn(DataConn)
			CloseRS(RS)
			
			
			'populate variable for lookup names used when exporting data
			Application.Lock
				 Application("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup) = Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup)
				Application("FIELD_LABELS" & formgroup &  "LIST")=Session("FIELD_LABELS" & formgroup &  "LIST")
				Application("FIELD_LABELS" & formgroup &  "DETAIL")=Session("FIELD_LABELS" & formgroup &  "DETAIL")
				Application("LOOKUP_NAMES_SELECT" & dbkey & formgroup)=Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup)
			Application.UnLock
			Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup)=""
			Session("FIELD_LABELS" & formgroup &  "DETAIL")=""
			Session("FIELD_LABELS" & formgroup &  "LIST")=""
			Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup)=""
			Session("table_list" & dbkey & formgroup & "DETAIL")=""
			Session("table_list" & dbkey & formgroup & "LIST")=""
			basetable =getbasetable(dbkey, formgroup, "basetable")
			Application.Lock
				Application(basetable & "RecordCount" & dbkey)=""
			Application.UnLock
			Session(basetable & "RecordCount" & dbkey)=""
		end if
		'debugging toggles. must switch in the form where the populate array function is called
		
	end if
		if CDD_DEBUG = true or CDD_RS_DEBUG = true or CDD_RESULT_DEBUG = true then
			Response.End
		end if
	if CDD_TIMER = true then
		logaction("Loading the formgroup variables took: " & timer-t0 & " seconds")
	end if
End Function

'Simple debuggin code for loggin sql when there is an error. called throughout functions onthis page.
Sub OutputSourceSQl(fromwhere, sql)
	Response.Write "Calling Function " & fromWhere & ":" & sql
	Response.end
End sub

Sub AddToTableGroup(dbkey, formgroup, table_name)
	on error resume next
	TableGroupArray = Application("TABLE_GROUP" & dbkey & formgroup)
	base_table =TableGroupArray(0)
	mol_table =TableGroupArray(1)
	sql_order = TableGroupArray(2)
	
	sql_order_array = Split(TableGroupArray(2), ",", -1)
	bExists = false
	for j = 0 to UBound(sql_order_array)
		If UCase(table_name) = UCase(sql_order_array(j)) then
			bExists = true
		end if
	next
	if  bExists = false then
		if add_sql_order <> "" then
			add_sql_order = add_sql_order & "," & table_name
		else
			add_sql_order = table_name
		end if
		
	end if
	
	if add_sql_order <> "" then
		new_sql_order =  sql_order & "," & add_sql_order
	else
		new_sql_order = sql_order
	end if
	Dim NewTableGroupArray
	ReDim NewTableGroupArray(2)
	NewTableGroupArray(0) = base_table
	NewTableGroupArray(1) = mol_table
	NewTableGroupArray(2) = new_sql_order
	
	Application.Lock
		Application("TABLE_GROUP" & dbkey & formgroup) = NewTableGroupArray
	Application.UnLock
	
	
End Sub

'Get values for each table in the formgroup. Equivalent to a Tables entry in the ini file
Sub GetTableValues(ByVal table_names, ByVal dbkey, ByRef DataConn, ByRef RS, ByVal BaseTableID, ByVal formgroup)

	' these tables are part of the formgroup definition
			'set this initially to false. it will be set later if a clob is found
			Application("CLOB_ORA" & dbkey & formgroup) = ""
			sql = GetTableIDSfromFromGroup("?")
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  DataConn
			cmd.CommandType = adCmdText
			cmd.CommandText = sql
			cmd.Parameters.Append cmd.CreateParameter("pformgroup", 5, 1, 0, formgroup) 
			on error resume next
			RS.Open cmd
			if err.number <> 0 then
				Response.Write "<b>SQL ERROR: </b>"
				OutputSourceSQl  "GetTableIDSfromFromGroup", sql
			end if
			on error goto 0
			Do While Not RS.EOF
				if TableRealName_list <> "" then
					TableRealName_list = TableRealName_list & "," &  RS("TABLE_ID")
				else
					TableRealName_list =  RS("TABLE_ID")
				end if
				RS.MoveNext
			loop
			RS.Close
		
	
		'get relationships
		bgetChildTables=true
		theArray = Split(TableRealName_list, ",", -1)
		
			for i = 0 to UBound(theArray)
				
				table_id = theArray(i)
				
				
				if CLng(table_id) = CLng(BaseTableID) then
					  bgetChildTables = false
				else
					  bgetChildTables = true
				end if
				
					SelectJoin_List=""
					if bgetChildTables = true then
							sql = getTableRelFromTableIDForBase("?", "?")
							set cmd = Server.CreateObject("adodb.command")
							cmd.ActiveConnection =  DataConn
							cmd.CommandType = adCmdText
							cmd.CommandText = sql
							cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
							cmd.Parameters.Append cmd.CreateParameter("pbase_id", 5, 1, 0, BaseTableID) 
							
							
							on error resume next
							RS.Open cmd
							if (RS.BOF and RS.EOF) then
								Response.write "<font color=""red"">ERROR: PARENT/CHILD releationship incorrectly set <font>"
								Response.Write "<br><b>SQL ERROR: </b>"
								OutputSourceSQl  "getTableRelFromTableID", sql
								Response.Write "<br><b>Please contact your BioSAR administrator to correct the problem </b>"
							Else
								RS.MoveFirst
								Do While Not RS.EOF
									
										'on error goto 0
										parent_table_id  = RS("TABLE_ID")
										parent_column_id =RS("COLUMN_ID")
										child_table_id = RS("CHILD_TABLE_ID")
										child_column_id= RS("CHILD_COLUMN_ID")
										join_type = RS("join_type")
										if CDD_DEBUG = true then
											Response.write "<b>parent_table_id</b>:" & parent_table_id
											Response.Write "<b>parent_column_id</b> :" & parent_column_id
											Response.Write "<b>child_table_id </b>:" & child_table_id
											Response.Write "<b>child_column_id</b> :" & child_column_id
										end if
										returnLinks = getChildTableInfo(DataConn, parent_table_id,parent_column_id,child_table_id,child_column_id,join_type)
										
										returnlinks_array = split(returnLinks, "|", -1)
										Table_real_name = returnlinks_array(0)
										child_table_name = returnlinks_array(1)
										SelectJoin_List_temp =returnlinks_array(2)
										SelectLink_List_temp= returnlinks_array(3)
										
										if SelectJoin_List <> "" then
											SelectJoin_List = SelectJoin_List & "|" & SelectJoin_List_temp
										else
											SelectJoin_List =  SelectJoin_List_temp
										end if
										
										if SelectLink_List <> "" then
											SelectLink_List = SelectLink_List & "|" & SelectLink_List_temp
										else
											SelectLink_List =  SelectLink_List_temp
										end if
									RS.MoveNext
							Loop
							RS.Close
							end if
						
							if err.number <> 0 then
									Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
									Response.Write "<br><b>SQL ERROR: </b>"
									OutputSourceSQl  "getTableRelFromTableID", sql
							end if
					else
						sql = getBaseTableRelFromTableID("?")
						set cmd = Server.CreateObject("adodb.command")
						cmd.ActiveConnection =  DataConn
						cmd.CommandType = adCmdText
						cmd.CommandText = sql
						cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
						
						
						on error resume next
						RS.Open cmd
							
						if err.number <> 0 then
							Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
							Response.Write "<br><b>SQL ERROR: </b>"
							OutputSourceSQl  "getBaseTableRelFromTableID",sql
						end if
						on error goto 0
						parent_table_id  = RS("TABLE_ID")
						parent_column_id =RS("BASE_COLUMN_ID")
						
						child_table_id = RS("TABLE_ID")
						child_column_id= RS("BASE_COLUMN_ID")
						returnLinks = getParentTableInfo(DataConn, parent_table_id,parent_column_id,child_table_id,child_column_id)

						RS.Close
						returnlinks_array = split(returnLinks, "|", -1)
						Table_real_name = returnlinks_array(0)
						parent_table_name = returnlinks_array(1)
						SelectJoin_List =returnlinks_array(2)
						SelectLink_List= returnlinks_array(3)
						Application.Lock
							Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup) = ""
						Application.UnLock
					end if
				
				SelectJoin=SelectJoin_List
				SelectLinks=SelectLink_List
				
				if bGetChildTables = True then
					temp_inter_tables = getInterTables(formgroup,DataConn, table_id)
					
					if temp_inter_tables <> "" then
						INTER_TABLES=parent_table_name & "," & temp_inter_tables
					else
						INTER_TABLES=parent_table_name
					end if
				end if
				if Not INTER_TABLES <> "" then
					INTER_TABLES = "NULL"
				end if
				InterTables=INTER_TABLES
				sql = getAllTableInfoFromTableID("?")
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
				
				
				on error resume next
					RS.Open cmd
					
					
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getAllTableInfoFromTableID", sql

					end if
				on error goto 0
				RelFields_str = ""
			
				bhasLookup = false
				Do While Not RS.EOF
					cowsDatatype = getCOWSDataType(RS("DATATYPE").value,RS("PRECISION").value,RS("SCALE").value)
					if RS("DATATYPE").value = "CLOB" then
					Application.Lock
						Application("CLOB_ORA" & dbkey & formgroup) = "TRUE"
					Application.UnLock
					end if
					Dim theName
					
					theName = RS("COLUMN_NAME")
					
					if RS("LOOKUP_TABLE_ID") <> "" then
						
					
						
						if isStructureColumn(RS("LOOKUP_COLUMN_DISPLAY").value, DataConn)then
						
							'if isMoleculeInFormgroup(dbkey, formgroup, table_id, DataConn) then
								getLookupInfo dbkey, formgroup, DataConn, Table_Real_Name,table_id,RS("COLUMN_NAME").value,cowsDatatype, RS("LOOKUP_TABLE_ID").value, RS("LOOKUP_COLUMN_ID").value, RS("LOOKUP_COLUMN_DISPLAY").value, RS("COLUMN_ID").value, 1
							'end if
							lookup_table_id = RS("LOOKUP_TABLE_ID")
							bhasLookup = true
						else
							if isInFormgroup(dbkey, formgroup,RS("COLUMN_ID").value, DataConn) then
								
								getLookupInfo dbkey, formgroup, DataConn, Table_Real_Name,table_id,RS("COLUMN_NAME").value,cowsDatatype, RS("LOOKUP_TABLE_ID").value, RS("LOOKUP_COLUMN_ID").value, RS("LOOKUP_COLUMN_DISPLAY").value,RS("COLUMN_ID").value, 0
							end if
						end if
					
					end if
					if RelFields_str <> "" then
						RelFields_str = RelFields_str & "," &  RS("COLUMN_NAME").value& ";" & cowsDatatype
					else
						RelFields_str =  RS("COLUMN_NAME").value & ";" & cowsDatatype
					end if
					
				RS.MoveNext
				loop
				RS.Close
				
				
				RelFields=RelFields_str
				
				
				sql = getColumnNameFromTableID("?")
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
				
				RS.Open cmd
				cmd.Parameters.delete "ptable_id"
				on error resume next
				
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getColumnNameFromTableID", sql
					end if
				on error goto 0
					Do While Not RS.EOF
						BaseColumnName = RS("COLUMN_NAME")
					RS.MoveNext
					loop
				RS.Close
				PrimaryKey= BaseColumnName
				SQLSyntax="ORACLE"
				
				'SelectKeyWord="DISTINCT" 'for now
				'SelectKeyWord=" /*+ ORDERED */ " 'for now
				SelectKeyWord="NULL"
				SelectAdditional="NULL"
				
				
				on error goto 0
					 
					
						if bhasLookup = true then
						'this is why the strucid isn't being set
						
							sql = getMOLIDColumnInfofromTableID("?","?")
							cmd.CommandText = sql
							cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
							cmd.Parameters.Append cmd.CreateParameter("ptable_id2", 5, 1, 0, lookup_table_id) 
							RS.Open cmd
							if not (RS.BOF and RS.EOF) then
								mol_id_Column_ID = RS("COLUMN_ID")
								STRUC_ID = RS("COLUMN_NAME")
							
								
								rs.close
								
								chem_conn_name = "CHEM_CONN" & table_id & dbkey & formgroup
								
								makeChemConn chem_conn_name, dbkey, parent_table_name, parent_column_id, STRUC_ID, mol_id_Column_ID, DataConn, RS
							
							else
								STRUC_ID="NULL"
								chem_conn_name = "NULL"
								RS.Close
							end if
						else
						
							sql = getMOLIDColumnInfofromTableID2("?")
							cmd.CommandText = sql
							cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
							
							RS.Open cmd
							if not (RS.BOF and RS.EOF) then
								mol_id_Column_ID = parent_column_id
								STRUC_ID = BaseColumnName
							
								
								rs.close
								
								chem_conn_name = "CHEM_CONN" & table_id & dbkey & formgroup
								
								makeChemConn chem_conn_name, dbkey, parent_table_name, parent_column_id, STRUC_ID, mol_id_Column_ID, DataConn, RS
							
							else
								STRUC_ID="NULL"
								chem_conn_name = "NULL"
								RS.Close
							end if
						end if
						on error resume next
					
				
				
				
				ChemConnection = chem_conn_name
				StrucFieldID= STRUC_ID 'not sure what to do here yet
				ADOConnection ="base_connection" 'Always base connection which is gathered from the ini file
				
				
				'create array.  
				if bGetChildTables = True then
					if CDD_DEBUG = true then Response.Write "<b><br>Table Array Name: </b>" & child_table_name & "<br>"
				else
					if CDD_DEBUG = true then Response.Write "<b><br>Table Array Name: </b>" & Parent_Table_Name & "<br>"
				end if
		
				Dim TableArray
				ReDim TableArray(15)
				'CDD_DEBUG = true
		
				TableArray(0) = Table_Real_Name 
				if CDD_DEBUG = true then Response.Write "<b><br>Table_Real_Name: </b>" & Table_Real_Name & "<br>"

				TableArray(1) = RelFields
				if CDD_DEBUG = true then Response.Write "<b>RelFields: </b>" & RelFields & "<br>"

				TableArray(2) = PrimaryKey
				if CDD_DEBUG = true then Response.Write "<b>PrimaryKey: </b>" & PrimaryKey & "<br>"

				TableArray(3) = SQLSyntax
				if CDD_DEBUG = true then Response.Write "<b>SQLSyntax: </b>" & SQLSyntax & "<br>"

				TableArray(4) = SelectKeyWord
				if CDD_DEBUG = true then Response.Write "<b>SelectKeyWord: </b>" & SelectKeyWord & "<br>"

				TableArray(5) = SelectJoin
				if CDD_DEBUG = true then Response.Write "<b>SelectJoin: </b>" & SelectJoin & "<br>"

				TableArray(6) = SelectLinks
				if CDD_DEBUG = true then Response.Write "<b>SelectLinks: </b>" & SelectLinks & "<br>"

				TableArray(7) = SelectAdditional
				if CDD_DEBUG = true then Response.Write "<b>SelectAdditional: </b>" & SelectAdditional & "<br>"

				TableArray(8) = InterTables
				if CDD_DEBUG = true then Response.Write "<b>InterTables: </b>" & InterTables & "<br>"

				TableArray(9) = ADOConnection
				if CDD_DEBUG = true then Response.Write "<b>ADOConnection: </b>" & ADOConnection & "<br>"

				TableArray(10) = ChemConnection
				if CDD_DEBUG = true then Response.Write "<b>ChemConnection: </b>" & ChemConnection & "<br>"

				TableArray(11) = StrucFieldID
				if CDD_DEBUG = true then Response.Write "<b>StrucFieldID: </b>" & StrucFieldID & "<br>"
				
				Application.Lock
					if bGetChildTables = True then
						Application(Trim(child_table_name) & dbkey & formgroup) = TableArray
						Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup) = BioSARaddto_stringNoDup(Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup), Trim(child_table_name), ",")

					else
						Application(Trim(Parent_Table_Name) & dbkey & formgroup) = TableArray
						Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup) = BioSARaddto_stringNoDup(Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup), Trim(Parent_Table_Name), ",")

					end if
				Application.UnLock
			
				
	
		next 'table from table_name_array
		
End Sub

'Get values for each table in the formgroup. Equivalent to a Tables entry in the ini file
Sub GetLookupTableValues(ByVal lookup_tables, ByVal dbkey, ByRef DataConn, ByRef RS, ByVal BaseTableID, ByVal formgroup)
		' each table_names entry is: lookup_table_name_alias & ":" & lookup_table_name & ":" & lookup_table_id
		
		If lookup_tables <> "" then 'these tables are being added and are not part of the formgroup definition
		
			lookup_array_items = split(lookup_tables, ",", -1)
			for i = 0 to UBound(lookup_array_items)
			
				lookup_array_items2 = split(lookup_array_items(i), ":", -1)
				PARENT_TABLE_NAME  = lookup_array_items2(0)
				LOOKUP_TABLE_NAME_ALIAS = lookup_array_items2(1)
				LOOKUP_COLUMN_NAME = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "LOOKUP_COLUMN_NAME")
				LOOKUP_COLUMN_DISPLAY_NAME = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "LOOKUP_COLUMN_DISPLAY_NAME")
				LOOKUP_REAL_NAME =  getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "LOOKUP_TABLE_NAME")
				PARENT_LINK_DISPLAY_NAME = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "PARENT_LINK_COLUMN_NAME")
				PARENT_TABLE_ID = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "PARENT_TABLE_ID")
				PARENT_LINK_COLUMN_ID = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "PARENT_LINK_COLUMN_ID")
				PARENT_LINK_COLUMN_IS_STRUCTURE = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "PARENT_LINK_COLUMN_IS_STRUCTURE")
				LINK_DATATYPE = getLookupInfoItem(dbkey, formgroup, parent_table_name, LOOKUP_TABLE_NAME_ALIAS, "LINK_DATATYPE")
				Join_Type = getJoinType(DataConn, PARENT_LINK_COLUMN_ID)
				if UCase(join_type) = "OUTER" then
					ADDJOIN="(+)"
				else
					ADDJOIN = ""
				end if
				SelectJoin = PARENT_TABLE_NAME & "." & PARENT_LINK_DISPLAY_NAME & "=" & LOOKUP_REAL_NAME & "." & LOOKUP_COLUMN_NAME & ADDJOIN
				SelectLinks = PARENT_TABLE_NAME & "." & PARENT_LINK_DISPLAY_NAME  & ";" & LINK_DATATYPE & "," &  LOOKUP_REAL_NAME & "." & LOOKUP_COLUMN_NAME & ";" & LINK_DATATYPE
				
				
				
				
				if CBool(PARENT_LINK_COLUMN_IS_STRUCTURE) = true then
					chem_conn_name = "CHEM_CONN" & PARENT_TABLE_ID & dbkey & formgroup
					ChemConnection = chem_conn_name
					StrucFieldID= LOOKUP_COLUMN_NAME
					Application.Lock
						if Application("CHEMconnections_str" & dbkey & formgroup) <> "" then
							Application("CHEMconnections_str" & dbkey & formgroup) = Application("CHEMconnections_str" & dbkey & formgroup) & "," & chem_conn_name
						else
							Application("CHEMconnections_str" & dbkey & formgroup) = chem_conn_name
						end if
					Application.UnLock
				else
					ChemConnection = "NULL"
					StrucFieldID= "NULL" 
				end if
				
				
				
				ADOConnection ="base_connection" 'Always base connection which is gathered from the ini file
				'create array.  
			
				if CDD_DEBUG = true then Response.Write "<b><br>Table Array Name: </b>" & LOOKUP_TABLE_NAME_ALIAS & "<br>"
					
			
				Dim TableArray
				ReDim TableArray(15)
				'CDD_DEBUG = true
				TableArray(0) = LOOKUP_REAL_NAME 
					if CDD_DEBUG = true then Response.Write "<b><br>Table_Real_Name: </b>" & Lookup_real_name & "<br>"
				TableArray(1) = LOOKUP_COLUMN_NAME & ";" & LINK_DATATYPE & "," & LOOKUP_COLUMN_DISPLAY_NAME & ";0"
					if CDD_DEBUG = true then Response.Write "<b>RelFields: </b>" & LOOKUP_COLUMN_NAME & ";" & LINK_DATATYPE & "," & LOOKUP_COLUMN_DISPLAY_NAME & ";0" & "<br>"
				TableArray(2) = LOOKUP_COLUMN_NAME
					if CDD_DEBUG = true then Response.Write "<b>PrimaryKey: </b>" & LOOKUP_COLUMN_NAME & "<br>"
				TableArray(3) = "ORACLE"
					if CDD_DEBUG = true then Response.Write "<b>SQLSyntax: </b>" & "ORACLE" & "<br>"
				TableArray(4) = "NULL"
					if CDD_DEBUG = true then Response.Write "<b>SelectKeyWord: </b>" & "NULL" & "<br>"
				TableArray(5) = SelectJoin
					if CDD_DEBUG = true then Response.Write "<b>SelectJoin: </b>" & SelectJoin & "<br>"
				TableArray(6) = SelectLinks
					if CDD_DEBUG = true then Response.Write "<b>SelectLinks: </b>" & SelectLinks & "<br>"
				TableArray(7) = "NULL"
					if CDD_DEBUG = true then Response.Write "<b>SelectAdditional: </b>" & "NULL" & "<br>"
				TableArray(8) = PARENT_TABLE_NAME
					if CDD_DEBUG = true then Response.Write "<b>InterTables: </b>" & PARENT_TABLE_NAME & "<br>"
				TableArray(9) = "base_connection"
					if CDD_DEBUG = true then Response.Write "<b>ADOConnection: </b>" & "base_connection" & "<br>"
				TableArray(10) = ChemConnection
					if CDD_DEBUG = true then Response.Write "<b>ChemConnection: </b>" & ChemConnection & "<br>"
				TableArray(11) = StrucFieldID
					if CDD_DEBUG = true then Response.Write "<b>StrucFieldID: </b>" & StrucFieldID & "<br>"
				Application.Lock
					Application(Trim(LOOKUP_TABLE_NAME_ALIAS) & dbkey & formgroup) = TableArray
					Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup) = BioSARaddto_stringNoDup(Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup), Trim(LOOKUP_TABLE_NAME_ALIAS),",")

				Application.UnLock
				AddToTableGroup dbkey, formgroup, LOOKUP_TABLE_NAME_ALIAS
				'AddToTableGroup dbkey, formgroup, LOOKUP_REAL_NAME

		next 'lookup table
	end if
End Sub


'Get values for each tablegroup in the formgroup. Equivalent to a Tables groups entry in the ini file

Function  GetTableGroupValues(ByVal formgroup, ByVal dbkey,ByRef DataConn, ByRef RS)
			sql = GetTableInfofromFormGroup(formgroup)
			
			on error resume next
				RS.Open sql, DataConn
				if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "GetTableInfofromFormGroup",sql
					end if
			on error goto 0
			if Not (RS.BOF AND RS.EOF) then
				basetable = RS("TABLE_NAME")
				basetableid = RS("TABLE_ID")
				RS.Close
			else
				GetTableGroupValues="NO_TABLES"
				exit function
			end if
		
		'Note: probably need to do something a bit more clever here - this is a short term sql-order 'til later.
			sql = GetSQLTableOrderFromFormGroup("?")
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  DataConn
			cmd.CommandType = adCmdText
			cmd.CommandText = sql
			cmd.Parameters.Append cmd.CreateParameter("pformgroup", 5, 1, 0, formgroup) 
			Set RS = cmd.Execute() 	
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "GetSQLTableOrderFromFormGroup", sql
					end if
				on error goto 0
		Do While Not RS.EOF 
			if SQLOrder <> "" then
				SQLOrder = SQLOrder & "," & RS("TABLE_NAME")
			else
				SQLOrder = RS("TABLE_NAME")
			end if
			RS.MoveNext
		loop
		RS.Close
		Dim TableGroupArray
		ReDim TableGroupArray(2)
		TableGroupArray(0) = BaseTable
		if CDD_DEBUG = true then Response.Write "<b>BaseTable: </b>" & BaseTable & "<br>"
		' calling this function stores all info about the moltable is stored in Session("MOLTABLE" & dbkey & formgroup) but returns the moltable name for the table group
		Moltable = BaseTable
		TableGroupArray(1) = MolTable
		if CDD_DEBUG = true then Response.Write "<b>MolTable: </b>" & MolTable & "<br>"

		TableGroupArray(2) = SQLOrder
		if CDD_DEBUG = true then Response.Write "<b>SQLOrder: </b>" & SQLOrder & "<br>"
		Application.Lock
			Application("TABLE_GROUP" & dbkey & formgroup)=TableGroupArray
		Application.UnLock
		GetTableGroupValues=basetableid
End Function


'Get values for each chem_connections in the formgroup. Equivalent to a ChemConnection group entry in the ini file

Sub makeChemConn(chem_conn_name, dbkey, table_name, table_id, column_name,mol_id_column_id, ByRef DataConn, ByRef RS)
	
	'if not isArray(Session(chem_conn_name)) then

				sql =getColumnInfoFromColumnID(mol_id_column_id)
			
				on error resume next
					RS.Open sql, DataConn
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getColumnInfoFromColumnID",sql
					end if
				on error goto 0
					if not (RS.BOF and RS.EOF) then
						
						StrucEngine = "CARTRIDGE"
						StrucFormName = "NULL"
						StrucFormPath = "NULL"
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName1 = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) 
							tableName2 = replace(returnTableName(DataConn,RS("TABLE_ID")), ".", "")
							tableName =  tableName1 & tablename2
							
						else
							tableName =  returnTableName(DataConn,RS("TABLE_ID"))
							
						end if
						StrucDBpath= "NULL"
						StrucTableName = tableName
					RS.Close
					End if

	
				'make sure forms can be opened and db is connected
				Dim ChemConnArray
				ReDim ChemConnArray(6)
				ChemConnArray(0) = StrucEngine
					if CDD_DEBUG = true then Response.Write "<b>StrucEngine: </b>" & StrucEngine & "<br>"

				ChemConnArray(1) = StrucFormName
					if CDD_DEBUG = true then Response.Write "<b>StrucFormName: </b>" & StrucFormName & "<br>"

				ChemConnArray(2) = StrucFormPath
					if CDD_DEBUG = true then Response.Write "<b>StrucFormPath: </b>" & StrucFormPath & "<br>"


				ChemConnArray(3) = StrucDBpath
					if CDD_DEBUG = true then Response.Write "<b>StrucDBpath: </b>" & StrucDBpath & "<br>"


				ChemConnArray(4) = StrucTableName
					if CDD_DEBUG = true then Response.Write "<b>StrucTableName: </b>" & StrucTableName & "<br>"


				ChemConnArray(5) = BaseFromPath
					if CDD_DEBUG = true then Response.Write "<b>BaseFromPath: </b>" & BaseFromPath & "<br>"


				'return array
				Application.Lock
					Application(chem_conn_name)= ChemConnArray
				
					if Application("CHEMconnections_str" & dbkey & formgroup) <> "" then
						Application("CHEMconnections_str" & dbkey & formgroup) = Application("CHEMconnections_str" & dbkey & formgroup) & "," & chem_conn_name
					else
						Application("CHEMconnections_str" & dbkey & formgroup) = chem_conn_name
					end if
				Application.UnLock
				
		
	'end if
end sub

'Get values for each  formgroup. Equivalent to a Formgoupr group entry in the ini file
Function GetFormGroupValues(formgroup, dbkey, ByRef DataConn, ByRef RS)
		sql = getURLFromFormGroup(formgroup, "QUERY")		
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "getURLFromFormGroup", sql
			end if
		on error goto 0
		if RS("URL") <> "" then
			queryformpath = RS("URL")
		else 
			queryformpath =dbkey & "_input_form.asp"
		end if
		RS.Close
		InputFormPath = queryformpath
		InputFormMode ="search"
		sql = getURLFromFormGroup(formgroup, "LIST")
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "getURLFromFormGroup", sql
			end if
		on error goto 0
		if RS("URL") <> "" then
			resultListView =  RS("URL") 
		else
			resultListView = dbkey & "_result_list.asp"
		end if
		RS.Close
		sql = getURLFromFormGroup(formgroup, "DETAIL")
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "getURLFromFormGroup", sql
			end if
		on error goto 0
		if RS("URL") <> "" then
			resultFormView =  RS("URL") 
		else
			resultFormView = dbkey & "_form.asp"
		end if
		RS.Close
	ResultFormPath =resultListView & ";" & resultFormView
	ResultFormMode ="list"

	'get all the searchable fields
		
		sql = GetAllFormItemsFromFormGroupAndFormType(formgroup, "QUERY")
				
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "GetAllFormItemsFromFormGroupAndFormType", sql
			end if
		'on error goto 0
					
		If Not (RS.EOF and RS.BOF) then	
			RS.MoveFirst
			Do While Not RS.EOF

			Select Case UCase(RS("DISP_TYP_NAME"))
				
				Case "STRUCTURE"
				
					if RS("LOOKUP_COLUMN_ID") <> "" then
						tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
						columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
						
					else
						tableName =  RS("TABLE_NAME")
						columnName = RS("COLUMN_NAME")
					end if
					if structure_items <> "" then
						structure_items = structure_items & "," & tableName & "." &  columnName
					else
						structure_items = tableName & "." &  columnName
					end if
					
				Case "MOLWEIGHT"
					if RS("LOOKUP_COLUMN_ID") <> "" then
						tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
						columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
						
					else
						tableName =  RS("TABLE_NAME")
						columnName = RS("COLUMN_NAME")
					end if
					if mw_items <> "" then
						mw_items = mw_items & "," & tableName & "." &  columnName & "_MOLWEIGHT"
					else
						mw_items = tableName & "." &  columnName & "_MOLWEIGHT"
					end if
				
				Case "FORMULA"
			
					if RS("LOOKUP_COLUMN_ID") <> "" then
						tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
						columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
						
					else
						tableName =  RS("TABLE_NAME")
						columnName = RS("COLUMN_NAME")
					end if
					if formula_items <> "" then
						formula_items = formula_items & "," & tableName& "." &  columnName & "_FORMULA"
					else
						formula_items = tableName & "." &  columnName & "_FORMULA"
					end if
				
				Case Else
					if RS("DATATYPE") = "CLOB" then
						Application.Lock
							Application("CLOB_ORA" & dbkey & formgroup) = "TRUE"
						Application.UnLock
					end if
					if rel_items <> "" then
						rel_items = rel_items & "," & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & ";" & getCOWSSearchDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"),RS("DISP_TYP_NAME"))
					else
						rel_items = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")& ";" & getCOWSSearchDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"),RS("DISP_TYP_NAME"))
					end if
				
				End Select		
				
				
				RS.MoveNext
			loop
			RS.Close
		else
			GetFormGroupValues = "no fields"
			exit function
		end if
		'set special variable that will determine if the plugin value is true. We only want the plugin value to be true if the query form has a structure field. 
		'this avoids the javascirpt error in the query form that occurs when detail and list view have structure fields but the query form doesn't.
		if structure_items <> "" then
			structure_items_in_query_form = structure_items
		else
			structure_items_in_query_form=""
		end if
		'get all the detail fields
		sql = GetAllFormItemsFromFormGroupAndFormType(formgroup, "DETAIL")
		
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "GetAllFormItemsFromFormGroupAndFormType", sql
			end if
		'on error goto 0
		RS.MoveFirst
		Do While Not RS.EOF
			if RS("COLUMN_NAME") <> "" then
					Select Case UCase(RS("DISP_TYP_NAME"))
				
					Case "STRUCTURE"
				
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						
						if structure_items <> "" then
							structure_items=BioSARaddto_stringNoDup(structure_items, tableName & "." &  columnName,",")
							'structure_items = structure_items & "," & tableName & "." &  columnName
						else
							structure_items = tableName & "." &  columnName
						end if
						
					Case "MOLWEIGHT"
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						if mw_items <> "" then
							mw_items=BioSARaddto_stringNoDup(mw_items, tableName & "." &  columnName & "_MOLWEIGHT",",")
							'mw_items = mw_items & "," & tableName & "." &  columnName & "_MOLWEIGHT"
						else
							mw_items = tableName & "." &  columnName & "_MOLWEIGHT"
						end if
				
					Case "FORMULA"
			
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						if formula_items <> "" then
							formula_items=BioSARaddto_stringNoDup(formula_items, tableName& "." &  columnName & "_FORMULA",",")
							'formula_items = formula_items & "," & tableName& "." &  columnName & "_FORMULA"
						else
							formula_items = tableName & "." &  columnName & "_FORMULA"
						end if
				
					Case Else
						if RS("DATATYPE") = "CLOB" then
							Application.Lock
								Session("CLOB_ORA" & dbkey & formgroup) = "TRUE"
							Application.UnLock
						end if
						if rel_items_display <> "" then
							rel_items_display = rel_items_display & "," & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & ";" & getCOWSDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"))
						else
							rel_items_display = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")& ";" & getCOWSDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"))
						end if
					end select
			end if
			RS.MoveNext
		loop
		RS.Close
		
		'get all the list fields
		sql = GetAllFormItemsFromFormGroupAndFormType(formgroup, "LIST")
		on error resume next
			RS.Open sql, DataConn
			if err.number <> 0 then
				Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
				Response.Write "<br><b>SQL ERROR: </b>"
				OutputSourceSQl  "GetAllFormItemsFromFormGroupAndFormType", sql
			end if
		'on error goto 0
		RS.MoveFirst
		Do While Not RS.EOF
			if RS("COLUMN_NAME") <> "" then
				Select Case UCase(RS("DISP_TYP_NAME"))
				
					Case "STRUCTURE"
				
						
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						
						if structure_items <> "" then
							structure_items=BioSARaddto_stringNoDup(structure_items, tableName & "." &  columnName,",")
							'structure_items = structure_items & "," & tableName & "." &  columnName
						else
							structure_items = tableName & "." &  columnName
						end if
						
					Case "MOLWEIGHT"
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						if mw_items <> "" then
							mw_items=BioSARaddto_stringNoDup(mw_items, tableName & "." &  columnName & "_MOLWEIGHT",",")
							'mw_items = mw_items & "," & tableName & "." &  columnName & "_MOLWEIGHT"
						else
							mw_items = tableName & "." &  columnName & "_MOLWEIGHT"
						end if
				
					Case "FORMULA"
			
						if RS("LOOKUP_COLUMN_ID") <> "" then
							tableName = returnTableName(DataConn, RS("LOOKUP_TABLE_ID")) & replace(RS("TABLE_NAME"), ".", "")
							columnName=returnColumnName(DataConn, RS("LOOKUP_COLUMN_DISPLAY"))
							
						else
							tableName =  RS("TABLE_NAME")
							columnName = RS("COLUMN_NAME")
						end if
						if formula_items <> "" then
							formula_items=BioSARaddto_stringNoDup(formula_items, tableName& "." &  columnName & "_FORMULA",",")
							'formula_items = formula_items & "," & tableName& "." &  columnName & "_FORMULA"
						else
							formula_items = tableName & "." &  columnName & "_FORMULA"
						end if
				
					Case Else
						if RS("DATATYPE") = "CLOB" then
							Application.Lock
								Application("CLOB_ORA" & dbkey & formgroup) = "TRUE"
							Application.UnLock
						end if
						if rel_items_list <> "" then
							rel_items_list = rel_items_list & "," & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & ";" &  getCOWSDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"))
						else
							rel_items_list = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")& ";" & getCOWSDataType(RS("DATATYPE"),RS("PRECISION"),RS("SCALE"))
						end if
				end select
			end if
			RS.MoveNext
		loop
		RS.Close
		
			
	if structure_items_in_query_form <> "" then
		PluginValue ="TRUE" 'need to check for chemfield -todo
	else
		PluginValue ="FALSE"
	end if

	
	FormGroupFlag="SINGLE_SEARCH"
	
	Searchable_AD0_FIELDS =rel_items

	StructureFields = structure_items
	
	MWFields = mw_items
	FormulaFields = formula_items
	SDFileFields = "TABLES:ALL"
	
	if Application("sdfExportMax") > 0 then
	    SDFileFields = SDFileFields & ";MAXRECORDS:" & Application("sdfExportMax") 
	end if
	
	TableGroup="TABLE_GROUP" & dbkey & formgroup
	RequiredFields=""
	NumListView="5"

	
	Dim formgrouparray(17)
	formgrouparray(0)=rel_items
	if CDD_DEBUG = true then Response.Write "<b>RelFields: </b>" & rel_items & "<br>"
	
	formgrouparray(1)=InputFormPath
	if CDD_DEBUG = true then Response.Write "<b>InputFormPath: </b>" & InputFormPath & "<br>"
	
	formgrouparray(2)=ResultFormPath
	if CDD_DEBUG = true then Response.Write "<b>InputFormPath: </b>" & InputFormPath & "<br>"
	
	formgrouparray(3)=InputFormMode
	if CDD_DEBUG = true then Response.Write "<b>InputFormMode: </b>" & InputFormMode & "<br>"
	
	formgrouparray(4)=ResultFormMode
	if CDD_DEBUG = true then Response.Write "<b>ResultFormMode: </b>" & ResultFormMode & "<br>"
	
	formgrouparray(5)=PluginValue
	if CDD_DEBUG = true then Response.Write "<b>PluginValue: </b>" & PluginValue & "<br>"
	
	formgrouparray(6)=FormGroupFlag
	if CDD_DEBUG = true then Response.Write "<b>FormGroupFlag: </b>" & FormGroupFlag & "<br>"
	
	formgrouparray(7)=StructureFields
	if CDD_DEBUG = true then Response.Write "<b>StructureFields: </b>" & StructureFields & "<br>"
	
	formgrouparray(8)=MWFields
	if CDD_DEBUG = true then Response.Write "<b>MWFields: </b>" & MWFields & "<br>"
	
	formgrouparray(9)=FormulaFields
	if CDD_DEBUG = true then Response.Write "<b>FormulaFields: </b>" & FormulaFields & "<br>"
	
	formgrouparray(10)=SDFileFields
	if CDD_DEBUG = true then Response.Write "<b>SDFileFields: </b>" & SDFileFields & "<br>"
	
	formgrouparray(11)=Rel_Fields_Display
	if CDD_DEBUG = true then Response.Write "<b>rel_fields_display: </b>" & rel_fields_display & "<br>"
	
	formgrouparray(12)=TableGroup
	if CDD_DEBUG = true then Response.Write "<b>TableGroup: </b>" & TableGroup & "<br>"
	
	formgrouparray(13)=RequiredFields
	if CDD_DEBUG = true then Response.Write "<b>RequiredFields: </b>" & RequiredFields & "<br>"
	
	formgrouparray(14)=NumListView
	if CDD_DEBUG = true then Response.Write "<b>NumListView: </b>" & NumListView & "<br>"

	
	formgrouparray(15)=rel_items_display
	if CDD_DEBUG = true then Response.Write "<b>DetailFields: </b>" & rel_items_display & "<br>"

	formgrouparray(16)=rel_items_list
	if CDD_DEBUG = true then Response.Write "<b>ListFields: </b>" & rel_items_list & "<br>"
	Application.Lock
		Application("FORM_GROUP" & dbkey & formgroup) = formgrouparray
	Application.UnLock
	
	GetFormGroupValues = ""

End Function

'Convert datatypes in DB form management schema to those used internally by cows. 

Function getCOWSDataType(ORAdatatype, Precision, theScale)
	dim cows_datatype
	Select Case UCase(ORAdatatype)
		CASE "VARCHAR2"
			cows_datatype = 0
		CASE "NUMBER"
			if Precision <> "" then
				on error resume next
					Test = CLng(Precision)
					testScale = CLng(theScale)
				on error goto 0
				If Test > 0  AND testScale > 0 then
					cows_datatype = 2
				else
					cows_datatype = 1
				end if
			else
				cows_datatype = 1
			end if
		CASE "CHAR"
			cows_datatype = 0
		CASE "DATE"
			cows_datatype = 8
		Case Else
			cows_datatype = 0
	end select
		
	
	getCOWSDataType=cows_datatype
End function

Function  getCOWSSearchDataType(ORAdatatype, Precision,theScale, cowsSpecialSearchType)

Select Case UCase(ORAdatatype)
		CASE "VARCHAR2"
			cows_datatype = 0
			if UCASE(cowsSpecialSearchType) = "TEXTBOXALLOWLIST" then
				
				cows_datatype = 5
			else
				cows_datatype = 0
			end if
		CASE "NUMBER"
			if Precision <> "" then
				on error resume next
					Test = CLng(Precision)
					TestScale = CLng(theScale)
				on error goto 0
				If Test > 0 AND TestScale > 0  then
					cows_datatype = 2
				else
					cows_datatype = 1
				end if
			else
				cows_datatype = 1
			end if
				'override this if other formatting is specified 
				if UCASE(cowsSpecialSearchType) = "TEXTBOXALLOWLIST"  then
					cows_datatype = 4
				end if
			
		CASE "CHAR"
			
			if UCASE(cowsSpecialSearchType) = "TEXTBOXALLOWLIST"  then
				cows_datatype = 5
			else
				cows_datatype = 0
			end if
		CASE "DATE"
			cows_datatype = 8
		Case Else
			cows_datatype = 0
			if UCASE(cowsSpecialSearchType) = "TEXTBOXALLOWLIST"  then
				cows_datatype = 5
			else
				cows_datatype = 0
			end if
	end select
		
	

	getCOWSSearchDataType=cows_datatype
End function

'Get inter tables entry used by  table entries. Equivalent to a INTER_TABLES entry in TABLE entry in the ini file

Function getInterTables(formgroup, ByRef DataConn, ByVal ChildTableID) 
	If Application("support_nested_tables") = "TRUE" then
		Dim sTables, sSQL
		Dim bNoParentFound
		Dim oRecordset
		Dim lCurrTableId
		Set RS = Server.CreateObject("ADODB.RECORDSET")
		lCurrTableId = ChildTableId
		bNoParentFound = False
		on error resume next
		
		Do While Not bNoParentFound
			sSQL = getParentRelationshipByParentTableID(formgroup, lCurrTableId)
			RS.Open  sSQL, DataConn
			if (RS.BOF and RS.EOF) then
				bNoParentFound = true
			Else
				if sTables <> "" then
					sTables = sTables & "," & RS("TABLE_NAME")
				else
					sTables = RS("TABLE_NAME")
				end if
				lCurrTableId = RS("TABLE_ID")
				RS.Close
			End If
		Loop
		On error resume next
		rs.Close
		getInterTables = sTables
	else
		getInterTables=""
	end if
End Function



'Parse out chemical fields in a formgroup
Function getFormgroupChemFields(dbkey, formgroup)
		on error resume next
		struc_fields_temp = GetFormGroupVal(dbkey, formgroup, kStructureFields)
		struc_fields = GetChemFieldCorr(dbkey, formgroup, struc_fields_temp, "structure")

		mw_fields_temp = GetFormGroupVal(dbkey, formgroup, kMWFields)
		mw_fields = GetChemFieldCorr(dbkey, formgroup, mw_fields_temp, "molweight")
		
		formula_fields_temp = GetFormGroupVal(dbkey, formgroup, kFormulaFields)
		formula_fields = GetChemFieldCorr(dbkey, formgroup, formula_fields_temp, "formula")
	
		full_table_list = struc_fields & "," & mw_fields & "," & formula_fields
		
		full_table_list_array = split(full_table_list, ",", -1)
		for i = 0 to Ubound(full_table_list_array)
			if full_table_list_array(i) <> "" then
				fullfieldname = full_table_list_array(i)
				table_name = parse_table_name(fullfieldname)
			
				if Not checkExistInList(final_list, table_name) = true   then
					if final_list <> "" then
						final_list = final_list & "," & table_name
					else
						final_list =table_name
					end if 'final_list <> "" then
				end if 'if Not checkExistInList(final_list, table_name) = true   then
			end if 'if full_list_array(i) <> "" then
		Next
		
		conn_list = getConnList(dbkey, formgroup, final_list)		
		getFormgroupChemFields = conn_list
		
End Function 

'simple function to parse table name and properly format
Function parse_table_name(byRef fullfieldname)
	fullfieldname_array = split(fullfieldname, ".", -1)
	if UBound(fullfieldname_array) = 2 then
		table_name = fullfieldname_array(0) & "." & fullfieldname_array(1)
	else
		table_name = fullfieldname_array(0) 
	end if
	parse_table_name = table_name
End function

'NOT USED get list of connections to store in application("ado_connectionstr" & dbkey & formgroup)
Function getConnList(dbkey, formgroup, table_list)
	on error resume next
	
	table_list_array = split(table_list, ",", -1)
		for i = 0 to Ubound(table_list_array)
			table_name = table_list_array(i)
			
			conn_name = GetTableVal(dbkey, Trim(table_name), kChemConnection)
			
		
			if conn_list <> "" then
				conn_list = conn_list & "," & conn_name
			else
				conn_list = conn_name
			end if
		next
	getConnList=conn_list
	
End Function

'simple function check is an item is in a list
Function checkExistInList(final_list, item_to_check)
		final_list_array =Split(final_list, ",", -1)
		inList = false
		for i = 0 to Ubound(final_list_array)
			if UCase(final_list_array(i))= UCase(item_to_check) then
				inList = true
				exit for
			end if
		next
		checkExistInList = inList
End Function

Sub getXMLTemplates(dbkey, formgroup, ByRef DataConn)
	
	on error resume next
	'Set mydict = Server.CreateObject("scripting.dictionary")
	Dim XMLTemp
	Set cmd = Server.CreateObject("adodb.command")
	Set RS = Server.CreateObject("adodb.recordset")
	
	cmd.ActiveConnection =  dataconn
	cmd.CommandType = adCmdText
	'get all formgroup information for a single table from the ordered RS1
	sql = "SELECT TEMPLATE_ID, TEMPLATE_DEF from biosardb.DB_XML_TEMPL_DEF where formgroup_id = ?"	
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup) 
	
	on error resume next
	RS.Open = cmd
	If (RS.EOF and RS.BOF) then
		
		CreateXMLTemplates(formgroup)
		
		RS.close
		RS.Open = cmd
	end if 
	cmd.Parameters.Delete "pFormgroupID"
	RS.MoveFirst
	Do while not RS.EOF
		TEMPLATE_ID= RS("TEMPLATE_ID")
		DEF = RS("TEMPLATE_DEF")
		Application.Lock
			Application("XML_DEFS" & dbkey & formgroup & TEMPLATE_ID) = DEF
			Application("TEMPLATE_IDS" & dbkey & formgroup) = BioSARaddto_stringNoDup(Application("TEMPLATE_IDS" & dbkey & formgroup), TEMPLATE_ID, ",")
		Application.UnLock
		RS.MoveNext
	Loop
	
End Sub
Sub ClearAppFormgroupXMLTemps()
	ids = Application("TEMPLATE_IDS" & dbkey & formgroup)
	temp = split(ids, ",", -1)
	for i = 0 to UBound(temp)
		Application.Lock
			Application("XML_AND_CSS_DEFS" & dbkey & formgroup & temp(i)) = ""
		Application.UnLock
	next
End Sub

Sub ClearAppFormgroupTables()
	tables = Application("ALL_FORMGROUP_TABLES" & dbkey & formgroup)
	temp = split(tables, ",", -1)
	for i = 0 to UBound(temp)
		Application.Lock
			Application(temp(i) & dbkey & formgroup)= ""
		Application.UnLock
	next
End Sub

		
'function to get Recordset definitions for storage in dictionary objects
'called from populateFormDefArrays

Sub getDisplayDefs(dbkey, formgroup, ByRef DataConn, display_view)

	Select Case UCase(display_view)
		case "QUERY"
			form_type_id = FORM_QUERY
		case "LIST"
			form_type_id = FORM_LIST
		case "DETAIL"
			form_type_id = FORM_DETAIL
	end select
	dim mytemp
	on error resume next
	'Set mydict = Server.CreateObject("scripting.dictionary")
	
	Set cmd = Server.CreateObject("adodb.command")
	Set RS = Server.CreateObject("adodb.recordset")
	
	cmd.ActiveConnection =  dataconn
	cmd.CommandType = adCmdText
	'get all formgroup information for a single table from the ordered RS1
	sql = "SELECT * from biosardb.DB_FORMGROUP_TABLES where formgroup_id = ? order by table_order asc"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup) 
	
	on error resume next
	RS.Open = cmd
	cmd.Parameters.Delete "pFormgroupID"
	
	if err.number <> 0 then
		Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
		Response.Write "<br><b>SQL ERROR: </b>"
		OutputSourceSQl  "GetALLFormGroupTableInfoForFormType", sql
	end if
	
	TABLE_NAME = ""
	TABLE_DISPLAY_NAME = ""
	TABLE_ID = ""
	TABLE_ORDER = ""
	
	if Not (RS.BOF and RS.EOF) then
		RS.MoveFirst
			Do While NOT RS.EOF 
				TABLE_NAME =RS("TABLE_NAME")
				TABLE_DISPLAY_NAME =RS("TABLE_DISPLAY_NAME")
				
				TABLE_ID = RS("TABLE_ID")
				TABLE_ORDER = RS("TABLE_ORDER")
						
				GET_ALL_DISPLAY_FIELDS = RS(UCase(display_view)  & "_ALIASES")
						
				if UCase(display_view) = "QUERY" then
					rs_output = TABLE_DISPLAY_NAME & ":::" & TABLE_NAME & ":::" & ""
				else
					rs_output = getRSOutput(dbkey, formgroup, RS, DataConn)
				end if
					
				Table_Names = BioSARaddto_stringNoDup(Table_Names, TABLE_NAME, ",")
				'protect commas
				protectTABLE_DISPLAY_NAME=protectCommas(TABLE_DISPLAY_NAME)
				Session("TABLE_LABELS" & formgroup & display_view) = BioSARaddto_stringNoDup(Session("TABLE_LABELS" & formgroup & display_view),Trim(UCase(TABLE_NAME))& ":" & protectTABLE_DISPLAY_NAME, ",")
					if display_view = "QUERY" then
						If TABLE_ORDER = "1" then
							rs_output = rs_output & GetAllDisplayFields
							if full_string_child <> "" then
								full_string_child = full_string_child & "|" & rs_output
							else
								full_string_child = rs_output
							end if
						else
							rs_output = rs_output & GET_ALL_DISPLAY_FIELDS
							if full_string_base <> "" then
								full_string_base = full_string_base & "|" & rs_output
							else
								full_string_base = rs_output
							end if
							
						end if
					end if
					myTemp = BioSARaddto_stringNoDup(myTemp, Trim(UCase(TABLE_NAME)) & ":CS_SUB_ITEM:" & Trim(rs_output), ":CS_ITEM:")
					'mydict.Add Trim(UCase(TABLE_NAME)), Trim(rs_output)
					if display_view = "QUERY" then
						getDisplayFields dbkey, formgroup, TABLE_ID,TABLE_NAME, DataConn, display_view
					end if
			RS.MoveNext
			Loop
			RS.Close
			
		end if
		'store completed strigns
		Application.Lock
			Application("TABLE_LABELS" & formgroup & display_view)=Session("TABLE_LABELS" & formgroup & display_view)
			Application("RS_DEFS" & display_view & dbkey & formgroup) = myTemp
		Application.UnLock
		
end Sub



'get information bout dsiplay fields
'called from getDisplayDefs<-populateFormDefArrays		
Sub getDisplayFields(dbkey, formgroup, table_id,table_name, byRef DataConn, display_View)
		
		form_type_id = FORM_QUERY
	
		Set RS = Server.CreateObject("adodb.recordset")
		Set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  dataconn
		cmd.CommandType = adCmdText
		'get all formgroup information for a single table from the ordered RS1
		'sql = "Select DB_VW_FORMITEMS_COMPACT.* from biosardb.DB_VW_FORMITEMS_COMPACT WHERE formtype_id=? AND table_id=? AND formgroup_id= ? order by table_order, column_order"
		sql = "Select dbvwfic.* from biosardb.DB_VW_FORMITEMS_COMPACT dbvwfic WHERE formtype_id=? AND table_id=? AND formgroup_id= ? order by table_order, column_order"
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pFormTypeID", 5, 1, 0, FORM_QUERY) 
		cmd.Parameters.Append cmd.CreateParameter("pTableID", 5, 1, 0, table_id)
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 5, 1, 0, formgroup) 
	
		on error resume next
		RS.Open = cmd
		
		cmd.Parameters.Delete "pFormTypeID"
		cmd.Parameters.Delete "pFormgroupID"
		cmd.Parameters.Delete "pTableID"
dim MyTempFields
		if err.number <> 0 then
			Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
			Response.Write "<br><b>SQL ERROR: </b>"
			OutputSourceSQl  "GetALLFGItemsForFormTypeAndTable", sql
		end if

			if NOT(RS.BOF and RS.EOF)then
				RS.MoveFirst
				
				'Set mydict = Server.CreateObject("scripting.dictionary")
			
				Do While Not RS.EOF
					field_output= getInputFieldDisplay(dbkey, formgroup,RS,DataConn)
					Trace "getInputOutput:::" & field_output, 30	
					if CDD_RESULT_DEBUG = True then
						Response.Write "getInputOutput:::" & field_output
						
					end if
					display_type = RS("DISPLAY_TYPE_NAME")
					table_name = RS("TABLE_NAME")
				
					column_name = RS("COLUMN_NAME")
					display_name = RS("DISPLAY_NAME")
					'protect COMMAS
					protectdisplay_name=protectCommas(display_name)
					isStrucField=false
					if (UCase(display_type) = "STRUCTURE" or UCase(display_type)= "MOLWEIGHT" or UCase(display_type)= "FORMULA") then
						isStrucField=true
						column_name = display_type
					end if
					
					if Not isStrucField then
						if Not CBool(Application("ALLOW_SORT_ALL_FIELDS")) = true then
							basetable = getbasetable(dbkey, formgroup, "basetable")
							if UCase(table_name) = UCase(basetable) then
								fullname_for_sort = table_name & "." &  column_name & ":"  &   protectdisplay_name
								if checkSortList(Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup), fullname_for_sort) then
									Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup)= BioSARaddto_stringNoDup(Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup), fullname_for_sort,",")
								end if
							end if 
						else
							fullname_for_sort = table_name & "." &  column_name & ":"  &   protectdisplay_name
							if checkSortList(Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup), fullname_for_sort) then
								 Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup)= BioSARaddto_stringNoDup( Session("QUERY_ALL_FIELD_NAMES" & dbkey & formgroup), fullname_for_sort, ",")
							end if
						end if
					end if
					fullname = table_name & "." &  column_name
					
					on error resume next	
					if field_output <> "" then
						MyTempFields =BioSARaddto_stringNoDup(MyTempFields,Trim(UCase(fullname)) & ":CS_SUB_ITEM:" & Trim(field_output),":CS_ITEM:")
						'mydict.ADD Trim(UCase(fullname)), Trim(field_output)
					end if
				
				RS.MoveNext
				Loop
			RS.Close
			Application.Lock
				 Application("FIELD_DEFS" & TABLE_NAME & display_view & dbkey & formgroup)=MyTempFields
			Application.UnLock
		else 'means there are no fields for the particular view so don't create a session variabl with a dictionary object
			Application.Lock
				Application("FIELD_DEFS" & TABLE_NAME & display_view & dbkey & formgroup)= "NULL"
			Application.UnLock
		end if
end Sub		



Function checkForBase64CDX(table_name)
				
				'this is where I need to put a function that returns information indicating
				'if the field has base64 in another table
		checkForbase64CDX = lookupTable		
End Function

Function getRSOutput(ByVal dbkey, ByVal formgroup, ByRef RS, ByRef DataConn)
	on error resume next
	'format as RSName and ":::" 
	
	thearray = Application("TABLE_GROUP" & dbkey & formgroup)
	base_table =thearray(kBaseTable)
	RS_Name = RS("TABLE_NAME") & "_RS"
	RS_TABLE_ID = RS("TABLE_ID")
	DisplaySQL_LIST = RS("DISPLAY_SQL_LIST")
	DisplaySQL_DETAIL = RS("DISPLAY_SQL_DETAIL")
	field_alias_list = RS("LIST_ALIASES")
	field_alias_detail = RS("DETAIL_ALIASES")
	
	RS_DISPLAY_NAME = RS("TABLE_DISPLAY_NAME")
	links = RS("LINKS")
	table_name = RS("TABLE_NAME")
	
	'export hit support

	ExportHitsList=BioSARfieldsforExport(field_alias_list, table_name)
	ExportHitsDetail=BioSARfieldsforExport(field_alias_detail, table_name)
	
	Session("FIELD_LABELS" & formgroup &  "LIST")=BioSARaddto_stringNoDup(Session("FIELD_LABELS" & formgroup &  "LIST"),ExportHitsList, ",")
	Session("FIELD_LABELS" & formgroup &  "DETAIL")=BioSARaddto_stringNoDup(Session("FIELD_LABELS" & formgroup &  "DETAIL"),ExportHitsDetail, ",")
	'end export hit support
	
	table_name2 = replace(table_name, ",", "_")
	link_keys = table_name2 & ";;;" & RS_DISPLAY_NAME & ";;;" &  links

	on error resume next
		
	Dim RS_SQL_ALL
	Dim RS_SQL_LIST
	Dim RS_SQL_DETAIL
	RS_SQL_ALL = ""
	RS_SQL_LIST = ""
	RS_SQL_DETAIL = ""
	RS_SQL_ALL = ""
	RS_SQL_LIST=link_keys & ";;;" & field_alias_list & ";;;" & DisplaySQL_LIST
	RS_SQL_DETAIL=link_keys & ";;;" & field_alias_detail & ";;;" & DisplaySQL_DETAIL
	
	if CDD_RS_DEBUG = true then
		Response.write RS_SQL_ALL & "<br>"
		Response.write RS_SQL_LIST & "<br>"
		Response.write RS_SQL_DETAIL & "<br>"
	end if

	getRSOutput = RS_DISPLAY_NAME & ":::" & RS_Name  & ":::" &  RS_SQL_ALL & "|:|" & RS_SQL_LIST & "|:|" & RS_SQL_DETAIL
End Function




Function getFGFieldNames(dbkey, formgroup, table_name, byRef DataConn, display_view)
	
	Dim RS
	Dim sql, field_names
	Dim bChemInfo
	sql = GetAllColFromFormGroupForTableAndDisplayType("?", "?", "?")
	
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pformgroup", 5, 1, 0, formgroup) 
	cmd.Parameters.Append cmd.CreateParameter("pdisplay_view", 5, 1, 0, display_view) 
	cmd.Parameters.Append cmd.CreateParameter("pText", 200, 1, Len(table_name)+ 1, table_name)
	
	bChemINFO = false
	Set RS = cmd.Execute
	If Not (RS.BOF and RS.EOF) then
		RS.MoveFirst
		Do While Not RS.EOF
			width = RS("WIDTH")
			height = RS("HEIGHT")
			if UCase(RS("DISP_TYP_NAME")) = "STRUCTURE"  then
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & " as STRUCTURE"
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID" & " as STRUCTURE"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if width <> "" and Not isNull(width) then
					width = CInt(width) * 0.5
				end if
				if height <> "" and Not isNull(height) then
					height = CInt(height) * 0.5
				end if
				if chemList <> "" then
					chemList = chemList & "," & RS("TABLE_NAME") & "." & "STRUCTURE" & ":;:" & "Structure" & ":;:" & width & ":;:" & height 
				else
					chemList = RS("TABLE_NAME") & "." & "STRUCTURE" & ":;:" & "Structure" & ":;:" & width & ":;:" & height 
				end if
				
			end if
			if UCase(RS("DISP_TYP_NAME")) = "FORMULA"  then
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & " as FORMULA"
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID"  & " as FORMULA"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if chemList <> "" then
					chemList = chemList & "," & RS("TABLE_NAME") & "." & "FORMULA" & ":;:" & "Formula" & ":;:" & width & ":;:" & height 
				else
					chemList = RS("TABLE_NAME") & "." & "FORMULA" & ":;:" & "Formula" & ":;:" &  width & ":;:" & height 
				end if
			end if
			if UCase(RS("DISP_TYP_NAME")) = "MOLWEIGHT"  then
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if RS("COLUMN_NAME") <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")  & " as MOLWEIGHT"
				else
					new_entry = RS("TABLE_NAME") & "." & "MOL_ID" & " as MOLWEIGHT"
				end if 
				if Not checkExistInList(field_names, new_entry) then
					field_names=field_names & "," & new_entry
				end if
				if chemList <> "" then
					chemList = chemList & "," & RS("TABLE_NAME") & "." & "MOLWEIGHT" & ":;:" & "MOLWEIGHT" & ":;:" & width & ":;:" & height 
				else
					chemList = RS("TABLE_NAME") & "." & "MOLWEIGHT" & ":;:" & "MOLWEIGHT" & ":;:" & width & ":;:" & height
				end if
			end if
			
			
			if RS("COLUMN_NAME") <> "" then
				UseLookupName = getLookupColumnDisplayInfoForSelect(dbkey, formgroup, RS("TABLE_NAME"),RS("COLUMN_NAME"))
				if UCase(RS("DISP_TYP_NAME")) = "DATEPICKER"  then
					height ="-1"
					width = "50"
				end if
				
				if  field_names <> "" then
					new_entry = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
					if Not checkExistInList(field_names, new_entry) then
						field_names=field_names & "," & new_entry
						if UseLookupName <> "" then
							field_list_with_display = field_list_with_display & "," & UseLookupName & ":;:" & RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height
						else
							field_list_with_display = field_list_with_display & "," & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & ":;:" & RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height
						end if
					end if
				else
					field_names=RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
					if UseLookupName <> "" then
						field_list_with_display = UseLookupName & ":;:" & RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height
					else
						field_list_with_display =  RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & ":;:" & RS("DISPLAY_NAME") & ":;:" & width & ":;:" & height

					end if
				end if
			else
				if chemInfo = true then
					
				'to do add cartridge formated calls for formula and mw
				end if
			end if
			if width <> "" then
				if CInt(width) > 0 then
					if running_width <> "" then
						running_width = CInt(running_height) + CInt(width)
					else
						running_width =  CInt(width)
					end if
				end if
			end if
			if height <> "" then
				if CInt(height) > 0  then
					if running_height <> "" then
						running_height = CInt(running_height) + CInt(height)
					else
						running_height =  CInt(height)
					end if
				end if
			end if
			
		RS.MoveNext
		Loop
		
	end if
	CloseRS(RS)
	if Not running_height <> "" then
		running_height = "NULL"
	end if
	if chemList <> "" then
		if field_list_with_display <> "" then
			getFGFieldNames = field_names & "|" & field_list_with_display & "," & chemList & ":CS_SIZES:" & running_width & "," & running_height
		else
			getFGFieldNames = field_names & "|" & chemList & ":CS_SIZES:" & running_width & "," & running_height
		end if
	else
		getFGFieldNames = field_names & "|" & field_list_with_display & ":CS_SIZES:" & running_width & "," & running_height
	end if
	
End Function


Function getInputFieldDisplay(ByVal dbkey, ByVal formgroup, ByRef RS, ByRef DataConn)

	on error resume next
	display_type = RS("DISPLAY_TYPE_NAME")
	width = RS("WIDTH")
	height = RS("HEIGHT")
	lookup_display_column = RS("LOOKUP_DISPLAY_COLUMN")	
				
	
	SELECT CASE UCase(display_type)
		Case "STRUCTURE"
			'theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = " " & ";;;"
			if lookup_display_column <> "" then
				tablename = RS("LOOKUP_TABLE_NAME")& replace(RS("TABLE_NAME"), ".", "")
				
				columnName = RS("LOOKUP_DISPLAY_COLUMN")
				fullfieldname = tablename  & "." & columnName
			else
				fullfieldname = RS("TABLE_NAME") & "." & RS("COLUMN_NAME")
			end if
			'ShowStrucInputField  dbkey, formgroup,"MolTable.Structure","5","340","180", "AllOptions", "SelectList"
			theReturn = theReturn & "ShowStrucInputField:;:" &_
			dbkey & "," & formgroup & ","  & fullfieldname & "," & "5" & "," &_
			width & "," & height & "," & "ALLOptions" & "," & "SelectList"
		
		Case "SELECT"
			'ShowLookUpList dbkey, formgroup,BaseRS,"Batches.Project_ID", Projects_List, Projects_Val,Projects_Text,0,true,"value","0" 
			Base_Table_name = RS("TABLE_NAME")
			base_Column_name = RS("COLUMN_NAME")
			Lookup_tablename = RS("LOOKUP_TABLE_NAME")
			Lookup_Column_Name = RS("LOOKUP_COLUMN_NAME")
			Lookup_Display_Column_Name = RS("LOOKUP_DISPLAY_COLUMN")
			sort_direction= RS("LOOKUP_SORT_DIRECT")
			'Lookup_Display_Name = RS2("LOOKUP_DISPLAY_NAME")
			theReturn = RS("DISPLAY_NAME")& ";;;"
			theSelectList= Lookup_tablename & ":*:" & Lookup_Column_Name & ":*:" & Lookup_Display_Column_Name & ":*:" & sort_direction
			theSelectVal=  Lookup_Column_Name
			theSelectText= Lookup_Display_Column_Name
			theSize= "0" 
			default_boolean= "true" 
			default_type = "value"
			default_value = "0" 'todo
			theReturn = theReturn & "ShowSelectList:;:" &_
			dbkey & "," & formgroup  & "," & Base_Table_name & "_RS" & ","  & Base_Table_name & "." & base_Column_name & "," &_
			theSelectList & "," & theSelectVal & "," & theSelectText & ","  &_
			theSize & "," & default_boolean & "," & default_type & "," &  default_value
		
		Case "TEXTAREA"
			theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "," &_
			RS("DISPLAY_TYPE_NAME") & ":" & height  & "," &_
			width

		Case "MOLWEIGHT"
			theReturn = " " & ";;;"
			'ShowInputField dbkey, formgroup,"MolTable.Molname",0,"30"
			theReturn = "MW" & ";;;"
			if lookup_display_column <> "" then
				tablename = RS("LOOKUP_TABLE_NAME")& replace(RS("TABLE_NAME"), ".", "")
				
				columnName = RS("LOOKUP_DISPLAY_COLUMN")
				fullfieldname = tablename  & "." & columnName  & "_MOLWEIGHT"
			else
				fullfieldname = RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "_MOLWEIGHT"
			end if
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & fullfieldname & "," &_
			RS("DISPLAY_TYPE_NAME") & ":" & RS("DISPLAY_OPTION")  & "," &_
			width	
		Case "FORMULA"
				theReturn = "Formula" & ";;;"
			if lookup_display_column <> "" then
				tablename = RS("LOOKUP_TABLE_NAME")& replace(RS("TABLE_NAME"), ".", "")
				
				columnName = RS("LOOKUP_DISPLAY_COLUMN")
				fullfieldname = tablename  & "." & columnName  & "_FORMULA"
			else
				fullfieldname = RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "_FORMULA"
			end if
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & fullfieldname & "," &_
			RS("DISPLAY_TYPE_NAME") & ":" & RS("DISPLAY_OPTION")  & "," &_
			width		
		
	
		Case "DATEPICKER"
			'ShowInputField dbkey, formgroup, "Reg_Numbers.Registry_Date", "DATE_PICKER:8","17"
			theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "," &_
			"DATE_PICKER" & ":" & "8"  & "," &_
			width
		
		Case "DATE"
			'ShowInputField dbkey, formgroup, "Reg_Numbers.Registry_Date", "DATE_PICKER:8","17"
			theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "," &_
			"DATE" & ":" & "8"  &"," &_
			width
		Case "CHECKBOX"
			'ShowInputField dbkey, formgroup,"MolTable.Molname",0,"30"
			theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "," &_
			"CHECKBOX" & ":" & "0"  & "," &_
			width
			
		
		Case ELSE
			'ShowInputField dbkey, formgroup,"MolTable.Molname",0,"30"
			
			theReturn = RS("DISPLAY_NAME") & ";;;"
			theReturn = theReturn & "ShowInputField:;:" &_
			dbkey & "," & formgroup &  ","  & RS("TABLE_NAME") & "." & RS("COLUMN_NAME") & "," &_
			RS("DISPLAY_TYPE_NAME") & ":" & RS("DISPLAY_OPTION")  & "," &_
			width
			
		End Select
	
	
	getInputFieldDisplay =theReturn 
End Function



Function getPublicFormGroupList(dbkey, formgroup, DataConn)
	sql="Select * from BIOSARDB.DB_FORMGROUP Where  BIOSARDB.DB_FORMGROUP.IS_PUBLIC = 'Y'"
	Set RS = DataConn.Execute(sql)
	If Not (RS.EOF and RS.BOF) then
		Do while not RS.EOF
			if  proper_fg_name_list <> "" then
				proper_fg_name_list = proper_fg_name_list & "," & RS("formgroup_name") & "_" & RS("formgroup_id")
			else
				proper_fg_name_list = RS("formgroup_name") & "_" & RS("formgroup_id")
			end if
		loop
		CloseRS(RS)
	end if
	getPublicFormGroupList=proper_fg_name_list
end Function

Sub getLookupInfo(dbkey,formgroup, ByRef DataConn, parent_table_name, parent_table_id, parent_link_column_name,parent_link_datatype, lookup_table_id,lookup_column_id,lookup_column_display,parent_column_id, isStructure)
		on error resume next
				
				lookup_table_id = lookup_table_id
				lookup_column_id = lookup_column_id
				lookup_column_display_id = lookup_column_display
				lookup_table_name = returnTableName(DataConn,lookup_table_id) 
				lookup_table_alias= returnTableName(DataConn,lookup_table_id) & replace(parent_table_name, ".", "")
				lookup_column_name = returnColumnName(DataConn, lookup_column_id)
				lookup_column_display_name = returnColumnName(DataConn, lookup_column_display_id)
				select_statement = "SELECT " & lookup_column_display_name & " FROM " & lookup_table_name & " WHERE " & parent_table_name & "." & parent_link_column_name & "=" & lookup_table_name & "." & lookup_column_name
				if Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup) <> "" then
					Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup)= Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup) & ";;;" & lookup_table_alias & ":::" & lookup_table_name & ":" & lookup_table_id & "," &_				
					lookup_column_name & ":" & lookup_column_id & "," & lookup_column_display_name & ":" & lookup_column_display_id & "," &_
					parent_table_name & ":" & parent_link_column_name & ":" & parent_table_id & ":" & parent_column_id & ":" &  isStructure & "," & parent_link_datatype
				else
					Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup)= lookup_table_alias & ":::" & lookup_table_name & ":" & lookup_table_id & "," &_
					lookup_column_name & ":" & lookup_column_id & "," & lookup_column_display_name & ":" & lookup_column_display_id & "," &_
					parent_table_name & ":" & parent_link_column_name & ":" & parent_table_id & ":" & parent_column_id & ":" & isStructure & "," & parent_link_datatype
				end if
				
				if Session("LOOKUP_NAMES" & dbkey & formgroup) <> "" then
					Session("LOOKUP_NAMES" & dbkey & formgroup) = Session("LOOKUP_NAMES" & dbkey & formgroup) & "," & parent_table_name & ":" & lookup_table_alias 
				else
					Session("LOOKUP_NAMES" & dbkey & formgroup) = parent_table_name & ":" & lookup_table_alias 
				end if
				if Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup) <> "" then
					Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup) = Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup) & "|" & parent_table_name & "." & parent_link_column_name & "::" & select_statement
				else
					Session("LOOKUP_NAMES_SELECT" & dbkey & formgroup) = parent_table_name & "." & parent_link_column_name & "::" & select_statement
				
				end if
				Session("FIELD_LABELS" & formgroup &  "LIST") = BioSARaddto_stringNoDup(Session("FIELD_LABELS" & formgroup &  "LIST"), parent_table_name & "." & parent_link_column_name & ":" & lookup_column_display_name, ",")
				Session("FIELD_LABELS" & formgroup &  "DETAIL") = BioSARaddto_stringNoDup(Session("FIELD_LABELS" & formgroup &  "DETAIL"), parent_table_name & "." & parent_link_column_name & ":" & lookup_column_display_name, ",")

				theReturn = lookup_table_name
		
End Sub

Function returnColumnName(byRef DataConn, column_id)
	sql = getColumnName("?")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  DataConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pcolumn_id", 5, 1, 0, column_id) 
	on error resume next
	Set rst2 = cmd.execute
	
	theReturn = rst2("COLUMN_NAME")
	rst2.close
	set rst2 = nothing
	returnColumnName= theReturn
End Function

Function returnTableName(byRef DataConn, Table_id)
	sql = getTableName("?")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  DataConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
	on error resume next
	Set rst2 = cmd.execute
	
	theReturn = rst2("Table_Name")
	
	rst2.close
	set rst2 = nothing
	returnTableName= theReturn
End Function

Function getALLLookupInfo(dbkey, formgroup, parent_table_name, item_name)
	Dim theReturn
	all_items = Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup)
	if all_items <> "" then
		lookup_array_all = Split(Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup), ";;;", -1)
		for i = 0 to UBound(lookup_array_all)
			lookup_array_single = split(lookup_array_all(i), ":::", -1)
			lookup_table = lookup_array_single(0)
			
			Select Case UCase(item_name)
				Case "FULL_LOOKUP_NAME"
					
					real_table = getLookupInfoItem(dbkey, formgroup, parent_table_name, lookup_table, "LOOKUP_TABLE_NAME") 
					if	real_table <> "" and Not (UCase(real_table) = UCase(parent_table_name)) then
						full_item = real_table & "." & getLookupInfoItem(dbkey, formgroup, parent_table_name, lookup_table, "LOOKUP_COLUMN_DISPLAY_NAME")
					else
						full_item = ""
					end if
					'AddToTableAliasTracker real_table, lookup_table
					if full_item <> "" then
						if theList <> "" then
							theList = theList & "," & full_item
						else
							theList = full_item
						end if
					end if
					
				 
				Case "TABLE_NAME"
					full_item = getLookupInfoItem(dbkey, formgroup, parent_table_name, lookup_table, "LOOKUP_TABLE_NAME")
					if theList <> "" then
						theList = theList & "," & full_item
					else
						theList =full_item
					end if
				Case "TABLE_ALIAS_NAME"
					if theList <> "" then
						theList = theList & "," & lookup_table
					else
						theList =lookup_table
					end if
					
			End SElect
		next
		
		theReturn=theList
	else
		theReturn = ""
	end if
	getALLLookupInfo = theReturn
End function

Function getLookupColumnDisplayInfoForSelect(dbkey, formgroup, table_name, columnName_input)
	if Session("LOOKUP_TABLE" & table_name & dbkey & formgroup) <> "" then
	
		lookup_array_all = Split(Session("LOOKUP_TABLE" & table_name & dbkey & formgroup), ";;;", -1)
		bItemFound = false
		for i = 0 to UBound(lookup_array_all)
			on error resume next
				lookup_array_single = split(lookup_array_all(i), ":::", -1)
				lookup_info_array = split(lookup_array_single(1), ",", -1)
				LOOKUP_COLUNN_NAMEArray = split(lookup_info_array(3), ":", -1)
				Lookup_ColumnName  = LOOKUP_COLUNN_NAMEArray(1)
			
				if UCase(Lookup_ColumnName) = UCase(columnName_input) then
				
					bItemFound = true
					
					LOOKUP_TABLE_NAMEArray = split(lookup_info_array(0), ":", -1)
					Lookup_table_name = LOOKUP_TABLE_NAMEArray(0)
					LOOKUP_COLUMN_DISPLAY_NAMEArray = split(lookup_info_array(2), ":", -1)
					LOOKUP_COLUMN_DISPLAY_NAME = LOOKUP_COLUMN_DISPLAY_NAMEArray(0)
					Full_Item = lookup_table_name & "." & LOOKUP_COLUMN_DISPLAY_NAME
					exit for
				end if
		next
		if bItemFound = true then
			theReturn = Full_Item
		else
			theReturn=""
		end if
	else
		theReturn = ""
	end if
	
		
	getLookupColumnDisplayInfoForSelect = theReturn
End Function

Function getLookupInfoItem(dbkey, formgroup, table_name, lookup_table, itemName)
	if Session("LOOKUP_TABLE" & table_name & dbkey & formgroup) <> "" then
		lookup_array_all = Split(Session("LOOKUP_TABLE" & table_name & dbkey & formgroup), ";;;", -1)
		for i = 0 to UBound(lookup_array_all)
			on error resume next
			lookup_array_single = split(lookup_array_all(i), ":::", -1)
			lookup_info = lookup_array_single(1)
			if UCase(lookup_array_single(0)) = UCase(lookup_table) then
				lookup_info_array = split(lookup_array_single(1), ",", -1)
				Select Case UCase(itemName)
					case "LOOKUP_TABLE_NAME"
						
						lookup_info_array2 = split(lookup_info_array(0), ":", -1)
						theReturn = lookup_info_array2(0)
						
					case "LOOKUP_TABLE_ID"
						lookup_info_array2 = split(lookup_info_array(0), ":", -1)
						theReturn = lookup_info_array(1)
								
					case "LOOKUP_COLUMN_ID"
						lookup_info_array2 = split(lookup_info_array(1), ":", -1)
						theReturn = lookup_info_array2(1)
					case "LOOKUP_COLUMN_NAME"
						lookup_info_array2 = split(lookup_info_array(1), ":", -1)
						theReturn = lookup_info_array2(0)
					case "LOOKUP_COLUMN_DISPLAY_ID"
						lookup_info_array2 = split(lookup_info_array(2), ":", -1)
						theReturn = lookup_info_array2(1)
					case "LOOKUP_COLUMN_DISPLAY_NAME"
						lookup_info_array2 = split(lookup_info_array(2), ":", -1)
						theReturn = lookup_info_array2(0)
					Case "PARENT_TABLE_NAME"
						lookup_info_array2 = split(lookup_info_array(3), ":", -1)
						theReturn = lookup_info_array2(0)
					Case "PARENT_LINK_COLUMN_NAME"
						lookup_info_array2 = split(lookup_info_array(3), ":", -1)
						theReturn = lookup_info_array2(1)
					Case "PARENT_TABLE_ID"
						lookup_info_array2 = split(lookup_info_array(3), ":", -1)
						theReturn = lookup_info_array2(2)
					Case "PARENT_LINK_COLUMN_ID"
						lookup_info_array2 = split(lookup_info_array(3), ":", -1)
						theReturn = lookup_info_array2(3)
					Case "PARENT_LINK_COLUMN_IS_STRUCTURE"
						lookup_info_array2 = split(lookup_info_array(3), ":", -1)
						theReturn = lookup_info_array2(4)
					Case "LINK_DATATYPE"
						theReturn = lookup_info_array(4)
					
				End Select
			exit for
			end if
		next
		else
			theReturn = ""
		end if
		getLookupInfoItem = theReturn
End Function


Function getLookUpMOLID(dbkey, formgroup, parent_table_name, item_name)

	all_items = Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup)
	
	if all_items <> "" then
	lookup_array_all = Split(Session("LOOKUP_TABLE" & parent_table_name & dbkey & formgroup), ";;;", -1)
		for i = 0 to UBound(lookup_array_all)
			lookup_array_single = split(lookup_array_all(i), ":::", -1)
			lookup_table = lookup_array_single(0)
			
			PARENT_COLUMN_NAME = getLookupInfoItem(dbkey, formgroup, parent_table_name, lookup_table, "PARENT_LINK_COLUMN_NAME")
			if UCase(PARENT_COLUMN_NAME) = "MOL_ID" then
				Select Case(UCase(item_name))
					Case "LOOKUP_TABLE_NAME"
						ItemFound = lookup_table
					Case "LOOKUP_COLUMN_DISPLAY_NAME"
						ItemFound = getLookupInfoItem(dbkey, formgroup, parent_table_name, lookup_table, "LOOKUP_COLUMN_DISPLAY_NAME")
				End Select
				exit for
			end if
		Next
		if ItemFound <> "" then
			theReturn = ItemFound
		else
			theReturn=""
		end if
	else
		theReturn = ""
	end if
	getLookUpMOLID = theReturn
End Function


Function isInFormgroup(dbkey, formgroup, columnID, ByRef DataConn)
	bAllow = false
	
	sql = "select BIOSARDB.db_vw_formitems_all.column_id from " &_
		" BIOSARDB.db_vw_formitems_all where " &_
		" BIOSARDB.db_vw_formitems_all.formgroup_id=?" &_
		" AND BIOSARDB.db_vw_formitems_all.column_id=?" &_
		" AND BIOSARDB.db_vw_formitems_all.formtype_id IN(2,3)"
	
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  DataConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pformgroup", 5, 1, 0, formgroup) 
	cmd.Parameters.Append cmd.CreateParameter("pcolumnID", 5, 1, 0, columnID) 
	on error resume next
	Set RS = cmd.execute	
	

	
	If NOt (RS.EOF and RS.BOF) then
		bAllow = true
		RS.Close
	end if
	isInFormgroup = bAllow
	
End Function

Function isMoleculeInFormgroup(dbkey, formgroup,table_id, ByRef DataConn)
	bAllow = false
	
		sql = "select BIOSARDB.db_vw_formitems_all.column_id from " &_
		" BIOSARDB.db_vw_formitems_all where " &_
		" Upper(BIOSARDB.db_vw_formitems_all.disp_typ_name) IN('STRUCTURE','MOLWEIGHT','FORMULA') "&_
		" AND BIOSARDB.db_vw_formitems_all.formgroup_id=?"&_
		" AND BIOSARDB.db_vw_formitems_all.table_id=?" &_
		" AND BIOSARDB.db_vw_formitems_all.formtype_id IN(2,3)"
	
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  DataConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pformgroup", 5, 1, 0, formgroup) 
	cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
	on error resume next
	Set RS = cmd.execute	
	
	If NOt (RS.EOF and RS.BOF) then
	
		bAllow = true
		RS.Close
	end if
	isMoleculeInFormgroup = bAllow
	
End Function

Function getChildTableInfo(DataConn, parent_table_id,parent_column_id,child_table_id,child_column_id,join_type)
		'get parent table info
		
		bGetChildTables=true
			sql=getParentTableInfoFromParentTableID("?")
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  DataConn
			cmd.CommandType = adCmdText
			cmd.CommandText = sql
			cmd.Parameters.Append cmd.CreateParameter("parent_table_id", 5, 1, 0, parent_table_id) 
			
			
			Set RS2 = Server.CreateObject("adodb.recordset")
			on error resume next
				RS2.Open cmd
				if err.number <> 0 then
					Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
					Response.Write "<br><b>SQL ERROR: </b>"
					OutputSourceSQl  "getParentTableInfoFromParentTableID", sql
					
				end if
			on error goto 0
			Parent_Table_Name = RS2("TABLE_NAME")
			Parent_DataType =  RS2("DATATYPE")
			Parent_Precision = RS2("PRECISION")
			Parent_Scale=RS2("SCALE")
			RS2.Close
			
			'get parent column information
			sql = getParentColumnInfoFromParentColID("?")
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  DataConn
			cmd.CommandType = adCmdText
			cmd.CommandText = sql
			cmd.Parameters.Append cmd.CreateParameter("pparent_column_id", 5, 1, 0, parent_column_id) 
			
			
			Set RS2 = Server.CreateObject("adodb.recordset")
		
			on error resume next
				RS2.Open cmd
				if err.number <> 0 then
					Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
					Response.Write "<br><b>SQL ERROR: </b>"
					OutputSourceSQl  "getParentColumnInfoFromParentColID", sql
				end if
			on error goto 0
			Parent_Column_Name = RS2("COLUMN_NAME")
			RS2.Close
		
		
			' get child  info
			if bGetChildTables = True then
				'child table
				sql = getChildTableInfoFromChildTableID("?")
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("pchild_table_id", 5, 1, 0, child_table_id) 
				
				
				Set RS2 = Server.CreateObject("adodb.recordset")
			
				on error resume next
				RS2.Open cmd
				if err.number <> 0 then
					Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
					Response.Write "<br><b>SQL ERROR: </b>"
					OutputSourceSQl  "getChildTableInfoFromChildTableID", sql
				end if
				on error goto 0
				Child_Table_Name = RS2("TABLE_NAME")
				RS2.Close
				'child column
				sql = getChildColumnInfoFromChildColID("?")
			
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("pchild_column_id", 5, 1, 0, child_column_id) 
				
				
				Set RS2 = Server.CreateObject("adodb.recordset")
			
				on error resume next
				RS2.Open cmd
				if err.number <> 0 then
					Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
					Response.Write "<br><b>SQL ERROR: </b>"
					OutputSourceSQl  "getChildColumnInfoFromChildColID", sql
				end if
				on error goto 0
				Child_Column_Name = RS2("COLUMN_NAME")
				Child_DataType =  RS2("DATATYPE")
				Child_Precision = RS2("PRECISION")
				Child_Scale=RS2("SCALE")
				RS2.Close
			else
				Child_Table_Name = Parent_Table_Name
				Child_Column_Name = Parent_Column_Name
			end if
			if UCase(join_type) = "OUTER" then
				add_to_join = "(+)"
			else
				add_to_join = ""
			end if
			if bGetChildTables = True then
				Table_Real_Name = child_table_name
				SelectJoin_List = child_table_name & "." & child_column_name & "=" & parent_table_name & "." &  parent_column_name & add_to_join
				SelectLink_List = child_table_name & "."  & child_column_name & ";" & getCOWSDataType(child_datatype,child_precision,child_scale) & "," & parent_table_name & "." &  parent_column_name & ";" & getCOWSDataType(parent_datatype,parent_precision,parent_scale)
			else
				Table_Real_Name = parent_table_name
				SelectJoin_List =parent_table_name & "." &  parent_column_name & "=" & child_table_name & "." & child_column_name & add_to_join
				SelectLink_List = parent_table_name & "." &  parent_column_name & ";" & getCOWSDataType(parent_datatype,parent_precision,parent_scale) & "," &  child_table_name & "."  & child_column_name & ";" & getCOWSDataType(child_datatype,child_precision,child_scale)
			end if
		
		getChildTableInfo= Table_real_name & "|" & child_table_name & "|" & SelectJoin_List & "|" & SelectLink_List
End Function
			
Function getParentTableInfo(DataConn, parent_table_id,parent_column_id,child_table_id,child_column_id)
			Dim Parent_Table_Name,Parent_DataType,Parent_Precision,Parent_Scale,Child_Table_Name,Child_Column_Name
			'get parent table info
				Set RS3 = Server.CreateObject("ADODB.RECORDSET")
					
				sql = getTableName("?")
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("pparent_table_id", 5, 1, 0, parent_table_id) 
				
				
				Set RS3 = Server.CreateObject("adodb.recordset")
			
				on error resume next
				RS3.Open cmd
				
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getTableName", sql
					end if
				on error goto 0
				Parent_Table_Name = RS3("TABLE_NAME")
				RS3.Close
					
				sql = getParentColumnInfoFromParentColID(parent_column_id)
				on error resume next
					RS3.Open sql, DataConn
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getParentColumnInfoFromParentColID", sql
					end if
				on error goto 0
					
				Parent_DataType =  RS3("DATATYPE")
				Parent_Precision = RS3("PRECISION")
				Parent_Scale=RS3("SCALE")
				RS3.Close
				
				'get parent column information
				sql = getParentColumnInfoFromParentColID("?")
				
			
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  DataConn
				cmd.CommandType = adCmdText
				cmd.CommandText = sql
				cmd.Parameters.Append cmd.CreateParameter("pparent_column_id", 5, 1, 0, parent_column_id) 
				
				
				Set RS3 = Server.CreateObject("adodb.recordset")
			
				on error resume next
				RS3.Open cmd
				
				
					if err.number <> 0 then
						Response.write "<font color=""red"">ERROR: " & Err.number & ":" &  Err.Description  & "</font>"
						Response.Write "<br><b>SQL ERROR: </b>"
						OutputSourceSQl  "getParentColumnInfoFromParentColID", sql
					end if
				on error goto 0
				Parent_Column_Name = RS3("COLUMN_NAME")
				RS3.Close
				
				
				Child_Table_Name = Parent_Table_Name
				Child_Column_Name = Parent_Column_Name
					
									
				Table_Real_Name = parent_table_name
				SelectJoin_List =parent_table_name & "." &  parent_column_name & "=" & child_table_name & "." & child_column_name
				SelectLink_List = parent_table_name & "." &  parent_column_name & ";" & getCOWSDataType(parent_datatype,parent_precision,parent_scale) & "," &  child_table_name & "."  & child_column_name & ";" & getCOWSDataType(parent_datatype,parent_precision,parent_scale)
		getParentTableInfo= Table_real_name & "|" & Parent_Table_Name & "|" & SelectJoin_List & "|" & SelectLink_List
End Function
		
function checkSortList(current_list, new_item)
			theReturn = false
			itemFound = false
			temp_array = split(current_list, ",", -1)
			for i = 0 to Ubound(temp_array)
				if temp_array(i) = new_item then
					itemFound = true
				end if
			next
			if itemFound = true then
				theReturn = false
			else
				theReturn = true
			end if
		checkSortList =theReturn
end function
	
	
Function getJoinType(byRef DataConn, PARENT_LINK_COLUMN_ID)
	sql = "select biosardb.db_column.lookup_join_type from biosardb.db_column where column_id=?"
			
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  DataConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pPARENT_LINK_COLUMN_ID", 5, 1, 0, PARENT_LINK_COLUMN_ID) 

	on error resume next
	Set RS = Server.CreateObject("ADODB.Recordset")
	RS.Open cmd
	jointype = RS("lookup_join_type")
	RS.Close
	getJoinType = jointype
End Function

Function BioSARaddto_stringNoDup(the_list,new_item,delimiter)
	bExists = false
	if the_list <> "" then
		temp_array = split(the_list, ",", -1)
		for i = 0 to UBound(temp_array)
			if UCase(temp_array(i)) = UCase(new_item) then
				bExists = true
				exit for
			end if
		next
		if bExists = false then
			if the_list <> "" then
				the_new_List = the_list & delimiter & new_item
			else
				the_new_List =  new_item
			end if
		else
			the_new_List = the_list
		end if
	else
		the_new_list=new_item
	end if
	BioSARaddto_stringNoDup = the_new_list
End function


Function addtoSQLADDstring(the_list,new_item)
	bExists = false
	temp_array = split(the_list, ",", -1)
	for i = 0 to UBound(temp_array)
		if UCase(temp_array(i)) = UCase(new_item) then
			bExists = true
			exit for
		end if
	next
	if bExists = false then
		if the_new_List <> "" then
			the_new_List = the_new_List & "," & new_item
		else
			the_new_List =  new_item
		end if
	else
		the_new_List = the_list
	end if
	addtoSQLADDstring = the_new_list
End function


function BioSARfieldsforExport(field_alias_list, table_name)
	temp = split(field_alias_list, ",", -1)
	for i =0 to Ubound(temp)
		temp2=split(temp(i), ":;:", -1)
		if UBound(temp2)>0 then
			the_item = table_name & "." & temp2(0) & ":" & temp2(1)
			full_list = BioSARaddto_stringNoDup(full_list, the_item, ",")
		end if
	next
	BioSARfieldsforExport=full_list
End function
	
</script>