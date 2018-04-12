<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetContainerAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<%
Dim Cmd
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

BatchContainerIDs = Request("BatchContainerIDs")
Distribution = Request("Distribution")
RackGridList = Request("RackGridList")
LocationID = Request("LocationID")
ContainerTypeID = Request("ContainerTypeID")
ContainerSize = Request("ContainerSize")
RequestID = Request("RequestID")
BarcodeDescID = Request("BarcodeDescID")
DateCertified = Request("DateCertified")
Mode = Request("Mode")
'for each key in Request.Form
'	Response.Write key & "=" & Request.Form(key) & "<BR>"
'next

'-- Store and pass on re-sampling information
rsNumContainersDisplay = request("NumContainersDisplay")
rsContainerIDList = Request("ContainerIDList")
rsLocationBarCode = Request("lpLocationBarCode")
rsContainerSize = Request("ContainerSize")
rsQtyContainer = Request("Sample1")
rsDist = split(Distribution,",")
rsDistOrig = split(rsDist(0),":")
rsRemaingOrigQty = rsDistOrig(3)

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

'Response.Write(replace(Request.Form,"&","<br>"))
'Response.End

if RackGridList <> "" then
	arrRack = split(RackGridList,",")
	currRack = 0
end if
arrTemp1 = split(Distribution,",")
currSampleIndex = 1

'-- Retire original container if user chooses
if Request("DisposeOrigContainer") = "on" then
	ContainerIDList = Request("ContainerIDList")
	arrContainerIDListTemp = split(ContainerIDList,",")
	if Application("DEFAULT_REALIQUOT_DISPOSED_LOC") <> "" then 
		AliquotDisposeLocation = Application("DEFAULT_REALIQUOT_DISPOSED_LOC")
	else
		AliquotDisposeLocation = "2"
	end if
	for k = 0 to ubound(arrContainerIDListTemp)
		DisposeFormData = "ContainerID=" & arrContainerIDListTemp(k)
		DisposeFormData = DisposeFormData & "&LocationID=" & AliquotDisposeLocation
		DisposeFormData = DisposeFormData & "&ContainerStatusID=6"
		DisposeFormData = DisposeFormData & Credentials
		'Response.Write(DisposeFormData & "<br><br>")
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/RetireContainer.asp", "ChemInv", DisposeFormData)
		'out = trim(httpResponse)
	next
end if

