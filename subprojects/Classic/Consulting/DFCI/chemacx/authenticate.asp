<%
Dim bDebugPrint 
bDebugPrint = true

'**********************************************************************************************
' This function decides whether a Browser is subscribed to a  given service based on its cookie 
' credentials.  It first tries to authenticate based on a subscriptionID cookie. 
' If no subscriptionID is present, then it tries to use user name and userid cookie.
' There is an option to allow a certain number of searches before authentication is enforced.
' There is also an option which does not authenticate the user on any particular service, but only
' checks for the existence of a valid user account or subscriptionid.  
Function IsBrowserSubscribed(pServiceID, pSiteName, pAuthenticateService, pNoOfSearchAllowed, pIgnoreSearchCountCookie)
	Dim IsSubscribed
	Dim SearchCountCookie
	Dim okOnService 
	
	'Checking for approval
	IsSubscribed = False
	
	okOnService = "okOnService_" & pServiceID	 
	ServiceApproved = CheckServiceSession(okOnService)	'checks to see if the service session exists
	if bDebugPrint then Response.Write "<BR>Session " & okOnService & "= " & ServiceApproved  & "<BR>"
	'This If Clause checks whether a the approved session has been created.
	'If it has been created it makes IsSubscribed true (will bypass further service approval)
	'If it doesn't exist then it will make it false (will need to go the next set of approval)
	If ServiceApproved then
		IsSubscribed = True
	Else
		IsSubscribed = False
	End If

	'Bypass authentication is number of searches is below maximum allowed
	'Doesn't use the counter if the ignore is set to true.
	If Not pIgnoreSearchCountCookie then
		SearchCountCookie = "SearchCount" & pSiteName 	'Will be the name of the search counting cookie
		SearchCount = CheckSearchCount(SearchCountCookie)	'calls for the number of searches
		if bDebugPrint then Response.Write "SearchCount= " & SearchCount & "<BR>"
		'This checks the SearchCount only if IsSubscribed is false
		'If IsSubscribed is true then the session searching has already been set, so SearchCOunt is irrelevant
		'If it is false then it checks the search count to see if it is higher then the maximum 
		'without logging in or being a member
		If Not IsSubscribed and SearchCount < pNoOfSearchAllowed then
			if bDebugPrint then Response.Write "Below maxcount= True"  & "<BR>"
			IsSubscribed = True
		ElseIf Not IsSubscribed and SearchCount >= pNoOfSearchesAllowed then
			if bDebugPrint then Response.Write "Below maxcount= False"  & "<BR>"
			IsSubscribed = False
		End If
	End If

	' Try to authenticate from subscriber id cookie
	If Not IsSubscribed then
		SubscriberInfoExists = CheckSubscriberInfo() 'checks to see if the subscriber cookie exists
		if bDebugPrint then Response.Write "SID Cookie= " & SubscriberInfoExists & "<BR>"
		If SubscriberInfoExists then
			SubscriberAllowed = AuthenticateSubscriber(Request.Cookies("SubscriptionID"), pServiceID, pAuthenticateService)
			if bDebugPrint then Response.Write "SID OK= " & SubscriberAllowed & "<BR>"
			If SubscriberAllowed = 1 then
				IsSubscribed =  True
			Else
				IsSubscribed = False
			End If
		ElseIf Not SubscriberInfoExists then
			IsSubscribed = False
		End if
	End if
	' Try to authenticate from username cookie
	If Not IsSubscribed then
		UserInfoExists = CheckUserInfo()	'checks to see if the userinfo exists
		if bDebugPrint then Response.Write "UName Cookie= " & UserInfoExists & "<BR>"
		If UserInfoExists then
			UserAllowed = AuthenticateUser(Request.Cookies("UserName"), Request.Cookies("UserID"), pServiceID, pAuthenticateService)
			if bDebugPrint then Response.Write "UName OK= " & UserAllowed & "<BR>"
			If UserAllowed = 1 then
				IsSubscribed =  True
			Else
				IsSubscribed = False
			End If
				
		ElseIf Not UserInfoExists then
			IsSubscribed = False
		End If
	End if
	if bDebugPrint then Response.Write "IsSubscribed to service " & pServiceID & "= " & IsSubscribed & "<BR>"
	IsBrowserSubscribed = IsSubscribed
End Function


