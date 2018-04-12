<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Manage functional group list</title>

</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit_ns"
formmode = Request( "formmode" )

' Set up a session variable for this as the current page displayed.
Session( "ReturnToConditionListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_FGROUPS"

'   If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""

%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>


<body  <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location_overrride" value = "<%=return_location_overrride%>">	

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
	connDB.Close
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if


' Successfully opened the connection to the database.

' Make a record set for the degradation experiment conditions.
Dim	rsFGroups
Set rsFGroups = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_FGROUPS order by Deg_FGroup_text"
rsFGroups.Open sSQL, connDB

%>
<table  border = 0  bordercolor = "blue">
	<tr>
		<td>
			New functional group<%
				if not rsFGroups.BOF or not rsFGroups.EOF then
					' If there are functional groups already in the database, put up the
					' asterisk for the "Make sure it isn't already there" notice.
				%>&#42;<% end if %>
		</td>

		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_FGROUPS.Deg_FGroup_text", "0", "50"%>
		</td>
	</tr>

	<%
	if not rsFGroups.BOF or not rsFGroups.EOF then
		' There are some functional group records.  Put up a notice to look over the
		' groups already in the database.
	%>
	<!-- Add a little vertical space. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>

	<tr>
		<td  colspan = 2>
			&#42;Please look over the list of functional groups below
			<strong>before</strong> adding another.
		</td>
	</tr>
	<% end if %>

	<!-- Add a little vertical space. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>
</table>

<%
if not rsFGroups.BOF or not rsFGroups.EOF then
	' There are some functional group records.  Put up a notice to look over the
	' groups already in the database, and then list those groups.
%>
<!-- Show all the conditions currently in the database. -->
<table  border = 0  bgcolor = "#E2E2E2"  bordercolor = "#C0C0C0">
<%
	rsFGroups.MoveFirst
	while not rsFGroups.EOF
%>
	<tr>
		<td  nowrap>
			<% Response.Write( rsFGroups.Fields( "Deg_FGroup_text" ) ) %>
		</td>
	</tr>
<%
		rsFGroups.MoveNext
	wend
end if  ' if not rsDegConds.BOF or not rsDegConds.EOF then ...

' Close the record set and database connection.
rsFGroups.Close
connDB.Close
%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
