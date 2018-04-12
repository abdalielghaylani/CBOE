<SCRIPT LANGUAGE="vbscript" RUNAT="server">


'********************************************************
'EXPOSE TABLES AND SCHEMA ROUTINES
'********************************************************

Function ExposeTable(ByVal Name)
	' expose table and columns to user
	on error resume next
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	' first see if the table is just hidden (IS_EXPOSED = 'N')
	Dim oRSTable
	Set oRSTable = Server.CreateObject("ADODB.Recordset")
	Set oRSTable = oConn.Execute("select * from BIOSARDB.db_table where Upper(table_name) = '" & UCase(Name) & "'")
	if not (oRSTable.bof and oRSTable.eof) then
		lTableID= oRSTable("TABLE_ID")
		oConn.Execute "update BIOSARDB.db_table set is_exposed = 'Y' where Upper(table_name) = '" & UCase(Name) & "'"
	else
		Set oRSTable = Server.CreateObject("ADODB.Recordset")
		Set oRSTable = oConn.Execute( SQLDBTableByName(Name))
		if not (oRSTable.bof and oRSTable.eof) then
			Dim sOwner
			Dim sShortName, sDispName
			sOwner = UCase(oRSTable("OWNER"))
			sShortName = UCase(oRSTable("TABLE_NAME"))
		else
			temp=split(Name, ".", -1)
			sOwner = UCase(temp(0))
			sShortName = UCase(temp(1))
		end if
		checkDBSchema sOwner
		sDispName = FormatPrettify(sShortName)
		Dim oRSNew
		Set oRSNew = Server.CreateObject("ADODB.Recordset")
		oRSNew.Open SQLExposedTableByName(Name), oConn, 1, 3
		oRSNew.AddNew
		oRSNew("OWNER") = UCase(sOwner)
		oRSNew("TABLE_NAME") = UCase(sOwner & "." & sShortName)
		oRSNew("TABLE_SHORT_NAME") = UCase(sShortName)
		oRSNew("DISPLAY_NAME") = sDispName
		oRSNew("DESCRIPTION") = sDispName
		oRSNew("IS_VIEW") = "N"
		oRSNew("IS_EXPOSED") = "Y"
		oRSNew.Update
		Dim lTableId
	
		lTableId = oRSNew("TABLE_ID")
		oRSNew.Close
		Dim oRSCols
	
		sql=SQLDBColumnByTableName(Name)
		Set oRSCols = Server.CreateObject("ADODB.Recordset")
		
		oRSCols.Open sql, oConn, 1, 3
		Dim oRSNewCols, oRSFK
		Set oRSNewCols = Server.CreateObject("ADODB.Recordset")
		oRSNewCols.Open SQLColumnsByTableId(lTableId), oConn, 1, 3
		counter = 0
		if Not (oRSCols.eof and oRSCols.bof) then
			oRSCols.MoveFirst
			Do until oRSCols.eof
				sDispname = formatprettify(orscols("column_name"))
				oRSNewCols.AddNew
				oRSNewCols("TABLE_ID") = lTableId
				oRSNewCols("COLUMN_NAME") = UCase(oRSCols("COLUMN_NAME"))
				oRSNewCols("DISPLAY_NAME") = sDispname
				oRSNewCols("DESCRIPTION") = sDispname
				
				oRSNewCols("DATATYPE") = oRSCols("DATA_TYPE")
				oRSNewCols("DEFAULT_COLUMN_ORDER") = counter
				
				index_type = setIndexType(oConn,oRSCols("DATA_TYPE").value,sOwner,sShortName, oRSCols("COLUMN_NAME").value)
					
				if not isNull(index_type) then
					select case index_type
						case "0" 'unknown
							content_type = 0
						case "1" 'no_index blob
							'at some point we could do some detection but for now we won't
							content_type = 0
						case "2" 'cartridge clob
							'try and look at a single clob field and detect the content
							content_type = detectCartridgeContentType(oConn,sOwner,sShortName, oRSCols("COLUMN_NAME").value)
						case "3" 'no_index Clob
							'at some point we could do some detection but for now we won't
							content_type = 0
						case "4" 'cartridge blob
							'there is only one possiblility - chemical/x-cdx
							content_type = 7
						case else
							content_type = 0
					end select
				else
					content_type = NULL
				end if
				oRSNewCols("IS_VISIBLE") = "Y"
				oRSNewCols("INDEX_TYPE_ID")= index_type
				oRSNewCols("CONTENT_TYPE_ID")= content_type
				oRSNewCols("SCALE") = oRSCols("DATA_SCALE")
				oRSNewCols("LENGTH") = oRSCols("DATA_LENGTH")
				oRSNewCols("PRECISION") = oRSCols("DATA_PRECISION")
				oRSNewCols("NULLABLE") = oRSCols("NULLABLE")
				counter = counter + 10
				orscols.movenext
			loop
			oRSNewCols.Update
			oRSCols.Close
			oRSNewCols.Close
		
		' look for the primary key information and add if found
		Dim oRSPK
		Set oRSPK = oConn.Execute(SQLPrimaryKeyByTableName(Name))
		
		if HasRecords(oRSPK) then
			dim lBaseId, oRSCol
			Set oRScol = oConn.Execute(SQLColumnByTableIdAndColumnName(lTableId, oRSPK.Fields("COLUMN_NAME")))
			lBaseId = oRSCol.Fields("COLUMN_ID")
			oRSNew.Open SQLExposedTableById(lTableId), oConn, 1, 3
			oRSNew("BASE_COLUMN_ID") = lBaseId
			
			oRSNew.Update
			oRSNew.Close
		end if
		else
		'there are no columsn, delete the table entry
		
		oConn.Execute "delete from BIOSARDB.db_table where table_id='" & lTableID & "'"
		oConn.Execute "commit"
		lTableID="NO_TABLE"
		end if
	end if
	if Not lTableID = "NO_TABLE" then
		AddTableToRootNode ltableID
	end if
	ExposeTable=lTableID
