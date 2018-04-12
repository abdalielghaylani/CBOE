<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) =1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<title>#DB_NAME Add Record Form</title>
</head>

<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<% 
' start add_record additions to input page
commit_type = "full_commit" ' when adding a stucture this must be full_commit. If no structure then this must be full_commit_ns.
formmode = Request("formmode")
add_record_action= "DUPLICATE_SEARCH_NO_ADD" ' change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are not added

'or ,"Duplicate_Search_Add" if you want to check for duplicates but still add them.  The duplicate id's in either case are stored in
'Session("duplicates_found" & dbkey) which you can look at and report after response is returned.


add_order = ">#DB_TABLE_ORDER" 'add comma delimited list of tables (or single table if only one). Table addition will cascade based on this if 
'more then one table is entered. The links between tables are those defined for the table in the ini file.
return_location_overrride ="" ' if you want to override, or append the default return location (which for this form is this form) then add informatio
'to this field. Session("CurrentLocation" & dbkey) is the standard return location. You can append a "&myfield=myvalue" to this to have it returned in the querystring.
%>
<script language = "javascript">MainWindow.commit_type = "<%=commit_type%>"</script>
<input type = "hidden" name = "MOLTABLE.BASE64_CDX" value = "">
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location_overrride" value = "<%=return_location_overrride%>">	


<!--#INPUT_FIELDS-->
</body>
</html>
