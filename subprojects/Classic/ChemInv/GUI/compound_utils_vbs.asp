<OBJECT RunAt="Server" Scope="Page" Id="Duplicates_dict" ProgID="Scripting.Dictionary"></OBJECT>
<OBJECT RunAt="Server" Scope="Page" Id="Duplicates2_dict" ProgID="Scripting.Dictionary"></OBJECT>
<!-- WJC unfortunately we cannot include ado.inc here because it is not protected against multiple includes -->
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
' WJC Millennium To hold the Null Default values
Dim dBALT_ID_1_Default
Dim dBALT_ID_2_Default
Dim dBALT_ID_3_Default
Dim dBALT_ID_4_Default
Dim dBALT_ID_5_Default
' WJC Millennium To hold the KeyId values
Dim dBALT_ID_1_KeyId
Dim dBALT_ID_2_KeyId
Dim dBALT_ID_3_KeyId
Dim dBALT_ID_4_KeyId
Dim dBALT_ID_5_KeyId
Dim dBSubstanceName
Dim conflictingFields
Dim inLineMarker
Dim CDCounter
Dim Location_Description
Dim LocationTypeIDFK
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
		SQL = "SELECT inv_compounds.Compound_ID, inv_compounds.CAS, inv_compounds.ACX_ID, inv_compounds.density, inv_compounds.ALT_ID_1, inv_compounds.ALT_ID_2, inv_compounds.ALT_ID_3, inv_compounds.ALT_ID_4, inv_compounds.ALT_ID_5, inv_compounds.Substance_Name, inv_compounds.Conflicting_Fields, inv_compounds.base64_cdx,CHEMINVDB2.AUTHORITY.GetLocationtype(inv_compounds.LOCATION_TYPE_ID_FK) AS Location_Description,inv_compounds.LOCATION_TYPE_ID_FK FROM inv_compounds WHERE inv_compounds.Compound_ID= " & pCompoundID	
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
		Location_Description = RS("Location_Description").value
		LocationTypeIDFK = RS("LOCATION_TYPE_ID_FK").value
		dBCAS = RS("CAS").value
		Session("CAS")= dBCAS
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
	CustomFieldsValue_CompoundGet(pCompoundID)  ' WJC Millennium load custom field values
    AltFieldsValue_CompoundGet()                ' WJC Millenium map custom alt_id values
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
	Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
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


	'BD 5/7/07 Decide which version of CreateSubstance.asp to use, createsubstance3.asp is used for ELN->INV integration
	dim CreateSubstanceFileName	
	If isCreateSubstance3 then
            CreateSubstanceFileName = "CreateSubstance3.asp"
	Else
            CreateSubstanceFileName = "CreateSubstance2.asp"
	End if
	'BD 5/07/07 modified if statement to use the CreateSubstancefileName variable set above
	if bShowCreateDuplicate then Response.Write " <a Class=MenuLink href=""#"" onClick=""document.repostForm.action = '"  & CreateSubstanceFileName  &  "?RegisterIfConflicts=true';document.repostForm.submit();return false;"">Create Duplicate</a> | " & vblf
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
	if Session("isCDP") = "TRUE" and detectModernBrowser = false then
		if session("DrawPref") = "ISIS" then
			ISISDraw = """True"""
        else
			ISISDraw = """False"""
		end if
		Response.Write "						<input type=""hidden"" name=""inline_" & dbCompoundID & """ value=""" & InLineCdx &""">" & vblf
		Response.Write "						<" & "script language=""JavaScript"">" & vblf
		Response.Write "							cd_insertObject(""chemical/x-cdx"", ""185"", ""130"", ""CD_" & dbCompoundID & """, """ & TempCdxPath & "mt.cdx"", ""true"", ""true"", escape(document.all.inline_" & dbCompoundID & ".value),  ""true""," & ISISDraw & ")" & vblf 
		Response.Write "						</" & "script>"
	else
		SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & Session.sessionid & "\"
		filePath = SessionDir & "structure" & "_" & 160 & "x" & 140 & ".gif"	
		SessionURLDir = Application("TempFileDirectoryHTTP" & "ChemInv") & "Sessiondir"  & "/" & Session.sessionid & "/"
		fileURL = SessionURLDir & "structure" & "_" & 160 & "x" & 140 & ".gif"	
		ConvertCDXtoGif_Inv filePath, Mid(InLineCdx, InStr(InLineCdx, "VmpD")), 160, 140
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
	Response.Write "						&nbsp;<em><b>" & HighlightConflictingField(dBCompoundID, "inv_compounds.Substance_Name", dBSubstanceName) & "</b></em>" & vblf
	Response.Write "					</td>" & vblf
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
	Response.Write	"<td align=right valign=top nowrap>Molecular Weight:</td><td class=""grayBackground"" align=right><span>" & Round(ConvertBase64toMW(Mid(InLineCdx, InStr(InLineCdx, "VmpD"))),3) & "</span></td>" & vblf

	Response.Write						ShowField("CompoundID:", "dBCompoundID", 15, "")					
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
	Response.Write	"<td align=right valign=top nowrap>Molecular Formula:</td><td class=""grayBackground"" align=right><span>" & ConvertBase64toMFormula(Mid(InLineCdx, InStr(InLineCdx, "VmpD"))) & "</span></td>" & vblf

	Response.Write						ShowField(HighlightConflictingField(dBCompoundID, "inv_compounds.CAS", "CAS Number:"), "dBCAS", 15, "")
	Response.Write "				</tr>" & vblf
	Response.Write "				<tr>" & vblf
	
	if Application("ENABLE_OWNERSHIP")="TRUE" then
	
	Response.Write						ShowField("Compound Location Type:", "Location_Description", 15, "")	
	END IF
	Response.Write						ShowField(HighlightConflictingField(dBCompoundID, "inv_compounds.ACX_ID", "ACX ID:"), "dBACX_ID", 15, "")
	Response.Write "				</tr>" & vblf
					
	j= 0
	Response.Write					"<TR>" & vblf
					For each key in alt_ids_dict
					    strLabel = alt_ids_dict.Item(Key)
					    if left(strLabel, 1) = "*" then strLabel = mid(strLabel,2)  ' WJC Millennium
	Response.write						ShowField(HighlightConflictingField(dBCompoundID, Key, strLabel) & ":", "dB" & Replace(Key, "inv_compounds.",""), 40, "")& vblf
	j = j + 1
	if (j/2 - int(j/2)) = 0 then Response.Write "</TR>" & vblf & "<tr>" & vblf
					Next 
	bShowBooleanAsList = True	' WJC future configuration option. If true show only a list of the non-default values
	Response.Write "</tr></Table></Table>" ' CSBR-158319: close the structure and substance data
	'CSBR-158722: Added the width and set the layout fixed
	Response.Write "<Table width=600 style='table-layout:fixed'>" ' CSBR-158319: open the custom fields in another table
	if bShowBooleanAsList <> False then
		strTab = ""
		strList = ""
		for each key in custom_fields_value_dict
			if CustomField_GetCategory(key) <> "" then	' WJC Note incorrect test for "boolean"
			'CSBR-158685: Display the Safety Data in the substance tab based on configuration
				if not (lcase(Application("DISPLAY_SAFETY_DATA"))="false" AND CustomField_GetCustomFieldGroupTypeName(key) = "Safety Data") then
					if (custom_fields_value_dict(key) <> CustomField_GetDefault(key)) then
						if strTab <> CustomField_GetCategory(key) then
							if strTab <> "" then
								'strList = Replace(strList, " ", "&nbsp;")
								strList = Replace(strList, Chr(255), ", ")
								Response.Write strList
								Response.Write "                    </td>" & vblf
							end if
							strTab = CustomField_GetCategory(key)
							strList = ""
							Response.Write "				<tr>" & vblf
							Response.Write "                    <td align='center' colspan='4'>&nbsp;<BR><u>" & strTab & "</u></td>" & vblf
							Response.Write "				</tr>" & vblf
							Response.Write "                    <td colspan='4'>" & vblf
						end if
						if strList <> "" then 
							if Instr(lcase(strTab),"sentence")>0 then
								strList= strList & "<BR>"
							else
								strList = strList & Chr(255)
							end if
						end if	
						'CSBR-158722: Don't replace the space inside the key with nbsp; the long text will not wrap
						strList = strList & ShowCustomSymbol(key) & key
					end if
				end if
			end if
		next
		if strTab <> "" then
			'strList = Replace(strList, " ", "&nbsp;")
			strList = Replace(strList, Chr(255), ", ")
			Response.Write strList
			Response.Write "                    </td>" & vblf
		end if
	end if

	strTab = ""
	nCol = 0
	nCols = 2
	for each key in custom_fields_value_dict
	    if (bShowBooleanAsList = False) And (custom_fields_value_dict(key) <> CustomField_GetDefault(key)) then
			'CSBR-158685: Display the Safety Data in the substance tab based on configuration
			if not (lcase(Application("DISPLAY_SAFETY_DATA"))="false" AND CustomField_GetCustomFieldGroupTypeName(key) = "Safety Data") then
				if strTab <> CustomField_GetCategory(key) then
					strTab = CustomField_GetCategory(key)
					if nCol <> 0 then
						while nCol <> 0
							Response.Write "                    <td></td>" & vblf
							nCol = (nCol + 1) Mod nCols
						wend
						Response.Write "				</tr>" & vblf
					end if
					Response.Write "				<tr>" & vblf
					Response.Write "                    <td align='center' colspan='4'>&nbsp;<BR><u>" & strTab & "</u></td>" & vblf
					Response.Write "				</tr>" & vblf
				end if
				if nCol = 0 then Response.Write "				<tr>" & vblf
				Response.Write "				    <td align=right valign=top nowrap>"
				Response.Write key & ":"                
				Response.Write "</td>" & vblf
				Response.Write "				    <td class='grayBackground' align=right>"
				Response.Write custom_fields_value_dict(key)
				Response.Write "</td>" & vblf
			
				if Instr(Lcase(strTab),"sentence")>0 then
					nCol=0
				else
					nCol = (nCol + 1) Mod nCols
				end if
				if nCol = 0 then Response.Write "				</tr>" & vblf
			end if
	    end if
	Next
    if nCol <> 0 then
        while nCol <> 0
            Response.Write "                    <td></td>" & vblf
            nCol = (nCol + 1) Mod nCols
        wend
        Response.Write "				</tr>" & vblf
    end if
	Response.Write "		</td>" & vblf
	Response.Write "	</tr>" & vblf
	Response.Write "</table>" & vblf
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
			Case Else
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

' WJC Millennium get category (custom_field_group_name)
function CustomField_GetCategory(key)
    CustomField_GetCategory = custom_fields_info_dict(key)(3)
End function    ' CustomField_GetCategory()

' WJC Millennium get default value (picklist_display)
function CustomField_GetDefault(key)
    CustomField_GetDefault = custom_fields_info_dict(key)(2)
End function    ' CustomField_GetDefault()

' WJC Millennium get key id (custom_field_id)
function CustomField_GetKeyId(key)
    CustomField_GetKeyId = custom_fields_info_dict(key)(0)
End function    ' CustomField_GetKeyId()

' WJC Millennium get picklist type id (picklist_type_id_fk)
function CustomField_GetPicklistId(key)
    CustomField_GetPicklistId = custom_fields_info_dict(key)(1)
End function    ' CustomField_GetPicklistId()

' WJC Millennium get readonly (readonly)
function CustomField_GetReadonly(key)
    CustomField_GetReadonly = custom_fields_info_dict(key)(4)
End function    ' CustomField_GetReadonly()

' CSBR-158486: Added new function to get the CustomFieldGroupTypeName from the array list
function CustomField_GetCustomFieldGroupTypeName(key)
    CustomField_GetCustomFieldGroupTypeName = custom_fields_info_dict(key)(5)
End function ' CustomField_GetCustomFieldGroupTypeName()
' WJC Millennium build custom_fields_info_dict
Sub CustomFieldsInfo_Construct()
	Dim RS
    GetInvConnection()
	
' CSBR-158486: Added CUSTOM_FIELD_GROUP_TYPE_NAME to the query
' CSBR-159547: Added Custom_Field_Group_ID to the order by to order in Old data first then new safety data
	Sql = "SELECT custom_field_name,custom_field_id,picklist_type_id_fk,picklist_display,custom_field_group_name,readonly,CUSTOM_FIELD_GROUP_TYPE_NAME FROM ChemInvDb2.Inv_Custom_Fields JOIN ChemInvDb2.Inv_Picklists ON (Inv_Custom_Fields.NULL_PICKLIST_ID_FK = Inv_Picklists.PICKLIST_ID) JOIN ChemInvDb2.Inv_Custom_Field_Groups ON (Inv_Custom_Fields.CUSTOM_FIELD_GROUP_ID_FK = Inv_Custom_Field_Groups.CUSTOM_FIELD_GROUP_ID) JOIN ChemInvDb2.INV_CUSTOM_FIELD_GROUP_TYPES ON (INV_CUSTOM_FIELD_GROUPS.CUSTOM_FIELD_GROUP_TYPE_ID_FK = INV_CUSTOM_FIELD_GROUP_TYPES.CUSTOM_FIELD_GROUP_TYPE_ID) WHERE (CUSTOM_FIELD_GROUP_ID_FK IS NOT NULL) ORDER BY Custom_Field_Group_ID,CUSTOM_FIELD_ID" 
	Set RS = Conn.Execute(Sql)
	
	custom_fields_info_dict.RemoveAll
	While NOT RS.EOF
' CSBR-158486: Added CUSTOM_FIELD_GROUP_TYPE_NAME to the array list
	    custom_fields_info_dict.Add RS("custom_field_name").value, Array(RS("custom_field_id").value, RS("picklist_type_id_fk").value, RS("picklist_display").value, RS("custom_field_group_name").value, RS("readonly").value, RS("CUSTOM_FIELD_GROUP_TYPE_NAME").value)
		RS.MoveNext
	Wend
	Set RS = Nothing
    Conn.Close
End sub ' CustomFieldsInfo_Construct()

' WJC Millennium build category (tab) list
' Assumes custom_fields_info_dict is sorted by category
function CustomFieldsInfo_GetCategories()
	Dim RS
    GetInvConnection()
	
'CSBR-158685: Added CUSTOM_FIELD_GROUP_TYPE_NAME to the query
'CSBR-158885: Added Custom_Field_Group_ID to the query and ordering by this to display in the required order
	Sql = "SELECT DISTINCT custom_field_group_name,CUSTOM_FIELD_GROUP_TYPE_NAME, Custom_Field_Group_ID FROM ChemInvDb2.Inv_Custom_field_groups JOIN ChemInvDb2.Inv_Custom_fields ON (Custom_Field_Group_ID_FK = Custom_Field_Group_ID) JOIN ChemInvDb2.INV_CUSTOM_FIELD_GROUP_TYPES ON (INV_CUSTOM_FIELD_GROUPS.CUSTOM_FIELD_GROUP_TYPE_ID_FK = INV_CUSTOM_FIELD_GROUP_TYPES.CUSTOM_FIELD_GROUP_TYPE_ID) ORDER BY Custom_Field_Group_ID" 
	Set RS = Conn.Execute(Sql)
	
	Dim return_dict
	set return_dict = Server.CreateObject("Scripting.Dictionary")
	While NOT RS.EOF
'CSBR-158685: Make the dict as an array and add CUSTOM_FIELD_GROUP_TYPE_NAME to the array list
	    return_dict.Add RS("custom_field_group_name").value, Array(RS("custom_field_group_name").value, RS("CUSTOM_FIELD_GROUP_TYPE_NAME").value)
		RS.MoveNext
	Wend
	Set RS = Nothing
    Conn.Close
    Set CustomFieldsInfo_GetCategories = return_dict
End function    ' CustomFieldsInfo_GetCategories()

' WJC Millennium load custom field values for a compound
sub CustomFieldsValue_CompoundGet(nCompoundID)
    if custom_fields_info_dict.Count = 0 then
        CustomFieldsInfo_Construct()
    end if
	custom_fields_value_dict.RemoveAll
    for each key in custom_fields_info_dict
        custom_fields_value_dict.Add key, CustomField_GetDefault(key)
    next
    if NOT IsEmpty(nCompoundID) then
	    Dim RS
        GetInvConnection()
	    Set cmd = Server.CreateObject("ADODB.Command")
	    cmd.ActiveConnection = Conn
'WJC    cmd.CommandType = adCmdText
	    cmd.CommandType = 1
        Sql = "SELECT custom_field_name,picklist_display FROM ChemInvDb2.Inv_Custom_Fields JOIN ChemInvDb2.Inv_Custom_Cpd_Field_Values ON Inv_Custom_Fields.Custom_Field_ID = Inv_Custom_Cpd_Field_Values.Custom_Field_ID_FK JOIN ChemInvDb2.Inv_Picklists ON (Inv_Custom_Cpd_Field_Values.PICKLIST_ID_FK = Inv_Picklists.PICKLIST_ID) WHERE Custom_Field_Group_ID_FK IS NOT NULL AND Compound_ID_FK = ?"
	    cmd.CommandText = sql
'WJC    cmd.Parameters.Append cmd.CreateParameter("compound", adNumeric, adParamInput, 0, nCompoundID)
	    cmd.Parameters.Append cmd.CreateParameter("compound", 131, 1, 0, nCompoundID)
	    Set RS = cmd.Execute
	    While NOT RS.EOF
	        custom_fields_value_dict.item(RS("custom_field_name").value) = RS("picklist_display").value
		    RS.MoveNext
	    Wend
	    Set RS = Nothing
	    set cmd = Nothing
        Conn.Close
    end if
End sub ' CustomFieldsValue_CompoundGet()

' WJC Millennium get default value (null_picklist_ID_FK)
function AltField_GetDefault(key)
    strLabel = alt_ids_dict.Item(Key)
    strLabel = mid(strLabel, 2)
    strVar = replace(key, "inv_compounds.","dB")
    if Eval(strVar & "_Default") = "" then
        ' Fetch and save Null value
        Set cmd = Server.CreateObject("ADODB.Command")
        cmd.ActiveConnection = Conn
'WJC    cmd.CommandType = adCmdText
	    cmd.CommandType = 1
        Sql = "SELECT Null_Picklist_ID_FK FROM ChemInvDb2.Inv_Custom_Fields WHERE Custom_Field_Name = ?"
        cmd.CommandText = sql
'WJC    cmd.Parameters.Append cmd.CreateParameter("Custom_Field_Name", adLongVarChar, adParamInput, len(strLabel) + 1, strLabel)
        cmd.Parameters.Append cmd.CreateParameter("Custom_Field_Name", 201, 1, len(strLabel) + 1, strLabel)
        Set RS = cmd.Execute
        if NOT RS.EOF then nPicklistID = Clng(RS("Null_Picklist_ID_FK").value) else nPicklistID = 0
        Set RS = Nothing
        set cmd = Nothing
        execute strVar & "_Default = nPicklistID"
    end if
    AltField_GetDefault = Eval(strVar & "_Default")
End function    ' AltField_GetDefault()

' WJC Millennium get default value (null_picklist_ID_FK)
function AltField_GetKeyId(key)
    strLabel = alt_ids_dict.Item(Key)
    strLabel = mid(strLabel, 2)
    strVar = replace(key, "inv_compounds.","dB")
    if Eval(strVar & "_KeyId") = "" then
        ' Fetch and save Null value
        Set cmd = Server.CreateObject("ADODB.Command")
        cmd.ActiveConnection = Conn
        cmd.CommandType = adCmdText
        Sql = "SELECT Custom_Field_ID FROM ChemInvDb2.Inv_Custom_Fields WHERE Custom_Field_Name = ?"
        cmd.CommandText = sql
        cmd.Parameters.Append cmd.CreateParameter("Custom_Field_Name", adLongVarChar, adParamInput, len(strLabel) + 1, strLabel)
        Set RS = cmd.Execute
        if NOT RS.EOF then nPicklistID = Clng(RS("Custom_Field_ID").value) else nPicklistID = 0
        Set RS = Nothing
        set cmd = Nothing
        execute strVar & "_KeyId = nPicklistID"
    end if
    AltField_GetKeyId = Eval(strVar & "_KeyId")
End function    ' AltField_GetKeyId()

' WJC Millennium map custom alt values from ID to display
sub AltFieldsValue_CompoundGet()
	Dim RS
    GetInvConnection()
	for each key in alt_ids_dict
	    if left(alt_ids_dict.item(key),1) = "*" then
            strLabel = alt_ids_dict.Item(Key)
            strLabel = mid(strLabel, 2)
	        strVar = replace(key, "inv_compounds.","dB")
            ' Override if field is not Null
	        if Eval(strVar) <> "" then nPicklistID = CLng(Eval(strVar)) else nPicklistID = AltField_GetDefault(key)
	        if nPicklistID > 0 then ' Paranoid, should always be true
	            Set cmd = Server.CreateObject("ADODB.Command")
	            cmd.ActiveConnection = Conn
'WJC            cmd.CommandType = adCmdText
	            cmd.CommandType = 1
                Sql = "SELECT Decode(Picklist_Code,'--','',Picklist_Code||' - ')||picklist_display AS DisplayText FROM ChemInvDb2.Inv_Picklists WHERE Picklist_ID = ?"
	            cmd.CommandText = sql
'WJC            cmd.Parameters.Append cmd.CreateParameter("Picklist_ID", adNumeric, adParamInput, 0, nPicklistID)
	            cmd.Parameters.Append cmd.CreateParameter("Picklist_ID", 131, 1, 0, nPicklistID)
	            Set RS = cmd.Execute
	            if RS.EOF then execute strVar & " = """"" else execute strVar & " = RS(""DisplayText"").value"
	            Set RS = Nothing
	            set cmd = Nothing
            else
                strVar = ""
            end if
	    end if
	Next
    Conn.Close
