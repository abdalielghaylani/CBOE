<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugPrint

bdebugPrint = false
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = "ContainerID=" & Request("ContainerID") & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteContainer.asp", "ChemInv", FormData)
'Response.Write(httpResponse)
'httpResponse = 1
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an empty Inventory Location</title>
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
<br><br><br><br><br><br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td height=50 valign=middle align=center>
		
			<% 
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					Session("bMultiSelect") = false
					Ccount = multiSelect_dict.Count
					multiSelect_dict.RemoveAll()
					Response.Write "<center><SPAN class=""GuiFeedback"">Container has been deleted</SPAN><center>"
					if Request("multiscan") = "1" then
						Response.Write "<script language=""javascript"">opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Containers have been deleted.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT LANGUAGE=javascript>"
						if Session("CurrentLocationID") <> "" then 
						Response.write "SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0); "
						end if
						Response.write "if (parent.opener.top.main) {parent.opener.top.main.location.reload();} opener.focus(); window.close()"
						Response.Write "</SCRIPT>" 	
					End if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Container could not be deleted</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
					Response.Write "<P><CODE>Oracle Error:<BR> " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</td>
	</tr>
</table>
</body>