<%
Dim Link_href
Dim Link_Text
Dim Link_image_src


Today =  Month(Now()) & "/" & Day(Now()) & "/" & Year(Now()) 
lw = DateAdd("ww", -1, Today)
LastWeek = Month(lw) & "/" & Day(lw) & "/" & Year(lw)
Select case CStr(Application("DATE_FORMAT"))
	case "8"
		Today = Month(Today) & "/" & Day(Today) & "/" & Year(Today)
		LastWeek = Month(LastWeek) & "/" & Day(LastWeek) & "/" & Year(LastWeek)
	case "9"
		Today = Day(Today) & "/" & Month(Today) & "/" & Year(Today)
		LastWeek = Day(LastWeek) & "/" & Month(LastWeek) & "/" & Year(LastWeek)
	case "10"
		Today = Year(Today) & "/" & Month(Today) & "/" & Day(Today)
		LastWeek = Year(LastWeek) & "/" & Month(LastWeek) & "/" & Day(LastWeek)
End Select


'Restamp Credentials cookie
UserName = Request.Cookies("CS_SEC_UserName")
UserID = Request.Cookies("CS_SEC_UserID")
if Len(UserName) > 0 then ProlongCookie "CS_SEC_UserName", UserName, Application("CookieExpiresMinutes")
if Len(UserID) > 0 then ProlongCookie "CS_SEC_UserID", UserID, Application("CookieExpiresMinutes")

'****************************************************************************************
'*	PURPOSE: Performs an HTTP request using CShttp and returns the HTTP response	                            
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The raw http response from the server  											*
'****************************************************************************************
Function CShttpRequest(pMethod, pHostName, pTarget, pReferrer, pData)
	Dim Connection,HttpRequest
	Dim HTTP_QUERY_STATUS_CODE, HTTP_QUERY_STATUS_TEXT
	Dim RequestType
	Dim httpResponse
	
	HTTP_QUERY_STATUS_CODE = 19
	HTTP_QUERY_STATUS_TEXT = 20
	
	on error resume next
	
	if UCASE(pMethod) = "GET" then
		RequestType = 0 ' This is a GET
	Else 
		Requesttype = 1 ' This is a POST
	End if
	
	FormHeaders = "Content-Type: application/x-www-form-urlencoded"
	
	set Connection = Server.CreateObject("CSHttp.Connection")
	Connection.Open (pHostName)
	set HttpRequest = Connection.Requests.Add(RequestType, pTarget, pReferrer, pData, FormHeaders)
	HttpRequest.Send
	StatusCode = httpRequest.Info(HTTP_QUERY_STATUS_CODE, 0) + ""

	If StatusCode <> "200" then
		httpResponse = httpRequest.Response
		'httpResponse = "HTTP Error: " & StatusCode & " : " & httpRequest.Info(HTTP_QUERY_STATUS_TEXT, 0)
	Else
		httpResponse = httpRequest.Response
	End If
	
    CShttpRequest = httpResponse
    Set httpRequest =Nothing
    httpRequest =""
    Set Connection = Nothing
    Connection=""
End Function

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
	
	URL = "http://" & pHostName & "/" & pTarget
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")


	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
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
'*	PURPOSE: Performs an HTTP request using ServerXMLHTTP object from the MSxml2 library
'*  Note this function adds the ability to set the timeouts for sending and receiving
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The raw http response from the server  									
'****************************************************************************************
Function CShttpRequest3(pMethod, pHostName, pTarget, pUserAgent, pData, pSendTimeout, pReceiveTimout)
	Dim objXmlHttp
	Dim httpResponse
	Dim URL
	Dim StatusCode
	Dim ResolveTimeout
	Dim ConnectTimeout
	
	URL = "http://" & pHostName & "/" & pTarget
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

	'default connect and resolve timeouts
	ResolveTimeout = 5 * 1000
	ConnectTimeout = 5 * 1000
	objXmlHttp.setTimeouts ResolveTimeout, ConnectTimeout, pSendTimeout, pReceiveTimeout

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
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
	
    CShttpRequest3 = httpResponse
	Set objXmlHttp = Nothing
End Function


