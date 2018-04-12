<%

Const SCHEMA_NAME = "BIOSARDB"

Function SQLGetAllOwners
	' get all users that own tables
'-- CSBR ID:128544
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: To select enable owners of not just tables but also views;
'-- Date: 08/24/2010
	'SQLGetAllOwners = "select user_id,username from all_users where username in (select owner from all_tables) order by username asc"
	SQLGetAllOwners = "select user_id,username from all_users where username in (select owner from all_tables) or " & _
						"username in (select owner from all_views) order by username asc"
'-- End of Change #128544#
End Function

Function SQLExposedTablesByOwner(ByVal Owner)
	' get exposed tables for owner
	SQLExposedTablesByOwner = "select * from BIOSARDB.db_table where is_exposed = 'Y' and  is_view = 'N' and owner = '" & Owner & "'" & _
							  " order by table_name asc"
End Function

Function SQLExposedTablesAndViewsByOwner(ByVal Owner)
	SQLExposedTablesAndViewsByOwner = "select * from BIOSARDB.db_table where owner = '" & Owner & "'"
end function

Function SQLAllTablesByOwner(ByVal Owner)
	' get all tables for owner
	SQLAllTablesByOwner = "select owner,table_name from all_tables where owner = '" & Owner & "'"
End Function

function SQLAllExposedTables
	' get all exposed tables
	SQLAllExposedTables = "select * from BIOSARDB.db_table where is_view = 'N' order by table_name asc"
end function

function SQLSchemaByName(byval name)
	' get schema by name
	SQLSchemaByName = "select * from BIOSARDB.db_schema where owner = '" & name & "'"
end function

function SQLDeleteSchemaByName(byval name)
	SQLDeleteSchemaByName = "delete from BIOSARDB.db_schema where owner = '" & name & "'"
end function

function SQLAllExposedSchemas
	' get all exposed schemas
	SQLAllExposedSchemas = "select Distinct * from BIOSARDB.db_schema order by owner"
end function

function SQLAllExposedViews
	' get all exposed views
	SQLAllExposedViews = "select table_id, table_name from BIOSARDB.db_table where is_view = 'Y' order by table_name"
end function

function SQLAllTablesAndSchemaNames
	' get all exposed everything
	SQLAllTablesAndSchemaNames = "select * from BIOSARDB.db_vw_table_schema where is_exposed = 'Y' order by schema_name, table_name"
end function

function SQLDBViewByName(ByVal Name)
	' get db table by full name
	Dim sOwner 
	Dim sTable
	Dim aAr
	aAr = Split(Name, ".")
	sOwner = aAr(0)
	sTable = aAr(1)
	SQLDBViewByName = "select all_views.* from all_views where view_name = '" & sTable & "' and owner = '" & sOwner & "'"
end function

function SQLDBTableByName(ByVal Name)
	' get db table by full name
	Dim sOwner 
	Dim sTable
	Dim aAr
	aAr = Split(Name, ".")
	sOwner = aAr(0)
	sTable = aAr(1)
	SQLDBTableByName = "select owner, table_name from all_tables where table_name = '" & sTable & "' and owner = '" & sOwner & "'"
end function

function SQLPrimaryKeyByTableName(ByVal Name)
	' get primary key column by table full name
	Dim sOwner 
	Dim sTable
	Dim aAr
	aAr = Split(Name, ".")
	sOwner = aAr(0)
	sTable = aAr(1)
	SQLPrimaryKeyByTableName = "select * from BIOSARDB.db_vw_col_constraints where table_name = '" & sTable & "' and owner = '" & sOwner & "'" & _
							   " and constraint_type = 'P'"	
end function

function SQLParentColumnsByColumn(ByVal TableName, ByVal ColumnName)
	' get related column by table full name and column name
	Dim sOwner 
	Dim sTable
	Dim aAr
	aAr = Split(TableName, ".")
	sOwner = aAr(0)
	sTable = aAr(1)
	SQLParentColumnsByColumn = "select * from BIOSARDB.db_vw_col_constraints" & _
							   " where constraint_name in " & _
							   "(select r_constraint_name from BIOSARDB.db_vw_col_constraints" & _
							   " where table_name = '" & sTable & "' and owner = '" & sOwner & "'" & _
							   " and constraint_type = 'R' and column_name = '"	& ColumnName & "')"
