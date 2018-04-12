<SCRIPT RUNAT="SERVER" LANGUAGE="VbScript">
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
	str = str & "<td bgcolor=#d3d3d3>"
	' Loop through array to concatenate the content
	For i = 0 to arrLen step 2
		str2 = str2 & Eval(tempArr(i)) 
		if i < arrLen then str2 = str2 & tempArr(i+1)
	Next
	str = str & TruncateInSpan(str2, truncateLength, id)
	str = str & "</td>"
	ShowField = str
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
	if len(strText) > Length then 
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & left(strText, Length) & "..."
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "</span>"
	TruncateInSpan = str
End function					
Function ShowStructure(id, height, width)
	Dim str	
	str = "<embed src=""" & Application("AppTempDirPathHTTP")& "/" & Application("appkey")& "Temp/mt.cdx"" border=""0"" width=""" & width & """ height=""" & height & """ ID=""" & id & """ name=""cdp" & id & """ type=""chemical/x-cdx"" viewonly=""true"" dataurl=""/cheminv/cheminv/cheminv_action.asp?dbname=cheminv&formgroup=base_form_group&dataaction=get_structure&Table=inv_compounds&Field=Structure&DisplayType=cdx&StrucID=" & id & """>"
	str = str & "<applet code=""camsoft.cdp.CDPHelperAppSimple"" width=""1"" height=""1"" name=""cdp" & id & """ archive=""camsoft.jar"" id=""" & id & """><param name=""ID"" value=""" & id & """><param name=""CABBASE"" value=""camsoft.cab""></applet>"
	ShowStructure = str 
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
	Dim str
	Dim RS
	Dim strSelected
	
	GetInvConnection()
	Set RS = Conn.Execute(sql)
	str = str & "<SELECT name=""" & name & """>"
	if RS.EOF AND RS.BOF then
		str = str &  "<OPTION value=""NULL"">Error:PickList is Empty!" 
	Else
		Do While NOT RS.EOF
			strSelected = ""
			If Cstr(RS("Value").value) = CStr(SelectedValue) then 
				strSelected = "Selected=""True"""
			End if
			str = str & "<OPTION " & strSelected & " value=""" & RS("Value") & """>" & RS("DisplayText")
			RS.MoveNext
		Loop
	End if
	str = str & "</SELECT>"
	ShowSelectBox = str
End function
'****************************************************************************************
'*	PURPOSE: Create HTML <td>s with caption and input box	                               
'*	INPUT: 	caption text, Name of field variable to populate input box, size, text                              
'*          to appear after the imput box, disable the box, mark caption as required		        
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
	
	str = "<td align=right valign=top nowrap width=""200"">"
	str = str & fieldCaption
	str = str & "</td>"
	str = str & "<td>"
	str = str & "<input type=text name=""i" & fieldVariableName & """ size=" & size & " value=""" & Eval(fieldVariableName)& """ " & Disabled & " >"
	str = str & textAfter
	str = str & "</td>"
	ShowInputBox = str
End function
</SCRIPT>
