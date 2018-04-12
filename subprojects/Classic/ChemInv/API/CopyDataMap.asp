<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:CopyDataMap<BR>"

dataMapID = Request("dataMapID")
dataMapName = Request("TemplateName")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CopyDataMap.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(dataMapID) then
	strError = strError & "Data Map ID is a required parameter<BR>"
	bWriteError = True
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".DATAMAPS.CopyDataMap", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 16
Cmd.Parameters.Append Cmd.CreateParameter("pDataMapID",131, 1, 0, dataMapID)
Cmd.Parameters.Append Cmd.CreateParameter("pName",200 , 1, 50, dataMapName)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".DATAMAPS.CopyDataMap")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
