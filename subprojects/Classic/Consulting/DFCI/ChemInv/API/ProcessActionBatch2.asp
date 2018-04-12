<%@ EnableSessionState="False" Language=VBScript %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/xml_utils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetOrderContainer_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetCreateContainer_cmd.asp"-->

<Script RUNAT="Server" Language="VbScript">

Server.ScriptTimeout = 3600 '1 hour timeout
Dim Conn
Dim Cmd
Dim isCommit
Dim aValuePairs
Dim strReturn

bDebugPrint = False
bWriteError = False
bFailure = false
strError = "Error:<BR>"

LocationID = NULL
UOMID = NULL
QtyMax = NULL
QtyInitial = NULL
ContainerTypeID = NULL
ContainerStatusID = NULL
actionType = NULL
Response.Expires = -1

RegServerName = Application("RegServerName")
InvServerName = Application("InvServerName")

isCommit = true

'-- base64_cdx for empty structure
INVMTCDX = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA"
REGMTCDX = "VmpDRDAxMD"

'Create the parser object
dim xmlDoc
set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
xmlDoc.async = false

'Load ActionBatchFile if exists or request object
xmlDoc.load(request)

Set root = xmlDoc.documentElement
AddAttributeToElement xmlDoc, root, "timeStamp", CStr(Now())
AddAttributeToElement xmlDoc, root, "actionType", action

