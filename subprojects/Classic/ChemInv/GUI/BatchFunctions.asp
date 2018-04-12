<SCRIPT RUNAT="Server" Language="VbScript">

Function GetBatchFieldMap(byref Conn)
    '-- update batch fields dict which stores the batching fields and their display name
	Dim RS
	
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = "{CALL " & Application("CHEMINV_USERNAME") & ".BATCH.GETBATCHFIELDS()}"
	Cmd.CommandType = adCmdText

    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set RS = Cmd.Execute
    Cmd.Properties ("PLSQLRSet") = FALSE
    dim isREG_ID, isBATCH_NUMBER
    isREG_ID=0
    isBATCH_NUMBER=0
    
    batching_fields_dict.RemoveAll
    while not RS.EOF
        batching_fields_dict.add cstr(RS("sort_order"))&"_"&cstr(RS("BATCH_TYPE_ID_FK")), cstr(RS("display_name"))
        if Rs("field_name")="REG_ID_FK" then isREG_ID=1
        if Rs("field_name")="BATCH_NUMBER_FK" then isBATCH_NUMBER=1
        RS.MoveNext
    wend
    Application("isRegBatch")=0
    if (isREG_ID=1) and (isBATCH_NUMBER=1) then Application("isRegBatch")=1
    'batching_fields_dict.add cstr(RS("sort_order")), RS("display_name")
	field_count = reg_fields_dict.count + custom_batch_property_fields_dict.Count + batching_fields_dict.Count
	Dim fieldMapArray()
	ReDim fieldMapArray(field_count+7, 2)
	
	' Reg Field Names
	fieldMapArray(0,0) = "BATCHID" 
	fieldMapArray(1,0) = "AmountRemaining" 
	fieldMapArray(6,0) = "Batch_Field_3"
	fieldMapArray(2,0) = "AmountAvailable" 
	fieldMapArray(3,0) = "AmountReserved" 
	fieldMapArray(4,0) = "Batch_Field_1" 
	fieldMapArray(5,0) = "Batch_Field_2" 
	index = 7
	For each Key in custom_batch_property_fields_dict
		fieldMapArray(index, 0) = cStr(Key)
		index = index + 1
	Next	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			fieldMapArray(index, 0) = cStr(Key)
			index = index + 1
		Next	
	end if
	
	' Column header text
	fieldMapArray(0,1) = "Batch ID"
	fieldMapArray(1,1) = "Amt Remaining"
	fieldMapArray(2,1) = "Amt Available"
	fieldMapArray(3,1) = "Amt Reserved"
	
	'fieldMapArray(5,1) = "Batch Field 1"
	'fieldMapArray(6,1) = "Batch Field 2"
	'fieldMapArray(4,1) = "Batch Field 3"
	index = 4
	
	For each Key1 in batching_fields_dict
		fieldMapArray(index, 1) = cStr(batching_fields_dict.Item(key1))
		index = index + 1
	Next	
	
	For each Key in custom_batch_property_fields_dict
		fieldMapArray(index, 1) = cStr(custom_batch_property_fields_dict.Item(key))
		index = index + 1
	Next	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			fieldMapArray(index, 1) = cStr(reg_fields_dict.Item(key))
			index = index + 1
		Next	
	end if
	' Default column widths	
	fieldMapArray(0,2) = 25
	fieldMapArray(1,2) = 12
	fieldMapArray(2,2) = 16
	fieldMapArray(3,2) = 16
	fieldMapArray(4,2) = 16
	'fieldMapArray(5,2) = 16
	'fieldMapArray(6,2) = 16
	'index = 7
	
	index = 5
	
	For each Key1 in batching_fields_dict
		fieldMapArray(index, 2) = 15
		index = index + 1
	Next	
	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			fieldMapArray(index, 2) = 15
			index = index + 1
		Next	
	end if
	'Change the following two lines to affect the default columns and column widths
	'for the screen reports. 
	'Use a pipe delimited list of ColumnID:ColumnWidth values
	'ColumnID is the first index from fieldMapArray above
	'ColumnWidth is expressed in characters 
	'The columns originally shown are: Batch ID, QtyAvailable, Substance, Reg Number, Batch Number
	
	Application("BatchReportColDef1") = "1:15|3:16|4:16|5:15|6:12"
	GetBatchFieldMap = fieldMapArray

End Function


