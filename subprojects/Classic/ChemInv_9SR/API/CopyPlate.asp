<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetCopyPlate_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:CopyPlate<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCopyPlate.htm"
	Response.end
End if

'Required Paramenters
PlateID = Request("PlateID")
NumCopies = Request("NumContainers")

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CopyPlate", adCmdStoredProc)
Call GetCopyPlate_cmd()
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CopyPlate")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close
Set Cmd = Nothing
Set Conn = Nothing
' Increment container count
'If out > 0 then
'	tempArr = split(NewIDS,"|")
'	numContainers = Ubound(tempArr) + 1
'	containerCount = CLng(Application("inv_Containers" & "RecordCount" & "ChemInv")) + numContainers 
'	Application.Lock
'		Application("inv_Containers" & "RecordCount" & "ChemInv") = containerCount
'	Application.UnLock
'	out= NewIDS
'End if 
Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
