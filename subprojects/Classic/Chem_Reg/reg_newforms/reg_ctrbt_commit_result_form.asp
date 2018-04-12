<%@ LANGUAGE=VBScript %>
<%	
Response.expires=0
Response.Buffer = true

'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"

formmode = UCase(request("formmode"))
uniqueid = Request.QueryString("unique_ID")
%>
<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/reg_security.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<script language = "javascript">
	var db_record_count = "<%=DBRecordCount%>"
	editButtonOverride =false
	regButtonOverride = false
	deleteButtonOverride=false
		//db_record_count = get_db_record_count("temp")
</script>

<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->



<title>Chemical Registration - Review/Register Results Form View</title>
</head>
<body <%=default_body%> >
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<input type = "hidden" name="duplicate_processing" value = "">

<input type="hidden" name="Temporary_Structures.Last_Mod_Person_ID" value="<%=entry_person_ID%>">
<input type="hidden" name="Temporary_Structures.Temp_Compound_ID" value="<%=uniqueid%>">
<%if CBOOL(Application("SALTS_USED")) = false then%>
<input type="hidden" name="Temporary_Structures.Salt_Code" value="1">
<%end if%>
<%if CBOOL(Application("SOLVATES_USED")) = false then%>

<input type="hidden" name="Temporary_Structures.Solvate_ID" value="1">
<%end if%>
<input type="hidden" name="orig_required_fields" value="<%=GetFormGroupVal(dbkey, formgroup, kRequiredFields)%>">
<%
'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated
'on error resume next 
compound_registered = checkRegTracking(BaseID)
duplicate_found = checkDupTracking(BaseID)
uniqueid = BaseID
bFBApproved=true
on error resume next
base64=Session("BASE64_CDX" & uniqueid & dbkey & formgroup)
if Not compound_registered <> "" then
	Set BaseRS = Server.CreateObject("adodb.recordset")
	If Not isObject(RegConn) then
	 Set RegConn = getRegConn(dbkey, formgroup)
	end if
	if RegConn.State=0 then ' assume user has been logged out
		DoLoggedOutMsg()
	end if
	'UpdateTempRecordCount(RegConn)
	set cmd = server.CreateObject("adodb.command")
	cmd.commandtype = adCmdText
	cmd.ActiveConnection = RegConn
	sql = "Select * from temporary_structures where temp_compound_id=?"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pTempID", 5, 1, 0, BaseID) 
	
	BaseRS.Open cmd
	
	cmd.Parameters.delete "pTempID"
	'BaseRSCount = GetRecordCount(dbkey, formgroup, BaseRS)
	commit_type = BaseRS("Commit_Type")
	'user info
	DBMSUser_ID = Session("CurrentUser" & dbkey)
	
	current_person_ID = getValueFromTablewConn(RegConn, "People", "User_ID", DBMSUser_ID, "Person_ID")
	
	
	RecordOwner_UserCode = BaseRS("Entry_Person_ID")
	entry_person_ID = getValueFromTablewConn(RegConn, "People", "User_ID", RecordOwner_UserCode, "Person_ID")
	Current_UserCode =entry_person_ID
'current record owner
	
	
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
	Select Case UCase(commit_type)
		Case "FULL_COMMIT"
			Compound_Type_Val =BaseRS("Compound_Type")
			Compound_Type_Text = getValueFromTablewConn(RegConn, "Compound_Type", "Compound_Type", Compound_Type_Val, "Description")

			Projects_Val = BaseRS("Project_ID")
			Projects_Text = getValueFromTablewConn(RegConn, "Projects", "Project_Internal_ID", Projects_Val, "Project_Name")
			
			Batch_Projects_Val = BaseRS("Batch_Project_ID")
			Batch_Projects_Text = getValueFromTablewConn(RegConn, "Batch_Projects", "Batch_Project_ID", Batch_Projects_Val, "Project_Name")

			Salts_Val =BaseRS("Salt_Code")
			Salts_Text = getValueFromTablewConn(RegConn,"salts", "salt_code", Salts_Val, "Salt_Name")			
			Solvates_Val = BaseRS("Solvate_ID")
			Solvates_Text = getValueFromTablewConn(RegConn,"Solvates", "solvate_id", Solvates_Val, "Solvate_Name")			

			dup_list = getDupList(dbkey, formgroup, BaseRS("duplicate"),RegConn)

			if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
				Notebooks_Val=BaseRS("Notebook_Number")
				Notebooks_Text = getNotebookVal(RegConn, Notebooks_Val)
			end if
				Sequences_Val = BaseRS("Sequence_ID")
				Sequences_Text = getValueFromTablewConn(RegConn,"sequence", "Sequence_ID", Sequences_Val, "Prefix")			
				CurrentRecordsequenceID= Sequences_Val

			if UCase(request("formmode")) = "EDIT_RECORD" then
				if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
					Notebooks_List= GetUserNotebookList(RegConn, UCase(Session("CurrentUser" & dbkey)))
				end if
				Salts_List= GetSaltsList(RegConn)
				Salts_Batch_List = GetSaltsListBatch(RegConn)
				Solvates_List = GetSolvatesList(RegConn)
				People_List = GetPeopleList(RegConn)
				person_id = BaseRS("SCIENTIST_ID")
				user_name = getPersonDisplayName(dbkey, formgroup, person_id ,RegConn)
				People_Text = user_name
				Projects_List = GetProjectsList(RegConn)
				Batch_Projects_List = GetBatchProjectsList(RegConn)
				Compound_Types_List= GetCompoundTypeList(RegConn)
				Sequences_List = GetSequenceList(RegConn)	
				
			End if
			Session("BASE64_CDX" & BaseID & dbkey & formgroup) = BaseRS("BASE64_CDX")			
			Session("TEMP_MOL_ID" & BaseID & dbkey & formgroup) = BaseRS("MOL_ID")
			
			bShowRegIdentifiers = true
			bShowRegCmpds = true
			bShowBatchData = true
			bShowRegSalt = false
			
	Case "BATCH_COMMIT"
			
			reg_ID = BaseRS("reg_internal_ID")
			if CBool(Application("APPROVED_FLAG_USED")) = True and CBool(Application("ALLOW_BATCH_FOR_UNAPPROVED_CMPD")) = false then
				
				firstBatchID = getFirstBatchID(RegConn, reg_ID)
				bFBApproved = getApprovedFlag(RegConn, reg_ID, firstBatchID)
			else
				bFBApproved=true
			end if
			
			Reg_Number = getValueFromTablewConn(RegConn,"Reg_Numbers", "reg_ID", reg_ID, "reg_number")	
			cpdDBCounter = 	getValueFromTablewConn(RegConn,"Reg_Numbers", "reg_ID", reg_ID, "cpd_internal_ID")
			CurrentRecordsequenceID = getValueFromTablewConn(RegConn,"Compound_Molecule", "cpd_database_counter", cpdDBCounter, "sequence_id")

			
			Salts_Val =BaseRS("Salt_Code")
			Salts_Text = getValueFromTablewConn(RegConn,"salts", "salt_code", Salts_Val, "Salt_Name")			
			Solvates_Val = BaseRS("Solvate_ID")
			Solvates_Text = getValueFromTablewConn(RegConn,"Solvates", "solvate_id", Solvates_Val, "Solvate_Name")			
			Batch_Projects_List = GetBatchProjectsList(RegConn)
			Batch_Projects_Val = BaseRS("Batch_Project_ID")
			
			Batch_Projects_Text = getValueFromTablewConn(RegConn, "Batch_Projects", "Batch_Project_ID", Batch_Projects_Val, "Project_Name")
			People_List = GetPeopleList(RegConn)
			if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
				Notebooks_Val=BaseRS("Notebook_Number")
				Notebooks_Text = getNotebookVal(RegConn, Notebooks_Val)
				'Current_UserCode = getValueFromTablewConn(RegConn, "Notebooks", "Notebook_Number", Notebooks_Val, "User_Code")
			end if
			if UCase(request("formmode")) = "EDIT_RECORD" then
				if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
					Notebooks_List= GetUserNotebookList(RegConn, UCase(Session("CurrentUser" & dbkey)))
					'Notebooks_List= GetNotebookList(RegConn)
				end if
				Salts_List= GetSaltsList(RegConn)
				Salts_Batch_List = GetSaltsListBatch(RegConn)
				Solvates_List = GetSolvatesList(RegConn)
			End if
			display_type = "reg_number"
			'registered compound information recordset generation 
			'assumes cpdDBCounter and reg_ID variables are populated
			getRegData RegConn, reg_ID, cpdDBCounter,display_type 
			bShowRegIdentifiers = true
			bShowRegCmpds = true
			bShowBatchData = false
			bShowRegSalt = true
			
		
			
	Case "ADD_IDENTIFIERS"
			
			reg_ID = BaseRS("reg_internal_ID")
			Current_UserCode =  BaseRS("Entry_Person_ID")
			'Set RS = Server.CreateObject("ADODB.RECORDSET")
			'sql = "select * from batches where reg_internal_ID = " & reg_ID & " Order By Batch_Internal_ID asc"
			'sql =  "select Entry_Person_ID from batches where reg_internal_ID = ? Order By Batch_Internal_ID asc"
			'cmd.CommandText = sql
			'cmd.Parameters.Append cmd.CreateParameter("pReg_ID", 5, 1, 0, reg_ID) 
			'RS.Open cmd
			'cmd.Parameters.Delete "pReg_ID"
			'If Not (RS.BOF and RS.EOF)then
				'RS.MoveFirst
				'Notebooks_Val = RS("Notebook_Internal_ID")
				'Current_UserCode = RS("Entry_Person_ID")
				'RS.Close
				'Current_UserCode = getValueFromTablewConn(RegConn, "Notebooks", "Notebook_Number", Notebooks_Val, "User_Code")
			'end if
			if Current_UserCode = "" then
				Response.Write "no user code found"
				Response.End
			end if
			'Set RS = nothing 
			cpdDBCounter = 	getValueFromTablewConn(RegConn,"Reg_Numbers", "reg_ID", reg_ID, "cpd_internal_ID")
			CurrentRecordsequenceID = getValueFromTablewConn(RegConn,"Compound_Molecule", "cpd_database_counter", cpdDBCounter, "sequence_id")

			display_type = "reg_number"
			'registered compound information recordset generation 
			'assumes cpdDBCounter and reg_ID variables are populated
			getRegData RegConn, reg_ID, cpdDBCounter,display_type 
			bShowRegIdentifiers = true
			bShowRegCmpds = true
			bShowBatchData = true
			bShowRegSalt = true
			'BaseRS.MoveFirst
			''sql = "Select * from Structures where cpd_internal_ID = " & cpdDBCounter
			'Set MoleculesRS = RegConn.Execute(sql)

	Case "ADD_SALT"
			
			cpdDBCounter  = BaseRS("cpd_internal_ID")
			Current_UserCode = BaseRS("Entry_Person_ID")
			reg_ID = ""
			CurrentRecordsequenceID = getValueFromTablewConn(RegConn,"Compound_Molecule", "cpd_database_counter", cpdDBCounter, "sequence_id")
			Salts_Val =BaseRS("Salt_Code")
			Salts_Text = getValueFromTablewConn(RegConn,"salts", "salt_code", Salts_Val, "Salt_Name")		
			Solvates_Val = BaseRS("Solvate_ID")
			Solvates_Text = getValueFromTablewConn(RegConn,"Solvates", "solvate_id", Solvates_Val, "Solvate_Name")			
			Salts_Batch_List = GetSaltsListBatch(RegConn)
			Batch_Projects_List = GetBatchProjectsList(RegConn)
			Batch_Projects_Val = BaseRS("Batch_Project_ID")
			Batch_Projects_Text = getValueFromTablewConn(RegConn, "Batch_Projects", "Batch_Project_ID", Batch_Projects_Val, "Project_Name")
			People_List = GetPeopleList(RegConn)
			if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
				Notebooks_Val=BaseRS("Notebook_Number")
				Notebooks_Text = getNotebookVal(RegConn, Notebooks_Val)
				'Current_UserCode = getValueFromTablewConn(RegConn, "Notebooks", "Notebook_Number", Notebooks_Val, "User_Code")
			end if
			if UCase(request("formmode")) = "EDIT_RECORD" then
				if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
					Notebooks_List= GetUserNotebookList(RegConn, UCase(Session("CurrentUser" & dbkey)))

				end if
				Salts_List= GetAvailSaltList(RegConn, cpdDBCounter  )
				Solvates_List = GetSolvatesList(RegConn)
			End if
			display_type = "root_number"
			bShowRegIdentifiers = False
			bShowRegCmpds = true
			bShowBatchData = true
			bShowRegSalt = false
			'registered compound information recordset generation 
			'assumes cpdDBCounter and reg_ID variables are populated
			getRegData RegConn, reg_ID, cpdDBCounter, display_type 
			'BaseRS.MoveFirst
			'sql = "Select * from Structures where cpd_internal_ID = " & cpdDBCounter
			'Set MoleculesRS = RegConn.Execute(sql)
		End Select
		person_id = BaseRS("entry_person_Id")
		user_name = getPersonDisplayName(dbkey, formgroup, person_id ,RegConn)
		Entry_Person =user_name
		if instr(Entry_Person, "&comma;")> 0 then
			Entry_Person = replace(Entry_Person, "&comma;", ",")
		end if
		person_id = BaseRS("SCIENTIST_ID")
		People_Val=person_id
		user_name = getPersonDisplayName(dbkey, formgroup, person_id ,RegConn)
		People_Text = user_name