Function GetRequestFieldMap()
	field_count = reg_fields_dict.count + custom_createrequest_fields_dict.Count + custom_fulfillrequest_fields_dict.Count
	Dim fieldMapArray()
	ReDim fieldMapArray(field_count+14, 2)
	
	' Reg Field Names
	fieldMapArray(0,0) = "BatchField1"
	fieldMapArray(1,0) = "BatchField2"
	fieldMapArray(2,0) = "BatchField3"
	fieldMapArray(3,0) = "RequestedBy"
	fieldMapArray(4,0) = "DeliveryLocation" 
	fieldMapArray(5,0) = "BatchAmount" 
	fieldMapArray(6,0) = "AmountRequested" 
	fieldMapArray(7,0) = "DateRequested" 
	fieldMapArray(8,0) = "DateRequired" 
	fieldMapArray(9,0) = "DeclineReason" 
	fieldMapArray(10,0) = "AssignedUser" 
	fieldMapArray(11,0) = "AmountDelivered" 
	fieldMapArray(12,0) = "BatchID" 
	fieldMapArray(13,0) = "ShipToName" 
'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Adding the Comments field to the column picker
'-- Date: 13/04/2010
	fieldMapArray(14,0) = "Request_Comments" 
	index = 15
'-- End of Change #123488#
	For each Key in custom_createrequest_fields_dict
		fieldMapArray(index, 0) = "Request" & cStr(Key)
		index = index + 1
	Next	
	For each Key in custom_fulfillrequest_fields_dict
		fieldMapArray(index, 0) = "Request" & cStr(Key)
		index = index + 1
	Next	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 0) = "Request" & cStr(Key)
			index = index + 1
			end if
		Next	
	end if
	' Column header text
    '-- update batch fields display names from the dict if it has any
	index = 0
	batchingFieldItems = batching_fields_dict.Items
	if batching_fields_dict.count > 0 then
		fieldMapArray(index, 1) = cStr(batchingFieldItems(index))
	else
		fieldMapArray(index, 1) = "Batch Field " & index + 1
	end if
	index = index + 1
	if batching_fields_dict.count > 1 then
		fieldMapArray(index, 1) = cStr(batchingFieldItems(index))
	else
		fieldMapArray(index, 1) = "Batch Field " & index + 1
	end if
	index = index + 1
	if batching_fields_dict.count > 2 then
		fieldMapArray(index, 1) = cStr(batchingFieldItems(index))
	else
		fieldMapArray(index, 1) = "Batch Field " & index + 1
	end if
	fieldMapArray(3,1) = "Requested By"
	fieldMapArray(4,1) = "Delivery Location"
	fieldMapArray(5,1) = "Batch Amount"
	fieldMapArray(6,1) = "Amount Required"
	fieldMapArray(7,1) = "Date Requested"
	fieldMapArray(8,1) = "Date Required"
	fieldMapArray(9,1) = "Cancelled Reason"
	fieldMapArray(10,1) = "Assigned User"
	fieldMapArray(11,1) = "Amount Delivered"
	fieldMapArray(12,1) = "Batch ID"
	fieldMapArray(13,1) = "Requested For"
'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Adding the Comments field to the column picker
'-- Date: 13/04/2010
	fieldMapArray(14,1) = "Request Comments" 
	index = 15
'-- End of Change #123488#
	For each Key in custom_createrequest_fields_dict
		fieldMapArray(index, 1) = cStr(custom_createrequest_fields_dict.Item(key))
		index = index + 1
	Next	
	For each Key in custom_fulfillrequest_fields_dict
		fieldMapArray(index, 1) = cStr(custom_fulfillrequest_fields_dict.Item(key))
		index = index + 1
	Next	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 1) = cStr(reg_fields_dict.Item(key))
			index = index + 1
			end if
		Next	
	end if
	' Default column widths	
	fieldMapArray(0,2) = 12
	fieldMapArray(1,2) = 12
	fieldMapArray(2,2) = 12
	fieldMapArray(3,2) = 16
	fieldMapArray(4,2) = 16
	fieldMapArray(5,2) = 16
	fieldMapArray(6,2) = 16
	fieldMapArray(7,2) = 16
	fieldMapArray(8,2) = 16
	fieldMapArray(9,2) = 20
	fieldMapArray(10,2) = 20
	fieldMapArray(11,2) = 15
	fieldMapArray(12,2) = 15
	fieldMapArray(13,2) = 16
'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Setting the default column width for the Comments field
'-- Date: 13/04/2010
	fieldMapArray(14,2) = 15 
	index = 15