for i=0 to ubound(arrTemp1)
	arrTemp2 = split(arrTemp1(i),":")
	currContainerID = arrTemp2(0)
	currContainerBarcode = arrTemp2(1)
	currNumSamples = cint(arrTemp2(2))
	currQtyRemaining = arrTemp2(3)
	currUOMAbv = arrTemp2(4)
	if currNumSamples > 0 then
		FormData = "ContainerID=" & currContainerID
		FormData = FormData & "&NumContainers=" & currNumSamples
		FormData = FormData & "&LocationID=" & LocationID
		FormData = FormData & "&ContainerTypeID=" & ContainerTypeID
		FormData = FormData & "&Action=sample"
		FormData = FormData & "&Mode=" & Mode
		FormData = FormData & "&ContainerSize=" & ContainerSize
		FormData = FormData & "&BarcodeDescID=" & BarcodeDescID
		for j = 1 to currNumSamples
			currQty = eval("Request(""Sample" & currSampleIndex & """)")
			FormData = FormData & "&Sample" & j & "=" & currQty
			currSampleIndex = currSampleIndex + 1
		next
		FormData = FormData & "&QtyRemaining=" & currQtyRemaining
		FormData = FormData & "&RequestID=" & RequestID
		FormData = FormData & "&DateCertified=" & DateCertified
		if len(Application("StatusRequestedSamples")) > 0 then	FormData = FormData & "&ContainerStatusID=" & Application("StatusRequestedSamples")
		FormData = FormData & Credentials
	
		'-- Allot the containers
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/AllotContainer.asp", "ChemInv", FormData)
		out = trim(httpResponse)

		'-- Assign created containers to rack locations
		if Request("AssignToRack") = "on" and RackGridList <> "" then
			arrContainerID = split(out,"|")
			for l = 0 to ubound(arrContainerID)
				QueryString = "ContainerIDs=" & arrContainerID(l) &_
					"&ValuePairs=Location_ID_FK%3D" & arrRack(currRack)
				QueryString = QueryString & Credentials
				currRack = currRack + 1
				httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
				
			next
		end if
		
	end if
next

'Response.Write(out & "##")
'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<script language=javascript src="/cheminv/choosecss.js"></script>
<script language=javascript src="/cheminv/gui/refreshgui.js"></script>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<br>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td height=50 valign=middle align=center>
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

			Select Case theAction
				Case "SelectContainer"
					Session("CurrentContainerID")= Request("ContainerID")

					'-- If assigned to Rack, use RackID instead of grid location id
					if not isBlank(Request("RackGridList")) and Request("AssignToRack") = "on" then
						if Request("SelectRackBySearch") = "on" then
							if Instr(Request("SuggestedRackID"),",") > 0 then
								arrSuggestRackID = split(Request("SuggestedRackID"),",")
								Session("CurrentLocationID") = arrSuggestRackID(0)
							else
								Session("CurrentLocationID")= Request("SuggestedRackID")
							end if
						else
							if Instr(Request("RackID"),",") > 0 then
								arrLocationRackID = split(Request("RackID"),",")
								Session("CurrentLocationID") = arrLocationRackID(0)
							else
								Session("CurrentLocationID")= Request("RackID")
							end if
						end if
					else
						Session("CurrentLocationID")= Request("LocationID")
					end if

					Response.Write "<span class=""guifeedback"">New Container has been created</span><br /><br />"

					'-- If user has not chosen to dispose original container and Qty of Orig is greater than 0, allow the to create additional samples
					resampleHTML = ""
					if Cdbl(rsRemaingOrigQty) > 0 and Request("DisposeOrigContainer") <> "on" then
						resampleHTML = resampleHTML & "<a class=""MenuLink"" href=""/Cheminv/GUI/CreateSamplesFromBatch.asp?Action=new&amp;FormStep=&amp;batchid=&amp;DefLocationID=" & rsLocationBarCode & "&amp;RequestID=" & RequestID & "&amp;ContainerIDList=" & rsContainerIDList & "&amp;QtyPerSample=" & rsQtyContainer & "&amp;ContainerSize=" & rsContainerSize & "&amp;NumContainersDisplay=" & rsNumContainersDisplay & """>Create Additional Samples</a><br /><br />"
						Response.Write resampleHTML
					end if

					if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then 
					
						if Request("SelectRackBySearch") = "on" then
							DisplayRackIDs = Request("SuggestedRackID")
						else
							DisplayRackIDs = Request("RackID")
						end if
						Response.write(DisplaySimpleRack(DisplayRackIDs,containerList,""))
						Response.Write "<br /><a href=""javascript:if (opener){opener.location.reload();} window.close()""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
						if Cdbl(rsRemaingOrigQty) > 0 and Request("DisposeOrigContainer") <> "on" then
							Response.Write "&nbsp;" & replace(resampleHTML,"Create Additional Samples","<img src=""/cheminv/graphics/sq_btn/btn_additionalsamples.gif"" border=""0"">")
						end if
					elseif lcase(Application("DISPLAY_CREATED_CONTAINERS_SUMMARY")) = "true" then
					
						containerList = replace(containerList,"|",",")
						'containerList = left(containerList, (len(containerList)-1))
						Response.Write(DisplayContainerSummary(containerList))
						'Response.Write "<br /><a href=""javascript:if (opener){opener.top.ListFrame.location.reload();}window.close()""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "<br /><a href=""javascript:SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
						
					end if 	
				Case "WriteAPIError"
					Response.Write "<p><code>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<span class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<p><center><a href=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<p><code>Oracle Error: " & out & "</code></p>" 
					Response.Write "<p><center><a href=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</td>
	</tr>
</table>
</body>			
