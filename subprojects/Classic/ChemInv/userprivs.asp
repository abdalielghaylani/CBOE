<%@ Language=VBScript %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
userprivileges = Request.QueryString("privsuser")
userSession = Request.QueryString("usersession")
%>

<script language = "javascript">
var check = <%=userprivileges%>
var userSessionValue = <%=userSession%>
if(check == 1 && userSessionValue == 1)
{
alert("Insufficient privileges, please talk to your system administrator");
document.location.href = "/cheminv/login.asp";
}
else if(check == 1 && userSessionValue == 0)
{
alert("Application has timed out");
document.location.href = "/cheminv/logoff.asp";
}
</script>


