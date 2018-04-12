<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
dim i

bDebugPrint = false
bWriteError = False
strError = "Error:UpdateRole<BR>"

dbKey = Request("dbKey")
RoleName = Request("RoleName")
PrivValueList= Request("PrivValueList")
'CSSPrivValueList= Request("CSSPrivValueList")
PrivNamesList= Request("PrivNamesList")
PrivTableName = Request("PrivTableName")


'AllPrivValuesList = PrivValueList & ", " & CSSPrivValueList
PrivNamesArray = split(PrivNamesList, ",")
PrivValueArray = split(replace(PrivValueList, "'", ""), ",")
 
 
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cfserverasp/help/admin/api/UpdateRole.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(RoleName) then
	strError = strError & "Role Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PrivTableName) then
	strError = strError & "PrivTableName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PrivValueList) then
	strError = strError & "PrivValueList is a required parameter<BR>"
	bWriteError = True
End if
'If IsEmpty(CSSPrivValueList) then
''	strError = strError & "CSSPrivValueList is a required parameter<BR>"
'	bWriteError = True
'End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
'Set Conn = GetCS_SecurityConnection(dbKey)
'Set Cmd = GetCommand(Conn, "CS_SECURITY.UPDATEROLE", adCmdStoredProc)
'Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 100, NULL)		
'Cmd.Parameters.Append Cmd.CreateParameter("PRoleNAME", 200, adParamInput, 50, RoleName) 
'Cmd.Parameters.Append Cmd.CreateParameter("PPrivTableName", 200, adParamInput, 50, PrivTableName)  
'Cmd.Parameters.Append Cmd.CreateParameter("PPrivValueList", 200, adParamInput, 2000, PrivValueList) 
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	'Call ExecuteCmd("CS_SECURITY.UpdateRole")
End if

'RC = Cmd.Parameters("RETURN_VALUE")

if IsNumeric(RC) then
	If RC < 0 then 
		Response.Write RC
		Response.End
	End if
Else
	Response.Write RC
	Response.End	
End if

Set Cmd = Nothing

'Set Cmd = GetCommand(Conn, "CS_SECURITY.MapPrivsToRole", adCmdStoredProc)	
'Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME", 200, adParamInput, 50, "") 
'Cmd.Parameters.Append Cmd.CreateParameter("PPRIVNAME", 200, adParamInput, 30, "") 
'Cmd.Parameters.Append Cmd.CreateParameter("PACTION", 200, adParamInput, 6, "") 
For i = 0 to Ubound(PrivNamesArray)
	if Trim(PrivValueArray(i)) = "1" then
		Action = "GRANT"
	Else
		Action = "REVOKE"
	End if
	'Cmd(0) = RoleName 
	'Cmd(1) = PrivNamesArray(i) 
	'Cmd(2) = Action
	'Call ExecuteCmd("CS_SECURITY.MapPrivsToRole")
	Response.Write  vblf & "--- " & PrivNamesArray(i) & " PRIVS" & vblf
	xx = doMapPrivilege(Conn, Conn, Conn, "ADD", RoleName ,PrivNamesArray(i) )
Next 
Response.Write RC
'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing


