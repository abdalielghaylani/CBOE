<%

Dim RptConn
'****************************************************************************************
'*	PURPOSE: Create and Open an ADO connection to the Database using connection info from ini file 
'*	INPUT: 	Uses COWS Application scope variables and expects a global Conn variable
'*	Uses COWS Session credentials to connect and redirects to login page if unavailable	                    				 
'*	OUTPUT: Creates and Opens an ADO connection called Conn in the pages global scope   
'****************************************************************************************

Sub Getbiosar_browserSettingsConnection()
on error resume next
	Dim ConnStr
	ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source= " & Replace(Application("ReportDBPath"), ".mdb", "_settings.mdb")
	'Response.Write "RPTConnStr= " & ConnStr
	'Response.end
	
	Set RPTConn = Server.CreateObject("ADODB.Connection")
	RPTConn.Open ConnStr
	
	response.Write err.description
End Sub

'****************************************************************************************
'*	PURPOSE: Performs an HTTP request using ServerXMLHTTP object from the MSxml2 library
'*  Note that this object is better than CSHTTP because it has better threading and can be 
'*  nested in relay posts 	                            
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The raw http response from the server  									
'****************************************************************************************
Function CShttpRequest2(pMethod, pHostName, pTarget, pUserAgent, pData)
	Dim objXmlHttp
	Dim httpResponse
	Dim URL
	Dim StatusCode
	'CSBR# 139459
	'Date of Change: 04-Mar-2011
	'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
	Dim portNumber
	portNumber = Request.ServerVariables("SERVER_PORT")
    on error resume next 
    if portNumber = "80" or inStr(pHostName, ":" & portNumber) > 0 then	    
	    URL = "http://" & pHostName & "/" & pTarget
	else	    
	    URL = "http://" & pHostName & ":" & portNumber &  "/" & pTarget
	end if
	'End of change

	
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")
	'2/18/2005 add timout properties


            'default connect and resolve timeouts

            ResolveTimeout = 5 * 1000

            ConnectTimeout = 5 * 1000
	    pSendTimeout = 60000
	    pReceiveTimeout = 60000

            objXmlHttp.setTimeouts ResolveTimeout, ConnectTimeout, pSendTimeout, pReceiveTimeout

 


	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	on error resume next
	objXmlHttp.open pMethod, URL, False
	if inStr(UCASE(err.Description),"MSXML")>0 then
		response.Write "A report cannot be generated. MSXML is not installed properly. Please report this problem to the administrator for this site."
	end if
	objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objXmlHttp.setRequestHeader "User-Agent", pUserAgent
	objXmlHttp.send pData

	' Print out the request status:
	StatusCode = objXmlHttp.status

	If StatusCode <> "200" then
		httpResponse = objXmlHttp.responseText
		'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
	Else
		httpResponse = objXmlHttp.responseText
	End If
	
    CShttpRequest2 = httpResponse
	Set objXmlHttp = Nothing
End Function




'****************************************************************************************
'*	PURPOSE: Truncates and <span>s a string.  The full string is placed in the title attribute
'*			<span> tag so that it will appear as a pop up help 	                            
'*	INPUT: the string, the truncatation length, the id attribute of <span> 	                    			
'*	OUTPUT: a pair of <span></span> elements containing the truncated content and the title 										
'****************************************************************************************
Function TruncateInSpan(strText, Length, id)
	Dim str
	str = "<span "
	'if the text contents are longer than the desired length 
	'then place the full text contents in the title popup box
	'and truncate the text inside the <span>
	if len(strText) > Length then 
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & left(strText, Length-3) & "..."
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "</span>"
	TruncateInSpan = str
End function					


Function ShowSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	on error resume next
	Getbiosar_browserSettingsConnection()
	Set RS = RPTConn.Execute(sql)
	if Not (RS.EOF and RS.BOF) = True then	
		ShowSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "", "")
	End if
	if err.number <> 0 then
		response.Write err.number & err.Description & ":" & name & sql
	end if
End Function

