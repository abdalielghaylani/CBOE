<%@ LANGUAGE=VBScript %>
<%	Response.expires=-1
Response.Buffer = true
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>

<html>
<head>
<%if Not Session("SITE_ACCESS_ALL" & dbkey) = True and Application("SITES_USED") = 1  then
	bOverrideRegAll = true
else
	bOverrideRegAll= false
end if%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<script language = "javascript">
	var db_record_count = "<%=DBRecordCount%>"
	var bOverrideRegAll = "<%=bOverrideRegAll%>"
	//db_record_count = get_db_record_count("temp")
</script>

<!--#INCLUDE FILE = "custom_functions.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE = "../source/reg_security.asp"-->

<!--#INCLUDE FILE = "../source/app_vbs.asp"-->

<title>Chemical Registration - Review/Register Compounds Results List View</title>
</head>
<body <%=default_body%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<%
	

	
	
if Not Session("SITE_ACCESS_ALL" & dbkey) = True and Application("SITES_USED") = 1  then
	bOverrideRegAll = true
else
	bOverrideRegAll= false
end if%>
<input type = "hidden" name="duplicate_processing" value = "">
<input type = "hidden" name = "temp_sort_by" value = "">
<input type = "hidden" name = "temp_sort_by_direction" value = "">

<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated
on error resume next 

'UpdateTempRecordCount(RegConn)
Set RegConn = GetRegConn(dbkey, formgroup)
if RegConn.State=0 then ' assume user has been logged out
	DoLoggedOutMsg()
end if
bFBApproved=true
compound_registered = checkRegTracking(BaseID)

duplicate_found = checkDupTracking(BaseID)
if Not compound_registered <> "" then
	Set BaseRS = Server.CreateObject("adodb.recordset")
	on error resume next
	set cmd = server.CreateObject("adodb.command")
	cmd.commandtype = adCmdText
	cmd.ActiveConnection = RegConn
	sql = "Select * from temporary_structures where temp_compound_id=?"
	cmd.CommandText = sql
	cmd.Parameters.Append cmd.CreateParameter("pTempID", 5, 1, 0, BaseID) 
	BaseRS.Open cmd
	cmd.Parameters.delete "pTempID"
	
	commit_type= BaseRS("Commit_Type")
	
	Select Case UCase(commit_type)
	Case "FULL_COMMIT"
		Set SaltsRS = Server.CreateObject("adodb.recordset")
		sql = "Select Salt_name From Salts where Salts.Salt_Code = ?"
		set cmd = server.CreateObject("adodb.command")
		cmd.commandtype = adCmdText
		cmd.ActiveConnection = RegConn
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pSaltID", 5, 1, 0, BaseRS("Salt_Code") ) 
		SaltsRS.Open cmd
		cmd.Parameters.delete "pSaltID"	
	
		Session("BASE64_CDX" & BaseID & dbkey & formgroup) = BaseRS("BASE64_CDX")
			
		Session("TEMP_MOL_ID" & BaseID & dbkey & formgroup) = BaseRS("MOL_ID")
		'if Session("TEMP_MOL_ID" & BaseID & dbkey & formgroup) > 0 then
			'if Not Len(Session("BASE64_CDX" & BaseID & dbkey & formgroup))> 0 then
			'PopulateTempBase64 dbkey, formgroup, BaseID 
			'End if
		'End if
		dup_list = getDupList(dbkey, formgroup, BaseRS("duplicate"),RegConn)
	
		'BaseRS.MoveFirst
		
	Case "ADD_SALT"
	
		cpdDBCounter = BaseRS("cpd_internal_ID")
		Set CompoundRS = Server.CreateObject("adodb.recordset")
		set cmd = server.CreateObject("adodb.command")
		cmd.commandtype = adCmdText
		cmd.ActiveConnection = RegConn
		sql = "Select Root_Number From Compound_Molecule Where cpd_database_counter=?"
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("cpdDBCounter", 5, 1, 0, cpdDBCounter) 
		CompoundRS.Open cmd
		cmd.Parameters.Delete "cpdDBCounter"
	

		Set SaltsRS = Server.CreateObject("adodb.recordset")
		sql = "Select Salt_name From Salts where Salts.Salt_Code = ?"
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pSaltID", 5, 1, 0, BaseRS("Salt_Code") ) 
		SaltsRS.Open cmd
		cmd.Parameters.delete "pSaltID"	
		
	Case "BATCH_COMMIT", "ADD_IDENTIFIERS"
		reg_int_ID = BaseRS("reg_internal_ID")
		if CBool(Application("APPROVED_FLAG_USED")) = True and CBool(Application("ALLOW_BATCH_FOR_UNAPPROVED_CMPD")) = false then
			firstBatchID = getFirstBatchID(RegConn, reg_int_ID)
			bFBApproved = getApprovedFlag(RegConn, reg_int_ID, firstBatchID)
		else
			bFBApproved=true
		end if
		set cmd = server.CreateObject("adodb.command")
		cmd.commandtype = adCmdText
		cmd.ActiveConnection = RegConn
		Set RegRS = Server.CreateObject("adodb.recordset")
		sql = "Select Reg_Number From Reg_Numbers Where Reg_ID=?"
		cmd.CommandText = sql
		cmd.Parameters.Append cmd.CreateParameter("pRegID", 5, 1, 0, Reg_Int_ID) 
		RegRS.Open cmd
		cmd.Parameters.delete "pRegID"	
	End Select
