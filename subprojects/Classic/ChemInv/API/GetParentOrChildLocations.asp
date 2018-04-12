<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPring
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:GetParentLocations:<BR>"
LocationID = Request("LocationID")
Direction = lcase(Request ("Direction")) ' parent or child
if IsEmpty(Direction) then  Direction = "parent"

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetParentLocations.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

GetInvConnection()
if Direction = "parent" Then
    SQL="SELECT location_id " & _
                       " FROM INV_LOCATIONS  " & _
                        " start with location_id=" & LocationID & " CONNECT BY PRIOR PARENT_ID = LOCATION_ID" 
else
    SQL="SELECT location_id " & _
                       " FROM INV_LOCATIONS  " & _
                        " start with location_id=" & LocationID & " CONNECT BY PRIOR LOCATION_ID = PARENT_ID" 
end if

Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 5, 1, 0, LocationID)
Set rsParentLocation = Cmd.Execute

While not (rsParentLocation.BOF or rsParentLocation.EOF)
    if cint(LocationID) <> cint(rsParentLocation("location_id")) then 
        ParentLocationList = ParentLocationList & rsParentLocation("location_id") & ","
    end if
	rsParentLocation.MoveNext
wend


rsParentLocation.Close
Set rsParentLocation = nothing
Conn.Close
Set Conn = nothing
if ParentLocationList = "" then 
  Response.Write ""
else 
  Response.Write mid(ParentLocationList,1,len(ParentLocationList)-1)
end if
Response.End
</SCRIPT>
