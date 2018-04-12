<%@ LANGUAGE=VBScript  %>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<%response.expires = 0%>
<%' Copyright 2001-2002, CambridgeSoft Corp., All Rights Reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<title><%=Application("appTitle")%> Registration Substance -- Form View</title>

</head>
<body>
<%
Dim RS, Conn
dbkey = "ChemInv"

RegID = Request("RegID")
BatchNumber = Request("BatchNumber")

GetInvConnection()
sql = "SELECT * FROM inv_vw_reg_batches WHERE inv_vw_reg_batches.regid = ? AND inv_vw_reg_batches.batchnumber = ?"
Set Cmd = server.createobject("ADODB.Command")
	Cmd.CommandText = sql
	Cmd.ActiveConnection = Conn
	Cmd.Parameters.Append Cmd.CreateParameter("RegID", 131, 1, 0, RegID)
	Cmd.Parameters.Append Cmd.CreateParameter("BatchNumber", 131, 1, 0, BatchNumber)
Set RS = Cmd.Execute
%>

<div align="center">
<form name="form1">
<table border="0">
<%
if Not(RS.BOF and RS.EOF) then
	Response.Write("<tr><td colspan=""2"" align=""center""><em><b>")
	if RS("RegName") <> "" then
		Response.Write(RS("RegName"))
	else
		Response.Write("No Substance Name")
	end if
	Response.Write("</b></em></td></tr>")
	Response.Write("<tr><td colspan=""2"" align=""center"">&nbsp;</td></tr>")	
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" and key <> "REGNAME" then
			Response.write("<tr><td align=""right"">" & reg_fields_dict.item(key) & ":</td><td class=""grayBackground"">" & RS(key) & "</td></tr>")
		end if
	next
end if
RS.Close()
Set RS = Nothing
%>
<tr><td>&nbsp;</td><td align="right"><a HREF="#" onclick="top.close(); return false;"><img SRC="../graphics/close.gif" border="0" alt="Close" WIDTH="61" HEIGHT="21"></a></td></tr>
</table>
</form>
</div>

</body>
</html>
