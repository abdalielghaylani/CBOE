<%
Sub GetCopyContainer_cmd()
	' Check for required parameters
	If IsEmpty(ContainerID) then
		strError = strError & "ContainerID is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(NumCopies) then
		strError = strError & "Number of Copies is a required parameter<BR>"
		bWriteError = True
	End if
	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
	Cmd.Parameters("RETURN_VALUE").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERID", 200, 1, 2000, ContainerID) 	
	Cmd.Parameters.Append Cmd.CreateParameter("PNUMCOPIES", 5, 1, 0, NumCopies)
	Cmd.Parameters.Append Cmd.CreateParameter("PNEWIDS",200, 2, 4000, NULL)
End Sub

%>