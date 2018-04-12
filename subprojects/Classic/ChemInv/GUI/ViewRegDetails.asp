<%@ Language=VBScript %>
<%
reg_number = Trim(Request("reg_number"))
URL = Application("SERVER_TYPE") & Application("RegServerName") & "/COERegistration/Forms/ViewMixture/ContentArea/ViewMixture.aspx?RegisteredObjectId=" & reg_number

Response.Redirect(URL)
Response.End

%>
