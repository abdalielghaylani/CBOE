<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/reports/ReportUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/biosar_browser_admin/admin_tool/utilities.asp"-->

<%

Dim strError
Dim bWriteError
Dim PrintDebug
Dim QueryText, QueryText2
bDebugPrint = false
bWriteError = False
strError = "Error:CreateSearchResultsReport<BR>"
'Standars COWS varaibles
dbkey = Trim(request("dbname"))
formgroup = Trim(request("formgroup"))
'RPT paths

RPTPath = Application("RPTPath")

ReportQueuePath = Application("ReportQueuePath")
ReportArchiveDBPath = Application("ReportArchiveDBPath")	

if  instr(Application("CDAX_ProgID"), "14")> 0 then
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_14.mdb")
elseif  instr(Application("CDAX_ProgID"), "12")> 0 then
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_12.mdb")
elseif  instr(Application("CDAX_ProgID"), "10")> 0 then
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_10.mdb")
elseif instr(Application("CDAX_ProgID"), "11")> 0 then
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_11.mdb")	
elseif instr(Application("CDAX_ProgID"), "9")> 0 then
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_9.mdb")		
else
	ReportDBPath = Replace(Application("ReportDBPath"), ".mdb", "_14.mdb")
end if
ReportsHTTPPath = Application("ReportsHTTPPath")
ReportName = request("ReportName")
ReportFormat = UCase(request("ReportFormat"))
'report variables
formgroup_name = request("formgroup_name")
formgroup_description =  request("formgroup_description")
if Not formgroup_description <> "" then
	formgroup_description = formgroup_name
end if
BaseTable = request("BaseTable")
'Server.ScriptTimeout = 100000
'Query sql
SQL_DETAIL = Request("SQL_DETAIL")
SQL_NEW_DETAIL = Request("SQL_NEW_DETAIL")
SQL_LIST = Request("SQL_LIST")
SQL_NEW_LIST = Request("SQL_NEW_LIST")
bCLOB_Ora = request("clob_ora")
ReportDir = Application("RPTPath") & "reports\"
ReportSettingsPath = Replace(Application("ReportDBPath"), ".mdb", "_Settings.mdb")
ProfileName = request("profile_name")
if Not ProfileName <> "" then
	Profile_name = "default"
end if
report_papersize= request("report_papersize")
if Not report_papersize <> "" then
	report_papersize = "1"
end if
ReportOrientation = request("report_orientation")
if Not ReportOrientation <> "" then
	ReportOrientation = "auto"
end if
NewPageEachRec = request("New_Page_Each_Rec")
if Not NewPageEachRec <> "" then
	NewPageEachRec = "0"
end if

ReportName = request("ReportName")
if Not ReportName <> "" then
	ReportName = "LIST"
end if

TableLayout = request("table_layout")
if Not TableLayout <> "" then
	TableLayout = "ACROSS"
end if

ReportFormat = UCase(request("ReportFormat"))
if Not ReportFormat <> "" then
	ReportFormat = "SNP"
end if
'Query Names
QUERY_NAMES_LIST = Request("QUERY_NAMES_LIST")

QUERY_NAMES_DETAIL= Request("QUERY_NAMES_DETAIL")

If Not Application("ReportSequenceNum")<> "" or Application("ReportSequenceNum") = NULL then
	Application.Lock
	Application("ReportSequenceNum") = 1
	Application.UnLock
end if
if CLng(Application("ReportSequenceNum")) = 100 then
	Application.Lock
	Application("ReportSequenceNum") = 1
	Application.UnLock
