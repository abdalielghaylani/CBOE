<%@ Language=VBScript %>

<SCRIPT LANGUAGE=vbscript RUNAT=Server>
level = Request.QueryString("level")
if level = "" then level = "1"
Select Case level
	Case "0","1","4","8","12"
		Session("10046TraceLevel") = level
		Response.Write "Oracle event 10046 trace level set to level " & level & " for this session."
		Response.Write "<BR>Use level zero to stop tracing."
	Case else
		Response.Write "Invalid trace level value. Use 0,1,4,8,12."
		Session("10046TraceLevel") = ""
End Select
</SCRIPT>