end Function

function SQLDBColumnByOwnerColumnAndTableName(BYval owner, byval table, byval column)
	' get db column by composite primary key
	SQLDBColumnByOwnerColumnAndTableName = _
		"select * from all_tab_columns where Upper(table_name) = '" & UCase(Table) & "' and Upper(owner) = '" & UCase(owner) & "'" & _
		" and column_name = '" & UCase(column) & "'"
end function

function SQLDBColumnByTableName(ByVal Name)
	Dim sOwner 
	Dim sTable
	Dim aAr
	aAr = Split(Name, ".")
	sOwner = aAr(0)
	sTable = aAr(1)
	' get db column by name
	SQLDBColumnByTableName = "select * from all_tab_columns where Upper(table_name) = '" & UCase(sTable) & "' and Upper(owner) = '" & UCase(sowner) & "'"
end function

Function SQLExposedTableByName(ByVal Name)
	' get exposed table by name
	SQLExposedTableByName = "select * from BIOSARDB.db_table where table_short_name = '" & Name & "'"
end function

Function SQLExposedTableByFullName(ByVal Name)
	' get exposed table by name
	SQLExposedTableByFullName = "select * from BIOSARDB.db_table where table_name = '" & Name & "'"
end function

Function SQLExposedViewsByOwner(ByVal Owner)
	' get exposed views for owner
	SQLExposedViewsByOwner = "select * from BIOSARDB.db_table where is_exposed = 'Y' and is_view = 'Y' and owner = '" & Owner & "'"
End Function

Function SQLAllViewsByOwner(ByVal Owner)
	' get all views for owner
	SQLAllViewsByOwner = "select owner, view_name from all_views where Upper(owner) = '" & UCase(Owner) & "'"
End Function

function SQLExposedTableById(ByVal Id)
	' get table info by id
	SQLExposedTableById = "select * from BIOSARDB.db_table where table_id = " & Id  & " order by upper(display_name) "
end function

function SQLColumnsByTableId(ByVal Id)
	' get column info by id
	SQLColumnsByTableId = "select * from BIOSARDB.db_column where table_id = " & Id & " order by default_column_order" 
end function

function SQLColumnsByTableIdANDColID(ByVal Id, ByVal colID)
	' get column info by id
	SQLColumnsByTableIdANDColID = "select * from BIOSARDB.db_column where table_id = " & Id & "column_id=" & colID
end function

function SQLExposedColumnsByTableId(ByVal id)
	SQLExposedColumnsByTableId = "select * from BIOSARDB.db_column where table_id = " & Id & _
								 " and is_visible = 'Y' order by column_name"
end function						

function SQLChildTablesByBaseTableId(byval id)
	' get child tables of this base table
	SQLChildTablesByBaseTableId = "select * from BIOSARDB.db_table where is_exposed = 'Y' and table_id in " & _
		"(select child_table_id from BIOSARDB.db_relationship where BIOSARDB.db_relationship.table_id = " & id & ") order by upper(display_name)"
end function

' forms

function SQLFormsByFormgroupId(ByVal fgid)
	' get forms
	SQLFormsByFormgroupId = "select * from BIOSARDB.db_form where formgroup_id = " & fgid
end function

function SQLFormsByFormgroupIdAndFormType(ByVal fgid, byval typid)
	' get forms
	SQLFormsByFormgroupIdAndFormType = SQLFormsByFormgroupId(fgid) & _
											" and formtype_id = " & typid
end function


function SQLFormitemsByTableIdAndFormGroupId(BYval TableId, byval groupId)
	' get form items by table id
	SQLFormitemsByTableIdAndFormGroupId = "select * from BIOSARDB.db_form_item where table_id = " & tableid & _
										" and form_id in (select form_id from db_form where formgroup_id = " & groupId & ")"
end function

function SQLFormitemsByVirtualColumnIdAndFormId(BYval cId, byval fid)
	' get form items by v_column id (for mw, formula and structures virtual columns)
	SQLFormitemsByVirtualColumnIdAndFormId = "select * from BIOSARDB.db_form_item where v_column_Id = " & cid  & _
									  " and form_id = " & fid
end function

