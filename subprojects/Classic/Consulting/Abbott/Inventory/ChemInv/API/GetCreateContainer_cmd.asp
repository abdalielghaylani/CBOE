<%
Sub GetCreateContainer_cmd()
	' Check for required parameters
	If IsEmpty(LocationID) then
		strError = strError & "LocationID is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(UOMID) then
		strError = strError & "Unit of Measure ID is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(QtyMax) then
		strError = strError & "QtyMax is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(QtyInitial) then
		strError = strError & "QtyInitial is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(ContainerTypeID) then
		strError = strError & "ContainerTypeID is a required parameter<BR>"
		bWriteError = True
	End if


	'Validate date parameters
	if ExpDate = "" then
		ExpDate = NULL
	Elseif IsDate(ExpDate) then
		ExpDate = CDate(ExpDate)
	Else
		strError = strError & "ExpDate could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if DateProduced = "" then
		DateProduced = NULL
	Elseif IsDate(DateProduced) then
		DateProduced = CDate(DateProduced)
	Else
		strError = strError & "Date produced could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if DateOrdered = "" then
		DateOrdered = NULL
	Elseif IsDate(DateOrdered) then
		DateOrdered = CDate(DateOrdered)
	Else
		strError = strError & "Date ordered could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if DateReceived = "" then
		DateReceived = NULL
	Elseif IsDate(DateReceived) then
		DateReceived = CDate(DateReceived)
	Else
		strError = strError & "Date received could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if

	' Optional parameters
	if ContainerStatus = "" then ContainerStatus = NULL
	if RegID = "" then RegID = NULL
	if BatchNumber = "" then BatchNumber = NULL
	if Barcode = "" then Barcode = NULL
	if BarcodeDescID = "" then BarcodeDescID = NULL
	if MinStockQty = "" then MinStockQty = NULL
	if MaxStockQty = "" then MaxStockQty = NULL
	if CompoundID = "" then CompoundID = NULL
	if ContainerDesc = "" then ContainerDesc = NULL
	if ContainerName = "" then ContainerName = NULL
	if TareWeight = "" then TareWeight = NULL
	if NetWeight = "" then NetWeight = NULL
	if FinalWeight = "" then FinalWeight = NULL
	if UOWID = "" then UOWID = NULL
	if Concentration = "" then Concentration = NULL
	if Density = "" then Density = NULL
	if UOCID = "" then UOCID = NULL
	if UODID = "" then UODID = NULL
	if Purity = "" then Purity = NULL
	if UOPID = "" then UOPID = NULL
	if Grade = "" then Grade = NULL
	if SolventIDFK = "" then SolventIDFK = NULL
	if lcase(SolventIDFK) = "null" then SolventIDFK = NULL
	if Comments = "" then Comments = NULL
	if StorageConditions = "" then StorageConditions = NULL
	if HandlingProcedures = "" then HandlingProcedures = NULL
	if SupplierID = "" then SupplierID = NULL
	if SupplierCatNum = "" then SupplierCatNum = NULL
	if LotNum = "" then LotNum = NULL
	if ContainerCost = "" then ContainerCost = NULL
	if OwnerID = "" then OwnerID = NULL
	'default to current user
	if CurrentUserID = "" then CurrenUserID = Ucase(Session("UserNameChemInv"))
	if PONumber = "" then PONumber = NULL
	if POLineNumber = "" then POLineNumber = NULL
	if ReqNumber = "" then ReqNumber = NULL
	if NumCopies = "" then NumCopies = 1
	if Field_1 = "" then Field_1 = NULL
	if Field_2 = "" then Field_2 = NULL
	if Field_3 = "" then Field_3 = NULL
	if Field_4 = "" then Field_4 = NULL
	if Field_5 = "" then Field_5 = NULL
	if Field_6 = "" then Field_6 = NULL
	if Field_7 = "" then Field_7 = NULL
	if Field_8 = "" then Field_8 = NULL
	if Field_9 = "" then Field_9 = NULL
	if Field_10 = "" then Field_10 = NULL

	if Date_1 = "" then
		Date_1 = NULL
	Elseif IsDate(Date_1) then
		Date_1 = CDate(Date_1)
	Else
		strError = strError & "Date_1 could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if Date_2 = "" then
		Date_2 = NULL
	Elseif IsDate(Date_2) then
		Date_2 = CDate(Date_2)
	Else
		strError = strError & "Date_2 could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if Date_3 = "" then
		Date_3 = NULL
	Elseif IsDate(Date_3) then
		Date_3 = CDate(Date_3)
	Else
		strError = strError & "Date_3 could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if Date_4 = "" then
		Date_4 = NULL
	Elseif IsDate(Date_4) then
		Date_4 = CDate(Date_4)
	Else
		strError = strError & "Date_4 could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	if Date_5 = "" then
		Date_5 = NULL
	Elseif IsDate(Date_5) then
		Date_5 = CDate(Date_5)
	Else
		strError = strError & "Date_5 could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if
	
	If bWriteError then
		' Respond with Error
		Response.Write strError
		Response.end
	End if

	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
	Cmd.Parameters("RETURN_VALUE").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PBARCODE",200, 1, 50, Barcode)
	Cmd.Parameters("PBARCODE").NumericScale = 0
	Cmd.Parameters("PBARCODE").Precision = 0
	Cmd.Parameters.Append Cmd.CreateParameter("PBARCODEDESCID",131, 1, 50, BarcodeDescID)
	Cmd.Parameters("PBARCODEDESCID").NumericScale = 9
	Cmd.Parameters("PBARCODEDESCID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",131, 1, 0, LocationID)
	Cmd.Parameters("PLOCATIONID").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PUOMID",131, 1, 0, UOMID)
	Cmd.Parameters("PUOMID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERTYPEID",131, 1, 0, ContainerTypeID)
	Cmd.Parameters("PCONTAINERTYPEID").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERSTATUSID",131, 1, 0, ContainerStatusID)
	Cmd.Parameters.Append Cmd.CreateParameter("PMAXQTY",5, 1, 0, QtyMax)
	Cmd.Parameters.Append Cmd.CreateParameter("PREGID",131, 1, 0, RegID)
	Cmd.Parameters("PREGID").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PBATCHNUMBER",131, 1, 0, BatchNumber)
	Cmd.Parameters("PBATCHNUMBER").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PINITIALQTY",5, 1, 0, QtyInitial)
	Cmd.Parameters.Append Cmd.CreateParameter("PQTYREMAINING",5, 1, 0, QtyInitial)
	Cmd.Parameters.Append Cmd.CreateParameter("PMINSTOCKQTY",5, 1, 0, MinStockQty)
	Cmd.Parameters.Append Cmd.CreateParameter("PMAXSTOCKQTY",5, 1, 0, MaxStockQty)
	Cmd.Parameters.Append Cmd.CreateParameter("PEXPDATE",135, 1, 0, ExpDate)
	Cmd.Parameters.Append Cmd.CreateParameter("PCOMPOUNDID",131, 1, 0, CompoundID)
	Cmd.Parameters("PCOMPOUNDID").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERNAME",200, 1, 255, ContainerName)
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERDESC",200, 1, 255, ContainerDesc)
	Cmd.Parameters.Append Cmd.CreateParameter("PTAREWEIGHT",5, 1, 0, TareWeight)
	Cmd.Parameters.Append Cmd.CreateParameter("PNETWEIGHT",5, 1, 0, NetWeight)
	Cmd.Parameters.Append Cmd.CreateParameter("PFINALWEIGHT",5, 1, 0, FinalWeight)
	Cmd.Parameters.Append Cmd.CreateParameter("PUOWID",131, 1, 0, UOWID)
	Cmd.Parameters("PUOWID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PPURITY",5, 1, 0, Purity)
	Cmd.Parameters.Append Cmd.CreateParameter("PUOPID",131, 1, 0, UOPID)
	Cmd.Parameters("PUOPID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATION",5, 1, 0, Concentration)
	Cmd.Parameters.Append Cmd.CreateParameter("PDENSITY",5, 1, 0, Density)
	Cmd.Parameters.Append Cmd.CreateParameter("PUOCID",131, 1, 0, UOCID)
	Cmd.Parameters("PUOCID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PUODID",131, 1, 0, UODID)
	Cmd.Parameters("PUODID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTIDFK",200, 1, 255, SolventIDFK)
	Cmd.Parameters.Append Cmd.CreateParameter("PGRADE",200, 1, 255, Grade)
	Cmd.Parameters.Append Cmd.CreateParameter("PCOMMENTS",200, 1, 4000, Comments)
	Cmd.Parameters.Append Cmd.CreateParameter("PSTORAGECONDITIONS",200, 1, 4000, StorageConditions)
	Cmd.Parameters.Append Cmd.CreateParameter("PHANDLINGPROCEDURES",200, 1, 4000, HandlingProcedures)
	Cmd.Parameters.Append Cmd.CreateParameter("PSUPPLIERID",131, 1, 0, SupplierID)
	Cmd.Parameters("PSUPPLIERID").Precision = 4
	Cmd.Parameters.Append Cmd.CreateParameter("PSUPPLIERCATNUM",200, 1, 50, SupplierCatNum)
	Cmd.Parameters.Append Cmd.CreateParameter("PLOTNUM",200, 1, 50, LotNum)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATEPRODUCED",135, 1, 0, DateProduced)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATEORDERED",135, 1, 0, DateOrdered)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATERECEIVED",135, 1, 0, DateReceived)
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERCOST",131, 1, 0, ContainerCost)
	Cmd.Parameters("PCONTAINERCOST").NumericScale = 2
	Cmd.Parameters("PCONTAINERCOST").Precision = 9
	Cmd.Parameters.Append Cmd.CreateParameter("PPONUMBER",200, 1, 50, PONumber)
	Cmd.Parameters.Append Cmd.CreateParameter("PPOLINENUMBER",200, 1, 50, POLineNumber)
	Cmd.Parameters.Append Cmd.CreateParameter("PREQNUMBER",200, 1, 50, ReqNumber)
	Cmd.Parameters.Append Cmd.CreateParameter("POWNERID",200, 1, 50, OwnerID)
	Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTUSERID",200, 1, 50, CurrentUserID)
	Cmd.Parameters.Append Cmd.CreateParameter("PNUMCOPIES", 5, 1, 0, NumCopies)
	Cmd.Parameters.Append Cmd.CreateParameter("PNEWIDS",200, 2, 4000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_1",200, 1, 2000, FIELD_1)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_2",200, 1, 2000, FIELD_2)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_3",200, 1, 2000, FIELD_3)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_4",200, 1, 2000, FIELD_4)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_5",200, 1, 2000, FIELD_5)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_6",200, 1, 2000, FIELD_6)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_7",200, 1, 2000, FIELD_7)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_8",200, 1, 2000, FIELD_8)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_9",200, 1, 2000, FIELD_9)
	Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_10",200, 1, 2000, FIELD_10)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATE_1",135, 1, 0, DATE_1)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATE_2",135, 1, 0, DATE_2)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATE_3",135, 1, 0, DATE_3)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATE_4",135, 1, 0, DATE_4)
	Cmd.Parameters.Append Cmd.CreateParameter("PDATE_5",135, 1, 0, DATE_5)
End Sub

%>