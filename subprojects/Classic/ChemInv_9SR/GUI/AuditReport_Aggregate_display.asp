<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

'SYAN added on 1/20/2005 to fix CSBR-50830
stop
app = request("app")
if app = "" then
	app = "inv"
end if
'End of SYAN modification


Response.Expires = -1
bDebugPrint = false
bWriteError = False
strError = "Error:AuditReport_Agregate_display.asp<BR>"

username = "'" & request("userID") & "'"
tabname = "'" & request("tablename") & "'"
'SYAN added on 1/20/2005 to fix CSBR-50830
docmgr_tabname = "'" & request("docmgr_tablename") & "'"
'End of SYAN modification
act =   "'" & Ucase(request("action")) & "'" 
lo_rid = request("lo_rid")
hi_rid = request("hi_rid")
fromDate = request("fromDate")
toDate = request("toDate")

if request("grpByTableName") <> "" then grpByTableName = CBool(request("grpByTableName")) 'else Session("audGrpByTableName") = false end if
if request("grpByUserName") <> "" then grpByUserName = CBool(request("grpByUserName")) 'else Session("audGrpByUserName") = false end if
if request("grpByAction") <> "" then grpByAction = CBool(request("grpByAction")) 'else Session("audGrpByAction") = false end if


grpByDate = request("grpByDate")
oldSortBy = Session("audSortBy") 
sortBy = Request("sortBy")

'-- only use session variables if this is a sort
if not isEmpty(sortBy) then
	if username = "''"  and not isEmpty(Session("audUserName")) then username = Session("audUserName")
	if tabname = "''" and not isEmpty(Session("audTabName")) then tabname = Session("audTabName")
	'SYAN added on 1/20/2005 to fix CSBR-50830
	if docmgr_tabname = "''" and not isEmpty(Session("docmgr_audTabName")) then docmgr_tabname = Session("docmgr_audTabName")
	'End of SYAN modification
	if act = "''" and not isEmpty(Session("audAct")) then act = Session("audAct")
	if lo_rid = "" then lo_rid = Session("audLo_RID")
	if hi_rid = "" then hi_rid = Session("audHi_RID")
	if fromDate = "" then fromDate = Session("audFromDate")
	if toDate = "" then toDate = Session("audToDate")
	if grpByTableName = false and not isEmpty(Session("audGrpByTableName")) then grpByTableName = Session("audGrpByTableName")
	if grpByUserName = false and not isEmpty(Session("audGrpByUserName")) then grpByUserName = Session("audGrpByUserName")
	if grpByAction = false and not isEmpty(Session("audGrpByAction")) then grpByAction = Session("audGrpByAction")
	if grpByDate = "" then grpByDate = Session("audGrpByDate")
	if sortBy = "" and Session("audPage") = "aggregate" then sortBy = Session("audSortBy")
end if
 
'add session vars
Session("audUserName") = username
Session("audTabName") = tabname
'SYAN added on 1/20/2005 to fix CSBR-50830
Session("docmgr_audTabName") = docmgr_tabname
'End of SYAN modification
Session("audAct") = Act
Session("audLo_RID") = lo_rid
Session("audHi_RID") = hi_rid
Session("audFromDate") = FromDate
Session("audToDate") = ToDate
Session("audGrpByTableName") = grpByTableName
Session("audGrpByUserName") = grpByUserName
Session("audGrpByAction") = grpByAction
Session("audGrpByDate") = grpByDate
Session("audSortBy") = sortBy
Session("audPage") = "aggregate"

if isEmpty(Session("audSortDir")) or (oldSortBy <> sortBy) or (oldSort = sortBy and Session("audSortDir") = "ASC") then 
	sortDir = "DESC"
elseif Session("audSortDir") = "DESC" then
	sortDIR = "ASC"
end if
Session("audSortDir") = sortDir


if grpByDate = "" then grpByDate = "D"

if username = "" then username = "NULL"
if tabname = "" then tabname = "NULL"
'SYAN added on 1/20/2005 to fix CSBR-50830
if docmgr_tabname = "" then docmgr_tabname = "NULL"
'End of SYAN modification
if act = "" then act = "NULL"
if lo_rid = "" then lo_rid = "NULL"
if hi_rid = "" then hi_rid = "NULL"
if fromDate = "" then
	lowdate = "NULL"
Elseif IsDate(fromDate) then
	lowdate = GetOracleDateString2(fromDate)
Else
	strError = strError & "From Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
if toDate = "" then
	highdate = "NULL"
Elseif IsDate(toDate) then
	highdate = GetOracleDateString2(toDate)
Else
	strError = strError & "To Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if


Select case Ucase(grpByDate)
	Case "H"
		DateExpression = "to_char(timestamp, 'MON DD YYYY HHAM')"
		DateFieldName = " HOUR"
	Case "D"
		DateExpression = "to_char(timestamp, 'MON DD YYYY')"
		DateFieldName = " DAY"
	Case "M"
		DateExpression = "to_char(timestamp, 'MON YYYY')"
		DateFieldName = " MONTH"
	Case "Y"
		DateExpression = "to_char(timestamp, 'YYYY')"
		DateFieldName = " YEAR"
