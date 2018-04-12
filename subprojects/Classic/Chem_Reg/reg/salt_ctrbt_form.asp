<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
Response.Buffer = true

'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>

<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->
<%  if Not checkHideField("SOURCE") then
		if Not checkHideField("VENDOR_NAME") AND instr(UCase(setDisplayType("source")), "ISVENDOR")>0 then
			if detectIE() = true and Not checkHideField("SOURCE") then
			
			%>
<style>
<!--
#VENDOR_NAME{visibility:hidden}
#VENDOR_NAME_LABEL{visibility:hidden}
#VENDOR_ID{visibility:hidden}
#VENDOR_ID_LABEL{visibility:hidden}
-->
</style>

<%else%>
<style>
<!--
#VENDOR_NAME_LABEL{position:relative;visibility:hidden}
#VENDOR_NAME{position:relative;visibility:hidden}
#VENDOR_ID_LABEL{position:relative;visibility:hidden}
#VENDOR_ID{position:relative;visibility:hidden}
-->
</style>
			<%end if%>
	<%end if%>
<%end if%>
<title>Chemical Registration - Add Salt Input Form</title>

<%record_added = request.querystring("record_added")
	if not record_added <> "" then
		record_added = "none"
	end if
	if Not record_added = "none" then%>
<script language="javascript">
		alert("Your record was added to the temporary table")

	</script>
<%end if%>
<script language="javascript">

var commit_type="add_salt"
var cpd_counter="<%=request.querystring("cpd_counter")%>"
var reg_ID = "<%=request.querystring("reg_ID")%>"
</script>
<%	
on error resume next
	commit_type = "add_salt"
	Set DataConn = GetNewConnection(dbkey, formgroup, "base_connection")
	if DataConn.State=0 then ' assume user has been logged out
		DoLoggedOutMsg()
	end if
	reg_ID = ""
	cpdDBCounter= Request.QueryString("cpd_counter")
	project_id = getValueFromTablewConn(DataConn,"Compound_Project", "cpd_internal_id", cpdDBCounter, "project_internal_id")

	if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
		Notebooks_List= GetUserNotebookList(DataConn, UCase(Session("CurrentUser" & dbkey)))
	else
		default_chemist_id = getValueFromTablewConn(DataConn, "People", "user_id", UCase(Session("CurrentUser" & dbkey)), "Person_id")
	end if
	Projects_List = GetProjectsList(DataConn)
	Batch_Projects_List= GetBatchProjectsList(DataConn)
	Compound_Type_List= GetCompoundTypeList(DataConn)
	Sequence_List = GetSequenceList(DataConn)			
	Salts_List= GetAvailSaltList(DataConn, cpdDBCounter)
	Salts_Batch_List = GetSaltsListBatch(DataConn)
	Solvates_List = GetSolvatesList(DataConn)
	display_type = "root_number"
	bShowRegIdentifiers = False
	bShowRegCmpds = true
	bShowBatchData = true
	bShowRegSalt = false
	People_List = GetPeopleList(DataConn)
	if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
		Notebooks_List= GetUserNotebookList(DataConn, UCase(Session("CurrentUser" & dbkey)))
	else
		default_chemist_id = getValueFromTablewConn(DataConn, "People", "user_id", UCase(Session("CurrentUser" & dbkey)), "Person_id")
	end if
	'registered compound information recordset generation 
	'assumes cpdDBCounter and reg_ID variables are populated
	getRegData DataConn, reg_ID, request("cpd_counter"), "Root_Number" 
	'BaseRS.MoveFirst
	sql = "Select * from Structures where cpd_internal_ID = " & cpdDBCounter
		Set MoleculesRS = DataConn.Execute(sql)
			
	DBMSUser_ID = Session("CurrentUser" & dbkey)
	entry_person_ID = getValueFromTablewConn(DataConn, "People", "User_ID", DBMSUser_ID, "Person_ID")
	'PRODUCT_TYPE_TEXT = CompoundRS("PRODUCT_TYPE")
	'PRODUCT_TYPE_VAL = PRODUCT_TYPE_TEXT
	'PRODUCT_TYPE_LIST = GetSelectList(Application("PRODUCT_TYPE_LOOKUP"))
		
	'UNITS_TEXT=BatchRS("Units")
	'UNITS_VAL = UNITS_TEXT
	'UNITS_LIST = GetSelectList(Application("UNITS_LOOKUP"))
	'SOURCE_TEXT=BatchRS("SOURCE")
	'SOURCE_VAL = SOURCE_TEXT
	'SOURCE_LIST = GetSelectList(Application("SOURCE_LOOKUP"))
	the_source_array = split(Application("SOURCE_LOOKUP"),",",-1)
	
