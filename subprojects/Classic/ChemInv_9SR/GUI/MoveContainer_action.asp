<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
DestinationLocationID = Request("ParentID")
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MoveContainer.asp", "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move and Inventory Container</title>
<script language="JavaScript">
	window.focus();
</script>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER="0" CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff Width=90%>
	<TR>
		<TD>
			<% 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) >= 0 then
					Session("bMultiSelect") = false
					Ccount = multiSelect_dict.Count
					multiSelect_dict.RemoveAll()
					Session("CurrentLocationID") = httpResponse
					Response.Write "<center><SPAN class=""GuiFeedback"">Container has been moved.</SPAN></center>"
					if Request("multiscan") = "1" then
						Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Containers have been moved.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>"
					End if
				else				
					Response.Write "<center><table><tr><td><P><CODE>" & Application(httpResponse) & "</CODE></P></td></tr></table></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Container could not be moved</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
					Response.Write "<P><CODE>Oracle Error:<BR> " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>