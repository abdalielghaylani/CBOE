<%@ LANGUAGE=VBScript %>
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%
'Adjust this depending on the size of the files you'll`be expecting; longer timeout for larger files!
Server.ScriptTimeout = 5400

Const ForWriting = 2
Const TristateTrue = -1
CrLf = Chr(13) & Chr(10)

'This function retreives a field's name
Function GetFieldName(infoStr)
	sPos = InStr(infoStr, "name=")
	EndPos = InStr(sPos + 6, infoStr, Chr(34) & ";")
	If EndPos = 0 Then
		EndPos = inStr(sPos + 6, infoStr, Chr(34))
	End If
	GetFieldName = mid(infoStr, sPos + 6, endPos - (sPos + 6))
End Function

'This function retreives a file field's filename
Function GetFileName(infoStr)
	sPos = InStr(infoStr, "filename=")
	EndPos = InStr(infoStr, Chr(34) & CrLf)
	if sPos > 0 and EndPos > sPos + 10 then
		GetFileName = Mid(infoStr, sPos + 10, EndPos - (sPos + 10))
	else
		GetFielName = ""
	end if
End Function

'This function retreives a file field's MIME type
Function GetFileType(infoStr)
	sPos = InStr(infoStr, "Content-Type: ")
	GetFileType = Mid(infoStr, sPos + 14)
End Function

'Return the current date/time as a numeric string
Function DateTimeString()
	DateTimeString = Year(Now()) & Month(Now()) & Day(Now()) & Hour(Now()) & Minute(Now()) & Second(Now())
end function

'Yank the file (and anything else) that was posted
PostData = ""
Dim biData
biData = Request.BinaryRead(Request.TotalBytes)
'Careful! It's binary! So, let's change it into`something a bit more manageable.
For nIndex = 1 to LenB(biData)
	PostData = PostData & Chr(AscB(MidB(biData,nIndex,1)))
Next

'Having used BinaryRead, the Request.Form collection is`no longer available to us. So, we have to parse the request variables ourselves!
'First, let's find that encoding type!
ContentType = Request.ServerVariables("HTTP_CONTENT_TYPE")
ctArray = Split(ContentType, ";")
'File posts only work well when the encoding is "multipart/form-data", so let's check for that!
Dim thePost, thePostFiles(9, 3) 
If Trim(ctArray(0)) = "multipart/form-data" Then
	ErrMsg = ""
	' grab the form boundary...
	bArray = Split(Trim(ctArray(1)), "=")
	Boundary = Trim(bArray(1))
	'Now use that to split up all the variables!
	FormData = Split(PostData, Boundary)
	'Extract the information for each variable and its data
	Set thePost = CreateObject("Scripting.Dictionary")
	FileCount = 0
	For x = 0 to UBound(FormData)
		'Two CrLfs mark the end of the information about this field; everything after that is the value
		InfoEnd = InStr(FormData(x), CrLf & CrLf)
		If InfoEnd > 0 Then
			'Get info for this field, minus stuff at the end
			varInfo = Mid(FormData(x), 3, InfoEnd - 3)
			'Get value for this field, being sure to skip CrLf pairs at the start and the CrLf at the end
			varValue = Mid(FormData(x), InfoEnd + 4, Len(FormData(x)) - InfoEnd - 7)
			'Is this a file?
			If (InStr(varInfo, "filename=") > 0) Then
				'Place it into our files array
				'(While this supports more than one file uploaded at a time we only consider the single file case in this example)
				thePostFiles(FileCount, 0) = GetFieldName(varInfo)
				thePostFiles(FileCount, 1) = varValue
				thePostFiles(FileCount, 2) = GetFileName(varInfo)
				thePostFiles(FileCount, 3) = GetFileType(varInfo)
				FileCount = FileCount + 1
			Else
				'It's a regular field
				thePost.add GetFieldName(varInfo), varValue
			End If
		End If
	Next
Else
	ErrMsg = "Wrong encoding type!"
End If 

'Save the actual posted file