'**********************************************************************************************
'This function checks whether a counter cookie exists for this site and
'then creates it or adds 1 to the value
function CheckSearchCount(CookieName)
	
	SearchCookieContent = Request.Cookies(CookieName)
	
	If Len(SearchCookieContent) > 0 then 
		
		CheckSearchCount = SearchCookieContent + 1
		Response.Cookies(CookieName) = CheckSearchCount
		Response.Cookies(CookieName).path = "/"
		Response.Cookies(CookieName).domain = ".cambridgesoft.com"
		Response.Cookies(CookieName).expires = DateAdd("d", 365, Now())
		
	Else
		
		Response.Cookies(CookieName) = 1
		Response.Cookies(CookieName).path = "/"
		Response.Cookies(CookieName).domain = ".cambridgesoft.com"
		Response.Cookies(CookieName).expires = DateAdd("d", 365, Now())
		CheckSearchCount = 1
		
	End If
	
End function

'**********************************************************************************************
'This function checks to see whether a Session Variable exists.
function CheckServiceSession(SessionName) 
	
	If Len(Session(SessionName)) > 0 then
		CheckServiceSession = True
	Else
		CheckServiceSession = False
	End If
	
end function

'**********************************************************************************************
'This function Checks for the existence of the Username and UserID cookies which are used
'for authentication.
function CheckUserInfo()
	If Len(Request.Cookies("UserName")) > 0 and Len(Request.Cookies("UserID")) > 0 then
		CheckUserInfo = True
	Else
		CheckUserInfo = False
	End If	
End function


'**********************************************************************************************
'This function Checks for the existence of the subscriber id cookie which are used
'for authentication.
function CheckSubscriberInfo()
	If Len(Request.Cookies("SubscriptionID")) > 0 then
		CheckSubscriberInfo = True
	Else
		CheckSubscriberInfo = False
	End If	
End function


'**********************************************************************************************
'This function calls the appropriate Service checking based on the authenticate service variable
function AuthenticateUser(UserName, UserID, ServiceID, AuthenticateService)
	Dim okOnService 
	
	okOnService = "okOnService_" & ServiceID
	If AuthenticateService then
		AuthenticateUser = CheckServiceAccess(UserName, UserID, ServiceID)
	Else
		 AuthenticateUser = CheckUser(UserName, UserID)
	End If 
	if AuthenticateUser then Session(okOnService) = "OK"
End function


'**********************************************************************************************
'This function calls the appropriate Service checking based on the authenticate service variable
function AuthenticateSubscriber(SubscriptionID, ServiceID, AuthenticateService)
	Dim okOnService 
	
	okOnService = "okOnService_" & ServiceID
	If AuthenticateService then
		AuthenticateSubscriber = CheckSubscriberAccess(SubscriptionID, ServiceID)
	Else
		AuthenticateSubscriber = CheckSubscriber(SubscriptionID)
	End If 
	
	if AuthenticateSubscriber then Session(okOnService) = "OK"
End function


'**********************************************************************************************
'This function authenticates a user for a  particular service
function CheckServiceAccess(UserName, UserID, ServiceID)

	ConnStr = "Provider= SQLOLEDB; Data Source=172.17.1.18; Initial Catalog=csWebUsers; User Id=cows2000; Password=cambridgesoft; Network=dbmssocn"
	Set cmdCheckServiceAccess = Server.CreateObject("ADODB.Command")
	
	With cmdCheckServiceAccess
		.ActiveConnection = ConnStr
		.CommandText = "CS_CheckServiceAccess"
		
		AdCmdStoredProc = 4
		.CommandType = AdCmdStoredProc
		
		'ReplaceParameters
		adInteger = 3
		adVarChar = 200
		adParamInput = 1
		adParamOutput = 2
		adParamReturnValue = 4
		
		.Parameters.Append .CreateParameter ("RETURN_VALUE", adInteger, adParamReturnValue)
		.Parameters.Append .CreateParameter ("@UserName", adVarChar, adParamInput, 100)
		.Parameters.Append .CreateParameter ("@UserID", adInteger, adParamInput)	
		.Parameters.Append .CreateParameter ("@ServiceID", adInteger, adParamInput)
		.Parameters.Append .CreateParameter ("@Status", adInteger, adParamOutput)
		
		.Parameters("@UserName") = UserName
		.Parameters("@UserID") = UserID 
		.Parameters("@ServiceID") = ServiceID
		
		adExecuteNoRecords = 128
		.Execute lngRecs, adExecuteNoRecords
		
		CheckServiceAccess = .Parameters("@Status")
		If bDebugPrint then Response.Write "ServiceID= " & ServiceID & "<BR>"
	End With
