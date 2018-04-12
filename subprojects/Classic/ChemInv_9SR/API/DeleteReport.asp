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
bWriteError = False
strError = "Error:DeleteReport<BR>"

'RPT paths

'Get Inv_Report Parameters
Report_ID = Request("Report_ID")


if Report_ID = "NULL" then
	strError = strError & "Report_ID is a required parameter<BR>"
	bWriteError = True
end if

if bDebugPrint then
		Response.write "Report_ID:" & Report_ID & "<BR>"
		Response.end
end if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command to delete the report and its parameters
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reports.DeleteReport", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PID", adNumeric, adParamInput, ,Report_ID) 			

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reports.DeleteReport")
End if
' Return the delete status
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
Response.End
%>