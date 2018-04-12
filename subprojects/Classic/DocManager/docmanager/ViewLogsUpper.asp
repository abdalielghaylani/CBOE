<%'Option Explicit%>


<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<%
dim dbkey
dim fso, fldr, files, f, loadFrom, logsLocation, output

StoreASPSessionID()

Set fso = CreateObject("Scripting.FileSystemObject")

dbkey = "docmanager"

loadFrom = ReadConfigEntry("LOAD_FROM")

if right(loadFrom, 1) <> "\" then
	loadFrom = loadFrom & "\"
end if

logsLocation = loadFrom & "Logs\"

function ReadConfigEntry(key)
	dim f, lineText, lineNumber, i, replaceString, retVal
	
	retVal = ""
	
	Set f = fso.OpenTextFile(Server.MapPath("/docmanager/FileBatchLoad/config.ini"), 1) 'ForReading
	
	' Get file content into an array:
	Dim contents
	contents = Split(f.ReadAll, vbCrLf)
	f.close
	
	For i = LBound(contents) to Ubound(contents)
		lineText = contents(i)
		if instr(lineText, key & "=") = 1 then
			retVal = Right(lineText, Len(lineText) - Len(key & "="))
			exit for
		end if
	Next
	
	ReadConfigEntry= retVal
 end function
 
Function SortAndOutputFiles(folderLocation)
	
	Dim folder, rs, f
	
	set folder = fso.getfolder(folderLocation)
	Const adVarChar = 200
	Const adDate = 7
	Const adBigInt = 20 
	
	'create a custom disconnected recordset with fields for filename, last modified date, and size.
	set rs = createobject("ador.recordset")
	rs.fields.append "filename", adVarChar, 255
	rs.fields.append "moddate", adDate
	rs.fields.append "filesize", adBigInt

	'opening without any connection info makes it "disconnected".
	rs.open
	'load it with file name, date, etc.
	for each f in folder.files
		rs.addnew array("filename", "moddate", "filesize"), array(f.name, f.datelastmodified, f.size)
		rs.update
	next


	if not (rs.bof and rs.eof) then 
		rs.sort = "moddate desc"
		rs.movefirst
		do until rs.eof
			Response.Write "<a href=""ViewLogsLower.asp?LogFileName=" & Server.URLEncode(rs("filename")) & "&logsLocation=" & Server.UrlEncode(folderLocation) & """ target=""lower""><font size=""2"">" & rs("filename") & "</font></a><br>"
			rs.movenext
		loop
	end if

	set rs = nothing
	set folder = nothing
End Function
%>

<html>
<head>
<title>View Logs</title>


</head>

<body background="<%=Application("UserWindowBackground")%>">

<table>
	<tr>
		<td width="100"></td>
		<td>
			<%
			if not fso.FolderExists(logsLocation) then%>
				There hasn't been any logs since a new value is specified as 'load from' location.
			<%else
				SortAndOutputFiles(logsLocation)
			end if
			
			Set fso = nothing
			%>

		</td>
	</tr>
</table>
</body>
</html>
