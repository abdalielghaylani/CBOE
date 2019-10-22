<%Response.Charset="UTF-8"%>
<%
Dim Link_href
Dim Link_Text
Dim Link_image_src
''Redirect page to HTTPS

if Ucase(Application("FORCEHTTPS")) = "TRUE" then
	if Request.ServerVariables("HTTPS") = "off" then 
                method = Request.ServerVariables("REQUEST_METHOD") 
                srvname = Request.ServerVariables("SERVER_NAME") 
                scrname = Request.ServerVariables("SCRIPT_NAME") 
                sRedirect = "https://" & srvname & scrname 
                sQString = Request.Querystring 
                if Len(sQString) > 0 Then sRedirect = sRedirect & "?" & sQString
                Response.Redirect(sRedirect)  
           end if
           
end if

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


'SM-Fixing CSBR-149133
'UserID = CryptVBS(Request.Cookies("CS_SEC_UserID").Item, Request.Cookies("CS_SEC_UserName").Item)
SSOticket= Request.Cookies("COESSO")
if isObject(session) then 
    if session("SSOAuthticket")<> Request.Cookies("COESSO") then
       Response.Cookies("COESSO")=SESSION("SSOAuthticket")
       SSOticket=SESSION("SSOAuthticket")
    end if
    if SSOticket<>"" and (len(UserName) = 0 or len(UserID) = 0) then 
        set SSOobj1 = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
        UserName = SSOobj1.GetUserFromTicket(SSOticket)
        UserID = getUserIDfromString(SSOobj1.GetUserDataFromTicket(SSOticket))        
        Response.Cookies("CS_SEC_UserName")=UserName
        Response.Cookies("CS_SEC_UserID")=UserID
    end if
end if
UserName = Request.Cookies("CS_SEC_UserName")
UserID = Request.Cookies("CS_SEC_UserID")
Session("UserIDcheminv") = UserID
'Restamp Credentials cookie
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
   	if err then 
	    if pHostName ="<INV_HTTP_SERVER_NAME>" or err.number="-2147467259" then     
           call ShowChemInvConnectionError
           Response.End
	    end if     
	end if 
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
	'CSBR# 139459
	'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
	Dim portNumber
	portNumber = Request.ServerVariables("SERVER_PORT")
    on error resume next 
    if portNumber = "80" or inStr(pHostName, ":" & portNumber) > 0 then
	    URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget
	else
	    URL = Application("SERVER_TYPE") & pHostName & ":" & portNumber & "/" & pTarget
	end if
	'End of change
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
    '-- set default timeouts, mainly I want to increase the default send/receive timeout from 30s to 5min
    SendTimeout = 300 * 1000
    ReceiveTimeout = 300 * 1000
	ResolveTimeout = 5 * 1000
	ConnectTimeout = 5 * 1000
	objXmlHttp.setTimeouts ResolveTimeout, ConnectTimeout, SendTimeout, ReceiveTimeout

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
	if err then 
	    if pHostName ="<INV_HTTP_SERVER_NAME>" or err.number="-2147467259" then     
           call ShowChemInvConnectionError
           Response.End
	    end if     
	end if 
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
	'CSBR# 139459
	'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
	Dim portNumber
	portNumber = Request.ServerVariables("SERVER_PORT")  
	if portNumber = "80" or inStr(pHostName, ":" & portNumber) > 0 then
	    URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget
	else	   
	    URL = Application("SERVER_TYPE") & pHostName & ":" & portNumber & "/" & pTarget
	end if
	'End of Change
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

    on error resume next 
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
	if err then 
	    if pHostName ="<INV_HTTP_SERVER_NAME>" or err.number="-2147467259" then     
           call ShowChemInvConnectionError
           Response.End
	    end if     
	end if 
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
	Dim ResolveTimeout
	Dim ConnectTimeout
	Dim SendTimeout
	Dim ReceiveTimeout

	URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget

	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")
	'default connect and resolve timeouts
	ResolveTimeout = 5 * 1000
	ConnectTimeout = 5 * 1000
	objXmlHttp.setTimeouts ResolveTimeout, ConnectTimeout, SendTimeout, ReceiveTimeout

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
	str = str & "<td class=""grayBackground"" align=right>"
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
	str = str & "<td class=""grayBackground"" align=right>"
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

'****************************************************************************************
Function LeftTruncateInSpan(strText, Length, id)
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
		str = str &  "..." & right(strText, Length-3) 
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "&nbsp;"
	str = str & "</span>"
	LeftTruncateInSpan = str
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
	if Len(fieldCaption) > 0 then
		str = "<td align=right valign=top nowrap>"
		str = str & fieldCaption
		str = str & "</td>"
	end if
	str = str & "<td>"
	str = str & ShowSelectBox(name, SelectedValue, sql)
	str = str & "</td>"
	ShowPickList = str
End function

Function ShowPickList2(fieldCaption, name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, ChangeScript)
	Dim str
	if Len(fieldCaption) > 0 then
		str = "<td align=right valign=top nowrap>"
		str = str & fieldCaption
		str = str & "</td>"
	end if
	str = str & "<td>"
	str = str & ShowSelectBox3(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, ChangeScript)
	str = str & "</td>"
	ShowPickList2 = str
End function

Function ShowPickList3(fieldCaption, name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, ChangeScript, size, ismulti, EmptyMessage)
	Dim str
	if Len(fieldCaption) > 0 then
		str = "<td align=right valign=top nowrap>"
		str = str & fieldCaption
		str = str & "</td>"
	end if
	str = str & "<td>"
	str = str & ShowMultiSelectBox2(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, ChangeScript, EmptyMessage)
	str = str & "</td>"
	ShowPickList3 = str
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
	'Response.Write(sql)
	Set RS = Conn.Execute(sql)
	ShowSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "", "", 1, false, "","")
End function

Function ShowSelectBox2(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowSelectBox2 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, 1, false, "", "")
End function

Function ShowSelectBox3(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, ChangeScript)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowSelectBox3 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, 1, false, ChangeScript,"")
End function

Function ShowMultiSelectBox(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowMultiSelectBox = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, "","")
End function

Function ShowMultiSelectBox2(name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, changeScript, EmptyMessage)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowMultiSelectBox2 = BuildSelectBox2(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismulti, changeScript ,EmptyMessage)
End function

Function ShowRPTSelectBox(name, SelectedValue, sql)
	Dim RS
	Dim strSelected
	GetInvConnection()
	'Response.Write sql
	'Response.end
	Set RS = Conn.Execute(sql)
	ShowRPTSelectBox = BuildSelectBox(RS, name, SelectedValue, 0, "Select One", "NULL",1,false,"","")
End function

Function RepeatString(n, str)
	For i = 1 to n
		tempstr = tempstr + str
	Next
	RepeatString = tempstr
End function

Function BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismultiple, ChangeScript, EmptyMessage)
	Dim str
	Dim DisplayText
	Dim multiple
   
	If ismultiple then
		multiple = " multiple"
	Else
		multiple = ""
	End if

	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT size=""" & size & """ name=""" & name & """" & multiple & " onchange=""" & ChangeScript & """>"
	if RS.EOF AND RS.BOF AND Len(FirstOptionText) = 0 then
		if EmptyMessage <> "" then
			str = str &  "<OPTION value=""NULL"">" & EmptyMessage
		else
			str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!"
		end if
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
            AddValueToList = true
            if(name = "ReportFormat") then AddValueToList = IsReportFormatAvailable(theValue)
            
            if(AddValueToList) then
			    'If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then
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
            End if
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	BuildSelectBox = str
End function

Function IsReportFormatAvailable(ReportFormat)
    'Dim AllReportFormats = "SNP,RTF,XLS,XLSX,PDF"
    IsReportFormatAvailable = false
    Select CASE ReportFormat
        'Case "SNP" ' Not sure why this case was added; Snapshot should be working in any case.
            'if (Application("MSOfficeVersion") < 12) then IsReportFormatAvailable = true
        Case "XLSX"
            if (Application("MSOfficeVersion") > 11) then IsReportFormatAvailable = true
        Case Else
            IsReportFormatAvailable = true
    End Select
End Function