'start general security
'record edit security 
'1: see if you have permissions to edit anything regardless of user 
	on error goto 0

	bCanEditPart=canEditPart(dbkey,"TEMP") 'can edit compound data or batch data
	
'2: determine the owners of the data
		EditRestrictIDs = Session("EditRestrictIDs" & dbkey) 'edit restrictions impossed by scope permissions
		currentRecordOwner = RecordOwner_UserCode
		'0 means no edit restrictions, -1 mean can't edit ' else there is a list of user ids
		if bCanEditPart = True then
			
			if Not (CInt(EditRestrictIDs = "0") AND (EditRestrictIDs ="-1"))  then
		
				bEditCompd = getEditFlag(dbkey, currentRecordOwner, EditRestrictIDs, "Edit_Compound_Temp")
				bEditIdent = getEditFlag(dbkey, currentRecordOwner, EditRestrictIDs, "Edit_IDentifiers_Temp")
				bEditBatch = getEditFlag(dbkey, currentRecordOwner, EditRestrictIDs, "Edit_Batch_Temp")
				bEditSalt =  getEditFlag(dbkey, currentRecordOwner, EditRestrictIDs, "Edit_Salt_Temp")
				bDeleteTemp =  getEditFlag(dbkey, currentRecordOwner, EditRestrictIDs, "Delete_Temp")
			
			end if
			if EditRestrictIDs = "0" then
				bEditCompd = true
				bEditIdent = true
				bEditBatch = true
				bEditSalt = true
				bDeleteTemp = true
			End if
			if EditRestrictIDs = "-1" then
				bEditCompd = false
				bEditIdent = false
				bEditBatch = false
				bEditSalt = false
				bDeleteTemp = false
			end if
		else
			
			bEditCompd = false
			bEditIdent = false
			bEditBatch = false
			bEditSalt = false
			bDeleteTemp = false
		end if

if bEditCompd = false and  bEditIdent= false and  bEditBatch= false and bEditSal = False or bSiteOverride=true then

'if any of the user restrictions have changed the edittability of the record override the show edit button call so it doens't appear
	%>
	<script language="javascript">
	
	editButtonOverride =true
	regButtonOverride = true
	
	</script>
<%end if
if bDeleteTemp = False or bSiteOverride=true then
'if any of the user restrictions have changed the edittability of the record override the show edit button call so it doens't appear
	%>
	<script language="javascript">
		
	deleteButtonOverride =true
	
	
	</script>
<%end if
if bFBApproved=false then
%>
	<script language="javascript">
		
	regButtonOverride =true
	
	
	</script>
<%
end if
'end general

'start producer and user lookup
if Application("USER_LOOKUP") = 1  or Application("PRODUCER_LOOKUP") = 1 then
	LOOKUP_TABLE_DESTINATION = "Temporary_Structures" 'This is case sensitive. Must be Batches or Temporary_Structures
	if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
		lookup_row_id = Batch_ID
	else
		lookup_row_id = BaseID
	end if
	PRODUCER_ID_FIELD = UCase(Application("producer_id_field"))
	if UCase(formmode) = "EDIT_RECORD" then%>
		<script language="javascript">
			batch_row_id = "<%=Batch_ID%>"
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
	if bEditCompd= true or UCase(formmode) = "EDIT" then
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

	if bEditIdent= true or UCase(formmode) = "EDIT" then
		ident_output = "raw" 
	else
		ident_output = "raw_no_edit"
	end if

	if bEditSalt= true or UCase(formmode) = "EDIT" then
		Salt_Output = Salts_List
	else
		Salt_Output = ""
	end if
	if bEditBatch = True or UCase(formmode) = "EDIT" then
		batch_output = "raw" 
		notebooks_output =Notebooks_List
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
end if
'check site access

		'use only if you want to tie the prefix to the site and restrict access in this manner
'if Not (Session("SITE_ACCESS_ALL" & dbkey) = True) AND not (CLng(UserValidSequenceID) = CLng(CurrentRecordsequenceID)) then%>
	<script language = "javascript">
	//editButtonOverride =true
	//regButtonOverride = true
	</script>
<%'end if%>
<%if  compound_registered <> "" then%>
<table border="0" >
  <tr>
    <td nowrap valign="top" align="left" ><br>
<script language="JavaScript">document.write ('Record ' + '<%=BaseRunningIndex%><br>')
//getMarkBtn(<%=BaseID%>)

    </script> </td><td valign="top" align="left" >  
	<%  
		reg_message = RegMessageOutput(compound_registered)
	   	bOverridePostRegScreen = false
	   	bOverridePostRegScreen = determinePostRegScreen(compound_registered)
			
	response.write "<br>" &  reg_message
	if bOverridePostRegScreen = true then
		altFormPostString = buildAltFormGroupActionStr(dbkey, "base_form_group", "reg_numbers.reg_id=" & Session("Reg_ID"),"edit", "",Session("currentlocation" & dbkey & formgroup),"return to review/register","TRUE")
		'altFormPostString = buildNoRSAltFormGroupActionStr(dbkey, "base_form_group", Session("Reg_ID"),"edit", "",Session("currentlocation" & dbkey & formgroup),"return to review/register","TRUE")
		%>
		<a href = "javascript:doSubmitAltFormGroupSearch(&quot;<%=altFormPostString%>&quot;)">go to full record</a>
		</td></tr></table>
		<%'start %'end registered compound information post reg_report%>
	<table border="0" >
	  <tr>
	    <td nowrap valign="top" align="left" ><br>
	    <%	
		RegOutput= Session("Reg_Number")
		 %> 