end function


function ExposeView(ByVal Name)
	

on error resume next
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	' first see if the table is just hidden (IS_EXPOSED = 'N')
	Dim oRSTable
	Set oRSTable = oConn.Execute("select * from BIOSARDB.db_table where  Upper(table_name) = '" & UCase(Name) & "'")
	if not oRSTable.eof then
		lTableID= oRSTable("TABLE_ID")
		oConn.Execute "update BIOSARDB.db_table set is_exposed = 'Y' where  Upper(table_name) = '" &  UCase(Name) & "'"
	else
		' expose view to user
		Dim oRSView
		Set oRSView = oConn.Execute(SQLDBViewByName(Name))
		Dim sOwner
		Dim sShortName, sDispName
		if not (oRSView.bof and oRSView.eof) then
			sOwner = UCase(oRSView("OWNER"))
			sShortName = UCase(oRSView("VIEW_NAME"))
		else
			temp=split(Name, ".", -1)
			sOwner = UCase(temp(0))
			sShortName = UCase(temp(1))
		end if
		checkDBSchema sOwner
		sDispName = FormatPrettify(sShortName)
		Dim oRSNew
		Set oRSNew = Server.CreateObject("ADODB.Recordset")
		oRSNew.Open SQLExposedTableByName(Name), oConn, 1, 3
		oRSNew.AddNew
		oRSNew("OWNER") = UCase(sOwner)
		oRSNew("TABLE_NAME") = UCase(sOwner & "." & sShortName)
		oRSNew("TABLE_SHORT_NAME") = UCase(sShortName)
		oRSNew("DISPLAY_NAME") = sDispName
		oRSNew("DESCRIPTION") = sDispName
		oRSNew("IS_VIEW") = "Y"
		oRSNew("IS_EXPOSED") = "Y"
		oRSNew.Update
		Dim lTableId
		lTableId = oRSNew("TABLE_ID")
		oRSNew.Close
		Dim oRSCols
		
		Set oRSCols = oConn.Execute(SQLDBColumnByTableName(Name))
		Dim oRSNewCols, oRSFK
		Set oRSNewCols = Server.CreateObject("ADODB.Recordset")
		
		oRSNewCols.Open SQLColumnsByTableId(lTableId), oConn, 1, 3
		counter = 0
		if not (oRSCols.BOF and oRSCols.EOF)then
			Do until oRSCols.eof
				sDispname = formatprettify(orscols("column_name"))
				oRSNewCols.AddNew
				oRSNewCols("TABLE_ID") = lTableId
				oRSNewCols("COLUMN_NAME") = UCase(oRSCols("COLUMN_NAME"))
				oRSNewCols("DISPLAY_NAME") = sDispname
				oRSNewCols("DESCRIPTION") = sDispname
				oRSNewCols("IS_VISIBLE") = "Y"
				oRSNewCols("DATATYPE") = oRSCols("DATA_TYPE")
				oRSNewCols("SCALE") = oRSCols("DATA_SCALE")
				oRSNewCols("LENGTH") = oRSCols("DATA_LENGTH")
				oRSNewCols("PRECISION") = oRSCols("DATA_PRECISION")
				oRSNewCols("NULLABLE") = oRSCols("NULLABLE")
				oRSNewCols("DEFAULT_COLUMN_ORDER") = counter
				counter = counter + 10
				
				index_type = setIndexType(oconn,oRSCols("DATA_TYPE").value,sOwner,sShortName, oRSCols("COLUMN_NAME").value)
			
				if not isNull(index_type) then
					select case index_type
					
						case "0" 'unknown
							content_type = 0
						case "1" 'no_index blob
							'at some point we could do some detection but for now we won't
							content_type = 0
						case "2" 'cartridge clob
							'try and look at a single clob field and detect the content
							content_type = detectCartridgeContentType(oConn,sOwner,sShortName, oRSCols("COLUMN_NAME").value)
						case "3" 'no_index Clob
							'at some point we could do some detection but for now we won't
							content_type = 0
						case "4" 'cartridge blob
							'there is only one possiblility - chemical/x-cdx
							content_type = 7
						case else
							content_type = 0
					end select
				else
					content_type = NULL
				end if
				
				oRSNewCols("INDEX_TYPE_ID")= index_type
				oRSNewCols("CONTENT_TYPE_ID")= content_type
				orscols.movenext
			loop
		oRSNewCols.Update
		oRSNewCols.Close
		else
			'there are no columns for the view, delete the table entry
			oConn.Execute "delete from BIOSARDB.db_table where table_id='" & lTableID & "'"
			oConn.Execute "commit"
			lTableID="NO_TABLE"
		end if
	end if
	
	if Not lTableID = "NO_TABLE" then
		AddTableToRootNode ltableID
	end if
	ExposeView=lTableID
End function