function SQLFormitemsByColumnIdAndFormId(BYval cId, byval fid)
	' get form items by column id
	SQLFormitemsByColumnIdAndFormId = "select i.*, c.datatype from BIOSARDB.db_form_item i, db_column c where i.column_id = c.column_id and i.column_id = " & cid  & _
									  " and i.form_id = " & fid
end function

function SQLFormitemsByFormItemIdAndFormId(BYval cId, byval fid)
	' get form items by column id
	SQLFormitemsByFormItemIdAndFormId = "select * from BIOSARDB.db_form_item where form_item_id = " & cid  & _
									  " and form_id = " & fid
end function

function SQLFormitemsByFormIdAndTableId(byval fid, byval tid)
	' get form items by column id
	SQLFormitemsByFormIdAndTableId = "select * from BIOSARDB.db_vw_formitems_all where form_id = " & fid & _
									  " and table_id = " & tid & " order by column_order"
end function

function SQLOptionsByFormtypeAndDisplayType(byval ftid, byval dtid)
	dim sret
	sRet = "select * from BIOSARDB.db_display_option where disp_opt_id in "
	sRet = sRet & "(select disp_opt_id from BIOSARDB.db_dtyp_dopt where disp_typ_id = " & dtid
	sRet = sRet & ") and disp_opt_id in (select disp_opt_id from BIOSARDB.db_formtype_dopt where "
	sRet = sRet & "formtype_id = " & ftid & ")"
	SQLOptionsByFormtypeAndDisplayType =  sret										 
end function

function SQLFormgroupsByUserId(ByVal Id)
	' get formgroups by id
	SQLFormgroupsByUserId = "select * from BIOSARDB.db_formgroup where user_id = '" & id & "'" & " order by formgroup_name"	
end function

function SQLFormgroupsAll(byVal userID)
	' get formgroups by id
	' DCW 2/24/2004 added the decode and order by clause, this also required that the current user_id be passed in.
	SQLFormgroupsAll = "select decode(upper(user_id), '" & userId  & "', 0, 1), dbfg.* from BIOSARDB.db_formgroup dbfg order by 1, user_id, formgroup_name"	
end function

function SQLGrantedFormgroups(byVal userID, byVal idlist)
	if idlist <> "" then
		' DCW 2/24/2004  added decode and order by clause to organize the form list example sql
		'Select decode(user_id, 'DANWEAVER', 0, 1), BIOSARDB.DB_FORMGROUP.* from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)='DANWEAVER' OR  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN (1565) order by 1, user_id, formgroup_name;
		sql = "Select decode(upper(user_id), '" & userId  & "', 0, 1), dbfg.* from BIOSARDB.DB_FORMGROUP dbfg Where upper(dbfg.USER_ID)='" & userId  & "' OR  dbfg.FORMGROUP_ID IN(" & idlist & ") order by 1, user_id, formgroup_name"
	else
		sql = "Select decode(upper(user_id), '" & userId  & "', 0, 1), dbfg.* from BIOSARDB.DB_FORMGROUP dbfg Where upper(dbfg.USER_ID)='" & userId  & "' order by 1, user_id, formgroup_name"
		' End CDW changes
	end if
SQLGrantedFormgroups = sql
end function

function SQLGrantedFormgroupsWhereNotOwner2(byVal userID, byVal idlist)
	if idlist <> "" then
		sql = "Select * from BIOSARDB.DB_FORMGROUP Where Not upper(BIOSARDB.DB_FORMGROUP.USER_ID)='" & userId  & "' OR  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & idlist & ")"
	else
		sql = "Select * from BIOSARDB.DB_FORMGROUP Where Not upper(BIOSARDB.DB_FORMGROUP.USER_ID)='" & userId  & "'"
	end if
	SQLGrantedFormgroupsWhereNotOwner = sql
end function

function SQLGrantedFormgroupsWhereNotOwner(byVal userID, byVal idlist)
	if idlist <> "" then
	'DCW 6/3/04: re-wrote this query to return the correct results for the intent of this function.
		sql = "Select * from BIOSARDB.DB_FORMGROUP Where upper (BIOSARDB.DB_FORMGROUP.USER_ID)<>'" & userId  & "' AND  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & idlist & ")"
	else
		'sql = "Select * from BIOSARDB.DB_FORMGROUP Where Not upper(BIOSARDB.DB_FORMGROUP.USER_ID)='" & userId  & "'"
		sql = "Select * from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)<>'" & userId  & "' AND BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & idlist & ")"
	end if
	SQLGrantedFormgroupsWhereNotOwner = sql
