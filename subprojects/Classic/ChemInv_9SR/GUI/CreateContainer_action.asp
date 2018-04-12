<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetContainerAttributes.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugprint
bdebugprint = false

AutoGen = Request("AutoGen")
AutoPrint = Request("AutoPrint")
ReturnToSearch = Request("ReturnToSearch")
ReturnToReconcile = Request("ReturnToReconcile")
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
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateContainer.asp", "ChemInv", FormData)
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
			PrevHREF = Request("HREF")
			if instr(PrevHREF,"GetData=new") then PrevHREF = replace(PrevHREF,"GetData=new","GetData=session")
			Select Case theAction
				Case "SelectContainer"
					Session("CurrentContainerID")= httpResponse
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">New Container has been created</SPAN>"
					Response.Write "<script language=javascript>"
					if ReturnToSearch = "true" then
						Response.Write	"if (opener) opener.top.location.href = '/cheminv/inputtoggle.asp?dataaction=db&dbname=cheminv';"
					else
						if Request("ReturnToReconcile") = "true" then
							multiSelect_dict.Add Trim(httpResponse), true
							Response.Write	"if (opener){opener.top.ListFrame.location.reload();}"
						else 
							Response.Write	"if (opener) opener.top.location.href = '/cheminv/cheminv/BrowseInventory_frset.asp?ClearNodes=0';"
						end if
					end if
					Response.Write "</script>"
					
					if Cbool(Request.Form("AutoPrint")) then
						ContainerList = Replace(ContainerList,"|",",")
						Response.write "<Script language=javascript>barcodeWindow = OpenDialog('/Cheminv/GUI/PrintLabel.asp?ContainerID=" & ContainerList & "', 'Diag2', 1); barcodeWindow.focus();</script>"		
					End if
						Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""location='" & PrevHREF & "'; return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
			%>
		</TD>
	</TR>
</TABLE>
</Body>