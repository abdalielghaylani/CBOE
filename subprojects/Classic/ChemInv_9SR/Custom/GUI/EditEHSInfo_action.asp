<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugprint
bdebugprint = false


ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

target = "/cheminv/custom/api/EditEHSInfo.asp"


httpResponse = CShttpRequest2("POST", ServerName, target, "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Edit EH&S Information</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
			<% 
			if bdebugprint then
				Response.Write httpResponse
				Response.end
			End if
			
			If Len(httpResponse) = 0 then
				theAction = "Ok"
			Else
				theAction = "WriteOtherError"
			End if
			Select Case theAction
				Case "Ok"
					Response.write "<Script language=javascript>opener.location.reload();window.close();</script>"		
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/ChemInv/graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
			%>
		</TD>
	</TR>
</TABLE>
</Body>