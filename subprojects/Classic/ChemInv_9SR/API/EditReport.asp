<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim strError
Dim bWriteError
Dim PrintDebug
Dim Conn
Dim Cmd

bDebugPrint = False
bWriteError = False
strError = "Error:EditReport<BR>"

'RPT paths

'Get Inv_Report Parameters
NumParams= Request("NumParams")
ReportName = Request("ReportName")
ReportDisplayName = Request("ReportDisplayName")
QueryName = Request("QueryName")
QuerySQL = Request("QuerySQL")
ReportDesc = Request("ReportDesc")
ReportType_ID=Request("ReportTypeID")
Report_ID = Request("Report_ID")

if ReportName = "NULL" OR ReportName = "" then
	strError = strError & "ReportName is a required parameter<BR>"
	bWriteError = True
end if
if ReportDisplayName= "NULL" OR ReportDisplayName= "" then
	strError = strError & "ReportDisplayName is a required parameter<BR>"
	bWriteError = True
end if
'if QueryName = "NULL" OR QueryName = "" then
'	strError = strError & "QueryName is a required parameter<BR>"
'	bWriteError = True
'end if
'if QuerySQL = "NULL" OR QuerySQL = "" then
'	strError = strError & " QuerySQL is a required parameter<BR>"
'	bWriteError = True
'end if
if Report_Desc = "NULL" OR Report_Desc = "" then
  Report_Desc = ""
end if

if bDebugPrint then
		Response.Write "QuerySQL = " & QuerySQL & "<BR>"
		Response.write "ReportName:" & ReportName & "<BR>"
		Response.write "ReportDisplayName:" & ReportDisplayName & "<BR>"
		Response.write "ReportType:" & ReportType_ID & "<BR>"
end if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'Parse SQL Query to allow insertion into the database
'QuerySQL = Replace(QuerySQL,"'","''")
'Response.write QuerySQL
'Response.end
' Set up and ADO command to insert Report in INV_REPORTS
if QuerySQL = "" or isNull(QuerySQL) then QuerySQL = " "
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reports.UpdateReport", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PID", adNumeric, adParamInput, ,Report_ID) 			
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTTYPEID", adNumeric, adParamInput, ,ReportType_ID) 			
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTNAME", adVarChar, adParamInput, 255, ReportName) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTDISPLAYNAME", adVarChar, adParamInput, 255, ReportDisplayName) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORT_DESC", adVarChar, adParamInput, 2000, ReportDesc) 
Cmd.Parameters.Append Cmd.CreateParameter("PQUERYNAME", adVarChar, adParamInput, 255, QueryName) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTSQL", adLongVarChar, 1, len(QuerySQL) + 1, QuerySQL)
Cmd.Properties("SPPrmsLOB") = TRUE
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.write "Query Length:" & len(QuerySQL) & "<BR>"	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reports.UpdateReport")
End if
Cmd.Properties("SPPrmsLOB") = FALSE
	
' Return the updated ReportID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Delete old Inv_ReportParams Parameters
' Set up and ADO command to delete Report Params in INV_REPORTSPARAMS
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ReportParams.DeleteParam", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 255, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTID", adNumeric, adParamInput, , Report_ID)  

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ReportParams.DeleteParam")
End if

'Insert Inv_ReportParams Parameters
NumParams = FormatNumber(NumParams)
if bDebugPrint then
	'Debugging section
	Response.write("ReportName:" & ReportName &"<br>")
	Response.write("NumParams:" & NumParams &"<br>")
End if

if NumParams <> 0 then
	for i=1 to NumParams
		IsRequired = Request("Parameter" & i & "IsRequired")
		if IsEmpty(IsRequired) then
			IsRequired = 0
		end if
		ParamName= Request("Parameter" & i & "Name")
		ParamDisplayName= Request("Parameter" & i & "DisplayName")
		ParamType= Request("Parameter" & i & "Type")
		'Response.write "ID" & Report_ID
		'Response.end
		' Set up and ADO command to insert Report Params in INV_REPORTSPARAMS
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ReportParams.InsertParam", adCmdStoredProc)
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adChar, adParamReturnValue, 255, NULL)
		Cmd.Parameters.Append Cmd.CreateParameter("PREPORTID", adNumeric, adParamInput, , Report_ID) 
		Cmd.Parameters.Append Cmd.CreateParameter("PPARAMDISPLAYNAME", adVarChar, adParamInput, 255, ParamDisplayName) 
		Cmd.Parameters.Append Cmd.CreateParameter("PPARAMNAME", adVarChar, adParamInput, 255, ParamName) 
		Cmd.Parameters.Append Cmd.CreateParameter("PPARAMTYPE", adVarChar, adParamInput, 10, ParamType) 
		Cmd.Parameters.Append Cmd.CreateParameter("PIS_REQUIRED", adNumeric, adParamInput, , IsRequired) 

		if bDebugPrint then
			For each p in Cmd.Parameters
				Response.Write p.name & " = " & p.value & "<BR>"
			Next	
		Else
			Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ReportParams.InsertParam")
			'Response.Write "Parameter " & Cmd.Parameters("RETURN_VALUE") & " inserted.<br>"
		End if
    next
end if

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
Response.End
%>