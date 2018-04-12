<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
'stop
dbkey="reg"

if (request("refreshHitlistCount") <> "" ) then
    
    Session("MarkedHits" & dbkey & "batches_form_group" & Session("UserName" & dbkey) & "start_check") = ""
    response.Redirect("/chem_reg/default.asp?formgroup=batches_form_group&dbname=reg")
end if



SendTo = "/jrm/CreateRequest.aspx"
'"http://DevSite2/cheminv/gui/ImportFromChemReg.asp"
'Response.Write sendTo
'Response.End
ProlongCookie "CS_SEC_UserName", Session("UserName" & dbkey), Application("CookieExpiresMinutes")
ProlongCookie "CS_SEC_UserID", CryptVBS(Session("UserID" & dbkey),Session("UserName" & dbkey)), Application("CookieExpiresMinutes")
if Request.Form("RegBatchNumberList")<> "" then
    RegBatchNumberList = Request.Form("RegBatchNumberList")
    BatchIDList = ""
else
    BatchIDList = Session("MarkedHitsregbatches_form_group")
    RegBatchNumberList = "" 
end if


if Request.Form("itemTypeID")<> "" then
    itemTypeID = Request.Form("itemTypeID")
else
    itemTypeID = "1" 
end if



%>
<HTML>
<HEAD>
</HEAD>
<BODY onload="form1.submit()">
<% 'response.write response.Cookies %>
	<Form Name="form1" method="POST" action="<%=SendTo%>">
		<input type="Hidden" name="BatchIDList" value="<%=BatchIDList%>">
		<input type="Hidden" name="RegBatchNumberList" value="<%=RegBatchNumberList%>">
		<input type="Hidden" name="CSUserName" value="<%=Session("UserName" & "reg")%>">
		<input type="Hidden" name="CSUserID" value="<%=Session("UserID" & "reg")%>">
		<input type="Hidden" name="itemTypeID" value="<%=itemTypeID%>">
	</Form>
</BODY>
</HTML>
