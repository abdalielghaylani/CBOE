<SCRIPT LANGUAGE="vbscript" RUNAT="server">
'**********************************************************************
'****** ADO Connection routines
'**********************************************************************

'-------------------------------------------------------------------------------
' Name: GetNewConnection(dbkey, formgroup, conn_name)
' Type:  function
' Purpose: Create data connection
' Inputs: dbkey as string, formgroup as string, table connection name as string
' Returns: data connection reference
'-------------------------------------------------------------------------------
Function GetNewConnection(ByVal dbkey, ByVal formgroup, ByVal conn_name)
		Dim DataConn
		
		full_conn_string = GetADOConnString(dbkey, formgroup, conn_name)
		Set DataConn=Server.CreateObject("ADODB.Connection")
			
		on error resume next

		DataConn.Open full_conn_string
		if err.number <> 0 then
			logaction "Error in GetNewConnection: " & err.Description & "constr:= " & full_conn_string
			DataConn.Close
			Set DataConn = Nothing
			DataConn = ""
			HandleError dbkey, formgroup, "conn_string", "GetNewConnection", err.number, err.description
		End if
		
		Set GetNewConnection = DataConn
	
End Function



'-------------------------------------------------------------------------------
' Name: GetADOConnString(dbkey, formgroup, conn_name)
' Type:  function
' Purpose: Get full connection string.
' Inputs: dbkey as string, formgroup as string, table connection name as string
' Returns: connection string as string
'-------------------------------------------------------------------------------
Function GetADOConnString(ByVal dbkey, ByVal formgroup, ByVal conn_name)
	bLoginRequired = false
	bLoginInfoNeeded = false
	if Not conn_name <> "" then conn_name = "base_connection"
	
	conn_info_array = Application(conn_name & dbkey)
		
	if not isArray(conn_info_array) then
		DoLoggedOutMsg()
	end if
	conn_type=conn_info_array(kConn)
		if sf_DEBUG = true then Response.Write "<br>conn_type:" & conn_type
	conn_string=conn_info_array(kConnStr)
		if sf_DEBUG = true then Response.Write "<br>conn_string:" & conn_string

	conn_conn_timeout=conn_info_array(kConnTimeOut)
		if sf_DEBUG = true then Response.Write "<br>conn_conn_timeout:" & conn_conn_timeout

	conn_command_timeout=conn_info_array(kConnConnTimeOut)
		if sf_DEBUG = true then Response.Write "<br>conn_command_timeout:" & conn_command_timeout

	conn_username=conn_info_array(kConnUserName)
		if sf_DEBUG = true then Response.Write "<br>conn_username:" & conn_username
		
	conn_password=conn_info_array(kConnPassword)
		if sf_DEBUG = true then Response.Write "<br>conn_password:" & conn_password

	on error resume next
	conn_dbms = conn_info_array(kDBMS)
	Application("BaseConnDBMS") = Ucase(conn_dbms)
		if err.number > 0 then conn_dbms = "ACCESS"
	on error goto 0
	if not conn_dbms <> "" then conn_dbms = "ACCESS"
			if sf_DEBUG = true then Response.Write "<br>conn_dbms:" & conn_dbms
	Select case UCase(conn_dbms)
		Case "ORACLE"
			if conn_username = "login_required"	then
				bLoginRequired = true 
				UserIDKeyword = "UID"
				If Application("UserIDKeyword") <> "" then UserIDKeyword = Application("UserIDKeyword")
				conn_username = UserIDKeyword & "=" & Application(dbkey & "_USERNAME")
				
			end if
			conn_password=conn_info_array(kConnPassword)
			if conn_password = "login_required"then
				bLoginRequired = true
				PWDKeyword = "PWD"
			
				If Application("PWDKeyword") <> "" then PWDKeyword =Application("PWDKeyword")
				'LJB 10/14/04  check encryption state of application schema password and take appropriate action.
	
				Application.Lock
					Application("ENCRYPT_PWD")=GetINIValue("optional", "GLOBALS", "ENCRYPT_PWD","web_app", "cfserver")
					Application("ENCRYPT_PWD_SECTION")=GetINIValue("optional", "GLOBALS", "ENCRYPT_PWD_SECTION","web_app", "cfserver")
					Application("ENCRYPT_PWD_KEYS")=GetINIValue("optional", "GLOBALS", "ENCRYPT_PWD_KEYS","web_app", "cfserver")
				Application.UnLock
				if Ucase(Application("ENCRYPT_PWD")) = "TRUE" then
						if  needsEncryption(dbkey, "APP_SCHEMA",Application("ENCRYPT_PWD_SECTION")) then 
							encryptPWDAndWriteToINI dbkey, "APP_SCHEMA",Application("ENCRYPT_PWD_SECTION")
							decryptPWDAndStore dbkey, "APP_SCHEMA",Application("ENCRYPT_PWD_SECTION")
						else
							decryptPWDAndStore dbkey, "APP_SCHEMA",Application("ENCRYPT_PWD_SECTION")
						end if
						Application.Lock
							Application("APP_SCHEMA_ENCRYPTION_CHECKED")="TRUE"
						Application.UnLock
				end if
			
				conn_password = PWDKeyword & "=" & Application(dbkey & "_PWD")			
				
			end if
			
		Case "SQLSERVER"
			if conn_username = "login_required"	then
				bLoginRequired = true 
				conn_username = "username=" & Session("UserName" & dbkey)
				
			end if
			conn_password=conn_info_array(kConnPassword)
			if conn_password = "login_required"then
				bLoginRequired = true
				conn_password = "password=" & Session("UserID" & dbkey)
				
			end if
				
		Case "ACCESS"
			if conn_username = "login_required"	then
				bLoginRequired = true 
				conn_username =  Session("UserName" & dbkey)
				
			end if
			conn_password=conn_info_array(kConnPassword)
			if conn_password = "login_required"then
				bLoginRequired = true
				conn_password = Session("UserID" & dbkey)
				
			end if
	End Select
	
	'create full connection string
	if conn_type = "NULL" or conn_type = "" or conn_type= "OLEDB" then
		full_conn_string = conn_string & "; " & conn_username & "; " & conn_password		
	else
		full_conn_string = conn_type & "=" & conn_string & "; " & conn_username & "; " & conn_password		
	end if	
	if sf_DEBUG = true then Response.Write "<br>full_conn_string:" & full_conn_string
	
	' Remember the first connection string
	if Application("FirstConnStr") = "" then 
		Application("FirstConnStr")= full_conn_string 
	End if
	
	GetADOConnString = full_conn_string
	
