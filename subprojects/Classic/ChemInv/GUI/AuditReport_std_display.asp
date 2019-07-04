<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

'SYAN added on 1/20/2005 to fix CSBR-50830
app = request("app")
if app = "" then
	app = "inv"
end if
'End of SYAN modification

bDebugPrint = false
bWriteError = False
strError = "Error:AuditReport_display.asp<BR>"

'Response.Write "request(tablename)=" & request("tablename")

username = "'" & request("userID") & "'"
tabname = "'" & request("tablename") & "'"
'SYAN added on 1/20/2005 to fix CSBR-50830
docmgr_tabname = "'" & request("docmgr_tablename") & "'"
'End of SYAN modification
act =   "'" & Ucase(request("action")) & "'" 
lo_rid = request("lo_rid")
hi_rid = request("hi_rid")
locationBarcode = request("locationBarcode")
containerBarcode = request("containerBarcode")
compoundID = request("compoundID")
fromDate = request("fromDate")
toDate = request("toDate")
column_name =   "'" & request("column_name") & "'" 
old_value =  "'" &  request("old_value") & "'" 
new_value =  "'" &  request("new_value") & "'"
oldSortBy = Session("audSortBy") 
sortBy = Request("sortBy")

'only use session variables if this is a sort
if not isEmpty(sortBy) then
	if username = "''"  and not isEmpty(Session("audUserName")) then username = Session("audUserName")
	if tabname = "''" and not isEmpty(Session("audTabName")) then tabname = Session("audTabName")
	'SYAN added on 1/20/2005 to fix CSBR-50830
	if docmgr_tabname = "''" and not isEmpty(Session("docmgr_audTabName")) then docmgr_tabname = Session("docmgr_audTabName")
	'End of SYAN modification
	if act = "''" and not isEmpty(Session("audAct")) then act = Session("audAct")
	if lo_rid = "" then lo_rid = Session("audLo_RID")
	if hi_rid = "" then hi_rid = Session("audHi_RID")
	if locationBarcode = "" then locationBarcode = Session("audLocationBarcode")
	if containerBarcode = "" then containerBarcode = Session("audContainerBarcode")
	if compoundID = "" then compoundID = Session("audCompoundID")
	if fromDate = "" then fromDate = Session("audFromDate")
	if toDate = "" then toDate = Session("audToDate")
	if column_name = "''" and not isEmpty(Session("audColumn_Name")) then column_name = Session("audColumn_Name")
	if old_value = "''" and not isEmpty(Session("audOld_Value")) then old_value = Session("audOld_Value")
	if new_value = "''" and not isEmpty(Session("audNew_Value")) then new_value = Session("audNew_Value")
	if sortBy = "" and Session("audPage") = "standard" then sortBy = Session("audSortBy")
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
Session("audLocationBarcode") = LocationBarcode
Session("audContainerBarcode") = ContainerBarcode
Session("audCompoundID") = CompoundID
Session("audFromDate") = FromDate
Session("audToDate") = ToDate
Session("audColumn_Name") = Column_Name
Session("audOld_Value") = Old_Value
Session("audNew_Value") = New_Value
Session("audSortBy") = sortBy
Session("audPage") = "standard"

if isEmpty(Session("audSortDir")) or (oldSortBy <> sortBy) or (oldSort = sortBy and Session("audSortDir") = "ASC") then 
	sortDir = "DESC"
elseif Session("audSortDir") = "DESC" then
	sortDIR = "ASC"
end if
Session("audSortDir") = sortDir

'Response.Write tabName
'Response.Write sortBy
'Response.End

if username = "''" then username = "NULL"
if tabname = "''" then tabname = "NULL"
'SYAN added on 1/20/2005 to fix CSBR-50830
if docmgr_tabname = "''" then docmgr_tabname = "NULL"
'End of SYAN modification
if act = "''" then act = "NULL"
if lo_rid = "" then lo_rid = "NULL"
if hi_rid = "" then hi_rid = "NULL"
if column_name = "''" then 
	column_name = "NULL"
	bColumnNameNull = false
