<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Manage salt list</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit_ns"
formmode = Request( "formmode" )

' Set up a session variable for this as the current page displayed.
Session( "ReturnToSaltListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_SALTS"

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

' Make a record set for the salts.
Dim	rsSalts_OrderedByCode
Set rsSalts_OrderedByCode = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_SALTS order by SALT_CODE"
rsSalts_OrderedByCode.Open sSQL, connDB

%>
<table  border = 0  bordercolor = "blue">
	<tr>
		<td  colspan = 3>
			Enter information for new salt<%
				if not rsSalts_OrderedByCode.BOF or not rsSalts_OrderedByCode.EOF then
					' If there are salts already in the database, put up the
					' asterisk for the "Make sure it isn't already there" notice.
				%>&#42;<% end if %>
				or salt code and new name for a salt whose name you wish to change.
		</td>
	</tr>

	<tr>
		<td  align = "right">
			&nbsp; &nbsp; &nbsp; Salt&nbsp;code:
		</td>
		<td  align = "left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_SALTS.SALT_CODE", 0, "2"%>
		</td>
	</tr>

	<tr>
		<td  align = "right">
			&nbsp; &nbsp; &nbsp; Salt&nbsp;name:
		</td>
		<td  align = "left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_SALTS.SALT_NAME", 0, "50"%>
		</td>
	</tr>
<%
	if not rsSalts_OrderedByCode.BOF or not rsSalts_OrderedByCode.EOF then
		' There are some salts already in the database.  Put up a notice to look over
		' those salts before adding a new one.
%>
	<!-- A little vertical spacing. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>

	<tr>
		<td  colspan = 2>
			&#42;Please look over the lists of salts below <strong>before</strong> adding another.
		</td>
	</tr>
<% end if %>
</table>

<%
if not rsSalts_OrderedByCode.BOF or not rsSalts_OrderedByCode.EOF then
	' There are some salts already in the database.  Put up a notice to look
	' over those salts, and then list them.
%>


<!-- Show all the salts currently in the database. -->
<table  border = 0  bgcolor = "#E2E2E2"  bordercolor = "red">
	<!-- Header for the whole table. -->
	<tr>
		<th  colspan = 3>
			Salts currently in the database
		</th>
	</tr>

	<!-- A little space between the note and the lists of salts. -->
	<tr>
		<td  height = 10>
		</td>
	</tr>

	<!-- Headers for the two lists of salts. -->
	<tr>
		<th  align = "left">
			Ordered by salt code
		</th>

		<!-- A cell for separating the two columns. -->
		<td  width = 40>
			&nbsp;
		</td>

		<th  align = "left">
			Ordered by salt name
		</th>
	</tr>

	<!-- And now for the lists of salts. -->
<%
	' We already have the list of salts ordered by salt code.  Make up a list ordered by name.
	Dim	rsSalts_OrderedByName
	Set rsSalts_OrderedByName = Server.CreateObject( "ADODB.Recordset" )
	sSQL = "select * from DRUGDEG_SALTS order by SALT_NAME"
	rsSalts_OrderedByName.Open sSQL, connDB

	rsSalts_OrderedByCode.MoveFirst
	rsSalts_OrderedByName.MoveFirst
	while not rsSalts_OrderedByCode.EOF
%>
	<tr>
		<td  nowrap>
			<% Response.Write( rsSalts_OrderedByCode.Fields( "SALT_CODE" ) ) %> - <% Response.Write( rsSalts_OrderedByCode.Fields( "SALT_NAME" ) ) %>
		</td>

		<!-- A cell for separating the two columns.  Width set with the column headings. -->
		<td>
		</td>

		<td  nowrap>
			<% Response.Write( rsSalts_OrderedByName.Fields( "SALT_NAME" ) ) %> (<% Response.Write( rsSalts_OrderedByName.Fields( "SALT_CODE" ) ) %>)
		</td>
	</tr>
<%
		rsSalts_OrderedByCode.MoveNext
		rsSalts_OrderedByName.MoveNext
	wend
end if  ' if not rsSalts.BOF or not rsSalts.EOF then ...

' Close the record sets and the database connection.
rsSalts_OrderedByCode.Close
rsSalts_OrderedByName.Close
connDB.Close
%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
