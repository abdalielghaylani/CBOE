<%@ LANGUAGE="VBScript" %>
<%
response.expires = 0
' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved
if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then
		response.redirect "/" & Application("appkey") & "/logged_out.asp"
	end if
end if

function GetSessionStuff()
	sout = ""
	For Each i in Session.Contents
		on error resume next
		sout = sout & i & " ===== " & Session.Contents(i) & "<br />"		
	Next
	GetSessionStuff = sout
end function
%>
<html>

<head>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<title>Parent list view</title>
	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<%
Trace "In:Parent_result_list.asp",20
'MRE added 1/13/05
'used to list the experiments for each result
Dim	rsExperiments
Dim	rsExperimentConds


' Set up a session variable for this as the current page displayed.
'Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
Session( "ReturnToCurrentPage" & dbkey ) = "/" & Application("appkey") &"/D3/Parent_result_list.asp?formgroup=base_form_group&formmode=list&dbname=D3&indexvalue=3&form_change=true"
'Session( "ReturnToCurrentPage" & dbkey )

' Set variables for the height and width of structure fields.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 160
widthStructArea = 220

' Set variables for some display variations.  I do this so that, if I change my mind about
' when to use a particular variation, I can put that logic up here.  For example, the  formula, weight, etc. had been listed in separate columns.  When first I designed the
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

'if ( false = bShowStructs ) then
'	bSeparateColumns = true
'else
	'changing to always be false (not sure why the change was made for columns so we are leaving for now
	bSeparateColumns = false
'end if


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
<table  border = 1  cellpadding = 2  cellspacing = 0>
<%
else
	' The printer-friendly form: set the width to a safe value for a printed page.
%>
<table  border = 1  cellpadding = 2  cellspacing = 0  width = 550>
<%
end if
Set connChem = GetConnection( dbkey, formgroup, "D3_PARENTS" )
'the insert molecule to use in the query further down
if session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup) <> "" then

		
	'Set connInsert = GetConnection( dbkey, formgroup, "D3_PARENTS" )
	'Set connChem = GetConnection( dbkey, formgroup, "D3_PARENTS" )
	
		connChem.Begintrans()
		if err.number <> 0 then
			err.Clear
			connChem.Begintrans()
		end if
	
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = connChem
	Cmd.CommandType = adCmdText
	
	sql = "BEGIN INSERT INTO cscartridge.tempQueries (id, query) VALUES (2, ?); END;"
	b64 = session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup)
	Trace "DoCartridgeSearch Query Molecule " & i & " = " & b64, 20
	Cmd.Parameters.Append Cmd.CreateParameter("p1", 201, 1, len(b64), b64 )	
	cmd.CommandText = sql
	cmd.properties("SPPRMSLOB") = true
	cmd.Execute
	cmd.properties("SPPRMSLOB") = false
	cmd.Parameters.delete "p1"
end if

	

%>

	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	<%
	'BaseID represents the primary key for the recordset from the base array for the current row
	'BaseActualIndex is the actual id for the index the record is at in the array
	'BaseRunningIndex is the id based on the number shown in list view
	'rsParent is the recordset that is pulled for each record generated
	'Set connChem = GetConnection( dbkey, formgroup, "D3_PARENTS" )
	sqlParent = GetDisplaySQL( dbkey, formgroup, "D3_PARENTS.*", "D3_PARENTS", "", BaseID, "" )
%>

<%

    
	Set rsParent = connChem.Execute( sqlParent )

	' Get the base64_CDX data for the parent compound.
	Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
	rsBase64.Open "Select * from D3_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connChem
	sName = rsParent.Fields( "GENERIC_NAME" )
	sTradeName = rsParent.Fields( "TRADE_NAME" )
	sCommonName = rsParent.Fields( "COMMON_NAME" )
	sSubmittedBy = rsParent.Fields( "SUBMITTED_BY" )
	sCompoundNumber = rsParent.Fields( "COMPOUND_NUMBER" )
	
	Trace "Output Data for Compound ID" & rsParent("PARENT_CMPD_KEY"),20
	
	sSaltList = GetSaltNameListFromSaltCodeList( rsParent.Fields( "SALT" ) )
	' Get the molecular formula.
	sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "D3_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
	' Get a nice HTML string for the formula, so the "3" in "CH3" is set lower than the letters.
	sFormula = GetHTMLStringForFormula( sFormula )

	' Get the molecular weight.
	sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "D3_BASE64.MolWeight", rsBase64( "MOL_ID" ), "raw", "1", "17" )
	' Keep only two decimal places for the molecular weight.
	' 2002-09-17: FormatNumber doesn't seem to work for the molecular weight I get using the new
	' COWS core: I can't explain why, I tested everything I could to figure it.  Anyway, only
	' thing I found that worked was to make my own formmating function.
	'sMW = MyFormatNumber( sMW, 4 )
	
