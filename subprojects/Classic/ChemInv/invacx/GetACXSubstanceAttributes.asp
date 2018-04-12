<%
' Get the database type for this connection
DBMS = Application("base_connection" & dbkey)(kDBMS)
'Response.Write "DBMS=" & DBMS & "<BR>"
if Ucase(DBMS) = "ORACLE" then
	synTableName = "acx_synonym"
	sql1 = "SELECT substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.synonymid = " & SynTableName & ".synonymid" & " AND substance.CSNum =? AND ROWNUM <2"
    sql2 = "SELECT substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.csnum = " & SynTableName & ".csnum" & " AND substance.CSNum =? AND ROWNUM <2"
else
	SynTableName = "synonym"
    sql1 = "SELECT TOP 1 substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.synonymid = " & SynTableName & ".synonymid" & " AND substance.CSNum =?"
    sql2 = "SELECT TOP 1 substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.csnum = " & SynTableName & ".csnum" & " AND substance.CSNum =?"
end if

Set DataConn=GetConnection(dbkey, formgroup, "substance")

'try to get attributes by synonymid first
Dim synCmd
Set synCmd = Server.CreateObject("ADODB.Command")
synCmd.activeConnection = DataConn
synCmd.Parameters.Append synCmd.CreateParameter("id",131, 1, 0, BaseID)
synCmd.CommandType = 1
synCmd.CommandText = sql1
Set BaseRS = synCmd.Execute

If BaseRS.EOF then
	'then try by csnum
	'sql = "SELECT TOP 1 substance.CSNum, substance.CAS, substance.ACX_ID, " & SynTableName & ".Name FROM substance, " & SynTableName & " WHERE substance.csnum = " & SynTableName & ".csnum" & " AND substance.CSNum =" & BaseID
	synCmd.CommandText = sql2
	Set BaseRS = synCmd.Execute
end if

CAS = BaseRS("CAS").value
ACX_ID = BaseRS("ACX_ID").value
SubstanceName = BaseRS("Name").value
%>
