<OBJECT RunAt="Server" Scope="Page" Id="Duplicates_dict" ProgID="Scripting.Dictionary"></OBJECT>
<OBJECT RunAt="Server" Scope="Page" Id="Duplicates2_dict" ProgID="Scripting.Dictionary"></OBJECT>
<%
Dim dBStructure
Dim dBCompoundID
Dim dupCompoundID
Dim dBCAS
Dim dBACX_ID
Dim dBDensity
Dim dBALT_ID_1
Dim dBALT_ID_2
Dim dBALT_ID_3
Dim dBALT_ID_4
Dim dBALT_ID_5
Dim dBSubstanceName
Dim conflictingFields
Dim inLineMarker
Dim CDCounter

CDCounter = 0
inLineMarker = "data:chemical/x-cdx;base64,"

Sub GetSubstanceAttributesFromDb(pCompoundID)
	Dim strWhere
	Dim j
	Dim BlankBase64CDX

	BlankBase64CDX=	"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0" &_
					"LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCA" &_
					"AAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAY" &_
					"IBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAw" &_
					"AAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQA" &_
					"BAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAA" &_
					"AACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA" &_
					"8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA"
	
	Call GetInvConnection()
	
	if NOT IsEmpty(pCompoundID) then 
		SQL = "SELECT inv_compounds.Compound_ID, inv_compounds.CAS, inv_compounds.ACX_ID, inv_compounds.density, inv_compounds.ALT_ID_1, inv_compounds.ALT_ID_2, inv_compounds.ALT_ID_3, inv_compounds.ALT_ID_4, inv_compounds.ALT_ID_5, inv_compounds.Substance_Name, inv_compounds.Conflicting_Fields, inv_compounds.base64_cdx FROM inv_compounds WHERE inv_compounds.Compound_ID= " & pCompoundID	
		'Response.Write sql
		'Response.end
		Set RS= Conn.Execute(SQL)
		if RS.BOF AND RS.EOF then
			Response.Write "<BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>"
			Response.Write "<P><CODE>Error:GetSubstanceAttributesFromDb</CODE></P>"
			Response.Write "<SPAN class=""GuiFeedback"">Could not retrieve substance data</SPAN>"
			Response.Write "</td></tr></table>"
			Response.end
		end if
	
		' Substance attributes
		dbStructure = RS("base64_cdx")
		dbRegisterIfConflicts = "false"
		dBCompoundID = Cstr(RS("Compound_ID").value)
		dBCAS = RS("CAS").value
		dBACX_ID = RS("ACX_ID").value
		dBDensity = RS("Density").value
		dBALT_ID_1 = RS("ALT_ID_1").value
		dBALT_ID_2 = RS("ALT_ID_2").value
		dBALT_ID_3 = RS("ALT_ID_3").value
		dBALT_ID_4 = RS("ALT_ID_4").value
		dBALT_ID_5 = RS("ALT_ID_5").value
		dBSubstanceName = RS("Substance_Name").value
		ConflictingFields = RS("Conflicting_Fields").value
		'Response.Write ConflictingFields & "<BR>"
		If ConflictingFields <> "" then
			dupCompoundID = dbCompoundID
			if Duplicates_dict.Exists(dBCompoundID) then Duplicates_dict.Remove(dBCompoundID)
			Duplicates_dict.Add dBCompoundID, ""
			Cf_array = split(ConflictingFields,"|")
			if cf_array(0) <> "" then
				Duplicates_dict.Item(dBCompoundID) =  Duplicates_dict.Item(dBCompoundID) & "STRUCTURE,"
			end if
			if cf_array(1) <> "" then
				'dBSubstanceName = cf_array(1)
				Duplicates_dict.Item(dBCompoundID) =  Duplicates_dict.Item(dBCompoundID) & "inv_compounds.Substance_Name,"
			end if
			if cf_array(2) <> "" then
				'dBCAS = cf_array(2)
				Duplicates_dict.Item(dBCompoundID) =  Duplicates_dict.Item(dBCompoundID) & "inv_compounds.CAS,"
			end if
			if cf_array(3) <> "" then
				'dBACX_ID = cf_array(3)
				Duplicates_dict.Item(dBCompoundID) =  Duplicates_dict.Item(dBCompoundID) & "inv_compounds.ACX_ID,"
			end if
			For j = 1 to 5
				if cf_array(j+3) <> "" then
					Duplicates_dict.Item(dBCompoundID) =  Duplicates_dict.Item(dBCompoundID) & "inv_compounds.ALT_ID_" & Cstr(j) & "," 
					'Execute("dBALT_ID_" & Cstr(j) & " = """ & cf_array(j+2) & """")
				end if
			Next
			Duplicates_dict.Item(dBCompoundID) =  Left(Duplicates_dict.Item(dBCompoundID), Len(Duplicates_dict.Item(dBCompoundID))-1)
			'Response.Write dBCompoundID & "-"  & Duplicates_dict.Item(dBCompoundID) & "<BR>"
		End if
	Else
		dbStructure = BlankBase64CDX
		dbRegisterIfConflicts = "false"
		dBCompoundID = "0"
		dBCAS = ""
		dBACX_ID = ""
		dBDensity = "1"
		dBALT_ID_1 = ""
		dBALT_ID_2 = ""
		dBALT_ID_3 = ""
		dBALT_ID_4 = ""
		dBALT_ID_5 = ""
		dBSubstanceName = ""
		ConflictingFields = ""
	End if