function UnexposeTable(ByVal Name)
	' unexpose table and related data (of which there is a lot)
	Dim lTableId
	Dim oRSTemp
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Set oRSTemp = oConn.execute(SQLExposedTableByFullName(Name))		
 	lTableId = oRSTemp("TABLE_ID")
 	oConn.Execute("update BIOSARDB.db_table set is_exposed = 'N' where table_id = " & lTableId)
	RemoveTableToRootNode ltableID
	if Application("DELETE_METADATA_WHEN_UNEXPOSING_TABLE_OR_VIEW") = 1 then
		Set oRSFG= Server.CreateObject("ADODB.Recordset")
		Set cmd= Server.CreateObject("ADODB.Command")
		cmd.CommandType=adCmdText
		cmd.ActiveConnection = oConn
		sql ="Select formgroup_id from biosardb.db_formgroup_tables where table_id = ? and table_order >1"
		cmd.Parameters.Append cmd.CreateParameter("ptable_name", 139, 1, 0, lTableId) 
		cmd.CommandText=sql
		oRSFG.open cmd
		if not (oRSFG.BOF and oRSFG.EOF) then
			do while not oRSFG.EOF
				formgroup_id = oRSFG("formgroup_id")
				dbkey = "biosar_browser"
				oConn.Execute("Delete from  biosardb.db_xml_templ_def where formgroup_id = " & formgroup_id)'deletes are templates so they will be regenerated
				Application.Lock
					Application("FORM_GROUP" & dbkey & formgroup_id)= "" 'force reloading of formgroup
				Application.UnLock
				oRSFG.MoveNext
			loop
			oRSFG.Close
			'force refresh of tree for current user. Other users will not see the change until they log out and in again.
			RefreshFormTreeSessVariable()
		end if
		
 		oConn.Execute(SQLDeleteFormgroupTablesByTableId(lTableId))
		oConn.Execute(SQLDeleteRelationshipsByTableId(lTableId))
		oConn.Execute(SQLDeleteFormItemsByTableId(lTableId))
		oConn.Execute(SQLDeleteColumnsByTableId(lTableId))
		Set oRSTemp = oConn.Execute(SQLFormgroupsByBaseTableId(lTableId))
		if HasRecords(oRSTemp) Then
			formgroup_id = oRSTemp("formgroup_id")
			dbkey = "biosar_browser"
			oConn.Execute("Delete from  biosardb.db_xml_templ_def where formgroup_id = " & formgroup_id) 'deletes are templates
			Application.Lock
				Application("FORM_GROUP" & dbkey & formgroup_id)= "" 'force reloading of formgroup
			Application.UnLock
			oConn.Execute("Delete from biosardb.tree_item where item_type_id =2 and item_id = " & formgroup_id) 'deletes form from all user and public trees
			oConn.Execute(SQLDeleteFormsByBaseTableId(lTableId))
			oConn.Execute(SQLDeleteFormgroupsByBaseTableId(lTableId))
		end if
		oConn.Execute(SQLDeleteTableById(lTableId))
	end if
End Function

Sub ExposeSchema(ByVal Name)
	on error resume next
	Dim oRSSchema
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Set oRSSchema = Server.CreateObject("ADODB.Recordset")
	
	oRSSchema.Open SQLSchemaByName(Name), oConn, 1, 3
	oRSSchema.AddNew
	oRSSchema("OWNER") = Name
	oRSSchema("DISPLAY_NAME") = Name
	oRSSchema.Update
	' when adding a schema, we add all tables by default
	Dim oRSTables
	Set oRSTables = oConn.Execute(SQLAllTablesByOwner(Name))
	Do until oRSTables.eof
		ExposeTable Name & "." & oRSTables.Fields("TABLE_NAME")
		oRSTables.MoveNext
	Loop
	oRSTables.Close
	' and all views
	Set oRSTables = oConn.Execute(SQLAllViewsByOwner(Name))
	do until oRSTables.eof
		ExposeView Name & "." & oRSTables.Fields("VIEW_NAME")
		oRSTables.movenext
	loop
	oRSTables.Close
	setContraints(Name)
	exit sub
end Sub


