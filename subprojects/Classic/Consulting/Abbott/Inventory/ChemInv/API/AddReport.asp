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

bDebugPrint = false
bWriteError = false
strError = "Error:AddReport<BR>"

'RPT paths

'Get Inv_Report Parameters
NumParams= Request("NumParams")
ReportName = Request("ReportName")
ReportDisplayName = Request("ReportDisplayName")
QueryName = Request("QueryName")
QuerySQL = Request("QuerySQL")
ReportDesc = Request("ReportDesc")
ReportTypeID=Request("ReportTypeID")

if ReportName = "NULL" OR ReportName = "" then
	strError = strError & "ReportName is a required parameter<BR>"
	bWriteError = True
end if
if ReportDisplayName = "NULL" OR ReportDisplayName = "" then
	strError = strError & "ReportDisplayName is a required parameter<BR>"
	bWriteError = True
end if
if QueryName = "NULL" OR QueryName = "" then
	strError = strError & "QueryName is a required parameter<BR>"
	bWriteError = True
end if
if QuerySQL = "NULL" OR QuerySQL = "" then
	strError = strError & " QuerySQL is a required parameter<BR>"
	bWriteError = True
end if

if bDebugPrint then
		Response.Write "QuerySQL = " & QuerySQL & "<BR>"
		Response.write "QueryName:" & QueryName & "<BR>"
		Response.write "ReportName:" & ReportName & "<BR>"
		Response.write "ReportDisplayName:" & ReportDisplayName & "<BR>"
		Response.write "ReportTypeID:" & ReportTypeID & "<BR>"
end if

'Parse SQL Query to allow insertion into the database
'QuerySQL = Replace(QuerySQL,"'","''")
' Set up and ADO command to insert Report in INV_REPORTS
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reports.CreateReport", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTNAME", adVarChar, adParamInput, 255, ReportName) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTDISPLAYNAME", adVarChar, adParamInput, 255, ReportDisplayName) 
Cmd.Parameters.Append Cmd.CreateParameter("PQUERYNAME", adVarChar, adParamInput, 255, QueryName) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTSQL", AdLongVarChar, 1, len(QuerySQL) + 1, QuerySQL) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORT_DESC", adVarChar, adParamInput, 2000, ReportDesc) 
Cmd.Parameters.Append Cmd.CreateParameter("PREPORTTYPE_ID",adNumeric, adParamInput, , ReportTypeID) 
Cmd.Properties("SPPrmsLOB") = TRUE

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.write "Query Length:" & len(QuerySQL) & "<BR>"	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reports.CreateReport")
	Cmd.Properties("SPPrmsLOB") = FALSE
End if
' Return the newly created ReportID
Report_ID = Cmd.Parameters("RETURN_VALUE")
Response.Write "Report with ID " & Report_ID & " inserted."


'Clean up
'Conn.Close
'Set Conn = Nothing
'Set Cmd = Nothing
'Response.end

'Get Inv_ReportParams Parameters
NumParams = FormatNumber(NumParams)
'Response.write "Num:" & NumParams
if NumParams <> 0 then
	for i=1 to NumParams
	'Response.write "Num:" & NumParams
	'Response.end
		'Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ReportParams.InsertParam", adCmdStoredProc)
		IsRequired = Request("Parameter" &i & "IsRequired")
		if IsEmpty(IsRequired) then
			IsRequired = 0
		end if
		ParamName= Request("Parameter" &i & "Name")
		ParamDisplayName= Request("Parameter" &i & "DisplayName")
		ParamType= Request("Parameter" &i & "Type")
		'Response.write "ID" & Report_ID
		'Response.end
		' Set up and ADO command to insert Report Params in INV_REPORTSPARAMS
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ReportParams.InsertParam", adCmdStoredProc)
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adChar, adParamReturnValue, 255, NULL)
		'Cmd.Parameters("RETURN_VALUE").Precision = 9			
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
			Response.Write "Parameter " & Cmd.Parameters("RETURN_VALUE") & " inserted.<br>"
		End if

    next
end if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

	
if bDebugPrint then
	'Debugging section
	Response.write("ReportName:" & ReportName &"<br>")
	Response.write("NumParams:" & NumParams &"<br>")
Else

Response.End

End if
%>