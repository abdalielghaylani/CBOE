<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE
strError = "Error:UpdateAddress<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateAddress.htm"
	Response.end
End if

'Response.Write Request.Form
'Response.End
'Required Paramenters
TableName = Request("TableName")
TablePKID = Request("TablePKID")

' Check for required parameters
if IsEmpty(TableName) OR TableName = "" then
	strError = strError & "TableName is a required parameter.<BR>"
	bWriteError = True
End if
If IsEmpty(TablePKID) or TablePKID = "" then
	strError = strError & "TablePKID is a required parameter.<BR>"
	bWriteError = True
End if
' Respond with Error
if bWriteError then
	Response.Write strError
	Response.end
End if

'optional parameters
AddressID = Request("AddressID")
ContactName = Request("ContactName")
Address1 = Request("Address1")
Address2 = Request("Address2")
Address3 = Request("Address3")
Address4 = Request("Address4")
City = Request("City")
StateIDFK = Request("StateIDFK")
CountryIDFK = Request("CountryIDFK")
ZIP = Request("ZIP")
FAX = Request("FAX")
Phone = Request("Phone")
Email = Request("Email")

action = "edit"
if IsEmpty(AddressID) or AddressID = "" then
	action = "new"
end if
if StateIDFK = "" then StateIDFK = null

'Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdateAddress", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLENAME", 200, 1, 500, TableName)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLEPKID", 200, 1, 500, TablePKID)
Cmd.Parameters.Append Cmd.CreateParameter("PADDRESSID", 200, 1, 5, AddressID)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTACTNAME", 200, 1, 500, ContactName)
Cmd.Parameters.Append Cmd.CreateParameter("PADDRESS1", 200, 1, 500, Address1)
Cmd.Parameters.Append Cmd.CreateParameter("PADDRESS2", 200, 1, 500, Address2)
Cmd.Parameters.Append Cmd.CreateParameter("PADDRESS3", 200, 1, 500, Address3)
Cmd.Parameters.Append Cmd.CreateParameter("PADDRESS4", 200, 1, 500, Address4)
Cmd.Parameters.Append Cmd.CreateParameter("PCITY", 200, 1, 500, City)
Cmd.Parameters.Append Cmd.CreateParameter("PSTATEIDFK", 3, 1, 0, StateIDFK)
Cmd.Parameters.Append Cmd.CreateParameter("PCOUNTRYIDFK", 3, 1, 0, CountryIDFK )
Cmd.Parameters.Append Cmd.CreateParameter("PZIP", 200, 1, 500, ZIP)
Cmd.Parameters.Append Cmd.CreateParameter("PFAX", 200, 1, 500, FAX)
Cmd.Parameters.Append Cmd.CreateParameter("PPHONE", 200, 1, 500, Phone)
Cmd.Parameters.Append Cmd.CreateParameter("PEMAIL", 200, 1, 500, Email)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdateAddress")
End if

' Return the Update Status
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