end function

Sub GetConnection (ByVal connection_name, ByVal currentDB)
			on error resume next
			set DataConn = getNewConnection(currentDB,"base_form_group",connection_name)
				
				if err.number <> 0  then
					logaction "Error in GetConnection: " & err.Source & "--" & err.Description 
					storeDescription = err.description
					returnedError = checkADOErrors(DataConn)
					if returnedError > 0 then
						conn_info_array = Application(connection_name & currentdb)
						conn_type=conn_info_array(kConn)
							if sf_DEBUG = true then Response.Write "<br>conn_type:" & conn_type
						conn_string=conn_info_array(kConnStr)
							if sf_DEBUG = true then Response.Write "<br>conn_string:" & conn_string

						conn_conn_timeout=conn_info_array(kConnTimeOut)
							if sf_DEBUG = true then Response.Write "<br>conn_conn_timeout:" & conn_conn_timeout

						conn_command_timeout=conn_info_array(kConnConnTimeOut)
							if sf_DEBUG = true then Response.Write "<br>conn_command_timeout:" & conn_command_timeout

						conn_username=conn_info_array(kConnUserName)
							if sf_DEBUG = true then Response.Write "<br>conn_username:" & conn_username
							
						conn_password=conn_info_array(kConnPassword)
							if sf_DEBUG = true then Response.Write "<br>conn_password:" & conn_password
						Application("inipath") = Application("AppPath")  & "\config\" & currentDB &".ini"
						udlpath =  Application("AppPath")  & "\config\" & currentDB &".udl"
						Select Case returnedError
							Case 1
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""
								Select case UCase(conn_type)
								case "DSN" 
										RaiseDBError "Possible Cause 1: ", "The DSN Name specified in the ini file does not exist: ", "<br>Current DSN name :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the DSN name in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "Solution 2: ", "Check that the SYSTEM DSN exists in the ODBC control panel." , ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								case "FILE NAME"
										RaiseDBError "Possible Cause 1: ", "The  path  to the database specified within the udl file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the path in the udl file", ""
										RaiseDBError "The udl file path is: ", udlpath  , ""
								case "DBQ"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The ini file path is: ", Application("inipath") , ""
								Case "OLEDB"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Possible Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								end select
						
							
							Case 2
							
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""
								Select case UCase(conn_type)
								case "DSN" 
										RaiseDBError "Possible Cause 1: ", "The DSN Name specified in the ini file does not exist: ", "<br>Current DSN name :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the DSN name in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "Solution 2: ", "Check that the DSN exists in the ODBC control panel." , ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								case "FILE NAME"
										RaiseDBError "Possible Cause 1: ", "The path TO the udl file specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the udl file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								case "DBQ"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								Case "OLEDB"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Possible Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								end select
						
								RaiseDBError "Possible Cause 2: ","The access permissions on the directory containing the database or the database file are too restrictive.", ""
								RaiseDBError "Solution 1: ", "From the server's desktop change the security permissions of the directory and contained database files to Authenticated Users having Read and Write Access" , ""
							
							Case 3
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""		
								RaiseDBError "Likely Cause: ", "The username and/or password are incorrect: " ,  " <br> Current username string: " & conn_username  & "<br>  Current Password String:" & conn_password
								RaiseDBError "Solution: ", "Correct the username and password in the " & UCase(connection_name) & " section of the ini file." , ""
								RaiseDBError "The INI file path is: ", Application("inipath") , ""
							
							Case 4
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""
								
								path = split(storeDescription, "is not", -1)
								filepath = path(0)
								Select case UCase(conn_type)
								case "DSN" 
										RaiseDBError "Possible Cause 1: ", "The file path specified in the DSN does not exist: ", "<br>Current DSN name :" & filepath
										RaiseDBError "Solution 1: ", "Correct the path within the DSN named in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								case "FILE NAME"
										RaiseDBError "Possible Cause 1: ", "The  path  to the database specified within the udl file does not exist: ", "<br>Current path :" & filepath
										RaiseDBError "Solution 1: ", "Correct the path in the udl file", ""
										RaiseDBError "The udl file path is: ", udlpath  , ""
								case "DBQ"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The ini file path is: ", Application("inipath") , ""
								Case "OLEDB"
										RaiseDBError "Possible Cause 1: ", "The file path specified in the ini file does not exist: ", "<br>Current path :" & conn_string
										RaiseDBError "Possible Solution 1: ", "Correct the file path in the " & UCase(connection_name) & " section of the ini file.", ""
										RaiseDBError "The INI file path is: ", Application("inipath") , ""
								end select
							Case 5
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""
								
						
								RaiseDBError "Possible Cause 1: ","The access permissions on the directory containing the database or the database file are too restrictive.", ""
								RaiseDBError "Solution 1: ", "From the server's desktop change the security permissions of the directory and contained database files to Authenticated Users having Read and Write Access" , ""
							Case 6
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""
								
						
								RaiseDBError "Possible Cause 1: ","The file is open by another user.", ""
								RaiseDBError "Solution 1: ", "Close the application using the file." , ""
						
							case 20
								RaiseDBError "Error Description: ","A connection to the datasource could not be established." , ""	
							
							End Select
						
						Set DataConn = Nothing
						DataConn = ""
					
					end if
				Else
				
					'check cmd object createion
					Set cmd = server.CreateObject("adodb.command")
					cmd.ActiveConnection = DataConn
					cmd.CommandType=adCmdText
					
					'check for read only if database is access or sqlserver
					
					
					
					syntax = UCase(Application("sqlSyntax"))
					if syntax = "ACCESS" or syntax="SQL_SERVER" or syntax= "SQLSERVER" then
						theReturn = checkDBReadOnly (cmd)
						if theReturn <> "" then
							RaiseDBError "Error Description: ", "The database is read only or the file permissions do not include read and write permissions to the database", ""
							RaiseDBError "Solution step 1: ", "Locate the database on the server, right-click and choose properties." , ""
							RaiseDBError "Solution step 2: ", "For the database file and the associated .mst and .msi files do the following:" , ""
							RaiseDBError "Solution step 3: ", "Deselect the read-only flag." , ""
							RaiseDBError "Solution step 4: ", "Click the Security tab." , ""
							RaiseDBError "Solution step 5: ", "Add the user ""Authenticated User"" and give that user read and write permissions on the database." , ""
							RaiseDBError "Solution step 6: ", "Click apply and exit the dialog." , ""
							RaiseDBError "Solution step 6: ", "Repeat this for the directory containing the database." , ""
							RaiseDBError "Solution step 6: ", "Try accessing the applicaion again." , ""

						end if
					end if
					DataConn.Close
					Set DataConn = Nothing
					DataConn = ""
				end if
		if NOT (UCASE(conn_username) = "LOGIN_REQUIRED" OR UCASE(conn_password) = "LOGIN_REQUIRED")then
			Application("LoginRequired" & currentdb) = 0
		else
			Application("LoginRequired" & currentdb) = 1
		end if