end if
temp_table_fields_list = getTempTableFieldNames(dbkey, formgroup)
%>

<table <%=table_main_L1%> width = "650">

	<tr>
		<td valign="top" width="150" nowrap>
			<table >
				<tr>
					<td><%response.write "Record " & BaseRunningIndex & " of " & BaseTotalRecords%></td>
				</tr>
				<tr>
					<td>
					<script language="JavaScript">
						<!-- Hide from older browsers
						compound_reg_js = "<%=compound_registered%>"
						var RegTempPriv = "<%=Session("Register_Temp" & dbkey)%>"
						var fApproved = "<%=bFBApproved%>"
						if(compound_reg_js =="") {
							if ((RegTempPriv == "True")&&(fApproved.toLowerCase() == "true")) {
								
								getMarkBtn(<%=BaseID%>)
								document.write ('<br>')
								getFormViewBtn("review_register_btn.gif", "reg_ctrbt_commit_result_form.asp", <%=BaseActualIndex%>)
							}
							else {
								getMarkBtn(<%=BaseID%>)
								document.write ('<br>')
								getFormViewBtn("review_btn.gif", "reg_ctrbt_commit_result_form.asp", <%=BaseActualIndex%>)
							}
							
						}else{
							//getMarkBtn(<%=BaseID%>)
							//document.write ('<br>')
							//getFormViewBtn("review_btn.gif", "reg_ctrbt_commit_result_form.asp", <%=BaseActualIndex%>)
							
						}
						document.write ('<br>')	
						// End script hiding -->
						</script></td>
				</tr>
			</table>
		</td>
		<td valign="top" width="530">
			<%
			if compound_registered <> "" then
			
				reg_message = RegMessageOutput(compound_registered)
				response.write reg_message
			else
				%>
			<table>
				<tr>
					<td width="100%">
						<table  width="100%" cellpadding="3" cellspacing="0">
							<tr>
								<td <%=td_header_bgcolor%>><font <%=font_header_default_2%>><strong>Registration Type</strong></font>: </td>
								<td <%=td_header_bgcolor%>><font <%=font_default_caption%>>
									<%
									Select Case UCase(commit_type)
										Case "FULL_COMMIT"
											response.write  "New Compound"
											Response.Write dup_list
											
											
										Case "BATCH_COMMIT"
											if bFBApproved = true then
												response.write "Add Batch to Registered Compound"
											else
												response.write "Review Batch for Unapproved Registered Compound"
											end if
					
											
										Case "ADD_SALT"
											response.write "Add Salt to Registered Parent Compound"
										Case "ADD_IDENTIFIERS"
											response.write "Add Identifiers to Registered Compound"
									End Select
									
									'SYAN added on 12/1/2004 to fix CSBR-49587
									if not checkHideField("Approved") and Application("PRE_REGISTER_APPROVED_FLAG") = 1 then
										bApproved = getTempApprovedFlag(RegConn, BaseRS("TEMP_COMPOUND_ID"))
						
										if bApproved = true then
											Response.Write "&nbsp;&nbsp;&nbsp;<IMG SRC=""/chem_reg/graphics/th_up.gif"" ALT=""True"">"
										else
											Response.Write "&nbsp;&nbsp;&nbsp;<IMG SRC=""/chem_reg/graphics/th_dn.gif"" ALT=""False"">"
										end if
									end if
									'End of SYAN modification
									%>
								
								</font></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td width="447">
						<%
						Select Case UCase(commit_type)
							Case "BATCH_COMMIT", "ADD_IDENTIFIERS"
						%>
						<%if duplicate_found <> "" then
						
							if instr(duplicate_found, "unapproved_compound_no_duplicates")>0 then
								reg_message = RegMessageOutput("unapproved_compound_no_duplicates")
								response.write "<br>" & reg_message
							end if
						end if
					%>
					</td>
				</tr>
				<tr>
					<td width="447">
						<table border="0" width="100%">
							<tr>
								<td>
									<table border="0">
										<tr>
											<td <%=td_caption_bgcolor%>><font <%=font_default_caption%>><b><%=getLabelName("Reg_Number")%></b></font></td>
											<td <%=td_bgcolor%>><font <%=font_default%>><%response.write RegRS("Reg_Number")%></font></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td><%Case "FULL_COMMIT"%>
					<%if duplicate_found <> "" then
						
						if instr(duplicate_found, "unapproved_compound_with_duplicates")>0 then
							reg_message = RegMessageOutput("unapproved_compound_with_duplicates")
							response.write "<br>" & reg_message
						else
							if instr(duplicate_found, "unapproved_compound_no_duplicates" )>0 then
							
								reg_message = RegMessageOutput("unapproved_compound_no_duplicates")
								response.write "<br>" & reg_message
							else
								reg_message = RegMessageOutput(duplicate_found)
								response.write "<br>" & reg_message
							end if
						end if
					else
						if unapproved = true then
							reg_message = RegMessageOutput("unapproved_compound_no_duplicates")
							response.write "<br>" & reg_message
						end if
					end if
					%>
					<p><%if UCase(Application("Batch_Level")) = "SALT" then
					%> </p>
        <table border="0" width="100%">
          <tr>
            <td>  <%on error resume next
           
					ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.BASE64_CDX", "BASE64CDX", 200, 150
          err.Clear()%>
