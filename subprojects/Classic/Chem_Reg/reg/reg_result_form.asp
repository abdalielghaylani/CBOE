<%@ LANGUAGE=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Response.expires=0
Response.Buffer = true

dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>

<html>
<head>
<%'start GIVADAUN customization - hide buttons%>
<link rel="Stylesheet" media="print" href="print.css">
<%'end GIVADAUN customization - hide buttons%>

<script language = "javascript">
	var approveButtonOverride = false
	var reg_id=""
	var cpdDBCounter=""
	var batch_row_id = ""
</script>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/reg_security.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<script language = "javascript">
	var db_record_count = "<%=DBRecordCount%>"
	//db_record_count = get_db_record_count("reg_numbers")
</script>
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->
<%
'on error resume next
edit_structure = Request.QueryString("edit_structure")
formmode = UCase(request("formmode"))
'uniqueid = Request.QueryString("unique_ID")
base64 = Session("BASE64_CDX" & uniqueid & dbkey & formgroup)
%>

<script language="javascript">
formmode="<%=formmode%>"

	function checkQAState(theform, elm,initial_appr, initial_qc) {
	var fullfieldname = elm.name.replace("_orig", "")
	currentName = ""
	otherName = ""
	uidTemp = fullfieldname.split(":")
	uid_value = uidTemp[0]
	qc_name = uid_value + ":Reg_Quality_Checked.Quality_Checked"
	approved_name = uid_value + ":Reg_Approved.Approved"

	currentValueTemp = eval('MainWindow.document.forms[0].elements["' + fullfieldname + '_orig' + '"]')
	currentValueTemp2= currentValueTemp.value
	currentValueTemp3 = currentValueTemp2.split(":")
	currentValue = currentValueTemp3[0]
	checkedCurrent = currentValueTemp.checked
	if(fullfieldname.indexOf('Approved') != -1){
		otherName = "Quality_Checked"
		currentName = "Approved"
		
		otherValueTemp = eval('MainWindow.document.forms[0].elements["' + qc_name + '_orig' + '"]')
		otherValueTemp2 =otherValueTemp.value
		otherValueTemp3 =otherValueTemp2.split(":")
		otherValue =otherValueTemp3[0]
		checkedOther = otherValueTemp.checked
	}
	else {
		otherName = "Approved"
		currentName = "Quality_Checked"
		otherValueTemp = eval('MainWindow.document.forms[0].elements["' +  approved_name + '_orig' + '"]')
		otherValueTemp2 =otherValueTemp.value
		otherValueTemp3 =otherValueTemp2.split(":")
		otherValue =otherValueTemp3[0]
		checkedOther = otherValueTemp.checked
	}
	if(checkedOther != checkedCurrent){
		if ((currentValueTemp.checked == true)&&(currentName == "Quality_Checked")){
			alert("cannot quality check if record is not approved")
			currentValueTemp.checked = false
		}
		else{
			if((currentValueTemp.checked == false)&&(currentName =="Approved")){
				alert("cannot remove approval and set quality check")
				currentValueTemp.checked = true
			}
			else{
				UpdateFieldVal(theform, elm)
			}
		}
	}	
	UpdateFieldVal(theform, elm)
}

///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// Type 1 is standard dialog used for funcitons like Move Container, ChangeQty etc
// Type 2 is the larger dialog used for Create/Edit Container and Substance selector
// Type 3 is the location browser dialog used from the Browse link
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type)
{
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 850px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";
	WindowDef_4 = "height=450, width= 550px, top=50, left=200";
	WindowDef_5 = "height=600, width= 800px, top=0, left=100";		
	var WindowDef = eval("WindowDef_" + type);
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	DialogWindow = window.open(url,name,attribs);
	return DialogWindow;
}

</script>

<title>Registration Form</title>

</head>

<body  <%=default_body%>>

<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_vbs.asp"-->


<input type="hidden" name="Batches.Last_Mod_Person_ID" value="<%=current_person_ID%>"><input type="hidden" name="Batches.Last_Mod_Date" value="<%=Date()%>"><input type="hidden" name="Batches.Salt_Name" value="no_salt"><input type="hidden" name="Batches.Salt_Internal_ID" value="1"><input type="hidden" name="Batches.Scientist_ID" value><input type="hidden" name="orig_required_fields" value="<%=GetFormGroupVal(dbkey, formgroup, kRequiredFields)%>"><%if not UCase(formmode) = "EDIT_RECORD" then%>
<input type="hidden" name="return_location" value="<%=Session("CurrentLocation" & dbkey & formgroup)%>"><%end if%>

<table border="0"><tr><td>
<%
'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BatchRS (below is the recordset that is pulled for each record generated
'on error resume next 
'create connection for use by all recordsets 
'on error resume next

Set DataConn = getRegConn(dbkey, formgroup)
if DataConn.State=0 then ' assume user has been logged out
	DoLoggedOutMsg()
end if
'UpdateBaseRecordCount dbkey, formgroup, DataConn

DBMSUser_ID = Session("CurrentUser" & dbkey)
current_person_ID = getValueFromTablewConn(DataConn, "People", "User_ID", DBMSUser_ID, "Person_ID")

'get base table recordset
Dim sql
Dim cmd
Dim BaseRS
Set BaseRS_cmd = Server.CreateObject("ADODB.COMMAND")
BaseRS_cmd.commandtype = adCmdText
BaseRS_cmd.ActiveConnection = DataConn
basetable = getbasetable(dbkey,formgroup, "basetable")
if UCase(basetable) = "BATCHES" then
	bBaseRegNum = false
else
	bBaseRegNum = true
end if
if bBaseRegNum = true then
	sql = "select * from reg_numbers where reg_id = ?"
else
	sql = "select * from reg_numbers where reg_id=(select reg_internal_id from batches where batch_internal_id = ?)"
	lsBatchID = baseid

end if

BaseRS_cmd.CommandText = sql
BaseRS_cmd.Parameters.Append BaseRS_cmd.CreateParameter("pRegID", 5, 1, 0, BaseID) 
Set BaseRS = Server.CreateObject("ADODB.RECORDSET")
BaseRS.Open BaseRS_cmd