'****************************************************************************************
'*	PURPOSE: Performs an HTTP request using ServerXMLHTTP object from the MSxml2 library
'*  returns the xmlHTTP Object 
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The XMLHTTP object  									
'****************************************************************************************
Function GetXMLHTTP(pMethod, pHostName, pTarget, pUserAgent, pData)
	Dim objXmlHttp
	Dim httpResponse
	Dim URL
	Dim StatusCode
	
	URL = "http://" & pHostName & "/" & pTarget
	
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
	objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objXmlHttp.setRequestHeader "User-Agent", pUserAgent
	objXmlHttp.send pData

    Set GetXMLHTTP = objXmlHttp
	Set objXmlHttp = Nothing
End Function



'****************************************************************************************
'*	PURPOSE: Writes a caption-content pair using the content from the display session variables 	                            
'*	INPUT: Caption as string, A string containing one or more fieldNames concatenated with a pipe
'*		   truncate length, the id value of the span tag 	                    			
'*	OUTPUT: a pair of <td></td> elements containing the caption and the Span truncated content  										
'****************************************************************************************
Function ShowField(fieldCaption, strFieldNames, truncateLength, id)
	Dim str
	Dim tempArr
	Dim arrLen
	
	' id defaults to fieldCaption
	if len(id) = 0 then id = fieldCaption
	' Split at pipe which is the concatenation marker
	tempArr = split(strFieldNames, "|")
	arrLen = Ubound(tempArr)
	str = "<td align=right valign=top nowrap>"
			
	str = str & fieldCaption
	str = str & "</td>"
	str = str & "<td bgcolor=#d3d3d3 align=right>"
	' Loop through array to concatenate the content
	For i = 0 to arrLen step 2
		str2 = str2 & Eval(tempArr(i)) 
		if i < arrLen then str2 = str2 & tempArr(i+1)
	Next
	' Concatenating to a null value should give null
	If isNull(Eval(tempArr(0))) then str2 = ""
	str = str & TruncateInSpan(str2, truncateLength, id)
	str = str & "</td>"
	ShowField = str
End function

Function ShowField2(fieldCaption, strFieldNames, truncateLength, id, anchor)
	Dim str
	Dim tempArr
	Dim arrLen
	' id defaults to fieldCaption
	if len(id) = 0 then id = fieldCaption
	' Split at pipe which is the concatenation marker
	tempArr = split(strFieldNames, "|")
	arrLen = Ubound(tempArr)
	str = "<td align=right valign=top nowrap>"
	str = str & fieldCaption
	str = str & "</td>"
	str = str & "<td bgcolor=#d3d3d3 align=right>"
	str = str & anchor
	' Loop through array to concatenate the content
	For i = 0 to arrLen step 2
		str2 = str2 & Eval(tempArr(i)) 
		if i < arrLen then str2 = str2 & tempArr(i+1)
	Next
	' Concatenating to a null value should give null
	If isNull(Eval(tempArr(0))) then str2 = ""
	str = str & TruncateInSpan(str2, truncateLength, id)
	str = str & "</a></td>"
	ShowField2 = str
End function
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
	if Length < 3 then
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & "..."
	elseif len(strText) > Length then 
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & left(strText, Length-3) & "..."
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "&nbsp;"
	str = str & "</span>"
	TruncateInSpan = str
End function					

