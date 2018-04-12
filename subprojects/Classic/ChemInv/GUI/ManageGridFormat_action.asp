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
GridType = Request("GridType")
if GridType = "rack" then
	GridText = "Rack Format"
else
	GridText = "Grid Format"
end if
Select Case Request("action")
	Case "create"	
		url = "/cheminv/api/CreateGridFormat.asp" 
		msg = "New " & GridText & " has been created"
	Case "update"
		url = "/cheminv/api/UpdateGridFormat.asp"
		msg = GridText & " has been updated"
	Case "delete"
		url = "/cheminv/api/DeleteGridFormat.asp"
		msg = GridText & " has been deleted"
End Select
httpResponse = CShttpRequest2("POST", ServerName, url, "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage <%=GridText%></title>
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
					Response.Write "<center><SPAN class=""GuiFeedback"">" & msg & "</SPAN>"
					if Request("action") = "update" or Request("action") = "delete" then 
					Response.Write "<br /><a href=""javascript:if (opener && opener.top.TreeFrame){opener.top.TreeFrame.location.href='/cheminv/cheminv/BrowseTree.asp?ClearNodes=1&TreeID=1&MaybeLocSearch=&formelm=&elm1=&elm2=&elm3=&LocationPickerID=&NodeURL=BuildList.asp&NodeTarget=ListFrame&GotoNode=0';opener.top.ListFrame.location.reload();} location.href='/cheminv/gui/menu.asp'""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
					else
					Response.Write "<BR><BR><a HREF=""/cheminv/gui/menu.asp""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
					end if
				else			
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Grid Format operation failed</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
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