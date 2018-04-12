<%@ LANGUAGE=VBScript  %>

<%response.expires = 0
filePath = request("filePath")
fileName = Replace(request("fileName"), "%20", " ")
if Not Session("UserValidated" & dbkey) = 1 then
	'response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if
'Dim binaryConverter
'Set binaryConverter = Server.CreateObject("Binary.Converter")
'Response.Write filePath & " -- " & fileName
'Response.End

 Set objStream = Server.CreateObject("ADODB.Stream")
      objStream.Type = 1
      objStream.Open
      objStream.LoadFromFile filePath
     

vcontentType = "application-download"
Response.Clear()
Response.ContentType = vcontentType
Response.AddHeader "Content-Disposition", "attachment; filename=" & fileName
'Response.BinaryWrite binaryConverter.ReadFromFile(CStr(filePath))
 Response.BinaryWrite(objStream.Read())
Response.Flush()
Set binaryConverter = Nothing
Response.End()
%>