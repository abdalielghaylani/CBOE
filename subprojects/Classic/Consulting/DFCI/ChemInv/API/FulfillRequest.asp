<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim httpResponse
Dim FormData
Dim Credentials
Dim QueryString
Dim ServerName
Dim arrValues()
Dim ContainerID
Dim Action
Dim NumContainers
Dim ContainerSize
Dim CurrValue
Dim bSameValue
Dim bDebugPrint
Dim strError
Dim bWriteError

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE

RequestID = Request("RequestID")
SampleContainerIDs = Request("SampleContainerIDs")

' Check for required parameters
If IsEmpty(RequestID) then
	strError = strError & "RequestID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SampleContainerIDs) then
	strError = strError & "SampleContainerIDs is a required parameter<BR>"
	bWriteError = True
end if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PSAMPLECONTAINERIDS",advarchar, 1, 500, SampleContainerIDs)
Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest")


/>