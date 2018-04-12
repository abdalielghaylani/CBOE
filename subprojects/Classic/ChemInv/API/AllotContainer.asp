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
if Application("ENABLE_OWNERSHIP")="TRUE" then
PrincipalID=Request("PrincipalID")
End if
'Also required are fields for the container quantities labeled sample1,sample2, ... or QtyRemaining

'Optional Parameters
SampleQtyUnit = Request("SampleQtyUnit")
BarcodeDescID = Request("BarcodeDescID")
DateCertified = Request("DateCertified")
ContainerStatusID = Request("ContainerStatusID")
'ShowFormVars true

'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Getting the Custom Fullfill Request fields from the form and replacing the single quote (') with 2 single qoutes ('')
'-- Date: 09/04/2010
For each Key in custom_fulfillrequest_fields_dict
	execute(key & " = Replace(Request(""" & key & """),""'"",""''"")")
Next	
'-- End of Change #123488#
RequestID = Request("RequestID")
if Request("RequestID")<>"" then 
    GetInvConnection()
    Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GETBATCHREQUESTBYREQUESTEDUNIT(?,?)}", adCmdText)	
    Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
    Cmd.Parameters("PREQUESTID").Precision = 9	
    Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set RS = Cmd.Execute
    if not (RS.EOF or RS.BOF) Then
        Cmd.Properties ("PLSQLRSet") = FALSE
        Field_1 = RS("field_1")
        Field_2 = RS("field_2")
        Field_3 = RS("field_3")
        Field_4 = RS("field_4")
        Field_5 = RS("field_5")
        Date_1 = RS("date_1")
        Date_2 = RS("date_2")
    end if   
end if

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

'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Validting the Custom Fullfill Request required fields from the form
'-- Date: 09/04/2010
For each Key in req_custom_fulfillrequest_fields_dict
	if IsEmpty(Request(Key)) then
		strError = strError & Key & " is a required parameter<BR>"
		bWriteError = True
	end if
Next	
'-- End of Change #123488#

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Define default values for custom fulfill request fields
'-- Date: 09/04/2010
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if
'-- End of Change #123488#

bSameValue = true

ServerName = Request.ServerVariables("Server_Name")
'-- Don't need to add credentials b/c they are included in the request.form
FormData = Request.Form
'Response.Write FormData & "<BR>"
'Response.End
chooseSampleQuantities = Request("chooseSampleQuantities")

'-- Create the new containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CopyContainer.asp", "ChemInv", FormData)
out = trim(httpResponse)
'Response.Write out & "<BR>"

'-- Initialize values array and set first container values
if Action = "split" then
	'-- set the container that was split to the last container in the array
	ReDim arrValues((NumContainers),2)
	arrValues(NumContainers-1,0) = ContainerID
	if chooseSampleQuantities = "1" then
		arrValues(NumContainers-1,1) = eval("Request(""" & Action & "1"")")
	else 
        arrValues(NumContainers-1,1) = Request("OriginalQty") / NumContainers
    end if	
elseif Action = "sample" then
	ReDim arrValues((NumContainers),2)
	arrValues(0,0) = ContainerID
    if chooseSampleQuantities = "1" then
    	arrValues(0,1) = Request("QtyRemaining")
    else 
        arrValues(0,1) = CDbl(Request("OriginalQty")) - (clng(NumContainers) * CDbl(Request("SampleQty")))
    end if
end if

