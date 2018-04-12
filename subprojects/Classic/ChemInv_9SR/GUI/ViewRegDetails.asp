<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<%
reg_number = Trim(Request("reg_number"))
URL = "http://" & Application("RegServerName") & "/chem_reg/reg/reg_post_action.asp"
user_id = Session("UserName" & "cheminv")
user_pwd = Session("UserID" & "cheminv")

'Response.Write(reg_number)
'Response.Write(URL)
'Response.End

%>
<html>
<head>
</head>
<body onLoad='document.forms[0].submit()'>
<form method='POST' action='<%=URL%>'>
<input type=hidden name=user_id value='<%=user_id%>'>
<input type=hidden name=user_pwd value='<%=user_pwd%>'>
<input type=hidden name=reg_method value='get_reg'>
<input type=hidden name=reg_number value='<%=reg_number%>'>
</form>
</body>
</html>