Function ShowProfileSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	on error resume next
	Getbiosar_browserSettingsConnection()
	Set RS = RPTConn.Execute(sql)
	if Not (RS.EOF and RS.BOF) = True then	
		ShowProfileSelectBox = BuildProfileSelectBox(RS, name, SelectedValue, 40, "", "")
	End if
	if err.number <> 0 then
		response.Write err.number & err.Description & ":" & name & sql
	end if
End Function

Function ShowTableLayoutSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	on error resume next
	Getbiosar_browserSettingsConnection()
	Set RS = RPTConn.Execute(sql)
	if Not (RS.EOF and RS.BOF) = True then	
		ShowTableLayoutSelectBox = BuildTableLayoutSelectBox(RS, name, SelectedValue, 0, "", "")
	End if
	if err.number <> 0 then
		response.Write err.number & err.Description & ":" & name & sql
	end if
End Function


Function ShowReportNameSelectBox(name, SelectedValue, sql, dbkey, formgroup)
	Dim RS
	Dim strSelected
	on error resume next
	msg=CheckValidReportVars(dbkey, formgroup)
	if  Not msg <> "" then

	Getbiosar_browserSettingsConnection()
	Set RS = RPTConn.Execute(sql)
	
	if Not (RS.EOF and RS.BOF) = True then	
			
			ShowReportNameSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "", "")
	End if
	if err.number <> 0 then
		response.Write err.number & err.Description & ":" & name & sql
	end if
	else
		response.Write msg
		response.end
	end if
End Function




Function CheckValidReportVars(dbkey, formgroup)
	Dim msg
	basetable = Session("BaseTable_Orig" & dbkey & formgroup)
	basetable_test = UCase(basetable) & "."
	ListSQL = Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "LIST")
	DetailSQL = Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "DETAIL")
	NormalizedListSQl = replace(ListSQL, " ", "")
	NormalizedDetailSQl = replace(DetailSQL, " ", "")
	if Not Trim(Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "LIST")) <> "" then
		msg = "There are no fields defined for list and/or detail view. A report cannot be generated"
	else
		if not inStr(NormalizedListSQL, basetable_test)> 0 then
			msg = "Base table must have at least one field displayed in  list and detail view."
		end if
	end if
	
	if Not Trim(Session("fg_fields_reportSQL_NEW" & dbkey & formgroup & "DETAIL")) <> "" then
		msg = "There are no fields defined for list and/or detail  view. A report cannot be generated"
	else
		if not inStr(NormalizedDetailSQL, basetable_test)> 0 then
			msg = "Base table must have at least one field displayed in  list and detail view."
		end if
	end if
	formgroup_name = Session("formgroup_full_name" & dbkey & formgroup)
	formgroup_description = Trim(Application("DESCRIPTION" & dbkey & formgroup))
	if not formgroup_description <> ""  or  isNull(formgroup_description) then
		formgroup_description = formgroup_name
		Application("DESCRIPTION" & dbkey & formgroup)=formgroup_name
	end if
	if not formgroup_description <> "" or  isNull(formgroup_description) then
		msg = "There is no form description."
	end if
	
	if msg <> "" then
		msg = msg & " Report cannot be generated."
	else
		msg = ""
	end if
	CheckValidReportVars = msg
End function 

Function BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim str
	Dim DisplayText
	
	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT name=""" & name & """>"
	if RS.EOF AND RS.BOF then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!"  & "</option>"
	Else
	
		If Len(FirstOptionText) > 0 then
			str = str &  "<OPTION value=""" & FirstOptionValue &  """>" & FirstOptionText & "</option>"
		End if
		Do While NOT RS.EOF
			strSelected = ""
			theValue = RS("Value").value
			If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then 
				strSelected = "Selected=""True"""
			End if
			DisplayText = RS("DisplayText")
			response.Write value
			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..." 
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<OPTION " & strSelected & " value=""" & RS("Value") & """ id=""" & DisplayText &  """>" & DisplayText & "</option>"
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	
	BuildSelectBox = str
End function

