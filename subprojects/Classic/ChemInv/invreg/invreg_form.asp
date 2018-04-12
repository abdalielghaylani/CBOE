<%@ LANGUAGE=VBScript  %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%response.expires = 0%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<script language="javascript">
</script>

<title>BUCKBALL Results - Form View</title>
</head>

<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated%>
<%Set DataConn=GetConnection(dbkey, formgroup, "MolTable")
sql =GetDisplaySQL(dbkey, formgroup,"MolTable.*","MolTable", "", BaseID, "")
Set BaseRS = DataConn.Execute(sql)
%>

<table>
  <tr>
    <td><font face="Arial" size="9" color="#8000">BuckyBase</font></td>
  </tr>
  <tr>
    <td><font face="Arial" size="2" color="#8000">© 1996 - 2002 CambridgeSoft Corporation</font></td>
  </tr>
  <tr>
    <td><font face="Arial" size="2" color="#8000">Fullerenes</font></td>
  </tr>
  <tr>
    <td><font face="Arial" size="3.5" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "Structure","MolTable.Structure", BaseID, "cdx","228","144"%></font></td>
  </tr>
  <tr>
    <td><table border="0" width="564">
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Name</font></td>
        <td width="465"><font face="Arial" size="2" color="#000000"><%ShowResult dbkey, formgroup, BaseRS,"MolTable.NAME","raw", 0, 0%></font></td>
      </tr>
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Formula</font></td>
        <td width="465"><font face="Arial" size="2" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "Formula","MolTable.Formula", BaseID, "raw", 1,"38"%></font></td>
      </tr>
      <tr>
        <td width="87"><font face="Arial" size="2" color="#000000">Molecular Weight</font></td>
        <td width="465"><font face="Arial" size="2" color="#000000"><%ShowCFWChemResult dbkey, formgroup, "MolWeight","MolTable.MolWeight", BaseID, "raw", 1,"18"%></font></td>
      </tr>
    </table>
    </td>
  </tr>
<%CloseRS(BaseRS)
 CloseConn(DataConn)%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp"-->
</table>
</body>
</html>
