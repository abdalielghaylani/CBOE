<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:IsPlateTypeAllowed<BR>"
LocationID = Request("LocationID")
PlateTypeID = Request("PlateTypeID")
IsPlateMap = Request("IsPlateMap")
PlateIDs =  Request("plateID")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

if PlateTypeID="" then PlateTypeID=NULL
if PlateIDs="" then PlateIDs=NULL
if IsPlateMap="" then IsPlateMap=NULL


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/IsPlateTypeAllowed.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PlateTypeID) and IsEmpty(plateIDs) then
	strError = strError & "Either PlateID or PlateTypeID is required <BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if
    Call GetInvConnection()
    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".IsPlateTypeAllowed", adCmdStoredProc)
    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 5, adParamReturnValue, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("pPlateID",200, 1, 4000, PlateIDs)
    Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID", 200, 1, 4000, LocationID)
    Cmd.Parameters.Append Cmd.CreateParameter("PPLATETYPEID", 5, 1, 0, PlateTypeID)
    Cmd.Parameters.Append Cmd.CreateParameter("PISPLATEMAP", 5, 1, 0, IsPlateMap)
    if bDebugPrint then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = " & p.value & "<BR>"
	    Next
	    Response.End	
    Else
	    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".IsPlateTypeAllowed")
    end if
    out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))
Conn.Close
Set Conn = nothing

Response.Write out

</SCRIPT>
