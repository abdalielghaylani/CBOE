<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if


'MRE 1/13/05 We may come to this page without going to the parent details page. 
'Set the variables here that we would have set on that page
wizardStep = session("wizard")

if wizardStep <> "" then
	dbkey = request("dbname")
	formgroup = "base_form_group"
	indexvalue = request("indexvalue")

	' BaseID represents the primary key in the recordset for the current record.
	'     It may be passed in via "keyparent" in the query string, or it may be set
	'     in form_action_vbs.asp.
	if 0 < Request.QueryString( "keyparent" ).Count then
		' There _is_ a "keyparent" entry in the query string, so get BaseID from "keyparent".
		BaseID = Request.QueryString( "keyparent" )( 1 )
	end if

	' Set up a session variable for this as the current page displayed.  Tack on "keyparent".
	'parent details page
	Session( "ReturnToCurrentPage" & dbkey ) = "/" & Application("Appkey") & "/drugdeg/parent_details_form.asp?formgroup=" & formgroup &"&dbname="& dbkey &"&formmode=edit&unique_id=" & indexvalue & "&commit_type=&indexvalue=" & indexvalue & "&form_change=true&keyparent=" & BaseID
	


	' Make and fill a session variable to be used for getting back to this display
	' with the proper querystring parameter settings.
	' CAP 26 Apr 2001: Added the test of formmode to make sure the return location is that
	'     of the _display_, not _edit_, form of the page.
	if "edit_record" <> lcase(formmode) then
		Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )
	end if
	Session( "ReturnToParentDetails" & dbkey ) = Session( "ReturnToCurrentPage" & dbkey )


	Set connDrugDeg = GetConnection( dbkey, formgroup, "DRUGDEG_PARENTS" )

	Dim	rsParent
	Set rsParent = Server.CreateObject( "ADODB.Recordset" )
	sSQL = GetDisplaySQL( dbkey, formgroup, "DRUGDEG_PARENTS.*", "DRUGDEG_PARENTS", "", BaseID, "" )

	rsParent.Open sSQL, connDrugDeg

	' Set up a session variable for the primary key of the object (parent) being displayed.
	Session( "PrimaryKey" & dbkey ) = rsParent.Fields( "PARENT_CMPD_KEY" )
	rsParent.Close
end if
'end of setting parent detail variables


%>
<html>

<head>

<title>Add Experiment display</title>
<link REL="stylesheet" TYPE="text/css" HREF="/drugdeg/styles.css">
</head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
	<!--#INCLUDE FILE = "../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	
	
<%



'Session("UserID" & dbkey)
commit_type = "full_commit_ns"
formmode = Request( "formmode" )


' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_EXPTS"

'   If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""


%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>

<body <%=Application("BODY_BACKGROUND")%>>

<%
'add some helper text for the wizard

select case lcase(wizardStep)
		case "wiz_add_deg_1"
		session("wizard") = "wiz_add_deg_2"
		%>
		<P>Adding a Degradation compound.
		<ol>
			<li>Select Parent compound
				
			<li class=highlightcopy>Select Experiment
				<ul>
					<li>Input the Experiment information
				</ul>
			<li>Add Degradant
		</ol>
		<%case "wiz_add_exp_1"
		session("wizard") = "wiz_add_exp_2"%>
		<P>Adding an Experiment.
		<ol>
			<li>Select Parent compound
				
			<li class=highlightcopy>Add Experiment
				<ul>
					<li>Input the Experiment information
				</ul>
		</ol>
		<%case "wiz_add_par_2"
		session("wizard") = "wiz_add_par_3"%>
		<P>Adding an Experiment.
		<ol>	
			<li class=highlightcopy>Add Experiment
				<ul>
					<li>Input the Experiment information
				</ul>
		</ol>
<%end select%>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location_overrride" value = "<%=return_location_overrride%>">	
<input type = "hidden" name = "DRUGDEG_EXPTS.PARENT_CMPD_FK" value = "<%=Request.QueryString( "keyparent" )%>">
<input type = "hidden" name = "DRUGDEG_EXPTS.BASE64_CDX" value = "">

<%if record_added = "true" then%>
<script language="javascript">
		alert( "Your record was added to the temporary table" )
</script>
<%end if
Session( "ReturnToExperimentInput" ) = Session( "CurrentLocation" & dbkey & formgroup )%>

<%
'	Response.Write( "<br>Querystring = " & Request.QueryString )
%>

<table  border = 0>
	<tr>
		<td>
			<strong>Degradation conditions</strong>
		</td>
	</tr>

	<tr>
		<td>
<% on error resume next
			Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
			sCondsList = GetDegCondsList( connBase )
'			Response.Write( "sCondsList = [ " & sCondsList & " ]" )
			ShowLookUpList dbkey, formgroup, BaseRS, "DRUGDEG_EXPTS.DEG_COND_FK", sCondsList, "", "", 0, true, "value", "0"
%>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td  height = 20>
		</td>
	</tr>

	<tr>
		<td>
			<strong>API state</strong>
		</td>
	</tr>
	<tr>
		<td>
			<%
				on error resume next
									
				sAPIFormulatedList = """API"":API,""Formulated product"":Formulated product,""API Excipient blend"":API Excipient blend"
								
	
				ShowLookUpList dbkey, formgroup, BaseRS, "DRUGDEG_EXPTS.API_FRM", sAPIFormulatedList, "", "", 0, false, "value", "API"
									
			%>
		</td>
	</tr>
	
	<!-- A blank row for separation. -->
	<tr>
		<td  height = 20>
		</td>
	</tr>

	<tr>
		<td>
			<strong>Experimental conditions</strong>
		</td>
	</tr>
	<tr>
		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_EXPTS.EXPT_CONDS", "TEXTAREA:5", "40"%>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td  height = 20>
		</td>
	</tr>

	<tr>
		<td>
			<strong>Notes</strong>
		</td>
	</tr>
	<tr>
		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_EXPTS.NOTES", "TEXTAREA:5", "40"%>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td  height = 20>
		</td>
	</tr>

	<tr>
		<td>
			<strong>References</strong>
		</td>
	</tr>
	<tr>
		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_EXPTS.REFERENCES", "TEXTAREA:5", "40"%>
			<br>Sample reference:<br>
			Waterman, K. C.; Adami, R. C.; Alsante, K. M.; <br>
			Antipas, A. S.; Arenson, D. R.; Carrier, R.; Hong, J.; <br>
			Landis, M. S.; Lombardo, F.; Shah, J. C.; Shalaev, E.; <br>
			Smith, S. W.; Wang, H. "Hydrolysis in Pharmaceutical <br>
			Formulations," <i>Pharmaceutical Development and Technology</i>, <br>
			2002, 7(2), 113-146. 
		</td>
	</tr>

</table>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
