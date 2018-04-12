<%@ LANGUAGE=VBScript %>
<html>

<head>
	<script language="javascript">
		var baseid = ""
	</script>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<%
	'MRE 11/15/04
	'included somewhere else
	'INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"
	
	

	%>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<%
' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )


' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' LJB 4/9/2001: changed names slightly
' CAP 26 Apr 2001: Added the test of formmode to allow cancelled expt. edit sessions
'     to return to the _display_ of expt. details.
if "edit_record" <> LCase( formmode ) then
	Session( "ReturnToExperimentDetails" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup ) & "&indexvalue=" & request("indexvalue")
end if

' Set variables for the height and width of the structure fields.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 230
widthStructArea = 230

'used to store parent rgrop status
bRGroup = false

' Get the experiment ID for the current experiment, since we use it rather a lot.
	keyExpt = Request.QueryString( "keyexpt" )
	
	' Connect to the database.
	set connDrugDeg = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )

	'this will allow direct links back in from docmanager....
	'i don't know if it is really the right thing but it works
	if keyExpt = "" then
		keyExpt = Session("PrimaryKeyDRUGDEG")
	end if

	' Make and fill a record set for the experiment.
	set rsExperiment = Server.CreateObject( "ADODB.Recordset" )
	rsExperiment.Open "Select * from DRUGDEG_EXPTS where EXPT_KEY = " & keyExpt, connDrugDeg
	baseid = keyExpt
	Session( "EDIT_THIS_RECORD" & dbkey ) = IsRecordEditable("DRUGDEG_EXPTS", "EXPT_KEY", keyExpt, connDrugDeg)
	' Make sure you have the first record.  There shouldn't be more than one, but better to be sure...
	rsExperiment.MoveFirst

	'Session( "ReturnToParentDetails" & dbkey ) = "/" & Application("appkey") & "/drugdeg/Parent_details_form.asp?formgroup=base_form_group&dbname=DRUGDEG&formmode=edit&unique_id=" & request("indexvalue") &"&commit_type=&indexvalue=" & request("indexvalue") & "&PagingMove=next_record&BaseRecordCount=" & Session("Base_RSRecordCount" & dbkey & "base_form_group") &"&form_change=true&time=51824.07&keyparent=" & rsExperiment.Fields( "PARENT_CMPD_FK" )
	
%>
	<script language="javascript">
		var baseid = <%=baseid%>
		var Edit_This_Record = "<%=Session( "EDIT_THIS_RECORD" & dbkey )%>"
	</script>
<%

' Set up a session variable for the primary key of the object (experiment) being displayed.
Session( "PrimaryKey" & dbkey ) = keyExpt



set rsParent = Server.CreateObject( "ADODB.Recordset" )
rsParent.Open "Select * from DRUGDEG_PARENTS where PARENT_CMPD_KEY = " & rsExperiment.Fields( "PARENT_CMPD_FK" ), connDrugDeg
if rsParent.BOF and rsParent.EOF then
	' There is no parent record!
%>			&nbsp;	<%
else
	' There is a parent record: add its info to this new sub-table.
	rsParent.MoveFirst
	
	
	Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
	rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg
						
	bRGroup = hasRgroup(sFormulaOriginal)
	CloseRS( rsBase64 )
end if						
rsParent.close
						
'add some helper text for the wizard
wizardStep = session("wizard")

select case lcase(wizardStep)
	case "wiz_add_exp_2"
		%>
		<p>The experiment has been added. What would you like to do next?
	<%case "wiz_edit_deg_1"%>
		<p>The degradant has been edited. What would you like to do next?	
	<%case "wiz_edit_deg_2"
		session("wizard")="wiz_edit_deg_1"%>
	<p>Editing a Degradation compound.
		<ol>
			<li>Select Parent compound
			<li>Select Experiment
			<li class="highlightcopy">Select Degradant
				<ul>
					<li>Choose a degradant to edit
					<li>If there are none available, you may add one to this expirement
				</ul>
		</ol>
	<%case else
		Session("wizard") = "wiz_add_par_2"
	
end select