<table  <%=registered_compound_table%> width = "650">

	<tr>
		<td>
			<table>
				<tr>
					<td colspan="5" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%>>Registered&nbsp;
					 Compound&nbsp;Information:&nbsp;&nbsp;</td></tr><tr>
					<td width="20%"><table><tr><td <%=td_bgcolor%>  nowrap><strong><font <%=font_default%>><%=RegOutput%></font></td>
					<td colspan="1" <%=td_bgcolor%> nowrap><strong><font <%=font_default_caption%>>&nbsp;&nbsp;&nbsp;&nbsp;Registered:</font>
					<strong><font <%=font_default%>><%=Session("RegDate")%></font></td>
                    <td colspan="1" <%=td_bgcolor%> nowrap><strong><font <%=font_default_caption%>>&nbsp;&nbsp;&nbsp;&nbsp;By:</font>
                    <strong><font <%=font_default%>><%=Session("RegPerson")%></font></td></tr></table>

                    </b></font>
				  </td>
				</tr>
				
				<tr>
					<table>
						<tr>
							<td align="left">
							    <table >
									<tr>
										<td valign="top" align="left"><table border = "1" bgcolor="#FFFFFF"><tr><td>
										<%Base64DecodeDirect dbkey, formgroup, Session("Base64_cdx"), "Structures.BASE64_CDX", Session("reg_id") , Session("cpdDBCounter"), "330" & ":BASE64CDX_NO_EDIT", "150"%> &nbsp;</tr></td></table>
										</td>
										
										<td valign="top" align="left">
											<table width = "320">
												
												<%if Application("Batch_Level") = "SALT" and CBool(Application("Salts_Used")) = True then %>
													<tr>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b>Salt Name</b></font></td>
													  <td <%=td_bgcolor%>><font <%=font_default%>><%=Session("Salt_Name")%>
													    </font></td>
													</tr>
												<%end if%>
												<%if CBOOL(Application("PROJECTS_USED")) = True then%><tr>
													<tr>
													  <%if CBOOL(Application("PROJECTS_NAMED_OWNER")) = true then%>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b>Owner</b></font></td>
													  <%else%>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("Project_ID")%></b></font></td>
													  <%end if%>
													  <td <%=td_bgcolor%>><font <%=font_default%>><%=Session("Project_Name")%>
													    </font></td>
													</tr>
												<%end if%>
                           
												<%if not checkHideField("COLLABORATOR_ID") then%>
														<tr>
															<td <%=td_caption_bgcolor%> nowrap align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("COLLABORATOR_ID")%></b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("Collaborator_ID") %></font>
															</td>
														</tr>
												<%end if%>
											
												<%if not checkHideField("PRODUCT_TYPE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("Product_Type")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("Product_Type") %>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("CAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("CasNums")%></font>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("RNO_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("RNO-No")%></font>
														</td>
													</tr>
												<%end if%>
												
												
												<%if not checkHideField("GROUP_CODE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("GroupCode")%></font>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
													    </td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("FEMA-No")%></font>
														</td>
													</tr>
												<%end if%>
												
												

												<%if not checkHideField_Ignore_Derived("MW") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font>
													    </td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("MW")%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField_Ignore_Derived("FORMULA") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("FORMULA")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=GetHTMLStringForFormula(Session("FORMULA"))%>
														</td>
													</tr>
												<%end if%>
											
											</table>
										</div>
									</td>
								</tr>
							</table>
						</td>
					</tr>
<%'Start Compound_Molecule Custom Fields%>
					<tr><td><table width="650">
										<tr>
											<td>  
												<table width = "650">
													
													                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_2")%>>    <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_2")%>
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
															<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_3")%>>    <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_4")%>>   <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_4")%>
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
															<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%=Session("INT_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_2")%>> <font <%=font_default%>><%=Session("INT_CMPD_FIELD_2")%>
														</font>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_3")%>>   <font <%=font_default%>><%=Session("INT_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_4")%>> <font <%=font_default%>><%=Session("INT_CMPD_FIELD_4")%>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_1")%>> <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_2")%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_2")%>
														</font>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_3")%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_3")%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_4")%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_4")%>
														</font>
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
													<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_1")%>>  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_2")%>>  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_2")%>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_3")%>>   <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_4")%>>   <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_4")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                               
                                            </table>
                                        </td>
                                    </tr>
								</table>
								</td></tr>
					<%'	end Compound_Molecule Custom Fields%>

					<tr>
						<td <%=td_default%> valign="top" align="left">
                        
							<table width="650">
								<%if not checkHideField("MW_TEXT") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MW_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("MW_TEXT")%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if not checkHideField("MF_TEXT") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("MF_TEXT")%></font>
										</td>
									</tr>
								<%end if %>
                   
								
								
								<%if CBool(application("compound_types_used")) = True then%>
                   
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Compound_Type")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td<%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("Compound_Type")%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("structure_comments_text")) = True then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("structure_comments_txt")%></b></font>
										</td>
									</tr>

									<tr>
									    <td  <%=td_bgcolor%> ><font <%=font_default%>>&nbsp;<%=Session("Struc_Comments_Text")%></font>
									    </td>
									 </tr>
								<%end if%>
								
								
						
								<%if not checkHideField("CHEMICAL_NAME") then%>

									<tr>
										<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
										</td>
									</tr>
              
									<tr>
										<td  <%=td_bgcolor%>>
											
											<font <%=font_default%>>&nbsp;<%=Session("ChemNames")%></font>
										</td>
									</tr>
								<%end if%>
								
								<%if CBool(Application("AUTOGENERATED_CHEMICAL_NAME")) = True then%>
									<%if not checkHideField_Ignore_Derived("CHEM_NAME_AUTOGEN") then%>
										<%if  checkHideField("CHEMICAL_NAME") then
											add_gen_text = ""
										else
											add_gen_text= ""
										end if
									end if
								end if%>

								<%if not checkHideField_Ignore_Derived("CHEM_NAME_AUTOGEN") then %>
									<tr>
										<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%>&nbsp;&nbsp;&nbsp;<%=add_gen_text%></b></font>
										</td>
									</tr>
								  
									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("ChemNamesAutoGen")%></font>
										</td>
									</tr>
								<%end if%>

								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong><br>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("Synonyms")%></font>
										</td>
									</tr>
								<%end if%>
								
							
							</table>
                        </div>
					</td>
				</tr>
			
                
            </table>
        </div>
	</td>
</tr>
</table>
	<%End if 'bOverridePostRegScreen = true%>
<%'end registered compound information post reg_report%>
	
	
	
	
	
	</td>
  </tr>
</table>
<%else%>
<%Select Case UCase(commit_type)
Case "FULL_COMMIT"%>
<%if duplicate_found <> "" then
		reg_message = "Duplicates where found for this compound while processing.  Please register manually by clicking the register button. You will then be given the opportunity to choose the action to take for this duplicate."
		reg_message = RegMessageOutput(duplicate_found)
		response.write reg_message 
	end if
	%>


     <%'chemreg header information%>
		<table border="0">
			<tr>
				<td width="40%" align="left" nowrap>
					<p align="left"><strong><font face="Arial" size="3" color="#182889">Compound Commit Form</font></strong></p>
				</td>
			    
				<td width="60%" align="right" nowrap>
					<p align="right"><font face="Arial" size="4" color="#182889"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
					<%=Application("display_appkey")& "&nbsp;"%>
					Chemical</b></font> <font face="Arial" size="4" color="#182889"><b>Registration
					System</b></font></p>
				</td>
			</tr>
		</table>
	<%'end chemreg header information%>
 <table >
	<tr>
		<td > 
			<table >
                <tr>
					<td >          
                    <table border="0">
						 <tr>
							<td  nowrap valign="top" align="left">
								<table border="0" cellspacing="0" cellpadding="0">
									<tr>
										<td valign="top" >
											<script language="JavaScript">document.write ('Record ' + '<%=BaseRunningIndex%><br>')
											getMarkBtn(<%=BaseID%>)

											</script></td>
										<td></td>
									</tr>
								</table>
							</td>
							<td valign="top" align="left" width="37">  
								<script language="JavaScript"><!-- hide from older browsers
									var  commit_type =  "<%=commit_type%>"
									var formmode = "<%=formmode%>"
									var uniqueid = "<%=baseid%>"

									windowloaded = false
								// End script hiding --></script>
							</td>
							<td valign="top" align="right" face = "Arial"><b>Registration Type:</b>
								<%
									Select Case UCase(commit_type)
									Case "FULL_COMMIT"
										response.write "New Compound"
										Response.Write dup_list
									Case "BATCH_COMMIT"
										if bFBApproved = true then
											response.write "Add Batch to Registered Compound"
										else
											response.write "Review Batch for Unapproved Registered Compound"
										end if
									Case "ADD_IDENTIFIERS"
										response.write "Add Identifiers to Registered Compound"
									Case "ADD_SALT"
										response.write "Add Salt to Registered Compound"
									End Select
								%>
							</td>
                      </tr>
                    </table>
                    </td>
				</tr>
			</table>
		<%'start compound information%>
                
