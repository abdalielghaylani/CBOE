<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/Getcreatecontainer_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:CreateContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateContainer.htm"
	Response.end
End if
 CsUserName = Request("tempCsUserName")
 CsUserID =Request("tempCsUserID")
 UserSettingsAction = Request("UserSettingsAction")
 if ucase(UserSettingsAction) = ucase("SAVE") then
    'Required Paramenters
    LocationID = Request("iLocationID") 
    if Request("Date_Created") <>"" then
        DateCreated = convertDateFormatELN(Request("Date_Created")) 
    end if 
    BarcodeDescID = Request("Barcode_Desc_ID")
    UOMID = Request("Unit_Of_Meas_ID_FK")
    ContainerTypeID = Request("Container_Type_ID_FK")
    ContainerStatusID = Request("Container_Status_ID_FK")
    UOWID = Request("Unit_of_WGHT_ID_FK")
    UOPID = Request("Unit_of_Purity_ID_FK")
    UOCID = Request("Unit_of_CONC_ID_FK")
    UODID = Request("Unit_of_DENSITY_ID_FK")
    SolventIDFK = Request("Solvent_ID_FK")
    SupplierID = Request("Supplier_ID_FK")
    RegID = Request("Reg_ID_FK")
    LocationBarcode = Request("lpLocationBarCode")
    if Application("ENABLE_OWNERSHIP")="TRUE" then
        if len(Request("PRINCIPAL_ID_FK")) > 0 then PrincipalID = Request("PRINCIPAL_ID_FK")
        if PrincipalID = 0 or PrincipalID ="0" then PrincipalID =""
        LocationTypeID=Request("LOCATION_TYPE_ID_FK")
    end if

    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".TranslateID2Name1", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("pBarcodeDescID",adVarChar, adParamInputOutput, 1000, BarcodeDescID)
    Cmd.Parameters.Append Cmd.CreateParameter("pLocationID",adVarChar, adParamInputOutput, 1000, LocationID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUOMID",adVarChar, adParamInputOutput, 2000, UOMID)
    Cmd.Parameters.Append Cmd.CreateParameter("pContainerTypeID",adVarChar, adParamInputOutput, 2000, ContainerTypeID)
    Cmd.Parameters.Append Cmd.CreateParameter("pContainerStatusID",adVarChar, adParamInputOutput, 2000, ContainerStatusID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUOWID",adVarChar, adParamInputOutput, 2000, UOWID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUOPID",adVarChar, adParamInputOutput, 2000, UOPID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUOCID",adVarChar, adParamInputOutput, 2000, UOCID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUODID",adVarChar, adParamInputOutput, 2000, UODID)
    Cmd.Parameters.Append Cmd.CreateParameter("pSolventIDFK",adVarChar, adParamInputOutput, 2000, SolventIDFK)
    Cmd.Parameters.Append Cmd.CreateParameter("pSupplierID",adVarChar, adParamInputOutput, 2000, SupplierID)
    Cmd.Parameters.Append Cmd.CreateParameter("pRegNumber",adVarChar, adParamInputOutput, 2000, RegID)

    if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".TranslateID2Name1")
    End if

     if Cmd.Parameters("pLocationID") <>"" then LocationID = Cmd.Parameters("pLocationID")
     if Cmd.Parameters("pBarcodeDescID") <>"" then BarcodeDescID = Cmd.Parameters("pBarcodeDescID")
     if Cmd.Parameters("pUOMID") <>"" then UOMID = Cmd.Parameters("pUOMID")
     if Cmd.Parameters("pContainerTypeID") <>"" then ContainerTypeID = Cmd.Parameters("pContainerTypeID")
     if Cmd.Parameters("pContainerTypeID") <>"" then ContainerStatusID = Cmd.Parameters("pContainerStatusID")
     if Cmd.Parameters("pUOWID") <>"" then UOWID = Cmd.Parameters("pUOWID")
     if Cmd.Parameters("pUOPID") <>"" then  UOPID = Cmd.Parameters("pUOPID")
     if Cmd.Parameters("pUOCID") <>"" then UOCID = Cmd.Parameters("pUOCID")
     if Cmd.Parameters("pUODID") <>"" then UODID = Cmd.Parameters("pUODID")
     if Cmd.Parameters("pSolventIDFK") <>""  then SolventIDFK = Cmd.Parameters("pSolventIDFK")
     if Cmd.Parameters("pSupplierID") <>""  then SupplierID = Cmd.Parameters("pSupplierID")
     if Cmd.Parameters("pRegNumber") <>"" then RegID = Cmd.Parameters("pRegNumber")

     Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GetContainerFields", adCmdStoredProc)
     Cmd.Parameters.Append Cmd.CreateParameter("oContainerFields",adVarChar, adParamOutput, 4000, NULL)
      if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GetContainerFields")
    End if

    ContainerFieldString = Cmd.Parameters("oContainerFields")
    ArrContainerFields = split(ContainerFieldString, ",")
    Set XMLContainer = Server.CreateObject("Msxml2.DOMDocument")
    XMLContainer.async = false
    With XMLContainer
        Set oRootNode = .createElement("inventory")
        Set .documentElement = oRootNode
        With oRootNode
            Set oNode = XMLContainer.createElement("containerfields")
            .appendChild oNode
        End With
    end with
    With oNode
        for each field in ArrContainerFields
            Set oNode1 = XMLContainer.createElement("field")
            oNode1.setAttribute "name", field
            if lcase(field) = "location_id_fk" then
                oNode1.setAttribute "value", LocationID
            elseif lcase(field) = "date_created" then  
                oNode1.setAttribute "value", DateCreated
            elseif lcase(field) = "barcode_desc_id" then  
                oNode1.setAttribute "value", BarcodeDescID
            elseif lcase(field) = "unit_of_meas_id_fk" then  
                oNode1.setAttribute "value", UOMID 
            elseif lcase(field) = "container_type_id_fk" then  
                oNode1.setAttribute "value", ContainerTypeID 
            elseif lcase(field) = "container_status_id_fk" then  
                oNode1.setAttribute "value", ContainerStatusID
            elseif lcase(field) = "unit_of_wght_id_fk" then  
                oNode1.setAttribute "value", UOWID 
            elseif lcase(field) = "unit_of_purity_id_fk" then  
                oNode1.setAttribute "value", UOPID 
            elseif lcase(field) = "unit_of_conc_id_fk" then  
                oNode1.setAttribute "value", UOCID  
            elseif lcase(field) = "unit_of_density_id_fk" then  
                oNode1.setAttribute "value", UODID   
            elseif lcase(field) = "solvent_id_fk" then  
                oNode1.setAttribute "value", SolventIDFK
            elseif lcase(field) = "supplier_id_fk" then  
                oNode1.setAttribute "value", SupplierID    
            elseif lcase(field) = "reg_id_fk" then  
                oNode1.setAttribute "value", RegID                            
            elseif lcase(field) = "principal_id_fk" then  
                oNode1.setAttribute "value", PrincipalID                   
            else    
                oNode1.setAttribute "value", Request(field)
            end if
            .appendChild oNode1

            if lcase(field) = "location_id_fk" then       
                Set oNode1 = XMLContainer.createElement("field")
                oNode1.setAttribute "name", "location_barcode"
                oNode1.setAttribute "value", LocationBarcode
                .appendChild oNode1
            end if                                           
        Next    
                                                     
    End With 
    ' Set up and ADO command
    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".SaveUserSettingsProc", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTUSERID",200, 1, 50, CsUserName)
    Cmd.Parameters.Append Cmd.CreateParameter("pUserSettingsXML", AdLongVarChar, adParamInput, len(XMLContainer.xml) + 1, XMLContainer.xml)
    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",AdVarChar, adParamReturnValue, 16384, NULL)

    Cmd.Properties("SPPrmsLOB") = TRUE
    if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".SaveUserSettingsProc")
    End if
    Response.write trim(Cmd.Parameters("RETURN_VALUE"))
    Response.End

 elseif ucase(UserSettingsAction) = ucase("FETCH") then
    CurrentUserID = request("CurrentUserID")

    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GetUserSettingsProc", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("pUserID",200, 1, 50, CurrentUserID)
    Cmd.Parameters.Append Cmd.CreateParameter("pUserSettingsXML", AdLongVarChar, adParamReturnValue, 16384 + 1, NULL)

    Cmd.Properties("SPPrmsLOB") = TRUE
    if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GetUserSettingsProc")
    End if
    if Cmd.Parameters("pUserSettingsXML") <>"" then
        Set XMLContainer = Server.CreateObject("Msxml2.DOMDocument")
        XMLContainer.async = false
        XMLContainer.loadxml(Cmd.Parameters("pUserSettingsXML"))
        Set parentnode = XMLContainer.SelectSingleNode("/inventory/containerfields")
        for each actionNode in parentnode.SelectNodes("field")
            'For Each fieldAttrib In actionNode.Attributes
            fieldname = actionNode.getAttribute("name")
            fieldval = actionNode.getAttribute("value")
            if lower(fieldname) = "date_created" then
                fieldval = convertDateFormatELN(fieldval)
            end if
            nameValPair = nameValPair &  getFieldName(fieldname) & "=" & fieldval & "::"
          Next
          nameValPair = mid(nameValPair,1,Len(nameValPair)-2)
          Response.Write nameValPair
     else
          Response.Write ""
     end if
     Response.End
