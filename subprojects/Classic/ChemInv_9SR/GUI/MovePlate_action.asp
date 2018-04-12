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

'Response.Write FormData
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/MovePlate.asp", "ChemInv", FormData)
'Response.Write httpResponse
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move an Inventory Plate</title>
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
					Count = plate_multiSelect_dict.Count
					plate_multiSelect_dict.RemoveAll()
					Session("CurrentLocationID") = httpResponse
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate has been moved.</SPAN></center>"
					if Request("multiscan") = "1" then
						Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Count & " Plates have been moved.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.parent.focus(); window.close();</SCRIPT>"
					End if
				else				
					Response.Write "<center><table><tr><td><P><CODE>" & Application(httpResponse) & "</CODE></P></td></tr></table></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate could not be moved</SPAN></center>"
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