end Sub

Function checkDBReadOnly(byRef cmd)
	on error resume next
	sql = "CREATE TABLE STARTUP_TEST"
	cmd.commandtext = sql
	cmd.execute
	
	if not err.number = -2147217911 then
		provider = DataConn.Provider
		sql = "DROP TABLE STARTUP_TEST"
		cmd.CommandText = sql
		cmd.execute
		theResult = ""
	else
		theResult = err.number & err.description
	end if
	checkDBReadOnly=theResult
End Function

Function checkADOErrors(ByRef DataConn)

	fatalError= 20
	test = err.Description
	test = UCase(test)
	if InStr(test, "THE PATH COULD NOT BE FOUND")> 0 then
		fatalError = 2
	end if
	if InStr(test, "DATA SOURCE NAME")> 0 then
		fatalError = 1
	end if
	if InStr(test, "DEFAULT DRIVER")> 0 then
		fatalError = 1
	end if
	
	if InStr(test, "IS NOT A VALID PATH")> 0 then
		fatalError = 4
	end if
	if InStr(test, "PERMISSION")> 0 then
		fatalError = 5
	end if
	if InStr(test, "LOGIN FAILED")> 0 then
		fatalError = 3
	end if
	if InStr(test, "SQLSETCONNECTATTR FAILED")> 0 then
		fatalError = 1
	end if
	If Instr(test, "in use")>0 then
		fatalError = 6
	end if
