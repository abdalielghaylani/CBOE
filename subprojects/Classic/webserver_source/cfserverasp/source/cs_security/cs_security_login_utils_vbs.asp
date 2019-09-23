<SCRIPT LANGUAGE=vbscript RUNAT=Server>
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


'Dim UserNameFromTicket
'Dim authcookieName

UserNameFromTicket = ""
authcookieName = ""
authenticationTicket = ""




Sub AuthenticateUserFromRequest(dbKey)

	if UCase(Application("AUTHENTICATION_MODE")) = "COELDAP" then
		set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
		if len(Application("authCookieName")) < 1 then
			authcookieName = SSOobj.GetCookieName()
			Application("authCookieName") = authcookieName
		else 
			authcookieName = Application("authCookieName")		
		end if
		
		
		if Len(Request.Cookies(authcookieName)) > 0 and Request.Cookies(authcookieName)<>"INVALID_USER" and Session("UserValidated" & dbKey) = 0 then
			authenticationTicket = Request.Cookies(authcookieName)
			ticketexpirationdate = SSOobj.GetTicketExpirationDate(authenticationTicket)
			if ticketexpirationdate < Now() then
				goodCookieTicket = false
				
				Response.Cookies(authcookieName).Expires = Now()
				authenticationTicket = ""
			else
				goodCookieTicket = true
				
			end if
		else
			goodCookieTicket = false
		end if
		
		Set SSOobj = nothing
	end if
		
	on error resume next
	Session("UserValidated" & dbKey) = 0	
	' Check for credentials Posted via a Form
	If Len(Request.Form("CSUserName")) > 0 then
		Session("UserName" & dbKey) = Request.Form("CSUserName") 
		Session("UserID" & dbKey) = Request.Form("CSUserID")
	Elseif Len(Request.Form("USER_ID")) > 0 then
		Session("UserName" & dbKey) = Request.Form("USER_ID") 
		Session("UserID" & dbKey) = Request.Form("USER_PWD")
	Elseif Len(Request("Ticket"))>0 then
		authenticationTicket = Request("Ticket")
		set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
		
		ticketexpirationdate = SSOobj.GetTicketExpirationDate(authenticationTicket)
		if ticketexpirationdate < Now() then
			if lcase(Request("Source")) = "eln" Then
			    authenticationTicket = SSOobj.RenewTicket(authenticationTicket)
                ticketexpirationdate = SSOobj.GetTicketExpirationDate(authenticationTicket)
			else
				response.clear
				response.write "The ticket passed is expired."
                session.abandon 
			    response.end 
			end if		
		end if
		
		UserNameFromTicket = SSOobj.GetUserFromTicket(authenticationTicket)
		if UserNameFromTicket <> "" then
			Session("UserName" & dbKey) = UserNameFromTicket
			Session("UserID" & dbKey) = DecryptPwdFromTicket(SSOobj, authenticationTicket)
		else
			authenticationTicket = ""
			response.clear
			response.write "Invalid Ticket"
			session.abandon 
			response.end 
		End if     
	Elseif goodCookieTicket then
	
		set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
		
		'ticketexpirationdate = SSOobj.GetTicketExpirationDate(Request.Cookies(authcookieName))
		'if ticketexpirationdate < Now() then
		'	response.clear
		'	response.write "The ticket passed is expired."
		'	session.abandon 
		'	response.end 
		'end if
		
		'UserNameFromTicket = SSOobj.GetUserFromTicket(Request.Cookies(authcookieName))
		UserNameFromTicket = SSOobj.GetUserFromTicket(authenticationTicket)
		if UserNameFromTicket <> "" then
			Session("UserName" & dbKey) = UserNameFromTicket
			Session("UserID" & dbKey) = "temp"
			
			'JHS 6/9/2009 - trying to make new users be able to login in security mode by using cookie/ticket combo
	 		'The problem is that the sso version of the ticket seems to be getting modified by security during login
	 		if Request.Cookies("CS_SEC_UserName") <> "" AND ucase(Request.Cookies("CS_SEC_UserName")) = ucase(UserNameFromTicket) AND Request.Cookies("CS_SEC_UserID") <> "" then
				'9/1/2009 - cookie from .net will now cryptvbs the password
				'Session("UserID" & dbKey) = Request.Cookies("CS_SEC_UserID")
			    Session("UserID" & dbKey) = CryptVBS(Request.Cookies("CS_SEC_UserID").Item, Request.Cookies("CS_SEC_UserName").Item)
			end if
			
		else
			response.clear
			response.write "Invalid Ticket"
			session.abandon 
			response.end 
		End if  	
		
		
	' Check for credentials Posted via Cookie	
	Elseif Len(Request.Cookies("CS_SEC_UserName")) > 0 then
		Session("UserName" & dbKey) = Request.Cookies("CS_SEC_UserName") 
		Session("UserID" & dbKey) = CryptVBS(Request.Cookies("CS_SEC_UserID").Item, Request.Cookies("CS_SEC_UserName").Item)	
	
	End if
	
	
	' Credentials found.  Try to authenticate
	if Len(Session("UserName" & dbKey))>0 AND Len(Session("UserID" & dbKey))>0 then
		isValid = 0
		
		isValid = DoUserValidate(dbKey, Application("PrivTableList"))
		Session("UserValidated" & dbKey) = isValid
	End if
	' Authentication failed. 
	' Send user to login page but ask the login page to relay them
	' to the current request URL after properly authenticated	

	if Session("UserValidated" & dbKey)<> 1 then
		Session("PostRelay") = Request("PostRelay")
		Session("RelayURL") = Request.ServerVariables("URL")
		Session("RelayMethod") = Request.ServerVariables("REQUEST_METHOD")
		if Session("PostRelay") = 1  AND Session("RelayMethod") = "POST" then
			for i = 1 to Request.Form.Count
				PostRelay_dict.Add Request.Form.Key(i), Request.Form.Item(i)
			next
		End if
		if lcase(Request("Source")) = "eln" Then
			Session("UserPreferenceLoginErrorMessage" & "cheminv") = "You do not have the required privileges to view this page"		
		else
			' DGB  reverting to redirect to login.asp instead of login.aspx because login.asp 
			' cleans up classic session before redirecting to login.aspx
			if Session("UserName" & dbKey) = "" then 
			Response.redirect "/" & Application("appkey")& "/userprivs.asp?privsuser=1&usersession=0"
			else
			Response.redirect "/" & Application("appkey")& "/userprivs.asp?privsuser=1&usersession=1"
			end if
			'response.redirect "/coemanager/forms/public/contentarea/login.aspx"
		end if
	else 'user authenticated
		Session("UserPreferenceLoginErrorMessage" & "cheminv") = ""
		Session("UserBaseConnectionStr") = GetUserBaseConnectionStr(dbkey)
	end if
