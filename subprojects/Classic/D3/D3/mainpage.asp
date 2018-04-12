<%@LANGUAGE = "VBScript"%>
<%
if CBool( Session( "DD_ADD_RECORDS" & dbkey )) <> True then
Response.Redirect "/D3/default.asp?formgroup=base_form_group&dbname=D3&dataaction=db"
end if

Response.Expires = 0
dbkey = Request( "dbname" )
Session("wizard") = ""
Session( "CurrentLocation" & dbkey & formgroup ) = ""
Session( "CurrentLocation" & dbkey ) = ""
Session( "LastLocation" & dbkey ) = ""

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
	<!--#INCLUDE VIRTUAL = "/D3/source/app_vbs.asp" -->

		<title>Main page, Drug Degradation database</title>

	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
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
					<td valign="top">
						<img src="/<%=Application("appkey")%>/graphics/D3_Logo_SideWeb.jpg" alt="Drug Degradation" align="center">
					</td>
					
				</tr>
			</table>
		</td>
	</tr>
</table>

<br>

<table border="0" bordercolor="red">
	<!-- +++++++++++++++++++++++++++++++++ -	<tr>		<td  colspan = 3>			<a href="/<%=Application( "appkey" )%>/D3/TableDump_list.asp?formgroup=base_form_group&dbname=<%=dbkey%>">				Dump table information			</a>		</td>	</tr>	<!-- +++++++++++++++++++++++++++++++++ -->

	<%if CBool( Session( "dd_Search" & dbkey )) = True then%>
	<!-- Search parent compounds. -->
	<tr>
		<td colspan="5"><a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db">Search</a>
			<br>
			<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True or CBool( Session( "DD_EDIT_RECORDS" & dbkey )) = True then%>
				Search and edit parent compounds
			<%else%>
				Search parent compounds, degradation compounds and experiments
			<%end if%>
		</td>

	</tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>
	<%end if%>

	<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True then%>
	<!-- Add new parent compound. -->
    <tr>
		<td>
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=AddParent_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=parent_full_commit&amp;record_added=false&amp;wizard=wiz_add_par_1">Add Parent Compound</a>
			<br>Add new parent compound
		</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db&amp;wizard=wiz_edit_par_1">Edit Parent Compound</a>
			<br>Edit new parent compound</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/docmanager/docmanager/mainpage.asp">DocManager</a>
			<br>Main page</td>
    </tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>
	
	<!-- Add new parent compound. -->
    <tr>
		<td>
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db&amp;wizard=wiz_add_exp_1">Add Experiment</a>
			<br>Add new experiment
		</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db&amp;wizard=wiz_edit_exp_1">Edit Experiment</a>
			<br>Edit an experiment</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/docmanager/default.asp?formgroup=base_form_group&dbname=docmanager&dataaction=db">Search DocManager</a>
			<br>Search documents in DocManager</td>
    </tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>

	<!-- Add new parent compound. -->
    <tr>
		<td>
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db&amp;wizard=wiz_add_deg_1">Add Degradant</a>
			<br>Add new degradant
		</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=db&amp;wizard=wiz_edit_deg_1">Edit Degradant</a>
			<br>Edit a degradant</td>
		<td>&nbsp;</td>
		<td><a class="headinglink" href="/docmanager/docmanager/src/locatedocs.asp">Submit Docs</a>
			<br>Add documents to DocManager</td>
    </tr>

	<!-- A blank row for separation. -->
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>

	<%end if%>

	<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True then%>
	<!-- Manage degradation conditions. -->
    <tr>
		<td colspan="5">
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=Condition_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=condition_list_admin&amp;record_added=false">Add Experimental Condition</a>
			<br>Manage the list of experimental conditions
		</td>
    </tr>
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>

	<!-- Manage functional groups. -->
    <tr>
		<td colspan="5">
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=FGroup_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=fgroup_list_admin&amp;record_added=false">Manage functional group list</a>
			<br>Manage the list of degradation functional groups
		</td>
    </tr>
	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>
	
	<!-- Manage salts. -->
    <tr>
		<td colspan="5">
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=Salt_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=salt_list_admin&amp;record_added=false">Manage salt list</a>
			<br>Manage the list of salts
		</td>
    </tr>

	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>
	
	<!-- Manage salts. -->
    <tr>
		<td colspan="5">
			<a class="headinglink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=Salt_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=salt_list_admin&amp;record_added=false">Manage salt list</a>
			<br>Manage Documents
		</td>
    </tr>

	<tr>
		<td height="20" colspan="5">&nbsp;</td>
	</tr>

    <%end if%>
</table>
    
<div align="center">
<table border="0" bordercolor="red" cellspacing="15">
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
	
	<tr>
		<td valign="top"><a class="headinglink" href="/cs_security/login.asp?ClearCookies=true">Logout</a>
	
		</td>
		
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/help/help.asp?formgroup=base_form_group&amp;dbname=D3','help','width=800,height=600,scrollbars=yes,status=no,resizable=yes');">Help</a>
	
		</td>
	
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/about.asp?formgroup=base_form_group&amp;dbname=D3','about','width=560,height=450,status=no,resizable=yes')">About the database</a>
		</td>
	</tr>
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
</table>
</div>

</body>

</html>