'set remaining container values
if inStr(out, "|") then
	arrContainerIDs = split(out, "|")
	for i = 0 to ubound(arrContainerIDs)
		arrValues(i+1,0) = arrContainerIDs(i)
	    if chooseSampleQuantities = "1" then
    		arrValues(i+1,1) = eval("Request(""" & Action & (i+1) & """)")	
        else
            if Action = "sample" then 
				arrValues(i+1,1) = Request("SampleQty")
			else 	
				arrValues(i+1,1) = Request("OriginalQty") / NumContainers
			end if 	
        end if    		
	next
else
	arrValues(1,0) = out
	if action = "split" then
		arrValues(1,1) = eval("Request(""" & Action & "2"")")
	elseif action = "sample" then
	    if chooseSampleQuantities = "1" then
		    arrValues(1,1) = eval("Request(""" & Action & "1"")")	
		else
		    if Action = "sample" then 
				arrValues(i+1,1) = Request("SampleQty")
			else 	
				arrValues(i+1,1) = Request("OriginalQty") / NumContainers
			end if
		end if
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
Credentials = "&CSUserName=" & Server.URLEncode(Request("CSUserName")) & "&CSUSerID=" & Server.URLEncode(Request("CSUSerID"))
if bSameValue then
	if action = "split" then
		QueryString = "ContainerIDs=" & SplitContainerIDs & _
			"&ValuePairs=" & _
			"Qty_Remaining%3D" & arrValues(0,1) & _
			"::Qty_Initial%3D" & arrValues(0,1) & _
			"::Qty_Max%3D" & ContainerSize & _
			"::Qty_available%3D" & arrValues(0,1) & _
			"::Location_ID_FK%3D" & LocationID & _			
			"::Def_Location_ID_FK%3D" & LocationID & _			
			"::Container_Type_ID_FK%3D" & ContainerTypeID & _
			"::Qty_Minstock%3DNULL" & _
			"::Parent_container_id_fk%3D" & ContainerID 'CSBR 131720, Done by : sjacob 
		if BarcodeDescID <> "" then QueryString = QueryString & "::barcode_desc_id%3D" & BarcodeDescID
		QueryString = QueryString & Credentials
		'Response.Write QueryString
		'response.End
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
			"::Qty_Minstock%3DNULL" 
			if Application("ENABLE_OWNERSHIP")="TRUE" then
			QueryString = QueryString & "::PRINCIPAL_ID_FK%3D" & PrincipalID 
			end if
			QueryString = QueryString & "::Parent_container_id_fk%3D" & arrValues(0,0) & _
			"::Location_ID_FK%3D" & LocationID & _
			"::Def_Location_ID_FK%3D" & LocationID & _
			"::Container_Type_ID_FK%3D" & ContainerTypeID & _
			"::date_certified%3D" & "TO_DATE('" & DateCertified & "','" & Application("DATE_FORMAT_STRING") & "')"
		if not isEmpty(SampleQtyUnit) and SampleQtyUnit <> "" then QueryString = QueryString & "::Unit_of_Meas_ID_FK%3D" & SampleQtyUnit
		if BarcodeDescID <> "" then QueryString = QueryString & "::barcode_desc_id%3D" & BarcodeDescID
		if ContainerStatusID <> "" and not isEmpty(ContainerStatusID) then QueryString = QueryString & "::container_status_id_fk=" & ContainerStatusID
		QueryString = QueryString & Credentials
		'   Response.Write QueryString & "<BR>"
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
    			"::Def_Location_ID_FK%3D" & LocationID & _
				"::Container_Type_ID_FK%3D" & ContainerTypeID
				if Application("ENABLE_OWNERSHIP")="TRUE" then
				QueryString = QueryString & "::PRINCIPAL_ID_FK%3D" & PrincipalID
				End if
			    QueryString = QueryString &	"::date_certified%3D" & "TO_DATE('" & DateCertified & "','" & Application("DATE_FORMAT_STRING") & "')"
			if action = "sample" then 
			    QueryString = QueryString & "::Parent_container_id_fk%3D" & arrValues(0,0)
			'CSBR     : 131720
			'Done by  : sjacob
			'Comments : To make lineage available in the case of split containers 
			'Start of change    
			elseif action = "split" then 
			    QueryString = QueryString & "::Parent_container_id_fk%3D" & ContainerID
			end if
			'End of change    
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


'-- Update Request if RequestID exists
'-- Except when page is called from CreateSamplesFromBatch process
'if mode <> "CreateSamplesFromBatch" then
	if not isEmpty(RequestID) then
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest", adCmdStoredProc)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PSAMPLECONTAINERIDS",advarchar, 1, 2000, out)
	'-- CSBR ID:123488
	'-- Change Done by : Manoj Unnikrishnan
	'-- Purpose: setting the custom fulfill request fields to the parameter
	'-- Date: 08/04/2010
        Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_1",200, adParamInput, 2000, FIELD_1)
        Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_2",200, adParamInput, 2000, FIELD_2)
        Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_3",200, adParamInput, 2000, FIELD_3)
        Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_4",200, adParamInput, 2000, FIELD_4)
        Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_5",200, adParamInput, 2000, FIELD_5)
        Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_1",135, 1, 0, DATE_1)
        Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_2",135, 1, 0, DATE_2)
        if bDebugPrint then
	        For each p in Cmd.Parameters
		        Response.Write p.name & " = " & p.value & "<BR>"
	        Next	
        Else
		    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest")
        End if
	'-- End of Change #123488#
	end if 
'end if

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
