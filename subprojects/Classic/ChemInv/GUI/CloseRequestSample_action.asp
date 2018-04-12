<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Close Requests</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Credentials

if Request.Form("ClosedRequestIDList") = "" then
	Response.Write "<center><SPAN class=""GuiFeedback"">You must check at least one request to close.</SPAN></center>"
	Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
	Response.end
End if 
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

'Response.Write FormData
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CloseRequestSample.asp", "ChemInv", FormData)

'Response.Write httpResponse & ":test"
'Response.End
numClosed = httpResponse
%>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If IsNumeric(numApproved) and IsNumeric(numDeclined) then 
					Response.Write "<center><SPAN class=""GuiFeedback"">" & numClosed & " requests have been closed.</SPAN></center>"
					if action = "edit" then
						Response.Write "<script language=""JavaScript"">window.close();</script>" 
					else
						Response.Write "<P><center><a HREF=""3"" onclick=""top.ActionFrame.document.form1.submit(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>" 
					end if
			Else
				Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
				Response.Write "<center><SPAN class=""GuiFeedback"">Requests could not be closed.</SPAN></center>"
				Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				'Response.Write httpresponse
				'Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>