End Sub

Function DoUserValidate(dbkey, PrivTableList)
	Dim DataConn
	Dim oLDAPAuthenticator
	Dim LDAPStatus
	
	Session("Edit_ID_Restrictions" & dbkey)= ""
	Session("LoginErrorMessage" & dbkey)= ""
	
	' bypass LDAP authentication if it looks like the pwd passed in is already the 
	' autogenrated oracle password
		
	if (UCase(Application("AUTHENTICATION_MODE")) = "LDAP") AND (NOT IsAutoGenPWD(Session("UserID" & dbKey))) then
		
		Set oLDAPAuthenticator = Server.CreateObject("CSSecurityLDAP.LDAPAuthenticator")	
		on error resume next
		LDAPStatus = oLDAPAuthenticator.Authenticate(Application("LDAPConfigXmlPath"), Application("ACTIVE_DIRECTORY_DOMAIN"),Session("UserName" & dbKey), Session("UserID" & dbKey)) 
		If Err then
			isValidUser = 0
			LDAPErrorDescription = Err.Description
			LDAPErrorNumber = Err.number 
			Select Case LDAPErrorNumber
				case -19951
					Session("LoginErrorMessage" & dbkey) = "<span title=""LDAP Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>LDAP Connection Error: Invalid LDAP Username or Password</span>"			
				Case Else
					Session("LoginErrorMessage" & dbkey) = "<span title=""LDAP Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>LDAP Connection Error: " & Err.Description & "</span>"			
			End Select
		else	
			Session("LDAPUserID" & dbKey) = ""
			Select Case LDAPStatus
				Case 1 ' User is authenticated by LDAP
					isValidUser = 1
					' Need to override the LDAP password with Generated Oracle password and remember the LDAP pwd for storing in cookie.
					Session("LDAPUserID" & dbKey) = Session("UserID" & dbKey)
					Session("UserID" & dbKey) = GeneratePwd(Session("UserName" & dbKey))
				Case -1 ' User is exempt from authentication by LDAP
					isValidUser = 1
				Case Else
					isValidUser = 0
					Session("LoginErrorMessage" & dbkey) = "<span title=""LDAP Connection Error for user " & Session("UserName" & dbkey) & ": "  & "CSSecurityLDAP.Authenticate Returned unknown status" & """>LDAP Connection Error: " & "CSSecurityLDAP.Authenticate Returned unknown status" & "</span>"				
			End Select
		End if
	elseif (UCase(Application("AUTHENTICATION_MODE")) = "COELDAP") AND (NOT IsAutoGenPWD(Session("UserID" & dbKey)) OR lcase(Request("Source")) = "eln") then
		set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
		
		'if SSOobj.IsExemptUser(Session("UserName" & dbKey)) then
		isExemptUser = SSOobj.IsExemptUser(UCASE(Session("UserName" & dbKey)))
		If isExemptUser then	
			isValidUser = 1
		else
		
			if UserNameFromTicket = "" then
				
				ValidateUser = SSOobj.Authenticate(Session("UserName" & dbKey),Session("UserID" & dbKey))
				if ValidateUser then
					Session("LDAPUserID" & dbKey) = Session("UserID" & dbKey)
					isValidUser = 1
				else
					isValidUser = 0
					Session("LoginErrorMessage" & dbkey) = "<span title=""LDAP Connection Error for user " & Session("UserName" & dbkey) & ": "  & "CSSecurityLDAP.Authenticate Returned unknown status" & """>LDAP Connection Error: " & "CSSecurityLDAP.Authenticate Returned unknown status" & "</span>"				
				end if
				
			else
				isValidUser = 1
			end if
			
			'JHS 6/10/2009 the password from the cookie should only be changed to the generated password
			'if the security mode of SSO is COELDAP...otherwise you want to use the password as provided 
			'from the cookie  - set higher up
			ldap = false
			if Request.Cookies("SSOPROVIDER") <> "" AND Request.Cookies("SSOPROVIDER") = "COELDAP" then
			    ldap = true
			end if
			
			
			if isValidUser = 1 AND  ldap = true then
				Session("UserID" & dbKey) = GeneratePwd(Session("UserName" & dbKey))
			end if
			
			
			'This is the code that was in the COE 10 release
			'if isValidUser = 1 then
			'	Session("UserID" & dbKey) = GeneratePwd(Session("UserName" & dbKey))
			'end if
			
			
			
		end if
			
			if isValidUser = 1 then
				
				
				'Jordan this is a reminder to 
				'put authentication ticket writing here
				'Not checked in
				authcookieName = SSOobj.GetCookieName()
				if UserNameFromTicket = "" then
					if not isExemptUser then
						authenticationTicket = SSOobj.GetAuthenticationTicket(Session("UserName" & dbKey),Session("LDAPUserID" & dbKey))
					else
						authenticationTicket = SSOobj.GetAuthenticationTicket(Session("UserName" & dbKey),Session("UserID" & dbKey))
					end if
     			else
					if isExemptUser and  Ucase(Session("UserID" & dbKey)) = "TEMP" then
						Session("UserID" & dbKey) = Session("UserName" & dbKey)
					end if	
     				
				end if
				on error resume next
				curtick = SSOobj.GetTicketExpirationDate(authenticationTicket)
				Session("SSOExpiration") = curtick
				Session("SSOAuthticket") = authenticationTicket
				If err.number > 0 then
					TraceError "ssofailure: " & err.number
				end if
				Response.Cookies(authcookieName).Path = "/"                 
				Response.Cookies(authcookieName) = authenticationTicket 
		
			
			end if
		'end if
		
	End if
				' DGB  get arround issue where ldap users are not getting the sso cookie stamped above
				Response.Cookies(authcookieName).Path = "/"                 
				Response.Cookies(authcookieName) = authenticationTicket 
	
	' No need to validate with Oracle if an LDAP error has already occurred.
	if Session("LoginErrorMessage" & dbkey) = "" then	
		on error resume next
		Set DataConn = GetCS_SecurityConnection(dbKey)		
		Session("MustChangePassword") = false

		If Err then
			isValidUser = 0
			oraErrorDescription = Err.Description
			oraErrorNumber = Mid(oraErrorDescription, InStr(1, oraErrorDescription, "ORA-")+4,5) 
			Select Case oraErrorNumber
				Case "28001"
					Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>Your Oracle password has expired.<BR>Contact your system administrator.</span>"
				Case "28000"
					Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>Your Oracle account is locked.<BR>Contact your system administrator.</span>"
				Case "28002" 'Seems like OLEDB provider never raises this exception :(
					Session("MustChangePassword") = true
				Case "01017"
					Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>Invalid username/password.  Logon denied.</span>"	
				Case "12154", "12514","12541"
					Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & ". Check the cs_security.udl file"">Invalid Oracle service name.</span>"	
				Case Else
					if err.number = -2147287037 then
						Response.Clear
						Response.Write "<BR><BR>Error while connecting to CS_SECURITY.<BR>Check CS_SECURITY_UDL_PATH entry in " & dbkey & "\config\cfserver.ini"
						Response.End
					end if
					Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error for user " & Session("UserName" & dbkey) & ": "  & Err.Description & """>Connection Error: " & Err.Description & "</span>"			
			End Select
		Else
			' The OLEDB  provider does not seem to reliably raise the Password Expired Exception even with PwdChgDlg=0
			' connection string attribute. So we check expiration. 
			Set RS = DataConn.execute("select 1 from user_users where account_status LIKE 'EXPIRED%'")
			if NOT (RS.EOF AND RS.BOF) then
				Session("MustChangePassword") = true
			End if
			isValidUser = 1
		End if 
	End if
	
	if isValidUser > 0 then
		PrivTableArray = Split(PrivTableList,",")
		
		'Loop over the privilege tables
		all_roles_id_str = ""
		For i = 0 to Ubound(PrivTableArray)
			roles_id_str =  GetGrantedRoles(dbkey, DataConn, PrivTableArray(i))
			if roles_id_str <> "" then 
				isOk = SetPrivilegeFlags(dbkey, DataConn, roles_id_str, PrivTableArray(i))
				if isOk = "" then
					isOK = CheckPeopleTable(dbkey, DataConn, Session("UserName" & dbKey))
					fullValidate = 1
				else
					Session("LoginErrorMessage" & dbkey) = isOK
				end if
				if isOk = "" then
					isOK = SetRestrictEdit(dbkey, DataConn, Session("UserName" & dbKey))
					if isOk = "" then
						fullyValidated = 1
					else
						Session("LoginErrorMessage" & dbkey) = isOk
						fullyValidated = 0
					end if
				else
					Session("LoginErrorMessage" & dbkey) = isOk
				end if
			end if
			all_roles_id_str = all_roles_id_str & roles_id_str
		Next
		If all_roles_id_str = "" then
			fullyValidated = 0
			Session("LoginErrorMessage" & dbkey) = "<span title=""Connection Error: " & Err.Description & """>No roles assigned to user " & UCase(Session("UserName" & dbkey))  & " in " & dbkey & "</span>"
		end if
	else
		fullyValidated = 0
	end if
	if fullyValidated then AuditLoginAction dbkey, "LOGIN"
	DoUserValidate=fullyValidated
	
End Function

Function GetGrantedRoles(dbkey, DataConn, PrivTableName)
	Dim Cmd
	Dim RS
	Dim role_list
	role_list = ""
	Set Cmd = GetCommand(DataConn, "CS_SECURITY.LOGIN.GETUSERROLEIDS", 4)
	Cmd.Parameters.Append Cmd.CreateParameter("pUserName", 200, 1, 30, UCase(Session("UserName" & dbkey))) 
	Cmd.Parameters.Append Cmd.CreateParameter("pPrivTableList", 200, 1, 30, Ucase(PrivTableName)) 
	Cmd.Properties ("PLSQLRSet") = TRUE  
	
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		role_list = RS.GetString(2,,,",","")
		role_list = left(role_list, len(role_list)-1)
	end if
	GetGrantedRoles = role_list
End function

Sub SetDefaultFlags(dbkey, ByRef DataConn, PrivTableName)
	Dim Cmd
	Set Cmd = GetCommand(DataConn, "CS_SECURITY.LOGIN.GETPRIVS", 4)
	Cmd.Parameters.Append Cmd.CreateParameter("pPrivTableName", 200, 1, 30, PrivTableName) 
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	for each field in rs.fields
		theVarName = field.name
		Session(theVarName & dbkey)= False
	next
	RS.close
	Set RS = Nothing
	Set Cmd = Nothing
End Sub

Function SetPrivilegeFlags(dbkey,ByRef DataConn, roles_granted_ids, PrivTableName)
	Dim Cmd
	
	ErrorsOccurred = ""
	SetDefaultFlags dbkey, DataConn, PrivTableName
	Set Cmd = GetCommand(DataConn, "CS_SECURITY.LOGIN.GETPRIVSBYROLEID", 4)
	Cmd.Parameters.Append Cmd.CreateParameter("pRoleIDList", 200, 1, 2000, roles_granted_ids) 
	Cmd.Parameters.Append Cmd.CreateParameter("pPrivTableName", 200, 1, 30, PrivTableName) 
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	if Not(RS.BOF AND RS.EOF) THEN
	Do while NOT RS.EOF
		for each field in rs.fields
			theVarName = field.name
			theValue = field.value
			if theValue = "" or isNull(theValue) then 
				theValue = 0
			end if
			If field.name <> "RID" and field.name <> "CREATOR" and field.name <> "TIMESTAMP" then
				If CBool(theValue)= True then 
					Session(theVarName & dbkey)= True
				end if
			end if
		next
		RS.MoveNext
	Loop
		RS.close
		
		MinRequiredPriv = Application("MinRequiredPriv")
		If InStr(MinRequiredPriv,",") then
			MinRequiredPriv = Left(MinRequiredPriv, InStr(MinRequiredPriv, ",")-1)
		End if
		if MinRequiredPriv <> "" then
			if NOT Session(MinRequiredPriv & dbkey) then
				ErrorsOccurred = "<span title=""Minimun required privilege is " & MinRequiredPriv  &""">User " & Session("UserName" & dbkey) & " has insufficient privileges to access the application</span>"
			End if
		End if
	else
		ErrorsOccurred = "No Privileges found for assigned roles:"
	end if	
	SetPrivilegeFlags=ErrorsOccurred	