if wizardStep <> "" then
	if "edit_record" <> LCase( formmode ) then
		' Allow the user to add a degradant only when not editting the experiment.
		if true = CBool( Session( "dd_Add_Records" & dbkey ) ) and true = CBOOL(Session( "EDIT_THIS_RECORD" & dbkey )) then
			' Allow the user to add a degradant only when authorized to do so. and when this record is not approved
	%>
	<p><a class="headinglink" href="Experiment_details_form.asp?dbname=drugdeg&amp;formgroup=AddExperiment_form_group&amp;formmode=edit_record&amp;formgroupflag_override=Experiment_details&amp;keyexpt=<%=keyExpt%>&amp;form_change=false">Edit Experiment</a>
	<br>Edit the experiment details
	
	<p><a class="headinglink" href="AddDegradant_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDegradant_form_group&amp;formmode=add_degradant&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag_override=<%="add_compounds"%>&amp;keyexpt=<%=keyExpt%>">Add a Degradant to this experiment</a>
		<br>Use this link to add a degradant to this experiment.
	<%
			Set rsDegradants = Server.CreateObject( "ADODB.Recordset" )
			sSQL = "SELECT * FROM DrugDeg_Degs WHERE Deg_Expt_FK = " & keyExpt & " ORDER BY DrugDeg_Degs.Mol_ID"
			rsDegradants.Open sSQL, connDrugDeg
				
			endTable = ""
			if ( ( not rsDegradants.BOF ) and ( not rsDegradants.EOF ) ) then
				' There are degradants, so make sure we start with the first.
				rsDegradants.MoveFirst
				Response.Write "<p><span class=""heading"">Degradants in this experiment</span>"
				Response.Write "<table>"
				endTable = "</table>"
			end if
				
			degCounter = 1
			while not rsDegradants.EOF
				
				sIdentifier = ""

				if rsDegradants.Fields( "ALIAS" ) <> "" then
					sIdentifier = rsDegradants.Fields( "ALIAS" )
				else
					sIdentifier = rsDegradants("compound_number")
				end if
					
				if isnull(sIdentifier) or sIdentifier = "" or sIdentifier= "-" then
					sIdentifier = "Degradant (Compound Id " & rsDegradants.Fields( "MOL_ID" ) & ")"
					degCounter = degCounter + 1
				end if
				sIdentifier = sIdentifier & " "
				
			%>
				<tr><td><%=sIdentifier%></td><td><a href="Degradant_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDegradant_form_group&amp;formmode=edit_record&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag_override=Degradant_details&amp;keydeg=<%=rsDegradants.Fields( "DEG_CMPD_KEY" )%>&amp;form_change=false&amp;wizard=wiz_edit_deg_1">Edit Degradant</a></td>

			<%
				' We need to determine whether there is already a mechanism for this degradant.
				Set rsMechanism = Server.CreateObject( "ADODB.Recordset" )
				rsMechanism.Open "Select * from DRUGDEG_MECHS where DEG_CMPD_FK = " & rsDegradants.Fields( "DEG_CMPD_KEY" ), connDrugDeg
				if rsMechanism.BOF and rsMechanism.EOF then
					' There is no mechanism for this degradant.
					if true = CBool( Session( "dd_Add_Records" & dbkey ) ) and true = CBOOL(Session( "EDIT_THIS_RECORD" & dbkey )) then
						' There isn't already a mechanism for this degradant and the user is
						' authorized to add records and the experiment is not approved. Put up the "Add" button.
			%>
						<td>&nbsp;&nbsp;<a href="AddMechanism_input_form.asp?dbname=<%=dbkey%>&amp;formmode=add_mechanism&amp;formgroup=AddMechanism_form_group&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag=add_mechanism&amp;formgroupflag_override=add_mechanism&amp;dataaction=mechanism_full_commit&amp;record_added=false&amp;keydeg=<%=rsDegradants.Fields( "DEG_CMPD_KEY" )%>">Add Mechanism</a></td></tr>
			<%
					end if  ' if the user is authorized to add records...
				else
					' There is a mechanism for producing this degradant.
					sMechDetails = "Mechanism_details_form.asp?dbname=" & dbkey & "&formgroup=AddMechanism_form_group&formmode=edit&parent_rgroup=" & bRGroup &"&formgroupflag_override=Mechanism_details&keydeg=" & rsDegradants.Fields( "DEG_CMPD_KEY" )
					sCallString = MakeQueryStringBeDifferent( sMechDetails )
			%>
					<td>&nbsp;&nbsp;<a href="<%=sCallString%>">View Mechanism</a></td></tr>
			<%
				end if  ' if there is a mechanism for this degradant...
				CloseRS( rsMechanism )
						
				rsDegradants.MoveNext
			wend
			rsDegradants.close
			response.write endTable
		end if  ' if authorized to add records...
	end if  ' if not editting the experiment...
	%>
	<p><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
			<br>Return here if you would like to add something else
