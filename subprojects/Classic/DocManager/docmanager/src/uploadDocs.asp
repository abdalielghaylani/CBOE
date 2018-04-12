<html>
<head>
<title>Document submission</title>

<!--#INCLUDE FILE="adovbs.inc"-->
<!--#INCLUDE FILE="upload.asp"-->
<!--#include file="functions.asp"-->

<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>
<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<%StoreASPSessionID()%>

</head>

<body background="<%=Application("UserWindowBackground")%>">
<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td><table border="0" width="560" cellspacing="0" cellpadding="0">
				<tr>
					<td colspan="2"><img src="/docmanager/docmanager/gifs/banner.gif"></td>
				</tr>
				
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
	Dim Upload, Field, saveReturn
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
			
			<%fileFullPath = WriteBinaryToFile(Field, fileName) 'in functions.asp. Field is JScriptTypeInfo.
			saveReturn = SaveFileAsHtml(fileFullPath) 'in functions.asp
			If Instr(saveReturn, "ERROR:") > 0 then
				Response.Write "<font size=""3"" color=""red"">" & saveReturn & "</font>"
				Response.end
			else
				Session("htmlFullPath") = saveReturn
			End if
		end if
		

		If Field.Name = "RLSProject" then
			RLSProjectID = Field.Value.String
		End if
		
		'stop
		'If Field.Name = "SEPNumber" then
		If Field.Name = "REGNumber" then
			'SEPNum = Field.Value.String
			REGNUM = Field.Value.String
		else		
			If Field.Name = "prefix" then
				prefix = Field.Value.String
			End if

			If Field.Name = "numberPart" then
				numberPart = Field.Value.String
			End if
			SEPNum = prefix & numberPart
		end if
	Next
	
	
	On Error GoTo 0
	Upload = Empty
End If
%>
<script language="javascript">
	var toPreview = '<%=request.querystring("toPreview")%>';
	var toSubmit = '<%=request.querystring("toSubmit")%>';
	
	if (toPreview == 'true') {
		//location.replace('previewDocs_frameset.asp?RLSProjectID=' + '<%=RLSProjectID%>' + '&SEPNum=' + '<%=SEPNum%>');
		location.replace('previewDocs_frameset.asp?RLSProjectID=' + '<%=RLSProjectID%>' + '&REGNum=' + '<%=REGNum%>');

	}
	else if (toSubmit == 'true') {
		//location.replace('submitFeedback.asp?RLSProjectID=' + '<%=RLSProjectID%>' + '&SEPNum=' + '<%=SEPNum%>');
		location.replace('submitFeedback.asp?RLSProjectID=' + '<%=RLSProjectID%>' + '&REGNum=' + '<%=REGNum%>');
	}
</script>