Sub setContraints(Name)
' set up foreign key constraints
	' loop over all tables in schema
	on error resume next
	dim sql
	set cmd = server.CreateObject("ADODB.command")
	Set oRSTables = Server.CreateObject("ADODB.Recordset")
	cmd.CommandType=adCmdText
	cmd.ActiveConnection = oConn
	sql ="select TABLE_ID,TABLE_NAME from BIOSARDB.db_table where is_exposed = 'Y' and  is_view = 'N' and owner =?"
	cmd.Parameters.Append cmd.CreateParameter("ptable_ownser", 200, 1, Len(Name) + 1, Name) 	
	cmd.CommandText=sql
	oRSTables.open cmd			

	dim oRSCols, oRSKids, oRSparent, oRSFK
	dim table_id, column_name, table_name
	
	Do while not oRSTables.eof
		table_id = ""
		table_name = ""
		column_name = ""
		table_id = oRSTables("TABLE_ID")
		table_name =oRSTables("TABLE_NAME")
		set cmd = server.CreateObject("ADODB.command")
		Set oRSCols = Server.CreateObject("ADODB.Recordset")
		cmd.CommandType=adCmdText
		cmd.ActiveConnection = oConn
		sql = "select COLUMN_ID,COLUMN_NAME from BIOSARDB.db_column where table_id = ?" 
		cmd.Parameters.Append cmd.CreateParameter("ptable_name", 139, 1, 0, table_id) 
		cmd.CommandText=sql
		oRSCols.open cmd
	
		
		' loop over all columns in table
		
		Do while not oRSCols.eof
			column_id = ""
			column_id = oRSCols("COLUMN_ID")
			column_name = oRSCols("COLUMN_NAME")
			sql = ""
			Dim aAr
			aAr = Split(table_name, ".")
			sOwner = aAr(0)
			sTable = aAr(1)
			sql = "select TABLE_NAME,COLUMN_NAME from BIOSARDB.db_vw_col_constraints" & _
							   " where constraint_name in " & _
							   "(select r_constraint_name from BIOSARDB.db_vw_col_constraints" & _
							   " where table_name = ? and owner = ?" & _
							   " and constraint_type = 'R' and column_name =?)"
			
			set cmd = server.CreateObject("ADODB.command")
			Set oRSFK = Server.CreateObject("ADODB.Recordset")
			cmd.CommandType=adCmdText
			cmd.ActiveConnection = oConn
			cmd.Parameters.Append cmd.CreateParameter("ptable_name", 200, 1, Len(sTable) + 1, sTable) 
			cmd.Parameters.Append cmd.CreateParameter("ptable_ownser", 200, 1, Len(sOwner) + 1, sOwner) 
			cmd.Parameters.Append cmd.CreateParameter("pcolumn_name", 200, 1, Len(column_name) +1, CStr(column_name)) 
			cmd.CommandText=sql
			oRSFK.open cmd
			' get foreign (parent) constraints
			If not (oRSFK.BOF and oRSFK.EOF) THen
				do while not oRSFK.EOF
					sql = ""
					oTable_name = oRSFK("TABLE_NAME")
					oColumnName = oRSFK("COLUMN_NAME")
					oRtablename = Name & "." & oTable_name
					
					sql =  "select table_id, column_id from BIOSARDB.db_vw_column_table where  Upper(table_name) = ? and Upper(column_name) = ?"
					set cmd = server.CreateObject("ADODB.command")
					Set oRSParent = Server.CreateObject("ADODB.Recordset")
					cmd.CommandType=adCmdText
					cmd.ActiveConnection = oConn
					cmd.Parameters.Append cmd.CreateParameter("ptable_name", 200, 1, Len(oRtablename) + 1, UCase(oRtablename)) 
					cmd.Parameters.Append cmd.CreateParameter("oColumnName", 200, 1, Len(oColumnName) + 1, UCase(oColumnName)) 
					cmd.CommandText=sql
					oRSParent.Open cmd
					ptable_id = oRSParent("TABLE_ID")
					pcolumn_id=oRSParent("COLUMN_ID")
					
					oRSParent.close
					childTableID= table_id
					childColId = column_id
					
					on error resume next
					set cmd = server.CreateObject("ADODB.command")
					cmd.CommandType=adCmdText
					cmd.ActiveConnection = oConn
					sql = "INSERT INTO BIOSARDB.db_relationship (TABLE_ID, COLUMN_ID, CHILD_TABLE_ID,CHILD_COLUMN_ID,JOIN_TYPE)Values(?,?,?,?,'OUTER')"
					cmd.CommandText = sql
					cmd.Parameters.Append cmd.CreateParameter("ptable_id", 139, 1, 0, ptable_id) 
					cmd.Parameters.Append cmd.CreateParameter("pcolumn_id", 139, 1,0,pcolumn_id )
					cmd.Parameters.Append cmd.CreateParameter("childTable", 139, 1,0,childTableID )
					cmd.Parameters.Append cmd.CreateParameter("childColId", 139, 1,0,childColId )
					cmd.Execute
					oRSFK.MoveNext
				loop
				oRSFK.close
			end if
			oRSCols.movenext
		Loop
		oRSCols.close
		oRSTables.movenext
	Loop
	oRSTables.close
End Sub

Sub UnexposeSchema(ByVal Name)
	on error resume next
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	oConn.Execute SQLDeleteSchemaByName(Name)
	' delete all tables in schema
	Dim oRSTables 
	Set oRSTables = oConn.Execute(SQLExposedTablesAndViewsByOwner(Name))
	Do until oRSTables.eof
		UnexposeTable oRSTables.Fields("TABLE_NAME")
		orsTables.movenext
	loop
End Sub

Function isCartridgeIndex(owner,table_name,column)
	
	Dim oRSCart
	dim sql
	sql = SQLisIndexedbyCartridge(owner,table_name, column)
	Set oRSCart = oConn.Execute(sql)
	if Not (oRSCart.bof and ORSCart.eof) then
		isCartridgeIndex = true
		oRSCart.close
	else
		isCartridgeIndex = false
	end if
	
End function

function getINDEXTypeID(index_name, datatype)
	Dim oRSindx
	
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Set oRSindx = oConn.Execute(SQLgetIndexTypeIDbyName(index_name, datatype))
	if Not (oRSindx.bof and oRSindx.eof) then
		theID = oRSindx("index_type_id")
	
		oRSindx.close
	else
		theID=1
	
	end if
	getINDEXTypeID=theID

End function

Function setIndexType(byRef oConn,byval datatype,byval owner,byval table_name, byval column)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Select Case UCase(datatype)
	Case "CLOB"
	
		if isCartridgeIndex(owner,table_name,column) then
			idxID= getINDEXTypeID("CS_CARTRIDGE",datatype)
		else
			idxID= getINDEXTypeID("NO_INDEX",datatype)
		end if 
	Case "BLOB"
		if isCartridgeIndex(owner,table_name,column) then
			idxID= getINDEXTypeID("CS_CARTRIDGE",datatype)
		else
			idxID= getINDEXTypeID("NO_INDEX",datatype)
		end if 
	Case Else
		idxID = NULL
	end select
	
	setIndexType = idxID
end function


Function setContentType()
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	sql = SQLgetUNKContentTypes()
	
	dim rs
	set rs = oConn.Execute(sql)
	If not (rs.EOF and rs.BOF) then
		theReturn = rs("content_type_id")
	else
		theReturn = NULL
	end if 
end function

function detectCartridgeContentType(byRef oConn,schema,table, column_name)

	Dim theReturn
	theReturn = ""
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	set RS = server.CreateObject("adodb.recordset")
	sql = "select " & column_name & " from " & schema & "." & table & " where rownum=1"
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	RS.open cmd
	if Not( rs.eof and rs.bof) then
		theValue = RS(column_name)
		If left(theValue, 3) = "Vmp" then
			theReturn=5
		else
			if instr(theValue, "<CDXML")>0 and instr(theValue, "<?xml")>0 then
				theReturn=9
			else 
			
				
				If left(theValue, 9) = chr(10) & "ChemDraw" or left(theValue, 8) = "ChemDraw" or left(theValue, 9) = " ChemDraw" or instr(left(theValue,20), "ChemDraw")>0 then
					theReturn = 6
				else
					theReturn = 0
				end if
			end if
		end if
		RS.close
	else 
		theReturn=0
	end if
	detectCartridgeContentType=theReturn