%>

</head>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type="hidden" name="return_location" value="<%=Session("CurrentLocation" & dbkey) %>"><input type="hidden" name="Temporary_Structures.Entry_Person_ID" value="<%=entry_person_ID%>"><input type="hidden" name="Temporary_Structures.Entry_Date" value="<%=Date()%>"><%if CBOOL(Application("SALTS_USED")) = False then %>
<%end if%>
<input type = "hidden" name = "cpdDBcounter" value ="<%=request("cpd_counter")%>">

<input type = "hidden" name = "Temporary_Structures.Entry_Person_ID" value ="<%=entry_person_ID%>">
<input type = "hidden" name = "Temporary_Structures.Entry_Date" value ="<%=Date()%>">
<% if CBOOL(Application("SALTS_USED")) = False OR UCase(Application("Batch_Level")) = "COMPOUND" then%>
<input type = "hidden" name = "Temporary_Structures.Salt_Code" value ="1">
<%end if%>
<% if CBOOL(Application("SOLVATES_USED")) = False  OR UCase(Application("Batch_Level")) = "COMPOUND" then%>
<input type = "hidden" name = "Temporary_Structures.Solvate_ID" value ="1">
<%end if%>
<% if CBOOL(Application("COMPOUND_TYPES_USED")) = False then%>
<input type = "hidden" name = "Temporary_Structures.Compound_Type" value ="1">
<%end if%>
<% if Not CBOOL(Application("PROJECTS_USED")) = True then%>
<input type = "hidden" name = "Temporary_Structures.Project_ID" value ="1">
<%else%>
<input type = "hidden" name = "Temporary_Structures.Project_ID" value ="<%=project_id%>">
<%end if%>

