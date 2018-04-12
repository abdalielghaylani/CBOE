<%


'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Routine:   fmtDateTime()
'
' Purpose:   Returns a Variant (String) containing an expression 
'            formatted according to instructions contained in a 
'            format expression
'
' Inputs :   Argument    : d
'            DataType    : Variant Date
'            Description : A *valid* variant date variable or an
'                        : expression that result in a variant date
'                        : variable
'
'            Argument    : pat
'            DataType    : Variant String
'            Description : An acceptable date and/or time pattern
'
' Outputs:   Argument    : None
'
' Returns:   Formatted Variant string representation of the date
'            passed in or an error message where applicable.
'
' Sample Usage :
'  strToday = fmtDateTime(Now(), "yyyy-mm-dd hh:mm:ss")
'  strYesterday = fmtDateTime(DateAdd("d", -1, Now()), "h:m:s")
'
' Author :   Kevin J. Turner  July 11, 2001
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Function fmtDateTime(byval d, byval pat)
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	' Acceptable date formatting parts:
	'  yy       - year value, 2-digits
	'  yyyy     - year value, 4-digits
	'  m        - month value (1 to 12), not zero-padded
	'  mm       - month value (1 to 12), zero-padded
	'  d        - day value (1 to 31), not zero-padded
	'  dd       - day value (1 to 31), zero-padded
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	' Acceptable time formatting parts:
	'  h        - hours value, not zero-padded
	'  hh       - hours value, zero-padded
	'  m or mm  - minutes value, always zero-padded
	'  s or ss  - seconds value, always zero-padded
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	' Acceptable time delimiters:
	'   colon (:)
	' Acceptable date delimiters:    
	'   space ( ) or hyphen (-)
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	' Assumptions made: 
	' DATE-ONLY patterns will NOT contain colons or spaces
	' TIME-ONLY patterns will NOT contain hyphens or forward slashes
	'   and WILL contain colons
	' DATE-TIME patterns WILL contain acceptable date part delimiters,
	'   acceptable time part delimiters, and a space to delimit the 
	'   date from the time
	''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' 
	' First, if a space character is present, then the patteren is 
	' split into a 2-element array: the first element being (typically)
	' the date portion, and the second element being (typically) the 
	' time portion. If no space character present, then we're working 
	' with a date or time part. Splitting this type of pattern with 
	' double-quotes ("") will result in a 1-element array consisting 
	' of a date portion or a time portion.
	Dim Tokens, token, delim, i, date_part, time_part, DateTokens, TimeTokens
	' do not treat Null values as erroneous parameters, handle them gracefully
	If IsNull(d) Then
	fmtDateTime = ""
	Exit Function
	End If
	If TypeName(d) <> "Date" Or Not IsDate(d) Then
	fmtDateTime = "Invalid date parameter."
	Exit Function
	End If
	' if there's a space in the string, then we're dealing with 
	' date *and* time formatting, otherwise it date OR time formatting
	' we doing
	If InStr(pat, " ") > 0 Then
	Tokens = Split(pat, " ") ' should be 2 tokens --> (0)date (1)time
	Else
	Tokens = Split(pat, "") ' date OR time formatting
	End If
	bIsDate = False : bIsTime = False
	For Each token In Tokens
	If InStr(token, "-") > 0 Or InStr(token, "/") > 0 Then
	' get the delimter used...
	If InStr(token, "-") Then
	delim = "-"
	ElseIf InStr(token, "/") Then
	delim = "/"
	End If
	' tokenize the date parts
	DateTokens = Split(token, delim)
	For i = 0 To UBound(DateTokens)
	' replace the time tokens with properly formatted values
	Select Case CStr(DateTokens(i))
	Case "yy"
	DateTokens(i) = Right(CStr(DatePart("yyyy", d)), 2)
	Case "yyyy"
	DateTokens(i) = CStr(DatePart("yyyy", d))
	Case "m"
	DateTokens(i) = CStr(DatePart("m", d))
	Case "mm"
	DateTokens(i) = ZeroPad(CStr(DatePart("m", d)),2)
	Case "d"
	DateTokens(i) = CStr(DatePart("d", d))
	Case "dd"
	DateTokens(i) = ZeroPad(CStr(DatePart("d", d)),2)
	Case Else
	fmtDateTime = "Invalid date format : " & token
	Exit Function
	End Select
	Next
	date_part = Join(DateTokens, delim)
	End If
	If InStr(token, ":") > 0 Then
	' tokenize the time parts
	TimeTokens = Split(token, ":")
	For i = 0 To UBound(TimeTokens)
	' replace the time tokens with properly formatted values
	Select Case CStr(TimeTokens(i))
	Case "h"
	TimeTokens(i) = Right(CStr(DatePart("h", d)), 2)
	Case "hh"
	TimeTokens(i) = ZeroPad(CStr(DatePart("h", d)),2)
	Case "m", "mm"  ' always zero-pad minutes
	TimeTokens(i) = ZeroPad(CStr(DatePart("n", d)),2)
	Case "s", "ss"  ' always zero-pad seconds
	TimeTokens(i) = ZeroPad(CStr(DatePart("s", d)),2)
	Case Else
	fmtDateTime = "Invalid time format : " & token
	Exit Function
	End Select
	Next
	time_part = Join(TimeTokens, ":")
	End If
	Next
	fmtDateTime =  Trim(date_part & " " & time_part)
End Function
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
' Routine:   ZeroPad()
'
' Purpose:   Pads a Variant String with zeros to a specified number
'            of digits, 
'               e.g.  "12" padded to 4 digits --> "0012"
'
' Inputs :   Argument    : str
'            DataType    : Variant String
'            Description : The string value to be padded
'
'            Argument    : iSize
'            DataType    : Variant Integer
'            Description : The total desired size of the returned
'                          string.
'
' Outputs:   Argument    : None
'
' Returns:   A zero-padded Variant string representation of the string
'            passed in.
'
' Sample Usage :
'  strVal = ZeroPad("14", 4)  ' <--  returns "0014"
'
' Author :   Kevin J. Turner  July 11, 2001
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Function ZeroPad(byval str, byval iSize)
ZeroPad = String((iSize - Len(str)), "0") & Trim(str)
End Function



%>