End Function

Function CheckPeopleTable(dbkey, DataConn, UserName)
	Dim Cmd
	Dim RSp
	Dim ErrorsOccurred
	
	ErrorsOccurred= ""
	Set Cmd = GetCommand(DataConn, "CS_SECURITY.LOGIN.GETPERSONID", 4)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",5, 4, 0, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("UserName", 200, 1, 30, UCase(UserName)) 
	Cmd.Execute
	PersonID = Cmd.Parameters("RETURN_VALUE")
	If  PersonID = 0 then
		ErrorsOccurred = "User <b>" & UCase(UserName) & "</b> is inactive or cannot be found."
	end if
	CheckPeopleTable = ErrorsOccurred
End Function

Function SetRestrictEdit(dbkey, DataConn, UserName)
	Dim cmd
	Dim RSp
	
	ErrorsOccurred= ""
	theIDs = -1 ' assume no edit
	if Session("Edit_Scope_All" & dbkey) = True then
		theIDS = 0
		ErrorsOccurred = ""
		Session("EditRestrictIDs" & dbkey)= theIDs
		SetRestrictEdit = ErrorsOccurred
		exit function
	end if

	if Session("Edit_Scope_Supervisor" & dbkey) = True then
		Set cmd = Server.CreateObject("ADODB.Command")
		cmd.CommandText= "SELECT s.person_id from people p, people s  where p.person_id = s.supervisor_internal_id and Upper(p.user_id) = ? UNION SELECT person_id FROM people where Upper(user_id) = ?"
		cmd.ActiveConnection = DataConn 
		Cmd.Parameters.Append Cmd.CreateParameter("UserName", 200, 1, 2000, UCase(UserName)) 
		Cmd.Parameters.Append Cmd.CreateParameter("UserName2", 200, 1, 2000, UCase(UserName))
		Set RSp = cmd.Execute
		If Not (RSp.BOF AND RSp.EOF) then
			theIDs = RSp.GetString(2,,,",","")
			theIDs = left(theIDs, len(theIDs)-1)
		else
			ErrorsOccurred = "User " & UserName & " not found in cs_security.people table"	
		end if
		Session("EditRestrictIDs" & dbkey)= theIDs
		SetRestrictEdit = ErrorsOccurred
		exit function
	end if
	
	if Session("Edit_Scope_Self" & dbkey) = True then
		Set cmd = Server.CreateObject("ADODB.Command")
		cmd.CommandText= "Select person_id from people where Upper(user_id)= ? "
		cmd.ActiveConnection = DataConn 
		Cmd.Parameters.Append Cmd.CreateParameter("UserName", 200, 1, 2000, UCase(UserName)) 
		Set RSp = cmd.Execute
		If NOT (RSp.BOF AND RSp.EOF) then
			theIDs = RSp("person_id") 
		else
			ErrorsOccurred = "User " & UserName & " not found in cs_security.people table"
		end if
		Session("EditRestrictIDs" & dbkey)= theIDs
		SetRestrictEdit = ErrorsOccurred
		exit function
	end if
	Session("EditRestrictIDs" & dbkey)= theIDs
	SetRestrictEdit=ErrorsOccurred