<% if CBOOL(Application("BATCH_PROJECTS_USED")) = False then%>
<input type = "hidden" name = "Temporary_Structures.Batch_Project_ID" value ="1">
<%end if%>
<% if CBOOL(Application("NOTEBOOK_USED")) = False then
	if  CBOOL(Application("NOTEBOOK_LOOKUP")) = FALSE then%>
		<input type = "hidden" name = "Temporary_Structures.Notebook_Number" value ="1">
		<%if Not Application("USER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "Temporary_Structures.Scientist_ID" value ="<%=default_chemist_id%>">
		<%end if%>
	<%else%>
		<%if Not Application("USER_LOOKUP") = 1 then%>
			<input type = "hidden" name = "Temporary_Structures.Scientist_ID" value ="1">
		<%end if%>
	<%end if%>
<%end if%>
<%'SEARCH and ADD Record Mode support for producer and user lookups when batch is the basetable
if Application("USER_LOOKUP") = 1  or Application("PRODUCER_LOOKUP") = 1 then
	LOOKUP_TABLE_DESTINATION = "Temporary_Structures" 'This is case sensitive. Must be Batches or Temporary_Structures
	if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
		lookup_row_id = Batch_ID
	else
		lookup_row_id = Tempid
	end if
	PRODUCER_ID_FIELD = UCase(Application("producer_id_field"))
	if Application("USER_LOOKUP") = 1 then%>
		<input type = "hidden" name = "<%=LOOKUP_TABLE_DESTINATION%>.Scientist_ID" value ="">
	<%end if
		if Application("PRODUCER_LOOKUP") = 1 then%>
		<input type = "hidden" name = "<%=UCase(LOOKUP_TABLE_DESTINATION)%>.<%=PRODUCER_ID_FIELD%>" value ="">
		<%end if
end if
'end producer and user lookup%>
<%'chemreg header information%>
<table border="0">
	<tr>
		<td width="40%" align="left" nowrap>
			<p align="left"><strong><font face="Arial" size="3" color="#182889">New Salt Submission Form</font></strong></p>
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
<%'start registered compound information%>

<table  <%=registered_compound_table%> width = "770">
			 <%if UCase(Application("BATCH_LEVEL")) = "SALT" then 
					RegOutput= Session("Root_Number")
               else 
                  RegOutput= Session("Reg_Number")
               end if%> 
	<tr>
		<td >
			<table>
				<tr>
					<td colspan="5" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%>>Registered&nbsp;
					 Root Compound&nbsp;Information:&nbsp;&nbsp;</td></tr><tr>
					<td width="20%"><table><tr><td <%=td_bgcolor%>  nowrap><strong><font <%=font_default%>><%=RegOutput%></font></td>
					<td colspan="1" <%=td_bgcolor%> nowrap><strong><font <%=font_default_caption%>>&nbsp;&nbsp;&nbsp;&nbsp;Registered:</font>
					<strong><font <%=font_default%>><%=Session("RootRegDate")%></font></td>
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
										<td valign="top" align="left"><table border = "1" bgcolor="#FFFFFF"><tr><td><%Base64DecodeDirect dbkey, formgroup, Session("Base64_cdx"), "Structures.BASE64_CDX", Session("reg_id") , Session("cpdDBCounter"), "330" & ":BASE64CDX_NO_EDIT", "200"%> &nbsp;</tr></td></table>
										</td>
										
										<td valign="top" align="left">
											<table width = "320">
												
												
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
										<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%><%=add_gen_text%></b></font>
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
	
<%'end registered compound information%>
<br>
<br>
<%'start salt information%>
<%if UCase(Application("Batch_Level")) = "SALT" then %>
	<input type = "hidden" name = "salts_batch_list" value ="<%=salts_batch_list%>">
	<table <%=table_main_L1%> width = "700">
		<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>New Compound Salt Information</font></strong>
			</td>
		</tr>
		
		<tr>
			<td  <%=td_bgcolor%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Temporary_Structures.Salt_Code", Salts_List, Salts_Val,Salts_Text,0,true,"value" & ":VALIDATE:" & "updateSaltName(this.form,this," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & ")","0" %>
			</td>
		</tr>
		</table>
  <%end if%>
<%'end salt information%>
<%'start new identifiers%>

			<table  <%=table_main_L1%> width = "650" >
						
						<tr>
							<td <%=td_header_bgcolor%>>
									<p align="left"><strong><font <%=font_header_default_1%>>New Identifiers</font></strong>
							</td>
                          </tr>
                          <tr>
                          <td>
                          <br>
                          <table>
							<%if not checkHideField("CHEMICAL_NAME") then%>

									<tr>
										<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("CHEMICAL_NAME")%></font></strong>
										</td>
									</tr>
              
									<tr>
										<td  <%=td_bgcolor%>>
												
											<font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Chemical_Name", setDisplayType("Chemical_Name"), "75"%></font>
										</td>
									</tr>
								<%end if%>
								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey,formgroup, "Temporary_Structures.Synonym_R", setDisplayType("Synonym_R"), "75"%></font>
										</td>
									</tr>
								<%end if%>
                          
								<%if not checkHideField("CAS_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%></b></font>
										</td>
											</tr>
										<tr>				
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.CAS_Number",   setDisplayType("CAS_Number"),"100"%></font>
										</td>
									</tr>
								<%end if%>
													
								<%if not checkHideField("GROUP_CODE") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
										</td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Group_Code", setDisplayType("GROUP_CODE"),"100"%></font>
										</td>
									</tr>
								<%end if%>
													
								<%if not checkHideField("RNO_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
										</td>
									</tr>
									<tr>						
										<td <%=td_bgcolor%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.RNO_Number", setDisplayType("RNO_NUMBER"),"100"%></font>
										</td>
									</tr>
								<%end if%>
								<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
									    </td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.FEMA_GRAS_Number",  setDisplayType("FEMA_GRAS_NUMBER"),"100"%></font>
										</td>
									</tr>
								<%end if%>
													
								
							
								<%if not checkHideField("Collaborator_ID") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("Collaborator_ID")%></b></font>
										</td>
									</tr>
									<tr>						
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.COLLABORATOR_ID",  setDisplayType("COLLABORATOR_ID"),"100"%></font>
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
</table><!--end add identifiers-->

<%'end new identifiers%>

<%'start batch info%> 
	<table <%=table_main_L1%> width = "685" >
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
											<td>
												<table>
												<tr> 
												<td <%=td_caption_bgcolor%> width = "200">  <%if not checkHideField("Formula_Weight") then%><font <%=font_default_caption%>><b><%=getLabelName("FORMULA_WEIGHT")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "200"> <%if not checkHideField("Percent_Active") then%> <font <%=font_default_caption%>><b><%=getLabelName("PERCENT_ACTIVE")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "200">  <font <%=font_default_caption%>></font>
												</td>
											</tr>
											
											<tr>
												<%if not checkHideField("Formula_Weight") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.FORMULA_WEIGHT",  setDisplayType("FORMULA_WEIGHT"),"30"%></font></td>
                                                <%end if%>	                               
                                                
                                                <%if not checkHideField("Percent_Active") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>> <%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.PERCENT_ACTIVE",  setDisplayType("PERCENT_ACTIVE"),"30"%></font></td>
												<%end if%>	
												
												<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>> </font></td>
                                            </tr>
										</table>
									</td>
								</tr>

								<tr>
									<td>
										<%if bShowBatchSalt = true then%>
										<table width="675">
											<tr>
												<td width="200" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("SALT_NAME")%></font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("SALT_MW")%></font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("SALT_EQUIVALENTS")%></font></strong>
												</td>
											</tr>

											<tr>
												<td <%=td_bgcolor%>>
													<table>
														<tr><%if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                          
															<%if UCase(Application("Batch_Level")) = "SALT" then %>
																<%if CBool(Application("SALT_EDITABLE_FOR_REG_SALTS")) = true THEN%>
																	<<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Salts_Name", Salts_Batch_List, "0","", "TEMPORARY_STRUCTURES.SALT_NAME","TEMPORARY_STRUCTURES.SALT_MW"%>
																</td>
																
																<td <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAME",    setDisplayType("SALT_NAME"),"25"%>
																</td>
																<%else%> 
																	<td <%=td_bgcolor%>>
																	<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAME",    "hidden","25"%>
																	<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAMEdisplay",    "SCRIPT:alertNoEdit(this.form," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & ")","25"%>
																	</td>
																<%end if%> 
															<%else%>
																<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Salts_Name", Salts_Batch_List, "0","", "TEMPORARY_STRUCTURES.SALT_NAME","TEMPORARY_STRUCTURES.SALT_MW"%>
																</td>
																
																<td <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_NAME",    setDisplayType("SALT_NAME"),"25"%>
																</td>
															<%end if%> 
															<%end if%> 
														</tr>
													</table>
												</td>
												                              
												
												<%if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>
													<%if UCase(Application("Batch_Level")) = "SALT" then %>
																<%if CBool(Application("SALT_EDITABLE_FOR_REG_SALTS")) = true THEN%>
																	
																<td <%=td_bgcolor%>>
														
																		<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MW",  setDisplayType("SALT_MW"),"12"%>
																</td>
																
																<%else%>
																
																<td <%=td_bgcolor%>>
																<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MW", "hidden","25"%>
																<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MWdisplay",  "SCRIPT:alertNoEdit(this.form," & "&quot;TEMPORARY_STRUCTURES.SALT_NAME&quot;,&quot;TEMPORARY_STRUCTURES.SALT_MW&quot;" & ")","12"%>
																</td>
															<%end if%> 
													
													<%else%> 
														<td <%=td_bgcolor%>>
														
															<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MW",  setDisplayType("SALT_MW"),"12"%>
														</td>
													
													<%end if %> 
												<%else%>
														<td <%=td_bgcolor%>>
														
														<%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SALT_MW",  setDisplayType("SALT_MW"),"12"%>
														</td>
												<%end if%>
													
												
												
												<td <%=td_bgcolor%>> <%ShowInputField dbkey, formgroup, "Temporary_Structures.Salt_Equivalents", setDisplayType("Salt_Equivalents"),"12"%>
												</td>
											</tr>
											<% end if
											if bShowBatchSolvate = true then%>
											<tr>
											     <td width="200" <%=td_caption_bgcolor%>> <strong><font <%=font_default_caption%>><%=getLabelName("SOLVATE_NAME")%></font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("SOLVATE_MW")%></font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%> ><strong><font <%=font_default_caption%>><%=getLabelName("SOLVATE_EQUIVALENTS")%></font></strong>
											     </td>
											</tr>

											<tr>
												<td <%=td_bgcolor%> > 
													<table>
														<tr><%if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                                 
																<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Temporary_Structures.Solvates_Name", Solvates_list, "0","", "TEMPORARY_STRUCTURES.SOLVATE_NAME","TEMPORARY_STRUCTURES.SOLVATE_MW"%>
																</td>                                  
															<%end if%>
															
															<td <%=td_bgcolor%>> <%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SOLVATE_NAME",     setDisplayType("SOLVATE_NAME"),"25"%>
															</td>
														</tr>
													</table>
												</td>
												
												<td <%=td_bgcolor%>>  <%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SOLVATE_MW",  setDisplayType("SOLVATE_MW"),"12"%>
												</td>
												
												<td <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Solvate_Equivalents", setDisplayType("Solvate_Equivalents"),"12"%>
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
							<table width="675" >
								<tr>
									<td>
										<table width="675" >
											<tr>
												<%'start user lookup labels SEARCH and ADD Mode
												If Application("USER_LOOKUP") = 1 or Application("USER_LOOKUP") = "" then%>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_Code")%></font></b> </td>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></font></b> </td>
												<%else%>
													<%if CBOOL(Application("NOTEBOOK_USED")) = true and  not checkHideField("SCIENTIST_ID")then%>
														<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
															<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("SCIENTIST_ID")%></strong>
															</td>
														<%end if%>
													<%end if%>
												<%end if
												'end user lookup labels%>
												<%if not checkHideField("PURITY") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("PURITY")%> </strong></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("APPEARANCE")%> </strong></font>
												</td>
												<%end if%>
												<%if not checkHideField("CREATION_DATE") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("CREATION_DATE")%></b></font><font <%=font_default%>>
												</td>
												<%end if%>
											</tr>

											<tr>
												<%' start scientist name lookup SEARCH and ADD mode
												If Application("USER_LOOKUP") = 1 then%>
													<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
													<%if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
														ShowInputField dbkey, formgroup, "Batches.Chemist_Code", "SCRIPT:loadHelperFrame(this.value)","6"
													else
														ShowInputField dbkey, formgroup, "Temporary_Structures.Chemist_Code", "SCRIPT:loadHelperFrame(this.value)","6"
													end if%>
													</td></font>
													<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%' This field is for display only from an input chemist code.
														' The field chemist_name doesn't exist.
														if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then
															ShowInputField dbkey, formgroup, "Batches.Chemist_Name", "SCRIPT:checkChemCode(this.value)","30"
														else
															ShowInputField dbkey, formgroup, "Temporary_Structures.Chemist_Name", "SCRIPT:checkTempChemCode(this.value)","30"
														end if%>
													</td></font>
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
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.PURITY",   setDisplayType("PURITY"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.APPEARANCE",  setDisplayType("APPEARANCE"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("Creation_Date") then%>
												<td width = "150" <%=td_bgcolor%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Creation_Date", "DATE_PICKER:8","12"%></font>
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
												<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("BATCH_PROJECT_ID")%></b></font>
												</td>
											<%end if%>
											<%'select a producer by choosing a user_code from a drop down list.
											If Application("PRODUCER_LOOKUP") = 1 or Application("PRODUCER_LOOKUP") = "" then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Code")%></font></b> </td>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Producer_Name")%></font></b> </td>
											<%end if%>
											<%'end select producer%>
											<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
												<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then%>
													<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("NOTEBOOK_NUMBER")%></b></font>
													</td>
												<%else%>
													<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("NOTEBOOK_TEXT")%></b></font>
													</td>
												<%end if%>
											<%end if%>
											
											<%if not checkHideField("NOTEBOOK_PAGE") then%>
												<td nowrap <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("NOTEBOOK_PAGE")%></b></font>
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
												  	<%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Batch_Project_ID", Batch_Projects_List, Batch_Projects_Val,Batch_Projects_Text,0,true,"value","0" %>
												</font>
												</td>
												<%end if%>
												<%'start producer name lookup SEARCH and ADD Mode
												If Application("PRODUCER_LOOKUP") = 1  then
           											if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														  <%ShowInputField dbkey, formgroup, "Batches.Producer_Code", "SCRIPT:loadHelperFrame2(this.value)","6"%>
														</font></td>
													<%else%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														  <%ShowInputField dbkey, formgroup, "Temporary_Structures.Producer_Code", "SCRIPT:loadHelperFrame2(this.value)","6"%>
														  </font></td>
													<%End if
													if UCase(LOOKUP_TABLE_DESTINATION) = "BATCHES" then%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%'this field is for display only from an input producer code - the field batches.producer_name doesn't exits.
														ShowInputField dbkey, formgroup, "Batches.Producer_Name", "SCRIPT:checkProdCode(this.value)","30"%>
														</font></td>
													<%else%>
														<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>>
														<%'this field is for display only from an input producer code - the field batches.producer_name doesn't exits.
														ShowInputField dbkey, formgroup, "Temporary_Structures.Producer_Name", "SCRIPT:checkTempProdCode(this.value)","30"%>
														</font></td>
													<%end if%>
												<%end if%>
												<%'end producer code lookup%>
											
											<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
												<%if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then%>
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Notebook_Number", Notebooks_List, Notebooks_Val,Notebooks_Text,0,true,"value","0" %>
													</td>
												<%else%>                                                
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Notebook_Text",   setDisplayType("Notebook_Text"),"25"%>
													</font></td>
												<%end if%>
										<%end if%>	
												
												<%if not checkHideField("NOTEBOOK_PAGE") then%>
													<td  valign="top" <%=td_bgcolor%>><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "Temporary_Structures.Notebook_Page",   setDisplayType("Notebook_Page"),"10"%></font>
													</td>
												<%end if%>
										

											<%if not checkHideField("AMOUNT") then%>
												<td valign="top" <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Amount",    setDisplayType("Amount"),"15"%></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT_UNITS") then%>
												<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.AMOUNT_UNITS",   setDisplayType("AMOUNT_UNITS"),"15"%></font>
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
												<table border="0" width="650">
													<%if not checkHideField("LIT_REF") then%>
														<tr>
															<td <%=td_caption_bgcolor%> colspan="3" ><font <%=font_default%>><b><%=getLabelName("LIT_REF")%></b></font>
															</td>
														</tr>

														<tr>
															<td colspan="3" <%=td_bgcolor%>  ><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Lit_Ref",     setDisplayType("Lit_Ref"), "72:2000"%></font>
															</td>
														</tr>
													<%end if%>

													<tr>
														<%if not checkHideField("SOURCE") then%>
															<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>>Source</font></strong>
															</td>
														<%end if%>
                                                 
														<%if not checkHideField("VENDOR_NAME") then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_NAME_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("VENDOR_NAME")%></font></strong></div>
															</td>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_ID_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("VENDOR_ID")%></font></strong></div>
															</td>
														<%end if%>
												
													</tr>
 
													<tr>
														<%if not checkHideField("SOURCE") then%>
															<font <%=font_default%>>
																<td nowrap  <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SOURCE",  setDisplayType("SOURCE"),"30:200"%>
																</td>
															</font>
														<%end if%>
                                                
														<%if not checkHideField("VENDOR_NAME")  then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_NAME"><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.VENDOR_NAME",  setDisplayType("VENDOR_NAME"),"30:200"%></div>
																</td>
															</font>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_ID"><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.VENDOR_ID", setDisplayType("VENDOR_ID"),"10:100"%></div>
																</td>
															</font>
														<%end if%>
													</tr>
												</table>
											</td>
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
																<%ShowInputField dbkey, formgroup,  "Temporary_Structures.Batch_Comment",  setDisplayType("Batch_Comment"), "72:2000"%>
															</font></td>
															
														</tr>
													<%end if%>
													
													<%if not checkHideField("PREPARATION") then%>
														<tr>
															<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("PREPARATION")%></b></font>
															</td>
														</tr>
														
														<tr>
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Preparation",   setDisplayType("Preparation"), "72:2000"%></font>
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
															<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Storage_Req_And_Warnings",   setDisplayType("Storage_Req_And_Warnings"), "72:2000"%>
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
														<%if not checkHideField("Temporary_Structures.H1NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.H1NMR")%></strong></font>   
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.H1NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.H1NMR")%> >  <font <%=font_default%>><strong><%ShowInputField dbkey, formgroup, "Temporary_Structures.H1NMR",setDisplayType("H1NMR"),"15:200"%></font>   
															</td>
														<%end if%>
                                              
														<%if not checkHideField("Temporary_Structures.MP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.MP")%></font></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.MP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.MP")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.MP",  setDisplayType("MP"),"15:200"%></font>
															</td>
														<%end if%>
													
													</tr>
													<tr>
														<%if not checkHideField("Temporary_Structures.C13NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.C13NMR")%></strong></font>
															</td>
														<%end if%>	  
														
														<%if not checkHideField("Temporary_Structures.C13NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.C13NMR")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.C13NMR",setDisplayType("C13NMR"),"15:200"%>   </font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Temporary_Structures.BP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.BP")%></font></strong></font>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.BP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.BP")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.BP",  setDisplayType("BP"),"15:200"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.HPLC") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.HPLC")%></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Temporary_Structures.HPLC") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.HPLC")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.HPLC",setDisplayType("HPLC"),"15:200"%>   </font>
															</td>
														<%end if%>	
                                              
														<%if not checkHideField("Temporary_Structures.SOLUBILITY") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.SOLUBILITY")%></font></strong></font>
															</td>
														<%end if%>	 
														
														<%if not checkHideField("Temporary_Structures.SOLUBILITY") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.SOLUBILITY")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.SOLUBILITY",  setDisplayType("SOLUBILITY"),"15:200"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.MS")%></strong></font>
														</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.MS")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.MS",     setDisplayType("MS"),"15:200"%></font>
															</td>
														<%end if%>		
                                               
                                              
														<%if not checkHideField("Temporary_Structures.Optical_Rotation") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.Optical_Rotation")%></font></strong></font>
															</td>
														<%end if%>	
															
														<%if not checkHideField("Temporary_Structures.Optical_Rotation") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Optical_Rotation")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Optical_Rotation",    setDisplayType("Optical_Rotation"),"15:200"%></font>
															</td>
														<%end if%>		
													</tr>

													<tr>
														<%if not checkHideField("Temporary_Structures.LC_UV_MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.LC_UV_MS")%></font></strong>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Temporary_Structures.LC_UV_MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.LC_UV_MS")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.LC_UV_MS",  setDisplayType("LC_UV_MS"),"15:200"%></font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Temporary_Structures.Physical_Form") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.Physical_Form")%></font></strong></font>
															</td>
														<%end if%> 
														
														<%if not checkHideField("Temporary_Structures.Physical_Form") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Physical_Form")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Physical_Form",  setDisplayType("Physical_Form"),"15:200"%></font>
															</td>
														<%end if%>
                                                </tr>

                                                <tr>
													<%if not checkHideField("Temporary_Structures.IR") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Temporary_Structures.IR")%></font></strong>
														</td>
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.IR") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.IR")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.IR",   setDisplayType("IR"),"15:200"%></font>
														</td>
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.LogD") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.LogD")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.LogD") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.LogD")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.LogD",  setDisplayType("LogD"),"15:200"%></font>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.UV_SPECTRUM")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.UV_SPECTRUM",  setDisplayType("UV_SPECTRUM"),"15:200"%></font>
														</td>
													<%end if%>
                                     
													<%if not checkHideField("Temporary_Structures.Refractive_Index") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>><font <%=font_default_caption%>> <strong> <%=getLabelName("Refractive_Index")%></strong></font>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Refractive_Index") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Refractive_Index")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Refractive_Index",  setDisplayType("Refractive_Index"),"15:200"%></font>
														</td>
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Temporary_Structures.GC") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.GC")%></font></strong>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.GC") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.GC")%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.GC",  setDisplayType("GC"),"15:200"%></font>
														</td>
													<%end if%>

													<%if not checkHideField("Temporary_Structures.FlashPoint") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.FlashPoint")%></strong>
														</font></strong>
														</td> 
													 <%end if%>

													<%if not checkHideField("Temporary_Structures.FlashPoint") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.FlashPoint")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.FlashPoint",  setDisplayType("FlashPoint"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.CHN_COMBUSTION")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.CHN_COMBUSTION",  setDisplayType("CHN_COMBUSTION"),"15:200"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Color") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Color")%></strong>
														</font></strong>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Color") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Color_1")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Color",  setDisplayType("Color"),"15:200"%>
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
															<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_1")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_1",  setDisplayType("Field_1"),"15:200"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.Field_2") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_2")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_2",  setDisplayType("field_2"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_3")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_3",  setDisplayType("Field_3"),"15:200"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_4") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_4")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_4",  setDisplayType("Field_4"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_5")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_5",  setDisplayType("Field_5"),"15:200"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Temporary_Structures.Field_6") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_6")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_6",  setDisplayType("Field_6"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_7")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_7",  setDisplayType("Field_7"),"15:200"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Temporary_Structures.Field_8") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_8")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_8") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_8")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_8",  setDisplayType("Field_8"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Temporary_Structures.Field_9")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_9",  setDisplayType("Field_9"),"15:200"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Temporary_Structures.Field_10") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Temporary_Structures.Field_10")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Temporary_Structures.Field_10") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Temporary_Structures.Field_10")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Field_10",  setDisplayType("Field_10"),"15:200"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_1",  setDisplayType("INT_BATCH_FIELD_1"),"15:10"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_2",  setDisplayType("INT_BATCH_FIELD_2"),"15:10"%>
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
														<td   <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_3",  setDisplayType("INT_BATCH_FIELD_3"),"15:10"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_4",  setDisplayType("INT_BATCH_FIELD_4"),"15:10"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_5",  setDisplayType("INT_BATCH_FIELD_5"),"15:10"%>
														</font>
														</td> 
													<%end if%>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6") then%>                           
														<td  width = "150"  <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_6")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_BATCH_FIELD_6",  setDisplayType("INT_BATCH_FIELD_6"),"15:10"%>
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
														<td   width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_1",  setDisplayType("REAL_BATCH_FIELD_1"),"15:8"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_2",  setDisplayType("REAL_BATCH_FIELD_2"),"15:8"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_3",  setDisplayType("REAL_BATCH_FIELD_3"),"15:8"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_4",  setDisplayType("REAL_BATCH_FIELD_4"),"15:8"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_5",  setDisplayType("REAL_BATCH_FIELD_5"),"15:8"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6") then%>                           
														<td width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_BATCH_FIELD_6",  setDisplayType("REAL_BATCH_FIELD_6"),"15:8"%>
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
														<td   width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_1")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_1",  setDisplayType("DATE_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_2",  setDisplayType("DATE_BATCH_FIELD_2"),"15"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_3",  setDisplayType("DATE_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4") then%>                           
														<td   "width = "150"  <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_4",  setDisplayType("DATE_BATCH_FIELD_4"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_5",  setDisplayType("DATE_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_BATCH_FIELD_6",  setDisplayType("DATE_BATCH_FIELD_6"),"15"%>
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
 
 <%'end batch info%> 
<%CloseConn(DataConn)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->

</body>

</html>