end function

function SQLColumnTableById(ByVal Id)
	SQLColumnTableById = "select * from BIOSARDB.db_vw_column_table where column_id = " & id
end function

function SQLColumnTableAll
	SQLColumnTableAll = "select * from BIOSARDB.db_vw_column_table order by table_name, column_name"
end function

function SQLColumnByTableID(byVal ID)
	SQLColumnByTableID = "select * from BIOSARDB.db_vw_column_table where table_id = " & id & " order by  column_name"
end function

function SQLColumnByTableIDandDataType(byVal ID, byVal DataType)
	if DataType = "CHAR" or DataType = "VARCHAR2" or DataType = "NUMBER" then
		DatatypeTxt = " AND (Datatype = 'CHAR' or DataType = 'VARCHAR2' or DataType = 'NUMBER')"
	else
		DatatypeTxt = " AND DataType = '" & DataType & "' "
	end if
	SQLColumnByTableIDandDataType = "select * from BIOSARDB.db_vw_column_table where table_id = " & id & DataTypeTxt &  " order by  column_name"
end function

function SQLColumnTableByTableNameAndColumnName(ByVal tablename, byval columnname)
	SQLColumnTableByTableNameAndColumnName = "select * from BIOSARDB.db_vw_column_table where table_name = '" & tablename & "' and column_name = '" & columnname & "' order by table_name"
end function

function SQLRelationshipsByColumnId(ByVal Id)
	' get relationshipss with given base id
	SQLRelationshipsByColumnId = "select * from BIOSARDB.db_relationship where column_id = " & id
end function

function SQLRelationshipsByChildColumnId(ByVal Id)
	' get relationshipss with given base id
	SQLRelationshipsByChildColumnId = "select * from BIOSARDB.db_relationship where child_column_id = " & id
end function

function SQLRelationshipByBaseAndChild(ByVal baseid, byval childid)
	' get relationship with composite primary key
	SQLRelationshipByBaseAndChild = "select * from BIOSARDB.db_relationship where column_id = " & baseid & _
									" and child_column_id = " & childid	
end function

function SQLDeleteRelationship(ByVal baseid, byval childid)
	SQLDeleteRelationship = "delete from BIOSARDB.db_relationship where column_id = " & baseid & _
							" and child_column_id = " & childid
end function

function SQLDeleteFromRelationshipByChildColumnID(byval id)
	SQLDeleteFromRelationshipByChildColumnID = "delete from BIOSARDB.db_relationship where child_column_id = " & id
end function

function SQLDeleteRelationshipsByTableId(Byval id)
	SQLDeleteRelationshipsByTableId = "delete from BIOSARDB.db_relationship where table_id = " & id & _
										" or child_table_id =" & id
end function

function SQLDeleteFormgroupTablesByTableId(ByVal Id)
	SQLDeleteFormgroupTablesByTableId = "delete from BIOSARDB.db_formgroup_tables where " & _
										  "table_id = " & id
end function

function SQLDeleteFormgroupTablesByFormgroupId(byval id)
	SQLDeleteFormgroupTablesByFormgroupId = "delete from BIOSARDB.db_formgroup_tables where " & _
										  "formgroup_id = " & id
	
end function

function SQLColumnById(ByVal Id)
	' get column by id
	SQLColumnById = "select * from BIOSARDB.db_column where column_id = " & Id
end Function


function SQLFormtypeById(ByVal Id)
	SQLFormtypeByid = "select * from BIOSARDB.db_formtype where formtype_id = " & id
end function

function SQLFormgroupsByBaseTableId(byval id)
	' get formgroups by base tableid
	SQLFormgroupsByBaseTableId = "select * from BIOSARDB.db_formgroup where base_table_id = " & Id
end function

function SQLFormgroupById(ByVal Id)
	' get formgroup by id
	SQLFormgroupById = "select * from BIOSARDB.db_formgroup where formgroup_id = " & Id
end function

