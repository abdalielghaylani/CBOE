<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">

Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim bXMLDebug

'set trace level to 15 or 20 to get the traces here
bDebugPrint = False
bXMLDebug = false
bWriteError = False
strError = "Error:CreatePlateXML<BR>"
RegServerName = Application("RegServerName")
InvServerName = Application("InvServerName")


Set oPlateXML = Server.CreateObject("MSXML2.DOMDocument.4.0")
oPlateXML.load(request)
If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug01_CreatePlateXML.xml")) 
'If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug" & replace(replace(time,":","")," ","") & "_CreatePlateXML.xml"))
'Response.End

'always decrement parent quantitites
bDecrementParents = true
' Redirect to help page if no xmldoc is passed
If Len(oPlateXML.xml) = 0 then
	Response.Redirect "/cheminv/help/admin/api/CreatePlateXML.htm"
	Response.end
End if

'get api variables from the xmldoc
Set oPlatesElement = oPlateXML.documentElement
With oPlatesElement
	RegisterCompounds = lcase(.getAttribute("REGISTERCOMPOUNDS"))
	RegUser = .getAttribute("REGUSER")
	RegPwd = .getAttribute("REGPWD")
	CSUserName = .getAttribute("CSUSERNAME")
	CSUserID = .getAttribute("CSUSERID")
	
	'reg parameters
	RegParameter = .getAttribute("REGPARAMETER")
	Sequence = .getAttribute("SEQUENCE")
	Project = .getAttribute("PROJECT")
	Compound = .getAttribute("COMPOUND")
	Notebook = .getAttribute("NOTEBOOK")
	Salt = .getAttribute("SALT")
	BatchProject = .getAttribute("BatchProject")
	Scientist = .getAttribute("Scientist")
End With

' Check for required parameters
If IsEmpty(RegisterCompounds) or IsNull(RegisterCompounds)  then
	strError = strError & "RegisterCompounds is a required parameter<BR>"
	bWriteError = True
else
	'RegUser and RegPwd are only req'd if RegisterCompounds = true
	If RegisterCompounds = "true" then
		'register compound credentials are required if register is true
		If IsEmpty(RegUser) or IsNull(RegUser) then
			strError = strError & "RegUser is a required parameter<BR>"
			bWriteError = True
		End if
		If IsEmpty(RegPwd) or IsNull(RegPwd) then
			strError = strError & "RegPwd is a required parameter<BR>"
			bWriteError = True
		End if
	End if
End if
If IsEmpty(CSUserName) or IsNull(CSUserName) then
	strError = strError & "CSUserName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(CSUserID) or IsNull(CSUserID) then
	strError = strError & "CSUserID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.End
end if

'Register compounds or add to inv_compounds
oPlateXML.setProperty "SelectionLanguage","XPath"
Set nlCompounds = oPlateXML.selectNodes("//COMPOUND")
'if bDebugPrint then Response.Write nlCompounds.length & "=NumCompounds<BR>"

Set oResultXML = server.CreateObject("MSXML2.DOMDocument")
Set oRootNode = oResultXML.createElement("CREATE_PLATE_RESULTS")
Set oSubstanceResultsNode = oResultXML.createElement("SUBSTANCE_RESULTS")

