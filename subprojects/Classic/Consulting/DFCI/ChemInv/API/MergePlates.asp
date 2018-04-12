<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:Merge Plates<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiMergePlates.htm"
	Response.end
End if

'Required Paramenters
MergePlateIDs = Request("AllMergePlateIDs")
PlateTypeID = Request("PlateType")
NumPlates = Request("NumPlates")

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".MERGEPLATES", adCmdStoredProc)

' Check for required parameters
If IsEmpty(MergePlateIDs) then
	strError = strError & "Merge Plate IDs is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PlateTypeID) then
	strError = strError & "Plate Type is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(NumPlates) then
	strError = strError & "Number of Plates is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PMERGEPLATEIDS", 200, 1, 2000, MergePlateIDs) 	
Cmd.Parameters.Append Cmd.CreateParameter("PPLATETYPEID", 5, 1, 0, PlateTypeID)
Cmd.Parameters.Append Cmd.CreateParameter("PNUMPLATES", 5, 1, 0, NumPlates)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".MERGEPLATES")
End if

' Return the newly created PlateIDs
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
'NewIDs =  Cmd.Parameters("PNEWIDS")
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
