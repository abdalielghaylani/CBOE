<SCRIPT LANGUAGE=vbscript RUNAT=Server>
'  Builds the fieldArray from column definition string
'  Column definition string contains pipe delimited fieldid:Colwidth
Function GetFieldArray(colDef, byref fieldMapArray)
	Dim fieldArray()
	Dim tempArr
	Dim tempArr2 
	Dim i
	Dim FirstColName
	Dim FirstColWidth
	
	'fieldMapArray = Application("ContainerFieldMap")
	
	tempArr = Split(colDef,"|")
	tempLength =  Ubound(tempArr)
	ReDim fieldArray(tempLength, 3)
	For i = 0 to tempLength
		tempArr2 = Split(tempArr(i),":")
		fieldArray(i,0) = fieldMapArray(tempArr2(0),0)
		fieldArray(i,1) = fieldMapArray(tempArr2(0),1)
		fieldArray(i,2) = Cint(tempArr2(1))
		fieldArray(i,3) = Cint(tempArr2(0))
	Next
	GetFieldArray = fieldArray
End Function

' Adds subitems to report view of ListView object based on fieldArray
Sub AddListViewSubItemsFromFieldArray(fieldArray)
	Dim i
	For i=0 to Ubound(fieldArray)
		fn = Cstr(fieldArray(i,0))
		if inStr(1,fn,"inv_compounds.") > 0 then
			fn = Replace(fn, "inv_compounds.", "")
		end if
		'Response.Write fn & ":" & RS(fn)
		'Response.Write TruncateInSpan(RS(fn), 1,"") & ":test<BR>"
		'Response.Write cInt(fieldArray(i,2)) & "<BR>"
		if NOT (fn = "Reg_Batch_ID" AND Application("RegServerName") = "NULL")  then
			Listx.ListSubItems.Add , , TruncateInSpan(RS(fn), CInt(fieldArray(i,2)),"") 	
		end if
	Next
End Sub

' Builds the column header for report view of ListView object based on fieldArray
Sub BuildListViewHeaderFromFieldArray(fieldArray, byref fieldMapArray, firstColumnIndex)
	Dim i
	'fieldMapArray = Application("ContainerFieldMap")
	FirstColFieldName = fieldMapArray(firstColumnIndex,0)
	FirstColName = fieldMapArray(firstColumnIndex,1)
	FirstColWidth = fieldMapArray(firstColumnIndex,2)
	ListView.ColumnHeaders.Add ,,FirstColName, CInt(8 * FirstColWidth), 0
	For i = 0 to Ubound(fieldArray) 
		if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL")  then
			ListView.ColumnHeaders.Add ,,Cstr(fieldArray(i,1)), CInt(8 * fieldArray(i,2)), 1
		End if
	Next
End Sub

' Writes HTML table header based on fieldArray
Sub WriteTableHeaderFromFieldArray(fieldArray,  byref fieldMapArray, firstColumnIndex)
	WriteTableHeaderFromFieldArray2 fieldArray, fieldMapArray, firstColumnIndex, 8
