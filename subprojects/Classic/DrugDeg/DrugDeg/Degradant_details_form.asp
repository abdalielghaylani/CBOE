<%@ LANGUAGE=VBScript %>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<link REL="stylesheet" TYPE="text/css" HREF="/drugdeg/styles.css">
</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<%
' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' CAP 26 Apr 2001: Added the test of formmode to allow cancelled degradant
'	edit sessions to return to the _display_ of degradant details.
if "edit_record" <> formmode then
	Session( "ReturnToDegradantDetails" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
end if

' Set up a variable for the degradant key, since we use it in a few places.
keyDeg = Request.QueryString( "keydeg" )

' Connect to the database.
set connChem = GetConnection( dbkey, formgroup, "DRUGDEG_DEGS" )

' Make and fill a record set for the degradant.
set rsDegradant = Server.CreateObject( "ADODB.Recordset" )
rsDegradant.Open "Select * from DRUGDEG_DEGS where DEG_CMPD_KEY = " & keyDeg, connChem

' Make sure you have the first record.  There shouldn't be more than one,
' but better to be sure...
rsDegradant.MoveFirst

' Set up a session variable for the primary key of the object (degradant) being displayed.
Session( "PrimaryKey" & dbkey ) = rsDegradant.Fields( "DEG_CMPD_KEY" )

' Set variables for the height and width of the structure field.
heightStructArea = 400
widthStructArea = 400

%>
<%
if request("wizard") = "wiz_edit_deg_1" then
	if session("wizard") <> "wiz_edit_deg_2" then
		session("wizard") = "wiz_edit_deg_1"
	end if
end if
'add some helper text for the wizard
wizardStep = session("wizard")
select case lcase(wizardStep)
	case "wiz_edit_deg_1"
		if "edit_record" = formmode then%>
		<P>Use the form below to edit the degradant.
		
	<%	end if
	case "wiz_edit_deg_2"
		if "edit" = formmode then%>
		<P>The degradant has been updated. What would you like to do next?
		<P><a class="headinglink" href="Experiment_details_form.asp?dbname=<%=dbkey%>&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=rsDegradant.Fields("DEG_EXPT_FK")%>">View the experiment for this degradant</a>
		<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
			<br>Return here if you would like to add something else<br>
		 
		
	<%	
	end if
end select

%>
<input type=hidden name="dataaction2" value="update_drugdeg">
<input type=hidden name="parent_rgroup" value="<%=Request("parent_rgroup")%>">
<input type=hidden name="DEG_EXPT_FK" value="<%=rsDegradant.Fields( "DEG_EXPT_FK" )%>">

<table  border = 1 cellpadding="3" cellspacing="0">
	<tr>
		<td  valign = "top">
		<%
			Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
			rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsDegradant.Fields( "MOL_ID" ), connChem
			'if rsBase64.EOF then
				'if LCase(formmode = "edit") then
					'Response.Write "No Structure Submitted"
				'else
					'ShowStrucInputField  dbkey, formgroup, "DRUGDEG_DEGS.Structure", "5", widthStructArea, heightStructArea, "Exact", "SelectList"
				'end if
			'else
			if not rsBase64.EOF then
				ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX", widthStructArea, heightStructArea
			
			'end if
			
			sMW = ""
			sFormula = ""
			' Get the molecular weight.
			sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "DRUGDEG_BASE64.MolWeight", rsBase64( "MOL_ID" ), "raw", "1", "17" )
			' Keep only two decimal places for the molecular weight.
			' 2002-09-17: FormatNumber doesn't seem to work for the molecular weight I get using the new
			' COWS core: I can't explain why, I tested everything I could to figure it.  Anyway, only
			' thing I found that worked was to make my own formmating function.
			'sMW = MyFormatNumber( sMW, 4 )
			
			' Get the molecular formula.
			FormulaOriginal = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.Formula", rsBase64( "MOL_ID" ), "raw", "1", "17" )
			' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
			
			'sFormula = GetHTMLStringForFormula( sFormulaOriginal )
			sFormula = FormulaOriginal
			end if
			rsBase64.Close
		%>
		
		</td>


		<td  valign = "top">
			<table  border = 0  color = "red">

				<tr>
					<td  valign = "top">
						<strong>Compound Number</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote> 
						<%
						if "edit" = LCase( formmode ) and "" = rsDegradant.Fields( "COMPOUND_NUMBER" ) then
							' Not editting and there is nothing in the ID field.
							Response.Write( "No Compound Number was entered" )
						else
							ShowResult dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.COMPOUND_NUMBER", "RAW", "0", "12"
						end if %>
						</blockquote>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<%if lcase(formmode) = "edit" and rsDegradant.Fields( "mol_id" ) <> "" then%>
						<strong>Compound&nbsp;ID</strong>
						<%end if%>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote> 
						<%
						if lcase(formmode) = "edit" and rsDegradant.Fields( "mol_id" ) = "" then
							 'Not editting and there is nothing in the ID field.
							Response.Write( "No Compound ID found" )
						elseif lcase(formmode) = "edit" and rsDegradant.Fields( "mol_id" ) <> "" then
							
							ShowResult dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.mol_id", "RAW", "0", "12"
						end if %>

						
						</blockquote>
					</td>
				</tr>
				<%'end if%>
				<!-- A little vertical spacing before the next item. -->
				<tr>
					<td  height = 20>
					</td>
				</tr>

				<tr>
					<td  valign = "top">
						<strong>Name</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>
						<%
						if "edit" = LCase( formmode ) and "" = rsDegradant.Fields( "ALIAS" ) then
							' Not editting and there is nothing in the alias field.
							Response.Write( "No name was entered" )
						else
							ShowResult dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.ALIAS", "RAW", "0", "30"
						end if 
						%>
						
						</blockquote>
						<%' ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
					</td>
				</tr>
				<!-- A little vertical spacing before the next item. -->
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<%if "edit_record" <> LCase( formmode ) then%>
				<tr>
					<td  valign = "top">
						<strong>Formula</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>
						<%
						
		
						if "edit" = LCase( formmode ) and sFormula <> "" then
							' Not editting and there is nothing in the alias field.
							Response.Write( sFormula )
		
						end if 
						%>
						
						</blockquote>
						<%' ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
					</td>
				</tr>
				<!-- A little vertical spacing before the next item. -->
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<strong>Molecular Weight</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>
						<%
						if "edit" = LCase( formmode ) and "" <> sMW  then
							' Not editting and there is nothing in the alias field.
							Response.Write( sMW )
						end if 
						%>
						
						</blockquote>
						<%' ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
					</td>
				</tr>
				<!-- A little vertical spacing before the next item. -->
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<%end if
				'Request("parent_rgroup") <> "" and cBool(Request("parent_rgroup")) = true and 
				if "edit" = LCase( formmode ) then%>
				<tr>
					<td  valign = "top">
						<strong>Change in Molecular Weight</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>
						<%
							'if rsDegradant.Fields( "MW_DIR_CHG" ) and  rsDegradant.Fields( "MW_DIR_CHG" ) = "1" then
							'	sDisplayDirection =  "+"
							'else
							'	sDisplayDirection =  "-"
							'end if
							
							'if "edit_record" <> LCase( formmode ) then
							'	if  "" <> rsDegradant.Fields( "MW_AMT_CHG" ) then
							'		Response.Write sDisplayDirection
							'	end if
							'else
							'	on error resume next
									
							'	sMWDirectionList = "1:+,2:-"
											
							'	ShowLookUpList dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.MW_DIR_CHG", sMWDirectionList, rsDegradant.Fields( "MW_DIR_CHG" ), sDisplayDirection, 0, false, "value", "1"
									
							'end if
							ShowResult dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.MW_AMT_CHG", "RAW", "0", "12"
							if hasRgroup(sFormulaOriginal) = true and "edit_record" <> LCase( formmode ) then
								Response.Write "<br>(note: R Groups affect the changed in MW)"
							end if%>
						</blockquote>
						
						<%' ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
					</td>
				</tr>
				<%else
					if "" = rsDegradant.Fields( "MW_AMT_CHG" ) then%>
				<tr>
					<td  valign = "top">
						<strong>Change in Molecular Weight</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>	
							
						No amount was entered
						
						</blockquote>
						

					</td>
				</tr>
					<%else%>
						
				<tr>
					<td  valign = "top">
						<strong>Change in Molecular Weight</strong>
						
						<br><font size="-1">Please enter a molecular weight change<br>
						if the parent or degradant contain an<br>
						R-Group. If no number is entered, the<br />
						change will be calulated. &nbsp;If you would<br />
						like the change to be recalculated please<br />
						delete the value below.
						</font>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<blockquote>		
							<%
							'if rsDegradant.Fields( "MW_DIR_CHG" ) and  rsDegradant.Fields( "MW_DIR_CHG" ) = "1" then
							'	sDisplayDirection =  "+"
							'else
							'	sDisplayDirection =  "-"
							'end if
								
							'sMWDirectionList = "1:+,2:-"
											
							'ShowLookUpList dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.MW_DIR_CHG", sMWDirectionList, rsDegradant.Fields( "MW_DIR_CHG" ), sDisplayDirection, 0, false, "value", "1"
									
							ShowResult dbkey, formgroup, rsDegradant, "DRUGDEG_DEGS.MW_AMT_CHG", "RAW", "0", "12"
							
					
							
							%>
							</blockquote>
							
							<%' ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
						</td>
					</tr>
				
						<%
					end if
				end if%>
						
				<tr>
					<td  valign = "top">
						<strong>Functional group(s) involved in degradation reaction</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">

						<script language="javascript">
							w_fgroup_list = ""
							w_current_fgroup_list = ""
						</script>
						<blockquote>
						<%
						current_deg_fgroup_list=GetCurrentFunctionalGroups(dbkey, formgroup,  connChem,  "Deg_FGroup_Key", "Deg_FGroup_text", "DRUGDEG_FGroups.Deg_FGroup_text", Session( "PrimaryKey" & dbkey ))
						if "edit" = LCase( formmode ) and "" = current_deg_fgroup_list then
							' Not editting and there is nothing in the alias field.
							Response.Write( "No functional groups were selected" )
						else
							
							
							if "edit_record" <> LCase( formmode ) then
								'write out the values
								if  "" <> current_deg_fgroup_list then
									temp_array = Split(current_deg_fgroup_list, ",", -1)
									for i = 0 to UBound(temp_array)
										temp_array2 = split(temp_array(i), ":", -1)
										sFGroup_Text = temp_array2(1)
										Response.Write sFGroup_Text & "<br>"
									next
								end if
							else
								on error resume next

						
						'get all the form groups
						all_fgroups_list = ListAllFGroupsOrdered(dbkey, formgroup, connChem, "Deg_FGroup_Key", "Deg_FGroup_text", "DRUGDEG_FGroups.Deg_FGroup_text")
						%>
						<script language="javascript">
							w_fgroup_list = "<%=all_fgroups_list%>"
							w_current_fgroup_list = "<%=current_deg_fgroup_list%>"
						</script>
						<%if all_fgroups_list <> "" then%>
							<select name="fgroups" width = "220" size = "6" ID="fgroups" onchange="updateHiddenFGroup()" multiple>
							</select>
						<%else%>
						No functional groups available.
						<%end if%>
							<input type = "hidden" value = "" name = "current_fgroups_hidden" ID="current_fgroups_hidden">
						<%
							end if
						end if 
						%>
						</blockquote>
						
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>





































