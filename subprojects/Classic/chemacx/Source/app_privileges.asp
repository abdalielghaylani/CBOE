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



	Select Case UCase(PrivilegeName)

		Case "CAMSOFT_LOG_ON"
			Privileges = "CONNECT"
			isOK = DoProcessPriv(DBA_Conn, ProcessType, RoleName, Privileges)
		Case "SEARCH_TEMP"
			Privileges = "SELECT ON TEMPORARY_STRUCTURES,SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, RoleName, Privileges)
			'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "SEARCH_REG"
			Privileges ="SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON ALT_IDS"
			Privileges = Privileges  & ",SELECT ON BATCHES,SELECT ON COMMIT_TYPES,SELECT ON COMPOUND_MOLECULE,SELECT ON COMPOUND_PROJECT"
			Privileges = Privileges  & ",SELECT ON COMPOUND_SALT,SELECT ON COMPOUND_TYPE,SELECT ON DUPLICATES,SELECT ON IDENTIFIERS"
			Privileges = Privileges  & ",SELECT ON MIXTURES,SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON REG_APPROVED,SELECT ON REG_NUMBERS"
			Privileges = Privileges  & ",SELECT ON REG_QUALITY_CHECKED,SELECT ON SALTS,SELECT ON SEQUENCE,SELECT ON SPECTRA,SELECT ON STRUCTURE_MIXTURE"
			Privileges = Privileges  & ",SELECT ON STRUCTURES,SELECT ON TEST_SAMPLES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
				'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		
		Case "ADD_COMPOUND_TEMP"
			Privileges = "INSERT ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_BATCH_TEMP"
			Privileges = "INSERT ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"
			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_SALT_TEMP"
			Privileges = "INSERT ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_IDENTIFIER_TEMP"
			Privileges = "INSERT ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_COMPOUND_TEMP"
			Privileges = "UPDATE ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_BATCH_TEMP"
			Privileges = "UPDATE ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_SALT_TEMP"
			Privileges = "UPDATE ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_IDENTIFIERS_TEMP"
			Privileges = "UPDATE ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_TEMP"
			Privileges = "DELETE ON TEMPORARY_STRUCTURES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "REGISTER_TEMP"
			Privileges = "UPDATE ON SEQUENCE,UPDATE REG_NUMBERS,INSERT ON ALT_IDS,INSERT ON BATCHES,INSERT ON COMPOUND_PROJECT,INSERT ON COMPOUND_MOLECULE"
			Privileges = Privileges & ",INSERT ON COMPOUND_SALT,INSERT ON DUPLICATES,INSERT ON MIXTURES,INSERT ON REG_NUMBERS"
			Privileges = Privileges & ",INSERT ON SPECTRA,INSERT ON STRUCTURE_MIXTURE,INSERT ON STRUCTURES,INSERT ON TEST_SAMPLES"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_COMPOUND_REG"
			Privileges = "UPDATE ON REG_NUMBERS,UPDATE ON STRUCTURES,UPDATE ON COMPOUND_MOLECULE,UPDATE ON COMPOUND_PROJECT"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_BATCH_REG"
			Privileges = "UPDATE ON BATCHES,UPDATE ON REG_NUMBERS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_SALT_REG"
			Privileges = "UPDATE ON COMPOUND_SALT,UPDATE ON REG_NUMBERS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_IDENTIFIERS_REG"
			Privileges = "UPDATE ON ALT_IDS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "DELETE_REG"
			Privileges = "DELETE ON ALT_IDS,DELETE ON BATCHES,DELETE ON COMPOUND_MOLECULE,DELETE ON COMPOUND_PROJECT"
			Privileges = Privileges & ",DELETE ON COMPOUND_SALT,DELETE ON DUPLICATES,DELETE ON MIXTURES"
			Privileges = Privileges & ",DELETE ON SPECTRA,DELETE ON STRUCTURE_MIXTURE,DELETE ON STRUCTURES,DELETE ON TEST_SAMPLES,DELETE ON REG_NUMBERS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "EDIT_SCOPE_SELF"
			'business logic only
		Case "EDIT_SCOPE_SUPERVISOR"
			'business logic only
		Case "EDIT_SCOPE_ALL"
			'business logic only
		Case "SET_APPROVED_FLAG"
			Privileges = "INSERT ON REG_APPROVED"			
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "TOGGLE_APPROVED_FLAG"
			Privileges = "DELETE ON REG_APPROVED"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "SET_QUALITY_CHECK_FLAG"
			Privileges = "INSERT ON REG_QUALITY_CHECKED"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "TOGGLE_QUALITY_CHECK_FLAG"
			Privileges = "DELETE ON REG_QUALITY_CHECKED"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)


		Case "EDIT_SALT_TABLE"
			Privileges = "SELECT ON SALTS,INSERT ON SALTS,UPDATE ON SALTS,DELETE ON SALTS" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_SEQUENCES_TABLE"
			Privileges = "SELECT ON SEQUENCE,INSERT ON SEQUENCE,UPDATE ON SEQUENCE,DELETE ON SEQUENCE" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_NOTEBOOKS_TABLE"
			Privileges = "SELECT ON NOTEBOOKS,INSERT ON NOTEBOOKS,UPDATE ON NOTEBOOKS,DELETE ON NOTEBOOKS" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_PROJECTS_TABLE"
			Privileges = "SELECT,UPDATE ON PROJECTS" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_PROJECTS_TABLE"
			Privileges = "INSERT,SELECT ON PROJECTS" 
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		
		Case "DELETE_PROJECTS_TABLE"
			Privileges = "DELETE ON PROJECTS" 
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_WORKGROUP"
			Privileges = "UPDATE ON PEOPLE" 
					Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_WORKGROUP"
			Privileges = "UPDATE ON PEOPLE" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_WORKGROUP"
			Privileges = "UPDATE ON PEOPLE" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_PROJECTS_TABLE"
			Privileges = "SELECT ON PROJECTS,INSERT ON PROJECTS,UPDATE ON PROJECTS" 
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_PROJECTS_TABLE"
			Privileges = "SELECT ON PROJECTS,INSERT ON PROJECTS,UPDATE ON PROJECTS,DELETE ON PROJECTS" 
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_COMPOUND_TYPE_TABLE"
			Privileges = "DELETE ON COMPOUND_TYPE" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_COMPOUND_TYPE_TABLE"
			Privileges = "INSERT ON COMPOUND_TYPE" 
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_COMPOUND_TYPE_TABLE"
			Privileges = "UPDATE ON COMPOUND_TYPE" 
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_USERS_TABLE"
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER"
			Privileges = Privileges & ",CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,GRANT ANY PRIVILEGE"
						Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(DBA_CONN, ProcessType, RoleName, Privileges)
			Privileges = "DELETE ON " & Sec_Table & ".SECURITY_ROLES,INSERT ON " & Sec_Table & ".SECURITY_ROLES,UPDATE ON " & Sec_Table & ".SECURITY_ROLES"
			Privileges = Privileges & ",DELETE ON " & Sec_Table & "." & UCase(Application("AppKey")) & "_REG_PRIVILEGES,INSERT ON " & Sec_Table & "." & UCase(Application("AppKey")) & "_REG_PRIVILEGES,UPDATE ON " & Sec_Table & "." & UCase(Application("PRIV_TABLE_NAME"))
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_PEOPLE_TABLE"
			Privileges ="UPDATE ON " & Sec_Table & ".PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_PEOPLE_TABLE"
			Privileges ="INSERT ON " & Sec_Table & ".PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "DELETE_PEOPLE_TABLE"
			Privileges ="DELETE ON " & Sec_Table & ".PEOPLE"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "EDIT_SITES_TABLE"
			Privileges ="UPDATE ON " & Sec_Table & ".SITES,INSERT ON " & Sec_Table & ".SITES,DELETE ON " & Sec_Table & ".SITES"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

	
		Case "EDIT_EVAL_DATA"
			Privileges ="SELECT ON EXPERIMENTS,INSERT ON EXPERIMENTS,UPDATE ON EXPERIMENTS,DELETE ON EXPERIMENTS"
			Privileges = Privileges & ",SELECT ON EXPERIMENTTYPE,INSERT ON EXPERIMENTTYPE,UPDATE ON EXPERIMENTTYPE,DELETE ON EXPERIMENTTYPE"
			Privileges = Privileges & ",UPDATE ON RESULTS,DELETE ON RESULTS,INSERT ON RESULTS"
			Privileges = Privileges & ",UPDATE ON RESULTTYPE,DELETE ON RESULTTYPE,INSERT ON RESULTTYPE"
			Privileges = Privileges & ",UPDATE ON PARAMETERS,DELETE ON PARAMETERS,INSERT ON PARAMETERS"
			Privileges = Privileges & ",UPDATE ON PARAMETERTYPE,DELETE ON PARAMETERTYPE,INSERT ON PARAMETERTYPE"
			Privileges = Privileges & ",UPDATE ON EXPERIMENTTYPEPARAMETERS,DELETE ON EXPERIMENTTYPEPARAMETERS,INSERT ON EXPERIMENTTYPEPARAMETERS"
			Privileges = Privileges & ",UPDATE ON EXPERIMENTTYPERESULTS,DELETE ON EXPERIMENTTYPERESULTS,INSERT ON EXPERIMENTTYPERESULTS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "ADD_EVAL_DATA"
			Privileges ="SELECT ON EXPERIMENTS,INSERT ON EXPERIMENTS,UPDATE ON EXPERIMENTS"
			Privileges = Privileges & ",INSERT ON EXPERIMENTTYPE,UPDATE ON EXPERIMENTTYPE"
			Privileges = Privileges & ",INSERT ON RESULTS,UPDATE ON RESULTS"
			Privileges = Privileges & ",INSERT ON PARAMETERS,UPDATE ON PARAMETERS"
			Privileges = Privileges & ",INSERT ON EXPERIMENTTYPERESULTS,UPDATE ON EXPERIMENTTYPERESULTS"
			Privileges = Privileges & ",INSERT ON EXPERIMENTTYPEPARAMETERS,UPDATE ON EXPERIMENTTYPEPARAMETERS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

		Case "SITE_ACCESS_ALL"
			'business logic only
		Case "SEARCH_EVAL_DATA"
			Privileges ="SELECT ON EXPERIMENTS"
			Privileges = Privileges & ",SELECT ON EXPERIMENTTYPE"
			Privileges = Privileges & ",SELECT ON RESULTS"
			Privileges = Privileges & ",SELECT ON RESULTTYPE"
			Privileges = Privileges & ",SELECT ON PARAMETERS"
			Privileges = Privileges & ",SELECT ON PARAMETERTYPE"
			Privileges = Privileges & ",SELECT ON EXPERIMENTTYPEPARAMETERS"
			Privileges = Privileges & ",SELECT ON EXPERIMENTTYPERESULTS"
			Privileges = Privileges  & ",SELECT ON NOTEBOOKS,SELECT ON PROJECTS,SELECT ON SALTS,SELECT ON COMPOUND_TYPE,SELECT ON IDENTIFIERS"

			isOK = DoProcessPriv(REG_CONN, ProcessType, RoleName, Privileges)
							'add sec privileges
			isOK = DoProcessPriv(SEC_CONN, ProcessType, RoleName, SEC_SelectPrivileges)

	end Select
	doMapPrivilege = isOK
End Function
%>