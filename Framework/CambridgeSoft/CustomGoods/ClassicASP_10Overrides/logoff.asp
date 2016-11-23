



<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%  
session.abandon    

If Request("ClearCookies") then
	session.Abandon
	Response.Cookies("CS_SEC_UserName")= ""
	Response.Cookies("CS_SEC_UserName").Path= "/"
	Response.Cookies("CS_SEC_UserID")= ""
	Response.Cookies("CS_SEC_UserID").Path= "/"
	' Log off from all cs_security applications
	Call KillCOWSSessions()
End if


%>

<script LANGUAGE="javascript">

if (window.name != null){
self.opener = this;
self.close()
}else{
window.location = 'coemanager/forms/public/contentarea/home2.aspx';
}
</script>