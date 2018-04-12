<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%' Copyright 1998-2002, CambridgeSoft Corp., All Rights Reserved%>
<%
'ShowMessageDialog( "Got here" )

'ShowMessageDialog( "Session( 'LoginRequired' & dbkey ) = " & Session( "LoginRequired" & dbkey ) )
'ShowMessageDialog( "Session( 'UserValidated' & dbkey ) = " & Session( "UserValidated" & dbkey ) )
'MRE added clear this session so that we will see the results list again when we do a search
Session("addParent") = ""


if Session( "LoginRequired" & dbkey ) = 1 then
	if Not Session( "UserValidated" & dbkey ) = 1 then
		response.redirect "/" & Application("Appkey") & "/logged_out.asp"
	end if
end if

%><html>

<head>
	<script language="javascript">
		var baseid = ""
	</script>
	
	<title>Parent details view</title>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
	<!--#INCLUDE FILE = "../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	<link REL="stylesheet" TYPE="text/css" HREF="/drugdeg/styles.css">
</head>

<body  <%=Application("BODY_BACKGROUND")%>>
<script language = "javascript">
<!-- Hide from older browsers
	//if(formmode.toLowerCase = "edit_record"){
		document.forms["cows_input_form"].onSubmit = "removeFromRelFields()"
	//}
// end script hiding -->
</script> 
<%

'JHS commented out 3/25/2003
'sCalledBy = UCase( Request.QueryString( "calledby" ) )
'if "" = sCalledBy then
'	sCalledBy = "INTERNAL"
'end if
'JHS commented out 3/25/2003 end

' BaseID represents the primary key in the recordset for the current record.
'     It may be passed in via "keyparent" in the query string, or it may be set
'     in form_action_vbs.asp.
if 0 < Request.QueryString( "keyparent" ).Count then
	' There _is_ a "keyparent" entry in the query string, so get BaseID from "keyparent".
	BaseID = Request.QueryString( "keyparent" )( 1 )
end if

' Set up a session variable for this as the current page displayed.  Tack on "keyparent".
Session( "ReturnToCurrentPage" & dbkey ) = lcase(Session("CurrentLocation" & dbkey & formgroup)) & "&keyparent=" & BaseID
Session( "ReturnToCurrentPage" & dbkey ) = replace(Session( "ReturnToCurrentPage" & dbkey ),"keyexpt_passthru","keyexpt")
' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
' CAP 26 Apr 2001: Added the test of formmode to make sure the return location is that
'     of the _display_, not _edit_, form of the page.
if "edit_record" <> lcase(formmode) then
	Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )
end if
Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )
' Set variables for the height and width of the structure area.  This
' makes it easier to change them (no searching around in the code).
heightStructArea = 230
widthStructArea = 230

Set connDrugDeg = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )

Dim	rsParent
Set rsParent = Server.CreateObject( "ADODB.Recordset" )
sSQL = GetDisplaySQL( dbkey, formgroup, "DRUGDEG_PARENTS.*", "DRUGDEG_PARENTS", "", BaseID, "" )

rsParent.Open sSQL, connDrugDeg

' Get the base64_CDX data for the parent compound.
Set rsBase64 = Server.CreateObject( "ADODB.Recordset" )
rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg
if rsBase64.EOF then
'create a blank record so they can add a strcuture if none was added previously
	rsBase64.close
	query = "insert into DRUGDEG_BASE64 (mol_id) values (" & rsParent.Fields( "MOL_ID" ) & ")"
	connDrugDeg.execute query
	rsBase64.Open "Select * from DRUGDEG_BASE64 where MOL_ID = " & rsParent.Fields( "MOL_ID" ), connDrugDeg
end if

commit_type = "full_commit"

' Set up a session variable for the primary key of the object (parent) being displayed.
Session( "PrimaryKey" & dbkey ) = rsParent.Fields( "PARENT_CMPD_KEY" )
Session( "EDIT_THIS_RECORD" & dbkey ) = IsRecordEditable("DRUGDEG_PARENTS", "PARENT_CMPD_KEY", rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg)
%>

<% if request("keyexpt_passthru") <> "" then%>
	<script>
		document.location = "Experiment_details_form.asp?dbname=<%=dbkey%>&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=request("keyexpt_passthru")%>"
	</script>
<%end if%>


<%
if session("wizard") = "wiz_add_par_3" then
	session("wizard") = "wiz_add_par_2"
end if

