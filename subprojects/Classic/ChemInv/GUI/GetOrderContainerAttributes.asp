<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/OrderContainerSQL.asp"-->
<%
'****************************************************************************************
'*	PURPOSE: fetch all container attributes for a given container_id and store them  
'*			in session variables for use by container view tab interface			 	                            
'*	INPUT: ContainerId AS Long, GetDbData AS Boolean passed as QueryString parameters  	                    			
'*	OUTPUT: Populates session variables with container attributes										
'****************************************************************************************
Dim Conn
Dim RS
Dim ExpYears

Dim RegID
Dim NoteBook
Dim Page
Dim RegAmount
Dim RegName
Dim BASE64_CDX
Dim RegBatchID


ExpYears = 2

' Receive Posted data
ContainerID = Request("ContainerID")
GetData = Request.QueryString("GetData")
AutoGen = Request.Form("AutoGen")
AutoPrint = Request.Form("AutoPrint")
ReturnToSearch = Request.Form("ReturnToSearch")
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

'MWS: Attributes for order entry
RushOrder = Request.Form("RushOrder")
If RushOrder <> "" then 
	Session("RushOrder") = RushOrder
Else	
	RushOrder = Session("RushOrder")
End if


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
	'set the session variables
	' Required Container attributes
	Session("LocationID") = "1" 'Session("CurrentLocationID")
	Session("LocationName") = "On Order" 'Session("CurrentLocationName")
	Session("ContainerID") = ""
	Session("ContainerName") = "" 
	Session("Barcode") = ""
	Session("RegID") = ""
	Session("RegNumber") = ""
	Session("ContainerTypeID") = ""
	Session("ContainerStatusID") = ""
	Session("QtyMax") = ""
	Session("QtyInitial") = ""
	Session("NumCopies") = "1"
	Session("RegBatchID")=""
	'CBOE-798 SJ Clearing the session values
    if Application("ENABLE_OWNERSHIP")="TRUE" then
	    Session("PrincipalID")=""
	    Session("LocationTypeID")=""
	end if
	
	'Pick List defaults
	Session("UOMIDOptionValue")= "5=g"
	Session("UOWIDOptionValue")= "5=g"
	Session("UOCIDOptionValue")= "14=mmol"
	Session("UOPIDOptionValue")= "12=%"
	Session("ContainerTypeID")= "1"
	Session("ContainerStatusID")= "1"
	Session("SupplierID") = "-1"	'No default Supplier
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
	tempArr = Split(Session("UOPIDOptionValue"),"=")
	Session("UOPAbv")= tempArr(1)
	Session("UOPID") = tempArr(0)
	Session("UOPName") = ""
	
	
	' Optional Container Attributes
	Session("TareWeight") = ""
	Session("MinStockQty") = ""
	Session("MaxStockQty") = ""
	Session("ContainerDesc") = ""
	
	' Contents attributes
	Session("Purity") = ""
	Session("Concentration") = ""
	Session("Grade") = ""
	Session("Solvent") = ""
	Session("Comments") = ""
	Session("RegID") = ""
	Session("BatchNumber") = ""
	tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears 
	Session("ExpDate") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
	
	' Calculated Quantities
	Session("QtyRemaining") = ""
	Session("QtyAvaliable") = ""
	Session("DateCreated") = ""
	
	' Substance attributes
	Session("CompoundID") = ""
	Session("CAS") = ""
	Session("ACX_ID") = ""
	Session("SubstanceName") = ""
	Session("Base64_CDX") = ""
	
	' Supplier Attributes
	Session("SupplierName") = ""
	Session("SupplierShortName") = ""
	Session("SupplierCatNum") = ""
	Session("LotNum") = ""
	Session("SupplierCatNum") = ""
	Session("DateProduced") = ""
	Session("DateReceived") = ""
	tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
	Session("DateOrdered") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)


	Session("ContainerCost") = ""
	Session("PONumber") = ""
	Session("ReqNumber") = ""
	' User attributes
	Session("OwnerID") = ""
	Session("CurrentUserID") = Ucase(Session("UserNameChemInv"))
	
	'MWS: Attributes for order entry
	'tempDate = Month(Now() + 1) & "/" & Day(Now() + 2) & "/" & Year(Now() + 1)
	Session("DueDate")  = GetLocalNowPlusDate(Application("DEFAULT_CONTAINER_ORDER_DUE_DAYS"),0,0)

	Session("Project") = GetUserProperty(Session("UserNameCheminv"), "Project")
	Session("Job") = GetUserProperty(Session("UserNameCheminv"), "Job")
	Session("DeliveryLocationID") = Session("CurrentLocationID")
	Session("UnknownSupplierName") = ""
	Session("UnknownSupplierContact") = ""
	Session("UnknownSupplierPhoneNumber") = ""
	Session("UnknownSupplierFAXNumber") = ""

	'MCD: Added for 'Order Reason'
	Session("Hassle") = 0
	Session("OrderReason") = ""
	Session("OrderReasonOther") = ""
	'MCD: end changes