Function ShowStructure(id, height, width)
	Dim str	
	if session("DrawPref") = "ISIS" then
		ISISDraw = " isisdraw=" & "'" & "True" &  "'"
	else
		ISISDraw = " isisdraw=" & "'" & "False" &  "'"
	end if	
	str = "<embed src='" & Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx' width='" & width & "' height='" & height & "' ID='" & id & "' name='cdp" & id & "' type='chemical/x-cdx' viewonly='true' dataurl='/cheminv/cheminv/cheminv_action.asp?dbname=cheminv&formgroup=base_form_group&dataaction=get_structure&Table=inv_compounds&Field=Structure&DisplayType=cdx&StrucID=" & id & "'" & ISISDraw & ">"
	ShowStructure = "<SCRIPT language=javascript>cd_insertObjectStr(""" & str & """);</" & "SCRIPT>" 
End function

Function ShowRegStructure(id, height, width)
	Dim str	
	if session("DrawPref") = "ISIS" then
		ISISDraw = " isisdraw=" & "'" & "True" &  "'"
	else
		ISISDraw = " isisdraw=" & "'" & "False" &  "'"
	end if	
	str = "<embed src='" & Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx' width='" & width & "' height='" & height & "' ID='" & id & "' name='cdp" & id & "' type='chemical/x-cdx' viewonly='true' dataurl='/cheminv/cheminv/cheminv_action.asp?dbname=invreg&formgroup=base_form_group&dataaction=get_structure&Table=Reg_Numbers&Field=Structure&DisplayType=cdx&StrucID=" & id & "'" & ISISDraw & ">"
	ShowRegStructure = "<SCRIPT language=javascript>cd_insertObjectStr(""" & str & """);</" & "SCRIPT>" 
End function

Function ShowInlineStructure(id, height, width, border,base64cdx)
	Dim str
	if base64cdx = "" or isEmpty(base64cdx) then
		base64cdx = ""
	else
		base64cdx =  "data:chemical/x-cdx;base64," & base64cdx	
	end if
	str = "<embed border='" & border & "' src='" & Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx' width='" & width & "' height='" & height & "' ID='" & id & "' name='cdp" & id & "' type='chemical/x-cdx' viewonly='true' dataurl='data:chemical/x-cdx;base64," & base64cdx & "'>"
	TempcdxPath = Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx"
	'str = str & "<applet code=""camsoft.cdp.CDPHelperAppSimple"" width=""1"" height=""1"" name=""cdp" & id & """ archive=""camsoft.jar"" id=""" & id & """><param name=""ID"" value=""" & id & """><param name=""CABBASE"" value=""camsoft.cab""></applet>"
	Response.Write "<input type=hidden name=inline value=""" & base64cdx & """>"
	if session("DrawPref") = "ISIS" then
		ISISDraw = """True"""
	else
		ISISDraw = """False"""
	end if
	ShowInlineStructure = "<SCRIPT language=javascript>cd_insertObject(""chemical/x-cdx"", """ &  width & """, """ & height & """, ""cdpnew"", """ & TempCdxPath & """, ""true"", ""true"", escape(document.all.inline.value),  ""true"", " & ISISDraw & ");</" & "SCRIPT>" 
End function

'****************************************************************************************
'*	PURPOSE: Create HTML Dropdown box from a sql query	                               
'*	INPUT: 	Caption for the drop down, name attribute of <OPTION> elements produced,                              
'*          sql statement returning two colums named Value and DisplayText, selected value		        
'*	OUTPUT: returns a pair of HTML <td>s with caption and <SELECT id=select1 name=select1> block as string                        
'****************************************************************************************
Function ShowPickList(fieldCaption, name, SelectedValue, sql)
	Dim str
	
	str = "<td align=right valign=top nowrap>"
	str = str & fieldCaption
	str = str & "</td>"
	str = str & "<td>"
	str = str & ShowSelectBox(name, SelectedValue, sql)
	str = str & "</td>"
	ShowPickList = str
End function

'****************************************************************************************
'*	PURPOSE: Create HTML Dropdown box from a sql query	                               
'*	INPUT: 	name attribute of <OPTION> elements produced, selected value                             
'*          sql statement returning two colums named Value and DisplayText		        
'*	OUTPUT: returns HTML SELECT BLOCK as string                        
'****************************************************************************************
Function ShowSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	GetInvConnection()
	Set RS = Conn.Execute(sql)
	ShowSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "", "", 1, false, "")
End function

Function ShowSelectBox2(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowSelectBox2 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, 1, false, "")
End function

Function ShowSelectBox3(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, ChangeScript)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowSelectBox3 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, 1, false, ChangeScript)
End function

Function ShowMultiSelectBox(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowMultiSelectBox = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, "")
End function

Function ShowMultiSelectBox2(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, ChangeScript)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowMultiSelectBox2 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, ChangeScript)
End function

Function ShowRPTSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowRPTSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "Select One", "NULL",1,false,"")
End function

Function RepeatString(n, str)
	For i = 1 to n
		tempstr = tempstr + str
	Next
	RepeatString = tempstr
End function

Function BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismultiple, ChangeScript)
	Dim str
	Dim DisplayText
	Dim multiple
	
	If ismultiple then 
		multiple = "MULTIPLE"
	Else
		multiple = ""
	End if
	 
	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT size=""" & size & """ name=""" & name & """" & multiple & " onchange=""" & ChangeScript & """>"
	if RS.EOF AND RS.BOF AND Len(FirstOptionText) = 0 then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!" 
	Else
		
		If Len(FirstOptionText) > 0 then
			If isValueInList(SelectedValue,FirstOptionValue,",") then
				strSelected = "Selected=""True"""
			End if
			str = str &  "<OPTION " & strSelected & " value=""" & FirstOptionValue &  """>" & FirstOptionText
		End if
		Do While NOT RS.EOF
			strSelected = ""
			theValue = RS("Value").value
			'If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then 
			'Response.Write SelectedValue &":"&theValue& ":"& isValueInList(SelectedValue,theValue,",") &"<BR>"
			If isValueInList(SelectedValue,theValue,",") then
				strSelected = "Selected=""True"""
			End if
			DisplayText = RS("DisplayText")
			
			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..." 
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<OPTION " & strSelected & " value=""" & RS("Value") & """ id=""" & DisplayText &  """>" & DisplayText
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	BuildSelectBox = str
End function