end function
 
Function CryptVBS(Text, Key)
	if(Request.Cookies("CS_SEC_Azure").Item = "") then
		Text = shiftChr(Text, -1)
		KeyLen = Len(Key)
		For i = 1 To Len(Text)
			KeyPtr = (KeyPtr + 1) Mod KeyLen
			sTxtChr = Mid(Text, i, 1)
			wTxtChr = Asc(stxtchr)
			wKeyChr = Asc(Mid(Key, KeyPtr + 1, 1))
			CryptKey = Chr(wTxtChr Xor wKeyChr)
			hold = hold & CryptKey
		Next
		CryptVBS = shiftChr(hold, 1)
	else
		CryptVBS = Key
	end if
End Function
Function shiftChr(str, s)
    
	for i = 1 to len(str)
		hold = hold & Chr(Asc(Mid(str,i,1))+ s)
	Next
	shiftChr = hold
End function

Sub GetSecurityINIValues()

'!LJB! 9/1/2002. Chages these variables to optionsal and added default values. Otherwise applications not have the cfserver.ini settings fail to load
	if Application("UseCSSecurityApp") = "" then
		Application("UseCSSecurityApp")=GetINIValue( "optional", "CS_SECURITY", "USE_CS_SECURITY_APP", "web_app", "cfserver")
		if (Application("UseCSSecurityApp")="INIEmpty" or Application("UseCSSecurityApp") = "NULL" or Application("UseCSSecurityApp") = "") then
			Application("UseCSSecurityApp")="0"
		end if
	end if
	
	if Application("CS_SECURITY_UDL_PATH") = "" then
		Application("CS_SECURITY_UDL_PATH")=GetINIValue( "optional", "CS_SECURITY", "CS_SECURITY_UDL_PATH", "web_app", "cfserver")
		if (Application("CS_SECURITY_UDL_PATH")="INIEmpty" or Application("CS_SECURITY_UDL_PATH") = "NULL" or Application("CS_SECURITY_UDL_PATH") = "") then
			Application("CS_SECURITY_UDL_PATH")="0"
		end if
	end if
	
	if Application("PrivTableList") = "" then
		Application("PrivTableList")=GetINIValue( "optional", "CS_SECURITY", "PRIVILEGE_TABLE_LIST", "web_app", "cfserver")

		if (Application("PrivTableList")="INIEmpty" or Application("PrivTableList") = "NULL" or Application("PrivTableList") = "") then
			Application("PrivTableList")="0"
			Application("PrivTableName")=""
		Else
			myarr = split(Application("PrivTableList"),",")
			Application("PrivTableName")= myarr(0)
		end if
	end if
	
	if Application("MinRequiredPriv") = "" then
		Application("MinRequiredPriv")=GetINIValue( "optional", "CS_SECURITY", "MINIMUM_REQUIRED_PRIVILEGE", "web_app", "cfserver")
		if (Application("MinRequiredPriv")="INIEmpty" or Application("PrivTableList") = "NULL" or Application("MinRequiredPriv") = "") then
			Application("MinRequiredPriv")="0"
		end if
	end if
	
	if Application("StartupLocation") = "" then
		Application("StartupLocation")=GetINIValue( "optional", "CS_SECURITY", "STARTUP_LOCATION", "web_app", "cfserver")
		if (Application("StartupLocation")="INIEmpty" or Application("StartupLocation") = "NULL" or Application("StartupLocation") = "") then
			Application("StartupLocation")="0"
		end if
	end if

	if Application("AllowCookieLogin") = "" then
		Application("AllowCookieLogin")=CBool(GetINIValue( "optional", "CS_SECURITY", "ALLOW_COOKIE_LOGIN", "web_app", "cfserver"))
		if (Application("AllowCookieLogin")="INIEmpty" or Application("AllowCookieLogin") = "NULL" or Application("AllowCookieLogin") = "") then
			Application("AllowCookieLogin")=false
		end if
	end if
	
	' DGB
	' This is now set for all apps in SetAdditionalAppVariables but we still need it here for the cs_Security app 
	 if Application("CookieExpiresMinutes") = "" then
		Application("CookieExpiresMinutes")=GetINIValue( "optional", "CS_SECURITY", "COOKIE_EXPIRES_MINUTES", "web_app", "cfserver")
		if (Application("CookieExpiresMinutes")="INIEmpty" or Application("CookieExpiresMinutes") = "NULL" or Application("CookieExpiresMinutes") = "") then
			Application("CookieExpiresMinutes")="20"
		end if
	 end if
	
	'read password validation configuration values
	if Application("pwdMinLength") = "" then
		Application("pwdMinLength")=GetINIValue( "optional", "CS_SECURITY", "PWD_MIN_LENGTH", "web_app", "cfserver")
		if (Application("pwdMinLength")="INIEmpty" or Application("pwdMinLength") = "NULL" or Application("pwdMinLength") = "") then
			Application("pwdMinLength")="5"
		end if
	end if
	
	if Application("pwdMaxLength") = "" then
		Application("pwdMaxLength")=GetINIValue( "optional", "CS_SECURITY", "PWD_MAX_LENGTH", "web_app", "cfserver")
		if (Application("pwdMaxLength")="INIEmpty" or Application("pwdMaxLength") = "NULL" or Application("pwdMaxLength") = "") then
			Application("pwdMaxLength")="12"
		end if
	end if
	
	
	if Application("pwdIllegalChars") = "" then
		Application("pwdIllegalChars")=GetINIValue( "optional", "CS_SECURITY", "PWD_ILLEGAL_CHARS", "web_app", "cfserver")
		if (Application("pwdIllegalChars")="INIEmpty" or Application("pwdIllegalChars") = "NULL" or Application("pwdIllegalChars") = "") then
			Application("pwdIllegalChars")="[\W]"
		end if
	end if
	
	if Application("pwdIllegalFirstChar") = "" then
		Application("pwdIllegalFirstChar")=GetINIValue( "optional", "CS_SECURITY", "PWD_ILLEGAL_FIRSTCHAR", "web_app", "cfserver")
		if (Application("pwdIllegalFirstChar")="INIEmpty" or Application("pwdIllegalFirstChar") = "NULL" or Application("pwdIllegalFirstChar") = "") then
			Application("pwdIllegalFirstChar")="NULL"
		end if
	end if
	
	if Application("pwdMustHaveChars") = "" then
		Application("pwdMustHaveChars")=GetINIValue( "optional", "CS_SECURITY", "PWD_MUSTHAVE_CHARS", "web_app", "cfserver")
		if (Application("pwdMustHaveChars")="INIEmpty" or Application("pwdMustHaveChars") = "NULL" or Application("pwdMustHaveChars") = "") then
			Application("pwdMustHaveChars")="NULL"
		end if
	end if
	
	if Application("pwdIllegalWordList") = "" then
		Application("pwdIllegalWordList")=GetINIValue( "optional", "CS_SECURITY", "PWD_ILLEGALWORD_LIST", "web_app", "cfserver")
		if (Application("pwdIllegalWordList")="INIEmpty" or Application("pwdIllegalWordList") = "NULL" or Application("pwdIllegalWordList") = "") then
			Application("pwdIllegalWordList")="password"
		end if
	end if
	
	if Application("pwdCannotMatchUserName") = "" then
		Application("pwdCannotMatchUserName")=GetINIValue( "optional", "CS_SECURITY", "PWD_CANNOT_MATCH_USERNAME", "web_app", "cfserver")
		if (Application("pwdCannotMatchUserName")="INIEmpty" or Application("pwdCannotMatchUserName") = "NULL" or Application("pwdCannotMatchUserName") = "") then
			Application("pwdCannotMatchUserName")="false"
		end if
	end if
	
