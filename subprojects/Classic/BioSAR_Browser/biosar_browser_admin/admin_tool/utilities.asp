
<!--#INCLUDE VIRTUAL = "/biosar_browser/cs_security/admin_utils_vbs.asp"-->

<%
' runtime switches
Const IGNORE_USER_PERMISSIONS = false		' true to display all tables to user
Const IGNORE_ADMIN_PERMISSIONS = false		' true to display all tables to admin
Const SINGLE_CHILD_LEVEL = True				' change this to false to enable multiple child levels
Const ENABLE_PUBLIC_FORMGROUPS = true		' true to allow user to create public formgroups

Const BASE_FORMGROUP = "base_form_group"
Const BASE_CONNECTION = "base_connection"

' long values (keys) with this value in VB will be considered
' to be null in the database
Const NULL_AS_LONG = -1

Const VCOL_STRUCTURE = -2
Const VCOL_FORMULA = -3
Const VCOL_MOLWEIGHT = -4

Const DISP_TYP_SELECT = 4
Const DISP_TYP_STRUCTURE = 7
Const DISP_TYP_FORMULA = 9
Const DISP_TYP_MOLWEIGHT = 10

Const FORM_QUERY = 1
Const FORM_LIST = 2
Const FORM_DETAIL = 3

Const CARTRIDGE_INDEX_TYPE=2
Const CARTRIDGE_INDEX_TYPE_BLOB=4
Const NO_INDEX_TYPE=1
Const NO_INDEX_TYPE_CLOB=3
Const UNSPECIFIED_INDEX_TYPE=0


Const LIST_COLUMN_ACROSS = 1
Const LIST_COLUMN_DOWN = 2
Const DETAIL = 3
Const DETAIL_CHILD = 4
Const EXCEL_TEMPLATE=5
if Not isArray(Application("illegalFormCharcters")) then
	Dim myArray(2)
	myArray(0)= Chr(39)
	myArray(1)= Chr(34)
	myArray(2) = Chr(126)
	
	Application("illegalFormCharcters") = myArray
	
	Application("illegalFormCharctersHTML") = "<br>Single Quote &#39;<br>Double Quote &#34;<br>tilde &#126;"
end if


Dim oConn
Dim uConn
Dim sConn

Function SchemaConnection(Byval Schema)

	' use this to get a connection with the privileges of the 
	' given schema
	Dim oConn
	' store the current username and user id
	Dim sOldUser, sOldId
	sOldUser = Session("UserName" & "biosar_browser")
	sOldId = Session("UserID" & "biosar_browser")
	' set session vars to the admin vars
	test1= Application("BIOSAR_BROWSER_USERNAME")
	test2 = Application("BIOSAR_BROWSER_PWD")
	if UCase(schema) = "BIOSARDB" then
		Session("UserName"& "biosar_browser") = Application("BIOSAR_BROWSER_USERNAME")
		Session("UserID" & "biosar_browser") = Application("BIOSAR_BROWSER_PWD")
	else
		Session("UserName" & "biosar_browser") = Schema
		' get password
		Dim password
		Dim oSysConn
		Set oSysCon = SysConnection
		dim oRS
		Set oRS = oSysCon.Execute("Select schema_password from BIOSARDB.db_schema where owner = '" & Schema & "'") 
		'decrypt password
		password = SAR_CryptVBS(oRS("schema_password"), UCase(trim(Schema)))
		Session("UserID" & "biosar_browser") = Password
	end if
	' get connection

	Set oConn = GetNewConnection( _
				"biosar_browser", _
				"base_form_group", _
				"base_connection")
	' reset session user and id back to value
	Session("UserName" & "biosar_browser") = sOldUser
	Session("UserID" & "biosar_browser") = sOldId
	Set SchemaConnection = oConn
end function 

Function UserConnection 
	' use this to get a connection with the privileges of the
	' logged-in user	
	'dbgbreak "sess_user=" & Session("UserName" & "biosar_browser") & _
	'		", sess_pwd =" &  Session("UserID" & "biosar_browser")
	Dim oConn
	Set oConn = GetNewConnection( _
				"biosar_browser", _
				"base_form_group", _
				"base_connection")
	Set UserConnection = oConn
End Function

Function SysConnection
	' use this to get a connection with system privileges
	Dim oConn
	' store the current username and user id
	Dim sOldUser, sOldId
	sOldUser = Session("UserName" & "biosar_browser")
	sOldId = Session("UserID" & "biosar_browser")
	' set session vars to the admin vars
	Session("UserName" & "biosar_browser") = Application("DBA_username")
	Session("UserID" & "biosar_browser") = Application("DBA_pwd")
	on error resume next
	Set oConn = GetNewConnection( _
				"biosar_browser", _
				"base_form_group", _
				"base_connection")
	if err then
		Response.Write "Error while trying to connect as system. Please verify the system credentials in cfserver.ini."
		Response.end
	end if
	' reset session user and id back to value
	Session("UserName" & "biosar_browser") = sOldUser
	Session("UserID" & "biosar_browser") = sOldId
	Set SysConnection = oConn
End Function

Function Bool2Check(ByVal BoolVal)
	' take a y or n and output checked or not
	if BoolVal = "Y" Then
		Bool2Check = " checked"
	Else
		Bool2Check = ""
	End If
End Function

Function Boolean2Check(ByVal BoolVal)
	' take a vb true/false and return checked or not
	if Boolval then	
		boolean2check = " checked"
	else
		boolean2check = ""
	end if
end function

Function Check2DBBool(ByVal check)
	' for form vals - converts "" to N, anything else to Y
	if check = "" then
		Check2DBBool = "N"
	else
		Check2DBBool = "Y"
	end if
end function

Function Check2DBBool2(ByVal check)
	if inStr(check) = "N" then
		Check2DBBool2 = "N"
	end if
	if inStr(check) = "Y" then
		Check2DBBool2 = "Y"
	end if
	if not check <> "" then
		Check2DBBool2 = "N"
	end if
end function

function GetBLongText(ByRef RS, ByVal Fieldname)
	' THIS DOESN'T WORK
	dim Numblocks
	dim LeftOver
	dim FileSize
	Dim BlockSize
	Dim strData
	dim i
	BlockSize = 4000   ' change to suit
	' get data from ORACLE LONG datatype
     FileSize = Rs(fieldname).ActualSize
     
     
	 Numblocks = Cint(FileSize / BlockSize)
     LeftOver = Cint(FileSize Mod BlockSize)
     
     Response.Write "filsize = " & filesize
     Response.Write "leftover = " & leftover
     Response.Write "numblocks = " & numblocks
     
     For i = 1 To Numblocks
         strData = strData & Rs(Fieldname).GetChunk(BlockSize)
     Next 
     strData = strData & Rs(Fieldname).GetChunk(LeftOver - 1)
     GetBLongText = strData
end function 

function GetLongText(ByRef RS, ByVal Datafield, byval sizefield)
	' THIS DOESN'T WORK
	Dim dataTarget

	
	' Set up toRS to refer to two fields, such as data and data_size
	dataTarget = rs(datafield).GetChunk(rs(sizefield))
	GetLongText = CStr(dataTarget)
end function

function ListRSVals(ByRef RS, ByVal ListFieldName, ByVal List) 
	' construct a comma-delimited list
	Dim sRet
	sRet = List
	If HasRecords(RS) Then
		rs.movefirst
		Do until rs.eof
			if sret <> "" then
				sret = sret & ","
			end if
			sRet = sRet & rs(ListFieldName)
			rs.movenext
		loop
	End if
	ListRSVals = sRet
end function

function TabSecurityList(ByRef RS, ByVal ListFieldName, ByVal List, byVal append_name) 
	' construct a comma-delimited list
	Dim sRet
	sRet = List
	If HasRecords(RS) Then
		rs.movefirst
		Do until rs.eof
			if sret <> "" then
				sret = sret & ","
			end if
			sRet = sRet & append_name  & rs(ListFieldName)
			rs.movenext
		loop
	End if
	TabSecurityList = sRet
end function

function TabSecurityList_AppendOwner(ByRef RS, ByVal ListFieldName, ByVal List, ByRef oConn, byVal append_name) 
	' construct a comma-delimited list
	Set oRS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	Dim sRet
	Dim bSkipEntry
	bSkipEntry = true
	sRet = List
	If HasRecords(RS) Then
		rs.movefirst
		Do until rs.eof
			temp = split(RS(ListFieldName), "_", -1)
			fg_id = Trim(temp(UBound(temp)))
			if isNumeric(fg_id) then
				cmd.CommandText = "select user_id, is_public from db_formgroup where formgroup_id =?"
				cmd.Parameters.Append cmd.CreateParameter("pid", 5, 1, 0, fg_id) 
				oRS.open cmd
				cmd.Parameters.Delete "pid"
				if Not (oRS.BOF and oRS.EOF) then
					if oRS("is_public") ="Y" or oRS("is_public") ="1" then
						fg_owner = " (" & oRS("user_id")  & ")"
						bSkipEntry = false	
					else
						bSkipEntry = true
					end if
				else
					fg_owner = ""
				end if
				oRS.Close
			end if
			if bSkipEntry = false then
				if sret <> "" then
					sret = sret & ","
				end if
				sRet = sRet & append_name  & rs(ListFieldName) & fg_owner
			end if
			rs.movenext
		loop
	End if
	TabSecurityList_AppendOwner = sRet
end function

function DictFromRS(ByRef RS, ByVal KeyFieldName, ByVal ItemFieldName,  ByVal DefItemValue)
	' Construct a dictionary from a recordset - make the keys the 
	' values in Recordset field KeyFieldName, make the items(values) 
	' the values in Recordset field ItemFieldName.  If DefItemVal is 
	' anything other than "", items will receive that value rather than
	' a value from the recordset.
	dim oDict
	dim sItemVal
	sItemVal = DefItemValue
	Set oDict = Server.CreateObject("Scripting.Dictionary")
	Do until RS.eof
		if sItemVal = "" then
			if itemfieldname <> "" then
				sitemval = rs(itemfieldname)
			end if
		end if
		oDict.Add cstr(rs(keyfieldname)), sitemval
		rs.movenext
	loop
	set dictfromrs = odict
end function

function CnvLong(ByVal theVal, ByVal Direction)
on error resume next
	' convert long values from null and back for database operations
	Select Case Direction
		Case "DB_TO_VB"
			if IsNull(theVal) then
				cnvlong = -1
			else
				cnvlong = clng(theval)
			end if
		Case "VB_TO_DB"
			if theval = NULL_AS_LONG then
				cnvlong = null
			else
				cnvlong = theval
			end if
	End Select

end function

function CnvString(ByVal theVal, ByVal Direction)
	' convert string values from null and back for database operations
	Select Case Direction
		Case "DB_TO_VB"
			if IsNull(theVal) then
				cnvstring = ""
			else
				cnvstring = theval
			end if
		Case "VB_TO_DB"
			if theval = "" then
				cnvstring = null
			else
				cnvstring = theval
			end if
	End Select
end function

function CnvStringSQL(byVal theVal)
	if isnull(theVal) then
		cnvSTringSQL = "''"
	elseif theval = "" then
		cnvSTringSQL = "''"
	else
		cnvstringsql = theval
	end if
end function

function IsStructureTable(ByVal id, ByRef oConn)
	' return true if this is a structure table.  It is a
	' structure table of there exists a column with a non-null
	' MST path, indicating presence of the structure file.
	Dim bRet
	Dim oRS
	sql = SQLtableHasStructures("?")
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pid", 5, 1, 0, id) 
	Set oRS = cmd.Execute 
	'Set oRS = oConn.Execute(SQLColumnByTableIdAndNonNullMstPath(id))
	if oRS.bof and ors.eof then
		bRet = false
	else
		bret = true
	end if
	IsStructureTable = bRet
end function


function  isStructureColumn(ByVal column_id, ByRef oConn)
	' return true if this is a structure column.  It is a
	' structure column if the field is indexed by the cartridge 
	Dim bRet
	Dim oRS
	
	if not isNull(column_id)  then
		on error resume next
		sql = SQLisStructureColumn("?")
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pcolumn_id", 5, 1, 0, column_id) 
		Set oRS = cmd.Execute() 
		
		if err.number <> 0 then
			Response.write err.description & column_id & sql 
		end if
		if oRS.bof and ors.eof then
			bRet = false
		else
			bret = true
		end if
	else
		bRet = false
	end if
	isStructureColumn = bRet
end function



function  isImageColumn(ByVal column_id, ByRef oConn)
	' return true if this is a structure column.  It is a
	' structure column if the field is indexed by the cartridge 
	Dim bRet
	Dim oRS
	
	if not isNull(column_id)  then
		on error resume next
		sql = SQLisImageColumn("?")
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pcolumn_id", 5, 1, 0, column_id) 
		Set oRS = cmd.Execute() 
		
		if err.number <> 0 then
			Response.write err.description & column_id & sql 
		end if
		if oRS.bof and ors.eof then
			bRet = false
		else
			bret = true
		end if
	else
		bRet = false
	end if
	isImageColumn = bRet
