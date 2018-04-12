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

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials

'-- Get ID of first Container created for further sampling
SamplingContainerID = ""
SamplingUOMID = ""
SamplingConcentration = ""
SamplingUOCID = ""
SamplingQtyMax = ""
SamplingQtyInitial = ""
SamplingCompoundID = ""
SamplingRegBatchID = ""
SamplingRegID = ""
SamplingBatchNumber = ""
SamplingContainerName = ""

'Response.Write(replace(FormData,"&","<br />"))
'Response.End


if Request("Action") = "new" then

	arrNewContainer = split(Request("iNumCopies"),",")
	arrSampleQty = split(Request("iSampleQty"),",")
	arrContainerTypeID = split(Request("iContainerTypeID"),",")
	arrContainerSize = split(Request("iContainerSize"),",")
	arrUOM = split(Request("iUOM"),",")
	arrConc = split(iif(Request("iConc")=""," ",Request("iConc")),",")
	arrUOC = split(Request("iUOC"),",")
	if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then
		arrRackGridList = split(iif(Request("RackGridList")=""," ",Request("RackGridList")),",")
	end if
	if not IsEmpty(Request("iBarcode")) then arrBarcode = split(Request("iBarcode"),",")
	cntRack = 0
	
	'-- Loop through number of sample volume types
	for i = 0 to ubound(arrNewContainer)

		arrUOMTemp = split(arrUOM(i),"=")
		arrUOCTemp = split(arrUOC(i),"=")
		if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then
			FormData = ""
		else
			FormData = "LocationID=" & Request("LocationID") & "&"
		end if
		FormData = FormData & "UOMID=" & Trim(arrUOMTemp(0)) & "&"
		FormData = FormData & "Concentration=" & Trim(arrConc(i)) & "&"
		FormData = FormData & "UOCID=" & Trim(arrUOCTemp(0)) & "&"
		FormData = FormData & "QtyMax=" & Trim(arrContainerSize(i)) & "&"
		FormData = FormData & "QtyInitial=" & Trim(arrSampleQty(i)) & "&"
		FormData = FormData & "ContainerTypeID=" & Trim(arrContainerTypeID(i)) & "&"
		FormData = FormData & "ContainerStatusID=1&"
		FormData = FormData & "ContainerName=" & Server.URLEncode(Request("ContainerName")) & "&"
		if not IsEmpty(Request("iBarcode")) then
			FormData = FormData & "Barcode=" & Trim(arrBarcode(i)) & "&"
		else
			FormData = FormData & "BarcodeDescID=" & Request("BarcodeDescID") & "&"
		end if
		FormData = FormData & "CompoundID=" & Request("CompoundID") & "&"
		FormData = FormData & "RegBatchID=" & Request("RegBatchID") & "&"
		FormData = FormData & "RegID=" & Request("RegID") & "&"
		FormData = FormData & "BatchNumber=" & Request("BatchNumber") & "&"
		FormData = FormData & "CurrentUserID=" & uCase(Session("UserName" & "ChemInv"))

		'-- Loop through number of copies for sample type
		FormData = FormData & Credentials
		for j = 1 to arrNewContainer(i)

			if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then
				tmpFormData = "LocationID=" & arrRackGridList(cntRack) & "&" & FormData
			elseif instr(FormData,"LocationID") = 0 then
				tmpFormData = "LocationID=" & Request("LocationID") & "&" & FormData
			else
				tmpFormData = FormData
			end if

			cntRack = cntRack + 1
			httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateContainer.asp", "ChemInv", tmpFormData)
			out = out & trim(httpResponse & "|")

			'-- Assign values for further sampling
			if SamplingContainerID = "" then
				SamplingContainerID = httpResponse
				SamplingUOMID = Trim(arrUOMTemp(0))
				SamplingConcentration = Trim(arrConc(i))
				SamplingUOCID = Trim(arrUOCTemp(0))
				SamplingQtyMax = Trim(arrContainerSize(i))
				SamplingQtyInitial = Trim(arrSampleQty(i))
				SamplingCompoundID = Request("CompoundID")
				SamplingRegBatchID = Request("RegBatchID")
				SamplingContainerName = Server.URLEncode(Request("ContainerName"))
				SamplingRegID = Request("RegID")
				SamplingBatchNumber = Request("BatchNumber")
			end if

		next
	next

	if Request("RequestID") <> "" then
		FormData = "?RequestID=" & Request("RequestID")
		'httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/FulfillBatchRequest.asp", "ChemInv", FormData)
		FormData = FormData & "&SampleContainerIDs=" & out
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/FulfillRequest.asp", "ChemInv", FormData)
	end if

