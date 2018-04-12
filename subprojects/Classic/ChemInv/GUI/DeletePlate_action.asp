<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugPrint

bdebugPrint = False
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = "PlateID=" & Request("PlateID") & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeletePlate.asp", "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Plate</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	//var openNodes = "<%=Session("TreeViewOpenNodes" & TreeID)%>";
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
			<% 
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If isNumeric(httpResponse) then
				If CLng(httpResponse) = 0 then
					Session("bMultiSelect") = false
					plate_multiSelect_dict.RemoveAll()
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate has been deleted</SPAN><center>"
					if Request("multiscan") = "1" then
						Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Plates have been deleted.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.focus(); window.close();</SCRIPT>" 
					End if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate could not be deleted</SPAN></center>"
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