End Sub
Sub WriteTableHeaderFromFieldArray2(fieldArray,  byref fieldMapArray, firstColumnIndex, columnWidthMultiplier)
	Dim i
	Dim defaultField
	
	if showplates then
		'defaultField = "PLATE_ID"
		defaultField = "PLATE_BARCODE"
	elseif ShowWells then
		defaultField = "WELL_ID"
	Else
		defaultField = "BARCODE"
	End if	
	for i = 0 to Ubound(fieldMapArray)
		if ucase(fieldMapArray(i,0)) = defaultField then 	defaultColumnIndex = i
		i = i + 1
	next
	'fieldMapArray = Application("ContainerFieldMap")
	FirstColFieldName = fieldMapArray(defaultColumnIndex,0)
	FirstColName = fieldMapArray(defaultColumnIndex,1)
	FirstColWidth = fieldMapArray(defaultColumnIndex,2)
	
	if SortDirection = 1 then
		arrowImage = "&nbsp;<IMG border=0 src=""/cheminv/graphics/asc_arrow.gif"">"
	Else
		arrowImage = "&nbsp;<IMG border=0 src=""/cheminv/graphics/desc_arrow.gif"">"
	End if	
	
	Response.Write "<TABLE cellspacing=1 cellpadding=1 border=0 bwidth=1700>"
	Response.Write "<TR><td colspan=40>"
	Response.Write "<span style=""font-size:8pt; font-face: arial"">" & headerTxt & "</span>"
	Response.Write "</td></TR>"
	Response.Write "	<TR>"
	Response.Write "	<Th CLASS=ListView  ALIGN=CENTER NOWRAP width=""" & CInt(columnWidthMultiplier * FirstColWidth) & """>"
	Response.Write			"<a class=MenuLink href=# onclick=""SortReport('" & defaultField & "'); return false;""><font color=black>" & FirstColName & "</font></a>"
	if SortByFieldName = defaultField then Response.Write arrowImage
	Response.Write "	</Th>"
	For i = 0 to Ubound(fieldArray)
		if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL")  then
			Response.Write "	<Th CLASS=ListView  ALIGN=CENTER NOWRAP width=" & columnWidthMultiplier * fieldArray(i,2) & ">"
			Response.Write			"<a class=MenuLink href=# onclick=""SortReport('" & fieldArray(i,0) &"'); return false;""><font color=black>" & fieldArray(i,1) &  "</font></a>"
			if SortByFieldName = fieldArray(i,0) then
				Response.Write arrowImage
			End if
			Response.Write "	</Th>"
		end if
	Next 
	Response.Write "<Td bgcolor=""#FFFFFF"">&nbsp;</Td>"
	Response.Write "	</TR>"
	Response.Write "</TABLE>"
End Sub

Sub GetSortfield()
	SortByFieldName = Request("SortByFieldName")
	SortDirection = Session("LastSortDirection")
	
	if SortByFieldName = "" then
			' different sorting remembered for plates and containers
			SortByFieldName = Session("LastSortByFieldName" & showplates)
	End if
	if SortByFieldName = "" then 
		if showplates then
			SortByFieldName = "PLATE_ID"
		else
			SortByFieldName = "Container_ID"
		End if
		Session("LastSortByFieldName" & showplates) = SortByFieldName
		SortDirection = 1
		Session("LastSortDirection") = 1
	End if
	
	
	if Request("SortByFieldName") = Session("LastSortByFieldName" & showplates) then
		SortDirection = -1 * CInt(Session("LastSortDirection"))
	End if
	
	Session("LastSortByFieldName" & showplates) = SortByFieldName
	Session("LastSortDirection") = SortDirection

	If SortDirection = 1 then
		SortDirectionTxt = "ASC"
	Else
		SortDirectionTxt = "DESC"
	End if 
End Sub

Function PageNavBar()
	Dim qs
	qs = ""
	if LocationID <> "" then qs = qs & "&LocationID=" & LocationID & "&LocationName=" & LocationName 
	if CompoundID <> "" then qs = qs &  "&CompoundID=" & CompoundID 
	if ContainerID <> "" then qs = qs &  "&ContainerID=" & ContainerID 
	ref = "<S" & "CRIPT language=javascript>document.all.mySpan.innerHTML = """ 
	scriptname=request.servervariables("script_name") 
	ref = ref & "<span style='font-size:8pt; font-family: arial'>Page " & mypage & " of " & maxcount & "&nbsp;"
	if (mypage mod 10) = 0 then
		counterstart = mypage - 9
	else
		counterstart = mypage - (mypage mod 10) + 1
	end if
	counterend = counterstart + 9
	if counterend > maxcount then counterend = maxcount
	if counterstart <> 1 then

	ref=ref & "<a class=MenuLink href='" & scriptname
	ref=ref & "?whichpage=" & (counterstart - 1)
	ref=ref & qs
	ref=ref & "'><b>&lt;</b></a>&nbsp;"
	end if
	ref = ref & "<span style='font-size:8pt; font-family: arial'>[</span>"
	for counter=counterstart to counterend
	If counter>=10 then
	pad=""
	end if
	if cstr(counter) <> mypage then
	ref=ref & "<a class=MenuLink href='" & scriptname
	ref=ref & "?whichpage=" & counter 
	ref=ref & qs
	ref=ref & "'>" & pad & counter & "</a>"
	else
	ref= ref & "<span style='font-size:8pt; font-family: arial'><b>" & pad & counter & "</b></span>"
	end if
	if counter <> counterend then ref = ref & " "
	next
	ref = ref & "<span style='font-size:8pt; font-family: arial'>]&nbsp;</span>"
	if counterend <> maxcount then
	ref=ref & "<a class=MenuLink href='" & scriptname
	ref=ref & "?whichpage=" & (counterend + 1)
	ref=ref & qs
	ref=ref & "'><b>&gt;</b></a>"
	
	end if
	ref = ref & "&nbsp;&nbsp;&nbsp;</span>"
	ref = ref & ""
	ref = ref & """;</s" & "cript>"
	PageNavBar = ref
end function

Function PageNavBar2()
	Dim qs
	qs = ""
	qs = qs & "&formgroup=" & formgroup & "&formmode=list&dbname=cheminv&PagingMove=previous_record"
	
	ref = "<S" & "CRIPT language=javascript>document.all.mySpan.innerHTML = """ 
	scriptname=request.servervariables("script_name") 
	ref = ref & "<table rows='1' cols='1'><tr><td><span style='font-size:8pt; font-family: arial'>Page " & mypage & " of " & maxcount & "&nbsp;"
	if (mypage mod 10) = 0 then
		counterstart = mypage - 9
	else
		counterstart = mypage - (mypage mod 10) + 1
	end if
	counterend = counterstart + 9
	if counterend > maxcount then counterend = maxcount
	if counterstart <> 1 then
		ref=ref & "<a class=MenuLink href='" & scriptname
		ref=ref & "?whichpage=" & (counterstart - 1)
		'DJP added maxcount to qs
		ref=ref & "&maxcount=" & maxcount
		ref=ref & qs
		ref=ref & "'><b>&lt;</b></a>&nbsp;"
	end if
	ref = ref & "<span style='font-size:8pt; font-family: arial'>[</span>"
	for counter=counterstart to counterend
		If counter>=10 then
			pad=""
		end if
		if cstr(counter) <> mypage then
			ref=ref & "<a class=MenuLink href='" & scriptname
			ref=ref & "?whichpage=" & counter
			'DJP added maxcount to qs
			ref=ref & "&maxcount=" & maxcount
			ref=ref & qs
			ref=ref & "'>" & pad & counter & "</a>"
		else
			ref= ref & "<span style='font-size:8pt; font-family: arial'><b>" & pad & counter & "</b></span>"
		end if
		if counter <> counterend then ref = ref & " "
	next
	ref = ref & "<span style='font-size:8pt; font-family: arial'>]&nbsp;</span>"
	if counterend <> maxcount then
		ref=ref & "<a class=MenuLink href='" & scriptname
		ref=ref & "?whichpage=" & (counterend + 1) 
		'DJP added maxcount to qs
		ref=ref & "&maxcount=" & maxcount
		ref=ref & qs
		ref=ref & "'><b>&gt;</b></a>"
	end if
	ref = ref & "</span></td>"
	ref = ref & "</table>"
	ref = ref & """;</s" & "cript>"
	PageNavBar2 = ref
end function

</SCRIPT>