End sub
Sub KillCOWSSessions()

	AuditLoginAction "cs_Security", "LOGOUT" 
	' Use JavaScript to expire all the ASPSESSIONID cookies 
	' listed in the COWSASPIDS cookie
	Response.Write "<SCRIP" & "T language=JavaScript>" & vblf
	CowsASPIDs = Request.Cookies("COWSASPIDS")
	if CowsASPIDS <> "" then
		Response.Write "var lastyear = new Date();" & vblf
		Response.write "lastyear.setFullYear(lastyear.getFullYear() -1);" & vblf
		tempArr = split(CowsASPIDS, ",")
		For i=0 to Ubound(tempArr)
			Response.Write "document.cookie='ASPSESSIONID" & tempArr(i) & "=;path=/;expires=' +  lastyear.toGMTString() + ';';" & vblf
		Next 
		Response.Write "document.cookie='COWSASPIDS=;path=/;expires=' +  lastyear.toGMTString() + ';';" & vblf 		
	end if
	Response.Write "document.location.href='/cs_security/login.asp?perform_validate=0';" & vblf
	Response.Write "</scrip" & "t>" & vblf
	Response.end
End sub

'-------------------------------------------------------------------------------
' Name: GetUserBaseConnectionStr(dbkey, formgroup, conn_name)
' Type:  function
' Purpose: Get connection string for base connection using session credentials
' Inputs: dbkey as string
' Returns: connection string as string
'-------------------------------------------------------------------------------
Function GetUserBaseConnectionStr(ByVal dbkey)
	if dbkey = "cs_security" then exit function
	bLoginRequired = false
	bLoginInfoNeeded = false
	conn_name = "base_connection"
	conn_info_array = Application(conn_name & dbkey)
	if not isArray(conn_info_array) then
		Response.Write "Error getting user connection string"
		Response.End
	end if
	conn_type=conn_info_array(kConn)
	conn_string=conn_info_array(kConnStr)
	conn_conn_timeout=conn_info_array(kConnTimeOut)
	conn_command_timeout=conn_info_array(kConnConnTimeOut)
	conn_username=conn_info_array(kConnUserName)
	conn_password=conn_info_array(kConnPassword)
	on error resume next
	conn_dbms = conn_info_array(kDBMS)
		if err.number > 0 then conn_dbms = "ACCESS"
	on error goto 0
	if not conn_dbms <> "" then conn_dbms = "ACCESS"
	if conn_username = "login_required"	then
		bLoginRequired = true 
		UserIDKeyword = "UID"
		If Application("UserIDKeyword") <> "" then UserIDKeyword = Application("UserIDKeyword")
		conn_username = UserIDKeyword & "=" & Session("UserName" & dbkey)
	end if
	conn_password=conn_info_array(kConnPassword)
	if conn_password = "login_required"then
		bLoginRequired = true
		PWDKeyword = "PWD"
		If Application("PWDKeyword") <> "" then PWDKeyword =Application("PWDKeyword")
		conn_password = PWDKeyword & "=" & Session("UserID" & dbkey)				
	end if
			
	'create full connection string
	if conn_type = "NULL" or conn_type = "" or conn_type= "OLEDB" then
		full_conn_string = conn_string & "; " & conn_username & "; " & conn_password		
	else
		full_conn_string = conn_type & "=" & conn_string & "; " & conn_username & "; " & conn_password		
	end if	
	GetUserBaseConnectionStr = full_conn_string