Set lf = server.createObject("Scripting.FileSystemObject")
for filenum = 0 to 1
	'Use the filename that came with the file
	'At this point, you need to determine what sort of client sent the file. Macintoshes only send the file 
	'name, with no path information, while Windows clients send the entire path of the file that was selected
	BrowserType = UCase(Request.ServerVariables("HTTP_USER_AGENT"))
	If (InStr(BrowserType, "WIN") > 0) Then
		'It's Windows; yank the filename off the end!
		sPos = InStrRev(thePostFiles(filenum, 2), "\")
		fName = Mid(thePostFiles(filenum, 2), sPos + 1)
	End If
	If (InStr(BrowserType, "MAC") > 0) Then
		'It's a Mac. Simple.
		'(Mac filenames can contain characters that are illegal under Windows, so look out for that!)
		fName = thePostFiles(filenum, 2)
	End If
	if fName <> "" then
		fName = DateTimeString() & "-" & fName
		if thePostFiles(filenum, 0) = "catalogfile" then
			thePost("catalogfile") = fName
		elseif thePostFiles(filenum, 0) = "logofile" then
			thePost("logofile") = fName
		end if
		FilePath = "./uploads/" & fname
		SavePath = Server.MapPath(FilePath)
		Set SaveFile = lf.CreateTextFile(SavePath, True)
		SaveFile.Write(thePostFiles(filenum, 1))
		SaveFile.Close
	end if
next

dim Conn
Dim UserName
Dim UserID

'get connection string from application variable
connection_array = Application("base_connection" & "chemacx")
if Lcase(connection_array(4)) = "login_required" then
	UserName = Application("UserIDKeyword") & "=" & Session("UserName" & "chemacx")
	UserID = Application("PWDKeyword") & "=" & Session("UserID" & "chemacx")
Else
	UserName = connection_array(4)
	UserID = connection_array(5)
End if	
ConnStr = connection_array(0) & "="  & connection_array(1) 
If UserName <> "" then ConnStr = ConnStr & ";" & UserName 
If UserID <> "" then ConnStr = ConnStr & ";" & UserID
'Response.Write ConnStr
'Response.end

Set Conn = Server.CreateObject("ADODB.Connection")
Conn.Open ConnStr 

thePost("submissionDate") = Now()
thePost("remoteIP") = request.ServerVariables("REMOTE_ADDR")
if len(thePost("remoteIP")) = 0		then thePost("remoteIP") = request.ServerVariables("REMOTE_HOST")
if len(thePost("remoteIP")) = 0		then thePost("remoteIP") = " "

if len(thePost("companyname")) = 0		then thePost("companyname") = " "
if len(thePost("address1")) = 0			then thePost("address1") = " "
if len(thePost("address2")) = 0			then thePost("address2") = " "
if len(thePost("city")) = 0				then thePost("city") = " "
if len(thePost("state")) = 0			then thePost("state") = " "
if len(thePost("zip")) = 0				then thePost("zip") = " "
if len(thePost("country")) = 0			then thePost("country") = " "
if len(thePost("phone")) = 0			then thePost("phone") = " "
if len(thePost("fax")) = 0				then thePost("fax") = " "
if len(thePost("email")) = 0			then thePost("email") = " "
if len(thePost("website")) = 0			then thePost("website") = " "
if len(thePost("catalogsource")) = 0	then thePost("catalogsource") = " "
if len(thePost("catalogfile")) = 0		then thePost("catalogfile") = " "
if len(thePost("updatefreq")) = 0		then thePost("updatefreq") = " "
if thePost("updatefreq") = "[time period]"	then thePost("updatefreq") = " "
if len(thePost("logo")) = 0				then thePost("logo") = " "
if len(thePost("logourl")) = 0			then thePost("logourl") = " "
if thePost("logourl") = "http://"		then thePost("logourl") = " "
if len(thePost("logofile")) = 0			then thePost("logofile") = " "
if len(thePost("restrictions")) = 0		then thePost("restrictions") = " "
if len(thePost("restrictionstext")) = 0	then thePost("restrictionstext") = " "
if len(thePost("techname")) = 0			then thePost("techname") = " "
if len(thePost("techemail")) = 0		then thePost("techemail") = " "

dim cn
set cn = Server.CreateObject("ADODB.Connection")
ConnStr2 = "FILE NAME=" & server.mappath("uploads/submissions.udl")' & server.mappath("uploads/submissions.udl")
cn.Open ConnStr2
set cmd = server.createobject("adodb.command")
with cmd
	.ActiveConnection = cn

	.Parameters.Append .CreateParameter("SubmissionDate",	133,1,len(thePost("submissionDate")),	thePost("submissionDate"))	
	.Parameters.Append .CreateParameter("RemoteIP",			200,1,len(thePost("remoteIP")),			thePost("remoteIP"))	
	.Parameters.Append .CreateParameter("CompanyName",		200,1,len(thePost("companyname")),		thePost("companyname"))	
	.Parameters.Append .CreateParameter("Address1",			200,1,len(thePost("address1")),			thePost("address1"))			
	.Parameters.Append .CreateParameter("Address2",			200,1,len(thePost("address2")),			thePost("address2"))			
	.Parameters.Append .CreateParameter("City",				200,1,len(thePost("city")),				thePost("city"))				
	.Parameters.Append .CreateParameter("State",			200,1,len(thePost("state")),			thePost("state"))			
	.Parameters.Append .CreateParameter("Zip",				200,1,len(thePost("zip")),				thePost("zip"))				
	.Parameters.Append .CreateParameter("Country",			200,1,len(thePost("country")),			thePost("country"))			
	.Parameters.Append .CreateParameter("Phone",			200,1,len(thePost("phone")),			thePost("phone"))			
	.Parameters.Append .CreateParameter("Fax",				200,1,len(thePost("fax")),				thePost("fax"))				
	.Parameters.Append .CreateParameter("Email",			200,1,len(thePost("email")),			thePost("email"))			
	.Parameters.Append .CreateParameter("Website",			200,1,len(thePost("website")),			thePost("website"))			
	.Parameters.Append .CreateParameter("CatalogSource",	200,1,len(thePost("catalogsource")),	thePost("catalogsource"))	
	.Parameters.Append .CreateParameter("CatalogFile",		200,1,len(thePost("catalogfile")),		thePost("catalogfile"))		
	.Parameters.Append .CreateParameter("UpdateFreq",		200,1,len(thePost("updatefreq")),		thePost("updatefreq"))		
	.Parameters.Append .CreateParameter("Logo",				200,1,len(thePost("logo")),				thePost("logo"))				
	.Parameters.Append .CreateParameter("LogoURL",			200,1,len(thePost("logourl")),			thePost("logourl"))			
	.Parameters.Append .CreateParameter("LogoFile",			200,1,len(thePost("logofile")),			thePost("logofile"))			
	.Parameters.Append .CreateParameter("Restrictions",		200,1,len(thePost("restrictions")),		thePost("restrictions"))		
	.Parameters.Append .CreateParameter("RestrictionsText",	200,1,len(thePost("restrictionstext")),	thePost("restrictionstext"))	
	.Parameters.Append .CreateParameter("TechName",			200,1,len(thePost("techname")),			thePost("techname"))			
	.Parameters.Append .CreateParameter("TechEmail",		200,1,len(thePost("techemail")),		thePost("techemail"))		

	.CommandText = "INSERT INTO Uploads (SubmissionDate,RemoteIP,CompanyName,Address1,Address2,City,State,Zip,Country,Phone,Fax,Email,Website," & _
					"CatalogSource,CatalogFile,UpdateFreq,Logo,LogoURL,LogoFile,Restrictions,RestrictionsText,TechName,TechEmail) " & _
					"Values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
	.Execute
end with

Set Mail = Server.CreateObject("Persits.MailSender") 
Mail.Host = "outgoing.cambridgesoft.com" ' Required
if instr(thePost("techemail"), "@") > 0 then
	Mail.From = thePost("techemail") ' Required
else
	Mail.From = "chemacx@cambridgesoft.com" ' Required
end if
Mail.FromName = thePost("techname") ' Optional 
Mail.AddAddress "chemacx@cambridgesoft.com"', "John Smith"
'Mail.AddCC "bjohnson@company2.com" ' Name is optional 
Mail.Subject = "ChemACX Submission from " & thePost("companyname")

bodytext = ""
bodytext = bodytext & "Company Name: " & thePost("companyname") & vbcrlf
bodytext = bodytext & "Address:      " & thePost("address1") & vbcrlf
if thePost("address2") <> " " then
	bodytext = bodytext & "              " & thePost("address2") & vbcrlf
end if
bodytext = bodytext & "City:         " & thePost("city") & vbcrlf
bodytext = bodytext & "State:        " & thePost("state") & vbcrlf
bodytext = bodytext & "Zip:          " & thePost("zip") & vbcrlf
if thePost("country") <> " " then
	bodytext = bodytext & "Country:      " & thePost("country") & vbcrlf
end if
bodytext = bodytext & vbcrlf
bodytext = bodytext & "Phone:        " & thePost("phone") & vbcrlf
bodytext = bodytext & "Fax:          " & thePost("fax") & vbcrlf
bodytext = bodytext & "Email:        " & thePost("email") & vbcrlf
bodytext = bodytext & "Website:      " & thePost("website") & vbcrlf
bodytext = bodytext & vbcrlf
bodytext = bodytext & thePost("catalogsource") & vbcrlf
if thePost("catalogfile") <> " " then
	bodytext = bodytext & "Catalog file uploaded as: " & thePost("catalogfile") & vbcrlf
end if
if thePost("updatefreq") <> " " then
	bodytext = bodytext & "Catalog updated every: " & thePost("updatefreq") & vbcrlf
end if
bodytext = bodytext & vbcrlf
bodytext = bodytext & thePost("logo") & vbcrlf
if thePost("logourl") <> " " then
	bodytext = bodytext & "Use logo from url: " & thePost("logourl") & vbcrlf
end if
if thePost("logofile") <> " " then
	bodytext = bodytext & "Logo file uploaded as:    " & thePost("logofile") & vbcrlf
end if
bodytext = bodytext & vbcrlf
bodytext = bodytext & thePost("restrictions") & vbcrlf
if thePost("restrictionstext") <> " " then
	bodytext = bodytext & thePost("restrictionstext") & vbcrlf
end if
bodytext = bodytext & vbcrlf
bodytext = bodytext & "Technical contact: " & thePost("techname")
if thePost("techemail") <> " " then
	bodytext = bodytext & " (" & thePost("techemail") & ")" & vbcrlf
end if

Mail.Body = bodytext
On Error Resume Next
Mail.Send
On Error Goto 0



'IIS may hang if you don't explicitly return SOMETHING.
'So, redirect to another page or provide some kind of
'feedback below...
%>

<style type="text/css">{  }
body, td { font-family: Verdana, arial, helvetica, sans-serif; font-size: x-small; }
tt, pre { font-family: monospace; }
sup, sub { font-size: 60%; }
-->
</style>
<%
' Determines whether the user is logged into ChemACX Pro or ChemACX Net
Session("IsNet") = Not Session("okOnService_10")
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<!--COWS1-->
<head>
<title>CambridgeSoft's ChemACX.com - Submission thanks?</title>
</head>


<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif" leftmargin="9" topmargin="9">

<!--#INCLUDE FILE = "banner.asp"-->

<TABLE valign="top" width="600" cellpadding="2" ID="Table1">
<tr><td></td>
<td>
<h2>Thank you for your submission</h2>
</td>
</tr>
<tr>
<td valign=top width="5">
<table cellpadding="0" cellspacing="1" ID="Table2">

<!--#INCLUDE FILE = "leftnavigationlist.asp"-->

</table>

</td>
<td valign=top>

<p>Thank you for submitting your catalog to ChemACX.  If you have requested that we contact you,
we hope to get in touch within the next several business days.  Otherwise, you submission has
been queued for processing, and we hope to include it in the update currently schedule for next
<% if month(now()) > 1 and month(now()) <= 7 then %>
September.
<% else %>
March.
<% end if %>
</p>
<p>
If you have any questions, please feel free to contact us at
<script language=javascript>
<!--
{
	var linktext = "chemacx @ cambridgesoft.com";
	var email1 = "chemacx";
	var email2 = "cambridgesoft.com";
	document.write("<a href=" + "mail" + "to:" + email1 + "@" + email2 + ">" + linktext + "</a>")
}
-->
</script>
at any time.
</p>
</td></tr></table>
</td></tr></table>
<p>&nbsp;</p>

</td>
</tr>
</table>
</body>
</html>



