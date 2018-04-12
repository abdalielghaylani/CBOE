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
strError = "Error:CreatePlateFormat<BR>"

PlateFormatName = Request("pPlateFormatName")
PhysPlateID = Request("pPhysPlateID")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreatePlateFormat.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(PlateFormatName) then
	strError = strError & "Plate Format Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PhysPlateID) then
	strError = strError & "PhysPlateID is a required parameter<BR>"
	bWriteError = True
End if


' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePlateFormat", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pPlateFormatName",200, 1, 50, PlateFormatName)
Cmd.Parameters.Append Cmd.CreateParameter("pPhysPlateIDFK",131, 1, 0, PhysPlateID)
Cmd.Parameters("pPhysPlateIDFK").Precision = 5
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".PLATESETTINGS.CreatePlateFormat")
End if
PlateFormatID = Cmd.Parameters("RETURN_VALUE")

' Return the newly created PlateFormatID
Response.Write PlateFormatID

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
