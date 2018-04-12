<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Manage Status list</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit_ns"
formmode = Request( "formmode" )

' Set up a session variable for this as the current page displayed.
Session( "ReturnToStatusListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_STATUSES"

'   If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""

%>
<script language = "javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>

<body <%=Application("BODY_BACKGROUND")%>>

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

' Make a record set for the statuses.
Dim	rsStatuses
Set rsStatuses = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_STATUSES order by STATUS_TEXT"
rsStatuses.Open sSQL, connDB

%>
<table  border = 0  bordercolor = "blue">
	<tr>
		<td  colspan = 3>
			Enter information for status.<%
				if not rsStatuses.BOF or not rsStatuses.EOF then
					' If there are statuses already in the database, put up the
					' asterisk for the "Make sure it isn't already there" notice.
				%>&#42;<% end if %>
				
		</td>
	</tr>


	<tr>
		<td  align = "right">
			&nbsp; &nbsp; &nbsp; Status&nbsp;name:
		</td>
		<td  align = "left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_STATUSES.STATUS_TEXT", 0, "50"%>
		</td>
	</tr>
<%
	if not rsStatuses.BOF or not rsStatuses.EOF then
		' There are some statuses already in the database.  Put up a notice to look over
		' those statuses before adding a new one.
%>
	<!-- A little vertical spacing. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>

	<tr>
		<td  colspan = 2>
			&#42;Please look over the lists of statuses below <strong>before</strong> adding another.
		</td>
	</tr>
<% end if %>
</table>

<%
if not rsStatuses.BOF or not rsStatuses.EOF then
	' There are some statuses already in the database.  Put up a notice to look
	' over those statuses, and then list them.
%>


<!-- Show all the statuses currently in the database. -->
<table  border = 0  bgcolor = "#E2E2E2"  bordercolor = "red">
	<!-- Header for the whole table. -->
	<tr>
		<th  >
			Statuses currently in the database
		</th>
	</tr>

	<!-- A little space between the note and the lists of statuses. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>

	
	<!-- And now for the lists of statuses. -->
<%

	rsStatuses.MoveFirst
	while not rsStatuses.EOF
%>
	<tr>
		<td  nowrap>
			<% Response.Write( rsStatuses.Fields( "STATUS_KEY" ) ) %> - <% Response.Write( rsStatuses.Fields( "STATUS_TEXT" ) ) %>
		</td>
	</tr>
<%
		rsStatuses.MoveNext

	wend
end if  ' if not rsStatuses.BOF or not rsStatuses.EOF then ...

' Close the record sets and the database connection.

rsStatuses.Close
connDB.Close
%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