if Not (BaseRS.BOF and BaseRS.EOF) then
	
	cpdDBCounter = CLng(BaseRS("cpd_internal_id"))
	reg_ID = BaseRS("reg_ID")
	baseRegNumber = BaseRS("reg_number")
	
	
	Dim CompoundRS_cmd
	Set CompoundRS_cmd = Server.CreateObject("ADODB.COMMAND")
	CompoundRS_cmd.commandtype = adCmdText
	CompoundRS_cmd.ActiveConnection = DataConn
	sql= "select * from compound_molecule where cpd_database_counter=?"
	CompoundRS_cmd.CommandText = sql
	CompoundRS_cmd.Parameters.Append CompoundRS_cmd.CreateParameter("pCPDID", 5, 1, 0, cpdDBCounter)
	Set CompoundRS = Server.CreateObject("ADODB.RECORDSET")
	CompoundRS.Open CompoundRS_cmd
	
	Compound_Type_List=GetCompoundTypeList(DataConn)
	Compound_Type_Val =CompoundRS("Compound_Type")
	Compound_Type_Text = getValueFromTablewConn(DataConn, "Compound_Type", "Compound_Type", Compound_Type_Val, "Description")
	If UCase(formmode) = "EDIT_RECORD" then
		bCompoundType = Compound_Type_Text%>
		<script language="javascript">
			setStrucReq("<%=bCompoundType%>")
		</script>
	<%end if
	
	
	Dim CAS_RS
	Dim CAS_cmd
	Set CAS_cmd = Server.CreateObject("ADODB.COMMAND")
	CAS_cmd.commandtype = adCmdText
	CAS_cmd.ActiveConnection = DataConn
	identifier_name = "cas_number"
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	CAS_cmd.CommandText = sql
	CAS_cmd.Parameters.Append CAS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	CAS_cmd.Parameters.Append CAS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set CAS_RS = Server.CreateObject("ADODB.RECORDSET")
	Set CAS_RS=CAS_cmd.execute
	
	
	
	Dim FEMA_RS
	identifier_name = "fema_gras_number"
	
	Dim FEMA_RS_cmd
	Set FEMA_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	FEMA_RS_cmd.commandtype = adCmdText
	FEMA_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	FEMA_RS_cmd.CommandText = sql
	FEMA_RS_cmd.Parameters.Append FEMA_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	FEMA_RS_cmd.Parameters.Append FEMA_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set FEMA_RS = Server.CreateObject("ADODB.RECORDSET")
	Set FEMA_RS=FEMA_RS_cmd.execute
	
	
	Dim COLLAB_RS
	
	Dim COLLAB_RS_cmd
	Set COLLAB_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	COLLAB_RS_cmd.commandtype = adCmdText
	COLLAB_RS_cmd.ActiveConnection = DataConn
	identifier_name = "collaborator_id"
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	COLLAB_RS_cmd.CommandText = sql
	COLLAB_RS_cmd.Parameters.Append COLLAB_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	COLLAB_RS_cmd.Parameters.Append COLLAB_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set COLLAB_RS = Server.CreateObject("ADODB.RECORDSET")
	Set COLLAB_RS=COLLAB_RS_cmd.execute
	
	
	Dim GROUP_CODE_RS
	identifier_name = "group_code"
	Dim GROUP_CODE_RS_cmd
	Set GROUP_CODE_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	GROUP_CODE_RS_cmd.commandtype = adCmdText
	GROUP_CODE_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	GROUP_CODE_RS_cmd.CommandText = sql
	GROUP_CODE_RS_cmd.Parameters.Append GROUP_CODE_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	GROUP_CODE_RS_cmd.Parameters.Append GROUP_CODE_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set GROUP_CODE_RS = Server.CreateObject("ADODB.RECORDSET")
	Set GROUP_CODE_RS=GROUP_CODE_RS_cmd.execute
	
	
	Dim RNO_RS
	identifier_name = "rno_number"
	Dim RNO_RS_cmd
	Set RNO_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	RNO_RS_cmd.commandtype = adCmdText
	RNO_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	RNO_RS_cmd.CommandText = sql
	RNO_RS_cmd.Parameters.Append RNO_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	RNO_RS_cmd.Parameters.Append RNO_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set RNO_RS = Server.CreateObject("ADODB.RECORDSET")
	Set RNO_RS=RNO_RS_cmd.execute

		
	Dim ChemNames_RS
	identifier_name = "chemical_name"
	Dim ChemNames_RS_cmd
	Set ChemNames_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	ChemNames_RS_cmd.commandtype = adCmdText
	ChemNames_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	ChemNames_RS_cmd.CommandText = sql
	ChemNames_RS_cmd.Parameters.Append ChemNames_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	ChemNames_RS_cmd.Parameters.Append ChemNames_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set ChemNames_RS = Server.CreateObject("ADODB.RECORDSET")
	Set ChemNames_RS=ChemNames_RS_cmd.execute
	
	Dim ChemNamesAG_RS
	identifier_name = "chem_name_autogen"
	Dim ChemNamesAG_RS_cmd
	Set ChemNamesAG_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	ChemNamesAG_RS_cmd.commandtype = adCmdText
	ChemNamesAG_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	ChemNamesAG_RS_cmd.CommandText = sql
	ChemNamesAG_RS_cmd.Parameters.Append ChemNamesAG_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	ChemNamesAG_RS_cmd.Parameters.Append ChemNamesAG_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set ChemNamesAG_RS = Server.CreateObject("ADODB.RECORDSET")
	Set ChemNamesAG_RS=ChemNamesAG_RS_cmd.execute

	Dim Syn_RS
	identifier_name = "synonym_r"
	Dim Syn_RS_cmd
	Set Syn_RS_cmd = Server.CreateObject("ADODB.COMMAND")
	Syn_RS_cmd.commandtype = adCmdText
	Syn_RS_cmd.ActiveConnection = DataConn
	sql = "Select Alt_IDS.Identifier, Alt_IDS.ID  from Alt_IDs,Identifiers where Alt_IDs.Identifier_type=Identifiers.Identifier_type AND Alt_ids.reg_internal_ID=? AND lower(Identifiers.Identifier_Descriptor)=?"
	Syn_RS_cmd.CommandText = sql
	Syn_RS_cmd.Parameters.Append Syn_RS_cmd.CreateParameter("pRegID", 5, 1, 0, reg_ID) 
	Syn_RS_cmd.Parameters.Append Syn_RS_cmd.CreateParameter("pIdentTypeID", 200, 1, Len(identifier_name) + 1, CStr(identifier_name)) 
	Set Syn_RS = Server.CreateObject("ADODB.RECORDSET")
	Set Syn_RS=Syn_RS_cmd.execute

	Dim Compound_ProjectRS
	Dim Compound_ProjectRS_cmd
	Set Compound_ProjectRS_cmd = Server.CreateObject("ADODB.COMMAND")
	Compound_ProjectRS_cmd.commandtype = adCmdText
	Compound_ProjectRS_cmd.ActiveConnection = DataConn
	sql = "select * from compound_project where cpd_internal_id=?"
	Compound_ProjectRS_cmd.Parameters.Append Compound_ProjectRS_cmd.CreateParameter("pCPDID", 5, 1, 0, cpdDBCounter) 
	Compound_ProjectRS_cmd.CommandText = sql
	Set Compound_ProjectRS = Server.CreateObject("ADODB.RECORDSET")
	Set Compound_ProjectRS=Compound_ProjectRS_cmd.execute
	Projects_Val = Compound_ProjectRS("Project_Internal_ID")
	Projects_Text = getValueFromTablewConn(DataConn, "Projects", "Project_Internal_ID", Projects_Val, "Project_Name")

	if CBOOL(Application("SALTS_USED")) = True then
		if UCase(Application("BATCH_LEVEL")) = "SALT" then
			Dim Compound_SaltRS_cmd
			Set Compound_SaltRS_cmd = Server.CreateObject("ADODB.COMMAND")
			Compound_SaltRS_cmd.commandtype = adCmdText
			Compound_SaltRS_cmd.ActiveConnection = DataConn	
			sql= "select salt_internal_id from compound_salt where reg_internal_id = ?"
			Compound_SaltRS_cmd.Parameters.Append Compound_SaltRS_cmd.CreateParameter("reg_ID", 5, 1, 0, reg_ID) 
			Compound_SaltRS_cmd.CommandText = sql
			Set Compound_SaltRS = Server.CreateObject("ADODB.RECORDSET")
			Compound_SaltRS.Open Compound_SaltRS_cmd
			
			Salts_Val=Compound_SaltRS("Salt_Internal_ID")
			Salts_Text = getValueFromTablewConn(DataConn,"salts", "Salt_Code", Salts_Val, "Salt_Name")
			Salts_MW = getValueFromTablewConn(DataConn,"salts", "Salt_Code", Salts_Val, "Salt_MW")
			Session("SALT_NAME") = Salts_Text
			Session("SALT_MW") = Salts_MW		
		end if
	end if
	
	Dim StructuresRS_cmd
	Set StructuresRS_cmd = Server.CreateObject("ADODB.COMMAND")
	StructuresRS_cmd.commandtype = adCmdText
	StructuresRS_cmd.ActiveConnection = DataConn	
	sql = "Select * from Structures where cpd_internal_id = ?"
	StructuresRS_cmd.Parameters.Append StructuresRS_cmd.CreateParameter("pCPDID", 5, 1, 0, cpdDBCounter) 
	StructuresRS_cmd.CommandText = sql
	Set StructuresRS = Server.CreateObject("ADODB.RECORDSET")
	StructuresRS.Open StructuresRS_cmd
	StructuresRS_cmd.Parameters.Delete "pCPDID"
	
	
	if bMANAGE_SYSTEM_DUPLICATES = true and cpdDBCounter > 0 then
		'stop
		Dim DupsRS2
		Set DupsRS2_cmd = Server.CreateObject("ADODB.COMMAND")
		DupsRS2_cmd.commandtype = adCmdText
		DupsRS2_cmd.ActiveConnection = DataConn
		'on error resume next
		'SYAN modified on 11/6/2006 to fix CSBR-66529
		'sql = "Select Duplicate from Duplicates where compound_id=?"
		sql = "Select distinct Duplicate from Duplicates where compound_id <> duplicate and compound_id=?"
		'End of SYAN modification
		DupsRS2_cmd.Parameters.Append DupsRS2_cmd.CreateParameter("pCPDID", 5, 1, 0, cpdDBCounter) 
		DupsRS2_cmd.CommandText = sql
		Set DupsRS2 = Server.CreateObject("ADODB.RECORDSET")
		DupsRS2.Open DupsRS2_cmd
		
		
		
		If Not (DupsRS2.BOF and DupsRS2.EOF) then
			DupsRS2.MoveFirst
			Do While Not DupsRS2.EOF
				dup_id = DupsRS2("duplicate")
				if dup_id > 0 then
					Set RegRS1_cmd = Server.CreateObject("ADODB.COMMAND")
					RegRS1_cmd.commandtype = adCmdText
					RegRS1_cmd.ActiveConnection = DataConn
					Sql = "Select distinct reg_number from reg_numbers where cpd_internal_id=?"
					RegRS1_cmd.Parameters.Append RegRS1_cmd.CreateParameter("pCPDID", 5, 1, 0, dup_id) 
					RegRS1_cmd.CommandText = sql
					Set RegRS1 = Server.CreateObject("ADODB.RECORDSET")
					RegRS1.Open RegRS1_cmd
					
					
					If Not (RegRS1.BOF and RegRS1.EOF) then
						RegRS1.MoveFirst
						Do While Not RegRS1.EOF
							reg_temp = RegRS1("reg_number")
							if regdups <> "" then
								regdups = regdups & "," & reg_temp
							else
								regdups =reg_temp
							end if
						RegRS1.MoveNext
						Loop
					
					end if
					RegRS1.Close
				end if
				
				'SYAN modified on 11/6/2006 to fix CSBR-66529
				'if DupsString <> "" then
				'	DupsString = DupsString & "," & regdups
				'else
					DupsString = regdups
				'end if
				'End of SYAN modification
			DupsRS2.MoveNext
			loop
			DupsRS2.close
		end if
	end if
		
	
	if UCase(request("formmode")) = "EDIT_RECORD" then
		if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
			Notebooks_List= GetNotebookList(DataConn)

		end if
		if CBOOL(Application("SALTS_USED")) = True then
			if UCase(Application("BATCH_LEVEL")) = "SALT" then
				Salts_List= GetAvailSaltList(DataConn,cpdDBCounter)
				Salts_Batch_List = GetSaltsListBatch(DataConn)
			else
				Salts_List = GetSaltsList(DataConn)
				Salts_Batch_List = GetSaltsListBatch(DataConn)
			end if
		end if
		
		Solvates_List = GetSolvatesList(DataConn)	
		Salts_List = Salts_Val & ":" & Salts_Text & "," & Salts_List
		Projects_List = GetProjectsList(DataConn)
		Batch_Projects_List = GetBatchProjectsList(DataConn)

		'if StructuresRS("Structures.BASE64_CDX") <> "" then
			'Compound_Types_List= GetCompoundTypeListNoStruc(DataConn)
		'else
			Compound_Types_List= GetCompoundTypeList(DataConn)
		'end if
		People_List = GetPeopleList(DataConn)
		Sequence_List = GetSequenceList(DataConn)			
		TrueFalseList = "-1:True,0:False"
	End if
	'PRODUCT_TYPE_TEXT = CompoundRS("PRODUCT_TYPE")
	'PRODUCT_TYPE_VAL = PRODUCT_TYPE_TEXT
	'PRODUCT_TYPE_LIST = GetSelectList(Application("PRODUCT_TYPE_LOOKUP"))
'site override
entry_person_ID = BaseRS("Registrar_Person_ID")
RecordOwner_UserCode=entry_person_ID
if Not Session("SITE_ACCESS_ALL" & dbkey) = True and Application("SITES_USED") = 1  then
record_owner_location = getValueFromTablewConn(RegConn, "People", "Person_ID", RecordOwner_UserCode, "Site_ID")
current_person_location = getValueFromTablewConn(RegConn, "People", "Person_ID", current_person_ID, "Site_ID")
if  CLng(record_owner_location) = CLng(current_person_location) then
	bSiteOverride=false
else
	bSiteOverride=true
end if
	'use only if you want to tie the prefix to the site and restrict access in this manner
	if Not Session("SITE_ACCESS_ALL" & dbkey) = True then
		internal_site_code = getValueFromTablewConn(RegConn, "Sites", "Site_ID", entry_person_location, "Site_Code")
		if Application("BuildSiteID") <> "" then
			Prefix = UCase(Application("BuildSiteID") & internal_site_code)
		else
			Prefix = UCase(internal_site_code)
		end if
		SequenceID = getValueFromTablewConn(RegConn, "Sequence", "Prefix", Prefix, "Sequence_ID")
		UserValidSequenceID= SequenceID

	end if
else
	bSiteOverride=false
end if		
'start general security
	
	bToggleApproved = Session("Toggle_Approved_Flag" & dbkey)
	bToggleQualityChecked =  Session("Toggle_Quality_Check_Flag" & dbkey)
	bSetApproved = Session("Set_Approved_Flag" & dbkey)
	bSetQualityChecked =  Session("Set_Quality_Check_Flag" & dbkey)

'1: see if you have permissions to edit anything regardless of user 

	bCanEditPart = canEditPart(dbkey, "REG") 'can edit compound data or batch data

'2: determine the owners of compd and batch data
	recordOwners = Session("EditRestrictIDs" & dbkey) 'edit restrictions impossed by scope permissions
'determine if you still have rights based on 1 and 2 and set editablebatcheIDS variable
	if bCanEditPart = True and Not (recordOwners ="-1")  then
		
		EditableBatchIDS = getEditableBatches(dbkey, DataConn, recordOwners, reg_ID)
	else
		EditableBatchIDS = ""
	end if
'end general
'start compound data security

'get first batch id for owner of compound data
		
	firstBatchID = getFirstBatchID(DataConn, reg_ID)
		
	bFBApproved = getApprovedFlag(DataConn, reg_ID, firstBatchID)
		
	bFBQualityChecked = getQualityCheckedFlag(DataConn, reg_ID, firstBatchID)

'determine if compound/identifier data is editable which is associated with batch1
	bEditCompd = getEditFlag(dbkey, firstBatchID,EditableBatchIDS, "Edit_Compound_Reg")
	bEditIdent=getEditFlag(dbkey, firstBatchID,EditableBatchIDS, "Edit_IDentifiers_Reg" )
	bEditSalt=getEditFlag(dbkey, firstBatchID,EditableBatchIDS, "Edit_Salt_Reg")

'override edittability based on qc and approved flags for compound data (first batch)
	if (bFBApproved AND bFBQualityChecked)then
		if (bToggleQualityChecked) then
			bEditCompd = true
			bEditIdent = true
			if UCase(Application("Batch_Level")) = "SALT" then
				bEditSalt = true
			end if	
		else
			bEditCompd = False
			bEditIdent = False
			if UCase(Application("Batch_Level")) = "SALT" then
				bEditSalt = False
			end if	
		end if
	end if

	if (bFBApproved AND Not bFBQualityChecked)then
		if (bToggleApproved) then
			bEditCompd = true
			bEditIdent = true
			if UCase(Application("Batch_Level")) = "SALT" then
				bEditSalt = true
			end if
		else
			bEditCompd = False
			bEditIdent = False
			if UCase(Application("Batch_Level")) = "SALT" then
				bEditSalt = False
			end if
		end if
	end if

			
		
	if bEditCompd= true then
		cmpd_output = "raw" 
		struc_output = "BASE64CDX"
		Projects_Output = Projects_List
		Compound_Type_Output = Compound_Types_List
	else
		cmpd_output = "raw_no_edit"
		struc_output = "BASE64CDX_NO_EDIT"
		Projects_Output = ""
		Compound_Type_Output = ""
	end if

	if bEditIdent= true then
		ident_output = "raw" 
	else
		ident_output = "raw_no_edit"
	end if

	if UCase(Application("Batch_Level")) = "SALT" then
		if bEditSalt= true then
			Salt_Output = Salts_List
		else
			Salt_Output = ""
		end if
	end if
	
	'stop
	if UCase(formmode) = "EDIT" then ' determine value  for edit mode
		Approved_output= bFBApproved
		QualityChecked_output = bFBQualityChecked
	else
		Dim approved_fullname
		Dim quality_checked_fullname
		approved_fullname = "Reg_Approved.Approved"
		quality_checked_fullname = "Reg_Quality_Checked.Quality_Checked"
		'determine state of approved and quality check checkboxes for edit_mode
		if bBaseRegNum = true then
			workingBatchID = firstBatchID
			bApproved = getApprovedFlag(DataConn, reg_ID, workingBatchID)
			bQualityChecked = getQualityCheckedFlag(DataConn, reg_ID, workingBatchID)
		else ' if the basetable is batches then the current records is based on batch_internal_id
			workingBatchID = BaseID
			bApproved = getApprovedFlag(DataConn, reg_ID, workingBatchID)
			bQualityChecked = getQualityCheckedFlag(DataConn, reg_ID, workingBatchID)
		end if
		
		
		if Not bFBApproved AND Not bFBQualityChecked then
			if bSetApproved = true then					
				Approved_output=getQACheckBox(Reg_ID, workingBatchID,approved_fullname,"SET")
			end if
			 if bSetQualityChecked = true then
				QualityChecked_output="False"
				QualityChecked_output=getQACheckBox(Reg_ID, workingBatchID,quality_checked_fullname,"SET")
			  end if
		End if
			if (bFBApproved AND Not bFBQualityChecked) then
				if bToggleApproved = true then
					Approved_output=getQACheckState(Reg_ID, workingBatchID,approved_fullname,"TOGGLE")
				end if
				if bSetQualityChecked = true then
					QualityChecked_output=getQACheckState(Reg_ID, workingBatchID,quality_checked_fullname,"SET")
				end if
			end if

			if (bFBApproved and bFBQualityChecked) then
				if bSetQualityChecked = true then
					QualityChecked_output=getQACheckBox(Reg_ID, workingBatchID,quality_checked_fullname,"TOGGLE")
					if bToggleApproved = true then
						Approved_output=bFBApproved
						Approved_output=getQACheckBox(Reg_ID, workingBatchID,approved_fullname,"TOGGLE")
					end if
				end if
			end if
	end if
	
	'get the appropriate reg number for display. This depends ont he base table. For standard reg systems this is reg_number. But it can also be batches and it that case the
	'reg number must be appended with the batch number to indicate it's uniqueness.
	if bBaseRegNum=false then
		'support for batches as basetable
		Dim TempBatchRS
		Set TempBatchRS_cmd = Server.CreateObject("ADODB.COMMAND")
		TempBatchRS_cmd.commandtype = adCmdText
		TempBatchRS_cmd.ActiveConnection = DataConn
		sql="select batch_number from batches where batch_internal_id = ?"
		TempBatchRS_cmd.CommandText = sql
		TempBatchRS_cmd.Parameters.Append TempBatchRS_cmd.CreateParameter("pBaseIDID", 5, 1, 0, BaseID) 
		Set TempBatchRS = Server.CreateObject("ADODB.RECORDSET")
		TempBatchRS.Open TempBatchRS_cmd
		fullRegNumber = BaseRS("reg_number") & "/" & padNumber(Application("BATCH_NUMBER_LENGTH_GUI"),TempBatchRS("Batch_Number"))
		baseregnumber = fullRegNumber
		
		
	else			
		baseregnumber = BaseRS("reg_number")
	end if		