checkADOErrors = fatalError

End Function	


sub CheckMstFiles(chem_conn_name, StrucDBpath)

	fileExists = doesFileExist(StrucDBpath)
	if fileExists = true then
		
		isReadOnly = isFileReadOnly(StrucDBpath)
		
		if isReadOnly = true then
			RaiseDBError "PROBLEM" ,"mst file at is read only or the permission on the mst file are too limited", ""
			RaiseDBError "SOLUTION" ,"Locate " & StrucDBpath  & " on the server and remove the read only attribute", ""
			RaiseDBError "SOLUTION2" ,"Locate " & StrucDBpath  & " on the server and change the security to Authenticated Users full control", ""
		end if
	else
		RaiseDBError "PROBLEM" ," mst file does not exist or the permissions on the mst file are too limited", ""
		RaiseDBError "SOLUTION" ,"Change the StrucDBpath in the  " & chem_conn_name  & " section of the ini file to the correct path", ""
		RaiseDBError "SOLUTION2" ,"If the path is correct then Locate " & StrucDBpath  & " on the server and change the security to Authenticated Users full control", ""

	end if 

end sub

Sub AppInitAllTables()
		'New sub to set additional Application variables
		SetAdditionalAppVariables()
		AppInitUserSettingTable()
		AppInitQueryTables()
		AppInitHitlistTables()
End Sub