</td>
          </tr>
          <tr>
            <td>
              <table border="0" width="100%">
                <tr>
                  <td <%=td_caption_bgcolor%> width="33%"><font <%=font_default_caption%>><b><%=getLabelName("Salt_Code")%></b></font></td>
                  <td <%=td_caption_bgcolor%> width="33%"><font <%=font_default_caption%>><b><%=getLabelName("Formula")%></b></font></td>
					<td <%=td_caption_bgcolor%> width="33%"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font></td>
                </tr>
                <tr>
                  <td <%=td_bgcolor%> width="33%"><font <%=font_default%>><%ShowResult dbkey, formgroup, SaltsRS,"Salts.Salt_Name", "raw", "", "20"%>
                  </font></td>
                 <td <%=td_bgcolor%> width="33%"><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.FORMULA2", "raw", setDisplayType("FORMULA"), "20"%></font></td>
                  <td <%=td_bgcolor%> width="33%"><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.MW2", "raw", setDisplayType("MW"), "20"%></font></td>
                </tr>
              </table>
</td>
          </tr>
        </table>
<%
Else
%>
        <table border="0" width="100%">
          <tr>
            <td width="450"><table border="0" width="100%">
              <tr>
                <td <%=td_bgcolor%> ><font <%=font_default%>><%
					ShowResult dbkey, formgroup, BaseRS,"Temporary_Structures.BASE64_CDX", "BASE64CDX", 200, 150
               %>
</td>
              
                <td width = "150" valign="top" align="left">
                  <table border="0" width="100%">
                    <tr>
                       <td <%=td_caption_bgcolor%> width="50%"><font <%=font_default_caption%>><b><%=getLabelName("Formula")%></b></font></td>
                        <td <%=td_bgcolor%> width="50%"><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.FORMULA2", "raw",  setDisplayType("Formula"), "20"%></font></td>
					</tr>
                     <tr>
                      <td <%=td_caption_bgcolor%> width="50%"><font <%=font_default_caption%>><b><%=getLabelName("MW")%></b></font></td>
					<td <%=td_bgcolor%> width="50%"><font <%=font_default%>><%ShowResult dbkey, formgroup, BaseRS, "Temporary_Structures.MW2", "raw", setDisplayType("MW"), "20"%></font></td>
                    </tr>
                  </table>
</td>
              </tr>
            </table>
            </td>
          </tr>
        </table>
<%End If
%>
<%Case "ADD_SALT"
%>
        </td>
      </tr>
      <tr>
        <td width="447"><table border="0" width="100%">
          <tr>
            <td><table border="0" width="439">
              <tr>
               <td <%=td_caption_bgcolor%> width="201"><font <%=font_default_caption%>><b>Root Number:</b></font></td>
               <td <%=td_bgcolor%> width="188"><font <%=font_default%>>&nbsp;<%=CompoundRS("Root_Number")%></font></td>
               <td <%=td_bgcolor%> width="30"><font <%=font_default%>><%ShowResult dbkey, formgroup, SaltsRS,"Salts.Salt_Name", "raw", "", "20"%></font></td>
              </tr>
            </table>
            </td>
          </tr>
        </table>
        </td>
      </tr>
      <tr>
        <td width="447"><%End Select%>
</td>
      </tr>
    </table>

<%end if 'if compound_registered%>
    </td>
  </tr>
</table>
<hr>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_footer_vbs.asp"-->
</body>
</html>
