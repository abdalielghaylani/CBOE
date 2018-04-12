<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'SYAN modified on 3/31/2004 to support parameterized SQL
Dim Conn
	'GetACXConnection()
	If Application("DBMS" & dbkey)= "ORACLE" then
		'SQLQuery = "SELECT DISTINCT ACX_Synonym.Name FROM ACX_Synonym WHERE ACX_Synonym.CsNum =" & Request.QueryString("CsNum")
		SQLQuery = "SELECT DISTINCT ACX_Synonym.Name FROM ACX_Synonym WHERE ACX_Synonym.CsNum=?"
	Else
		'SQLQuery = "SELECT DISTINCT Synonym.Name FROM Synonym WHERE Synonym.CsNum =" & Request.QueryString("CsNum")
		SQLQuery = "SELECT DISTINCT Synonym.Name FROM Synonym WHERE Synonym.CsNum=?"
	End if
	SQLQuery_Parameters = "CsNum" & "|" & adInteger & "|" & adParamInput & "|" & "|" & Request.QueryString("CsNum")
	'set SynRS = Conn.Execute(SQLQuery)
	Set SynRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
	'set Conn= nothing
%>	