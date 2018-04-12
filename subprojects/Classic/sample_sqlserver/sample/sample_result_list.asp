<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->


<title>sample Results-List View</title>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</head>

<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table border="1" cellpadding="0" cellspacing="0">
<%if Not Session("fEmptyRecordset" & dbkey) = True  then%>
  <tr>
    <td>&nbsp;</td>
    <td><strong><small>Structure</small></strong></td>
    <td><strong><small>Molecule Name</small></strong></td>
    <td><strong><small>MW</small></strong></td>
    <td><strong><small>Formula</small></strong></td>
  </tr>
<%end if%>
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated%>
<%Set DataConn=GetConnection(dbkey, formgroup, "MolTable")
sql =GetDisplaySQL(dbkey, formgroup,"MolTable.*","MolTable", "", BaseID, "")
Set BaseRS = DataConn.Execute(sql)
if Not (BaseRS.BOF and BaseRS.EOF) then 'added for applications that have delete capability 


%>
  <tr>
   <nobr> <td><script language="javascript">
getRecordNumber(<%=BaseRunningIndex%>)
document.write ('<br>')
getMarkBtn(<%=BaseID%>)
document.write ('<br>')
getFormViewBtn("show_details_btn.gif","sample_form.asp","<%=BaseActualIndex%>")
</script></td></nobr>
    <td><font face="MS Sans Serif" size="1" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "Structure","MolTable.Structure", BaseRS("ID"), "cdx","212","156"%></font></td>
    <td><font face="MS Sans Serif" size="1" color="#000000"><%ShowResult dbkey, formgroup, BaseRS,"MolTable.Molname","raw", 0, 0%></font></td>
    <td><font face="MS Sans Serif" size="1" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "MolWeight","MolTable.MolWeight", BaseRS("ID"), "raw", 1,"17"%></font></td>
    <td><font face="MS Sans Serif" size="1" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "Formula","MolTable.Formula", BaseRS("ID"), "raw", 1,"45"%></font></td>
  </tr>
    <%CloseRS(BaseRS)
CloseConn(DataConn)
else 'if BaseRS.BOF and BaseRS.EOF) = true then the record was deleted. 
%>

<tr>
   <nobr> <td><script language="javascript">
getRecordNumber(<%=BaseRunningIndex%>)
document.write ('<br>')
</script></td></nobr>
    <td>record deleted</td> 
     <td>&nbsp;</td>
      <td>&nbsp;</td>
       <td>&nbsp;</td> </tr><%
end if 'if NOT (BaseRS.BOF and BaseRS.EOF). added for applications that have delete capability %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</table>
</body>
</html>
