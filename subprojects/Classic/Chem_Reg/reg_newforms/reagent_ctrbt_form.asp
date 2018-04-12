<%@ LANGUAGE=VBScript %>
<%	
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
Response.expires=0
Response.Buffer = true
dbkey="reg"

if Not Session("UserValidated" & dbkey) = 1 then  
	response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if
%>

<html>
<head>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->


<%
'record_added=Request.QueryString("record_added")
commit_type = "full_commit"
formmode = Request("formmode")
%>

<script language="javascript">
	MainWindow.commit_type = "<%=commit_type%>"
</script>

<title>Chemical Registration - Add Reagents Input Form</title>

<%	
on error resume next
Set DataConn = GetNewConnection(dbkey, formgroup, "base_connection")
if DataConn.State=0 then ' assume user has been logged out
	DoLoggedOutMsg()
end if

if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
	Notebooks_List= GetUserNotebookList(DataConn, UCase(Session("CurrentUser" & dbkey)))
else
	default_chemist_id = getValueFromTablewConn(DataConn, "People", "user_id", UCase(Session("CurrentUser" & dbkey)), "Person_id")
end if

Salts_List= GetSaltsList(DataConn)
Salts_Batch_List = GetSaltsListBatch(DataConn)
Solvates_List= GetSolvatesList(DataConn)
People_List = GetPeopleList(DataConn)	
Projects_List= GetProjectsList(DataConn)
Batch_Projects_List= GetBatchProjectsList(DataConn)
Compound_Type_List= GetCompoundTypeList(DataConn)
sequences_list = GetSequenceList(DataConn)
	
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
default_source_value = the_source_array(0)
	
	
DBMSUser_ID = Session("CurrentUser" & dbkey)
if Application("REAGENT_SEQUENCE") <> "" then
	default_prefix_id = getValueFromTablewConn(DataConn, "Sequence", "Prefix", Application("REAGENT_SEQUENCE"), "Sequence_ID")
else
	default_prefix_id =""
end if

entry_person_ID = getValueFromTablewConn(DataConn, "People", "User_ID", DBMSUser_ID, "Person_ID")
entry_person_location = getValueFromTablewConn(DataConn, "People", "Person_ID", entry_person_ID, "Site_ID")
'use only if you want to tie the prefix to the site and restrict access in this manner
'if Not Session("SITE_ACCESS_ALL" & dbkey) = True then
	'internal_site_code = getValueFromTablewConn(DataConn, "Sites", "Site_ID", entry_person_location, "Site_Code")
	'if Application("BuildSiteID") <> "" then
		'Prefix = UCase(Application("BuildSiteID") & internal_site_code)
	'else
		'Prefix = UCase(internal_site_code)
	'end if
	'SequenceID = getValueFromTablewConn(DataConn, "Sequence", "Prefix", Prefix, "Sequence_ID")
	'UserValidSequenceID= SequenceID

'end if
cmpd_output="raw"
ident_output = "raw"
batch_output = "raw"
%>
</head>

<body <%=default_body%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%if record_added = "true" then%>
	<script language="javascript">
			//alert("Your record was added to the temporary table")
	</script>
<%end if%>

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

<% if CBOOL(Application("PROJECTS_USED")) = False then%>
<input type = "hidden" name = "Temporary_Structures.Project_ID" value ="1">
<%end if%>

<% if CBOOL(Application("BATCH_PROJECTS_USED")) = False then%>
<input type = "hidden" name = "Temporary_Structures.Batch_Project_ID" value ="1">
<%end if%>

<% if CBOOL(Application("NOTEBOOK_USED")) = False then
	if  CBOOL(Application("NOTEBOOK_LOOKUP")) = FALSE then%>
		<input type = "hidden" name = "Temporary_Structures.Notebook_Number" value ="1">
		<input type = "hidden" name = "Temporary_Structures.Scientist_ID" value ="<%=default_chemist_id%>">
	<%else%>
		<input type = "hidden" name = "Temporary_Structures.Scientist_ID" value ="1">
	<%end if%>
