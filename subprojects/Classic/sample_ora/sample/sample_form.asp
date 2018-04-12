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
	
	'DGB:  Moved the code that read the MAINTABLE into the Case Statement below
	
	
	'DGB: Code to manage the URL of the hyperlinks used for
	'switching between tables
	
	' Get the entire querystring for this page
	QS = Request.QueryString
	
	' Check which tabl to display
	TabName = Request.QueryString("TabName")
	if TabName <> "" then
		' Remove the TabName section of the querystring
		' because it is added later on to the PageURL
		QS = Replace(QS, "&TabName=" & TabName,"")
	else
		'Set the default tab
		TabName = "MAINTABLE"
	end if
	
	'Build the PageURL
	PageURL = Request.ServerVariables("SCRIPT_NAME") & "?" & QS
	
%>


<!---DGB: Added the hyperlinks to switch between tables to display --->
<BR>
<table>
 <tr>
	<td><a href="<%=PageURL & "&TabName=MAINTABLE"%>">&nbsp;Main Table</a></td>
	<td><a href="<%=PageURL & "&TabName=SUBTABLE"%>">&nbsp;Sub Table</a></td>
 </tr>
</table>
<BR><BR>
<%
'DGB:  Added Select Cases for displaying the MAINTABLE or SUBTABLE
Select Case TabName
	Case "MAINTABLE"
	
	'DGB:  This code was moved from above because we only need to execute it
	' when we are actually going to show the MAINTABLE data
	sql =GetDisplaySQL(dbkey, formgroup, "Synonyms_r.*", "Synonyms_r", "", BaseRS("ID"), "")
	Set SynonymsRS = DataConn.Execute(sql)
	sql ="select * from sample.graphics where mol_id = " & basers("mol_id")
	Set GraphicsRS = DataConn.Execute(sql)
%>	
		
	<input type="hidden" name="table_delete_order" value ="<%=table_delete_order%>">

	<table border="0" width="500">
	  <tr>
	   <nobr> <td valign="top" width="110" rowspan="5"><font face="MS Sans Serif" size="1"
	    color="#000000"></font><script language="JavaScript"><!--

	getRecordNumber(<%=BaseRunningIndex%>)
	document.write ('<br>')
	getMarkBtn(<%=BaseID%>)
	// --></script><font face="MS Sans Serif"
	    size="1" color="#000000"></font></td></nobr>
	    <td valign="top" width="513" colspan="2"><font face="MS Sans Serif" size="1"
	    color="#000000"><%ShowResult dbkey, formgroup, BaseRS,"MolTable.BASE64_CDX", "BASE64CDX", "212","156"%></font></td>
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
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowCFWChemResult dbkey, formgroup, "MolWeight","MolTable.MolWeight", BaseRS("ID"), "raw_no_edit", 1,"17"%></font></td>
	  </tr>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">Formula</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowCFWChemResult dbkey, formgroup, "Formula","MolTable.Formula", BaseRS("ID"), "raw_no_edit", 1,"45"%></font></td>
	  </tr>
	  </table>
	  <table>
	  <tr>
	    <td valign="top" width="206"><strong><font face="MS Sans Serif" color="#000000" size="2">H1NMR</font></strong></td>
	    <td valign="top" width="307"><font face="MS Sans Serif" color="#000000" size="2"><%ShowResult dbkey, formgroup, GraphicsRS,"Graphics.Graphic","PICTURE:image/gif",  "80", "80"%></font></td>
	  </tr>
	</table>

<%
'DGB Added case to display SUBTABLE
Case "SUBTABLE"
	' Read the subtable data into a recordset
	sql =GetDisplaySQL(dbkey, formgroup,"MySubtable.*","MySubtable", "", BaseRS("MOL_ID"), "")
	Set MySubtableRS = DataConn.Execute(sql)
%>		

	Here is the subtable data:<BR>
	<table border="1">
	<%Do While Not MySubtableRS.EOF = True%>
		<tr>
		  <td> 
			<%ShowResult dbkey, formgroup, MySubtableRS,"MySubtableRS.id","raw", 0, "40"%>
		  </td>
		  <td>
			<%ShowResult dbkey, formgroup, MySubtableRS,"MySubtableRS.mydate","raw", 0, "40"%>
		  </td>
		  <td>
			<%ShowResult dbkey, formgroup, MySubtableRS,"MySubtableRS.mytext","raw", 0, "40"%>
		  </td>
		</tr>  	
		<%MySubtableRS.MoveNext%>
	<%loop%>
	</table>
	
<%
End Select
'DGB: END of new SUBTABLE case and Select statement
%>



	<p>&nbsp;</p>
	  <%CloseRS(BaseRS)
	CloseConn(SynonymsRS)

else 'if BaseRS.BOF and BaseRS.EOF) = true then the record was deleted.

	Response.Write "record deleted"

end if 'if NOT (BaseRS.BOF and BaseRS.EOF). added for applications that have delete capability %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
</body>

</html>