' This sub is a good place to initialize application variables which were previously initialized
' in global_app_vbs.asp.  It is better to do it here because this code runs during app start up and
' avoids the use of extra logic to check if the variable has been created.
Sub SetAdditionalAppVariables()
	
	' Setting COWS version used by about boxes to same version as COE version in chemoffice.ini
	Application("COWSVersion") = GetINIValue2("optional", "GLOBALS", "CHEMOFFICE_ENTERPRISE_VERSION", "chemoffice", "chemoffice")
	
	' Optional ini for CDAX progID, defaults to 9
	Application("CDAX_ProgID") = GetINIValue2("optional", "GLOBALS", "CDAX_PROGID", "chemoffice", "chemoffice")
	Application("ForceHTTPS") = GetINIValue2("optional", "GLOBALS", "FORCE_HTTPS", "chemoffice", "chemoffice")
	if Ucase(Application("ForceHTTPS")) = "TRUE" then
             Application("SERVER_TYPE")="https://"
        else
             Application("SERVER_TYPE")="http://"
        end if

	SetIniParamDefault "CDAX_ProgID", "ChemDrawControl10.ChemDrawCtl"
	' Check LDAP Authentication Option
	Application("LDAPConfigXmlPath") = Application("ServerDrive") & "\"&  Application("ServerRoot") & "\"  & Application("DocumentRoot") & "\chemoffice\config\LDAPConfig.xml"
	SetIniParamDefault "AUTHENTICATION_MODE", "COELDAP"
	
	' Check Active Directory Domain
	
	SetIniParamDefault "ACTIVE_DIRECTORY_DOMAIN", ""
	
	' Check LDAP Service Account Name
	
	SetIniParamDefault "LDAP_SERVICE_ACCOUNT_NAME", "ORACLE"
	
	' Check LDAP Service Account PWD
	
	SetIniParamDefault "LDAP_SERVICE_ACCOUNT_PWD", ""
	
	' Check UNCORRELATED_SDF_FLATENING
	Application("UNCORRELATED_SDF_FLATENING") = GetINIValue("optional", "GLOBALS", "UNCORRELATED_SDF_FLATENING", "web_app", "cfserver")
	SetIniParamDefault "UNCORRELATED_SDF_FLATENING", "0"
	
	' MolServer Version read from cfserver ini, then from config.ini, defaults to 9 
	Application("MOLSERVER_VERSION")=GetINIValue( "optional", "GLOBALS", "MOLSERVER_VERSION", "web_app", "cfserver")
	if (Application("MOLSERVER_VERSION")="INIEmpty" or Application("MOLSERVER_VERSION") = "NULL" or Application("MOLSERVER_VERSION") = "") then
		Application("MOLSERVER_VERSION")=GetINIValue2( "optional", "GLOBALS", "MOLSERVER_VERSION", "chemoffice", "chemoffice")
		SetIniParamDefault "MOLSERVER_VERSION", "9"
	end if
	
	' Get Cartridge Version and Major Version
	if Application("BaseConnDBMS") = "ORACLE" then
		Application("CSCartridgeVersion") = GetCsCartridgeVersion()
	else
		Application("CSCartridgeVersion") = ""
		Application("CSCartridgeMajorVersion")=""
	End if
	
	' Optional ini entries to override the text in the global search database selector
	Application("GlobalSearchDisplayText")=GetINIValue( "optional", "GLOBALS", "GLOBAL_SEARCH_DISPLAY_TEXT", "web_app", "cfserver")
	Application("GlobalSearchResultText") = GetINIValue( "optional", "GLOBALS", "GLOBAL_SEARCH_RESULT_TEXT", "web_app", "cfserver")

	' Optional entries for Add Mode in wizard apps
	Application("AllowAddMode")  = GetINIValue( "optional", "GLOBALS", "ALLOW_ADD_MODE", "web_app", "cfserver")
	SetIniParamDefault "AllowAddMode", "false"
	
	' Float Format
	Application("FLOAT_FORMAT") = GetINIValue("optional", "GLOBALS", "FLOAT_FORMAT", "web_app", "cfserver")
	SetIniParamDefault "FLOAT_FORMAT", "8"
	
	if Application("Date_Format") = "" then
	    Application("Date_Format") = GetINIValue("optional", "GLOBALS", "DATE_FORMAT", "web_app", "cfserver")
	    SetIniParamDefault "DATA_FORMAT", "9"
	End if
	
	if Application("Date_Format_String") = "" then
	    Select Case Application("Date_Format")
		    Case "8"
			    Application("Date_Format_String") = "MM/DD/YYYY"
		    Case "9"
			    Application("Date_Format_String") = "DD/MM/YYYY"
		    Case "10"
			    Application("Date_Format_String") = "YYYY/MM/DD"
	    End Select
	End if
	
	
	'LJB 5/3/2004 Enable SDFile Search
	Application("ENABLE_SDFILE_SEARCH") = GetINIValue("optional", "GLOBALS", "ENABLE_SDFILE_SEARCh", "web_app", "cfserver")
	SetIniParamDefault "ENABLE_SDFILE_SEARCH", "0"

	'DGB sdfile export options 
	Application("ALLOW_FLAT_SDFILE_EXPORT") = GetINIValue("optional", "GLOBALS", "ALLOW_FLAT_SDFILE_EXPORT", "web_app", "cfserver")
	SetIniParamDefault "ALLOW_FLAT_SDFILE_EXPORT", "0"
	Application("ALLOW_RDFILE_EXPORT") = GetINIValue("optional", "GLOBALS", "ALLOW_RDFILE_EXPORT", "web_app", "cfserver")
	SetIniParamDefault "ALLOW_RDFILE_EXPORT", "0"


	'LJB 10/14/04  check encryption state of other pwd keys in ENCRYPT_PWD_KEYS key
	if Ucase(Application("ENCRYPT_PWD")) = "TRUE" and Ucase(Application("APP_SCHEMA_ENCRYPTION_CHECKED"))="TRUE" then
		pwd_keys_array = split(Application("ENCRYPT_PWD_KEYS"), ",", -1)
		for t = 0 to UBound(pwd_keys_array)
			pwd_key = pwd_keys_array(t)
			if Application(pwd_key & "_CHECKED") ="" then
				if  needsEncryption("", pwd_key,Application("ENCRYPT_PWD_SECTION")) then 
					encryptPWDAndWriteToINI dbkey, pwd_key,Application("ENCRYPT_PWD_SECTION")
					decryptPWDAndStore "", pwd_key,Application("ENCRYPT_PWD_SECTION")
				else
					decryptPWDAndStore "", pwd_key,Application("ENCRYPT_PWD_SECTION")
				end if
				Application.Lock
					Application(pwd_key & "_CHECKED") ="TRUE"
				Application.UnLock
			end if
		next
	end if
	
	'LJB 3/2005 Export dialog Template feature.
	Application("ENABLE_EXPORT_TEMPLATES") = GetINIValue("optional", "GLOBALS", "ENABLE_EXPORT_TEMPLATES", "web_app", "cfserver")
	SetIniParamDefault "ENABLE_EXPORT_TEMPLATES", "1"
	
	'MW ROUND.
	Application("MW_ROUND_DIGIT") = GetINIValue("optional", "GLOBALS", "MW_ROUND_DIGIT", "web_app", "cfserver")
	SetIniParamDefault "MW_ROUND_DIGIT", "5"
	
	'USE_MDB_STRUCTURE  Attempts display of CDX document stored in MDB
	Application("USE_MDB_STRUCTURE") = GetINIValue("optional", "GLOBALS", "USE_MDB_STRUCTURE", "web_app", "cfserver")
	SetIniParamDefault "USE_MDB_STRUCTURE", "0"
	
	'To fix CSBR-144175: Allowing CSCARTRIDGE_USERNAME to be configured
	Application("CSCARTRIDGE_USERNAME") = GetINIValue("optional", "GLOBALS", "CSCARTRIDGE_USERNAME", "web_app", "cfserver")
	SetIniParamDefault "CSCARTRIDGE_USERNAME", "CsCartridge"

	'To fix CSBR-144175: Allowing CSCARTRIDGE_PWD to be configured
	Application("CSCARTRIDGE_PWD") = GetINIValue("optional", "GLOBALS", "CSCARTRIDGE_PWD", "web_app", "cfserver")
	SetIniParamDefault "CSCARTRIDGE_PWD", "CsCartridge"
	
	'Optional entry to require additional user attributes while creating a cs_security user
	'Used to determin the requried fields on the user account page.
	Application("UserInfoRequiredFieldList") = "username,password,roles,lastname"
	additionalFields=GetINIValue( "option", "CS_SECURITY", "ADDITIONAL_REQUIRED_USER_ATTRIBUTES", "web_app", "cfserver")
	if NOT (additionalFields="INIEmpty" or additionalFields = "NULL" or additionalFields = "") then
		Application("UserInfoRequiredFieldList") = Application("UserInfoRequiredFieldList") & "," & additionalFields 
	end if
	userFieldList = split(Application("UserInfoRequiredFieldList"),",")
	for j = 0 to UBound(userFieldList)
		if userFieldList(j) <> "" then
			Application("UserInfoRequiredFieldList" & UCase(userFieldList(j))) = true
		end if
	next

	' DGB
	' Cookie Expiration must be set for non cs_Security appications.  We set it here for all apps instead of setting it in let
	' GetSecurityINIValues
	Application("CookieExpiresMinutes")=GetINIValue( "optional", "CS_SECURITY", "COOKIE_EXPIRES_MINUTES", "web_app", "cfserver")
	if (Application("CookieExpiresMinutes")="INIEmpty" or Application("CookieExpiresMinutes") = "NULL" or Application("CookieExpiresMinutes") = "") then
		Application("CookieExpiresMinutes")="20"
	end if
	' DGB
	' New optional setting to support in-memory browser cookies
	Application("PERSIST_AUTHENTICATION_COOKIES")=GetINIValue( "optional", "CS_SECURITY", "PERSIST_AUTHENTICATION_COOKIES", "web_app", "cfserver")
	if (Application("PERSIST_AUTHENTICATION_COOKIES")="INIEmpty" or Application("PERSIST_AUTHENTICATION_COOKIES") = "NULL" or Application("PERSIST_AUTHENTICATION_COOKIES") = "") then
		Application("PERSIST_AUTHENTICATION_COOKIES")="1"
	end if

	' ORA_SERVICENAME Required for all apps to use chemimp during sdf search
	if IsEmpty(Application("ORA_SERVICENAME")) then
		Application("ORA_SERVICENAME")=GetINIValue( "optional", "GLOBALS", "ORA_SERVICENAME", "web_app", "cfserver")
		if (Application("ORA_SERVICENAME")="INIEmpty" or Application("ORA_SERVICENAME") = "NULL" or Application("ORA_SERVICENAME") = "") then
			Application("ORA_SERVICENAME")=""
		end if
	End if
	
	
	SetIniParamDefault "SHOW_ISIS_PREF", "0"
	
