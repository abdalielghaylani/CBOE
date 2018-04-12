<%
' The RegisterInvCompound procedure adds substances to the inv_compounds table
' It Identifies preexisting or conflicting substances and optionally creates duplicate
' substances.   
CallText = "{call " & Application("CHEMINV_USERNAME") & ".compounds.RegisterInvCompound(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)}"
'Response.Write CallText & "<BR>"
Call GetInvCommand(CallText, adCmdText)		
Cmd.Parameters.Append Cmd.CreateParameter("pCompoundID", adNumeric, adParamInput, 0, EditCompoundID) 
Cmd.Parameters.Append Cmd.CreateParameter("pRegisterIfConflicts", adVarChar, adParamInput, 10, RegisterIfConflicts) 
Cmd.Parameters.Append Cmd.CreateParameter("pSN", 200, adParamInput, 255, SubstanceName) 
Cmd.Parameters.Append Cmd.CreateParameter("pCAS", 200, adParamInput, 15, CAS) 
Cmd.Parameters.Append Cmd.CreateParameter("pACX_ID", 200, adParamInput, 15, ACX_ID) 
Cmd.Parameters.Append Cmd.CreateParameter("pDensity",adNumeric, adParamInput, 0, Density)
Cmd.Parameters.Append Cmd.CreateParameter("pcLogP",adNumeric, adParamInput, 0, cLogP)
Cmd.Parameters.Append Cmd.CreateParameter("pRotatable_Bonds",adNumeric, adParamInput, 0, Rotatable_Bonds)
Cmd.Parameters.Append Cmd.CreateParameter("pTot_Pol_Surf_Area",adNumeric, adParamInput, 0, Tot_Pol_Surf_Area)
Cmd.Parameters.Append Cmd.CreateParameter("pHBond_Acceptors",adNumeric, adParamInput, 0, HBond_Acceptors)
Cmd.Parameters.Append Cmd.CreateParameter("pHBond_Donors",adNumeric, adParamInput, 0, HBond_Donors)
Cmd.Parameters.Append Cmd.CreateParameter("pALT_ID_1", 200, adParamInput, 2000, Alt_ID_1) 
Cmd.Parameters.Append Cmd.CreateParameter("pALT_ID_2", 200, adParamInput, 2000, Alt_ID_2)
Cmd.Parameters.Append Cmd.CreateParameter("pALT_ID_3", 200, adParamInput, 2000, Alt_ID_3)
Cmd.Parameters.Append Cmd.CreateParameter("pALT_ID_4", 200, adParamInput, 2000, Alt_ID_4)
Cmd.Parameters.Append Cmd.CreateParameter("pALT_ID_5", 200, adParamInput, 2000, Alt_ID_5)
Cmd.Parameters.Append Cmd.CreateParameter("pUniqueAltIDList", 200, adParamInput, 50, UniqueAltIDList)
Cmd.Parameters.Append Cmd.CreateParameter("pStructure", adLongVarChar, 1, len(Structure)+1, Structure)
Cmd.Parameters.Append Cmd.CreateParameter("oDupStr", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupSN", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupCAS", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupACX", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupAlt_ID_1", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupAlt_ID_2", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupAlt_ID_3", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupAlt_ID_4", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oDupAlt_ID_5", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oCompoundID", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oIsExistingCompound", adNumeric, adParamOutPut, 0, Null)
Cmd.Parameters.Append Cmd.CreateParameter("oIsDuplicateCompound", adNumeric, adParamOutPut, 0, Null)
Cmd.Properties("SPPrmsLOB") = TRUE

if bDebugPrint then
	Response.Write "Call Text= " & CallText & "<BR><BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	'Response.end
Else
	Cmd.execute
	Cmd.Properties("SPPrmsLOB") = FALSE

	DupStr = LngNull(cmd.parameters("oDupStr").value)
	DupSN = LngNull(cmd.parameters("oDupSN").value)
	DupCAS = LngNull(cmd.parameters("oDupCAS").value)
	DupACX = LngNull(cmd.parameters("oDupACX").value)
	DupAlt_ID_1 = LngNull(cmd.parameters("oDupAlt_ID_1").value)
	DupAlt_ID_2 = LngNull(cmd.parameters("oDupAlt_ID_2").value)
	DupAlt_ID_3 = LngNull(cmd.parameters("oDupAlt_ID_3").value)
	DupAlt_ID_4 = LngNull(cmd.parameters("oDupAlt_ID_4").value)
	DupAlt_ID_5 = LngNull(cmd.parameters("oDupAlt_ID_5").value)
	oCompoundID = LngNull(cmd.parameters("oCompoundID").value)
	oIsExistingCompound = LngNull(cmd.parameters("oIsExistingCompound").value)
	oIsDuplicateCompound = LngNull(cmd.parameters("oIsDuplicateCompound").value)
	if false then
		Response.Write "oDupStr= " & cmd.parameters("oDupStr").value & "<BR>"
		Response.Write "oDupSN= " & cmd.parameters("oDupSN").value & "<BR>"
		Response.Write "oDupCAS= " & cmd.parameters("oDupCAS").value & "<BR>"
		Response.Write "oDupACX= " & cmd.parameters("oDupACX").value & "<BR>"
		Response.Write "oDupAlt_ID_1= " & cmd.parameters("oDupAlt_ID_1").value & "<BR>"
		Response.Write "oDupAlt_ID_2= " & cmd.parameters("oDupAlt_ID_2").value & "<BR>"
		Response.Write "oDupAlt_ID_3= " & cmd.parameters("oDupAlt_ID_3").value & "<BR>"
		Response.Write "oDupAlt_ID_4= " & cmd.parameters("oDupAlt_ID_4").value & "<BR>"
		Response.Write "oDupAlt_ID_5= " & cmd.parameters("oDupAlt_ID_5").value & "<BR>"
		Response.Write "oCompoundID= " & cmd.parameters("oCompoundID").value & "<BR>"
		Response.Write "oIsExistingCompound= " & cmd.parameters("oISExistingCompound").value & "<BR>"
		Response.Write "oIsDuplicateCompound= " & cmd.parameters("oISDuplicateCompound").value & "<BR>"	
	end if
	' Increment substance count
	'If isEmpty(Application("inv_CompoundsRecordCountChemInv")) then
	'	Application.Lock
	'		Application("inv_Compounds" & "RecordCount" & "ChemInv") = GetSubstanceCount()
	'	Application.UnLock
	'end if
	'substanceCount = CLng(Application("inv_Compounds" & "RecordCount" & "ChemInv")) + 1
	'Application.Lock
	'	Application("inv_Compounds" & "RecordCount" & "ChemInv") = substanceCount
	'Application.UnLock
	