end function


Function doExposeTables()
			if not isObject(oConn) then
				Set oConn = SysConnection
			end if
			Dim aTableArr
			Dim oExposedTables
			Dim oRSExposed
			Dim vTableName

			Dim sDesc
			on error resume next
			
			' find out which tables to remove and add
			Set oExposedTables = Server.CreateObject("Scripting.Dictionary")
			Set oRSExposed = oConn.execute(SQLExposedTablesByOwner(sOwner))
			Set oExposedTables = DictFromRS(oRSExposed, "TABLE_NAME", "", "old")
			For Each vTableName in request("table_names")
				if oExposedTables.Exists(CStr(vTableName)) Then
					oExposedTables.Item(vTableName) = "keep"
				Else
					oExposedTables.Add vTableName, "new"
				End If
			Next
			For Each vTableName in oExposedTables.Keys
				If oExposedTables.Item(vTableName) = "old" Then
					UnexposeTable vTableName
				ElseIf oExposedTables.Item(vTableName) = "new" then
					ExposeTable vTableName
				End if
			Next
			' now do the same for views
			oExposedTables.RemoveAll
			Set oRSExposed = oConn.eXecute(SQLExposedViewsByOwner(sOwner))
			Do until oRSExposed.eof
				oExposedTables.Add CSTr(oRSExposed("TABLE_NAME")), "old"
				oRSExposed.movenext
			loop
			For Each vTableName in request("view_names")
				if oExposedTables.Exists(CStr(vTableName)) Then
					oExposedTables.Item(vTableName) = "keep"
				Else
					oExposedTables.Add vTableName, "new"
				End If
			Next
			For Each vTableName in oExposedTables.Keys
				If oExposedTables.Item(vTableName) = "old" Then
					UnexposeTable vTableName
				ElseIf oExposedTables.Item(vTableName) = "new" then
					
					ExposeView vTableName
				End if
			Next
	End function
	
	function doEditTable(lTableId,table_display_name,primary_key)
	
		' apply changes to table
		Dim oRSModTable
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		on error resume next
		sql = SQLExposedTableById(lTableId)
		Set oRSModTable = Server.CreateObject("ADODB.Recordset")
		oRSModTable.Open  sql, oConn,adOpenStatic,adLockOptimistic
		oRSModTable("DISPLAY_NAME") = table_display_name
		oRSModTable("DESCRIPTION") = request("table_description")
		'oRSModTable("BASE_COLUMN_ID") = primary_key
		oRSModTable.Update
		oRSModTable.Close
		' apply changes to columns
	
	End Function
	
	function doEditPrimaryKey(lTableId,primary_key)
	
		' apply changes to table
		Dim oRSModTable
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		on error resume next
		sql = SQLExposedTableById(lTableId)
		Set oRSModTable = Server.CreateObject("ADODB.Recordset")
		oRSModTable.Open  sql, oConn,adOpenStatic,adLockOptimistic
		oRSModTable("BASE_COLUMN_ID") = primary_key
		oRSModTable.Update
		oRSModTable.Close
		' apply changes to columns
	
	End Function
	
	
	function doEditColumn(lTableId,lColID,column_is_visible,column_order,column_display_name,column_description)
		' apply changes to table

		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		Dim oRSModCol
		Set oRSModCol = Server.CreateObject("ADODB.Recordset")
		oRSModCol.Open SQLColumnsByTableIdANDColID(lTableId,lColID), oConn, 1, 3
		
		if Not (oRSModCol.BOF and oRSModCol.EOF) then
			oRSModCol("IS_VISIBLE") = Check2DBBool(column_is_visible)
			oRSModCol("DEFAULT_COLUMN_ORDER") = column_order
			oRSModCol("DISPLAY_NAME") = column_display_name 
			oRSModCol("DESCRIPTION") =column_description 
		end if
		oRSModCol.Update
		oRSModCol.close
		'redisplay table list
		
	End Function

function doExposeSchemas()
		on error resume next
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		dim vSchemaName
		Dim oExposedSchemas
		' display busy dialog
		FlushImageToClient Application("ANIMATED_GIF_PATH")
		FlushMessageToClient "Scanning tables for relationships and primary keys.  This may take several minutes, please wait..."
		
		' find out which schemas to remove and add
		Set oRSExposed = oConn.Execute (SQLAllExposedSchemas)
		Set oExposedSchemas = DictFromRS(oRSExposed, "OWNER", "", "old")
		For Each vSchemaName in Request("schema_names")
			if oEXposedSchemas.Exists(CStr(vSchemaName)) then
				oExposedSchemas.Item(vSchemaName) = "keep"
			Else
				oExposedSchemas.Add vSchemaName, "new"
			End If
		Next
		' now add and remove scheomas
		For Each vSchemaName in oExposedSchemas.Keys
			If oExposedSchemas.Item(vSchemaName) = "old" Then
				UnexposeSchema vSchemaName
			ElseIf oExposedSchemas.Item(vSchemaName) = "new" then
				ExposeSchema vSchemaName
			End if
		Next
End Function

