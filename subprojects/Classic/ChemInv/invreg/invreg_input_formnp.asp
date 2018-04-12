<%@ LANGUAGE=VBScript 
 %>
 <!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

<title><%=Application("appTitle")%> -- Registy Search/Refine Form</title>
</head>

<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<tr><td><font face="Arial" size="2" color="#000000">Mol_ID<br></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000"><%ShowInputField dbkey, formgroup,"MolTable.MOL_ID",1,"16"%></font></td></tr>
<tr><td><font face="MS Sans Serif" size="2" color="#000000">Structure<br></font></td></tr>
<tr><td><font face="Arial" size="3.5" color="#000000"><%ShowStrucInputField  dbkey, formgroup,"MolTable.Structure","4","228","144", "AllOptions", "SelectList"%></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000">Formula<br></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000"><%ShowInputField dbkey, formgroup,"MolTable.Formula",4,"38"%></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000">Molecular Weight<br></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000"><%ShowInputField dbkey, formgroup,"MolTable.MolWeight",0,"18"%></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000">Name<br></font></td></tr>
<tr><td><font face="Arial" size="2" color="#000000"><%ShowInputField dbkey, formgroup,"MolTable.NAME",0,"18"%></font></td></tr>

<!--#INPUT_FIELDS-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
