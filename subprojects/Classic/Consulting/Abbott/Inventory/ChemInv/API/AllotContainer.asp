<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim httpResponse
Dim FormData
Dim Credentials
Dim QueryString
Dim ServerName
Dim arrValues()
Dim ContainerID
Dim Action
Dim NumContainers
Dim ContainerSize
Dim CurrValue
Dim bSameValue
Dim bDebugPrint
Dim strError
Dim bWriteError

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:AllotContainer<BR>"

'Required Parameters
ContainerID = Request("ContainerID")
NumContainers = Request("NumContainers")
LocationID = Request("LocationID")
ContainerTypeID = Request("ContainerTypeID")
Action = Request("action")
Mode = Request("mode")
ContainerSize = Request("ContainerSize")
'Also required are fields for the container quantities labeled sample1,sample2, ... or QtyRemaining

'Optional Parameters
SampleQtyUnit = Request("SampleQtyUnit")
BarcodeDescID = Request("BarcodeDescID")
DateCertified = Request("DateCertified")
ContainerStatusID = Request("ContainerStatusID")
'ShowFormVars true

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/AllotContainer.htm"
	Response.end
End if


' Check for required parameters
If IsEmpty(ContainerID) then
	strError = strError & "ContainerID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(NumContainers) then
	strError = strError & "NumContainers is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ContainerTypeID) then
	strError = strError & "ContainerTypeID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(Action) then
	strError = strError & "Action is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ContainerSize) then
	strError = strError & "ContainerSize is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

bSameValue = true

ServerName = Request.ServerVariables("Server_Name")
'-- Don't need to add credentials b/c they are included in the request.form
FormData = Request.Form
'Response.Write FormData & "<BR>"
'Response.End

'-- Create the new containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CopyContainer.asp", "ChemInv", FormData)
out = trim(httpResponse)
'Response.Write out & "<BR>"

'-- Initialize values array and set first container values
if Action = "split" then
	ReDim arrValues((NumContainers),2)
	arrValues(0,0) = ContainerID
	arrValues(0,1) = eval("Request(""" & Action & "1"")")
elseif Action = "sample" then
	ReDim arrValues((NumContainers),2)
	arrValues(0,0) = ContainerID
	arrValues(0,1) = Request("QtyRemaining")
end if

'set remaining container values
if inStr(out, "|") then
	arrContainerIDs = split(out, "|")
	for i = 0 to ubound(arrContainerIDs)
		arrValues(i+1,0) = arrContainerIDs(i)
		arrValues(i+1,1) = eval("Request(""" & Action & (i+1) & """)")	
	next
else
	arrValues(1,0) = out
	if action = "split" then
		arrValues(1,1) = eval("Request(""" & Action & "2"")")
	elseif action = "sample" then
		arrValues(1,1) = eval("Request(""" & Action & "1"")")	
	end if 
end if

'-- Test for same qty values in order to do a single update
if action = "split" then
	CurrValue = arrValues(0,1)
	SplitContainerIDs = ""
	for i = 0 to ubound(arrValues,1)
		if CurrValue <> arrValues(i,1) then
			bSameValue = false
			exit for
		end if
		CurrValue = arrValues(i,1)
		SplitContainerIDs = SplitContainerIDs & arrValues(i,0) & ","
	next
	SplitContainerIDs = Mid(SplitContainerIDs,1,(len(SplitContainerIDs)-1))	

'-- Assume that the first value will always be different so start with the 2nd
elseif action = "sample" then
	CurrValue = arrValues(1,1)
	SampleContainerIDs = ""
	for i = 1 to ubound(arrValues,1)
		if CurrValue <> arrValues(i,1) then
			bSameValue = false
			exit for
		end if
		CurrValue = arrValues(i,1)		
		SampleContainerIDs = SampleContainerIDs & arrValues(i,0) & ","
	next
	SampleContainerIDs = Mid(SampleContainerIDs,1,(len(SampleContainerIDs)-1))
end if