Function BuildSelectBox2(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismultiple, ChangeScript, EmptyMessage)
	Dim str
	Dim DisplayText
	Dim multiple

	If ismultiple then
		multiple = " multiple"
	Else
		multiple = ""
	End if

	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT size=""" & size & """ name=""" & name & """ id=""" & name & """" & multiple & " onchange=""" & ChangeScript & """>"
	if RS.EOF AND RS.BOF AND Len(FirstOptionText) = 0 then
		if EmptyMessage <> "" then
			str = str &  "<OPTION value=""NULL"">" & EmptyMessage
		else
			str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!"
		end if
	Else

		If Len(FirstOptionText) > 0 then
			If isValueInList(SelectedValue,FirstOptionValue,",") then
				strSelected = "Selected=""True"""
			End if
			str = str &  "<option " & strSelected & " value=""" & FirstOptionValue &  """>" & FirstOptionText & "</option>"
		End if
		Do While NOT RS.EOF
			strSelected = ""
			theValue = RS("Value").value
			'If LCase(Cstr(theValue)) = LCase(CStr(SelectedValue)) then
			If isValueInList(SelectedValue,theValue,",") then
				strSelected = "Selected=""True"""
			End if
			DisplayText = RS("DisplayText")

			If TruncateLength > 0 AND Len(DisplayText) > TruncateLength then
				DisplayText =  Left(DisplayText, TruncateLength-3) & "..."
			Else
				DisplayText =  DisplayText
			End if
			str = str & "<option " & strSelected & " value=""" & RS("Value") & """ id=""" & DisplayText &  """>" & DisplayText & "</option>"
			if RS("DefaultValue") <> "" then
				strDefValue = strDefValue & "<input type=""hidden"" name=""Rack" & RS("Value") & "Def"" value=""" & RS("DefaultValue") & """>"
			end if
			RS.MoveNext
		Loop
	End if
	str = str & "</select>" & strDefValue
	BuildSelectBox2 = str
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
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		list =  RS.GetString(2,,,",","")
		GetListFromSQL = Left(List, Len(list)-1)
	Else
		GetListFromSQL = ""
	End if
End function

Function GetListFromSQL1(sql, DataLocationID)
	Dim RS
	Dim cmd
	GetInvConnection()
	set cmd = server.createobject("ADODB.Command")
	cmd.CommandText = sql
	cmd.ActiveConnection = Conn
	Cmd.Parameters.Append Cmd.CreateParameter("DataLocationID", 131, 1, 0, DataLocationID)
	Set RS = cmd.execute
	if NOT (RS.EOF AND RS.BOF) then
		list =  RS.GetString(2,,,",","")
		GetListFromSQL1 = Left(List, Len(list)-1)
	Else
		GetListFromSQL1 = ""
	End if
End function

Function GetListFromSQLRow1(sql, LocationID)
	Dim RS
	Dim cmd
	GetInvConnection()
	'Response.Write sql
	set cmd = server.createobject("ADODB.Command")
	cmd.CommandText = sql
	cmd.ActiveConnection = Conn
	cmd.Parameters.Append cmd.CreateParameter("@Location_ID", adInteger, adParamInput, , LocationID)
	Set RS = cmd.execute
	'Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
	    result = RS.GetString(2,1,",","","")
	    result = left(result,len(result)-1)
		GetListFromSQLRow1 = result
	Else
		GetListFromSQLRow1 = ""
	End if
End function

Function GetListFromSQLRow2(sql, LocationBarcode)
	Dim RS
	Dim cmd
	GetInvConnection()
	'Response.Write sql
	set cmd = server.createobject("ADODB.Command")
	cmd.CommandText = sql
	cmd.ActiveConnection = Conn
	cmd.Parameters.Append cmd.CreateParameter("@Location_Barcode", adVarchar, adParamInput, 50, LocationBarcode)
	Set RS = cmd.execute
	'Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
	    result = RS.GetString(2,1,",","","")
	    result = left(result,len(result)-1)
		GetListFromSQLRow2 = result
	Else
		GetListFromSQLRow2 = ""
	End if
End function

Function GetListFromSQLRow(sql)
	Dim RS
	GetInvConnection()
	'Response.Write sql

	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
	    result = RS.GetString(2,1,",","","")
	    result = left(result,len(result)-1)
		GetListFromSQLRow = result
	Else
		GetListFromSQLRow = ""
	End if
End function

Function GetListFromSQLRS(sql)
    Dim RS
	
	GetInvConnection()
	'Response.Write sql	

	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
        result = RS.GetString(adClipString,,"|",",","")
        result = left(result,len(result)-1)
		GetListFromSQLRS = result		
	Else	    
		GetListFromSQLRS = ""		
	End if
End function

Function GetListFromSQLRow_REG(sql)
	Dim RS
	GetRegConnection()
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
	    result = RS.GetString(2,1,",","","")
        result = left(result,len(result)-1)
		GetListFromSQLRow_REG =  result
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

	if Len(fieldCaption) > 0 then
		if isRequired then fieldCaption = "<span class=""required"">" & fieldCaption & "</span>"
		str = "<td align=right valign=top nowrap width=""150"">"
		str = str & fieldCaption
		str = str & "</td>"
	end if

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
	str = str & "><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></p></center>"
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
	str = str & " target=_top><img SRC=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>"
	CloseButton = str
end function

function CancelButton(href, onclick)
	Dim str
	str = "<a href=""" & href & """ "
	if len(onclick) > 0 then str = str & "onclick=""" & onclick & """ "
	str = str & " target=_top><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0""></a>"
	CancelButton = str
end function

Sub ShowLocationPicker(formelm, elm1, elm2, elm3, size2, size3, isLocSearch)
	ServerName = Application("InvServerName")
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<span class=""required"" id=""LocationPathSpan"" title="""">"
	'CSBR 144145 SJ
	'To restore the location id from the saved locations menu in the search form
	'Start of change 
	If Not IsEmpty(Request.QueryString("special")) then
        if formgroup = "base_form_group" then 'CBOE-1575 SJ To restore the location barcode
           dbvalue =  Session("SearchData" & "inv_containers_subsearch_alias.Location_ID_FK" & dbkey & formgroup)
        elseif formgroup = "containers_np_form_group" then
	        dbvalue = Session("SearchData" & "inv_containers.location_id_fk" &  dbkey & formgroup)
        elseif formgroup = "plates_form_group" then
            dbvalue = Session("SearchData" & "inv_plates.location_id_fk" &  dbkey & formgroup)
        end if
	    arr1 = Split(dbvalue,"=")
	    If UBound(arr1) > 0 Then 'CSBR 146419 SJ : Checking the array is empty.
            If UBound(arr1) = 1 then
                Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
                QueryString = "LocationID=" & Trim(arr1(1))
                QueryString = QueryString & Credentials
                loc = CShttpRequest2("POST",ServerName,"/cheminv/api/GetLocationFromID.asp","ChemInv",QueryString) 'CBOE-1575 SJ To restore the location barcode in the textbox when refining the query
                arr1 = Split(loc,",")
	            Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" value=""" & Trim(arr1(1)) & """ name=""" & elm2 & """ onfocus=""UpdateLocationPickerFromBarCode(this.value," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"" onblur=""UpdateLocationPickerFromBarCode(this.value," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
            ElseIf UBound(arr1) = 2 then 'CBOE-1433 SJ : to remove the invalid text in the location id text box when search sub location is selected.
                Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
                QueryString = "LocationID=" & replace(Trim(arr1(2)),")","")
                QueryString = QueryString & Credentials
                loc = CShttpRequest2("POST",ServerName,"/cheminv/api/GetLocationFromID.asp","ChemInv",QueryString) 'CBOE-1575 SJ To restore the location barcode in the textbox when refining the query
                arr1 = Split(loc,",")
                Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" value=""" & Trim(arr1(1)) & """ name=""" & elm2 & """ onfocus=""UpdateLocationPickerFromBarCode(this.value," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"" onblur=""UpdateLocationPickerFromBarCode(this.value," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
            End If
	    Else
	        Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	    End If
	Else
	    Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); if(" & formelm & "['" & elm3 & "'].value == '') {return false;} else{ return true;}"">"    'CBOE-1725 added condition to check whether location is exists or not
	End If
	'End of change
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;</span><a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); if(" & formelm & "['"& elm2 & "']) {" & formelm & "['"& elm2 & "'].focus();}</script>"
End Sub

Sub ShowLocationPicker2(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, loc)
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
'-- CSBR ID:132599
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Show the browse link if only the user is allowed to set the delivery location
'-- Date: 11/02/2010
	if Application("ALLOW_USER_TO_SET_DELIVERY_LOCATION") = "1" then
		disabled = ""
	else
		disabled = " disabled "
	end if
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')""" & disabled & ">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;"
	if Application("ALLOW_USER_TO_SET_DELIVERY_LOCATION") = "1" then
		Response.Write "<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DeliveryLocationID") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
	end if
'End of Change #132599#
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & loc & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowLocationPicker3(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, elm1JS)
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
	'Response.Write "<input type=""text"" size=""" & size2 &""" Value="""" onpropertychange=""" & elm1JS & ";"" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" size=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	'Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled onpropertychange=""if(document.form1.IsRack.value=='true'){document.form1.lpLocationName.disabled = true;UpdateLocationPickerFromID(document.form1.iLocationID.value,document.form1,'iLocationID','lpLocationBarCode', 'lpLocationName');};"">&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
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

'-- allows you to set the location id of initial location and the default location that the location browser opens to
Sub ShowLocationPicker5(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, loc,isMultiSelectRacks)
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation1('" & id & "', '" & loc & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "'," & lcase(Cstr(isMultiSelectRacks )) & ");return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & loc & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowLocationPicker6(formelm, elm1, elm2, elm3, size2, size3, isLocSearch,isMultiSelectRacks )
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """>"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation1('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "'," & lcase(Cstr(isMultiSelectRacks )) & ");return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); if(" & formelm & "['"& elm2 & "']) {" & formelm & "['"& elm2 & "'].focus();}</script>"
End Sub

Sub ShowLocationPicker7(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, elm1JS, DefaultLocation)
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
	'Response.Write "<input type=""text"" size=""" & size2 &""" Value="""" onpropertychange=""" & elm1JS & ";"" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" size=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	'Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled onpropertychange=""if(document.form1.IsRack.value=='true'){document.form1.lpLocationName.disabled = true;UpdateLocationPickerFromID(document.form1.iLocationID.value,document.form1,'iLocationID','lpLocationBarCode', 'lpLocationName');};"">&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & DefaultLocation & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & DefaultLocation & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
End Sub

