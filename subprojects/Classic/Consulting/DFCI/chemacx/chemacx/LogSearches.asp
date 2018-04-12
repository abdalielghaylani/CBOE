<%
if UCASE(Application("bLogSearches")) = "TRUE"  then
	Select Case LCase(Request.QueryString("dataaction"))
		Case "search" , "search_no_gui"
			LogSearch(1) ' Log a plugin search
		Case "query_string"
			LogSearch(0) ' Log a nonplugin search
	END Select
End if

' The following custom code is used to log searches 
Sub LogSearch(isPlugin)
	
	' Access version
	theCookie = Request.Cookies("UserID")
	' Oracle version
	'theCookie = Session("UserNameChemACX")
	

	if theCookie = "" then
		theUSerID = "NULL" 
	Else
		theUserID = theCookie
	End if
	Set AdoConn = Server.CreateObject("ADODB.Connection")
	'Access version
	SQLQuery = "INSERT INTO SearchLog (SearchTimeStamp,IP,ASPSessionID,isplugin,host,referer,CSUserID) VALUES (GetDate(),'" & Request.ServerVariables("REMOTE_HOST") & "'," & Session.SessionID & "," & isplugin & ",'" & Request.ServerVariables("HTTP_HOST") &"','" & Left(Request.ServerVariables("HTTP_REFERER"),49) & "'," & theUserID &")"
	' Oracle Version
	'SQLQuery = "INSERT INTO SearchLog (SearchTimeStamp,IP,ASPSessionID,isplugin,host,referer,CSUserID) VALUES (sysdate,'" & Request.ServerVariables("REMOTE_HOST") & "'," & Session.SessionID & "," & isplugin & ",'" & Request.ServerVariables("HTTP_HOST") &"','" & Left(Request.ServerVariables("HTTP_REFERER"),49) & "','" & theUserID &"')"
	
	'on Error resume next
	AdoConn.Open Application("SearchLogDbConnStr")
	'on Error resume next
	set xxx = AdoConn.Execute(SQLQuery)
	set AdoConn= nothing
End Sub
%>