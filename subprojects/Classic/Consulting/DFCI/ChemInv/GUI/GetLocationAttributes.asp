<%
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
'****************************************************************************************
'*	PURPOSE: fetch all Location attributes for the current location
'*	INPUT: GetData querystring parameter DB or New, defaults to new
'*	OUTPUT: Populates local variables with location attributes
'****************************************************************************************
Dim Conn
Dim RS

' Receive Posted data
if Request("LocationID") <> "" and Request("LocationID") > 0 then
	LocationID = Request("LocationID")
else
	LocationID = Session("CurrentLocationID")
end if

If isEmpty(LocationID) then
	Response.Write "<BR><BR><BR><BR><BR><center><P><CODE>No location selected</CODE></P></center>"
	Response.Write "<center><SPAN class=""GuiFeedback"">Close this window and select a location from the tree</SPAN></center>"
	Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"
	Response.end
End if
GetData = Request.QueryString("GetData")
If GetData = "" then GetData = "new"

// the default barcode description is 2 = Location Barcodes
BarcodeDescID = 2

if lCase(GetData)= "db" then
	isEdit= true
	Headertxt = "edit"
	Call GetInvConnection()
	SQL = "SELECT Location_Name, Location_Description, Location_Type_ID_FK, Location_Barcode, Owner_ID_FK AS LocationOwnerID, Address_ID_FK FROM inv_Locations WHERE Location_ID=" & LocationID
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	LocationName= RS("Location_Name")
	LocationBarcode= RS("Location_Barcode")
	LocationDesc= RS("Location_Description")
	LocationTypeID= RS("Location_Type_ID_FK")
	LocationOwnerID = RS("LocationOwnerID")
	AddressID = RS("Address_ID_FK")
	action = "EditLocation_action.asp"
Else
	isEdit= False
	Headertxt = "create"
	LocationName= ""
	LocationBarcode= ""
	LocationDesc= ""
	LocationTypeID = 0
	LocationOwnerID = Ucase(Session("UserNameChemInv"))
	AddressID = ""
	action = "NewLocation_action.asp"
End if
Set RS = Nothing
Set Conn = Nothing
%>