wizardStep = session("wizard")
if "edit_record" <> LCase( formmode ) then
	select case lcase(wizardStep)
		case "wiz_add_par_1"
			session("wizard") = "wiz_add_par_2"
			%>
			<p>You have added a new parent compound. What would you like to do now?
			<%if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then%><br>
			<a class="headinglink" href="AddExperiment_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=add_experiment&amp;formgroupflag=add_experiment&amp;formgroupflag_override=add_experiment&amp;dataaction=experiment_full_commit&amp;record_added=false&amp;keyparent=<%=rsParent.Fields( "PARENT_CMPD_KEY" ) %>">Add an Experiment to this Parent Compound</a>
			
			<%end if
			' Make a record set for the experiments.
			Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

			' Make a record set for the experiment types.
			Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

			' Fill the record set for the experiments.
			rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg

			if rsExperiments.BOF and rsExperiments.EOF then

			else
				' There are some experiment records.
				Response.Write "<p><span class=""heading"">View an existing experiment</span>"
				rsExperiments.MoveFirst
				while not rsExperiments.EOF
					' Fill the record set for the experiment conditions.
					rsExperimentConds.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connDrugDeg

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
			rsExperiments.Close%>
				
			<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
				<br>Return here if you would like to add something else<br>
		<%case "wiz_add_par_4"
			session("wizard") = "wiz_add_par_2"
			%>
			<p>You have added a new experiment to this parent compound. What would you like to do now?
			<%if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then%>
				<br><a class="headinglink" href="AddExperiment_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=add_experiment&amp;formgroupflag=add_experiment&amp;formgroupflag_override=add_experiment&amp;dataaction=experiment_full_commit&amp;record_added=false&amp;keyparent=<%=rsParent.Fields( "PARENT_CMPD_KEY" ) %>">Add another Experiment to this Parent Compound</a>
			
			<%end if
			' Make a record set for the experiments.
			Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

			' Make a record set for the experiment types.
			Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

			' Fill the record set for the experiments.
			rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg

			if rsExperiments.BOF and rsExperiments.EOF then

			else
				' There are some experiment records.
				Response.Write "<p><span class=""heading"">View an existing experiment</span>"
				rsExperiments.MoveFirst
				while not rsExperiments.EOF
					' Fill the record set for the experiment conditions.
					rsExperimentConds.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connDrugDeg

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
			rsExperiments.Close%>
		
			<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
				<br>Return here if you would like to add something else<br>
		<%case else
			if session("wizard") = "wiz_edit_par_1" then
				session("wizard") = ""
			else
				session("wizard") = "wiz_add_par_2"
			end if
			if true = CBool( Session( "DD_Add_Records" & dbkey ) ) then%>
			<a class="headinglink" href="AddExperiment_input_form.asp?dbname=<%=dbkey%>&amp;formgroup=AddExperiment_form_group&amp;formmode=add_experiment&amp;formgroupflag=add_experiment&amp;formgroupflag_override=add_experiment&amp;dataaction=experiment_full_commit&amp;record_added=false&amp;keyparent=<%=rsParent.Fields( "PARENT_CMPD_KEY" ) %>">Add an Experiment to this Parent Compound</a>

			<%end if
			' Make a record set for the experiments.
			Set rsExperiments = Server.CreateObject( "ADODB.Recordset" )

			' Make a record set for the experiment types.
			Set rsExperimentConds = Server.CreateObject( "ADODB.Recordset" )

			' Fill the record set for the experiments.
			rsExperiments.Open "SELECT * FROM DrugDeg_Expts WHERE Parent_Cmpd_FK = " & rsParent.Fields( "PARENT_CMPD_KEY" ), connDrugDeg

			if rsExperiments.BOF and rsExperiments.EOF then

			else
				' There are some experiment records.
				Response.Write "<p><span class=""heading"">View an existing experiment</span>"
				rsExperiments.MoveFirst
				while not rsExperiments.EOF
					' Fill the record set for the experiment conditions.
					rsExperimentConds.Open "Select * from DRUGDEG_CONDS where DEG_COND_KEY = " & rsExperiments.Fields( "DEG_COND_FK" ), connDrugDeg

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
			rsExperiments.Close%>		
			<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
				<br>Return here if you would like to add something else<br>
			
	<%end select
else
	Response.Write "Edit the parent compound"
end if '  "edit_record" <> LCase( formmode ) %>

	<script language="javascript">
		var baseid = <%=baseid%>
		var Edit_This_Record = "<%=Session( "EDIT_THIS_RECORD" & dbkey )%>"
	</script>
<script language="JavaScript">
	var commit_type = "<%=commit_type%>"
	var formmode = "<%=lcase(formmode)%>"
	var uniqueid = "<%=baseid%>"
	windowloaded = false
</script>

<%
'-- CSBR ID:133586
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: To show the degradants link in the search results details page
'-- Date: 11/12/2010
if request("degs") = "true" then
	
%>
<!--#INCLUDE FILE="Parent_details_form-Degs.asp"-->
<%elseif "edit_record" <> LCase( formmode ) then
' The user is not editting the parent information.%>
<!--#INCLUDE FILE="Parent_details_form-View.asp"-->
<%
else
	' The user _is_ editting the parent information (and is authorized to do so),
	' so we do things a little differently.
%>
<!--#INCLUDE FILE="Parent_details_form-Modify.asp"-->
<%
end if  ' if the user is editting the parent information...
'-- End of Change #133586#
%>

&nbsp;<br>

<%
CloseRS( rsBase64 )
CloseRS( rsParent )
CloseConn( connDrugDeg )

%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>
</html>
 