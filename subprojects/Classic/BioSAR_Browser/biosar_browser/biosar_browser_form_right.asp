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
		if instr( request.ServerVariables("http_referer"), "_form_right.asp")>0 then
				if isobject(session("child_display_dom")) then
				
					order_by = ""
					order_by1 =request("lastsortFields")
					order_by2 =request("sortFields")
					if order_by2 <> "" then
						order_by=order_by2
					else
						order_by=order_by1
					end if
				
					output = RS2HTML(Session("DETAIL_RS"),session("child_display_dom"),NULL,NULL,theIndex,theIndex,order_by,"","")
					output = replace(output, "&amp;fromList=true","")
					output = replace(output, "&fromList=true","")
					output = replace(output, "_form_left.asp?", "_form_right.asp?")
					Session("RIGHT" & "DETAIL" &  dbkey & formgroup)=output
					Response.Write Session("RIGHT" & "DETAIL" &  dbkey & formgroup)
					
				else
				Response.Write ""
				end if
		else
			Response.Write Session("RIGHT" & "DETAIL" &  dbkey & formgroup)
			session("lastinnersort") = ""
		end if
		
		%>
	</tr>
</table>	
</form>

</body>
</html>
