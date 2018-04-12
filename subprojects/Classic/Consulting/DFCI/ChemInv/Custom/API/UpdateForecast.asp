<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim location
Dim product
Dim forecasttype
dim assay
dim period
dim repetitions
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = false
bWriteError = true
strError = "Error:UpdatePlate<BR>"


'Required Paramenters
location = Request("location")
product = Request("product")
forecasttype=request("forecasttype")
assay=request("assay")
period=request("period")
repetitions=request("repetitions")

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".InsertUpdateForecast", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("location", 200, 1, 25, location)
Cmd.Parameters.Append Cmd.CreateParameter("ndc", 200,1, 25,NDC)
Cmd.Parameters.Append Cmd.CreateParameter("amount", 3,,, Amount)


if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".insertUpdateForecast")
End if

' Return the Update Status
out = "1"
response.write "hello"
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
