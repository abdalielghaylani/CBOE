<%@ LANGUAGE="VBScript" %>
<%Response.Expires = -1
Response.Buffer = true
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
%>
<html>
<head>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<script language = "javascript">
	var db_record_count = "<%=DBRecordCount%>"
	//db_record_count = get_db_record_count("reg_numbers")
	
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

<!--#INCLUDE FILE = "../source/reg_security.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->
<title>Chemical Registration - Registered Compounds Results List View</title>
</head>
<body <%=Application("BODY_BACKGROUND")%> bgProperties="fixed">
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%	Session("ListLocation" & dbkey & formgroup)=Session("CurrentLocation" & dbkey & formgroup)
on error resume next 
	'create connection for use by all recordsets 
	Set DataConn = GetConnection(dbkey, formgroup, "Reg_Numbers")
	if DataConn.State=0 then ' assume user has been logged out
		DoLoggedOutMsg()
	end if
	
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_vbs.asp"-->
<%
	'BaseID represents the primary key for the recordset from the base array for the current row
	'BaseActualIndex is the actual id for the index the record is at in the array
	'BaseRunningIndex if the id based on the number shown in list view
	'BastTotalRecords is the recordcount for the array
	'BaseRS (below is the recordset that is pulled for each record generated


	UpdateBaseRecordCount dbkey, formgroup, DataConn


	'get base table recordset	
	'get base table recordset
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
	
	
	if Not (BaseRS.BOF=True and BaseRS.EOF=True) then
		BaseRS.MoveFirst
		cpdDBCounter = BaseRS("cpd_internal_id")
		reg_ID = BaseRS("reg_ID")
	
		' Get batch_internal_id for use later...
		
		if bBaseRegNum=false then
			Dim BatchRS
			Set BatchRS_cmd = Server.CreateObject("ADODB.COMMAND")
			BatchRS_cmd.commandtype = adCmdText
			BatchRS_cmd.ActiveConnection = DataConn
			sql="select batch_number from batches where batch_internal_id = ?"
			BatchRS_cmd.CommandText = sql
			BatchRS_cmd.Parameters.Append BatchRS_cmd.CreateParameter("pBaseIDID", 5, 1, 0, BaseID) 
			Set BatchRS = Server.CreateObject("ADODB.RECORDSET")
			BatchRS.Open BatchRS_cmd
			fullRegNumber = BaseRS("reg_number") & "/" & padNumber(Application("BATCH_NUMBER_LENGTH_GUI"),BatchRS("Batch_Number"))
			baseregnumber = fullRegNumber
		else	
		
			fullRegNumber = BaseRS("reg_number") 
		end if
		
		sql= GetDisplaySQL(dbkey, formgroup, "Compound_Molecule.*","Compound_Molecule", "", cpdDBCounter, "")
		Set CompoundRS  =DataConn.Execute(sql)

		
		Compound_Type_Val =CompoundRS("Compound_Type")
		Compound_Type_Text = getValueFromTablewConn(DataConn, "Compound_Type", "Compound_Type", Compound_Type_Val, "Description")
		
		If (CBool(Application("APPROVED_FLAG_USED")) = True or CBool(Application("QUALITY_CHECKED_FLAG_USED")) = True )then
			If (UCase(Application("APPROVED_SCOPE"))= "BATCH" OR UCase(Application("QUALITY_CHECKED_SCOPE")) = "BATCH") then
				'SYAN modified on 6/6/2005 to fix CSBR-55513
				sql = "Select Min(Batch_Internal_ID) as FirstID from batches where reg_internal_id=" & reg_id
				'End of SYAN modification
				Set BatchRS= DataConn.Execute(sql)
				firstBatchID=BatchRS("FirstID")
			else
				firstBatchID = ""
			end if
			if CBool(Application("APPROVED_FLAG_USED")) = True then
				'stop
				bFBApproved = getApprovedFlag(DataConn, reg_ID, firstBatchID)
			end if
			if CBool(Application("QUALITY_CHECKED_FLAG_USED")) = True then
				bFBQualityChecked = getQualityCheckedFlag(DataConn, reg_ID, firstBatchID)
			end if
		end if
	
	
	
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
		
		
		
	
		sql= GetDisplaySQL(dbkey, formgroup, "Compound_Project.*","Compound_Project", "", cpdDBCounter  , "")
		Set Compound_ProjectRS=DataConn.Execute(sql)
	
		Projects_Val = Compound_ProjectRS("Project_Internal_ID")
		Projects_Text = getValueFromTablewConn(DataConn, "Projects", "Project_Internal_ID", Projects_Val, "Project_Name")

		if UCase(Application("Batch_Level")) = "SALT" then
			sql= GetDisplaySQL(dbkey, formgroup, "Compound_Salt.*","Compound_Salt", "", reg_ID , "")
			Set Compound_SaltRS=DataConn.Execute(sql)
			Salts_Val=Compound_SaltRS("Salt_Internal_ID")
			Salts_Text = getValueFromTablewConn(DataConn,"salts", "salt_code", Salts_Val, "Salt_Name")			
		end if
			
		if UCase(request("formmode")) = "EDIT_RECORD" then
			if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
				Notebooks_List= GetUserNotebookList(DataConn, UCase(Session("CurrentUser" & dbkey)))

			end if
			Salts_List= GetAvailSaltList(DataConn,cpdDBCounter)
			Salts_List = Salts_Val & ":" & Salts_Text & "," & Salts_List
			Projects_List = GetProjectsList(DataConn)
			Compound_Types_List= GetCompoundTypeList(DataConn)
			Sequence_List = GetSequenceList(DataConn)			
			TrueFalseList = "-1:True,0:False"
		End if

		sql = "Select * from Structures where cpd_internal_id = " & cpdDBCounter
		Set StructuresRS = DataConn.Execute(sql)
		
		sql = "Select Count(batch_internal_id) as COUNT from batches where reg_internal_id = " & baseid
		Set CountRS = DataConn.Execute(sql)
		BatchCount = CountRS("COUNT")
		CountRS.Close