Function doMapPrivilege(ByRef REG_Conn, ByRef DBA_Conn, ByRef SEC_Conn, ProcessTypeIn, PrivilegeName, PrivilegeFieldName)
		on error resume next
		if ProcessTypeIn = "ADD" then
			'temp = Split(PrivilegeFieldName, ";",-1)
			'temp2 = Split(temp(0), ".",-1)
			'PrivilegeName = temp2(1)
			'value = Request(temp(0))
			PrivilegeName = PrivilegeFieldName
			value = 1
			if value = "1" then 
				ProcessType = "ADD"
			else
				Exit Function 'no need to do anything, this is a new role
			end if
		else
			temp = Split(PrivilegeFieldName, ".",-1)
			PrivilegeName = temp(2)
			value = Request(PrivilegeFieldName)
			if value = "1" then 
				ProcessType = "ADD"
			else
				ProcessType = "REVOKE"
			end if
		end if
		'Sec_Table= Application("SEC_USERNAME")
		'Appkey = Application("AppKey")
		
	
				

		SEC_SelectPrivileges = "SELECT ON " & UCase(Application("PRIV_TABLE_NAME"))
		SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON PEOPLE"
		SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON SITES"
		SEC_SelectPrivileges = SEC_SelectPrivileges &  ",SELECT ON PRIVILEGE_TABLES"
		SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON SECURITY_ROLES"
		
		FG_MGMENT_SELECT ="SELECT ON DB_FORM"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_FORMGROUP"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_FORMGROUP_TABLES"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_FORMTYPE "
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_FORMTYPE_DOPT"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_FORM_ITEM"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_QUERY"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_TABLE"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_COLUMN"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_DATATYPE_DISPLAY_TYPE"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_DISPLAY_OPTION"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_DISPLAY_TYPE"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_DTYP_DOPT"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_RELATIONSHIP"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_SCHEMA"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_STRUCTURES"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_TABLE"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_CHILD_TABLES"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_COLUMN_TABLE"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_FORMGROUP_TABLES"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_FORMITEMS_ALL"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_PARENT_TABLES"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON DB_VW_TABLE_SCHEMA"
		FG_MGMENT_SELECT =FG_MGMENT_SELECT & ",SELECT ON EXCEPTIONS"

		
		FG_MGMENT_DELETE ="DELETE UPDATE ON DB_FORM"
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_FORMGROUP"
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_FORMGROUP_TABLES"
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_FORMTYPE "
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_FORMTYPE_DOPT"
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_FORM_ITEM"
		FG_MGMENT_DELETE =FG_MGMENT_DELETE & ",DELETE ON DB_QUERY"
		
		FG_MGMENT_INSERT ="INSERT ON DB_FORM"
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_FORMGROUP"
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_FORMGROUP_TABLES"
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_FORMTYPE "
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_FORMTYPE_DOPT"
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_FORM_ITEM"
		FG_MGMENT_INSERT =FG_MGMENT_INSERT & ",INSERT ON DB_QUERY"
		
		FG_MGMENT_UPDATE ="UPDATE ON DB_FORM"
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_FORMGROUP"
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_FORMGROUP_TABLES"
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_FORMTYPE "
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_FORMTYPE_DOPT"
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_FORM_ITEM"
		FG_MGMENT_UPDATE =FG_MGMENT_UPDATE & ",UPDATE ON DB_QUERY"
		
		FG_ADMIN_MGMENT_SELECT =  "SELECT, INSERT, DELETE, UPDATE ON DB_COLUMN"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_DATATYPE_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_DISPLAY_OPTION"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_DTYP_DOPT"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_RELATIONSHIP"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_SCHEMA"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_STRUCTURES "
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_TABLE"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_CHILD_TABLES"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_COLUMN_TABLE"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_FORMGROUP_TABLES "
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_FORMITEMS_ALL"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_PARENT_TABLES"
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON DB_VW_TABLE_SCHEMA "
		FG_ADMIN_MGMENT_SELECT = FG_ADMIN_MGMENT_SELECT & ",SELECT, INSERT, DELETE, UPDATE ON EXCEPTIONS"

		FG_ADMIN_MGMENT_DELETE =  "DELETE ON DB_COLUMN"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_DATATYPE_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_DISPLAY_OPTION"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_DTYP_DOPT"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_RELATIONSHIP"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_SCHEMA"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_STRUCTURES "
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_TABLE"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_CHILD_TABLES"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_COLUMN_TABLE"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_FORMGROUP_TABLES "
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_FORMITEMS_ALL"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_PARENT_TABLES"
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON DB_VW_TABLE_SCHEMA "
		FG_ADMIN_MGMENT_DELETE = FG_ADMIN_MGMENT_DELETE & ",DELETE ON EXCEPTIONS"

		FG_ADMIN_MGMENT_INSERT =  "INSERT ON DB_COLUMN"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_DATATYPE_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_DISPLAY_OPTION"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_DTYP_DOPT"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_RELATIONSHIP"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_SCHEMA"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_STRUCTURES "
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_TABLE"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_CHILD_TABLES"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_COLUMN_TABLE"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_FORMGROUP_TABLES "
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_FORMITEMS_ALL"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_PARENT_TABLES"
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON DB_VW_TABLE_SCHEMA "
		FG_ADMIN_MGMENT_INSERT = FG_ADMIN_MGMENT_INSERT & ",INSERT ON EXCEPTIONS"

		FG_ADMIN_MGMENT_UPDATE =  "UPDATE ON DB_COLUMN"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_DATATYPE_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_DISPLAY_OPTION"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_DISPLAY_TYPE"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_DTYP_DOPT"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_RELATIONSHIP"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_SCHEMA"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_STRUCTURES "
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_TABLE"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_VW_CHILD_TABLES"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_VW_COLUMN_TABLE"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON DB_VW_FORMGROUP_TABLES "
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON CS_SECURITY.DB_VW_FORMITEMS_ALL"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON CS_SECURITY.DB_VW_PARENT_TABLES"
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON CS_SECURITY.DB_VW_TABLE_SCHEMA "
		FG_ADMIN_MGMENT_UPDATE = FG_ADMIN_MGMENT_UPDATE & ",UPDATE ON CS_SECURITY.EXCEPTIONS"

			


	Select Case UCase(PrivilegeName)

		Case "CAMSOFT_LOG_ON"
			Privileges = "CONNECT"
			isOK = DoProcessPriv(DBA_Conn, ProcessType, RoleName, Privileges)
		Case "ADD_USER_FORMGROUP"
			Privileges = FG_MGMENT_SELECT & "," & FG_MGMENT_DELETE& "," &FG_MGMENT_INSERT& "," &FG_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)		
		Case "EDIT_USER_FORMGROUP"	
			Privileges = FG_MGMENT_SELECT& "," &FG_MGMENT_DELETE& "," &FG_MGMENT_INSERT& "," &FG_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "DELETE_USER_FORMGROUP"	
			Privileges = FG_MGMENT_SELECT& "," &FG_MGMENT_DELETE& "," &FG_MGMENT_INSERT& "," &FG_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		
		Case "SET_FORMGROUP_PUBLIC "
			'business logic only
		Case "ADD_ADMIN_SCHEMA"
			Privileges = FG_ADMIN_MGMENT_SELECT& "," &FG_ADMIN_MGMENT_DELETE& "," &FG_ADMIN_MGMENT_INSERT& "," &FG_ADMIN_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "EDIT_ADMIN_SCHEMA"
			Privileges = FG_ADMIN_MGMENT_SELECT& "," &FG_ADMIN_MGMENT_DELETE& "," &FG_ADMIN_MGMENT_INSERT& "," &FG_ADMIN_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "DELETE_ADMIN_SCHEMA"
			Privileges = FG_ADMIN_MGMENT_SELECT& "," &FG_ADMIN_MGMENT_DELETE& "," &FG_ADMIN_MGMENT_INSERT& "," &FG_ADMIN_MGMENT_UPDATE
			isOK = DoProcessPriv(User_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "IS_FORMGROUP_PUBLIC"
			'business logic only
		Case "ADD_ROLES_TABLE"	
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER"
			Privileges = Privileges & ",CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,GRANT ANY PRIVILEGE"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			Privileges = "DELETE ON SECURITY_ROLES,INSERT ON SECURITY_ROLES,UPDATE ON SECURITY_ROLES"
			Privileges = Privileges & ",DELETE ON CHEM_REG_PRIVILEGES,INSERT ON CHEM_REG_PRIVILEGES,UPDATE ON " & UCase(Application("PRIV_TABLE_NAME"))
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "EDIT_ROLES_TABLE"	
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER"
			Privileges = Privileges & ",CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,GRANT ANY PRIVILEGE"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			Privileges = "DELETE ON SECURITY_ROLES,INSERT ON SECURITY_ROLES,UPDATE ON SECURITY_ROLES"
			Privileges = Privileges & ",DELETE ON CHEM_REG_PRIVILEGES,INSERT ON CHEM_REG_PRIVILEGES,UPDATE ON " & UCase(Application("PRIV_TABLE_NAME"))
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "DELETE_ROLES_TABLE"	
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER"
			Privileges = Privileges & ",CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,GRANT ANY PRIVILEGE"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			Privileges = "DELETE ON SECURITY_ROLES,INSERT ON SECURITY_ROLES,UPDATE ON SECURITY_ROLES"
			Privileges = Privileges & ",DELETE ON CHEM_REG_PRIVILEGES,INSERT ON CHEM_REG_PRIVILEGES,UPDATE ON " & UCase(Application("PRIV_TABLE_NAME"))
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		
		
		Case "EDIT_PEOPLE_TABLE"
			Privileges ="UPDATE ON PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "EDIT_USERS_TABLE"
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER"
			Privileges = Privileges & ",CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,GRANT ANY PRIVILEGE"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			Privileges = "DELETE ON SECURITY_ROLES,INSERT ON SECURITY_ROLES,UPDATE ON SECURITY_ROLES"
			Privileges = Privileges & ",DELETE ON CHEM_REG_PRIVILEGES,INSERT ON CHEM_REG_PRIVILEGES,UPDATE ON " & UCase(Application("PRIV_TABLE_NAME"))
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)
		Case "ADD_PEOPLE_TABLE"
			Privileges ="INSERT ON PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_PEOPLE_TABLE"
			Privileges ="DELETE ON PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		

	end Select
	CloseRS(RS)
	doMapPrivilege = isOK

End Function
Function DoProcessPriv(DBA_Conn, ProcessType, PrivilegeName, Privileges)
	Dim i
	PrivArray = Split(Privileges,",")
	for i = 0 to Ubound(PrivArray)
		tempArr = Split(PrivArray(i), " ON ")
		Priv = tempArr(0)
		Object= tempArr(1)
		if NOT ((Object = "SALTS") OR (Object = "COMPOUND_TYPE") OR (Object = "IDENTIFIERS") OR (Object = "PROJECTS") OR (Object = "NOTEBOOKS")) then
			Response.Write "INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES "
			Response.Write "('" & PrivilegeName & "', '" & Priv & "', 'CS_SECURITY', '" & Object  & "');" & vblf
		ELSE
			Response.Write "INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES "
			Response.Write "('" & PrivilegeName & "', '" & Priv & "', 'REGDB', '" & Object  & "');" & vblf
		end if	
	next
	DoProcessPriv = ""
End function
</SCRIPT>
