<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = request("dbname")
formgroup = request("formgroup")
formmode = request("formmode")
Response.ExpiresAbsolute = Now()
if UCase(request.QueryString("output_type")) = "FRAMETAB" then
	Session("CurrentMode" & dbkey & formgroup)= "FRAMETAB"
elseif 	Session("CurrentMode" & dbkey & formgroup)= "FRAMETAB" and UCase(request.QueryString("output_type")) = "" then
	Session("CurrentMode" & dbkey & formgroup)= "FRAMETAB"
else
	Session("CurrentMode" & dbkey & formgroup)= "FRAMES"
end if
%>
<script language="JavaScript">
	
	if  (top.main.location.href.indexOf("biosar_browser_form.asp") == -1){
		top.main.location.href = "/<%=Application("Appkey")%>/<%=dbkey%>/biosar_browser_form.asp?<%=request.QueryString%>";
	}
	MainWindow = top.main
	theMainFrame = top.main.mainFrame
	
	
	try{
		top.main.rightFrame.location.reload(true)
	}catch(e){
	
	}
</script> 
</script> 
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
%>

<html>

<head>

<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE = "../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../reports/reports_app_js.js"-->

<title>#DB_NAME Results - Form View</title>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->

</head>
<body <%=body_default%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table  width="650" >
	<tr>
	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
		<%	getJumpForms dbkey, formgroup, Application("BASE_TABLEbiosar_browser" & formgroup), "biosar_browser_form_left.asp"%>
	<%	
		Session("RIGHT" & display_view & dbkey  & formgroup) = ""
		GetItems formgroup, dbkey,  "DETAIL", UCase(Session("CurrentMode" & dbkey & formgroup))%>
		
		<%Response.Write Session("LEFT" & "DETAIL" & dbkey & formgroup)%>
	</tr>
	
</table>	
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>
</html>