Sub ShowLocationPicker8(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, elm1JS, isMultiSelectRacks )
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')"">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation1('" & id & "', '" & Session("DefaultLocation") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "'," & lcase(Cstr(isMultiSelectRacks )) & ");return false"" tabindex=""-1"">Browse</a>"
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & LocationID & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); if(" & formelm & "['"& elm2 & "']) {" & formelm & "['"& elm2 & "'].focus();}</script>"
End Sub

Sub ShowLocationPicker9(formelm, elm1, elm2, elm3, size2, size3, isLocSearch, loc, elm1JS)
	Response.Write "<input type=""hidden"" Value="""" name=""isRack"">"
	Response.Write "<input type=""hidden"" Value="""" name=""" & elm1 & """ onpropertychange=""" & elm1JS & ";"">"
'-- CSBR ID:132599
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Show the browse link if only the user is allowed to set the delivery location
'-- Date: 11/02/2010
	if Application("ALLOW_USER_TO_SET_DELIVERY_LOCATION") = "1" then
		disabled = ""
	else
		disabled = " disabled "
	end if
	Response.Write "<input TYPE=""Text"" SIZE=""" & size2 &""" Value="""" name=""" & elm2 & """ onchange=""UpdateLocationPickerFromBarCode(this.value, " & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "')""" & disabled & ">"
	Response.Write "<input type=""text"" name=""" & elm3 & """ size=""" & size3 & """ value="""" disabled>&nbsp;"
	if Application("ALLOW_USER_TO_SET_DELIVERY_LOCATION") = "1" then
		Response.Write "<a class=""MenuLink"" HREF=""Select%20a%20location"" onclick=""PickLocation('" & id & "', '" & Session("DeliveryLocationID") & "'," & lcase(Cstr(isLocSearch)) & ",'" & formelm & "','" & elm1 & "','" & elm2 & "','" & elm3 & "');return false"" tabindex=""-1"">Browse</a>"
	end if
'End of Change #132599#
	Response.Write "<script language=javascript>UpdateLocationPickerFromID('" & loc & "'," & formelm & ",'" & elm1 & "','" & elm2 & "', '" & elm3 & "'); " & formelm & "['"& elm2 & "'].focus();</script>"
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

Sub GetURLs(fk_value, table_name, fk_name, url_type, link_text_override, link_title, section_title)
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
	isFirst = true
	if NOT (RS.BOF AND RS.EOF) then
		Do while NOT RS.EOF
            '-- write the section header
            if isFirst then
                Response.Write("<div class=""tasktitle"">" & section_title & "</div>")
            end if
			Link_image_src = RS("image_src")
			Link_href = RS("URL")
			if Link_href <> "" then
				if link_text_override <> "" then
					Link_text = link_text_override
				Else
					if Link_image_src <> "" then
						Link_text = "<IMG border=0 src=""" & Link_image_src & """ title=""" & RS("link_txt") & """ alt=""" & RS("link_txt") & """>"
					else
						Link_text = RS("link_txt")
					End if
				End if
			    Response.Write "<a class=""MenuLink"" target=""_new"" href=""" &  Link_href & """ title=""" & link_title & """>" &  Link_text & "</a><br/>" 
			    isFirst = false
			end if
		RS.MoveNext
		Loop
	End if
	RS.Close
End sub

Function WriteUserProperty(UserID, PropertyName, Value)
	GetInvConnection()
	dim sql1 'CBOE-346 SJ Declaring local variable so that the search sql string may not get replaced.
	If GetUserProperty(UserID, PropertyName)= "" then
		sql1 = "INSERT INTO " & Application("CHEMINV_USERNAME") & ".inv_user_properties (user_ID_FK, PropertyName, PropertyValue, Time_stamp) VALUES ('" & UserID & "','" & PropertyName & "','" & Value & "', sysdate)"
	Else
		sql1 = "UPDATE " & Application("CHEMINV_USERNAME") & ".inv_user_properties SET PropertyValue='" & value & "', Time_stamp= sysdate WHERE user_ID_FK='" & UserID & "' AND PropertyName='" & PropertyName & "'"
	End if
	'Response.Write sql1
	'Response.end
	Conn.Execute sql1, numrecs ,adExecuteNoRecords
	WriteUserProperty = sql1
End function

Function GetUserProperty(UserID, PropertyName)
	GetInvConnection()
	dim sql2 'CBOE-346 SJ Declaring local variable so that the search sql string may not get replaced.
	sql2 = "SELECT PropertyValue FROM " & Application("CHEMINV_USERNAME") & ".inv_user_properties WHERE PropertyName = '" & PropertyName & "' AND user_id_fk='" & UserID & "'"
	'Response.Write sql2
	'Response.end
	Set RS = Conn.Execute(sql2)
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
	Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
	FormData = "SupplierID=" & SupplierID & "&CatalogNumber=" & SupplierCatNum & "&CasNumber=" & CAS & Credentials

	httpResponse = CShttpRequest2("POST", ServerName, "chemacx/msdx/msdxget.asp?killsession=true&returnType=list", "ChemInv", FormData)
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
		Response.Write "<a href=" & Application("SERVER_TYPE") &  Application("ACXServerName") & "/chemacx/msdx/msdxget.asp?killsession=true&" & QS & """ class=""MenuLink"" target=""_new"" title=""View materials safety data from MSDX" & vblf & "(Adobe Acrobat required)"">MSDX</a>"
	Else
		Response.Write "<a href=""#"" disabled title=""No materials safety data available from MSDX"" class=""MenuLink"" onclick=""return false;"">MSDX</a>"
	End if
End Sub

Function HasMSDS(CAS, SupplierID, SupplierCatNum)
	Dim FormData
	Dim httpResponse
	Dim tempArr

	ServerName = Application("ACXServerName")
	Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
	FormData = "SupplierID=" & SupplierID & "&CatalogNumber=" & SupplierCatNum & "&CasNumber=" & CAS & Credentials
	
	httpResponse = CShttpRequest2("POST", ServerName, "chemacx/samsds/samsdsget.asp?killsession=true&returnType=list", "ChemInv", FormData)
	tempArr= split(httpResponse, ",")
	if IsNumeric(tempArr(0)) then
			If tempArr(0) > 0 then 
				HasMSDS = true
				Exit Function
			End if
	End if
	HasMSDS = False
End Function

Sub ShowMSDSLink(CAS, SupplierID, SupplierCatNum, HasMSDS)
	Dim QS
	
	if HasMSDS  OR HasMSDS = -1 then
		QS = "CasNumber=" & CAS
		SupplierID = Cstr(SupplierID)
		QS = QS & "&SupplierID=" & SupplierID
		if SupplierCatNum <> "" then QS = QS & "&CatalogNumber=" & SupplierCatNum 
		Response.Write "<a href=" & Application("SERVER_TYPE") &  Application("ACXServerName") & "/chemacx/samsds/samsdsget.asp?killsession=true&" & QS & """ class=""MenuLink"" target=""_new"" title=""View materials safety data from MSDS" & vblf & """>MSDS</a>"
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
'************************************************************************************************
'*	PURPOSE: Generate x new hidden fields and fixed amount of text boxes 
'*	INPUT: name prefix, number of new textboxes, initial value
'*	OUTPUT: x number of distinct text boxes with the specified initial value and hidden fields 
'*  NOTES:  The OnInputChangeFunction must accept a single integer parameter indicating which input field
'*  is being modified.
'************************************************************************************************
Function GenerateFields3(CaptionPrefix, CaptionSuffix, NamePrefix, NumFields, arrValues,RecordsPerPage,PageNum,OnSelectChangeFunction,OnInputChangeFunction)
	Dim html
	html = ""
	dim totalPages
	for i = 1 to NumFields
			html = html & "<input TYPE=""hidden"" NAME=""" & NamePrefix & i & """ VALUE=""" & arrValues(i-1) & """>"
	next
	if cint(NumFields) > cint(RecordsPerPage) then 
	    html = html &"<tr><TD>Page:&nbsp; "
		totalPages = NumFields/RecordsPerPage
		if   NumFields mod RecordsPerPage <> 0 then 
		    totalPages = totalPages+1    
		end if 		
	   html = html & "<select NAME=""selectpage"" onchange=""" & OnSelectChangeFunction & """>"
	   for i=1 to totalPages
	    html = html & "<option value= """ & i & """ >" & i & "</option>"
	   next
	   html = html & "</select></td></tr>"
	end if 
	TotalDisplayFields =  RecordsPerPage
	if cint(NumFields) < cint(RecordsPerPage) then TotalDisplayFields= cint(NumFields) 
	for i = 1 to TotalDisplayFields
		html = html &"<tr><td align=""right"" nowrap><span class=""required"" id=""" & "caption" & i & """ class=""required"" style=""display:block"">" & CaptionPrefix & i & " " & CaptionSuffix & ":</span></td>"
		html = html & "<td><span id=""" & "textfield" & i & """ class=""required"" style=""display:block""><input TYPE=""text"" SIZE=""10"" Maxlength=""50"" NAME=""" & "container" &  i & """ VALUE=""" & arrValues(i-1) & """ onchange=""" & OnInputChangeFunction & "(" & i & " );""></span></td></tr>"
	next
	GenerateFields3 = html
