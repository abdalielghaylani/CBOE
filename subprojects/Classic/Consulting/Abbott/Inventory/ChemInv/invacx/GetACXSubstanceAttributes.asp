<%
' Get the database type for this connection
DBMS = Application("base_connection" & dbkey)(kDBMS)
'Response.Write "DBMS=" & DBMS & "<BR>"
if Ucase(DBMS) = "ORACLE" then
	synTableName = "acx_synonym"
else
	SynTableName = "synonym"
end if

Set DataConn=GetConnection(dbkey, formgroup, "substance")

'try to get attributes by synonymid first
sql = "SELECT TOP 1 substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.synonymid = " & SynTableName & ".synonymid" & " AND substance.CSNum =" & BaseID
'Response.Write sql
'Response.end
Set BaseRS = DataConn.Execute(sql)

If BaseRS.EOF then
	'then try by csnum
	sql = "SELECT TOP 1 substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.csnum = " & SynTableName & ".csnum" & " AND substance.CSNum =" & BaseID
	Set BaseRS = DataConn.Execute(sql)
end if

CAS = BaseRS("CAS").value
ACX_ID = BaseRS("ACX_ID").value
SubstanceName = BaseRS("Name").value
%>