End Select

'SYAN added on 1/20/2005 to fix CSBR-50830
if app = "inv" then

	sql =	"SELECT Count(*) Num"
	if grpByTableName then sql = sql & ", lower(table_Name) ""TABLE_NAME"""
	if grpByAction then sql = sql & ", DECODE(action, 'I', 'INSERT', 'U', 'UPDATE', 'D', 'DELETE') Action"		
	if grpByUserName then sql = sql & ", lower(user_name) as Modified_by"
	sql = sql & ", " & DateExpression &  DateFieldName &_
			" FROM " & Application("CHEMINV_USERNAME") & ".INV_VW_AUDIT_AGGREGATE" &_
			" WHERE Upper(user_name) LIKE NVL(UPPER(" & username &"),'%')" &_
			" AND Upper(action) LIKE NVL(UPPER(" & act &"),'%')" &_
			" AND Upper(table_name) LIKE NVL(UPPER(" & tabname & "),'%') " &_
			" AND rid BETWEEN NVL(" & lo_rid & ",1) AND NVL(" & hi_rid & ",999999999999)" &_
			" AND timestamp BETWEEN NVL(" & lowdate & ",to_date('1980-01-01 0:0:0','YYYY-MM-DD HH24:MI:SS')) AND NVL(" & highdate & ",SYSDATE)" &_
			" GROUP BY " 
	if grpByTableName then sql = sql & " table_Name,"
	if grpByAction then sql = sql & " action,"		
	if grpByUserName then sql = sql & " user_name,"
			
	sql = sql & DateExpression

	if sortBy = "" then 
		'sql = sql & " ORDER BY  v.user_name, v.table_name, v.timestamp DESC"	
	else
		'sortBy = replace(sortBy, "MODIFIED_BY", "USER_NAME")
		sql = sql & " ORDER BY  " & sortBy & " " & sortDir
	end if

	if bDebugPrint then 
		Response.Write "User: " & username & "<BR>"
		Response.Write "Table: " & tabName & "<BR>"
		Response.Write "Action: " & act & "<BR>"
		Response.Write "From Date: " & fromDate & "<BR>"
		Response.Write "To Date: " & toDate & "<BR>" 
		Response.Write "sortby: " & sortby & "<BR>"
		Response.Write "grpByTableName: " & grpByTableName & "<BR>"
		Response.Write "grpByUserName: " & grpByUserName & "<BR>"
		Response.Write "grpByAction: " & grpByAction & "<BR>"
		Response.Write "grpByDate: " & grpByDate & "<BR>"
		Response.Write sql
		Response.End
	end if
	Response.Expires = -1
	GetInvConnection()
elseif app = "docmanager" then
	ConnStr = "File Name=" & Server.MapPath("/docmanager/config/docmanager.udl") & ";User ID=docmgr;Password=oracle"
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr 

	sql =	"SELECT Count(*) Num"
	if grpByTableName then sql = sql & ", lower(table_Name) ""TABLE_NAME"""
	if grpByAction then sql = sql & ", DECODE(action, 'I', 'INSERT', 'U', 'UPDATE', 'D', 'DELETE') Action"		
	if grpByUserName then sql = sql & ", lower(user_name) as Modified_by"
	sql = sql & ", " & DateExpression &  DateFieldName &_
			" FROM " & "docmgr.docmgr_vw_audit_aggregate" &_
			" WHERE Upper(user_name) LIKE NVL(UPPER(" & username &"),'%')" &_
			" AND Upper(action) LIKE NVL(UPPER(" & act &"),'%')" &_
			" AND Upper(table_name) LIKE NVL(UPPER(" & docmgr_tabname & "),'%') " &_
			" AND rid BETWEEN NVL(" & lo_rid & ",1) AND NVL(" & hi_rid & ",999999999999)" &_
			" AND timestamp BETWEEN NVL(" & lowdate & ",to_date('1980-01-01 0:0:0','YYYY-MM-DD HH24:MI:SS')) AND NVL(" & highdate & ",SYSDATE)" &_
			" GROUP BY " 
	if grpByTableName then sql = sql & " table_Name,"
	if grpByAction then sql = sql & " action,"		
	if grpByUserName then sql = sql & " user_name,"
			
	sql = sql & DateExpression

	if sortBy = "" then 
		'sql = sql & " ORDER BY  v.user_name, v.table_name, v.timestamp DESC"	
	else
		sql = sql & " ORDER BY  " & sortBy & " " & sortDir
	end if

elseif app = "cs_security" then

end if
'End of SYAN modification
'Response.Write SQL
'Response.End

Set RS = Conn.execute(sql)
Response.ContentType = "text/xml"
Response.Write "<?xml:stylesheet type=""text/xsl"" href=""recordsetxml.xsl""?>" & vbCrLf
RS.Save Response, 1
'Response.Write sql
%>




