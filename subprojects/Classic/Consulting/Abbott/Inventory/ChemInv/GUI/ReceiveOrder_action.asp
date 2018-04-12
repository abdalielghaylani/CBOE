<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Credentials

'for each key in Request.Form
'	Response.Write key & "=" & request(key) & "<BR>"
'next

OrderID = Request("OrderID")
ReceivedContainerIDs = Request("selectChckBox")
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = "ReceivedContainerIDs=" & ReceivedContainerIDs
FormData = FormData & "&OrderID=" & OrderID
FormData = FormData & Credentials
'Response.Write FormData
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/ReceiveOrder.asp", "ChemInv", FormData)

'Response.Write httpResponse & ":test"
'Response.End
if instr(httpResponse,",") then
	arrResponse = split(httpResponse,",")
	numReceived = ubound(arrResponse) + 1
else 
	numReceived = 1
end if

action = Request("action")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Receive Order</title>
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
			If IsNumeric(numReceived) then 
					Response.Write "<center><SPAN class=""GuiFeedback"">" & numReceived & " containers have been received.</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""top.location.reload(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>" 
			Else
				Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
				Response.Write "<center><SPAN class=""GuiFeedback"">Containers could not be received.</SPAN></center>"
				Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				'Response.Write httpresponse
				'Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>