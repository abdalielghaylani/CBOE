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
	Response.Redirect "/cheminv/help/admin/api/UpdateWell.htm"
	Response.end
End if

'Required Paramenters
WellIDs = Request("WellIDs")
PlateIDs = Request("PlateIDs")
ValuePairs = Request("ValuePairs")

if len(WellIDs) = 0 then WellIDs = ""
if len(PlateIDs) = 0 then PlateIDs = ""

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdateWell", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PWELLIDS", 200, 1, 4000, WellIDs)
Cmd.Parameters.Append Cmd.CreateParameter("PPLATEIDS", 200, 1, 4000, PlateIDs)
Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 4000, ValuePairs)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdateWell")
End if

' Return the Update Status
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
