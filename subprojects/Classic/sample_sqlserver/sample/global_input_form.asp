<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title>Global Search/Refine Form</title>
</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<table border="0">
  <tr>
    <td><table border="0">
      <tr>
        <td nowrap valign="top" align="left">&nbsp;<%ShowStrucInputField  dbkey, formgroup, "MolTable.Structure", "1", 300, 200, "AllOptions", "SelectList"%> <p>&nbsp;</td>
        <td valign="top" align="left" nowrap><table border="0">
          <tr>
            <td bgcolor="#C0C0C0"><font color="#FFFFFF"><strong>Formula</strong></font></td>
          </tr>
          <tr>
            <td><%ShowInputField dbkey, formgroup, "MolTable.Formula", "0","30"%>
</td>
          </tr>
          <tr>
            <td bgcolor="#C0C0C0"><font color="#FFFFFF"><strong>Molecular Weight</strong></font></td>
          </tr>
          <tr>
            <td><%ShowInputField dbkey, formgroup, "MolTable.MolWeight", "0","30"%>
</td>
          </tr>
        </table>
        </td>
      </tr>
    </table>
    </td>
  </tr>
</table>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
