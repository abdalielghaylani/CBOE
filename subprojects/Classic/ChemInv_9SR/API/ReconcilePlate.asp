<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim LocationID
Dim ContainerID
Dim Conn
Dim Cmd

Dim strError
Dim bWriteError

bDebugPrint = false
bWriteError = False
strError = "Error:ReconcilePlate<BR>"

kLocationMissing = 4
kStatusMovedDuringReconciliation = 21
kStatusMissingDuringReconciliation = 22 
bCleanup = false

'variable names say container for historical reasons
LocationID = Request("LocationID")
VerifiedContainerIDList = Request("VerifiedContainerIDList")
AwolContainerIDList = Request("AwolContainerIDList")
MissingContainerIDList = Request("MissingContainerIDList")

'Response.Write Request.Form & "<BR>"
'Response.Write MissingContainerIDList & "<BR>"
' Redirect to help page if no parameters are passed
if Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ReconcilePlate.htm"
	Response.end
End if


' Check for required parameters
if IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
if VerifiedContainerIDList="" AND AwolContainerIDList= "" AND MissingContainerIDList = "" then
	strError = strError & "VerifiedContainerIDList or AwolContainerIDList or MissingContainerIDList is a required parameter<BR>"
	bWriteError = True
End if

' Respond with Error
if bWriteError then
	Response.Write strError
	Response.end
End if

' verified plates stay where they are
If Len(VerifiedContainerIDList) > 0 then
	' do nothing
End if

out="0"
' missing plates are movied to the missing location
If Len(MissingContainerIDList) > 0 then
	bCleanup = true
	ValuePairs = "location_id_fk=" & kLocationMissing
	' Set up an ADO command
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes", adCmdStoredProc)

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PPLATEIDS", 200, 1, 4000, MissingContainerIDList)
	Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 4000, ValuePairs)

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
	Else
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes")
	End if

	' Return the Update Status
	out = Cstr(Cmd.Parameters("RETURN_VALUE"))
End if

out2="0"
' awol containers are moved to the current locations=
If Not IsEmpty(AwolContainerIDList) then
	bCleanup = true
	ValuePairs = "location_id_fk=" & LocationID
	' Set up an ADO command
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes", adCmdStoredProc)

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PPLATEIDS", 200, 1, 4000, AwolContainerIDList)
	Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 4000, ValuePairs)

	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
	Else
		Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes")
	End if

	' Return the Update Status
	out2 = Cstr(Cmd.Parameters("RETURN_VALUE"))
End if
if out = "0" and out2 = "0" then
	RP = 1
else
	if out <> "0" then
		RP = out
	else 
		RP = out2
	end if
end if
	
Response.Write RP

if bCleanup then
	'Clean up
	Conn.Close
	Set Conn = Nothing
	Set Cmd = Nothing
end if
</SCRIPT>
