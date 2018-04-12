<%
Response.Buffer=True
Response.Expires=0

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
%>
