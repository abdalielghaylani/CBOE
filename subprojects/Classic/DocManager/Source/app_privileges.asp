<%
Function doMapPrivilege(ByRef REG_Conn, ByRef DBA_Conn, ByRef SEC_Conn, ProcessTypeIn, RoleName, PrivilegeFieldName)
	if ProcessTypeIn = "ADD" then
		temp = Split(PrivilegeFieldName, ";",-1)
		temp2 = Split(temp(0), ".",-1)
		PrivilegeName = temp2(1)
		value = Request(temp(0))
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
	Sec_Table= Application("SEC_USERNAME")
	Appkey = Application("AppKey")
		
	SEC_SelectPrivileges = "SELECT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
	SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON " & Sec_Table & "." & "PEOPLE"
	SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON " & Sec_Table & "." & "SITES"
	SEC_SelectPrivileges = SEC_SelectPrivileges &  ",SELECT ON " & Sec_Table & "." & "PRIVILEGE_TABLES"
	SEC_SelectPrivileges = SEC_SelectPrivileges  & ",SELECT ON " & Sec_Table & "." & "SECURITY_ROLES"


	basicSelectPriv = "SELECT ON DOCMGR.DOCMGR_DOCUMENTS, " & _
						"SELECT ON DOCMGR.DOCMGR_STRUCTURES, " & _
						"SELECT ON CSDOHITLIST, " & _
						"SELECT ON CSDOHITLISTID"
	
	basicUpdatePriv = "UPDATE ON DOCMGR.DOCMGR_DOCUMENTS, " & _
						"UPDATE ON DOCMGR.DOCMGR_STRUCTURES, " & _
						"UPDATE ON CSDOHITLIST, " & _
						"UPDATE ON CSDOHITLISTID"

	basicInsertPriv = "INSERT ON DOCMGR.DOCMGR_DOCUMENTS, " & _
						"INSERT ON DOCMGR.DOCMGR_STRUCTURES, " & _
						"INSERT ON CSDOHITLIST, " & _
						"INSERT ON CSDOHITLISTID"
						
	basicDeletePriv = "DELETE ON DOCMGR.DOCMGR_DOCUMENTS, " & _
						"DELETE ON DOCMGR.DOCMGR_STRUCTURES, " & _
						"DELETE ON CSDOHITLIST, " & _
						"DELETE ON CSDOHITLISTID"
						
	Select Case UCase(PrivilegeName)

		Case "CAMSOFT_LOG_ON"
			Privileges = "CONNECT"
			isOK = DoProcessPriv(DBA_Conn, ProcessType, RoleName, Privileges)

		Case "SEARCH_DOCS"
			Privileges = basicSelectPriv
			
			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "SUBMIT_DOCS"
			Privileges = basicSelectPriv & ", " & _
						basicUpdatePriv & ", " & _
						basicInsertPriv
			
			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "BATCHLOAD_DOCS"
			Privileges = basicSelectPriv & ", " & _
						basicUpdatePriv & ", " & _
						basicInsertPriv
			

		Case "VIEW_HISTORY"
			Privileges = basicSelectPriv & ", " & _
						basicUpdatePriv & ", " & _
						basicInsertPriv

			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_MY_DOCS"
			Privileges = basicSelectPriv & ", " & _
						basicUpdatePriv & ", " & _
						basicInsertPriv & ", " & _
						basicDeletePriv
									
			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_ALL_DOCS"
			Privileges = basicSelectPriv & ", " & _
						basicUpdatePriv & ", " & _
						basicInsertPriv & ", " & _
						basicDeletePriv
									
			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_USERS_TABLE"
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)

			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_USERS_TABLE"
		
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"
			
			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_USERS_TABLE"
		
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"
			
			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
						
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_ROLES_TABLE"
		
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"
			
			
			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			
			Privileges = "SELECT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & ".SECURITY_ROLES"
			
			Privileges = Privileges & ", " & "SELECT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_ROLES_TABLE"
		
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"
			
			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			
			Privileges = "SELECT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & ".SECURITY_ROLES"
			
			Privileges = Privileges & ", " & "SELECT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
		
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_ROLES_TABLE"
		
			Privileges ="SELECT ANY TABLE, CREATE USER, DROP USER, ALTER USER"
			Privileges = Privileges & ", " & "CREATE ROLE, ALTER ANY ROLE, DROP ANY ROLE, GRANT ANY ROLE, GRANT ANY PRIVILEGE"
			
			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			
			Privileges = "SELECT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & ".SECURITY_ROLES"
			
			Privileges = Privileges & ", " & "SELECT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "UPDATE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "INSERT ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			Privileges = Privileges & ", " & "DELETE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_PEOPLE_TABLE"
			Privileges ="UPDATE ON " & Sec_Table & ".PEOPLE"
			
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_PEOPLE_TABLE"
			Privileges ="INSERT ON " & Sec_Table & ".PEOPLE"
			
			'is the following one necessary? --syan
			'Privileges = Privileges  & ", " & "SELECT ON NOTEBOOKS, SELECT ON PROJECTS, SELECT ON SALTS, SELECT ON COMPOUND_TYPE, SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_PEOPLE_TABLE"
			Privileges ="DELETE ON " & Sec_Table & ".PEOPLE"

			'is the following one necessary? --syan
			'Privileges = Privileges  & ", " & "SELECT ON NOTEBOOKS, SELECT ON PROJECTS, SELECT ON SALTS, SELECT ON COMPOUND_TYPE, SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
			'add security privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

	end Select
	doMapPrivilege = isOK
End Function
%>