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
strError = "Error:CalcVolume<BR>"
MolarAmount = Request("MolarAmount")
'MolarUnitFK = Request("MolarUnitFK")
Concentration = Request("Concentration")
ConcentrationUnitID = Request("ConcentrationUnitID")
VolumeUnitID = Request("VolumeUnitID")
CurrSolventVolume = Request("CurrSolventVolume")
CurrSolventVolumeUnitID = Request("CurrSolventVolumeUnitID")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CalcVolume.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(MolarAmount) then
	'strError = strError & "MolarAmount is a required parameter<BR>"
	'bWriteError = True
	MolarAmount= 0
End if
'If IsEmpty(MolarUnitFk) then
'	strError = strError & "MolarUnitFK is a required parameter<BR>"
'	bWriteError = True
'End if
If IsEmpty(Concentration) then
	strError = strError & "Concentration is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ConcentrationUnitID) then
	strError = strError & "ConcentrationUnitID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(VolumeUnitID) then
	strError = strError & "VolumeUnitID is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ChemCalcs.GetAddedSolventVolume", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 5, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PMOLARAMOUNT", 5, 1, 0, MolarAmount)
'Cmd.Parameters.Append Cmd.CreateParameter("PMOLARUNITFK", 5, 1, 0, MolarUnitFk)
Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATION", 5, 1, 0, Concentration)
Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATIONUNITID", 5, 1, 0, ConcentrationUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("PVOLUMEUNITID", 5, 1, 0, VolumeUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("PCURRSOLVENTVOLUME", 5, 1, 0, CurrSolventVolume)
Cmd.Parameters.Append Cmd.CreateParameter("PCURRSOLVENTVOLUMEUNITID", 5, 1, 0, CurrSolventVolumeUnitID)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ChemCalcs.GetAddedSolventVolume")
end if
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing

Response.Write out

</SCRIPT>