'-- Update quantities, location, containertype, date_certified
'-- Set qty_minstock = NULL for new containers
Credentials = "&CSUserName=" & Request("CSUserName") & "&CSUSerID=" & Request("CSUSerID")
if bSameValue then
	if action = "split" then
		QueryString = "ContainerIDs=" & SplitContainerIDs & _
			"&ValuePairs=" & _
			"Qty_Remaining%3D" & arrValues(0,1) & _
			"::Qty_Initial%3D" & arrValues(0,1) & _
			"::Qty_Max%3D" & ContainerSize & _
			"::Qty_available%3D" & arrValues(0,1) & _
			"::Location_ID_FK%3D" & LocationID & _
			"::Container_Type_ID_FK%3D" & ContainerTypeID & _
			"::Qty_Minstock%3DNULL"
		if BarcodeDescID <> "" then QueryString = QueryString & "::barcode_desc_id%3D" & BarcodeDescID
		QueryString = QueryString & Credentials
		'Response.Write QueryString
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
		out2 = httpResponse
		'Response.Write out2
		'Response.End
	elseif action = "sample" then
	
		'-- Update original container
		QueryString = "ContainerIDs=" & arrValues(0,0) & _
			"&ValuePairs=" & _
			"Qty_Remaining%3D" & arrValues(0,1) & _
			"::Qty_available%3D" & arrValues(0,1)
		QueryString = QueryString & Credentials
		'Response.Write QueryString
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
		out2 = httpResponse
		'Response.Write out2
		'Response.End

		'-- Update the rest
		QueryString = "ContainerIDs=" & SampleContainerIDs & _
			"&ValuePairs=" & _
			"Qty_Remaining%3D" & arrValues(1,1) & _
			"::Qty_Initial%3D" & arrValues(1,1) & _
			"::Qty_Max%3D" & ContainerSize & _
			"::Qty_available%3D" & arrValues(1,1) & _
			"::Qty_Minstock%3DNULL" & _
			"::Parent_container_id_fk%3D" & arrValues(0,0) & _
			"::Location_ID_FK%3D" & LocationID & _
			"::Container_Type_ID_FK%3D" & ContainerTypeID & _
			"::date_certified%3D" & "TO_DATE('" & DateCertified & "','" & Application("DATE_FORMAT_STRING") & "')"
		if not isEmpty(SampleQtyUnit) and SampleQtyUnit <> "" then QueryString = QueryString & "::Unit_of_Meas_ID_FK%3D" & SampleQtyUnit
		if BarcodeDescID <> "" then QueryString = QueryString & "::barcode_desc_id%3D" & BarcodeDescID
		if ContainerStatusID <> "" and not isEmpty(ContainerStatusID) then QueryString = QueryString & "::container_status_id_fk=" & ContainerStatusID
		QueryString = QueryString & Credentials
		'Response.Write QueryString & "<BR>"
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
		out2 = httpResponse
		'Response.Write out2 & "<BR>"
		'Response.End
	end if
else
	for i = 0 to ubound(arrValues,1)
		if action = "sample" and i = 0 then
			'don't update the container size for the original container
			QueryString = "ContainerIDs=" & arrValues(i,0) & _
				"&ValuePairs=" & _
				"Qty_Remaining%3D" & arrValues(i,1) & _
				"::Qty_available%3D" & arrValues(i,1)
		else
			
			QueryString = "ContainerIDs=" & arrValues(i,0) & _
				"&ValuePairs=" & _
				"Qty_Remaining%3D" & arrValues(i,1) & _
				"::Qty_Initial%3D" & arrValues(i,1) & _
				"::Qty_Max%3D" & ContainerSize & _
				"::Qty_available%3D" & arrValues(i,1) & _
				"::Qty_Minstock%3DNULL" & _
				"::Location_ID_FK%3D" & LocationID & _
				"::Container_Type_ID_FK%3D" & ContainerTypeID & _
				"::date_certified%3D" & "TO_DATE('" & DateCertified & "','" & Application("DATE_FORMAT_STRING") & "')"
			if action = "sample" then QueryString = QueryString & "::Parent_container_id_fk%3D" & arrValues(0,0)
			if ContainerStatusID <> "" and not isEmpty(ContainerStatusID) then QueryString = QueryString & "::container_status_id_fk=" & ContainerStatusID
			if not isEmpty(SampleQtyUnit) and SampleQtyUnit <> "" then QueryString = QueryString & "::Unit_of_Meas_ID_FK%3D" & SampleQtyUnit
			if BarcodeDescID <> "" then QueryString = QueryString & "::barcode_desc_id%3D" & BarcodeDescID
		end if
		QueryString = QueryString & Credentials
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
		out2 = httpResponse
	next
end if
'Response.Write out2
'Response.End

'Response.Write querystring
'Response.End

if action = "split" then
	'dispose original container
	FormData = "ContainerID=" & ContainerID & Credentials
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteContainer.asp", "ChemInv", FormData)
	'set the quantity to 0
	QueryString = "ContainerIDs=" & ContainerID & _
		"&ValuePairs=" & _
		"Qty_Remaining%3D0" & _
		"::Qty_available%3D0"		
	QueryString = QueryString & Credentials
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
	out2 = httpResponse
end if

RequestID = Request("RequestID")
'-- Update Request if RequestID exists
'-- Except when page is called from CreateSamplesFromBatch process
if mode <> "CreateSamplesFromBatch" then
	if not isEmpty(RequestID) then
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest", adCmdStoredProc)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PSAMPLECONTAINERIDS",advarchar, 1, 500, out)
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest")
	end if 
end if

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