end function

' Used to renew the credential cookies that control the cs_security session
' Uses cookie expiration time when persistent cookies are allowed.
' Uses a timestamp cookie that tracks UTC time when in memory cookies are used
Sub ProlongCookie(cookieName, value, minutes)
	Response.Cookies(cookieName) = value
	Response.Cookies(cookieName).Path= "/"
	if Application("PERSIST_AUTHENTICATION_COOKIES") then
		Response.Cookies(cookieName).expires = DateAdd("n", minutes, now())
	end if
	ProlongSSOTicketCookie()
	' This cookie tracks the expiration in UTC time
	Response.Cookies("cstimestamp") = getUtcNow(now()) + (cLng(minutes) * 60 * 1000)
	Response.Cookies("cstimestamp").Path = "/"
End sub

'jhs 10/2/2007 - add prolonging of sso cookie for .net ticket
'kfd 6/11/2009 - add session test tol bypass for sessionless calls, eg postback
Sub ProlongSSOTicketCookie()
	if isObject(session) then 
	  'DGB  prevent SOAP error when authentication ticket is not there
	  if Session("SSOAuthticket") <> "" then
        if Application("authCookieName") <> "" and UCase(Application("AUTHENTICATION_MODE")) = "COELDAP" then

                authcookieName = Application("authCookieName")
                if DateDiff("n", Now(), Session("SSOExpiration")) < 5 then 
                    set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
                    
                    Session("SSOAuthticket") = SSOobj.RenewTicket(Session("SSOAuthticket"))
                    Session("SSOExpiration") = SSOobj.GetTicketExpirationDate(Session("SSOAuthticket"))
                    
                    Response.Cookies(authcookieName).Path = "/"                 
                    Response.Cookies(authcookieName) = Session("SSOAuthticket") 
                    set SSOobj = nothing
                end if
        end if
	   end if	
    end if
