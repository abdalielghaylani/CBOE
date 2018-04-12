<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>sample Search/Refine Form</title>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>
<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table><tr><td>
<a href="/<%=Application("appkey")%>/inputtoggle.asp?dbname=<%=request("dbname")%>&formgroup=basenp_form_group">Click here for non-plugin search</a>
</td></tr></table>

<table border="0">
  <tr>
    <td valign="top" colspan="3"><table border="0">
      <tr>
        <td><%ShowStrucInputField  dbkey, formgroup,"MolTable.Structure","5","340","180", "AllOptions", "SelectList"%>
</td>
      </tr>
    </table>
    </td>
  </tr>
  <tr>
    <td rowspan="4" valign="top"></td>
    <td valign="top"><strong><font face="MS Sans Serif" color="#000000" size="2">Molecule Name</font></strong></td>
    <td valign="top"><%ShowInputField dbkey, formgroup,"MolTable.Molname",0,"30"%>
</td>
  </tr>
  <tr>
    <td valign="top"><strong><font face="MS Sans Serif" color="#000000" size="2">Synonym</font></strong></td>
    <td valign="top"><%ShowInputField dbkey, formgroup,"Synonyms_r.Synonym_r",0,"30"%>
</td>
  </tr>
  <tr>
    <td valign="top"><strong><font face="MS Sans Serif" color="#000000" size="2">Molecular
    Weight</font></strong></td>
    <td valign="top"><%ShowInputField dbkey, formgroup,"MolTable.MolWeight",0,"30"%>
</td>
  </tr>
  <tr>
    <td valign="top"><strong><font face="MS Sans Serif" color="#000000" size="2">Formula</font></strong></td>
    <td valign="top"><%ShowInputField dbkey, formgroup,"MolTable.Formula",4,"30"%>
</td>
  </tr>
</table>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
