<%@LANGUAGE="VBScript"%>
<%
'stop
Response.Expires = 0
dbkey = Request("dbname")
if dbkey = "" then dbkey = "docmanager"
'if Not Session("UserValidated" & dbkey) = 1 then
'	response.redirect "../login.asp?dbname=" & dbkey & "&formgroup=base_form_group&perform_validate=0"
'end if
%>

<script language="javascript">
if(parent.location.href != window.location.href) parent.location.href = window.location.href;
function go_get_sql_string (formgroup,sql_string,table_name,limit_access_to) {
		document.cows_input_form.sql_string.value = sql_string
		document.cows_input_form.action= "/<%=Application("Appkey")%>/default.asp?dbname=" & dbkey & "&formgroup=" + formgroup + "&dataaction=get_sql_string&sql_source=session_sql_string&table_name=" + table_name + "&limit_access=" + limit_access_to
		document.cows_input_form.submit()
}
</script>
<!-- cs_security -->
<!--#include virtual="/cfserverasp/source/cs_security/cs_security_utils.js"-->

<%
Session("CurrentLocation" & dbkey) = ""
If Not Session("CurrentUser" & dbkey) <> "" then
	theuser = Session("UserName" & dbkey)
	if theuser <> "" then
		Session("CurrentUser" & dbkey) = theuser
	end if
end if
%>

<html>

<head>
<%
projects_sql = "Select * from projects"
workgroup_sql = "Select * from people"
experimenttype_sql= "Select experimenttype.experiment_type_id from experimenttype"

'set flags to true until security is in place, whether to display
'the buttons should based on user's previlige
'Session("Search_Docs" & dbkey) = True
'Session("Edit_Users_Table" & dbkey) = True
'Session("Edit_Sites_Table" & dbkey) = True
'Session("Edit_People_Table" & dbkey) = True
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
<meta NAME="GENERATOR" Content="Microsoft FrontPage 4.0">
<title>Document Manager - Main Page</title>
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!-- CBOE-1823 added code to display Document Manager help on F1 click. Debu 05SEP13 -->
 <script language="javascript" type="text/javascript">
     function onkeydown_handler() {
         switch (event.keyCode) {
             case 112: // 'F1'
                 document.onhelp = function () { return (false); }
                 window.onhelp = function () { return (false); }
                 event.returnValue = false;
                 event.keyCode = 0;
                 OpenDialog('../../../../CBOEHelp/CBOEContextHelp/Doc%20Manager%20Webhelp/Default.htm', 'help', 2);
                 return false;
                 break;
         }
     }
     document.attachEvent("onkeydown", onkeydown_handler);
    </script>
</head>



<body background="<%=Application("UserWindowBackground")%>">

	<!---JHS added 4/9/2003--->
	<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
		<!-- The table for the banner. -->
		<tr>

			<td valign="top" width="300">
				<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
			</td>

			<td>
					<font face="Arial" color="#0099FF" size="4"><i>
						Main Menu
					</i></font>
			</td>
		</tr>
	</table>
	<!---JHS added 4/9/2003 end--->

<form name="cows_input_form" method="post" action="/<%=Application("appkey")%>/default.asp">
	<input type="hidden" name="sql_string">
</form>

<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td><table border="0">
				<!--- JHS commented out 4/9/2003 tr>					<td colspan="2"><img src="/docmanager/docmanager/gifs/banner.gif"></td>				</tr--->

				<tr>
					<td align="right"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & dbkey)%></font></font></td>
					<td align="center"><a href="/<%=Application("appkey")%>/logoff.asp"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td>
				</tr>
			</table>
			<br>
			<br>
			<table width="300" border="0">	
				<tr><td colspan="2"><br><b><font color="#990000">User Tools:</font></b></td>
				</tr>
				<%if Session("SUBMIT_DOCS" & dbkey) = True then %>
					<tr><td height="40"><b><font color="#000099">Submit Documents</font></b></td>
						<td align="left">
							<a href="/<%=Application("appkey")%>/docmanager/src/locatedocs.asp"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>
					    </td>
					</tr>
				<%end if%>

				<%if Session("SEARCH_DOCS" & dbkey) = True then %>
					<tr><td height="40"><b><font color="#000099">Search Documents</font></b></td>
						<td align="left">
							<a href="/<%=Application("appkey")%>/default.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
			            </td>
					</tr>
				<%end if%>
				
				<%if Session("BATCHLOAD_DOCS" & dbkey) = True then %>
					<tr><td height="40"><b><font color="#000099">Batch Submit</font></b></td>
						<td align="left">
							<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
			            </td>
					</tr>
				<%end if%>

				<%if Session("VIEW_HISTORY" & dbkey) = True then %>
					<tr><td height="40"><b><font color="#000099">Recent Activities</font></b></td>
						<td align="left">
							<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
			            </td>
					</tr>
				<%end if%>

				<%'if Session("RUN_DB_JOBS" & dbkey) = True then %>
					<!--tr><td height="40"><b><font color="#000099">Link Documents To BioSar</font></b></td>
						<td align="left">
							<a href="/<%=Application("appkey")%>/docmanager/RunJobs.asp"><img src="/<%=Application("appkey")%>/graphics/link_documents_btn.gif" border="0"></a>
			            </td>
					</tr-->
				<%'end if%>
				
				
				<% 
				'leaving just in case but hiding all cs security links
				showSecurity = false
				if showSecurity then
					<!--SYAN modified on on 2/22/2006 to fix CSBR-64263-->
					if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>	
				<tr>
					<td height="40"><b><font color="#000099">Change Password</font></b></td>
					<td align="left">
						<a href="#" onclick="OpenDialog('/docmanager/cs_security/password.asp?dbkey=docmanager&amp;PrivTableName=DOCMANAGER_PRIVILEGES', 'Musers', 2); return false">
						<img src="/<%=Application("appkey")%>/graphics/changepassword.gif" border="0"></a>
					</td>
				</tr>
					<%End if%>
				<%
					if (Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey)) then%>
					<tr><td height="40"><b><font color="#000099">Manage users</font></td>
						<td align="left">
						<a href="#" onclick="OpenDialog('/docmanager/cs_security/manageUsers.asp?dbkey=docmanager&amp;PrivTableName=DOCMANAGER_PRIVILEGES', 'Musers', 2); return false">
						<img src="/<%=Application("appkey")%>/graphics/usr_mgr_btn.gif" border="0"></a>
						</td>
					</tr>
					<%end if%>
					<%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>

					<tr><td height="40"><b><font color="#000099">Manage Roles</font></b></td>
						<td align="left">
						<a href="#" onclick="OpenDialog('/docmanager/cs_security/manageroles.asp?dbkey=docmanager&amp;PrivTableName=DOCMANAGER_PRIVILEGES', 'Musers', 2); return false">
							<img src="/<%=Application("appkey")%>/graphics/manage_roles_btn.gif" border="0"></a>
						</td>
					</tr>
				<%	end if
				end if%>			
				<!--End of SYAN modification-->
				
				<tr><td height="40"><b><font color="#000099">WebServer Home</font></b></td>
						<td align="left">
						<a href="/cs_security/home.asp">
							<img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
						</td>
					</tr>
			</table>
		</td>
	</tr>
</table>


</body>
</html>