end if


Set paramR = Nothing
Set Cmd = Nothing
Set Conn = Nothing

function getFieldName (dbfield)
    Select case ucase(dbfield)
        case "BARCODE_DESC_ID"
            getFieldName = "BarcodeDescID"
        case "COMPOUND_ID_FK"
            getFieldName = "CompoundID"      
        case "Date_Created"
            getFieldName = "DateCreated"      
        case "BARCODE"
	        getFieldName = "Barcode"    
        case "LOCATION_ID_FK"
	        getFieldName = "LocationID"
        case "UNIT_OF_MEAS_ID_FK"
	        getFieldName = "UOMID"
        case "CONTAINER_TYPE_ID_FK"
	        getFieldName = "ContainerTypeID"
        case "CONTAINER_STATUS_ID_FK"
	        getFieldName = "ContainerStatusID"
        case "QTY_MAX"
	        getFieldName = "QtyMax"
        case "REG_ID_FK"
	        getFieldName = "RegID"
        case "BATCH_NUMBER_FK"
	        getFieldName = "BatchNumber"
        case "QTY_INITIAL"
	        getFieldName = "QtyInitial"
        case "QTY_MAXSTOCK"
	        getFieldName = "MaxStockQty"
        case "QTY_MINSTOCK"
	        getFieldName = "MinStockQty"
        case "DATE_EXPIRES"
	        getFieldName = "ExpDate"
        case "CONTAINER_NAME"
	        getFieldName = "ContainerName"
        case "CONTAINER_DESCRIPTION"
	        getFieldName = "ContainerDesc"
        case "TARE_WEIGHT"
	        getFieldName = "TareWeight"
        case "NET_WGHT"
	        getFieldName = "NetWeight"
        case "FINAL_WGHT"
	        getFieldName = "FinalWeight"
        case "UNIT_OF_WGHT_ID_FK"
	        getFieldName = "UOWID"
        case "PURITY"
	        getFieldName = "Purity"
        case "UNIT_OF_PURITY_ID_FK"
	        getFieldName = "UOPID"
        case "CONCENTRATION"
	        getFieldName = "Concentration"
        case "DENSITY"
	        getFieldName = "Density"
        case "UNIT_OF_CONC_ID_FK"
	        getFieldName = "UOCID"
        case "UNIT_OF_DENSITY_ID_FK"
	        getFieldName = "UODID"
        case "SOLVENT_ID_FK"
	        getFieldName = "SolventIDFK"
        case "GRADE"
	        getFieldName = "Grade"
        case "CONTAINER_COMMENTS"
	        getFieldName = "Comments"
        case "STORAGE_CONDITIONS"
	        getFieldName = "StorageConditions"
        case "HANDLING_PROCEDURES"
	        getFieldName = "HandlingProcedures"
        case "SUPPLIER_ID_FK"
	        getFieldName = "SupplierID"
        case "SUPPLIER_CATNUM"
	        getFieldName = "SupplierCatNum"
        case "LOT_NUM"
	        getFieldName = "LotNum"
        case "DATE_PRODUCED"
	        getFieldName = "DateProduced"
        case "DATE_ORDERED"
	        getFieldName = "DateOrdered"
        case "DATE_RECEIVED"
	        getFieldName = "DateReceived"
        case "CONTAINER_COST"
	        getFieldName = "ContainerCost"
        case "UNIT_OF_COST_ID_FK"
	        getFieldName = "UOCostID"
        case "PO_LINE_NUMBER"
	        getFieldName = "POLineNumber"
        case "PO_NUMBER"
	        getFieldName = "PONumber"
        case "REQ_NUMBER"
	        getFieldName = "ReqNumber"
        case "PNUMCOPIES"
	        getFieldName = "NumCopies"
        case "FIELD_1"
	        getFieldName = "Field_1"
        case "FIELD_2"
	        getFieldName = "Field_2"
        case "FIELD_3"
	        getFieldName = "Field_3"
        case "FIELD_4"
	        getFieldName = "Field_4"
        case "FIELD_5"
	        getFieldName = "Field_5"
        case "FIELD_6"
	        getFieldName = "Field_6"
        case "FIELD_7"
	        getFieldName = "Field_7"
        case "FIELD_8"
	        getFieldName = "Field_8"
        case "FIELD_9"
	        getFieldName = "Field_9"
        case "FIELD_10"
	        getFieldName = "Field_10"
        case "DATE_1"
	        getFieldName = "Date_1"
        case "DATE_2"
	        getFieldName = "Date_2"
        case "DATE_3"
	        getFieldName = "Date_3"
        case "DATE_4"
	        getFieldName = "Date_4"
        case "DATE_5"
	        getFieldName = "Date_5"
        case "OWNER_ID_FK"
	        getFieldName = "OwnerID"
        case "CURRENT_USER_ID_FK"
	        getFieldName = "CurrentUserID"
        case "PRINCIPAL_ID_FK"
    	    getFieldName = "PrincipalID"
        case "LOCATION_TYPE_ID_FK"
    	    getFieldName = "LocationTypeID"
 end select
end function
</SCRIPT>