end function
function MstFilePath(ByVal id, byref oconn)
	' return mst file path if there is one for this table
	' only looks at columns whose name is "MOL_ID"
	' returns "" if no MOL_ID column, "MOL_ID" if there is a MOL_ID 
	' column, but its MST_FILE_PATH is empty, and the path if 
	' there is a MOL_ID column with a value set for MST_FILE_PATH
	Dim bRet
	Dim oRS
	Set oRS = oConn.Execute(SQLColumnByTableIdAndColumnName(id, "MOL_ID"))
	if oRS.bof and ors.eof then
		bRet = ""
	else
		if IsNull(oRS("MST_FILE_PATH")) or oRS("MST_FILE_PATH") = "" Then
			bret = "MOL_ID"
		Else
			bret = oRS("MST_FILE_PATH")
		end if
	end if
	MstFilePath = bRet
end function

function AuthorizedTables(byval SchemaName)

	' returns a dictionary of the tables the user is authorized to select from
	' in SchemaName.  If SchemaName = "", returns all tables
	' user is authorized to select from from all schemas
	on error resume next
	Dim oUserConn, oDict, oRS
	Set oUserConn = UserConnection

	' DbgBreak "dbkey_admin_tools = " & "biosar_browser"
	Set oDict = Server.CreateObject("Scripting.Dictionary")
	dim sSQL
	' find grants due to roles
	sSQL = "select * from role_tab_privs where upper(privilege) = 'SELECT'"
	if SchemaName <> "" THen
		sSQL = sSQL & " and owner ='" & SchemaName & "'"
	end if
	'set cmd = Server.CreateObject("adodb.command")
	'cmd.ActiveConnection =  oUserConn
	'cmd.CommandType = adCmdText
	'cmd.CommandText = sql
	'cmd.Parameters.Append cmd.CreateParameter("pSchemaName", 200, 1, Len(SchemaName)+ 1, SchemaName)
	Set oRS = oUserConn.Execute(sSQL)
	
	do until ors.eof
		
		' there could be duplicates due to overlapping roles
		if not oDict.Exists(cstr(oRS("OWNER") & "." & oRS("TABLE_NAME"))) then
			oDict.Add cstr(oRS("OWNER") & "." & oRS("TABLE_NAME")), ""
		end if
		oRS.MoveNext
	loop
	' now do the same for grants due to user
	sSQL = "select * from user_tab_privs where upper(privilege) = 'SELECT'"
	if SchemaName <> "" THen
		sSQL = sSQL & " and owner = '" & SchemaName & "'"
	end if
	Set oRS = oUserConn.Execute(sSQL)
	do until ors.eof
		' there could be duplicates due to overlapping roles
		if not oDict.Exists(cstr(oRS("OWNER") & "." & oRS("TABLE_NAME"))) then
			oDict.Add cstr(oRS("OWNER") & "." & oRS("TABLE_NAME")), ""
		end if
		oRS.MoveNext
	loop
	Set AuthorizedTables = oDict
end function

Function getSchemaTables(ByVal schemaName, byRef oSchemaConn)
	Set oDict = Server.CreateObject("Scripting.Dictionary")
	sql = "select * from user_tables"
	Set oRS = oSchemaConn.Execute(sql)
	if not (oRS.bof and oRS.eof) then
		do until oRS.eof
			' there could be duplicates due to overlapping roles
			if not oDict.Exists(cstr(schemaName & "." & oRS("TABLE_NAME"))) then
				oDict.Add cstr(schemaName & "." & oRS("TABLE_NAME")), ""
			end if
			oRS.MoveNext
		loop
	end if
	
	sql = "select * from user_views"
	Set oRS = oSchemaConn.Execute(sql)
	if not (oRS.bof and oRS.eof) then
		do until oRS.eof
			' there could be duplicates due to overlapping roles
			if not oDict.Exists(cstr(schemaName & "." & oRS("VIEW_NAME"))) then
				oDict.Add cstr(schemaName & "." & oRS("VIEW_NAME")), ""
			end if
			oRS.MoveNext
		loop
	end if
	
	Set getSchemaTables = oDict
End Function

function DisplaytypeIdsForColumn(ByRef oRSColumn)
	' get a list of display type ids appropriate for column
	Dim sDatatype
	Dim sRet
	on error resume next
	sDatatype = oRSColumn("DATATYPE")

	If InStr(sDataType, "NUMBER") <> 0 Then
		Dim lLookup
		lLookup = CnvLong(oRSColumn("LOOKUP_COLUMN_ID"), "DB_TO_VB")
		' if a lookup id exists, then display a select statement
		If lLookup > -1 Then
			sRet = "4"
		Else
			sRet = "1,2,12"
		End If
	ElseIf InStr(sDataType, "DATE") <> 0 Then
		' display date picker or date
		sRet = "6,5"
	ElseIf InStr(sDataType, "VARCHAR") <> 0 Then
		Dim lLength
		lLength = CnvLong(oRSColumn("LENGTH"), "DB_TO_VB")
		If lLength > 1 Or lLength = NULL_AS_LONG then
			sRet = "1,2,12"
		Else
			' if length is 1, display a checkbox
			sRet = "3"
		End If
	ElseIF InStr(sDataType, "BLOB") <> 0 Then
		sRet = "13,14"
	ElseIF InStr(sDataType, "CLOB") <> 0 Then
		sRet = "14"
	Else	
		' default is textbox
		sRet = "1"
	End If 
	DisplaytypeIdsForColumn = sRet
end function

function HasRecords(ByRef ors)
	
	'LJB 6/18/2002 - without the on error resume next many places would hang
	on error resume next
	' return true if a recordset has records
	if ors is nothing then
		HasRecords = false
	else	
		if (ors.bof and ors.eof) then
			HasRecords = false
		else
			HasRecords = true
		end if
	end if
	if err.number <> 0 then
		HasRecords = false
	end if
end function

function FormatPrettify(byval sraw)
	' make suggested display names
	Dim sRet, n
	' replace underscores with spaces
	sRet = Replace(sraw, "_", " ")
	sRet = lCase(sret)
	' uppercase the first letter
	sRet = UCase(Left(sRet, 1)) & mid(sRet, 2)
	FormatPrettify = Sret
end function

function RolifyName(byVal sRaw, byVal formId)
	' make a name acceptable for use in an Oracle role name
	Dim sRet, lMaxNameLength
	lMaxNameLength = 29 - Len("_" & cstr(formId))
	' make sure names are backwards-compatible with old names
	' (this is the only processing we did <7.2.29)
	sRet = Replace(sRaw, "'", "")
	sRet = Replace(sRet, " ", "")
    ' uppercase everything
    sret = UCase(sret)
    ' now remove any other non-alphabetic, non-numeric, and non-underscore characters
    Dim I, sCopy, sChar 
    sCopy = sret
    sret = ""
    Dim lCount
    lCount = 0
    For I = 1 To Len(sCopy)
        sChar = Mid(sCopy, I, 1)
        If ((Asc(sChar) > 64 And Asc(sChar) < 91) Or (Asc(sChar) > 47 And Asc(sChar) < 58) Or Asc(sChar) = 125) and lCount < lMaxNameLength Then
            If lCount = 0 And Not (Asc(sChar) > 64 And Asc(sChar) < 91) Then
                ' make sure first character is alphabetic
                sret = sret & "N"
            End If
            sret = sret & sChar
            lCount = lCount + 1
        End If
    Next
	RolifyName = sRet & "_" & formId
ENd function


function AdoDatatypeToOracleDatatype(ByVal adoType)
	' examine an ADO field and convert its
	' datatype to an Oracle datatype
	Dim sRet
	Select Case CLng(adoType)
		Case adDate
			sRet = "DATE"
		Case adDecimal, adDouble, adInteger, adNumeric
			sRet = "NUMBER"
		Case adLongVarBinary
			sRet = "BLOB"
		Case adLongVarChar
			sRet = "CLOB"
		Case Else	
			sRet = "VARCHAR2"
	End Select
	AdoDatatypeToOracleDatatype = sRet
end function

function DbgBreak(byval msg)
	' output message and die
	Response.write msg
	Response.End
end function

function UserHasPrivilege(byval dbkey, byval formgroup, byval username, byval privilegename)
	' determine whether or not a user is enabled for a 
	' given privilege
	dim bRet
	bRet = false
	Dim oConn, oDict, oRS
	Set oConn = UserConnection
	dim privtable
	privtable = dbKey & "_PRIVILEGES"
	dim sSQL
	sSQL = "select " & privilegename & " from " & privtable & ", " & _
		   "security_roles where " & _
		   privtable & ".role_internal_id = security_roles.role_id" & _
		   " and security_roles.role_name in (" & _
		   getSingleUserSQL(dbkey, formgroup, "biosar_browser_privileges", username) & ")"
	Set oRS = oUserConn.Execute(sSQL)
	do until ors.eof
		' see if the privilege we want is set to 1
		if ors(privilegename) = 1 then
			bRet = true
			exit do
		end if
		oRS.MoveNext
	loop
	UserHasPrivilege = bRet
end function


Sub DeleteFromReports(formgroup_id)
	
	on error resume next
	Dim ConnStr
	ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source= " & Application("ReportDBPath")
	Set RPTConn = Server.CreateObject("ADODB.Connection")
	RPTConn.Open ConnStr 
	sql = "delete from Reports where reporttype_id=" & formgroup_id
	RPTConn.Execute(sql)
	sql = "delete from ReportTypes where reporttype_id=" & formgroup_id
	RPTConn.Execute(sql)
	RPTConn.Close
	Set RPTConn = Nothing
	
End Sub

Function ChopString(ByVal aString, ByVal Numchars)
	if len(aString) > NumChars then
		chopstring = left(astring, numchars - 4) & " ..."
	else
		chopstring = astring
	end if
End Function

Function CreateFormGroup(Name, Description)
	' create a new formgroup and forms with the given name
	' add a new formgroup
	Dim oRSNewFormGroup
	Set oRSNewFormGroup = Server.CreateObject("ADODB.Recordset")	
	oRSNewFormGroup.Open SQLFormgroupById(NULL_AS_LONG), oConn, 1, 3
	oRSNewFormGroup.AddNew
	oRSNewFormGroup("FORMGROUP_NAME") = chopstring(Name, 49)
	oRSNewFormGroup("DESCRIPTION") = description
	oRsNewFormgroup("CREATED_DATE") = CDate(Now)
	oRsNewFormgroup("IS_PUBLIC") = "N"
	oRsNewFormgroup("USER_ID") = UCase(Session("UserName" & "biosar_browser"))
	oRSNewFormGroup.Update
	Dim lNewFGId
	lNewFGId = oRSNewFormGroup("FORMGROUP_ID")
	oRSNewFormGroup.close
	' add 3 new forms for this formgroup
	Dim n
	Dim oRSNewForm
	Set oRSNewForm = Server.CreateObject("ADODB.Recordset")
	oRSNewForm.Open SQLFormsByFormgroupId(lNewFGId), oConn, 1, 3
	For n = 1 to 3
		oRSNewForm.AddNew
		oRSNewForm("FORMGROUP_ID") = lNewFGId
		oRSNewForm("FORMTYPE_ID") = n
 		' sName = Name & " - " 
		sName = lNewFGId & " - "
		Select Case n
			Case 1
				sName = sName & "Query"
			case 2
				sName = sName & "List"
			case 3
				sName = sName & "Detail"
		End Select
		oRSNewform("FORM_NAME") = sName
	next
	oRSNewform.UpdateBatch
	oRSNewForm.close
	CreateFormGroup = lNewFGId
end function


Function SAR_CryptVBS(Text, Key)
	if  isNull(Text)  then exit function
	Text = SAR_shiftChr(Text, -1)
	KeyLen = Len(Key)
	For i = 1 To Len(Text)
		KeyPtr = (KeyPtr + 1) Mod KeyLen
		sTxtChr = Mid(Text, i, 1)
		wTxtChr = Asc(stxtchr)
		wKeyChr = Asc(Mid(Key, KeyPtr + 1, 1))
		CryptKey = Chr(wTxtChr Xor wKeyChr)
		hold = hold & CryptKey
	Next
	SAR_CryptVBS =  SAR_shiftChr(hold, 1)
End Function

Function  SAR_shiftChr(str, s)
	for i = 1 to len(str)
		hold = hold & Chr(Asc(Mid(str,i,1))+ s)
	Next
	SAR_shiftChr = hold
End function

