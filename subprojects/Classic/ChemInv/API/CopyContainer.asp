<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/Getcopycontainer_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:CopyContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CopyContainer.htm"
	Response.end
End if

'Required Paramenters
ContainerID = Request("ContainerID")
Action = Request("Action")
NumContainers = Request("NumContainers")

'if Action = "split" then 
'	NumCopies = NumContainers-1
'elseif Action = "sample" then
	NumCopies = NumContainers
'end if	

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CopyContainer", adCmdStoredProc)
Call GetCopyContainer_cmd()

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CopyContainer")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close
Set paramR = Nothing
Set Cmd = Nothing
Set Conn = Nothing
' Increment container count
If out > 0 then
	tempArr = split(NewIDS,"|")
	numContainers = Ubound(tempArr) + 1
	If isEmpty(Application("inv_ContainersRecordCountChemInv")) then
		Application.Lock
			Application("inv_Containers" & "RecordCount" & "ChemInv") = GetContainerCount()
		Application.UnLock
	end if
	containerCount = CLng(Application("inv_Containers" & "RecordCount" & "ChemInv")) + numContainers 
	Application.Lock
		Application("inv_Containers" & "RecordCount" & "ChemInv") = containerCount
	Application.UnLock
	out= NewIDS
End if 
Response.ContentType = "Text/Plain"
Response.Write out
Response.end

Function GetContainerCount()
	dim RS

	GetInvConnection()
	sql = "SELECT count(*) as count from inv_containers"
	Set RS = Conn.Execute(sql)
	theReturn = RS("count")
	Conn.Close
	Set Conn = nothing
	Set RS = nothing
	GetContainerCount = theReturn
end function

</SCRIPT>
