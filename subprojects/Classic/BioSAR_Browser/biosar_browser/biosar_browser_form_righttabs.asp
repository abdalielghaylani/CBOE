<%@ LANGUAGE=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
dbkey = request("dbname")
formgroup = request("formgroup")
Response.ExpiresAbsolute = Now()
'stop
if Request("output_type") <> "" then
	Session("CurrentMode" & dbkey & formgroup)= UCase(Request("output_type"))
end if
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/rs2html.asp"-->

<html>
<head>
	<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>

	<script language="JavaScript"> 
	var alert_cdax_version = "<%=APPLICATION("ALERT_CDAX_VERSION")%>"
	if (window.doPluginDetect != null) {
		cd_includeWrapperFile("/cfserverasp/source/", doPluginDetect)
	}
	else {
		cd_includeWrapperFile("/cfserverasp/source/")
	}
	</script>
<%
if Application("biosarxsloverride")="FALSE" then
	usecss = "biosar"
else
	if lcase(Application("hasCustomCSS" & formgroup)) = "true" then
		usecss = formgroup & "detail.css"
	else
		usecss = "customdetail.css"
	end if
end if
%>

<script language="javascript">
		var useStyleSheet = '<%=usecss%>';
</script>	
<script LANGUAGE="javascript" src="/biosar_browser/source/Choosedetailcss.js"></script>

<title>Right</title>


</head>
<body <%=body_default%>>
<form name=cows_input_form>
<table  width="400" >
	<tr>
			<%
			output =  Session("RIGHT" & "DETAIL" &  dbkey & formgroup)
			output = replace(output, "<A CLASS=""TabView""", "<A CLASS=""TabView"" target=""mainFrame""")
			'output = replace(output, "_form_left.asp?formgroup", "_form_righttabs.asp?formgroup")
			output= replace(output, "CLASS=""columnNameClass"" title=""""><a href", "CLASS=""columnNameClass"" title=""""><a target=""mainFrame"" href")
			Response.Write output
			session("lastinnersort") = ""

		
		%>
	</tr>
</table>	
</form>

</body>
</html>