End Sub

Sub ChangeSubstanceAttributesFromDb(pCompoundID)
	Call GetInvConnection()
	SQL = "SELECT inv_compounds.Compound_ID, inv_compounds.CAS, inv_compounds.ACX_ID, inv_compounds.Substance_Name, inv_compounds.Base64_CDX FROM inv_Compounds WHERE inv_compounds.Compound_ID=" & pCompoundID
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
	Session("SubstanceName") = RS("Substance_Name").value
	Session("Base64_CDX") = RS("Base64_CDX").value
End Sub


Sub SetContainerSessionVarsFromDb(pContainerID)
	Call GetInvConnection()
	
	SQL = SQL & "AND inv_Containers.Container_ID ="  & ContainerID
	
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	'Response.write RS.GetString(,,"&nbsp;","<BR>")
	'Response.end
	
	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetContainerSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve container data for Container_ID " & ContainerID & " or User may not have permission(s) to view the container information.</SPAN>"
		Response.Write "</td></tr></table>"
		Response.end
	end if
	
	'set the session variables
	' Required Container attributes
	Session("ContainerID") = RS("Container_ID").value
	Session("LocationName") = "On Order" 'RS("Location_Name").value
	Session("LocationID") = "1" 'RS("Location_ID_FK").value
	Session("ContainerName") = RS("Container_Name").value
	if isCopy then
		Session("Barcode") = ""
	Else
		Session("Barcode") = RS("Barcode").value
	End if
	Session("RegID") = RS("Reg_ID_FK").value
	Session("BatchNumber") = RS("Batch_Number_FK").value
	Session("ContainerTypeID") = RS("Container_Type_ID_FK").value
	Session("ContainerStatusID") = RS("Container_Status_ID_FK").value
	Session("ContainerTypeName") = RS("Container_Type_Name").value
	Session("ContainerStatusName") = RS("Container_Status_Name").value
	Session("QtyMax") = RS("Qty_Max").value
	Session("QtyInitial") = RS("Qty_Initial").value
	Session("UOMName") = RS("UOMName").value
	Session("UOMAbv") = RS("UOMAbv").value
	Session("UOMID") = RS("UOMID").value
		
	' Optional Container Attributes
	Session("TareWeight") = RS("Tare_Weight")
	Session("UOWName") = RS("UOWName").value
	Session("UOWAbv") = RS("UOWAbv").value
	Session("UOWID") = RS("UOWID").value
	Session("MinStockQty") = RS("Qty_MinStock").value
	Session("MaxStockQty") = RS("Qty_MaxStock").value
	Session("ContainerDesc") = RS("Container_Description").value
	
	' Contents attributes
	Session("Purity") = RS("Purity").value
	Session("UOPName") = RS("UOPName").value
	Session("UOPAbv") = RS("UOPAbv").value
	Session("UOPID") = RS("UOPID").value
	Session("Concentration") = RS("Concentration")
	Session("UOCName") = RS("UOCName").value
	Session("UOCAbv") = RS("UOCAbv").value
	Session("UOCID") = RS("UOCID").value
	Session("Grade") = RS("Grade")
	Session("Solvent") = RS("Solvent")
	Session("Comments") = RS("Container_Comments")
	
	if isCopy then
		tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears
		Session("ExpDate") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
	else
	Session("ExpDate") = RS("Date_Expires")
	End if
	' Calculated Quantities
	Session("QtyRemaining") = RS("Qty_Remaining").value
	Session("QtyAvailable") = RS("Qty_Available").value
	Session("DateCreated") = RS("Date_Created")
	
	' Substance attributes
	Session("CompoundID") = RS("Compound_ID_FK").value
	Session("CAS") = RS("CAS").value
	Session("ACX_ID") = RS("ACX_ID").value
	Session("SubstanceName") = RS("Substance_Name").value
	Session("Base64_CDX") = RS("Base64_CDX").value
	
	' Supplier Attributes
	Session("SupplierID") = RS("Supplier_ID_FK").value
	If Len(Session("SupplierID")) = 0 then Session("SupplierID") = "-1"	'No default Supplier
	Session("SupplierShortName") = RS("Supplier_Short_Name").value
	Session("SupplierName") = RS("Supplier_Name").value
	Session("SupplierCatNum") = RS("Supplier_Name").value
	Session("LotNum") = RS("LOT_NUM").value
	Session("DateProduced") = RS("DATE_PRODUCED").value
	Session("DateOrdered") = RS("DATE_ORDERED").value
	if isCopy then
		tempDate = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
		Session("DateReceived") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
	else
		Session("DateReceived") = RS("DATE_RECEIVED").value
	End if
	Session("ContainerCost") = RS("CONTAINER_COST").value
	Session("PONumber") = RS("PO_NUMBER").value
	Session("ReqNumber") = RS("REQ_NUMBER").value
	Session("SupplierCatNum") = RS("SUPPLIER_CATNUM").value
	
	' User attributes
	Session("OwnerID") = RS("Owner_ID_FK").value
	Session("CurrentUserID") = RS("Current_User_ID_FK").value
	
	if Session("RegID") <> "" then
		GetRegBatchAttributes Session("RegID"), Session("BatchNumber")
	else
		'Reg Attributes
		Session("RegID") = ""
		Session("NoteBook") = ""
		Session("Page") = ""
		Session("RegAmount") = ""
		Session("RegPurity") = ""
		Session("RegName") = ""
		Session("BASE64_CDX") =  ""
		Session("RegBatchID") = ""
		Session("RegScientist") = ""
	End if
	
	'MWS: Attributes for order entry	
	tempDate = Month(Now() + 1) & "/" & Day(Now() + 2) & "/" & Year(Now() + 1)
	Session("DueDate") = ConvertDateToStr(Application("DATE_FORMAT"), tempDate)
	
	Session("Project") = ""
	Session("Job") = ""
	Session("DeliveryLocationID") = ""
	Session("UnknownSupplierName") = ""
	Session("UnknownSupplierContact") = ""
	Session("UnknownSupplierPhoneNumber") = ""
	Session("UnknownSupplierFAXNumber") = ""	
