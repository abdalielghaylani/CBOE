<html>
<head>
<title>Document submission</title>

<!--#INCLUDE FILE="adovbs.inc"-->
<!--#INCLUDE FILE="upload.asp"-->
<!--#include file="functions.asp"-->
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->

<%StoreASPSessionID()%>

</head>

<body background="<%=Application("UserWindowBackground")%>">
<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td><table border="0" width="560" cellspacing="0" cellpadding="0">			
				<tr>
					<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
				</tr>
				
				<tr><td height="20"></td></tr>
			</table>

			<table cellspacing="0" cellpadding="0">
				<tr>
					<td><font size="4">Uploading file...</font></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</body>
</html>
<%
Response.Flush
'stop
Server.ScriptTimeout = 1200
Session("previewFeedback") = ""
Session("submitFeedback") = ""

'Dim RLSProjectID
'stop
If Request.ServerVariables("REQUEST_METHOD") = "POST" Then
	Dim Upload, Field
	Set Upload = GetUpload() 'function in upload.asp
	Session("fileLength") = Upload.Item("file").Length
	For Each Field In Upload.Items
		If Field.FileName <> "" Then 'This field is uploaded file
			fileName = Field.FileName
			hexStringLen = Len(Field.Value.HexString) 
			if hexStringLen = 0 then%>
				<script language="javascript">
					alert('The document you are submitting does not exist or is a zero-length document. Please make sure the path is correct.');
					window.history.back();
				</script>
				<%Response.end%>
			<%end if%>
			
			<%
			fileFullPath = WriteBinaryToFile(Field, fileName) 'in functions.asp. Field is JScriptTypeInfo.
			Session("fileName") = fileName
            Session("fileServerPath") = fileFullPath
		end if
				
		If Field.Name = "prefix" then
			prefix = Field.Value.String
		End if

		If Field.Name = "numberPart" then
			numberPart = Field.Value.String
		End if
	Next
	
	
	On Error GoTo 0
	Upload = Empty
End If
%>
<script language="javascript">
    self.close();
</script>

