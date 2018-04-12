<!--#INCLUDE VIRTUAL = "/cfserveradmin/login/asplogin.asi"--><%
Response.Buffer=True
Response.Expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
on error resume next
If (VarType(Application("~WC~WebClassManager")) = 0) Then
	Application.Lock
	If (VarType(Application("~WC~WebClassManager")) = 0) Then
		Set Application("~WC~WebClassManager") = Server.CreateObject("WebClassRuntime.WebClassManager")
	End If
	Application.UnLock
End If

Application("~WC~WebClassManager").ProcessRetainInstanceWebClass "WebServerAdmin.WebEditor", _
		"~WC~WebServerAdmin.WebEditor", _
		Server, _
		Application, _
		Session, _
		Request, _
		Response
		
		if err.number > 0 then 
			Response.Write "<h1><small>" & "The following error occurred while starting the administrative tools:" & "</small></h1><br>"
			Response.write  "<strong>" & "Error Number: " & "</strong>" & err.number & "<br>"
			Response.write  "<strong>" & "Description: " & "</strong>" & err.description
		end if
%>
