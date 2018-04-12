<%@ Language=VBScript %>
<SCRIPT LANGUAGE=vbscript RUNAT=Server>
Response.Expires = -1
level = Request.QueryString("level")
scope = lcase(Request.QueryString("scope"))
if scope <> "application" AND scope <> "session" then scope = "session"

if isNumeric(level) AND NOT IsEmpty(level) then
	Response.Write "CfwTraceLevel set to level " & level & " in " & scope & " scope."   	
	if level = 0 then level = "" 
	if scope = "session" then Session("CfwTraceLevel") = level
	if scope = "application" then
		Application.Lock
			Application("CfwTraceLevel") = level
		Application.UnLock
	End if
	
else
	if level = ""  or IsEmpty(level)then
		if Application("CfwTraceLevel") <> "" then
			Response.Write "CfWTraceLevel is currently set to " & Application("CfwTraceLevel") & " in application scope." 
		elseif Session("CfwTraceLevel") <> ""  then
			Response.Write "CfWTraceLevel is currently set to " & Session("CfwTraceLevel") & " in session scope." 
		else
			Response.Write "CfWTraceLevel is currently not set in either application or session scope."
		end if
	Else
		if scope = "session" then Session("CfwTraceLevel") = ""
		if scope = "application" then
			Application.Lock
				Application("CfwTraceLevel") = ""
			Application.UnLock
		End if
		Response.Write "Invalid trace level value. Use numeric value."
	End if
End if
</SCRIPT>

