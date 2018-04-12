<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetReorderContainer_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug


Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:ReorderContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ReorderContainer.htm"
	Response.end
End if

'Required Paramenters
DeliveryLocationID = Request("DeliveryLocationID")
ContainerID = Request("ContainerID")
ContainerName = Request("ContainerName")
Comments = Request("Comments")
OwnerID = Request("OwnerID")
CurrentUserID = Request("CurrentUserID")
NumCopies = Request("NumCopies")
Project = Request("Project")
Job = Request("Job")
RushOrder = Request("RushOrder")
DueDate = Request("DueDate")
ReorderReason = Request("ReorderReason")
ReorderReasonOther = Request("ReorderReasonOther")

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ReorderContainer", adCmdStoredProc)
Call GetReorderContainer_cmd()

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ReorderContainer")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close
Set paramR = Nothing
Set Cmd = Nothing
Set Conn = Nothing

'Response.Write "out = " & out & "<P>"
'Response.Write "NewIDs = " & NewIDs
'Response.End

' Increment container count
If out > 0 then
	tempArr = split(NewIDS,"|")
	numContainers = Ubound(tempArr) + 1
	containerCount = CLng(Application("inv_Containers" & "RecordCount" & "ChemInv")) + numContainers 
	Application.Lock
		Application("inv_Containers" & "RecordCount" & "ChemInv") = containerCount
	Application.UnLock
	out= NewIDS
End if 
Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