End sub

Sub SetContainerSessionVarsFromPostedData()
	' Receive Posted data
	iNumCopies = Request.Form("iNumCopies")
	LocationID = "1" 'Request.QueryString("LocationID")
	LocationName = "On Order" 'Request.QueryString("LocationName")
	iContainerID = Request.Form("iContainerID")
	iContainerTypeID = Request.Form("iContainerTypeID")
	iContainerStatusID = Request.Form("iContainerStatusID")
	iUOMID =  Request.Form("iUOMID")
	iUOWID = Request.Form("iUOWID")
	iUOPID = Request.Form("iUOPID")
	iUOCID = Request.Form("iUOCID")
	iQtyMax =  Request.Form("iQtyMax")
	iQtyInitial =  Request.Form("iQtyInitial")
	iContainerName = Request.Form("iContainerName")
	iBarcode = Request.Form("iBarcode")
	iRegID = Request.Form("iRegID")
	iBatchNumber = Request.Form("iBatchNumber")
	iContainerDesc = Request.Form("iContainerDesc")
	iMinStockQty = Request.Form("iMinStockQty")
	iMaxStockQty = Request.Form("iMaxStockQty")
	iExpDate =  Request.Form("iExpDate")
	iCompoundID = Request.Form("iCompoundID")
	iTareWeight = Request.Form("iTareWeight")
	iPurity = Request.Form("iPurity")
	iConcentration = Request.Form("iConcentration")
	iGrade = Request.Form("iGrade")
	iSolvent = Request.Form("iSolvent")
	iComments = Request.Form("iComments")
	iSupplierID = Request.Form("iSupplierID")
	iSupplierName = Request.Form("iSupplierName")
	iSupplierShortName = Request.Form("iSupplierShortName")
	iLotNum = Request.Form("iLotNum")
	iDateProduced = Request.Form("iDateProduced")
	iDateOrdered = Request.Form("iDateOrdered")
	iDateReceived = Request.Form("iDateReceived")
	iContainerCost = Request.Form("iContainerCost")
	iPONumber = Request.Form("iPONumber")
	iReqNumber = Request.Form("iReqNumber")
	iSupplierCatNum = Request.Form("iSupplierCatNum")
	iOwnerID = Request.Form("iOwnerID")
	iCurrentUserID = Request.Form("iCurrentUserID")
	'CBOE-798 SJ 
    if Application("ENABLE_OWNERSHIP")="TRUE" then
	    iPrincipalID=Request.Form("PrincipalID")
	    iLocationTypeID=Request.Form("LocationTypeID")
	end if
	
	'MWS: Attributes for order entry
    iDueDate = Request.Form("iDueDate")
	iProject = Request.Form("iProject")
	iJob = Request.Form("iJob")
	iDeliveryLocationID = Request.Form("iDeliveryLocationID")
	iUnknownSupplierName = Request.Form("iUnknownSupplierName")
	iUnknownSupplierContact = Request.Form("iUnknownSupplierContact")
	iUnknownSupplierPhoneNumber = Request.Form("iUnknownSupplierPhoneNumber")
	iUnknownSupplierFAXNumber = Request.Form("iUnknownSupplierFAXNumber")
	'MCD: Added for 'Order Reason'
	iOrderReason = Request.Form("iOrderReason")
	iOrderReasonOther = Request.Form("iOrderReasonOther")

	' Set session variables to store posted data
	if Not IsEmpty(LocationID) then Session("LocationID")= LocationID
	if Not IsEmpty(LocationName) then Session("LocationName")= LocationName
	if Not IsEmpty(iContainerID)then  Session("ContainerID")= iContainerID
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
	if Not IsEmpty(iUOPID) then 
		tempArr = Split(iUOPID,"=")
		Session("UOPIDOptionValue")= iUOPID
		Session("UOPAbv")= tempArr(1)
		Session("UOPID") = tempArr(0) 
	End if
	if Not IsEmpty(iNumCopies)then Session("NumCopies")= iNumCopies
	if Not IsEmpty(iQtyMax)then Session("QtyMax")= iQtyMax
	if Not IsEmpty(iQtyInitial)then Session("QtyInitial")= iQtyInitial
	if Not IsEmpty(iContainerName) then Session("ContainerName")= iContainerName
	if Not IsEmpty(iBarcode) then Session("Barcode")= iBarcode
	if Not IsEmpty(iRegID) then Session("RegID")= iRegID
	if Not IsEmpty(iBatchNumber) then Session("BatchNumber")= iBatchNumber
	if Not IsEmpty(iContainerDesc) then Session("ContainerDesc")= iContainerDesc
	if Not IsEmpty(iComments) then Session("Comments")= iComments
	if Not IsEmpty(iMinStockQty) then Session("MinStockQty")= iMinStockQty
	if Not IsEmpty(iMaxStockQty) then Session("MaxStockQty")= iMaxStockQty 
	if Not IsEmpty(iExpDate) then Session("ExpDate")= iExpDate
	if Not IsEmpty(iCompoundID) then Session("CompoundID")= iCompoundID
	if Not IsEmpty(iTareWeight) then Session("TareWeight")= iTareWeight
	if Not IsEmpty(iPurity) then Session("Purity")= iPurity
	if Not IsEmpty(iConcentration) then Session("Concentration")= iConcentration
	if Not IsEmpty(iGrade) then Session("Grade")= iGrade
	if Not IsEmpty(iSolvent) then Session("Solvent")= iSolvent
	if Not IsEmpty(iSupplierID) then Session("SupplierID")= iSupplierID
	if Not IsEmpty(iSupplierName) then Session("SupplierName")= iSupplierName
	if Not IsEmpty(iSupplierShortName) then Session("SupplierShortName")= iSupplierShortName
	if Not IsEmpty(iLotNum) then Session("LotNum")= iLotNum
	if Not IsEmpty(iDateProduced) then Session("DateProduced")= iDateProduced
	if Not IsEmpty(iDateOrdered) then Session("DateOrdered")= iDateOrdered
	if Not IsEmpty(iDateReceived) then Session("DateReceived")= iDateReceived
	if Not IsEmpty(iContainerCost) then Session("ContainerCost")= iContainerCost
	if Not IsEmpty(iPONumber) then Session("PONumber")= iPONumber
	if Not IsEmpty(iReqNumber) then Session("ReqNumber")= iReqNumber
	if Not IsEmpty(iSupplierCatNum) then Session("SupplierCatNum")= iSupplierCatNum
	if Not IsEmpty(iOwnerID) then Session("OwnerID")= iOwnerID
	if Not IsEmpty(iCurrentUserID) then Session("CurrentUserID")= iCurrentUserID
	'CBOE-798 SJ 
    if Application("ENABLE_OWNERSHIP")="TRUE" then
        if Not IsEmpty(iPrincipalID) then Session("PrincipalID")=iPrincipalID
        if Not IsEmpty(iLocationTypeID) then Session("LocationTypeID")=iLocationTypeID
    End if
	
	'MWS: Attributes for order entry
	if Not IsEmpty(iDueDate) then Session("DueDate")= iDueDate
	if Not IsEmpty(iProject) then Session("Project")= iProject
	if Not IsEmpty(iJob) then Session("Job")= iJob
	if Not IsEmpty(iDeliveryLocationID) then Session("DeliveryLocationID")= iDeliveryLocationID
	if Not IsEmpty(iUnknownSupplierName) then Session("UnknownSupplierName")= iUnknownSupplierName
	if Not IsEmpty(iUnknownSupplierContact) then Session("UnknownSupplierContact")= iUnknownSupplierContact
	if Not IsEmpty(iUnknownSupplierPhoneNumber) then Session("UnknownSupplierPhoneNumber")= iUnknownSupplierPhoneNumber
	if Not IsEmpty(iUnknownSupplierFAXNumber) then Session("UnknownSupplierFAXNumber")= iUnknownSupplierFAXNumber

	'MCD: Added for 'Order Reason'
	'Check for an 'Available' container with the same compound
	Call GetInvConnection()
	Set RS2 = Conn.Execute(SQL2)
	if RS2.BOF AND RS2.EOF then
		'No matching 'Available' container, don't hassle the customer
		Session("Hassle") = 0
	else
		Session("Hassle") = 1
	end if

	if Not isEmpty(iOrderReason) Then Session("OrderReason") = iOrderReason
	if Not isEmpty(iOrderReasonOther) Then Session("OrderReasonOther") = iOrderReasonOther
	'MCD: end changes
