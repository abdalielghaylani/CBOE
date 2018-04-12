<%@ LANGUAGE=VBScript  %>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE FILE = "../../cs_security/admin_utils_vbs.asp"-->
<!--#INCLUDE FILE = "../../cs_security/admin_utils_js.js"-->
<body>

<form name="cows_input_form" method="post" action="/<%=Application("AppKey")%>/cs_security/manage_security/manage_security_action.asp?dbname=<%=request("dbname")%>&theCaller' + thecaller + '&formgroup=manage_roles_form_group&dataaction=add_existing_role&roleExisting=true" ID="Form1">
	<input type="text" name="role_name" value = "" width = "100" ID="Text1">
	<input type="hidden" name="theCaller" value = "' + thecaller + '" ID="Hidden1">
	<input type="button" name="Add Existing Role" value = "Add Existing Role" onClick="document.cows_input_form.submit()" ID="Button1">

</form>
</body>
</html>
