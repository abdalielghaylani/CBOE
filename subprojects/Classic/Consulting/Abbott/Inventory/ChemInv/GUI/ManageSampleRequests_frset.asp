<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%
if not Session("IsFirstRequest") then 
	StoreASPSessionID()
else
	Session("IsFirstRequest") = false
	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
end if

QS = Request.QueryString
'ft = Request("ft")
'if ft = "" then ft = "std"
if Request("filter") = "true" then
	dispSrc = "ManageSampleRequests_display.asp?" & QS
Else
	dispSrc = "blank.html"
End if	
	
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Sample Requests</title>
<SCRIPT LANGUAGE=javascript>
<!--
window.focus()
//-->
</SCRIPT>

</head>

<frameset rows="245,*">
		<frame name="ActionFrame" src="ManageSampleRequests_filter.asp?<%=QS%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no">
		<frame name="DisplayFrame" src="<%=dispSrc%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
</frameset>

</html>