end function


'**********************************************************************************************
'This function authenticates a user for a  particular service
function CheckSubscriberAccess(SubscriptionID, ServiceID)

	ConnStr = "Provider= SQLOLEDB; Data Source=172.17.1.18; Initial Catalog=csWebUsers; User Id=cows2000; Password=cambridgesoft; Network=dbmssocn"
	Set cmdCheckServiceAccess = Server.CreateObject("ADODB.Command")
	
	With cmdCheckServiceAccess
		.ActiveConnection = ConnStr
		.CommandText = "CS_CheckSubscriptionAccess"
		
		AdCmdStoredProc = 4
		.CommandType = AdCmdStoredProc
		
		'ReplaceParameters
		adInteger = 3
		adVarChar = 200
		adParamInput = 1
		adParamOutput = 2
		adParamReturnValue = 4
		
		.Parameters.Append .CreateParameter ("RETURN_VALUE", adInteger, adParamReturnValue)
		.Parameters.Append .CreateParameter ("@SubscriptionID", adInteger, adParamInput)	
		.Parameters.Append .CreateParameter ("@ServiceID", adInteger, adParamInput)
		.Parameters.Append .CreateParameter ("@Status", adInteger, adParamOutput)
		
		.Parameters("@SubscriptionID") = SubscriptionID
		.Parameters("@ServiceID") = ServiceID
		
		adExecuteNoRecords = 128
		.Execute lngRecs, adExecuteNoRecords
		
		CheckSubscriberAccess = .Parameters("@Status")
	End With
end function




'**********************************************************************************************
'This function checks to see if someone is a real user or not
function CheckUser(UserName, UserID)

	ConnStr = "Provider= SQLOLEDB; Data Source=172.17.1.18; Initial Catalog=csWebUsers; User Id=cows2000; Password=cambridgesoft; Network=dbmssocn"
	Set cmdCheckUser = Server.CreateObject("ADODB.Command")
	With cmdCheckUser
		.ActiveConnection = ConnStr
		.CommandText = "CS_CheckUser"
		
		AdCmdStoredProc = 4
		.CommandType = AdCmdStoredProc
		
		'ReplaceParameters
		adInteger = 3
		adVarChar = 200
		adParamInput = 1
		adParamOutput = 2
		adParamReturnValue = 4
		
		.Parameters.Append .CreateParameter ("RETURN_VALUE", adInteger, adParamReturnValue)
		.Parameters.Append .CreateParameter ("@UserName", adVarChar, adParamInput, 100)
		.Parameters.Append .CreateParameter ("@UserID", adInteger, adParamInput)	
		.Parameters.Append .CreateParameter ("@Status", adInteger, adParamOutput)
		
		
		.Parameters("@UserName") = UserName
		.Parameters("@UserID") = UserID 
		
		adExecuteNoRecords = 128
		.Execute lngRecs, adExecuteNoRecords
		
		CheckUser = .Parameters("@Status")
	End With
End function

'**********************************************************************************************
'This function checks to see if someone is a real user or not
function CheckSubscriber(SubscriptionID)

	ConnStr = "Provider= SQLOLEDB; Data Source=172.17.1.18; Initial Catalog=csWebUsers; User Id=cows2000; Password=cambridgesoft; Network=dbmssocn"
	Set cmdCheckUser = Server.CreateObject("ADODB.Command")
	With cmdCheckUser
		.ActiveConnection = ConnStr
		.CommandText = "CS_CheckSubscriptionIDExists"
		
		AdCmdStoredProc = 4
		.CommandType = AdCmdStoredProc
		
		'ReplaceParameters
		adInteger = 3
		adVarChar = 200
		adParamInput = 1
		adParamOutput = 2
		adParamReturnValue = 4
		
		.Parameters.Append .CreateParameter ("RETURN_VALUE", adInteger, adParamReturnValue)
		.Parameters.Append .CreateParameter ("@SubscriptionID", adInteger, adParamInput)	
		.Parameters.Append .CreateParameter ("@Status", adInteger, adParamOutput)
		
		.Parameters("@SubscriptionID") = SubscriptionID 
		
		adExecuteNoRecords = 128
		.Execute lngRecs, adExecuteNoRecords
		
		CheckSubscriber = .Parameters("@Status")
	End With
End function
%>

