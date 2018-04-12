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
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

Select Case Request("action")
	Case "create"	
		url = "/cheminv/api/CreateWellContentType.asp" 
		msg = "New Well Format has been created"
	Case "update"
		url = "/cheminv/api/UpdateWellContentType.asp"
		msg = "Well Format has been updated"
	Case "delete"
		url = "/cheminv/api/DeleteWellContentType.asp"
		msg = "Well Format has been deleted"
End Select
httpResponse = CShttpRequest2("POST", ServerName, url, "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Grid Format</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">" & msg & "</SPAN>"
					Response.Write "<BR><BR><a HREF=""/cheminv/gui/PlateSettings.asp""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else			
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Well Format operation failed</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
				Response.Write "<center><P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>