function SQLChildColumnsByColumnId(ByVal Id)	
	' get child columns for column id
	SQLChildColumnsByColumnId = "select * from BIOSARDB.db_relationship where column_id = " & Id
end function

function SQLFormgroupTablesByFormgroupId(byval id)
	SQLFormgroupTablesByFormgroupId = "select * from BIOSARDB.db_formgroup_tables where formgroup_id = " & id & _
	" order by table_order"
end function

function SQLColFormItemByTableId(byVal id)
	SQLColFormItemByTableId = "select * from BIOSARDB.db_vw_column_form_item where table_id = " & id
end function

function SQLChildTablesByTableId(byval id)
	SQLChildTablesByTableId = "select * from BIOSARDB.db_vw_child_tables where table_id = " & id
end function

function SQLParentTablesByChildTableId(byVal id)
	SQLParentTablesByChildTableId = "select * from BIOSARDB.db_vw_parent_tables where child_table_id = " & id
end function							 

function SQLFormgroupTableByPK(byval tableid, byval formgroupid)
	SQLFormgroupTableByPK = "select * from BIOSARDB.db_formgroup_tables where " & _
							" table_id = " & tableid & " and formgroup_id = " & formgroupid
end function

function SQLColumnByTableIdAndNonNullMstPath(byval id)
	SQLColumnByTableIdAndNonNullMstPath = "select column_id from BIOSARDB.db_column where table_id = " & id & _
										  " and mst_file_path is not null"	
end function

function SQLColumnByTableIdAndColumnName(byval id, byVal columnname)
	SQLColumnByTableIdAndColumnName = "select * from BIOSARDB.db_column where table_id = " & id & _
										  " and column_name = '" & columnname & "'"
end function

function SQLDisplayTypeByDatatype(byval datatype)
	SQLDisplayTypeByDatatype = "select * from BIOSARDB.db_display_type where disp_typ_id in " & _
							   " (select disp_typ_id from BIOSARDB.db_datatype_display_type where " & _
							   " datatype = '" & datatype & "')" 
end function

function SQLDisplayTypeById(byval id)
	SQLDisplayTypeById = "select * from BIOSARDB.db_display_type where disp_typ_id in (" & id & ") order by disp_typ_id"
end function

' update functions

function SQLUpdateFormgroup(ByVal Name, ByVal Desc, byVal IsPublic, ByVal Id)
	SQLUpdateFormgroup = "update BIOSARDB.db_formgroup set formgroup_name = '" & name & "'" & _
									", description = '" & desc & "'" & _
									", is_public = '" & IsPublic & "'" & " where formgroup_id = " & id
end function

function SQLUpdateSchemaDisplayNameAndPassword(Byval owner, byval desc, byval password)
	
	SQLUpdateSchemaDisplayNameAndPassword = "update BIOSARDB.db_schema set display_name = '" & desc & "'" & _
								 ", schema_password = '" & password & "'" & _
							     " where owner = '" & owner & "'"
end function

' delete functions

function SQLDeleteTableById(ByVal Id)
	' delete table
	SQLDeleteTableById = "delete from BIOSARDB.db_table where table_id = " & id
end function

function SQLDeleteColumnsByTableId(ByVal Id)
	' delete columns belonging to table
	SQLDeleteColumnsByTableId = "delete from BIOSARDB.db_column where table_id in " & id 
end function

function SQLDeleteFormItemsByTableId(ByVal Id)
	' delete formitems belonging to table
	SQLDeleteFormItemsByTableId = "delete from BIOSARDB.db_form_item where table_id = " & id
end function

function SQLDeleteFormItemByColumnIdAndFormId(byVal colid, byval formid)
	SQLDeleteFormItemByColumnIdAndFormId = "delete from BIOSARDB.db_form_item where column_id = " & colid & _
										   " and form_id = " & formid
end function	
function SQLDeleteFormItemByVirtualColumnIdAndFormId(byVal colid, byval formid)
	SQLDeleteFormItemByVirtualColumnIdAndFormId = "delete from BIOSARDB.db_form_item where V_column_id = " & colid & _
										   " and form_id = " & formid
end function									  

function SQLDeleteFormItemsByFormGroupId(ByVal id)
	SQLDeleteFormItemsByFormGroupId = "delete from BIOSARDB.db_form_item where form_id in " & _
									  "(select form_id from db_form where formgroup_id = " & id & ")"
