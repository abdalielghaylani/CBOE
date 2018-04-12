<%
Sub GetReorderContainer_cmd()
	' Check for required parameters
	If IsEmpty(ContainerID) then
		strError = strError & "ContainerID is a required parameter<BR>"
		bWriteError = True
	End if

	if DueDate = "" then
		strError = strError & "DueDate is a required parameter<BR>"
		bWriteError = True
	Elseif IsDate(DueDate) then
		DateReceived = CDate(DueDate)
	Else
		strError = strError & "Date received could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if

	If Project = "" then
		strError = strError & "Project is a required parameter<BR>"
		bWriteError = True
	End if

	If Job = "" then
		strError = strError & "Job is a required parameter<BR>"
		bWriteError = True
	End if

	If IsEmpty(DeliveryLocationID) then
		strError = strError & "DeliveryLocationID is a required parameter<BR>"
		bWriteError = True
	End if

	' Optional parameters
	if ContainerName = "" then ContainerName = NULL
	if Comments = "" then Comments = NULL
	'Owner is now hard-coded to "Array"
	'if OwnerID = "" then OwnerID = NULL
	if CurrentUserID = "" then CurrenUserID = NULL
	if NumCopies = "" then NumCopies = 1

	if RushOrder = "" then 
	  RushOrder = 0 
	else 
	  RushOrder = 1
	end if

	if ReorderReason = "" Then ReorderReason = NULL
	if ReorderReasonOther = "" Then ReorderReasonOther = NULL
	
	if ReorderReason <> "3" then
		ReorderReasonOther = ""
	End If
	
	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
	Cmd.Parameters("RETURN_VALUE").Precision = 9
	cmd.Parameters.Append Cmd.CreateParameter("pOldContainerID", 131, 1, , ContainerID)
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERNAME",200, 1, 255, ContainerName)
	Cmd.Parameters.Append Cmd.CreateParameter("PCOMMENTS",200, 1, 4000, Comments)
	Cmd.Parameters.Append Cmd.CreateParameter("POWNERID",200, 1, 50, OwnerID)
	Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTUSERID",200, 1, 50, CurrentUserID)
	Cmd.Parameters.Append Cmd.CreateParameter("PNUMCOPIES",5, 1, , NumCopies)
	Cmd.Parameters.Append Cmd.CreateParameter("pDueDate",135, 1, , DueDate)
	Cmd.Parameters.Append Cmd.CreateParameter("pProjectNo",200, 1, 9, Project)
	Cmd.Parameters.Append Cmd.CreateParameter("pJobNo",200, 1, 4, Job)
	Cmd.Parameters.Append Cmd.CreateParameter("pIsRushOrder",5, 1, , RushOrder)
	Cmd.Parameters.Append Cmd.CreateParameter("pDeliveryLocationID",131, 1, , DeliveryLocationID)
	Cmd.Parameters.Append Cmd.CreateParameter("pOrderReason",131, 1, , ReorderReason)
	Cmd.Parameters.Append Cmd.CreateParameter("pOrderReasonIfOtherText",200, 1, 255, ReorderReasonOther)

	Cmd.Parameters.Append Cmd.CreateParameter("PNEWIDS",200, 2, 4000, NULL)

End Sub

%>