<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetContainerAttributes.asp"-->
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
FormData = Request.Form & Credentials

'Response.Write(replace(FormData,"&","<br>"))
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "cheminv/api/CreateContainer.asp", "ChemInv", FormData)
tempArr = split(httpResponse,"|")

'-- Use this list to render created Containers list
displayContainerList = httpResponse

'-- Update location of new Containers to Rack grid locations
RackAssignment = ""
if RackGridList <> "" and Request("AssignToRack") = "on" then
	tempRackArr = split(RackGridList,",")
	if ubound(tempArr) = ubound(tempRackArr) then
		for i=0 to ubound(tempArr)
			QueryString = "ContainerIDs=" & tempArr(i) & _
				"&ValuePairs=Location_ID_FK%3D" & tempRackArr(i)
			QueryString = QueryString & Credentials
			output = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
			if output = "0" then
				if i > 0 then RackAssignment = RackAssignment & ","
				RackAssignment = RackAssignment & tempArr(i) & "::" & tempRackArr(i)
			end if			
		next
	else
		Response.Write("Error during Rack grid location assignment. The number of Containers created do not match the number of Rack Grid Locations.")
	end if
end if 

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
			
			If RackAssignment <> "" and Request("AssignToRack") = "on" then
				theAction = "SelectRack"	
			Elseif isNumeric(httpResponse) then
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
				Case "SelectRack"
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<span class=""GuiFeedback"">New Container has been created and assigned to Racks</span><br /><br />"

					'-- Display simple Rack grid
					Response.write(DisplaySimpleRack(Request("RackID"),httpResponse,""))

					'Response.Write "<script language=javascript>"
					'Response.Write "if (opener){opener.top.ListFrame.location.reload();}"
					'Response.Write "</script>"
					Response.Write "<br /><a href=""javascript:if (opener){opener.top.ListFrame.location.reload();} window.close();""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
					Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
				
				Case "SelectContainer"
					
					Session("CurrentContainerID")= httpResponse
					Session("CurrentLocationID")= Request("LocationID") 
					Response.Write "<SPAN class=""GuiFeedback"">New Container has been created</span><br /><br />"
					
					if Cbool(Request.Form("AutoPrint")) then
						ContainerList = Replace(ContainerList,"|",",")
						Response.write "<script language=javascript>" & vblf
						Response.write "if(opener) {var barcodeWindow = opener.OpenDialog('/Cheminv/GUI/PrintLabelOption.asp?ShowInList=containers&ContainerID=" & ContainerList & "', 'Diag2', 1);} else {var barcodeWindow = window.OpenDialog('/Cheminv/GUI/PrintLabelOption.asp?ShowInList=containers&ContainerID=" & ContainerList & "', 'Diag2', 1);}" & vblf						
						Response.write "</script>" & vblf
					end if
					if lcase(Application("DISPLAY_CREATED_CONTAINERS_SUMMARY")) = "true" then
					'-- Display summary of created Containers
						containerList = replace(displayContainerList,"|",",")
						'containerList = left(containerList, (len(containerList)-1))
						Response.Write(DisplayContainerSummary(containerList))
						
						Select Case Request.Form("RefreshOpenerLocation")
						    Case ""
                                if Ucase(Request.Form("PageSource"))<> "ELN" then
                                    Response.Write "<br /><a href=""javascript:if (opener){opener.top.location.href = '/cheminv/cheminv/BrowseInventory_frset.asp?ClearNodes=0';} window.close()""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
                                end if  					       
						    Case "true"
						        Response.Write "<br /><a href=""javascript:if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(true);} else if (opener){if (typeof(opener.NewRegWindowHandle.refreshRecordDetails) != 'undefined'){opener.NewRegWindowHandle.refreshRecordDetails();} else {opener.top.location.reload();}} window.close();""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						    Case "false"
						        Response.Write "<br /><a href=""javascript:if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);}window.close();""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						End Select
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
						'Response.write "<script language=javascript>if (opener){opener.top.ListFrame.location.reload();}</script>"
						if Cbool(Request.Form("AutoPrint")) then
						    Response.write "<script language=javascript>if( barcodeWindow ) barcodeWindow.focus();</script>" & vblf
						end if 
					else
					'-- Close windows and refresh tree frame					    
						Response.Write "<script language=javascript>"
						if ReturnToSearch = "true" then
						    Response.Write	"if (opener) opener.top.location.href = '/cheminv/inputtoggle.asp?dataaction=db&dbname=cheminv';"
						else
							if Request("ReturnToReconcile") = "true" then
								multiSelect_dict.Add Trim(httpResponse), true
								Response.Write	"if (opener){opener.top.ListFrame.location.reload();}"
							else 							    
								Response.Write	vblf & "if (opener){ " & vblf
								Response.Write	"opener.top.location.href = '/cheminv/cheminv/BrowseInventory_frset.asp?ClearNodes=0';" & vblf
								'Response.Write	"opener.top.ListFrame.location.reload();" & vblf
								Response.Write	"}" & vblf
							end if
						end if
						if Cbool(Request.Form("AutoPrint")) then
						    Response.write "if( barcodeWindow ) barcodeWindow.focus();" & vblf
						end if
						Response.write "window.close();" & vblf
						Response.Write "</script>" & vblf						
					end if					
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
		</td>
	</tr>
</table>

</body>
</html>