End function
'****************************************************************************************
'*	PURPOSE: Generate x new text boxes
'*	INPUT: name prefix, number of new textboxes, initial value, function to be call on change event
'*	OUTPUT: x number of distinct text boxes with the specified initial value
'****************************************************************************************
Function GenerateFields2(CaptionPrefix, CaptionSuffix, NamePrefix, NumFields, arrValues,FunctionName)
	Dim html
	html = ""
	for i = 1 to NumFields
		html = html &"<tr><td align=""right"" nowrap><span class=""required"">" & CaptionPrefix & i & " " & CaptionSuffix & ":</span></td>"
		html = html & "<td><input TYPE=""text"" SIZE=""10"" Maxlength=""50"" NAME=""" & NamePrefix & i & """ VALUE=""" & arrValues(i-1) & """ onchange=""" & FunctionName & "();""></td></tr>"
	next
	GenerateFields2 = html
End function
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
	ProlongSSOTicketCookie()
	' This cookie tracks the expiration in UTC time
	Response.Cookies("cstimestamp") = getUtcNow(now()) + (cLng(minutes) * 60 * 1000)
	Response.Cookies("cstimestamp").Path = "/"
End sub

Sub ProlongSSOTicketCookie()
	if isObject(session) then 
	  'DGB  prevent SOAP error when authentication ticket is not there
	  if Session("SSOAuthticket") <> "" then
        if Application("authCookieName") <> "" and UCase(Application("AUTHENTICATION_MODE")) = "COELDAP" then

                authcookieName = Application("authCookieName")
                if DateDiff("n", Now(), Session("SSOExpiration")) < 5 then 
                    set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
                    
                    Session("SSOAuthticket") = SSOobj.RenewTicket(Session("SSOAuthticket"))
                    Session("SSOExpiration") = SSOobj.GetTicketExpirationDate(Session("SSOAuthticket"))
                    
                    Response.Cookies(authcookieName).Path = "/"                 
                    Response.Cookies(authcookieName) = Session("SSOAuthticket") 
                    set SSOobj = nothing
                end if
        end if
	   end if	
    end if
end sub


