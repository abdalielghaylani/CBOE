<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/custom/api/EditEHSInfo_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim theID

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:EditEHSInfo<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/EditEHSInfo.htm"
	Response.end
End if

'Required Parameters
ContainerID = Request("ContainerID")
CompoundID = Request("CompoundID")
action = Request("action")
TrackBy = Request("TrackBy")
EHSGroup1 = Request("EHSGroup1")
EHSGroup2 = Request("EHSGroup2")
EHSGroup3 = Request("EHSGroup3")
EHSHealth = Request("EHSHealth")
EHSFlammability = Request("EHSFlammability")
EHSReactivity = Request("EHSReactivity")
EHSPackingGroup = Request("EHSPackingGroup")
EHSUNNumber = Request("EHSUNNumber")
EHSIsOSHACarcinogen = Request("EHSIsOSHACarcinogen")
EHSACGIHCarcinogenCategory = Request("EHSACGIHCarcinogenCategory")
EHSIsSensitizer = Request("EHSIsSensitizer")
EHSIsRefrigerated = Request("EHSIsRefrigerated")
EHSIARCCarcinogen = Request("EHSIARCCarcinogen")
EHSEUCarcinogen = Request("EHSEUCarcinogen")
EHSIsDefaultSource = Request("EHSIsDefaultSource")

' Set up and ADO command
if Trackby = "CAS" then
	if action = "edit" then
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdateSubstanceHazMatData", adCmdStoredProc)
	Else
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".InsertSubstanceHazMatData", adCmdStoredProc)
	End if
		theID = CompoundID
Else
	If EHSIsDefaultSource = "1" or EHSIsDefaultSource = "2" Then
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".InsertHazMatData", adCmdStoredProc)
	Else
		Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdateHazMatData", adCmdStoredProc)
	End If
		theID = ContainerID
End if
Call GetEditEHSInfo_cmd()

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".EditHazMatInfo")
End if

Conn.Close
Set Cmd = Nothing
Set Conn = Nothing

</SCRIPT>
