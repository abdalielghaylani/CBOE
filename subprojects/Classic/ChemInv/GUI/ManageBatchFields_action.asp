<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/BatchFunctions.asp" -->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false
Dim Conn

'showformVars(true)

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
clear = Request("clear")

FormData = Request.Form
FormData = FormData & Credentials
'Response.Write(FormData)
'Response.End

if clear = "1" then 
    httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/ClearBatchFields.asp", "ChemInv", FormData)
else 
    httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateBatchFields.asp", "ChemInv", FormData)
end if

UpdateFieldmaps() 'updating field dictionary 
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Grouping Fields</title>
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
					msg = "Grouping fields updated."
					Response.Write "<center><span class=""GuiFeedback"">" & msg & "</span>"
					Response.Write "<br><br><a href=""menu.asp""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				else
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<span class=""GuiFeedback"" title=""" & httpResponse & """>Manage Batching Fields failed.</span>"
					Response.Write "<p><a href=""3"" onclick=""history.back(); return false;""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
				Response.Write "<center><p><a href=""3"" onclick=""history.back(); return false;""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</td>
	</tr>
</table>
</body>
<%
Sub UpdateFieldmaps()
    Call GetInvConnection()
    Application("BatchFieldMap") = GetBatchFieldMap(Conn)
    Application("RequestFieldMap") = GetRequestFieldMap()
    Application("ReservationFieldMap") = GetReservationFieldMap()
    Application("NumBatchTypes")= GetNumBatchTypes(Conn)
End Sub 
%> 
