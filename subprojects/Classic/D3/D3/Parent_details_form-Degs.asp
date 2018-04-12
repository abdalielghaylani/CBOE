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

' Set variables for the height and width of the structure area.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 230
widthStructArea = 460

' Make and fill a record set for the experiment.
set rsExperiment = Server.CreateObject( "ADODB.Recordset" )

rsExperiment.Open "Select * from D3_EXPTS where PARENT_CMPD_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connD3
	
	
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
			var baseid = <%=rsParent.Fields( "PARENT_CMPD_KEY" )%>
			var Edit_This_Record = "false"
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

	end if						

							
	%>
	<P class="heading">Parent Degradants
	<!-- Create a table which will hold the sub-tables for experiment data and parent info. -->
	<table border="0" bordercolor="red">
		<tr>
			

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
%>			<table border="2" bordercolor="gray" >
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

				<tr>
					<!-- One row (no borders, so one cell) for the list of degradation experiments. -->
					<td valign="top">
						

						<!-- Show the degradation experiments for this parent compound. -->
<%
				' Make and fill a record set for the experiments done on this parent.

				maxCmpdsInRow = 3
				widthDegStructDrawing = 210
				heightDegStructDrawing = 210
	
				rsExperiment.MoveFirst
				Set rsDegradants = Server.CreateObject( "ADODB.Recordset" )
						
				while not rsExperiment.EOF
					' Make and fill a record set for the degradation type.
					set rsDegType = Server.CreateObject( "ADODB.Recordset" )
					rsDegType.Open "Select * from D3_CONDS where DEG_COND_KEY = " & rsExperiment.Fields( "DEG_COND_FK" ), connD3
							
						
					parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&unique_id&"&commit_type=&keyexpt=" & rsExperiment.Fields( "EXPT_KEY" )
				%>
				<div class='headingblock'>
				<nobr>
					<script>
				
						getFormViewLink("<%=rsDegType.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt=<%=rsExperiments.Fields( "EXPT_KEY" )%>")
				
					</script>
				</nobr>
				</div>
					
					<!--<a class="headinglink" href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">
					<%= rsDegType.Fields( "DEG_COND_TEXT" ) %></a>
								-->
					<%

					' Close the record set for the experiment type.
					CloseRS( rsDegType )%>
					<!-- Now the degradants. -->
						<table border="1" bordercolor="gray" width=100%>
								
						<%
	
						sSQL = "SELECT * FROM D3_Degs WHERE Deg_Expt_FK = " & rsExperiment.Fields( "EXPT_KEY" ) & " ORDER BY D3_Degs.Mol_ID"
						rsDegradants.Open sSQL, connD3

					if ( ( not rsDegradants.BOF ) and ( not rsDegradants.EOF ) ) then
						' There are degradants, so make sure we start with the first.
						rsDegradants.MoveFirst
					else%>
						<tr>
							<td> <P>No degradants for this experiment.	</td>
						</tr>
					<%end if
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
								if sMW <> "" then
									'Response.Write "<br><br><br>MW: "
									sMW = replace (sMW,"&nbsp;","")
									sMW = FormatNumber( sMW, 2 )
									
								end if		
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
										<td><%=sMW%></td>
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
					CloseRS( rsDegradants )	
					%>
					</table>
					
					<%' Get the next parent experiment record.
					rsExperiment.MoveNext
					if not rsExperiment.Eof and not rsExperiment.BOF then%>
						<P>&nbsp;</p>
					<%end if
				wend

%>
					</td>
				</tr>
			</table>
<%			end if  ' if there is a parent record ...
					
%>		</td>
	</tr>

</table>

<p> &nbsp;  <!-- Just to put a little space in. -->


	
<p>

<%
' Close the record set for degradant compounds.
CloseRS( rsDegradants )

	
Response.Write "<p> &nbsp;  <!-- Just to put a little space below the above link. -->"

	

	' Close the record set for the experiments.
end if ' if rsExperiment.EOF or rsExperiment.BOF
CloseRS( rsExperiment )


%>