<%@ LANGUAGE="VBScript" %>
<%
response.expires = 0
' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved
if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then
		response.redirect "/" & Application("appkey") & "/logged_out.asp"
	end if
end if

'MRE skip this page and go to the details page if we just added a parent
if Session("addParent") <> "" and Session("addParent") = true then
	Response.Redirect "/" & Application("appkey") & "/drugdeg/Parent_details_form.asp?formgroup=base_form_group&dbname=drugdeg&formmode=edit&unique_id=1&commit_type=&indexvalue=1&form_change=true"
end if

%>
<html>

<head>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<title>Parent list view</title>
	<link REL="stylesheet" TYPE="text/css" HREF="/drugdeg/styles.css">
</head>

<body <%=Application("BODY_BACKGROUND")%>>
<%
'add some helper text
wizardStep = session("wizard")

select case lcase(wizardStep)
		case "wiz_add_deg_1"%>
		<P>Adding a Degradation compound.
		<ol>
			<li>Select Parent compound
				
			<li class=highlightcopy>Select Experiment
				<ul>
					<li>Select the experiment from the parent compound you are interested in.
					<li>If you do not see the experiment you want, click the "Add Experiment" link next to the parent compound
				</ul>
			<li>Add Degradant
		</ol>
		<%case "wiz_add_exp_1"%>
		<P>Adding an Experiment.
		<ol>
			<li>Select Parent compound
				
			<li class=highlightcopy>Select Experiment
				<ul>
					<li>Select the experiment from the parent compound you are interested in.
					<li>If you do not see the experiment you want, click the "Add Experiment" link next to the parent compound
				</ul>
		</ol>
		<%case "wiz_edit_par_1"%>
		<P>Edit a Parent Compound.
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>Use the form below to search for a parent compound
				</ul>
		</ol>
		<%case "wiz_edit_exp_1"%>
		<P>Editing an Experiment.
		<ol>
			<li >Select Parent compound
				<ul>
					<li>You may use the form below to search for a parent compound
				</ul>
			<li class=highlightcopy>Select Experiment
				<ul>
					<li>Choose an expirement to edit from the list next to a parent compound in your search results
				</ul>
		</ol>
		<%case "wiz_edit_deg_1"
			session("wizard") = "wiz_edit_deg_2"%>
		<P>Editing a Degradation compound.
		<ol>
			<li >Select Parent compound
			<li class=highlightcopy>Select Experiment
				<ul>
					<li>Choose an expirement from the list next to a parent compound in your search results
				</ul>
			<li>Select Degradant
		</ol>
<%end select

'MRE added 1/13/05
'used to list the experiments for each result
Dim	rsExperiments
Dim	rsExperimentConds


' Set up a session variable for this as the current page displayed.
'Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
Session( "ReturnToCurrentPage" & dbkey ) = "/drugdeg/DrugDeg/Parent_result_list.asp?formgroup=base_form_group&formmode=list&dbname=DRUGDEG&indexvalue=3&form_change=true"
'Session( "ReturnToCurrentPage" & dbkey )

' Set variables for the height and width of structure fields.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 160
widthStructArea = 220

' Set variables for some display variations.  I do this so that, if I change my mind about
' when to use a particular variation, I can put that logic up here.  For example, the Compound, formula, weight, etc. had been listed in separate columns.  When first I designed the
' printer-friendly form I figured that, when showing the structure, I could stack all those
' items vertically in one column.  Later I decided that I'd like to do that _whenever_ showing
' structures.  I made up a variable, bSeparateColumns, to indicate how the data were to be
' laid out.
if ( "DoNotShowStructs" <> Request.Cookies( "UserResultsPrefs" & dbkey & formgroup ) ) then
	bShowStructs = true
else
	bShowStructs = false
end if

if ( "1" = Request.Cookies( "PrinterFriendly" & dbkey & formgroup ) ) then
	bPrinterFriendly = true
else
	bPrinterFriendly = false
end if

if ( false = bShowStructs ) then
	bSeparateColumns = true
else
	bSeparateColumns = false
end if
bSeparateColumns = false

%>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<%
' For both of the following "table" tags I used "cellpadding = 2" because the cell contents
' were just too tight up against the cell walls, and with the walls so thin the contents of
' one cell were just too close to those of another.