Set oCmpdResultXML = server.CreateObject("MSXML2.DOMDocument")
if RegisterCompounds = "true" then
	timeoutOld = server.ScriptTimeout
	server.ScriptTimeout = 600
	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUSerID
	int i = 0
	Dim oXMLHTTP
	Dim httpResponse
	Dim URL
	Dim StatusCode
	
	URL = "http://" & RegServerName & "/chem_reg/reg/reg_post_action.asp"
	' This is the server safe version from MSXML3.
	if IsObject(Session("SessionXMLHTTP")) then
		Set oXMLHTTP = Session("SessionXMLHTTP")
	else
		Set oXMLHTTP = Server.CreateObject("Msxml2.ServerXMLHTTP")
		Set Session("SessionXMLHTTP") = oXMLHTTP
		trace "Creating the ServerXMLHTTP.", 15
	end if
	oXMLHTTP.setTimeouts 1000000,1000000,1000000,1000000
	
	For Each oNode in nlCompounds
		i= i +1
		Set oParentNode = oNode.parentNode
		SubstanceName = oNode.GetAttribute("SUBSTANCE_NAME")
		Structure = oNode.GetAttribute("BASE64_CDX")
		CAS = oNode.GetAttribute("CAS")
		ACX_ID = oNode.GetAttribute("ACX_ID")
		
		reg_method = "REG_PERM"
		user_ID = RegUser
		user_pwd = RegPwd		
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
		'QueryString = QueryString & "&Temporary_Structures.Scientist_ID=" & Temporary_Structures_Scientist_ID
		'QueryString = QueryString & "&Temporary_Structures.Project_ID=" & Temporary_Structures_Project_ID
		'QueryString = QueryString & "&Temporary_Structures.Compound_Type=" & Temporary_Structures_Compound_Type
		'QueryString = QueryString & "&Temporary_Structures.Notebook_Number=" & Temporary_Structures_Notebook_Number
		'QueryString = QueryString & "&TEMPORARY_STRUCTURES.batch_project_id=" & TEMPORARY_STRUCTURES_batch_project_id
		'QueryString = QueryString & "&Temporary_Structures.Salt_Code=" & Temporary_Structures_Salt_Code
		QueryString = QueryString & "&Temporary_Structures.Structure=" & Temporary_Structures_Structure
		QueryString = QueryString & "&Return_All_Reg_Data=" & Return_All_Reg_Data
		
		' add other registration information found in file
		' this list autogenerated from an excel file
		' DJP: Updated this list to look for selected defaults on some fields
		if oNode.GetAttribute("PRODUCER") <> "" Then QueryString = QueryString & "&Temporary_Structures.PRODUCER=" & AttribEncode(oNode,"PRODUCER")
		if oNode.GetAttribute("NOTEBOOK_PAGE") <> "" Then QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_PAGE=" & oNode.GetAttribute("NOTEBOOK_PAGE")
		if oNode.GetAttribute("NOTEBOOK_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_TEXT=" & AttribEncode(oNode,"NOTEBOOK_TEXT")
		if oNode.GetAttribute("NOTEBOOK_NUMBER") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_NUMBER=" & oNode.GetAttribute("NOTEBOOK_NUMBER")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.NOTEBOOK_NUMBER=" & Temporary_Structures_Notebook_Number
		end if
		if oNode.GetAttribute("PROJECT_ID") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.PROJECT_ID=" & oNode.GetAttribute("PROJECT_ID")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.PROJECT_ID=" & Temporary_Structures_Project_ID
		end if
		if oNode.GetAttribute("BATCH_COMMENT") <> "" Then QueryString = QueryString & "&Temporary_Structures.BATCH_COMMENT=" & AttribEncode(oNode,"BATCH_COMMENT")
		if oNode.GetAttribute("CHEMICAL_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHEMICAL_NAME=" & AttribEncode(oNode,"CHEMICAL_NAME")
		if oNode.GetAttribute("SYNONYM_R") <> "" Then QueryString = QueryString & "&Temporary_Structures.SYNONYM_R=" & AttribEncode(oNode,"SYNONYM_R")
		if oNode.GetAttribute("LIT_REF") <> "" Then QueryString = QueryString & "&Temporary_Structures.LIT_REF=" & AttribEncode(oNode,"LIT_REF")
		if oNode.GetAttribute("PREPARATION") <> "" Then QueryString = QueryString & "&Temporary_Structures.PREPARATION=" & AttribEncode(oNode,"PREPARATION")
		if oNode.GetAttribute("STORAGE_REQ_AND_WARNINGS") <> "" Then QueryString = QueryString & "&Temporary_Structures.STORAGE_REQ_AND_WARNINGS=" & AttribEncode(oNode,"STORAGE_REQ_AND_WARNINGS")
		if oNode.GetAttribute("SEQUENCE_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SEQUENCE_ID=" & oNode.GetAttribute("SEQUENCE_ID")
		if oNode.GetAttribute("SPECTRUM_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SPECTRUM_ID=" & oNode.GetAttribute("SPECTRUM_ID")
		if oNode.GetAttribute("CAS_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.CAS_NUMBER=" & oNode.GetAttribute("CAS_NUMBER")
		if oNode.GetAttribute("RNO_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.RNO_NUMBER=" & oNode.GetAttribute("RNO_NUMBER")
		if oNode.GetAttribute("FEMA_GRAS_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.FEMA_GRAS_NUMBER=" & oNode.GetAttribute("FEMA_GRAS_NUMBER")
		if oNode.GetAttribute("GROUP_CODE") <> "" Then QueryString = QueryString & "&Temporary_Structures.GROUP_CODE=" & oNode.GetAttribute("GROUP_CODE")
		if oNode.GetAttribute("SCIENTIST_ID") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.SCIENTIST_ID=" & oNode.GetAttribute("SCIENTIST_ID")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.SCIENTIST_ID=" & Temporary_Structures_Scientist_ID
		end if
		if oNode.GetAttribute("BP") <> "" Then QueryString = QueryString & "&Temporary_Structures.BP=" & oNode.GetAttribute("BP")
		if oNode.GetAttribute("MP") <> "" Then QueryString = QueryString & "&Temporary_Structures.MP=" & oNode.GetAttribute("MP")
		if oNode.GetAttribute("H1NMR") <> "" Then QueryString = QueryString & "&Temporary_Structures.H1NMR=" & oNode.GetAttribute("H1NMR")
		if oNode.GetAttribute("C13NMR") <> "" Then QueryString = QueryString & "&Temporary_Structures.C13NMR=" & oNode.GetAttribute("C13NMR")
		if oNode.GetAttribute("MS") <> "" Then QueryString = QueryString & "&Temporary_Structures.MS=" & oNode.GetAttribute("MS")
		if oNode.GetAttribute("IR") <> "" Then QueryString = QueryString & "&Temporary_Structures.IR=" & oNode.GetAttribute("IR")
		if oNode.GetAttribute("GC") <> "" Then QueryString = QueryString & "&Temporary_Structures.GC=" & oNode.GetAttribute("GC")
		if oNode.GetAttribute("PHYSICAL_FORM") <> "" Then QueryString = QueryString & "&Temporary_Structures.PHYSICAL_FORM=" & oNode.GetAttribute("PHYSICAL_FORM")
		if oNode.GetAttribute("COLOR") <> "" Then QueryString = QueryString & "&Temporary_Structures.COLOR=" & oNode.GetAttribute("COLOR")
		if oNode.GetAttribute("FLASHPOINT") <> "" Then QueryString = QueryString & "&Temporary_Structures.FLASHPOINT=" & oNode.GetAttribute("FLASHPOINT")
		if oNode.GetAttribute("HPLC") <> "" Then QueryString = QueryString & "&Temporary_Structures.HPLC=" & oNode.GetAttribute("HPLC")
		if oNode.GetAttribute("OPTICAL_ROTATION") <> "" Then QueryString = QueryString & "&Temporary_Structures.OPTICAL_ROTATION=" & oNode.GetAttribute("OPTICAL_ROTATION")
		if oNode.GetAttribute("REFRACTIVE_INDEX") <> "" Then QueryString = QueryString & "&Temporary_Structures.REFRACTIVE_INDEX=" & oNode.GetAttribute("REFRACTIVE_INDEX")
		if oNode.GetAttribute("CREATION_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.CREATION_DATE=" & oNode.GetAttribute("CREATION_DATE")
		if oNode.GetAttribute("COMPOUND_TYPE") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.COMPOUND_TYPE=" & oNode.GetAttribute("COMPOUND_TYPE")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.COMPOUND_TYPE=" & Temporary_Structures_Compound_Type
		end if
		if oNode.GetAttribute("STRUCTURE_COMMENTS_TXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.STRUCTURE_COMMENTS_TXT=" & AttribEncode(oNode,"STRUCTURE_COMMENTS_TXT")
		if oNode.GetAttribute("ENTRY_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.ENTRY_DATE=" & oNode.GetAttribute("ENTRY_DATE")
		if oNode.GetAttribute("LAST_MOD_DATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.LAST_MOD_DATE=" & oNode.GetAttribute("LAST_MOD_DATE")
		if oNode.GetAttribute("SALT_CODE") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.SALT_CODE=" & oNode.GetAttribute("SALT_CODE")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.SALT_CODE=" & Temporary_Structures_Salt_Code			
		end if
		if oNode.GetAttribute("SALT_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_NAME=" & AttribEncode(oNode,"SALT_NAME")
		if oNode.GetAttribute("SALT_MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_MW=" & oNode.GetAttribute("SALT_MW")
		if oNode.GetAttribute("SALT_EQUIVALENTS") <> "" Then QueryString = QueryString & "&Temporary_Structures.SALT_EQUIVALENTS=" & oNode.GetAttribute("SALT_EQUIVALENTS")
		if oNode.GetAttribute("SOLVATE_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_ID=" & oNode.GetAttribute("SOLVATE_ID")
		if oNode.GetAttribute("SOLVATE_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_NAME=" & AttribEncode(oNode,"SOLVATE_NAME")
		if oNode.GetAttribute("SOLVATE_MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_MW=" & oNode.GetAttribute("SOLVATE_MW")
		if oNode.GetAttribute("SOLVATE_EQUIVALENTS") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLVATE_EQUIVALENTS=" & oNode.GetAttribute("SOLVATE_EQUIVALENTS")
		if oNode.GetAttribute("FORMULA_WEIGHT") <> "" Then QueryString = QueryString & "&Temporary_Structures.FORMULA_WEIGHT=" & oNode.GetAttribute("FORMULA_WEIGHT")
		if oNode.GetAttribute("BATCH_FORMULA") <> "" Then QueryString = QueryString & "&Temporary_Structures.BATCH_FORMULA=" & oNode.GetAttribute("BATCH_FORMULA")
		if oNode.GetAttribute("BATCH_PROJECT_ID") <> "" Then 
			QueryString = QueryString & "&Temporary_Structures.BATCH_PROJECT_ID=" & oNode.GetAttribute("BATCH_PROJECT_ID")
		else
			'use the selected default value
			QueryString = QueryString & "&Temporary_Structures.BATCH_PROJECT_ID=" & TEMPORARY_STRUCTURES_batch_project_id
		end if
		if oNode.GetAttribute("SOURCE") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOURCE=" & oNode.GetAttribute("SOURCE")
		if oNode.GetAttribute("VENDOR_NAME") <> "" Then QueryString = QueryString & "&Temporary_Structures.VENDOR_NAME=" & AttribEncode(oNode,"VENDOR_NAME")
		if oNode.GetAttribute("VENDOR_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.VENDOR_ID=" & oNode.GetAttribute("VENDOR_ID")
		if oNode.GetAttribute("PERCENT_ACTIVE") <> "" Then QueryString = QueryString & "&Temporary_Structures.PERCENT_ACTIVE=" & oNode.GetAttribute("PERCENT_ACTIVE")
		if oNode.GetAttribute("AMOUNT_UNITS") <> "" Then QueryString = QueryString & "&Temporary_Structures.AMOUNT_UNITS=" & oNode.GetAttribute("AMOUNT_UNITS")
		if oNode.GetAttribute("PURITY") <> "" Then QueryString = QueryString & "&Temporary_Structures.PURITY=" & oNode.GetAttribute("PURITY")
		if oNode.GetAttribute("LC_UV_MS") <> "" Then QueryString = QueryString & "&Temporary_Structures.LC_UV_MS=" & oNode.GetAttribute("LC_UV_MS")
		if oNode.GetAttribute("CHN_COMBUSTION") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHN_COMBUSTION=" & oNode.GetAttribute("CHN_COMBUSTION")
		if oNode.GetAttribute("UV_SPECTRUM") <> "" Then QueryString = QueryString & "&Temporary_Structures.UV_SPECTRUM=" & oNode.GetAttribute("UV_SPECTRUM")
		if oNode.GetAttribute("APPEARANCE") <> "" Then QueryString = QueryString & "&Temporary_Structures.APPEARANCE=" & AttribEncode(oNode,"APPEARANCE")
		if oNode.GetAttribute("LOGD") <> "" Then QueryString = QueryString & "&Temporary_Structures.LOGD=" & oNode.GetAttribute("LOGD")
		if oNode.GetAttribute("SOLUBILITY") <> "" Then QueryString = QueryString & "&Temporary_Structures.SOLUBILITY=" & oNode.GetAttribute("SOLUBILITY")
		if oNode.GetAttribute("COLLABORATOR_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.COLLABORATOR_ID=" & oNode.GetAttribute("COLLABORATOR_ID")
		if oNode.GetAttribute("PRODUCT_TYPE") <> "" Then QueryString = QueryString & "&Temporary_Structures.PRODUCT_TYPE=" & oNode.GetAttribute("PRODUCT_TYPE")
		if oNode.GetAttribute("CHIRAL") <> "" Then QueryString = QueryString & "&Temporary_Structures.CHIRAL=" & oNode.GetAttribute("CHIRAL")
		if oNode.GetAttribute("CLOGP") <> "" Then QueryString = QueryString & "&Temporary_Structures.CLOGP=" & oNode.GetAttribute("CLOGP")
		if oNode.GetAttribute("H_BOND_DONORS") <> "" Then QueryString = QueryString & "&Temporary_Structures.H_BOND_DONORS=" & oNode.GetAttribute("H_BOND_DONORS")
		if oNode.GetAttribute("H_BOND_ACCEPTORS") <> "" Then QueryString = QueryString & "&Temporary_Structures.H_BOND_ACCEPTORS=" & oNode.GetAttribute("H_BOND_ACCEPTORS")
		if oNode.GetAttribute("BASE64_CDX") <> "" Then QueryString = QueryString & "&Temporary_Structures.BASE64_CDX=" & oNode.GetAttribute("BASE64_CDX")
		if oNode.GetAttribute("LAST_MOD_PERSON_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.LAST_MOD_PERSON_ID=" & oNode.GetAttribute("LAST_MOD_PERSON_ID")
		if oNode.GetAttribute("ENTRY_PERSON_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.ENTRY_PERSON_ID=" & oNode.GetAttribute("ENTRY_PERSON_ID")
		if oNode.GetAttribute("MW_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW_TEXT=" & oNode.GetAttribute("MW_TEXT")
		if oNode.GetAttribute("MF_TEXT") <> "" Then QueryString = QueryString & "&Temporary_Structures.MF_TEXT=" & oNode.GetAttribute("MF_TEXT")
		if oNode.GetAttribute("MW") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW=" & oNode.GetAttribute("MW")
		if oNode.GetAttribute("MW2") <> "" Then QueryString = QueryString & "&Temporary_Structures.MW2=" & oNode.GetAttribute("MW2")
		if oNode.GetAttribute("FORMULA2") <> "" Then QueryString = QueryString & "&Temporary_Structures.FORMULA2=" & oNode.GetAttribute("FORMULA2")
		if oNode.GetAttribute("AMOUNT") <> "" Then QueryString = QueryString & "&Temporary_Structures.AMOUNT=" & oNode.GetAttribute("AMOUNT")
		if oNode.GetAttribute("DUPLICATE") <> "" Then QueryString = QueryString & "&Temporary_Structures.DUPLICATE=" & oNode.GetAttribute("DUPLICATE")
		if oNode.GetAttribute("FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_1=" & AttribEncode(oNode,"FIELD_1")
		if oNode.GetAttribute("FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_2=" & AttribEncode(oNode,"FIELD_2")
		if oNode.GetAttribute("FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_3=" & AttribEncode(oNode,"FIELD_3")
		if oNode.GetAttribute("FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_4=" & AttribEncode(oNode,"FIELD_4")
		if oNode.GetAttribute("FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_5=" & AttribEncode(oNode,"FIELD_5")
		if oNode.GetAttribute("FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_6=" & AttribEncode(oNode,"FIELD_6")
		if oNode.GetAttribute("FIELD_7") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_7=" & AttribEncode(oNode,"FIELD_7")
		if oNode.GetAttribute("FIELD_8") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_8=" & AttribEncode(oNode,"FIELD_8")
		if oNode.GetAttribute("FIELD_9") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_9=" & AttribEncode(oNode,"FIELD_9")
		if oNode.GetAttribute("FIELD_10") <> "" Then QueryString = QueryString & "&Temporary_Structures.FIELD_10=" & AttribEncode(oNode,"FIELD_10")
		if oNode.GetAttribute("LOAD_ID") <> "" Then QueryString = QueryString & "&Temporary_Structures.LOAD_ID=" & oNode.GetAttribute("LOAD_ID")
		if oNode.GetAttribute("DATETIME_STAMP") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATETIME_STAMP=" & oNode.GetAttribute("DATETIME_STAMP")
		if oNode.GetAttribute("TXT_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_1=" & AttribEncode(oNode,"TXT_CMPD_FIELD_1")
		if oNode.GetAttribute("TXT_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_2=" & AttribEncode(oNode,"TXT_CMPD_FIELD_2")
		if oNode.GetAttribute("TXT_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_3=" & AttribEncode(oNode,"TXT_CMPD_FIELD_3")
		if oNode.GetAttribute("TXT_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.TXT_CMPD_FIELD_4=" & oNode.GetAttribute("TXT_CMPD_FIELD_4")
		if oNode.GetAttribute("INT_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_1=" & oNode.GetAttribute("INT_BATCH_FIELD_1")
		if oNode.GetAttribute("INT_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_2=" & oNode.GetAttribute("INT_BATCH_FIELD_2")
		if oNode.GetAttribute("INT_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_3=" & oNode.GetAttribute("INT_BATCH_FIELD_3")
		if oNode.GetAttribute("INT_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_4=" & oNode.GetAttribute("INT_BATCH_FIELD_4")
		if oNode.GetAttribute("INT_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_5=" & oNode.GetAttribute("INT_BATCH_FIELD_5")
		if oNode.GetAttribute("INT_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_BATCH_FIELD_6=" & oNode.GetAttribute("INT_BATCH_FIELD_6")
		if oNode.GetAttribute("INT_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_1=" & oNode.GetAttribute("INT_CMPD_FIELD_1")
		if oNode.GetAttribute("INT_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_2=" & oNode.GetAttribute("INT_CMPD_FIELD_2")
		if oNode.GetAttribute("INT_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_3=" & oNode.GetAttribute("INT_CMPD_FIELD_3")
		if oNode.GetAttribute("INT_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.INT_CMPD_FIELD_4=" & oNode.GetAttribute("INT_CMPD_FIELD_4")
		if oNode.GetAttribute("REAL_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_1=" & oNode.GetAttribute("REAL_BATCH_FIELD_1")
		if oNode.GetAttribute("REAL_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_2=" & oNode.GetAttribute("REAL_BATCH_FIELD_2")
		if oNode.GetAttribute("REAL_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_3=" & oNode.GetAttribute("REAL_BATCH_FIELD_3")
		if oNode.GetAttribute("REAL_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_4=" & oNode.GetAttribute("REAL_BATCH_FIELD_4")
		if oNode.GetAttribute("REAL_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_5=" & oNode.GetAttribute("REAL_BATCH_FIELD_5")
		if oNode.GetAttribute("REAL_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_BATCH_FIELD_6=" & oNode.GetAttribute("REAL_BATCH_FIELD_6")
		if oNode.GetAttribute("REAL_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_1=" & oNode.GetAttribute("REAL_CMPD_FIELD_1")
		if oNode.GetAttribute("REAL_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_2=" & oNode.GetAttribute("REAL_CMPD_FIELD_2")
		if oNode.GetAttribute("REAL_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_3=" & oNode.GetAttribute("REAL_CMPD_FIELD_3")
		if oNode.GetAttribute("REAL_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.REAL_CMPD_FIELD_4=" & oNode.GetAttribute("REAL_CMPD_FIELD_4")
		if oNode.GetAttribute("DATE_BATCH_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_1=" & oNode.GetAttribute("DATE_BATCH_FIELD_1")
		if oNode.GetAttribute("DATE_BATCH_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_2=" & oNode.GetAttribute("DATE_BATCH_FIELD_2")
		if oNode.GetAttribute("DATE_BATCH_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_3=" & oNode.GetAttribute("DATE_BATCH_FIELD_3")
		if oNode.GetAttribute("DATE_BATCH_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_4=" & oNode.GetAttribute("DATE_BATCH_FIELD_4")
		if oNode.GetAttribute("DATE_BATCH_FIELD_5") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_5=" & oNode.GetAttribute("DATE_BATCH_FIELD_5")
		if oNode.GetAttribute("DATE_BATCH_FIELD_6") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_BATCH_FIELD_6=" & oNode.GetAttribute("DATE_BATCH_FIELD_6")
		if oNode.GetAttribute("DATE_CMPD_FIELD_1") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_1=" & oNode.GetAttribute("DATE_CMPD_FIELD_1")
		if oNode.GetAttribute("DATE_CMPD_FIELD_2") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_2=" & oNode.GetAttribute("DATE_CMPD_FIELD_2")
		if oNode.GetAttribute("DATE_CMPD_FIELD_3") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_3=" & oNode.GetAttribute("DATE_CMPD_FIELD_3")
		if oNode.GetAttribute("DATE_CMPD_FIELD_4") <> "" Then QueryString = QueryString & "&Temporary_Structures.DATE_CMPD_FIELD_4=" & oNode.GetAttribute("DATE_CMPD_FIELD_4")
		if oNode.GetAttribute("LEGACY_REG_NUMBER") <> "" Then QueryString = QueryString & "&Temporary_Structures.LEGACY_REG_NUMBER=" & oNode.GetAttribute("LEGACY_REG_NUMBER")
		
		if bDebugPrint then 
			Response.Write QueryString & "<BR>"
			'Response.End
		end if			
		trace QueryString, 20
		'Response.Write QueryString
		'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
		oXMLHTTP.open "POST", URL, False
		oXMLHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
		oXMLHTTP.setRequestHeader "User-Agent", "ChemInv"
	
		oXMLHTTP.send QueryString

		StatusCode = oXMLHTTP.status
		If StatusCode <> "200" then
			httpResponse = oXMLHTTP.responseText
			'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
		Else
			httpResponse = oXMLHTTP.responseText
		End If
		'httpResponse = CShttpRequest2("POST", RegServerName , "/chem_reg/reg/reg_post_action.asp", "ChemInv", QueryString)

		out2 = httpResponse
		Response.Write out2 & "=RegSubstance Return Value<BR>" 
		trace out2 & "=RegSubstance Return Value<BR>", 15
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
			Case "new_batch"
				regNumber = arrRegData(1)
				batchNumber = arrRegData(2)
				regID = arrRegData(3)
				cpdInternalID = arrRegData(4)
			Case else
				'single duplicate case
				regID = ""
				batchNumber = ""

'				Select Case lcase(reg_parameter)
'					Case "user_input"
'						if instr(lcase(action),"duplicate")>0 then
'							regID = ""
'							batchNumber = ""
'						else
'							batchNumber = arrRegData(2)
'							regID = arrRegData(3)
'							cpdInternalID = arrRegData(4)
'						end if
'					
'					Case "new_batch"
'						batchNumber = arrRegData(2)
'						regID = arrRegData(3)
'						cpdInternalID = arrRegData(4)
'					
'					Case "override"
'						regID = GetRegID(regNumber,user_id,user_pwd)
'						if isNull(regID) then 
'							regID = ""
'							batchNumber = ""
'						else
'							batchNumber = "1"
'						end if					
'					Case "unique_del_temp"
					
'				End Select
		end Select
		
		Set oRegIDNode = oPlateXML.createAttribute("REG_ID_FK")
		oRegIDNode.Value = cstr(regID)
		oParentNode.SetAttributeNode(oRegIDNode)
		Set oBatchNumberNode = oPlateXML.createAttribute("BATCH_NUMBER_FK")
		oBatchNumberNode.Value = batchNumber
		oParentNode.SetAttributeNode(oBatchNumberNode)

		'Response.End
		if bDebugPrint then Response.Write out2 & "=RegSubstance Return Value<BR>" 
	Next
	' set oXMLHTTP = nothing
elseif RegisterCompounds = "false" then
	Credentials = "&CSUserName=" & CSUserName & "&CSUSerID=" & CSUSerID
	For Each oNode In nlCompounds
		Set oParentNode = oNode.parentNode
		SubstanceName = oNode.GetAttribute("SUBSTANCE_NAME")
		Structure = oNode.GetAttribute("BASE64_CDX")
		CAS = oNode.GetAttribute("CAS")
		ACX_ID = oNode.GetAttribute("ACX_ID")
		Density = oNode.GetAttribute("DENSITY")
		cLogP = oNode.GetAttribute("CLOGP")
		Rotatable_Bonds = oNode.GetAttribute("ROTATABLE_BONDS")
		Tot_Pol_Surf_Area = oNode.GetAttribute("TOT_POL_SURF_AREA")
		HBond_Acceptors = oNode.GetAttribute("HBOND_ACCEPTORS")
		HBond_Donors = oNode.GetAttribute("HBOND_DONORS")
		ALT_ID_1 = oNode.GetAttribute("ALT_ID_1")
		ALT_ID_2 = oNode.GetAttribute("ALT_ID_2")
		ALT_ID_3 = oNode.GetAttribute("ALT_ID_3")
		ALT_ID_4 = oNode.GetAttribute("ALT_ID_4")
		ALT_ID_5 = oNode.GetAttribute("ALT_ID_5")
		'Response.Write Density & ":" & cLogP & ":" & Rotatable_Bonds & ":" & Tot_Pol_Surf_Area & ":" & HBond_Acceptors & ":" & HBond_Donors & "<BR>"		

		QueryString = "inv_compounds.Substance_Name=" & SubstanceName
		QueryString = QueryString & "&RegisterIfConflicts=true"
		QueryString = QueryString & "&ResolveConflictsLater=1"
		QueryString = QueryString & "&inv_compounds.CAS=" & CAS
		QueryString = QueryString & "&inv_compounds.ACX_ID=" & ACX_ID
		QueryString = QueryString & "&inv_compounds.Density=" & Density
		QueryString = QueryString & "&inv_compounds.cLogP=" & cLogP
		QueryString = QueryString & "&inv_compounds.Rotatable_Bonds=" & Rotatable_Bonds
		QueryString = QueryString & "&inv_compounds.Tot_Pol_Surf_Area=" & Tot_Pol_Surf_Area
		QueryString = QueryString & "&inv_compounds.HBond_Acceptors=" & HBond_Acceptors
		QueryString = QueryString & "&inv_compounds.HBond_Donors=" & HBond_Donors								
		QueryString = QueryString & "&inv_compounds.ALT_ID_1=" & ALT_ID_1
		QueryString = QueryString & "&inv_compounds.ALT_ID_2=" & ALT_ID_2
		QueryString = QueryString & "&inv_compounds.ALT_ID_3=" & ALT_ID_3
		QueryString = QueryString & "&inv_compounds.ALT_ID_4=" & ALT_ID_4
		QueryString = QueryString & "&inv_compounds.ALT_ID_5=" & ALT_ID_5
		'QueryString = QueryString & "&inv_compounds.Structure=" & Server.URLEncode(Structure)
		QueryString = QueryString & "&inv_compounds.Structure=" & Structure
		QueryString = QueryString & Credentials
		trace QueryString, 20
		if bDebugPrint then 
			Response.Write QueryString & "<BR>"
			Response.End
		end if
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/CreateSubstance.asp", "ChemInv", QueryString)
		out2 = httpResponse
		'Response.Write out2 & "=CreateSubstance Return Value<BR>" 
		trace out2 & "=CreateSubstance Return Value", 15
		oCmpdResultXML.loadXML(out2)
		oCmpdResultXML.setProperty "SelectionLanguage","XPath"
		'substance already exists
		Set nlExistingSubstances = oCmpdResultXML.selectNodes("//EXISTINGSUBSTANCE")
		if nlExistingSubstances.Length > 0 then Set nlToCheck = nlExistingSubstances
		'substance created
		Set nlNewSubstances = oCmpdResultXML.selectNodes("//NEWSUBSTANCE")
		if nlNewSubstances.Length > 0 then Set nlToCheck = nlNewSubstances
		'duplicate substance
		Set nlDuplicateSubstances = oCmpdResultXML.selectNodes("//DUPLICATESUBSTANCE")
		if nlDuplicateSubstances.Length > 0 then Set nlToCheck = nlDuplicateSubstances
		'conflicting substances
		Set nlConflictingSubstances = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES")
		if nlConflictingSubstances.Length > 0 then Set nlToCheck = oCmpdResultXML.selectNodes("//CONFLICTINGSUBSTANCES/CONFLICT")
		'RCNode means Registered Compound Node
		if isObject(nlToCheck) then
			For Each oRCNode in nlToCheck
				oSubstanceResultsNode.appendChild(oRCNode)				
				CompoundID = oRCNode.GetAttribute("CompoundID")
			Next
		end if
		Set oCompoundIDNode = oPlateXML.createAttribute("COMPOUND_ID_FK")
		oCompoundIDNode.Value = CompoundID
		oParentNode.SetAttributeNode(oCompoundIDNode)
	Next
end if

'check for solvent name if found then do a lookup and add if its not there
Set nlSolventNodes = oPlateXML.selectNodes("//*[string-length(@SOLVENT)>0]")
Set oSolventDict = server.CreateObject("scripting.dictionary")
For Each oNode In nlSolventNodes
	solventName = oNode.selectSingleNode("@SOLVENT").nodeValue
	If not oSolventDict.Exists(solventName) then
		QueryString = "TableName=inv_solvents"
		QueryString = QueryString & "&TableValue=" & SolventName
		QueryString = QueryString & "&InsertIfNotFound=true"
		httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/LookUpValue.asp", "ChemInv", QueryString)
		if instr(httpResponse,"Error")>0 then
			solventID = null
		else
			solventID = httpResponse
			oSolventDict.Add solventName,solventID
		end if
	else
		solventID = oSolventDict.Item(solventName)	
	end if
	oNode.removeAttribute("SOLVENT")
	Set oSolventIDNode = oPlateXML.createAttribute("SOLVENT_ID_FK")
	oSolventIDNode.Value = solventID
	oNode.SetAttributeNode(oSolventIDNode)
Next


If bXMLDebug then oPlateXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug02_CreatePlateXML.xml")) 
' Set up an ADO command
' can't use GetInvCommand b/c it calls GetInvConnection which references the Request object
connection_array = Application("base_connection" & "cheminv")
ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & CSUserName & ";" & Application("PWDKeyword") & "=" & CSUserID
Set Conn = Server.CreateObject("ADODB.Connection")
Conn.Open ConnStr 
Set Cmd = Server.CreateObject("ADODB.Command")
Cmd.ActiveConnection = Conn
Cmd.CommandType = adCmdStoredProc
Cmd.CommandText = Application("CHEMINV_USERNAME") & ".CreatePlateXML"

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pPlateXML", AdLongVarChar, 1, len(oPlateXML.xml) + 1, oPlateXML.xml)
Cmd.Properties("SPPrmsLOB") = TRUE
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	'Conn.Execute("Alter session set sql_trace = true")
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CreatePlateXML")
	'on error resume next
	'Cmd.Execute
	'Response.Write err.Description & ":" & err.number
	'Conn.Execute("Alter session set sql_trace = false")
End if
oRootNode.appendChild(oSubstanceResultsNode)
Set oPlateIDNode = oResultXML.createAttribute("PLATE_ID")
oPlateIDNode.Value = trim(Cmd.Parameters("RETURN_VALUE"))
oRootNode.SetAttributeNode(oPlateIDNode)
oResultXML.appendChild(oRootNode)
'Response.Write trim(Cmd.Parameters("RETURN_VALUE"))
'Return an XML doc with the plateIDs and substance registration info
'Response.Write Cmd.Parameters("RETURN_VALUE")
Cmd.Properties("SPPrmsLOB") = FALSE
Response.Write oResultXML.xml 
If bXMLDebug then oResultXML.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug03_CreatePlateXML.xml")) 
'Clean up
Conn.Close
Set oPlateXML = nothing
Set oResultXML = nothing
set oCmpdResultXML = nothing
Set nlCompounds = nothing
Set nlExistingSubstances = nothing
Set nlNewSubstances = nothing
Set nlDuplicateSubstances = nothing
Set nlToCheck = nothing
Set Conn = Nothing
Set Cmd = Nothing

Response.End


Sub SetSolventID(solventName)

End Sub


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


'DJP: Added 10/7/2004 for IM8SR2
Function GetRegID(RegNumber, user_id, user_pwd)
	If (Application("RegServerName") = "NULL")then
		Response.Write "<center><BR><BR><BR><BR><span class=""GUIFeedback"">You have insufficient Privileges to view Registration Database<BR>Contact your system administrator.</span></center>"
		Response.end
	End if
	
	if CBool(Application("UseNotebookTable")) then
		notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS NoteBook," 
	else
		notebookSQL = " batches.notebook_text AS NoteBook,"
	end if
	strSQL = "SELECT" &_
			 " reg_id " &_
			 " FROM reg_numbers" &_
			 " WHERE reg_numbers.reg_number =?" 

	'Response.Write "<BR><BR>"
	'Response.Write strSQL & ":" & RegNumber
	'Response.end
	connection_array = Application("base_connection" & "invreg")
	'Cannot use the oledb connection because it truncates the Base64_cdx Long field
	'ConnStr =  "dsn=chem_reg;UID=" & UserName & ";PWD=" & UserID
	'Now that reg uses clobs we can use the udl
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & user_id & ";" & Application("PWDKeyword") & "=" & user_pwd
	'Response.Write "<BR><BR><BR><BR>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr 

	'GetRegConnection()
	Set Cmd = GetCommand(Conn, strSQL, &H0001) 'adCmdText
	Cmd.Parameters.Append Cmd.CreateParameter("RegNumber", 200, 1, 255, RegNumber)
	Set rsRegID = Cmd.Execute
	
	if not (rsRegID.BOF or rsRegID.EOF) then
		RegID = rsRegID("reg_id")
		GetRegID = RegID
	else
		GetRegID = null
	end if
End Function

Function AttribEncode(ByRef oNode, attribName)
	AttribEncode = Server.URLEncode(oNode.GetAttribute(attribName))
End Function


</SCRIPT>
