<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Cmd
Dim Conn
Dim httpResponse
Dim FormData
Dim ServerName
DestinationLocationID = Request("ParentID")
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

RackGridList = Request("RackGridList")
ContainerIDList = Request("ContainerID")

'Response.Write(replace(FormData,"&","<br>"))
'Response.End

if RackGridList <> "" then
	tmpRackGridList = split(RackGridList,",")
	tmpContainerID = split(ContainerIDList,",")
	QueryString = ""
	for i = 0 to uBound(tmpRackGridList)
		QueryString = "ContainerID=" & tmpContainerID(i) & "&LocationID=" & tmpRackGridList(i) & Credentials
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MoveContainer.asp", "ChemInv", QueryString)
	next
else
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MoveContainer.asp", "ChemInv", FormData)
'Response.Write(httpResponse)
'Response.End

end if

'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move Inventory Container</title>
<script language="JavaScript">
	window.focus();
</script>
<script language=javascript src="/cheminv/choosecss.js"></script>
<script language=javascript src="/cheminv/gui/refreshgui.js"></script>
</head>
<body>
<br><br><br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td align="center">
			<% 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					Session("bMultiSelect") = false
					Ccount = multiSelect_dict.Count
					multiSelect_dict.RemoveAll()
					if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then 
					'-- Display simple Rack grid
						if Request("SuggestedRackID") <> "" then
							Session("CurrentLocationID") = Request("SuggestedRackID")
						else
							Session("CurrentLocationID") = Request("RackID")
						end if
						Response.write(DisplaySimpleRack(Session("CurrentLocationID"),ContainerIDList,""))
						Response.Write "<br /><a href=""javascript:if (opener){SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();}""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
					else
					'-- Display text container has moved
						Session("CurrentLocationID") = httpResponse
						Response.Write "<center><SPAN class=""GuiFeedback"">Container has been moved.</SPAN></center>"
						if Request("multiscan") = "1" then
							Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Containers have been moved.'; opener.focus(); window.close();</SCRIPT>"				
						Else
							Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>"
						End if
					end if
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
		</td>
	</tr>
</table>
</body>