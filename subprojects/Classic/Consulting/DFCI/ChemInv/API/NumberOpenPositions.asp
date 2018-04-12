<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script runat="Server" language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

'-- this api checks to see how many open rack positions there are for a given list of location ids

bDebugPrint = False
bWriteError = False
strError = "Error:NumberOpenPositions<BR>"
locationIds = Request("locationIds")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/NumberOpenPositions.htm"
	Response.end
End if

'-- Check for required parameters
If IsEmpty(locationIds) then
	strError = strError & "locationIds is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	'-- Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Racks.multiOpenPositionCount", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 131, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONIDS", 200, 1, 2000, locationIds)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Racks.multiOpenPositionCount")
end if
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing
Set Cmd = nothing
Set RS = nothing

Response.Write out

</script>
