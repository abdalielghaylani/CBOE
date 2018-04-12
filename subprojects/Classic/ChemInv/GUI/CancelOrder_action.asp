<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Cancel an Order</title>
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

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials
'Response.Write FormData
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CancelOrder.asp", "ChemInv", FormData)

'Response.Write httpResponse & "=return<BR>"
'Response.End
%>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If IsNumeric(httpResponse) then 
				if cint(httpResponse) = 1 then 
					Response.Write "<center><SPAN class=""GuiFeedback"">" & httpResponse & " order cancelled.<BR></SPAN></center>"
					Response.Write "<script language=""JavaScript"">opener.location.reload();window.close();</script>" 
				else
					bError = true
				end if
			else
				bError = true
			end if
			
			if bError then
				Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
				Response.Write "<center><SPAN class=""GuiFeedback"">Order could not be cancelled.</SPAN></center>"
				Response.Write "<P><center><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				'Response.Write httpresponse
				'Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>