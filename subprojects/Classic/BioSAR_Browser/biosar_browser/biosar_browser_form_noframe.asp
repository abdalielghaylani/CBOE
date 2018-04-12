<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = request("dbname")
formgroup = request("formgroup")
formmode = request("formmode")
Response.ExpiresAbsolute = Now()
if Request("output_type") <> "" then
	Session("CurrentMode" & dbkey & formgroup)= Request("output_type")
end if%>
<script language="JavaScript">

	if  (top.main.location.href.indexOf("biosar_browser_form_noframe.asp") == -1){
		top.main.location.href = "biosar_browser_form_noframe.asp?<%=request.QueryString%>";
	}
	//MainWindow = <%=Application("MainWindow")%>
	//alert(MainWindow.Name)
</script> 


<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
%>
<script language="javascript">
</script>

<html>

<head>
<!--#INCLUDE FILE = "../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>#DB_NAME Results - Form View</title>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<!--#INCLUDE FILE="../reports/reports_app_js.js"-->

</head>
<body <%=body_default%>>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table  width="650" >
	<tr><td>
	<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
		<%	getJumpForms dbkey, formgroup, Application("BASE_TABLEbiosar_browser" & formgroup), "biosar_browser_form_noframe.asp"%>

		<%
		'DGB added a workround for widget layout limitation.
		'We reuse the html generated in the FRAMES case so that
		'we can call the base and child saparately.
		theCurrentMode = UCase(Session("CurrentMode" & dbkey & formgroup)) 
		if theCurrentMode = "REPORT" then
			GetItems formgroup, dbkey,  "DETAIL", "FRAMES"
			Response.Write Session("LEFT" & "DETAIL" & dbkey & formgroup)
			Response.Write "<BR>"
			GetItems formgroup, dbkey,  "DETAIL", "FRAMES"
			Response.Write Session("RIGHT" & "DETAIL" & dbkey & formgroup)
		else
			
			GetItems formgroup, dbkey,  "DETAIL", theCurrentMode
			Response.Write Session("LEFT" & "DETAIL" & dbkey & formgroup)
		
		end if
		%>
	</td></tr>
</table>	
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>

