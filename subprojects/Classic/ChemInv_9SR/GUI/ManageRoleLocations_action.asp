<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false

'showformVars(true)

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

FormData = Request.Form
FormData = FormData & Credentials
'Response.Write(FormData)
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateRoleLocations.asp", "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Role Locations</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<br><br><br><br><br><br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff>
	<tr>
		<td>
			<%
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					msg = "Locations updated."
					Response.Write "<center><span class=""GuiFeedback"">" & msg & "</span>"
					Response.Write "<br><br><a href=""menu.asp""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<span class=""GuiFeedback"" title=""" & httpResponse & """>Manage User Locations operation failed</span>"
					Response.Write "<p><a href=""3"" onclick=""history.back(); return false;""><img src=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
				Response.Write "<center><p><a href=""3"" onclick=""history.back(); return false;""><img src=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</td>
	</tr>
</table>
</body>
