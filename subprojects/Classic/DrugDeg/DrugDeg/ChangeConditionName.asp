<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Change condition text</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit_ns"
formmode = Request( "formmode" )

' start add_record information additions to input page

' Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_CONDS"

' If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""

%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>

<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location_overrride" value = "<%=return_location_overrride%>">	
<input type = "hidden" name = "DegCondKey" value ="<%=Request.QueryString( "keyprimary" )%>">
<input type = "hidden" name = "Operation" value ="MODIFY">

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

' Make a record set for the degradation experiment condition whose text we are changing.
Dim	rsDegCond
Set rsDegCond = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_CONDS where DEG_COND_KEY = " & Request.QueryString( "keyprimary" )
rsDegCond.Open sSQL, connDB
%>
<table  border = 0  bordercolor = "blue">
	<tr>
		<td  align = "right">
			Current degradation condition name:
		</td>

		<td>
			<%=rsDegCond.Fields( "DEG_COND_TEXT" )%>
		</td>
	</tr>

	<tr>
		<td  align = "right">
			New degradation condition name:
		</td>

		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_CONDS.DEG_COND_TEXT", 0, "50"%>
		</td>
	</tr>
</table>

<%
' Close the record set and database connection.
rsDegCond.Close
connDB.Close
%>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->

</body>

</html>
