<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Empty a Trash Location</title>
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
<%
Response.Write "<DIV ALIGN=""center"" ID=""processingImage""><img src=""" & Application("ANIMATED_GIF_PATH") & """ WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugPrint

bdebugPrint = false
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = "LocationID=" & Request("LocationID") & "&trashType=" & Request("trashType") & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/EmptyTrash.asp", "ChemInv", FormData)
%>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
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
					Response.Write "<center><SPAN class=""GuiFeedback"">Trash location emptied.</SPAN><center>"
					if Request("multiscan") = "1" then
						Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Containers have been deleted.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0);opener.focus();window.close()</SCRIPT>" 	
					End if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Trash could not be emptied.</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
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