if ( false = bPrinterFriendly ) then
	' Not the printer-friendly form, so we don't need to set the table width.
%>
<br clear="all">
<table  border = 1  cellpadding = 2  cellspacing = 0 width="90%">
<%
else
	' The printer-friendly form: set the width to a safe value for a printed page.
%>
<table  border = 1  cellpadding = 2  cellspacing = 0  width = 550>
<%
end if


if false then
	if not Session( "fEmptyRecordset" & dbkey & formgroup ) = True then
	%>
		<tr>
			<%
			if ( false = bPrinterFriendly ) then
				
			%>
			<th>&nbsp;</th>
			<% end if %>

			<!-- <th>Compound Info</th> -->
			<%
			if ( true = bShowStructs ) then
				' We are to show structures, so we need a column for those.
			%>
			<th>Structure</th>
			<% end if %>

			<%
			if ( true = bSeparateColumns ) then
				' Use one column for each different data item.
			%>
			<th>Formula</th>
			<th>MW</th>
			<th>Salt(s)</th>
			<th>Generic Name</th>
			<%
			else
				' We are to put the rest of the data into one cell, stacked vertically.
			%>
			<th>&nbsp</th>
			<%
			end if
			%>
		</tr>
	<%end if
end if ' if false%>

	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	<%
	'BaseID represents the primary key for the recordset from the base array for the current row
	'BaseActualIndex is the actual id for the index the record is at in the array
	'BaseRunningIndex is the id based on the number shown in list view
	'rsParent is the recordset that is pulled for each record generated
	Set connChem = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )
	sqlParent = GetDisplaySQL( dbkey, formgroup, "DRUGDEG_PARENTS.*", "DRUGDEG_PARENTS", "", BaseID, "" )
%>
<!--
	<tr>
		<td  colspan = 10>
			sqlParent = "<%=sqlParent%>"
		</td>
	</tr>
-->
<%
	Set rsParent = connChem.Execute( sqlParent )

	' Get the base64_CDX data for the parent compound.
	Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
	rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connChem
	sName = rsParent.Fields( "GENERIC_NAME" )
	sTradeName = rsParent.Fields( "TRADE_NAME" )
	sCommonName = rsParent.Fields( "COMMON_NAME" )
	sSubmittedBy = rsParent.Fields( "SUBMITTED_BY" )
	sCompoundNumber = rsParent.Fields( "COMPOUND_NUMBER" )
	
	sSaltList = GetSaltNameListFromSaltCodeList( rsParent.Fields( "SALT" ) )
	' Get the molecular formula.
	sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.Formula", rsBase64( "MOL_ID" ), "raw", "1", "17" )
	' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
	'sFormula = GetHTMLStringForFormula( sFormula )

	' Get the molecular weight.
	sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "DRUGDEG_BASE64.MolWeight", rsBase64( "MOL_ID" ), "raw", "1", "17" )
	' Keep only two decimal places for the molecular weight.
	' 2002-09-17: FormatNumber doesn't seem to work for the molecular weight I get using the new
	' COWS core: I can't explain why, I tested everything I could to figure it.  Anyway, only
	' thing I found that worked was to make my own formmating function.
	'sMW = MyFormatNumber( sMW, 4 )
	
%>

	<tr>
		<%
		if ( false = bPrinterFriendly ) then
			' Not the printer-friendly form, so we put the record number & total, the
			' "Show Details" button, and the "Mark Record" button in the first column.
			
		%>
		<td  align = "center" width="80">
			<nobr> <!-- Introduced to avoid breaks in the "Record 21 of 75" line. -->
			<script language = "javascript">
				getRecordNumber( <%=BaseRunningIndex%> )
				document.write( '<br>' )
				getMarkBtn( <%=BaseID%> )
				document.write( '<br>' )
				getFormViewBtn( "show_details_btn.gif", "Parent_details_form.asp", "<%=BaseActualIndex%>", "edit", "", "base_form_group" )