Function IsValueInList(list, value, delimiter)
	dim l
	dim v
	l = delimiter & list & delimiter
	v = delimiter & value & delimiter
	If instr(1, l, v) then
		IsValueInList = true
	Else
		IsValueInList = false
	End if
End function

Function GetArrayFromSQL(sql)
	Dim RS
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		GetArrayFromSQL = RS.GetRows()
	Else
		GetArrayFromSQL = ""
	End if
End function

Function GetListFromSQL(sql)
	Dim RS
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		list =  RS.GetString(2,,,",","")
		GetListFromSQL = Left(List, Len(list)-1)
	Else
		GetListFromSQL = ""
	End if
End function

Function GetListFromSQLRow(sql)
	Dim RS
	GetInvConnection()
	'Response.Write sql
	
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		GetListFromSQLRow =  RS.GetString(2,1,",","","")
	Else
		GetListFromSQLRow = ""
	End if
End function

Function GetListFromSQLRow_REG(sql)
	Dim RS
	GetRegConnection()
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		GetListFromSQLRow_REG =  RS.GetString(2,1,",","","")
	Else
		GetListFromSQLRow_REG = ""
	End if
End function

'****************************************************************************************
'*	PURPOSE: Create HTML <td>s with caption and input box	                               
'*	INPUT: 	caption text, Name of field variable to populate input box, size, text                              
'*          to appear after the input box, disable the box, mark caption as required		        
'*	OUTPUT: returns HTML <td>s as string                        
'****************************************************************************************		
Function ShowInputBox(fieldCaption, fieldVariableName, size, textAfter, isDisabled, isRequired)
	Dim str
	Dim val
	Dim Disabled
	
	if isRequired then fieldCaption = "<span class=""required"">" & fieldCaption & "</span>"
	
	if isDisabled then 
		Disabled = "disabled onfocus=""blur()"""
	Else
		Disabled = ""
	End if	
	
	fieldValue = iif(isNull(Eval(fieldVariableName)),"",Eval(fieldVariableName))
	
	str = "<td align=right valign=top nowrap width=""150"">"
	str = str & fieldCaption
	str = str & "</td>"
	str = str & "<td>"
	str = str & "<input type=text name=""i" & fieldVariableName & """ size=" & size & " value=""" & replace(fieldValue,"""","&quot;") & """ " & Disabled & " >"
	str = str & textAfter
	str = str & "</td>"
	ShowInputBox = str
End function

Function ShowInputBox2(fieldCaption, fieldVariableName, captionWidth, inputSize, textAfter, isDisabled, isRequired)
	Dim str
	Dim val
	Dim Disabled
	
	if isRequired then fieldCaption = "<SPAN CLASS=""required"">" & fieldCaption & "</SPAN>"
	
	if isDisabled then 
		Disabled = "DISABLED ONFOCUS=""blur()"""
	Else
		Disabled = ""
	End if	
	
	str = "<TD ALIGN=""right"" VALIGN=""top"" NOWRAP WIDTH=""" & captionWidth & """>"
	str = str & fieldCaption
	str = str & "</TD>"
	str = str & "<TD>"
	str = str & "<INPUT TYPE=""text"" NAME=""i" & fieldVariableName & """ SIZE=""" & inputSize & """ VALUE=""" & Eval(fieldVariableName)& """ " & Disabled & " >"
	str = str & textAfter
	str = str & "</TD>"
	ShowInputBox2 = str
End function
Function CheckSelected(str1, str2)
	CheckSelected = ""
	if str1 = str2 then
		CheckSelected = " selected "
	End if
End function

Sub FlushImageToClient(strImageName)		
		Response.Buffer = True
		Response.Flush
		Response.Write String(255," ")
		'!DGB! 10/17/01
		'Path of gif should point to /source/graphics not to application navbuttons path.
		'Unless there is an explicit override from the ini file
		Response.Write "<BR><BR><BR><TABLE Width=500  border=0><tr><td valign=middle align=center><IMG SRC=""" & strImageName & """></td></tr></TABLE>"
		Response.Flush
End Sub
Function GetNewEditDelLinks(elmName, url)
	Dim str
	str = "&nbsp;"
	str = str & GetLink(url & "New", "ValidateClick('" & elmName & "','" & url & "','create'); return false", "New")
	str = str & " | " & GetLink(url & "Edit", "ValidateClick('" & elmName & "','" & url & "','update'); return false", "Edit")
	str= str & " | " & GetLink(url & "Delete", "ValidateClick('" & elmName & "','" & url & "','delete'); return false", "Delete")
				
	GetNewEditDelLinks = str
End Function
			
Function GetLink(href,onclick, text)
	Dim str
	str = "<a class=""MenuLink"" href=""" & href & """ onclick=""" & onclick & """>" & text & "</a>"
	GetLink = str
End function

function OkButton(href, onclick)
	Dim str
	str = "<center><p><a href=""" & href & """ "
	if len(onclick) > 0 then str = str & "onclick=""" & onclick & """ "
	str = str & "><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></p></center>"
	OkButton = str
end function

function GetCancelButton()
	Dim ReturnURL
	ReturnURL = Session("GUIReturnURL")
	if ReturnURL <> "" then
		GetCancelButton = CancelButton(ReturnURL, "")
	else	
		GetCancelButton = CancelButton("Close this window", "top.close();return false")	
	End if
End function

function GetCloseButton()
	Dim ReturnURL
	ReturnURL = Session("GUIReturnURL")
	if ReturnURL <> "" then
		GetCloseButton = CloseButton(ReturnURL, "")
	else	
		GetCloseButton = CloseButton("Close this window", "top.close();return false")	
	End if
End function

function GetOKButton()
	Dim ReturnURL
	ReturnURL = Session("GUIReturnURL")
	if ReturnURL <> "" then
		GetOKButton = OkButton(ReturnURL, "")
	else	
		GetOKButton = OkButton("Close this window", "top.close();return false")	
	End if
End function

function CloseButton(href, onclick)
	Dim str
	str = "<a href=""" & href & """ "
	if len(onclick) > 0 then str = str & "onclick=""" & onclick & """ "
	str = str & " target=_top><img SRC=""../graphics/close_dialog_btn.gif"" border=""0""></a>"
	CloseButton = str
end function

function CancelButton(href, onclick)
	Dim str
	str = "<a href=""" & href & """ "
	if len(onclick) > 0 then str = str & "onclick=""" & onclick & """ "
	str = str & " target=_top><img SRC=""../graphics/cancel_dialog_btn.gif"" border=""0""></a>"
	CancelButton = str
end function

Sub ShowLocationPicker(formelm, elm1, elm2, elm3, size2, size3, isLocSearch)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowLocationPicker2(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, loc)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DeliveryLocationID") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & loc & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowLocationPicker3(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, elm1JS)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

'-- this creates the location picker, but hides the location name box
Sub ShowLocationPicker4(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, elm1JS)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""hidden"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowPlateMapLocationPicker(formelm, elm1, elm2, elm3, size2, size3, isLocSearch)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickPlateMapLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

'MWS: Custom function modelled off of ShowLocationPicker
Sub ShowReadOnlyLocationWithoutPicker(formelm, elm1, elm2, elm3, size2, size3, isLocSearch)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" disabled name=""" & elm2 & """>"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>"  
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "');</script>"
End Sub
Function GetBarcodeIcon()
	Dim out
	out = "<IMG align=""absmiddle"" border=0  src=""/cheminv/graphics/barcode_icon.gif"">"	
	GetBarcodeIcon = out
End function

Function GetNumSelector (name, numstart, numstop, dhtml)
	Dim out
	Dim s
	s = 1
	if  numstart > numstop then s = -1
	out = "<Select name=""" & name & """ size=""1""" & dhtml & ">"
	For i = numstart to numstop step s
		if i < 10 then pad = "0"
		out = out & "<option value=""" & pad & CStr(i) & """>" & pad & CStr(i)
	Next
	out = out & "</select>"
	
	GetNumSelector = out
End function

Function htmlNull(str)
	if IsNull(str) then 
		htmlNull = "&nbsp;"
	else
		htmlNull = str
	end if
End function

Function StrNull(str)
	if IsNull(str) then 
		StrNull = ""
	else
		StrNull = str
	end if
End function

Function LngNull(n)
	if IsNull(n) then 
		LngNull = 0
	else
		LngNull = CLng(n)
	end if
End function

Sub GetURLs(fk_value, table_name, fk_name, url_type, link_text_override, link_title)
	Dim RS
	Dim sql
	Link_text = ""
	Link_image_src = ""
	Link_href = ""
	if url_type = "" then 
		url_type = "NULL"
	else
		url_type = "'" & url_type & "'"
	End if
	GetInvConnection()
	sql = "SELECT url, link_txt, image_src" &_
		  " FROM " & Application("CHEMINV_USERNAME") & ".inv_url" &_
		  " WHERE fk_value=" & fk_value &_
		  " AND upper(fk_name)='" & Ucase(fk_name) & "'" &_ 
		  " AND upper(table_name) ='" & Ucase(table_name) & "'" &_
		  " ORDER BY sort_order"	
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	if NOT (RS.BOF AND RS.EOF) then
		Do while NOT RS.EOF
			Link_image_src = RS("image_src")
			Link_href = RS("URL")
			if Link_href <> "" then
				if link_text_override <> "" then
					Link_text = link_text_override
				Else
					if Link_image_src <> "" then
						Link_text = "<IMG border=0 src=""" & Link_image_src & """ alt=""" & RS("link_txt") & """>"
					else
						Link_text = RS("link_txt")
					End if
				End if
			Response.Write "<BR><a class=""MenuLink"" target=""_new"" href=""" &  Link_href & """ title=""" & link_title & """>" &  Link_text & "</a>" 
			end if	
		RS.MoveNext
		Loop
	End if
	RS.Close
End sub

Function WriteUserProperty(UserID, PropertyName, Value)
	GetInvConnection()
	If GetUserProperty(UserID, PropertyName)= "" then 
		sql = "INSERT INTO " & Application("CHEMINV_USERNAME") & ".inv_user_properties (user_ID_FK, PropertyName, PropertyValue, Time_stamp) VALUES ('" & UserID & "','" & PropertyName & "','" & Value & "', sysdate)"
	Else
		sql = "UPDATE " & Application("CHEMINV_USERNAME") & ".inv_user_properties SET PropertyValue='" & value & "', Time_stamp= sysdate WHERE user_ID_FK='" & UserID & "' AND PropertyName='" & PropertyName & "'"
	End if 
	'Response.Write sql
	'Response.end
	Conn.Execute sql, numrecs ,adExecuteNoRecords
	WriteUserProperty = sql
End function

Function GetUserProperty(UserID, PropertyName)
	GetInvConnection()
	sql = "SELECT PropertyValue FROM " & Application("CHEMINV_USERNAME") & ".inv_user_properties WHERE PropertyName = '" & PropertyName & "' AND user_id_fk='" & UserID & "'"
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		GetUserProperty= RS("PropertyValue").value
	Else
		GetUserProperty = ""
	End if
End Function

Function RTrunc(str, length)
	if len(Trim(str)) < length then
		RTrunc = str
	else
		RTrunc = "..." & Right(str, length)
	end if
End function

Function HasMSDX(CAS, SupplierID, SupplierCatNum)
	Dim FormData
	Dim httpResponse
	Dim tempArr

	ServerName = Application("ACXServerName")
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
	FormData = "SupplierID=" & SupplierID & "&CatalogNumber=" & SupplierCatNum & "&CasNumber=" & CAS & Credentials
	
	httpResponse = CShttpRequest2("POST", ServerName, "chemacx/samsds/samsdsget.asp?killsession=true&returnType=list", "ChemInv", FormData)
	'Response.Write httpresponse
	tempArr= split(httpResponse, ",")
	if IsNumeric(tempArr(0)) then
			If tempArr(0) > 0 then 
				HasMSDX = true
				Exit Function
			End if
	End if
	HasMSDX = False
End Function

Sub ShowMSDXLink(CAS, SupplierID, SupplierCatNum, HasMSDX)
	Dim QS
	
	if HasMSDX  OR HasMSDX = -1 then
		QS = "CasNumber=" & CAS
		SupplierID = Cstr(SupplierID)
		'-- DJP: commented this line for CSBR-59625 fix
		'if SupplierID = "" then SupplierID = 0 
		QS = QS & "&SupplierID=" & SupplierID
		if SupplierCatNum <> "" then QS = QS & "&CatalogNumber=" & SupplierCatNum 
		Response.Write "<a href=""http://" &  Application("ACXServerName") & "/chemacx/samsds/samsdsget.asp?killsession=true&" & QS & """ class=""MenuLink"" target=""_new"" title=""View materials safety data from MSDS" & vblf & """>MSDS</a>"
	Else
		Response.Write "<a href=""#"" disabled title=""No materials safety data available from MSDS"" class=""MenuLink"" onclick=""return false;"">MSDS</a>"
	End if
End Sub

Sub ShowSearchButton()
	if lcase(Request("formmode"))= "refine" then 
		buttonName="apply_btn.gif"
		action = "apply"
	else
		buttonName="search_btn.gif"
		action = "search"
	end if
	Response.Write "<input type=""image"" border=""0"" src=""/cheminv/graphics/" & buttonName & """>"
End sub

'****************************************************************************************
'*	PURPOSE: Implements the IIF function in vbscript
'*	INPUT: 	boolExp: boolean expression to be evaluated
'*			trueStr: value of the function if true
'*			falseStr: value of the function if false
'*	OUTPUT: returns a string                        
'****************************************************************************************		
function iif(boolExp, trueStr, falseStr)
  if boolExp then
    iif = trueStr
  else 
    iif = falseStr
  end if
end function

'****************************************************************************************
'*	PURPOSE: Generate x new text boxes
'*	INPUT: name prefix, number of new textboxes, initial value
'*	OUTPUT: x number of distinct text boxes with the specified initial value
'****************************************************************************************
Function GenerateFields(CaptionPrefix, CaptionSuffix, NamePrefix, NumFields, arrValues)
	Dim html
	html = ""
	for i = 1 to NumFields
		html = html &"<tr><td align=""right"" nowrap><span class=""required"">" & CaptionPrefix & i & " " & CaptionSuffix & ":</span></td>"
		html = html & "<td><input TYPE=""text"" SIZE=""10"" Maxlength=""50"" NAME=""" & NamePrefix & i & """ VALUE=""" & arrValues(i-1) & """></td></tr>"
	next
	GenerateFields = html
End function			

'Takes a reference to a shaped recordset and returns a string of parent 
'well info in the format:(comma separated list)|(comma separated list)... 
'in the following order:'ParentWellLinks|ParentPlateLinks|ParentRegBatchIDs|ParentCompoundIDs
function GetParentWellInfoString(ByRef rsWellParent, delimiter)

	ParentWellLinks = ""
	ParentPlateLinks = ""
	ParentRegBatchIDs = ""
	ParentCompoundIDs = ""
	while not (rsWellParent.BOF or rsWellParent.EOF)
		Set rsParent = rsWellParent("rsParent").value
		while not (rsParent.BOF or rsParent.EOF)
			ParentWellLinks = ParentWellLinks & rsParent("ParentWellLink") & delimiter
			ParentPlateLinks = ParentPlateLinks & rsParent("ParentPlateLink") & delimiter
			'ParentRegBatchIDs = ParentRegBatchIDs & rsParent("reg_batch_id") & delimiter
			'ParentCompoundIDs = ParentCompoundIDs & rsParent("compound_id_fk") & delimiter
			rsParent.movenext
		wend
		rsWellParent.movenext
	wend						
	if len(ParentWellIDs) > 0 then
		ParentWellLinks = left(ParentWellLinks,(len(ParentWellLinks)-len(delimiter)))
		ParentPlateLinks = left(ParentPlateLinks,(len(ParentPlateLinks)-len(delimiter)))
		ParentRegBatchIDs = left(ParentRegBatchIDs,(len(ParentRegBatchIDs)-len(delimiter)))
		ParentCompoundIDs = left(ParentCompoundIDs,(len(ParentCompoundIDs)-len(delimiter)))
	end if
	
	GetParentWellInfoString = ParentWellLinks & "|" & _
								ParentPlateLinks & "|" & _
								ParentRegBatchIDs & "|" & _
								ParentCompoundIDs
		
end function

' Used to renew the credential cookies that control the cs_security session
' Uses cookie expiration time when persistent cookies are allowed.
' Uses a timestamp cookie that tracks UTC time when in memory cookies are used
Sub ProlongCookie(cookieName, value, minutes)
	Response.Cookies(cookieName) = value
	Response.Cookies(cookieName).Path= "/"
	if Application("PERSIST_AUTHENTICATION_COOKIES") then
		Response.Cookies(cookieName).expires = DateAdd("n", minutes, now())
	end if
	' This cookie tracks the expiration in UTC time
	Response.Cookies("cstimestamp") = getUtcNow(now()) + (cLng(minutes) * 60 * 1000)
	Response.Cookies("cstimestamp").Path = "/"
End sub


'-- Returns a comma delimited list from the dictionary
Function Dict2List(ByRef oDict, numPerRow)
	checkRow = true
	list = ""
	if isEmpty(numPerRow) or isNull(numPerRow) then checkRow = false
	rowCount = 1
	for each key in oDict
		list = list & key & ","
		if checkRow and rowCount = numPerRow then 
			list = list & "<BR>"
			rowCount = 0
		end if
		rowCount = rowCount + 1
	next
	if len(list) > 0 then list = left(list,(len(list)-1))
	Dict2List = list
End Function

'-- returns string that can be displayed in an HTML text field
Function htmlValue(str)
	htmlValue = replace(str,"""","&quot;")
End Function

Function GetCDAX()
	on error resume next
	Set GetCDAX = Server.CreateObject(Application("CDAX_ProgID"))
	if err then
		Response.Write "Error while initializing CDAX using ProgID= " & Application("CDAX_ProgID")
		Response.end
	end if	
End Function

' Inventory version of this core function
' Creates a gif file of a predefined size based on a base64CDX string
Sub ConvertCDXtoGIF_Inv (Byval filePath, Byval base64CDX, Byval gifWidth,Byval gifHeight)
		set oCDAX =  GetCDAX()
		oCDAX.DataEncoded = true
		if len(base64CDX) > 0 then oCDAX.Data("chemical/x-cdx") = base64CDX
		oCDAX.dataEncoded = false
		oCDAX.SaveAs filePath,1715882311,72,gifWidth, gifHeight
		Set oCDAX = Nothing
End Sub

'-- builds the list of options for a custom list
function GetCustomListOptions(customFieldType, customFieldKey, selectedValue, overrideDefaultValue)
	defaultValue = ""
	'list = custom_lists_dict.Item("CUSTOM_FIELDS." & key)
	list = custom_lists_dict.Item(customFieldType & "." & customFieldKey)
	arrList1 = split(list, "!")
	if ubound(arrList1) = 1 then defaultValue = arrList1(1)
	arrList2 = split(arrList1(0),"|")
	options = ""
	if not isNull(overrideDefaultValue) then
		options = options & "<OPTION SELECTED VALUE="""">" & overrideDefaultValue & "</OPTION>"
	end if
	for i=0 to ubound(arrList2)
		selectText = ""
		if isNull(overrideDefaultValue) then
			if selectedValue <> "" and not isEmpty(selectedValue) then
				if selectedValue = arrList2(i) then selectText = " SELECTED"
			elseif arrList2(i) = defaultValue then 
				selectText = " SELECTED"
			end if
		end if
		options = options & "<OPTION VALUE=""" & arrList2(i) & """" & selectText & ">" & arrList2(i) & "</OPTION>"
	next
	GetCustomListOptions = options
end function 

%>
<SCRIPT LANGUAGE=jscript RUNAT=Server>
function getUtcNow(strDate){
	return Date.parse(strDate)
}
</SCRIPT>