Function UserFormgroupsExist(dbkey, formgroup)
	if Session("UserFormgroupsExist") = true then 
		theReturn = true
	else
		theReturn = ""
		Set DataConn = getNewConnection(dbkey, formgroup, "base_connection")
		Set RS = Server.CreateObject("ADODB.RECORDSET")
		Set cmd = Server.CreateObject("ADODB.Command")
		Cmd.ActiveConnection = DataConn
		cmd.CommandType = adCmdText
		user_name = UCase(Session("UserName" & dbkey))
		valid_user_public_formgroup_ids= getAllUserPublicFormgroups(dbkey, formgroup, Session("UserName" & dbkey), DataConn)
		if valid_user_public_formgroup_ids <> "" then
			sql = "Select Count(FORMGROUP_ID) as theCount from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)=? OR  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & valid_user_public_formgroup_ids & ")"
		else
			sql = "Select Count(FORMGROUP_ID) as theCount from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)=?"
		end if
		on error resume next
		cmd.commandtext =sql
		Cmd.Parameters.Append Cmd.CreateParameter("USER_NAME", 201, 1, len(user_name), user_name)
		RS.Open cmd
		if Not err.number <> 0 then
			NumFormgroups = RS("theCount")
			' JHS 4/23/2007 CLng is a better choice here
			'If CInt(NumFormgroups) > 0 then
			If CLng(NumFormgroups) > 0 then
				theReturn = true
				Session("UserFormgroupsExist") = true 
			else
				theReturn = false
			end if
		else
			theReturn = false
		end if
		CloseRS(RS)
		CloseConn(DataConn)
	end if
	UserFormgroupsExist = theReturn	
End Function

Function getAllUserPublicFormgroups(dbkey, formgroup, user_name,byref User_Conn)
	
	Dim RS
	Dim DBA_Conn, SEC_Conn
	Dim cmd
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	Set cmd = Server.CreateObject("ADODB.Command")
	on error resume next
	if not isObject(User_conn) then
		set user_conn = UserConnection
	end if
	Cmd.ActiveConnection = User_Conn
	cmd.CommandType = adCmdText
	sql = "SELECT granted_role FROM user_role_privs UNION SELECT granted_role FROM role_role_privs UNION SELECT role FROM role_role_privs"
	cmd.commandtext =sql
	RS.open cmd 
	
	If not (RS.BOF and RS.EOF)then
		RS.MoveFirst		
		priv_table = UCase(Application("PRIV_TABLE_NAME"))
		Do While Not RS.EOF	
			role_name = RS("Granted_Role")
			if Not role_name = "BIOSAR_MASTER_ROLE" then
				if roles_list <> "" then
					roles_list = roles_list & "," & "'" & role_name& "'"
				else
					roles_list = "'" & role_name& "'"
				end if
			end if
			RS.MoveNext
		Loop
		RS.Close
	end if
	if trim(roles_list) <> "" then
		sql = "Select CS_SECURITY.SECURITY_ROLES.ROLE_NAME From CS_SECURITY.SECURITY_ROLES,CS_SECURITY." & priv_table &_
				" WHERE  CS_SECURITY.SECURITY_ROLES.ROLE_ID = CS_SECURITY." & priv_table & ".role_internal_id" &_
				
				" AND CS_SECURITY." & priv_table & ".IS_FORMGROUP_ROLE=1" &_
				" AND CS_SECURITY.SECURITY_ROLES.ROLE_NAME IN (" & roles_list &  ")" 
		
		Set RS = USER_CONN.Execute(sql)

		if not (RS.BOF and RS.EOF) then
			do While Not RS.EOF
				formgroup_role_name = RS("ROLE_NAME")
				formgroup_role_name_array = split(formgroup_role_name, "_", -1)
				formgroup_id = formgroup_role_name_array(UBound(formgroup_role_name_array))
				if valid_formgroup_ids <> "" then
					valid_formgroup_ids = valid_formgroup_ids & "," & formgroup_id
				else
					valid_formgroup_ids = formgroup_id
				end if
			RS.MoveNext
			loop
		end if
	else
		valid_formgroup_ids= ""
	end if
	'CloseConn(DBA_Conn)
	
	getAllUserPublicFormgroups = valid_formgroup_ids
End Function	


'Get nested roles for a given role name 

Function getNestedRoles(formgroup, ByRef USER_CONN, ByVal role_name) 
	Dim sRoles, sSQL
	Dim bNoParentFound
	Dim oRecordset
	Dim lCurrRole
	Dim RS
	Set RS = Server.CreateObject("ADODB.RECORDSET")
	Dim cmd
	Set cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = USER_CONN
	cmd.CommandType = adCmdText
	
	lCurrRole = "'" & UCase(role_name) & "'"
	
	

	bNoParentFound = False
	
	Do While Not bNoParentFound
		sSQL = "select granted_role from role_role_privs where upper(role) =?"
		cmd.commandtext =sSQL
		Cmd.Parameters.Append Cmd.CreateParameter("lCurrRole", 201, 1, len(lCurrRole), lCurrRole)
		'on error resume next
		RS.Open cmd
		Cmd.Parameters.Delete "lCurrRole"
		lCurrRole = ""
		
		if (RS.BOF and RS.EOF) then
			bNoParentFound = true
		Else
			Do While Not RS.EOF
				
				if sRoles <> "" then
					sRoles = sRoles & "," & "'" & RS("GRANTED_ROLE")  & "'"
				else
					sRoles = "'" & RS("GRANTED_ROLE") & "'"
				end if
				
				if lCurrRole <> "" then
					lCurrRole = lCurrRole & "," & "'" & RS("GRANTED_ROLE") & "'"
				else
					lCurrRole = "'" & RS("GRANTED_ROLE") & "'"
				end if
				
				RS.MoveNext
			Loop
			RS.Close
			
		End If
		
	Loop
	
	CloseRS(RS)
	
	getNestedRoles = sRoles
End Function

Sub setFormGroupInfo(formgroup, dbkey, DataConn, RS)
	
	sql = "select * from BIOSARDB.db_formgroup where formgroup_id =" &  formgroup
	Set RS = DataConn.Execute(sql)
	if Not (RS.BOF and RS.EOF) then
		RS.MoveFirst
		currentLoadedList = Application("form_group_array" & "biosar_browser")
		ItemFound = isInAppList(currentLoadedList, formgroup)
		if ItemFound = false then
			currentCount = UBound(currentLoadedList)
			redim currentLoadedList(currentCount +1)
			currentLoadedList(currentCount +1) = formgroup
		end if
		Application.Lock
			Application("formgroup_full_name" & dbkey & formgroup_id)=RS("FORMGROUP_NAME")
			Application("FORMGROUP_OWNER" & dbkey & formgroup) =RS("USER_ID")
			Application("FORMGROUP_IS_PUBLIC" & dbkey & formgroup) =RS("IS_PUBLIC")
			Application("DESCRIPTION" & dbkey & formgroup) =RS("DESCRIPTION")
			Application("FORMGROUP_NAME" & dbkey & formgroup) =RS("FORMGROUP_NAME")
			Application("BASE_TABLE" & dbkey & formgroup) =RS("BASE_TABLE_ID")
			Application("form_group_array"& "biosar_browser")=currentLoadedList
			Application("FORMGROUP_BIOVIZ_INTEGRATION"& formgroup)=RS("BIOVIZ")
			Application("FORMGROUP_EXCEL_INTEGRATION"& formgroup)=RS("EXCEL")
		Application.UnLock
	end if
	RS.Close
		
		
end Sub

function isInAppList(currentList, newItem)
	bItemFound = false
	for i = 0 to UBound(currentList)
		if UCase(Trim(currentList(i)))= UCase(trim(newItem)) then
		
			bItemFound = true
			exit for
		end if
	next
	
isInAppList = bItemFound
end function


function getAllPublicFormgrupsNotOwned (dbkey, formgroup, user_name,byref User_Conn, formgroup_id_list)
		sql = SQLGrantedFormgroupsWhereNotOwner(ucase(Session("UserName" & Session("dbkey_admin_tools"))),valid_user_public_formgroup_ids)
		Set RS = User_Conn.Execute(sql)
		if Not (RS.BOF and RS.EOF) then
			do while not rs.eof
				if valid_formgroup_ids <> "" then
					valid_formgroup_ids = valid_formgroup_ids & "," & RS("formgroup_id")
				else
					valid_formgroup_ids = RS("formgroup_id")
				end if
			RS.MoveNext
			loop
		else
		valid_formgroup_ids=""
		end if
	  getAllPublicFormgrupsNotOwned = valid_formgroup_ids
end function

Function addto_string(the_list,new_item,delimiter)
	bExists = false
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

	addto_string = the_new_list
End function

Function GetAllBioSARRoles(dbkey, formgroup)
	Dim role_string
	role_string=""
	pPrivTableName="biosar_browser_privileges"
	if not isObject(oConn) then
		Set sysConn = SysConnection
	end if
	sql="Select role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
		" where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
		" and cs_security.biosar_browser_privileges.is_formgroup_role != 1"

	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  SysConnection
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	If Not(RS.BOF and RS.EOF) then
		Do while Not RS.Eof
			if role_string <> "" then
				role_string = role_string & ",'" & RS("ROLE_NAME") & "'"
			else
				role_string = "'" & RS("ROLE_NAME") & "'"
			end if
			RS.MoveNext
		loop
		RS.Close
	end if
	
	GetAllBioSARRoles=role_string
End Function

Function GetCreateFGBioSARRoles(dbkey, formgroup)
	Dim role_string
	role_string=""
	pPrivTableName="biosar_browser_privileges"
	if not isObject(oConn) then
		Set sysConn = SysConnection
	end if
	sql="Select role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
		" where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
		" and cs_security.biosar_browser_privileges.is_formgroup_role != 1"
		
		

	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  SysConnection
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	If Not(RS.BOF and RS.EOF) then
		Do while Not RS.Eof
			if role_string <> "" then
				role_string = role_string & ",'" & RS("ROLE_NAME") & "'"
			else
				role_string = "'" & RS("ROLE_NAME") & "'"
			end if
			RS.MoveNext
		loop
		RS.Close
	end if
	
	GetCreateFGBioSARRoles=role_string
End Function


Function GetFGBioSARRoles(dbkey, formgroup)
	Dim role_string
	role_string=""
	pPrivTableName="biosar_browser_privileges"
	if not isObject(oConn) then
		Set sysConn = SysConnection
	end if
	sql="Select role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
		" where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
		" and cs_security.biosar_browser_privileges.is_formgroup_role = 1" 

	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  SysConnection
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	If Not(RS.BOF and RS.EOF) then
		Do while Not RS.Eof
			if role_string <> "" then
				role_string = role_string & ",'" & RS("ROLE_NAME") & "'"
			else
				role_string = "'" & RS("ROLE_NAME") & "'"
			end if
			RS.MoveNext
		loop
		RS.Close
	end if
	
	GetFGBioSARRoles=role_string
End Function

Function getLookupDataType(byRef oConn,lColID)
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = "select datatype from db_column where column_id = ?"
	cmd.Parameters.Append cmd.CreateParameter("plColID", 5, 1, 0, lColID) 
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	if not (RS.EOF and RS.BOF)then
		theReturn = RS("DATATYPE")
		RS.CLOSE
	end if
	getLookupDataType=theReturn

End Function

Function getLookupContentType(byRef oConn,lColID)
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = "select content_type_id from db_column where column_id = ?"
	cmd.Parameters.Append cmd.CreateParameter("plColID", 5, 1, 0, lColID) 
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	if not (RS.EOF and RS.BOF)then
		theReturn = RS("content_type_id")
		RS.CLOSE
	end if
	getLookupContentType=theReturn

End Function

Function getLookupIndexType(byRef oConn,lColID)
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd.CommandText = "select index_type_id from db_column where column_id = ?"
	cmd.Parameters.Append cmd.CreateParameter("plColID", 5, 1, 0, lColID) 
	Set RS = server.CreateObject("adodb.recordset")
	RS.Open cmd
	if not (RS.EOF and RS.BOF)then
		theReturn = RS("index_type_id")
		RS.CLOSE
	end if
	getLookupContentType=theReturn

End Function

