<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Add parent</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added = Request.QueryString( "record_added" )
commit_type = "full_commit"
formmode = lcase(Request( "formmode" ))

' Set up a session variable for this as the current page displayed.
'Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

Session("ReturnToCurrentPageDrugDeg") = "/drugdeg/DRUGDEG/AddParent_input_form.asp?record_added=false&formmode=add_parent&formgroup=AddParent_form_group&dbname=DRUGDEG"
'Response.Write Session( "ReturnToCurrentPage" & dbkey )
'Response.write "<br>"

' Set up a session variable for getting back here.
Session( "ReturnToAddParent" & dbkey ) = Session("ReturnToCurrentPage" & dbkey)
'Response.write Session( "ReturnToAddParent" & dbkey )
'Response.write "<br>"

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
'add_record_action = "Duplicate_Search_No_Add"
add_record_action = "Duplicate_Search_Add"
'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
'add_order = "DRUGDEG_PARENTS"
add_order = "DRUGDEG_PARENTS,DRUGDEG_BASE64"

'   If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session("CurrentLocation" & dbkey & formgroup) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = ""

'Response.Write Session("CurrentLocation" & dbkey & formgroup)
'Response.write "<br>"
' Set variables for the height and width of the structure field.
' This makes it easier to change them (no searching around in the code).
heightStructArea = 400
widthStructArea = 400

%>
<script language="javascript"><!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
// End Script hiding --></script>

<body <%=Application("BODY_BACKGROUND")%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input type = "hidden" name = "DRUGDEG_PARENTS.BASE64_CDX" value = "">
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location_overrride" value = "<%=return_location_overrride%>">	

<%if record_added = "true" then%>
<script language="javascript">
	alert("Your record was added to the temporary table")
</script>
<%end if%>

<table  border = 0>
	<tr>
		<td  valign = "top">
			<table  border = 0  color = "green">
				<tr>
					<td>
						<% ShowStrucInputField  dbkey, formgroup, "DRUGDEG_BASE64.Structure", "5", widthStructArea, heightStructArea, "Exact", "SelectList" %>
					</td>
				</tr>
			</table>
		</td>

		<td  valign = "top">
			<table  border = 0  color = "red">
				<tr>
					<td  valign = "top">
						<strong>Generic Name</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						&nbsp; &nbsp; &nbsp; <%ShowInputField dbkey, formgroup, "DRUGDEG_PARENTS.GENERIC_NAME", 0, "30"%>
					</td>
				</tr>
				<!-- A little vertical spacing before the next item. -->
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<strong>Trade Name</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						&nbsp; &nbsp; &nbsp; <%ShowInputField dbkey, formgroup, "DRUGDEG_PARENTS.TRADE_NAME", 0, "30"%>
					</td>
				</tr>
				
				<!-- A little vertical spacing before the next item. -->
				
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<strong>Common/Other Name</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						&nbsp; &nbsp; &nbsp; <%ShowInputField dbkey, formgroup, "DRUGDEG_PARENTS.COMMON_NAME", 0, "30"%>
					</td>
				</tr>
				
				<!-- A little vertical spacing before the next item. -->
				
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<strong>Compound Number</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						&nbsp; &nbsp; &nbsp; <%ShowInputField dbkey, formgroup, "DRUGDEG_PARENTS.COMPOUND_NUMBER", 0, "30"%>
					</td>
				</tr>
				
				<!-- A little vertical spacing before the next item. -->
				
				<tr>
					<td  height = 20>
					</td>
				</tr>
			</table>

			<%
			' Open a connection for the salts table.
			Set connDB = GetNewConnection( dbkey, formgroup, "base_connection" )
			%>
			<table  border = 1>
				<tr>
					<td  colspan = 2  align = "center">
						<strong>Salts</strong>
					</td>
				</tr>
				<%
				' We may have gotten here after an unsuccessful attempt to add a parent, so
				' there may be salts we need to mark off in the checkboxes.
				sSelectedSaltCodes = Session( "SelectedSalts" & dbkey )

				' Make and fill a record set for the available salts.
				Dim	rsSalts
				Set rsSalts = Server.CreateObject( "ADODB.Recordset" )
				sSQL = "select SALT_CODE, SALT_NAME from DRUGDEG_SALTS order by SALT_CODE"
				rsSalts.Open sSQL, connDB
				while not rsSalts.EOF
				%>
				<tr>
					<td  valign = "top">
						<%
						' Determine whether the current salt is one of those in the list of selected salts.
						if ( 0 < instr( UCase( sSelectedSaltCodes ), UCase( rsSalts.Fields( "SALT_CODE" ) ) ) ) then
							' The current salt _is_ in the list of selected salts.  The box is checked.
						%>
						<input  type = "checkbox"  name = "DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts( "SALT_CODE" )%>"  checked>
						<%
						else
							' The current salt is not in the list of selected salts.
						%>
						<input  type = "checkbox"  name = "DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts( "SALT_CODE" )%>">
						<%
						end if  ' if the current salt is in the list of selected salts...
						%>
						<%=rsSalts( "SALT_CODE" )%> - <%=rsSalts( "SALT_NAME" )%>
						<!-- Without the following &nbsp, there can be a blank line between
							the longest line and the line which follows. -->
						&nbsp;
					</td>

					<td  valign = "top">
						<%
						rsSalts.MoveNext
						if not rsSalts.EOF then  ' if there _is_ a next salt...

							' Determine whether the current salt is one of those in the list of selected salts.
							if ( 0 < instr( UCase( sSelectedSaltCodes ), UCase( rsSalts.Fields( "SALT_CODE" ) ) ) ) then
								' The current salt _is_ in the list of selected salts.  The box is checked.
						%>
						<input  type = "checkbox"  name = "DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts( "SALT_CODE" )%>"  checked>
						<%
							else
								' The current salt is not in the list of selected salts.
						%>
						<input  type = "checkbox"  name = "DRUGDEG_PARENTS.SALT"  value = "<%=rsSalts( "SALT_CODE" )%>">
						<%
							end if  ' if the current salt is in the list of selected salts...
						%>
						<%=rsSalts( "SALT_CODE" )%> - <%=rsSalts( "SALT_NAME" )%>
						<%
							rsSalts.MoveNext
						end if  ' if there _is_ a next salt...
						%>
						<!-- Without the following &nbsp, there can be a blank line between
							the longest line and the line which follows. -->
						&nbsp;
					</td>
				</tr>
<%
				wend  ' while there are still salts...

				CloseRS( rsSalts )
				CloseConn( connDB )
%>
			</table>
		</td>
	</tr>
</table>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
