<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:SolvatePlate<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/SolvatePlate.htm"
	Response.end
End if

'Required Paramenters
PlateIDList = Request("PlateIDList")
SolventIDList = Request("SolventIDList")
SolventVolumeList = Request("SolventVolumeList")
SolventVolumeUnitIDList = Request("SolventVolumeUnitIDList")
ConcentrationList = Request("ConcentrationList")
ConcentrationUnitIDList = Request("ConcentrationUnitIDList")

' Check for required parameters
If IsEmpty(PlateIDList) then
	strError = strError & "PlateIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventIDList) then
	strError = strError & "SolventIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolumeList) then
	strError = strError & "SolventVolumeList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolumeUnitIDList) then
	strError = strError & "SolventVolumeUnitIDList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ConcentrationList) then
	strError = strError & "ConcentrationList is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ConcentrationUnitIDList) then
	strError = strError & "ConcentrationUnitIDList is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".SOLVATEPLATES", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PPLATEIDLIST", 200, 1, 2000, PlateIDList) 	
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTIDLIST", 200, 1, 2000, SolventIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMELIST", 200, 1, 2000, SolventVolumeList)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMEUNITIDLIST", 200, 1, 2000, SolventVolumeUnitIDList)
Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATIONLIST", 200, 1, len(ConcentrationList), ConcentrationList)
Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATIONUNITIDLIST", 200, 1, 2000, ConcentrationUnitIDList)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".SOLVATEPLATES")
End if

' Return the newly created PlateIDs
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
'NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

'Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
