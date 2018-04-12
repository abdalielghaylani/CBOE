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
				ShowResult dbkey, formgroup, rsBase64, "DRUGDEG_BASE64.BASE64_CDX", "Base64CDX", widthStructArea, heightStructArea 
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
						if instr(0,request("edit_structure"),"true") >0 then
							Session("MW" & dbkey & formgroup & "DRUGDEG_BASE64" & "MolWeight" & rsBase64( "MOL_ID" )) = ""
						end if
						sMW = getShowCFWChemResult(dbkey, formgroup, "MolWeight", "DRUGDEG_BASE64.MolWeight", rsBase64( "MOL_ID" ), "raw", "1", "17")
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
						if instr(0,request("edit_structure"),"true") >0 then
							Session("Formula" & dbkey & formgroup & "DRUGDEG_BASE64" & "Formula" & rsBase64( "MOL_ID" )) = ""
						end if
						sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "DRUGDEG_BASE64.Formula", rsBase64( "MOL_ID" ), "raw", "1", "17" )
						'sFormula = GetHTMLStringForFormula( sFormula )
						Response.Write( sFormula)
%>					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Status</strong>
					</td>
					<td valign="top">
						<% 
						
							Dim	rsDisplayStatus
							Set rsDisplayStatus = Server.CreateObject( "ADODB.Recordset" )
							sSQL = "Select STATUS_TEXT from DRUGDEG_STATUSES where STATUS_KEY =" & rsParent("STATUS")

							rsDisplayStatus.Open sSQL, connDrugDeg

						ShowResult dbkey, formgroup, rsDisplayStatus, "DRUGDEG_STATUS.STATUS_TEXT", "RAW", 0, 40 %>
						
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Generic Name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.GENERIC_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Trade name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.TRADE_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Common/Other name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMMON_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Submitted by</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.SUBMITTED_BY", "RAW", 0, 40 %>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<strong>Compound number</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "DRUGDEG_PARENTS.COMPOUND_NUMBER", "RAW", 0, 40 %>
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
			<%if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then%>
				<strong><nobr>View/Edit Experiments and Degradation Products</nobr></strong>
			<%else%>
				<strong><nobr>View Experiments and Degradation Products</nobr></strong>
			<%end if%>
<%
	' Make a record set for the experiments.
	Dim	rsExperiments
	Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

	' Make a record set for the experiment types.
	Dim	rsExperimentConds
	Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

	' Fill the record set for the experiments.
	rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg

	if rsExperiments.BOF and rsExperiments.EOF then
		' There are no degradation experiments for this parent compound.
%>
			<br>&nbsp; &nbsp; &nbsp; None
<%
	else
		' There are some experiment records.
		rsExperiments.MoveFirst
		while not rsExperiments.EOF
			' Fill the record set for the experiment conditions.
			rsExperimentConds.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connDrugDeg

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
				<b>Add experiment<!--<img SRC="/<%=Application( "appkey" )%>/graphics/Button_AddExperiment.gif" border="0">-->
			</a>
			<%end if%>
<%
		end if  ' if the user is allowed to add records, ...
		
	'JHS commented out 3/25/2003	
	'end if  ' if we can to this page from within DrugDeg, ...
	'JHS commented out 3/25/2003 end
	
%>		</td>
	</tr>
<!-- CSBR ID:133586 -->
<!-- Change Done by : Manoj Unnikrishnan -->
<!-- Purpose: Showing the Degradants link in the search result details; Added to show the link as part of this requirement -->
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
<!-- End of Change #133586# -->
	
</table>

<%if ThisAppIsIntegratedWith("DocManager") then
	if UCASE(Application("IntegratedWith")) = "DOCMANAGER" and Session("SEARCH_DOCS" & dbkey) then
				
		if CBool(Session("DD_Add_Records" & dbkey)) = true then
			if not bPrinterFriendly then
				showdelete = true
			else
				showdelete = false
			end if
		else 
			showdelete = false
		end if
'
'
		pData = "ReturnType=html&showlogo=true&extLinkID=" & rsParent.Fields( "PARENT_CMPD_KEY" ) & "&LinkType=drugdegparentid&showsubmitdate=true&showsubmitter=true&showdelete=" & showdelete
		pData = pData & "&csusername=" & Session("username" & "Drugdeg")  & "&csuserid=" & Session("UserID" & "drugdeg")
		pHostName = Application("DOCMANAGER_SERVER_NAME")
		pTarget = "/docmanager/docmanager/externallinks/getDocumentsNoGUI.asp"
		pUserAgent = "DrugDeg"
		pMethod = "POST"
		
				
		URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget
		
		
'		' This is the server safe version from MSXML3.
		Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")
'
'		' Syntax:
'		'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
		objXmlHttp.open pMethod, URL, False
		objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
		objXmlHttp.setRequestHeader "User-Agent", pUserAgent
		objXmlHttp.send pData
'
'		' Print out the request status:
		StatusCode = objXmlHttp.status
'
		If StatusCode <> "200" then
			httpResponse = objXmlHttp.responseText
			httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
		Else
			httpResponse = objXmlHttp.responseText
		End If
'
		'httpResponse = CShttpRequest2(pMethod, pHostName, pTarget, pUserAgent, pData)
'

		Response.Write "<br><br>"
'		
		if bPrinterFriendly then	
			Response.Write "<table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"">"
			Response.Write "<tr><td width=""600"">"
		end if
		Response.Write "<b>Associated Documents in Doc Manager</b>"	
		Response.Write "<br><br>"
		Response.write httpResponse
'		
		if bPrinterFriendly then
			Response.Write "</td></tr></table>"
		end if
'	
'
''Show Add button based on privileges
	if not bPrinterFriendly then	
		if true = CBool( Session( "DD_LINK_DOCUMENTS" & dbkey ) ) then
			' Allow the addition of links to documents for this parent compound,
			' _if_ the user is authorized to do so.
%>
			<!-- Make a link to the display for adding a document link. -->
			<br><br>						
			<script language="javascript">
			function launchDocMgrWindow(){
			window.open( '/docmanager/default.asp?dataaction=db&formgroup=base_form_group&dbname=docmanager&extAppName=drugdeg&LinkType=drugdegparentid&linkfieldname=ParentID&showselect=true&extlinkid=<%=rsParent.Fields("PARENT_CMPD_KEY").value%>', 'docmgrwindow', 'toolbar=no,location=no,scrollbars=yes,width=800,height=600')}
			</script>
			<a href="AddDocumentLink_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddDocLink_form_group&amp;formmode=add_doclink&amp;formgroupflag=add_doclink&amp;formgroupflag_override=add_doclink&amp;dataaction=add_doclink_full_commit&amp;record_added=false&amp;keyparent=<%=rsParent.Fields( "PARENT_CMPD_KEY")%>">
			<a href="#" onclick="launchDocMgrWindow()"><img src="/<%=Application( "appkey" )%>/graphics/Button_AddDocLink.gif" border="0"></a>
<%
	end if
		end if  ' if authorized to add records, ...
	end if
end if
%>

