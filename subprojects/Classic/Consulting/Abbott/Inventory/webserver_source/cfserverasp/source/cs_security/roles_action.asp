<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

'Response.Write Request("PrivilegeList")
'Response.end

'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

dbKey = Request("dbKey")
action = Lcase(Request.QueryString("action"))
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & dbkey) & "&CSUSerID=" & Session("UserID" & dbkey)
FormData = Request.Form & Credentials
'Response.Write FormData 
'Response.end
Select Case action
	Case "delete"
		APIURL = Application("AppPathHTTP")& "/cs_security/DeleteRole.asp"
	Case "edit"
		APIURL = Application("AppPathHTTP")& "/cs_security/UpdateRole.asp"
	Case "create"
		APIURL = Application("AppPathHTTP")& "/cs_security/CreateRole.asp"
	Case "roles"
		APIURL = Application("AppPathHTTP")& "/cs_security/UpdateRolesGrantedToRole.asp"
	Case "users"
		APIURL = Application("AppPathHTTP")& "/cs_security/UpdateUsersGrantedaRole.asp"	
End Select

httpResponse = CShttpRequest2("POST", ServerName, APIURL, "CS_SECURITY", FormData)
'Response.Write httpResponse
'response.end
%>
<html>
<head>
<title>Manage Roles</title>
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
					Response.Write "<center><SPAN class=""GuiFeedback"">Role change has been processed</SPAN><center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>top.opener.location.href='manageRoles.asp?dbkey=" & Request("dbkey") & "&PrivTableName=" & Request("PrivTableName") & "';top.window.close()</SCRIPT>"
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Role could not be changed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write httpresponse
				Response.Write "<center><SPAN class=""GuiFeedback"">Role could not be changed</SPAN></center>"
				Response.Write "<P><center><a HREF=""Ok"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>