Function BuildProfileSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim str
	Dim DisplayText

	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT onChange=""doRefresh()"" name=""" & name & """>"
	if RS.EOF AND RS.BOF then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!"  & "</option>"
	Else
	
		If Len(FirstOptionText) > 0 then
			str = str &  "<OPTION value=""" & FirstOptionValue &  """>" & FirstOptionText & "</option>"
		End if
		Do While NOT RS.EOF
			strSelected = ""
			theValue = RS("Value").value & "|" & RS("owner").value
			If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then 
				strSelected = "Selected=""True"""
			End if
			DisplayText = RS("Name") & " - " & RS("DisplayText")
			response.Write value
			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..." 
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<OPTION " & strSelected & " value=""" & RS("Value") & "|" & RS("owner") & """ id=""" & DisplayText &  """>" & DisplayText & "</option>"
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	
	BuildProfileSelectBox = str
End function

Function BuildTableLayoutSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim str
	Dim DisplayText
	
	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT onChange=""doSetNewPage()"" name=""" & name & """>"
	if RS.EOF AND RS.BOF then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!"  & "</option>"
	Else
	
		If Len(FirstOptionText) > 0 then
			str = str &  "<OPTION value=""" & FirstOptionValue &  """>" & FirstOptionText & "</option>"
		End if
		Do While NOT RS.EOF
			strSelected = ""
			theValue = RS("Value").value
			If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then 
				strSelected = "Selected=""True"""
			End if
			DisplayText = RS("DisplayText")
			response.Write value
			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..." 
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<OPTION " & strSelected & " value=""" & RS("Value")  & """ id=""" & DisplayText &  """>" & DisplayText & "</option>"
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	
	BuildTableLayoutSelectBox = str
End function


Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub

'LJB 5/2/2005 Function to populate report sql with sort from XML widget 
Function populateOrderBy(inputSQL)
	outputSQL = inputSql
	if Session("ReportTables")<> "" then
		temp = split(Session("ReportTables"), ",", -1)
		for i = 0 to UBound(temp)
			tablename = temp(i)
			if i = 0 then'basetable
				tableSort = GetLastWidgetBaseTableSort(tablename)
			else
				tableSort = GetLastWidgetChildTableSort(tablename)
			end if
			
			replaceText = "#" & tablename & "ORDER_BY#"
			if tableSort <> "" then
				if i = 0 then
					if Trim(tableSort) <> "" then
						if Instr(inputSQL, " ORDER BY ")> 0 then
							replaceTextWith = tableSort
						else
							replaceTextWith = " ORDER BY " & tableSort
						end if
					else
						replaceTextWith = ""
					end if
				else
					if Trim(tableSort) <> "" then
						replaceTextWith = " ORDER BY " & tableSort
					else
						replaceTextWith = ""
					end if
				end if
			else
			replaceTextWith = ""
			end if
			inputSql = replace(inputSql, replaceText, replaceTextWith)
		next
		outputSql = inputSql
	end if
	populateOrderBy = outputSQL
End Function


'LJB 5/1/2005 Function to get the fields in the sort list fo the base table when the XML widget is used to sort
function GetLastWidgetBaseTableSort(basetable)
	dim theReturn
	theReturn = ""
	if Session("allOuterSorts") <> "" then
		temp = split(Session("allOuterSorts"), "|", -1)
		'return the last sort
		theReturn = basetable & "." & temp(UBound(temp))
	end if
	GetLastWidgetBaseTableSort=theReturn
End Function

'LJB 5/1/2005 Function to get the fields in the sort list for a child table when the XML widget is used to sort
function GetLastWidgetChildTableSort(childTableName)'
	dim theReturn
	theReturn=""
	hitlistid = Session("HitListID" & "biosar_browser" & "1189")
	if Session("allInnerSorts") <> "" then
		temp = split(Session("allInnerSorts"), "|", -1) 
		'loop backwards since we are interested in the most recent sort
		for i = 0 to UBound(temp)
			temp2 = split(temp(i), ".", -1)
			test = Trim(replace(childTableName, ".", "_"))& "_RS"
			if Trim(UCase(temp2(0))) = test then
				theReturn = childTableName & "." & temp2(1) ' return the fieldname and the sort direction
				exit for
			end if
		next
	end if
	GetLastWidgetChildTableSort=theReturn
end function

%>
