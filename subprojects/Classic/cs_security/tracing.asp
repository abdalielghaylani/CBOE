<%@ Language=VBScript %>
<!-- #include virtual="/cs_security/variables.asp" -->
<!-- #include virtual="/cs_security/functions.asp" -->

<%
PAGE_URL = "ChemOffice Enterprise -- Application Tracing"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = "<a href=""/cfserveradmin/AdminSource/webeditor.asp""><b>Administrative Tools</b></a>&nbsp;|&nbsp;<a href=""http://www.cambridgesoft.com/services/topics.cfm?FID=6"" target=""_blank""><b>Technical Notes</b></a>"
%>

<!-- #include virtual="/cs_security/header.asp" -->
<SCRIPT LANGUAGE=javascript>
	function doit(){
		var dbkey = form1.dbkey[form1.dbkey.selectedIndex].value
		
		form1.action = "/" + dbkey + "/user_info.asp"
		form1.submit(); 
	}
	
	window.onload = function(){window.focus()}
</SCRIPT>

<table border="0" width="100%" cellpadding="0" style="border-collapse: collapse"><tr><td align="right">
<%
CS_SEC_UserName = UCase(Request.Cookies("CS_SEC_UserName"))
If Len(CS_SEC_UserName) > 0 Then Response.write "Current login: " & CS_SEC_UserName & "&nbsp;&nbsp;"
%>	
</td></tr></table><br>
<form name="form1" method="GET">
	Select an application to trace:
	<select name="dbkey">
		<%
		for i = 1 to ubound(loaded_apps_array)
		 if loaded_apps_array(i) <> "INIEmpty" then
				Response.Write "<option value='" & loaded_apps_array(i) & "'>" & loaded_apps_array(i) & vbcrlf
			end if
		next
		%>
	</select>
	<input type="hidden" value="1" name="ManageTracing">
	<input type="button" value="Ok" onclick="doit()">
</form>
<center>
	<input type="button" value="Cancel" onClick="window.close()">
</center>
<!-- #include virtual="/cs_security/footer.asp" -->