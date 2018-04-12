<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/attribute_defaults.asp"-->
<%
bDebugPrint = false
'****************************************************************************************
'*	PURPOSE: fetch all container attributes for a given container_id and store them
'*			in session variables for use by container view tab interface
'*	INPUT: ContainerId AS Long, GetDbData AS Boolean passed as QueryString parameters
'*	OUTPUT: Populates session variables with container attributes
'****************************************************************************************
Dim Conn
Dim RS
Dim ExpYears

Dim cpdID
Dim NoteBook
Dim Page
Dim RegAmount
Dim RegName
Dim RegSynonym
Dim BASE64_CDX
Dim RegBatchID

ExpYears = Application("ExpDateIncrement")
if ExpYears = "" or isEmpty(ExpYears) then ExpYears = 2

'-- Receive Posted data
ContainerID = Request("ContainerID")
GetData = Request.QueryString("GetData")
AutoGen = Request("AutoGen")
AutoPrint = Request.Form("AutoPrint")
ReturnToSearch = Request.Form("ReturnToSearch")
ReturnToReconcile = Request("ReturnToReconcile")
newRegID = Request("newRegID")
newBatchNumber = Request("newBatchNumber")
newCompoundID = Request("newCompoundID")
sTab = Request.QueryString("TB")

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


if NOT ContainerID = "" then
	Session("ContainerID") = ContainerID
End if

if ContainerID = "" and LCase(GetData)= "db" then
	Response.Redirect "/cheminv/cheminv/SelectContainerMsg.asp"
End if

'-- Keep track of the original location b/c if they go to the browse window for a new location the Session("CurrentLocationID") gets set to 0
Session("OriginalCurrentLocationID") = Session("CurrentLocationID")

'-- If user has selected a rack or contents of rack, set the default location to parent of rack
if APPLICATION("RACKS_ENABLED") then

	'-- Variable to pull up list of Racks in child location
	if RackListLocationID = "" then RackListLocationID = Session("CurrentLocationID")

	'-- ValidateContainerInGrid returns COLLAPSE_CHILD_NODES, PARENT_ID for container
	if ContainerID <> "" then
		RackGridInfo = ValidateContainerInGrid(ContainerID)
	end if

	'Response.Write("@@@" & RackGridInfo & ":" & Session("CurrentLocationID") & "@@@")
	if Session("CurrentLocationID") <> "" then
		'rackTemp = split(ValidateLocationIsRack(Session("CurrentLocationID")),"::")
		'isRack = rackTemp(0)
		'rackParent = rackTemp(1)
		'if Request("ShowFullRackList") = "true" then
		'	Session("ParentRackLocationID") = rackParent
		'else
		'	Session("ParentRackLocationID") = ""
		'end if
		
		'Response.Write("###" & isRack & ":" & rackParent & "###")
		'if isRack = "1" then
		'	Session("CurrentLocationID") = rackParent
		'end if
	end if
end if

Select Case GetData
	Case "db"
		if CBool(Request.QueryString("isEdit")) then
			Session("isEdit")= True
		Else
			Session("isEdit")= false
			if CBool(Request.QueryString("isCopy")) then
				isCopy = true
			Else
				isCopy = false
			End if
		End if
		Call SetContainerSessionVarsFromDb(ContainerID)
	Case "new"
		Session("isEdit")= False
		Call ClearContainerSessionVars()
	Case "session"
		'don't do anything and the local vars get populated by the session vars
	Case Else
		Call SetContainerSessionVarsFromPostedData()
		if Len(newCompoundID)>0 then
			if sTab = "RegSubstance" OR sTab="" then sTab = "Substance"
			Session("RegID") = ""
			Session("BatchNumber") = ""
			Session("RegBatchID") = ""
			ChangeSubstanceAttributesFromDb(newCompoundID)
		elseif Len(newRegID)>0 then
			if sTab = "Substance" OR sTab="" then sTab = "RegSubstance"
			Session("CompoundID") = ""
			GetRegBatchAttributes newRegID, newBatchNumber
		End if
End Select