Sub setQueryAndHitlistVars()
	if Not OverrideManageQueries = true then
		'this function checks whether the user has any queries or hitlist for ANY of their formgroups or public formgroups
		'The core function only checks for a single formgroup which is not correct for biosar. 
		user_id = session("username" & "biosar_browser")
		if not Session("BS_CHECK_HITLIST_AND_QUERY_VARS") = "done" then
			Set RS  = Server.CreateObject("adodb.recordset")
			GetUserSettingsConnection dbkey, "base_form_group", "base_connection"
	'populate  query history  vars
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  UserSettingConn
			cmd.CommandType = adCmdText
			sql = "select  formgroup, query_id,is_public from biosardb.db_query where query_name ='HISTORY' and (Upper(user_id)= ?)"
			cmd.CommandText = sql
			cmd.Parameters.Append cmd.CreateParameter("pUserID", 200, 1, Len(user_id)+ 1, UCase(user_id))
			RS.Open cmd
			on error resume next
			'get Query History for all formgroups. This user
			if Not (RS.BOF and RS.EOF) then
				Do while Not RS.EOF
					Session("QueryHistoryExists" & "biosar_browser" & RS("FORMGROUP"))=  RS("QUERY_ID")
					RS.MoveNext
				loop
			end if
			RS.Close
	'populate saved query  vars
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  UserSettingConn
			cmd.CommandType = adCmdText
			sql = "select  formgroup, query_id,is_public from biosardb.db_query where Not query_name  = 'HISTORY' and (Upper(user_id)= ? or is_public = '1')"
			cmd.CommandText =sql
			cmd.Parameters.Append cmd.CreateParameter("pUserID", 200, 1, Len(user_id)+ 1, UCase(user_id))
			RS.Open cmd
			on error resume next
			'get Saved Queries for all formgroups. This user
			if Not (RS.BOF and RS.EOF) then
				Do while Not RS.EOF
					Session("SavedQueriesExists" & "biosar_browser" & RS("FORMGROUP"))=  RS("QUERY_ID")
					RS.MoveNext
				loop
			end if
			RS.Close
	'populate hitlist history vars
			Set Cmd = Server.CreateObject("ADODB.COMMAND")
			Cmd.ActiveConnection = UserSettingConn
			Cmd.CommandType = adCmdText
			sql = "SELECT ID,FORMGROUP FROM biosardb.CSDOHITLISTID WHERE CSDOHITLISTID.USER_ID= ? "
			cmd.CommandText =sql
			cmd.Parameters.Append cmd.CreateParameter("pUserID", 200, 1, Len(user_id)+ 1, UCase(user_id))
			RS.Open cmd
			on error resume next
			
			'get Hitlist History for all formgroups. This user
			if Not (RS.BOF and RS.EOF) then
				Do while Not RS.EOF
					Session("HitListHistory" & "biosar_browser" & RS("FORMGROUP"))=  RS("ID")
					RS.MoveNext
				loop
			end if
			RS.Close
	'populate userhitlist vars
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  UserSettingConn
			cmd.CommandType = adCmdText
			sql = "SELECT ID,FORMGROUP FROM biosardb.USERHITLISTID WHERE USERHITLISTID.USER_ID= ? or is_public = '1'"
			cmd.CommandText =sql
			cmd.Parameters.Append cmd.CreateParameter("pUserID", 200, 1, Len(user_id)+ 1, UCase(user_id))

			RS.Open cmd
			on error resume next
			'get saved Hitlists for all formgroups. This user and public
			if Not (RS.BOF and RS.EOF) then
				Do while Not RS.EOF
					Session("HitListExists" & "biosar_browser" & RS("FORMGROUP"))=  RS("ID")
					RS.MoveNext
				loop
			end if
			RS.Close
		
			Session("BS_CHECK_HITLIST_AND_QUERY_VARS") = "done"
		end if
	else
		Session("BS_CHECK_HITLIST_AND_QUERY_VARS") = "done"
	end if
End Sub

Sub DeleteParameters(oCmd)
	Dim i
	on error resume next
	if oCmd.parameters.count > 0 then
		if err then exit sub	 
		for i = oCmd.parameters.count -1 to 0 step -1
			oCmd.parameters.delete(i)
		next
	end if	
End sub


function protectXMLReserved(inputstr)
	if isNull(inputstr) then 
		new_inputstr = inputstr
	else
		new_inputstr = replace(inputstr, "<", "&#60;")
		new_inputstr = replace(new_inputstr, ">", "&#62;")
	end if
	protectXMLReserved = new_inputstr
end function 

function protectCommas(inputstr)
	new_inputstr = replace(inputstr, ",", "&#44;")
	protectCommas = new_inputstr
end function 

function unprotectCommas(inputstr)
	new_inputstr = replace(inputstr, "&#44;", ",")
	unprotectCommas = new_inputstr
end function 

Sub grantTableSelectPriv(schema_name,table_name)
	if Application("TABLE_SECURITY") = 1 then
		Set oSchemaConn = SchemaConnection(schema_name)
		on error resume next
	
		'revoke select privileges from all roles/users
		if request("roles_hidden") <> "" then
			
			vRoles = Split(request("roles_hidden"), ",")
			
			check = request("roles_hidden")
			for each vRole in vRoles
				' split off descriptor
				arr = split(vRole, ": ")
				theUserOrRole = ""
				if not Instr(UCase(arr(0)), "NESTED")>0 then
					if InStr( arr(1), " (")>0 and Instr( arr(1), ")")>0 then
						temp = split(arr(1), " (", -1)
						theUserOrRole = Trim(temp(0))
					else
						theUserOrRole = Trim(arr(1))
					end if
					on error resume next
					
					oSchemaConn.Execute("revoke select on " & table_name & " from " & theUserOrRole)
					err.Clear()
				end if
			next
		end if
	
		'grant this new role to all the roles in Request("current_roles")
		if request("current_roles_hidden") <> "" then
	
			
			vRoles = Split(request("current_roles_hidden"), ",")
			for each vRole in vRoles
				' split off descriptor
				theUserOrRole = ""
				arr = split(vRole, ": ")
				if not Instr(UCase(arr(0)), "NESTED")>0 then
					if InStr( arr(1), " (")>0 and Instr( arr(1), ")")>0 then
						temp = split(arr(1), " (", -1)
						theUserOrRole = Trim(temp(0))
					else
						theUserOrRole = Trim(arr(1))
					end if
					on error resume next
					oSchemaConn.Execute("grant select on " & table_name & " to " & theUserOrRole)
				end if
			next
		end if
	end if
End Sub

function getPowerUserRoles()
	if Not Session("ROLE_WITH_EDIT_ANY_FORMGROUP") <> "" then
		
		set cmd = Server.CreateObject("adodb.command")
		on error resume next
		cmd.ActiveConnection =  oConn
		if err then
			Response.Write "Error while trying to connect as system. Please verify the system credentials in cfserver.ini."
			Response.end
		end if
		cmd.CommandType = adCmdText
		sql = "select role_name from cs_security.security_roles,cs_security.biosar_browser_privileges where biosar_browser_privileges.role_internal_id = security_roles.role_id and edit_any_formgroup='1'"
		cmd.CommandText =sql
		Set RS = Server.CreateObject("adodb.recordset")
		RS.Open cmd
		on error resume next
		
		if Not (RS.BOF and RS.EOF) then
			Do while Not RS.EOF
				if power_roles <> "" then
					power_roles=power_roles & "," &  RS("ROLE_NAME")
				else
					power_roles= RS("ROLE_NAME")
				end if
				RS.MoveNext
			loop
		end if
		RS.Close
		Session("ROLE_WITH_EDIT_ANY_FORMGROUP") = power_roles
	end if
	getPowerUserRoles=Session("ROLE_WITH_EDIT_ANY_FORMGROUP") 
End Function

Sub CheckExistsPowerRole()
	if Not Session("CheckExistsPowerRole") <> "" then
	
		power_roles = getPowerUserRoles()
		power_roles_array = split(power_roles, ",")
		Set sysConn = SysConnection
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  sysConn
		cmd.CommandType = adCmdText
		sql = "select count(*) as theCount from dba_roles where role='BIOSAR_MASTER_ROLE'"		
		cmd.CommandText =sql
		Set RS = Server.CreateObject("adodb.recordset")
		RS.Open cmd
		theCount = RS("theCount")
		RS.Close
		'jhs 4/23/2007 CLng seems like a safer choice here
		'if CInt(theCount) > 0 then
		if CLng(theCount) > 0 then
			Session("CheckExistsPowerRole") = "1"
			for i = 0 to ubound(power_roles_array)
				sysConn.execute("grant BIOSAR_MASTER_ROLE  to " & power_roles_array(i))
			next
		else
			sysConn.Execute("create role BIOSAR_MASTER_ROLE not identified")
			sysConn.Execute("revoke BIOSAR_MASTER_ROLE from system")
			oConn.Execute("insert into cs_security.security_roles (privilege_table_int_id, role_name) " & _
					"values ((select privilege_table_id from privilege_tables where privilege_table_name = 'BIOSAR_BROWSER_PRIVILEGES'), " & _
					"'" & "BIOSAR_MASTER_ROLE" & "'" & ")")
			' add new role to privileges
			set rs = oConn.execute("select role_id from cs_security.security_roles where role_name = '" & sRoleName & "'")
			sNewRoleId = rs("role_id")
			oConn.Execute("insert into cs_security." & sPrivTable & "(role_internal_id, is_formgroup_role) values (" & sNewRoleId & ", 1)")
	
			'grant BS_MASTER_ROLE to all power roles that have edit_any_formgroup. Likely these roles are already granted. But no harm is done regranting.
			
			for i = 0 to ubound(power_roles_array)
				sysConn.execute("grant BIOSAR_MASTER_ROLE  to " & power_roles_array(i))
			next
			Session("CheckExistsPowerRole") = "1"
		end if
	end if
End Sub

'because 8i and 9i limit the number of roles a user can have (exceeding them makes user loose login privilgegs) we need to remove all the roles from the biosar_master_role and grant the selects directly.
Sub ModifyMasterRole()
	If Not Application("MasterRoleClean") <> "" then
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		CheckExistsPowerRole()
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "Select granted_role from dba_role_privs where grantee = 'BIOSAR_MASTER_ROLE'"
		cmd.CommandText = cmd_sql
		Set RS = cmd.Execute() 
		on error resume next
		if Not (RS.EOF and RS.BOF)then
			Do while Not RS.EOF
				formgroup = RS("granted_role")
				if formgroup <> "" then
					temp= split(formgroup, "_", -1)
					formgroup_id = temp(Ubound(temp))
					'grant individual tables to master role
					GrantFGTablesToMaster formgroup_id
					'revoke formgroup from master role
					set cmd2 = Server.CreateObject("adodb.command")
					cmd2.ActiveConnection =  oConn
					cmd2.CommandType = adCmdText
					cmd_sql = "revoke " & formgroup & " from BIOSAR_MASTER_ROLE"
					cmd2.CommandText = cmd_sql
					cmd2.Execute
				end if
				RS.MoveNext
			loop
		end if
		RS.close
	Application.Lock
	Application("MasterRoleClean") = "clean"
	Application.UnLock
	end if
End Sub

'grant table selects to master role
Sub GrantFGTablesToMaster(lGroupID)
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		
		Dim oRSTables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "Select table_name from BIOSARDB.db_vw_formgroup_tables where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		Dim oSchemaConn, sSchemaErrs
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				Dim arrTable
				' 0 - schema name, 1 - table name
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					'grant to master role
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")
					oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close		
		
		'grant select on all lookup tables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "select distinct lookup_table_name as table_name from BIOSARDB.DB_VW_FORMITEMS_COMPACT where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
						on error resume next
						oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")
						oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close
End sub

'grant table selects to biosardb with admin option so biosardb can create views
Sub GrantFGTablesToBIOSARDBwAdmin(lGroupID)
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		
		Dim oRSTables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "Select table_name from BIOSARDB.db_vw_formgroup_tables where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		Dim oSchemaConn, sSchemaErrs
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				Dim arrTable
				' 0 - schema name, 1 - table name
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					'grant to master role
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSARDB with grant option")
					oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close		
		
		'grant select on all lookup tables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "select distinct lookup_table_name as table_name from BIOSARDB.DB_VW_FORMITEMS_COMPACT where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
						on error resume next
						oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSARDB with grant option")
						oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close
