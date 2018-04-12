<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

url = "/cheminv/api/AssignUserToRequest.asp"
msg = "Your Request has been processed."
httpResponse = CShttpRequest2("POST", ServerName, url, "ChemInv", FormData)

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Assign User to Request</title>
<script language=javascript src="/cheminv/choosecss.js"></script>
<script language=javascript src="/cheminv/gui/refreshgui.js"></script>
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
					Response.Write "<center><span class=""GuiFeedback"">" & msg & "</span>"
					Response.Write "<br><br><a href=""Ok"" onclick=""if (opener){opener.location.reload();} window.close(); return false;""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				else			
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Request operation failed</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""opener.focus();window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
				Response.Write "<center><P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>