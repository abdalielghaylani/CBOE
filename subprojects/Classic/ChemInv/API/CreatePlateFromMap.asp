<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetCopyPlate_cmd.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/GetUpdatePlate_cmd.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim PlateIDs
Dim ValuePairs
Dim strError
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:CreatePlateFromMap<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreatePlateFromMap.htm"
	Response.end
End if

' Create new plates by copying the Plate maps then update the new plate with the entered attributes

'Required Paramenters
PlateIDs = Request("PlateIDs")
PlateID = PlateIDs
ValuePairs = Request("ValuePairs")
NumCopies = Request("NumCopies")
BarcodeDescID = Request("BarcodeDescID")
CopyType = Request("CopyType")
out = ""

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CopyPlate", adCmdStoredProc)
Call GetCopyPlate_cmd()
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CopyPlate")
End if

' Get the newly created PlateIDs
PlateIDsOut = Cmd.Parameters("RETURN_VALUE")
PlateIDs = replace(PlateIDsOut,"|",",")

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes", adCmdStoredProc)
Call GetUpdatePlate_cmd()
Call CheckUpdatePlateCmdParameters()
if bDebugPrint then
    For each p in Cmd.Parameters
	    Response.Write p.name & " = " & p.value & "<BR>"
    Next	
Else
    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdatePlateAttributes")
End if    

out = out & PlateIDsOut & ","
out =  left(out,len(out)-1)

Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>