function renderBoxHeader (imgSrc, title, additional, tableWidth)
	renderBoxHeader = "" & vbcrlf  & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<!-- SECTION HEADER: " & uCase(title) & " -->" & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"" width=""" & width & """>" & vbcrlf &_
"	<tr>" & vbcrlf &_
"		<td><img border=""0"" src=""/cheminv/graphics/box/hbl.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"		<td class=""boxHeader""><table cellpadding=0 cellspacing=0><tr><td>" & imgSrc & "&nbsp;</td><td class=""grayBackground""><font size=""1""><strong>" & title & "</strong>&nbsp;&nbsp;&nbsp;</td><td align=""right""><font size=""1"">" & additional & "</td></tr></table></td>"  & vbcrlf &_
"		<td><img border=""0"" src=""/cheminv/graphics/box/hbr.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf
end function

function renderBoxBegin (title, additional)
	renderBoxBegin = "" & vbcrlf  & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<!-- SECTION BOX: " & uCase(title) & " -->" & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"">" & vbcrlf &_
"	<tr>" & vbcrlf &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/hl.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"		<td class=""boxHeader""><strong>" & title & "</strong>&nbsp;&nbsp;&nbsp;&nbsp;" & additional & "</td>"  & vbcrlf &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/hr.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/tl.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td width=""100%""><img border=""0"" src=""/cs_security/graphics/pixel.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/tr.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/ml.gif""><img border=""0"" src=""/cs_security/graphics/box/ml.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td width=""100%"" valign=""top"">" & vbcrlf
end function

function renderBoxEnd()
	renderBoxEnd = "		" & vbcrlf  &_
"	</td>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/mr.gif""><img border=""0"" src=""/cs_security/graphics/box/mr.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/bl.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/bm.gif"" width=""100%"">" & vbcrlf  &_
"		<img border=""0"" src=""/cs_security/graphics/box/bm.gif"" width=""5"" height=""15""></td>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/br.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>"
end function


function ValidateLocationInGrid(LocationID)
	Dim RS
	GetInvConnection()
	'sql = "SELECT Count(*) as Cnt FROM " &  Application("CHEMINV_USERNAME") & ".INV_GRID_ELEMENT WHERE Location_ID_FK=" & LocationID
	sql = "select l.location_id ||'::'|| vl.name ||'::'|| vl.location_id ||'::'|| p.location_id ||'::'|| p.location_name || '::' || l.collapse_child_nodes as RackGridInfo from " &  Application("CHEMINV_USERNAME") & ".inv_locations l, " &  Application("CHEMINV_USERNAME") & ".inv_vw_grid_location vl, " &  Application("CHEMINV_USERNAME") & ".inv_locations p where l.location_id=" & LocationID & " and l.location_id = vl.location_id and vl.parent_id = p.location_id and p.collapse_child_nodes = 1"
	'Response.Write(sql)
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		ValidateLocationInGrid = RS("RackGridInfo")
	Else
		ValidateLocationInGrid = ""
	End if
End function

function isGridElement(LocationID)
    Dim RS
    GetInvConnection()
    SQL = "select count(*) as theCount from " &  Application("CHEMINV_USERNAME") & ".inv_locations l, " &  Application("CHEMINV_USERNAME") & ".inv_vw_grid_location vl, " &  Application("CHEMINV_USERNAME") & ".inv_locations p where l.location_id=" & LocationID & " and l.location_id = vl.location_id and vl.parent_id = p.location_id "
	Set RS = Conn.Execute(sql)
    isGridElement = RS("theCount")
End function

FUNCTION isAuthorisedLocation(LocationID)
    Call GetInvConnection()
	SQL = "SELECT Decode(CHEMINVDB2.Authority.LocationIsAuthorized(Location_ID),NULL,0,CHEMINVDB2.Authority.LocationIsAuthorized(Location_ID)) as isAuthorised FROM inv_Locations WHERE Location_ID=" & LocationID
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
    isAuthorisedLocation=RS("isAuthorised")
END FUNCTION

FUNCTION isPublicLocation(LocationID)
    Call GetInvConnection()
	SQL = "SELECT isPublic FROM inv_Locations WHERE Location_ID=" & LocationID
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
    isPublicLocation=RS("isPublic")
END FUNCTION
function ValidateContainerInGrid(ContainerID)
	Dim RS
	GetInvConnection()
	sql = "select l.location_id || '::' || cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || '::' || vl.location_id || '::' || p.location_id || '::' || p.location_name || '::' || l.collapse_child_nodes || '::' || vl.name as RackGridInfo from " &  Application("CHEMINV_USERNAME") & ".inv_containers c, " &  Application("CHEMINV_USERNAME") & ".inv_locations l, " &  Application("CHEMINV_USERNAME") & ".Inv_Vw_Grid_Location vl, " &  Application("CHEMINV_USERNAME") & ".inv_locations p where c.container_id = " & ContainerID & " and c.location_id_fk = l.location_id and vl.location_id = c.location_id_fk and vl.parent_id = p.location_id and p.collapse_child_nodes = 1 "
	'Response.Write(sql)
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		ValidateContainerInGrid = RS("RackGridInfo")
	Else
		ValidateContainerInGrid = ""
	End if
End function

function ValidateLocationIsRack(LocationID)
	if LocationID="" then LocationID=0
	Dim RS
	GetInvConnection()
	'sql = "SELECT COLLAPSE_CHILD_NODES,Parent_ID FROM " &  Application("CHEMINV_USERNAME") & ".INV_LOCATIONS WHERE LOCATION_ID=" & LocationID " &_
	'sql = "SELECT case " &_
	'			"when (select count(location_id_fk) from inv_grid_storage where location_id_fk in ( SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID in (SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID=" & LocationID & "))) = 0 then " &_
	'			"	(SELECT collapse_child_nodes || '::' || parent_id FROM INV_LOCATIONS WHERE LOCATION_ID=" & LocationID & ") " &_
	'			"when (select count(location_id_fk) from inv_grid_storage, inv_locations where inv_grid_storage.location_id_fk = inv_locations.location_id and collapse_child_nodes = 1 and location_id_fk in ( SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID in (SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID=" & LocationID & "))) > 0 then " &_
	'			"	(select collapse_child_nodes || '::' || parent_id from inv_locations where location_id in (select location_id_fk from inv_grid_storage where location_id_fk in (SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID in (SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID=" & LocationID & ")))) " &_
	'			"else (SELECT collapse_child_nodes || '::' || parent_id FROM INV_LOCATIONS WHERE LOCATION_ID=" & LocationID & ") " &_
	'		"end as LocationData From Dual"
	'sql = "SELECT case " &_
	'		"when (select count(location_id_fk) from inv_grid_storage where location_id_fk in (SELECT parent_id FROM INV_LOCATIONS WHERE LOCATION_ID in (SELECT parent_id  FROM INV_LOCATIONS  WHERE LOCATION_ID = " & LocationID & "))) = 0 then " &_
	'		"	(SELECT '::' || parent_id  FROM INV_LOCATIONS  WHERE LOCATION_ID = " & LocationID & ")" &_
	'		"	else (Select '1::' || parent_id from inv_locations where location_id not in (0) and collapse_child_nodes is null and location_id not in (select vl.location_id from inv_vw_grid_location vl, inv_locations l where vl.parent_id = l.location_id and l.collapse_child_nodes=1) and level = (   Select min(level) from inv_locations   where location_id not in (0)   and collapse_child_nodes is null and location_id not in (select vl.location_id from inv_vw_grid_location vl, inv_locations l where vl.parent_id = l.location_id and l.collapse_child_nodes=1) connect by location_id in (prior parent_id) start with location_id = " & LocationID & ") connect by location_id in (prior parent_id) start with location_id = " & LocationID & "  ) end " &_
	'		"	as LocationData From Dual"
	'Response.Write(sql)
	sql = "select collapse_child_nodes || '::' || ( " &_
		"Select location_id " &_
		"from inv_locations " &_
		"where location_id not in (0) " &_
		"and collapse_child_nodes is null " &_
		"and location_id not in (select vl.location_id from inv_vw_grid_location vl, inv_locations l where vl.location_id = l.location_id and l.collapse_child_nodes=1) " &_
		"and level = ( " &_
		"  Select min(level) " &_
		"  from inv_locations " &_
		"  where location_id not in (0) " &_
		"  and collapse_child_nodes is null " &_
		"  and location_id not in (select vl.location_id from inv_vw_grid_location vl, inv_locations l where vl.location_id = l.location_id and l.collapse_child_nodes=1) " &_
		"  connect by location_id in (prior parent_id) " &_
		"  start with location_id = " & LocationID &_
		") " &_
		"connect by location_id in (prior parent_id) " &_
		"start with location_id = " & LocationID &_
		") as LocationData " &_
		"from inv_locations  " &_
		"where location_id = " & LocationID
	'Response.Write(sql)
	Set RS = Conn.Execute(sql)
	if NOT (RS.EOF AND RS.BOF) then
		'ValidateLocationIsRack = RS("COLLAPSE_CHILD_NODES") & "::" & RS("Parent_ID")
		ValidateLocationIsRack = RS("LocationData")
	End if
End function

function isRackLocation(locationId)
    
    GetInvConnection()
    Set Cmd = nothing    
    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Racks.isRackLocation", adCmdStoredProc)

    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",131, adparamreturnvalue, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",131, 1, 0, locationId)
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Racks.isRackLocation")

    isRackLocation = cbool(Cmd.Parameters("RETURN_VALUE"))

end function

'-- Displays simple table versions of a Rack.
function DisplaySimpleRack(RackIDList,SelectedRackGrids,Comments)
	Dim RS
	arrRackIDs = split(Trim(RackIDList),",")

	html = ""
	for j = 0 to ubound(arrRackIDs)
	if arrRackIDs(j) <> "" then

		'-- Get list of containers duplicated in Grid cells
		DuplicatesList = GetDuplicateRackContainerID(arrRackIDs(j))
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?,?)}", adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID", 200, 1, 30, arrRackIDs(j))
'-- CSBR ID:131045
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Modified to get the length of server name; it was previously hard coded to 30; in case when server name has more characters, its an error
'-- Date: 27/09/2010
		Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, len(Application("RegServerName")), Application("RegServerName"))
'-- End of Change #131045#
		Cmd.Properties ("PLSQLRSet") = TRUE
		Set RS = Server.CreateObject("ADODB.Recordset")
		RS.CursorLocation = aduseClient
		RS.LockType = adLockOptimistic
		RS.Open Cmd
		RS.ActiveConnection = Nothing
		RS.filter = "COL_INDEX=1"
		rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")
		numRows = Ubound(rowName_arr,2) + 1
		RS.filter = 0
		RS.Movefirst
		RS.filter = "ROW_INDEX=1"
		colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
		NumCols = Ubound(colName_arr,2) + 1
		if NumCols > 15 then
			cellWidth = 90
		else
			cellWidth = 90
		end if
		html = html & vbcrlf & "<table class=""rack"" border=""1"" cellpadding=""3"" cellspacing=""0"">"
		summaryHtml = ""
		For currRow = 1 to numRows
			RS.filter = 0
			RS.Movefirst
			RS.filter = "ROW_INDEX=" & currRow
			rowName = RS("ROWNAME")
			if currRow = 1 then
				html = html & vbtab & "<tr><td align=""center"" colspan=""" & NumCols & """><strong><font fize=""verdana"">" & RS("location_name_full") & "</td></tr>" & vbcrlf
			end if
			html = html & vbtab & "<tr>" & vbcrlf

			hasDups = false
			bReassignGridID = false
			While NOT RS.EOF
				RackName = RS("location_name")
				GridData = RS("grid_data").value
				highlightCell = false
				if GridData <> "" then
					'Response.Write(GridData)
					arrGridData = split(GridData,"::")
					GridType = arrGridData(0)
					GridID = arrGridData(1)
					GridBarcode = arrGridData(2)
					GridBatchField1 = arrGridData(9)
					GridBatchField2 = arrGridData(11)
					SubstanceName = arrGridData(13)
					'Begin: SM Fix CSBR-72213
					if Application("RegServerName") <> "NULL" then
					if GridType <> "Rack" then RegNumber = arrGridData(20)
					end if	
					'End: SM Fix CSBR-72213
					if isBlank(GridID) then GridID = RS("location_id")
					GridDisplay = "<img src=""" & arrGridData(3) & """ border=""0"">&nbsp;<strong>" & GridBarcode & "</strong>"
					if instr(SelectedRackGrids,GridID) > 0 then
						bgColor = "e1e1e1"
						GridDisplay = GridDisplay
						highlightCell = true
					else
						bgColor = "ffffff"
					end if
					if arrGridData(16) <> "" then GridDisplay = GridDisplay & "<br />Size:" & FormatNumber(arrGridData(16),2)
					if arrGridData(17) <> "" then GridDisplay = GridDisplay & "<br />Amt:" & FormatNumber(arrGridData(17),2)
					if Application("RegServerName") <> "NULL" then
						if arrGridData(19) <> "" then GridDisplay = GridDisplay & "<br />Conc:" & FormatNumber(arrGridData(19),2)
					else 
						if arrGridData(18) <> "" then GridDisplay = GridDisplay & "<br />Conc:" & FormatNumber(arrGridData(18),2)
					end if 	
					if lcase(GridType) = "container" then
						duplicateElementIDs = ""
						if DuplicatesList <> "" then
							arrDuplicatesList = split(DuplicatesList,",")
							for l = 0 to ubound(arrDuplicatesList)
								arrDupValues = split(arrDuplicatesList(l),"::")
								dupContainerID = arrDupValues(0)
								dupRackGridID = arrDupValues(1)
								dupRackName = arrDupValues(2)
								if dupRackName = RS("name") then
									if duplicateElementIDs <> "" then
										duplicateElementIDs = duplicateElementIDs & "," & dupContainerID
									else
										duplicateElementIDs = duplicateElementIDs & dupContainerID
									end if
								end if
							next
						end if
					end if
					if duplicateElementIDs <> "" then
						GridDisplay = "<span style=""font-weight: bold; color: red;"">Error</span>"
					end if

				else
					GridDisplay = RS("name")
					bgColor = "ffffff"

				end if
				colIndex = RS("COL_INDEX")
				if highlightCell then
					html = html & vbtab & vbtab & "<td bgcolor=""#" & bgColor & """ align=""center"" width=""" & cellWidth & """ nowrap><table border=""2"" style=""border-collapse:collapse;""><tr><td><font size=""1"" face=""verdana"">" & GridDisplay & "</td></tr></table></td>" & vbcrlf
				else
					html = html & vbtab & vbtab & "<td bgcolor=""#" & bgColor & """ align=""center"" width=""" & cellWidth & """ nowrap><font size=""1"" face=""verdana"">" & GridDisplay & "</td>" & vbcrlf
				end if
				RS.MoveNext
			Wend
			html = html & vbtab & "<tr>" & vbcrlf
		Next
		html = html & "</table><br />" & vbcrlf
		html = html & DisplayRackContainerSummary(arrRackIDs(j),DuplicatesList)
	end if
	next
	Response.Write("<div class=""racks"" align=""center"">")
	if Comments <> "" then
		Response.Write("<div style=""padding:4px;margin-bottom:10px;font-weight:bold;"">" & Comments & "</div>")
	end if
	Response.Write(html & "</div><br />")

end function

function DisplayRackContainerSummary(LocationID,DuplicatesList)

		'-- Set up fieldArray containing the column definition for the on screen report
		If NOT IsArray(Session("DisplayRackReportFieldArray1")) then
			ColDefStr = GetUserProperty(Session("UserNameCheminv"),"DisplayRackChemInvFA1")
			If ColDefStr = "" OR IsNull(ColDefStr) then
				'-- Default column definition
				ColDefstr= Application("DisplayRackReportColDef1")
				rc = WriteUserProperty(Session("UserNameCheminv"), "DisplayRackChemInvFA1", ColDefstr)
			End if
			fieldArray = GetFieldArray(colDefstr, Application("DisplayRackFieldMap"))
			Session("DisplayRackReportFieldArray1") = fieldArray
		Else
			fieldArray = Session("DisplayRackReportFieldArray1")
		End if

		summaryHtml = ""
		summaryHtml = summaryHtml & "<table cellspacing=""3"" cellpadding=""2"" style=""border-collapse: collapse;"" border=""1""><tr><th>Position</th>"
		for i=0 to ubound(fieldArray)
				if fieldArray(i,0) = BatchAmount and bShowBatch then
					summaryHtml = summaryHtml & "<th nowrap width=""" & fieldArray(i,2) & """>" & fieldArray(i,1) & "</th>"
				else
					summaryHtml = summaryHtml & "<th nowrap width=""" & fieldArray(i,2) & """>" & fieldArray(i,1) & "</th>"
				end if
		next

		summaryHtml = summaryHtml & "</tr>" & vbcrlf
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRIDCONTAINERS(?,?)}", adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
'-- CSBR ID:131045
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Modified to get the length of server name; it was previously hard coded to 30; in case when server name has more characters, its an error
'-- Date: 27/09/2010
		Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, len(Application("RegServerName")), Application("RegServerName"))
'-- End of Change #131045#
		Cmd.Properties ("PLSQLRSet") = TRUE
		Set RSSummary = Server.CreateObject("ADODB.Recordset")
		RSSummary.Open Cmd
		hasDupes = false
		renderedPositions = ""
		While NOT RSSummary.EOF

			'-- Only display locations in grid for a barcode exists
			if RSSummary("Barcode") <> "" then

				'-- Only render grids that aren't already list, this is true for duplicates
				if instr(renderedPositions,RSSummary("GridName") & ", ") = 0 then
					duplicateElementIDs = ""
					if DuplicatesList <> "" then
						arrDuplicatesList = split(DuplicatesList,",")
						for l = 0 to ubound(arrDuplicatesList)
							arrDupValues = split(arrDuplicatesList(l),"::")
							dupContainerID = arrDupValues(0)
							dupRackGridID = arrDupValues(1)
							dupRackName = arrDupValues(2)
							if dupRackName = RSSummary("GridName") then
								if duplicateElementIDs <> "" then
									duplicateElementIDs = duplicateElementIDs & "," & dupContainerID
								else
									duplicateElementIDs = duplicateElementIDs & dupContainerID
								end if
								hasDupes = true
							end if
						next
					end if

					summaryHtml = summaryHtml & "<tr><td align=""right"">" & RSSummary("GridName") & "</td>"
					if hasDupes then
						numCols = 0
						for i=0 to ubound(fieldArray)
							numCols = numCols + 1
                        next
						summaryHtml = summaryHtml & "<td colspan=""" & numCols & """><span style=""font-weight: bold; color: red;"">Error:&nbsp;</span>Container ID's, " & duplicateElementIDs & " are dupliated in position " & RSSummary("GridName") & " of Rack " & LocationID & "</td>"
					else
						for i=0 to ubound(fieldArray)
								summaryHtml = summaryHtml & "<td width=""" & fieldArray(i,2) & """ nowrap>"
								execute("summaryHtml = summaryHtml & TruncateInSpan(RSSummary(""" & fieldArray(i,0) & """), fieldArray(i,2), """")")
								summaryHtml = summaryHtml & "</td>" & vbcrlf
						next
					end if
					summaryHtml = summaryHtml & "</tr>" & vbcrlf
					hasDupes = false
					renderedPositions = renderedPositions & RSSummary("GridName") & ", "
				end if
			end if

		RSSummary.MoveNext
		Wend
		RSSummary.Close()
		Set RSSummary = Nothing
		summaryHtml = summaryHtml & "</table>"
		DisplayRackContainerSummary = summaryHtml