End sub
Sub CreateFormgroupRole(sOldName, lGroupID,sNewName)
		if not isObject(oConn) then
			Set oConn = SysConnection
		end if
		if Not sNewName <> "" then
			sql="Select formgroup_name from BIOSARDB.DB_FORMGROUP Where  db_formgroup.formgroup_id = " & lGroupID
			Set RS = oConn.Execute(sql)
			If Not (RS.EOF and RS.BOF) then
				sNewName = rs("formgroup_name")
			end if
			rs.close
		end if
		
		
		
		' now handle formgroup permissions
		Dim sPrivTable
		sPrivTable = "BIOSAR_BROWSER_PRIVILEGES"
		Dim sPrivTableId
		Dim sOldRoleName, sOldRoleId, sNewRoleId
					
		Set rs = oConn.Execute("select privilege_table_id from cs_security.privilege_tables where privilege_table_name = '" & sPrivTable & "'")
		sPrivTableId = rs("privilege_table_id")
		sOldRoleName = RolifyName(sOldName, lgroupid)
		Set rs = oConn.EXecute("select role_id from cs_security.security_roles where role_name = '" & sOldRoleName & "'")
		if HasRecords(rs) then
			sOldRoleId = rs("role_id")
		else
			sOldRoleId = -1
		End if
		' drop existing role
		' will generate error if role doesn't exist
		On Error Resume Next
		oConn.Execute("drop role " & sOldRoleName)
		oConn.Execute("commit")
		On Error Goto 0
		' delete from security_roles
		oConn.Execute("delete from cs_security.security_roles where role_name = '" & sOldRoleName & "'")
		' delete from privileges
		oConn.Execute("delete from cs_security." & sPrivTable & " where role_internal_id = " & sOldRoleID)
		Dim sRoleName
		sRoleName = RolifyName(sNewName, lGroupId)
		' dbgbreak srolename
		oConn.Execute("create role " & sRoleName & " not identified")
		oConn.Execute("revoke " & sRoleName & " from system")
		' add new role to security_roles
		oConn.Execute("insert into cs_security.security_roles (privilege_table_int_id, role_name) " & _
					"values ((select privilege_table_id from privilege_tables where privilege_table_name = 'BIOSAR_BROWSER_PRIVILEGES'), " & _
					"'" & sRoleName & "'" & ")")
		
		' add new role to privileges
		set rs = oConn.execute("select role_id from cs_security.security_roles where role_name = '" & sRoleName & "'")
		sNewRoleId = rs("role_id")
		oConn.Execute("insert into cs_security." & sPrivTable & "(role_internal_id, is_formgroup_role) values (" & sNewRoleId & ", 1)")
		
		' grant select on all tables in formgroup to this new role
		Dim oRSTables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "Select table_name from BIOSARDB.db_vw_formgroup_tables where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		Dim oSchemaConn, sSchemaErrs
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				Dim arrTable
				' 0 - schema name, 1 - table name
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & sRoleName)
					'grant to master role
					CheckExistsPowerRole()
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")
					oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close		
		
		'grant select on all lookup tables
		set cmd = Server.CreateObject("adodb.command")
		cmd.ActiveConnection =  oConn
		cmd.CommandType = adCmdText
		cmd_sql = "select distinct lookup_table_name as table_name from BIOSARDB.DB_VW_FORMITEMS_COMPACT where formgroup_id = ?"
		cmd.CommandText = cmd_sql
		cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
		Set oRSTables = cmd.Execute() 
				
		if HasRecords(oRSTables) THen
			do until oRSTables.eof
				' you need a schema connection to grant selects on
				' the tables in a schema
				if trim(oRSTables("table_name"))<> "" then 'if this is empty it means there are no fields defined for the table - just pass by for now.
					arrTable = Split(oRSTables("table_name"), ".")
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					'if the form isn't public grant the lookup tables to the owner. The formgroup role is created here for the express purpose of granting to roles that have edit_any_form privilege
						if not request("current_roles_hidden") <> "" then
							oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & Session("username" & dbkey))
						end if
						oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & sRoleName)
						'grant lookups to master role
						oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")
						oSchemaConn.Execute("commit")
					if err.number <> 0 then 
						if sschemaerrs <> "" then
							sschemaerrs = sschemaerrs & ", "
						end if
						sSchemaErrs = sschemaerrs & arrTable(0)
						err.clear
					end if
				end if
				oRSTables.movenext
			loop
		end if
		oRSTables.close
		'grant this new role to all the roles in Request("current_roles")
			if request("current_roles_hidden") <> "" then
				dim vRoles
				vRoles = Split(request("current_roles_hidden"), ",")
				for each vRole in vRoles
					' split off descriptor
					dim arr
					arr = split(vRole, ": ")
					oConn.Execute("grant " & sRoleName & " to " & arr(1))
				next
			end if
		if Application("BIOVIZ_INTEGRATION")=1   then
			GrantSelectOnViews(sRoleName)
		end if
									
End Sub
					
Sub GrantBTtoBiosardb(lGroupID)
	'grant basetable to biosardb for hitlist management
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  oConn
	cmd.CommandType = adCmdText
	cmd_sql = "select table_name from BIOSARDB.DB_FORMGROUP_TABLES where formgroup_id = ? and table_order =1"
	cmd.CommandText = cmd_sql
	cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
	Set oRSTables = cmd.Execute() 
	if HasRecords(oRSTables) THen
			if trim(oRSTables("table_name")<> "") then 'if this is empty it means there are no fields defined for the table - just pass by for now.
				arrTable = Split(oRSTables("table_name"), ".")
				Set oSchemaConn = SchemaConnection(arrTable(0))
				on error resume next
				oSchemaConn.Execute("grant select on " & arrTable(1) & " to biosardb")
				oSchemaConn.Execute("commit")
				if err.number <> 0 then 
					if sschemaerrs <> "" then
						sschemaerrs = sschemaerrs & ", "
					end if
					sSchemaErrs = sschemaerrs & arrTable(0)
					err.clear
				end if
			end if
	end if
	oRSTables.close
end sub

Sub GrantCartIndextoBiosardb(lGroupID)
	if not isObject(sysConn) then
		Set sysConn = SysConnection
	end if
	set cmd = Server.CreateObject("adodb.command")
	cmd.ActiveConnection =  sysConn
	cmd.CommandType = adCmdText
	cmd_sql = "select lookup_table_name, table_name  from biosardb.db_VW_formitems_compact where display_type_name = 'STRUCTURE' and formgroup_id=?"
	cmd.CommandText = cmd_sql
	cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
	Set oRSTables = cmd.Execute() 
	if not (oRSTables.BOF and ORSTables.EOF) then
		Do While Not oRSTables.EOF
			lookup_full_name = oRSTables("lookup_table_name")
			if lookup_full_name <> "" then
				full_name = lookup_full_name
			else
				full_name = oRSTables("table_name")
			end if
			temp = split(full_name, ".", -1)
			schemaname = temp(0)
			table_name = temp(1)
			set cmd2 = Server.CreateObject("adodb.command")
			cmd2.ActiveConnection =  sysConn
			cmd_sql= "SELECT INDEX_NAME FROM CsCartridge.all_csc_indexes WHERE upper(OWNER) = ? AND Upper(TABLE_NAME) = ?"
			cmd2.CommandType = adCmdText
			cmd2.CommandText = cmd_sql
			cmd2.Parameters.Append cmd2.CreateParameter("pSchemaName", 200, 1, Len(schemaname)+ 1, UCase(schemaname))
			cmd2.Parameters.Append cmd2.CreateParameter("pTableName", 200, 1, Len(table_name)+ 1, UCase(table_name))
			Set oRSTables2 = cmd2.Execute() 
			if Not (oRSTables2.BOF and oRSTables2.EOF) then
				Do While not oRSTables2.EOF
					Set oSchemaConn = SchemaConnection(schemaname)
					set cmd3 = Server.CreateObject("adodb.command")
					cmd3.ActiveConnection =  oSchemaConn
					index_name ="CsCartridge." & schemaname & "_" &  oRSTables2("INDEX_NAME")
					cmd_sql= "Grant select on " & index_name & " to biosardb"
					cmd3.CommandType = adCmdText
					cmd3.CommandText = cmd_sql
					cmd3.execute
					cmd_sql= "Grant select on " & table_name & " to biosardb"
					cmd3.CommandText = cmd_sql
					cmd3.execute
					oRSTables2.MoveNext
				loop
				oRSTables2.Close
			end if
		oRSTables.MoveNext
		loop
		oRSTables.Close
		end if
end sub

Sub SetIsPublicFlag(lgroupID, isPublic,new_name, new_desc)
	Dim sName, sDesc,sPublic
	sName = Replace(new_name, "'", "''")
	sDesc = Replace(new_desc, "'", "''")
				
	if isPublic = "" then
		sPublic = "N"
	else
		if isPublic = true or isPublic = "Y" then
			sPublic= "Y"
		else
			sPublic = "N"
		end if
	end if
	' DbgBreak "is_public = " & request("is_public") & ", sPublic = " & sPublic	
	oConn.Execute SQLUpdateFormgroup(sName, sDesc, sPublic, lgroupId)
End Sub		




Sub checkFGRole(dbkey, formgroup, DataConn)
	if DataConn.State <> 1 then
		Set DataConn = getNewConnection(dbkey, formgroup, "base_connection")
	end if
	sql="Select formgroup_name from BIOSARDB.DB_FORMGROUP Where  db_formgroup.formgroup_id = " & formgroup
	Set RS = DataConn.Execute(sql)
	If Not (RS.EOF and RS.BOF) then
		formgroup_name = rs("formgroup_name")
	end if
	rs.close
	sRoleName = RolifyName(formgroup_name, formgroup)
	sql="Select count(*) as theCount from cs_security.security_roles Where  role_name= '" & sRoleName & "'"
	Set RS = DataConn.Execute(sql)
	If Not (RS.EOF and RS.BOF) then
		theCount = rs("theCount")
	end if
	RS.Close
	if not CLng(theCount)> 0 then
		CreateFormgroupRole sRoleName,formgroup,sRoleName
	end if
End Sub		

Sub SetBioVizIntegration (lGroupID, enable_bioviz)
	if enable_bioviz = "1" then
		enable_bioviz=1
	end if
	if enable_bioviz = "0" then
		enable_bioviz=0
	end if
		
	if enable_bioviz = "" then enable_bioviz = 0
	Dim oRSNewFormGroup
	Set oRSNewFormGroup = Server.CreateObject("ADODB.Recordset")	
	oRSNewFormGroup.Open SQLFormgroupById(lGroupID), oConn, 1, 3
	oRSNewFormGroup("BIOVIZ") = enable_bioviz
	oRSNewFormGroup.Update
	oRSNewFormGroup.close
	Application.Lock
		Application("FORMGROUP_BIOVIZ_INTEGRATION"& lGroupID)=enable_bioviz
	Application.UnLock
	if enable_bioviz = 1 then
		GrantFGTablesToBIOSARDBwAdmin(lGroupID)
	end if
End Sub
Sub SetExcelIntegration (lGroupID, enable_excel)
	if enable_excel = "1" then
		enable_excel=1
	end if
	if enable_excel = "0" then
		enable_excel=0
	end if
		
	if enable_excel = "" then enable_excel = 0
	Dim oRSNewFormGroup
	Set oRSNewFormGroup = Server.CreateObject("ADODB.Recordset")	
	oRSNewFormGroup.Open SQLFormgroupById(lGroupID), oConn, 1, 3
	oRSNewFormGroup("EXCEL") = enable_excel
	oRSNewFormGroup.Update
	oRSNewFormGroup.close
	Application.Lock
		Application("FORMGROUP_EXCEL_INTEGRATION"& lGroupID)=enable_excel
	Application.UnLock
	
End Sub

