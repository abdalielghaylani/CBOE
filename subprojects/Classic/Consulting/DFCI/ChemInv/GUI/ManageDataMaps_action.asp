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
mapAction = Request("mapAction")
dataMapColumnList = ""
dataMapFieldList = ""
for i=1 to Request("NumDataCol")
	if i <> 1 then
		dataMapColumnList = dataMapColumnList & ","
		dataMapFieldList = dataMapFieldList & ","
	end if
	dataMapColumnList = dataMapColumnList & i
	dataMapFieldList = dataMapFieldList & Request("ColumnMap" & i)
next

FormData = Request.Form & "&dataMapColumnList=" & dataMapColumnList & "&dataMapFieldList=" & dataMapFieldList
FormData = FormData & Credentials
'Response.Write(FormData)
'Response.End

if mapAction = "delete" then
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteDataMap.asp", "ChemInv", FormData)
elseif mapAction = "update" then
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/EditDataMap.asp", "ChemInv", FormData)
elseif mapAction = "copy" then
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CopyDataMap.asp", "ChemInv", FormData)
else
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateDataMap.asp", "ChemInv", FormData)
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create an Import Template</title>
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
					if mapAction = "create" then
						msg = "The template, <u>" & Request("TemplateName") & "</u>, has been successfully created."
					elseif mapAction = "update" then
						msg = "The template has been successfully updated."
					elseif mapAction = "copy" then
						msg = "The template has been successfully copied."
					elseif mapAction = "delete" then
						msg = "The template has been successfully deleted."
					end if
					Response.Write "<center><span class=""GuiFeedback"">" & msg & "</span>"
					Response.Write "<br><br><a href=""PlateSettings.asp?ReturnURL=menu.asp""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<span class=""GuiFeedback"">Creation of an Import Template operation failed</span>"
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
