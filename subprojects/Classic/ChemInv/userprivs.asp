<%@ Language=VBScript %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
userprivileges = Request.QueryString("privsuser")
%>

<script language = "javascript">
var check = <%=userprivileges%>
if(check == 1)
{
alert("Insufficient privileges, please talk to your system administrator");
document.location.href = "/cheminv/login.asp";
}
</script>