End Sub

Function GetCSCartridgeVersion()
	Dim out
	Dim c, RS
	
	Set c= Server.CreateObject("ADODB.Connection")
	c.Open(Application("FirstConnStr"))
	Set RS = c.Execute("Select value from cscartridge.globals where ID = 'VERSION'")
	on error resume next
	if err then
		RaiseDBError "PROBLEM" ," error while trying to determine CsCartridge version", ""
		RaiseDBError "SOLUTION" ,"Verify that CsCarridge is properly installed", ""
		RaiseDBError "ERROR MESSAGE" , err.Source & "-" & err.Description, ""
	else
		if (RS.eof and Rs.eof) then
			RaiseDBError "PROBLEM" ," version not found while reading CsCartridge GLOBALS table.", ""
			RaiseDBError "SOLUTION" ,"Check the CScartridge.Globals table is present and contains a version entry.", ""
		else
			vTemp = RS(0).value
			vTemp = replace(vTemp,chr(34),"")
			
			vTempArr = split(vTemp,".")
			
			
			if err then
				RaiseDBError "PROBLEM" ," Error while parsing Cartridge version from cscartridge.globals table", ""
				RaiseDBError "SOLUTION" ,"Check that the value conforms to Major.Minor.SubMinor.BuildNum format", ""	
			else
				GetCSCartridgeVersion = vTemp
				Application("CsCartridgeMajorVersion")= vTempArr(0)
				Application("CsCartridgeMinorVersion")= vTempArr(1)
				Application("CsCartridgeSubMinorVersion")= vTempArr(2)
				Application("CsCartridgeBuildNumber")= vTempArr(3)	
			end if		
		end if	
	end if
End function

Sub SessInitAllTables()
		SessInitHitlistTables()
		SessInitQueryTables()
		' This does not make sense here
		' We want to call this during session start but this sub is
		' when session end.  Moving this call somewhere were it will
		' called after the session has started.  Note that the cookie 
		' wont be there until the second request so we need to call
		' after session has started.
		'StoreASPSessionIDCookie()
End Sub


'UTILITY FUNCTIONS

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Purpose: 
'			sets the default value for a parameter read from an ini entry
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Sub SetIniParamDefault(varName, value)
	if (Application(varName) = "INIEmpty" or Application(varName) = "NULL" or Application(varName) = "") then
		Application(varName) = value
	end if
