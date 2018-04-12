<%
Sub CheckUpdatePlateCmdParameters()
	' Check for required parameters
	If IsEmpty(PlateIDs) then
		strError = strError & "PlateIDs is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(ValuePairs) then
		strError = strError & "ValuePairs is a required parameter<BR>"
		bWriteError = True
	End if
	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if
End Sub
Sub GetUpdatePlate_cmd()
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERIDS", 200, 1, 4000, PlateIDs)
	Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 4000, ValuePairs)
End Sub

%>