End Sub



' Returns a compound_id from the compounds table matching the given unique field
' name and field value.  Use only fields that have been defined with a unique constraint
' Returns 0 when no match is found
Function GetCompoundID(FieldName, FieldValue)
	Dim CompoundID
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GetCompoundId", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",131, 4, 0, 0)
	Cmd.Parameters("RETURN_VALUE").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELDNAME",200, 1, 255, FieldName)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELDVALUE",200, 1, 255, FieldValue)
	'Response.Write FieldName & "=" & FieldValue & "<BR>"
	Cmd.Execute
	CompoundID = CLng(Cmd.Parameters("RETURN_VALUE").value)	
	GetCompoundID = CompoundID 
	'Response.Write CompoundID
	'Response.End
End function
	

' Checks if a given compoundid is from a duplicate. A duplicate compound is one
' for which conflicting fields exists.
Function IsDuplicateCompound(compoundID)
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".IsDuplicateCompound", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",131, 4, 0, 0)
	Cmd.Parameters.Append Cmd.CreateParameter("PCOMPOUNDID",131, 1, 0, cLng(compoundID))
	Cmd.Execute
	IsDuplicateCompound = Cbool(Cmd.Parameters("RETURN_VALUE").value)	 
	'Response.Write CompoundID
	'Response.End
End function

	
Function GetCompoundIDFromStructure(pBase64cdx)		
	Dim CompoundID
	ServerName = Application("InvServerName")
	Target = "/cheminv/cheminv/cheminv_action.asp?killSession=1&return_data=CSV&formgroup=substances_form_group&dataaction=search&dbname=cheminv"
	FormData = "metadata_directive=blind&inv_compounds.Structure=" & Server.UrlEncode(pBase64cdx) 
	FormData = FormData & "&inv_compounds.Structure.sstype=1" ' Exact search for dup check
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
	FormData = FormData & Credentials
	'Response.Write FormData
	'Response.end
	httpResponse = CShttpRequest2("POST", ServerName, Target, "ChemInv", FormData)
	'Response.Write httpresponse
	'Response.end
	if httpResponse = "no_records_found" then
		CompoundID = 0
	else
		if IsNumeric(left(httpResponse,1)) then
			tempArr = split(httpResponse, ",")
			CompoundID = tempArr(0)
			if IsDuplicateCompound(CompoundID) then
				CompoundID = 0
			End if
		Else
			Response.Write "Error from CreateSubstance2:GetCompoundIdFomStructure:<BR>"
			Response.Write httpResponse
		End if
	End if
	GetCompoundIDFromStructure = CompoundID
End function
	

