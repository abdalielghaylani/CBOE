<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:InvLoaderSQL<BR>"
StatementID = Request("StatementID")
Params = Request("Params")

UserID = Ucase(Request("UserID")) ' to be used for Group Security
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	' Response.Redirect "/cheminv/help/admin/api/InvLoaderSQL.htm"
	' Response.end
End if

' Check for required parameters
If IsEmpty(StatementID) then
	strError = strError & "StatementID is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

StatementID = CInt(StatementID)

'********************************************************************************************
'* This is the InvLoader enumeration for reference
'*		public enum StatementIds
'*		{
'*			// explicit numbering for easier matching with InvLoaderSQL.asp page
'*			NumberOfCompoundWells = 0,
'*			PlateLocations = 1,
'*			PlateFormats = 2,
'*			PlateFormatFull = 3,
'*			PlateTypes = 4,
'*			Libraries = 5,
'*			BarcodeDescs = 6,
'*			BarcodePrefixes = 7,
'*			PlateDimensions = 8,
'*			WellFormats = 9,
'*			ExistingPlates = 10,
'*			RegID = 11,
'*			ContainerLocations = 13,
'*			ContainerTypes = 14,
'*			ContainerStatus = 15,
'*			Units = 16,
'*			Owners = 17
'*          GroupSecurity = 18
'*		}
'********************************************************************************************


SELECT CASE (StatementID)
	case 0	
		' this is a command, handled below
	case 1
		if Application("ENABLE_OWNERSHIP")="TRUE" then
            strSQL = "select * from inv_vw_plate_locations where " & Application("CHEMINV_USERNAME") & ".Authority.LocationIsAuthorized(Location_ID, '" & UserID & "') = 1"
        else
             strSQL = "select * from inv_vw_plate_locations" 
        end if
	case 2
		strSQL = "Select plate_format_id from inv_plate_format"
	Case 3
		strSQL = "select plate_format_id, plate_format_name from inv_plate_format"
	Case 4
		strSQL = "select plate_type_id, plate_type_name from inv_plate_types"
	Case 5
		strSQL ="select enum_id, enum_value from inv_enumeration where eset_id_fk = 5"
	case 6
		strSQL = "select barcode_desc_id, barcode_desc_name from inv_barcode_desc"
	case 7
		strSQL = "select barcode_desc_id, prefix from inv_barcode_desc"
	Case 8
		strSQL = "select row_count, col_count from inv_vw_plate_format where plate_format_id = " & Params
	case 9
		strSQL = "Select * from inv_vw_well_format where plate_format_id_fk = " & Params
	case 10
		strSQL = "SELECT plate_barcode FROM inv_plates WHERE plate_barcode IN (" & Params & ")"
	case 11
		strSQL = "SELECT reg_id FROM reg_numbers WHERE reg_number = '" & Params & "'"
	case 12
		strSQL = "SELECT unit_id FROM inv_units WHERE lower(unit_name) = 'micromolar'"
	case 13
		'strSQL = "SELECT inv_locations.*, location_name || ' (' || location_barcode || ')' as location_name_barcode FROM inv_locations WHERE parent_id IS NOT NULL ORDER BY location_name"
		strSQL = "SELECT inv_locations.*, location_name || ' (' || location_barcode || ')' as location_name_barcode "
        strSQL = strSQL & "FROM inv_locations "
        strSQL = strSQL & "WHERE parent_id IS NOT NULL "
        strSQL = strSQL & "AND RACKS.isRack(inv_locations.location_id) = 0 "
        strSQL = strSQL & "AND RACKS.isRackLocation(inv_locations.location_id) = 0 "
        strSQL = strSQL & "ORDER BY location_name"
	case 14
		strSQL = "SELECT * FROM inv_container_types"
	case 15
		strSQL = "SELECT * FROM inv_container_status"	
	case 16
		strSQL = "SELECT UNIT_ID AS Value, Unit_Name ||' ('||Unit_Abreviation||')' AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (" & Params & ") ORDER BY lower(DisplayText) ASC"
	case 17
		strSQL = "SELECT owner_ID AS Value, description AS DisplayText FROM inv_owners ORDER BY lower(description) ASC"
	CASE ELSE
	
END SELECT

GetInvConnection()

Select Case (StatementID)
	Case 0 ' command for number of compound wells
		' Set up an ADO command
		Dim Cmd, s
		Set Cmd = Server.CreateObject("ADODB.Command")
		Set Cmd.ActiveConnection = Conn
		Cmd.CommandText = "GETNUMBEROFCOMPOUNDWELLS"
		Cmd.CommandType = adCmdStoredProc
		With Cmd.Parameters
			s = "RETURN_VALUE"
			.Append Cmd.CreateParameter(s, adNumeric, adParamReturnValue, 0, Null)
			Cmd.Parameters(s).Precision = 9
			s = "p_plate_format_id"
			.Append Cmd.CreateParameter(s, adNumeric, adParamInput, 0, Params)
			Cmd.Parameters(s).Precision = 9
		End With
		On Error Resume Next
		Cmd.Execute
		If Err then
			out =  Cmd.CommandText & ": " & Err.description	
		else
			out = Cmd.Parameters("RETURN_VALUE")
		End if
		' Return success	
		Response.Write out
   	Case Else ' use sql statement
		' Initialize objects:
		Set rs = Server.CreateObject("ADODB.Recordset")
		
		' Open Recordset and save to a stream.	
		rs.CursorLocation = adUseClient		
     	rs.Open strSql, Conn, adOpenForwardOnly, adLockReadOnly
     	
     	Response.ContentType = "text/xml"
		' response.Write "<?xml version=""1.0"" encoding=""iso-8859-1""?>" & vbcrlf 
		'Response.Write "<?xml:stylesheet type=""text/xsl"" href=""recordsetxml.xsl""?>" & vbCrLf
		rs.Save Response, adPersistXML		
		
		' Cleanup:
		rs.Close
		Set rs = Nothing
end select

 %>