end if
ReportOutputName = "BioSARRpt_" & Application("ReportSequenceNum") 
username = Session("UserName" & "biosar_browser")
encryptedPassword = SAR_CryptVBS(Session("UserID" & "biosar_browser"), UCase(username))
if (ReportFormat <> "NULL" AND ReportName <> "NULL") then
		'Create RPT
		on error resume next
		ReportGenerator = request("CSUserName")
		
		Select Case UCase(ReportName)
			Case "LIST"
				on error resume next
				'first generate list view report
				SendReportName = "generic_report"
				full_Report_name = formgroup_name & "_" & "LIST" & "_" & formgroup
				QueryText = SQL_NEW_LIST &  "|" & full_Report_name &  "|" & basetable & "|" & formgroup_description & "_List"
				if UCase(bCLOB_ORA) = "TRUE" then
					QueryText = QueryText & "|||||ORA"
				else
					QueryText = QueryText & "|||||MS"
				end if
				QueryText = QueryText & "|||||" & TableLayout
				QueryText = QueryText & "|||||" & ReportSettingsPath
				QueryText = QueryText & "|||||" & ProfileName
				QueryText = QueryText & "|||||" & ReportOrientation
				QueryText = QueryText & "|||||" & NewPageEachRec
				QueryText = QueryText & "|||||" & report_papersize
				QueryText = QueryText & "|||||" & Session("hitlistID" & dbkey & formgroup) 
				QueryText = QueryText & "|||||" & Application("ReportLogoPath")
				QueryText = QueryText & "|||||" & ReportDir
				QueryText = QueryText & "|||||" & ReportFormat
				QueryText = QueryText & "|||||" & ReportOutputName
				QueryText = QueryText & "|||||" & username
				QueryText = QueryText & "|||||" & encryptedPassword
				QueryText = QueryText & "|||||" & Application("ReportMaxHits") + 1
				
				
				QueryName = "generic_query"
				on error resume next
				Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
				TempReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, SendReportName, QueryName, QueryText, ReportFormat,ReportGenerator)
				
				if Not inStr(TempReportFileName, "ERROR")>0  then
					'ReportFileName = formgroup_name & "_" & "LIST" & "_" & formgroup & "." & ReportFormat
					'ReportFileName = Replace(ReportFileName, "*", "")
					ReportFileName = ReportOutputName & "." & ReportFormat
				else
					ReportFileName=TempReportFileName
				end if
				
				Set ReportQ = Nothing
				err.Clear()
				
			Case "DETAIL"
			
				'generate detail view report
				SendReportName = "generic_report"
				full_Report_name = formgroup_name & "_" & "DETAIL" & "_" & formgroup
				QueryText = SQL_NEW_DETAIL &  "|" & full_Report_name & "|" & basetable & "|" & formgroup_description & "_Detail"
				if UCase(bCLOB_ORA) = "TRUE" then
					QueryText = QueryText & "|||||ORA"
				else
					QueryText = QueryText & "|||||MS"
				end if
				QueryText = QueryText & "|||||" & TableLayout
				QueryText = QueryText & "|||||" & ReportSettingsPath
				QueryText = QueryText & "|||||" & ProfileName
				QueryText = QueryText & "|||||" & ReportOrientation
				QueryText = QueryText & "|||||" & NewPageEachRec
				QueryText = QueryText & "|||||" & report_papersize
				QueryText = QueryText & "|||||" & Session("hitlistID" & dbkey & formgroup) 
				QueryText = QueryText & "|||||" & Application("ReportLogoPath")
				QueryText = QueryText & "|||||" & ReportDir
				QueryText = QueryText & "|||||" & ReportFormat
				QueryText = QueryText & "|||||" & ReportOutputName 
				QueryText = QueryText & "|||||" & username
				QueryText = QueryText & "|||||" & encryptedPassword
				QueryText = QueryText & "|||||" & Application("ReportMaxHits") + 1
				
				QueryName = "generic_query"
				on error resume next
				Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
				TempReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, SendReportName, QueryName, QueryText, ReportFormat,ReportGenerator)
				
				if Not inStr(TempReportFileName, "ERROR")>0  then
					'ReportFileName = formgroup_name & "_" & "DETAIL" & "_" & formgroup & "." & ReportFormat
					'ReportFileName = Replace(ReportFileName, "*", "")
					ReportFileName = ReportOutputName & "." & ReportFormat
				else
					ReportFileName=TempReportFileName
				end if
				
				Set ReportQ = Nothing
				err.Clear()
				
			
			End Select
		'increment report sequence mnumber
		Application.Lock
		Application("ReportSequenceNum") = CLng(Application("ReportSequenceNum")) + 1
		Application.UnLock
		
		Response.ContentType = "text/html"
		If InStr(1,UCase(ReportFileName),"REPORT ERROR") = 0 Then	
			if inStr(UCase(ReportFileName), "METHOD") > 0 then
				ReportFileName = "The report generator can not be located. Please report this problem to your administrator."
			end if
			Response.Write ReportsHTTPPath & ReportFileName
		Else 
			
			Response.Write ReportFileName
			
		End if


end if


%>
