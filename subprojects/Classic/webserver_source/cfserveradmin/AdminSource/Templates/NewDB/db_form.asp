<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) =1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<script language="javascript">
#JAVA_SCRIPT
</script>

<title>#DB_NAME Results - Form View</title>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>
<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated%>
<!--#RS-->
<input type="hidden" name="table_delete_order" value ="<%=table_delete_order%>">
<table>
	<tr>
		<td valign=top>
			<script language="JavaScript">
				getRecordNumber(<%=BaseRunningIndex%>)
				document.write ('<br>')
				getMarkBtn(<%=BaseID%>)
			</script>
		</td>
	</tr>
</table><BR>		
<!--#RESULT_FIELDS-->
</body>
</html>
