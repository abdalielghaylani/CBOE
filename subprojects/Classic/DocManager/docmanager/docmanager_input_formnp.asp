s<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved
dbkey=Request("dbname")
if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if%>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>docmanager Search/Refine Form</title>
</head>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<!--#INCLUDE FILE = "../source/secure_nav.asp"-->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->


<BODY background="<%=Application("UserWindowBackground")%>">
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<html>
<table border = "1"><tr><td><table border = "0">
<tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>MOL_ID</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.MOL_ID",1,"32"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>MOL_ID</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.DOCID",1,"32"%></font></td><td nowrap valign = "top"><font face="Arial" size="2" color="#000000"><strong>Structure</strong></font></td><td valign = "top"><font face="Arial" size="2" color="#000000"><%ShowStrucInputField  dbkey, formgroup,"DOCMGR_STRUCTURES.STRUCTURE","4","470","302", "AllOptions", "SelectList"%></font></td>
</tr><tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>Formula</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.FORMULA",4,"32"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>MolWeight</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.MOLWEIGHT",0,"31.2"%></font></td>
</tr><tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCID</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DOCID",1,"32.8"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>TITLE</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.TITLE",0,"64"%></font></td>
</tr><tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCLOCATION</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DOCLOCATION",0,"64"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCNAME</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DOCNAME",0,"32"%></font></td>
</tr><tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>AUTHOR</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.AUTHOR",0,"32"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>SUBMITTER</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.SUBMITTER",0,"32"%></font></td>
</tr><tr>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>SUBMITTER_COMMENTS</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.SUBMITTER_COMMENTS",0,"64"%></font></td>
<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DATE_SUBMITTED</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DATE_SUBMITTED",8,"32"%></font></td>
</tr><tr>
</tr>
<!--#INPUT_FIELDS-->
</table></td></tr></table>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