'-- End of Change #123488#
	For each Key in custom_createrequest_fields_dict
		fieldMapArray(index, 2) = 15
		index = index + 1
	Next	
	For each Key in custom_fulfillrequest_fields_dict
		fieldMapArray(index, 2) = 15
		index = index + 1
	Next	
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 2) = 15
			index = index + 1
			end if
		Next	
	end if
	'Change the following two lines to affect the default columns and column widths
	'for the screen reports. 
	'Use a pipe delimited list of ColumnID:ColumnWidth values
	'ColumnID is the first index from fieldMapArray above
	'ColumnWidth is expressed in characters 
	'The columns originally shown are: Batch ID, QtyAvailable, Substance, Reg Number, Batch Number

	Application("RequestReportColDef1") = "0:15|1:15|5:15|3:12|10:15|8:15"
	Application("RequestReportColDef2") = "0:15|1:15|5:15|4:15|3:12|10:15|8:15"
	Application("RequestReportColDef3") = "0:15|1:15|5:15|3:12|10:15|8:15"
	Application("RequestReportColDef4") = "0:15|1:15|5:15|3:12|10:15|8:15"
	Application("RequestReportColDef5") = "0:15|1:15|5:15|3:12|10:15|8:15"
	Application("RequestReportColDef6") = "0:15|1:15|5:15|3:12|10:15|11:15|8:15"
	Application("RequestReportColDef7") = "0:15|1:15|5:15|3:12|10:15|8:15|9:20"
	Application("RequestReportColDef8") = "0:15|1:15|5:15|3:12|10:15|8:15"
	Application("RequestReportColDef9") = "0:15|1:15|5:15|3:12|10:15|8:15"
	'SM Fix for CSBR-72222
	Application("RequestReportColDef10") = "3:60|8:30|6:25"

	GetRequestFieldMap = fieldMapArray

	'-- Request status
	'1	Unknown
	'2	New
	'3	Approved
	'4	Declined
	'5	Filled
	'6	Closed
	'7	Cancelled
	'8	Pending
	'9	Reserved
	'10 My Requests
	
End Function


Function GetReservationFieldMap()

	field_count = reg_fields_dict.count
	Dim fieldMapArray()
	ReDim fieldMapArray(field_count+8, 2)
	
	' Reg Field Names
	fieldMapArray(0,0) = "RequestedBy"
	fieldMapArray(1,0) = "DeliveryLocation" 
	fieldMapArray(2,0) = "BatchAmount" 
	fieldMapArray(3,0) = "AmountRequested" 
	fieldMapArray(4,0) = "DateRequested" 
	fieldMapArray(5,0) = "DateRequired" 
	fieldMapArray(6,0) = "Organization" 
	fieldMapArray(7,0) = "BatchID" 
	index = 8
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 0) = "Request" & cStr(Key)
			index = index + 1
			end if
		Next	
	end if
	' Column header text
	fieldMapArray(0,1) = "Requested By"
	fieldMapArray(1,1) = "Delivery Location"
	fieldMapArray(2,1) = "Batch Amount"
	fieldMapArray(3,1) = "Amount Required"
	fieldMapArray(4,1) = "Date Requested"
	fieldMapArray(5,1) = "Date Required"
	fieldMapArray(6,1) = "Organization"
	fieldMapArray(7,1) = "Batch ID"
	index = 8
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 1) = cStr(reg_fields_dict.Item(key))
			index = index + 1
			end if
		Next	
	end if
	' Default column widths	
	fieldMapArray(0,2) = 16
	fieldMapArray(1,2) = 16
	fieldMapArray(2,2) = 16
	fieldMapArray(3,2) = 16
	fieldMapArray(4,2) = 16
	fieldMapArray(5,2) = 16
	fieldMapArray(6,2) = 40
	fieldMapArray(7,2) = 16
	index = 8
	if Application("RegServerName") <> "NULL" and Application("isRegBatch") then
		For each Key in reg_fields_dict
			if key <> "BASE64_CDX" then
			fieldMapArray(index, 2) = 15
			index = index + 1
			end if
		Next	
	end if
	'Change the following two lines to affect the default columns and column widths
	'for the screen reports. 
	'Use a pipe delimited list of ColumnID:ColumnWidth values
	'ColumnID is the first index from fieldMapArray above
	'ColumnWidth is expressed in characters 
	'The columns originally shown are: Batch ID, QtyAvailable, Substance, Reg Number, Batch Number

	Application("ReservationReportColDef1") = "3:15|6:120|4:15|5:15|1:15"

	GetReservationFieldMap = fieldMapArray

	
End Function

Function GetNumBatchTypes(byref Conn)
 
    Dim RS
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = "select max(batch_type_id_fk) as count from " & Application("CHEMINV_USERNAME") &".inv_container_batch_fields"
	Cmd.CommandType = adCmdText

    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set RS = Cmd.Execute
    Cmd.Properties ("PLSQLRSet") = FALSE
    dim NumBatchTypes
    NumBatchTypes= 0
    if RS.EOF or RS.BOF Then
        NumBatchTypes= 0
    else
       if RS("count")<>"" then NumBatchTypes= RS("count")
    end if	
    GetNumBatchTypes=NumBatchTypes
End Function
</SCRIPT>