elseif lcase(column_name) = "'null'" then
	bColumnNameNull = true
end if
if old_value = "''" then 
	old_value = "NULL"
	bOldValueNull = false
elseif lcase(old_value) = "'null'" then
	bOldValueNull = true
end if
if new_value = "''" then 
	new_value = "NULL"
	bNewValueNull = false
elseif lcase(new_value) = "'null'" then
	bNewValueNull = true
end if
if fromDate = "" then
	lowdate = "NULL"
Elseif IsDate(fromDate) then
	lowdate = GetOracleDateString3(fromDate)
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
'SYAN added on 1/20/2005 to fix CSBR-50830
if app = "inv" then
	sql =	"SELECT v.raid RAID," & _
			" v.rid RID," &_
			" lower(table_name) TABLE_NAME," &_
			" DECODE(action, 'I', 'INSERT', 'U', 'UPDATE', 'D', 'DELETE') Action," &_
			" lower(user_name) Modified_by," &_
			" DECODE(Column_Name, NULL, 'NULL', Column_Name) Column_Name," &_
			" DECODE(Old_Value, NULL,'NULL', Old_Value) Old_value," &_
			" DECODE(New_Value, NULL,'NULL', New_Value) New_value," &_
			" TO_CHAR(timestamp, '" & Application("DATE_FORMAT_STRING") & " HH24:MI:SS') timestamp" &_
			" FROM " & Application("CHEMINV_USERNAME") & ".audit_column c" & "," & Application("CHEMINV_USERNAME") & ".inv_vw_audit_aggregate v" &_
		    " WHERE v.raid = c.raid(+) " &_
			" AND Upper(v.user_name) LIKE NVL(UPPER(" & username &"),'%')" &_
			" AND Upper(v.table_name) LIKE NVL(UPPER(" & tabname & "),'%')" &_
			" AND Upper(v.action) LIKE NVL(UPPER(" & act & "),'%')" &_
			" AND v.rid BETWEEN NVL(" & lo_rid & ",1) AND NVL(" & hi_rid & ",999999999999)" &_
			" AND v.timestamp BETWEEN NVL(" & lowdate & ",to_date('1980-01-01 0:0:0','YYYY-MM-DD HH24:MI:SS')) AND NVL(" & highdate & ",SYSDATE)"
	if containerBarcode <> "" then
			sql = sql & " AND v.rid = (SELECT rid from inv_containers where barcode =?)"
	End if
	if locationBarcode <> "" then
			sql = sql & " AND v.rid = (SELECT rid from inv_locations where location_barcode =?)"
	End if
	if compoundID <> "" then
			sql = sql & " AND v.rid = (SELECT rid from inv_compounds where compound_id =?)"
	End if
	if act = "'U'" then	
		if bColumnNameNull then
			sql = sql & " AND upper(c.column_name) is null"		
		elseif column_name <> "NULL" then
			sql = sql &	" AND upper(c.column_name) LIKE NVL(upper(" & column_name & "),'%')"
		end if
		if bOldValueNull then
			sql = sql & " AND upper(c.old_value) is null"		
		elseif old_value <> "NULL" then
			sql = sql &	" AND upper(c.old_value) LIKE NVL(upper(" & old_value & "),'%')"
		end if
		if bNewValueNull then
			sql = sql & " AND upper(c.new_value) is null"
		elseif new_value <> "NULL" then
			sql = sql & " AND upper(c.new_value) LIKE NVL(upper(" & new_value & "),'%')"
		end if
	end if
	if sortBy = "" then 
		sql = sql & " ORDER BY  v.user_name, v.table_name, v.timestamp DESC"	
	else
		sql = sql & " ORDER BY  " & sortBy & " " & sortDir
	end if


	if bDebugPrint then 
		Response.Write "User: " & username & "<BR>"
		Response.Write "Table: " & tabName & "<BR>"
		Response.Write "Action: " & act & "<BR>"
		Response.Write "From Date: " & fromDate & "<BR>"
		Response.Write "To Date: " & toDate & "<BR>" 
		Response.Write sql
		Response.End
	end if
	Response.Expires = -1
	GetInvConnection()
	Set Cmd = server.createobject("ADODB.Command")
	Cmd.CommandText = sql
	Cmd.ActiveConnection = Conn
	if containerBarcode <> "" then
	Cmd.Parameters.Append Cmd.CreateParameter("containerBarcode", adVarchar, adParamInput, 50, containerBarcode)
	End if
	if locationBarcode <> "" then
	Cmd.Parameters.Append Cmd.CreateParameter("locationBarcode", adVarchar, adParamInput, 50, locationBarcode)
	End if
	if compoundID <> "" then
	Cmd.Parameters.Append Cmd.CreateParameter("compoundID", 131, 1, 0, compoundID)
	End if
	Set RS = Cmd.Execute