Sub ClearContainerSessionVars()

	SetDefaultAttributeValues()

	' Set the session variables
	' Required Container attributes
	Session("LocationID") = Session("CurrentLocationID")
	Session("LocationName") = Session("CurrentLocationName")
	Session("ContainerID") = ""
	Session("ContainerName") = ""
	Session("Barcode") = Request("Barcode")
	'Session("BarcodeDescID") = ""
	Session("RegID") = ""
	Session("RegNumber") = ""
	Session("RegBatchID") = ""
	Session("QtyMax") = ""
	Session("QtyInitial") = ""
	Session("Family") = ""

	tempArr = Split(Session("UOMIDOptionValue"),"=")
	Session("UOMAbv")= tempArr(1)
	Session("UOMID") = tempArr(0)
	Session("UOMName") = ""
	tempArr = Split(Session("UOWIDOptionValue"),"=")
	Session("UOWAbv")= tempArr(1)
	Session("UOWID") = tempArr(0)
	Session("UOWName") = ""
	tempArr = Split(Session("UOCIDOptionValue"),"=")
	Session("UOCAbv")= tempArr(1)
	Session("UOCID") = tempArr(0)
	Session("UOCName") = ""
	tempArr = Split(Session("UODIDOptionValue"),"=")
	Session("UODAbv")= tempArr(1)
	Session("UODID") = tempArr(0)
	Session("UODName") = ""
	tempArr = Split(Session("UOPIDOptionValue"),"=")
	Session("UOPAbv")= tempArr(1)
	Session("UOPID") = tempArr(0)
	Session("UOPName") = ""
	Session("NumCopies") = ""
	
	' Optional Container Attributes
	Session("TareWeight") = ""
	Session("NetWeight") = ""
	Session("FinalWeight") = ""
	Session("MinStockQty") = ""
	Session("MaxStockQty") = ""
	Session("ContainerDesc") = ""

	' Contents attributes
	Session("Purity") = ""
	Session("Concentration") = ""
	Session("Density") = ""
	Session("Grade") = ""
	Session("SolventIDFK") = ""
	Session("Comments") = ""
	Session("StorageConditions") = ""
	Session("HandlingProcedures") = ""
	Session("RegID") = ""
	Session("BatchNumber") = ""


	' Calculated Quantities
	Session("QtyRemaining") = ""
	Session("QtyAvaliable") = ""
	Session("DateCreated") = ""
	Session("TotalQtyReserved") = ""

	' Substance attributes
	Session("CompoundID") = ""
	Session("CAS") = ""
	Session("ACX_ID") = ""
	Session("ALT_ID_1") = ""
	Session("ALT_ID_2") = ""
	Session("ALT_ID_3") = ""
	Session("ALT_ID_4") = ""
	Session("ALT_ID_5") = ""
	Session("SubstanceName") = ""
	Session("Base64_CDX") = ""

	' Supplier Attributes
	Session("SupplierName") = ""
	Session("SupplierShortName") = ""
	Session("SupplierCatNum") = ""
	Session("LotNum") = ""
	Session("SupplierCatNum") = ""
	Session("DateProduced") = ""
	Session("DateOrdered") = ""

	Session("ContainerCost") = ""
	tempArr = Split(Session("UOCostIDOptionValue"),"=")
	Session("UOCostAbv")= tempArr(1)
	Session("UOCostID") = tempArr(0)
	Session("UOCostName") = ""
	Session("PONumber") = ""
	Session("POLineNumber") = ""
	Session("ReqNumber") = ""

	Session("CurrentUserID") = Ucase(Session("UserNameChemInv"))
	Session("RequestID") = ""

	' Certification attributes
	Session("DateCertified") = ""
	Session("DateApproved") = ""

	'-- Rack attributes
	Session("RackGridID") = ""

End Sub

Sub ChangeSubstanceAttributesFromDb(pCompoundID)
	Call GetInvConnection()
	SQL = "SELECT inv_compounds.Compound_ID, inv_compounds.CAS, inv_compounds.ACX_ID, inv_compounds.ALT_ID_1, inv_compounds.ALT_ID_2, inv_compounds.ALT_ID_3, inv_compounds.ALT_ID_4, inv_compounds.ALT_ID_5, inv_compounds.Substance_Name, inv_compounds.Base64_CDX FROM inv_Compounds WHERE inv_compounds.Compound_ID=" & pCompoundID
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set RS= Conn.Execute(SQL)
	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:ChangeSubstanceAttributesFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve compound data for Compound_ID " & pCompoundID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if

	' Substance attributes
	Session("CompoundID") = RS("Compound_ID").value
	Session("CAS") = RS("CAS").value
	Session("ACX_ID") = RS("ACX_ID").value
	Session("ALT_ID_1") = RS("ALT_ID_1").value
	Session("ALT_ID_2") = RS("ALT_ID_2").value
	Session("ALT_ID_3") = RS("ALT_ID_3").value
	Session("ALT_ID_4") = RS("ALT_ID_4").value
	Session("ALT_ID_5") = RS("ALT_ID_5").value
	Session("SubstanceName") = RS("Substance_Name").value
	Session("Base64_CDX") = RS("Base64_CDX").value
