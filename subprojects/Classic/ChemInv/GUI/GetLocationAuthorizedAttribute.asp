<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<input TYPE="hidden" id="tempCsUserName" name="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" id="tempCsUserID" name="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetAuthorizedLocation.asp"))%>>