'end compound data security
 Session("PreBatchAddLocation"  & dbkey & formgroup)=Session("CurrentLocation" & dbkey & formgroup)
 'response.Write Session("PreBatchAddLocation"  & dbkey & formgroup)
commit_type = "full_commit"
'use only if you want to tie the prefix to the site and restrict access in this manner

'if Not (Session("SITE_ACCESS_ALL" & dbkey) = True) AND not (CLng(UserValidSequenceID) = CLng(CurrentRecordsequenceID)) then%>
<script language="javascript">
	//editButtonOverride =true
	//regButtonOverride = true
</script>
<%'end if%>

<input type="hidden" name="reg_id" value="<%=reg_id%>">

<script language="JavaScript">
	var  commit_type =  "<%=commit_type%>"
	var formmode = "<%=formmode%>"
	var uniqueid = "<%=baseid%>"
	reg_id = "<%=reg_id%>"
	cpdDBCounter = "<%=cpdDBCounter%>"
	windowloaded = false
</script>


<table <%=table_main_mark_edit%>>
	<tr>
		
	
<%Session("result_form_return_location")=session("currentlocation" & dbkey)%>
<table border="0" bordercolor="Silver" width="600" cellspacing="0" cellpadding="2">
	<tr>
			<td width="300" <%=td_default%>><%response.write BaseRunningIndex & " of "  & BaseTotalRecords%>
			Registered Compounds
		</td>
		
   
		<td width="200" align="right" nowrap><b><font face="Arial" size="3" color="#182889"><%=Application("display_appkey")& "&nbsp;"%>
			Structure Database</font></b>
		</td>
		
		<td width="100" <%=td_default%>>
			<script language="JavaScript">
				getMarkBtn(<%=BaseID%>)
			</script>
		</td>

	</tr>
	<%if regdups <> "" then
		response.Write "<tr><td><FONT FACE=""Arial"" SIZE=""1"" COLOR=""Red"">Registry Duplicate(s): " & regdups & "</font></tr>"
	end if%>
</table>

