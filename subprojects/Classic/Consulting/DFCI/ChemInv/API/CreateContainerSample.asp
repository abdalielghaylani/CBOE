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
Dim out
Dim out2


Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:CreateContainerSample<br>"

'-- Required Parameters
ContainerName = Request("ContainerName")
ContainerID = Request("ContainerID")
NumContainers = Request("NumContainers")
LocationID = Request("LocationID")
ContainerTypeID = Request("ContainerTypeID")
Action = Request("Action")
ContainerSize = Request("ContainerSize")

'-- Optional Parameters
SampleQtyUnit = Request("SampleQtyUnit")
BarcodeDescID = Request("BarcodeDescID")
DateCertified = Request("DateCertified")
ContainerStatusID = Request("ContainerStatusID")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateContainerSample.htm"
	Response.end
End if

'-- Check for required parameters
If IsEmpty(ContainerID) and Action <> "new" then
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
If bWriteError then
	Response.Write strError
	Response.end
End if

bSameValue = true

ServerName = Request.ServerVariables("Server_Name")
FormData = Request.Form
'Response.Write(FormData & "<br>")
'Response.End

arrNumCopies = split(Request("iNumCopies"),",")
if Request("iBarcode") <> "" then
	arrBarcode = split(Request("iBarcode"),",")
end if
arrSampleQty = split(Request("iSampleQty"),",")
arrContainerTypeID = split(Request("iContainerTypeID"),",")
arrContainerSize = split(Request("iContainerSize"),",")
if Action = "new" then
	arrUOM = split(Request("iUOM"),",")
end if
arrUOC = split(Request("iUOC"),",")
arrConc = split(iif(Request("iConc")=""," ",Request("iConc")),",")

out = ""
out2 = ""

'-- Set counter to keep track of Rack ID assignment
if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then 
	cntRackID = 0
	arrRackGridList = split(iif(Request("RackGridList")=""," ",Request("RackGridList")),",")
end if

'-- Loop through iNumCopies array and copy/update containers in each group
for i = 0 to ubound(arrNumCopies)

	FormData = Replace(Request.Form,"NumContainers=" & NumContainers,"NumContainers=" & arrNumCopies(i))
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CopyContainer.asp", "ChemInv", FormData)
	out = trim(httpResponse)

	'-- initialize values array and set first container values
	ReDim arrValues((arrNumCopies(i)))
	arrValues(0) = ContainerID

	'-- set remaining container values
	if inStr(out, "|") then
		arrContainerIDs = split(out, "|")
		for j = 0 to ubound(arrContainerIDs)
			arrValues(j+1) = arrContainerIDs(j)
		next
	else
		arrValues(1) = out
	end if

	Credentials = "&CSUserName=" & Request("CSUserName") & "&CSUSerID=" & Request("CSUSerID")

	'-- Update copied containers with form values
	for k = 0 to ubound(arrValues)
		if Application("RACKS_ENABLED") and Request("RackGridList") <> "" and Request("AssignToRack") = "on" then 
			LocationID = arrRackGridList(cntRackID)
		else
			LocationID = LocationID
		end if
		if k = 0 then
			QueryString = "ContainerIDs=" & arrValues(k) & _
				"&ValuePairs=" & _
				"Qty_Remaining%3D" & Trim(Request("QtyRemaining")) & _
				"::Qty_available%3D" & Trim(Request("QtyRemaining"))
		else
			cntRackID = cntRackID + 1
			QueryString = "ContainerIDs=" & arrValues(k) &_ 
				"&ValuePairs=" & _
				"Qty_Remaining%3D" & Trim(arrSampleQty(i)) & _
				"::Qty_Initial%3D" & Trim(arrSampleQty(i)) & _
				"::Qty_Max%3D" & Trim(arrContainerSize(i)) & _
				"::Qty_available%3D" & Trim(arrSampleQty(i)) & _
				"::Qty_Minstock%3DNULL" & _
				"::Location_ID_FK%3D" & LocationID & _
				"::Concentration%3D" & Trim(iif(len(trim(arrConc(i)))=0,"NULL",arrConc(i))) & _
				"::Container_Type_ID_FK%3D" & arrContainerTypeID(i)
			if ContainerID <> "" then
				QueryString = QueryString & "::Parent_Container_ID_FK%3D" & ContainerID
			end if
			if Action = "new" then
				if arrUOM(i) <> "" then 
					arrUOMTemp = split(arrUOM(i),"=")
					QueryString = QueryString & "::Unit_of_Meas_ID_FK%3D" & arrUOMTemp(0)
				end if
			end if
			if arrUOC(i) <> "" then 
				arrUOCTemp = split(arrUOC(i),"=")
				QueryString = QueryString & "::Unit_of_Conc_ID_FK%3D" & arrUOCTemp(0)
			end if
			if Request("iBarcode") <> "" then
				if arrBarcode(i) <> "" then
					QueryString = QueryString & "::Barcode%3D'" & Trim(arrBarcode(i)) & "'"
				end if
			end if
			if BarcodeDescID <> "" and Request("iBarcode") = "" then QueryString = QueryString & "::barcode_desc_id%3D" & Trim(BarcodeDescID)
		end if
		QueryString = QueryString & Credentials
		logAction(QueryString)
		httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainer2.asp", "ChemInv", QueryString)
		if k > 0 then out2 = out2 & arrValues(k) & "|"
		
	next
next

Response.ContentType = "Text/Plain"
Response.Write out2
'Response.end

Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub



</SCRIPT>
