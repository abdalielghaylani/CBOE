<%@LANGUAGE = "VBScript"%>
<%
Response.Expires = 0
dbkey = Request( "dbname" )
if dbkey = "" then
	response.write "the name of the database (dbkey) is missing in the query string or form that requested this page."
	response.end
end if

if 1 = Application( "LoginRequired" & dbkey ) then
	if not 1 = Session( "UserValidated" & dbkey ) then
		response.redirect "/" & Application("appkey") & "/login.asp?dbname=" & dbkey & "&formgroup=base_form_group&perform_validate=0"
	end if
end if
%>
<script language="javascript">
<!-- Hide from older browsers
if ( parent.location.href != window.location.href ) {
	parent.location.href = window.location.href;
}

function go_get_sql_string( formgroup, sql_string, table_name, limit_access_to )
{
	document.cows_input_form.sql_string.value = sql_string
	document.cows_input_form.action = "/<%=Application( "appkey" )%>/default.asp?dbname=<%=dbkey%>&formgroup=" + formgroup + "&dataaction=get_sql_string&sql_source=session_sql_string&table_name=" + table_name + "&limit_access=" + limit_access_to
	document.cows_input_form.submit()
}

// End script hiding -->
</script>
<%
Session( "CurrentLocation" & dbkey & formgroup ) = ""
If Not Session( "CurrentUser" & dbkey ) <> "" then
	theuser = Session( "UserName" & dbkey )
	if theuser <> "" then
		Session( "CurrentUser" & dbkey ) = theuser
	end if
end if
%>
<html>

<head>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->

		<title>Main page, Drug Degradation database</title>

</head>

<body bgcolor="#e0e0ff">

<form name="cows_input_form" method="post" action="/<%=Application( "appkey" )%>/default.asp">
	<input type="hidden" name="sql_string">
</form>

<img src="/<%=Application( "appkey" )%>/graphics/cnco.gif" alt="cnco.gif (2252 bytes)">
<img src="/<%=Application( "appkey" )%>/graphics/Banner_MainPage.gif" alt="Drug degradation database">

<!-- ==================== -<br>Application( "LoginRequired" & dbkey ) = "<%=Application( "LoginRequired" & dbkey )%>"<br>Session( "UserValidated" & dbkey ) = "<%=Session( "UserValidated" & dbkey )%>"<!-- ==================== -->

<br>
<table border="0" bordercolor="red">
	<!-- +++++++++++++++++++++++++++++++++ -	<tr>		<td  colspan = 3>			<a href="/<%=Application( "appkey" )%>/DrugDeg/TableDump_list.asp?formgroup=base_form_group&dbname=<%=dbkey%>">				Dump table information			</a>		</td>	</tr>	<!-- +++++++++++++++++++++++++++++++++ -->

	<%if CBool( Session( "dd_Search" & dbkey )) = True then%>
	<!-- Search parent compounds. -->
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_Search.gif" border="0" alt="Search parent cmpds">
			</a>
		</td>

		<!-- An empty cell to put space between button and explanatory text. -->
		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Search for parent compounds</strong>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20">
		</td>
	</tr>
	<%end if%>

	<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True then%>
	<!-- Add new parent compound. -->
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=AddParent_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=parent_full_commit&amp;record_added=false">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_AddParent.gif" border="0" alt="Add parent cmpd">
			</a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Add new parent compound</strong>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20">
		</td>
	</tr>
	<%end if%>

	<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True then%>
	<!-- Manage degradation conditions. -->
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=Condition_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=condition_list_admin&amp;record_added=false">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_ConditionList.gif" border="0" alt="Manage degradation list">
			</a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage the list of degradation conditions</strong>
		</td>
	</tr>

	<!-- Manage salts. -->
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=Salt_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=salt_list_admin&amp;record_added=false">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_SaltList.gif" border="0" alt="Manage salt list">
			</a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage the list of salts</strong>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20">
		</td>
	</tr>
	<%end if%>

	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=manage_users_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=change_password"><img src="/<%=Application( "appkey" )%>/graphics/changepassword.gif" border="0"></a>
		</td>

		<!-- An empty cell to put space between button and explanatory text. -->
		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Change password</strong>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20">
		</td>
	</tr>
</table>

<% if Application( "LoginRequired" & dbkey ) = 1 then%>
<table border="0" bordercolor="blue">
	<!-- User login information. -->
	<tr>
		<td>
	<%
		if "Anonymous" = Session( "LoginType" & dbkey ) then
	%>
			You are currently logged in as: <strong>Anonymous</strong>
	<% else	%>
			You are currently logged in as: <strong><%=UCase(Session( "UserName" & dbkey ))%></strong>
	<% end if %>
		</td>

		<!-- An empty cell to put space between button and explanatory text. -->
		<td width="20">
			&nbsp;
		</td>

		<td>
			<a href="/<%=Application( "appkey" )%>/logoff.asp">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_LogOff_big.gif" border="0">
			</a>
		</td>
	</tr>
</table>
<%end if%>

</body>

</html>
