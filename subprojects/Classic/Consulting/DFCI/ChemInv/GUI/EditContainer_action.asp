<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

'Response.Write(replace(FormData,"&","<br>") & "<br><br>")
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateAllContainerFields.asp", "ChemInv", FormData)

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update a New Inventory Container</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->
</script>
</head>
<body>
<br><br><br><br><br><br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff>
	<tr>
		<td height=50 valign=middle>
			<% 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					if Request("LocationID") = Session("CurrentLocationID") then
						Session("CurrentLocationID") = Request("LocationID")
					end if
					Response.Write "<center><SPAN class=""GuiFeedback"">Container has been updated</SPAN></center>"
					if (Request("RackID") <> "" and Request("AssignToRack")="on") or (Request("LocationID") <> Session("CurrentLocationID")) then
						if Request("RackID") <> "" and cStr(Request("RackID")) <> "NULL" then 
							RedirectLocation = Request("RackID")
						elseif Session("CurrentLocationID") <> "" then
							RedirectLocation = Session("CurrentLocationID")
						else
							RedirectLocation = Request("LocationID")
						end if
						Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocation(" & RedirectLocation & ", 'Rack Location', " & Request("ContainerID") & "); opener.focus(); window.close();</SCRIPT>" 
					else
						Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>"
					end if 
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Container could not be updated</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</td>
	</tr>
</table>
</body>