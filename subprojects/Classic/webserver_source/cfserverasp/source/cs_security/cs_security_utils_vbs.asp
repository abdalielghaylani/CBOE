<SCRIPT LANGUAGE=vbscript RUNAT=Server>
//Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

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
    if portNumber = "80" or inStr(pHostName, ":" & portNumber) > 0 then
	    URL = "http://" & pHostName & "/" & pTarget
	else
	    URL = "http://" & pHostName & ":" & portNumber & "/" & pTarget	    
	end if
	'End of change
	
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

Function GetCS_SecurityConnection(dbKey)
	Dim ConnStr
	Dim Conn
	if(Request.Cookies("CS_SEC_Azure").Item <> "") then
		UserName =  Application("SEC_USERNAME")
		UserID = Application("SEC_PWD")
	else
		UserName = Session("UserName" & dbkey)
		UserID = Session("UserID" & dbkey)
    end if
	ConnStr = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & UserName & ";Password=" & UserID
	
	'Response.End
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
	Set GetCS_SecurityConnection =  Conn
End Function

Function GetAdminCS_SecurityConnection()
	Dim ConnStr
	Dim Conn
	ConnStr = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & Application("SEC_USERNAME") & ";Password=" & Application("SEC_PWD")
	'Response.Write "ConnString=" & ConnStr
	'Response.End
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
	Set GetAdminCS_SecurityConnection =  Conn
End Function

Function GetCommand(ByRef Conn, CommandText, CommandType)
	Dim Cmd
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = Conn
	Cmd.CommandType = CommandType
	Cmd.CommandText = CommandText
	Set GetCommand = Cmd
End Function

Sub ExecuteCmd(strID)
	On Error Resume Next
	Cmd.Execute
	If Err then
		Select Case True 
			Case inStr(Err.description,"ORA-20003") > 0	
		'If inStr(Err.description,"ORA-20003") > 0 then
			Response.Write  strID & ": Cannot execute invalid Oracle procedure."
			Case inStr(Err.description,"access violation") > 0
			Response.Write  strID & ": Current user is not allowed to execute Oracle procedure."
			Case Else
			Response.Write strID & ": " & Err.description	
		'End if
		End Select
		Response.end
	End if
End Sub

Function ShowSelectBox2(ByRef Conn, name, SelectedValue, sql, TruncateLength, FirstOptionText, FirstOptionValue)
	Dim RS
	Dim strSelected
	Set RS = Conn.Execute(sql)
	ShowSelectBox2 = BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, 1, false)
End function

Function BuildSelectBox(RS, name, SelectedValue, TruncateLength, FirstOptionText, FirstOptionValue, size, ismultiple)
	Dim str
	Dim DisplayText
	Dim multiple
	
	If ismultiple then 
		multiple = "MULTIPLE"
	Else
		multiple = ""
	End if
	 
	If IsNull(SelectedValue) then SelectedValue = ""
	str = str & "<SELECT size=""" & size & """ name=""" & name & """" & multiple & " >"
	if RS.EOF AND RS.BOF then
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

function GetCancelButton()
	Dim ReturnURL
	ReturnURL = Session("GUIReturnURL")
	if ReturnURL <> "" then
		GetCancelButton = CancelButton(ReturnURL, "")
	else	
		GetCancelButton = CancelButton("Close this window", "top.close();return false")	
	End if
End function

function CancelButton(href, onclick)
	Dim str
	str = "<a href=""" & href & """ "
	if len(onclick) > 0 then str = str & "onclick=""" & onclick & """ "
	str = str & " target=_top><img SRC=""../graphics/close.gif"" border=""0""></a>"
	CancelButton = str
end function


Function StrNull(str)
	if IsNull(str) then 
		StrNull = ""
	else
		StrNull = str
	end if
End function

Sub DumpCookies()
	For Each strKey In Request.Cookies
      Response.Write strKey & " = " & Request.Cookies(strKey) & "<BR>"
      If Request.Cookies(strKey).HasKeys Then
        For Each strSubKey In Request.Cookies(strKey)
          Response.Write "->" & strKey & "(" & strSubKey & ") = " & _
            Request.Cookies(strKey)(strSubKey) & "<BR>"
        Next
      End If
    Next
    Response.Write "Cookie Header: " & Request.ServerVariables("HTTP_COOKIE")
End sub

Function GeneratePwd(s)
	GeneratePwd = "7" & UCase(strReverse(s)) & "11C"
End function

</SCRIPT>