elseif app = "docmanager" then
	ConnStr = "File Name=" & Server.MapPath("/docmanager/config/docmanager.udl") & ";User ID=docmgr;Password=oracle"
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr 
	
	sql =	"SELECT v.raid RAID," & _
			" v.rid RID," &_
			" lower(table_name) TABLE_NAME," &_
			" DECODE(action, 'I', 'INSERT', 'U', 'UPDATE', 'D', 'DELETE') Action," &_
			" lower(user_name) Modified_by," &_
			" DECODE(Column_Name, NULL, 'NULL', Column_Name) Column_Name," &_
			" DECODE(Old_Value, NULL,'NULL', Old_Value) Old_value," &_
			" DECODE(New_Value, NULL,'NULL', New_Value) New_value," &_
			" TO_CHAR(timestamp, '" & Application("DATE_FORMAT_STRING") & " HH24:MI:SS') timestamp" &_
			" FROM docmgr.audit_column c" & ", docmgr.docmgr_vw_audit_aggregate v" &_
		    " WHERE v.raid = c.raid(+) " &_
			" AND Upper(v.user_name) LIKE NVL(UPPER(" & username &"),'%')" &_
			" AND Upper(v.table_name) LIKE NVL(UPPER(" & docmgr_tabname & "),'%')" &_
			" AND Upper(v.action) LIKE NVL(UPPER(" & act & "),'%')" &_
			" AND v.rid BETWEEN NVL(" & lo_rid & ",1) AND NVL(" & hi_rid & ",999999999999)" &_
			" AND v.timestamp BETWEEN NVL(" & lowdate & ",to_date('1980-01-01 0:0:0','YYYY-MM-DD HH24:MI:SS')) AND NVL(" & highdate & ",SYSDATE)"

	if act = "'U'" then	
		if column_name <> "NULL" then sql = sql &	" AND upper(c.column_name) LIKE NVL(upper(" & column_name & "),'%')"
		if bOldValueNull then
			sql = sql & " AND upper(c.old_value) is null"		
		elseif old_value <> "NULL" then
			sql = sql &	" AND upper(c.old_value) LIKE NVL(upper(" & old_value & "),'%')"
		end if
		if bNewValueNull then
			sql = sql & " AND upper(c.new_value) is null"
		elseif new_value <> "NULL" then
			sql = sql & " AND upper(c.new_value) LIKE NVL(upper(" & new_value & "),'%')"
		end if
	end if
	if sortBy = "" then 
		sql = sql & " ORDER BY  v.user_name, v.table_name, v.timestamp DESC"	
	else
		sql = sql & " ORDER BY  " & sortBy & " " & sortDir
	end if
Set RS = Conn.execute(sql)
elseif app = "cs_security" then

end if
'End of SYAN modification

Response.ContentType = "text/xml"

Response.Write "<?xml-stylesheet type=""text/xsl"" href=""recordsetxml.xsl""?>" & vbCrLf

RS.Save Response, 1
'Response.Write sql

%>





