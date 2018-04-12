	<script language="javascript">
		var baseid = ""
	</script>
	
<%
' Set up a session variable for this as the current page displayed.
'Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' LJB 4/9/2001: changed names slightly
' CAP 26 Apr 2001: Added the test of formmode to allow cancelled expt. edit sessions
'     to return to the _display_ of expt. details.
'if "edit_record" <> LCase( formmode ) then
'	Session( "ReturnToExperimentDetails" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
'end if

' Set variables for the height and width of the structure fields.  This
' makes it easier to change them (no searching around in the code).

'used to store parent rgrop status
bRGroup = false

' Get the experiment ID for the current experiment, since we use it rather a lot.
keyExpt = Request.QueryString( "keyexpt" )

'Indexvalue - used to return to the proper place in the search results list 
unique_id = request("unique_id")
unique_id = Session("RecordRange" & dbkey & formgroup)

' Make and fill a record set for the experiment.
set rsExperiment = Server.CreateObject( "ADODB.Recordset" )
if keyExpt = "" then
	rsExperiment.Open "Select * from D3_EXPTS where PARENT_CMPD_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connD3
else
	rsExperiment.Open "Select * from D3_EXPTS where EXPT_KEY = " & keyExpt, connD3
end if
	
if rsExperiment.EOF or rsExperiment.BOF then%>
	<!--#INCLUDE FILE="Parent_details_form-Simple.asp"-->
