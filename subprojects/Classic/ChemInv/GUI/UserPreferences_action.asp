<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<%

'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Cmd
Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugprint
bdebugprint = false

AutoGen = Request("AutoGen")
AutoPrint = Request("AutoPrint")
ReturnToSearch = Request("ReturnToSearch")
ReturnToReconcile = Request("ReturnToReconcile")
RackGridList = Request("RackGridList")
Source = Request("PageSource")

If AutoGen <> "" then 
	Session("AutoGen") = AutoGen
Else	
	AutoGen = Session("AutoGen")
End if
If AutoGen = "" then AutoGen = "true"

If AutoPrint <> "" then 
	Session("AutoPrint") = AutoPrint
Else	
	AutoPrint = Session("AutoPrint")
End if
If AutoPrint = "" then AutoPrint = "false"

If ReturnToSearch <> "" then 
	Session("ReturnToSearch") = ReturnToSearch
Else	
	ReturnToSearch = Session("ReturnToSearch")
End if

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials & "&UserSettingsAction=save"

'Response.Write(replace(FormData,"&","<br>"))
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, "cheminv/api/CreateUserPreferences.asp", "ChemInv", FormData)
displayContainerList = httpResponse

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Save User Settings</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();	
</script>

</head>
<body style="overflow:auto">
<br /><br /><br />
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td height=50 valign=middle align=center>
			<%  
			if bdebugprint then
				Response.Write httpResponse
				Response.end
			End if
			if lcase(httpResponse) = "success" then 
					Response.Write "<SPAN class=""GuiFeedback""><center>User preferences for containers Saved</SPAN>"
                    if ucase(Source) <> "ELN" then	
					Response.Write "<P><center><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			        end if 
			else
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					if ucase(Source) <> "ELN" then		
                       Response.Write "<P><center><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					end if 
			End if
			%>
		</td>
	</tr>
</table>

</body>
</html>
