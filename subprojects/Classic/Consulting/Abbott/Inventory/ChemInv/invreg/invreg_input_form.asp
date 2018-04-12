<%@ LANGUAGE=VBScript 
 %>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title><%=Application("appTitle")%> -- Registy Search/Refine Form</title>
</head>

<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<table>
  <tr>
    <td><font face="Arial" size="9" color="#8000">ChemInventory Registy </font></td>
  </tr>
  <tr>
    <td><font face="Arial" size="3.5" color="#000000"><%ShowStrucInputField  dbkey, formgroup,"Reg_Numbers.Structure","4","228","144", "AllOptions", "SelectList"%></font></td>
  </tr>
  <tr>
    <td><table border="0" width="564">
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Name</font></td>
        <td width="465"><%ShowInputField dbkey, formgroup,"MolTable.NAME",0,"40"%>
</td>
      </tr>
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Formula</font></td>
        <td width="465"><%ShowInputField dbkey, formgroup,"Reg_Numbers.Formula",4,"40"%>
</td>
      </tr>
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Molecular Weight</font></td>
        <td width="465"><%ShowInputField dbkey, formgroup,"Reg_Numbers.MolWeight",0,"40"%>
</td>
      </tr>
    </table>
    </td>
  </tr>
</table>
<!--#INPUT_FIELDS-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