End Sub


Sub SetContainerSessionVarsFromDb(pContainerID)
	Call GetInvConnection()

%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/ContainerSQL.asp"-->
<%
	SQL = SQL & "AND inv_Containers.Container_ID ="  & ContainerID
	'Response.Write SQL
	'Response.end

	Set RS= Conn.Execute(SQL)
	'Response.write RS.GetString(,,"&nbsp;","<BR>")
	'Response.end
	Set oData = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.4.0")
	RS.Save oData, 1
	Set Session("oViewContainerData") = oData
	Set oData = nothing

	'get Container Request info
	SQL = "SELECT DECODE(inv_requests.date_delivered, NULL, inv_requests.request_id, NULL) AS request_id FROM inv_requests WHERE request_type_id_fk = 1 AND container_id_fk = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
	Set rsRequest = Cmd.Execute

	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetContainerSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve container data for Container_ID " & ContainerID & "</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if

	'-- Set the session variables
	'-- Required Container attributes
	Session("ContainerID") = RS("Container_ID").value
	Session("LocationName") = RS("Location_Name").value
	Session("LocationType") = RS("Location_Type_ID_FK").value
	Session("LocationID") = RS("Location_ID_FK").value
	Session("OrigLocationID") = RS("Location_ID_FK").value

	'-- If Rack Management is enable, assign LocationID to its parent
	'if APPLICATION("RACKS_ENABLED") then
	'	if Session("CurrentLocationID") <> "" and Session("CurrentLocationID") > 0 then
	'		Session("LocationID") = Session("CurrentLocationID")
	'	end if
	'end if

	Session("ContainerName") = RS("Container_Name").value
	if isCopy then
		Session("Barcode") = ""
	Else
		Session("Barcode") = RS("Barcode").value
	End if
    if Application("RegServerName") <> "NULL" then
	Session("RegID") = RS("Reg_ID_FK").value
	Session("BatchNumber") = RS("Batch_Number_FK").value
		Session("RegNumber") = RS("RegNumber").value
	end if
	Session("ContainerTypeID") = RS("Container_Type_ID_FK").value
	Session("ContainerStatusID") = RS("Container_Status_ID_FK").value
	Session("ContainerTypeName") = RS("Container_Type_Name").value
	Session("ContainerStatusName") = RS("Container_Status_Name").value
	Session("QtyMax") = RS("Qty_Max").value
	Session("QtyInitial") = RS("Qty_Initial").value
	Session("UOMName") = RS("UOMName").value
	Session("UOMAbv") = RS("UOMAbv").value
	Session("UOMID") = RS("UOMID").value
	Session("Family") = RS("Family").value

	' Optional Container Attributes
	Session("TareWeight") = RS("Tare_Weight")
	Session("NetWeight") = RS("Net_Weight")
	Session("FinalWeight") = RS("Final_Weight")
	Session("UOWName") = RS("UOWName").value
	Session("UOWAbv") = RS("UOWAbv").value
	Session("UOWID") = RS("UOWID").value
	Session("MinStockQty") = RS("Qty_MinStock").value
	Session("MaxStockQty") = RS("Qty_MaxStock").value
	Session("ContainerDesc") = RS("Container_Description").value
	Session("ParentContainerID") = RS("Parent_container_id")
	Session("ParentContainerBarcode") = RS("Parent_container_barcode")

	' Contents attributes
	Session("Purity") = RS("Purity").value
	Session("UOPName") = RS("UOPName").value
	Session("UOPAbv") = RS("UOPAbv").value
	Session("UOPID") = RS("UOPID").value
	Session("Concentration") = RS("Concentration")
	Session("Density") = RS("Density")
	Session("UOCName") = RS("UOCName").value
	Session("UOCAbv") = RS("UOCAbv").value
	Session("UOCID") = RS("UOCID").value
	Session("UODName") = RS("UODName").value
	Session("UODAbv") = RS("UODAbv").value
	Session("UODID") = RS("UODID").value
	Session("Grade") = RS("Grade")
	Session("SolventIDFK") = RS("Solvent_ID_FK")
	Session("Comments") = RS("Container_Comments")
	Session("StorageConditions") = RS("Storage_Conditions")
	Session("HandlingProcedures") = RS("Handling_Procedures")

	if isCopy then
		Select case CStr(Application("DATE_FORMAT"))
			case "8"
				Session("ExpDate") = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears
			case "9"
				Session("ExpDate") = Day(Now()) & "/" & Month(Now()) & "/" & Year(Now()) + ExpYears
			case "10"
				Session("ExpDate") = Year(Now()) + ExpYears & "/" & Month(Now()) & "/" & Day(Now())
		End Select
	else
	Session("ExpDate") = RS("Date_Expires")
	End if
	' Calculated Quantities
	Session("QtyRemaining") = RS("Qty_Remaining").value
	Session("QtyAvailable") = RS("Qty_Available").value
	Session("DateCreated") = RS("Date_Created")
	Session("TotalQtyReserved") = RS("TotalQtyReserved").value


	' Substance attributes
	Session("CompoundID") = RS("Compound_ID_FK").value
	Session("CAS") = RS("CAS").value
	Session("ACX_ID") = RS("ACX_ID").value
	Session("ALT_ID_1") = RS("ALT_ID_1").value
	Session("ALT_ID_2") = RS("ALT_ID_2").value
	Session("ALT_ID_3") = RS("ALT_ID_3").value
	Session("ALT_ID_4") = RS("ALT_ID_4").value
	Session("ALT_ID_5") = RS("ALT_ID_5").value
	Session("SubstanceName") = RS("Substance_Name").value
	Session("Base64_CDX") = RS("Base64_CDX").value

	' Supplier Attributes
	Session("SupplierID") = RS("Supplier_ID_FK").value
	If Len(Session("SupplierID")) = 0 then Session("SupplierID") = "0"
	Session("SupplierShortName") = RS("Supplier_Short_Name").value
	Session("SupplierName") = RS("Supplier_Name").value
	Session("SupplierCatNum") = RS("Supplier_CatNum").value
	Session("LotNum") = RS("LOT_NUM").value
	Session("DateProduced") = RS("DATE_PRODUCED").value
	Session("DateOrdered") = RS("DATE_ORDERED").value
	if isCopy then
		Session("DateReceived") = Today
	else
		Session("DateReceived") = RS("DATE_RECEIVED").value
	End if
	Session("ContainerCost") = RS("CONTAINER_COST").value
	Session("UOCostID") = RS("UOCostID").value
	Session("UOCostName") = RS("UOCostName").value
	Session("UOCostAbv") = RS("UOCostAbv").value
	Session("PONumber") = RS("PO_NUMBER").value
	Session("POLineNumber") = RS("PO_Line_NUMBER").value
	Session("ReqNumber") = RS("REQ_NUMBER").value
	Session("SupplierCatNum") = RS("SUPPLIER_CATNUM").value

	' User attributes
	Session("OwnerID") = RS("Owner_ID_FK").value
	Session("CurrentUserID") = RS("Current_User_ID_FK").value
	if rsRequest.BOF or rsRequest.EOF THEN
		Session("RequestID") = null
	else
		Session("RequestID") = rsRequest("Request_ID").value
	end if

	' Certification attributes
	Session("DateCertified") = RS("DATE_CERTIFIED").value
	Session("DateApproved") = RS("DATE_APPROVED").value

	'Custom attributes
	Session("Field_1") = RS("Field_1").value
	Session("Field_2") = RS("Field_2").value
	Session("Field_3") = RS("Field_3").value
	Session("Field_4") = RS("Field_4").value
	Session("Field_5") = RS("Field_5").value
	Session("Field_6") = RS("Field_6").value
	Session("Field_7") = RS("Field_7").value
	Session("Field_8") = RS("Field_8").value
	Session("Field_9") = RS("Field_9").value
	Session("Field_10") = RS("Field_10").value
	Session("Date_1") = RS("Date_1").value
	Session("Date_2") = RS("Date_2").value
	Session("Date_3") = RS("Date_3").value
	Session("Date_4") = RS("Date_4").value
	Session("Date_5") = RS("Date_5").value

	Session("BatchID") = RS("Batch_ID_FK").value

	'-- Rack attributes
	Session("RackGridID") = RS("Location_ID_FK").value

	if Session("RegID") <> "" then
		GetRegBatchAttributes Session("RegID"), Session("BatchNumber")
	else
		'-- Clear Session Reg Attributes
		for each key in reg_fields_dict
			Session(key) = ""
		next
	End if