<%end if 'wizardStep <> "" %>

<!-- Create a table which will hold the sub-tables for experiment data and parent info. -->
<table border="0" bordercolor="red">
	<tr>
		<td valign="top">

			<!-- The sub-table on the left side will hold the experiment data. -->
			<table border="2" bordercolor="gray" width="400">
				<!-- The status. -->
				<tr>
					<td align="center">
						<nobr>
						<strong>Status:</strong>
						<%
						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the status.
							
							Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
							sSQL = "Select STATUS_TEXT from DRUGDEG_STATUSES where STATUS_KEY =" & rsExperiment.Fields( "STATUS" )

							rsDisplayStatus.Open sSQL, connDrugDeg
							Response.Write	rsDisplayStatus.Fields( "STATUS_TEXT" )
							CloseRS( rsDisplayStatus )
						else
							'only let them change if they have permission.
							if CBool( Session( "DD_APPROVE_RECORDS" & dbkey )) = True then
								' Editting, so put up the full-blown drop-down menu.
								sStatusText = getStatusText (rsExperiment.Fields("STATUS"), connDrugDeg)
							
								on error resume next
								Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
							
								sStatusList = GetStatusList( connDrugDeg )
								'	Response.Write( "sStatusList = [ " & sStatusList & " ]" )
								ShowLookUpList dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.STATUS", sStatusList, rsExperiment.Fields("STATUS"), sStatusText, 0, false, "value", rsExperiment.Fields("STATUS")
							else
								'just display the status
								Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
								sSQL = "Select STATUS_TEXT from DRUGDEG_STATUSES where STATUS_KEY =" & rsExperiment.Fields( "STATUS" )

								rsDisplayStatus.Open sSQL, connDrugDeg
								Response.Write	rsDisplayStatus.Fields( "STATUS_TEXT" )
								CloseRS( rsDisplayStatus )
							end if
							%>
							<input type="hidden" name="old_status" value="<%=rsExperiment.Fields( "STATUS" )%>">
							<input type="hidden" name="dataaction2" value="SEND_EMAIL">
							<input type="hidden" name="sendemail" value="SEND_EMAIL">
							<%
						end if
						%>
						</nobr>
					</td>
				</tr>
				
				<!-- The degradation type. -->
				<tr>
					<td align="center">
						<nobr>
						<strong>Degradation:</strong>
						<%
						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the degradation type.
							set rsDegType = Server.CreateObject( "ADODB.Recordset" )
							rsDegType.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiment.Fields( "DEG_COND_FK" ), connDrugDeg
						%>
						<%=rsDegType.Fields( "DEG_COND_TEXT" )%>
						<%
							CloseRS( rsDegType )
						else
							' Editting, so put up the full-blown drop-down menu.
							sCondsList = GetDegCondsList(connDrugDeg)
							ShowLookUpList dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.DEG_COND_FK", sCondsList, rsExperiment.Fields("DEG_COND_FK"), "selected", 0, true, "value", rsExperiment.Fields( "DEG_COND_FK" )
							
						end if
						%>
						</nobr>
					</td>
				</tr>

				<!-- Put in a little vertical space. -->
				<tr>
					<td height="20">
					</td>
				</tr>

				<tr>
