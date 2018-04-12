<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:Update Par Level<BR>"

CsUserName =Request("tempCsUserName") 
CsUserID = URLDecode(CryptVBS(request("tempCsUserID"),"protocolpi"))

ProtocolID = Request("ProtocolID")
ProtocolPIID = Request("ProtocolPIID")
PI = request("PI")
PINCINUM = request("PINCINUM")
StartDate = Request("StartDate")
EndDate = Request("EndDate")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
'	Response.Redirect "/cheminv/help/admin/api/CreateSynonym.htm"
	Response.end
End if


'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(ProtocolID)  then
	strError = strError & "Protocol ID is a required parameter<BR>"
	bWriteError = True
End if

	if StartDate = "" then
		StartDate = NULL
	Elseif IsDate(StartDate) then
		StartDate = CDate(StartDate)
	Else
		strError = strError & "StartDate could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if


	if EndDate = "" then
		EndDate = NULL
	Elseif IsDate(EndDate) then
		EndDate = CDate(EndDate)
	Else
		strError = strError & "EndDate could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if


If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CreateOrUpdateProtocolPI", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PPROTOCOLID",adNumeric, adParamInput, , PROTOCOLID) 
Cmd.Parameters("PPROTOCOLID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PPROTOCOLPIID",adNumeric, adParamInput, , PROTOCOLPIID) 
Cmd.Parameters("PPROTOCOLPIID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PPI",200, adParamInput, 255, PI)
Cmd.Parameters.Append Cmd.CreateParameter("PPINCINUM",200, adParamInput, 100, PINCINUM)
Cmd.Parameters.Append Cmd.CreateParameter("Pstartdate",135, 1, 0, startdate)
Cmd.Parameters.Append Cmd.CreateParameter("Penddate",135, 1, 0, enddate)	

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CREATEORUPDATEPROTOCOLPI")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing

'Function to Decode the encrypted string 
Function URLDecode(str) 
	str = Replace(str, "+", " ") 
    For i = 1 To Len(str) 
		sT = Mid(str, i, 1) 
        If sT = "%" Then 
			If i+2 < Len(str) Then 
				sR = sR & _ 
                Chr(CLng("&H" & Mid(str, i+1, 2))) 
                i = i+2 
            End If 
        Else 
			sR = sR & sT 
        End If 
   Next 
   URLDecode = sR 
End Function 
</SCRIPT>
