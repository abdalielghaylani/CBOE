<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
	<title>Manage functional group list</title>
	
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>

<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit_ns"
formmode = Request( "formmode" )

' Set up a session variable for this as the current page displayed.
'JHS commented out 4/2/03
'Session( "ReturnToConditionListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
'JHS commented out 4/2/03 end

'JHS added 4/2/03
sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/FunctionalGroup_input_form.asp?record_added=false&formmode=add_fgroup&formgroup=FGroup_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"
Session("ReturnToFunctionalGroupListAdmin" & dbkey) = sPath
'JHS added 4/2/03 end

' start add_record information additions to input page

' Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_FGROUPS"

' If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""

%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>
<link REL="stylesheet" TYPE="text/css" HREF="/drugdeg/styles.css">
<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type="hidden" name="add_order" value="<%=add_order%>">
<input type="hidden" name="add_record_action" value="<%=add_record_action%>">	
<input type="hidden" name="commit_type" value="<%=commit_type%>">	
<input type="hidden" name="return_location_overrride" value="<%=return_location_overrride%>">	
<input type="hidden" name="Operation" value="ADD">

<%if record_added = "true" then%>
<script language="javascript">
	alert("Your record was added to the temporary table")
</script>
<%end if%>


<%
' Open a connection for the current conditions list.
Set connDB = GetNewConnection( dbkey, formgroup, "base_connection" )

if 0 <> err.number then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if


' Successfully opened the connection to the database.

' Make a record set for the degradation experiment conditions.
Dim	rsFGroups
Set rsFGroups = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_FGROUPS order by DEG_FGROUP_TEXT"
rsFGroups.Open sSQL, connDB

%>
<table border="0" bordercolor="blue">
	<tr>
		<td>
			New functional group<%
				if not rsFGroups.BOF or not rsFGroups.EOF then
					' If there are conditions already in the database, put up the
					' asterisk for the "Make sure it isn't already there" notice.
				%>*<% end if %>
		</td>

		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_FGROUPS.DEG_FGROUP_TEXT", 0, "50"%>
		</td>
	</tr>

	<%
	if not rsFGroups.BOF or not rsFGroups.EOF then
		' There are some degradation condition records.  Put up a notice to look over the
		' conditions already in the database.
	%>
	<!-- Add a little vertical space. -->
	<tr>
		<td height="10">
		</td>
	</tr>

	<tr>
		<td colspan="2">
			*Please look over the list of functional groups below
			<strong>before</strong> adding another.
		</td>
	</tr>
	<% end if %>

	<!-- Add a little vertical space. -->
	<tr>
		<td height="10">
		</td>
	</tr>
</table>

<%
if not rsFGroups.BOF or not rsFGroups.EOF then
	' There are some degradation condition records.  Put up a notice to look over the
	' conditions already in the database, and then list those conditions.
%>
<!-- Show all the conditions currently in the database. -->
<table border="1" bgcolor="#E2E2E2" bordercolor="#C0C0C0">
	<!-- Header for the whole table. -->
	<tr>
		<th colspan="3">
			Functional groups currently in the database
		</th>
	</tr>

<%
	rsFGroups.MoveFirst
	while not rsFGroups.EOF
%>
	<tr>
		<td>
<%
		if FunctionalGroupHasDegradants( rsFGroups.Fields( "DEG_FGROUP_KEY" ), connDB ) then
			' There are experiments for this particular condition.  It is not to be deleted.
			' Put in a space to ensure the cell gets drawn.
%>
			&nbsp;
<%
		else
			' There are no experiments for this particular condition.  Allow it to be deleted.
			keyFGroup = rsFGroups.Fields( "DEG_FGROUP_KEY" )

			sCall = "/" & Application( "AppKey" ) & "/" & dbkey & "/DeleteConfirm.asp?dbname=" & dbkey & "&formgroup=base_form_group&deltype=fgroup&keyprimary=" & keyFGroup
%>
			<nobr><a href="<%=sCall%>">
				Delete Functional Group
			</a>
<!-- -			<br><%=sCall%><!-- -->
<%
		end if
%>
		</td>

		<td>
			<!-- <a href="ChangeFunctionalGroupName.asp?dbname=<%=dbkey%>&amp;formgroup=FGroup_form_group&amp;formmode=modify_fgroup&amp;formgroupflag=ChangeFunctionalGroupName&amp;keyprimary=<%=rsFGroups.Fields( "DEG_FGROUP_KEY" )%>">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_ChangeText.gif" border="0">
			</a>-->
		</td>

		<td nowrap>
			<% Response.Write( rsFGroups.Fields( "DEG_FGROUP_TEXT" ) ) %>
		</td>
	</tr>
<%
		rsFGroups.MoveNext
	wend
end if  ' if not rsFGroups.BOF or not rsFGroups.EOF then ...

' Close the record set and database connection.
rsFGroups.Close
connDB.Close
%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
