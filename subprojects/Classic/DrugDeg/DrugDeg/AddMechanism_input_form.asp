<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Add degradation mechanism</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added=Request.QueryString("record_added")
commit_type = "full_commit"
formmode = Request("formmode")

' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
add_order = "DRUGDEG_MECHS"

'   If you want to override or append the default return location then add information to this
' field. Session("CurrentLocation" & dbkey & formgroup) is the standard return location. You can
' append a "&myfield=myvalue" to this to have it returned in the querystring.
return_location_overrride = Session( "ReturnToDegradantDetails" & dbkey )

' Get the key for the degradant for which we are adding a mechanism.
keyDeg = Request.QueryString( "keydeg" )
%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<input  type = "hidden"  name = "add_order"  value ="<%=add_order%>">
<input  type = "hidden"  name = "add_record_action"  value = "<%=add_record_action%>">	
<input  type = "hidden"  name = "commit_type"  value = "<%=commit_type%>">	
<input  type = "hidden"  name = "return_location_overrride"  value = "<%=return_location_overrride%>">	
<input  type = "hidden"  name = "DRUGDEG_MECHS.BASE64_CDX"  value = "">
<input  type = "hidden"  name = "DRUGDEG_MECHS.DEG_CMPD_FK"  value = "<%=keyDeg%>">

<%if record_added = "true" then%>
<script language="javascript">
		alert("Your record was added to the temporary table")
</script>
<%end if%>

<table  border = 0>
	<tr>
		<td  valign = "top"  colspan = 3>
			<table  border = 0>
				<tr>
					<td>
						<% ShowStrucInputField  dbkey, formgroup, "DRUGDEG_MECHS.Structure", "5", 800, 500, "Exact", "SelectList" %>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
