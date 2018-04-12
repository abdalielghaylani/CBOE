<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
action = Lcase(Request.QueryString("action"))
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials
'Response.Write FormData 
'Response.end
Select Case action
	Case "delete"
		APIURL = "/cheminv/api/DeleteLink.asp"
	Case "edit"
		APIURL = "/cheminv/api/UpdateLink.asp"
	Case "create"
		APIURL = "/cheminv/api/CreateLink.asp"
End Select

httpResponse = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", FormData)

%>
<html>
<head>
<title>Manage Links</title>
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
		<td height=50 valign=middle nowrap>
			<%
			If IsNumeric(httpresponse) then 
				If Clng(httpResponse) > 0 then
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Link change has been processed</SPAN><center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>opener.location.href='/cheminv/gui/manageLinks.asp?fk_value=" & Request("fk_value") & "&fk_name=" & Request("fk_name") & "&Table_Name=" & Request("Table_Name") & "&LinkType=" & Request("LinkType") & "';window.close()</SCRIPT>"
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Link could not be changed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write httpresponse
				Response.end
			End if
			%>
		</td>
	</tr>
</table>
</body>