<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<input TYPE="hidden" NAME="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetAuthorizedLocation.asp"))%>>
