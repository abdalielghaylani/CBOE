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
strError = "Error:LookUpValue<BR>"
TableName = Request("TableName")
TableValue = Request("TableValue")
InsertIfNotFound = Request("InsertIfNotFound")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/LookUpValue.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(TableName) then
	strError = strError & "TableName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(TableValue) then
	strError = strError & "TableValue is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if
If IsEmpty(InsertIfNotFound) then InsertIfNotFound = "false"

Call GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".LookUpValue", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 50, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLENAME", 200, 1, 100, TableName)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLEVALUE", 200, 1, 100, TableValue)
Cmd.Parameters.Append Cmd.CreateParameter("PINSERTIFNOTFOUND", 200, 1, 10, InsertIfNotFound)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".LookUpValue")
end if
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing

Response.Write out

</SCRIPT>