end function

'-- Displays simple table versions of a Rack.
function DisplaySimpleMovedRack(RackIDList,SelectedRackGrids,ContainerGridPositionIDList)
	Dim RS
	arrRackIDs = split(Trim(RackIDList),",")
	html = ""
	for j = 0 to ubound(arrRackIDs)
	if Trim(arrRackIDs(j)) <> "" then
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?,?)}", adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, arrRackIDs(j))
		Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, 30, Application("RegServerName"))
		Cmd.Properties ("PLSQLRSet") = TRUE
		Set RS = Server.CreateObject("ADODB.Recordset")
		RS.CursorLocation = aduseClient
		RS.LockType = adLockOptimistic
		RS.Open Cmd
		RS.ActiveConnection = Nothing
		RS.filter = "COL_INDEX=1"
		rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")
		numRows = Ubound(rowName_arr,2) + 1
		RS.filter = 0
		RS.Movefirst
		RS.filter = "ROW_INDEX=1"
		colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
		NumCols = Ubound(colName_arr,2) + 1
		if NumCols > 15 then
			cellWidth = 70
		else
			cellWidth = 70
		end if

		html = html & vbcrlf & "<table class=""rack"" border=""1"" cellpadding=""3"" cellspacing=""0"">"
		For currRow = 1 to numRows
			RS.filter = 0
			RS.Movefirst
			RS.filter = "ROW_INDEX=" & currRow
			rowName = RS("ROWNAME")
			if currRow = 1 then
				html = html & vbtab & "<tr><td align=""center"" colspan=""" & NumCols & """><font fize=""verdana"">" & RS("location_name_full") & "</td></tr>" & vbcrlf
			end if
			html = html & vbtab & "<tr>" & vbcrlf

			While NOT RS.EOF
				highlightCell = false
				RackName = RS("location_name_full")
				GridData = RS("grid_data").value
				GridLocationID = cLng(RS("location_id"))
				if GridData <> "" then
					arrGridData = split(GridData,"::")
					GridID = arrGridData(1)
					GridElementLocationID = ""
					GridBarcode = arrGridData(2)
					if isBlank(GridID) then GridID = RS("location_id")
					GridDisplay = "<img src=""" & arrGridData(3) & """ border=""0""><strong>&nbsp;" & GridBarcode & "</strong>"
					if instr(SelectedRackGrids,GridID) > 0 then
						bgColor = "e1e1e1"
						GridDisplay = GridDisplay
						highlightCell = true

					else
						bgColor = "ffffff"
					end if
					if arrGridData(16) <> "" then GridDisplay = GridDisplay & "<br />Size:" & FormatNumber(arrGridData(16),2)
					if arrGridData(17) <> "" then GridDisplay = GridDisplay & "<br />Amt:" & FormatNumber(arrGridData(17),2)
					if arrGridData(19) <> "" then GridDisplay = GridDisplay & "<br />Conc:" & FormatNumber(arrGridData(19),2)
				elseif instr(ContainerGridPositionIDList,GridLocationID) > 0 then
						bgColor = "e1e1e1"
						GridDisplay = "<img src=""/cheminv/images/listview/flask_closed_icon_16.gif"" border=""0"">" & GridBarcode & "*"
				else
					GridDisplay = RS("name")
					bgColor = "ffffff"
				end if
				colIndex = RS("COL_INDEX")
				if highlightCell then
					html = html & vbtab & vbtab & "<td bgcolor=""#" & bgColor & """ align=""center"" width=""" & cellWidth & """ nowrap><table border=""2"" style=""border-collapse:collapse;""><tr><td><font size=""1"" face=""verdana"">" & GridDisplay & "</td></tr></table></td>" & vbcrlf
				else
					html = html & vbtab & vbtab & "<td bgcolor=""#" & bgColor & """ align=""center"" width=""" & cellWidth & """ nowrap><font size=""1"" face=""verdana"">" & GridDisplay & "</td>" & vbcrlf
				end if
				'html = html & vbtab & vbtab & "<td bgcolor=""#" & bgColor & """ align=""center"" width=""" & cellWidth & """><font size=""1"" face=""verdana"">" & GridDisplay & "</td>" & vbcrlf
				RS.MoveNext
			Wend
			html = html & vbtab & "<tr>" & vbcrlf
		Next
		html = html & "</table><br /><br />" & vbcrlf
	end if
	next
	htmlWrapper = ""
	htmlWrapper = htmlWrapper & "<div class=""racks"" align=""center"">"
	htmlWrapper = htmlWrapper & "<table cellspacing=""2""><tr>"
	htmlWrapper = htmlWrapper & "<td class=""grayBackground""><img src=""/cheminv/graphics/pixel.gif"" width=""10"" height=""10"" border=""0"">&nbsp;</td><td>* Containers for Fulfillment</td>"
	htmlWrapper = htmlWrapper & "</tr></table><br />"
	htmlWrapper = htmlWrapper & html & "</div><br />"

	DisplaySimpleMovedRack = htmlWrapper

end function

'-- Displays table of newly created or updated Containers
Function DisplayContainerSummary(containerIDList)
	Dim RS
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".GUIUTILS.DISPLAYCREATEDCONTAINERS(?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("P_CONTAINERLIST", 200, 1, 8000, containerIDList)
	Cmd.Properties ("PLSQLRSet") = TRUE
	Set RS = Server.CreateObject("ADODB.Recordset")
	RS.Open Cmd
	Response.Write("<table border=""1"" cellspacing=""0"" cellpadding=""5"" style=""border-collapse: collapse"">" & vbcrlf)
	Response.Write("<tr><th>Barcode</td><th>Name</td><th>Size</td><th>Amnt</td><th>Conc</td><th>Location</td>" & vbcrlf)
	While NOT RS.EOF
		Response.Write("<tr>")
		Response.write("<td>" & RS("barcode") & "</td>")
		Response.Write("<td>" & RS("container_name") & "</td>")
		Response.Write("<td>" & RS("containersize") & "</td>")
		Response.Write("<td>" & RS("amountavailable") & "</td>")
		Response.Write("<td>" & RS("concentration_text") & "</td>")
		Response.Write("<td>" & RS("location_path") & "</td>")
		Response.Write("</tr>" & vbcrlf)
		RS.MoveNext
	Wend
	Response.Write("</table>")
	RS.Close()
End Function