<%end if%>


<input type = "hidden" name = "Temporary_Structures.BASE64_CDX" value = "">
<input type = "hidden" name = "orig_required_fields" value="<%=GetFormGroupVal(dbkey, formgroup, kRequiredFields)%>">

<%'chemreg header information%>
<table border="0">
	<tr>
		<td width="40%" align="left" nowrap>
			<p align="left"><strong><font face="Arial" size="3" color="#182889">New	Compound Submission Form</font></strong></p>
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

<%'start compound information%>
                
<table <%=table_main_L1%> width = "650">
	<tr>
		<td >
			<table>
				<tr>
				  <td colspan="2" width="100%">
				  </td>
				</tr>
				
				<tr>
					<table>
						<tr>
							<td align="left">
							    <table border="0" >
									<tr>
										<td valign="top" align="left"><%ShowStrucInputField dbkey, formgroup, "Temporary_Structures.Structure", "1", 320, 200, "", "SelectList"%>
										</td>
										
										<td valign="top" align="left">
											<table width = "330">
												<tr>
													<td colspan="2" <%=td_header_bgcolor%>><strong><font <%=font_header_default_2%> face="Arial">Compound Information</font></strong></td>
												</tr>

												
												
												<%if CBOOL(Application("PROJECTS_USED")) = True then%><tr>
													<%if  Not CBOOL(Application("PROJECTS_NAMED_OWNER")) = true then%>
														<td <%=td_caption_bgcolor%> width="160" align="right"><font <%=font_default_caption%>><b><%=getLabelName("Project_ID")%></b></font></td>
														<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Temporary_Structures.Project_ID", Projects_List, Projects_Val,Projects_Text,0,false,"value","1" %></font></td>
													<%end if%>
													</tr>
												<%end if%>
                            
												<%if not checkHideField("Sequence_ID") then%>
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
															
															<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.COLLABORATOR_ID", setDisplayType("COLLABORATOR_ID"),"15"%></font>
															</td>
														</tr>
												<%end if%>
											
												<%if not checkHideField("PRODUCT_TYPE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("PRODUCT_TYPE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.PRODUCT_TYPE", setDisplayType("PRODUCT_TYPE"),"15"%>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
													    </td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.FEMA_GRAS_Number", setDisplayType("FEMA_GRAS_NUMBER"),"15"%></font>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("GROUP_CODE") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Group_Code", setDisplayType("GROUP_CODE"),"17"%></font>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("RNO_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%>align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%></b></font>
														</td>
														
														<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.RNO_Number", setDisplayType("RNO_NUMBER"),"17"%></font>
														</td>
													</tr>
												<%end if%>
												
												<%if not checkHideField("CAS_NUMBER") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.CAS_Number", setDisplayType("CAS_NUMBER"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("MW") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font>
													    </td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.MolWeight", setDisplayType("MW"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("FORMULA") then%>
													<tr>
														<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("Formula")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Formula", setDisplayType("Formula"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("CHIRAL") then%>
												    <tr>
														<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CHIRAL")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.CHIRAL", setDisplayType("CHIRAL"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("CLogP") then%>
													<tr>
														<td <%=td_caption_bgcolor%>align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CLogP")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.CLogP",  setDisplayType("CLogP"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%
												if not checkHideField("H_BOND_DONORS") then%>
													<tr>
														<td  <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_DONORS")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.H_BOND_DONORS", setDisplayType("H_BOND_DONORS"),"15"%></font>
														</td>
													</tr>
												<%end if%>

												<%if not checkHideField("H_BOND_ACCEPTORS") then%>
													<tr>
														<td <%=td_caption_bgcolor%>  align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_ACCEPTORS")%></b></font>
														</td>
														
														<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.H_BOND_ACCEPTORS",setDisplayType("H_BOND_ACCEPTORS"),"15"%></font>
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
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_1",  setDisplayType("TXT_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_2",  setDisplayType("TXT_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_3",  setDisplayType("TXT_CMPD_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.TXT_CMPD_FIELD_4",  setDisplayType("TXT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												           
                                                 <tr>
													                                             
                                             		<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1") then%>
														<td align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1") then%>                           
														<td  <%=td_bgcolor_c1%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_CMPD_FIELD_1",  setDisplayType("INT_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2") then%>
														<td align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_CMPD_FIELD_2",  setDisplayType("INT_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_CMPD_FIELD_3",  setDisplayType("INT_CMPD_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.INT_CMPD_FIELD_4",  setDisplayType("INT_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
												</tr>	
												
                   
												
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1") then%>
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_1",  setDisplayType("REAL_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_2",  setDisplayType("REAL_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3") then%>                           
														<td <%=td_bgcolor_c1%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_3",  setDisplayType("REAL_CMPD_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.REAL_CMPD_FIELD_4",  setDisplayType("REAL_CMPD_FIELD_4"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
												                                                
                                                <tr>
                                                <%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1") then%>
														<td  align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1") then%>                           
														<td   <%=td_bgcolor_c1%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_1",  setDisplayType("DATE_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2") then%>
														<td  align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2") then%>                           
														<td  <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_2",  setDisplayType("DATE_CMPD_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3") then%>
														<td  align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3") then%>                           
														<td  <%=td_bgcolor_c1%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_3",  setDisplayType("DATE_CMPD_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4") then%>
														<td align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4") then%>                           
														<td   <%=td_bgcolor_c2%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "TEMPORARY_STRUCTURES.DATE_CMPD_FIELD_4",  setDisplayType("DATE_CMPD_FIELD_4"),"15"%>
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
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MW_Text")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.MW_Text", setDisplayType("MW_Text"),"100"%></font>
										</td>
									</tr>
								<%end if %>
                   
								<%if not checkHideField("MF_TEXT") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_Text")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.MF_Text", setDisplayType("MF_Text"),"100"%></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("compound_types_used")) = True then%>
                   
									<tr>
										<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Compound_Type")%></b></font>
										</td>
									</tr>
									
									<tr>
										<td<%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Temporary_Structures.Compound_Type",Compound_Type_List, "","",0,false,"value:validate:checkCompoundType(this.name)","1" %></font>
										</td>
									</tr>
								<%end if %>
								
								<%if CBool(application("STRUCTURE_COMMENTS_TEXT")) = True then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("structure_comments_txt")%></b></font>
										</td>
									</tr>

									<tr>
									    <td  <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.STRUCTURE_COMMENTS_TXT", setDisplayType("STRUCTURE_COMMENTS_TXT"),"100"%></font>
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
											
											<font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Chemical_Name", setDisplayType("CHEMICAL_NAME"), "75"%></font>
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
										<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%><%=add_gen_text%></b></font>
										</td>
									</tr>
								  
									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>>
											<p>
											<%ShowInputField dbkey,formgroup,  "TEMPORARY_STRUCTURES.CHEM_NAME_AUTOGEN",setDisplayType("CHEM_NAME_AUTOGEN"), "75"%></font>
										</td>
									</tr>
								<%end if%>

								<%if not checkHideField("Synonym_R") then%>
									<tr>
										<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_R")%></font></strong><br>
										</td>
									</tr>

									<tr>
										<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey,formgroup, "Temporary_Structures.Synonym_R", setDisplayType("Synonym_R"), "75"%></font>
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
<%'start salt information%>
<%if ( UCase(Application("Batch_Level")) = "SALT" and CBOOL(Application("SALTS_USED")) = true) then%>
	<table <%=table_main_L1%> width = "695">
		<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>New Compound Salt Information</font></strong>
			</td>
		</tr>
		
		<tr>
			<td  <%=td_bgcolor%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Temporary_Structures.Salt_Code", Salts_List, Salts_Val,Salts_Text,0,false,"value","1" %>
			</td>
		</tr>
		</table>

  <%end if%>
<%'end salt information%>

  <!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
  <%CloseConn(DataConn)%>

</body>