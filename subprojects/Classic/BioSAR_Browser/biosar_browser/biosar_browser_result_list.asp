<%@ LANGUAGE=VBScript %>
<%Response.ExpiresAbsolute = Now()%>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
CDD_RESULT_DEBUG = false
%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<!--#INCLUDE FILE = "../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>#DB_NAME Results-List View</title>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE="../reports/reports_app_js.js"-->

</head>
<body <%=body_default%>>
<%theResult = getIsFormGroupPublic(formgroup, dbkey)
if Not (Session("hitlistRecordCount" & dbkey & formgroup) = "0" or Session("hitlistRecordCount" & dbkey & formgroup)= "")then%>
<script language = "javascript">
		//document.write(getGenerateReportButton())
</script>
<%end if%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_vbs.asp"-->

		<%
		
			getJumpForms dbkey, formgroup, Application("BASE_TABLEbiosar_browser" & formgroup), "biosar_browser_result_list.asp"
%>
		<%
		GetItems formgroup, dbkey,  "LIST", "REPORT"%>
		<%Response.Write Session("LEFT" & "LIST" & dbkey & formgroup)
		%>
		
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>
</html>