<!--					<td  align = "center">						Experimental conditions						<br>						<% ShowResult dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50" %>					</td>-->
					<td>
						<center>
							<strong>Experimental conditions</strong>
						</center>
						<br>
						<%
						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the experimental conditions.
							Response.Write( rsExperiment.Fields( "EXPT_CONDS" ) )
						else
							' Editting, so put up a text area.
						%>
							<center>
						<%
							ShowResult dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50"
						%>
							</center>
						<%
						end if
						%>
					</td>
				</tr>

				<!-- Put in a little vertical space. -->
				<tr>
					<td height="20">
					</td>
				</tr>
				
				<tr>
				
					<td>
						<center>
							<strong>API state</strong>
						</center>
						<br>
						<%
						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the experimental conditions.
							Response.Write( rsExperiment.Fields( "API_FRM" ) )
						else
							' Editting, so put up a text area.
						%>
							<center>
						<%
							
							sAPIFormulatedList = """API"":API,""Formulated product"":Formulated product,""API Excipient blend"":API Excipient blend"
							ShowLookUpList dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.API_FRM", sAPIFormulatedList, """" & rsExperiment.Fields( "API_FRM" ) & """", rsExperiment.Fields( "API_FRM" ), 0, false, "value", "API"
								
							'ShowResult dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50"
						%>
							</center>
						<%
						end if
						%>
					</td>
				</tr>

				<!-- Put in a little vertical space. -->
				<tr>
					<td height="20">
					</td>
				</tr>

				<tr>
					<td>
						<center>
							<strong>Notes</strong>
						</center>
						<br>
						<%
						'  Why this code?  We already have a record set for the experiment.
						'set rsExperimentNotes = Server.CreateObject( "ADODB.Recordset" )
						'rsExperimentNotes.Open "Select * from DRUGDEG_EXPTS where EXPT_KEY = " & keyExpt, connDrugDeg
						'rsExperimentNotes.MoveFirst

						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the experimental conditions.
							'Response.Write( rsExperimentNotes.Fields( "NOTES" ) )
							Response.Write( rsExperiment.Fields( "NOTES" ) )
						else
							' Editting, so put up a text area.
							if rsExperiment("NOTES") <> "" then
								strNotes = replace(rsExperiment("NOTES"),"""","&quot;")
							else
								strNotes = ""
							end if
						%>
						<center>							
									<script language="javascript">
			addRelLoaded("UID.<%=keyExpt%>:DRUGDEG_EXPTS.NOTES")
		</script>
	<input type="hidden"  name="UID.<%=keyExpt%>:DRUGDEG_EXPTS.NOTES" value="<%=strNotes%>"><TEXTAREA name="UID.<%=keyExpt%>:DRUGDEG_EXPTS.NOTES_orig" cols="50" rows="7" wrap="soft"  onBlur="UpdateFieldVal(this.form,this)"><%=strNotes%></textarea>

						
						<%
						'	ShowResult dbkey, formgroup, rsExperimentNotes, "DRUGDEG_EXPTS.NOTES", "TEXTAREA", "6", "50"
						%>
						</center>
						<%
						end if

						'CloseRS( rsExperimentNotes )
						%>
					</td>
				</tr>

				<!-- Put in a little vertical space. -->
				<tr>
					<td height="20">
					</td>
				</tr>

				<tr>