Sub DeleteBioVizTableViews(formgroupid)
on error resume next
	sql = "select table_display_name from db_formgroup_tables where formgroup_id=?"
	set oRS = Server.CreateObject("adodb.recordset")
	set cmd = Server.CreateObject("adodb.command")
	Set sConn = schemaConnection("BIOSARDB")
	cmd.ActiveConnection =  sConn
	cmd.CommandType = adCmdText
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("formgorupid", 5, 1, 0, formgroupid) 
	oRS.Open cmd
	If not (oRS.BOF and oRS.EOF) then
		do While not oRS.EOF
			table_name = oRS("table_display_name")
			view_name = table_name & "_" & formgroupid
			sql = "drop view biosardb.""" & table_name & "_" & formgroupid & "_L" & """" 
			sConn.Execute(sql)
			sql = "drop view biosardb.""" & table_name & "_" & formgroupid & "_D" & """" 
			sConn.Execute(sql)
			oRS.MoveNext
		loop
	
	end if
	oRS.close
	
End Sub
		
Sub GrantSelectOnViews(sName)
on error resume next
	if Session("VIEWS_NEEDING_GRANTS") <> "" then
		temp = split(Session("VIEWS_NEEDING_GRANTS"), ",", -1)
		for i = 0 to UBound(temp)
			Set sConn = schemaConnection("BIOSARDB")
			'grant to role
			sql ="grant select on " & quotedstring(temp(i)) & " to " & sName
			
			sConn.execute(sql)
			sConn.execute("commit")
			sql ="grant select on " & quotedstring(temp(i)) & " to " & "BIOSAR_MASTER_ROLE"
			sConn.execute(sql)
			sConn.execute("commit")
			'grant to master role
		next
	end if
	Session("VIEWS_NEEDING_GRANTS")=""
End sub

Function DoBSEncryptDecryptCommand (encryptType,inputText, secretKey)
 Set EncryptedData = Server.CreateObject("CAPICOM.EncryptedData")

 Select Case UCase(EncryptType)
	case "ENCRYPT"

		' Create the EncryptedData object.
		' Set algorithm, key size, and encryption password.
		'algorightm = 0 -> ALGORITHM_RC2
		EncryptedData.Algorithm.Name = 0
		'keylength = 3 ->128
		EncryptedData.Algorithm.KeyLength = 3
		EncryptedData.SetSecret secretKey
		' Now encrypt it.
		EncryptedData.Content = inputText
	
		outText = EncryptedData.Encrypt(CAPICOM_ENCODE_BASE64)
		'CRs and LFs need to be protected since the valid encrypted string contains them, but the ini file strips them.
		'protect CR's
		outText =replace(outText, chr(13), ":CHR13:")
		'protect LF's
		outText =replace(outText, chr(10), ":CHR10:")
	case "DECRYPT"
		' Set decryption password.
		EncryptedData.SetSecret secretKey
		'DoDecryptCommand=EncryptedData.SecretKey
		on error resume next
		'CRs and LFs need to be restored since the valid encrypted string contains them. However you cannot store them in the ini file without protecting them since they get stripped out.
		'unprotect CRs
		inputText =replace(inputText, ":CHR13:", chr(13))
		'unprotect LFs
		inputText =replace(inputText, ":CHR10:", chr(10))
		EncryptedData.Decrypt(inputText)
		'check to see if invalid key error is returned which means the data is not encrypted
		if err.number = -2146881269 then
			outText = "NOT_ENCRYPTED"
			err.clear
		else
			outText = EncryptedData.Content
		end if
   end select
   Set EncryptedData = Nothing
   DoBSEncryptDecryptCommand=OutText
End Function

Function isOracleReserverdWord(byVal colName) 
	if Not isObject(Session("ORACLE_RESERVED_WORDS")) then
		Set Session("ORACLE_RESERVED_WORDS")= OracleReservedWordsDict()
	end if
	if Session("ORACLE_RESERVED_WORDS").Exists(colName) then
		isOracleReserverdWord= true
	else
		isOracleReserverdWord= false
	end if

End Function


Function OracleReservedWordsDict() 
    Dim oDict 
    Set oDict = Server.CreateObject("scripting.dictionary")
    With oDict
        .Add "ACCESS", ""
        .Add "ACCOUNT", ""
        .Add "ACTIVATE", ""
        .Add "ADD", ""
        .Add "ADMIN", ""
        .Add "ADVISE", ""
        .Add "AFTER", ""
        .Add "ALL", ""
        .Add "ALL_ROWS", ""
        .Add "ALLOCATE", ""
        .Add "ALTER", ""
        .Add "ANALYZE", ""
        .Add "AND", ""
        .Add "ANY", ""
        .Add "ARCHIVE", ""
        .Add "ARCHIVELOG", ""
        .Add "ARRAY", ""
        .Add "AS", ""
        .Add "ASC", ""
        .Add "AT", ""
        .Add "AUDIT", ""
        .Add "AUTHENTICATED", ""
        .Add "AUTHORIZATION", ""
        .Add "AUTOEXTEND", ""
        .Add "AUTOMATIC", ""
        .Add "BACKUP", ""
        .Add "BECOME", ""
        .Add "BEFORE", ""
        .Add "BEGIN", ""
        .Add "BETWEEN", ""
        .Add "BFILE", ""
        .Add "BITMAP", ""
        .Add "BLOB", ""
        .Add "BLOCK", ""
        .Add "BODY", ""
        .Add "BY", ""
        .Add "CACHE", ""
        .Add "CACHE_INSTANCES", ""
        .Add "CANCEL", ""
        .Add "CASCADE", ""
        .Add "CAST", ""
        .Add "CFILE", ""
        .Add "CHAINED", ""
        .Add "CHANGE", ""
        .Add "CHAR", ""
        .Add "CHAR_CS", ""
        .Add "CHARACTER", ""
        .Add "CHECK", ""
        .Add "CHECKPOINT", ""
        .Add "CHOOSE", ""
        .Add "CHUNK", ""
        .Add "CLEAR", ""
        .Add "CLOB", ""
        .Add "CLONE", ""
        .Add "CLOSE", ""
        .Add "CLOSE_CACHED_OPEN_CURSORS", ""
        .Add "CLUSTER", ""
        .Add "COALESCE", ""
        .Add "COLUMN", ""
        .Add "COLUMNS", ""
        .Add "COMMENT", ""
        .Add "COMMIT", ""
        .Add "COMMITTED", ""
        .Add "COMPATIBILITY", ""
        .Add "COMPILE", ""
        .Add "COMPLETE", ""
        .Add "COMPOSITE_LIMIT", ""
        .Add "COMPRESS", ""
        .Add "COMPUTE", ""
        .Add "CONNECT", ""
        .Add "CONNECT_TIME", ""
        .Add "CONSTRAINT", ""
        .Add "CONSTRAINTS", ""
        .Add "CONTENTS", ""
        .Add "CONTINUE", ""
        .Add "CONTROLFILE", ""
        .Add "CONVERT", ""
        .Add "COST", ""
        .Add "CPU_PER_CALL", ""
        .Add "CPU_PER_SESSION", ""
        .Add "CREATE", ""
        .Add "CURRENT", ""
        .Add "CURRENT_SCHEMA", ""
        .Add "CURRENT_USER", ""
        .Add "CURSOR", ""
        .Add "CYCLE", ""
        .Add "DANGLING", ""
        .Add "DATABASE", ""
        .Add "DATAFILE", ""
        .Add "DATAFILES", ""
        .Add "DATAOBJNO", ""
        .Add "DATE", ""
        .Add "DBA", ""
        .Add "DBHIGH", ""
        .Add "DBLOW", ""
        .Add "DBMAC", ""
        .Add "DEALLOCATE", ""
        .Add "DEBUG", ""
        .Add "DEC", ""
        .Add "DECIMAL", ""
        .Add "DECLARE", ""
        .Add "DEFAULT", ""
        .Add "DEFERRABLE", ""
        .Add "DEFERRED", ""
        .Add "DEGREE", ""
        .Add "DELETE", ""
        .Add "DEREF", ""
        .Add "DESC", ""
        .Add "DIRECTORY", ""
        .Add "DISABLE", ""
        .Add "DISCONNECT", ""
        .Add "DISMOUNT", ""
        .Add "DISTINCT", ""
        .Add "DISTRIBUTED", ""
        .Add "DML", ""
        .Add "DOUBLE", ""
        .Add "DROP", ""
        .Add "DUMP", ""
        .Add "EACH", ""
        .Add "ELSE", ""
        .Add "ENABLE", ""
        .Add "END", ""
        .Add "ENFORCE", ""
        .Add "ENTRY", ""
        .Add "ESCAPE", ""
        .Add "ESTIMATE", ""
        .Add "EVENTS", ""
        .Add "EXCEPT", ""
        .Add "EXCEPTIONS", ""
        .Add "EXCHANGE", ""
        .Add "EXCLUDING", ""
        .Add "EXCLUSIVE", ""
        .Add "EXECUTE", ""
        .Add "EXISTS", ""
        .Add "EXPIRE", ""
        .Add "EXPLAIN", ""
        .Add "EXTENT", ""
        .Add "EXTENTS", ""
        .Add "EXTERNALLY", ""
        .Add "FAILED_LOGIN_ATTEMPTS", ""
        .Add "FALSE", ""
        .Add "FAST", ""
        .Add "FILE", ""
        .Add "FIRST_ROWS", ""
        .Add "FLAGGER", ""
        .Add "FLOAT", ""
        .Add "FLOB", ""
        .Add "FLUSH", ""
        .Add "FOR", ""
        .Add "FORCE", ""
        .Add "FOREIGN", ""
        .Add "FREELIST", ""
        .Add "FREELISTS", ""
        .Add "FROM", ""
        .Add "FULL", ""
        .Add "FUNCTION", ""
        .Add "GLOBAL", ""
        .Add "GLOBAL_NAME", ""
        .Add "GLOBALLY", ""
        .Add "GRANT", ""
        .Add "GROUP", ""
        .Add "GROUPS", ""
        .Add "HASH", ""
        .Add "HASHKEYS", ""
        .Add "HAVING", ""
        .Add "HEADER", ""
        .Add "HEAP", ""
        .Add "IDENTIFIED", ""
        .Add "IDGENERATORS", ""
        .Add "IDLE_TIME", ""
        .Add "IF", ""
        .Add "IMMEDIATE", ""
        .Add "IN", ""
        .Add "INCLUDING", ""
        .Add "INCREMENT", ""
        .Add "IND_PARTITION", ""
        .Add "INDEX", ""
        .Add "INDEXED", ""
        .Add "INDEXES", ""
        .Add "INDICATOR", ""
        .Add "INITIAL", ""
        .Add "INITIALLY", ""
        .Add "INITRANS", ""
        .Add "INSERT", ""
        .Add "INSTANCE", ""
        .Add "INSTANCES", ""
        .Add "INSTEAD", ""
        .Add "INT", ""
        .Add "INTEGER", ""
        .Add "INTERMEDIATE", ""
        .Add "INTERSECT", ""
        .Add "INTO", ""
        .Add "IS", ""
        .Add "ISOLATION", ""
        .Add "ISOLATION_LEVEL", ""
        .Add "KEEP", ""
        .Add "KEY", ""
        .Add "KILL", ""
        .Add "LABEL", ""
        .Add "LAYER", ""
        .Add "LESS", ""
        .Add "LEVEL", ""
        .Add "LIBRARY", ""
        .Add "LIKE", ""
        .Add "LIMIT", ""
        .Add "LINK", ""
        .Add "LIST", ""
        .Add "LOB", ""
        .Add "LOCAL", ""
        .Add "LOCK", ""
        .Add "LOCKED", ""
        .Add "LOG", ""
        .Add "LOGFILE", ""
        .Add "LOGGING", ""
        .Add "LOGICAL_READS_PER_CALL", ""
        .Add "LOGICAL_READS_PER_SESSION", ""
        .Add "LONG", ""
        .Add "MANAGE", ""
        .Add "MASTER", ""
        .Add "MAX", ""
        .Add "MAXARCHLOGS", ""
        .Add "MAXDATAFILES", ""
        .Add "MAXEXTENTS", ""
        .Add "MAXINSTANCES", ""
        .Add "MAXLOGFILES", ""
        .Add "MAXLOGHISTORY", ""
        .Add "MAXLOGMEMBERS", ""
        .Add "MAXSIZE", ""
        .Add "MAXTRANS", ""
        .Add "MAXVALUE", ""
        .Add "MEMBER", ""
        .Add "MIN", ""
        .Add "MINEXTENTS", ""
        .Add "MINIMUM", ""
        .Add "MINUS", ""
        .Add "MINVALUE", ""
        .Add "MLS_LABEL_FORMAT", ""
        .Add "MLSLABEL", ""
        .Add "MODE", ""
        .Add "MODIFY", ""
        .Add "MOUNT", ""
        .Add "MOVE", ""
        .Add "MTS_DISPATCHERS", ""
        .Add "MULTISET", ""
        .Add "NATIONAL", ""
        .Add "NCHAR", ""
        .Add "NCHAR_CS", ""
        .Add "NCLOB", ""
        .Add "NEEDED", ""
        .Add "NESTED", ""
        .Add "NETWORK", ""
        .Add "NEW", ""
        .Add "NEXT", ""
        .Add "NOARCHIVELOG", ""
        .Add "NOAUDIT", ""
        .Add "NOCACHE", ""
        .Add "NOCOMPRESS", ""
        .Add "NOCYCLE", ""
        .Add "NOFORCE", ""
        .Add "NOLOGGING", ""
        .Add "NOMAXVALUE", ""
        .Add "NOMINVALUE", ""
        .Add "NONE", ""
        .Add "NOORDER", ""
        .Add "NOOVERRIDE", ""
        .Add "NOPARALLEL", ""
        .Add "NORESETLOGS", ""
        .Add "NOREVERSE", ""
        .Add "NORMAL", ""
        .Add "NOSORT", ""
        .Add "NOT", ""
        .Add "NOTHING", ""
        .Add "NOWAIT", ""
        .Add "NULL", ""
        .Add "NUMBER", ""
        .Add "NUMERIC", ""
        .Add "NVARCHAR2", ""
        .Add "OBJECT", ""
        .Add "OBJNO", ""
        .Add "OBJNO_REUSE", ""
        .Add "OF", ""
        .Add "OFF", ""
        .Add "OFFLINE", ""
        .Add "OID", ""
        .Add "OIDINDEX", ""
        .Add "OLD", ""
        .Add "ON", ""
        .Add "ONLINE", ""
        .Add "ONLY", ""
        .Add "OPCODE", ""
        .Add "OPEN", ""
        .Add "OPTIMAL", ""
        .Add "OPTIMIZER_GOAL", ""
        .Add "OPTION", ""
        .Add "OR", ""
        .Add "ORDER", ""
        .Add "ORGANIZATION", ""
        .Add "OSLABEL", ""
        .Add "OVERFLOW", ""
        .Add "OWN", ""
        .Add "PACKAGE", ""
        .Add "PARALLEL", ""
        .Add "PARTITION", ""
        .Add "PASSWORD", ""
        .Add "PASSWORD_GRACE_TIME", ""
        .Add "PASSWORD_LIFE_TIME", ""
        .Add "PASSWORD_LOCK_TIME", ""
        .Add "PASSWORD_REUSE_MAX", ""
        .Add "PASSWORD_REUSE_TIME", ""
        .Add "PASSWORD_VERIFY_FUNCTION", ""
        .Add "PCTFREE", ""
        .Add "PCTINCREASE", ""
        .Add "PCTTHRESHOLD", ""
        .Add "PCTUSED", ""
        .Add "PCTVERSION", ""
        .Add "PERCENT", ""
        .Add "PERMANENT", ""
        .Add "PLAN", ""
        .Add "PLSQL_DEBUG", ""
        .Add "POST_TRANSACTION", ""
        .Add "PRECISION", ""
        .Add "PRESERVE", ""
        .Add "PRIMARY", ""
        .Add "PRIOR", ""
        .Add "PRIVATE", ""
        .Add "PRIVATE_SGA", ""
        .Add "PRIVILEGE", ""
        .Add "PRIVILEGES", ""
        .Add "PROCEDURE", ""
        .Add "PROFILE", ""
        .Add "PUBLIC", ""
        .Add "PURGE", ""
        .Add "QUEUE", ""
        .Add "QUOTA", ""
        .Add "RANGE", ""
        .Add "RAW", ""
        .Add "RBA", ""
        .Add "READ", ""
        .Add "READUP", ""
        .Add "REAL", ""
        .Add "REBUILD", ""
        .Add "RECOVER", ""
        .Add "RECOVERABLE", ""
        .Add "RECOVERY", ""
        .Add "REF", ""
        .Add "REFERENCES", ""
        .Add "REFERENCING", ""
        .Add "REFRESH", ""
        .Add "RENAME", ""
        .Add "REPLACE", ""
        .Add "RESET", ""
        .Add "RESETLOGS", ""
        .Add "RESIZE", ""
        .Add "RESOURCE", ""
        .Add "RESTRICTED", ""
        .Add "RETURN", ""
        .Add "RETURNING", ""
        .Add "REUSE", ""
        .Add "REVERSE", ""
        .Add "REVOKE", ""
        .Add "ROLE", ""
        .Add "ROLES", ""
        .Add "ROLLBACK", ""
        .Add "ROW", ""
        .Add "ROWID", ""
        .Add "ROWNUM", ""
        .Add "ROWS", ""
        .Add "RULE", ""
        .Add "SAMPLE", ""
        .Add "SAVEPOINT", ""
        .Add "SB4", ""
        .Add "SCAN_INSTANCES", ""
        .Add "SCHEMA", ""
        .Add "SCN", ""
        .Add "SCOPE", ""
        .Add "SD_ALL", ""
        .Add "SD_INHIBIT", ""
        .Add "SD_SHOW", ""
        .Add "SEG_BLOCK", ""
        .Add "SEG_FILE", ""
        .Add "SEGMENT", ""
        .Add "SELECT", ""
        .Add "SEQUENCE", ""
        .Add "SERIALIZABLE", ""
        .Add "SESSION", ""
        .Add "SESSION_CACHED_CURSORS", ""
        .Add "SESSIONS_PER_USER", ""
        .Add "SET", ""
        .Add "SHARE", ""
        .Add "SHARED", ""
        .Add "SHARED_POOL", ""
        .Add "SHRINK", ""
        .Add "SIZE", ""
        .Add "SKIP", ""
        .Add "SKIP_UNUSABLE_INDEXES", ""
        .Add "SMALLINT", ""
        .Add "SNAPSHOT", ""
        .Add "SOME", ""
        .Add "SORT", ""
        .Add "SPECIFICATION", ""
        .Add "SPLIT", ""
        .Add "SQL_TRACE", ""
        .Add "STANDBY", ""
        .Add "START", ""
        .Add "STATEMENT_ID", ""
        .Add "STATISTICS", ""
        .Add "STOP", ""
        .Add "STORAGE", ""
        .Add "STORE", ""
        .Add "STRUCTURE", ""
        .Add "SUCCESSFUL", ""
        .Add "SWITCH", ""
        .Add "SYNONYM", ""
        .Add "SYS_OP_ENFORCE_NOT_NULL$", ""
        .Add "SYS_OP_NTCIMG$", ""
        .Add "SYSDATE", ""
        .Add "SYSDBA", ""
        .Add "SYSOPER", ""
        .Add "SYSTEM", ""
        .Add "TABLE", ""
        .Add "TABLES", ""
        .Add "TABLESPACE", ""
        .Add "TABLESPACE_NO", ""
        .Add "TABNO", ""
        .Add "TEMPORARY", ""
        .Add "THAN", ""
        .Add "THE", ""
        .Add "THEN", ""
        .Add "THREAD", ""
        .Add "TIME", ""
        .Add "TIMESTAMP", ""
        .Add "TO", ""
        .Add "TOPLEVEL", ""
        .Add "TRACE", ""
        .Add "TRACING", ""
        .Add "TRANSACTION", ""
        .Add "TRANSITIONAL", ""
        .Add "TRIGGER", ""
        .Add "TRIGGERS", ""
        .Add "TRUE", ""
        .Add "TRUNCATE", ""
        .Add "TX", ""
        .Add "TYPE", ""
        .Add "UB2", ""
        .Add "UBA", ""
        .Add "UID", ""
        .Add "UNARCHIVED", ""
        .Add "UNDO", ""
        .Add "UNION", ""
        .Add "UNIQUE", ""
        .Add "UNLIMITED", ""
        .Add "UNLOCK", ""
        .Add "UNRECOVERABLE", ""
        .Add "UNTIL", ""
        .Add "UNUSABLE", ""
        .Add "UNUSED", ""
        .Add "UPDATABLE", ""
        .Add "UPDATE", ""
        .Add "USAGE", ""
        .Add "USE", ""
        .Add "USER", ""
        .Add "USING", ""
        .Add "VALIDATE", ""
        .Add "VALIDATION", ""
        .Add "VALUE", ""
        .Add "VALUES", ""
        .Add "VARCHAR", ""
        .Add "VARCHAR2", ""
        .Add "VARYING", ""
        .Add "VIEW", ""
        .Add "WHEN", ""
        .Add "WHENEVER", ""
        .Add "WHERE", ""
        .Add "WITH", ""
        .Add "WITHOUT", ""
        .Add "WORK", ""
        .Add "WRITE", ""
        .Add "WRITEDOWN", ""
        .Add "WRITEUP", ""
        .Add "XID", ""
    End With
    Set OracleReservedWordsDict = oDict
End Function

Sub RefreshFormTreeSessVariable()

'wipe out session variable used by main_page, manage_forms and organize forms so that trees will be refreshed
			
	TreeID =1
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =2
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =3
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =5
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =6
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""

End Sub

Sub RefreshTableTreeSessVariable()

'wipe out session variable used by, schema management and organize child tables so that trees will be refreshed
	TreeID =4
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =8
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =9
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""
	TreeID =11
	Set Session("TVNodes" & TreeID)=nothing
	Session("TVNodes" & TreeID)=""

End Sub

Sub AddFormToRootNode(formgroup_id)
	'Session("PRIVATE_CATEGORY_TREE_NODE_ROOT_ID") 
	'Session("PRIVATE_CATEGORY_TREE_TYPE_ID")

	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if
	sql = "select TREE_SEQ.NEXTVAL as seq_id from Dual"
	Set RS2 = sConn.execute(sql)
	seq_id =RS2("seq_id")
	RS2.Close
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = sConn
	Cmd2.CommandType = adCmdText
	Cmd2.CommandText ="insert into tree_item (id, node_id, item_type_id, item_id) values(?, ?,?,?)"
	cmd2.Parameters.Append cmd2.CreateParameter("pseq_id", 5, 1, 0,seq_id ) 
	cmd2.Parameters.Append cmd2.CreateParameter("pnode_id", 5, 1, 0, Session("PRIVATE_CATEGORY_TREE_NODE_ROOT_ID")) 
	cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
	cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
	cmd2.execute
	sConn.Execute("commit")
	RefreshFormTreeSessVariable()
End Sub

Sub RemoveFormToRootNode(formgroup_id)
	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = sConn
	Cmd2.CommandType = adCmdText
	'DGB make sure to remove only from users root node, not from anywhere else
	Cmd2.CommandText ="delete from tree_item where node_id= ? and item_type_id = ? and item_id=?"
	cmd2.Parameters.Append cmd2.CreateParameter("pnode_id", 5, 1, 0, Session("PRIVATE_CATEGORY_TREE_NODE_ROOT_ID")) 
	cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
	cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
	cmd2.execute
	sConn.Execute("commit")
	RefreshFormTreeSessVariable()
End Sub



Sub AddPublicFormToRootNode(formgroup_id)
	'Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID") 
	'Application("PUBLIC_CATEGORY_TREE_TYPE_ID")
	'Application("PUBLIC_CATEGORY_TREE_ITEM_TYPE_ID")
	
	'check if node is already there
	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if

	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = sConn
	Cmd2.CommandType = adCmdText
	Cmd2.CommandText ="Select Count(*) as theCount from  tree_item where node_id= ? and item_type_id = ? and item_id=?"
	cmd2.Parameters.Append cmd2.CreateParameter("pnode_id", 5, 1, 0, Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID")) '
	cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
	cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
	Set RS2 = cmd2.Execute() 
	
	'You don't need to readd the public node anymore as long as there is one other public node.
	Set Cmd3 = Server.CreateObject("ADODB.COMMAND")
	Cmd3.ActiveConnection = sConn
	Cmd3.CommandType = adCmdText
	Cmd3.CommandText = "select  Count(*) as theCount from BIOSARDB.TREE_ITEM inner join BIOSARDB.TREE_node on BIOSARDB.TREE_ITEM.node_id = BIOSARDB.TREE_node.id inner join biosardb.tree_type on BIOSARDB.TREE_node.tree_type_id = BIOSARDB.TREE_TYPE.id where  BIOSARDB.TREE_ITEM.item_id=? and tree_type_id=? and node_id<>?"
	cmd3.Parameters.Append cmd3.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
	cmd3.Parameters.Append cmd3.CreateParameter("pnode_id", 5, 1, 0, Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID")) '
	cmd3.Parameters.Append cmd3.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
	
	Set RS3 = cmd3.Execute() 
	

'on error resume next 'pass by unique constraint error
	if  CLng(RS2("theCount"))=0 and CLng(RS3("theCount"))=0 then
		RS2.Close
		sql = "select TREE_SEQ.NEXTVAL as seq_id from Dual"
		Set RS2 = sConn.execute(sql)
		seq_id =RS2("seq_id")
		RS2.Close
		Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
		Cmd2.ActiveConnection = sConn
		Cmd2.CommandType = adCmdText
		Cmd2.CommandText ="insert into tree_item (id, node_id, item_type_id, item_id) values(?, ?,?,?)"
		cmd2.Parameters.Append cmd2.CreateParameter("pseq_id", 5, 1, 0,seq_id ) 
		cmd2.Parameters.Append cmd2.CreateParameter("pnode_id", 5, 1, 0, Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID")) 
		cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
		cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
		cmd2.execute
		sConn.Execute("commit")
	end if
	RefreshFormTreeSessVariable()
End Sub


Sub RemovePublicFormFromTree(formgroup_id)
	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = sConn
	Cmd2.CommandType = adCmdText
	'DGB this was removing the form regardless of whether it was public
	'Modified the sql to ensure it only deletes public forms
	Cmd2.CommandText ="delete from tree_item where node_id = " & Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID") & " and item_type_id = ? and item_id=?"
	cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "2") 
	cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, formgroup_id) 
	cmd2.execute
	sConn.Execute("commit")
	RefreshFormTreeSessVariable()
End Sub

Sub AddTableToRootNode(table_id)
	'Application("PROJECTS_TREE_NODE_ROOT_ID") = node_id
	'Application("PROJECTS_TREE_TYPE_ID")=1
	'Application("PROJECTS_TREE_ITEM_TYPE_ID")=2
	on error resume next
	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if
	'check to see if the node exists:
	Set Cmd3 = Server.CreateObject("ADODB.COMMAND")
	Cmd3.ActiveConnection = sConn
	Cmd3.CommandType = adCmdText
	Cmd3.CommandText ="select id from tree_item where item_id= ?"
	Cmd3.Parameters.Append Cmd3.CreateParameter("pformgroup_id", 5, 1, 0, table_id) 
	set rs = Cmd3.execute()
	if (rs.eof and rs.bof)then
		rs.close
		sql = "select TREE_SEQ.NEXTVAL as seq_id from Dual"
		Set RS2 = sConn.execute(sql)
		seq_id =RS2("seq_id")
		RS2.Close
		Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
		Cmd2.ActiveConnection = sConn
		Cmd2.CommandType = adCmdText
		Cmd2.CommandText ="insert into tree_item (id, node_id, item_type_id, item_id) values(?, ?,?,?)"
		cmd2.Parameters.Append cmd2.CreateParameter("pseq_id", 5, 1, 0,seq_id ) 
		cmd2.Parameters.Append cmd2.CreateParameter("pnode_id", 5, 1, 0, Application("PROJECTS_TREE_NODE_ROOT_ID")) 
		cmd2.Parameters.Append cmd2.CreateParameter("pitem_type_id", 5, 1, 0, "1") 
		cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, table_id) 
		cmd2.execute
		sConn.Execute("commit")
	end if
	RefreshTableTreeSessVariable()
End Sub


Sub RemoveTableToRootNode(table_id)
	if not isObject(sConn)  then
		Set sConn = SchemaConnection("biosardb")
	end if
	Set Cmd2 = Server.CreateObject("ADODB.COMMAND")
	Cmd2.ActiveConnection = sConn
	Cmd2.CommandType = adCmdText
	Cmd2.CommandText ="delete from tree_item where item_type_id = '1' and item_id=?"
	cmd2.Parameters.Append cmd2.CreateParameter("pformgroup_id", 5, 1, 0, table_id) 
	cmd2.execute
	sConn.Execute("commit")
	RefreshTableTreeSessVariable()
End Sub

function isDup(the_list, new_item)
	bExists = false
	if the_list <> "" then
		temp_array = split(the_list, ",", -1)
		for i = 0 to UBound(temp_array)
			if UCase(temp_array(i)) = UCase(new_item) then
				bExists = true
				exit for
			end if
		next
	end if
	if bExists = false then
		isDup = false
	end if
	if bExists = true then
		isDup = true
	end if
end function


sub nullOutTableCookies()
	if inStr(Request.ServerVariables("HTTP_REFERER"),"admin_table_list.asp")>0 then
		if  request.Cookies("ColumnChanges") <> "" or  request.Cookies("TableChanges") <> "" then
				Colid= request.Cookies("ColumnChanges")
				Colid_array = split(Colid, ",", -1)
				for i = 0 to UBound (Colid_array)
					response.Cookies("ColumnChanges" & lColID)=""
				next
				response.Cookies("ColumnChanges")=""
				response.Cookies("TableChanges")=""
				
		end if%>
		<script language="javascript">
			resetChanges()
		</script>
		<%
			response.Cookies("current_roles_hidden")=""
			response.Cookies("roles_hidden")=""
	end if
end sub



sub nullOutFormCookies()
	
	theReferrer = Request.ServerVariables("HTTP_REFERER")
	
	if (inStr(theReferrer,"user_tables.asp")=0 and  inStr(theReferrer,"user_columns.asp")=0 and  inStr(theReferrer,"user_column_order.asp")=0 and  inStr(theReferrer,"user_choose_child_table.asp")=0 and   inStr(theReferrer,"user_choose_base_table.asp")=0)  then
		
			response.Cookies("current_roles_hidden")=""
			response.Cookies("roles_hidden")=""
			response.Cookies("enable_bioviz")=""
			response.Cookies("enable_excel")=""
			response.Cookies("return_to_form")=""
	end if
end sub
'****************************************************************************************
'*	PURPOSE: Truncates and <span>s a string.  The full string is placed in the title attribute
'*			<span> tag so that it will appear as a pop up help 	                            
'*	INPUT: the string, the truncatation length, the id attribute of <span> 	                    			
'*	OUTPUT: a pair of <span></span> elements containing the truncated content and the title 										
'****************************************************************************************
Function TruncateInSpan(strText, Length, id)
	Dim str
	str = "<span "
	'if the text contents are longer than the desired length 
	'then place the full text contents in the title popup box
	'and truncate the text inside the <span>
	if len(strText) > Length then 
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & left(strText, Length-3) & "..."
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "</span>"
	TruncateInSpan = str
End function	

function getNodePath(node_id,ByRef Conn)
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText =  "biosardb.Tree.GetNodePath"
	Cmd.CommandType = adCmdStoredProc
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, node_id)
	Cmd.Execute
	getNodePath = Cmd.Parameters(0).Value
	
end function
	
function truncateViewName(name,index,fg_specifier,total_length)
	viewNameLength =  total_length - (len(fg_specifier) + len(index))
	tempName = Left(name, viewNameLength)
	FinalName = tempName & index & fg_specifier
	truncateViewName=FinalName
end function

Sub SetASPCookie(cookieName, value)
	Response.Cookies(cookieName) = value
	Response.Cookies(cookieName).expires = DateAdd("n", "2", now())
	
end sub		

Function CheckTableError(theError)
	if instr(err.description, theError)> 0  then
		select case theError
			case "ORA-01437"
			
				theReturn=err.Description & "<br>"
				theReturn= theReturn & "The Security Tab feature will not function with Oracle version 8. Please change the TABLE_SECURITY setting in the cfserver.ini file to 0 so that this feature is not be available."
				
			case "ORA-01458"
				theReturn=err.Description & "<br>"
				theReturn= theReturn & "This error occurs when you are using an incompatible OLEDB driver relative to your Oracle base installation."
		end select
		CheckTableError=theReturn
	else
		CheckTableError=""
	end if
End Function

Function HasParentRelationship(cmd, child_column_id, parent_table_id)

		if not isObject(uConn) then
		Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
		end if
		sql = "Select count(*) theCount from biosardb.db_relationship where table_id=? and child_column_id = ?"
		
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pParentTableID",139, 1, 0, parent_table_id) 
		cmd.Parameters.Append cmd.CreateParameter("pChildColumnID", 139, 1, 0, child_column_id) 
	
		on error resume next
		Set RS2 = cmd.Execute
		uConn.Errors.clear
		cmd.Parameters.Delete "pParentTableID"
		cmd.Parameters.Delete "pChildColumnID"
	
		count = CLng(RS2("theCount"))
		if count > 0 then
			HasParentRelationship = true
		else
		
			HasParentRelationship = false
		end if

End function	


Function isChildLinkingField (cmd, parent_column_id, parent_table_id, formgroup_id)

		if not isObject(uConn) then
			Set uConn = GetNewConnection( _
					Session("dbkey_admin_tools"), _
					"base_form_group", _
					"base_connection")
		end if
		sql = "Select count(*) theCount from biosardb.db_relationship where table_id=? and column_id = ? " &_
			" and child_table_id in(select table_id from biosardb.db_formgroup_tables where formgroup_id=?)"
		
		DeleteParameters(cmd)
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pParentTableID",139, 1, 0, parent_table_id) 
		cmd.Parameters.Append cmd.CreateParameter("pChildColumnID", 139, 1, 0, parent_column_id.value) 
		cmd.Parameters.Append cmd.CreateParameter("pFormgroupID", 139, 1, 0, formgroup_id) 
	
		'on error resume next
		Set RS2 = cmd.Execute
		'uConn.Errors.clear
		'cmd.Parameters.Delete "pParentTableID"
		'cmd.Parameters.Delete "pChildColumnID"
		'cmd.Parameters.Delete "pFormgroupID"
		
		
		
		count = CLng(RS2("theCount").value)
		if count > 0 then
			isChildLinkingField = true
		else
			isChildLinkingField = false
		end if

end function

Function removeIllegalChars(vInput)
	
	temp = Application("illegalFormCharcters")
	for i = 0 to UBound(temp)
		vInput=replace(vInput, temp(i), "")
	next
	removeIllegalChars = vInput
End Function

function MyHTMLEncode(str)
	if isNull(str) then
		MyHTMLEncode = "&nbsp;"
	else
		MyHTMLEncode = server.HTMLEncode(str)
	end if

end function


'Added by JHS for jump form functionality 4/16/2006
Sub getJumpForms(dbkey, formgroup, currbasetable, redirectpage)

		Set DataConn = getNewConnection(dbkey, formgroup, "base_connection")
		
		Set RS = Server.CreateObject("ADODB.RECORDSET")
		Set cmd = Server.CreateObject("ADODB.Command")
		Cmd.ActiveConnection = DataConn
		cmd.CommandType = adCmdText
		user_name = UCase(Session("UserName" & dbkey))
		'Response.Write user_name
		'this seems to get public forms and forms that are specific
		valid_user_public_formgroup_ids= getAllUserPublicFormgroups(dbkey, formgroup, Session("UserName" & dbkey), DataConn)
		'Response.Write valid_user_public_formgroup_ids
		'this should add those forms which are owned by the current user
		'this makes testing wth biosar_admin pretty worthless, since he has access to everything
		
		'jhs 10/29/2007 -commented out because it is overridden in the next if then
		'if valid_user_public_formgroup_ids <> "" then
		'	sql = "Select formgroup_id from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)=? OR  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & valid_user_public_formgroup_ids & ")"
		'else
		'	sql = "Select formgroup_id from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)=?"
		'end if

		'we need to filter by basetable so we need a new parameter
		if valid_user_public_formgroup_ids <> "" then
			sql = "Select formgroup_id, formgroup_name,description from BIOSARDB.DB_FORMGROUP Where (upper(BIOSARDB.DB_FORMGROUP.USER_ID)=? OR  BIOSARDB.DB_FORMGROUP.FORMGROUP_ID IN(" & valid_user_public_formgroup_ids & ")) AND BIOSARDB.DB_FORMGROUP.BASE_TABLE_ID = ?"
		else
			sql = "Select formgroup_id,formgroup_name,description from BIOSARDB.DB_FORMGROUP Where upper(BIOSARDB.DB_FORMGROUP.USER_ID)=? AND BIOSARDB.DB_FORMGROUP.BASE_TABLE_ID = ?"
		end if
		on error resume next
		cmd.commandtext =sql
		
		Cmd.Parameters.Append Cmd.CreateParameter("USER_NAME", 201, 1, len(user_name), user_name)
		Cmd.Parameters.Append Cmd.CreateParameter("BASE_TABLE_ID", adInteger, 1,, currbasetable)
		RS.Open cmd
		'Response.write  rs.EOF
		curqs = Request.ServerVariables("query_string")
		curformgroup = Request("formgroup")
		thehitlistid = Session("hitlistID" & dbkey & formgroup)
		Response.Write "<table ><tr><td class=""query_table_cell_name"" nowrap>Jump to:" 
		Response.Write "<select name=""jumpselect"" onChange=doJumpForm()>"
		while not rs.EOF
			
			if request("formmode") <> "" then
				curformmode = request("formmode")
			else
				curformmode = "list"
			end if
			
			newfg = rs("formgroup_id")
			newqs = Replace(curqs,"formgroup=" & curformgroup,"formgroup=" & newfg)
			'redirectpage = "/biosar_browser/biosar_browser/" & redirectpage 
			'returnfields = newqs			'returnfields = '& "?" & newqs
			'return_location = "/biosar_browser/biosar_browser/biosar_browser_form.asp?formgroup=" & rs("formgroup_id") & "&dbname=biosar_browser&formmode=" & curformmode & "&unique_id=7&commit_type="
			'&unique_id=6&indexvalue=6&
			'jhs 8/3/2007 - changed the following code to use data action to jump to form using restore hitlist action instead of changeform
			 cfpath = "/biosar_browser/biosar_browser/biosar_browser_action.asp?dataaction=restore_hitlist&dbname=biosar_browser&formgroup=" & newfg & "&db_hitlist_item=" &  thehitlistid & ":" & Session("HitListRecordcount" & dbkey & formgroup) & "&restore_type=differentform" & "&formmode=" & curformmode & "&formmode_override=" & curformmode & "&return_data=redirect" '& "&return_location=" & redirectpage '& "&return_fields=" & returnfields  '& "&oldformgroup=" & curformgroup
			 cfpath = cfpath & "&unique_id=" & Session("LastStartingIndex" & dbkey & formgroup)
			 cfpath = cfpath & "&redirectpage=/biosar_browser/biosar_browser/" & redirectpage
			 cfpath = cfpath & "&order_by=" & Session("order_by" & dbkey & formgroup) &  "&sort_direction=" & Session("sort_direction" & dbkey & formgroup)

			'Response.Write "<option value=""changeform.asp?" & newqs & """"
			Response.Write "<option value=""" & cfpath & """"
			if cstr(curformgroup) = cstr(newfg) then
				Response.Write " selected"
			end if
			Response.Write ">" 
			Response.write  rs("formgroup_name") & "-" & rs("description") & "</option>"
			rs.MoveNext
		wend
		Response.Write "</select>"
		Response.Write "</td></tr></table>"
		CloseRS(RS)
		CloseConn(DataConn)




End Sub
%>
