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
strError = "Error:CreatePlateType<BR>"

PlateTypeName = Request("PlateTypeName")
MaxFreezeThaw = Request("MaxFreezeThaw")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreatePlateType.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(PlateTypeName) then
	strError = strError & "Plate Type Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(MaxFreezeThaw) then
	strError = strError & "MaxFreezeThaw is a required parameter<BR>"
	bWriteError = True
End if


' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePlateType", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pPlateTypeName",200, 1, 50, PlateTypeName)
Cmd.Parameters.Append Cmd.CreateParameter("pMaxFreezeThaw",131, 1, 0, MaxFreezeThaw)
Cmd.Parameters("pMaxFreezeThaw").Precision = 5
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePlateType")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
