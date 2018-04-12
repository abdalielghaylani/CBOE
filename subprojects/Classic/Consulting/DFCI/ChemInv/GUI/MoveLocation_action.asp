<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = False
DestinationLocationID = Request("ParentID")
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

'Response.Write formdata
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MoveLocation.asp", "ChemInv", FormData)
LocationType = Request("LocationType")
if LocationType = "rack" then
	LocationText = "Rack"
else
	LocationText = "Location"
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move and Inventory Location</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=Session("TreeViewOpenNodes1")%>";
//-->
</script>
</head>
<body>
<br><br><br><br><br><br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff>
	<tr>
		<td>
			<%
			If bDebugPrint then 
				Response.Write httpResponse
				Response.End
			End if
			If IsNumeric(httpResponse) then 
				If CLng(httpResponse) >= 0 then
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">" & LocationText & " has been moved.</SPAN></center>"
					if Session("GUIReturnURL") = "" then
						Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(1, " & Request("LocationID") & ", 0);opener.focus();window.close()</SCRIPT>" 
					Else
						Response.write OkButton(Session("GUIReturnURL"), "")	
					End if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">" & LocationText & " could not be moved</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
				Response.End
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>