End sub

Sub SetContainerSessionVarsFromPostedData()
	' Receive Posted data
	iNumCopies = Request.Form("iNumCopies")
	'LocationID = Request.QueryString("LocationID")
	LocationID = Request.Form("iLocationID")
	iLocationID = Request.Form("iLocationID")
	OrigLocationID = Request.Form("OrigLocationID")
	LocationName = Request.QueryString("LocationName")
	LocationType = Request.QueryString("LocationType")
	iContainerID = Request.Form("iContainerID")
	iContainerTypeID = Request.Form("iContainerTypeID")
	iContainerStatusID = Request.Form("iContainerStatusID")
	iUOMID =  Request.Form("iUOMID")
	iUOWID = Request.Form("iUOWID")
	iUOPID = Request.Form("iUOPID")
	iUOCID = Request.Form("iUOCID")
	iUODID = Request.Form("iUODID")
	iUOCostID = Request.Form("iUOCostID")
	iQtyMax =  Request.Form("iQtyMax")
	iQtyRemaining =  Request.Form("iQtyRemaining")
	iQtyInitial =  Request.Form("iQtyInitial")
	iContainerName = Request.Form("iContainerName")
	iBarcode = Request("iBarcode")
	iBarcodeDescID = Request.Form("iBarcodeDescID")
	iRegID = Request.Form("iRegID")
	iBatchNumber = Request.Form("iBatchNumber")
	iContainerDesc = Request.Form("iContainerDesc")
	iMinStockQty = Request.Form("iMinStockQty")
	iMaxStockQty = Request.Form("iMaxStockQty")
	iExpDate =  Request.Form("iExpDate")
	iDateCertified = Request.Form("iDateCertified")
	iDateApproved = Request.Form("iDateApproved")
	iCompoundID = Request.Form("iCompoundID")
	iTareWeight = Request.Form("iTareWeight")
	iNetWeight = Request.Form("iNetWeight")
	iFinalWeight = Request.Form("iFinalWeight")
	iPurity = Request.Form("iPurity")
	iConcentration = Request.Form("iConcentration")
	iDensity = Request.Form("iDensity")
	iGrade = Request.Form("iGrade")
	iSolventIDFK = Request.Form("iSolventIDFK")
	iComments = Request.Form("iComments")
	iStorageConditions = Request.Form("iStorageConditions")
	iHandlingProcedures = Request.Form("iHandlingProcedures")
	iSupplierID = Request.Form("iSupplierID")
	iSupplierName = Request.Form("iSupplierName")
	iSupplierShortName = Request.Form("iSupplierShortName")
	iLotNum = Request.Form("iLotNum")
	iDateProduced = Request.Form("iDateProduced")
	iDateOrdered = Request.Form("iDateOrdered")
	iDateReceived = Request.Form("iDateReceived")
	iContainerCost = Request.Form("iContainerCost")
	iPONumber = Request.Form("iPONumber")
	iPOLineNumber = Request.Form("iPOLineNumber")
	iReqNumber = Request.Form("iReqNumber")
	iSupplierCatNum = Request.Form("iSupplierCatNum")
	iOwnerID = Request.Form("iOwnerID")
	iCurrentUserID = Request.Form("iCurrentUserID")
	iField_1 = Request.Form("iField_1")
	iField_2 = Request.Form("iField_2")
	iField_3 = Request.Form("iField_3")
	iField_4 = Request.Form("iField_4")
	iField_5 = Request.Form("iField_5")
	iField_6 = Request.Form("iField_6")
	iField_7 = Request.Form("iField_7")
	iField_8 = Request.Form("iField_8")
	iField_9 = Request.Form("iField_9")
	iField_10 = Request.Form("iField_10")
	iDate_1 = Request.Form("iDate_1")
	iDate_2 = Request.Form("iDate_2")
	iDate_3 = Request.Form("iDate_3")
	iDate_4 = Request.Form("iDate_4")
	iDate_5 = Request.Form("iDate_5")

	'-- Rack attributes
	iRackGridID = Request.Form("iRackGridID")
	if not IsEmpty(RackGridID) then Session("RackGridID") = RackGridID

	' Set session variables to store posted data
	if Not IsEmpty(LocationID) then Session("LocationID")= LocationID
	if Not IsEmpty(OrigLocationID) then Session("OrigLocationID")= OrigLocationID

	if Not IsEmpty(LocationName) then Session("LocationName")= LocationName
	if Not IsEmpty(LocationType) then Session("LocationType")= LocationType
	if Not IsEmpty(iContainerID)then  Session("ContainerID")= iContainerID
	if Not IsEmpty(iLocationID)then  Session("LocationID")= iLocationID
	if Not IsEmpty(iContainerTypeID)then  Session("ContainerTypeID")= iContainerTypeID
	if Not IsEmpty(iContainerStatusID)then  Session("ContainerStatusID")= iContainerStatusID
	if Not IsEmpty(iUOMID) then
		tempArr = Split(iUOMID,"=")
		Session("UOMIDOptionValue")= iUOMID
		Session("UOMAbv")= tempArr(1)
		Session("UOMID") = tempArr(0)
	End if
	if Not IsEmpty(iUOWID) then
		tempArr = Split(iUOWID,"=")
		Session("UOWIDOptionValue")= iUOWID
		Session("UOWAbv")= tempArr(1)
		Session("UOWID") = tempArr(0)
	End if
	if Not IsEmpty(iUOCID) then
		tempArr = Split(iUOCID,"=")
		Session("UOCIDOptionValue")= iUOCID
		Session("UOCAbv")= tempArr(1)
		Session("UOCID") = tempArr(0)
	End if
	if Not IsEmpty(iUODID) then
		tempArr = Split(iUODID,"=")
		Session("UODIDOptionValue")= iUODID
		Session("UODAbv")= tempArr(1)
		Session("UODID") = tempArr(0)
	End if
	if Not IsEmpty(iUOPID) then
		tempArr = Split(iUOPID,"=")
		Session("UOPIDOptionValue")= iUOPID
		Session("UOPAbv")= tempArr(1)
		Session("UOPID") = tempArr(0)
	End if
	if Not IsEmpty(iUOCostID) then 
		tempArr = Split(iUOCostID,"=")
		Session("UOCostIDOptionValue")= iUOCostID
		Session("UOCostAbv")= tempArr(1)
		Session("UOCostID") = tempArr(0) 
	End if
	if Not IsEmpty(iNumCopies)then Session("NumCopies")= iNumCopies
	if Not IsEmpty(iQtyMax)then Session("QtyMax")= iQtyMax
	if Not IsEmpty(iQtyInitial)then Session("QtyInitial")= iQtyInitial
	if Not IsEmpty(iQtyRemaining)then Session("QtyRemaining")= iQtyRemaining
	if Not IsEmpty(iContainerName) then Session("ContainerName")= iContainerName
	if Not IsEmpty(iBarcode) then Session("Barcode")= iBarcode
	if Not IsEmpty(iBarcodeDescID) then Session("BarcodeDescID") = iBarcodeDescID
	if Not IsEmpty(iRegID) then Session("RegID")= iRegID
	if Not IsEmpty(iBatchNumber) then Session("BatchNumber")= iBatchNumber
	if Request.Form("iContainerDesc").Count > 0 then Session("ContainerDesc")= iContainerDesc
	if Request.Form("iComments").Count > 0 then Session("Comments")= iComments
	if Request.Form("iStorageConditions").Count > 0 then Session("StorageConditions") = iStorageConditions
	if Request.Form("iHandlingProcedures").Count > 0 then Session("HandlingProcedures") = iHandlingProcedures
	if Not IsEmpty(iMinStockQty) then Session("MinStockQty")= iMinStockQty
	if Not IsEmpty(iMaxStockQty) then Session("MaxStockQty")= iMaxStockQty
	if Request.Form("iExpDate").Count > 0 then Session("ExpDate")= iExpDate
	if Not IsEmpty(iDateCertified) then Session("DateCertified")= iDateCertified
	if Not IsEmpty(iDateApproved) then Session("DateApproved")= iDateApproved
	if Not IsEmpty(iCompoundID) then Session("CompoundID")= iCompoundID
	if Not IsEmpty(iTareWeight) then Session("TareWeight")= iTareWeight
	if Not IsEmpty(iNetWeight) then Session("NetWeight")= iNetWeight
	if Not IsEmpty(iFinalWeight) then Session("FinalWeight")= iFinalWeight
	if Not IsEmpty(iPurity) then Session("Purity")= iPurity
	if Not IsEmpty(iConcentration) then Session("Concentration")= iConcentration
	if Not IsEmpty(iDensity) then Session("Density")= iDensity
	if Not IsEmpty(iGrade) then Session("Grade")= iGrade
	if Not IsEmpty(iSolventIDFK) then Session("SolventIDFK")= iSolventIDFK
	if Not IsEmpty(iSupplierID) then Session("SupplierID")= iSupplierID
	if Not IsEmpty(iSupplierName) then Session("SupplierName")= iSupplierName
	if Not IsEmpty(iSupplierShortName) then Session("SupplierShortName")= iSupplierShortName
	if Request.Form("iLotNum").Count > 0 then Session("LotNum")= iLotNum
	if Request.Form("iDateProduced").Count > 0 then Session("DateProduced")= iDateProduced
	if Request.Form("iDateOrdered").Count > 0 then Session("DateOrdered")= iDateOrdered
	if Request.Form("iDateReceived").Count > 0 then Session("DateReceived")= iDateReceived
	if Not IsEmpty(iContainerCost) then Session("ContainerCost")= iContainerCost
	if Request.Form("iPONumber").Count > 0 then Session("PONumber")= iPONumber
	if Request.Form("iPOLineNumber").Count > 0 then Session("POLineNumber")= iPOLineNumber
	if Request.Form("iReqNumber").Count > 0 then Session("ReqNumber")= iReqNumber
	if Request.Form("iSupplierCatNum").Count > 0 then Session("SupplierCatNum")= iSupplierCatNum
	if Not IsEmpty(iOwnerID) then Session("OwnerID")= iOwnerID
	if Not IsEmpty(iCurrentUserID) then Session("CurrentUserID")= iCurrentUserID
	if Request.Form("iField_1").Count > 0 then Session("Field_1")= iField_1
	if Request.Form("iField_2").Count > 0 then Session("Field_2")= iField_2
	if Request.Form("iField_3").Count > 0 then Session("Field_3")= iField_3
	if Request.Form("iField_4").Count > 0 then Session("Field_4")= iField_4
	if Request.Form("iField_5").Count > 0 then Session("Field_5")= iField_5
	if Request.Form("iField_6").Count > 0 then Session("Field_6")= iField_6
	if Request.Form("iField_7").Count > 0 then Session("Field_7")= iField_7
	if Request.Form("iField_8").Count > 0 then Session("Field_8")= iField_8
	if Request.Form("iField_9").Count > 0 then Session("Field_9")= iField_9
	if Request.Form("iField_10").Count > 0 then Session("Field_10")= iField_10
	if Request.Form("iDate_1").Count then Session("Date_1")= iDate_1
	if Request.Form("iDate_2").Count > 0 then Session("Date_2")= iDate_2
	if Request.Form("iDate_3").Count > 0 then Session("Date_3")= iDate_3
	if Request.Form("iDate_4").Count > 0 then Session("Date_4")= iDate_4
	if Request.Form("iDate_5").Count > 0 then Session("Date_5")= iDate_5

