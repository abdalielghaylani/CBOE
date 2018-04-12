<style type="text/css">{  }
body, td { font-family: Verdana, arial, helvetica, sans-serif; font-size: x-small; }
tt, pre { font-family: monospace; }
sup, sub { font-size: 60%; }
</style>
<%@ LANGUAGE=VBScript %>
<%
' Determines whether the user is logged into ChemACX Pro or ChemACX Net
Session("IsNet") = Not Session("okOnService_10")
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<!--COWS1-->
<head>
<title>CambridgeSoft's ChemACX.com - Search, find and buy chemicals online.</title>
</head>


<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif" leftmargin="9" topmargin="9">

<!--#INCLUDE FILE = "banner.asp"-->

<TABLE valign="top" width="600" cellpadding="2">
<tr><td></td>
<td>
<h2>Suppliers listed in ChemACX</h2>
<p>Suppliers marked with an asterisk (*) can be searched in ChemACX Net; others are available in ChemACX Pro only</p>
<p>&nbsp;</p>
</td>
</tr>
<tr>
<td valign=top width="5">
<table cellpadding="0" cellspacing="1">

<!--#INCLUDE FILE = "leftnavigationlist.asp"-->

</table>

</td>
<td valign=top>

<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%

'get connection string from application variable
connection_array = Application( "base_connection" & "chemacx")
ConnStr = connection_array(0) & "="  & connection_array(1)
'response.write ConnStr
Session("suppliersTosearch") = Replace(request.cookies("acxprefsuplist"), ":",",")

' variables from configuration ini file
tabWhereField = Application("tabWhereField")
tab1WhereValue = Application("tab1WhereValue")
tab3WhereValue = Application("tab3WhereValue")	




' Overwrite variables from configuration ini file
'tabWhereField = "SupplierType"
'tab1WhereValue = "1"
'tab3WhereValue = "2,3"	
	
SQLQuery = "SELECT Name, SupplierType FROM Supplier WHERE [Supplier].[" & tabWhereField & "] IN (" & tab1WhereValue & "," & tab3WhereValue &  ") ORDER BY Name"

Dim conn
Dim RS

GetACXConnection()
Set RS = Conn.execute(SQLQuery)
RS.MoveFirst

if not rs.eof then
	firstChar = UCase(left(rs.fields("Name"), 1))
	response.Write "<p><b>" & firstChar & "</b></p>"
	response.Write "<p>"
	do while not rs.eof
		if firstChar <> UCase(left(rs.fields("Name"), 1)) then
			response.Write "</p>"
			firstChar = UCase(left(rs.fields("Name"), 1))
			response.Write "<p><b>" & firstChar & "</b></p>"
			response.Write "<p>"
		end if
		if rs.fields("SupplierType") = 1 then
			response.Write("* ")
		end if
		response.Write rs.fields("Name")
		response.write "<br>"
		rs.movenext
	loop
	response.Write "</p>"
end if
%>


</td>
</tr>
</table>
</body>
</html>



