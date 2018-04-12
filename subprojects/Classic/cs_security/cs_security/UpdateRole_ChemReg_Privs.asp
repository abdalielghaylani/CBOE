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
Set Conn = GetCS_SecurityConnection(dbKey)
Set Cmd = GetCommand(Conn, "CS_SECURITY.UPDATEROLE", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 100, NULL)		
Cmd.Parameters.Append Cmd.CreateParameter("PRoleNAME", 200, adParamInput, 50, RoleName) 
Cmd.Parameters.Append Cmd.CreateParameter("PPrivTableName", 200, adParamInput, 50, PrivTableName)  
Cmd.Parameters.Append Cmd.CreateParameter("PPrivValueList", 200, adParamInput, 2000, PrivValueList) 
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd("CS_SECURITY.UpdateRole")
End if

RC = Cmd.Parameters("RETURN_VALUE")

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
		
				



'CS Security Tables

SecurityTablesSELECT = "SELECT ON CHEM_REG_PRIVILEGES,SELECT ON PEOPLE,SELECT ON PRIVILEGE_TABLES,SELECT ON SECURITY_ROLES,SELECT ON SITES"
SecurityTablesDELETE = "DELETE ON CHEM_REG_PRIVILEGES,DELETE ON PEOPLE,DELETE ON PRIVILEGE_TABLES,DELETE ON SECURITY_ROLES,DELETE ON SITES"
SecurityTablesINSERT = "INSERT ON CHEM_REG_PRIVILEGES,INSERT ON PEOPLE,INSERT ON PRIVILEGE_TABLES,INSERT ON SECURITY_ROLES,INSERT ON SITES"
SecurityTablesUpdate = "Update ON CHEM_REG_PRIVILEGES,Update ON PEOPLE,Update ON PRIVILEGE_TABLES,Update ON SECURITY_ROLES,Update ON SITES"


'CSDO hitlist tables 

HitListTables = "SELECT,INSERT,UPDATE,DELETE ON CSDOHITLIST,SELECT,INSERT,UPDATE,DELETE ON CSDOHITLISTID"

'Temporary Compound Table

TempCompoundsSELECT = "SELECT ON COMMIT_TYPES,SELECT ON TEMPORARY_STRUCTURES"
TempCompoundsDELETE = "DELETE ON COMMIT_TYPES,DELETE ON TEMPORARY_STRUCTURES"
TempCompoundsINSERT = "INSERT ON COMMIT_TYPES,INSERT ON TEMPORARY_STRUCTURES"
TempCompoundsUPDATE = "UPDATE ON COMMIT_TYPES,UPDATE ON TEMPORARY_STRUCTURES"

'Analytics Lookup Tables

AnalyticsLookupTablesSELECT ="SELECT ON RESULTTYPE,SELECT ON PARAMETERTYPE,SELECT ON EXPERIMENTTYPE,SELECT ON EXPERIMENTTYPEPARAMETERS,SELECT ON EXPERIMENTTYPEPARAMETERS,SELECT ON EXPERIMENTTYPERESULTS"
AnalyticsLookupTablesDELETE ="DELETE ON RESULTTYPE,DELETE ON PARAMETERTYPE,DELETE ON EXPERIMENTTYPE,DELETE ON EXPERIMENTTYPEPARAMETERS,DELETE ON EXPERIMENTTYPEPARAMETERS,DELETE ON EXPERIMENTTYPERESULTS"
AnalyticsLookupTablesINSERT ="INSERT ON RESULTTYPE,INSERT ON PARAMETERTYPE,INSERT ON EXPERIMENTTYPE,INSERT ON EXPERIMENTTYPEPARAMETERS,INSERT ON EXPERIMENTTYPEPARAMETERS,INSERT ON EXPERIMENTTYPERESULTS"
AnalyticsLookupTablesUPDATE ="UPDATE ON RESULTTYPE,UPDATE ON PARAMETERTYPE,UPDATE ON EXPERIMENTTYPE,UPDATE ON EXPERIMENTTYPEPARAMETERS,UPDATE ON EXPERIMENTTYPEPARAMETERS,UPDATE ON EXPERIMENTTYPERESULTS"

'Analytics Data

