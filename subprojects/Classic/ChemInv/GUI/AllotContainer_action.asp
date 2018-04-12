<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

'response.Write FormData
'response.End
'allot the containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/AllotContainer.asp", "ChemInv", FormData)
out = trim(httpResponse)

'Response.Write out
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create a New Inventory Container</title>
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
			If isNumeric(out) then
				if Clng(out) > 0 then
					containerList = out
					theAction = "SelectContainer"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "SelectContainer"
					Session("CurrentContainerID")= Request("ContainerID")
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">New Container has been created</SPAN>"
					Response.Write "<script language=javascript>"
					Response.Write	"if (opener.top.main){opener.top.main.ListFrame.location.reload();}"
					Response.Write	"else if (opener){opener.top.ListFrame.location.reload();}"
					Response.Write "</script>"
					Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