End sub


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Purpose: 
'    Generates a boolean indicating if a file exists
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Function doesFileExist(filepath)
	Dim fso
	

	Set fso = Server.CreateObject("scripting.filesystemobject")
	If NOT fso.FileExists(filepath) then
		doesFileExist=false
	else
		doesFileExist=true
	end if
	 set fso = nothing
end function
	



''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Purpose: 
'    Generates a boolean indicating if a file is real only
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Function isFileReadOnly(filepath) ' File can be a file or folder

   Dim S
   Dim Attr
   Set fso = Server.CreateObject("scripting.filesystemobject")
   set f1 = fso.GetFile(filePath)
   FileAttrReadOnly = 1
   Attr = f1.Attributes

   If Attr = 0 Then
      isFileReadOnly = false
   End If

   If Attr And FileAttrReadOnly Then
	isFileReadOnly = true
   else
	isFileReadOnly = false
   end if
   set fso = nothing

End Function


'provides access to the ini file via the cowsUtil.dll.
'The original GetINIValue function present in the apps global.asa could not read from chemoffice.ini
'this alternate function can.
Function GetINIValue2(ByVal allowempty, ByVal theSection, ByVal theKeyname, ByVal INIType, ByVal ININame )

	on error resume next
	if not isObject(INIVar) then
		Set INIVAR= Server.CreateObject("cowsUtils.cowsini") 
	end if
	Select Case INIType
		Case "cows"
			inivalue = INIVAR.VBGetPrivateProfileString(Trim(theSection), Trim(theKeyname), "cows.ini")
			if inivalue = "INIEmpty" then
				emptyval = "ini file: winnt/" & ININame & ".ini. <br>"
			End if
		Case "chemoffice"
			if ININame = "menubar" then
				chemoffice_path = Application("ServerDrive") & "\"&  Application("ServerRoot") & "\"  & Application("DocumentRoot") & "\chemoffice\config\menubar.ini"
			else
				chemoffice_path = Application("ServerDrive") & "\"&  Application("ServerRoot") & "\"  & Application("DocumentRoot") & "\chemoffice\config\chemoffice.ini"
			end if
			inivalue = INIVAR.VBGetPrivateProfileString(Trim(theSection), Trim(theKeyname), Trim(chemoffice_path))
			if inivalue = "INIEmpty" then
				emptyval = "ini file: /chemoffice/" & ININame & ".ini. <br>"
			End if
		Case "chemoffice_menu"
			'!DGB! 02/2003 use MapPath instead of building it up.  Works better when debugging
			chemoffice_path = Server.MapPath("/cfserverasp/source") & "\menubar.ini"
			inivalue = INIVAR.VBGetPrivateProfileString(Trim(theSection), Trim(theKeyname), Trim(chemoffice_path))
			if inivalue = "INIEmpty" then
				emptyval = ""
			End if
		Case "web_app"
		
			Application("inipath") = Application("AppPath")  & "\config\" & ININame &".ini"
			
			inivalue = INIVAR.VBGetPrivateProfileString(Trim(theSection), Trim(theKeyname), Trim(Application("inipath")))
			if inivalue = "INIEmpty" then
				emptyval = "ini file:  /config/" & ININame & ".ini. <br>" 
			End if
		
	End Select
	if inivalue = "INIEmpty" then
		Select case allowempty
			case "required"
				RaiseDBError "A required entry in the ini file is missing. ", emptyval , " SECTION:  " & theSection & "<br> KEY:  "& theKeyname
			case "optional"
				inivalue = ""
		End select
	end if
	
	GetINIValue2 = Trim(inivalue)
	
End Function




'LJB 10/14/04  identify if value is encrpyted
Function needsEncryption(dbkey, pwd_type, section)
	Select Case UCase(pwd_type)
		case "APP_SCHEMA"
			password_value = GetINIValue("optional", section, dbkey & "_PWD","web_app", "cfserver")
			encrypt_key =  lCase(Trim(Application(dbkey & "_USERNAME")))
			if  not DoEncryptDecryptCommand("DECRYPT",password_value, encrypt_key)= "NOT_ENCRYPTED" then
				needsEncryption = false
			else
				needsEncryption = true
			end if
		case Else
			'all other values in the ENCRYPT_PWD_KEYS key of the cfserver.ini
			password_value = GetINIValue("optional", section, pwd_type,"web_app", "cfserver")
			username_key = replace(pwd_type, "_PWD", "_USERNAME")
			encrypt_key =  lCase(Trim(Application(username_key)))
			if  UCase(Trim(password_value)) = "NULL" or Trim(password_value) = "" then
				needsEncryption = false
			else
				if  not DoEncryptDecryptCommand("DECRYPT",password_value, encrypt_key)= "NOT_ENCRYPTED" then
					needsEncryption = false
				else
					needsEncryption = true
				end if
			end if
	end select
End function

