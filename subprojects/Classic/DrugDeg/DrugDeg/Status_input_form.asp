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
'JHS commented out 4/2/03
'Session( "ReturnToStatusListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
'JHS commente out 4/2/03 end

'JHS added 4/2/03
sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Status_input_form.asp?record_added=false&formmode=add_status&formgroup=Status_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"
Session( "ReturnToStatusListAdmin" & dbkey ) = sPath
'JHS added 4/2/03 end

' start add_record information additions to input page

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

if err.number <> 0 then
	' The connection couldn't be opened.
	Set connDB = nothing
	connDB = ""

	' Redirect to an error dialog.
	Response.Redirect( "db-not-open-error.html" )
end if



%>
<strong>Enter new status information and click &quot;Add Record&quot;</strong>
<br>
<table border="0" bordercolor="blue">
	
	<tr>
		<td align="right">
			Status name:
		</td>
		<td align="left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_STATUS.STATUS_TEXT", 0, "50"%>
		</td>
	</tr>
</table>

<br clear="all"><br clear="all">
<%
Set rsGetStatuses = Server.CreateObject("ADODB.Recordset")
Set rsGetStatuses = connDB.execute("select * from DRUGDEG_STATUSES order by upper(STATUS_TEXT)")

%>
<script language="javascript">
function gotoEditStatus(statusSelectForm){
	if (statusSelectForm.selectedIndex >=0) {
		document.location.href = "ChangeStatusName.asp?dbname=drugdeg&formgroup=Status_form_group&formmode=modify_status&formgroupflag=ChangeStatusName&keyprimary=" + statusSelectForm.options[statusSelectForm.selectedIndex].value;
	}
	else {
		alert('You must select a status.');
	}
}
</script>
<strong>Select a status to edit</strong><br clear="all">

<table border="0" bordercolor="blue">
	<tr>
		<td align="right">
			<select name="StatusListForEdits" id="StatusListForEdits" size="10" ondblclick="gotoEditStatus(this.form.StatusListForEdits)">
			<%

			while not rsGetStatuses.EOF

				Response.Write "<option value=""" & rsGetStatuses.Fields("STATUS_KEY").value & """>"
				Response.write rsGetStatuses.Fields("STATUS_TEXT") & " (" & rsGetStatuses.Fields("STATUS_KEY") & ")"
				Response.Write "</option>"

				rsGetStatuses.MoveNext
			wend 
			%>
			</select>
		</td>
		<td>
			<a href="#" onclick="gotoEditStatus(document.cows_input_form.StatusListForEdits);"><img src="/<%=Application("appkey")%>/graphics/edit_record_btn.gif" border="0"></a>
			<!---input type="submit" name="EditStatusSubmit" onclick="gotoEditStatus(this.form.StatusListForEdits);return false;" value="Edit Status"--->
		</td>
	</tr>
</table>
<%
rsGetStatuses.Close
connDB.Close
%>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->



</body>
</html>