<%else
	if keyExpt = "" then	
		keyExpt = rsExperiment("EXPT_KEY")
	end if
	

	baseid = keyExpt
	Session( "EDIT_THIS_RECORD" & dbkey ) = IsRecordEditable("D3_EXPTS", "EXPT_KEY", keyExpt, connD3)
	%>
		<script language="javascript">
			var baseid = <%=baseid%>
			var Edit_This_Record = "<%=Session( "EDIT_THIS_RECORD" & dbkey )%>"
		</script>
	<%
	' Make sure you have the first record.  There shouldn't be more than one, but better to be sure...
	rsExperiment.MoveFirst


	' Set up a session variable for the primary key of the object (experiment) being displayed.
	'Session( "PrimaryKey" & dbkey ) = keyExpt



	'set rsParent = Server.CreateObject( "ADODB.Recordset" )
	'rsParent.Open "Select * from D3_PARENTS where PARENT_CMPD_KEY = " & rsExperiment.Fields( "PARENT_CMPD_FK" ), connD3
	if rsBase64.BOF and rsBase64.EOF then
		' There is no parent record!
	%>			&nbsp;	<%
	else
		' There is a parent record: add its info to this new sub-table.
		rsBase64.MoveFirst

							
		bRGroup = hasRgroup(sFormulaOriginal)
		
	end if						

							
	%>
	<!-- Create a table which will hold the sub-tables for experiment data and parent info. -->
	<table border="0" bordercolor="red">
		<tr>
			<td valign="top">

				<!-- The sub-table on the left side will hold the experiment data. -->
				<table border="2" bordercolor="gray" width="400">
					<%if CBool(Session("DD_APPROVE_RECORDS" & dbkey)) = true then%>
					<!-- The status. -->
					<tr>
						<td align="center">
							<nobr>
							<strong>Status:</strong>
							<%
							if "edit_record" <> LCase( formmode ) then
								' Displaying, so just write the status.
								
								Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
								sSQL = "Select STATUS_TEXT from D3_STATUSES where STATUS_KEY =" & rsExperiment.Fields( "STATUS" )

								rsDisplayStatus.Open sSQL, connD3
								Response.Write	rsDisplayStatus.Fields( "STATUS_TEXT" )
								CloseRS( rsDisplayStatus )
							else
								'only let them change if they have permission.
								if CBool( Session( "DD_APPROVE_RECORDS" & dbkey )) = True then
									' Editting, so put up the full-blown drop-down menu.
									sStatusText = getStatusText (rsExperiment.Fields("STATUS"), connD3)
								
									on error resume next
									Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
								
									sStatusList = GetStatusList( connD3 )
									'	Response.Write( "sStatusList = [ " & sStatusList & " ]" )
									ShowLookUpList dbkey, formgroup, rsExperiment, "D3_EXPTS.STATUS", sStatusList, rsExperiment.Fields("STATUS"), sStatusText, 0, false, "value", rsExperiment.Fields("STATUS")
								else
									'just display the status
									Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
									sSQL = "Select STATUS_TEXT from D3_STATUSES where STATUS_KEY =" & rsExperiment.Fields( "STATUS" )

									rsDisplayStatus.Open sSQL, connD3
									Response.Write	rsDisplayStatus.Fields( "STATUS_TEXT" )
									CloseRS( rsDisplayStatus )
								end if
								%>
								<input type="hidden" name="old_status" value="<%=rsExperiment.Fields( "STATUS" )%>">
								<input type="hidden" name="dataaction2" value="SEND_EMAIL">
								<%
							end if
							%>
							</nobr>
						</td>
					</tr>
					<%end if%>
					<!-- The degradation type. -->
					<tr>
						<td align="center">
							<nobr>
							<strong>Degradation:</strong>
							<%
							if "edit_record" <> LCase( formmode ) then
								' Displaying, so just write the degradation type.
								set rsDegType = Server.CreateObject( "ADODB.Recordset" )
								rsDegType.Open "Select * from D3_CONDS where DEG_COND_KEY = " & rsExperiment.Fields( "DEG_COND_FK" ), connD3
							%>
							<%=rsDegType.Fields( "DEG_COND_TEXT" )%>
							<%
								CloseRS( rsDegType )
							else
								' Editting, so put up the full-blown drop-down menu.
								sCondsList = GetDegCondsList(connD3)
								ShowLookUpList dbkey, formgroup, rsExperiment, "D3_EXPTS.DEG_COND_FK", sCondsList, rsExperiment.Fields("DEG_COND_FK"), "selected", 0, true, "value", rsExperiment.Fields( "DEG_COND_FK" )
								
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
	<!--					<td  align = "center">						Experimental conditions						<br>						<% ShowResult dbkey, formgroup, rsExperiment, "D3_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50" %>					</td>-->
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
								ShowResult dbkey, formgroup, rsExperiment, "D3_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50"
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
								ShowLookUpList dbkey, formgroup, rsExperiment, "D3_EXPTS.API_FRM", sAPIFormulatedList, """" & rsExperiment.Fields( "API_FRM" ) & """", rsExperiment.Fields( "API_FRM" ), 0, false, "value", "API"
									
								'ShowResult dbkey, formgroup, rsExperiment, "D3_EXPTS.EXPT_CONDS", "TEXTAREA", "2", "50"
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
							set rsExperimentNotes = Server.CreateObject( "ADODB.Recordset" )
							rsExperimentNotes.Open "Select * from D3_EXPTS where EXPT_KEY = " & keyExpt, connD3
							rsExperimentNotes.MoveFirst

							if "edit_record" <> LCase( formmode ) then
								' Displaying, so just write the experimental conditions.
								Response.Write( rsExperimentNotes.Fields( "NOTES" ) )
							else
								' Editting, so put up a text area.
							%>
							<center><%
								ShowResult dbkey, formgroup, rsExperimentNotes, "D3_EXPTS.NOTES", "TEXTAREA", "6", "50"
							%>
							</center>
							<%
							end if

							CloseRS( rsExperimentNotes )
							%>
						</td>
					</tr>

					<!-- Put in a little vertical space. -->
					<tr>
						<td height="20">
						</td>
					</tr>

					<tr>
	<!--					<td  align = "center">						References						<br>						<% ShowResult dbkey, formgroup, rsExperiment, "D3_EXPTS.REFERENCES", "TEXTAREA", "4", "50" %>					</td>-->
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
							%>
								<center>
							<%
								ShowResult dbkey, formgroup, rsExperiment, "D3_EXPTS.REFERENCES", "TEXTAREA", "4", "50"
							%>
								</center>
							Sample reference:<br>
							Waterman, K. C.; Adami, R. C.; Alsante, K. M.; Antipas, A. S.; Arenson, D. R.; Carrier, R.; Hong, J.; Landis, M. S.; Lombardo, F.; Shah, J. C.; Shalaev, E.; Smith, S. W.; Wang, H. &quot;Hydrolysis in Pharmaceutical Formulations,&quot; <i>PharmaceuticalD evelopment and Technology</i>, 2002, 7(2), 113-146. 
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
							rsBase64.Open "Select * from D3_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connD3
							ShowResult dbkey, formgroup, rsBase64, "D3_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea

							' Get the molecular formula.
							sFormulaOriginal = getShowCFWChemResult( dbkey, formgroup, "Formula", "D3_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
							' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
							bRGroup = hasRgroup(sFormulaOriginal)
							CloseRS( rsBase64 )
							
	%>

						</td>
					</tr>
	<%
					set rsParentExpts = Server.CreateObject( "ADODB.Recordset" )
					rsParentExpts.Open "Select * from D3_EXPTS where PARENT_CMPD_FK = " & rsExperiment.Fields( "PARENT_CMPD_FK" ) & " order by EXPT_KEY", connD3

					if not rsParentExpts.eod and not rsParentExpts.BOF then
						rsParentExpts.MoveFirst
		%>
						<tr>
							<!-- One row (no borders, so one cell) for the list of degradation experiments. -->
							<td valign="top">
								Degradation experiments:

								<!-- Show the degradation experiments for this parent compound. -->
		<%
						' Make and fill a record set for the experiments done on this parent.
						
						nOtherExpt = 0
						while not rsParentExpts.EOF
							' Make and fill a record set for the degradation type.
							set rsDegType = Server.CreateObject( "ADODB.Recordset" )
							rsDegType.Open "Select * from D3_CONDS where DEG_COND_KEY = " & rsParentExpts.Fields( "DEG_COND_FK" ), connD3

							if CInt( rsParentExpts.Fields( "EXPT_KEY" ) ) = CInt( keyExpt ) then
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
								nOtherExpt = nOtherExpt + 1
		%>
								<br>
								<nobr>
								&nbsp; &nbsp; &nbsp; <%
								if "edit_record" <> LCase( formmode ) then
									' Permit looking at other experiments only when the
									' user is not editting the experiment information.
									parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&unique_id&"&commit_type=&keyexpt=" & rsParentExpts.Fields( "EXPT_KEY" )
						%>
									<nobr>
									<script>
										getFormViewLink("<%=rsDegType.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt=<%=rsParentExpts.Fields( "EXPT_KEY" )%>")
									</script>
									</nobr>
									<!-- <a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')"> -->
								
									<!--<a href="Experiment_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=<%=formgroup%>&amp;formmode=edit&amp;formgroupflag_override=<%="Experiment_details"%>&amp;keyexpt=<% Response.Write( rsParentExpts.Fields( "EXPT_KEY" ) ) %>">--><%
								else
									Response.Write( rsDegType.Fields( "DEG_COND_TEXT" ) )
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
						if nOtherExpt > 0 then
							parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&unique_id&"&commit_type=&degs=true"
						%>
									<p><nobr>
									<script>
										getFormViewLink("View all degradants for this parent","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&degs=true")
									</script>
									<!--<a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">									View all degradants for this parent</a>-->
						<%end if%>
							</td>
						</tr>
					<%end if%>
				</table>
	<%			end if  ' if there is a parent record ...
				
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
	sSQL = "SELECT * FROM D3_Degs WHERE Deg_Expt_FK = " & keyExpt & " ORDER BY D3_Degs.Mol_ID"
	rsDegradants.Open sSQL, connD3

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
				Set rsBase64_deg = Server.CreateObject( "ADODB.Recordset" )
				rsBase64_deg.Open "Select * from D3_BASE64 where MOL_ID = " & rsDegradants.Fields( "MOL_ID" ), connD3
				ShowResult dbkey, formgroup, rsBase64_deg, "D3_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthDegStructDrawing, heightDegStructDrawing
				sMW = ""
				sFormula = ""
				' Get the molecular weight.
				sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "D3_BASE64.MolWeight", rsBase64_deg( "MOL_ID" ), "raw", "1", "17" )
				' Keep only two decimal places for the molecular weight.
				' 2002-09-17: FormatNumber doesn't seem to work for the molecular weight I get using the new
				' COWS core: I can't explain why, I tested everything I could to figure it.  Anyway, only
				' thing I found that worked was to make my own formmating function.
				'sMW = MyFormatNumber( sMW, 4 )
				
				' Get the molecular formula.
				sFormulaOriginal = getShowCFWChemResult( dbkey, formgroup, "Formula", "D3_BASE64.BASE64_CDX", rsBase64_deg( "MOL_ID" ), "raw", "1", "17" )
				' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
				
				'sFormula = GetHTMLStringForFormula( sFormulaOriginal )
				sFormula = sFormulaOriginal
				CloseRS( rsBase64_deg )


				sIdentifier = rsDegradants.Fields( "ALIAS" )

	%>
				<table>
					<tr>
						<td colspan="2" align="center"><strong><%=sIdentifier%></strong></td>
					</tr>
					<tr>
						<td><strong>Formula </strong></td>
						<td><%=sFormula%></td>
					</tr>
					<tr>
						<td><strong>MW </strong></td>
						<td><%

						if sMW <> "" then
							'Response.Write "MW: "
							sMW = replace (sMW,"&nbsp;","")
							sMW = FormatNumber( sMW, 2 )
							Response.Write(  sMW )
						end if
						%></td>
					</tr>

					<%if "" <> rsDegradants.Fields( "DEG_EXPT_FK" ) then%>
					<tr>
						<td><strong>Compound&nbsp;ID</strong></td>
						<td><%= rsDegradants.Fields("mol_id")%></td>
					</tr>
					<%end if%>
					<%if "" <> rsDegradants.Fields( "MW_AMT_CHG" ) then%>
					<tr>
						<td><strong>Change in MW </strong></td>
						<td><%= rsDegradants.Fields("MW_AMT_CHG")%></td>
					</tr>
					<%end if%>
					<%
					current_deg_fgroup_list=GetCurrentFunctionalGroups(dbkey, formgroup,  connD3,  "Deg_FGroup_Key", "Deg_FGroup_text", "D3_FGroups.Deg_FGroup_text", rsDegradants.Fields("DEG_CMPD_KEY"))
					if "" <> current_deg_fgroup_list then%>
					<tr>
						<td valign="top"><strong><strong>Functional Groups</strong></strong></td>
						<td valign="top"><%if  "" <> current_deg_fgroup_list then
								temp_array = Split(current_deg_fgroup_list, ",", -1)
								for i = 0 to UBound(temp_array)
									temp_array2 = split(temp_array(i), ":", -1)
									sFGroup_Text = temp_array2(1)
									Response.Write sFGroup_Text & "<br>"
								next
							end if%></td>
					</tr>
					<%end if%>
				</table>
				
	<%

				if "edit_record" <> LCase( formmode ) then
					' Allow the user to access the degradant details only when not
					' editting the experiment.
	%>
					<br>
					<!-- <a class="headinglink" href="Degradant_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDegradant_form_group&amp;formmode=edit&amp;parent_rgroup=<%=bRGroup%>&amp;formgroupflag_override=Degradant_details&amp;keydeg=<%=rsDegradants.Fields( "DEG_CMPD_KEY" )%>">View Details</a><br> -->

	<%
					' We need to determine whether there is already a mechanism for this degradant.
					Set rsMechanism = Server.CreateObject( "ADODB.Recordset" )
					rsMechanism.Open "Select * from D3_MECHS where DEG_CMPD_FK = " & rsDegradants.Fields( "DEG_CMPD_KEY" ), connD3
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
		if UCASE(Application("IntegratedWith")) = "DOCMANAGER" then

			if CBool(Session("DD_APPROVE_RECORDS" & dbkey)) = true then
				if not bPrinterFriendly then
					showdelete = true
				else
					showdelete = false
				end if
			else 
				showdelete = false
			end if

			if Session("UserID" & "D3") = "" then
				Session("UserID" & "D3") ="D3browser"
				Session("username" & "D3")  ="D3browser"
			end if
			
			pData = "ReturnType=html&showlogo=true&extLinkID=" & rsExperiment.Fields( "EXPT_KEY" ) & "&LinkType=D3exptid&showsubmitdate=true&showsubmitter=true&showdelete=" & showdelete
			pData = pData & "&csusername=" & Session("username" & "D3")  & "&csuserid=" & Session("UserID" & "D3") &"&ShowFrameset=false"
			'Response.Write pData
			'pHostName = "dddev"
			pHostName = Application("DOCMANAGER_SERVER_NAME")
			pTarget = "/docmanager/docmanager/externallinks/getDocumentsNoGUI.asp"
			pUserAgent = "D3"
			pMethod = "POST"
					
			URL = "http://" & pHostName & "/" & pTarget
			
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
						window.open( '/docmanager/default.asp?dataaction=db&formgroup=base_form_group&dbname=docmanager&extAppName=D3&LinkType=D3exptid&linkfieldname=ExptID&showselect=true&extlinkid=<%=rsExperiment.Fields("EXPT_KEY").value%>', 'docmgrwindow', 'toolbar=no,location=no,scrollbars=yes,width=800,height=600')
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
end if ' if rsExperiment.EOF or rsExperiment.BOF
CloseRS( rsExperiment )


%>