End if

	'Check for duplicate structure
	dupStrucID = -1
	if structure <> ""  AND Request("isEmptyStruc") <> "1" then

		dupID = DupStr
		If dupID > 0 AND dupID <> CompoundID then
			dupStrucID = dupID
			dupID = CStr(dupID)
			Call AddToDuplicatesDict(dupID, "STRUCTURE")
			Call AddToDuplicatesDict(Cstr(CompoundID), "STRUCTURE")
			Duplicates2_dict.Add "STRUCTURE", "1"
			bDuplicatesFound = true
		else
			Duplicates2_dict.Add "STRUCTURE", ""
		end if
	end if
	
	' Check for duplicate substance name	
	dupSNID = -2
	dupID = DupSN
	If dupID > 0 AND dupID <> CompoundID then
		dupSNID = dupID
		dupID = CStr(dupID)
		Call AddToDuplicatesDict(dupID, "inv_compounds.Substance_Name")
		Call AddToDuplicatesDict(Cstr(CompoundID), "inv_compounds.Substance_Name") 
		Duplicates2_dict.Add "inv_compounds.Substance_Name", SubstanceName
		bDuplicatesFound = true
	else
		Duplicates2_dict.Add "inv_compounds.Substance_Name", ""
	end if
	
	' Check for duplicate CAS
	dupCASID = -3
	dupID = DupCAS
	If dupID > 0 AND dupID <> CompoundID then
		dupCASID = dupID
		dupID = CStr(dupID)
		Call AddToDuplicatesDict(dupID, "inv_compounds.CAS")
		Call AddToDuplicatesDict(Cstr(CompoundID), "inv_compounds.CAS")
		Duplicates2_dict.Add "inv_compounds.CAS", CAS	 
		bDuplicatesFound = true
	else
		Duplicates2_dict.Add "inv_compounds.CAS", ""
	end if
	
	' Check for duplicate ACX_ID
	dupACXID = -4
	dupID = DupACX
	If dupID > 0 AND dupID <> CompoundID then
		dupACXID = dupID
		dupID = CStr(dupID)
		Call AddToDuplicatesDict(dupID, "inv_compounds.ACX_ID")
		Call AddToDuplicatesDict(Cstr(CompoundID), "inv_compounds.ACX_ID")
		Duplicates2_dict.Add "inv_compounds.ACX_ID", ACX_ID	 
		bDuplicatesFound = true
	else
		Duplicates2_dict.Add "inv_compounds.ACX_ID", ""
	end if
	
	For k = 1 to 5
		Duplicates2_dict.Add "inv_compounds.ALT_ID_" & cStr(k), ""
	Next
	
	'Check duplicate custom fields
	For each key in unique_alt_ids_dict
		cKey = Replace(Key,"inv_compounds.","")
		dupID = eval("Dup" & ckey)
		If dupID > 0 AND dupID <> CompoundID then
			dupID = CStr(dupID)
			Call AddToDuplicatesDict(dupID, key)
			Call AddToDuplicatesDict(Cstr(CompoundID), key)
			Duplicates2_dict.Item(Key)= Eval(cKey)
			bDuplicatesFound = true
		end if
	Next 

	For each key in Duplicates2_dict 
		if key <> "" then conflictingFields = conflictingFields & Duplicates2_dict.Item(key) & "|"
	Next
	conflictingFields = Left(conflictingFields, Len(conflictingFields)-1)
	
	Function GetSubstanceCount()
	dim RS

	GetInvConnection()
	sql = "SELECT count(*) as count from inv_compounds"
	Set RS = Conn.Execute(sql)
	theReturn = RS("count")
	Conn.Close
	Set Conn = nothing
	Set RS = nothing
	GetSubstanceCount = 0
end function
%>