'LJB 10/14/04  decrpyt password and store in application variable used by core
Sub decryptPWDAndStore(dbkey, pwd_type, section)
	dbkey = UCase(dbkey)
	Select Case UCase(pwd_type)
		case "APP_SCHEMA"
			'schema password
			encrypt_key =  lCase(Trim(Application(dbkey & "_USERNAME")))
			password_value = GetINIValue("optional", section, dbkey & "_PWD","web_app", "cfserver")
			decrypted_value = DoEncryptDecryptCommand("DECRYPT",password_value, encrypt_key)
			Application.Lock
				Application(dbkey & "_PWD")=decrypted_value
			Application.UnLock
		case Else
			'all other values in the ENCRYPT_PWD_KEYS key of the cfserver.ini
			password_value = GetINIValue("optional", section, pwd_type,"web_app", "cfserver")
			username_key = replace(pwd_type, "_PWD", "_USERNAME")
			encrypt_key =  lCase(Trim(Application(username_key)))
			if  UCase(Trim(password_value)) = "NULL" or Trim(password_value) = "" then
				decrypted_value = ""
			else
				decrypted_value = DoEncryptDecryptCommand("DECRYPT",password_value, encrypt_key)
			end if
			Application.Lock
				Application(pwd_type)=decrypted_value
			Application.UnLock
	end select
End Sub


'LJB 10/14/04 encrpyt password and write to ini file
Sub encryptPWDAndWriteToINI(dbkey, pwd_type, section)

	Select Case UCase(pwd_type)
		case "APP_SCHEMA"
			'schema password
			key = dbkey & "_PWD"
			password_value =  GetINIValue("optional", section, key,"web_app", "cfserver")
			password_value = password_value 
			encrypt_key =  lCase(Trim(Application(dbkey & "_USERNAME")))
			encrypted_value =  DoEncryptDecryptCommand("ENCRYPT",password_value, encrypt_key)
			success = SetINIValue(section, key, encrypted_value, "APP")
		case Else
			'all other values in the ENCRYPT_PWD_KEYS key of the cfserver.ini
			key =pwd_type
			password_value =  GetINIValue("optional", section, key,"web_app", "cfserver")
			password_value =  password_value 
			if  UCase(Trim(password_value)) = "NULL" or Trim(password_value) = "" then
				encrypted_value = ""
			else
				
				username_key = replace(pwd_type, "_PWD", "_USERNAME")
				encrypt_key =  lCase(Trim(Application(username_key)))
				encrypted_value =  DoEncryptDecryptCommand("ENCRYPT",password_value, encrypt_key)
				success = SetINIValue(section, key, encrypted_value, "APP")
			end if
		End select
	
End Sub

'LJB 10/14/04  write value to ini file
Function SetINIValue(section, key, value, ini_type)
	on error resume next
	if not isObject(INIVar) then
		Set INIVAR= Server.CreateObject("cowsUtils.cowsini") 
	end if
	select case UCase(ini_type)
		case "APP"
			ini_path = Application("ServerDrive") & "\"&  Application("ServerRoot") & "\"  & Application("DocumentRoot") & "\" & Application("COWSRoot") & "\" & Application("Appkey") & "\config\cfserver.ini"
		case "CHEMOFFICE"
			ini_path = Application("ServerDrive") & "\"&  Application("ServerRoot") & "\"  & Application("DocumentRoot") & "\" & Application("COWSRoot") & "\config\chemoffice.ini"
	end select
    success = INIVAR.VBWritePrivateProfileString(Cstr(value), UCase(section), UCase(key), CStr(ini_path))
	SetINIValue = success
End Function



'******************************************************************************
''LJB 10/14/04  encryption routine use CAPICOM dll distributed by Microsoft.  
' Subroutine: DoEncryptDecryptCommand
'
' Synopsis  : Encrypt Decrypt input string 
'
' Parameter : encryptType - DECRYPT|ENCRYPT
'
'             inputText - encrypted text for description or clear text for encryption
'
'             secretKey - secret key used for encryptiopn
'******************************************************************************

Function DoEncryptDecryptCommand (encryptType,inputText, secretKey)
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
   DoEncryptDecryptCommand=OutText
End Function 


' Store the ASPSessionID cookie identifier.
' Will be used by cs_security logoff page to terminate all sessions
Sub StoreASPSessionID()
	' The only way to get at the ASPSession cookie is by parsing the cookie header
	strCookieHeader = Request.ServerVariables("HTTP_COOKIE")
	start = InstrRev(strCookieHeader, "ASPSESSIONID", -1, 1) + 12
	if start > 12 then
		equalsign = InStr(start, strCookieHeader, "=")
		ASPcookieID= Mid(strCookieHeader, start , equalsign - start)
		CowsASPIDs = Request.Cookies("COWSASPIDS")
		' Store all COWS related ASPCookieIDs in a separate cookie
		if NOT inStr(1, CowsASPIDs, ASPcookieID) > 0 then
			if CowsASPIDs = "" then
				CowsASPIDs = ASPcookieID
			Else
				CowsASPIDs = CowsASPIDs & "," & ASPcookieID
			End if	
			Response.Cookies("COWSASPIDS") = CowsASPIDs
			Response.Cookies("COWSASPIDS").Path = "/"
		End if	 
	end if
End Sub
</script>