Sub DisplaySubstance(Caption, Header, bShowSelect, bShowCreateDuplicate, bShowEdit, bShowEditExisting, bShowConflicts, InLineCdx)
	Dim TempCdxPath
	
	TempCdxPath = Application("TempFileDirectoryHTTP" & "cheminv")

	Response.Write "<center>" & vblf
	Response.Write "	<span class=""GuiFeedback"">" & vblf
	Response.Write			Caption	
	Response.Write "	</span>" & vblf
	Response.Write "	<table border=""0"">" & vblf 
	Response.Write "	<tr>" & vblf
	Response.Write "		<th colspan=""2"">" & vblf
	Response.Write "			<table border=0 cellspacing=0 cellpadding=0 width=""100%"">" & vblf
	Response.Write "				<tr>" & vblf  
	Response.Write "					<td align=center>" & vblf   				
	Response.Write							"<b>" & Header & "</b>" & vblf
	Response.Write "						<input type=hidden name=""SN_" & dBCompoundID & """ value=""" & dBSubstanceName & """>" & vblf
	Response.Write "					</td>" & vblf
	Response.Write "					<td align=right>" & vblf   				
	if NOT Session("bManageMode") then
		if lcase(action)="showconflicts"  AND Header <> "Conflicting Substance" then
			Response.Write "	<a Class=MenuLink href=""/cheminv/gui/DeleteSubstance_action.asp?SelectSN=" & dbSubstanceName & "&SelectCompoundID=" & dbCompoundID & "&CompoundID=" & CompoundID &""">Select and Delete Dup</a> | " & vblf
		Else
			if bShowSelect then Response.Write "	<a Class=MenuLink href=""#"" onClick=""DoSelectSubstance(" & dBCompoundID& ",document.form1['SN_" & dBCompoundID &  "'].value)"">Select</a> | " & vblf
		End if
	End if
	if bShowCreateDuplicate then Response.Write " 	<a Class=MenuLink href=""#"" onClick=""document.repostForm.action = 'CreateSubstance2.asp?RegisterIfConflicts=true';document.repostForm.submit();return false;"">Create Duplicate</a> | " & vblf
	If Session("INV_MANAGE_SUBSTANCES" & "cheminv") then
		if bShowEdit then Response.Write "		<a Class=MenuLink href=""#"" onclick=""document.repostForm.action = 'CreateOrEditSubstance.asp';document.repostForm.submit();return false;"">Edit</a>" & vblf
		if bShowEditExisting then 
			if NOT bSameWindow then
				Response.Write "		<a Class=MenuLink href=""#"" onclick=""OpenDialog('CreateOrEditSubstance.asp?action=edit&CompoundID=" & dBCompoundID & "', 'SubsManager_" & dBCompoundID &  "', 2);return false"">Edit</a>" & vblf		
			Else
				Response.Write "		<a Class=MenuLink href=""CreateOrEditSubstance.asp?action=edit&CompoundID=" & dBCompoundID & """>Edit</a>" & vblf		
			End if
		End if
		if bShowConflicts then 
			Response.Write "		<a Class=MenuLink href=""#"" onclick=""OpenDialog('CreateOrEditContainer.asp?TB=Required&editsubstance=true&isEdit=true&getData=db&amp;ContainerID=" & ContainerID & "&CompoundID=" & dBCompoundID & "', 'SubsManager_" & dBCompoundID &  "', 2);return false"">Edit</a>" & vblf		
			Response.Write "	|	<a Class=MenuLink href=""#"" onclick=""OpenDialog('CreateOrEditContainer.asp?TB=Required&showconflicts=1&isEdit=true&getData=db&amp;ContainerID=" & ContainerID & "', 'ShowConflicts', 2); return false"">Show Conflicts</a>" & vblf	
		End if
	End if
	Response.Write "					</td>" & vblf 
	   
	Response.Write "				</tr>" & vblf
	Response.Write "			</table>" & vblf  
	Response.Write "		</th>" & vblf
	Response.Write "	</tr>"	& vblf
	Response.Write "	<tr>" & vblf
	Response.Write "		<td>" & vblf
	if IsConfictingField(dBCompoundID, "STRUCTURE") then bordercolor = "red"
	Response.Write "			<table border=""1"" bordercolor=""" & bordercolor & """>" & vblf
	Response.Write "				<tr>" & vblf
	
	Response.Write "					<td>" & vblf										
	if Session("isCDP") = "TRUE" then
		Response.Write "						<input type=""hidden"" name=""inline_" & dbCompoundID & """ value=""" & InLineCdx &""">" & vblf
		Response.Write "						<" & "script language=""JavaScript"">" & vblf
		Response.Write "							cd_insertObject(""chemical/x-cdx"", ""185"", ""130"", ""CD_" & dbCompoundID & """, """ & TempCdxPath & "mt.cdx"", ""true"", ""true"", escape(document.all.inline_" & dbCompoundID & ".value),  ""true"")" & vblf 
		Response.Write "						</" & "script>"
	else
		SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
		filePath = SessionDir & "structure" & "_" & dBCompoundID & "_" & 160 & "x" & 140 & ".gif"	
		SessionURLDir = Application("TempFileDirectoryHTTP" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
		fileURL = "/chemoffice" & SessionURLDir & "structure" & "_" & dBCompoundID & "_" & 160 & "x" & 140 & ".gif"	
		'Response.Write(filePath & "<br>" & fileURL)
		ConvertCDXtoGif_Inv filePath, InLineCdx, 160, 140
		Response.Write "<img src=""" & fileURL & """ width=""160"" height=""140"" border=""0"">"
	end if
	Response.Write "					</td>" & vblf
	Response.Write "				</tr>" & vblf
	Response.Write "			</table>" & vblf
	Response.Write "		</td>" & vblf
	Response.Write "		<td valign=""top"">" & vblf
	Response.Write "			<table border=""0"" cellpadding=""1"" cellspacing=""2"">" & vblf
	Response.Write "				<tr>" & vblf
	Response.Write "					<td colspan=""4"" align=""center"">" & vblf
	Response.Write "						&nbsp;<em><b>" & HighlightConflictingField(dBCompoundID, "inv_compounds.Substance_Name", TruncateInSpan(dBSubstanceName, 50, "")) & "</b></em>" & vblf
	Response.Write "					</td>" & vblf
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
		
	Response.Write						ShowField("Molecular Weight:", "MOLWEIGHT", 15, "MOLWEIGHT" & CDCounter)
	
	Response.Write						ShowField("CompoundID:", "dBCompoundID", 15, "")					
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
	
	Response.Write						ShowField("Molecular Formula:", "FORMULA", 15, "FORMULA" & CDCounter)
	
	Response.Write						ShowField(HighlightConflictingField(dBCompoundID, "inv_compounds.CAS", "CAS Number:"), "dBCAS", 15, "")
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
	Response.Write "					<td></td><td></td>" & vblf
	
	Response.Write						ShowField(HighlightConflictingField(dBCompoundID, "inv_compounds.ACX_ID", "ACX ID:"), "dBACX_ID", 15, "")
	Response.Write "				</tr>" & vblf
					
	j= 0
	Response.Write					"<TR>" & vblf
					For each key in alt_ids_dict
	Response.write						ShowField(HighlightConflictingField(dBCompoundID, Key, alt_ids_dict.Item(Key)), "dB" & Replace(Key, "inv_compounds.",""), 20, "") & vblf
	j = j + 1
	if (j/2 - int(j/2)) = 0 then Response.Write "</TR>" & vblf & "<tr>" & vblf
					Next 
	Response.Write "			</table>" & vblf
	Response.Write "		</td>" & vblf
	Response.Write "	</tr>" & vblf
	Response.Write "</table>" & vblf
	Response.Write "</center>" & vblf
	CDCounter = CDCounter + 1
End sub

Function IsConfictingField(dupCpdID, FieldName)
	Dim matchingFields
	
	If NOT IsObject(Duplicates_dict) then 
		IsConfictingField = false
		Exit function
	End if
	matchingFields = "," & lCase(Duplicates_dict.Item(CStr(dupCpdID))) & ","
	'Response.Write dupCpdID & "-" & fieldName & "-" & matchingFields & "<BR>"
	If inStr(1, matchingFields, "," & lcase(FieldName) & ",") > 0 then
		IsConfictingField = true
	Else
		IsConfictingField = false
	End if
	
End Function

Function HighlightConflictingField(dupCpdID, FieldName, FieldText)
	Dim str
	if IsConfictingField(dupCpdID, FieldName) then
		str = "<font color=red><b>" & FieldText & "</b></font>"
	else
		str = FieldText
	End if
	HighlightConflictingField = str
End function

function RemoveConflictingFieldsFromRequest(strForm)
	Dim cc
	Dim fname
	Dim i
	
	For each key in Duplicates2_dict
		'Response.Write "Duplicate " & Duplicates_dict.Item(key) & " found at " & key & "<BR>"
		strForm = Replace(strForm, "Duplicate_Search_No_Add" , "")
		if len(Key) > 0 then
				nameValuePair = "&" & key & "=" & Request(key)
				'Response.Write nameValuePair & "<BR>"
				'if key <> "STRUCTURE" then strForm = Replace(strForm, nameValuePair, "&" & key & "=")
		end if
	Next
	'Response.Write strForm & conflictingFields
	'Response.End
	RemoveConflictingFieldsFromRequest = strForm & conflictingFields
end function

Sub AddToDuplicatesDict(ID, value)
	if Duplicates_dict.Exists(ID)then
		Duplicates_dict.Item(ID) = Duplicates_dict.Item(ID) & "," & value 
	Else
		Duplicates_dict.Add ID, Trim(value)
	End if
End Sub

Function GetPreExistingSubstanceID()
		'Response.Write "dupACXID= " & dupACXID & "<BR>"
		'Response.Write "dupStrucID= " & dupStrucID & "<BR>" 
		'Response.Write "dupCASID= " & dupCASID & "<BR>" 
		'Response.Write "dupSNID= " & dupSNID & "<BR>" 
		Select Case true
			Case (dupACXID > 0)
				GetPreExistingSubstanceID = dupACXID
			Case (dupStrucID = dupCASID)
				GetPreExistingSubstanceID = dupStrucID
			Case (dupStrucID = dupSNID)
				GetPreExistingSubstanceID = dupStrucID
			Case (dupCASID = dupSNID)
				GetPreExistingSubstanceID = dupCASID
			Case default
				GetPreExistingSubstanceID = 0
		End select		
end function

'''''''''''''''''''''''''''''''''''''''''''''''
'Function ConvertStrToArray(str, segmentLength)
'Breaks a string into segments and returns and  
'an array of string segments
Function ConvertStrToArray(str, segmentLength)
	dim n, pos, i, si, buff, del
	
	del = "|~"
	n = Len(str) 'number of characters
	pos = 1 'starting position
	i = 0 'starting index		
	while pos < n 
		si = Mid(str, pos, segmentLength)
		if buff <> "" then
			buff = buff & del & si
		else
			buff = si
		end if
		pos = pos + segmentLength 
		i = i + 1 
	wend
	'add an extra delimeter to the end to ensure a minimum of two segments
	if n <= segmentLength then buff = buff & del
	ConvertStrToArray = split(buff, del)	
	'ConvertStrToArray = buff
End function

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'GetMoleculeParamsAsString(MoleculeAsArray byRef)
'Returns string with text required to pass a molecule to
'the cartridge as parametrized ADO operators.
'It uses the xarray constructor syntax if needed.  
Function GetMoleculeParamsAsString(byRef MoleculeAsArray)
	Dim numSegments, buff, i
	
	numSegments = Ubound(MoleculeAsArray) + 1
	if numSegments = 2 then
		buff = "?,?"
	Else
		buff = "CsCartridge.XArray("
		For i = 1 to numSegments -1
			buff = buff & "?,"
		Next
		buff = buff & "?)"
	 End if
	 GetMoleculeParamsAsString = buff
End function

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'AddMoleculeParametersToADOCommand(cmd byref, MoleculeAsArray byRef)
'Creates appends and populates parameters used to pass a molecule to
'the the cscartridge using ADO.
'Since Oracle parameters must be passed in the expected order, this  
'sub needs to be called at the appriate place in the parameter population
'process
Sub AddMoleculeParametersToADOCommand(byref cmd, byRef MoleculeAsArray)
	Dim i, parValue
	
	For i = 0 to Ubound(MoleculeAsArray)
		Cmd.Parameters.Append Cmd.CreateParameter("xpar" & i, adLongVarchar, adParamInput, Len(MoleculeAsArray(i)) + 1, MoleculeAsArray(i))
	Next
End sub
%>
