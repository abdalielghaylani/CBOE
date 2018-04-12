<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:CalcMolarConc<BR>"
MolarAmount = Request("MolarAmount")
MolarUnitFK = Request("MolarUnitFK")
SolventVolume1 = Request("SolventVolume1")
SolventVolumeUnitID1 = Request("SolventVolumeUnitID1")
SolventVolume2 = Request("SolventVolume2")
SolventVolumeUnitID2 = Request("SolventVolumeUnitID2")
ConcentrationUnitID = Request("ConcentrationUnitID")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CalcMolarConc.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(MolarAmount) then
	strError = strError & "MolarAmount is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolume1) then
	strError = strError & "SolventVolume1 is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SolventVolumeUnitID1) then
	strError = strError & "SolventVolumeUnitID1 is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ChemCalcs.GetMolarConc", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 5, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PMOLARAMOUNT", 5, 1, 0, MolarAmount)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUME1", 5, 1, 0, SolventVolume1)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMEUNITID1", 5, 1, 0, SolventVolumeUnitID1)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUME2", 5, 1, 0, SolventVolume2)
Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTVOLUMEUNITID2", 5, 1, 0, SolventVolumeUnitID2)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ChemCalcs.GetMolarConc")
end if
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing

Response.Write out

</SCRIPT>