AnalyticsDataSELECT ="SELECT ON RESULTS,SELECT ON EXPERIMENTS,SELECT ON PARAMETERS"
AnalyticsDataDELETE ="DELETE ON RESULTS,DELETE ON EXPERIMENTS,DELETE ON PARAMETERS"
AnalyticsDataINSERT ="INSERT ON RESULTS,INSERT ON EXPERIMENTS,INSERT ON PARAMETERS"
AnalyticsDataUPDATE ="UPDATE ON RESULTS,UPDATE ON EXPERIMENTS,UPDATE ON PARAMETERS"

'Reg LookupTables

RegLookupTablesSELECT = AnalyticsDataSELECT & "," & AnalyticsLookupTablesSELECT & "," & "SELECT ON UTILIZATIONS,SELECT ON SOLVATES,SELECT ON SEQUENCE,SELECT ON SALTS,SELECT ON PROJECTS,SELECT ON NOTEBOOKS,SELECT ON COMPOUND_TYPE,SELECT ON  BATCH_PROJECTS,SELECT ON BATCH_PROJ_UTILIZATIONS"


'Permanent Tables

RegisteredCompoundsSELECT = AnalyticsDataSELECT & "," & "SELECT ON ALT_IDS,SELECT ON BATCHES,SELECT ON CMPD_MOL_UTILIZATIONS,SELECT ON COMPOUND_MOLECULE"
	RegisteredCompoundsSELECT =  RegisteredCompoundsSELECT & ",SELECT ON COMPOUND_PROJECT,SELECT ON DUPLICATES,SELECT ON COMPOUND_TYPE,SELECT ON COMPOUND_SALT"
	RegisteredCompoundsSELECT =  RegisteredCompoundsSELECT & ",SELECT ON IDENTIFIERS,SELECT ON MIXTURES,SELECT ON MOLFILES,SELECT ON REG_APPROVED,SELECT ON REG_NUMBERS"
	RegisteredCompoundsSELECT =  RegisteredCompoundsSELECT & ",SELECT ON REG_QUALITY_CHECKED,SELECT ON SPECTRA,SELECT ON STRUCTURES,SELECT ON STRUCTURE_MIXTURE"
	RegisteredCompoundsSELECT =  RegisteredCompoundsSELECT & ",SELECT ON TEST_SAMPLES"

RegisteredCompoundsDELETE = "DELETE ON ALT_IDS,DELETE ON BATCHES,DELETE ON CMPD_MOL_UTILIZATIONS,DELETE ON COMPOUND_MOLECULE"
	RegisteredCompoundsDELETE =  RegisteredCompoundsDELETE & ",DELETE ON COMPOUND_PROJECT,DELETE ON DUPLICATES,DELETE ON COMPOUND_TYPE,DELETE ON COMPOUND_SALT"
	RegisteredCompoundsDELETE =  RegisteredCompoundsDELETE & ",DELETE ON IDENTIFIERS,DELETE ON MIXTURES,DELETE ON MOLFILES,DELETE ON REG_APPROVED,DELETE ON REG_NUMBERS"
	RegisteredCompoundsDELETE =  RegisteredCompoundsDELETE & ",DELETE ON REG_QUALITY_CHECKED,DELETE ON SPECTRA,DELETE ON STRUCTURES,DELETE ON STRUCTURE_MIXTURE"
	RegisteredCompoundsDELETE =  RegisteredCompoundsDELETE & ",DELETE ON TEST_SAMPLES"

RegisteredCompoundsINSERT = "INSERT ON ALT_IDS,INSERT ON BATCHES,INSERT ON CMPD_MOL_UTILIZATIONS,INSERT ON COMPOUND_MOLECULE"
	RegisteredCompoundsINSERT =  RegisteredCompoundsINSERT & ",INSERT ON COMPOUND_PROJECT,INSERT ON DUPLICATES,INSERT ON COMPOUND_TYPE,INSERT ON COMPOUND_SALT"
	RegisteredCompoundsINSERT =  RegisteredCompoundsINSERT & ",INSERT ON IDENTIFIERS,INSERT ON MIXTURES,INSERT ON MOLFILES,INSERT ON REG_APPROVED,INSERT ON REG_NUMBERS"
	RegisteredCompoundsINSERT =  RegisteredCompoundsINSERT & ",INSERT ON REG_QUALITY_CHECKED,INSERT ON SPECTRA,INSERT ON STRUCTURES,INSERT ON STRUCTURE_MIXTURE"
	RegisteredCompoundsINSERT =  RegisteredCompoundsINSERT & ",INSERT ON TEST_SAMPLES"