// goFormView("/drugdeg/DrugDeg/Parent_details_form.asp?formgroup=base_form_group&amp;dbname=DrugDeg&amp;formmode=edit&amp;unique_id=1&amp;commit_type=","1")
			</script>
			<%
			'boolEditable = IsRecordEditable("DRUGDEG_PARENTS", "PARENT_CMPD_KEY", BaseID, connDrugDeg)
			'if wizardStep = "wiz_edit_par_1" and true = CBool( Session( "DD_Edit_Records" & dbkey ) ) and boolEditable = true then
			if wizardStep = "wiz_edit_par_1" and true = CBool( Session( "DD_Edit_Records" & dbkey ) ) then
			%>
				<br><A class="headinglink" href="Parent_details_form.asp?formgroup=base_form_group&dbname=drugdeg&formmode=edit_record&unique_id=<%=BaseActualIndex%>&commit_type=&indexvalue=<%=BaseActualIndex%>&keyparent=<%=BaseID%>&form_change=false&time=59286.12">EDIT</a>
			<%end if%>
			</nobr>
		</td>
		<% end if %>
		
		<td  align = "center"  valign = "middle">
		<%
		if ( true = bShowStructs ) then
			' We are to show structures.
		%>
			<% ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea %>
		<br><%
		end if
		'sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
		sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.Formula", rsBase64( "MOL_ID" ), "raw", "1", "17" )
		'sFormula = GetHTMLStringForFormula( sFormula )
		Response.Write( sFormula )
		'sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "DRUGDEG_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
		sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "DRUGDEG_BASE64.MolWeight", rsBase64( "MOL_ID" ), "raw", "1", "17" )
		if sMW <> "" then
			Response.Write "<br><br>MW: "
			sMW = replace (sMW,"&nbsp;","")
			sMW = FormatNumber( sMW, 2 )
			Response.Write(  sMW )
		end if
		%>
		
		</td>
		

		<%if ( false = bSeparateColumns ) then
			' Put the rest of the data into one cell, stacked vertically.

		%>
		<td valign="top">
		
				<!---JHS added 3/24/2003 --->
				<!---replaced an old layout --->
				<table  border = 1  cellpadding = "2" cellspacing="0" width = "100%">

				<%
				if rsParent.Fields( "GENERIC_NAME" ) <> "" then%>
				<tr>
					<td  valign = "top">
						<STRONG>Generic Name</STRONG>
					</td>
					<td  valign = "top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.GENERIC_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if
				if rsParent.Fields( "TRADE_NAME" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Trade name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.TRADE_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if
				if rsParent.Fields( "COMMON_NAME" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Common/Other name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMMON_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if
				if rsParent.Fields( "COMPOUND_NUMBER" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Compound Number</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMPOUND_NUMBER", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if%>
				<tr>
					<td  valign = "top">
						
							<strong>Salts</strong>
						
					</td>
					<td  valign = "top">
						<%
						sSaltNameList = GetSaltNameListFromSaltCodeList( rsParent.Fields( "SALT" ) )
						%><%=sSaltNameList%>&nbsp;  <!-- The blank assures we'll have cell borders. -->
					</td>
				</tr>
				<!-- CSBR ID:133586 -->
				<!-- Change Done by : Manoj Unnikrishnan -->
				<!-- Purpose: Removed the IF condition to check wizardstep <> ""; Due to this Experiments link was not displayed initially in the search result list -->
				<!-- Date: 11/12/2010 -->
				<tr>
					<td  valign = "top">
						
							<strong>Experiments</strong>
						
					</td>
					<td  valign = "top">
						
					<%
						' Make a record set for the experiments.
						
						Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

						' Make a record set for the experiment types.
						
						Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

						' Fill the record set for the experiments.
						rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connChem

						if rsExperiments.BOF and rsExperiments.EOF then
							' There are no degradation experiments for this parent compound.
							Response.Write "&nbsp;"
						else
							'Response.Write "<strong><nobr>Current Experiments:</nobr></strong>"
							' There are some experiment records.
							rsExperiments.MoveFirst
							while not rsExperiments.EOF
								' Fill the record set for the experiment conditions.
								rsExperimentConds.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connChem

								parentPassThruURL = "/" & Application("appkey") & "/drugdeg/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&keyexpt_passthru=" & rsExperiments.Fields( "EXPT_KEY" )
					%>
								<!--<br>&nbsp; &nbsp; &nbsp; <nobr>-->
								<nobr>
								<script>
								getFormViewLink("<%=rsExperimentConds.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt_passthru=<%=rsExperiments.Fields( "EXPT_KEY" )%>")
								</script>
								</nobr><br>
								
								<!---a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">
									<% Response.Write( rsExperimentConds.Fields( "DEG_COND_TEXT" ) ) %>
								</a>
								</nobr--->
					<%
								' Close the record set for the experiment type.
								rsExperimentConds.Close

								' Get the next experiment record.
								rsExperiments.MoveNext
							wend
							Response.write "<br><br>"
						end if  'if not rsExperiments.BOF or not rsExperiments.EOF then ...

						' Close the record set for the experiment type.
						rsExperiments.Close
						
						
						if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then
							' Allow the addition of degradation experiments for this parent compound,
							' _if_ the user is authorized to do so.
							if not bPrinterFriendly then%>
								<!-- Make a link to the display for adding an experiment. -->
								
								
								<a href="AddExperiment_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=add_experiment&amp;formgroupflag=add_experiment&amp;formgroupflag_override=add_experiment&amp;dataaction=experiment_full_commit&amp;record_added=false&amp;indexvalue=<%=BaseActualIndex%>&amp;keyparent=<% Response.Write( rsParent.Fields( "PARENT_CMPD_KEY" ) ) %>">
									<b>Create a new experiment</b>
								</a>
							<%end if
						end if  ' if the user is allowed to add records, ... %>
					</td>
				</tr>
				<!-- End of Change #133586#; Removed the corresponding END IF from here -->
				<!-- CSBR ID:133586 -->
				<!-- Change Done by : Manoj Unnikrishnan -->
				<!-- Purpose: Showing the Degradants link in the search result list was deliberatly disabled; Enabled to show the link as part of this requirement -->
				<!-- Date: 11/12/2010 -->
				<tr>
					<td  valign = "top">
						
							<strong>Degradants</strong>
						
					</td>
					<td  valign = "top">
						<%parentPassThruURL = "/" & Application("appkey") & "/drugdeg/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&degs=true&keyparent=" & BaseID
						%>
								<nobr>
								<script>
								getFormViewLink("View Degradants","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&degs=true&preserve_qs=1")
								</script>
								</nobr><br>
								<!-- <a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">
								no</a>-->
								
					</td>
				</tr>
				<!-- End of Change #133586#; Removed the corresponding disabling END IF from here -->
			</table>
			<!---JHS added 3/24/2003 end --->
			
		</td>
		<%
		
		else
			' Use one column for each different data item.
		
		%>

		<td  align = "center">
			<% Response.Write( sFormula ) %>
		</td>

		<td  align = "center">
			<% Response.Write( sMW ) %>
		</td>


		<td  align = "center">
			<%
			if "" <> Trim( sSaltList ) then
				Response.Write( sSaltList )
			else
				Response.Write( "&nbsp" )
			end if %>
		</td>

		<td  align = "center">
			<%
			if "" <> Trim( sName ) then
				Response.Write( sName )
			else
				Response.Write( "&nbsp" )
			end if %>
		</td>
		<% end if  ' if showing structures and printer-friendly ... %>
	</tr>
	<%
'-- CSBR ID:133589 & 133592
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Added the degradant fields like Formula, MW, Compound Number & Common/Other Name fields to the condition to show the matching degradant records for these fields
'-- Date: 11/12/2010
	if (session("SearchData" & "DRUGDEG_DEGS.MW_AMT_CHG" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEG_BASE64.Formula" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEG_BASE64.MolWeight" & dbkey & formgroup) <> "" or _
			session("SearchData" & "VW_DRUGDEG_COMPOUNDS.COMPOUND_NUMBER" & dbkey & formgroup) <> "" or _
			session("SearchData" & "VW_DRUGDEG_NAME.NAME" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "") then
'-- End of Change #133589 & 133592#
			%>
		</table>
		<%
		sSQL = "SELECT DRUGDEG_BASE64.BASE64_CDX , DRUGDEG_BASE64.MOL_ID as base64_mol_id, DRUGDEG_DEGS.*, DRUGDEG_CONDS.DEG_COND_TEXT "&_
						" from DRUGDEG_PARENTS, DRUGDEG_EXPTS, DRUGDEG_DEGS, DRUGDEG_CONDS, DRUGDEG_BASE64 "
		if session("SearchData" & "DRUGDEG_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "" then
			sSQL = sSQL & ",DRUGDEG_DEGSFGROUP " 
		end if
	
		sSQL = sSQL &	" where DRUGDEG_PARENTS.PARENT_CMPD_KEY = " & rsParent.Fields( "PARENT_CMPD_KEY" ) &" and "&_
						" DRUGDEG_PARENTS.PARENT_CMPD_KEY = DRUGDEG_EXPTS.PARENT_CMPD_FK and "&_
						" DRUGDEG_DEGS.DEG_EXPT_FK = DRUGDEG_EXPTS.EXPT_KEY and "&_
						" DRUGDEG_CONDS.DEG_COND_KEY = DRUGDEG_EXPTS.DEG_COND_FK and "&_
						" DRUGDEG_DEGS.MOL_ID = DRUGDEG_BASE64.MOL_ID  "
		if session("SearchData" & "DRUGDEG_DEGS.MW_AMT_CHG" & dbkey & formgroup) <> "" then
'-- CSBR ID:133589
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Add the change in MW range search
'-- Date: 11/22/2010
			mw_amt_chg_fieldValue = trim(session("SearchData" & "DRUGDEG_DEGS.MW_AMT_CHG" & dbkey & formgroup))
			if instr(mw_amt_chg_fieldValue,">") > 0 or instr(mw_amt_chg_fieldValue,"<") > 0 then
				strCompare = ""
			else
				mw_amt_chg_arr = split(mw_amt_chg_fieldValue,"-")
				arrlen = ubound(mw_amt_chg_arr)
				Select Case arrlen
					Case 0 ' single value & positive MW change
						strCompare = "=" & trim(mw_amt_chg_arr(0)) ' use the equality comparison
					Case 1 ' single negative value or simple range
						if trim(mw_amt_chg_arr(0)) = "" then 'single negative value
							strCompare = "= -" & trim(mw_amt_chg_arr(1)) ' use the equality comparison
						else	' simple range
							strCompare = " BETWEEN " & trim(mw_amt_chg_arr(0)) & " AND " & trim(mw_amt_chg_arr(1)) ' use the range operator
						end if	
					Case 2	' range with one negative
						if trim(mw_amt_chg_arr(0)) = "" then	'first is negative
							strCompare = " BETWEEN -" & trim(mw_amt_chg_arr(1)) & " AND " & trim(mw_amt_chg_arr(2)) ' use the range operator
						Else	' second is negative; dooesn't work;this is a wrong SQL
							strCompare = " BETWEEN " & trim(mw_amt_chg_arr(0)) & " AND -" & trim(mw_amt_chg_arr(2)) ' use the range operator
						End if
					Case 3	' range with two negatives
						strCompare = " BETWEEN -" & trim(mw_amt_chg_arr(1)) & " AND -" & trim(mw_amt_chg_arr(3)) ' use the range operator
				End select
			end if
			sSQL = sSQL & " and DRUGDEG_DEGS.MW_AMT_CHG  " & strCompare
'-- End of Change #133589#
		end if
		if session("SearchData" & "DRUGDEG_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "" then
	
			sSQL = sSQL & " and DRUGDEG_DEGSFGROUP.DEG_CMPD_ID = DRUGDEG_DEGS.DEG_CMPD_KEY and "&_
							" DRUGDEG_DEGSFGROUP.DEG_FGROUP_ID = " & session("SearchData" & "DRUGDEG_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup)
		end if

'-- CSBR ID:133592
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Add the views to the where clause
'-- Date: 11/12/2010
		if session("SearchData" & "VW_DRUGDEG_COMPOUNDS.COMPOUND_NUMBER" & dbkey & formgroup) <> "" then
			sSQL = sSQL & " and lower(DRUGDEG_DEGS.COMPOUND_NUMBER) like '%" & lcase(session("SearchData" & "VW_DRUGDEG_COMPOUNDS.COMPOUND_NUMBER" & dbkey & formgroup)) & "%'"
		end if
		if session("SearchData" & "VW_DRUGDEG_NAME.NAME" & dbkey & formgroup) <> "" then
			sSQL = sSQL & " and lower(DRUGDEG_DEGS.ALIAS) like '%" & lcase(session("SearchData" & "VW_DRUGDEG_NAME.NAME" & dbkey & formgroup)) & "%'"
		end if
'-- End of Change #133592#
		
'-- CSBR ID:133589
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: To filter out NON-Matches from the degradant records for Formula, MW fields; Also takes into consideration when both Parent & degradant data are provided for search
'-- Date: 11/12/2010
		if ((session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEG_BASE64.Formula" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEG_BASE64.MolWeight" & dbkey & formgroup) <> "") AND _
			not (session("SearchData" & "DRUGDEG_BASE64.Formula" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_BASE64.MolWeight" & dbkey & formgroup) <> "" or _
			session("SearchData" & "DRUGDEG_DEGS.MW_AMT_CHG" & dbkey & formgroup) <> "")) then
'-- End of Change #133589#
			'connChem.beginTrans
			'forcesql = "Insert into cscartridge.tempQueries (id,query) values(86,'" & session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup) &  "'"
			'connChem.Execute(forcesql)
			
	
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = connChem
	Cmd.CommandType = adCmdText
			
		connChem.Begintrans()
		'if IsArray(Session("base64Array")) then 
		'	arrlength = Ubound(Session("base64Array"))
		'	for i= 0 to arrlength
				sql = "BEGIN INSERT INTO cscartridge.tempQueries (id, query) VALUES (86, ?); END;"
				b64 = session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup)
				Trace "DoCartridgeSearch Query Molecule " & i & " = " & b64, 20
				Cmd.Parameters.Append Cmd.CreateParameter("p1", 201, 1, len(b64), b64 )	
				cmd.CommandText = sql
				cmd.properties("SPPRMSLOB") = true
				cmd.Execute
				cmd.properties("SPPRMSLOB") = false
				cmd.Parameters.delete "p1"
		'	Next
        'end if
		'sql = GetTopRows(sqlString, maxHits)
		Cmd.CommandText = "Select query from cscartridge.tempQueries where id = 86"
		'Trace "DoCartridgeSearch SQL= " & sql, 19
		't0 = timer
        Set rsjhs = Cmd.Execute
        ' The following line accesses the RS.  If this is not done then the RS
        ' dies during the commit.
        'a= RS.EOF
        
		myvar = rsjhs("query")
        'response.Write RS.Source (debug)
        'response.end (debug)
        
			
			
			structWhere = Session("SearchData" & "FullStrWhere" & dbkey & formgroup)
			
			'structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 0",session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup))
			'structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 1",session("SearchData" & "DRUGDEG_DEG_BASE64.Structure" & dbkey & formgroup))
			
			structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 0","SELECT query from cscartridge.tempQueries where id = 86")
			structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 1","SELECT query from cscartridge.tempQueries where id = 86")
			
			structWhere = replace(structWhere,"DRUGDEG_DEG_BASE64.BASE64_CDX","DRUGDEG_BASE64.BASE64_CDX")
			
			sSQL = sSQL & " and " & structWhere
		end if
		sSQL = sSQL & " order by DEG_COND_TEXT"
		'Response.Write sSQL
		Set rsBase64Deg = Server.CreateObject( "ADODB.Recordset" )
		rsBase64Deg.Open sSQL, connChem
		connChem.CommitTrans()
'-- CSBR ID:133589 & 133592
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Added the IF condition to show the matching degradants table only if matching degradant records are found; Moved the code to here from above
'-- Date: 11/12/2010
		if not rsBase64Deg.EOF then
			' We are to show structures.
		%>
		
			<%if ( false = bPrinterFriendly ) then
				' Not the printer-friendly form, so we don't need to set the table width.
			%>
				
				<table  border = 1  cellpadding = 2  cellspacing = 0 width = "90%">
			<%
			else
				' The printer-friendly form: set the width to a safe value for a printed page.
			%>
				<table  border = 1  cellpadding = 2  cellspacing = 0  width = 550>
			<%
			end if%>
				<tr>
			<%if ( false = bPrinterFriendly ) then%>
				<td valign=top width=90>Matching<br>Degradants<br><img src="/graphics/spacer.gif" width=80 height=1></td>	
			<%end if%>
			<td   align = "center"  valign = "center" width="100%">
				<table border=0>
					<tr>
						<%coltotal = 0
						while not rsBase64Deg.EOF
							coltotal = coltotal + 1
							if coltotal > 3 then
								Response.Write "</tr><tr>"
								coltotal = 1
							end if
							%>
						<td align=center>
							<table border=1 cellpadding=5>
								<tr>
									<td valign="top" >
						
								<%
								parentPassThruURL = "/" & Application("appkey") & "/drugdeg/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&keyexpt=" & rsBase64Deg.Fields( "DEG_EXPT_FK" )%>
								<P><nobr>
								<script>
								getFormViewLink("<%=rsBase64Deg.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt_passthru=<%=rsBase64Deg.Fields( "DEG_EXPT_FK" )%>")
								</script>
								</nobr>
								
								<%
								if rsBase64Deg.Fields( "DEG_CMPD_KEY" ) <> "" then
								Set rsFunctionalGroups = Server.CreateObject( "ADODB.Recordset" )
								sSQL = "select  drugdeg_fgroups.DEG_FGROUP_TEXT from drugdeg_fgroups, drugdeg_degs, " & _
											" drugdeg_degsfgroup where " & _
											" drugdeg_degs.deg_cmpd_key = drugdeg_degsfgroup.deg_cmpd_id " & _
											" and drugdeg_fgroups.deg_fgroup_key = drugdeg_degsfgroup.deg_fgroup_id " & _
											" and DRUGDEG_DEGS.DEG_CMPD_KEY = " & rsBase64Deg.Fields( "DEG_CMPD_KEY" )
											'" and drugdeg_expts.expt_key = " & rsBase64Deg.Fields( "DEG_EXPT_FK" )
								rsFunctionalGroups.Open sSQL, connChem
								if not rsFunctionalGroups.EOF then
									Response.Write "<br><strong>Functional Groups:</strong><br>"
									while not rsFunctionalGroups.EOF
										Response.Write "<li>" & rsFunctionalGroups("DEG_FGROUP_TEXT") 
										rsFunctionalGroups.MoveNext
										
									wend
								end if
								rsFunctionalGroups.Close
								
								end if
								
								Response.Write "<BR><strong>Change in MW: </strong>" &  rsBase64Deg.Fields( "MW_AMT_CHG")
								if bShowStructs = true then
								Response.Write "<br>"
									ShowResult dbkey, "AddDegradant_form_group", rsBase64Deg, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea 
									
								end if
									
								sFormula = getShowCFWChemResult( dbkey, "AddDegradant_form_group", "Formula", "DRUGDEG_BASE64.BASE64_CDX", rsBase64Deg( "base64_mol_id" ), "raw", "1", "17" )
								sFormula = GetHTMLStringForFormula( sFormula )
								sFormula = StandardizeHTML(sFormula)
											
								Response.Write "<p align=center>"
								Response.Write( sFormula )
								
								sMW = getShowCFWChemResult( dbkey, "AddDegradant_form_group", "MolWeight", "DRUGDEG_BASE64.BASE64_CDX", rsBase64Deg( "base64_mol_id" ), "raw", "1", "17" )
								if sMW <> "" then
									Response.Write "<br><br>MW: "
									sMW = replace (sMW,"&nbsp;","")
									sMW = FormatNumber( sMW, 2 )
									Response.Write(  sMW )
								end if
								Response.Write "</p>"
								'get functional groups
								'sSQL = "Select * from 
								
								%>
									</td>
								</tr>
							</table>
						</td>
						<td>&nbsp;</td>
						<%rsBase64Deg.MoveNext
						wend
						if coltotal < 4 then
							while coltotal < 4
								Response.Write "<td>&nbsp;</td>"
								coltotal = coltotal  + 1
							wend
						end if%>
					</tr>
				</table>
			</td>
			</tr>
			</table>
		<%
		end if
'-- End of Change #133589 & 133592#; Added the corresponding END IF
		%>
		
	<%
	else
		Response.Write "</table>"
	end if 'degradant search fields
	
		
	if ( false = bPrinterFriendly ) then
		' Not the printer-friendly form, so we don't need to set the table width.
	%>
	<br clear="all">
	<table  border = 1  cellpadding = 2  cellspacing = 0 width = "90%">
	<%
	else
		' The printer-friendly form: set the width to a safe value for a printed page.
	%>
	<table  border = 1  cellpadding = 2  cellspacing = 0  width = 550>
	<%
	end if
	
	CloseRS( rsBase64 )
	CloseRS( rsParent )
	CloseConn( connChem )
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</table>

<br>  <!-- For a little more space. -->

</body>

</html>