function RefreshTable(ByVal TableId)
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	' unexpose table and reexpose it, returning new table id
	Dim lTableId, sIsView
	Dim oRSTemp
	lTableID = TableId
	Set oRSTemp = oConn.Execute("Select * from BIOSARDB.db_table where table_id = " & ltableId)
 	Name = oRSTemp("TABLE_NAME")
 	sIsView = oRSTemp("IS_VIEW")
 	oConn.Execute(SQLDeleteFormgroupTablesByTableId(lTableId))
	oConn.Execute(SQLDeleteRelationshipsByTableId(lTableId))
	oConn.Execute(SQLDeleteFormItemsByTableId(lTableId))
	oConn.Execute(SQLDeleteColumnsByTableId(lTableId))
	' delete lookup table rels
	oConn.Execute "update BIOSARDB.db_column set lookup_table_id = null, lookup_column_id = null, lookup_column_display = null " & _
				  " where lookup_table_id = " & TableId
	Set oRSTemp = oConn.Execute(SQLFormgroupsByBaseTableId(lTableId))
	if HasRecords(oRSTemp) Then
		oConn.Execute(SQLDeleteFormsByBaseTableId(lTableId))
		oConn.Execute(SQLDeleteFormgroupsByBaseTableId(lTableId))
	end if
	oConn.Execute(SQLDeleteTableById(lTableId))
	' now add it back in
	if sISView = "Y" then
		ExposeView Name
	else
		ExposeTable Name
	end if
	set orstemp = oconn.execute("select table_id from BIOSARDB.db_table where Upper(table_name) = '" & UCase(Name) & "'")
	if not (orstemp.bof and orstemp.eof) then
		RefreshTable = orsTemp("TABLE_ID")
	else
		RefreshTable = "table no longer exists"
	end if
End Function


Function doEditRels(lChildID, lParentColumnID,nParentColumnID,join_type)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Dim oRSKids
	' find out which relations already exist
	Set oRSkids = Server.CreateObject("ADODB.Recordset")
	oRSKids.Open SQLRelationshipsByChildColumnId(lChildId), oconn, 1, 3
	dim oRSt
	if lParentColumnId <> "" then
	Set oRST = oConn.Execute (SQLColumnTableById(lParentColumnId))
		parentTableID = oRST("TABLE_ID")
	else
		exit function
	end if
	Set cRST = oConn.Execute (SQLColumnTableById(lChildId))
	lchildTableID = cRST("TABLE_ID")
	if nParentColumnId = "-1" then
		oConn.Execute SQLDeleteFromRelationshipByChildColumnID(lchildId)
		UpdateChildInfoForFormgroups lparentColumnID, lchildTableID 
	else
		
		if not HasRecords(oRSKids) then
			oRSKids.AddNew
			bAddNew = true
		else
			bAddNew = false
		end if
		oRSKids("TABLE_ID") = parentTableID
		oRSKids("COLUMN_ID")= lParentColumnId
		oRSKIDS("CHILD_TABLE_ID") = lchildTableID
		oRSKids("CHILD_COLUMN_ID") = lChildId
		oRSKids("JOIN_TYPE") = join_type

		oRSKids.Update
		oRSKids.Close
		if bAddNew = false then
			UpdateFormgroupTemplates  lparentColumnID, lchildTableID 
		end if
	end if
	
End Function

Function doEditContentType(lContentTypeId,lBaseId,lTableId)
	
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	lId = CnvLong(lContentTypeId, "VB_TO_DB")
	
	
	Set oRSModify = Server.CreateObject("ADODB.Recordset")
	oRSModify.Open SQLColumnById(lBaseId), oConn, 1, 3
	oRSModify("CONTENT_TYPE_ID") = lId
	oRSModify.Update
	oRSModify.Close
	UpdateFormgroupTemplatesByColID lBaseId
	
End function 

Function doEditIndexType(lIndexTypeId,lBaseId,lTableId)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	
	
	Set oRSModify = Server.CreateObject("ADODB.Recordset")
	oRSModify.Open SQLColumnById(lBaseId), oConn, 1, 3
	oRSModify("INDEX_TYPE_ID") = lIndexTypeId
	oRSModify.Update
	oRSModify.Close
	UpdateFormgroupTemplatesByColID lBaseId
	
End function 


Function EditLookup(lColId,nlookup_id_col,lookup_join,lookup_sort,lookup_id_col,lookup_id_val)

	Dim lId
	dim lval
	dim lTbId
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if

	if (lookup_id_col <> "" and lookup_id_val  <> "") and Not (instr(UCase(lookup_id_col), "SELECT")> 0 or instr(UCase(lookup_id_val), "SELECT")> 0 )  Then
		If nlookup_id_col = "-1" then
			lTbId = Null
			lId = NULL 
			lVal = NULL
			lookup_join = NULL
			lookup_sort = NULL
		else
			lId = lookup_id_col
			lVal = lookup_id_val
			
			Set oRSTable = oConn.Execute(SQLColumnTableById(lId))
			lTbId = oRSTable("TABLE_ID")
		end if
		Dim oRSModify
		Set oRSModify = Server.CreateObject("ADODB.Recordset")
		oRSModify.Open SQLColumnById(lColId), oConn, 1, 3
		oRSModify("LOOKUP_COLUMN_ID") = lId
		oRSModify("LOOKUP_COLUMN_DISPLAY") = lVal
		oRSModify("LOOKUP_TABLE_ID") = lTbId
		oRSModify("LOOKUP_JOIN_TYPE") =lookup_join 
		oRSModify("LOOKUP_SORT_DIRECT") = lookup_sort
		
		oRSModify.Update
		oRSModify.Close
		UpdateFormgroupTemplatesByColID lColId
	end if

End function 


Function getColIDFromName(table_id, column_name)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	sql = "select column_id from biosardb.db_column where table_id=? and column_name =?"
	set cmd = server.CreateObject("ADODB.command")
	set RS = server.CreateObject("ADODB.Recordset")
	cmd.CommandType=adCmdText
	cmd.ActiveConnection = oConn
	cmd.Parameters.Append cmd.CreateParameter("ptable_id", 5, 1, 0, table_id) 
	cmd.Parameters.Append cmd.CreateParameter("pcolumn_name", 200, 1, Len(column_name) +1, CStr(UCase(column_name))) 
	cmd.CommandText=sql
	RS.Open cmd
	if Not (RS.BOF and RS.EOF) then
		ColID = rs("COLUMN_ID")
		RS.Close
		Set cmd = nothing
	else
		ColID=""
	end if
	
	getColIDFromName=ColID