End Sub

	'-- Set local variables
	isEdit = Session("isEdit")
	ContainerID = Session("ContainerID")
	LocationName = Session("LocationName")
	LocationType = Session("LocationType")
	LocationID = Session("LocationID")
	OrigLocationID = Session("OrigLocationID")
	ContainerName = Session("ContainerName")
	Barcode = Session("Barcode")
	BarcodeDescID = Session("BarcodeDescID")
	ContainerTypeID = Session("ContainerTypeID")
	ContainerStatusID = Session("ContainerStatusID")
	ContainerTypeName = Session("ContainerTypeName")
	ContainerStatusName = Session("ContainerStatusName")
	NumCopies = Session("NumCopies")
	QtyMax = Session("QtyMax")
	QtyInitial = Session("QtyInitial")
	UOMName = Session("UOMName")
	UOMAbv = Session("UOMAbv")
	UOMID = Session("UOMID")
	Session("UOMIDOptionValue")= UOMID & "=" & UOMAbv

	' Optional Container Attributes
	TareWeight = Session("TareWeight")
	NetWeight = Session("NetWeight")
	FinalWeight = Session("FinalWeight")
	UOWName = Session("UOWName")
	UOWAbv = Session("UOWAbv")
	UOWID = Session("UOWID")
	Session("UOWIDOptionValue") = UOWID & "=" & UOWAbv
	MinStockQty = Session("MinStockQty")
	MaxStockQty = Session("MaxStockQty")
	ContainerDesc = Session("ContainerDesc")
	ParentContainerID = Session("ParentContainerID")
	ParentContainerBarcode = Session("ParentContainerBarcode")
	Family = Session("Family")

	' Contents attributes
	Purity = Session("Purity")
	UOPName = Session("UOPName")
	UOPAbv = Session("UOPAbv")
	UOPID = Session("UOPID")
	Session("UOPIDOptionValue")= UOPID & "=" & UOPAbv
	Concentration = Session("Concentration")
	Density = Session("Density")
	UOCName = Session("UOCName")
	UOCAbv = Session("UOCAbv")
	UOCID = Session("UOCID")
	UODName = Session("UODName")
	UODAbv = Session("UODAbv")
	UODID = Session("UODID")
	Session("UOCIDOptionValue") = UOCID & "=" & UOCAbv
	Session("UODIDOptionValue") = UODID & "=" & UODAbv
	Grade = Session("Grade")
	SolventIDFK = Session("SolventIDFK")
	Comments = Session("Comments")
	StorageConditions = Session("StorageConditions")
	HandlingProcedures = Session("HandlingProcedures")
	RegID = Session("RegID")
	BatchNumber = Session("BatchNumber")
	RegNumber = Session("RegNumber")
	ExpDate = Session("ExpDate")

	' Calculated Quantities
	QtyRemaining = Session("QtyRemaining")
	QtyAvailable = Session("QtyAvailable")
	DateCreated = Session("DateCreated")
	TotalQtyReserved = Session("TotalQtyReserved")

	' Substance attributes
	CompoundID = Session("CompoundID")
	CAS = Session("CAS")
	ACX_ID = Session("ACX_ID")
	ALT_ID_1 = Session("ALT_ID_1")
	ALT_ID_2 = Session("ALT_ID_2")
	ALT_ID_3 = Session("ALT_ID_3")
	ALT_ID_4 = Session("ALT_ID_4")
	ALT_ID_5 = Session("ALT_ID_5")
	SubstanceName = Session("SubstanceName")
	Base64_CDX = Session("Base64_CDX")

	' Supplier Attributes
	SupplierID = Session("SupplierID")
	SupplierShortName = Session("SupplierShortName")
	SupplierName = Session("SupplierName")
	LotNum = Session("LotNum")
	DateProduced = Session("DateProduced")
	DateOrdered = Session("DateOrdered")
	DateReceived = Session("DateReceived")
	ContainerCost = Session("ContainerCost")
	UOCostID = Session("UOCostID")
	UOCostName = Session("UOCostName")
	UOCostAbv = Session("UOCostAbv")
	PONumber = Session("PONumber")
	POLineNumber = Session("POLineNumber")
	ReqNumber = Session("ReqNumber")
	SupplierCatNum = Session("SupplierCatNum")

	'User attributes
	OwnerID = Session("OwnerID")
	CurrentUserID = Session("CurrentUserID")
	RequestId = Session("RequestID")
	'If (RequestID = "" or IsEmpty(RequestID)) and Session("sTab") = "Requests" then Session("sTab") = ""

	' Certification attributes
	DateCertified = Session("DateCertified")
	DateApproved = Session("DateApproved")

	'Custom Attributes
	Field_1 = Session("Field_1")
	Field_2 = Session("Field_2")
    Field_3 = Session("Field_3")
	Field_4 = Session("Field_4")
	Field_5 = Session("Field_5")
	Field_6 = Session("Field_6")
	Field_7 = Session("Field_7")
	Field_8 = Session("Field_8")
	Field_9 = Session("Field_9")
	Field_10 = Session("Field_10")
	Date_1 = Session("Date_1")
	Date_2 = Session("Date_2")
	Date_3 = Session("Date_3")
	Date_4 = Session("Date_4")
	Date_5 = Session("Date_5")


	BatchID = Session("BatchID")
	for each key in reg_fields_dict
		'Response.Write(reg_fields_dict.item(key) & " : " & key & "<br>")
		execute(key & " = Session(""" & key & """)")
	next

	Session("CurrentLocationID") = Session("OriginalCurrentLocationID")
	'Session("CurrentContainerName") = LocationName
	Session("CurrentContainerID") = ContainerID

	'-- Rack attributes
	RackGridID = Session("RackGridID")

%>
