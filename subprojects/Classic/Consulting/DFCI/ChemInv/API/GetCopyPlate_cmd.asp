<%
Sub GetCopyPlate_cmd()
	' Check for required parameters
	If IsEmpty(PlateID) then
		strError = strError & "PlateID is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(NumCopies) then
		strError = strError & "NumCopies is a required parameter<BR>"
		bWriteError = True
	End if
	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if
	'optional parameters
	if isEmpty(BarcodeDescID) or BarcodeDescID = "" then 
		BarcodeDescID = null
	end if
	
	if isEmpty(CopyType) or CopyType = "" then
	    CopyType = 0    ' Default to a "light" copy
	end if

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters("RETURN_VALUE").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PLATEID", 5, 1, 0, PlateID) 	
	Cmd.Parameters.Append Cmd.CreateParameter("PNUMCOPIES", 5, 1, 0, NumCopies)
	Cmd.Parameters.Append Cmd.CreateParameter("PBARCODEDESCID", 5, 1, 0, BarcodeDescID)
	Cmd.Parameters.Append Cmd.CreateParameter("PCOPYTYPE", 5, 1, 0, CopyType)
	Cmd.Parameters.Append Cmd.CreateParameter("PNEWIDS",200, 2, 4000, NULL)
End Sub

%>