<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

'-- this api checks to see if a given compoundID exists in the inv_compounds table

bDebugPrint = False
bWriteError = False
strError = "Error:IsValidCompoundID<BR>"
compoundID = Request("compoundID")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/IsValidCompoundID.htm"
	Response.end
End if

'-- Check for required parameters
If IsEmpty(compoundID) then
	strError = strError & "CompoundID is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	'-- Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
SQL = "select 1 from inv_compounds where compound_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("compoundID", 5, 1, 0, compoundID)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Set RS = Cmd.Execute
	if RS.EOF then
	    out = "0"
	else
	    out = "1"
	end if
end if


Conn.Close
Set Conn = nothing
Set Cmd = nothing
Set RS = nothing

Response.Write out

</SCRIPT>
