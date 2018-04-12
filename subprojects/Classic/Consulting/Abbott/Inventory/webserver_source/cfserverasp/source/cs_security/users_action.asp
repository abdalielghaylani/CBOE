<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
dbKey = Request("dbKey")
action = Lcase(Request.QueryString("action"))
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & dbKey) & "&CSUSerID=" & Session("UserID" & dbKey)
FormData = Request.Form & Credentials
'Response.Write FormData 
'Response.end
Select Case action
	Case "delete"
		APIURL = Application("AppPathHTTP")& "/cs_security/DeleteUser.asp"
	Case "edit"
		APIURL = Application("AppPathHTTP")& "/cs_security/UpdateUser.asp"
	Case "create"
		APIURL = Application("AppPathHTTP")& "/cs_security/CreateUser.asp"
	Case "changepwd"
		APIURL = Application("AppPathHTTP")& "/cs_security/ChangePWD.asp"	
End Select
httpResponse = CShttpRequest2("POST", ServerName, APIURL, "CS_SECURITY", FormData)
%>
<html>
<head>
<title>Manage Users</title>
<SCRIPT LANGUAGE=javascript src="/cfserverasp/source/cs_security/Choosecss.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<%
			If IsNumeric(httpresponse) then 
				If Clng(httpResponse) > 0 then
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					if action = "changepwd" then
							Response.Write "<center><SPAN class=""GuiFeedback"">Password has been changed.  <BR>As a security measure, you will need to login again.</SPAN><center>"
					else
						Response.Write "<center><SPAN class=""GuiFeedback"">User change has been processed</SPAN><center>"
					end if
					if action <> "changepwd" then
						Response.Write "<SCRIPT LANGUAGE=javascript>top.opener.location.href='manageUsers.asp?dbkey=" & Request("dbkey") & "&PrivTableName=" & Request("PrivTableName") & "';top.window.close()</SCRIPT>"
					else
						if Session("MustChangePassword") then
							Session("MustChangePassword") = false
							Response.Write "<P><center><a HREF=""Ok"" onclick=""location.href='../logoff.asp?ClearCookies=true'; return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"
						else
							Response.Write "<P><center><a HREF=""Ok"" onclick=""top.opener.location.href='../logoff.asp?ClearCookies=true';window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
						end if
					end if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">User could not be changed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write httpresponse
				Response.Write "<center><SPAN class=""GuiFeedback"">User could not be changed</SPAN></center>"
				Response.Write "<P><center><a HREF=""Ok"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>