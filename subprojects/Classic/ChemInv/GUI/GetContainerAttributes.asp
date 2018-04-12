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
Dim Cmd1
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

        'JHS Use the full New Container form from Registration and load use a Map
        'we only deal with mapping if the RegBatchID is passed and the application is configured for using it
        if Request.QueryString("vRegBatchID") <> "" and UCase(Application("UseRegMap")) = "TRUE" then

            goodmap = false

            'if the map value has been previously set and wasn't set to Nomap then we should use it
            'NOMAP essentially means that either 1) The xml file was not there 2) There was an error loading it
            if Application("RegMap") <> null and Application("RegMap") <> "" and Application("RegMap") <> "NOMAP" then
               goodmap = true
            else
                'if the map has never been created we should create it
                if Application("RegMap") = null or Application("RegMap") = "" then
                    Call SetRegMap            
                end if

                'if the map was no good then don't use it
                if Application("RegMap") <> "NOMAP" then
                    goodmap = true
                end if  
            end if
       
            'Use the map to set session variables
            if goodmap = true then
                 Call SetContainerSessionVarsFromRegWebService()
            end if
        end if 
        if lcase(Request("source")) = "eln" then
            call SetELNValues(request("valuepairs"))
        end if 
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
	if Application("ENABLE_OWNERSHIP")="TRUE" then
	Session("OwnershipType")=""
	Session("LocationTypeID")=""
	end if
	
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
	SetDefaultAttributeValues()
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
	SQL = "SELECT inv_requests.request_id AS request_id FROM inv_requests WHERE request_type_id_fk = 1 AND container_id_fk = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
	Set rsRequest = Cmd.Execute

	if RS.BOF AND RS.EOF then
		Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
		Response.Write "<P><CODE>Error:SetContainerSessionVarsFromDb</CODE></P>"
		Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve container data for Container_ID " & ContainerID & " or User may not have permission(s) to view the container information.</SPAN>"
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
        if ExpYears > 0 then
		    Select case CStr(Application("DATE_FORMAT"))
			    case "8"
				    Session("ExpDate") = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) + ExpYears
			    case "9"
				    Session("ExpDate") = Day(Now()) & "/" & Month(Now()) & "/" & Year(Now()) + ExpYears
			    case "10"
				    Session("ExpDate") = Year(Now()) + ExpYears & "/" & Month(Now()) & "/" & Day(Now())
		    End Select
        else
            Session("ExpDate") = ""
		end if
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
	'CSBR-153626: SJ: Adding session to contain the db values when the edit option is selected.
	Session("UOCostIDOptionValue") = RS("UOCOSTID").value & "=" & RS("UOCostAbv").value
	Session("PONumber") = RS("PO_NUMBER").value
	Session("POLineNumber") = RS("PO_Line_NUMBER").value
	Session("ReqNumber") = RS("REQ_NUMBER").value
	Session("SupplierCatNum") = RS("SUPPLIER_CATNUM").value

	' User attributes
	if Application("ENABLE_OWNERSHIP")="TRUE" then	
	Session("isAuthorised") = RS("isAuthorised").value
	end if
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
	Session("BatchID1") = RS("Batch_ID_FK").value
	Session("BatchID2") = RS("BATCH_ID2_FK").value
	Session("BatchID3") = RS("BATCH_ID3_FK").value
	if Application("ENABLE_OWNERSHIP")="TRUE" then
	Session("OwnershipType") = RS("OwnershipType").value
	Session("LocationTypeID") = RS("LocationType").value
	end if

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

Sub SetRegMap()

    		Set oResultXML = server.CreateObject("MSXML2.DOMDocument")

            sPath = "\ChemInv\config\xml_templates\InvRegMap.xml"
            on error resume next
            pathToRegMap = Server.MapPath(sPath)

                        
            if err.number<> 0 then
                Application("RegMap") = "NOMAP"
                trace "Failed to load the Registration map at this location: " & sPath, 0
                on error goto 0
            end if

            on error resume next
            oResultXML.load pathToRegMap
            
            if err.number<> 0 then
                Application("RegMap") = "NOMAP"
                trace "Failed to load the Registration map at this location: " & pathToRegMap, 0
                on error goto 0
            else
                Application("RegMap") = oResultXML.xml
            end if
End Sub