%>
<table <%=table_main_L1%> width="750">
	<tr>
		<td valign="top" width="140" nowrap>
			<table >
				<tr>
					<td><%response.write "Record " & BaseRunningIndex & " of "  & BaseTotalRecords%></td>
				</tr>
				<tr>
					<td><script language="JavaScript"><!--
						
						getMarkBtn(<%=BaseID%>)
						document.write ('<br>')
						
						getFormViewBtn("show_details_btn.gif","reg_result_form.asp","<%=BaseActualIndex%>")
						document.write ('<br>')
						// --></script>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" width="630">
			<table >
				<tr>
					<td width="100%">
						<table  width="100%" cellpadding="3" cellspacing="0">
							<INPUT type="hidden" name="reg_internal_ID" value="<%=reg_ID%>">
							<INPUT type="hidden" name="cpdDBCounter" value="<%=cpdDBCounter%>">
				
							<tr>
								<td >
										<table width="100%" >
											<tr>
												<td >
												
													<tr><font  <%=font_caption%>>
														<%if bBaseRegNum = true then %>
                      										<td <%=td_bgcolor%> nowrap><%=fullregnumber%> </td>
															<td width="160" align="center" valign="top"># Batches:  <%=BatchCount%></td>
														<%else
															'support basetable = batches%>
															<td <%=td_bgcolor%> nowrap><%=fullregnumber%> </td>
															
														<%end if%>
														<%if Application("APPROVED_FLAG_USED") = 1 then%> </font>
														
														<td width="93" nowrap align="right" valign="top"><font face="Arial" size="2"><b><%=getLabelName("Approved")%></b></font></td>
														<td width="20" align="center" valign="top"><%
															if bFBApproved = True Then%>
																<%Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
															else
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
															end if
															end if%>
														</td>
														
														<%if Application("QUALITY_CHECKED_FLAG_USED") = 1 then%>
														<td width="134" nowrap align="right" valign="top"><font face="Arial" size="2"><b><%=getLabelName("Quality_Checked")%></b></font></td>
														<td width="22" align="center" valign="top"><%
															if bFBQualityChecked = True Then
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
															else
																Response.Write "<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
															end if%>
														</td>
														<%end if%> 

													</font></tr>
												</table>
											</td>
										
											<tr>
												<td>
												<% 
													ShowResult dbkey, formgroup, StructuresRS,"Structures.BASE64_CDX", "Base64CDX", 200, 150
												%>
											    </td>
												<td width="380">
													<table >
														
														
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
															<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%>
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
															<td <%=td_caption_bgcolor%> align="right" width="170"><b><font <%=font_default_caption%>><%=getLabelName("MW")%></font></b></td>
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.MW2", "raw", setDisplayType("MW"), "15"%></font></td>
														</tr>
													<%end if%>
													 <%if not checkHideField("FORMULA") then%>
														<tr>
															<td <%=td_caption_bgcolor%> align="right" width="170"><b><font <%=font_default_caption%>><%=getLabelName("FORMULA")%></font></b></td>
															<td <%=td_bgcolor%>><font <%=font_default%>>
																<%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.FORMULA2", "raw", setDisplayType("FORMULA"), "15" %>
																</font>
															</td>
														</tr>
													<%end if%>
													
													 <%if not checkHideField("MW_TEXT") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><b><font <%=font_default_caption%>><%=getLabelName("MW_TEXT")%></font></b></td>
														<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.MW_Text", "raw" ,setDisplayType("MW_Text"),"6"%></font></td>
													</tr>
													<%end if%>
													 <%if not checkHideField("MF_TEXT") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><b><font <%=font_default_caption%>><%=getLabelName("MF_TEXT")%></font></b></td>
														<td <%=td_bgcolor%>><font <%=font_default%>><%ShowResult dbkey, formgroup, CompoundRS, "Compound_Molecule.MF_Text", "raw" ,setDisplayType("MF_TEXT"),"6"%></font></td>
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

													<!--End of SYAN modification-->
												</table>
											</td>
										</tr>
						            </table>
						        </td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table width="100%">
	<tr>
		<td width="10%">&nbsp;</td><td width="60%"><hr width="100%"></td><td>&nbsp;</td>
	</tr>
</table>

<%
else
	Response.Write "<P><FONT & font_caption_red & >Record Deleted.</FONT></P>"
end if%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp"-->
<%'close and kill connection after all rows are generated
CloseConn(DataConn)%>
</body>
</html>

