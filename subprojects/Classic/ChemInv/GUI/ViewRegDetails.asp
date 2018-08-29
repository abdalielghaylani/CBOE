<%@ Language=VBScript %>
<%
reg_number = Trim(Request("reg_number"))
UserAgent = Request.ServerVariables("HTTP_USER_AGENT")
If InStr(UCASE(UserAgent), "CHROME")>0 OR InStr(UCASE(UserAgent), "FIREFOX")>0 then 
    URL = Application("SERVER_TYPE") & Application("NewRegServerUrl") & "/records/" & reg_number
else
    URL = Application("SERVER_TYPE") & Application("RegServerName") & "/COERegistration/Forms/ViewMixture/ContentArea/ViewMixture.aspx?RegisteredObjectId=" & reg_number
end if
Response.Redirect(URL)
Response.End

%>