<table <%=table_main_L1%> width = "664">
	<%'SYAN added on 11/29/2004 to fix CSBR-49587%>
		<tr><td><table>
					<tr>
						<%
							bApproved = getTempApprovedFlag(RegConn, BaseRS("TEMP_COMPOUND_ID"))
							
							if bApproved = true then
								status = 1
							else
								status = 0
							end if
							
							checkBox = getTempApprovedCheckBox(BaseRS("TEMP_COMPOUND_ID"), "TEMPORARY_STRUCTURES.APPROVED", status)
						%>
						<%if not checkHideField("Approved") and Application("PRE_REGISTER_APPROVED_FLAG") = 1 then%>
							
							<td <%=td_caption_bgcolor%> valign="top" align="left"><font <%=font_default_caption%> size="2"><b><%=getLabelName("Approved")%></b></font> 
								<%if UCase(formmode) = "EDIT_RECORD" and LCase(Session("SET_APPROVED_FLAG" & dbkey)) = "true" then
									Response.Write checkBox
								else%>
									<%if bApproved = True Then%>
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

						<%if not checkHideField("Approved_by") and Application("PRE_REGISTER_APPROVED_FLAG") = 1 and UCase(formmode) <> "EDIT_RECORD" then%>
							<td <%=td_caption_bgcolor%> align="right">
								&nbsp;&nbsp;&nbsp;
								<font <%=font_default_caption%>><b><%=getLabelName("Approved_by")%></b>
								<%=getValueFromTablewConn(RegConn, "People", "PERSON_ID", BaseRS("APPROVED_BY"), "USER_ID")
		%>
							</td>
						<%end if%>

						<%if not checkHideField("Approval_Date") and Application("PRE_REGISTER_APPROVED_FLAG") = 1 and UCase(formmode) <> "EDIT_RECORD" then%>
							<td <%=td_caption_bgcolor%> align="right">
								&nbsp;&nbsp;&nbsp;
								<font <%=font_default_caption%>><b><%=getLabelName("Approval_Date")%></b>
								<%showresult dbkey, formgroup, basers, "Temporary_Structures.APPROVED_DATE", "raw_no_edit","8","20"%>
								<%showresult dbkey, formgroup, basers, "Temporary_Structures.APPROVED_TIME", "raw_no_edit", setDisplayType("APPROVED_TIME"), "20"%>
							</td>
						<%end if%>
					</tr>
				</table>
			</td>			
		</tr>
	<%'End of SYAN modification%>
	
		<tr>
		<td >
			<table>
				<tr>
				  <td colspan="2" width="100%">
				  </td>
				</tr>
				
				<tr>
					<table >
						<tr>
							<td align="left" >
							    <table border="0" >
									<tr>
										<td valign="top" align="left"><table border="1"><tr><td><%ShowResult dbkey, formgroup,BaseRS, "Temporary_Structures.BASE64_CDX", struc_output, 325,220%> &nbsp;</td></tr></table></td>
										
										<td valign="top" align="left">
											<table width = "330">
												<tr>
													<td colspan="2" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%>>Compound Information</font></strong></td>
												</tr>

												<%if not checkHideField("ENTRY_DATE") then%>
												<tr>
													<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("Entry_Date")%></b></td>
													<td <%=td_bgcolor%> width="160"><font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.Entry_Date", "raw_no_edit","8","20"%></font></td>
												</tr>
												<%end if%>
												<%if not checkHideField("LAST_MOD_DATE") then%>
												<tr>
													<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("Last_Mod_Date")%></b></td>
													<td <%=td_bgcolor%> width="160"><font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.Last_Mod_Date", "raw_no_edit","8","20"%></font></td>
												</tr>
												<%end if%>

												<%if not checkHideField("ENTRY_PERSON_ID") then%>
													<tr>
													<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("ENTRY_PERSON_ID")%></b></font></td>
													<td <%=td_bgcolor%> width="160"><font <%=font_default%>><%=Entry_Person%></font></td>
												</tr>
												<%end if%>
												
												<%if CBOOL(Application("PROJECTS_USED")) = True then%><tr>
													<%if  Not CBOOL(Application("PROJECTS_NAMED_OWNER")) = true then%>
														<td <%=td_caption_bgcolor%> width="160" align = "right"><font <%=font_default_caption%>><b><%=getLabelName("Project_ID")%></b></font></td>
														<td <%=td_bgcolor%> >  <font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Project_ID", Projects_List, Projects_Val,Projects_Text,0,true,"value","0" %></font></td>
													<%else%>
														<td <%=td_caption_bgcolor%> width="160" align = "right"><font <%=font_default_caption%>><b>Owner</b></font></td>
														<td <%=td_bgcolor%> >  <font <%=font_default%>><%=Projects_Text%></font></td>
													<%end if%>
													</tr>
												<%end if%>
                            
												<%if not checkHideField("PREFIX") then%>
													<tr>
                        								<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("Sequence_ID")%></b></font></td>						                            
                           
														<%if default_prefix_id <> "" then%>
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Sequence_ID", Sequences_List, Sequences_Val,Sequences_Text,0,false,"value",default_prefix_id %></font></td>
														<%else%>
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Sequence_ID", Sequences_List, Sequences_Val,Sequences_Text,0,true,"value","0" %></font></td>
														<%end if%>
													</tr>
												 <%end if%>

                           
												<%if not checkHideField("COLLABORATOR_ID") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("COLLABORATOR_ID")%></b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, BASERS,  "TEMPORARY_STRUCTURES.COLLABORATOR_ID",ident_output, setDisplayType("COLLABORATOR_ID"),"15"%></font>
															</td>
														</tr>
												<%end if%>
											
												<%if not checkHideField("PRODUCT_TYPE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("PRODUCT_TYPE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup,BASERS,  "TEMPORARY_STRUCTURES.PRODUCT_TYPE", cmpd_output,setDisplayType("PRODUCT_TYPE"),"15"%>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("CAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("CAS_Number")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,  "Temporary_Structures.CAS_Number", ident_output, setDisplayType("CAS_Number"),"15"%></font>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("RNO_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%>align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.RNO_NUMBER", ident_output,setDisplayType("RNO_NUMBER"),"15"%></font>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("GROUP_CODE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.GROUP_CODE",ident_output, setDisplayType("GROUP_CODE"),"15"%></font>
														</td>
													</tr>
												<%end if%>
												
												
												<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
													    </td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "Temporary_Structures.FEMA_GRAS_NUMBER",ident_output,  setDisplayType("FEMA_GRAS_NUMBER"),"15"%></font>
														</td>
													</tr>
												<%end if%>
												
												

												<%if not checkHideField("MW") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font>
													    </td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,  "Temporary_Structures.MW2", isDerived("MW2",cmpd_output), setDisplayType("MW"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("FORMULA") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FORMULA")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,  "Temporary_Structures.FORMULA2", isDerived("FORMULA2",cmpd_output), setDisplayType("FORMULA"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("CHIRAL") then%>
												    <tr>
														<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CHIRAL")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "TEMPORARY_STRUCTURES.CHIRAL",isDerived("CHIRAL",cmpd_output) ,  setDisplayType("CHIRAL"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("CLogP") then%>
													<tr>
														<td <%=td_caption_bgcolor%>align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CLogP")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers, "TEMPORARY_STRUCTURES.CLogP", isDerived("CLogP",cmpd_output) , setDisplayType("CLogP"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("H_BOND_DONORS") then%>
													<tr>
														<td  <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_DONORS")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "TEMPORARY_STRUCTURES.H_BOND_DONORS",isDerived("H_BOND_DONORS",cmpd_output),setDisplayType("H_BOND_DONORS"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("H_BOND_ACCEPTORS") then%>
													<tr>
														<td <%=td_caption_bgcolor%>  align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_ACCEPTORS")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "TEMPORARY_STRUCTURES.H_BOND_ACCEPTORS",isDerived("H_BOND_ACCEPTORS",cmpd_output) ,setDisplayType("H_BOND_ACCEPTORS"),"15"%></font>
														</td>
													</tr>
												<%end if%>
											</table>
										</div>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				<%'Start Compound_Molecule Custom Fields%>
					<tr><td><table width="650">
										<tr>
											<td>  
												<table width = "650">
													
													                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_1") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_1",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_1"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_2")%>>    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_3")%>>    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_3"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4",  cmpd_output, setDisplayType("TXT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												           
                                                 <tr>
													                                             
                                             		<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1",  cmpd_output, setDisplayType("INT_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_2")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2",  cmpd_output, setDisplayType("INT_CMPD_FIELD_2"),"15"%>														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3",  cmpd_output, setDisplayType("INT_CMPD_FIELD_3"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_4")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4",  cmpd_output, setDisplayType("INT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                   
												
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_1")%>> <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_1"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_2"),"15"%>															</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4",  cmpd_output, setDisplayType("REAL_CMPD_FIELD_4"),"15"%>															</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                                
                                                <tr>
                                                <%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1") then%>                           
													<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_1"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_2"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4",  cmpd_output, setDisplayType("DATE_CMPD_FIELD_4"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                               
                                            </table>
                                        </td>
                                    </tr>
								</table>
								</td></tr>
					<%'	end Compound_Molecule Custom Fields%>					<tr>
						<td <%=td_default%> valign="top" align="left" >
                        
							<table width="664">
								<%if not checkHideField("MW_TEXT") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MW_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.MW_Text",cmpd_output, setDisplayType("MW_TEXT"),"100"%></font>
										</td>
									</tr>
								<%end if %>
                   
								<%if not checkHideField("MF_TEXT") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.MF_Text",cmpd_output, setDisplayType("MF_TEXT"),"100"%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("compound_types_used")) = True then%>
                   
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Compound_Type")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td<%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Temporary_Structures.Compound_Type",Compound_Type_output, Compound_type_val,Compound_type_text,1,false,"value:validate:checkCompoundType(this.name)","0" %></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("structure_comments_text")) = True then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("STRUCTURE_COMMENTS_TXT")%></b></font>
										</td>
									</tr>

									<tr>
									    <td  <%=td_bgcolor%> ><font <%=font_default%>><%showresult dbkey, formgroup, basers,  "TEMPORARY_STRUCTURES.STRUCTURE_COMMENTS_TXT",cmpd_output, setDisplayType("STRUCTURE_COMMENTS_TXT"),"100"%></font>
									    </td>
									 </tr>
								<%end if%>
								
								
						
								<%if not checkHideField("CHEMICAL_NAME") then%>

									<tr>
										<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
										</td>
									</tr>
              
									<tr>
										<td  <%=td_bgcolor%>>
											
											<font <%=font_default%>><%showresult dbkey, formgroup, basers,  "Temporary_Structures.Chemical_Name", ident_output,setDisplayType("CHEMICAL_NAME"), "75"%></font>
										</td>
									</tr>
								<%end if%>
								
								<%if CBool(Application("AUTOGENERATED_CHEMICAL_NAME")) = True then%>
									<%if not checkHideField("CHEM_NAME_AUTOGEN") then%>
										<%if  checkHideField("CHEMICAL_NAME") then
											add_gen_text = ""
										else
											add_gen_text= ""
										end if
									end if
								end if%>

								<%if not checkHideField("CHEM_NAME_AUTOGEN") then %>
									<tr>
										<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%>&nbsp; <%=add_gen_text%></b></font>
										</td>
									</tr>
								  
									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>
											&nbsp;
											<%ShowResult dbkey, formgroup, BaseRS,  "TEMPORARY_STRUCTURES.CHEM_NAME_AUTOGEN",isDerived("CHEM_NAME_AUTOGEN",ident_output), setDisplayType("CHEM_NAME_AUTOGEN"),"75"%></font>
										</td>
									</tr>
								<%end if%>

								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong><br>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Synonym_R", ident_output,setDisplayType("Synonym_R"), "75"%></font>
										</td>
									</tr>
								<%end if%>

							</table>
                        </div>
					</td>
				</tr>
			
                
            </table>
        </div>
	</td>
