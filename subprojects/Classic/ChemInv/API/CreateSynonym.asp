<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:CreateSynonym<BR>"

CompoundID = Request("CompoundID")
SubstanceName = Request("SubstanceName")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateSynonym.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(CompoundID) then
	strError = strError & "CompoundID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SubstanceName) then
	strError = strError & "SubstanceName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Compounds.CreateSynonym", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PCOMPOUNDID",adNumeric, adParamInput, , CompoundID) 
Cmd.Parameters("PCOMPOUNDID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PSUBSTANCENAME",200, adParamInput, 255, SubstanceName)	
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Compounds.CreateSynonym")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