Function GetDuplicateRackContainerID(LocationID)
	'-- Get list of containers that are duplicated in Rack grids
	DuplicatesList = ""
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.REPORTINVALIDGRIDS(?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
	Cmd.Properties ("PLSQLRSet") = TRUE
	Set RSDuplicates = Cmd.Execute
	While NOT RSDuplicates.EOF
		if DuplicatesList <> "" then
			DuplicatesList = DuplicatesList & "," & RSDuplicates("container_id") & "::" & RSDuplicates("location_id_fk") & "::" & RSDuplicates("name")
		else
			DuplicatesList = DuplicatesList & RSDuplicates("container_id") & "::" & RSDuplicates("location_id_fk") & "::" & RSDuplicates("name")
		end if
	RSDuplicates.MoveNext
	Wend
	RSDuplicates.Close()
	Set RSDuplicates = Nothing

	GetDuplicateRackContainerID = DuplicatesList
End Function

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

sub ShowMenuLinks(byref arrLinks, byref arrCategories)

%>
<script type="text/javascript">
	sfHover = function () {
		var sfEls = document.getElementById("nav").getElementsByTagName("LI");
		for (var i = 0; i < sfEls.length; i++) {
			sfEls[i].onmouseover = function () {
				this.className += " sfhover";
			}
			sfEls[i].onmouseout = function () {
				this.className = this.className.replace(new RegExp(" sfhover\\b"), "");
			}
		}
	}
	if (navigator.userAgent.toLowerCase().indexOf('msie') != -1) {
		if (window.attachEvent) window.attachEvent("onload", sfHover);
	}
	else {
		window.addEventListener("load", sfHover);
	}
</script>

<%
    WritePullDownMenu arrLinks, arrCategories

end sub


sub WritePullDownMenu(byref arrLinks, byref arrCategories)

	Response.Write "<div class=""dropDownMenuControl"">" & chr(13)
	Response.Write "<ul id=""nav"">" & chr(13)
	for	i=0 to ubound(arrCategories)
	    if i = 0 then
	        listClass = "firstList"
	    else 
	        listClass = ""
	    end if
	    if ucase(arrCategories(i)) <> "NULL" then Response.Write "<li class=""" & listClass & """>" & arrCategories(i) & chr(13)
		if ucase(arrCategories(i)) <> "NULL" then response.Write "<ul>" & chr(13)
		WritePullDownMenuLinks arrLinks, arrCategories(i)
		if ucase(arrCategories(i)) <> "NULL" then response.Write "</ul>" & chr(13)
		if ucase(arrCategories(i)) <> "NULL" then Response.Write "</li>" & chr(13)
	next
	response.Write classicLinks & chr(13)
	Response.Write "</ul>" & chr(13)
	Response.Write "</div>" & chr(13)
	
end sub

sub WritePullDownMenuLinks(byref arrLinks, category)
	altTitle = ""
	for j = 0 to ubound(arrLinks)
		if arrLinks(j,6) <> "" then altTitle = arrLinks(j,6)
		if arrLinks(j,5) = category then
			if arrLinks(j,0) = 1 then
	            '-- if the dialog size is "NULL" don't open a dialog
			    if ucase(arrLinks(j,3)) = "NULL" then
    				Response.Write "<li><a href=""" & arrLinks(j,1) & """ title=""" & altTitle & """ class=""contextMenu"" onmouseover=""this.className = 'menuOn'"" onmouseout=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</a></li>" & chr(13)
			    else
    				Response.Write "<li><a href=""#"" title=""" & altTitle & """ onclick=""OpenDialog('" & arrLinks(j,1) & "', 'Diag', " & arrLinks(j,3) & "); return false"" class=""contextMenu"" onmouseover=""this.className = 'menuOn'"" onmouseout=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</a></li>" & chr(13)
			    end if
			elseif arrLinks(j,4) = 1 then
				Response.Write "<li><a href=""#"" title=""" & altTitle & """ class=""contextMenu"" style=""color:#808080;cursor: default;"">" & arrLinks(j,2) & "</A></li>" & chr(13)
			end if
		end if
	next

end sub

sub WriteContextMenu(byref arrLinks, byref arrCategories)
	
	Response.Write "<DIV ID=""contextMenu"" ONMOUSEOUT=""menu = this; this.tid = setTimeout ('menu.style.visibility = \'hidden\'', 20);"" ONMOUSEOVER=""clearTimeout(this.tid);"">"
	for i = 0 to ubound(arrCategories)	
		Response.Write "<strong>" & arrCategories(i) & "</strong><br />"
		WriteContextMenuLinks arrLinks, arrCategories(i)
		Response.Write "<br />"
	next
	Response.Write "</div>" & chr(13)

%>
<script language="javascript" type="text/javascript">
var menu;
function showMenu (evt) {
  if (document.all) {

	if (parseInt(navigator.appVersion) > 3) {
	 if (navigator.appName=="Netscape") {
	  winW = window.innerWidth;
	  winH = window.innerHeight;
	 }
	 if (navigator.appName.indexOf("Microsoft")!=-1) {
	  winW = document.body.offsetWidth;
	  winH = document.body.offsetHeight;
	 }
	}

	varAdjHeight = 0;
	varAdjWidth = 0;
	if (event.clientY > (winH-195)) {
		varAdjHeight = event.clientY-195;
	} else {
		varAdjHeight = event.clientY;
	}
	if (event.clientX > (winW-346)) {
		varAdjWidth = event.clientX-346;
	} else {
		varAdjWidth = event.clientX;
	}
    document.all.contextMenu.style.pixelLeft = varAdjWidth;
    document.all.contextMenu.style.pixelTop = varAdjHeight;
    //document.all.contextMenu.style.pixelTop = event.clientX;
    //document.all.contextMenu.style.pixelTop = event.clientY;
    document.all.contextMenu.style.visibility = 'visible';
    return false;
  }
  else if (document.getElementById('contextMenu')) {
	 winW = window.innerWidth;
	 winH = window.innerHeight;

	varAdjHeight = 0;
	varAdjWidth = 0;
	if (event.clientY > (winH-195)) {
		varAdjHeight = event.clientY-195;
	} else {
		varAdjHeight = event.clientY;
	}
	if (event.clientX > (winW-346)) {
		varAdjWidth = event.clientX-346;
	} else {
		varAdjWidth = event.clientX;
	}
    document.getElementById('contextMenu').style.left = varAdjWidth;
    document.getElementById('contextMenu').style.top = varAdjHeight;
    document.getElementById('contextMenu').style.visibility = 'visible';
    return false;
  }
  return true;
}
if (document.all)
  document.oncontextmenu =showMenu;
 if (document.getElementById('contextMenu')){ 
	document.addEventListener('contextmenu',function (e)
	{
		e.preventDefault();
		showMenu();
		return false;
	} );
}
</script>

<%
end sub

sub WriteContextMenuLinks(byref arrLinks, category)
	
	isFirst = true
	for j = 0 to ubound(arrLinks)
		if arrLinks(j,5) = category then
			if arrLinks(j,0) = 1 then
				if not isFirst then Response.Write "&nbsp;|&nbsp;" 
				Response.Write "<A HREF=""#" & arrLinks(j,2) & """ onclick=""OpenDialog('" & arrLinks(j,1) & "', 'Diag', " & arrLinks(j,3) & "); return false"" CLASS=""contextMenu"" ONMOUSEOVER=""this.className = 'menuOn'"" ONMOUSEOUT=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</A>"
				isFirst = false					
			elseif arrLinks(j,4) = 1 then
				if not isFirst then Response.Write "&nbsp;|&nbsp;" 
				Response.Write "<A HREF=""#"" CLASS=""contextMenu"" style=""color:#808080;cursor: default;"">" & arrLinks(j,2) & "</A>"
				isFirst = false					
			end if
		end if
	next

end sub

'-- get the location to refresh to
function getRefreshLocationId(locationId)
    ServerName = Application("InvServerName")
    Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
    FormData = "locationId=" & selectedLocationID & Credentials   
    getRefreshLocationId = CShttpRequest2("POST", ServerName, "/cheminv/api/GetRefreshLocationId.asp", "ChemInv", FormData)
end function

'Show a meaningful error message
Sub ShowChemInvConnectionError()
    Response.Write 	"<br><br><center><b><font color=""maroon"" >Requested Operation cannot be completed.<br>Please contact your system administrator.</font></b><br><br> 	 Reason:Invalid INV_SERVER_NAME in Invconfig.ini file."
	Response.Write  "<br><br> <a href=""#"" onclick=""if(opener){opener.focus();window.close(); return false;} else {history.back(); return false;}""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a> </center>"
End Sub

'Ownership set by user and group
function GetOwnerShipGroupList()
    
	Call GetInvConnection()
	SQL = "select b.PRINCIPAL_ID ,a.Group_Name from coedb.COEGROUP a,coedb.COEPRINCIPAL b where a.Group_ID=b.Group_ID order by lower(a.Group_Name)"
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set GroupRS= Conn.Execute(SQL)
    UserList=""
        Do While NOT GroupRS.EOF
			
			UserList =UserList& GroupRS("PRINCIPAL_ID").value & "," & GroupRS("Group_Name")
			UserList =UserList& "|"
			GroupRS.MoveNext
		Loop
		
    GetOwnerShipGroupList=UserList
	
End function
function GetOwnerShipUserList()
    
	Call GetInvConnection()
	SQL = "select b.PRINCIPAL_ID ,Upper(a.Last_Name||' '||a.First_Name)/*a.user_code||DECODE(a.First_Name, NULL, '', ', '||a.First_Name)*/ as USer_ID from coedb.PEOPLE a,coedb.COEPRINCIPAL b where a.Person_ID=b.Person_ID and a.ACTIVE=1 order by lower(a.Last_Name)"
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set UserRS= Conn.Execute(SQL)
    UserList=""
        Do While NOT UserRS.EOF
			
			UserList =UserList& UserRS("PRINCIPAL_ID").value & "," & UserRS("USer_ID")
			UserList =UserList& "|"
			UserRS.MoveNext
		Loop
		
    GetOwnerShipUserList=UserList
	
End function
function GetBatchingFieldNames()
    
	Call GetInvConnection()
	SQL = "select batch_type, batch_order from  cheminvdb2.inv_batch_type order by batch_order"
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set BatchRS= Conn.Execute(SQL)
    BatchNames=""
    count=1
        Do While NOT BatchRS.EOF
		    BatchNames = BatchNames  & BatchRS("batch_type") & "|" & BatchRS("batch_order") & ";"
			BatchRS.MoveNext
		Loop
	if len(BatchNames) > 0 then
        GetBatchingFieldNames=Mid(BatchNames,1, len(BatchNames)-1)
	end if
End function
function GetUOMAmountValuePairs(pAvailabeUOM)
   ' Response.Write pAvailabeUOM
    'Response.End
	Call GetInvConnection()
	SQL = "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & pAvailabeUOM & ") ORDER BY lower(DisplayText) ASC"
	if bDebugPrint then
		Response.Write sql
		Response.end
	end if
	Set UOMRS= Conn.Execute(SQL)
    UOMStr=""
    count=1
        Do While NOT UOMRS.EOF
		    UOMStr = UOMStr  & UOMRS("Value") & "|" & UOMRS("DisplayText") & ";"
			UOMRS.MoveNext
		Loop
		
    GetUOMAmountValuePairs=Mid(UOMStr,1, len(UOMStr)-1)
	