End sub ' AltFieldsValue_CompoundGet()

function ShowCustomPicklist(Key)
    ' Prepare
    strLabel = alt_ids_dict.Item(Key)
    strLabel = mid(strLabel, 2)
    strVar = replace(Key, "inv_compounds.","dB")
    strVal = Eval(strVar)
    ' Open
    strHTML = ""
    strHTML = strHTML & strLabel & ":" & "<br>"
    strHTML = strHTML & "<SELECT name='" & Key & "'" & " onchange=''" & ">"
    ' Populate OPTIONs
	Dim RS
    GetInvConnection()
    Set cmd = Server.CreateObject("ADODB.Command")
    cmd.ActiveConnection = Conn
    cmd.CommandType = adCmdText
    Sql = "SELECT Picklist_ID AS Value, Decode(Picklist_Code,'--','',Picklist_Code||' - ')||picklist_display AS DisplayText FROM ChemInvDb2.Inv_Picklists JOIN ChemInvDb2.Inv_Picklist_Types ON Picklist_Domain = Picklist_Type_ID JOIN ChemInvDb2.Inv_Custom_Fields ON Picklist_Type_ID_FK = Picklist_Type_ID WHERE Custom_Field_Name = ? ORDER BY DisplayText"
    cmd.CommandText = sql
    cmd.Parameters.Append cmd.CreateParameter("Custom_Field_Name", adLongVarChar, adParamInput, len(strLabel) + 1, strLabel)
    Set RS = cmd.Execute
	While NOT RS.EOF
	    lngValue = Clng(RS("Value").value)
	    if lngValue = AltField_GetDefault(Key) then lngValue = ""
	    strDisplayText = RS("DisplayText").value
        strHTML = strHTML & "<OPTION"
        strHTML = strHTML & " value='" & lngValue & "'"
        if strDisplayText = strVal then
            strHTML = strHTML & " selected='true'"
        end if
        strHTML = strHTML & " id='" & strDisplayText & "'"
        strHTML = strHTML & ">"
        strHTML = strHTML & strDisplayText
		RS.MoveNext
	Wend
    Set RS = Nothing
    set cmd = Nothing
    Conn.Close
    ' Close
    strHTML = strHTML & "</SELECT>"
    ShowCustomPicklist = strHTML
