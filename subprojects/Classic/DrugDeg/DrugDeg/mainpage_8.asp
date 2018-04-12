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


'ShowMessageDialog( _
'	"mainpage.asp: " & _
'	"    Edit_Records: " & Session( "Edit_Records" & dbkey ) & _
'	"    Add_Records: " & Session( "Add_Records" & dbkey ) & _
'	"    Search_Records: " & Session( "Search" & dbkey ) & _
'	"    Delete_Records: " & Session( "dd_Delete_Records" & dbkey ) _
')
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
<%'if CBool(Application("UseCSSecurityApp")) = true then%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js" -->
<!-- script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/cs_security_utils.js"></script -->
<%'end if%>

	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/drugdeg/source/app_vbs.asp" -->

		<title>Main page, Drug Degradation database</title>
</head>



<body <%=Application("BODY_BACKGROUND")%>>

<form name="cows_input_form" method="post" action="/<%=Application( "appkey" )%>/default.asp">
	<input type="hidden" name="sql_string">
</form>

<table>
	<tr>
		<td>
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>
					<td valign="top" width="300">
						<img src="/drugdeg/graphics/logo_drugdeg_250.gif" alt="Drug Degradation" align="center">
					</td>
					<td>
						<font face="Arial" color="#990066" size="4"><i>
						Main Page
						</i></font>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>

<br>

<table border="0" bordercolor="red">
	<!-- +++++++++++++++++++++++++++++++++ -	<tr>		<td  colspan = 3>			<a href="/<%=Application( "appkey" )%>/DrugDeg/TableDump_list.asp?formgroup=base_form_group&dbname=<%=dbkey%>">				Dump table information			</a>		</td>	</tr>	<!-- +++++++++++++++++++++++++++++++++ -->

	<%if CBool( Session( "dd_Search" & dbkey )) = True then%>
	<!-- Search parent compounds. -->
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_Search.gif" border="0" alt="Search parent cmpds">
			</a>
		</td>

		<!-- An empty cell to put space between button and explanatory text. -->
		<td width="20">
			&nbsp;
		</td>

		<td>
			<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True or CBool( Session( "DD_EDIT_RECORDS" & dbkey )) = True then%>
		
			<strong>Search and edit parent compounds</strong><br>
			<strong>View, add and edit information for degradation compounds and experiments</strong><br>
			
			<%else%>
				<strong>Search parent compounds, degradation compounds and experiments</strong><br>
			<%end if%>
		</td>
	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="3">
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
		<td height="20" colspan="3">
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
	<!-- Manage functional groups. -->
    <tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=FGroup_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=fgroup_list_admin&amp;record_added=false">
				Manage functional group list
			</a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage the list of degradation functional groups</strong>
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
		<td height="20" colspan="3">
		</td>
	</tr>
    <%end if%>
    <%if false = True then%>
    <%'if CBool( Session( "DD_APPROVE_RECORDS" & dbkey )) = True then%>
	<!-- manage statuses. -->
    <tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/default.asp?formgroup=Status_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=status_list_admin&amp;record_added=false">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_StatusList.gif" border="0" alt="Manage status list">
			</a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage the list of statuses</strong>
		</td>
    </tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="3">
		</td>
	</tr>
	<%end if%>
<%if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%>
	 <tr>
		<td>
			<a href="#" onclick="OpenDialog('/drugdeg/cs_security/manageUsers.asp?dbkey=drugdeg&amp;PrivTableName=DRUGDEG_PRIVILEGES', 'Musers', 2); return false">
				<img src="/<%=Application("appkey")%>/graphics/usr_mgr_btn.gif" border="0"></a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage users</strong>
		</td>
    </tr>
<%End if%> 
<%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
<tr>
		<td>
			<a href="#" onclick="OpenDialog('/drugdeg/cs_security/manageroles.asp?dbkey=drugdeg&amp;PrivTableName=DRUGDEG_PRIVILEGES', 'Musers', 2); return false">
				<img src="/<%=Application("appkey")%>/graphics/manage_roles_btn.gif" border="0"></a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Manage roles</strong>
		</td>
    </tr>	
<%End if%>
<%if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>		     
<tr>
		<td>
			<a href="#" onclick="OpenDialog('/drugdeg/cs_security/password.asp?dbkey=drugdeg&amp;PrivTableName=DRUGDEG_PRIVILEGES', 'Musers', 2); return false">
				<img src="/<%=Application("appkey")%>/graphics/changepassword.gif" border="0"></a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Change Password</strong>
		</td>
    </tr>	
<%End if%>
	<tr>
		<td height="20" colspan="3">
		</td>
	</tr>
<% if Application( "LoginRequired" & dbkey ) = 1 then%>
	<tr>
		<td>
			<a href="/<%=Application( "appkey" )%>/logoff.asp">
				<img src="/<%=Application( "appkey" )%>/graphics/Button_LogOff_big.gif" border="0"></a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			You are currently logged in as: <strong><%=UCase(Session( "UserName" & dbkey ))%></strong>
		</td>
    </tr>
    
    <tr>
		<td>
			<a href="/cs_security/home.asp">
				<img src="/<%=Application( "appkey" )%>/graphics/home_oval_btn.gif" border="0"></a>
		</td>

		<td width="20">
			&nbsp;
		</td>

		<td>
			<strong>Return to the ChemOffice WebServer Home Page</strong>
		</td>
    </tr>	
    	
<%end if%>
</table>
<div align="center">
	<a class="MenuLink" href="/cs_security/login.asp?ClearCookies=true"><img border="0" SRC="/cs_security/graphics/logoff.gif"></a>
</div>
<br clear="all">
<a href="#" onclick="window.open('/drugdeg/help/help.asp?formgroup=base_form_group&amp;dbname=drugdeg','help','width=800,height=600,scrollbars=yes,status=no,resizable=yes');">
<img src="/cfserverasp/source/graphics/navbuttons/help_btn.gif" border="0"></a>

<a href="#" onclick="window.open('/drugdeg/about.asp?formgroup=base_form_group&amp;dbname=drugdeg','about','width=560,height=450,status=no,resizable=yes')">
<img src="/cfserverasp/source/graphics/navbuttons/about_btn.gif" border="0"></a>


</body>

</html>
