<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/attribute_defaults.asp"-->
<%
    CurrentUserID = Session("UserName" & "cheminv")
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
    UserSettingXML = Cmd.Parameters("pUserSettingsXML")
    if UserSettingXML <>"" Then
        Set XMLContainer = Server.CreateObject("Msxml2.DOMDocument")
        XMLContainer.async = false
        XMLContainer.loadxml(UserSettingXML)
        Set parentnode = XMLContainer.SelectSingleNode("/inventory/containerfields")
        for each actionNode in parentnode.SelectNodes("field")
            fieldname = actionNode.getAttribute("name")
            fieldval = actionNode.getAttribute("value")
            if lcase(fieldname) = "date_created" then
                 if trim(fieldval) <> "" then 
                    fieldval = convertDateFormatELN(fieldval)
                 end if
            end if
            Session(lcase(fieldname)) =  fieldval
        Next
        Call GetInvCommand(Application("CHEMINV_USERNAME") & ".TranslateID2Name", adCmdStoredProc)
        Cmd.Parameters.Append Cmd.CreateParameter("pBarcodeDescID",adVarChar, adParamInputOutput, 1000, session("barcode_desc_id"))
        Cmd.Parameters.Append Cmd.CreateParameter("pLocationID",adVarChar, adParamInputOutput, 1000, session("location_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pUOMID",adVarChar, adParamInputOutput, 2000, session("unit_of_meas_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pContainerTypeID",adVarChar, adParamInputOutput, 2000, session("container_type_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pContainerStatusID",adVarChar, adParamInputOutput, 2000, session("container_status_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pUOWID",adVarChar, adParamInputOutput, 2000, session("unit_of_wght_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pUOPID",adVarChar, adParamInputOutput, 2000, session("unit_of_purity_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pUOCID",adVarChar, adParamInputOutput, 2000, session("unit_of_conc_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pUODID",adVarChar, adParamInputOutput, 2000, session("unit_of_density_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pSolventIDFK",adVarChar, adParamInputOutput, 2000, session("solvent_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pSupplierID",adVarChar, adParamInputOutput, 2000, session("supplier_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pRegNumber",adVarChar, adParamInputOutput, 2000, session("reg_id_fk"))
        Cmd.Parameters.Append Cmd.CreateParameter("pLocationBarcode",adVarChar, adParamInputOutput, 2000,  iif(session("location_barcode")="",null,session("location_barcode")))
        Cmd.Parameters.Append Cmd.CreateParameter("pIsFullLocation",adVarChar, adParamInput, 2000, "true")
        if bDebugPrint then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = " & p.value & "<BR>"
	    Next
	    Response.End	
        Else
	        Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".TranslateID2Name")
        End if
    
         if Cmd.Parameters("pLocationID") <>"" then 
            session("location_id_fk") = Cmd.Parameters("pLocationID")
         else
            session("location_id_fk") = ""
         end if   
         if Cmd.Parameters("pBarcodeDescID") <>"" then session("barcode_desc_id") = Cmd.Parameters("pBarcodeDescID")
         if Cmd.Parameters("pUOMID") <>"" then session("unit_of_meas_id_fk") = Cmd.Parameters("pUOMID")
         if Cmd.Parameters("pContainerTypeID") <>"" then session("container_type_id_fk") = Cmd.Parameters("pContainerTypeID")
         if Cmd.Parameters("pContainerTypeID") <>"" then session("container_status_id_fk") = Cmd.Parameters("pContainerStatusID")
         if Cmd.Parameters("pUOWID") <>"" then session("unit_of_wght_id_fk") = Cmd.Parameters("pUOWID")
         if Cmd.Parameters("pUOPID") <>"" then  session("unit_of_purity_id_fk") = Cmd.Parameters("pUOPID")
         if Cmd.Parameters("pUOCID") <>"" then session("unit_of_conc_id_fk") = Cmd.Parameters("pUOCID")
         if Cmd.Parameters("pUODID") <>"" then session("unit_of_density_id_fk") = Cmd.Parameters("pUODID")
         if Cmd.Parameters("pSolventIDFK") <>""  then session("solvent_id_fk") = Cmd.Parameters("pSolventIDFK")
         if Cmd.Parameters("pSupplierID") <>""  then session("supplier_id_fk") = Cmd.Parameters("pSupplierID")
         if Cmd.Parameters("pRegNumber") <>"" then session("reg_id_fk") = Cmd.Parameters("pRegNumber")

  end if
  
%>