End function

Function getTableIDFromName(owner, table_name)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	sql = "select table_id from biosardb.db_table  where owner=? and table_short_name =?"
	set cmd = server.CreateObject("ADODB.command")
	set RS = server.CreateObject("ADODB.Recordset")
	cmd.CommandType=adCmdText
	cmd.ActiveConnection = oConn
	cmd.Parameters.Append cmd.CreateParameter("pschema", 200, 1, Len(owner) +1, CStr(UCase(owner))) 
	cmd.Parameters.Append cmd.CreateParameter("pcolumn_name", 200, 1, Len(table_name) +1, CStr(UCase(table_name))) 
	cmd.CommandText=sql
	RS.Open cmd
	if Not (RS.BOF and RS.EOF) then
		table_id = rs("table_id")
		RS.Close
		Set cmd = nothing
	else
		table_id=""
	end if
	
	getTableIDFromName=table_id

End function


Sub RefreshSchemas()

on error resume next
	' Open the connection
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = oConn
	cmd.CommandText = "GRANT SELECT ANY TABLE to BIOSARDB"
	Cmd.CommandType = adCmdText
	Cmd.Execute
	Cmd.CommandText = Application("BIOSAR_BROWSER_USERNAME") & ".ValidateAndRepairSchema.UpdateOutOfSyncTableColumns"
	Cmd.CommandType = adCmdStoredProc
	Cmd.Execute
	Cmd.CommandText = Application("BIOSAR_BROWSER_USERNAME") & ".ValidateAndRepairSchema.InsertMissingTableColumns"
	Cmd.CommandType = adCmdStoredProc
	Cmd.Execute
	cmd.CommandText = "REVOKE SELECT ANY TABLE FROM BIOSARDB"
	Cmd.CommandType = adCmdText
	Cmd.Execute
End sub

Sub UpdateChildInfoForFormgroups(lParentColumnId, childtableID)
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Dim cRST
	Set cRST = oConn.Execute ("select distinct formgroup_id from BIOSARDB.db_vw_formgroup_tables where table_id =" & childtableID & " AND table_order >1")
	if Not (cRSt.BOF and cRSt.EOF) then
		do while not cRST.EOF 
			formgroupid = cRST("formgroup_id")
			oConn.execute SQLDeleteFormgroupTablesByPK(childtableID, formgroupid)
			oConn.execute SQLDeleteFormItemsByFormGroupIdAndTableID(childtableID, formgroupid)
			CreateXMLTemplates(formgroupid)
			cRST.MoveNext
		Loop
	end if
	cRST.close
End Sub


Sub UpdateFormgroupTemplates(lparentColumnID, lchildTableID)

	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Dim cRST
	Set cRST = oConn.Execute ("select distinct formgroup_id from BIOSARDB.db_vw_formgroup_tables where table_id =" & lchildTableID & " AND table_order >1")
	if Not (cRSt.BOF and cRSt.EOF) then
		do while not cRST.EOF 
		formgroupid = cRST("formgroup_id")
		CreateXMLTemplates(formgroupid)
		cRST.MoveNext
		Loop
	end if
	cRST.close
End Sub


Sub UpdateFormgroupTemplatesByColID(lColID)
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Dim cRST
	Set cRST = oConn.Execute ("select distinct formgroup_id from BIOSARDB.db_vw_formitems_compact where column_id=" & lColID)
	if Not (cRSt.BOF and cRSt.EOF) then
		do while not cRST.EOF 
		formgroupid = cRST("formgroup_id")
		CreateXMLTemplates(formgroupid)
		cRST.MoveNext
		Loop
	end if
	cRST.close
End Sub

Sub checkDBSchema(schema_name)
	on error resume next
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	Set oRSSchema = Server.CreateObject("adodb.recordset")
	oRSSchema.Open "select * from BIOSARDB.DB_Schema where owner = '" & schema_name & "'", oConn, 1, 3
	if (oRSSchema.BOF and oRSSchema.EOF) then
		oRSSchema.AddNew
		oRSSchema("Owner") = UCase(schema_name)
		ORSSchema("DISPLAY_NAME") = UCase(schema_name)
		ORSSchema.Update	
		ORSSchema.Close	
	end if
End Sub

