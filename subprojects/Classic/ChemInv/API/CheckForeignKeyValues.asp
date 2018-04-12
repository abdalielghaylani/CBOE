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
strError = "Error:CheckForeignKeyValues<BR>"
TableName = Request("TableName")
TableValues = Request("TableValues")
ColumnName = Request("Fieldname")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CheckForeignKeyValues.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(TableName) then
	strError = strError & "TableName is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(TableValues) then
	strError = strError & "TableValues is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ColumnName) then
	strError = strError & "ColumnName is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CHECKFOREIGNKEYVALUES", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 16384, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLENAME", adVarChar, 1, Len(TableName), TableName)
Cmd.Parameters.Append Cmd.CreateParameter("pcolumnname", adVarChar, 1, Len(Columnname), ColumnName)

Cmd.Parameters.Append Cmd.CreateParameter("PTABLEVALUES", adVarChar, 1, Len(TableValues), TableValues)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CHECKFOREIGNKEYVALUES")
end if
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing

Response.Write out

</SCRIPT>