end sub

' Audit Login
' ------------
Sub AuditLoginAction(dbkey, action)
	if Application("AUDIT_USER_LOGINS")<> "1" then exit sub '=====> EXIT POINT
	Dim DataConn
	Set DataConn = GetCS_SecurityConnection(dbKey)		

	Dim CmdRcrd, CmdUpdate
	Set CmdRcrd = GetCommand(DataConn, "CS_SECURITY.AUDIT_TRAIL.RECORD_TRANSACTION", 4)
	CmdRcrd.Parameters.Append CmdRcrd.CreateParameter("raid", 131, 1, 30, 9999) 
	CmdRcrd.Parameters.Append CmdRcrd.CreateParameter("tabname", 200, 1, 30, Ucase(action)) 
	CmdRcrd.Parameters.Append CmdRcrd.CreateParameter("erid", 131, 1, 30, 9999) 
	CmdRcrd.Parameters.Append CmdRcrd.CreateParameter("act", 200, 1, 30, "I") 
	on error resume next
	CmdRcrd.Execute
	if err then
		Response.Write "Error in AuditLoginAction.<BR>"
		Response.Write err.Source & "<BR>"
		Response.Write err.Description & "<BR>"
		Response.end
	end if
End Sub

Function GeneratePwd(s)
	GeneratePwd = "7" & UCase(strReverse(s)) & "11C"