<%
' Close the record set for the degradant.
rsDegradant.Close

' Close the database connection.
connChem.Close
%>

<!-- START: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->
</form>
<%WriteAppletCode()%>

<form name = "nav_variables" method = "post" Action = "<%=Session("CurrentLocation" & dbkey & formgroup)%>">
<input type = "hidden" name = "RecordRange" Value = "<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type = "hidden" name = "CurrentRecord" Value = "<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type = "hidden" name = "AtStart" Value = "<%=Session("AtStart" & dbkey & formgroup)%>">
<input type = "hidden" name = "AtEnd" Value = "<%=Session("AtEnd" & dbkey & formgroup)%>">
<input type = "hidden" name = "Base_RSRecordCount" Value = "<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type = "hidden" name = "TotalRecords" Value = "<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type = "hidden" name = "PagingMove" Value = "<%=Session("PagingMove" & dbkey & formgroup)%>">
<input type = "hidden" name = "CommitType" Value = "<%=commit_type%>">
<input type = "hidden" name = "TableName" Value = "<%=table_name%>">
<input type = "hidden" name = "UniqueID" Value = "<%=uniqueid%>">
<input type = "hidden" name = "BaseID" Value = "<%=BaseID%>">
<input type = "hidden" name = "CPDDBCounter" Value = "<%=cpdDBCounter%>">
<input type = "hidden" name = "CurrentIndex" Value = "<%=currentindex%>">
<input type = "hidden" name = "BaseActualIndex" Value = "<%=BaseActualIndex%>">
<input type = "hidden" name = "Base64" Value = "<%=Session("BASE64_CDX" & uniqueid & dbkey & formgroup)%>">
<input type = "hidden" name = "Stored_Location" Value = "">

</form>
<script language = "javascript">
	window.onload = function(){loadframes()}
	function loadframes(){
		loadUserInfoFrame()
		//loadNavBarFrame()
		<%if "edit_record" = LCase( formmode ) then%>
		fill_deg_fgroup_list();
		updateHiddenFGroup();
		<%end if%>
		DoAfterOnLoad ? AfterOnLoad():true;
	}
</script>
<!-- END: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->

</body>

</html>
