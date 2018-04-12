<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->


<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:GetLocationFromID<BR>"
LocationID = Request("LocationID")
TargetType = Request("TargetType")
TargetTypeID = Request("TargetTypeID")


CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")
'Response.Write CsUserName & CsUserID
'Response.End
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	'Response.Redirect "/cheminv/help/admin/api/GetLocationFromID.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if

If IsEmpty(TargetTypeID) then
	 strError = strError & "ContainerID/PlateID/LocationTypeID is a required parameter <BR>"
	 bWriteError = True
End if


If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

If IsEmpty(TargetType) then
	TargetType=NULL
else
TargetType=lcase(TargetType)
End if


Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Authority.isValidLocationType", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pLocationID",131, 1, 0, LocationID)
Cmd.Parameters("pLocationID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pTargetType",200, 1, 4000, TargetType)
Cmd.Parameters.Append Cmd.CreateParameter("pTargetTypeID",131, 1, 0, TargetTypeID)
Cmd.Parameters("pTargetTypeID").NumericScale = 8
Cmd.Parameters("pTargetTypeID").Precision = 0

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Authority.isValidLocationType")
End if

'Response.Write strSQL
'Response.end
out = CStr(Cmd.Parameters("RETURN_VALUE"))


' Return success	
Response.Write out 

</SCRIPT>