End Sub

' Set local variables
	isEdit = Session("isEdit")
	ContainerID = Session("ContainerID")
	LocationName = Session("LocationName")
	LocationID = Session("LocationID")
	ContainerName = Session("ContainerName")
	Barcode = Session("Barcode")
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
	'CBOE-798 SJ 
    if Application("ENABLE_OWNERSHIP")="TRUE" then
        PrincipalID=Session("PrincipalID")
        LocationTypeID=Session("LocationTypeID")
    end if
		
	' Optional Container Attributes
	TareWeight = Session("TareWeight")
	UOWName = Session("UOWName")
	UOWAbv = Session("UOWAbv")
	UOWID = Session("UOWID")
	Session("UOWIDOptionValue") = UOWID & "=" & UOWAbv
	MinStockQty = Session("MinStockQty")
	MaxStockQty = Session("MaxStockQty")
	ContainerDesc = Session("ContainerDesc")
	
	' Contents attributes
	Purity = Session("Purity")
	UOPName = Session("UOPName")
	UOPAbv = Session("UOPAbv")
	UOPID = Session("UOPID")
	Session("UOPIDOptionValue")= UOPID & "=" & UOPAbv
	Concentration = Session("Concentration")
	UOCName = Session("UOCName")
	UOCAbv = Session("UOCAbv")
	UOCID = Session("UOCID")
	Session("UOCIDOptionValue") = UOCID & "=" & UOCAbv
	Grade = Session("Grade")
	Solvent = Session("Solvent")
	Comments = Session("Comments")
	RegID = Session("RegID")
	BatchNumber = Session("BatchNumber")
	ExpDate = Session("ExpDate")
	
	' Calculated Quantities
	QtyRemaining = Session("QtyRemaining")
	QtyAvailable = Session("QtyAvailable")
	DateCreated = Session("DateCreated")
	
	' Substance attributes
	CompoundID = Session("CompoundID")
	CAS = Session("CAS")
	ACX_ID = Session("ACX_ID")
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
	PONumber = Session("PONumber")  
	ReqNumber = Session("ReqNumber")      
	SupplierCatNum = Session("SupplierCatNum")
	
	'User attributes
	OwnerID = Session("OwnerID")
	CurrentUserID = Session("CurrentUserID")
	
	'Reg Attributes
	NoteBook =Session("NoteBook")  
	Page = Session("Page")  
	RegAmount = Session("RegAmount")
	RegPurity = Session("RegPurity")  
	RegName = Session("RegName") 
	BASE64_CDX = Session("BASE64_CDX")  
	RegBatchId = Session("RegBatchID") 
	RegScientist = Session("RegScientist") 
	 
	Session("CurrentLocationID") = LocationID
	Session("CurrentContainerName") = LocationName
	Session("CurrentContainerID") = ContainerID
	
	'MWS: Attributes for order entry
	DueDate = Session("DueDate")
	Project = Session("Project")
	Job = Session("Job")
	DeliveryLocationID = Session("DeliveryLocationID")
	UnknownSupplierName = Session("UnknownSupplierName")
	UnknownSupplierContact = Session("UnknownSupplierContact")
	UnknownSupplierPhoneNumber = Session("UnknownSupplierPhoneNumber")
	UnknownSupplierFAXNumber = Session("UnknownSupplierFAXNumber")
	'MCD: added for 'Order Reason'
	OrderReason = Session("OrderReason")
	OrderReasonOther = Session("OrderReasonOther")
%>
