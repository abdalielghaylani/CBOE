<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = False
bWriteError = False
strError = "Error:AddReformatMap<BR>"
ServerName = Request.ServerVariables("Server_Name")

MapXML = Request("MapXML")
MapName = Request("MapName")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/AddReformatMap.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(MapXML) then
	strError = strError & "MapXML is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(MapName) then
	strError = strError & "MapName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.End
end if

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Reformat.InsertReformatMap", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 5, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pName", adVarChar, 1, 200, MapName)
Cmd.Properties("SPPrmsLOB") = TRUE
Cmd.Parameters.Append Cmd.CreateParameter("pPlateXML", AdLongVarChar, 1, 300000, MapXML)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Reformat.InsertReformatMap")
End if

' Return the newly created XMLDocIDs
Response.Write Cmd.Parameters("RETURN_VALUE")
Cmd.Properties("SPPrmsLOB") = FALSE

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing

Response.End

</SCRIPT>
