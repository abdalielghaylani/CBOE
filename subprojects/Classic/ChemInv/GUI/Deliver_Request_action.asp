<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Deliver Requested Containers</title>
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

if Request.Form("DeliveredRequestIDList") = "" then
	Response.Write "<center><SPAN class=""GuiFeedback"">You must check at least one container to deliver.</SPAN></center>"
	Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
	Response.end
End if 
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeliverRequest.asp", "ChemInv", FormData)
%>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If IsNumeric(httpresponse) then 
				If CLng(httpResponse) > 0 then
					Session("bMultiSelect") = false
					multiSelect_dict.RemoveAll()
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Session("CurrentContainerID") = 0
					Response.Write "<center><SPAN class=""GuiFeedback"">" & CLng(httpResponse) & " Containers have been marked as delivered.</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""top.ActionFrame.document.form1.submit(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>" 
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Containers could not be marked as delivered.</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpresponse
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>