' Set Cred. for Create Substance
CSUserName = root.getAttribute("CSUSERNAME")
CSUserID = root.getAttribute("CSUSERID")
' Set ID expected by GetCreateContainer_cmd
CurrentUserID = root.getAttribute("CSUSERID")
'Process CREATESUBSTANCE Tags
For Each actionNode In root.SelectNodes("CREATESUBSTANCE")
	'Response.Write("In Create Substance")
	FormData = ""
	For Each reqAttrib In actionNode.Attributes
    	'-- InvLoader 9.1 is adding substance name as an attribute and as a sub-element of CREATESUBSTANCE so for now i'll check for it here and not add it twice
        if reqAttrib.NodeName <> "SUBSTANCE_NAME" then
		    FormData = FormData & "&inv_compounds." & reqAttrib.NodeName & "=" & CnvString(reqAttrib.text)
		end if
	Next
	FormData = FormData & "&inv_compounds.substance_name=" & actionNode.SelectSingleNode("substanceName").text 
	strucData = actionNode.SelectSingleNode("structure").text 
	if Len(strucData) > 0 then
		FormData = FormData & "&inv_compounds.structure=" & Server.URLEncode(strucData)
	else
		FormData = FormData & "&inv_compounds.structure=" & Server.URLEncode(INVMTCDX)
		'FormData = FormData & "&inv_compounds.structure="
	end if
	FormData = Right(FormData, Len(FormData)-1)
	ServerName = Application("InvServerName")
	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUserID
	'Target = "/cheminv/api/CreateSubstance.asp?ResolveConflictsLater=1&RegisterIfConflicts=true&action=" & action
	Target = "/cheminv/api/CreateSubstance.asp?action=" & action

	FormData = FormData & Credentials & "&ResolveConflictsLater=1&RegisterIfConflicts=true"
	'FormData = FormData & Credentials
	'logaction(formData)
	sendTO = 5*60*1000
	receiveTO = 5*60*1000
	httpResponse = CShttpRequest3("POST", ServerName, Target, "ChemInv", FormData, sendTO, receiveTO)
	
	firstQuote = InStr(1,httpResponse,"""")
	secondQuote = InStr(firstQuote +1,httpResponse,"""")
	cID = Mid(httpResponse, firstQuote + 1 , SecondQuote - firstQuote - 1)
	
	Select Case true
		Case inStr(1,httpResponse,"<DUPLICATESUBSTANCE") > 0
			result = "WARNING"
			message = "Duplicate substance created CompoundID= " & cID
		Case inStr(1,httpResponse,"<NEWSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "New substance created " & cID
		Case inStr(1,httpResponse,"<EXISTINGSUBSTANCE") > 0
			result = "SUCCEEDED"
			message = "Existing substance used CompoundID= " & cID
		Case inStr(1,httpResponse,"<CONFLICTINGSUBSTANCES") > 0
			result = "SUCCEEDED"
			message = "Conflicting substance created, CompoundID= " & cID
		Case else
			Response.Write "Error while creating inventory substance.<BR>"
			Response.Write httpResponse & "<BR><BR>"
	End Select
	CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
	AddAttributeToElement xmlDoc, returnValueNode, "COMPOUNDID", cID
	packageID = ActionNode.SelectSingleNode("./@PACKAGEID").text
	Set MatchingNode = root.SelectSingleNode("CREATECONTAINER[@ID='" & packageID & "']")
	AddAttributeToElement xmlDoc, MatchingNode, "CompoundID", cID
	actionNode.removeChild(actionNode.SelectSingleNode("structure"))
	'Response.Write(packageID)
	Response.Write ""
	Response.Flush
		
Next 

' Process REGISTERSUBSTANCE Tags
' ------------------------------
Dim oXMLHTTP
Set oXMLHTTP = Server.CreateObject("Msxml2.ServerXMLHTTP")
xmlTimeOut = 60*60*1000 '1 hr timeout
oXMLHTTP.setTimeouts 1000000,1000000,xmlTimeOut,xmlTimeOut

For Each actionNode In root.SelectNodes("REGISTERSUBSTANCE")

	'RegisterCompounds = lcase(actionNode.getAttribute("REGISTERCOMPOUNDS"))
	RegParameter = actionNode.getAttribute("REGPARAMETER")
	Sequence = actionNode.getAttribute("SEQUENCE")
	Project = actionNode.getAttribute("PROJECT")
	Compound = actionNode.getAttribute("COMPOUND")
	Notebook = actionNode.getAttribute("NOTEBOOK")
	Salt = actionNode.getAttribute("SALT")
	BatchProject = actionNode.getAttribute("BatchProject")
	Scientist = actionNode.getAttribute("Scientist")

	'timeoutOld = server.ScriptTimeout
	'server.ScriptTimeout = 600

	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUSerID

	int i = 0
	Dim httpResponse
	Dim URL
	Dim StatusCode
	
	URL = "http://" & RegServerName & "/chem_reg/reg/reg_post_action.asp"
	' This is the server safe version from MSXML3.
	'if IsObject(Session("SessionXMLHTTP")) then
	'	Set oXMLHTTP = Session("SessionXMLHTTP")
	'else
	'	Set oXMLHTTP = Server.CreateObject("Msxml2.ServerXMLHTTP")
	'	Set Session("SessionXMLHTTP") = oXMLHTTP
	'	trace "Creating the ServerXMLHTTP.", 15
	'end if
	
	i= i +1
	Set oParentNode = actionNode.parentNode

	SubstanceName = actionNode.GetAttribute("CHEMICAL_NAME")
	Structure = actionNode.SelectSingleNode("structure").text 
	if Len(Structure) > 0 then
		Structure = Server.URLEncode(Structure)
	else
		Structure = Server.URLEncode(REGMTCDX)
	end if
	CAS = actionNode.GetAttribute("CAS")
	ACX_ID = actionNode.GetAttribute("ACX_ID")

	reg_method = "REG_PERM"
	user_ID = CSUserName
	user_pwd = CSUserID		
	'supports any reg_parameters coming in, NEW_BATCH is the default
	reg_parameter = RegParameter
	if isEmpty(reg_parameter) or isNull(reg_parameter)  or reg_parameter = "" then reg_parameter = "NEW_BATCH"
	Temporary_Structures_temp_compound_id = ""
	Temporary_Structures_cpd_internal_id = ""
	Temporary_Structures_reg_ID = ""
	Temporary_Structures_Scientist_ID = Scientist
	Temporary_Structures_Sequence_ID = Sequence
	Temporary_Structures_Project_ID = Project
	Temporary_Structures_Compound_Type = Compound	
	Temporary_Structures_Notebook_Number = Notebook
	Temporary_Structures_Salt_Code = Salt
	TEMPORARY_STRUCTURES_batch_project_id= BatchProject
	Temporary_Structures_Structure = Structure
	Return_All_Reg_Data = "true"
		
	QueryString = "reg_method=" & reg_method
	QueryString = QueryString & "&user_ID=" & user_ID
	QueryString = QueryString & "&user_pwd=" & user_pwd
	QueryString = QueryString & "&reg_parameter=" & reg_parameter
	QueryString = QueryString & "&Temporary_Structures.temp_compound_id=" & Temporary_Structures_temp_compound_id
	QueryString = QueryString & "&Temporary_Structures.cpd_internal_id=" & Temporary_Structures_cpd_internal_id
	QueryString = QueryString & "&Temporary_Structures.reg_ID=" & Temporary_Structures_reg_ID
	QueryString = QueryString & "&Temporary_Structures.Sequence_ID=" & Temporary_Structures_Sequence_ID
	QueryString = QueryString & "&Temporary_Structures.Structure=" & Temporary_Structures_Structure
	QueryString = QueryString & "&Return_All_Reg_Data=" & Return_All_Reg_Data

	' add other registration information found in file
	' this list autogenerated from an excel file
	' DJP: Updated this list to look for selected defaults on some fields
	if actionNode.GetAttribute("PRODUCER") <> "" Then QueryString = QueryString & "&Temporary_Structures.PRODUCER=" & actionNode.GetAttribute("PRODUCER")
	if actionNode.GetAttribute("NOTEBOOK_PAGE") <> "" Then QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_PAGE=" & actionNode.GetAttribute("NOTEBOOK_PAGE")
	if actionNode.GetAttribute("NOTEBOOK_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_TEXT=" & actionNode.GetAttribute("NOTEBOOK_TEXT")
	if actionNode.GetAttribute("NOTEBOOK_NUMBER") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_NUMBER=" & actionNode.GetAttribute("NOTEBOOK_NUMBER")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_NUMBER=" & Temporary_Structures_Notebook_Number
	end if
	if actionNode.GetAttribute("PROJECT_ID") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.PROJECT_ID=" & actionNode.GetAttribute("PROJECT_ID")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.PROJECT_ID=" & Temporary_Structures_Project_ID
	end if
	if actionNode.GetAttribute("BATCH_COMMENT") <> "" Then QueryString = QueryString & "&Temporary_Structures.BATCH_COMMENT=" & actionNode.GetAttribute("BATCH_COMMENT")
	if actionNode.GetAttribute("CHEMICAL_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHEMICAL_NAME=" & actionNode.GetAttribute("CHEMICAL_NAME")
	if actionNode.GetAttribute("SYNONYM_R") <> "" Then QueryString = QueryString & "&Temporary_Structures.SYNONYM_R=" & actionNode.GetAttribute("SYNONYM_R")
	if actionNode.GetAttribute("LIT_REF") <> "" Then QueryString = QueryString & "&Temporary_Structures.LIT_REF=" & actionNode.GetAttribute("LIT_REF")
	if actionNode.GetAttribute("PREPARATION") <> "" Then QueryString = QueryString & "&Temporary_Structures.PREPARATION=" & actionNode.GetAttribute("PREPARATION")
	if actionNode.GetAttribute("STORAGE_REQ_AND_WARNINGS") <> "" Then QueryString = QueryString & "&Temporary_Structures.STORAGE_REQ_AND_WARNINGS=" & actionNode.GetAttribute("STORAGE_REQ_AND_WARNINGS")
	if actionNode.GetAttribute("SEQUENCE_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SEQUENCE_ID=" & actionNode.GetAttribute("SEQUENCE_ID")
	if actionNode.GetAttribute("SPECTRUM_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SPECTRUM_ID=" & actionNode.GetAttribute("SPECTRUM_ID")
	if actionNode.GetAttribute("CAS_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.CAS_NUMBER=" & actionNode.GetAttribute("CAS_NUMBER")
	if actionNode.GetAttribute("RNO_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.RNO_NUMBER=" & actionNode.GetAttribute("RNO_NUMBER")
	if actionNode.GetAttribute("FEMA_GRAS_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.FEMA_GRAS_NUMBER=" & actionNode.GetAttribute("FEMA_GRAS_NUMBER")
	if actionNode.GetAttribute("GROUP_CODE") <> "" Then QueryString = QueryString & "&Temporary_Structures.GROUP_CODE=" & actionNode.GetAttribute("GROUP_CODE")
	if actionNode.GetAttribute("SCIENTIST_ID") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.SCIENTIST_ID=" & actionNode.GetAttribute("SCIENTIST_ID")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.SCIENTIST_ID=" & Temporary_Structures_Scientist_ID
	end if
	if actionNode.GetAttribute("BP") <> "" Then QueryString = QueryString & "&Temporary_Structures.BP=" & actionNode.GetAttribute("BP")
	if actionNode.GetAttribute("MP") <> "" Then QueryString = QueryString & "&Temporary_Structures.MP=" & actionNode.GetAttribute("MP")
	if actionNode.GetAttribute("H1NMR") <> "" Then QueryString = QueryString & "&Temporary_Structures.H1NMR=" & actionNode.GetAttribute("H1NMR")
	if actionNode.GetAttribute("C13NMR") <> "" Then QueryString = QueryString & "&Temporary_Structures.C13NMR=" & actionNode.GetAttribute("C13NMR")
	if actionNode.GetAttribute("MS") <> "" Then QueryString = QueryString & "&Temporary_Structures.MS=" & actionNode.GetAttribute("MS")
	if actionNode.GetAttribute("IR") <> "" Then QueryString = QueryString & "&Temporary_Structures.IR=" & actionNode.GetAttribute("IR")
	if actionNode.GetAttribute("GC") <> "" Then QueryString = QueryString & "&Temporary_Structures.GC=" & actionNode.GetAttribute("GC")
	if actionNode.GetAttribute("PHYSICAL_FORM") <> "" Then QueryString = QueryString & "&Temporary_Structures.PHYSICAL_FORM=" & actionNode.GetAttribute("PHYSICAL_FORM")
	if actionNode.GetAttribute("COLOR") <> "" Then QueryString = QueryString & "&Temporary_Structures.COLOR=" & actionNode.GetAttribute("COLOR")
	if actionNode.GetAttribute("FLASHPOINT") <> "" Then QueryString = QueryString & "&Temporary_Structures.FLASHPOINT=" & actionNode.GetAttribute("FLASHPOINT")
	if actionNode.GetAttribute("HPLC") <> "" Then QueryString = QueryString & "&Temporary_Structures.HPLC=" & actionNode.GetAttribute("HPLC")
	if actionNode.GetAttribute("OPTICAL_ROTATION") <> "" Then QueryString = QueryString & "&Temporary_Structures.OPTICAL_ROTATION=" & actionNode.GetAttribute("OPTICAL_ROTATION")
	if actionNode.GetAttribute("REFRACTIVE_INDEX") <> "" Then QueryString = QueryString & "&Temporary_Structures.REFRACTIVE_INDEX=" & actionNode.GetAttribute("REFRACTIVE_INDEX")
	if actionNode.GetAttribute("CREATION_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.CREATION_DATE=" & actionNode.GetAttribute("CREATION_DATE")
	if actionNode.GetAttribute("COMPOUND_TYPE") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.COMPOUND_TYPE=" & actionNode.GetAttribute("COMPOUND_TYPE")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.COMPOUND_TYPE=" & Temporary_Structures_Compound_Type
	end if
	if actionNode.GetAttribute("STRUCTURE_COMMENTS_TXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.STRUCTURE_COMMENTS_TXT=" & actionNode.GetAttribute("STRUCTURE_COMMENTS_TXT")
	if actionNode.GetAttribute("ENTRY_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.ENTRY_DATE=" & actionNode.GetAttribute("ENTRY_DATE")
	if actionNode.GetAttribute("LAST_MOD_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.LAST_MOD_DATE=" & actionNode.GetAttribute("LAST_MOD_DATE")
	if actionNode.GetAttribute("SALT_CODE") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.SALT_CODE=" & actionNode.GetAttribute("SALT_CODE")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.SALT_CODE=" & Temporary_Structures_Salt_Code			
	end if
	if actionNode.GetAttribute("SALT_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_NAME=" & actionNode.GetAttribute("SALT_NAME")
	if actionNode.GetAttribute("SALT_MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_MW=" & actionNode.GetAttribute("SALT_MW")
	if actionNode.GetAttribute("SALT_EQUIVALENTS") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_EQUIVALENTS=" & actionNode.GetAttribute("SALT_EQUIVALENTS")
	if actionNode.GetAttribute("SOLVATE_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_ID=" & actionNode.GetAttribute("SOLVATE_ID")
	if actionNode.GetAttribute("SOLVATE_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_NAME=" & actionNode.GetAttribute("SOLVATE_NAME")
	if actionNode.GetAttribute("SOLVATE_MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_MW=" & actionNode.GetAttribute("SOLVATE_MW")
	if actionNode.GetAttribute("SOLVATE_EQUIVALENTS") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_EQUIVALENTS=" & actionNode.GetAttribute("SOLVATE_EQUIVALENTS")
	if actionNode.GetAttribute("FORMULA_WEIGHT") <> "" Then QueryString = QueryString & "&Temporary_Structures.FORMULA_WEIGHT=" & actionNode.GetAttribute("FORMULA_WEIGHT")
	if actionNode.GetAttribute("BATCH_FORMULA") <> "" Then QueryString = QueryString & "&Temporary_Structures.BATCH_FORMULA=" & actionNode.GetAttribute("BATCH_FORMULA")
	if actionNode.GetAttribute("BATCH_PROJECT_ID") <> "" Then 
		QueryString = QueryString & "&Temporary_Structures.BATCH_PROJECT_ID=" & actionNode.GetAttribute("BATCH_PROJECT_ID")
	else
		'use the selected default value
		QueryString = QueryString & "&Temporary_Structures.BATCH_PROJECT_ID=" & TEMPORARY_STRUCTURES_batch_project_id
	end if
	
	' Structure data is stored outside of the ACTIONBATCH attributes
	if actionNode.SelectSingleNode("structure").text <> "" Then QueryString = QueryString & "&Temporary_Structures.BASE64_CDX=" & Server.URLEncode(actionNode.SelectSingleNode("structure").text)

	if actionNode.GetAttribute("SOURCE") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOURCE=" & actionNode.GetAttribute("SOURCE")
	if actionNode.GetAttribute("VENDOR_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.VENDOR_NAME=" & actionNode.GetAttribute("VENDOR_NAME")
	if actionNode.GetAttribute("VENDOR_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.VENDOR_ID=" & actionNode.GetAttribute("VENDOR_ID")
	if actionNode.GetAttribute("PERCENT_ACTIVE") <> "" Then QueryString = QueryString & "&Temporary_Structures.PERCENT_ACTIVE=" & actionNode.GetAttribute("PERCENT_ACTIVE")
	if actionNode.GetAttribute("AMOUNT_UNITS") <> "" Then QueryString = QueryString & "&Temporary_Structures.AMOUNT_UNITS=" & actionNode.GetAttribute("AMOUNT_UNITS")
	if actionNode.GetAttribute("PURITY") <> "" Then QueryString = QueryString & "&Temporary_Structures.PURITY=" & actionNode.GetAttribute("PURITY")
	if actionNode.GetAttribute("LC_UV_MS") <> "" Then QueryString = QueryString & "&Temporary_Structures.LC_UV_MS=" & actionNode.GetAttribute("LC_UV_MS")
	if actionNode.GetAttribute("CHN_COMBUSTION") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHN_COMBUSTION=" & actionNode.GetAttribute("CHN_COMBUSTION")
	if actionNode.GetAttribute("UV_SPECTRUM") <> "" Then QueryString = QueryString & "&Temporary_Structures.UV_SPECTRUM=" & actionNode.GetAttribute("UV_SPECTRUM")
	if actionNode.GetAttribute("APPEARANCE") <> "" Then QueryString = QueryString & "&Temporary_Structures.APPEARANCE=" & actionNode.GetAttribute("APPEARANCE")
	if actionNode.GetAttribute("LOGD") <> "" Then QueryString = QueryString & "&Temporary_Structures.LOGD=" & actionNode.GetAttribute("LOGD")
	if actionNode.GetAttribute("SOLUBILITY") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLUBILITY=" & actionNode.GetAttribute("SOLUBILITY")
	if actionNode.GetAttribute("COLLABORATOR_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.COLLABORATOR_ID=" & actionNode.GetAttribute("COLLABORATOR_ID")
	if actionNode.GetAttribute("PRODUCT_TYPE") <> "" Then QueryString = QueryString & "&Temporary_Structures.PRODUCT_TYPE=" & actionNode.GetAttribute("PRODUCT_TYPE")
	if actionNode.GetAttribute("CHIRAL") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHIRAL=" & actionNode.GetAttribute("CHIRAL")
	if actionNode.GetAttribute("CLOGP") <> "" Then QueryString = QueryString & "&Temporary_Structures.CLOGP=" & actionNode.GetAttribute("CLOGP")
	if actionNode.GetAttribute("H_BOND_DONORS") <> "" Then QueryString = QueryString & "&Temporary_Structures.H_BOND_DONORS=" & actionNode.GetAttribute("H_BOND_DONORS")
	if actionNode.GetAttribute("H_BOND_ACCEPTORS") <> "" Then QueryString = QueryString & "&Temporary_Structures.H_BOND_ACCEPTORS=" & actionNode.GetAttribute("H_BOND_ACCEPTORS")
	if actionNode.GetAttribute("LAST_MOD_PERSON_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.LAST_MOD_PERSON_ID=" & actionNode.GetAttribute("LAST_MOD_PERSON_ID")
	if actionNode.GetAttribute("ENTRY_PERSON_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.ENTRY_PERSON_ID=" & actionNode.GetAttribute("ENTRY_PERSON_ID")
	if actionNode.GetAttribute("MW_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW_TEXT=" & actionNode.GetAttribute("MW_TEXT")
	if actionNode.GetAttribute("MF_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.MF_TEXT=" & actionNode.GetAttribute("MF_TEXT")
	if actionNode.GetAttribute("MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW=" & actionNode.GetAttribute("MW")
	if actionNode.GetAttribute("MW2") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW2=" & actionNode.GetAttribute("MW2")
	if actionNode.GetAttribute("FORMULA2") <> "" Then QueryString = QueryString & "&Temporary_Structures.FORMULA2=" & actionNode.GetAttribute("FORMULA2")
	if actionNode.GetAttribute("AMOUNT") <> "" Then QueryString = QueryString & "&Temporary_Structures.AMOUNT=" & actionNode.GetAttribute("AMOUNT")
	if actionNode.GetAttribute("DUPLICATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.DUPLICATE=" & actionNode.GetAttribute("DUPLICATE")
	if actionNode.GetAttribute("FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_1=" & actionNode.GetAttribute("FIELD_1")
	if actionNode.GetAttribute("FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_2=" & actionNode.GetAttribute("FIELD_2")
	if actionNode.GetAttribute("FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_3=" & actionNode.GetAttribute("FIELD_3")
	if actionNode.GetAttribute("FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_4=" & actionNode.GetAttribute("FIELD_4")
	if actionNode.GetAttribute("FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_5=" & actionNode.GetAttribute("FIELD_5")
	if actionNode.GetAttribute("FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_6=" & actionNode.GetAttribute("FIELD_6")
	if actionNode.GetAttribute("FIELD_7") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_7=" & actionNode.GetAttribute("FIELD_7")
	if actionNode.GetAttribute("FIELD_8") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_8=" & actionNode.GetAttribute("FIELD_8")
	if actionNode.GetAttribute("FIELD_9") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_9=" & actionNode.GetAttribute("FIELD_9")
	if actionNode.GetAttribute("FIELD_10") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_10=" & actionNode.GetAttribute("FIELD_10")
	if actionNode.GetAttribute("LOAD_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.LOAD_ID=" & actionNode.GetAttribute("LOAD_ID")
	if actionNode.GetAttribute("DATETIME_STAMP") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATETIME_STAMP=" & actionNode.GetAttribute("DATETIME_STAMP")
	if actionNode.GetAttribute("TXT_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_1=" & actionNode.GetAttribute("TXT_CMPD_FIELD_1")
	if actionNode.GetAttribute("TXT_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_2=" & actionNode.GetAttribute("TXT_CMPD_FIELD_2")
	if actionNode.GetAttribute("TXT_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_3=" & actionNode.GetAttribute("TXT_CMPD_FIELD_3")
	if actionNode.GetAttribute("TXT_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_4=" & actionNode.GetAttribute("TXT_CMPD_FIELD_4")
	if actionNode.GetAttribute("INT_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_1=" & actionNode.GetAttribute("INT_BATCH_FIELD_1")
	if actionNode.GetAttribute("INT_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_2=" & actionNode.GetAttribute("INT_BATCH_FIELD_2")
	if actionNode.GetAttribute("INT_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_3=" & actionNode.GetAttribute("INT_BATCH_FIELD_3")
	if actionNode.GetAttribute("INT_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_4=" & actionNode.GetAttribute("INT_BATCH_FIELD_4")
	if actionNode.GetAttribute("INT_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_5=" & actionNode.GetAttribute("INT_BATCH_FIELD_5")
	if actionNode.GetAttribute("INT_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_6=" & actionNode.GetAttribute("INT_BATCH_FIELD_6")
	if actionNode.GetAttribute("INT_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_1=" & actionNode.GetAttribute("INT_CMPD_FIELD_1")
	if actionNode.GetAttribute("INT_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_2=" & actionNode.GetAttribute("INT_CMPD_FIELD_2")
	if actionNode.GetAttribute("INT_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_3=" & actionNode.GetAttribute("INT_CMPD_FIELD_3")
	if actionNode.GetAttribute("INT_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_4=" & actionNode.GetAttribute("INT_CMPD_FIELD_4")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_1=" & actionNode.GetAttribute("REAL_BATCH_FIELD_1")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_2=" & actionNode.GetAttribute("REAL_BATCH_FIELD_2")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_3=" & actionNode.GetAttribute("REAL_BATCH_FIELD_3")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_4=" & actionNode.GetAttribute("REAL_BATCH_FIELD_4")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_5=" & actionNode.GetAttribute("REAL_BATCH_FIELD_5")
	if actionNode.GetAttribute("REAL_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_6=" & actionNode.GetAttribute("REAL_BATCH_FIELD_6")
	if actionNode.GetAttribute("REAL_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_1=" & actionNode.GetAttribute("REAL_CMPD_FIELD_1")
	if actionNode.GetAttribute("REAL_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_2=" & actionNode.GetAttribute("REAL_CMPD_FIELD_2")
	if actionNode.GetAttribute("REAL_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_3=" & actionNode.GetAttribute("REAL_CMPD_FIELD_3")
	if actionNode.GetAttribute("REAL_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_4=" & actionNode.GetAttribute("REAL_CMPD_FIELD_4")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_1=" & actionNode.GetAttribute("DATE_BATCH_FIELD_1")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_2=" & actionNode.GetAttribute("DATE_BATCH_FIELD_2")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_3=" & actionNode.GetAttribute("DATE_BATCH_FIELD_3")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_4=" & actionNode.GetAttribute("DATE_BATCH_FIELD_4")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_5=" & actionNode.GetAttribute("DATE_BATCH_FIELD_5")
	if actionNode.GetAttribute("DATE_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_6=" & actionNode.GetAttribute("DATE_BATCH_FIELD_6")
	if actionNode.GetAttribute("DATE_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_1=" & actionNode.GetAttribute("DATE_CMPD_FIELD_1")
	if actionNode.GetAttribute("DATE_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_2=" & actionNode.GetAttribute("DATE_CMPD_FIELD_2")
	if actionNode.GetAttribute("DATE_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_3=" & actionNode.GetAttribute("DATE_CMPD_FIELD_3")
	if actionNode.GetAttribute("DATE_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_4=" & actionNode.GetAttribute("DATE_CMPD_FIELD_4")
	if actionNode.GetAttribute("LEGACY_REG_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.LEGACY_REG_NUMBER=" & actionNode.GetAttribute("LEGACY_REG_NUMBER")

	if bDebugPrint then 
		Response.Write QueryString & "<BR>"
		'Response.End
	end if			
	'if  Structure = "" then logaction(QueryString)
	
	'trace QueryString, 20
	'Response.Write QueryString
	'Response.End
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	oXMLHTTP.open "POST", URL, False
	oXMLHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	oXMLHTTP.setRequestHeader "User-Agent", "ChemInv"

	oXMLHTTP.send QueryString
	'logAction(QueryString)
	'logAction (err.Description)

	StatusCode = oXMLHTTP.status
	If StatusCode <> "200" then
		httpResponse = oXMLHTTP.responseText
		'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
	Else
		httpResponse = oXMLHTTP.responseText
	End If
	'httpResponse = CShttpRequest2("POST", RegServerName , "/chem_reg/reg/reg_post_action.asp", "ChemInv", QueryString)
	out2 = httpResponse
	'Response.Write out2 & "=RegSubstance Return Value<BR>" 
	'trace out2 & "=RegSubstance Return Value<BR>", 15

	
	'Response.Write RegServerName 
	'Response.End
	'Response.Write out2 & "=CreateSubstance Return Value<BR>" 
	'Response.End
	'reg response data depends on the reg_parameter
	'USER_INPUT;NEW_BATCH;OVERRIDE;UNIQUE_DEL_TEMP
		
	arrRegData = split(out2,":")
	action = arrRegData(0)

	Select Case lcase(action)
		Case "multiple duplicates"
			regID = ""
			batchNumber = ""			
			result = "WARNING"
			message = "Multiple Duplicates, RegID=" & regID & ", BatchNumber=" & batchNumber 
		Case "new_compound"
			if ubound(arrRegData) = 1 then
				regNumber = trim(arrRegData(1))
				regID = GetRegID(regNumber,user_id,user_pwd)
				if isNull(regID) then 
					regID = ""
					batchNumber = ""
				else
					batchNumber = "1"
				end if					
			else
				batchNumber = arrRegData(2)
				regID = arrRegData(3)
				cpdInternalID = arrRegData(4)
			end if
			result = "SUCCEEDED"
			message = "Compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
		Case "new compound"
			if ubound(arrRegData) = 1 then
				regNumber = trim(arrRegData(1))
				regID = GetRegID(regNumber,user_id,user_pwd)
				if isNull(regID) then 
					regID = ""
					batchNumber = ""
				else
					batchNumber = "1"
				end if					
			else
				batchNumber = arrRegData(2)
				regID = arrRegData(3)
				cpdInternalID = arrRegData(4)
			end if
			result = "SUCCEEDED"
			message = "Compound registered, RegID=" & regID & ", BatchNumber=" & batchNumber 
		Case "new_batch"
			regNumber = arrRegData(1)
			batchNumber = arrRegData(2)
			regID = arrRegData(3)
			cpdInternalID = arrRegData(4)
		Case else
			'single duplicate case
			regID = ""
			batchNumber = ""
			result = "WARNING"
			message = "Single duplicate, RegID=" & regID & ", BatchNumber=" & batchNumber 

	end Select
		
	'Logaction(out2 & ":" & Server.ScriptTimeout)		
	'Response.Write out2 & ":" & Server.ScriptTimeout
	'Response.Flush
	'Response.End
	if bDebugPrint then Response.Write out2 & "=RegSubstance Return Value<BR>" 
	' set oXMLHTTP = nothing

	CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
	AddAttributeToElement xmlDoc, returnValueNode, "REGID", cStr(regID)
	AddAttributeToElement xmlDoc, returnValueNode, "BATCHNUMBER", batchNumber
	packageID = ActionNode.SelectSingleNode("./@PACKAGEID").text
	Set MatchingNode = root.SelectSingleNode("CREATECONTAINER[@ID='" & packageID & "']")
	AddAttributeToElement xmlDoc, MatchingNode, "REGID", cStr(regID)
	AddAttributeToElement xmlDoc, MatchingNode, "BATCHNUMBER", batchNumber
	actionNode.removeChild(actionNode.SelectSingleNode("structure"))
	'Response.Write(packageID)
	Response.Write ""
	Response.Flush

Next 
'Response.End

GetSessionlessInvConnection()

'Process MOVECONTAINER Tags
set actionNodeList = root.SelectNodes("MOVECONTAINER")
if actionNodeList.length > 0 then
    Set GetPrimaryKeyCmd = Server.CreateObject("ADODB.Command")
    Set GetPrimaryKeyCmd.ActiveConnection = Conn
    GetPrimaryKeyCmd.CommandType = adCmdStoredProc
    GetPrimaryKeyCmd.CommandText = Application("CHEMINV_USERNAME") & ".GETPRIMARYKEYIDS"
    Set PKReturnValueParam = GetPrimaryKeyCmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 32000, NULL)
    GetPrimaryKeyCmd.Parameters.Append PKReturnValueParam
    Set TableNameParam = GetPrimaryKeyCmd.CreateParameter("PTABLENAME", adVarChar, adParamInput, 1, "")
    GetPrimaryKeyCmd.Parameters.Append TableNameParam
    Set TableValuesParam = GetPrimaryKeyCmd.CreateParameter("PTABLEVALUES", adVarChar, adParamInput, 1, "")
    GetPrimaryKeyCmd.Parameters.Append TableValuesParam
    GetPrimaryKeyCmd.Prepared = true

    Set MoveContainerCmd = Server.CreateObject("ADODB.Command")
    Set MoveContainerCmd.ActiveConnection = Conn
    MoveContainerCmd.CommandType = adCmdStoredProc
    MoveContainerCmd.CommandText = Application("CHEMINV_USERNAME") & ".MOVECONTAINER"
    Set MCReturnValueParam = MoveContainerCmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 8000, NULL)
    MoveContainerCmd.Parameters.Append MCReturnValueParam
    Set LocationIDParam = MoveContainerCmd.CreateParameter("PLOCATIONID", adVarChar, adParamInput, 1, "")
    MoveContainerCmd.Parameters.Append LocationIDParam
    Set ContainerIDParam = MoveContainerCmd.CreateParameter("PCONTAINERID", adVarChar, adParamInput, 1, "")
    MoveContainerCmd.Parameters.Append ContainerIDParam
    MoveContainerCmd.Prepared = true

    For Each actionNode In actionNodeList
	    ContainerID = null
	    LocationID = null
	    message = ""
	    For Each reqAttrib In actionNode.Attributes
	        if reqAttrib.NodeName = "CONTAINER_BARCODE" then
	            ContainerBarcode = CnvString(reqAttrib.text)
	            TableNameParam.size = Len("inv_containers")
	            TableNameParam.value = "inv_containers"	        
	            TableValuesParam.size = Len(ContainerBarcode)
	            TableValuesParam.value = ContainerBarcode
	            strReturn = ExecuteOracleFunction( GetPrimaryKeyCmd, "GETPRIMARYKEYIDS" )
	            if strReturn <> "" then
	                message = strReturn
	            else
	                if (instr(PKReturnValueParam.value,"Error") > 0) then
	                    message = PKReturnValueParam.value
	                elseif (PKReturnValueParam.value <> "NOT FOUND") then	                    
		                aValuePairs = split(PKReturnValueParam.value, "=")
		                if UBound(aValuePairs) > 0 then
			                ContainerID = aValuePairs(1)
			            end if
		            end if
		        end if
	        elseif reqAttrib.NodeName = "LOCATION_BARCODE" then
	            LocationBarcode = CnvString(reqAttrib.text)
	            TableNameParam.size = Len("inv_locations")
	            TableNameParam.value = "inv_locations"
	            TableValuesParam.size = Len(LocationBarcode)
	            TableValuesParam.value = LocationBarcode
	            strReturn = ExecuteOracleFunction( GetPrimaryKeyCmd, "GETPRIMARYKEYIDS" )
	            if strReturn <> "" then
	                message = strReturn
	            else
	                if (instr(PKReturnValueParam.value,"Error") > 0) then
	                    message = PKReturnValueParam.value
	                elseif (PKReturnValueParam.value <> "NOT FOUND") then
		                aValuePairs = split(PKReturnValueParam.value, "=")
		                if UBound(aValuePairs) > 0 then
			                LocationID = aValuePairs(1)
			            end if
		            end if
		        end if
	        end if
	    Next
    	
	    if not IsNull(ContainerID) and not IsNull(LocationID) then
	        LocationIDParam.size = Len(LocationID)
	        LocationIDParam.value = LocationID
	        ContainerIDParam.size = Len(ContainerID)
            ContainerIDParam.value = ContainerID
            strReturn = ExecuteOracleFunction( MoveContainerCmd, "MOVECONTAINER" )
            if strReturn <> "" then
                result = "FAILED"
                message = strReturn
	        else            
                if (instr(MCReturnValueParam.value,"Error") > 0) then
	                result = "FAILED"
			        message =  MCReturnValueParam.value
	            else
	                result = "SUCCEEDED"
			        message = "Container moved"
	            end if
	        end if
	    else
	        result = "FAILED"		    
	    end if	
    	
	    CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
	    Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")		
    Next 
    
    set GetPrimaryKeyCmd = nothing
    set MoveContainerCmd = nothing

end if      ' if actionNodeList.length > 0 then


' Process CREATECONTAINER Tags
' ----------------------------

'The following vars are expected by the GetOrderContainer_cmd but are reset later as params
DueDate="1/1/2000"
Project="1"
Job="1"
DeliveryLocationID="1"
ActionNodeName = "CREATECONTAINER"
bIsFirst = True

Conn.BeginTrans()

Set Cmd = Server.CreateObject("ADODB.Command")
Set Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc

For Each actionNode In root.SelectNodes(ActionNodeName)

	' TODO generalize to other actions
	actionName = actionNode.NodeName

	lastCommandText = Cmd.CommandText

	CommandText = Application("CHEMINV_USERNAME") & ".CreateContainer"
	Cmd.CommandText = CommandText
	if lastCommandText <> CommandText then
		if not bIsFirst then Call DeleteParameters()
		Call GetCreateContainer_cmd()
	end if

	'Loop the required params
	For Each reqAttrib In actionNode.Attributes
		pname= "p" & reqAttrib.NodeName
		if pname <> "pID" then
			if reqAttrib.text <> ""  then
				if pname = "pQTY_MAX" then
					pname = "pMAXQTY"
				end if
				Cmd.Parameters(pname) = CnvString(reqAttrib.text)
			end if
		end if

	Next ' required Attribute

	Cmd.Parameters("pCurrentUserID") = ucase(CSUserName)
	'Loop optional attributes

	For Each optAttrib In actionNode.selectNodes("./OPTIONALPARAMS/*")
		Cmd.Parameters("p" & optAttrib.NodeName) = CnvString(optAttrib.text)
		'Response.Write  optAttrib.NodeName & " = " & optAttrib.text & "<BR>"
	Next ' optional attribute

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
		Response.write "<HR>"
	Else
	
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & "." & ActionNodeName)
		returnCode =  Cstr(Cmd.Parameters("RETURN_VALUE"))

		if returnCode > 0 then
			result = "SUCCEEDED"
			message = "ContainerID " & returnCode
		Else
			bFailure = true
			result = "FAILED"
			message = Application(returnCode)
		End if
		CreateOrReplaceNode xmlDoc, "RETURNVALUE", actionNode, message, "RESULT", result
		Set returnValueNode = actionNode.SelectSingleNode("RETURNVALUE")
		AddAttributeToElement xmlDoc, returnValueNode, "CONTAINERID", returnCode
		CreateOrReplaceNode xmlDoc, "RETURNCODE", actionNode, returnCode, "", null
	End if	
	bIsFirst = false

	
Next ' actionNode

AddAttributeToElement xmlDoc, root, "ErrorsFound", Cstr(bFailure)

Response.Write(xmlDoc.xml)

if isCommit then
	Conn.CommitTrans()
else
	Conn.RollBackTrans()
end if

Set oXMLHTTP = Nothing

Sub DeleteParameters()
	for i = 0 to (Cmd.Parameters.Count-1)
		Cmd.Parameters.Delete(0)
	next
end sub


'-------------------------------------------------------------------------------
' Name: LogAction(inputstr)
' Type: Sub
' Purpose:  writes imformation to a output file 
' Inputs:   inputstr  as string - variable to output
' Returns:	none
' Comments: writes informtion to /inetput/cfwlog.txt file
'-------------------------------------------------------------------------------
Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub

Function ExecuteOracleFunction(objCommand, strName)
    Dim strReturn
    strReturn = ""
	On Error Resume Next
	objCommand.Execute
	If Err then
		Select Case True
			Case inStr(Err.description,"ORA-20003") > 0
		        strReturn = strName & ": Cannot execute invalid Oracle procedure."
			Case inStr(Err.description,"access violation") > 0
			    strReturn = strName & ": Current user is not allowed to execute Oracle procedure."
			Case Else
			    strReturn = strName & ": " & Err.description
		End Select		
	End if
	on error goto 0
	
	ExecuteOracleFunction = strReturn
End function
	

</SCRIPT>