RegisteredCompoundsUPDATE = "UPDATE ON ALT_IDS,UPDATE ON BATCHES,UPDATE ON CMPD_MOL_UTILIZATIONS,UPDATE ON COMPOUND_MOLECULE"
	RegisteredCompoundsUPDATE =  RegisteredCompoundsUPDATE & ",UPDATE ON COMPOUND_PROJECT,UPDATE ON DUPLICATES,UPDATE ON COMPOUND_TYPE,UPDATE ON COMPOUND_SALT"
	RegisteredCompoundsUPDATE =  RegisteredCompoundsUPDATE & ",UPDATE ON IDENTIFIERS,UPDATE ON MIXTURES,UPDATE ON MOLFILES,UPDATE ON REG_APPROVED,UPDATE ON REG_NUMBERS"
	RegisteredCompoundsUPDATE =  RegisteredCompoundsUPDATE & ",UPDATE ON REG_QUALITY_CHECKED,UPDATE ON SPECTRA,UPDATE ON STRUCTURES,UPDATE ON STRUCTURE_MIXTURE"
	RegisteredCompoundsUPDATE =  RegisteredCompoundsUPDATE & ",UPDATE ON TEST_SAMPLES,UPDATE ON SEQUENCE"


if ProcessType = "ADD" then
	bAdd = True
end if
if ProcessType = "REVOKE" then
	bRevoke = True
end if

