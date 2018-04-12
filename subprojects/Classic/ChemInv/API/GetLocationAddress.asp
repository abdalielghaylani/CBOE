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
strError = "Error:GetLocationAddress<BR>"
LocationID = Request("LocationID")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetLocationAddress.htm"
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
SQL = "SELECT ADDRESS_ID, CONTACT_NAME, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, CITY, STATE_ID_FK, a.COUNTRY_ID_FK, ZIP, FAX, PHONE, EMAIL, State_Name, State_Abbreviation, Country_Name " & _
			"FROM inv_locations, inv_address a, inv_states, inv_country " & _
			"WHERE address_id_fk = address_id " &_
				"AND a.state_id_fk = state_id(+) " & _
				"AND a.country_id_fk = country_id(+) " & _
				"AND location_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 5, 1, 0, LocationID)
Set rsAddress = Cmd.Execute
'Response.Write Sql
'Response.End

'EmptyAddress = "::::::::::::::::::::::::::::::"
Address = ""
while not (rsAddress.BOF or rsAddress.EOF)
	for each field in rsAddress.Fields
		Address = Address & field & "::"
		'Response.Write field & "<BR>"
	next
	rsAddress.MoveNext
wend


rsAddress.Close
Set rsAddress = nothing
Conn.Close
Set Conn = nothing
Response.Write err.Description

If Address = "" then 
	'Address = EmptyAddress
	Address = ""
else
	Address = left(Address,len(Address)-2)
end if
Response.Write Address

</SCRIPT>
