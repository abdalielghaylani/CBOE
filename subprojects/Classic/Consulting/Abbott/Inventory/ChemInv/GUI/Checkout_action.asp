<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CheckOutContainer.asp", "ChemInv", FormData)
'Response.Write httpResponse
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Check out an Inventory Container</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=1 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff Width=90%>
	<TR>
		<TD>
			<% 
			If IsNumeric(httpresponse) then 
				If CLng(httpResponse) > 0 then
					Session("bMultiSelect") = false
					Ccount = multiSelect_dict.Count
					multiSelect_dict.RemoveAll()
					Session("CurrentLocationID") = httpResponse
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Container has been checked out.</SPAN></center>"
					if Request("multiscan") = "1" then
						Response.Write "<SCRIPT language=JavaScript>opener.location.href='multiscan_list.asp?clear=1&message=" & Ccount & " Containers have been checked in/out.'; opener.focus(); window.close();</SCRIPT>"				
					Else
						Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>"				
					End if
				else				
					Response.Write "<P><CODE>ChemInv API Error: " & Application(httpResponse) & "</CODE></P>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Container could not be checked out</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpresponse
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>