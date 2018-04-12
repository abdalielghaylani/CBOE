<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
	<title>Manage conditions list</title>
	
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
sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Condition_input_form.asp?record_added=false&formmode=add_condition&formgroup=Condition_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"
Session("ReturnToConditionListAdmin" & dbkey) = sPath
'JHS added 4/2/03 end

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
Dim	rsDegConds
Set rsDegConds = Server.CreateObject( "ADODB.Recordset" )
sSQL = "select * from DRUGDEG_CONDS order by DEG_COND_TEXT"
rsDegConds.Open sSQL, connDB

%>
<table border="0" bordercolor="blue">
	<tr>
		<td>
			New degradation condition<%
				if not rsDegConds.BOF or not rsDegConds.EOF then
					' If there are conditions already in the database, put up the
					' asterisk for the "Make sure it isn't already there" notice.
				%>*<% end if %>
		</td>

		<td>
			<%ShowInputField dbkey, formgroup, "DRUGDEG_CONDS.DEG_COND_TEXT", 0, "50"%>
		</td>
	</tr>

	<%
	if not rsDegConds.BOF or not rsDegConds.EOF then
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
			*Please look over the list of degradation conditions below
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
if not rsDegConds.BOF or not rsDegConds.EOF then
	' There are some degradation condition records.  Put up a notice to look over the
	' conditions already in the database, and then list those conditions.
%>
<!-- Show all the conditions currently in the database. -->
<table border="1" bgcolor="#E2E2E2" bordercolor="#C0C0C0">
	<!-- Header for the whole table. -->
	<tr>
		<th colspan="3">
			Conditions currently in the database
		</th>
	</tr>

<%
	rsDegConds.MoveFirst
	while not rsDegConds.EOF
%>
	<tr>
		<td>
<%
		if ConditionHasExperiments( rsDegConds.Fields( "DEG_COND_KEY" ), connDB ) then
			' There are experiments for this particular condition.  It is not to be deleted.
			' Put in a space to ensure the cell gets drawn.
%>
			&nbsp;
<%
		else
			' There are no experiments for this particular condition.  Allow it to be deleted.
			keyCond = rsDegConds.Fields( "DEG_COND_KEY" )

			sCall = "/" & Application( "AppKey" ) & "/" & dbkey & "/DeleteConfirm.asp?dbname=" & dbkey & "&formgroup=base_form_group&deltype=condition&keyprimary=" & keyCond
%>
			<a href="<%=sCall%>">
				<img SRC="/drugdeg/graphics/Button_DeleteCondition.gif" BORDER="0">
			</a>
<!-- -			<br><%=sCall%><!-- -->
<%
		end if
%>
		</td>

		<td>
			<a href="ChangeConditionName.asp?dbname=<%=dbkey%>&amp;formgroup=Condition_form_group&amp;formmode=modify_condition&amp;formgroupflag=ChangeConditionName&amp;keyprimary=<%=rsDegConds.Fields( "DEG_COND_KEY" )%>">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_ChangeText.gif" border="0">
			</a>
		</td>

		<td nowrap>
			<% Response.Write( rsDegConds.Fields( "DEG_COND_TEXT" ) ) %>
		</td>
	</tr>
<%
		rsDegConds.MoveNext
	wend
end if  ' if not rsDegConds.BOF or not rsDegConds.EOF then ...

' Close the record set and database connection.
rsDegConds.Close
connDB.Close
%>
</table>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