end function    ' ShowCustomPicklist()

function ShowCustomSymbol(strCustomField)
    ' Prepare
    ' Populate OPTIONs
	Dim RS
    GetInvConnection()
    Set cmd = Server.CreateObject("ADODB.Command")
    cmd.ActiveConnection = Conn
    cmd.CommandType = adCmdText
    Sql = "SELECT symbol_link FROM cheminvdb2.INV_CUSTOM_FIELDS where symbol_link is not null and Custom_Field_Name = ?"

    cmd.CommandText = sql

    ShowCustomSymbol=""
    cmd.Parameters.Append cmd.CreateParameter("Custom_Field_Name", adLongVarChar, adParamInput, len(strCustomField) + 1, strCustomField)

    Set RS = cmd.Execute
	if not RS.EOF then	    
	    ShowCustomSymbol = "<img src=""" & RS("symbol_link") & """>&nbsp;"	
	end if
    Set RS = Nothing
    set cmd = Nothing
    Conn.Close    
    
end function    ' ShowCustomSymbol()

sub ManageTable_Event(strTablename, strAction)
    ' WJC get real table name even if it is actually a VIEW
    strRealTablename = UCase(strTableName)
    if InStr(strRealTablename, "VIEW") = 1 then
        if InStr(strRealTablename, "_") > 0 then
           strRealTablename = Mid(strRealTablename, InStr(strRealTablename, "_") + 1)
        end if
    end if
    ' WJC these are the three tables used by CustomFieldsInfo_Construct()
    if InStr("[INV_PICKLISTS][INV_CUSTOM_FIELD_GROUPS][INV_CUSTOM_FIELDS]", "[" & strRealTablename & "]") > 0 then
        ' WJC deleting the dictionary is sufficent as it is rebuilt as needed
        custom_fields_info_dict.RemoveAll
    end if
end sub ' ManageTable_Event()

Function detectModernBrowser()
    ModernBrowser = false
	UserAgent = Request.ServerVariables("HTTP_USER_AGENT")
		If InStr(UCASE(UserAgent), "CHROME")>0 OR InStr(UCASE(UserAgent), "FIREFOX")>0 then
			ModernBrowser = true
		end if
	detectModernBrowser = ModernBrowser
End Function
%>