End function

Function IsAutoGenPWD(s)
	if left(s,1) = "7" and Right(s,3)= "11C" then
		IsAutoGenPWD = true
	else
		IsAutoGenPWD = false
	end if
end function
' Store the ASPSessionID cookie identifier.
' Will be used by cs_security logoff page to terminate all sessions
Sub StoreASPSessionID()
  if Session("ASPSessionIDStored")="" then	
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
	Session("ASPSessionIDStored") = "1"
  end if	
End Sub

Function DecryptPwdFromTicket(ByRef SSOobj, authenticationTicket)
        Dim pwd, rc
        Dim startPos
        on error resume next
    	rc = SSOobj.GetUserDataFromTicket(authenticationTicket)
	    startPos = instr(1,rc, "=")
	    if startPos > 0 then
	        pwd = right(rc, len(rc)- startPos)
	    end if
	    if err <> 0 then
	        response.Write "An unexpected error occurred in DecryptPwdFromTicket<BR>"
	        response.Write "Source: " & err.Source & "<br>Description: " & err.Description
	        response.End
	    else
	        DecryptPwdFromTicket = pwd
	    end if
End Function

'CSBR ID     : 128893
'Modified by : sjacob
'Comments    : Adding code to authenticate LDAP users in InvLoader.
'Start of change
Function IsValidLDAPUser(UserId, Password)
	Dim oXML
	Dim COEFrameworkConfigPath
	Dim sAuthentication
	
	Set oXML = server.CreateObject("MSXML2.DOMDOCUMENT.4.0")
	COEFrameworkConfigPath = GetConfigFilePath()
	If COEFrameworkConfigPath = "File Not Found" Then
	    isValidUser = 0
	    Exit Function
	End If     
	oXML.load(COEFrameworkConfigPath)
	sAuthentication = oXML.SelectSingleNode("//SSOConfiguration/ProviderConfiguration/Settings/add[@Name='DEFAULT_PROVIDER']").SelectSingleNode("@Value").Text
	If (UCase(sAuthentication) = "COELDAP") Then	
	    set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
		IsExemptUser = SSOobj.IsExemptUser(UCASE(UserId))
		If isExemptUser then	
			IsValidUser = 0
		else
			ValidateUser = SSOobj.Authenticate(UserId,Password)
			If ValidateUser then
				IsValidUser = 1
			Else
				IsValidUser = 0
			End if
		End if
	Else
		IsValidUser = 0
	End If
	IsValidLDAPUser = IsValidUser
End Function

'This function is to return the path of COEFrameworkconfig.xml file from any drive 
Function GetConfigFilePath()

    Const ALL_USERS_APPLICATION_DATA = &H23&

    Set objShell = CreateObject("Shell.Application")
    Set objFolder = objShell.Namespace(ALL_USERS_APPLICATION_DATA)
    Set objFolderItem = objFolder.Self
    
    sPath = objFolderItem.Path
    set fso = Server.CreateObject("Scripting.FileSystemObject")

    If fso.FolderExists(sPath) = True then
	    sPath = sPath & "\PerkinElmer\"
	    If fso.FolderExists(sPath) = True Then
	        sPath = sPath & "ChemOfficeEnterprise\"
	        If fso.FolderExists(sPath) = True Then    
		        sPath = sPath & "COEFrameworkConfig.xml"
		        If fso.FileExists(sPath) = True Then
		            GetConfigFilePath = sPath
		        Else
		            GetConfigFilePath = "File Not Found"    
		        End If    
		    End If	
	    End If	
    End If
    
	set fso = nothing
	set objShell = nothing
	set objFolder = nothing
	set objFolderItem = nothing
	    
End Function

</SCRIPT>
<SCRIPT LANGUAGE=jscript RUNAT=Server>
function getUtcNow(strDate){
	return Date.parse(strDate)
}
</SCRIPT>
