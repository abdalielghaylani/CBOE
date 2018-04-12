<%@ LANGUAGE=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Response.expires=0
Response.Buffer = true

dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
on error resume next
%>

<html>
<head>
	<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:HOVER {Text-Decoration: underline; color:#182889; font-size:8pt; font-family: verdana}
	</style>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<script language = "javascript">
	var db_record_count = "<%=DBRecordCount%>"
	//db_record_count = get_db_record_count("reg_numbers")
	
	
	// DGB 04/2006 CHANGE: Implement Batch Searching
	// Posts the form when a tab is clicked
	function SwitchTab(sTab, formgroup) {
		var url = "reg_input_form.asp?TB=" + sTab + "&formgroup=" + formgroup + "&formmode=search&dbname=reg"; 
		//alert(url);
		this.location.href = url;
	}
	// DGB 04/2006 END CHANGE: Implement Batch Searching
</script>

<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->

<title>Registration Enterprise - Registered Compounds Search Input Form</title>
<%
Set DataConn = GetNewConnection(dbkey, formgroup, "base_connection")
if DataConn.State=0 then ' assume user has been logged out
	DoLoggedOutMsg()
end if
'UpdateBaseRecordCount dbkey, formgroup, DataConn
%>



<%
if CBOOL(Application("NOTEBOOK_LOOKUP")) = True then
	Notebooks_List= GetNotebookList(DataConn)
end if

Salts_List= GetSaltsList(DataConn)
Salts_Batch_List = GetSaltsListBatch(DataConn)
Solvates_List = GetSolvatesList(DataConn)
Projects_List= GetProjectsList(DataConn)
Batch_Projects_List = GetBatchProjectsList(DataConn)
Compound_Type_List= GetCompoundTypeList(DataConn)
Sequences_List = GetSequenceList(DataConn)
People_List = GetPeopleList(DataConn)			

DBMSUser_ID = Session("CurrentUser" & dbkey)
current_person_ID = getValueFromTablewConn(DataConn, "People", "User_ID", DBMSUser_ID, "Person_ID")
People_Val = current_person_ID
person_id = current_person_ID
user_name = getPersonDisplayName(dbkey, formgroup, person_id ,DataConn)
People_Text =user_name
yes_no_list = "0:FALSE,1:TRUE"
	
blank_entry = ":,"

%>
</head>

<body <%=default_body%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<% if UCase(getbasetable(dbkey, formgroup, "basetable")) = "BATCHES" then%>
<input type = "hidden" name = "order_by" value = "Batches.Batch_Internal_ID ASC">
<%else%>
<input type = "hidden" name = "order_by" value = "REG_NUMBERS.REG_NUMBER ASC">
<%end if
'SEARCH and ADD Record Mode support for producer and user lookups when batch is the basetable
if Application("USER_LOOKUP") = 1  or Application("PRODUCER_LOOKUP") = 1 then
	LOOKUP_TABLE_DESTINATION = "Batches" 'This is case sensitive. Must be Batches or Temporary_Structures
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
'end producer and user lookup
'chemreg header information%>
<table border="0">
	<tr>
		<td width="40%" align="left" nowrap>
			<strong><font face="Arial" size="3" color="#182889">Search Registry</font></strong>
		</td>

		<td width="60%" align="right" nowrap>
			<font face="Arial" size="4" color="#182889"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%=Application("display_appkey")& "&nbsp;"%></b></font> 
			<font face="Arial" size="4" color="#182889"><b>Registration Enterprise</b></font>
		</td>
	</tr>
</table>
                
<%'end chemreg header information%>	
    <!--#INCLUDE VIRTUAL = "/chem_reg/reg/SearchFormTabs.asp"-->         
<input type="hidden" name="Reg_Numbers.Reg_ID" value=">0">
<table <%=table_main_L1%>>
	<tr>
		<td>
			<%'start compound information%>
			<table width = "668">
				<tr>
					<table>
						<tr>
							<td align="left">
								<%'startreg input extras%>
								<table border="0" align="left" >
									<tr>
										<td  align="right">
											<table border="0" align="left" >
												<tr>
													<td align="left" valign="top">
														<table border="0">
															<tr><%if not checkHideField("Reg_Number") then%>
																<td <%=td_caption_bgcolor%> valign="top" align="left" width="220" nowrap>
																	<b><font <%=font_default_caption%> size="2"><%=getLabelName("Reg_Number")%>
																	 </font><font <%=font_default_small%> >                                       
																	(i.e. <%=GetExample(dbkey, formgroup,"Reg_Num")%>)</font></b>
																</td>
																<%end if%>
																<%if not checkHideField("Sequence_Number") then%>
																<td <%=td_caption_bgcolor%> valign="top" align="left" width="200" nowrap>
																	<b><font <%=font_default_caption%> size="2"><%=getLabelName("Sequence_Number")%>
																	</font><font <%=font_default_small%> >																	 ( i.e. 1 or >1)     
																</font></td> 
																  <%end if%>                                </font></b>                                              </td>
																<%if not checkHideField("Approved") then%>
																<%if Application("Approved_flag_used") = 1 then%>
																	<td <%=td_caption_bgcolor%> valign="top" align="left" width="100"><font <%=font_default_caption%> size="2"><b><%=getLabelName("Approved")%><br></b></font>
																	</td> 
																<%end if%>
																<%end if%>	 
																<%if not checkHideField("Quality_Checked") then%>
																<%if Application("Quality_Checked_flag_used") = 1 then%>      
																	<td <%=td_caption_bgcolor%> valign="top" align="left" width="100"><font <%=font_default_caption%> size="2"><b><%=getLabelName("Quality_Checked")%><br> </b></font>
																	</td>
																<%end if%>
																<%end if%>	 
															</tr>

															<tr><%if not checkHideField("Reg_Number") then%>
																<td <%=td_bgcolor%> valign="top" align="left" width="290">
																	<font <%=font_default%>><b><%ShowInputField dbkey, formgroup, "Reg_Numbers.Reg_Number", "0","17"%>
																	<script language="javascript">
																		getOpenFileButton("Reg_Numbers.Reg_Number")
																	</script></font></b></font>
																</td>
																<%end if%>
																<%if not checkHideField("Sequence_Number") then%>
																<td <%=td_bgcolor%> valign="top" align="left" width="200">
																     <b><font <%=font_default%> size="2"><%ShowInputField dbkey, formgroup, "Reg_Numbers.Sequence_Number", setDisplayType("Sequence_Number"),"17"%></font></b>
																<script language="javascript">
																		getOpenFileButton("Reg_Numbers.Sequence_Number")
																	</script></font></b></font></td>
																	<%end if%>
																<%if not checkHideField("Approved") then%>
																<%if Application("Approved_flag_used") = 1 then%>                                     
																	<td <%=td_bgcolor%> valign="top" align="left" width="100"><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Reg_Approved.Approved", yes_no_list, "yes","1",0,true,"value","0" %></font>
																	</td> 
																<%end if%>
																<%end if%>
																<%if not checkHideField("Quality_Checked") then%>
																<%if Application("Quality_Checked_flag_used") = 1 then%>                                   
																	<td  <%=td_bgcolor%> valign="top" align="left" width="100"><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Reg_Quality_Checked.Quality_Checked", yes_no_list, "yes","1",0,true,"value","0" %></font>
																	</td>  
																<%end if%>
																<%end if%>
															</tr>
														</table>
													</td>
												</tr>
			
			                                    <tr>
													<td nowrap align="left" valign="top">  
														<table width = "635">
															<tr><%if not checkHideField("Registry_Date") then%>
																<td  <%=td_caption_bgcolor%>valign="top" align="left" width="200" nowrap><font <%=font_default%> size="2"><b><%=getLabelName("Registry_Date")%></font><font <%=font_default_small%>>
																<%'SYAN added 1/7/2003 to fix CSBR-40197%>
																
																<%Select case Application("DATE_FORMAT")%>
																	<%case "8"%>
																		(mm/dd/yyyy)
																	<%case "9"%>
																		(dd/mm/yyyy)
																	<%case "10"%>
																		(yyyy/mm/dd)
																	<%case else%>
																		(mm/dd/yyyy)
																<%end select%>
																<%'End SYAN changes%>
																
																	</b></font>
																</td>
																<%end if%>
																<%if not checkHideField("Registrar_Person_ID") then%>
																<td  <%=td_caption_bgcolor%>valign="top" align="left" width="200"><b><font <%=font_default_caption%> size="2"><%=getLabelName("Registrar_Person_ID")%>
																	<br>
																	</font></b>
																</td>
																<%end if%>
																<%if not checkHideField("Last_Mod_Person_ID") then%>
																<td  <%=td_caption_bgcolor%>valign="top" align="left" width="200"><font <%=font_default_caption%> size="2"><b><%=getLabelName("Last_Mod_Person_ID")%>
																	 <br></b></font>
																</td>
																<%end if%>
															</tr>
                                          
															<tr>
																<%if not checkHideField("Registry_Date") then%>
																<td <%=td_bgcolor%> valign="top" align="left" width="200"><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Reg_Numbers.Registry_Date", "DATE_PICKER:8","17"%></font>
																</td>
																<%end if%>
																<%if not checkHideField("Registrar_Person_ID") then%>
																<td <%=td_bgcolor%> valign="top" align="left" width="200"><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Reg_Numbers.Registrar_Person_ID", People_List,  "","",0,true,"value","0" %></font>
																</td>
																<%end if%>
																<%if not checkHideField("Last_Mod_Person_ID") then%>
																<td <%=td_bgcolor%> valign="top" align="left" width="200"><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Compound_Molecule.Last_Mod_Person_ID", People_List, People_Text,People_Val,0,true,"value","0" %></font>
																</td>
																<%end if%>
															</tr>
														</table>
													</td>
												</tr>
											</table>
										</td>
									</tr>
								</table>
								
								<%'reg input extras%>
								
								<tr>
									<td>
										<table border="0" >
											<tr>
												<td valign="top" align="left"><%ShowStrucInputField dbkey, formgroup, "Structures.Structure", "5","310","200", "AllOptions", "SelectList"%>
												</td>
 
												<td valign="top" align="left">
													<table width = "330">
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
														
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,Compound_ProjectRS,"Projects.Project_Internal_ID", Projects_List, Projects_Val,Projects_Text,0,true,"value","0" %></font>
															</td>
														</tr>
														<%end if%>
														
														<%if not checkHideField("Sequence_Internal_ID") then%>
															<tr>
                        										<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("Sequence_Internal_ID")%></b></font>
                        										</td>						                            
																<td <%=td_bgcolor%> width="160"><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Compound_Molecule.Sequence_Internal_ID", Sequences_List, Sequences_Val,Sequences_Text,0,true,"value","0" %></font>
																</td>
																
															</tr>
														<%end if%>

                           
														<%if not checkHideField("COLLABORATOR_ID") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("COLLABORATOR_ID")%></b></font>
															    </td>
																
																<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&7",setDisplayType("COLLABORATOR_ID"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														 
														<%if not checkHideField("PRODUCT_TYPE") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("PRODUCT_TYPE")%></b></font>
																</td>
														
																<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "COMPOUND_MOLECULE.PRODUCT_TYPE", setDisplayType("COMPOUND_MOLECULE.PRODUCT_TYPE"),"15"%>
																</td>
														</tr>
														<%end if%>
														
														
														
														
														
														<%if not checkHideField("CAS_NUMBER") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CAS_NUMBER")%>
																	</b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&1",   setDisplayType("CAS_NUMBER"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														<%if not checkHideField("RNO_NUMBER") then%>
															<tr>
																<td <%=td_caption_bgcolor%>align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("RNO_NUMBER")%>
																	</b></font>
																</td>
																
																<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Alt_IDs.Identifier:&3",  setDisplayType("RNO_NUMBER"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														<%if not checkHideField("GROUP_CODE") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%>
																	</b></font>
																</td>
																
																<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Alt_IDs.Identifier:&5", setDisplayType("GROUP_CODE"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%>
																	</b></font>
																</td>
																
																<td <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&4", setDisplayType("FEMA_GRAS_NUMBER"),"15"%></font>
																</td>
															</tr>
														<%end if%>
									 
														<%if not checkHideField("MW") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("MW")%>
																	</b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Structures.MolWeight", setDisplayType("MW"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														
														<%if not checkHideField("FORMULA") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170"><font <%=font_default_caption%>><b><%=getLabelName("FORMULA")%>
																	</b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Structures.Formula",  setDisplayType("FORMULA"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														
														<%if not checkHideField("CHIRAL") then%>
														    <tr>
																<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("Chiral")%></b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "COMPOUND_MOLECULE.CHIRAL", setDisplayType("Chiral"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														
														<%if not checkHideField("CLogP") then%>
															<tr>
																<td <%=td_caption_bgcolor%>align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("CLogP")%></b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.CLogP", setDisplayType("CLogP"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														
														<%if not checkHideField("H_BOND_DONORS") then%>
															<tr>
															<td  <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_DONORS")%>
															</b></font>
															</td>
															
															<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "COMPOUND_MOLECULE.H_BOND_DONORS",setDisplayType("H_BOND_DONORS"),"15"%></font>
															</td>
															</tr>
														<%end if%>
														
														<%if not checkHideField("H_BOND_ACCEPTORS") then%>
															<tr>
																<td <%=td_caption_bgcolor%>  align="right" width="170">  <font <%=font_default_caption%>><b><%=getLabelName("H_BOND_ACCEPTORS")%>
																	</b></font>
																</td>
																
																<td  <%=td_bgcolor%> width="160">  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "COMPOUND_MOLECULE.H_BOND_ACCEPTORS",setDisplayType("H_BOND_ACCEPTORS"),"15"%></font>
																</td>
															</tr>
														<%end if%>
														
														<%'start identifiers field%>
														<%if not checkHideField("CAS_NUMBER") then%>
															<tr>
																<td <%=td_caption_bgcolor%> align="right" width="170">  <font <%=font_default_caption%>><b><A href="" onClick="alert('Identifiers are CAS Number, Chemical Name and Synonyms');return false;">All Identifiers</A></b></font>
																</td>
																
																<td <%=td_default%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Alt_IDs.Identifier:&9", "0", "15"%></font>
																</td>
															</tr>
														<%else%>
															<tr>
																<td <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><A href="" onClick="alert('Identifiers  Chemical Name and Synonyms');return false;">All Identifiers</A></b></font>
																</td>
																
																<td <%=td_default%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Alt_IDs.Identifier:&4", "0", "15"%></font>
																</td>
															</tr>
														<%end if%>          
														<%'end identifiers field%>
														<!--SYAN added on 2/14/2005 to fix CSBR-50932-->
															<tr>
																<td bgcolor="#CCCCCC" align="right" width="170">
																	<font  face="Verdana" size="2"><b>SDFile Search:</b></font>
																</td>
																<td width="160">
																	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/sdfileSearch.asp"-->
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
														<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.TXT_CMPD_FIELD_1",  setDisplayType("TXT_CMPD_FIELD_1"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_2") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_2")%>>    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.TXT_CMPD_FIELD_2",  setDisplayType("TXT_CMPD_FIELD_2"),"15"%>
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
															<td  width = "150" <%=setBackGroundDisplay("c1","TXT_CMPD_FIELD_3")%>>    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.TXT_CMPD_FIELD_3",  setDisplayType("TXT_CMPD_FIELD_3"),"15"%>

														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("TXT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.TXT_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","TXT_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.TXT_CMPD_FIELD_4",  setDisplayType("TXT_CMPD_FIELD_4"),"15"%>
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
															<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.INT_CMPD_FIELD_1",  setDisplayType("INT_CMPD_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_2")%>> <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.INT_CMPD_FIELD_2",  setDisplayType("INT_CMPD_FIELD_2"),"15"%>														</font>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","INT_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.INT_CMPD_FIELD_3",  setDisplayType("INT_CMPD_FIELD_3"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.INT_CMPD_FIELD_4") then%>                           
															<td  width = "150" <%=setBackGroundDisplay("c2","INT_CMPD_FIELD_4")%>> <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.INT_CMPD_FIELD_4",  setDisplayType("INT_CMPD_FIELD_4"),"15"%>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_1")%>> <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.REAL_CMPD_FIELD_1",  setDisplayType("REAL_CMPD_FIELD_1"),"15"%>														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.REAL_CMPD_FIELD_2",  setDisplayType("REAL_CMPD_FIELD_2"),"15"%>															</font>
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
														<td  width = "150" <%=setBackGroundDisplay("c1","REAL_CMPD_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.REAL_CMPD_FIELD_3",  setDisplayType("REAL_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.REAL_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_CMPD_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.REAL_CMPD_FIELD_4",  setDisplayType("REAL_CMPD_FIELD_4"),"15"%>															</font>
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
													<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_1")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.DATE_CMPD_FIELD_1",  setDisplayType("DATE_CMPD_FIELD_1"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.DATE_CMPD_FIELD_2",  setDisplayType("DATE_CMPD_FIELD_2"),"15"%>	
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
														<td  width = "150" <%=setBackGroundDisplay("c1","DATE_CMPD_FIELD_3")%>>   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.DATE_CMPD_FIELD_3",  setDisplayType("DATE_CMPD_FIELD_3"),"15"%>	
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_CMPD_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("COMPOUND_MOLECULE.DATE_CMPD_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_CMPD_FIELD_4")%>>   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "COMPOUND_MOLECULE.DATE_CMPD_FIELD_4",  setDisplayType("DATE_CMPD_FIELD_4"),"15"%>	
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
											<td <%=td_bgcolor%>>
												<font <%=font_default%>><%ShowInputField dbkey, formgroup, "Compound_Molecule.MW_Text", setDisplayType("MW_TEXT"),"100"%></font>
												</td>
										</tr>
									<%end if %>
                   
									<%if not checkHideField("MF_TEXT") then%>
										<tr>
											<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("MF_TEXT")%></b></font>
											</td>
										</tr>
									
										<tr>
											<td <%=td_bgcolor%>>
											<font <%=font_default%>><%ShowInputField dbkey, formgroup, "Compound_Molecule.MF_Text",setDisplayType("MF_TEXT"),"100"%></font>
											</td>
										</tr>
									<%end if %>
									
									<%if CBool(application("compound_types_used")) = True then%>
										<tr>
											<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("compound_type")%></b></font>
											</td>
										</tr>
										
										<tr>
											<td	<%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Compound_Molecule.Compound_Type",Compound_Type_List, "","",0,true,"value","0" %></font>
											</td>
										</tr>
									<%end if %>
									
									<%if CBool(application("structure_comments_text")) = True then%>
										<tr>
											<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("STRUCTURE_COMMENTS_TXT")%></b></font>
											</td>
										</tr>
										
										<tr>
											<td  <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "COMPOUND_MOLECULE.STRUCTURE_COMMENTS_TXT",setDisplayType("STRUCTURE_COMMENTS_TXT"),"100"%></font>
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
												<font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&0",  setDisplayType("CHEMICAL_NAME"), "75"%></font>
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
											<td  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("CHEM_NAME_AUTOGEN")%> <%=add_gen_text%></b></font>
											</td>
										</tr>

										<tr>
											<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&6", setDisplayType("CHEMICAL_NAME"), "75"%></font>
											</td>
										</tr>
									<%end if%>
									
									<%if not checkHideField("Synonym_R") then%>
										<tr>
											<td  <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Synonym_r")%></font></strong><br>
											</td>
										</tr>

										<tr>
											<td  <%=td_bgcolor%>> <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Alt_IDs.Identifier:&2", setDisplayType("Synonym_r"), "75"%></font>
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
<%if UCase(Application("Batch_Level")) = "SALT" then%>
<table <%=table_main_L1%> width = "680" >
	<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>Salt Information</font></strong>
			</td>
		</tr>
	
	<tr>
		<td  <%=td_bgcolor%>><%ShowLookUpList dbkey, formgroup,BaseRS, "Compound_Salt.Salt_Internal_ID", Salts_List, Salts_Val,Salts_Text,0,true,"value","0" %>
		</td>
	</tr>
</table>
<%end if%>
<%'end salt information%>



<%'start batch info%> 
	<table <%=table_main_L1%> width = "650" >
		<tr>
			<td <%=td_header_bgcolor%>>
				<p align="left"><strong><font <%=font_header_default_1%>>Batch Information</font></strong>
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
												<td <%=td_caption_bgcolor%> width = "200">  <%if not checkHideField("Formula_Weight") then%><font <%=font_default_caption%>><b><%=getLabelName("Formula_Weight")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "200"> <%if not checkHideField("Percent_Active") then%> <font <%=font_default_caption%>><b><%=getLabelName("Percent_Active")%></b><%end if%></font>
												</td>
												
												<td <%=td_caption_bgcolor%> width = "200">  <font <%=font_default_caption%>></font>
												</td>
											</tr>
											
											<tr>
												<%if not checkHideField("Formula_Weight") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>>                                           
														<%ShowInputField dbkey, formgroup,  "BATCHES.FORMULA_WEIGHT", setDisplayType("FORMULA_WEIGHT"),"15"%></font>
													</td>
                                                <%end if%>	                               
                                                
                                                <%if not checkHideField("Percent_Active") then%>
													<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>>
														 <%ShowInputField dbkey, formgroup,  "BATCHES.PERCENT_ACTIVE", setDisplayType("PERCENT_ACTIVE"),"15"%></font>
													 </td>
												<%end if%>	
												
												<td width = "200" <%=td_bgcolor%>><font <%=font_default_caption%>></td>                                                  </font></td>
                                            </tr>
										</table>
									</td>
								</tr>

								<tr>
									<td>
									<%if bShowBatchSalt = true then%>
										<table width="650">
											<tr>
												<td width="200" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_Name")%>&nbsp;</font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_MW")%></font></strong>
												</td>
												
												<td width="150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Salt_Equivalents")%></font></strong>
												</td>
											</tr>

											<tr>
												<td <%=td_bgcolor%>>
													<table>
														<tr><%'SYAN modified on 11/29/2006 to fix CSBR-61618
															'if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                          
															<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Batches.Salt_Name", Salts_Batch_List, "0","", "BATCHES.SALT_NAME","BATCHES.SALT_MW"%>
															</td>
															<%'end if
															'End of SYAN modification
															%> 
															
															<td <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "BATCHES.SALT_NAME",   setDisplayType("SALT_NAME"),"25"%>
															</td>
														</tr>
													</table>
												</td>
												                              
												<td <%=td_bgcolor%> ><%ShowInputField dbkey, formgroup, "BATCHES.SALT_MW",  setDisplayType("SALT_MW"),"12"%>
												</td>
												
												<td <%=td_bgcolor%>> <%ShowInputField dbkey, formgroup, "Batches.Salt_Equivalents",  setDisplayType("Salt_Equivalents"),"12"%>
												</td>
											</tr>
											<% end if
											if bShowBatchSolvate = true then%>
											<tr>
											     <td width="200" <%=td_caption_bgcolor%>> <strong><font <%=font_default_caption%>><%=getLabelName("Solvate_Name")%>&nbsp;</font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Solvate_MW")%></font></strong>
											     </td>
											     
											     <td width = "150" <%=td_caption_bgcolor%> ><strong><font <%=font_default_caption%>><%=getLabelName("Solvate_Equivalents")%></font></strong>
											     </td>
											</tr>
											
											<tr>
												<td <%=td_bgcolor%> > 
													<table>
														<tr><%'SYAN modified on 11/29/2006 to fix CSBR-61618
															'if (UCase(formmode) = "EDIT_RECORD" OR Instr(UCase(formmode),"ADD_")>0) then%>                                 
																<td <%=td_bgcolor%>><%ShowAutoFillLookUpList dbkey, formgroup, BaseRS, "Batches.Solvates_Name", Solvates_list, "0","", "BATCHES.SOLVATE_NAME","BATCHES.SOLVATE_MW"%>
																</td>                                  
															<%'end if
															'End of SYAN modification%>
															
															<td <%=td_bgcolor%>> <%ShowInputField dbkey, formgroup, "BATCHES.SOLVATE_NAME",    setDisplayType("SOLVATE_NAME"),"25"%>
															</td>
														</tr>
													</table>
												</td>
												
												<td <%=td_bgcolor%>>  <%ShowInputField dbkey, formgroup, "BATCHES.SOLVATE_MW",  setDisplayType("SOLVATE_MW"),"12"%>
												</td>
												
												<td <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "Batches.Solvate_Equivalents", setDisplayType("Solvate_Equivalents"),"12"%>
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
							<table width="650" >
								<tr>
									<td>
										<table width="650" >
											<tr>
											<%if not checkHideField("BATCH_REG_PERSON_ID") then%>
											<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("BATCH_REG_PERSON_ID")%> </strong></font>
												</td>
											<%end if%>
											<%if not checkHideField("BATCH_REG_DATE") then%>	
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("BATCH_REG_DATE")%></b></font><font <%=font_default%>>
												</td>
											<%end if%>
											<%if not checkHideField("BATCH_NUMBER") then%>	
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("BATCH_NUMBER")%></b></font><font <%=font_default%>>
												</td>
											<%end if%>
											</tr>
											<tr>
											
											<%if not checkHideField("Batch_Reg_Person_ID") then%>	
											<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Batches.Batch_Reg_Person_ID", People_List, People_Val,People_Text,0,true,"value","0" %></font></font><font  <%=font_default%>></font>
												</td>
											<%end if%>
											<%if not checkHideField("Batch_Reg_Date") then%>	
													<td width = "150" <%=td_bgcolor%>><%ShowInputField dbkey, formgroup,  "Batches.Batch_Reg_Date",   "DATE_PICKER:8","10"%></font>
													</td>
											<%end if%>
											<%if not checkHideField("BATCH_NUMBER") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Batch_Number",   setDisplayType("BATCH_NUMBER"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
											</tr>
											<tr>
												<%'start user lookup labels SEARCH and ADD Mode
												If Application("USER_LOOKUP") = 1 or Application("USER_LOOKUP") = "" then%>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_Code")%></font></b> </td>
													<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></font></b> </td>
												<%else%>
													<%if CBOOL(Application("NOTEBOOK_USED")) = true and  not checkHideField("SCIENTIST_ID")then%>
														<%'if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
														<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Scientist_ID")%></strong>
														</td>
														<%'end if%>
													<%end if%>
												<%end if
												'end user lookup labels%>
												<%if not checkHideField("Purity") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Purity")%> </strong></font>
												</td>
												<%end if%>
												<%if not checkHideField("Appearance") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><strong><%=getLabelName("Appearance")%> </strong></font>
												</td>
												<%end if%>
												<%if not checkHideField("Creation_Date") then%>
												<td width = "150" <%=td_caption_bgcolor%>><font  <%=font_default_caption%>><b><%=getLabelName("Creation_Date")%></b></font><font <%=font_default%>>
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
													<%if CBOOL(Application("NOTEBOOK_USED")) = true then%>
													<%'if CBOOL(Application("NOTEBOOK_LOOKUP")) = False then%>
														<td width = "150" <%=td_bgcolor%> ><%ShowLookUpList dbkey, formgroup,BaseRS,"Batches.Scientist_ID", People_List, People_Val,People_Text,0,true,"value","0" %>
														</td>
													<%'end if%>
													<%end if%>
												<%end if%>
												<%'end scientist name lookup%>
												
												<%if not checkHideField("PURITY") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.PURITY",   setDisplayType("PURITY"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("APPEARANCE") then%>
												<td width = "150" <%=td_bgcolor%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.APPEARANCE",   setDisplayType("APPEARANCE"),"15"%></font><font  <%=font_default%>></font>
												</td>
												<%end if%>
												<%if not checkHideField("Creation_Date") then%>
												<td width = "150" <%=td_bgcolor%>><%ShowInputField dbkey, formgroup,  "Batches.Creation_Date",    "DATE_PICKER:8","10"%></font>
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
												<td width = "200" <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b> &nbsp;<%=getLabelName("Batch_Project_ID")%></b></font>
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
											  	<%ShowLookUpList dbkey, formgroup,BaseRS,"Batches.Batch_Project_ID", Batch_Projects_List, Batch_Projects_Val,Batch_Projects_Text,0,true,"value","0" %>
												</td>
											<%end if%>
											</font>
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
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowLookUpList dbkey, formgroup,BaseRS,"Batches.Notebook_Internal_ID", Notebooks_List, Notebooks_Val,Notebooks_Text,0,true,"value","0" %>
													</td>
												<%else%>                                                
													<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Notebook_Text",   setDisplayType("Notebook_Text"),"25"%>
													</font></td>
												<%end if%>
												
											<%end if%>
											<%if not checkHideField("NOTEBOOK_PAGE") then%>
													<td  valign="top" <%=td_bgcolor%>><font <%=font_default%>>
														<%ShowInputField dbkey, formgroup, "Batches.Notebook_Page",  "0","10"%></font>
													</td>
											<%end if%>

											<%if not checkHideField("AMOUNT") then%>
												<td valign="top" <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Amount",    setDisplayType("Amount"),"15"%></font>
												</td>
											<%end if%>
											
											<%if not checkHideField("AMOUNT_UNITS") then%>
												<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.AMOUNT_UNITS",   setDisplayType("AMOUNT_UNITS"),"15"%></font>
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
															<td colspan="3" <%=td_bgcolor%>  ><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.Lit_Ref",setDisplayType("Lit_Ref"), "72"%></font>
															</td>
														</tr>
													<%end if%>

													<tr>
														<%if not checkHideField("SOURCE") then%>
															<td <%=td_caption_bgcolor%>><strong><font <%=font_default_caption%>><%=getLabelName("Source")%></font></strong>
															</td>
														<%end if%>
                                                 
														<%if not checkHideField("VENDOR_NAME") then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_NAME_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("Vendor_Name")%></font></strong></div>
															</td>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<td <%=td_caption_bgcolor%>><div id ="VENDOR_ID_LABEL"><strong><font <%=font_default_caption%>><%=getLabelName("Vendor_ID")%></font></strong></div>
															</td>
														<%end if%>
												
													</tr>
 
													<tr>
														<%if not checkHideField("SOURCE") then%>
															<font <%=font_default%>>
																<td nowrap  <%=td_bgcolor%>><%ShowInputField dbkey, formgroup, "BATCHES.SOURCE",  setDisplayType("SOURCE"),"15"%>
																</td>
															</font>
														<%end if%>
                                                
														<%if not checkHideField("VENDOR_NAME")  then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_NAME"><%ShowInputField dbkey, formgroup, "BATCHES.VENDOR_NAME",  setDisplayType("VENDOR_NAME"),"15"%></div>
																</td>
															</font>
														<%end if%>

														<%if not checkHideField("VENDOR_ID") then%>
															<font <%=font_default%>>
																<td nowrap <%=td_bgcolor%>><div id ="VENDOR_ID"><%ShowInputField dbkey, formgroup, "BATCHES.VENDOR_ID", setDisplayType("VENDOR_ID"),"10"%></div>
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
															<td  <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Batch_Comment")%></b>
																</font>
															</td>
														</tr>

														<tr>
															<td  <%=td_bgcolor%>><font <%=font_default%>>
																<%ShowInputField dbkey, formgroup,  "Batches.Batch_Comment",  setDisplayType("Batch_Comment"), "72" %>
															</font></td>
															
														</tr>
													<%end if%>
													
													<%if not checkHideField("PREPARATION") then%>
														<tr>
															<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Preparation")%></b></font>
															</td>
														</tr>
														
														<tr>
															<td <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.Preparation",   setDisplayType("Preparation"), "72"%></font>
															</td>
														</tr>
													<%end if%>
											
													<%if not checkHideField("STORAGE_REQ_AND_WARNINGS") then%>
														<tr>
															<td <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><b><%=getLabelName("Storage_Req_And_Warnings")%></b>
																</font>
															</td>
														</tr>
														
														<tr>
															<td  <%=td_bgcolor%>><font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.Storage_Req_And_Warnings",   setDisplayType("Storage_Req_And_Warnings"), "72"%>
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
															<td width="150" <%=setBackGroundDisplay("c1","Batches.H1NMR")%> >  <font <%=font_default%>><strong><%ShowInputField dbkey, formgroup, "Batches.H1NMR",setDisplayType("H1NMR"),"15"%></font>   
															</td>
														<%end if%>
                                              
														<%if not checkHideField("Batches.MP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.MP")%></font></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Batches.MP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.MP")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.MP",  setDisplayType("MP"),"15"%></font>
															</td>
														<%end if%>
													
													</tr>
													<tr>
														<%if not checkHideField("Batches.C13NMR") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.C13NMR")%></strong></font>
															</td>
														<%end if%>	  
														
														<%if not checkHideField("Batches.C13NMR") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.C13NMR")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.C13NMR",setDisplayType("C13NMR"),"15"%>   </font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Batches.BP") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.BP")%></font></strong></font>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.BP") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.BP")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.BP",  setDisplayType("BP"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Batches.HPLC") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.HPLC")%></strong></font>
															</td>
														<%end if%>
														
														<%if not checkHideField("Batches.HPLC") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.HPLC")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.HPLC",setDisplayType("HPLC"),"15"%>   </font>
															</td>
														<%end if%>	
                                              
														<%if not checkHideField("Batches.SOLUBILITY") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.SOLUBILITY")%></font></strong></font>
															</td>
														<%end if%>	 
														
														<%if not checkHideField("Batches.SOLUBILITY") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.SOLUBILITY")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.SOLUBILITY",  setDisplayType("SOLUBILITY"),"15"%></font>
															</td>
														<%end if%>	
													</tr>

													<tr>
														<%if not checkHideField("Batches.MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.MS")%></strong></font>
														</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.MS")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.MS",     setDisplayType("MS"),"15"%></font>
															</td>
														<%end if%>		
                                               
                                              
														<%if not checkHideField("Batches.Optical_Rotation") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.Optical_Rotation")%></font></strong></font>
															</td>
														<%end if%>	
															
														<%if not checkHideField("Batches.Optical_Rotation") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.Optical_Rotation")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.Optical_Rotation",    setDisplayType("Optical_Rotation"),"15"%></font>
															</td>
														<%end if%>		
													</tr>

													<tr>
														<%if not checkHideField("Batches.LC_UV_MS") then%>
															<td width="200" align="right"  <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.LC_UV_MS")%></font></strong>
															</td>
														<%end if%>	
														
														<%if not checkHideField("Batches.LC_UV_MS") then%>
															<td width="150" <%=setBackGroundDisplay("c1","Batches.LC_UV_MS")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.LC_UV_MS",  setDisplayType("LC_UV_MS"),"15"%></font>
															</td>
														<%end if%>	
                                               
														<%if not checkHideField("Batches.Physical_Form") then%>
															<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.Physical_Form")%></font></strong></font>
															</td>
														<%end if%> 
														
														<%if not checkHideField("Batches.Physical_Form") then%>
															<td width="150" <%=setBackGroundDisplay("c2","Batches.Physical_Form")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.Physical_Form",  setDisplayType("Physical_Form"),"15"%></font>
															</td>
														<%end if%>
                                                </tr>

                                                <tr>
													<%if not checkHideField("Batches.IR") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <font <%=font_default_caption%>><strong><%=getLabelName("Batches.IR")%></font></strong>
														</td>
													<%end if%>
													
													<%if not checkHideField("Batches.IR") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Batches.IR")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Batches.IR",   setDisplayType("IR"),"15"%></font>
														</td>
													<%end if%>
                                                
													<%if not checkHideField("Batches.LogD") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.LogD")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.LogD") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Batches.LogD")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.LogD",  setDisplayType("LogD"),"15"%></font>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.UV_SPECTRUM")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.UV_SPECTRUM",  setDisplayType("UV_SPECTRUM"),"15"%></font>
														</td>
													<%end if%>
                                     
													<%if not checkHideField("Batches.Refractive_Index") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>><font <%=font_default_caption%>> <strong> <%=getLabelName("Refractive_Index")%></strong></font>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.Refractive_Index") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Refractive_Index")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Refractive_Index",  setDisplayType("Refractive_Index"),"15"%></font>
														</td>
													<%end if%>
												</tr>	
 
                                                <tr>
													<%if not checkHideField("Batches.GC") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.GC")%></font></strong>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.GC") then%>
														<td width="150" <%=setBackGroundDisplay("c1","Batches.GC")%> ><font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.GC",  setDisplayType("GC"),"15"%></font>
														</td>
													<%end if%>

													<%if not checkHideField("Batches.FlashPoint") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.FlashPoint")%></strong>
														</font></strong>
														</td> 
													 <%end if%>

													<%if not checkHideField("Batches.FlashPoint") then%>
														<td width="150" <%=setBackGroundDisplay("c2","Batches.FlashPoint")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.FlashPoint",  setDisplayType("FlashPoint"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.CHN_COMBUSTION")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.CHN_COMBUSTION",  setDisplayType("CHN_COMBUSTION"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Color") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Color")%></strong>
														</font></strong>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Color") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Color_1")%> > <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Color",  setDisplayType("Color"),"15"%>
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
															<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_1")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_1",  setDisplayType("Field_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.Field_2") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_2")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_2",  setDisplayType("Field_2"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_3")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_3",  setDisplayType("Field_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_4") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_4")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_4",  setDisplayType("Field_4"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_5")%> >   <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_5",  setDisplayType("Field_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("Batches.Field_6") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_6")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_6",  setDisplayType("Field_6"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_7")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_7",  setDisplayType("Field_7"),"15"%>
														</font>
														</td> 
													<%end if%>
												
													<%if not checkHideField("Batches.Field_8") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_8")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_8") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_8")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_8",  setDisplayType("Field_8"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","Batches.Field_9")%> >    <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_9",  setDisplayType("Field_9"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("Batches.Field_10") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("Batches.Field_10")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("Batches.Field_10") then%>                           
														<td width="150" <%=setBackGroundDisplay("c2","Batches.Field_10")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Batches.Field_10",  setDisplayType("Field_10"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                 <tr>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_1") then%>
														<td width="200" align="right"  <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_1") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_1",  setDisplayType("INT_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_2") then%>                           
														<td  width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_2",  setDisplayType("INT_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_3") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_3") then%>                           
														<td   <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_3",  setDisplayType("INT_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_4") then%>
														<td width="200" align="right"<%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_4") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_4",  setDisplayType("INT_BATCH_FIELD_4"),"15"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","INT_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_5",  setDisplayType("INT_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("INT_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.INT_BATCH_FIELD_6") then%>                           
														<td  width = "150"  <%=setBackGroundDisplay("c2","INT_BATCH_FIELD_6")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.INT_BATCH_FIELD_6",  setDisplayType("INT_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
												
                                                <tr>
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_1")%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_1",  setDisplayType("REAL_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_2",  setDisplayType("REAL_BATCH_FIELD_2"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>	
                                                
                                                <tr>
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_3") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_3")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_3") then%>                           
														<td width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_3",  setDisplayType("REAL_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_4") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_4",  setDisplayType("REAL_BATCH_FIELD_4"),"15"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","REAL_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_5",  setDisplayType("REAL_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("REAL_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.REAL_BATCH_FIELD_6") then%>                           
														<td width = "150" <%=setBackGroundDisplay("c2","REAL_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.REAL_BATCH_FIELD_6",  setDisplayType("REAL_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>
                                                     
                                                
                                                <tr>
                                                <%if not checkHideField("BATCHES.DATE_BATCH_FIELD_1") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>  <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_1")%></strong>
														</font>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_1") then%>                           
														<td   width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_1")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_1",  setDisplayType("DATE_BATCH_FIELD_1"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_2") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_2")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_2") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_2")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_2",  setDisplayType("DATE_BATCH_FIELD_2"),"15"%>
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
														<td  width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_3")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_3",  setDisplayType("DATE_BATCH_FIELD_3"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_4") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_4")%></strong>
														</font></strong>
														</td> 
													<%end if%>

													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_4") then%>                           
														<td   "width = "150"  <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_4")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_4",  setDisplayType("DATE_BATCH_FIELD_4"),"15"%>
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
														<td width="150" <%=setBackGroundDisplay("c1","DATE_BATCH_FIELD_5")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_5",  setDisplayType("DATE_BATCH_FIELD_5"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_6") then%>
														<td width="200" align="right" <%=td_caption_bgcolor%>>    <strong><font <%=font_default_caption%>><%=getLabelName("DATE_BATCH_FIELD_6")%></strong>
														</font></strong>
														</td> 
													<%end if%>
													
													<%if not checkHideField("BATCHES.DATE_BATCH_FIELD_6") then%>                           
														<td  width = "150" <%=setBackGroundDisplay("c2","DATE_BATCH_FIELD_6")%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "BATCHES.DATE_BATCH_FIELD_6",  setDisplayType("DATE_BATCH_FIELD_6"),"15"%>
														</font>
														</td> 
													<%end if%>
                                                </tr>

                                            </table>
                                        </td>
                                    </tr>
								</table>
								<!--end physical properties-->								</tr>
						</td>
					</table>
				</tr>
			</td>
		</table>
	</tr>
</td>
</table>
 
 <%'end batch info%> 

		</td>
	</tr>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>