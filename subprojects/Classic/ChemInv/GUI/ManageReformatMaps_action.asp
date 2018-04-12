<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Conn
Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false
reformatMapName = Request("iReformatMapName")

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
mapAction = Request("mapAction")

FormData = Request.Form
FormData = FormData & Credentials
'Response.Write(FormData)
'Response.End

if mapAction = "delete" then
	returnValue = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteReformatMap.asp", "ChemInv", FormData)
else
	returnValue = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateReformatMap.asp", "ChemInv", FormData)
end if

'Response.Write returnValue
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Reformat Maps</title>
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
				Response.Write returnValue
				Response.End
			End if
			If isNumeric(returnValue) then
				If CLng(returnValue) > 0 then
					if mapAction = "create" then
						msg = "The reformat map, <u>" & reformatMapName & "</u>, has been successfully created."
					elseif mapAction = "delete" then
						msg = "The reformat map has been successfully deleted."
					end if
					Response.Write "<center><span class=""GuiFeedback"">" & msg & "</span>"
					Response.Write "<br><br><a href=""PlateSettings.asp?ReturnURL=menu.asp""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else
					Response.Write "<center><P><CODE>" & Application(returnValue) & "</CODE></P>"
					Response.Write "<span class=""GuiFeedback"">Reformat map operation failed.</span>"
					Response.Write "<p><a href=""3"" onclick=""history.back(); return false;""><img src=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write returnValue
				Response.Write "<center><p><a href=""3"" onclick=""history.back(); return false;""><img src=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</td>
	</tr>
</table>
</body>
