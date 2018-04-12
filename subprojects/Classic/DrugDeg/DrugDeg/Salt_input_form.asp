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
'JHS commented out 4/2/03
'Session( "ReturnToSaltListAdmin" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )
'JHS commente out 4/2/03 end

'JHS added 4/2/03
sPath = "/" & Application( "AppKey" ) & "/" & dbkey & "/Salt_input_form.asp?record_added=false&formmode=add_salt&formgroup=Salt_form_group&dbname=" & dbkey & "&commit_type=full_commit_ns"
Session( "ReturnToSaltListAdmin" & dbkey ) = sPath
'JHS added 4/2/03 end

' start add_record information additions to input page

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
<strong>Enter new salt information and click &quot;Add Record&quot;</strong>
<br>
<table border="0" bordercolor="blue">
	<tr>
		<td align="right">
			Salt code:
		</td>
		<td align="left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_SALTS.SALT_CODE", 0, "2"%>
		</td>
	</tr>

	<tr>
		<td align="right">
			Salt name:
		</td>
		<td align="left">
			<%ShowInputField dbkey, formgroup, "DRUGDEG_SALTS.SALT_NAME", 0, "50"%>
		</td>
	</tr>
</table>

<br clear="all"><br clear="all">
<%
Set rsGetSalts = Server.CreateObject("ADODB.Recordset")
Set rsGetSalts = connDB.execute("select * from DRUGDEG_SALTS order by upper(SALT_NAME)")

%>
<script language="javascript">
function gotoEditSalt(saltSelectForm){
	if (saltSelectForm.selectedIndex >=0) {
		document.location.href = "ChangeSaltName.asp?dbname=drugdeg&formgroup=Salt_form_group&formmode=modify_salt&formgroupflag=ChangeSaltName&keyprimary=" + saltSelectForm.options[saltSelectForm.selectedIndex].value;
	}
	else {
		alert('You must select a salt.');
	}
}
</script>
<strong>Select a salt to edit</strong><br clear="all">

<table border="0" bordercolor="blue">
	<tr>
		<td align="right">
			<select name="SaltListForEdits" id="SaltListForEdits" size="10" ondblclick="gotoEditSalt(this.form.SaltListForEdits)">
			<%

			while not rsGetSalts.EOF

				Response.Write "<option value=""" & rsGetSalts.Fields("SALT_KEY").value & """>"
				Response.write rsGetSalts.Fields("SALT_NAME") & " (" & rsGetSalts.Fields("SALT_CODE") & ")"
				Response.Write "</option>"

				rsGetSalts.MoveNext
			wend 
			%>
			</select>
		</td>
		<td>
			<a href="#" onclick="gotoEditSalt(document.cows_input_form.SaltListForEdits);"><img src="/<%=Application("appkey")%>/graphics/edit_record_btn.gif" border="0"></a>
			<!---input type="submit" name="EditSaltSubmit" onclick="gotoEditSalt(this.form.SaltListForEdits);return false;" value="Edit Salt"--->
		</td>
	</tr>
</table>
<%
connDB.Close
%>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->



</body>
</html>