Sub setTableContraints(table_id,table_name)
	
		column_name = ""
		set cmd = server.CreateObject("ADODB.command")
		Set oRSCols = Server.CreateObject("ADODB.Recordset")
		cmd.CommandType=adCmdText
		cmd.ActiveConnection = oConn
		sql = "select COLUMN_ID,COLUMN_NAME from BIOSARDB.db_column where table_id = ?" 
		cmd.Parameters.Append cmd.CreateParameter("ptable_name", 139, 1, 0, table_id) 
		cmd.CommandText=sql
		oRSCols.open cmd
		' loop over all columns in table
		if Not (oRSCols.EOF and oRSCols.BOF) then
			Do while not oRSCols.eof
				column_id = ""
				column_id = oRSCols("COLUMN_ID")
				column_name = oRSCols("COLUMN_NAME")
				sql = ""
				Dim aAr
				aAr = Split(table_name, ".")
				sOwner = aAr(0)
				sTable = aAr(1)
				sql = "select TABLE_NAME,COLUMN_NAME from BIOSARDB.db_vw_col_constraints" & _
								" where constraint_name in " & _
								"(select r_constraint_name from BIOSARDB.db_vw_col_constraints" & _
								" where Upper(table_name) = ? and Upper(owner) = ?" & _
								" and constraint_type = 'R' and Upper(column_name) =?)"
				
				set cmd = server.CreateObject("ADODB.command")
				Set oRSFK = Server.CreateObject("ADODB.Recordset")
				cmd.CommandType=adCmdText
				cmd.ActiveConnection = oConn
				cmd.Parameters.Append cmd.CreateParameter("ptable_name", 200, 1, Len(sTable) + 1, UCase(sTable))
				cmd.Parameters.Append cmd.CreateParameter("ptable_ownser", 200, 1, Len(sOwner) + 1, UCase(sOwner)) 
				cmd.Parameters.Append cmd.CreateParameter("pcolumn_name", 200, 1, Len(column_name) +1, UCase(CStr(column_name)))
				cmd.CommandText=sql
				oRSFK.open cmd
				' get foreign (parent) constraints
				If not (oRSFK.BOF and oRSFK.EOF) THen
					do while not oRSFK.EOF
						sql = ""
						oTable_name = oRSFK("TABLE_NAME")
						oColumnName = oRSFK("COLUMN_NAME")
						oRtablename = Name & "." & oTable_name
						
						sql =  "select table_id, column_id from BIOSARDB.db_vw_column_table where Upper(table_name) = ? and Upper(column_name) = ?"
						set cmd = server.CreateObject("ADODB.command")
						Set oRSParent = Server.CreateObject("ADODB.Recordset")
						cmd.CommandType=adCmdText
						cmd.ActiveConnection = oConn
						cmd.Parameters.Append cmd.CreateParameter("ptable_name", 200, 1, Len(oRtablename) + 1, UCase(oRtablename)) 
						cmd.Parameters.Append cmd.CreateParameter("oColumnName", 200, 1, Len(oColumnName) + 1, UCase(oColumnName)) 
						cmd.CommandText=sql
						oRSParent.Open cmd
						if not (oRSParent.EOF and oRSParent.BOF) then
							ptable_id = oRSParent("TABLE_ID")
							pcolumn_id=oRSParent("COLUMN_ID")
							
							oRSParent.close
							childTableID= table_id
							childColId = column_id
							
							on error resume next
							set cmd = server.CreateObject("ADODB.command")
							cmd.CommandType=adCmdText
							cmd.ActiveConnection = oConn
							sql = "INSERT INTO BIOSARDB.db_relationship (TABLE_ID, COLUMN_ID, CHILD_TABLE_ID,CHILD_COLUMN_ID,JOIN_TYPE)Values(?,?,?,?,'OUTER')"
							cmd.CommandText = sql
							cmd.Parameters.Append cmd.CreateParameter("ptable_id", 139, 1, 0, ptable_id) 
							cmd.Parameters.Append cmd.CreateParameter("pcolumn_id", 139, 1,0,pcolumn_id )
							cmd.Parameters.Append cmd.CreateParameter("childTable", 139, 1,0,childTableID )
							cmd.Parameters.Append cmd.CreateParameter("childColId", 139, 1,0,childColId )
							cmd.Execute
						end if
						oRSFK.MoveNext
					loop
					oRSFK.close
				end if
				oRSCols.movenext
			Loop
			oRSCols.close
		end if
	
End Sub


Sub doSetMimeType(ptable_id, mimetype)
	if not isObject(oConn) then
		Set oConn = SysConnection
	end if
	sql = "select column_id from biosardb.db_column  where table_id=? and datatype='BLOB' and index_type_id = '1'"
	set cmd = server.CreateObject("ADODB.command")
	set RS = server.CreateObject("ADODB.Recordset")
	cmd.CommandType=adCmdText
	cmd.ActiveConnection = oConn
	cmd.Parameters.Append cmd.CreateParameter("ptable_id", 139, 1, 0, ptable_id) 
	cmd.CommandText=sql
	
	RS.Open cmd
	if Not (RS.BOF and RS.EOF) then
		content_type_id = getMimeTypeID(oConn,mimetype)
		if trim(content_type_id) <> "" then
			do while not RS.EOF
				set cmd3 = server.CreateObject("ADODB.command")
				cmd3.CommandType=adCmdText
				cmd3.ActiveConnection = oConn
				cmd3.CommandText="UPDATE db_column SET content_type_id = ?  WHERE  table_id = ? AND column_id = ?"
				cmd3.Parameters.Append cmd3.CreateParameter("pcontentType", 139, 1, 0, content_type_id) 
				cmd3.Parameters.Append cmd3.CreateParameter("ptable_id", 139, 1, 0, ptable_id) 
				cmd3.Parameters.Append cmd3.CreateParameter("pcolumn_id", 139, 1,0,RS("column_id") )
				cmd3.Execute
				RS.MoveNext
			Loop
			RS.Close
		end if
	end if
	
	
End sub

Function getMimeTypeID(byref conn, mimetype)
		if not isObject(conn) then
			Set conn = SysConnection
		end if
		sql = "select content_type_id from DB_http_content_type where mime_type = ?"
		set cmd2 = server.CreateObject("ADODB.command")
		set RS2 = server.CreateObject("ADODB.Recordset")
		cmd2.CommandType=adCmdText
		cmd2.ActiveConnection = conn
		cmd2.Parameters.Append cmd2.CreateParameter("pmimetype", 200, 1, Len(mimetype) + 1, UCase(Trim(mimetype))) 	
		cmd2.CommandText=sql
		set RS2 = cmd2.execute
		if not (RS2.BOF and RS2.EOF) then
			mime_type_id = RS2("content_type_id")
			RS2.Close
		else
			mime_type_id=""
		end if
	getMimeTypeID=mime_type_id

end Function 
</script>