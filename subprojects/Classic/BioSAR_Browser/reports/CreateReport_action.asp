<%@ Language=VBScript %>

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
Response.Buffer = true

%>
<html>
<head>
<!--#INCLUDE VIRTUAL = "/biosar_browser/reports/ReportUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/reports/reports_app_js.js"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->

<%

server.ScriptTimeout = 600
Dim httpResponse
Dim FormData
Dim ServerName
dbkey = request("dbname")
formmode = request("formmode")
formgroup = request("formgroup")
if Session("ReportHitListMessage" & dbkey & formgroup) <> "" then
	response.write "<font color = ""red"">" &  Session("ReportHitListMessage" & dbkey & formgroup) & "</font><br>"
end if
formgroup_name = replace(Application("formgroup_full_name" & dbkey & formgroup), ".", "")
formgroup_description = replace(Application("formgroup_description" & dbkey & formgroup), ".", "")
if Not formgroup_description <> "" then
	formgroup_description = formgroup_name
	Application("formgroup_description" & dbkey & formgroup) = formgroup_name
end if
'FullSingleRS_SQL = Session("FullSingleRS_SQL" & "LIST" & dbkey & formgroup)
BaseTable = Session("BaseTable" & dbkey & formgroup)
SQL_NEW_LIST=Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "LIST")
SQL_NEW_LIST = populateOrderBy(SQL_NEW_LIST)

SQL_LIST=Session("fg_fields_reportSQL" & dbkey & formgroup & "LIST")
SQL_LIST = populateOrderBy(SQL_LIST)


QUERY_NAMES_LIST = Session("fg_fields_QueryNames" & dbkey & formgroup & "LIST")

SQL_NEW_DETAIL=Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "DETAIL")
SQL_NEW_DETAIL = populateOrderBy(SQL_NEW_DETAIL)

SQL_DETAIL=Session("fg_fields_reportSQL" & dbkey & formgroup & "DETAIL")
SQL_DETAIL = populateOrderBy(SQL_DETAIL)

QUERY_NAMES_DETAIL = Session("fg_fields_QueryNames" & dbkey & formgroup & "DETAIL")
bCLOB_ORA =Session("CLOB_ORA" & dbkey & formgroup)
ReportName = Request("ReportName")
ReportFormat = Request("ReportFormat")
Run_Report = UCase(request("Run_Report"))

If ReportFormat = "" then ReportFormat = "snp"
If ReportName = "" then ReportName = "LIST"
if (ReportFormat <> "NULL" AND ReportName <> "NULL") AND Run_Report = "TRUE" then

	ServerName = Request.ServerVariables("Server_Name")
	Credentials = "&CSUserName=" & Session("UserName" & "biosar_browser") & "&CSUSerID=" & Session("UserID" & "biosar_browser")
	ReportFunction = "CreateSearchResultsReport.asp"
	FormData = "ReportFormat=" & ReportFormat & "&ReportName=" & ReportName & Credentials & "&dbname=" & dbkey & "&formgroup=" & formgroup
	FormData = FormData & "&formgroup_name="  & Server.URLEncode(formgroup_name)
	FormData = FormData & "&formgroup_description="  & Server.URLEncode(formgroup_description)
	FormData = FormData & "&SQL_NEW_LIST=" & Server.URLEncode(SQL_NEW_LIST)
	FormData = FormData & "&SQL_LIST=" & Server.URLEncode(SQL_LIST)
	FormData = FormData & "&QUERY_NAMES_LIST=" & Server.URLEncode(QUERY_NAMES_LIST)
	FormData = FormData & "&SQL_NEW_DETAIL=" & Server.URLEncode(SQL_NEW_DETAIL)
	FormData = FormData & "&SQL_DETAIL=" & Server.URLEncode(SQL_DETAIL)
	FormData = FormData & "&QUERY_NAMES_DETAIL=" & Server.URLEncode(QUERY_NAMES_DETAIL)
	FormData = FormData & "&BaseTable=" & BaseTable  & "&clob_ora=" & bCLOB_ORA
	FormData = FormData & "&profile_name=" & request("profile_name")  
	FormData = FormData & "&table_layout=" & request("table_layout")  
	FormData = FormData & "&New_Page_Each_Rec=" & request("New_Page_Each_Rec")  
	FormData = FormData & "&report_orientation=" & request("report_orientation")  
	FormData = FormData & "&report_papersize=" & request("report_papersize")  
	ReportTypeID = 2
	httpResponse = CShttpRequest2("POST", ServerName, "/biosar_browser/reports/" & ReportFunction , "BioSAR_Browser", FormData)
	
else
httpResponse=""
end if
'Response.Write httpResponse
'Response.end
%>

<title> BioSAR Browser Create Report</title>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->

</script>
</head>
<body>

	<table border="0" cellspacing="0" cellpadding="2" width="398" align="left">
		<tr>
			<td valign="top" align="left" width="313" nowrap><font size = 4>
				<b>BioSAR Browser Reports </b></font>
			</td>
			<td align="right" valign="top" width="377">
											
							<table>
                              <tr>
                                <td>
									<%if Session("hitlistid" & dbkey & formgroup) <> "" AND Not CheckValidReportVars(dbkey, formgroup)<> "" then%>		
							<input type = "button" name = "Generate_Report" value = "Run Report" onclick ="doRunReport()" style="float: right"></td>
                                <%else
									if Not Session("hitlistid" & dbkey & formgroup) <> "" then
										Response.write "your session has timed out"
										else
										if CheckValidReportVars(dbkey, formgroup)<> "" then
											Response.Write CheckValidReportVars(dbkey, formgroup)
										end if
									end if
                                end if%>
                               
                                <td>	<input type = "button" name = "Close" value = "Close" onclick ="javascript:parent.close()"></td>
                              </tr>
                            </table>