%>
<!-- -
	<tr>
		<td  colspan = 4>
			<%=myPath%>
		</td>
	</tr>
<!-- -->

	<tr>
		<%
		if ( false = bPrinterFriendly ) then
			' Not the printer-friendly form, so we put the record number & total, the
			' "Show Details" button, and the "Mark Record" button in the first column.
		%>
		<td  align = "center">
			<nobr> <!-- Introduced to avoid breaks in the "Record 21 of 75" line. -->
			<script language = "javascript">
				getRecordNumber( <%=BaseRunningIndex%> )
				document.write( '<br>' )
				getMarkBtn( <%=BaseID%> )
				document.write( '<br>' )
				//MRE 1/31/05 removed for end user app
				//getFormViewBtn( "show_details_btn.gif", "Parent_details_form.asp", "<%=BaseActualIndex%>", "edit", "", "base_form_group" )
// goFormView("/D3/D3/Parent_details_form.asp?formgroup=base_form_group&amp;dbname=D3&amp;formmode=edit&amp;unique_id=1&amp;commit_type=","1")
			</script>
			</nobr>
		</td>
		<% end if %>
		

		
		<%
		if ( true = bShowStructs ) then
			' We are to show structures.
		%><td  align = "center"  valign = "center">
			<% ShowResult dbkey, formgroup, rsBase64, "D3_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea %>
		<br><%
						sFormula = getShowCFWChemResult( dbkey, formgroup, "Formula", "D3_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
						sFormula = GetHTMLStringForFormula( sFormula )
						Response.Write( sFormula )
					sMW = getShowCFWChemResult( dbkey, formgroup, "MolWeight", "D3_BASE64.BASE64_CDX", rsBase64( "MOL_ID" ), "raw", "1", "17" )
					if sMW <> "" then
						Response.Write "<br><br><br>MW: "
						sMW = replace (sMW,"&nbsp;","")
						sMW = FormatNumber( sMW, 2 )
						Response.Write(  sMW )
					end if
					%>
					
		</td>
		<% end if

		if ( false = bSeparateColumns ) then
			' Put the rest of the data into one cell, stacked vertically.

		%>
		<td valign="top">
				
				<!---JHS added 3/24/2003 --->
				<!---replaced an old layout --->
				<table  border = 1  cellpadding = "2" cellspacing="0" width = "100%">

				<%if rsParent.Fields( "GENERIC_NAME" ) <> "" then%>
				<tr>
					<td  valign = "top">
						<STRONG>Generic Name</STRONG>
					</td>
					<td  valign = "top">
						<% ShowResult dbkey, formgroup, rsParent, "D3_PARENTS.GENERIC_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if%>
				<%if rsParent.Fields( "TRADE_NAME" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Trade name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "D3_PARENTS.TRADE_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if%>
				<%if rsParent.Fields( "COMMON_NAME" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Common/Other name</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "D3_PARENTS.COMMON_NAME", "RAW", 0, 40 %>
					</td>
				</tr>
				<%end if%>
				<%if rsParent.Fields( "COMPOUND_NUMBER" ) <> "" then%>
				<tr>
					<td valign="top">
						<strong>Compound Id</strong>
					</td>
					<td valign="top">
						<% ShowResult dbkey, formgroup, rsParent, "D3_PARENTS.COMPOUND_NUMBER", "RAW", 0, 40 %>
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
						rsExperiments.Open "SELECT * FROM D3_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connChem

						if rsExperiments.BOF and rsExperiments.EOF then
							' There are no degradation experiments for this parent compound.
							Response.Write "&nbsp;"
						else
							Trace "Starting Experiments for " & rsParent("PARENT_CMPD_KEY"),20
							' There are some experiment records.
							rsExperiments.MoveFirst
							while not rsExperiments.EOF
								' Fill the record set for the experiment conditions.
								rsExperimentConds.Open "Select * from D3_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connChem

								
								parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&keyexpt=" & rsExperiments.Fields( "EXPT_KEY" ) &"&keyparent=" & BaseID
					%>
								<nobr>
								<script>
								getFormViewLink("<%=rsExperimentConds.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt=<%=rsExperiments.Fields( "EXPT_KEY" )%>")
								</script>
								</nobr><br>
								<!-- <a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">
								<nobr>

									<% 'Response.Write( rsExperimentConds.Fields( "DEG_COND_TEXT" ) ) %>
								</a>-->
								
					<%
								' Close the record set for the experiment type.
								rsExperimentConds.Close
								Trace "Experiment " & rsExperiments.Fields( "EXPT_KEY" ),20
								' Get the next experiment record.
								rsExperiments.MoveNext
							wend
							Response.write "<br><br>"
						end if  'if not rsExperiments.BOF or not rsExperiments.EOF then ...

						' Close the record set for the experiment type.
						rsExperiments.Close
					 %>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						
							<strong>Degradants</strong>
						
					</td>
					<td  valign = "top">
						<%parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&degs=true&keyparent=" & BaseID
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
	
	'Response.write "asd" & session("SearchData" & "D3_DEG_BASE64.Structure.sstype" & dbkey & formgroup)
	Trace "Building sql statement for degrant", 20
	if session("SearchData" & "D3_DEGS.MW_AMT_CHG" & dbkey & formgroup) <> "" or _
			session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup) <> "" or _
			session("SearchData" & "D3_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "" then %>
		<tr>
		<%if ( false = bPrinterFriendly ) then%>
			<td valign=top>Matching Degradants</td>	
		<%end if
		sSQL = "Select D3_BASE64.BASE64_CDX , D3_BASE64.MOL_ID as base64_mol_id, D3_DEGS.*, D3_CONDS.DEG_COND_TEXT "&_
						" from D3_PARENTS, D3_EXPTS, D3_DEGS, D3_CONDS, D3_BASE64 "
		if session("SearchData" & "D3_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "" then
			sSQL = sSQL & ",D3_DEGSFGROUP " 
		end if
		
		sSQL = sSQL &	" where D3_PARENTS.PARENT_CMPD_KEY = " & rsParent.Fields( "PARENT_CMPD_KEY" ) &" and "&_
						" D3_PARENTS.PARENT_CMPD_KEY = D3_EXPTS.PARENT_CMPD_FK and "&_
						" D3_DEGS.DEG_EXPT_FK = D3_EXPTS.EXPT_KEY and "&_
						" D3_CONDS.DEG_COND_KEY = D3_EXPTS.DEG_COND_FK and "&_
						" D3_DEGS.MOL_ID = D3_BASE64.MOL_ID  "
						
		if session("SearchData" & "D3_DEGS.MW_AMT_CHG" & dbkey & formgroup) <> "" then
			if instr(session("SearchData" & "D3_DEGS.MW_AMT_CHG" & dbkey & formgroup),">") > 0 or instr(session("SearchData" & "D3_DEGS.MW_AMT_CHG" & dbkey & formgroup),"<") > 0 then
				strCompare = ""
			else
				strCompare = "="
			end if
			sSQL = sSQL & " and D3_DEGS.MW_AMT_CHG  " & strCompare & session("SearchData" & "D3_DEGS.MW_AMT_CHG" & dbkey & formgroup)
		end if
		
		if session("SearchData" & "D3_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup) <> "" then
	
			sSQL = sSQL & " and D3_DEGSFGROUP.DEG_CMPD_ID = D3_DEGS.DEG_CMPD_KEY and "&_
							" D3_DEGSFGROUP.DEG_FGROUP_ID = " & session("SearchData" & "D3_DEGSFGROUP.DEG_FGROUP_ID" & dbkey & formgroup)
		end if
		
		if session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup) <> "" then
			'structWhere = Session("SearchData" & "FullStrWhere" & dbkey & formgroup)
			'Trace "All Session stuff<br>" & GetSessionStuff, 20
			'Response.Write "<b>" & structWhere & "</b>"
			'Response.Write "<br><font color=red>" & session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup) & "</font>"
			
			'jhs 9/4/2007 instead of doing this the structure in session should be inserted
			'structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 0",session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup))
			'structWhere = replace(structWhere,"SELECT query from cscartridge.tempQueries where id = 1",session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup))
			

			'jhs 9/4/2007 - seems like it would be better to build the query manually instead of using this replace method
			'structWhere = replace(structWhere,"D3_DEG_BASE64.BASE64_CDX","D3_BASE64.BASE64_CDX")
			
			'jhs 9/4/2007 - since we are building the query this won't be needed anymore
			'get rid of any assumption of structure search type since we might be basing the search of the
			'parent structure setting
			'structWhere = replace(structWhere,"IDENTITY=YES","")
			'structWhere = replace(structWhere,"FULL=YES","")
			'structWhere = replace(structWhere,"SIMILARITY=YES","")
			
			
			'create a replacement string
			if instr(1,Session("SearchData" & "EXACT" & dbkey & formgroup),"D3_DEG_BASE64.Structure")>0 then
				strstring = "FULL=YES,"
			elseif instr(1,Session("SearchData" & "IDENTITY" & dbkey & formgroup),"D3_DEG_BASE64.Structure")>0 then
				strstring = "IDENTITY=YES,"
			elseif instr(1,Session("SearchData" & "IDENTITY" & dbkey & formgroup),"D3_DEG_BASE64.Structure")>0 then
				strstring = "SIMILARITY=YES,"
			else
				 'substructure just needs blank string
				 'this will also default the results to substructure if the replacements fail
				 strstring = ""
			end if
			
			structWhere = "CSCartridge.MoleculeContains(D3_BASE64.BASE64_CDX,'SELECT query from cscartridge.tempQueries where id = 2', '','" & strstring & "PERMITEXTRANEOUSFRAGMENTS=YES,PERMITEXTRANEOUSFRAGMENTSIFRXN=YES,FRAGMENTSOVERLAP=YES')=1"
			
			
			
			'look for the empty quotes which are passed to cartridge
			'structWhere = replace(structWhere, "'','", "'','" & strstring & ",")
				
			
			'Response.Write "<P>" &  sSQL & " and " & structWhere
			sSQL = sSQL & " and " & structWhere
		end if
		sSQL = sSQL & " order by DEG_COND_TEXT"
		Trace "SQL Built<br>" & sSQL, 20
		'Response.Write sSQL
		Set rsBase64Deg = Server.CreateObject( "ADODB.Recordset" )
		rsBase64Deg.Open sSQL, connChem
		if ( true = bShowStructs ) then
			' We are to show structures.
		%><td  colspan=2 align = "center"  valign = "center">
			<table border=0>
				<tr>
					<%coltotal = 0
					while not rsBase64Deg.EOF
						coltotal = coltotal + 1
						if coltotal > 3 then
							Response.Write "</tr><tr>"
							coltotal = 1
						end if%>
					<td align=center><%'sSQL%>
							<%Trace "Getting MF and MW for " &  rsBase64Deg( "base64_mol_id" ), 20%>
							<% ShowResult dbkey, "AddDegradant_form_group", rsBase64Deg, "D3_BASE64.BASE64_CDX", "Base64CDX_NO_EDIT", widthStructArea, heightStructArea %>
						<br><%
							sFormula = getShowCFWChemResult( dbkey, "AddDegradant_form_group", "Formula", "D3_BASE64.BASE64_CDX", rsBase64Deg( "base64_mol_id" ), "raw", "1", "17" )
							sFormula = GetHTMLStringForFormula( sFormula )
							sFormula = StandardizeHTML(sFormula)
										
							Response.Write( sFormula )
							
							sMW = getShowCFWChemResult( dbkey, "AddDegradant_form_group", "MolWeight", "D3_BASE64.BASE64_CDX", rsBase64Deg( "base64_mol_id" ), "raw", "1", "17" )
							if sMW <> "" then
								Response.Write "<br><br><br>MW: "
								sMW = replace (sMW,"&nbsp;","")
								sMW = FormatNumber( sMW, 2 )
								Response.Write(  sMW )
							end if
							
							parentPassThruURL = "/" & Application("appkey") & "/D3/parent_details_form.asp?formgroup=base_form_group&dbname="& dbkey &"&formmode=edit&unique_id="&BaseActualIndex&"&commit_type=&keyexpt=" & rsBase64Deg.Fields( "DEG_EXPT_FK" )
							%>
								<P><nobr>
								<script>
								getFormViewLink("<%=rsBase64Deg.Fields( "DEG_COND_TEXT" )%>","Parent_details_form.asp", "<%=BaseActualIndex%>", "edit","<%=BaseActualIndex%>", "&keyexpt=<%=rsBase64Deg.Fields( "DEG_EXPT_FK" )%>")
								</script>
								</nobr>
								
								<!-- <a href="javascript:parentPassThru('Parent_details_form.asp','<%=BaseActualIndex%>', '<%=parentPassThruURL%>')">
								<% 'Response.Write( rsBase64Deg.Fields( "DEG_COND_TEXT" ) ) %>
								</a>-->
						</td>
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
		<%end if
		
	end if 'degradant search fields
	Response.Write "</tr></table>"
		
	if ( false = bPrinterFriendly ) then
		' Not the printer-friendly form, so we don't need to set the table width.
	%>
	<br clear="all">
	<table  border = 1  cellpadding = 2  cellspacing = 0>
	<%
	else
		' The printer-friendly form: set the width to a safe value for a printed page.
	%>
	<table  border = 1  cellpadding = 2  cellspacing = 0  width = 550>
	<%
	end if
	CloseRS( rsBase64 )
	CloseRS( rsParent )
	
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</table>
<%
CloseConn( connChem )

if session("SearchData" & "D3_DEG_BASE64.Structure" & dbkey & formgroup) <> "" then
	conn.CommitTrans
end if
%>
<br>  <!-- For a little more space. -->
<%Trace "Out:Parent_result_list.asp",20 %>
</body>

</html>
