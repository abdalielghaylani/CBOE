<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = request("dbname")
formgroup = request("formgroup")
formmode = request("formmode")
Response.ExpiresAbsolute = Now()
result_form = "bioassay_hts_form"
%>
<script language="JavaScript">
	if  (top.main.location.href.indexOf("<%=result_form%>.asp") == -1){
		top.main.location.href = "<%=result_form%>.asp?<%=request.QueryString%>";
	}
	theMainFrame = top.main.mainFrame;
	top.main.rightFrame.location.reload(true)
</script> 
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
%>

<html>

<head>
<!--#INCLUDE FILE = "../../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>#DB_NAME Results - Form View</title>
<!--#INCLUDE FILE="../../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../../source/app_js.js"-->
<!--#INCLUDE FILE="../../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->

</head>
<body <%=body_default%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table  width="650" >
	<tr>
	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
	<%GetItems formgroup, dbkey,  "DETAIL", "FRAMES_CHECK_TABLES"%>

		<%Response.Write Session("LEFT" & "DETAIL" & dbkey & formgroup)%>
	</tr>
</table>	
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>