<!--					<td  align = "center">						References						<br>						<% ShowResult dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.REFERENCES", "TEXTAREA", "4", "50" %>					</td>-->
					<td>
						<center>
							<strong>References</strong>
						</center>
						<br>
						<%
						if "edit_record" <> LCase( formmode ) then
							' Displaying, so just write the experimental conditions.
							Response.Write( rsExperiment.Fields( "REFERENCES" ) )

						else
							' Editting, so put up a text area.
							if rsExperiment("REFERENCES") <> "" then
								strRef = replace(rsExperiment("REFERENCES"),"""","&quot;")
							else
								strRef = ""
							end if
						%>
							<center>							
									<script language="javascript">
			addRelLoaded("UID.<%=keyExpt%>:DRUGDEG_EXPTS.REFERENCES")
		</script>
	<input type="hidden"  name="UID.<%=keyExpt%>:DRUGDEG_EXPTS.REFERENCES" value="<%=strRef%>"><TEXTAREA name="UID.<%=keyExpt%>:DRUGDEG_EXPTS.REFERENCES_orig" cols="50" rows="7" wrap="soft"  onBlur="UpdateFieldVal(this.form,this)"><%=strRef%></textarea>

						<%
							'ShowResult dbkey, formgroup, rsExperiment, "DRUGDEG_EXPTS.REFERENCES", "TEXTAREA", "7", "50"
						%>
							</center>
						Sample reference:<br>
						Waterman, K. C.; Adami, R. C.; Alsante, K. M.; Antipas, A. S.; Arenson, D. R.; Carrier, R.; Hong, J.; Landis, M. S.; Lombardo, F.; Shah, J. C.; Shalaev, E.; Smith, S. W.; Wang, H. &quot;Hydrolysis in Pharmaceutical Formulations,&quot; <i>Pharmaceutical Development and Technology</i>, 2002, 7(2), 113-146. 
						<%
						end if
						%>
					</td>
				</tr>
			</table>
		</td>


		<td valign="top">
			<!-- The sub-table on the right side will hold some parent info. -->
<%
			' Make and fill a record set for the parent on which the experiment was performed.
			set rsParent = Server.CreateObject( "ADODB.Recordset" )
			rsParent.Open "Select * from DRUGDEG_PARENTS where PARENT_CMPD_KEY = " & rsExperiment.Fields( "PARENT_CMPD_FK" ), connDrugDeg
			if rsParent.BOF and rsParent.EOF then
				' There is no parent record!
%>			&nbsp;	<%
			else
				' There is a parent record: add its info to this new sub-table.
				rsParent.MoveFirst
%>			<table border="2" bordercolor="gray">
				<tr>
					<td align="center">
						Parent compound
						<br>
<%
						' Show the structure of the parent compound.
						Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
						rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg
						ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea

						' Get the molecular formula.
						sFormulaOriginal = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
						' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
						bRGroup = hasRgroup(sFormulaOriginal)
						CloseRS( rsBase64 )
						
						
%>

					</td>
				</tr>

				<tr>
					<!-- One row (no borders, so one cell) for the list of degradation experiments. -->
					<td valign="top">
						Degradation experiments:

						<!-- Show the degradation experiments for this parent compound. -->
<%
				' Make and fill a record set for the experiments done on this parent.
				set rsParentExpts = Server.CreateObject( "ADODB.Recordset" )
				rsParentExpts.Open "Select * from DRUGDEG_EXPTS where PARENT_CMPD_FK = " & rsExperiment.Fields( "PARENT_CMPD_FK" ) & " order by EXPT_KEY", connDrugDeg

				rsParentExpts.MoveFirst
				nOtherExpt = 0
				while not rsParentExpts.EOF
					' Make and fill a record set for the degradation type.
					set rsDegType = Server.CreateObject( "ADODB.Recordset" )
					rsDegType.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsParentExpts.Fields( "DEG_COND_FK" ), connDrugDeg

					'if CInt( rsParentExpts.Fields( "EXPT_KEY" ) ) = CInt( keyExpt ) then
					if CLng(rsParentExpts.Fields("EXPT_KEY")) = CLng(keyExpt) then
					
						' This is the current experiment.  Display the text for the degradation
						' type to keep this experiment in context with the others.
%>
						<br>
						<nobr>
						&nbsp; &nbsp; &nbsp;
						(<% Response.Write( rsDegType.Fields( "DEG_COND_TEXT" ) ) %>)
						</nobr>
<%
					else
						' This is an other experiment.

						' LJB NOTE1: add information to the querystring that will make the
						'     standard cows source work and allow you to modify the navbar.asp
						'     file to get your buttons.
%>
						<br>
						<nobr>
						&nbsp; &nbsp; &nbsp; <%
						if "edit_record" <> LCase( formmode ) then
							' Permit looking at other experiments only when the
							' user is not editting the experiment information.
						%>
						<a href="Experiment_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=<%=formgroup%>&amp;formmode=edit&amp;formgroupflag_override=<%="Experiment_details"%>&amp;keyexpt=<% Response.Write( rsParentExpts.Fields( "EXPT_KEY" ) ) %>"><%
						end if

						Response.Write( rsDegType.Fields( "DEG_COND_TEXT" ) )

						if "edit_record" <> LCase( formmode ) then
							' Close up the <a> if we used it.
						%></a>
						<%
						end if
						%>
						</nobr>
<%
					end if  ' if an other experiment ...

					' Close the record set for the experiment type.
					CloseRS( rsDegType )

					' Get the next parent experiment record.
					rsParentExpts.MoveNext
				wend

				' Close the record set for the experiments on the parent.
				CloseRS( rsParentExpts )
%>
					</td>
				</tr>
			</table>
<%			end if  ' if there is a parent record ...
			CloseRS( rsParent )
%>		</td>
	</tr>

</table>

<p> &nbsp;  <!-- Just to put a little space in. -->


<!-- Now the degradants. -->
<%
maxCmpdsInRow = 3
widthDegStructDrawing = 210
heightDegStructDrawing = 210
%><table border="1" bordercolor="gray">
	<tr>
		<td align="center" colspan="<% Response.Write( maxCmpdsInRow ) %>">
			Degradant compounds
		</td>
	</tr>

<%
Set rsDegradants = Server.CreateObject( "ADODB.Recordset" )
sSQL = "SELECT * FROM DrugDeg_Degs WHERE Deg_Expt_FK = " & keyExpt & " ORDER BY DrugDeg_Degs.Mol_ID"
rsDegradants.Open sSQL, connDrugDeg

if ( ( not rsDegradants.BOF ) and ( not rsDegradants.EOF ) ) then
	' There are degradants, so make sure we start with the first.
	rsDegradants.MoveFirst
end if
nCmpdInRow = 0
while not rsDegradants.EOF
	dbDegradantMolID = rsDegradants.Fields( "MOL_ID" )
	if 0 = nCmpdInRow then
		' We need to start a new row.
%>	<tr>
<%
	end if
%>
		<td align="center" valign="center">
<%
			' Show the structure of the degradant compound.
			Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
			rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsDegradants.Fields( "MOL_ID" ), connDrugDeg
			ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthDegStructDrawing, heightDegStructDrawing
			CloseRS( rsBase64 )

			
			sIdentifier = rsDegradants.Fields( "ALIAS" )
			
%>
			<br><%=sIdentifier%>
				<%if rsDegradants.Fields( "COMPOUND_NUMBER" ) <> "" then
					if sIdentifier <> "" then
						Response.Write "<br>"
					end if
					Response.Write rsDegradants.Fields( "COMPOUND_NUMBER" )
				end if  	%>		

<%

			if "edit_record" <> LCase( formmode ) then
				' Allow the user to access the degradant details only when not
				' editting the experiment.
%>
			<br>
			<a class="headinglink" href="Degradant_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDegradant_form_group&amp;formmode=edit&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag_override=Degradant_details&amp;keydeg=<%=rsDegradants.Fields( "DEG_CMPD_KEY" )%>">View Details</a><br>

<%
			' We need to determine whether there is already a mechanism for this degradant.
			Set rsMechanism = Server.CreateObject( "ADODB.Recordset" )
			rsMechanism.Open "Select * from DRUGDEG_MECHS where DEG_CMPD_FK = " & rsDegradants.Fields( "DEG_CMPD_KEY" ), connDrugDeg
			if rsMechanism.BOF and rsMechanism.EOF then
				' There is no mechanism for this degradant.
				if true = CBool( Session( "dd_Add_Records" & dbkey ) ) and true = CBOOL(Session( "EDIT_THIS_RECORD" & dbkey )) then
					' There isn't already a mechanism for this degradant and the user is
					' authorized to add records and the experiment is not approved. Put up the "Add" button.
%>
			<a class="headinglink" href="AddMechanism_input_form.asp?dbname=<%=dbkey%>&amp;formmode=add_mechanism&amp;formgroup=AddMechanism_form_group&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag=add_mechanism&amp;formgroupflag_override=add_mechanism&amp;dataaction=mechanism_full_commit&amp;record_added=false&amp;keydeg=<%=rsDegradants.Fields( "DEG_CMPD_KEY" )%>">Add Mechanism</a>
<%
				end if  ' if the user is authorized to add records...
			else
				' There is a mechanism for producing this degradant.
				sMechDetails = "Mechanism_details_form.asp?dbname=" & dbkey & "&formgroup=AddMechanism_form_group&formmode=edit&parent_rgroup=" & bRGroup &"&formgroupflag_override=Mechanism_details&keydeg=" & rsDegradants.Fields( "DEG_CMPD_KEY" )
				sCallString = MakeQueryStringBeDifferent( sMechDetails )
%>
			<a class="headinglink" href="<%=sCallString%>">View Mechanism</a>
<%
			end if  ' if there is a mechanism for this degradant...
			CloseRS( rsMechanism )
%>

<%
			end if  ' if the user is not editting the experiment...
%>
		</td>
<%
	nCmpdInRow = nCmpdInRow + 1
	if nCmpdInRow = maxCmpdsInRow then
%>
	</tr>
<%
		' Reset the counter.
		nCmpdInRow = 0
	end if

	rsDegradants.MoveNext
wend

if 0 <> nCmpdInRow then
	' We didn't finish the last row, so do so here.
	nCellInRow = nCmpdInRow
	while nCellInRow < maxCmpdsInRow
%>
		<td width="<% Response.Write( widthDegStructDrawing ) %>">
			&nbsp;
		</td>
<%
		nCellInRow = nCellInRow + 1
	wend
%>
	</tr>
<%
end if
%>
</table>
<p>

<%
' Close the record set for degradant compounds.
CloseRS( rsDegradants )

if "edit_record" <> LCase( formmode ) then
	' Allow the user to add a degradant only when not editting the experiment.
	if true = CBool( Session( "dd_Add_Records" & dbkey ) ) and true = CBOOL(Session( "EDIT_THIS_RECORD" & dbkey )) then
		' Allow the user to add a degradant only when authorized to do so. and when this record is not approved
%>
<a href="AddDegradant_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDegradant_form_group&amp;formmode=add_degradant&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag_override=<%="add_compounds"%>&amp;keyexpt=<% Response.Write( keyExpt ) %>">
	<img src="/<%=Application( "appkey" )%>/graphics/Button_AddDegradant.gif" border="0">
</a>
<%
	end if  ' if authorized to add records...
end if  ' if not editting the experiment...
%>

<p> &nbsp;  <!-- Just to put a little space below the above link. -->

<%

if "edit_record" <> LCase( formmode ) then
'if ThisAppIsIntegratedWith("DocManager") then
	if UCASE(Application("IntegratedWith")) = "DOCMANAGER" and Session("SEARCH_DOCS" & dbkey) then

		if CBool(Session("DD_APPROVE_RECORDS" & dbkey)) = true then
			if not bPrinterFriendly then
				showdelete = true
			else
				showdelete = false
			end if
		else 
			showdelete = false
		end if


		pData = "ReturnType=html&showlogo=true&extLinkID=" & rsExperiment.Fields( "EXPT_KEY" ) & "&LinkType=drugdegexptid&showsubmitdate=true&showsubmitter=true&showdelete=" & showdelete
		pData = pData & "&csusername=" & Session("username" & "Drugdeg")  & "&csuserid=" & Session("UserID" & "drugdeg")
		'Response.Write pData
		'pHostName = "dddev"
		pHostName = Application("DOCMANAGER_SERVER_NAME")
		pTarget = "/docmanager/docmanager/externallinks/getDocumentsNoGUI.asp"
		pUserAgent = "DrugDeg"
		pMethod = "POST"
				
		URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget
		
		' This is the server safe version from MSXML3.
		Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

		' Syntax:
		'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
		objXmlHttp.open pMethod, URL, False
		objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
		objXmlHttp.setRequestHeader "User-Agent", pUserAgent
		objXmlHttp.send pData

		' Print out the request status:
		StatusCode = objXmlHttp.status

		If StatusCode <> "200" then
			httpResponse = objXmlHttp.responseText
			'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
		Else
			httpResponse = objXmlHttp.responseText
		End If

		'httpResponse = CShttpRequest2(pMethod, pHostName, pTarget, pUserAgent, pData)


		Response.Write "<br><br>"
		
		if bPrinterFriendly then	
			Response.Write "<table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"">"
			Response.Write "<tr><td width=""600"">"
		end if
		Response.Write "<b>Associated Documents in Doc Manager</b>"	
		Response.Write "<br><br>"
		Response.write httpResponse
		
		if bPrinterFriendly then
			Response.Write "</td></tr></table>"
		end if
	

'Show Add button based on privileges
		if not bPrinterFriendly then	
			if true = CBool( Session( "DD_LINK_DOCUMENTS" & dbkey ) ) then
				' Allow the addition of links to documents for this parent compound,
				' _if_ the user is authorized to do so.
	%>
				<!-- Make a link to the display for adding a document link. -->

				<br><br>

				
				<script language="javascript">
					function launchDocMgrWindow(){
					window.open( '/docmanager/default.asp?dataaction=db&formgroup=base_form_group&dbname=docmanager&extAppName=drugdeg&LinkType=drugdegexptid&linkfieldname=ExptID&showselect=true&extlinkid=<%=rsExperiment.Fields("EXPT_KEY").value%>', 'docmgrwindow', 'toolbar=no,location=no,scrollbars=yes,width=800,height=600')
					}
				</script>
				<!---a href="AddDocumentLink_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDocLink_form_group&amp;formmode=add_doclink&amp;formgroupflag=add_doclink&amp;formgroupflag_override=add_doclink&amp;dataaction=add_doclink_full_commit&amp;record_added=false&amp;keyparent=<% Response.Write(rsExperiment.Fields( "EXPT_KEY")) %>"--->
				<a href="#" onclick="launchDocMgrWindow()"><img src="/<%=Application( "appkey" )%>/graphics/Button_AddDocLink.gif" border="0"></a>
	<%
			end if  ' if authorized to add records, ...
		end if
	end if
end if

' Close the record set for the experiments.
CloseRS( rsExperiment )

' Close the database connection.
connDrugDeg.Close
%>

<!-- START: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->
</form>
<%WriteAppletCode()%>

<form name="nav_variables" method="post" Action="<%=Session("CurrentLocation" & dbkey & formgroup)%>">
<input type="hidden" name="RecordRange" Value="<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type="hidden" name="CurrentRecord" Value="<%=Session("RecordRange" & dbkey & formgroup)%>">
<input type="hidden" name="AtStart" Value="<%=Session("AtStart" & dbkey & formgroup)%>">
<input type="hidden" name="AtEnd" Value="<%=Session("AtEnd" & dbkey & formgroup)%>">
<input type="hidden" name="Base_RSRecordCount" Value="<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type="hidden" name="TotalRecords" Value="<%=Session("Base_RSRecordCount" & dbkey & formgroup)%>">
<input type="hidden" name="PagingMove" Value="<%=Session("PagingMove" & dbkey & formgroup)%>">
<input type="hidden" name="CommitType" Value="<%=commit_type%>">
<input type="hidden" name="TableName" Value="<%=table_name%>">
<input type="hidden" name="UniqueID" Value="<%=uniqueid%>">
<input type="hidden" name="BaseID" Value="<%=BaseID%>">
<input type="hidden" name="CPDDBCounter" Value="<%=cpdDBCounter%>">
<input type="hidden" name="CurrentIndex" Value="<%=currentindex%>">
<input type="hidden" name="BaseActualIndex" Value="<%=BaseActualIndex%>">
<input type="hidden" name="Base64" Value="<%=Session("BASE64_CDX" & uniqueid & dbkey & formgroup)%>">
<input type="hidden" name="Stored_Location" Value>

</form>
<script language="javascript">
	window.onload = function(){loadframes()}
	function loadframes(){
		loadUserInfoFrame()
		//loadNavBarFrame()

		DoAfterOnLoad ? AfterOnLoad():true;
	}
</script>
<!-- END: MUST HAVE ALL OF THE FOLLOWING FROM THE FOOTER FILE SINCE NO RECORDSET IS BEING USED -->

</body>

</html>