Sub SetContainerSessionVarsFromRegWebService()
    serviceInitialized = false
    successfulRegCall = false
    regXMLloaded = false

    pRegBatchID = Request.QueryString("vRegBatchID")

    'call from reg
    set soapClient = CreateObject("MSSOAP.SoapClient30")
    set headerHandler = CreateObject("COEHeaderServer.COEHeaderHandler")
    'headerHandler.UserName = Ucase(Session("UserNameChemInv"))
   ' headerHandler.Password = Session("UserIDChemInv")
    headerHandler.Ticket= session("SSOAuthTicket")' Request.Cookies("COESSO")
    set soapClient.HeaderHandler = headerHandler
    soapClient.ClientProperty("ServerHTTPRequest") = True

     on error resume next
    call soapClient.MSSoapInit(Application("SERVER_TYPE") & Application("RegServerName") & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")

    'if there is an error in retrieving the regxml we should log the error
    if err.number <> 0 then
        trace "Failed to initialize the Reg WebService", 0
        trace "Error Description: " & soapClient.Detail, 0
        on error goto 0
    else
        serviceInitialized = true
    end if


    ' if the service is healthy
    if serviceInitialized then

        soapClient.ConnectorProperty("Timeout") = 120000    ' sets timeout to 120 secs

        on error resume next
        regXML = soapClient.RetrieveRegistryRecordByBatchID(pRegBatchID, true)

        'if there is an error in retrieving the regxml we should log the error
        if err.number <> 0 then
            trace "The Registration XML could not be retrieved with the following errors", 0
            trace "Error Description: " & soapClient.Detail, 0
            on error goto 0
        else
            successfulRegCall = true
        end if


    end if

    if successfulRegCall then

        set xmlRegBase = Server.CreateObject("Msxml2.DOMDocument")
        xmlRegBase.async = false
    
   
        on error resume next
        xmlRegBase.loadXML(regXML)

        'if there is an error in loading the regxml we should log the error
        if err.number <> 0 then
            trace "The Registration XML could not be loaded with the following errors", 0
            trace "Error Number: " & err.number, 0
            trace "Error Description: " & err.description, 0
            on error goto 0
        else
            regXMLloaded = true
        end if

    end if
    
    if regXMLloaded then

        'read map from applciation variable
        set regMap = Server.CreateObject("Msxml2.DOMDocument")
        regMap.loadXML(Application("RegMap"))
    
        'Get list of fields

        Session("CompoundID") = ""
	    GetRegBatchAttributes xmlRegBase.selectSingleNode("MultiCompoundRegistryRecord/RegNumber/RegID").text, xmlRegBase.selectSingleNode("MultiCompoundRegistryRecord/BatchList/Batch/BatchNumber").text

        set invFields = regMap.selectNodes("RegNewInvContainerMap/fields/field")
   

        For i = 0 To (invFields.length - 1)
            Set invField = invFields(i)
		
            invVar = invField.GetAttribute("invSessionVar")
            xpath = invField.GetAttribute("xPath")        
            constantVal = invField.GetAttribute("constantVal") 	

			xpathsplit = Split(xpath, "{vRegBatchID}")
			if UBound(xpathsplit) > 0 then
				xpath = Join(xpathsplit, pRegBatchID)
			end if
			
            value = ""

            if constantVal <> "" then
                value = constantVal
            end if

            if xpath <> "" then
                if value <> "" then
                    value = value & " "
                end if

                'GetXPathVal
                on error resume next
                xPathVal = xmlRegBase.selectSingleNode(xPath).text

                'if there is an error then we skip that particular xpath instead of blowing up
                if err.number <> 0 then
                    trace "The registration node " & xPath & " was skipped because it could not be found", 0
                    on error goto 0
                else
                    value = value & xPathVal
                end if

            end if

           if value <> "" then
                Session(invVar) = value
				'Drop down lists uses as value the following string UOMID=UOMAbv
				'And typically only an ID is updated from other apps (IE: Reg), so we must ensure the unit of abreviation is updated too
				if invVar = "UOMID" then
					Call GetInvConnection()
					SQL = "SELECT inv_units.unit_abreviation UOMAbv FROM inv_units WHERE inv_units.unit_id=" & value
					Set RS = Conn.Execute(SQL)
					Session("UOMAbv") = RS("UOMAbv")
					Session("UOMIDOptionValue")= Session("UOMID")  & "=" & Session("UOMAbv") 
				end if
           end if

	    Next
    end if
    'Set oFSO = CreateObject("Scripting.FileSystemObject")
    'Set oFile = oFSO.CreateTextFile("NewRegistryRecord.xml", True)
    'oFile.Write(strXMLTemplate)
    'oFile.Close



end sub

Sub SetContainerSessionVarsFromPostedData()
	' Receive Posted data
	iNumCopies = Request.Form("iNumCopies")
	'LocationID = Request.QueryString("LocationID")
	LocationID = Request.Form("iLocationID")
	iLocationID = Request.Form("iLocationID")
	if Application("ENABLE_OWNERSHIP")="TRUE" then
	LocationTypeID=Request.Form("iLocationTypeID")
	end if
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
	if Application("ENABLE_OWNERSHIP")="TRUE" AND Not IsEmpty(LocationTypeID) then Session("LocationTypeID")= LocationTypeID
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
	if Application("ENABLE_OWNERSHIP")="TRUE" then
    OwnershipType=Session("OwnershipType")
    LocationTypeID=Session("LocationTypeID")
    end if

	' Calculated Quantities
	QtyRemaining = Session("QtyRemaining")
	QtyAvailable = Session("QtyAvailable")
	DateCreated = Session("DateCreated")
	TotalQtyReserved = Session("TotalQtyReserved")
	
	' Substance attributes	
	if not IsEmpty(RegNumber) and not IsNull(RegNumber) then
	    ' We have a dummy record for the compound_id, so wipe it out to not confuse the user
	    Session("CompoundID") = ""
	    CompoundID = ""	    
	else
	    CompoundID = Session("CompoundID")	    
	end if
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
	BatchID1 = Session("BatchID")
	BatchID2 = Session("BatchID2")
	BatchID3 = Session("BatchID3")
	for each key in reg_fields_dict
		'Response.Write(reg_fields_dict.item(key) & " : " & key & "<br>")
		execute(key & " = Session(""" & key & """)")
	next

	Session("CurrentLocationID") = Session("OriginalCurrentLocationID")
	'Session("CurrentContainerName") = LocationName
	Session("CurrentContainerID") = ContainerID

	'-- Rack attributes
	RackGridID = Session("RackGridID")
    RegBatchID = Session("RegBatchID")
    RegID = Session("RegID")
Sub SetELNValues(valuePairs)
  	FieldsArray = split(valuePairs, "::")
    for each field in FieldsArray
        FieldVal = split(field, "=")
        if trim(FieldVal(1)) <> "" then
            Select case lcase(FieldVal(0))
            case "barcode_desc_id"
               Session("BarcodeDescID") =  trim(FieldVal(1))          
            case "compound_id_fk"
               Session("CompoundID") =  trim(FieldVal(1))
            case "barcode"
               Session("Barcode") =  trim(FieldVal(1))                     
            case "location_id_fk"
               Session("LocationID") =  trim(FieldVal(1))
            case "unit_of_meas_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UOMAbv") =  trim(FieldVal(2)) ' in case if Units has both id & abv separated with =
				else
					Session("UOMAbv") =  trim(FieldVal(1))
				end if
            case "container_type_id_fk"
               Session("ContainerTypeID") =  trim(FieldVal(1))
            case "container_status_id_fk"
               Session("ContainerStatusID") =  trim(FieldVal(1))
            case "qty_max"
               Session("QtyMax") =  trim(FieldVal(1))
            case "qty_initial"
               Session("QtyInitial") =  trim(FieldVal(1))
               Session("QtyRemaining") =  trim(FieldVal(1))
               Session("QtyAvailable") =  trim(FieldVal(1))
            case "reg_id_fk"
               Session("RegNumber") =  trim(FieldVal(1))
            case "batch_number_fk"
               Session("BatchNumber") =  trim(FieldVal(1))
            case "qty_minstock"
               Session("MinStockQty") =  trim(FieldVal(1))
            case "qty_maxstock"
               Session("MaxStockQty") =  trim(FieldVal(1))
            case "date_expires"
               Session("ExpDate") =  trim(FieldVal(1))
            case "container_name"
               Session("ContainerName") =  trim(FieldVal(1))
            case "container_description"
               Session("ContainerDesc") =  trim(FieldVal(1))
            case "tare_weight"
               Session("TareWeight") =  trim(FieldVal(1))
            case "net_wght"
               Session("NetWeight") =  trim(FieldVal(1))
            case "final_wght"
               Session("FinalWeight") =  trim(FieldVal(1))
            case "purity"
               Session("Purity") =  trim(FieldVal(1))
            case "unit_of_purity_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UOPAbv") =  trim(FieldVal(2))
				else
					Session("UOPAbv") =  trim(FieldVal(1))
				end if
            case "concentration"
               Session("Concentration") =  trim(FieldVal(1))
            case "unit_of_conc_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UOCAbv") =  trim(FieldVal(2))
				else
					Session("UOCAbv") =  trim(FieldVal(1))
				end if
            case "density"
               Session("Density") =  trim(FieldVal(1))
            case "unit_of_density_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UODAbv") =  trim(FieldVal(2))
				else
					Session("UODAbv") =  trim(FieldVal(1))
				end if
            case "solvent_id_fk"
               Session("SolventIDFK") =  trim(FieldVal(1))
            case "grade"
               Session("Grade") =  trim(FieldVal(1))
            case "container_comments"
               Session("Comments") =  trim(FieldVal(1))
            case "storage_conditions"
               Session("StorageConditions") =  trim(FieldVal(1))
            case "handling_procedures"
               Session("HandlingProcedures") =  trim(FieldVal(1))
            case "supplier_id_fk"
               Session("SupplierID") =  trim(FieldVal(1))
            case "supplier_catnum"
               Session("SupplierCatNum") =  trim(FieldVal(1))
            case "date_produced"
               Session("DateProduced") =  trim(FieldVal(1))
            case "date_ordered"
               Session("DateOrdered") =  trim(FieldVal(1))
            case "lot_num"
               Session("LotNum") =  trim(FieldVal(1))
            case "date_received"
               Session("DateReceived") =  trim(FieldVal(1))
            case "container_cost"
               Session("ContainerCost") =  trim(FieldVal(1))
            case "po_number"
               Session("PONumber") =  trim(FieldVal(1))
             case "po_line_number"
               Session("POLineNumber") =  trim(FieldVal(1))
            case "req_number"
               Session("ReqNumber") =  trim(FieldVal(1))
            case "pnumcopies"
               Session("NumCopies") =  trim(FieldVal(1))
            case "unit_of_cost_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UOCostID") =  trim(FieldVal(2))
				else
					Session("UOCostID") =  trim(FieldVal(1))
				end if
            case "unit_of_wght_id_fk"
				if ubound(FieldVal) > 1 then
					Session("UOWAbv") =  trim(FieldVal(2))
				else
					Session("UOWAbv") =  trim(FieldVal(1))
				end if
           case "owner_id_fk"
               Session("OwnerID") =  trim(FieldVal(1))
           case "current_user_id_fk"
               Session("CurrentUserID") =  trim(FieldVal(1))
           case "field_1"
               Session("Field_1") =  trim(FieldVal(1))
           case "field_2"
               Session("Field_2") =  trim(FieldVal(1))
           case "field_3"
               Session("Field_3") =  trim(FieldVal(1))
          case "field_4"
               Session("Field_4") =  trim(FieldVal(1))
           case "field_5"
               Session("Field_5") =  trim(FieldVal(1))
           case "field_6"
               Session("Field_6") =  trim(FieldVal(1))
          case "field_7"
               Session("Field_7") =  trim(FieldVal(1))
           case "field_8"
               Session("Field_8") =  trim(FieldVal(1))
           case "field_9"
               Session("Field_9") =  trim(FieldVal(1))
          case "field_10"
               Session("Field_10") =  trim(FieldVal(1))
           case "date_1"
               Session("Date_1") =  trim(FieldVal(1))
           case "date_2"
               Session("Date_2") =  trim(FieldVal(1))       
           case "date_3"
               Session("Date_3") =  trim(FieldVal(1))
           case "date_4"
               Session("Date_4") =  trim(FieldVal(1))   
           case "date_5"
               Session("Date_5") =  trim(FieldVal(1))
           'if application("enable_ownership")="true" then
               case "principal_id_fk"
                   Session("OwnershipType") =  trim(FieldVal(1))
               case "location_type_id_fk"
                   Session("LocationTypeID") =  trim(FieldVal(1))                                                              
          ' end if
            end select
        end if
    next
      Call GetInvCommandbyRefObj(Application("CHEMINV_USERNAME") & ".TranslateID2Name", adCmdStoredProc, cmd1)
        Cmd1.Parameters.Append Cmd1.CreateParameter("pBarcodeDescID",adVarChar, adParamInputOutput, 1000, session("BarcodeDescID"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pLocationID",adVarChar, adParamInputOutput, 1000, session("LocationID"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pUOMID",adVarChar, adParamInputOutput, 2000, session("UOMAbv"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pContainerTypeID",adVarChar, adParamInputOutput, 2000, session("ContainerTypeID"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pContainerStatusID",adVarChar, adParamInputOutput, 2000, session("ContainerStatusID"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pUOWID",adVarChar, adParamInputOutput, 2000, session("UOWAbv"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pUOPID",adVarChar, adParamInputOutput, 2000, session("UOPAbv"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pUOCID",adVarChar, adParamInputOutput, 2000, session("UOCAbv"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pUODID",adVarChar, adParamInputOutput, 2000, session("UODAbv"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pSolventIDFK",adVarChar, adParamInputOutput, 2000, session("SolventIDFK"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pSupplierID",adVarChar, adParamInputOutput, 2000, session("SupplierID"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pRegNumber",adVarChar, adParamInputOutput, 2000, session("RegNumber"))
        Cmd1.Parameters.Append Cmd1.CreateParameter("pLocationBarcode",adVarChar, adParamInputOutput, 2000, null)
        Cmd1.Parameters.Append Cmd1.CreateParameter("pIsFullLocation",adVarChar, adParamInput, 2000, "true")
        if bDebugPrint then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = " & p.value & "<BR>"
	    Next
	    Response.End	
        Else
	        Call ExecuteCmdbyRefObj(Application("CHEMINV_USERNAME") & ".TranslateID2Name", cmd1)
        End if
        if Cmd1.Parameters("pLocationID") <>"" then session("LocationID") = Cmd1.Parameters("pLocationID")
        if Cmd1.Parameters("pBarcodeDescID") <>"" then session("BarcodeDescID") = Cmd1.Parameters("pBarcodeDescID")
        if Cmd1.Parameters("pUOMID") <>"" then Session("UOMID") = Cmd1.Parameters("pUOMID")
        if Cmd1.Parameters("pContainerTypeID") <>"" then session("ContainerTypeID") = Cmd1.Parameters("pContainerTypeID")
        if Cmd1.Parameters("pContainerTypeID") <>"" then session("ContainerStatusID") = Cmd1.Parameters("pContainerStatusID")
        if Cmd1.Parameters("pUOWID") <>"" then session("UOWID") = Cmd1.Parameters("pUOWID")
        if Cmd1.Parameters("pUOPID") <>"" then  session("UOPID") = Cmd1.Parameters("pUOPID")
        if Cmd1.Parameters("pUOCID") <>"" then session("UOCID") = Cmd1.Parameters("pUOCID")
        if Cmd1.Parameters("pUODID") <>"" then session("UODID") = Cmd1.Parameters("pUODID")
        if Cmd1.Parameters("pSolventIDFK") <>""  then session("SolventIDFK") = Cmd1.Parameters("pSolventIDFK")
        if Cmd1.Parameters("pSupplierID") <>""  then session("SupplierID") = Cmd1.Parameters("pSupplierID")
        if Cmd1.Parameters("pRegNumber") <>"" then session("RegNumber") = Cmd1.Parameters("pRegNumber")
        
        Set Cmd1 = nothing

        if session("RegNumber") <> "" and Session("BatchNumber") <>"" then
            
            SQL = "SELECT " &_
			" REGBATCHID, REGID  " &_
			" FROM " & Application("CHEMINV_USERNAME") &" .inv_vw_reg_batches " &_
			" Where  batchnumber = '" & Session("BatchNumber")  & "'" &_
			" AND REGID=" & session("RegNumber")  
            Call GetInvConnection() 	
	        Set RS= Conn.Execute(SQL)

	        if not (RS.BOF AND RS.EOF) then
                Session("RegBatchID") = RS("REGBATCHID")
                RegBatchID = RS("REGBATCHID")
                Session("RegID") = RS("REGID")
                RegID = RS("REGID")
            end if
        end if

End Sub
%>