end function

function SQLDeleteFormItemsByFormGroupIdAndTableID(ByVal table_id,ByVal formgroupid)
	SQLDeleteFormItemsByFormGroupIdAndTableID = "delete from biosardb.db_form_item where form_id in " & _
									  "(select form_id from db_form where formgroup_id = " & formgroupid &  ") AND table_id=" & table_id 
end function
function SQLDeleteFormsByFormgroupId(ByVal formid)
	SQLDeleteFormsByFormgroupId = "delete from BIOSARDB.db_form where formgroup_id = " & formid
end function

function SQLDeleteFormgroupById(ByVal id)
	SQLDeleteFormgroupById = "delete from BIOSARDB.db_formgroup where formgroup_id = " & id
end function 

function SQLDeleteFormgroupsByBaseTableId(ByVal id)
	SQLDeleteFormgroupsByBaseTableId = "delete from BIOSARDB.db_formgroup where base_table_id  = " & id
end function

function SQLDeleteFormsByBaseTableId(ByVal id)
	SQLDeleteFormsByBaseTableId = "delete from BIOSARDB.db_form where formgroup_id in " & _
								  " (select formgroup_id from BIOSARDB.db_formgroup where base_table_id = " & id & ")"
end function								 

function SQLDeleteFormgroupTablesByPK(byval tableid, byval formgroupid)
	SQLDeleteFormgroupTablesByPK = "delete from BIOSARDB.db_formgroup_tables where " & _
								   " table_id = " & tableid & " and formgroup_id = " & formgroupid
end function

' insert functions

function SQLInsertSchema(byval name, byval desc)
	SQLInsertSchema = "insert into BIOSARDB.db_schema(owner, display_name)" & _
					  " values ('" & name & "', '" & desc & "')"
end function


function SQLgetIndexTypes(datatype)
	SQLgetIndexTypes = "select * from BIOSARDB.db_index_type where data_type='" & datatype & "'"

end function


function SQLgetContentTypes(byval index_type_id)
	if not indextype <> "" then indextype = 0
	SQLgetContentTypes = "select * from BIOSARDB.db_http_content_type where index_type='" & index_type_id & "' OR index_type='0'"
end function


function SQLgetIndexTypeName(index_type_id)
	SQLgetIndexTypeName="select index_type from  BIOSARDB.db_index_type where index_type_id=" & index_type_id
end function 

function SQLgetContentTypeName(content_type_id)
	SQLgetContentTypeName="select mime_type from  BIOSARDB.db_http_content_type where content_type_id=" & content_type_id
end function 

function SQLisIndexedbyCartridge(owner,table_name,column)
	SQLisIndexedbyCartridge = "select * from cscartridge.all_csc_indexes where " &_
	"owner='" & owner & "' and "&_
	" table_name='" & table_name & "' and " &_
	" COLUMN_NAME='" & column  & "'"
	
	
end function

function SQLgetIndexTypeIDbyName(index_type, datatype)
	SQLgetIndexTypeIDbyName="select index_type_id from  BIOSARDB.db_index_type where index_type='" & index_type & "' and data_type='" & datatype & "'"
end function


function SQLisStructureColumn(column_id)

	SQLisStructureColumn = "select BIOSARDB.db_column.column_id  from BIOSARDB.db_column where BIOSARDB.db_column.column_id = " & column_id   &_
	" AND (db_column.index_type_ID=" & CARTRIDGE_INDEX_TYPE & " or  db_column.index_type_ID=" & CARTRIDGE_INDEX_TYPE_BLOB & ")"
End function


function SQLisImageColumn(column_id)

	SQLisImageColumn = "select BIOSARDB.db_column.column_id  from BIOSARDB.db_column where BIOSARDB.db_column.column_id = " & column_id   &_
	" AND db_column.index_type_ID=1 and db_column.content_type_ID>0"
End function

function SQLDeleteFormItemsByFormGroupIdAndTableID(ByVal table_id,ByVal formgroupid)
	SQLDeleteFormItemsByFormGroupIdAndTableID = "delete from BIOSARDB.db_form_item where form_id in " & _
									  "(select form_id from db_form where formgroup_id = " & formgroupid &  ") AND table_id=" & table_id 
end function
%>