&nbsp;</tr>
				</td>
		<tr>
			<td colspan="2" width="700">
				<form name="form1" action="CreateReport_action.asp" method="GET">
				
				<table border="0" cellspacing="0" cellpadding="0" align="left" width="600" style="border-collapse: collapse" bordercolor="#111111">
				<input type = "hidden" name = "dbname" value = "<%=dbkey%>">
				<input type = "hidden" name = "formgroup" value = "<%=formgroup%>">
			    <input type = "hidden" name = "run_report" value = "true">
					
						<tr><td align="right" width="150" nowrap>
							Data view:&nbsp;</td><td align="left" width="250">
							<%
							
							
							if request("ReportName") <> "" then
								reportNameDefault = request("ReportName") 
							else
								reportNameDefault = "LIST"
							end if
							Response.write ShowReportNameSelectBox("ReportName",reportNameDefault, "SELECT Description AS DisplayText, Value FROM RptSettingLkup Where lkup_name ='dataview'",dbkey, formgroup)
							
							%>

						</td></tr><tr>
						<td align="right" width="150" nowrap>
							Table layout:&nbsp;</td><td align="left" width="250">
							<%
							
							if request("table_layout") <> "" then
								table_layoutDefault = request("table_layout") 
							else
								table_layoutDefault = "ACROSS"
							end if
							Response.write ShowTableLayoutSelectBox("table_layout",table_layoutDefault,"SELECT Description AS DisplayText, Value as [value] FROM RptSettingLkup Where lkup_name ='tablelayout' ")
							
							%>

						</td></tr><tr>
						<td align="right"width="150" nowrap>
							Settings Profile:&nbsp;</td><td align="left" width="250">
							<%
							
							profile_info = request("profile_name")
							
							if not profile_info <> "" then
								profile_info = "1|biosar_admin"
							end if
							Response.write ShowProfileSelectBox("profile_name", profile_info,"SELECT Owner, Profile_Name as Name, Description AS DisplayText, Profile_id as [value] FROM RptProfiles where owner='" & Session("username" & "biosar_browser") & "' OR is_public='1'")
								response.write "<script language = ""javascript"">"
								response.write  "getProfileButtons()"
								response.write "</script>"
							
							%>
							
							

						</td></tr><tr><td align="right" width="150" nowrap>
							Page Orientation:&nbsp;</td><td align="left" width="400">
							<%
							
							if request("report_orientation") <> "" then
								report_orientationDefault = request("report_orientation") 
							else
								report_orientationDefault = "AUTO"
							end if
							Response.write ShowSelectBox("report_orientation",report_orientationDefault, "SELECT Description AS DisplayText,Value  as [value] FROM RptSettingLkup Where lkup_name = 'ReportOrientation'")
							
							%>

						</td></tr><tr>
						</td></tr><tr><td align="right"width="150" nowrap>
							Page Size:&nbsp;</td><td align="left" width="400">
							<%
							
							if request("report_papersize") <> "" then
								report_papersizeDefault = request("report_papersize") 
							else
								report_papersizeDefault = "1"
							end if
							Response.write ShowSelectBox("report_papersize",report_papersizeDefault, "SELECT Description AS DisplayText,Value  as [value] FROM RptSettingLkup Where lkup_name = 'RptPaperSize'")
							
							%>

						</td></tr><tr>
						<td align="right"width="150" nowrap>
							1 Record Each Page:&nbsp;</td><td align="left" width="250">
							<%
							
							if request("New_Page_Each_Rec") <> "" then
								New_Page_Each_RecDefault = request("New_Page_Each_Rec") 
							else
								New_Page_Each_RecDefault = "0"
							end if
							Response.write ShowSelectBox("New_Page_Each_Rec",New_Page_Each_RecDefault, "SELECT Description AS DisplayText,Value as [value]  FROM RptSettingLkup Where lkup_name = 'NewPageEachRecord'")
							
							%>

						</td>	</tr>
						
					<tr>
						<td align="right" width="150" nowrap>
							Report Format:&nbsp;</td><td align="left" width="250">
							<%
							
							if request("ReportFormat") <> "" then
								ReportFormatDefault = request("ReportFormat") 
							else
								ReportFormatDefault = "SNP"
							end if
							if detectNS = true then
								sql = "SELECT Description AS DisplayText, Value as [value] FROM RptSettingLkup where lkup_name='reportformat' and not value = 'SNP'"
							else
								sql = "SELECT Description AS DisplayText, Value as [value] FROM RptSettingLkup where lkup_name='reportformat'"
							end if
							Response.write ShowSelectBox("ReportFormat",ReportFormatDefault, sql)
							
							on error resume next
							RPTConn.close
							
							%>

						</td>	
					</tr>
					<tr>
						<td width="250">
							<br>
&nbsp;<td align="left" width="1">
							
							&nbsp;</td>	
						</td>
					</tr>
				</table>
				</form>
			</td>
		</tr>
	</table>
	<%
If Not InStr(UCase(httpResponse),"ERROR")> 0 Then%>		
		<script LANGUAGE="javascript">top.DisplayFrame.location.href = "DisplayReport.asp?ReportFormat=<%=ReportFormat%>&ReportURL=<%=httpResponse%>";</script>
<%
Else
%>
		<script LANGUAGE="javascript">top.DisplayFrame.location.href = "DisplayReport.asp?ReportFormat=GUI_FEEDBACK&ReportURL=<%=httpResponse%>";</script>
<%End if%>		

</body>
