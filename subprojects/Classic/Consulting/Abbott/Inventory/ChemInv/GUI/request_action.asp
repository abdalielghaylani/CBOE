<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

action = Lcase(Request.QueryString("action"))
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

'for request sample generate QtyList
if Request("RequestTypeID") = "2" and (action="edit" or action="create") then
	NumContainers = Request("NumContainers")
	QtyRequired = 0 
	for i = 1 to NumContainers
		'Response.Write "QtyRequired = QtyRequired + cdbl(Request(""sample" & i & """)<BR>"
		execute("QtyRequired = QtyRequired + cdbl(Request(""sample" & i & """))")
		execute("QtyList = QtyList & Request(""sample" & i & """) & "",""")						
	next
	QtyList = trim(left(QtyList,len(QtyList)-1))
	FormData = Request.Form & "&QtyRequired=" & QtyRequired & Credentials
	FormData = FormData & "&QtyList=" & QtyList
	'Response.Write FormData
	'Response.End
else
	FormData = Request.Form & Credentials
end if

Select Case action
	Case "delete"
		APIURL = "/cheminv/api/DeleteRequest.asp"
	Case "undodelivery"
		APIURL = "/cheminv/api/UndoDelivery.asp"	
	Case "edit"
		APIURL = "/cheminv/api/UpdateRequest.asp"
	Case "create"
		APIURL = "/cheminv/api/CreateRequest.asp"
	Case "cancel"
		APIURL = "/cheminv/api/CancelRequest.asp"
End Select
httpResponse = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", FormData)
'Response.Write httpResponse
'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Request an Inventory Container</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=Session("TreeViewOpenNodes" & TreeID)%>";
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<%
			If IsNumeric(httpresponse) then 
				If Clng(httpResponse) > 0 then
					Session("sTab") = "Requests"
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Request has been processed</SPAN><center>"
					Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & Request("cLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & Request("containerID") & "); opener.focus(); window.close();</SCRIPT>"
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Request could not be processed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write FormatApiError(APIURL, httpresponse)
				Response.end
			End if
			
			Function FormatApiError(APIURL, ErrMsg)
				FormatApiError = "<center><table width=""80%""><tr><th nowrap valign=top>API Error at:</th><td>" & APIURL & "</td></tr><tr><th nowrap valign=top>Oracle Error:</th><td>" & ErrMsg & "</td></tr></table></center>"
			End function
			%>
		</TD>
	</TR>
</TABLE>
</Body>