<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
rc= WriteUserProperty(Session("UserNameCheminv"), "Project" , Request("Project"))
rc= WriteUserProperty(Session("UserNameCheminv"), "Job" , Request("Job"))


Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugprint
Dim Conn
bdebugprint = false

AutoGen = Request("AutoGen")
'AutoPrint = Request("AutoPrint")
ReturnToSearch = Request("ReturnToSearch")
If AutoGen <> "" then 
	Session("AutoGen") = AutoGen
Else	
	AutoGen = Session("AutoGen")
End if
If AutoGen = "" then AutoGen = "true"

If ReturnToSearch <> "" then 
	Session("ReturnToSearch") = ReturnToSearch
Else	
	ReturnToSearch = Session("ReturnToSearch")
End if

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/OrderContainer.asp", "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Order New Substance</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
			<% 
			if bdebugprint then
				Response.Write httpResponse
				Response.end
			End if
			
			If isNumeric(httpResponse) then
				if Clng(httpResponse) > 0 then
					containerList = httpResponse
					theAction = "SelectContainer"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(httpResponse,18),"|") then
				containerList = httpResponse
				httpResponse = left(httpResponse,InStr(httpResponse,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			Select Case theAction
				Case "SelectContainer"
					Session("CurrentContainerID")= httpResponse
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">New Container has been created</SPAN>"
					Response.Write "<script language=javascript>"
					if ReturnToSearch = "true" then
						Response.Write	"opener.top.location.href = '/cheminv/inputtoggle.asp?dataaction=db&dbname=cheminv';"
					else
						Response.Write	"opener.top.location.href = '/cheminv/cheminv/BrowseInventory_frset.asp?ClearNodes=0';"
					end if
					Response.Write "</script>"
					
					'if Cbool(Request.Form("AutoPrint")) then
					'	ContainerList = Replace(ContainerList,"|",",")
					'	Response.write "<Script language=javascript>barcodeWindow = OpenDialog('/Cheminv/GUI/PrintLabel.asp?ContainerID=" & ContainerList & "', 'Diag2', 1); barcodeWindow.focus();</script>"		
					'End if
						Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
			%>
		</TD>
	</TR>
</TABLE>
</Body>