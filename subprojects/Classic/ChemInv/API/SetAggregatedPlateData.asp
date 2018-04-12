<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim PlateIDs
Dim ValuePairs
Dim strError
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:UpdateWell<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/SetAggregatedPlateData.htm"
	Response.end
End if

'Required Paramenters
PlateIDs = Request("PlateIDs")

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".PLATECHEM.SetAggregatedPlateData", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("pPlateIDList", 200, 1, 4000, PlateIDs)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".PLATECHEM.SetAggregatedPlateData")
End if
Conn.Close
Set Cmd = Nothing
Set Conn = Nothing
</SCRIPT>
