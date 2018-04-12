<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
Response.Buffer = true
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>

<html>


<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE= "../source/secure_nav.asp"-->
<!--#INCLUDE FILE= "../source/app_js.js"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->

<title>Chemical Registration - Add Identifiers Input Form</title>


<%
record_added = request.querystring("record_added")
	if not record_added <> "" then
		record_added = "none"
	end if
	if Not record_added = "none" then%>
<script language="javascript">
	identLocation = "<%=CBool(Application("Identifiers_To_Temp"))%>"
		if(identLocation.toLowerCase() = "true"){
			alert("Identifiers were added to the temporary table")
		}
		else{
			alert("Identifiers where added to the permanent registry tables")
		}

	</script>
<%end if%>
<script language="javascript">
var commit_type="add_IDentifiers"
var cpd_counter="<%=request.querystring("cpd_counter")%>"
var reg_ID = "<%=request.querystring("the_reg_ID")%>"

</script>
<%	on error resume next
			commit_type = "add_IDentifiers"
			Set DataConn = GetNewConnection(dbkey, formgroup, "base_connection")
			if DataConn.State=0 then ' assume user has been logged out
				DoLoggedOutMsg()
			end if
			
			reg_ID =  Request.QueryString("the_reg_ID")
			cpdDBCounter = 	getValueFromTablewConn(DataConn,"Reg_Numbers", "reg_ID", reg_ID, "cpd_internal_ID")
			reg_number = 	getValueFromTablewConn(DataConn,"Reg_Numbers", "reg_ID", reg_ID, "Reg_Number")
			project_id = getValueFromTablewConn(DataConn,"Compound_Project", "cpd_internal_id", cpdDBCounter, "project_internal_id")
			
			display_type = "reg_number"
			'registered compound information recordset generation 
			'assumes cpdDBCounter and reg_ID variables are populated
			getRegData DataConn, reg_ID, cpdDBCounter,display_type 
			bShowRegIdentifiers = true
			bShowRegCmpds = true
			bShowBatchData = true
			bShowRegSalt = true
			'BaseRS.MoveFirst	
			
		sql = "Select * from Structures where cpd_internal_ID = " & cpdDBCounter
		Set MoleculesRS = DataConn.Execute(sql)

DBMSUser_ID = Session("CurrentUser" & dbkey)
	entry_person_ID = getValueFromTablewConn(DataConn, "People", "User_ID", DBMSUser_ID, "Person_ID")


%>
</head>

<body>
<!--#INCLUDE FILE= "../source/app_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type = "hidden" name = "return_location" value ="<%=Session("CurrentLocation" & dbkey & formgroup) %>">

<input type = "hidden" name = "Temporary_Structures.Entry_Person_ID" value ="<%=entry_person_ID%>">
<input type = "hidden" name = "Temporary_Structures.Entry_Date" value ="<%=Date()%>">
<input type = "hidden" name = "Temporary_Structures.Salt_Name" value ="no_salt">
<input type = "hidden" name = "Temporary_Structures.Salt_Code" value ="1">
<input type = "hidden" name = "Temporary_Structures.Scientist_ID" value ="">
<input type = "hidden" name = "Reg_Internal_ID" value ="<%=reg_id%>">
<input type = "hidden" name = "Temporary_Structures.Reg_Internal_ID" value ="<%=reg_id%>">
<input type = "hidden" name = "Temporary_Structures.CPD_Internal_ID" value ="<%=cpdDBCounter%>">
<input type = "hidden" name = "Temporary_Structures.Project_ID" value ="<%=project_id%>">


<%'chemreg header information%>
<table border="0">
	<tr>
		<td width="40%" align="left" nowrap>
			<p align="left"><strong><font face="Arial" size="3" color="#182889">New	Identifiers Submission Form</font></strong></p>
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

<table  <%=registered_compound_table%> width = "790">
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
										<td valign="top" align="left"><table border = "1" bgcolor=#ffffff><tr><td>
										<%Base64DecodeDirect dbkey, formgroup, Session("Base64_cdx"), "Structures.BASE64_CDX", Session("reg_id") , Session("cpdDBCounter"), "330" & ":BASE64CDX_NO_EDIT", "200"%> &nbsp;</tr></td></table>
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
<%'start new identifiers%>
<br>
<br>
			<table  <%=table_main_L1%> width = "675" >
						
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
												
											<font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.Chemical_Name", setDisplayType("CHEMICAL_NAME"), "75"%></font>
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
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.CAS_Number",   setDisplayType("CAS_NUMBER"),"100"%></font>
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
								<%if not checkHideField("GROUP_CODE") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("GROUP_CODE")%></b></font>
										</td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup, "Temporary_Structures.Group_Code",setDisplayType("GROUP_CODE"),"100"%></font>
										</td>
									</tr>
								<%end if%>			
								
								<%if not checkHideField("FEMA_GRAS_NUMBER") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("FEMA_GRAS_NUMBER")%></b></font>
									    </td>
									</tr>
									<tr>					
										<td <%=td_bgcolor%>>  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "Temporary_Structures.FEMA_GRAS_Number", setDisplayType("FEMA_GRAS_NUMBER"),"100"%></font>
										</td>
									</tr>
								<%end if%>
													
								
							
								<%if not checkHideField("Collaborator_ID") then%>
									<tr>
										<td <%=td_caption_bgcolor%> ><font <%=font_default_caption%>><b><%=getLabelName("Collaborator_ID")%></b></font>
										</td>
									</tr>
									<tr>						
										<td  <%=td_bgcolor%> >  <font <%=font_default%>><%ShowInputField dbkey, formgroup,  "TEMPORARY_STRUCTURES.COLLABORATOR_ID",  setDisplayType("Collaborator_ID"),"100"%></font>
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


<%CloseConn(DataConn)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>

</html>