'Set RS = server.createObject("ADODB.RECORDSET")
Select Case UCase(PrivilegeName)

	Case "CAMSOFT_LOG_ON"    
		Privileges = "CONNECT"   
		isOK = DoProcessPriv(DBA_Conn, ProcessType, PrivilegeName, Privileges)
	                     
	Case  "SEARCH_TEMP"  
		if bAdd then
			Privileges = TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = TempCompoundsSELECT 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if

		                                     
	Case  "SEARCH_REG"       
		if bAdd then
			Privileges = RegisteredCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = RegisteredCompoundsSELECT 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if               
	                                   
	Case  "ADD_COMPOUND_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_TEMP,ADD_SALT_TEMP,ADD_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                      
	Case  "ADD_BATCH_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_COMPOUND_TEMP,ADD_SALT_TEMP,ADD_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if       
	                                       
	Case  "ADD_SALT_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_TEMP,ADD_COMPOUND_TEMP,ADD_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	Case  "ADD_IDENTIFIER_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_TEMP,ADD_COMPOUND_TEMP,ADD_SALT_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                  
	Case  "EDIT_COMPOUND_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "EDIT_BATCH_TEMP,EDIT_SALT_TEMP,EDIT_IDENTIFIER_TEMP,ADD_BATCH_TEMP,ADD_SALT_TEMP,ADD_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                     
	Case  "EDIT_BATCH_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_COMPOUND_TEMP,ADD_SALT_TEMP,ADD_IDENTIFIER_TEMP,EDIT_COMPOUND_TEMP,EDIT_SALT_TEMP,EDIT_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if       
	                                      
	Case  "EDIT_SALT_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_TEMP,ADD_COMPOUND_TEMP,ADD_IDENTIFIER_TEMP,EDIT_BATCH_TEMP,EDIT_COMPOUND_TEMP,EDIT_IDENTIFIER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                               
	Case  "EDIT_IDENTIFIERS_TEMP" 
		if bAdd then
			Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_TEMP,ADD_COMPOUND_TEMP,ADD_SALT_TEMP,EDIT_BATCH_TEMP,EDIT_COMPOUND_TEMP,EDIT_SALT_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                         
	Case  "DELETE_TEMP" 
		if bAdd then
			Privileges = TempCompoundsDELETE & "," & TempCompoundsINSERT & "," & TempCompoundsUPDATE & "," & TempCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = TempCompoundsDELETE 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			dependencies = "ADD_BATCH_TEMP,ADD_COMPOUND_TEMP,ADD_SALT_TEMP,EDIT_BATCH_TEMP,EDIT_COMPOUND_TEMP,EDIT_SALT_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = TempCompoundsINSERT & "," & TempCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                       
	Case  "REGISTER_TEMP" 
		if bAdd then
			Privileges = TempCompoundsDELETE  & "," & RegisteredCompoundsSELECT & "," & RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "DELETE_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = "TempCompoundsDELETE"
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if

			dependencies = "SEARCH_REG,SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG,EDIT_SEQUENCES_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                   
	Case  "EDIT_COMPOUND_REG" 

		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                      
	Case  "EDIT_BATCH_REG" 
		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                       
	Case  "EDIT_SALT_REG" 
		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                
	Case  "EDIT_IDENTIFIERS_REG" 
		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                          
	Case  "DELETE_REG" 

		if bAdd then
			Privileges =  RegisteredCompoundsDELETE & "," &RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "RegisteredCompoundsDELETE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                     
	Case  "EDIT_SCOPE_SELF" 
	                               
	Case  "EDIT_SCOPE_SUPERVISOR" 
	                                      
	Case  "EDIT_SCOPE_ALL" 
	                                   
	Case  "SET_APPROVED_FLAG" 
		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                
	Case  "TOGGLE_APPROVED_FLAG" 
		if bAdd then
			Privileges =  "DELETE ON REG_APPROVED," & RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON REG_APPROVED"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			dependencies = "SET_QUALITY_CHECK_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	Case  "SET_QUALITY_CHECK_FLAG" 
		if bAdd then
			Privileges =  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,TOGGLE_QUALITY_CHECK_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	            
	Case  "TOGGLE_QUALITY_CHECK_FLAG" 
		if bAdd then
			Privileges =  "DELETE ON REG_QUALITY_CHECKED" & "," &  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges =  "DELETE ON REG_QUALITY_CHECKED" 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			dependencies = "SET_QUALITY_CHECK_FLAG,TOGGLE_APPROVED_FLAG,SET_APPROVED_FLAG,REGISTER_TEMP,EDIT_COMPOUND_REG,EDIT_BATCH_REG,EDIT_SALT_REG,EDIT_IDENTIFIERS_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if    
	                                     
	Case  "EDIT_SALT_TABLE" 
		if bAdd then
			Privileges =  "UPDATE ON SALTS" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges ="UPDATE ON SALTS" 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
		
	Case  "ADD_SALT_TABLE" 
		if bAdd then
			Privileges =  "INSERT ON SALTS" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if RevokeOK then
			Privileges = "INSERT ON SALTS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
		
		
	Case  "DELETE_SALT_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON SALTS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON SALTS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if  
	                    
		
	                                
	Case  "EDIT_SEQUENCES_TABLE" 

		if bAdd then
			Privileges =  "UPDATE ON SEQUENCE" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "REGISTER_TEMP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, PrivilegeName,dependencies)
			if RevokeOK then
				Privileges = "UPDATE ON SEQUENCE" 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if

	Case  "ADD_SEQUENCES_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON SEQUENCE" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON SEQUENCE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
		
	Case  "DELETE_SEQUENCES_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON SEQUENCE" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON SEQUENCE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if  
	                                
	Case  "EDIT_NOTEBOOKS_TABLE" 
		if bAdd then
			Privileges =  "UPDATE ON NOTEBOOKS" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges ="UPDATE ON NOTEBOOKS" 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
		
	Case  "DELETE_NOTEBOOKS_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON NOTEBOOKS" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON NOTEBOOKS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
		
	Case  "ADD_NOTEBOOKS_TABLE" 
		if bAdd then
			Privileges =  "INSERT ON NOTEBOOKS" & "," &  RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON NOTEBOOKS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
	                                 
	Case  "EDIT_PROJECTS_TABLE" 

		if bAdd then
			Privileges =  "UPDATE ON PROJECTS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "UPDATE ON PROJECTS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
	                                 
	Case  "ADD_PROJECTS_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON PROJECTS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON PROJECTS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                               
	Case  "DELETE_PROJECTS_TABLE" 

		if bAdd then
			Privileges =  "DELETE ON PROJECTS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON PROJECTS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                                 
	Case  "EDIT_SOLVATES_TABLE" 
		if bAdd then
			Privileges =  "UPDATE ON SOLVATES" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "UPDATE ON SOLVATES"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                           
	                                  
	Case  "ADD_SOLVATES_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON SOLVATES" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON SOLVATES"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                               
	Case  "DELETE_SOLVATES_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON SOLVATES" & ","  & RegLookupTablesSELECT & "," & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON SOLVATES"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                            
	Case  "EDIT_COMPOUND_TYPE_TABLE" 
		if bAdd then
			Privileges =  "UPDATE ON COMPOUND_TYPE" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "UPDATE ON COMPOUND_TYPE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                            
	Case  "ADD_COMPOUND_TYPE_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON COMPOUND_TYPE" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON COMPOUND_TYPE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                          
	Case  "DELETE_COMPOUND_TYPE_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON COMPOUND_TYPE" & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON COMPOUND_TYPE"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                             
	Case  "EDIT_UTILIZATIONS_TABLE" 

		if bAdd then
			Privileges =  "UPDATE ON UTILIZATIONS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
				Privileges = "UPDATE ON UTILIZATIONS"
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                              
	Case  "ADD_UTILIZATIONS_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON UTILIZATIONS" & "," & RegLookupTablesSELECT & "," & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON UTILIZATIONS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                           
	Case  "DELETE_UTILIZATIONS_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON UTILIZATIONS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON UTILIZATIONS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                         
	                           
	Case  "EDIT_BATCH_PROJECTS_TABLE" 

		if bAdd then
			Privileges =  "UPDATE ON BATCH_PROJECTS,UPDATE ON BATCH_PROJ_UTILIZATIONS,INSERT ON BATCH_PROJ_UTILIZATIONS" & "," &  RegisteredCompoundsINSERT & "," & RegisteredCompoundsUPDATE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "ADD_BATCH_PROJECTS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = "INSERT ON BATCH_PROJ_UTILIZATIONS"
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
			Privileges = "UPDATE ON BATCH_PROJECTS,UPDATE ON BATCH_PROJ_UTILIZATIONS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                            
	Case  "ADD_BATCH_PROJECTS_TABLE" 

		if bAdd then
			Privileges =  "INSERT ON BATCH_PROJECTS,INSERT ON BATCH_PROJ_UTILIZATIONS" & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "INSERT ON BATCH_PROJECTS,INSERT ON BATCH_PROJ_UTILIZATIONS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if
	                         
	Case  "DELETE_BATCH_PROJECTS_TABLE" 
		if bAdd then
			Privileges =  "DELETE ON BATCH_PROJECTS,DELETE ON BATCH_PROJ_UTILIZATIONS" & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = "DELETE ON BATCH_PROJECTS,DELETE ON BATCH_PROJ_UTILIZATIONS"
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if   
	                                   
	Case  "EDIT_USERS_TABLE" 
	
	
	
	

		if bAdd then
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER,CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,CREATE ROLE,GRANT ANY PRIVILEGE"
			isOK = DoProcessPriv(DBA_CONN, ProcessType, PrivilegeName, Privileges)   

			Privileges =Privileges & "," & SecurityTablesSELECT & "," &  SecurityTablesDELETE & "," &  SecurityTablesINSERT & "," & SecurityTablesUPDATE
	  		isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   

	    end if
	    if bRevoke then
			Privileges ="SELECT ANY TABLE,CREATE USER,DROP USER,ALTER USER,CREATE ROLE,ALTER ANY ROLE,DROP ANY ROLE,GRANT ANY ROLE,CREATE ROLE,GRANT ANY PRIVILEGE"
			isOK = DoProcessPriv(DBA_CONN, ProcessType, PrivilegeName, Privileges)
			
			dependencies = "DELETE_PEOPLE_TABLE,DELETE_WORKGROUP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = "SecurityTablesDELETE"
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
			
			dependencies = "EDIT_PEOPLE_TABLE,EDIT_WORKGROUP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges =  SecurityTablesUPDATE
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
			
			dependencies = "ADD_PEOPLE_TABLE,ADD_WORKGROUP"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesINSERT 
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if   
	                            
	Case  "EDIT_PEOPLE_TABLE" 

		if bAdd then
			Privileges = SecurityTablesSELECT & "," & SecurityTablesUPDATE
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "EDIT_WORKGROUP,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesUPDATE
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if
	                                   
	Case  "ADD_PEOPLE_TABLE"

		if bAdd then
			Privileges = SecurityTablesSELECT & "," & SecurityTablesINSERT
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "ADD_WORKGROUP,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesINSERT
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if 
	                                
	Case  "DELETE_PEOPLE_TABLE" 

		if bAdd then
			Privileges = SecurityTablesSELECT & "," &  SecurityTablesDELETE 
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "DELETE_WORKGROUP,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesDELETE
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if 
	                                      
	Case  "EDIT_WORKGROUP" 
		if bAdd then
			Privileges = SecurityTablesSELECT  & "," & SecurityTablesUPDATE
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "EDIT_PEOPLE_TABLE,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesUPDATE
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if
	                                       
	Case  "ADD_WORKGROUP" 

		if bAdd then
			Privileges = SecurityTablesSELECT & "," & SecurityTablesINSERT
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "ADD_PEOPLE,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesINSERT
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if 
	                                    
	Case  "DELETE_WORKGROUP" 

		if bAdd then
			Privileges = SecurityTablesSELECT & "," &  SecurityTablesDELETE 
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)   
	    end if
	    if bRevoke then
			dependencies = "DELETE_PEOPLE_TABLE,EDIT_USERS_TABLE"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = SecurityTablesDELETE
				isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
			end if
		end if 
	                                      
	Case  "EDIT_EVAL_DATA" 
		if bAdd then
			Privileges = AnalyticsDataUPDATE &  "," & RegisteredCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = AnalyticsDataUPDATE 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if        
	                                      
	Case  "ADD_EVAL_DATA" 
		if bAdd then
			Privileges = AnalyticsDataINSERT &  "," & RegisteredCompoundsSELECT  & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = AnalyticsDataINSERT 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if        
	                                    
	Case  "SEARCH_EVAL_DATA"
		if bAdd then
			Privileges = RegisteredCompoundsSELECT  & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			dependencies = "SEARCH_REG"
			RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
			if RevokeOK then
				Privileges = RegisteredCompoundsSELECT 
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			end if
		end if  
		
	Case "DELETE_EVAL_DATA"  

		if bAdd then
			Privileges = AnalyticsDataDELETE  & "," & RegisteredCompoundsSELECT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = AnalyticsDataDELETE 
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if  

	Case  "EDIT_SITES_TABLE" 

		if bAdd then
			Privileges =   RegLookupTablesSELECT  & "," & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			Privileges = "UPDATE ON SITES" & "," & SecurityTablesSELECT
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if
		if bRevoke then
			Privileges = "UPDATE ON SITES"
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if
	                            


	Case  "ADD_SITES_TABLE"

		if bAdd then
			Privileges =   RegLookupTablesSELECT  & "," & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			Privileges = "INSERT ON SITES" & "," & SecurityTablesSELECT
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if
		if bRevoke then
			Privileges = "INSERT ON SITES"
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if
		
		

	Case  "DELETE_SITES_TABLE"
		if bAdd then
			Privileges =   RegLookupTablesSELECT  & "," & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			Privileges = "DELETE ON SITES" & "," & SecurityTablesSELECT
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if
		if bRevoke then
			Privileges = "INSERT ON SITES"
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, Privileges)
		end if

	Case "EDIT_ANALYTICS_TABLES"
		if bAdd then
			Privileges =  AnalyticsLookupTablesUPDATE & "," & AnalyticsLookupTablesINSERT & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
				dependencies = "ADD_Analytics_TABLES"
				RevokeOK = checkDependency(dbkey, formgroup, SEC_CONN, RS, RoleName,dependencies)
				if RevokeOK then
					Privileges = AnalyticsLookupTablesINSERT 
					isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
				end if
				Privileges = AnalyticsLookupTablesUPDATE
				isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                  

	Case "DELETE_ANALYTICS_TABLES"
		if bAdd then
			Privileges =  AnalyticsLookupTablesDELETE & "," & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = AnalyticsLookupTablesDELETE
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if  
		
	Case "ADD_ANALYTICS_TABLES"
		if bAdd then
			Privileges =  AnalyticsLookupTablesINSERT & ","  & RegLookupTablesSELECT & ","  & HitListTables
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
			isOK = DoProcessPriv(SEC_CONN, ProcessType, PrivilegeName, SecurityTablesSELECT)
		end if
		if bRevoke then
			Privileges = AnalyticsLookupTablesINSERT
			isOK = DoProcessPriv(REG_Conn, ProcessType, PrivilegeName, Privileges)
		end if    
	                                     
	Case  "SITE_ACCESS_ALL" 

End Select
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
		if NOT ((Object = "PEOPLE") OR (Object = "SITES") OR (Object = "CHEM_REG_PRIVILEGES") OR (Object = "SECURITY_ROLES") OR (Object = "PRIVILEGE_TABLES")) then
			Response.Write "INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES "
			Response.Write "('" & PrivilegeName & "', '" & Priv & "', 'REGDB', '" & Object  & "');" & vblf
		ELSE
			Response.Write "INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES "
			Response.Write "('" & PrivilegeName & "', '" & Priv & "', 'CS_SECURITY', '" & Object  & "');" & vblf
		end if	
	next
	DoProcessPriv = ""
End function
</SCRIPT>