</tr>
</table>
	
<%'end compound information%>
        <%        Case ELSE
%>

 <%'chemreg header information
		Select Case UCase(commit_type) 
				Case "ADD_SALT" 
					theHeaderText= "New Salt Registry Form"
				Case "BATCH_COMMIT"
					theHeaderText="New Batch Registry Form"
				Case "ADD_IDENTIFIERS"
					theHeaderText="New Identifier Registry Form"
		End Select

		Select Case UCase(commit_type)
			Case "FULL_COMMIT"
				task_text =  "New Compound"
			Case "BATCH_COMMIT"
				task_text =  "Add Batch to Registered Compound"
			Case "ADD_IDENTIFIERS"
				task_text =   "Add Identifiers to Registered Compound"
			Case "ADD_SALT"
				task_text =   "Add Salt to Registered Compound"
			End Select
%>
		<table width = "650">
			<tr>
				<td width="40%" align="left" nowrap>
					<p align="left"><strong><font face="Arial" size="3" color="#182889"><%=theHeaderText%></font></strong></p>
				</td>
			    
				<td width="60%" align="right" nowrap>
					<p align="right"><font face="Arial" size="4" color="#182889"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
					<%=Application("display_appkey")& "&nbsp;"%>
					Chemical</b></font> <font face="Arial" size="4" color="#182889"><b>Registration
					System</b></font></p>
				</td>
			</tr>
		</table>

		<table width = "650">
			<tr>
				<td valign="top" width="80">
					<script language="JavaScript">document.write ('Record ' + '<%=BaseRunningIndex%><br>')
					getMarkBtn(<%=BaseID%>)

					</script>
				</td>
				<td valign="top" align="left" ><b><font <%=font_default_caption%>>Registration Type:&nbsp;&nbsp;</font></b><font <%=font_default%>><%=task_text%></font>
				</td>
			</tr>
		</table>
		<table>
			<tr>
				<td nowrap valign="top" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Entry_Date")%></b></font></td>
				<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Entry_Date", "raw_no_edit","DATE_PICKER:8","20"%></font></td>
						
				<td nowrap valign="top" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Entry_Person_ID")%>&nbsp;</font></b></td>
				<td <%=td_bgcolor%>> <font <%=font_default%>><%=entry_person%></font></td>
			</tr>
		</table>
				
				
		<script language="JavaScript"><!-- hide from older browsers
			var  commit_type =  "<%=commit_type%>"
			var formmode = "<%=formmode%>"
			var uniqueid = "<%=baseid%>"

			windowloaded = false
		// End script hiding --></script>
	
          
        
<%'end chemreg header information%>
				
<%'start registered compound information%>