<table <%=table_main_L1%> width = "650">
	<tr>
		<td >
			<table border="0">
				<tr>
					<td colspan="2" width="100%" align="left">
						<table width="100%">
							<tr>
								<%'START COMPOUND SCOPE APPROVED AND/OR QUALITY CHECK  FLAGS%>
								<%if (Application("APPROVED_FLAG_USED") = 1 OR Application("QUALITY_CHECKED_FLAG_USED")= 1 )then%>
									<td width = "130">
										<table>
											<tr>
												<% 
												if Application("APPROVED_FLAG_USED") = 1 AND (Application("APPROVED_SCOPE") = "COMPOUND" or (Application("APPROVED_SCOPE") = "BATCH" AND bBaseRegNum=false)) then%>
													<td <%=td_caption_bgcolor%> width="40" nowrap align="right"><font <%=font_default_caption%>><b><%=getLabelName("Approved")%></b></font>
													</td>
												
													<td <%=td_caption_bgcolor%> width="10" align="left">
														<%if UCase(formmode) = "EDIT_RECORD" then
															Response.Write Approved_output
														else%>
															<%if bFBApproved = True Then%>
																<script language="javascript">
																	if (approveButtonOverride == false){
																		approveButtonOverride =true
																	}
																</script>
															<%Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
															else
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
															end if
														end if%>
													</td>
												<%end if%>
												
												<%if Application("QUALITY_CHECKED_FLAG_USED") = 1 AND (Application("QUALITY_CHECKED_SCOPE") = "COMPOUND" or (Application("QUALITY_CHECKED_SCOPE") = "BATCH" AND bBaseRegNum=false)) then%>
													<td <%=td_caption_bgcolor%> width="120" nowrap align="right"><font face="Arial" size="2"><b><%=getLabelName("Quality_Checked")%></b></font>
													</td>
													
													<td <%=td_caption_bgcolor%> width="10" align="center">
														<%if UCase(formmode) = "EDIT_RECORD" then
															Response.Write QualityChecked_output
														else
															if bFBQualityChecked = True Then
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
															else
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
															end if
														end if%>
													</td>
												<%end if%>
											</tr>
										</table>
									</td>
								<%end if %>
                
								<%'END COMPOUND LEVEL APPROVE AND QUALITY CHECK FLAGS%>
								<td <%=td_caption_bgcolor%> width="550" nowrap><font <%=font_default%>><strong><%=baseRegNumber%>
									</strong></font><font <%=font_default_caption%>><strong>&nbsp;&nbsp;&nbsp;Registered:</strong><font>&nbsp;<font <%=font_default_caption%>>
									<%'=BaseRS("Registry_Date")%>
									<%'SYAN changed 12/16/2003 to fix CSBR-35466%>
									<%showresult dbkey, formgroup, basers, "Reg_Numbers.Registry_Date", "raw_no_edit","8","20"%>									<font <%=font_default_caption%>><strong>
									By:&nbsp;
									</font></strong><font <%=font_default%>>
									<%person_id = BaseRS("REGISTRAR_PERSON_ID")
									user_name = getPersonDisplayName(dbkey, formgroup, person_id ,DataConn)
									
									registrar=user_name
									
						            Response.Write  registrar%></font>
						        </td>
  
					            <td valign="top" align="left" width="37">
									<script language="JavaScript"><!-- hide from older browsers
										var  commit_type =  "<%=commit_type%>"
										var formmode = "<%=formmode%>"
										var uniqueid = "<%=baseid%>"
										windowloaded = false
										// End script hiding -->
									</script>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				<tr>
      				<table <%=table_sub_main_L1%>>
						<tr>
							<td  align="left">
								<table border="0" >
									<tr>
										<td valign="top" align="left">
											<table border="1">
												<tr>
													<td><%
													ShowResult dbkey, formgroup, StructuresRS,"Structures.BASE64_CDX", struc_output, 330, 200%>
														</font>
													&nbsp;</td>
												</tr>
											</table>
										</td>

										<td valign="top" align="left">
											<div align="left">
												<table>
													<tr>
														<td colspan="2" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%> face="Arial">Compound Information</font></strong></td>
													</tr>
							
							
													<%if CBOOL(Application("PROJECTS_USED")) = True then%>
														<tr>
															<%if CBOOL(Application("PROJECTS_NAMED_OWNER")) = true then%>
																<td <%=td_caption_bgcolor%> width="160" align="right"><font <%=font_default_caption%>><b>Owner</b></font>
																</td>
															<%else%>
																<td <%=td_caption_bgcolor%> width="160" align="right"><font <%=font_default_caption%>><b><%=getLabelName("Project_Internal_ID")%></b></font>
																</td>
															<%end if%>
														
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,Compound_ProjectRS,"Compound_Project.Project_Internal_ID", Projects_List, Projects_Val,Projects_Text,0,true,"value","0" %></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("COLLABORATOR_ID") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("COLLABORATOR_ID")%>
																</b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160"><font <%=font_default%>>
																<%
																	if UCase(formmode) = "EDIT_RECORD" then
																		if Not (COLLAB_RS.EOF AND COLLAB_RS.BOF) then
																			Do While Not COLLAB_RS.EOF
																				ShowResult dbkey, formgroup, COLLAB_RS, "Alt_IDs.Identifier", ident_output, "0","15" 
																				COLLAB_RS.MoveNext
																			loop
																		else
																			
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.COLLABORATOR_ID", setDisplayTypeOverride("search","COLLABORATOR_ID") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.COLLABORATOR_ID&quot;" & ")" , "15" 
																			
																		end if
																	else
																	out_ids=""
																	test_val = ""
																		Do While Not COLLAB_RS.EOF
																			test_val  = COLLAB_RS("identifier")
																				if Trim(test_val) <> "" then
																					if out_ids <> "" then
																						out_ids = out_ids & "," &  test_val
																					else
																						out_ids =  test_val
																					end if
																				end if
																			COLLAB_RS.MoveNext
																		loop
																		Response.Write out_ids
																	end if
																	
																%></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("PRODUCT_TYPE") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("PRODUCT_TYPE")%>
																</b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>>
															<%ShowResult dbkey, formgroup, CompoundRS, "COMPOUND_MOLECULE.PRODUCT_TYPE",cmpd_output, setDisplayType("PRODUCT_TYPE"), "15" %>
															</td>
														</tr>
													<%end if%>
													<%if not checkHideField("CAS_NUMBER") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%>
																</b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>>
																<%if UCase(formmode) = "EDIT_RECORD" then
																		if Not (CAS_RS.EOF AND CAS_RS.BOF) then
																			Do While Not CAS_RS.EOF
																				ShowResult dbkey, formgroup, CAS_RS, "Alt_IDs.Identifier", ident_output, setDisplayType("CAS_NUMBER"),"15" 
																				CAS_RS.MoveNext
																			loop
																		else
																			
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.CAS_NUMBER", setDisplayTypeOverride("search","CAS_NUMBER") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.CAS_NUMBER&quot;" & ")" , "15" 
																			
																		end if
																	else
																	out_ids=""
																	test_val = ""
																		Do While Not CAS_RS.EOF
																			test_val  = CAS_RS("identifier")
																				if Trim(test_val) <> "" then
																					if out_ids <> "" then
																						out_ids = out_ids & "," &  test_val
																					else
																						out_ids =  test_val
																					end if
																				end if
																			CAS_RS.MoveNext
																		loop
																		Response.Write out_ids
																	end if%></font>
															</td>
														</tr>
													<%end if%>                            
														<%if not checkHideField("RNO_NUMBER") then%>
														<tr>
															<td <%=td_caption_bgcolor%>align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%>
																</b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>>
																<%if UCase(formmode) = "EDIT_RECORD" then
																		If Not (RNO_RS.EOF and RNO_RS.BOF) then
																			Do While Not RNO_RS.EOF
																				ShowResult dbkey, formgroup, RNO_RS, "Alt_IDs.Identifier", ident_output, setDisplayType("RNO_NUMBER"),"15" 
																			RNO_RS.MoveNext
																			loop
																		else
																			
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.RNO_NUMBER", setDisplayTypeOverride("search","RNO_NUMBER") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.RNO_NUMBER&quot;" & ")" , "15" 
																			
																		end if
																	else
																	out_ids=""
																	test_val = ""
																		Do While Not RNO_RS.EOF
																			test_val  = RNO_RS("identifier")
																				if Trim(test_val) <> "" then
																					if out_ids <> "" then
																						out_ids = out_ids & "," &  test_val
																					else
																						out_ids =  test_val
																					end if
																				end if
																			RNO_RS.MoveNext
																		loop
																		Response.Write out_ids
																	end if%></font>
															</td>
														</tr>
													<%end if%>
													                             
													<%if not checkHideField("GROUP_CODE") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%>
																</b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>>
																<%if UCase(formmode) = "EDIT_RECORD" then
																		If Not (GROUP_CODE_RS.EOF and GROUP_CODE_RS.BOF) then
																			Do While Not GROUP_CODE_RS.EOF
																				ShowResult dbkey, formgroup, GROUP_CODE_RS, "Alt_IDs.Identifier", ident_output, setDisplayType("GROUP_CODE"),"15" 
																			GROUP_CODE_RS.MoveNext
																			loop
																		else
																			
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.GROUP_CODE", setDisplayTypeOverride("search","GROUP_CODE") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.GROUP_CODE&quot;" & ")" , "15" 
																			
																		end if
																	else
																	out_ids=""
																	test_val = ""
																		Do While Not GROUP_CODE_RS.EOF
																			test_val  = GROUP_CODE_RS("identifier")
																				if Trim(test_val) <> "" then
																					if out_ids <> "" then
																						out_ids = out_ids & "," &  test_val
																					else
																						out_ids =  test_val
																					end if
																				end if
																			GROUP_CODE_RS.MoveNext
																		loop
																		Response.Write out_ids
																	end if%></font>
															</td>
														</tr>
													<%end if%>
													                             
												
													<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%>
																</b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160"><font <%=font_default%>>
																<%
																	if UCase(formmode) = "EDIT_RECORD" then
																		if Not (FEMA_RS.EOF AND FEMA_RS.BOF) then
																			Do While Not FEMA_RS.EOF
																				ShowResult dbkey, formgroup, FEMA_RS, "Alt_IDs.Identifier", ident_output, setDisplayType("FEMA_GRAS_NUMBER"),"15" 
																			FEMA_RS.MoveNext
																			loop
																		else
																			
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.FEMA_GRAS_NUMBER", setDisplayTypeOverride("search","FEMA_GRAS_NUMBER") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.FEMA_GRAS_NUMBER&quot;" & ")" , "15" 
																			
																		end if
																		
																	else
																	out_ids=""
																	test_val = ""
																		Do While Not FEMA_RS.EOF
																			test_val  = FEMA_RS("identifier")
																				if Trim(test_val) <> "" then
																					if out_ids <> "" then
																						out_ids = out_ids & "," &  test_val
																					else
																						out_ids =  test_val
																					end if
																				end if
																			FEMA_RS.MoveNext
																		loop
																		Response.Write out_ids
																	end if
																	
																%></font>
															</td>
														</tr>
													<%end if%>                             
													
																				
													<%if not checkHideField("MW") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("MW")%>
																</b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.MW2", isDerived("MW2", cmpd_output),  setDisplayType("MW"), "15"%></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("FORMULA") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FORMULA")%>
																</b></font>
															</td>
														
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.FORMULA2", isDerived("FORMULA2", cmpd_output), setDisplayType("FORMULA"), "15" %></font>
															</td>
														</tr>
													<%end if%>
													                             
													<%if not checkHideField("CHIRAL") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CHIRAL")%></b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.CHIRAL", isDerived("CHIRAL", cmpd_output),  setDisplayType("CHIRAL"),"15"%></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("CLogP") then%>
														<tr>
															<td <%=td_caption_bgcolor%>align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CLogP")%></b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.CLogP", isDerived("CLogP", cmpd_output), setDisplayType("CLogP"),"15"%></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("H_BOND_DONORS") then%>
														<tr>
															<td  <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_DONORS")%>
																</b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.H_BOND_DONORS",isDerived("H_BOND_DONORS", cmpd_output),setDisplayType("H_BOND_DONORS"),"15"%></font>
															</td>
														</tr>
													<%end if%>
													                            
													<%if not checkHideField("H_BOND_ACCEPTORS") then%>
														<tr>
															<td <%=td_caption_bgcolor%>  align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_ACCEPTORS")%></b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.H_BOND_ACCEPTORS",isDerived("H_BOND_ACCEPTORS", cmpd_output),setDisplayType("H_BOND_ACCEPTORS"),"15"%></font>
															</td>
														</tr>
													<%end if%>
													
													<!--SYAN added on 7/12/2005 to link to docmanager documents-->
													<%'stop%>
													<%IF CBool(Application("SHOW_DOCMANAGER_LINK")) then%>
														<tr>
														<td>
													   <%if Session("SEARCH_DOCS" & dbkey) then%>
															<%If Session("SUBMIT_DOCS" & dbkey) then%>
																<a href="#" onclick="OpenDialog('manageDocuments.asp?FK_value=<%=BaseRS("root_number")%>&FK_name=Root%20Number&Table_Name=REG_NUMBERS&LINK_TYPE=CHEMREGREGNUMBER', 'Documents_Window', 2); return false;" title="Manage documents associated to this compound">Manage Documents</a>
															<%else%>
																<a href="#" onclick="alert('You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.'); return false;">Manage Documents</a>
															<%end if%>
														<%else%>
															<a href="#" onclick="alert('You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.'); return false;">Manage Documents</a>
													   <%end if%>
														</td>
														</tr>
													<%end if%>

													<tr><td>
														</td>
													</tr>
													<!--End of SYAN modification-->
								
                           						</table>
											</div>
										</td>
									</tr>
								</table>
							</td>
						</tr>
									<%'Start Compound_Molecule Custom Fields%>
					<tr><td>
												<table width = "650" border="1">
													
													                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.TXT_CMPD_FIELD_1",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_1"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_2")%>>    <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.TXT_CMPD_FIELD_2",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_3") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_3")%>>    <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.TXT_CMPD_FIELD_3",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_3"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.TXT_CMPD_FIELD_4",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												           
                                                 <tr>
													                                             
                                             		<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_1") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.INT_CMPD_FIELD_1",  cmpd_output, setDisplayType("INT_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_2")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.INT_CMPD_FIELD_2",  cmpd_output, setDisplayType("INT_CMPD_FIELD_2"),"15"%>														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.INT_CMPD_FIELD_3",  cmpd_output, setDisplayType("INT_CMPD_FIELD_3"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_4")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.INT_CMPD_FIELD_4",  cmpd_output, setDisplayType("INT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                   
												
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_1") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_1")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.REAL_CMPD_FIELD_1",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_1"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.REAL_CMPD_FIELD_2",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_2"),"15"%>															</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.REAL_CMPD_FIELD_3",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.REAL_CMPD_FIELD_4",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_4"),"15"%>															</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                                
                                                <tr>
                                                <%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_1") then%>                           
													<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.DATE_CMPD_FIELD_1",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_1"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.DATE_CMPD_FIELD_2",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_2"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.DATE_CMPD_FIELD_3",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"COMPOUND_MOLECULE.DATE_CMPD_FIELD_4",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_4"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                </tr>	
										</table>
									</td>
								</tr>
					<%'	end Compound_Molecule Custom Fields%>		
						<tr>
							<td >
								<div align="left">
									<table width = "650" border="0">
										<tr>
											<td >  
												<div align="left">
													<table width="650">
                    
														<%if not checkHideField("MW_TEXT") then%>
															<tr>
																<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MW_TEXT")%></b></font>
																</td>
															</tr>
															
															<tr>
																<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"Compound_Molecule.MW_Text",cmpd_output, setDisplayType("MW_Text"),"100"%></font>
																&nbsp;</td>
															</tr>
														<%end if %>
														                   
														<%if not checkHideField("MF_TEXT") then%>
															<tr>
																<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_TEXT")%></b></font>
																</td>
															</tr>
															
															<tr>
																<td <%=td_bgcolor%>>
																<font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS,"Compound_Molecule.MF_Text",cmpd_output,  setDisplayType("MF_Text"),"100"%></font>
															&nbsp;</td>
															</tr>
														<%end if %>
														
														<%if CBool(application("compound_types_used")) = True then%>
															<tr>
																<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Compound_Type")%></b></font>
																</td>
															</tr>
																
															<tr>
																<td<%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,CompoundRS, "Compound_Molecule.Compound_Type",Compound_Type_output, Compound_type_val,Compound_type_text,1,false,"value:validate:checkCompoundType(this.name)","0" %></font>
																</td>
															</tr>
														<%end if %>
														
														<%if CBool(application("structure_comments_text")) = True then%>
															<tr>
																<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("STRUCTURE_COMMENTS_TXT")%></b></font>
																</td>
															</tr>

															<tr>
																<td  <%=td_bgcolor%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "COMPOUND_MOLECULE.STRUCTURE_COMMENTS_TXT",cmpd_output,  setDisplayType("STRUCTURE_COMMENTS_TXT"),"100"%></font>
																</td>
															
														<%end if%>
														</tr><!--???move up???-->
														
														<%if not checkHideField("CHEMICAL_NAME") then%>
															<tr>
																<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
																</td>
															</tr>
              
															<tr>
																<td  <%=td_bgcolor%>>&nbsp;
																	<font <%=font_default%>>
																	<%if (ChemNames_RS.BOF AND ChemNames_RS.EOF) AND UCase(formmode) = "EDIT_RECORD" then
																		Response.Write "<tr><td>"
																		ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.CHEMICAL_NAME", setDisplayTypeOverride("search","CHEMICAL_NAME") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.CHEMICAL_NAME&quot;" & ")" , "75" 
																		Response.Write "</td></tr>"
																		else
																		if  Not (ChemNames_RS.BOF AND ChemNames_RS.EOF)= True then
																			theResult = ""
																			if Not UCase(formmode) = "EDIT_RECORD" then
																				Do While Not ChemNames_RS.EOF = True 
																					
																					if theResult <> "" then
																						theResult = theResult & "," & Trim(ChemNames_RS("Identifier"))
																					else
																						theResult = Trim(ChemNames_RS("Identifier"))
																					end if
						
																					ChemNames_RS.MoveNext
																				loop
																				Response.Write Server.HTMLEncode(theResult)
																			else
																				
																					Do While Not ChemNames_RS.EOF = True 
																						Response.Write "<tr><td>"
																						ShowResult dbkey, formgroup, ChemNames_RS,"Alt_IDs.Identifier", ident_output,setDisplayType("CHEMICAL_NAME"),"75"
																						Response.Write "</td></tr>"
																						ChemNames_RS.MoveNext
																					loop
																				
																			end if
																		end if
																	end if%></font>
																</td>
															</tr>
														<%end if%>
														
													
														
														<%if not checkHideField("CHEM_NAME_AUTOGEN") then %>
															<tr>
																<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%>
																	<%=add_gen_text%></b></font>
																</td>
															</tr>
															
															<tr>
																<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;
																
																<%if (ChemNamesAG_RS.BOF AND ChemNamesAG_RS.EOF) AND UCase(formmode) = "EDIT_RECORD" then
																		Response.Write "<tr><td>"
																		
																		ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.CHEM_NAME_AUTOGEN", setDisplayTypeOverride("search","CHEM_NAME_AUTOGEN") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.CHEM_NAME_AUTOGEN&quot;" & ")" , "75" 
																		Response.Write "</td></tr>"
																
																else
																	if  Not (ChemNamesAG_RS.BOF AND ChemNamesAG_RS.EOF)= True then
																			theResult = ""
																			if Not UCase(formmode) = "EDIT_RECORD" then
																				Do While Not ChemNamesAG_RS.EOF = True 
																					
																					if theResult <> "" then
																						theResult = theResult & "," & Trim(ChemNamesAG_RS("Identifier"))
																					else
																						theResult = Trim(ChemNamesAG_RS("Identifier"))
																					end if
						
																					ChemNamesAG_RS.MoveNext
																				loop
																				Response.write Server.HTMLEncode(theResult)
																			else
																			
																					Do While Not ChemNamesAG_RS.EOF = True 
																						Response.Write "<tr><td>"
																					
																						ShowResult dbkey, formgroup, ChemNamesAG_RS,"Alt_IDs.Identifier", ident_output,setDisplayType("CHEM_NAME_AUTOGEN"),"75"
																						Response.Write "</td></tr>"
																						ChemNamesAG_RS.MoveNext
																					loop
																				
																			end if
																		end if
																end if%>
															</tr>
														<%end if%>
														
														<%if not checkHideField("Synonym_R") then%>
															<tr>
																<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong><br>
																</td>
															</tr>

															<tr>
																<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;
																	<%if (Syn_RS.BOF AND Syn_RS.EOF) AND UCase(formmode) = "EDIT_RECORD" then
																			Response.Write "<tr><td>"
																			ShowInputField dbkey, formgroup, "NEW." &  reg_id & ":ALT_IDS.SYNONYM_R", setDisplayTypeOverride("search","SYNONYM_R") & ":VALIDATE:UpdateAddEditVal(&quot;NEW." &  reg_id & ":ALT_IDS.SYNONYM_R&quot;" & ")" , "75" 
																			Response.Write "</td></tr>"
																		else
																			if  Not (Syn_RS.BOF AND Syn_RS.EOF)= True then
																				theResult = ""
																				if Not UCase(formmode) = "EDIT_RECORD" then
																					Do While Not Syn_RS.EOF = True 
																						
																						if theResult <> "" then
																							theResult = theResult & "," & Trim(Syn_RS("Identifier"))
																						else
																							theResult = Trim(Syn_RS("Identifier"))
																						end if
						
																						Syn_RS.MoveNext
																					loop
																					
																					Response.write Server.HTMLEncode(theResult)
																				else
																						Do While Not Syn_RS.EOF = True 
																						
																							Response.Write "<tr><td>"
																							ShowResult dbkey, formgroup, Syn_RS,"Alt_IDs.Identifier", ident_output,setDisplayType("SYNONYM_R"),"75"
																							Response.Write "</td></tr>"
																							Syn_RS.MoveNext
																						loop
																					
																				end if
																			end if
																	end if%></font>
																</td>
															</tr>
														<%end if%>
														<!--???missing </table> tag???-->

														<!--start utilizations-->   
														<% if CBool(Application("UTILIZATION_PERMISSIONS_USED")) = true then%>
														<table <%=table_ident2%>>
															<tr>
																<td >
																	<table border="0" width = "650">
																		<tr>
																			<td <%=td_caption_bgcolor%> colspan="2" width = "650"><font <%=font_default_caption%>><b><%=getLabelName("utilization_text")%>
																				</b></font>
																			</td>
																		</tr>
																		
																		
																			<% if not UCase(formmode) = "EDIT_RECORD" then%>
																				<tr>
																					<td>
																						<%Set UtilDisplayRS = server.createobject("adodb.recordset")
																				        sql = "Select cmpd_mol_utilizations.utilization_boolean, utilizations.utilization_text from cmpd_mol_utilizations, utilizations where cmpd_mol_utilizations.utilization_id = utilizations.utilization_id and cmpd_mol_utilizations.cpd_internal_id=" & cpdDBCounter
																				        UtilDisplayRS.Open sql, DataConn
																				        
																				        if not (UtilDisplayRS.BOF and UtilDisplayRS.EOF) then
																				        	Do While Not UtilDisplayRS.EOF
																				        		response.write "<tr><td " & td_caption_bgcolor & " width = ""50%"">" & UtilDisplayRS("UTILIZATION_TEXT") & "</td>"
																				        		utilBool =  UtilDisplayRS("UTILIZATION_BOOLEAN")
																				        		if CBool(utilBool) = False then
																				        			utilBoolText = "No"
																				        		else
																				        			utilBoolText = "Yes"
																				        		end if
																				   
																				        		response.write "<td " & td_bgcolor & "width = ""50%"">" & utilBoolText  & "</td></tr>"
																								UtilDisplayRS.MoveNext
																							loop
																							UtilDisplayRS.Close
																				        else%>
																							<tr><td <%=td_bgcolor%> align "left" width = "650" >&nbsp;
																								</td>
																							</tr>
																						<%end if
																				else 'if formmode is Edit_record
																					Set UtilRS = server.createobject("ADODB.Recordset")
																					UtilRS.Open "select * from utilizations", DataConn
																					if Not (UtilRS.EOF and UtilRS.BOF)then
																						UtilRS.MoveFirst
																					else%>
																						<td <%=td_bgcolor%>><font <%=font_default%>>
																					 		No Utilizations Found
																							</font>
																						</td>
																					<%end if
																					
																					Set cmpMolUtilRS = server.createobject("ADODB.Recordset")
																					if Not (UtilRS.BOF  and UtilRS.EOF)then
																						'UtilRS.MoveFirst		
																						Do While Not UtilRS.EOF	
																					 		yes_no_not_setutilList = "-1:NOT SET,1:YES,0:NO"
																					 		yes_no_utilList = "1:YES,0:NO"	
																					 		UtilText = UtilRS("UTILIZATION_TEXT")		
																								
																					 		itemFound = false
																					 		sql = "select * from cmpd_mol_utilizations where cpd_internal_id="& cpdDBcounter & " and utilization_id=" & UtilRS("utilization_id")
																								
																					 		cmpMolUtilRS.Open sql,DataConn
																					 		if Not (cmpMolUtilRS.BOF  and cmpMolUtilRS.EOF) then
																					 			cmpMolUtilRS.MoveFirst
																					 			yes_no_utilVal = cmpMolUtilRS("UTILIZATION_BOOLEAN")
																										
																					 			if CBool(yes_no_utilVal) = True then
																					 					yes_no_utilText = "YES"
																					 			else	
																					 				if CBool(yes_no_utilVal) = False then
																					 					yes_no_utilText = "NO"
																					 				end if
																					 			end if%>
																								<tr>
																									<td <%=td_caption_bgcolor%> width = "50%"><%=UtilText%>
																									</td>
																									<td <%=td_bgcolor%> width = "50%"> <%ShowLookUpList dbkey, formgroup,cmpMolUtilRS,"CMPD_MOL_UTILIZATIONS.UTILIZATION_BOOLEAN" ,yes_no_utilList,  yes_no_utilVal,yes_no_utilTEXT,0,true,"value","0"%>
																									</td>
																								</tr>
																								<%cmpMolUtilRS.Close
																							else 'no batch_projects found so output not set value%>
																								<tr>
																									<td <%=td_caption_bgcolor%> width = "50%"><%=UtilText%>
																									</td>
																									
																									<td <%=td_bgcolor%> width = "50%"><%'This means it is a new permission that is not set
																									'below the recordset containing the cpd_internal_id of itnerest is used. This makes the cpd parsable as UID.cpdInternalid
																									ShowLookUpList dbkey, formgroup,cmpMolUtilRS,"CMPD_MOL_UTILIZATION_ID." & UtilRS("UTILIZATION_ID")& ":" & CompoundRS("cpd_database_counter") ,yes_no_not_setutilList,  yes_no_not_setutilListVal,yes_no_not_setutilListTEXT,0,true,"value","-1"%>
																									</td>
																								</tr>
																							<%end if 'if Not (cmpMolUtilRS.BOF  and cmpMolUtilRS.EOF)then
									
																							UtilRS.MoveNext
																						Loop		
																					end if 'if not formmode = "edit_record"
																				end if 'if CBool(Application("UTILIZATION_PERMISSIONS_USED")) = true%>
																			</table>
																		
																	</td>
																</tr>
															</table>
															<%end if%>
															<!--end utilizations -->
														</td>
													</tr>
												</table>
											</div>
										</td>
									</tr>
			
									<tr>
										<td <%=td_default_left%>></td>
									</tr>
                      			</table>
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
  