else

	'-- Simple Re-Aliquot samples
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateContainerSample.asp", "ChesmInv", FormData)
	out = trim(httpResponse)

	SamplingContainerID = Request("ContainerID")
	arrConc = split(iif(Request("iConc")=""," ",Request("iConc")),",")
	SamplingConcentration = Trim(arrConc(0))
	arrUOC = split(Request("iUOC"),",")
	arrUOCTemp = split(arrUOC(0),"=")
	SamplingUOCID = Trim(arrUOCTemp(0))
	arrContainerSize = split(Request("iContainerSize"),",")
	SamplingQtyMax = Trim(arrContainerSize(0))
	arrSampleQty = split(Request("iSampleQty"),",")
	SamplingQtyInitial = Request("QtyRemaining")
	SamplingCompoundID = Request("CompoundID")
	SamplingRegBatchID = Request("RegBatchID")
	SamplingContainerName = Server.URLEncode(Request("ContainerName"))
	SamplingRegID = Request("RegID")
	SamplingBatchNumber = Request("BatchNumber")

end if


'-- Retire original container if user chooses
if Request("DisposeOrigContainer") = "on" then
	if Application("DEFAULT_REALIQUOT_DISPOSED_LOC") <> "" then 
		AliquotDisposeLocation = Application("DEFAULT_REALIQUOT_DISPOSED_LOC")
	else
		AliquotDisposeLocation = "2"
	end if
	DisposeFormData = "ContainerID=" & ContainerID
	DisposeFormData = DisposeFormData & "&LocationID=" & AliquotDisposeLocation
	DisposeFormData = DisposeFormData & "&ContainerStatusID=6"
	DisposeFormData = DisposeFormData & Credentials
	httpResponse2 = CShttpRequest2("POST", ServerName, "/cheminv/api/RetireContainer.asp", "ChemInv", DisposeFormData)
end if


'Response.Write("<br><br>" & out)
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
<br><br><br>
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

					Response.Write "<span class=""GuiFeedback"">New Container has been created</span><br /><br />"

					resampleHTML = ""
					if SamplingContainerID <> "" then
						if Request("Action") = "new" then
							ActionStr = "Action=new"
						else
							ActionStr = "Action=sample&ContainerID=" & SamplingContainerID & "&QtyRemaining=" & SamplingQtyInitial & "&UOMAbv=" & Request("UOMAbv") & "&ContainerName=" & SamplingContainerName & "&ContainerSize=" & SamplingQtyMax
						end if
						
						'-- Link for further resampling
						resampleHTML = resampleHTML & "<a class=""MenuLink"" href=""/cheminv/gui/CreateContainerSample.asp?" & ActionStr & _
							"&SamplingContainerID=" & SamplingContainerID &_
							"&SamplingUOMID=" & SamplingUOMID &_
							"&SamplingConcentration=" & SamplingConcentration &_
							"&SamplingUOCID=" & SamplingUOCID &_
							"&SamplingQtyMax=" & SamplingQtyMax &_
							"&SamplingQtyInitial=" & SamplingQtyInitial &_
							"&SamplingCompoundID=" & SamplingCompoundID &_
							"&SamplingRegBatchID=" & SamplingRegBatchID &_
							"&SamplingRegID=" & SamplingRegID &_
							"&SamplingBatchNumber=" & SamplingBatchNumber &_
							"&SamplingContainerName=" & SamplingContainerName &_
							""">Create Additional Samples</a><br /><br />"
					end if
					
					Response.Write resampleHTML

					if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then
					'-- Display simple Rack grid
					
						if Request("SelectRackBySearch") = "on" then
							DisplayRackIDs = Request("SuggestedRackID")
						else
							DisplayRackIDs = Request("RackID")
						end if
						Response.write(DisplaySimpleRack(DisplayRackIDs,containerList,""))
						Response.Write "<br /><a href=""javascript:if (opener){ if (opener.top.ListFrame) { opener.top.ListFrame.location.reload();} } window.close()""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						'Response.Write "<br /><a href=""javascript:if (opener){SelectLocationNode(0, " & Request("RackID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();}""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;" & replace(resampleHTML,"Create Additional Samples","<img src=""/cheminv/graphics/sq_btn/btn_additionalsamples.gif"" border=""0"">")

					elseif lcase(Application("DISPLAY_CREATED_CONTAINERS_SUMMARY")) = "true" then
					'-- Display summary of created Containers

						containerList = replace(Trim(containerList),"|",",")
						containerList = left(containerList, (len(containerList)-1))
						'response.Write containerList
						'response.End
						Response.Write(DisplayContainerSummary(containerList))
						'Response.Write "<br /><a href=""javascript:if (opener){opener.top.ListFrame.location.reload();}window.close()""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "<br /><a href=""javascript:SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();""><img src=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>"
						Response.Write "&nbsp;<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>"
						'Response.write "<script language=javascript>if (opener){opener.top.ListFrame.location.reload();}</script>"
					else
					'-- Automatically close window
						Response.write "<Script language=javascript>if (opener.top.ListFrame){opener.top.ListFrame.location.reload();} window.close();</script>"
					end if

					'-- Print Barcode Labels
					if Request("AutoPrint") = "on" then
						if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then
							ContainerList = Replace(trim(ContainerList),"|",",")
							ContainerList = left(ContainerList, (len(ContainerList)-1))
						end if
						Response.write "<Script language=javascript>barcodeWindow = OpenDialog('/Cheminv/GUI/PrintLabel.asp?ContainerID=" & ContainerList & "', 'Diag2', 1); barcodeWindow.focus();</script>"
					end if

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
		</td>
	</tr>
</table>
</body>
