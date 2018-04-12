<%'JHS added 3/25/2003%>
<br clear="all">
<%'JHS added 3/25/2003 end%>
<table border="1" cellpadding="3" cellspacing="0" width="500">
	<tr>
<%

'JHS commented out 3/25/2003
'if ( "INTERNAL" = sCalledBy ) then
	' This page was called from within the DrugDeg app.  Show the number of records in the
	' hitlist and the rank of this compound in that list.
'JHS commented out 3/25/2003 end

if (Request.Cookies("PrinterFriendly" & dbkey & formgroup ))="1" then
	bPrinterFriendly = true
else
	bPrinterFriendly = false
end if


%>		<!-- There aren't yet 20 rows, but "rowspan = 20" doesn't require 20 rows, so			it allows for new rows without having to alter this number for a while. -->
		<%if not bPrinterFriendly then%>
		<td valign="top" align="center" width="100" rowspan="20">
			
			<nobr>
			<script language="JavaScript"><!--
				// BaseRunningIndex is the rank of the current record ("5th of 20").
				getRecordNumber( <%=BaseRunningIndex%> )
				document.write( '<br>' )

				getMarkBtn( <%=BaseID%> )
			// --></script>
			</nobr>
			
		</td>
		<%end if%>
<%

'JHS commented out 3/25/2003
'end if  ' if we're looking at one compound from a list, ...
'JHS commented out 3/25/2003 end

%>		<!-- The cell for the parent compound's structure. -->

	    <td valign="top" align="center">
			<%
				ShowResult dbkey, formgroup, rsBase64, "DrugDeg_BASE64.BASE64_CDX", "Base64CDX", widthStructArea, heightStructArea 
			%>
		</td>

		<!-- The cell for the rest of the parent information. -->
		<td valign="top">
		
			<!---JHS added 3/24/2003 --->
				<!---replaced an old layout --->
				<table border="1" cellpadding="2" cellspacing="0" width="100%">
				<%
				sName = rsParent.Fields( "GENERIC_NAME" )
				sTradeName = rsParent.Fields( "TRADE_NAME" )
				sCommonName = rsParent.Fields( "COMMON_NAME" )
				sSubmittedBy = rsParent.Fields( "SUBMITTED_BY" )
				sCompoundNumber = rsParent.Fields( "COMPOUND_NUMBER" )
				
				%>

				<tr>
					<td>
						<strong><nobr>Molecular weight</nobr></strong>
					</td>
					<td valign="top">
						<%
						sMW = getShowCFWChemResult(dbkey, formgroup, "MolWeight", "DrugDeg_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17")
						'sMW = MyFormatNumber(sMW, 2)
						Response.Write(sMW)
%>					</td>
				</tr>

				<tr>
					<td valign="top">
						<strong>Formula</strong>
					</td>
					<td valign="top">
						<%
						sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "DrugDeg_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
						sFormula = GetHTMLStringForFormula( sFormula )
						Response.Write( sFormula )
%>					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Status</strong>
					</td>
					<td valign="top">
						<% 
						
							
							Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
							sSQL = "Select STATUS_TEXT from DrugDeg_STATUSES where STATUS_KEY =" & rsParent("STATUS")

							rsDisplayStatus.Open sSQL, connDrugDeg

						ShowResult dbkey, formgroup, rsDisplayStatus, "DrugDeg_STATUS.STATUS_TEXT", "RAW", 0, 40 %>
						
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Generic Name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DrugDeg_PARENTS.GENERIC_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Trade name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DrugDeg_PARENTS.TRADE_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Common/Other name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DrugDeg_PARENTS.COMMON_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Submitted by</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DrugDeg_PARENTS.SUBMITTED_BY", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Compound number</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DrugDeg_PARENTS.COMPOUND_NUMBER", "RAW", 0, 40 %>
					</td>
				</tr>

				<tr>
					<td valign="top">
						
							<strong>Salts</strong>
						
					</td>
					<td valign="top">
						<%
						sSaltNameList = GetSaltNameListFromSaltCodeList( rsParent.Fields( "SALT" ) )
						%><%=sSaltNameList%>&nbsp;  <!-- The blank assures we'll have cell borders. -->
					</td>
				</tr>
			</table>
			<!---JHS added 3/24/2003 end --->
			
		</td>
	</tr>

	<!-- Show the types of degradation experiments for this parent compound. -->
	<tr>
		<td colspan="2">
			<strong><nobr>View Experiments and Degradation Products</nobr></strong>
<%
	' Make a record set for the experiments.
	
	Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

	' Make a record set for the experiment types.
	
	Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

	' Fill the record set for the experiments.
	rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg

	if rsExperiments.BOF and rsExperiments.EOF then
		' There are no degradation experiments for this parent compound.
%>
			<br>&nbsp; &nbsp; &nbsp; No experiments or degradants found.
<%
	else
		' There are some experiment records.
		rsExperiments.MoveFirst
		while not rsExperiments.EOF
			' Fill the record set for the experiment conditions.
			rsExperimentConds.Open "Select * from DrugDeg_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connDrugDeg

			' LJB NOTE1: add information to the querystring that will make the standard cows
			'     source work and allow you to modify the navbar.asp file to get your buttons.
			' LJB NOTE2: Camden, the formgroup was not set so I changed it 4/9/2001
%>
			<br>&nbsp; &nbsp; &nbsp; <nobr>
			<a href="Experiment_details_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=<%="edit"%>&amp;formgroupflag_override=<%="Experiment_details"%>&amp;keyexpt=<% Response.Write( rsExperiments.Fields( "EXPT_KEY" ) ) %>">
				<% Response.Write( rsExperimentConds.Fields( "DEG_COND_TEXT" ) ) %>
			</a>
			</nobr>
<%
			' Close the record set for the experiment type.
			rsExperimentConds.Close

			' Get the next experiment record.
			rsExperiments.MoveNext
		wend
	end if  'if not rsExperiments.BOF or not rsExperiments.EOF then ...

	' Close the record set for the experiment type.
	rsExperiments.Close
	
	'JHS commented out 3/25/2003
	'if ( "INTERNAL" = sCalledBy ) then
		' This page was called from within the DrugDeg app, so we may want to allow
		' experiments to be added.
	'JHS commented out 3/25/2003 end
	
	
		if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then
			' Allow the addition of degradation experiments for this parent compound,
			' _if_ the user is authorized to do so.
%>
			<%if not bPrinterFriendly then%>
			<!-- Make a link to the display for adding an experiment. -->
			<br><br>
			
			<a class="headinglink" href="AddExperiment_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=add_experiment&amp;formgroupflag=add_experiment&amp;formgroupflag_override=add_experiment&amp;dataaction=experiment_full_commit&amp;record_added=false&amp;keyparent=<% Response.Write( rsParent.Fields( "PARENT_CMPD_KEY" ) ) %>">
				<b>Add experiment/degradants<!--<img SRC="/<%=Application( "appkey" )%>/graphics/Button_AddExperiment.gif" border="0">-->
			</a>
			<%end if%>
<%
		end if  ' if the user is allowed to add records, ...
		
	'JHS commented out 3/25/2003	
	'end if  ' if we can to this page from within DrugDeg, ...
	'JHS commented out 3/25/2003 end
	
%>		</td>
	</tr>
</table>