<table  <%=registered_compound_table%> width = "650">
			 <%if UCase("COMMIT_TYPE") = "SALT" then 
					RegOutput= Session("Root_Number")
					regDate = Session("RootRegDate")
               else 
                  RegOutput= Session("Reg_Number")
                  regDate = Session("RegDate")
               end if%> 
	<tr>
		<td >
			<table>
				<tr>
					<td colspan="5" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%>>Registered&nbsp;
					 Compound&nbsp;Information:&nbsp;&nbsp;</td></tr><tr>
					<td width="20%"><table><tr><td <%=td_bgcolor%>  nowrap><strong><font <%=font_default%>><%=RegOutput%></font></td>
					<td colspan="1" <%=td_bgcolor%> nowrap><strong><font <%=font_default_caption%>>&nbsp;&nbsp;&nbsp;&nbsp;Registered:</font>
					<strong><font <%=font_default%>><%=regDate%></font></td>
                    <td colspan="1" <%=td_bgcolor%> nowrap><strong><font <%=font_default_caption%>>&nbsp;&nbsp;&nbsp;&nbsp;By:</font>
                    <strong><font <%=font_default%>><%=Session("RegPerson")%></font></td></tr></table>

                    </b></font>
				  </td>
				</tr>
				
				<tr>
					<table>
						<tr>
							<td align="left">
							    <table >
									<tr>
										<td valign="top" align="left"><table border = "1"><tr><td><%Base64DecodeDirect dbkey, formgroup, Session("Base64_cdx"), "Structures.BASE64_CDX", Session("reg_id") , Session("cpdDBCounter"), "330" & ":BASE64CDX_NO_EDIT", "150"%> &nbsp;</tr></td></table>
										</td>
										
										<td valign="top" align="left">
											<table width = "320">
												
												<!--<%if Application("Batch_Level") = "SALT" and CBool(Application("Salts_Used")) = True and Not UCase(Commit_Type) = "ADD_SALT" then %>
													<tr>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b>Salt Name</b></font></td>
													  <td <%=td_bgcolor%>><font <%=font_default%>><%=Session("Salt_Name")%>
													    </font></td>
													</tr>
												<%end if%>-->
												<%if CBOOL(Application("PROJECTS_USED")) = True then%><tr>
													<tr>
													  <%if CBOOL(Application("PROJECTS_NAMED_OWNER")) = true then%>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b>Owner</b></font></td>
													  <%else%>
													  <td <%=td_caption_bgcolor%> align = "right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("Project_Internal_ID")%></b></font></td>
													  <%end if%>
													  <td <%=td_bgcolor%>><font <%=font_default%>><%=Session("Project_Name")%>
													    </font></td>
													</tr>
												<%end if%>
                           
												<%if not checkHideField("COLLABORATOR_ID") then%>
														<tr>
															<td <%=td_caption_bgcolor%> nowrap align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("COLLABORATOR_ID")%></b></font>
															</td>
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("Collaborator_ID") %></font>
															</td>
														</tr>
												<%end if%>
											
												<%if not checkHideField("PRODUCT_TYPE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("Product_Type")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("Product_Type") %>
														</td>
													</tr>
												<%end if%>
												
												
												<%if not checkHideField("CAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("CasNums")%></font>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("RNO_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%>align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("RNO-No")%></font>
														</td>
													</tr>
												<%end if%>
												<%if not checkHideField("GROUP_CODE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("GroupCode")%></font>
														</td>
													</tr>
												<%end if%>
												
												
												
												<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
													    </td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("FEMA-No")%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField_Ignore_Derived("MW") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font>
													    </td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=Session("MW")%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField_Ignore_Derived("FORMULA") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="160"><font <%=font_default_caption%>><b><%=getLabelName("FORMULA")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%=GetHTMLStringForFormula(Session("FORMULA"))%>
														</td>
													</tr>
												<%end if%>
											
											</table>
										</div>
									</td>
								</tr>
							</table>
						</td>
					</tr>
<%'Start Compound_Molecule Custom Fields%>
					<tr><td><table width="650">
										<tr>
											<td>  
												<table width = "650">
													
													                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_2")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("TXT_CMPD_FIELD_4")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												           
                                                 <tr>
													                                             
                                             		<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_1") then%>
														<td align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_1") then%>                           
														<td  <%=td_bgcolor_c1%> >  <font <%=font_default%>><%=Session("INT_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>
														<td align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("INT_CMPD_FIELD_2")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%=Session("INT_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("INT_CMPD_FIELD_4")%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                   
												
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_1") then%>
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_2")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_3") then%>                           
														<td <%=td_bgcolor_c1%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_3")%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("REAL_CMPD_FIELD_4")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                                
                                                <tr>
                                                <%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_1") then%>
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_1")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_2")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_3")%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>
														<td align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%=Session("DATE_CMPD_FIELD_4")%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                               
                                            </table>
                                        </td>
                                    </tr>
								</table>
								</td></tr>
					<%'	end Compound_Molecule Custom Fields%>

					<tr>
						<td <%=td_default%> valign="top" align="left">
                        
							<table width="650">
								<%if not checkHideField("MW_TEXT") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MW_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("MW_TEXT")%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if not checkHideField("MF_TEXT") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_TEXT")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("MF_TEXT")%></font>
										</td>
									</tr>
								<%end if %>
                   
								
								
								<%if CBool(application("compound_types_used")) = True then%>
                   
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Compound_Type")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td<%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("Compound_Type")%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("structure_comments_text")) = True then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("structure_comments_txt")%></b></font>
										</td>
									</tr>

									<tr>
									    <td  <%=td_bgcolor%> ><font <%=font_default%>>&nbsp;<%=Session("Struc_Comments_Text")%></font>
									    </td>
									 </tr>
								<%end if%>
								
								<% if not UCase(Commit_Type) = "ADD_SALT" then%>
						
								<%if not checkHideField("CHEMICAL_NAME") then%>

									<tr>
										<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
										</td>
									</tr>
              
									<tr>
										<td  <%=td_bgcolor%>>
											
											<font <%=font_default%>>&nbsp;<%=Session("ChemNames")%></font>
										</td>
									</tr>
								<%end if%>
								
								<%if CBool(Application("AUTOGENERATED_CHEMICAL_NAME")) = True then%>
									<%if not checkHideField_Ignore_Derived("CHEM_NAME_AUTOGEN") then%>
										<%if  checkHideField("CHEMICAL_NAME") then
											add_gen_text = ""
										else
											add_gen_text= ""
										end if
									end if
								end if%>

								<%if not checkHideField_Ignore_Derived("CHEM_NAME_AUTOGEN") then %>
									<tr>
										<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%>&nbsp;<%=add_gen_text%></b></font>
										</td>
									</tr>
								  
									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("ChemNamesAutoGen")%></font>
										</td>
									</tr>
								<%end if%>

								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong><br>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>&nbsp;<%=Session("Synonyms")%></font>
										</td>
									</tr>
								<%end if%>
								<%end if%>
							
							</table>
                        </div>
					</td>
				</tr>
			
                
            </table>
        </div>
	</td>
</tr>
</table>
	
<%'end registered compound information%>
 

 <%End Select%>

           
<%Select Case UCase(commit_type)
Case "FULL_COMMIT", "BATCH_COMMIT", "ADD_SALT", "ADD_IDENTIFIERS"
%>
<script language="JavaScript">
  var  commit_type =  "<%=commit_type%>"
  var formmode = "<%=formmode%>"
  var uniqueid = "<%=baseid%>"
	windowloaded = false
            </script>
<!--start new identifiers-->
<% if UCase(commit_type) = "ADD_SALT" or UCase(commit_type) = "FULL_COMMIT" then %>  
<br>
<%'start salt information%>
<%if UCase(Application("Batch_Level")) = "SALT" then%>
		<input type = "hidden" name = "salts_batch_list" value ="<%=salts_batch_list%>">

	<table <%=table_main_L1%> width = "698" >
		<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>New Salt Information</font></strong>
			</td>
		</tr>
		
		<tr>
			<td  <%=td_bgcolor%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Temporary_Structures.Salt_Code", Salts_List, Salts_Val,Salts_Text,0,true,"value" & ":VALIDATE:" & "updateSaltName(this.form,this," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & "," & baseid & ")","0" %>


			</td>
		</tr>
		</table>
  <%end if%>
<%'end salt information%>
<%end if%>
<% if UCase(commit_type) = "ADD_SALT" OR UCase(commit_type) = "ADD_IDENTIFIERS" then%>
			<table  <%=table_main_L1%> width = "681" >
						
						<tr>
							<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>New Identifier Information</font></strong>
			</td>
                          </tr>
                          <tr>
                          <td>
                          <br>
                          <table width = "680">
							<%if not checkHideField("CHEMICAL_NAME") then%>

									<tr>
										<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
										</td>
									</tr>
              
									<tr>
										<td  <%=td_bgcolor%>>
												
											<font <%=font_default%>><%showresult dbkey, formgroup, basers,  "Temporary_Structures.Chemical_Name", ident_output,setDisplayType("Chemical_Name"), "75"%></font>
										</td>
									</tr>
								<%end if%>
								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Synonym_R", ident_output,setDisplayType("Synonym_R"), "75"%></font>
										</td>
									</tr>
								<%end if%>
                          
								<%if not checkHideField("CAS_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%></b></font>
										</td>
										</tr>
									<tr>					
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "Temporary_Structures.CAS_Number", ident_output,  setDisplayType("CAS_NUMBER"),"100"%></font>
										</td>
									</tr>
								<%end if%>
								<%if not checkHideField("RNO_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
										</td>
									</tr>
									<tr>						
										<td <%=td_bgcolor%> >  <font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.RNO_NUMBER",ident_output,  setDisplayType("RNO_NUMBER"),"100"%></font>\
										</td>
									</tr>
								<%end if%>
																		
								<%if not checkHideField("GROUP_CODE") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
										</td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%showresult dbkey, formgroup, basers, "Temporary_Structures.GROUP_CODE",ident_output, setDisplayType("GROUP_CODE"),"100"%></font>
										</td>
									</tr>
								<%end if%>
								<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
									    </td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "Temporary_Structures.FEMA_GRAS_NUMBER",ident_output, setDisplayType("FEMA_GRAS_NUMBER"),"100"%></font>
										</td>
									</tr>
								<%end if%>					
							
								
							
								<%if not checkHideField("Collaborator_ID") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("Collaborator_ID")%></b></font>
										</td>
									</tr>
									<tr>						
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%showresult dbkey, formgroup, basers,  "TEMPORARY_STRUCTURES.COLLABORATOR_ID", ident_output,  setDisplayType("Collaborator_ID"),"100"%></font>
										</td>
									</tr>
								<%end if%>
							</table>
							</td>
						</tr>
                         
				</table>
				</td>
			</tr>
		</table>
		</td>
	</tr>
</table><!--end new identifiers-->
<%end if%>

<%if not UCase(commit_type) = "ADD_IDENTIFIERS" then%>

<%'start batch info%> 
	<table <%=table_main_L1%> width = "664" >
		<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>New Batch Information</font></strong>
			</td>
		</tr>
	
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
												<td <%=td_caption_bgcolor%> width = "150">  <%if not checkHideField("Formula_Weight") then%><font <%=font_default_caption%>><b><%=getLabelName("Formula_Weight")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "150"> <%if not checkHideField("Percent_Active") then%> <font <%=font_default_caption%>><b><%=getLabelName("Percent_Active")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "150">  <font <%=font_default_caption%>></font>
												</td>
											</tr>
											
											<tr>
												<%if not checkHideField("Formula_Weight") then%>
													<td width = "150" <%=td_bgcolor%>><font <%=font_default_caption%>>                                        
													<%ShowResult dbkey, formgroup, BaseRS, "TEMPORARY_STRUCTURES.FORMULA_WEIGHT", isDerived("FORMULA_WEIGHT",batch_output),setDisplayType("FORMULA_WEIGHT"),"15"%></font></td>
                                                <%end if%>	                               
                                                
                                                <%if not checkHideField("Percent_Active") then%>
													<td width = "150" <%=td_bgcolor%>><font <%=font_default_caption%>>
													 <%ShowResult dbkey, formgroup, BaseRS, "TEMPORARY_STRUCTURES.PERCENT_ACTIVE", isDerived("PERCENT_ACTIVE",batch_output), setDisplayType("PERCENT_ACTIVE"),"15"%></font></td>
												<%end if%>	
												
												<td width = "150" <%=td_bgcolor%>><font <%=font_default_caption%>>
												</font></td>
                                            </tr>
										</table>
									</td>
								</tr>

								<tr>
									<td>
										<%if bShowBatchSalt = true then%>
										<table width="664">
											<tr>
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_Name")%>&nbsp;</font></strong>
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
																		<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Salts_Name", Salts_Batch_List, "0","", "TEMPORARY_STRUCTURES.SALT_NAME","TEMPORARY_STRUCTURES.SALT_MW"%>
																		</td>
																		<td <%=td_bgcolor%>>
																		<%ShowResult dbkey, formgroup,BaseRS, "TEMPORARY_STRUCTURES.SALT_NAME",   batch_output,  setDisplayType("SALT_NAME"),"25"%>
																	
																		</td>
																	<%else%> 
																		<td <%=td_bgcolor%>>
																		<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAME",    "hidden","25"%>
																		<%ShowInputField dbkey, formgroup, "UID." & baseid & ":TEMPORARY_STRUCTURES.SALT_NAME",    "hidden","25"%>
																		<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAMEdisplay",    "SCRIPT:alertNoEdit(this.form," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & ")!" & Basers("SALT_NAME"),"25"%>
																		</td>
																	<%end if%> 
															
																<%else%>
																	<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Salts_Name", Salts_Batch_List, "0","", "TEMPORARY_STRUCTURES.SALT_NAME","TEMPORARY_STRUCTURES.SALT_MW"%>
																	</td>
																	
																	<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup,BaseRS, "TEMPORARY_STRUCTURES.SALT_NAME",   batch_output,  setDisplayType("SALT_NAME"),"25"%>
																	</td>
																<%end if%> 
															<%else%> 
															
															
															<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SALT_NAME",  batch_output,  setDisplayType("SALT_NAME"),"25"%>
															</td>
															<%end if%> 
														</tr>
													</table>
												</td>
												<%if UCase(Application("Batch_Level")) = "SALT" then 
													if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>
															<%if CBool(Application("SALT_EDITABLE_FOR_REG_SALTS")) = true THEN%>
																	
																<td <%=td_bgcolor%>>
																<%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
																</td>
																
															<%else%>
																
																<td <%=td_bgcolor%>>
																<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MW", "hidden","25"%>
																<%ShowInputField dbkey, formgroup, "UID." & baseid & ":TEMPORARY_STRUCTURES.SALT_MW", "hidden","25"%>
																
																<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MWdisplay",  "SCRIPT:alertNoEdit(this.form," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & ")!" & Basers("SALT_MW"),"12"%>
																</td>
															<%end if%> 
													   
													<%else%>                               
														<td <%=td_bgcolor%> ><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
														</td>	
													<%end if%>
												<%else%>                               
												<td <%=td_bgcolor%> ><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SALT_MW", batch_output,setDisplayType("SALT_MW"),"12"%>
												</td>
												<%end if%> 
												<td <%=td_bgcolor%>> <%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Salt_Equivalents",  batch_output,setDisplayType("Salt_Equivalents"), "12"%>
												</td>
											</tr>
											<% end if
											if bShowBatchSolvate = true then%>
											<tr>
											     <td width="150" <%=td_caption_bgcolor%>> <strong><font <%=font_default_caption%>><%=getLabelName("Solvate_Name")%></font></strong>
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
																<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Solvates_Name", Solvates_list, "0","", "TEMPORARY_STRUCTURES.SOLVATE_NAME","TEMPORARY_STRUCTURES.SOLVATE_MW"%>
																</td>                                  
															<%end if%>
															
															<td <%=td_bgcolor%>> <%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SOLVATE_NAME",   batch_output,setDisplayType("SOLVATE_NAME"),"25"%>
															</td>
														</tr>
													</table>
												</td>
												
												<td <%=td_bgcolor%>>  <%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SOLVATE_MW",  batch_output,setDisplayType("SOLVATE_MW"),"12"%>
												</td>
												
												<td <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Solvate_Equivalents",batch_output,setDisplayType("Solvate_Equivalents"), "12"%>
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
                    <%'start user/substance batch data%>
                           		  
					<tr>
						<td> 
							<table width="664" >
								<tr>
									<td>
										<table width="664" >
											<tr><%'start support for selecting a producer by choosing a user_code from a drop down list.
												If Application("USER_LOOKUP") = 1 or Application("USER_LOOKUP") = "" then%>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_Code")%></font></b> </td>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></font></b> </td>
												<%else%>
													<%if CBOOL(Application("NOTEBOOK_USED")) = true and  not checkHideField("SCIENTIST_ID")then%>
														<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%> </strong>
													</td>
														<%end if%>
													<%end if%>
												<%end if%>
												<%'end user lookup labels%>
												<%if not checkHideField("PURITY") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Purity")%>  </strong></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Appearance")%>  </strong></font>
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
												'stop
												if UCase(formmode)="EDIT_RECORD" AND bEditBatch = True then
													if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
														theSciValue =getValueFromTablewConn(RegConn, "People", "Person_ID", BatchRS("Scientist_ID"), "User_Code")%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "Batches.Chemist_Code", "SCRIPT:loadHelperFrame_edit(this.value," & lookup_row_id & ")","9"%>
														</font></td>
													<%else
														theSciValue =getValueFromTablewConn(RegConn, "People", "Person_ID", BaseRS("Scientist_ID"), "User_Code")%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "Temporary_Structures.Chemist_Code", "SCRIPT:loadHelperFrame_edit(this.value," & lookup_row_id & ")","9"%>
														</font></td>
													<%end if%>
												<%else
													%><td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
													<%if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
														Response.write getValueFromTablewConn(RegConn, "People", "Person_ID", BatchRS("Scientist_ID"), "User_Code")
													else
														Response.write getValueFromTablewConn(RegConn, "People", "Person_ID", BaseRS("Scientist_ID"), "User_Code")
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
														set rs = RegConn.Execute(sql)
														if not (rs.eof and rs.bof)then
															theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
															closers(rs)
														end if
													' This field is for display only from an input chemist code.
													' The field temporary.chemist_name doesn't exist.
														if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
															<input type = "text" name = "Batches.Chemist_Name" value = "<%=theValue%>" size = "20" onChange= "checkChemCode(this.value)">
															<script name = "javascript">
																	putChemCode("<%=theSciValue%>")
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
														set rs = RegConn.Execute(sql)
														if not (rs.eof and rs.bof)then
															theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
															closers(rs)
															Response.Write theValue	
														end if
													end if%>
												</font></td>
												<%else%>
													<%if CBOOL(Application("NOTEBOOK_USED")) = true and  not checkHideField("SCIENTIST_ID")then%>
														<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
															<td width = "150" <%=td_bgcolor%> ><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Scientist_ID", People_List, People_Val,People_Text,0,false,"value",default_chemist_id %>
															</td>
														<%end if%>
													<%end if%>
												<%end if%>
												<%'end scientist name lookup%>
												<%if not checkHideField("PURITY") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.PURITY", batch_output, setDisplayType("PURITY"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.APPEARANCE", batch_output, setDisplayType("APPEARANCE"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("Creation_Date") then%>
												<td width = "150" <%=td_bgcolor%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Creation_Date",  batch_output,  "DATE_PICKER:8","10"%></font>
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
                            <table width="664">
                                <tr>
									<td>
										<table width="664" >
											<%if CBool(Application("Batch_Projects_Used")) = true then%>
												<td width = "150" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b> &nbsp;<%=getLabelName("Batch_Project_Id")%></b></font>
												</td>
											<%end if%>
											<%'select a producer by choosing a user_code from a drop down list.
											If Application("PRODUCER_LOOKUP") = 1 or Application("PRODUCER_LOOKUP") = "" then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Code")%></font></b> </td>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Name")%></font></b> </td>
											<%end if%>
											<%'end select producer%>
											<%if CBool(Application("NOTEBOOK_USED")) = true then%>
												<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
												<td width = "150" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b> <%=getLabelName("Notebook_Number")%></b></font>
												</td>
												<%else%>
												<td width = "150" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b> <%=getLabelName("Notebook_Text")%></b></font>
												</td>
												<%end if%>
											<%end if%>
											
											<%if not checkHideField("NOTEBOOK_PAGE") then%>
												<td nowrap <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Notebook_Page")%></b></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT") then%>
												<td nowrap <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("Amount")%></b></font>
												</td>
                                            <%end if%>
 
                                            <%if not checkHideField("AMOUNT_UNITS") then%>
												<td nowrap <%=td_caption_bgcolor%> ><font  <%=font_default_caption%>><b><%=getLabelName("Amount_Units")%></b></font>
												</td>
                                            <%end if%>
										</tr>
										<tr>
											
												<%if CBool(Application("Batch_Projects_Used")) = true then%>
												<td  valign="top" <%=td_bgcolor%>><font <%=font_default%>>
												  	<%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Batch_Project_ID", Batch_Projects_List, Batch_Projects_Val,Batch_Projects_Text,0,true,"value","0" %>
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
           													theProdCode= getValueFromTablewConn(RegConn, "People", "Person_ID", prodID, "User_Code")
           													if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<%ShowInputField dbkey, formgroup, "Batches.Producer_Code", "SCRIPT:loadHelperFrame2_edit(this.value," & lookup_row_id & ")","9"%>
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
															<%Response.write getValueFromTablewConn(RegConn, "People", "Person_ID", prodID, "User_Code")%>
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
															set rs = RegConn.Execute(sql)
															if not (rs.eof and rs.bof)then
																theValue = rs("First_Name") & " " & rs("Middle_Name")& " " & rs("Last_Name")
																closers(rs)
															end if 
															if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
																<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
																<input type = "text" name = "Batches.Producer_Name" value = "<%=theValue%>" size = "20" onBlur= "checkProdCode(this.value)">
																<script name = "javascript">
																	putProdCode("<%=theProdCode%>")
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
																set rs = RegConn.Execute(sql)
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
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Notebook_Number", Notebooks_List, Notebooks_Val,Notebooks_Text,0,true,"value","0" %>
													</td>
												<%else%>                                                
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Notebook_Text",  batch_output,  setDisplayType("Notebook_Text"),"25"%>
													</font></td>
												<%end if%>
											<%end if%>
												
												<%if not checkHideField("NOTEBOOK_PAGE") then%>
													<td  valign="top" <%=td_bgcolor%>><font <%=font_default%>>
														<%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Notebook_Page",  batch_output, setDisplayType("Notebook_Page"),"10"%></font>
													</td>
												<%end if%>
										

											<%if not checkHideField("AMOUNT") then%>
												<td valign="top" <%=td_bgcolor%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Amount",  batch_output,  setDisplayType("Amount"),"15"%></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT_UNITS") then%>
											
												<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup,basers, "TEMPORARY_STRUCTURES.AMOUNT_UNITS",  batch_output,  setDisplayType("AMOUNT_UNITS"),"15"%></font>
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
									<table >
										<tr>
											<td>  
												<table border="0" width="664">
													<%if not checkHideField("LIT_REF") then%>
														<tr>
															<td <%=td_caption_bgcolor%> colspan="3" ><font <%=font_default%>><b><%=getLabelName("LIT_REF")%></b></font>
															</td>
														</tr>

														<tr>
															<td colspan="3" <%=td_bgcolor%>  ><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Lit_Ref",   batch_output,  setDisplayType("Lit_Ref"), "72"%></font>
															</td>
														</tr>
													<%end if%>

													<tr>
														<%if not checkHideField("SOURCE") then%>
															<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Source")%></font></strong>
															</td>
														<%end if%>
                                                 
														<%if not checkHideField("VENDOR_NAME") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BaseRS("Source")) = "VENDOR" then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_NAME_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("Vendor_Name")%></font></strong></div>
															<%'end if%>
															</td>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BaseRS("Source")) = "VENDOR" then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_ID_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("Vendor_ID")%></font></strong></div>
															<%'end if%>
															</td>
														<%end if%>
												
													</tr>
 
													<tr>
														<%if not checkHideField("SOURCE") then%>
															<font <%=font_default%>>
																<td nowrap  <%=td_bgcolor%>>&nbsp;<%ShowResult dbkey, formgroup,BaseRS, "TEMPORARY_STRUCTURES.SOURCE", batch_output, setDisplayType("SOURCE"),"15"%>
																</td>
															</font>
														<%end if%>
                                                
														<%if not checkHideField("VENDOR_NAME")  then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BaseRS("Source")) = "VENDOR" then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_NAME"><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.VENDOR_NAME",  batch_output, setDisplayType("VENDOR_NAME"),"15"%></div>
																</td>
															</font>
															<%'end if%>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<% 'if  UCase(Formmode) = "EDIT_RECORD" or UCase(BaseRS("Source")) = "VENDOR" then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_ID"><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.VENDOR_ID", batch_output, setDisplayType("VENDOR_ID"),"10"%></div>
																</td>
															</font>
															<%'end if%>
														<%end if%>
													</tr>
												</table>
											</td>
										</tr>
									</table>
                                    
                                    <table>
										<tr>
											<td>
												<table border = "0" width = "664">
													<%if not checkHideField("BATCH_COMMENT") then%>
														<tr>
															<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("BATCH_COMMENT")%></b>
																</font>
															</td>
														</tr>

														<tr>
															<td  <%=td_bgcolor%>><font <%=font_default%>>
																<%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Batch_Comment",   batch_output,  setDisplayType("Batch_Comment"), "72"%>
															</font></td>
															
														</tr>
													<%end if%>
													
													<%if not checkHideField("PREPARATION") then%>
														<tr>
															<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("PREPARATION")%></b></font>
															</td>
														</tr>
														
														<tr>
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Preparation", batch_output,  setDisplayType("Preparation"), "72"%></font>
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
															<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Storage_Req_And_Warnings",batch_output,    setDisplayType("Storage_Req_And_Warnings"), "72"%>
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
									<table  width="664">
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
														<%if not checkHideField("Temporary_Structures.H1NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.H1NMR")%></strong></font>   
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.H1NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.H1NMR")%> >  <font <%=font_default%>><strong><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.H1NMR",batch_output, setDisplayType("H1NMR"),"15"%></font>   
															</td>
														<%end if%>
                                              
														<%if not checkHideField("Temporary_Structures.MP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.MP")%></font></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.MP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.MP")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,  "Temporary_Structures.MP",  batch_output, setDisplayType("MP"),"15"%></font>
															</td>
														<%end if%>
													
													</tr>
													<tr>
														<%if not checkHideField("Temporary_Structures.C13NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.C13NMR")%></strong></font>
															</td>
														<%end if%>	  
														
														<%if not checkHideField("Temporary_Structures.C13NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.C13NMR")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.C13NMR",batch_output, setDisplayType("C13NMR"),"15"%>   </font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Temporary_Structures.BP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.BP")%></font></strong></font>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.BP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.BP")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.BP",  batch_output, setDisplayType("BP"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.HPLC") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.HPLC")%></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.HPLC") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.HPLC")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.HPLC", batch_output,setDisplayType("HPLC"),"15"%>   </font>
															</td>
														<%end if%>	
                                              
														<%if not checkHideField("Temporary_Structures.SOLUBILITY") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.SOLUBILITY")%></font></strong></font>
															</td>
														<%end if%>	 
														
														<%if not checkHideField("Temporary_Structures.SOLUBILITY") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.SOLUBILITY")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.SOLUBILITY",  batch_output, setDisplayType("SOLUBILITY"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.MS")%></strong></font>
														</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.MS")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.MS",     batch_output ,setDisplayType("MS"),"15"%></font>
															</td>
														<%end if%>		
                                               
                                              
														<%if not checkHideField("Temporary_Structures.Optical_Rotation") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.Optical_Rotation")%></font></strong></font>
															</td>
														<%end if%>	
															
														<%if not checkHideField("Temporary_Structures.Optical_Rotation") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Optical_Rotation")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Optical_Rotation",    batch_output ,setDisplayType("Optical_Rotation"),"15"%></font>
															</td>
														<%end if%>		
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.LC_UV_MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.LC_UV_MS")%></font></strong>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.LC_UV_MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.LC_UV_MS")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.LC_UV_MS",  batch_output ,setDisplayType("LC_UV_MS"),"15"%></font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Temporary_Structures.Physical_Form") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.Physical_Form")%></font></strong></font>
															</td>
														<%end if%> 
														
														<%if not checkHideField("Temporary_Structures.Physical_Form") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Physical_Form")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.Physical_Form",  batch_output ,setDisplayType("Physical_Form"),"15"%></font>
															</td>
														<%end if%>
                                                </tr>

                                                <tr>
													<%if not checkHideField("Temporary_Structures.IR") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.IR")%></font></strong>
														</td>
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.IR") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.IR")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.IR",   batch_output ,setDisplayType("IR"),"15"%></font>
														</td>
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.LogD") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.LogD")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.LogD") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.LogD")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.LogD",  batch_output ,setDisplayType("LogD"),"15"%></font>
														</font>
														</td>  
													<%end if%>
												</tr>	

												<tr>
													<%if not checkHideField("Temporary_Structures.UV_SPECTRUM") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.UV_SPECTRUM")%></font></strong>
														</td>
													<%end if%>
												 
													<%if not checkHideField("Temporary_Structures.UV_SPECTRUM") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.UV_SPECTRUM")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.UV_SPECTRUM",  batch_output ,setDisplayType("UV_SPECTRUM"),"15"%></font>
														</td>
													<%end if%>
                                     
													<%if not checkHideField("Temporary_Structures.Refractive_Index") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>><font <%=font_default_caption%>> <strong> <%=getLabelName("Refractive_Index")%></strong></font>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Refractive_Index") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Refractive_Index")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Refractive_Index",  batch_output ,setDisplayType("Refractive_Index"),"15"%></font>
														</td>
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Temporary_Structures.GC") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.GC")%></font></strong>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.GC") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.GC")%> ><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.GC",  batch_output ,setDisplayType("GC"),"15"%></font>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.FlashPoint") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.FlashPoint")%></strong>
														</font></strong>
														</td> 
													 <%end if%>

													<%if not checkHideField("Temporary_Structures.FlashPoint") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.FlashPoint")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.FlashPoint",  batch_output ,setDisplayType("FlashPoint"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Temporary_Structures.CHN_COMBUSTION") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.CHN_COMBUSTION")%></strong>
														</font>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.CHN_COMBUSTION") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.CHN_COMBUSTION")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.CHN_COMBUSTION",  batch_output ,setDisplayType("CHN_COMBUSTION"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Color") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Color")%></strong>
														</font></strong>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Color") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Color_1")%> > <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Color",  batch_output ,setDisplayType("Color"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	

                                                <tr>
													<%if not checkHideField("Temporary_Structures.Field_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_1") then%>                           
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_1")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_1",  batch_output ,setDisplayType("Field_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.Field_2") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_2")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_2",  batch_output ,setDisplayType("Field_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("Temporary_Structures.Field_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_3") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_3")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_3",  batch_output ,setDisplayType("Field_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_4") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_4")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_4",  batch_output ,setDisplayType("Field_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("Temporary_Structures.Field_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_5") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_5")%> >   <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_5",  batch_output ,setDisplayType("Field_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.Field_6") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_6")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_6",  batch_output ,setDisplayType("Field_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                     
                                                <tr>
													<%if not checkHideField("Temporary_Structures.Field_7") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_7")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_7") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_7")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_7",  batch_output ,setDisplayType("Field_7"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Field_8") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_8")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_8") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_8")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_8",  batch_output ,setDisplayType("Field_8"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                
                                                <tr>
													<%if not checkHideField("Temporary_Structures.Field_9") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_9")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_9") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_9")%> >    <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_9",  batch_output ,setDisplayType("Field_9"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_10") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_10")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_10") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_10")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.Field_10",  batch_output ,setDisplayType("Field_10"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                 <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_1") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_1",  batch_output ,setDisplayType("INT_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2",  batch_output ,setDisplayType("INT_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_3") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_3") then%>                           
														<td   <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_3",  batch_output ,setDisplayType("INT_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4",  batch_output ,setDisplayType("INT_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                                                <tr>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_5")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_5") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_5",  batch_output ,setDisplayType("INT_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6") then%>                           
														<td  width = "150"  <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_6")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6", batch_output,  setDisplayType("INT_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_1",  batch_output ,setDisplayType("REAL_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2",  batch_output ,setDisplayType("REAL_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_3") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_3",  batch_output ,setDisplayType("REAL_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4",  batch_output ,setDisplayType("REAL_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_5") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_5",  batch_output ,setDisplayType("REAL_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6") then%>                           
														<td width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6",  batch_output ,setDisplayType("REAL_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                     
                                                
                                                <tr>
                                                <%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_1")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_1",  batch_output ,setDisplayType("DATE_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2",  batch_output ,setDisplayType("DATE_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_3") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_3",  batch_output ,setDisplayType("DATE_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4") then%>                           
														<td   "width = "150"  <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4",  batch_output ,setDisplayType("DATE_BATCH_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												<tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_5") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_5")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_5") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_5",  batch_output ,setDisplayType("DATE_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS,"TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6",  batch_output ,setDisplayType("DATE_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>

                                            </table>
                                        </td>
                                    </tr>
								</table>
								<!--end physical properties--></body>


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
<%end if 'if not UCase(commit_type) = "ADD_IDENTIFIERS" then%>

<%end select
end if
if request("formmode") = "edit_record" then
	compound_type = getValueFromTablewConn(RegConn, "compound_type", "compound_type", Compound_Type_Val, "description")

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
<%end if%>



<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_footer_vbs.asp"-->
<script language="JavaScript">
	windowloaded = true
</script>


</table>
</body>
</html>