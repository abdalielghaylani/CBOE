<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<script language="javascript">
</script>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<title>sample Results - Form View</title>
</head>
<body <%=Application("BODY_BACKGROUND")%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%'BaseID represents the primary key for the recordset from the base array for the current row
'BaseActualIndex is the actual id for the index the record is at in the array
'BaseRunningIndex if the id based on the number shown in list view
'BastTotalRecords is the recordcount for the array
'BaseRS (below is the recordset that is pulled for each record generated%>
<%
table_delete_order="Synonyms_r,Moltable" ' order in which to delete records from tables if requested

Set DataConn=GetConnection(dbkey, formgroup, "MolTable")
sql =GetDisplaySQL(dbkey, formgroup,"MolTable.*","MolTable", "", BaseID, "")
Set BaseRS = DataConn.Execute(sql)
if Not (BaseRS.BOF and BaseRS.EOF) then 'added for applications that have delete capability 
	sql =GetDisplaySQL(dbkey, formgroup, "Synonyms_r.*", "Synonyms_r", "", BaseRS("MOL_ID"), "")
	Set SynonymsRS = DataConn.Execute(sql)
	%>


	<input type="hidden" name="table_delete_order" value ="<%=table_delete_order%>">

	<table border="0" width="500">
	  <tr>
	   <nobr> <td valign="top" width="110" rowspan="6"><font face="MS Sans Serif" size="1"
	    color="#000000"></font><script language="JavaScript"><!--

	getRecordNumber(<%=BaseRunningIndex%>)
	document.write ('<br>')
	getMarkBtn(<%=BaseID%>)
	// --></script><font face="MS Sans Serif"
	    size="1" color="#000000"></font></td></nobr>
	    <td valign="top" width="513" colspan="2"><font face="MS Sans Serif" size="1"
	    color="#000000"><%ShowCFWChemResult dbkey, formgroup, "Structure","MolTable.Structure", BaseRS("MOL_ID"), "cdx","212","156"%></font></td>
	  </tr>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Molecule
	    Name</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowResult dbkey, formgroup, BaseRS,"MolTable.Molname","raw", 0, "30"%></font></td>
	  </tr>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Synonyms</font></strong></td>
	    <td valign="top" width="307">
	    <%Do While Not SynonymsRS.EOF = True
	   
			ShowResult dbkey, formgroup, SynonymsRS,"Synonyms_r.Synonym_r","raw", 0, "40"
			Response.Write "<br>"
			SynonymsRS.MoveNext
	    loop%>
	</td>
	  </tr>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Molecular
	    Weight</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowCFWChemResult dbkey, formgroup, "MolWeight","MolTable.MolWeight", BaseRS("MOL_ID"), "raw_no_edit", 1,"17"%></font></td>
	  </tr>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Formula</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowCFWChemResult dbkey, formgroup, "Formula","MolTable.Formula", BaseRS("MOL_ID"), "raw_no_edit", 1,"45"%></font></td>
	  </tr>

	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Entry Date</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowResult dbkey, formgroup, BaseRS, "MolTable.Entry_Date",  "raw",  "DATE_PICKER:8","10"%></font></td>
	  </tr>

	</table>

	<p>&nbsp;</p>
	  <%CloseRS(BaseRS)
	CloseConn(SynonymsRS)

else 'if BaseRS.BOF and BaseRS.EOF) = true then the record was deleted.

	Response.Write "record deleted"

end if 'if NOT (BaseRS.BOF and BaseRS.EOF). added for applications that have delete capability %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>

</html>