End function
' This function is going to be used for getting Regnumber/regbatch id for a Inventory container batch 
function GetRegAttributes(batchid)
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".BATCH.GetRegBatchid", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 4000, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 20
Cmd.Parameters.Append Cmd.CreateParameter("pBatchid",131, 1, 0, BatchID)
Cmd.Parameters("pBatchid").Precision = 9
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
		
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".BATCH.GetRegBatchid")
End if
if isnull(Cmd.Parameters("RETURN_VALUE")) then
    RegDisplayValue = ""
else
    RegDisplayValue =CStr(Cmd.Parameters("RETURN_VALUE"))
end if 
GetRegAttributes= RegDisplayValue
end function
%>

<%
sub DisplayRegBatchInfoFromWebService(fullRegNumber)

    
    serviceInitialized = false
    successfulRegCall = false
    regXMLloaded = false

    pRegBatchID = fullRegNumber

    set soapClient = CreateObject("MSSOAP.SoapClient30")
    set headerHandler = CreateObject("COEHeaderServer.COEHeaderHandler")
    'headerHandler.UserName = Ucase(Session("UserNameChemInv"))
    'headerHandler.Password = Session("UserIDChemInv")
    headerHandler.Ticket= session("SSOAuthTicket")' Request.Cookies("COESSO")
    set soapClient.HeaderHandler = headerHandler
    soapClient.ClientProperty("ServerHTTPRequest") = True

    on error resume next
    call soapClient.MSSoapInit(Application("SERVER_TYPE") & Application("RegServerName") & "/COERegistration/webservices/COERegistrationServices.asmx?wsdl")

      'if there is an error in retrieving the regxml we should log the error
    if err.number <> 0 then
        trace "Failed to initialize the Reg WebService", 0
        trace "Error Description: " & soapClient.Detail, 0
        on error goto 0
    else
        serviceInitialized = true
    end if


    ' if the service is healthy
    if serviceInitialized then

        soapClient.ConnectorProperty("Timeout") = 120000    ' sets timeout to 120 secs

        on error resume next
        regXML = soapClient.RetrieveRegistryRecordByFullRegNum(fullRegNumber, true)

        'if there is an error in retrieving the regxml we should log the error
        if err.number <> 0 then
            trace "The Registration XML could not be retrieved with the following errors", 0
            trace "Error Description: " & soapClient.Detail, 0
            on error goto 0
        else
            successfulRegCall = true
        end if


    end if
    
    if successfulRegCall then

        set xmlRegBase = Server.CreateObject("Msxml2.DOMDocument")
        xmlRegBase.async = false
    
   
        on error resume next
        xmlRegBase.loadXML(regXML)

        'if there is an error in loading the regxml we should log the error
        if err.number <> 0 then
            trace "The Registration XML could not be loaded with the following errors", 0
            trace "Error Number: " & err.number, 0
            trace "Error Description: " & err.description, 0
            on error goto 0
        else
            regXMLloaded = true
        end if

    end if
    


    if regXMLloaded then

        'read map from applciation variable
        set regMap = Server.CreateObject("Msxml2.DOMDocument")
        regMap.loadXML(Application("RegRequestMap"))
    
        'Get list of fields


	    GetRegBatchAttributes xmlRegBase.selectSingleNode("MultiCompoundRegistryRecord/RegNumber/RegID").text, xmlRegBase.selectSingleNode("MultiCompoundRegistryRecord/BatchList/Batch/BatchNumber").text

        set invFields = regMap.selectNodes("RegRequestSampleMap/fields/field")
        
        For i = 0 To (invFields.length - 1)
            Set invField = invFields(i)
		
            invVar = invField.GetAttribute("label")
            xpath = invField.GetAttribute("xPath")        
            constantVal = invField.GetAttribute("constantVal") 	

			xpathsplit = Split(xpath, "{vRegBatchID}")
			if UBound(xpathsplit) > 0 then
				xpath = Join(xpathsplit, pRegBatchID)
			end if
			
            value = ""

            if constantVal <> "" then
                value = constantVal
            end if

            if xpath <> "" then
                if value <> "" then
                    value = value & " "
                end if

                'GetXPathVal
                on error resume next
                xPathVal = xmlRegBase.selectSingleNode(xPath).text

                'if there is an error then we skip that particular xpath instead of blowing up
                if err.number <> 0 then
                    trace "The registration node " & xPath & " was skipped because it could not be found", 0
                    on error goto 0
                else
                    value = value & xPathVal
                end if

            end if

           if value <> "" then
                
                Response.Write "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & invVar & ": " & value & "<br>"
                'if (i < invFields.length - 1) then response.Write "<br>"



           end if

	    Next
    end if

end sub

'this sub will check whether there is data to call and only call the actual display method as needed.
sub DisplayRegBatchInfo(fullRegNumber)
        if Application("UseRegRequestMap") then

            'if the map value has been previously set and wasn't set to Nomap then we should use it
            'NOMAP essentially means that either 1) The xml file was not there 2) There was an error loading it
            if Application("RegRequestMap") <> null and Application("RegRequestMap") <> "" and Application("RegRequestMap") <> "NOMAP" then
                goodmap = true
            else
                'if the map has never been created we should create it
                if Application("RegRequestMap") = null or Application("RegRequestMap") = "" then
                    Call SetRegRequestMap            
                end if

                'if the map was no good then don't use it
                if Application("RegRequestMap") <> "NOMAP" then
                    goodmap = true
                end if  
            end if
       
            'Use the map to set session variables
            if goodmap = true then
                    Call DisplayRegBatchInfoFromWebService(fullRegNumber)
            end if
        end if
end sub

Sub SetRegRequestMap()
    
    		Set oResultXML = server.CreateObject("MSXML2.DOMDocument")

            sPath = "\ChemInv\config\xml_templates\InvRequestSampleMap.xml"
            on error resume next
            pathToRegMap = Server.MapPath(sPath)

                        
            if err.number<> 0 then
                Application("RegRequestMap") = "NOMAP"
                trace "Failed to load the Registration map at this location: " & sPath, 0
                on error goto 0
            end if

            on error resume next
            oResultXML.load pathToRegMap
            
            if err.number<> 0 then
                Application("RegRequestMap") = "NOMAP"
                trace "Failed to load the Registration map at this location: " & pathToRegMap, 0
                on error goto 0
            else
                Application("RegRequestMap") = oResultXML.xml
            end if
End Sub
function getUserIDFromString(UserString)
    tempUserArray= split(UserString,"|")
    for each key in tempUserArray
        if instr(key,"=")>0 then
            tempKey = split(key,"=")
            if lcase(trim(tempKey(0)))=lcase("cssecuritypassword") then    
                getUserIDFromString=  tempKey(1)
                exit for
            end if    
        end if 
    next
end function
Function CryptVBS(Text, Key)
	Text = shiftChr(Text, -1)
	KeyLen = Len(Key)
	For i = 1 To Len(Text)
		KeyPtr = (KeyPtr + 1) Mod KeyLen
		sTxtChr = Mid(Text, i, 1)
		wTxtChr = Asc(stxtchr)
		wKeyChr = Asc(Mid(Key, KeyPtr + 1, 1))
		CryptKey = Chr(wTxtChr Xor wKeyChr)
		hold = hold & CryptKey
	Next
	CryptVBS = shiftChr(hold, 1)
End Function
Function shiftChr(str, s)
	for i = 1 to len(str)
		hold = hold & Chr(Asc(Mid(str,i,1))+ s)
	Next
	shiftChr = hold
End function
%>


<SCRIPT LANGUAGE=jscript RUNAT=Server>
function getUtcNow(strDate){
	return Date.parse(strDate)
}
</SCRIPT>