<%
'batch data security starts with the batchrs loop
Dim Batch_cmd
Set Batch_cmd = Server.CreateObject("ADODB.COMMAND")
Batch_cmd.commandtype = adCmdText
Batch_cmd.ActiveConnection = DataConn	
if bBaseRegNum = true then		
	sql = "Select * from Batches where reg_internal_ID=? Order by Batch_Number asc"
	theunqiueID = reg_ID
else
	sql = "Select * from Batches where batch_internal_id=?"
	theunqiueID = baseid
end if
Batch_cmd.Parameters.Append Batch_cmd.CreateParameter("theunqiueID", 5, 1, 0, theunqiueID) 
Batch_cmd.CommandText = sql
Set BatchRS = Server.CreateObject("ADODB.RECORDSET")
BatchRS.Open Batch_cmd

if Not(BatchRS.BOF and BatchRS.EOF) then
Do While Not BatchRS.EOF=True
	if bBaseRegNum=true then
		fullRegNumber = BaseRS("reg_number") & " Batch: " & padNumber(Application("BATCH_NUMBER_LENGTH_GUI"),BatchRS("Batch_Number"))
	end if		
Batch_ID = BatchRS("Batch_Internal_ID")
'support for producer and user lookups when batch is the basetable
if Application("USER_LOOKUP") = 1  or Application("PRODUCER_LOOKUP") = 1 then
	LOOKUP_TABLE_DESTINATION = "Batches" 'This is case sensitive. Must be Batches or Temporary_Structures
	if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
		lookup_row_id = Batch_ID
	else
		lookup_row_id = Tempid
	end if
	PRODUCER_ID_FIELD = UCase(Application("producer_id_field"))
	if UCase(formmode) = "EDIT_RECORD" then%>
		<script language="javascript">
			batch_row_id = "<%=lookup_row_id%>"
		</script>
		<%if Application("USER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "UID.<%=lookup_row_id%>:<%=LOOKUP_TABLE_DESTINATION%>.Scientist_ID" value ="">
		<%end if
		if Application("PRODUCER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "UID.<%=lookup_row_id%>:<%=LOOKUP_TABLE_DESTINATION%>.<%=PRODUCER_ID_FIELD%>" value ="">
		<%end if%>
	<%else
		if Application("USER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "<%=LOOKUP_TABLE_DESTINATION%>.Scientist_ID" value ="">
		<%end if
			if Application("PRODUCER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "<%=LOOKUP_TABLE_DESTINATION%>.<%=PRODUCER_ID_FIELD%>" value ="">
		<%end if
	end if
end if
'end producer and user lookup
bApproved = getApprovedFlag(DataConn, reg_ID, Batch_ID)
bQualityChecked = getQualityCheckedFlag(DataConn, reg_ID, Batch_ID)
Batch_Projects_Val = BatchRS("Batch_Project_ID")
Batch_Projects_Text = getValueFromTablewConn(DataConn, "Batch_Projects", "Batch_Project_ID", Batch_Projects_Val, "Project_Name")
				
person_id = BatchRS("SCIENTIST_ID")
People_Val=person_id
user_name = getPersonDisplayName(dbkey, formgroup, person_id ,DataConn)

People_Text = user_name
'determine batch edit based on approved and quality check flag settings and secondarily, upon
'user rights for editting
if (bApproved AND bQualityChecked)then
	if (bToggleQualityChecked) then
		bEditBatch=getEditFlag(dbkey,Batch_ID,EditableBatchIDS, "Edit_Batch_Reg" )
	else
		bEditBatch = false
	end if
end if
'giv only
if (bApproved AND Not bQualityChecked)then
	if (bToggleApproved) then
	else
		bEditBatch = false
	end if
end if

if (Not bApproved AND Not bQualityChecked)then
	bEditBatch=getEditFlag(dbkey,Batch_ID,EditableBatchIDS, "Edit_Batch_Reg" )
end if
				
				
bEditBatch=getEditFlag(dbkey,Batch_ID,EditableBatchIDS, "Edit_Batch_Reg" )

if bEditBatch = True then
	batch_output = "raw" 
	if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
		notebooks_output =Notebooks_List
	end if
	if UCase(Application("Batch_Level"))= "COMPOUND" then
		salts_output = Salts_List
	end if
else 
	notebooks_output = ""
	batch_output = "raw_no_edit"
	if UCase(Application("Batch_Level"))= "COMPOUND" then
		salts_output = ""
	end if
end if
if Application("APPROVED_SCOPE") = "BATCH" or (Application("QUALITY_CHECKED_SCOPE") = "BATCH" AND bBaseRegNum) then		
	if UCase(formmode) = "EDIT" then ' determine value of checkbox for edit mode
		Approved_output= bApproved
		QualityChecked_output = bQualityChecked
	else
		approved_fullname = "Reg_Approved.Approved"
		quality_checked_fullname = "Reg_Quality_Checked.Quality_Checked"
			'determine state of approved and quality check checkboxes for edit_mode

			if Not bApproved AND Not bQualityChecked then
				if bSetApproved = true then
					Approved_output=getQACheckBox(Reg_ID, Batch_ID,approved_fullname,"SET")
				end if
				if bSetQualityChecked = true then
					QualityChecked_output="False"
					QualityChecked_output=getQACheckBox(Reg_ID, Batch_ID,quality_checked_fullname,"SET")
				end if
			End if
			if (bApproved AND Not bQualityChecked) then
				if bToggleApproved = true then
					Approved_output=getQACheckState(Reg_ID, Batch_ID,approved_fullname,"TOGGLE")
				end if
				if bSetQualityChecked = true then
					QualityChecked_output=getQACheckState(Reg_ID, Batch_ID,quality_checked_fullname,"SET")
				end if
			end if

			if (bApproved and bQualityChecked) then
				if bSetQualityChecked = true then
					QualityChecked_output=getQACheckBox(Reg_ID, Batch_ID,quality_checked_fullname,"TOGGLE")
					if bToggleApproved = true then
						Approved_output=bApproved
						Approved_output=getQACheckBox(Reg_ID, Batch_ID,approved_fullname,"TOGGLE")
					end if
				end if
			end if
	 end if
end if				
	
		
' Get the batchID to use in the Analytics links
lsBatchID = BatchRS("batch_internal_ID")
				
%>
<%'if any of the user restrictions have changed the edittability of the record override the show edit button call so it doens't appear
	
if(bEditCompd and bEditIdent and  bEditBatch and bEditSalt) = False or bSiteOverride=true then%>
	<script language="javascript">
		if (editButtonOverride == false){
			editButtonOverride =true
		}
	</script>
<%end if

regdate = BatchRS("BATCH_REG_DATE")
lastModDate = BatchRS("LAST_MOD_DATE")
entryperson = BatchRS("BATCH_REG_PERSON_ID")
person_id = BatchRS("Batch_Reg_person_id")
user_name = getPersonDisplayName(dbkey, formgroup, person_id ,DataConn)
batch_registrar_person=user_name

entryperson = BatchRS("ENTRY_PERSON_ID")
user_name = getPersonDisplayName(dbkey, formgroup, entryperson ,DataConn)
batch_entry_person=user_name

modperson = BatchRS("LAST_MOD_PERSON_ID")
user_name = getPersonDisplayName(dbkey, formgroup, modperson ,DataConn)
mod_person=user_name

%>
<%'SYAN added 10/18/2004 to fix CSBR-48507%>
<%'GIVAUDAN HIDE print customization%>
<div class = "hideWhenPrint">
	<p>&nbsp;
</div>
<%'GIVAUDAN HIDE print customization end %>
<%'End of SYAN modification%>

<table <%=table_main_L2%> width = "680">
	<tr>
		<td <%=td_header_bgcolor%>><font <%=font_default_caption%>><b>Batch Information for <%=fullRegNumber%><br>Entered by: <%=batch_entry_person%><br>Registered: 
		<%'=regdate%> 
		<%'SYAN changed 12/16/2003 to fix CSBR-35466%>
		<%showresult dbkey, formgroup, BatchRS, "Batches.BATCH_REG_DATE", "raw_no_edit","8","20"%>									<font <%=font_default_caption%>><strong>

		by: <%=batch_registrar_person%> <br> <%if lastModDate <> "" then%> Last Modified:  
		<%'=lastModDate%> 
		<%'SYAN changed 12/16/2003 to fix CSBR-35466%>
		<%showresult dbkey, formgroup, BatchRS, "Batches.LAST_MOD_DATE", "raw_no_edit","8","20"%>									<font <%=font_default_caption%>><strong>

		By: <%=mod_person%> <%end if%> 
			</b></font>
		</td>
    
		<%'START BATCH SCOPE APPROVED AND/OR QUALITY CHECK  FLAGS
		 %>
		<%if ((Application("APPROVED_FLAG_USED") = 1 OR Application("QUALITY_CHECKED_FLAG_USED")=1) AND (Application("APPROVED_SCOPE") = "BATCH" or Application("QUALITY_CHECKED_SCOPE") = "BATCH")) AND (not UCase(basetable)="BATCHES") then%>
			
			<td width="200">
				<table width="200" border="0" bordercolor="#FFC8CB" cellspacing="0" cellpadding="2">
					<tr>
						<%if (Application("APPROVED_FLAG_USED") = 1 AND Application("APPROVED_SCOPE") = "BATCH") then%>
							<td <%=td_header_bgcolor%> width="60" nowrap align="right"><font face="Arial" size="2"><b><%=getLabelName("Approved")%></b></font>
							</td>
						
							<td <%=td_header_bgcolor%> width="10" align="center">
								<%if UCase(formmode) = "EDIT_RECORD" then
									Response.Write Approved_output
								else
									if bApproved = True Then%>
										<script language="javascript">
											if (approveButtonOverride == false){
												approveButtonOverride =true
											}
										</script>
										<%Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
									else
										Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
									end if
								end if%>
							</td>
						<%end if%>
						
						<%if (Application("QUALITY_CHECKED_FLAG_USED") = 1 AND Application("QUALITY_CHECKED_SCOPE") = "BATCH") then%>
							<td  <%=td_header_bgcolor%> width="120" nowrap align="right"><font face="Arial" size="2"><b><%=getLabelName("Quality_Checked")%></b></font>
							</td>
							
							<td <%=td_header_bgcolor%> width="10" align="center">
								<%if UCase(formmode) = "EDIT_RECORD" then
									Response.Write QualityChecked_output
								else
									if bQualityChecked = True Then
										Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
									else
										Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
									end if
								end if%>
							</td>
						<%end if%>
					</tr>
				</table>
			
<%end if%>

<%'END BATCH LEVEL APPROVE AND QUALITY CHECK FLAGS%>

<%'GIVAUDAN HIDE print customization%>
 <div class = "hideWhenPrint">
<%'GIVAUDAN HIDE print customization end %>

<td <%=td_header_bgcolor%> width = 40%>
	<%if  Session("delete_batch_reg" & dbkey) = True and Not bSiteOverride=true then%>
		<script language="javascript">
		  		document.write (getBatchDeleteBtn("<%=lsBatchID%>" ))
		 </script>
	<%end if%>
  <%'SYAN commented out on 1/3/2008 to fix CSBR-81630%>
	<%'if  Session("Search_Eval_Data" & dbkey) = True AND CBool(Application("Analytics_Used")) = True then%>
		<!--script language="javascript">
		  		getFormViewBtn("analytics_data_btn.gif",  "/<%=Application("Appkey")%>/analytics/Analytics_form.asp", "<%=BaseActualIndex%>","manage_analytics","self","analytics_form_group","<%=lsBatchID%>" )
		  		document.write ('&nbsp;')
		 </script-->
	<%'end if%>
     
	<%'if  Session("ADD_EVAL_DATA" & dbkey) = True AND CBool(Application("Analytics_Used")) = True then%>
		<!--script language="javascript">
			getFormViewBtn("enter_data_btn.gif", "/<%=Application("Appkey")%>/analytics/Analytics_DE_start.asp", "<%=BaseActualIndex%>","manage_analytics","self","analytics_form_group","<%=lsBatchID%>")
		</script-->
	<%'end if%>
 	<%'end of SYAN modification%>
<%'GIVAUDAN HIDE print customization%>
 </div>
<%'GIVAUDAN HIDE print customization end %>

	<%if CBOOL(Application("NOTEBOOK_USED")) = true then
		if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
			Notebooks_Val=BatchRS("Notebook_Internal_ID")
			Notebooks_Name = getValueFromTablewConn(DataConn, "Notebooks", "Notebook_Number", Notebooks_Val, "Notebook_Name")
			if CBool(Application("SHOW_NOTEBOOK_USER"))=true then
				Notebooks_user = getValueFromTablewConn(DataConn, "Notebooks", "Notebook_Number", Notebooks_Val, "User_Code")
				person_id = Notebooks_user
				User_Name = getPersonDisplayName(dbkey, formgroup, person_id ,DataConn)		
				Notebooks_Text = Notebooks_Name & "-" & User_Name
			else
				Notebooks_Text = Notebooks_Name 
			end if
		end if
	end if%>
</td>
</table>

<%'start batch info%> 
	<table <%=table_main_L1%> width = "650" >	
		<tr>
			<td> 
				<%'start salt/solvate batch data%>
								<%if CBOOL(Application("SALTS_USED")) = true then
									bShowBatchSalt = true
								else
									bShowBatchSalt = false
								end if
 
								if CBOOL(Application("SOLVATES_USED")) = true then
									bShowBatchSolvate = true
								else
									bShowBatchSolvate = false
								end if%>
								<%if bShowBatchSolvate = true or bShowBatchSalt = true then%>
								<table>
									<tr><td valign="top" align="left" width="650">&nbsp;
										<table border="0" >
								<tr>
									<td>
										<table>
											<tr> 
												<td <%=td_caption_bgcolor%> width = "150">  <%if not checkHideField("Formula_Weight") then%><font <%=font_default_caption%>><b><%=getLabelName("Formula_Weight")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "150"> <%if not checkHideField("Percent_Active") then%> <font <%=font_default_caption%>><b><%=getLabelName("Percent_Active")%></b><%end if%></font>
												</td>

                        <td <%=td_caption_bgcolor%> width = "150"><font <%=font_default_caption%>><b><%=getLabelName("Batch_Formula")%></b>
                          </font>
                        </td>
                      </tr>
											
											<tr>
												<%if not checkHideField("Formula_Weight") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>>                                        
													<%ShowResult dbkey, formgroup, BatchRS, "BATCHES.FORMULA_WEIGHT", isDerived("FORMULA_WEIGHT",batch_output),setDisplayType("FORMULA_WEIGHT"),"15"%></font></td>
                                                <%end if%>	                               
                                                
                                                <%if not checkHideField("Percent_Active") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>>
													 <%ShowResult dbkey, formgroup, BatchRS, "BATCHES.PERCENT_ACTIVE", isDerived("PERCENT_ACTIVE",batch_output),setDisplayType("PERCENT_ACTIVE"),"15"%></font></td>
												<%end if%>

                          <td width = "150" 
                            <%=td_bgcolor%>><font 
                              <%=font_default_caption%>>
                              <%ShowResult dbkey, formgroup, BatchRS, "BATCHES.BATCH_FORMULA", batch_output, setDisplayType("BATCH_FORMULA"),"15"%></font>
                          </td>
                        </tr>
										</table>
									</td>
								</tr>

								<tr>
									<td>
										<%if bShowBatchSalt = true then%>
										<table width="650">
											<tr>
												<td width="200" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_Name")%></font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_MW")%></font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_Equivalents")%></font></strong>
												</td>
											</tr>

											<tr>
												<td <%=td_bgcolor%>>
													<table>
													<tr><%if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                          
															                        
																<%if UCase(Application("Batch_Level")) = "SALT" then %>
																		
																	<%if CBool(Application("SALT_EDITABLE_FOR_REG_SALTS")) = true THEN%>
																		<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BatchRS, "Batches.Salt_Name", Salts_Batch_List, "0","", "BATCHES.SALT_NAME","BATCHES.SALT_MW"%>
																		</td>
																		<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_NAME",  batch_output,  setDisplayType("SALT_NAME"),"25"%>
																		</td>
																	<%else%> 
																		<td <%=td_bgcolor%>>
																		<input type = "hidden" name = "<%="UID." & batchrs("batch_internal_id")%>:BATCHES.SALT_NAME_orig", value ="<%=BatchRS("SALT_NAME")%>">
																			<%ShowInputField dbkey, formgroup, "BATCHES.SALT_NAMEdisplay",    "SCRIPT:alertNoEdit(this.form," & "&quot;BATCHES.SALT_NAME&quot;,&quot;BATCHES.SALT_MW&quot;" & ")!" & BatchRS("SALT_NAME"),"25"%>
																		</td>
																	<%end if%> 
															
																<%else%>
																	<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BatchRS, "Batches.Salt_Name", Salts_Batch_List, "0","", "BATCHES.SALT_NAME","BATCHES.SALT_MW"%>
																		</td>
																	<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_NAME",  batch_output,  setDisplayType("SALT_NAME"),"25"%>
																</td>
																<%end if%> 
															<%else%> 
															
															
															<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_NAME",  batch_output,  setDisplayType("SALT_NAME"),"25"%>
															</td>
															<%end if%> 
														</tr>
														
													</table>
												</td>
												                              
												<%if UCase(Application("Batch_Level")) = "SALT" then 
													if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>
															<%if CBool(Application("SALT_EDITABLE_FOR_REG_SALTS")) = true THEN%>
																<td <%=td_bgcolor%> ><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
																</td>	
															<%else%>
																<td <%=td_bgcolor%>>
																<input type = "hidden" name = "<%="UID." & batchrs("batch_internal_id")%>:BATCHES.SALT_MW_orig", value ="<%=BatchRS("salt_mw")%>">
																<%ShowInputField dbkey, formgroup, "BATCHES.SALT_MWdisplay",  "SCRIPT:alertNoEdit(this.form," & "&quot;BATCHES.SALT_NAME&quot;,&quot;BATCHES.SALT_MW&quot;" & ")!" & BatchRS("SALT_MW"),"12"%>
																</td>
															<%end if%> 
													   
													<%else%>                               
														<td <%=td_bgcolor%> ><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
														</td>	
													<%end if%>
												<%else%>                               
												<td <%=td_bgcolor%> ><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
												</td>
												<%end if%> 
												
												<td <%=td_bgcolor%>> <%ShowResult dbkey, formgroup, BatchRS,"Batches.Salt_Equivalents",   batch_output,setDisplayType("Salt_Equivalents"), "12"%>
												</td>
											</tr>
												<% end if
											if bShowBatchSolvate = true then%>
											<tr>
											     <td width="200" <%=td_caption_bgcolor%>> <strong><font <%=font_default_caption%>><%=getLabelName("Solvate_Name")%></font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Solvate_MW")%></font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%> ><strong><font <%=font_default_caption%>><%=getLabelName("Solvate_Equivalents")%></font></strong>
											     </td>
											</tr>

											<tr>
												<td <%=td_bgcolor%> > 
													<table>
														<tr><%if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                                 
																<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BatchRS, "Batches.Solvates_Name", Solvates_list, "0","", "BATCHES.SOLVATE_NAME","BATCHES.SOLVATE_MW"%>
																</td>                                  
															<%end if%>
															
															<td <%=td_bgcolor%>> <%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SOLVATE_NAME",   batch_output,setDisplayType("SOLVATE_NAME"),"25"%>
															</td>
														</tr>
													</table>
												</td>
												
												<td <%=td_bgcolor%>>  <%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SOLVATE_MW",  batch_output,setDisplayType("SOLVATE_MW"),"12"%>
												</td>
												
												<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Solvate_Equivalents",   batch_output,setDisplayType("Solvate_Equivalents"), "12"%>
												</td>
											</tr>
												<%end if%>
										</table>
									</td>
								</tr>
							</table>
								<%end if%>
                            <%'end salt/solvate batch data%>
                        </td>    
                    </tr> 
                    <tr>
                    </tr>
                    <%'start user/substance batch data%>
                           		  
					<tr>
						<td> 
							<table width="650" >
								<tr>
									<td>
										<table width="650" >
											<tr>
											<%'start user lookup labels
											If Application("USER_LOOKUP") = 1 or Application("USER_LOOKUP") = "" then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_Code")%></font></b> </td>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></font></b> </td>
											<%else%>
												<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
													<%'SYAN modified on 10/29/2007 to fix CSBR-80620
															if CBOOL(Application("USER_LOOKUP")) = False then
															'if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then
															'End of SYAN modification
														%>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></strong>
													</td>
													<%end if%>
												<%end if%>	
											<%end if%>	
											<%'end user lookup labels%>
											<%if not checkHideField("PURITY") then%>
											<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Purity")%></strong></font>
											</td>
											<%end if%>
											<%if not checkHideField("APPEARANCE") then%>
											<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Appearance")%></strong></font>
											</td>
											<%end if%>
											<%if not checkHideField("CREATION_DATE") then%>
											<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("Creation_Date")%></b></font><font <%=font_default%>>
											</td>
											<%end if%>
											</tr>

											<tr>
											<%'start scientist name lookup
											If Application("USER_LOOKUP") = 1 then
												if UCase(formmode)="EDIT_RECORD" AND bEditBatch = True then
													if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
														theSciValue =getValueFromTablewConn(DataConn, "People", "Person_ID", BatchRS("Scientist_ID"), "User_Code")%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "UID." & lookup_row_id & ":Batches.Chemist_Code", "SCRIPT:loadHelperFrame_edit(this.value," & lookup_row_id & ")","9"%>
														</font></td>
													<%else
														theSciValue =getValueFromTablewConn(DataConn, "People", "Person_ID", BaseRS("Scientist_ID"), "User_Code")%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "Temporary_Structures.Chemist_Code", "SCRIPT:loadHelperFrame_edit(this.value," & lookup_row_id & ")","9"%>
														</font></td>
													<%end if%>
												<%else%><td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
													<%if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
														Response.write getValueFromTablewConn(DataConn, "People", "Person_ID", BatchRS("Scientist_ID"), "User_Code")
													else
														Response.write getValueFromTablewConn(DataConn, "People", "Person_ID", BaseRS("Scientist_ID"), "User_Code")
													end if%></font></td>
												<%end if%>
													<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
												<%
												theValue = ""
													if UCase(formmode)="EDIT_RECORD" AND bEditBatch = True then
														if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
															if isNull(BatchRS("Scientist_ID")) then 
																SciID = "1"
															else 
																SciID = BatchRS("Scientist_ID")
															end if
														else
															if isNull(BaseRS("Scientist_ID")) then 
																SciID = "1"
															else 
																SciID = BaseRS("Scientist_ID")
															end if
														end if							
														sql = "Select * from People Where person_ID =" & SciID
														set rs = DataConn.Execute(sql)
														if not (rs.eof and rs.bof)then
															theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
															closers(rs)
														end if
													' This field is for display only from an input chemist code.
													' The field temporary.chemist_name doesn't exist.
														if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
															<input type = "text" name = "UID.<%=lookup_row_id%>:Batches.Chemist_Name" value = "<%=theValue%>" size = "20" onChange= "checkChemCode(this.value,'<%=lookup_row_id%>')">
															<script name = "javascript">
																	putChemCode("<%=theSciValue%>","<%=lookup_row_id%>")
															</script>
														<%else%>
															<input type = "text" name = "Temporary_Structures.Chemist_Name" value = "<%=theValue%>" size = "20" onChange= "checkTempChemCode(this.value)">
															<script name = "javascript">
																	putTempChemCode("<%=theSciValue%>")
															</script>
														<%end if
													else
														if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
															if isNull(BatchRS("Scientist_ID")) then 
																SciID = "1"
															else 
																SciID = BatchRS("Scientist_ID")
															end if
														else
															if isNull(BaseRS("Scientist_ID")) then 
																SciID = "1"
															else 
																SciID = BaseRS("Scientist_ID")
															end if							

														end if							
														sql = "Select * from People Where person_ID =" & SciID
														set rs = DataConn.Execute(sql)
														if not (rs.eof and rs.bof)then
															theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
															closers(rs)
															Response.Write theValue	
														end if
													end if%>
												</font></td>
												<%else%>
													<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
														<%'SYAN modified on 10/29/2007 to fix CSBR-80620
															if CBOOL(Application("USER_LOOKUP")) = False then
															'if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then
															'End of SYAN modification
														%>
															<td width = "150" <%=td_bgcolor%> ><%ShowLookUpList dbkey, formgroup,BatchRS,"Batches.Scientist_ID", People_List, People_Val,People_Text,0,false,"value",default_chemist_id %>
															</td>
														<%end if%>
													<%end if%>
												<%end if%>
												<%'end scientist name lookup%>
												<%if not checkHideField("PURITY") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.PURITY", batch_output, setDisplayType("PURITY"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.APPEARANCE", batch_output, setDisplayType("APPEARANCE"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("Creation_Date") then%>
												<td width = "150" <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Creation_Date",  batch_output,  "DATE_PICKER:8","10"%></font>
												</td>
												<%end if%>
											</tr>
										</table>
									</td>
								</tr> 
							</table>
						</td>
					</tr>
													
					<tr>
						<td valign="top" align="left">
                            <table width="650">
                                <tr>
									<td>
										<table width="650" >
											<%if CBool(Application("Batch_Projects_Used")) = true then%>
												<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b> <%=getLabelName("Batch_Project_ID")%></b></font>
												</td>
											<%end if%>
											<%'select a producer by choosing a user_code from a drop down list.
											If Application("PRODUCER_LOOKUP") = 1 or Application("PRODUCER_LOOKUP") = "" then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Code")%></font></b> </td>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Name")%></font></b> </td>
											<%end if%>
											<%'end select producer%>
											<%if CBool(Application("NOTEBOOK_USED")) = true then%>
												<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then%>
													<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Notebook_Number")%></b></font>
													</td>
												<%else%>
												<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Notebook_Text")%></b></font>
													</td>
												<%end if%>
											<%end if%>
											
											<%if not checkHideField("NOTEBOOK_PAGE") then%>
												<td nowrap <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Notebook_Page")%></b></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT") then%>
												<td nowrap <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("AMOUNT")%></b></font>
												</td>
                                            <%end if%>
 
                                            <%if not checkHideField("AMOUNT_UNITS") then%>
												<td nowrap <%=td_caption_bgcolor%> ><font  <%=font_default_caption%>><b><%=getLabelName("AMOUNT_UNITS")%></b></font>
												</td>
                                            <%end if%>
										</tr>
										<tr>
											
												<%if CBool(Application("Batch_Projects_Used")) = true then%>
												  	<td  valign="top" <%=td_bgcolor%>><font <%=font_default%>>
												  	<%ShowLookUpList dbkey, formgroup,BatchRS,"Batches.Batch_Project_ID", Batch_Projects_List, Batch_Projects_Val,Batch_Projects_Text,0,true,"value","0" %>
													</font>
													</td>
												<%end if%>
											
												<%'start producer name lookup
												If Application("PRODUCER_LOOKUP") = 1  then
														if UCase(formmode)="EDIT_RECORD" AND bEditBatch = True then
															if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
																if isNull(BatchRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BatchRS(PRODUCER_ID_FIELD)
																end if
															else
																if isNull(BaseRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BaseRS(PRODUCER_ID_FIELD)
																end if
															end if
															
           													theProdCode= getValueFromTablewConn(DataConn, "People", "Person_ID", prodID, "User_Code")
           													if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<%ShowInputField dbkey, formgroup, "UID." & lookup_row_id & ":Batches.Producer_Code", "SCRIPT:loadHelperFrame2_edit(this.value," & lookup_row_id & ")","9"%>
																</font></td>
															<%else%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<%ShowInputField dbkey, formgroup, "Temporary_Structures.Producer_Code", "SCRIPT:loadHelperFrame2_edit(this.value," & lookup_row_id & ")","9"%>
																</font></td>
															<%End if%>
														<%else
															if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
																if isNull(BatchRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BatchRS(PRODUCER_ID_FIELD)
																end if
															else
																if isNull(BaseRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BaseRS(PRODUCER_ID_FIELD)
																end if
															end if
															%>
															<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
															<%Response.write getValueFromTablewConn(DataConn, "People", "Person_ID", prodID, "User_Code")%>
															</font></td>
														<%end if
														if UCase(formmode)="EDIT_RECORD" AND bEditBatch = True then
															' This field is for display only from an input chemist code.
															' The field temporary.producer_name doesn't exits.
															if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
																if isNull(BatchRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BatchRS(PRODUCER_ID_FIELD)
																end if
															else 
																if isNull(BaseRS(PRODUCER_ID_FIELD)) then 
																	prodID = "1"
																else 
																	prodID = BaseRS(PRODUCER_ID_FIELD)
																end if
															end if
															sql = "Select * from People Where person_ID =" & prodID
															set rs = DataConn.Execute(sql)
															if not (rs.eof and rs.bof)then
																theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
																closers(rs)
															end if 
															if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<input type = "text" name = "UID.<%=lookup_row_id%>:Batches.Producer_Name" value = "<%=theValue%>" size = "20" onBlur= "checkProdCode(this.value,'<%=lookup_row_id%>')">
																<script name = "javascript">
																	putProdCode("<%=theProdCode%>","<%=lookup_row_id%>")
																</script>
																</font></td>
															<%else%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<input type = "text" name = "Temporary_Structures.Producer_Name" value = "<%=theValue%>" size = "20" onBlur= "checkTempProdCode(this.value)">
																<script name = "javascript">
																	putTempProdCode("<%=theProdCode%>")
																</script>
																</font></td>
															<%end if%>
														<%else
																if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
																	if isNull(BatchRS(PRODUCER_ID_FIELD)) then 
																		prodID = "1"
																	else 
																		prodID = BatchRS(PRODUCER_ID_FIELD)
																	end if
																else
																	if isNull(BaseRS(PRODUCER_ID_FIELD)) then 
																		prodID = "1"
																	else 
																		prodID = BaseRS(PRODUCER_ID_FIELD)
																	end if
																end if
																sql = "Select * from People Where person_ID =" & prodID
																set rs = DataConn.Execute(sql)
																if not (rs.eof and rs.bof)then
																	theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
																	closers(rs)
																	%>
																	<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>  
																	<%Response.Write theValue	%>
																	</font></td>
																<%end if
														end if%>
												<%end if%>
												<%'end producer code lookup%>
											<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
												<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then%>
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BatchRS,"Batches.Notebook_Internal_ID", Notebooks_List, Notebooks_Val,Notebooks_Text,0,true,"value","0" %>
													</td>
												<%else%>                                                
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Notebook_Text",  batch_output,   setDisplayType("Notebook_Text"),"25"%>
													</font></td>
												<%end if%>
												
											<%end if%>	
											<%if not checkHideField("NOTEBOOK_PAGE") then%>
												<td  <%=td_bgcolor%>><font <%=font_default%>>
													<%ShowResult dbkey, formgroup, BatchRS,"Batches.Notebook_Page",  batch_output,  setDisplayType("Notebook_Page"),"10"%></font>
												</td>
											<%end if%>

											<%if not checkHideField("AMOUNT") then%>
												<td  <%=td_bgcolor%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Amount",  batch_output,   setDisplayType("Amount"),"15"%></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT_UNITS") then%>
												<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "BATCHES.AMOUNT_UNITS",  batch_output, setDisplayType("AMOUNT_UNITS"),"15"%></font>
												</td>
											<%end if%>
										</tr>
									</table>
                                </td>
                            </tr>
                        </table>
                        <%'end user/substance batch data%>
					</td>
				</tr>

				<tr>
					<td>
						<%'start vendor and comments area%>
                        <table border="0">
							<tr>
								<td width = "100%">   
									<table>
										<tr>
											<td>  
												<table border="0" width="650">
													<%if not checkHideField("LIT_REF") then%>
														<tr>
															<td <%=td_caption_bgcolor%> colspan="3" ><font <%=font_default%>><b><%=getLabelName("LIT_REF")%></b></font>
															</td>
														</tr>

														<tr>
															<td colspan="3" <%=td_bgcolor%>  ><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Lit_Ref",   batch_output,  setDisplayType("Lit_Ref"), "72"%></font>
															</td>
														</tr>
													<%end if%>

													<tr>
														<%if not checkHideField("SOURCE") then%>
															<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("SOURCE")%></font></strong>
															</td>
														<%end if%>
                                                 
														<%if not checkHideField("VENDOR_NAME") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BatchRS("Source")) = "VENDOR" then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_NAME_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("VENDOR_NAME")%></font></strong></div>
															<%'end if%>
															</td>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BatchRS("Source")) = "VENDOR" then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_ID_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("VENDOR_ID")%></font></strong></div>
															<%'end if%>
															</td>
														<%end if%>
												
													</tr>
 
													<tr>
														<%if not checkHideField("SOURCE") then%>
															<font <%=font_default%>>
																<td nowrap  <%=td_bgcolor%>>&nbsp;<%ShowResult dbkey, formgroup,BatchRS, "BATCHES.SOURCE", batch_output, setDisplayType("SOURCE"),"15"%>
																</td>
															</font>
														<%end if%>
                                                
														<%if not checkHideField("VENDOR_NAME")  then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BatchRS("Source")) = "VENDOR" then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_NAME"><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.VENDOR_NAME",  batch_output, setDisplayType("VENDOR_NAME"),"15"%></div>
																</td>
															</font>
															<%'end if%>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BatchRS("Source")) = "VENDOR" then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_ID"><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.VENDOR_ID", batch_output, setDisplayType("VENDOR_ID"),"10"%></div>
																</td>
															</font>
															<%'end if%>
														<%end if%>
													</tr>
									</table>
                                    
                                    <table>
										<tr>
											<td>
												<table border = "0" width = "650">
													<%if not checkHideField("BATCH_COMMENT") then%>
														<tr>
															<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("BATCH_COMMENT")%></b>
																</font>
															</td>
														</tr>

														<tr>
															<td  <%=td_bgcolor%>><font <%=font_default%>>
																<%ShowResult dbkey, formgroup, BatchRS, "Batches.Batch_Comment",   batch_output,   setDisplayType("Batch_Comment"), "72"%>
															</font></td>
															
														</tr>
													<%end if%>
													
													<%if not checkHideField("PREPARATION") then%>
														<tr>
															<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("PREPARATION")%></b></font>
															</td>
														</tr>
														
														<tr>
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Preparation", batch_output,  setDisplayType("Preparation"), "72"%></font>
															</td>
														</tr>
													<%end if%>
											
													<%if not checkHideField("STORAGE_REQ_AND_WARNINGS") then%>
														<tr>
															<td <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("STORAGE_REQ_AND_WARNINGS")%></b>
																</font>
															</td>
														</tr>
														
														<tr>
															<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Storage_Req_And_Warnings",batch_output, setDisplayType("Storage_Req_And_Warnings"), "72"%>
															</font></td> 
															
														</tr>
													<%end if%>
												</table>
											</td>
										</tr>
									</table>
                                    <%'end vendor and comments area%>
								</td>
							</tr>

							<tr>
								<td>
									<table  width="650">
										<tr>
											<td <%=td_header_bgcolor%>>
												<font <%=font_header_default_2%>><b>Physical & Analytical Information</b></font>
											</td>
										</tr>
									</table>
								</td>
							</tr>

							<tr>
								<td><!--Start physical properties-->
									<table width="650">
										<tr>
											<td>  
												<table width = "650">
													
													<tr>
														<%if not checkHideField("Batches.H1NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.H1NMR")%></strong></font>   
															</td>
														<%end if%>
														
														<%if not checkHideField("Batches.H1NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.H1NMR")%> >  <font <%=font_default%>><strong><%ShowResult dbkey, formgroup, BatchRS,"Batches.H1NMR",batch_output, setDisplayType("H1NMR"),"15"%></font>   
															</td>
														<%end if%>
                                              
														<%if not checkHideField("Batches.MP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.MP")%></font></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Batches.MP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.MP")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.MP",  batch_output, setDisplayType("MP"),"15"%></font>
															</td>
														<%end if%>
													
													</tr>
													<tr>
														<%if not checkHideField("Batches.C13NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.C13NMR")%></strong></font>
															</td>
														<%end if%>	  
														
														<%if not checkHideField("Batches.C13NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.C13NMR")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.C13NMR",batch_output, setDisplayType("C13NMR"),"15"%>   </font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Batches.BP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.BP")%></font></strong></font>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.BP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.BP")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.BP",  batch_output, setDisplayType("BP"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Batches.HPLC") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.HPLC")%></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Batches.HPLC") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.HPLC")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.HPLC",batch_output, setDisplayType("HPLC"),"15"%>   </font>
															</td>
														<%end if%>	
                                              
														<%if not checkHideField("Batches.SOLUBILITY") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.SOLUBILITY")%></font></strong></font>
															</td>
														<%end if%>	 
														
														<%if not checkHideField("Batches.SOLUBILITY") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.SOLUBILITY")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.SOLUBILITY",  batch_output, setDisplayType("SOLUBILITY"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Batches.MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.MS")%></strong></font>
														</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.MS")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.MS",     batch_output, setDisplayType("MS"),"15"%></font>
															</td>
														<%end if%>		
                                               
                                              
														<%if not checkHideField("Batches.Optical_Rotation") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.Optical_Rotation")%></font></strong></font>
															</td>
														<%end if%>	
															
														<%if not checkHideField("Batches.Optical_Rotation") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.Optical_Rotation")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Optical_Rotation",    batch_output, setDisplayType("Optical_Rotation"),"15"%></font>
															</td>
														<%end if%>		
													</tr>

													<tr>
														<%if not checkHideField("Batches.LC_UV_MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.LC_UV_MS")%></font></strong>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.LC_UV_MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.LC_UV_MS")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.LC_UV_MS",  batch_output, setDisplayType("LC_UV_MS"),"15"%></font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Batches.Physical_Form") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.Physical_Form")%></font></strong></font>
															</td>
														<%end if%> 
														
														<%if not checkHideField("Batches.Physical_Form") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.Physical_Form")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.Physical_Form",  batch_output, setDisplayType("Physical_Form"),"15"%></font>
															</td>
														<%end if%>
                                                </tr>

                                                <tr>
													<%if not checkHideField("Batches.IR") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.IR")%></font></strong>
														</td>
													<%end if%>
													
													<%if not checkHideField("Batches.IR") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Batches.IR")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS, "Batches.IR",   batch_output, setDisplayType("IR"),"15"%></font>
														</td>
													<%end if%>
                                                
													<%if not checkHideField("Batches.LogD") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.LogD")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.LogD") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Batches.LogD")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.LogD",  batch_output, setDisplayType("LogD"),"15"%></font>
														</font>
														</td>  
													<%end if%>
												</tr>	

												<tr>
													<%if not checkHideField("Batches.UV_SPECTRUM") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.UV_SPECTRUM")%></font></strong>
														</td>
													<%end if%>
												 
													<%if not checkHideField("Batches.UV_SPECTRUM") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Batches.UV_SPECTRUM")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.UV_SPECTRUM",  batch_output, setDisplayType("UV_SPECTRUM"),"15"%></font>
														</td>
													<%end if%>
                                     
													<%if not checkHideField("Batches.Refractive_Index") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>><font <%=font_default_caption%>> <strong> <%=getLabelName("Refractive_Index")%></strong></font>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.Refractive_Index") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Refractive_Index")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Refractive_Index",  batch_output, setDisplayType("Refractive_Index"),"15"%></font>
														</td>
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Batches.GC") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.GC")%></font></strong>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.GC") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Batches.GC")%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.GC",  batch_output, setDisplayType("GC"),"15"%></font>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.FlashPoint") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.FlashPoint")%></strong>
														</font></strong>
														</td> 
													 <%end if%>

													<%if not checkHideField("Batches.FlashPoint") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Batches.FlashPoint")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.FlashPoint",  batch_output, setDisplayType("FlashPoint"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Batches.CHN_COMBUSTION") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.CHN_COMBUSTION")%></strong>
														</font>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.CHN_COMBUSTION") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Batches.CHN_COMBUSTION")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.CHN_COMBUSTION",  batch_output, setDisplayType("CHN_COMBUSTION"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Color") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Color")%></strong>
														</font></strong>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Color") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Color_1")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Color",  batch_output, setDisplayType("Color"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	

                                                <tr>
													<%if not checkHideField("Batches.Field_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_1") then%>                           
															<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_1")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_1",  batch_output, setDisplayType("Field_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.Field_2") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_2")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_2",  batch_output, setDisplayType("Field_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("Batches.Field_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_3") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_3")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_3",  batch_output, setDisplayType("Field_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_4") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_4")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_4",  batch_output, setDisplayType("Field_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("Batches.Field_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_5") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_5")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_5",  batch_output, setDisplayType("Field_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.Field_6") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_6")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_6",  batch_output, setDisplayType("Field_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                     
                                                <tr>
													<%if not checkHideField("Batches.Field_7") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_7")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_7") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_7")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_7",  batch_output, setDisplayType("Field_7"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Field_8") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_8")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_8") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_8")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_8",  batch_output, setDisplayType("Field_8"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                
                                                <tr>
													<%if not checkHideField("Batches.Field_9") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_9")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_9") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_9")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_9",  batch_output, setDisplayType("Field_9"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_10") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_10")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_10") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_10")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"Batches.Field_10",  batch_output, setDisplayType("Field_10"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                 <tr>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_1") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_1",  batch_output, setDisplayType("INT_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_2") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_2",  batch_output, setDisplayType("INT_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_3") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_3",  batch_output, setDisplayType("INT_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_4") then%>                           
														<td  width = "150"  <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_4",  batch_output, setDisplayType("INT_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                                                <tr>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_5")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_5") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_5",  batch_output, setDisplayType("INT_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_6") then%>                           
														<td   width = "150"  <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_6")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.INT_BATCH_FIELD_6",  batch_output, setDisplayType("INT_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
                                                <tr>
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_1",  batch_output, setDisplayType("REAL_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_2",  batch_output, setDisplayType("REAL_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_3") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_3",  batch_output, setDisplayType("REAL_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_4",  batch_output, setDisplayType("REAL_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_5") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_5",  batch_output, setDisplayType("REAL_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_6") then%>                           
														<td width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.REAL_BATCH_FIELD_6",  batch_output, setDisplayType("REAL_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                     
                                                
                                                <tr>
                                                <%if not checkHideField("BATCHES.DATE_BATCH_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_1",  batch_output, setDisplayType("DATE_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_2",  batch_output, setDisplayType("DATE_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_3") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_3",  batch_output, setDisplayType("DATE_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_4") then%>                           
														<td   width = "150"  <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_4",  batch_output, setDisplayType("DATE_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_5") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_5",  batch_output, setDisplayType("DATE_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_6") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BatchRS,"BATCHES.DATE_BATCH_FIELD_6",  batch_output, setDisplayType("DATE_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>

                                            </table>
                                        </td>
                                    </tr>
								</table>
								<!--end physical properties-->							</tr>
						</td>
					</table>
				</tr>
			</td>
		</table>
	</tr>
</td>
</table>
	</tr>
</td>
</table>
 
 <%'end batch info%> 
<%
			BatchRS.MoveNext
		Loop
	else
		Response.Write "<P><FONT FACE=""Arial"" SIZE=""2"" COLOR=""Red"">No batch information associated with this compound</FONT></P>"
	end if

	'cleanup
	'close and kill recordset before each row is generated
	CloseRS(BatchRS)
	CloseRS(BatchRS)
	CloseRS(Compound_ProjectRS)
	CloseRS(CAS_RS)
	CloseRS(Syn_RS)
	CloseRS(ChemNames_RS)
	CloseRS(StructuresRS)
	if UCase(Application("Batch_Level")) = "SALT" then
		CloseRS(Compound_SaltRS)
	end if
else
	Response.Write "<P>Record " & BaseRunningIndex & " deleted</p>"
end if
if request("formmode") = "edit_record" then
	compound_type = getValueFromTablewConn(DataConn, "compound_type", "compound_type", Compound_Type_Val, "description")

	if UCase(compound_type)= UCase(Application("NO_STRUCTURE_TEXT")) then%>
	<script language="javascript">
	reqfieldsarray=required_fields.split(",")
			for(i=0;i<reqfieldsarray.length;i++){
				testcase = reqfieldsarray[i].toLowerCase()

				if(testcase.indexOf(".structure")==-1){

					if(newstring == ""){
						newstring = reqfieldsarray[i]
					}
					else
					{
						newstring = newstring + "," + reqfieldsarray[i]
						
					}
				
				}
				override_non_chemical_submit_flag = "ALLOW"

			}
	</script>
	<%end if%>
<%end if
  %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp"-->
<script language="JavaScript">
	windowloaded = true
</script>
</body>

</html>