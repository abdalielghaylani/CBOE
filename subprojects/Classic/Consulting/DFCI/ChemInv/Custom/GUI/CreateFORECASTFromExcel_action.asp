<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/upload.asp"-->

<%
Dim oUpload
Dim sFileName
Dim sPath
Dim cnExcel
Dim rsData 
Dim bDebugPrint
Dim Conn
Dim Cmd
Dim errCount
Dim sError
dim counter
counter=0
bDebugPrint = false
errCount = 0
sError = "Error(s) occurred.<BR>"

' Instantiate Upload Class
Set oUpload = GetUpload()
' Grab the file name
sFileName = oUpload.Item("File1").FileName
' Compile path to save file to
sPath = Server.MapPath("\" & Application("AppKey") & "\custom\uploads\" & sFileName)

' Save the binary data to the file system
oUpload("File1").value.SaveAs sPath
' Release upload object from memory
Set oUpload = Nothing

' Create a new ADO Connection Object
Set cnExcel = Server.CreateObject("ADODB.Connection")
sConnStr = "Provider=MSDASQL.1;Persist Security " & _
           "Info=False;Extended Properties=" & Chr(34) & _
           "DSN=Excel Files;DBQ=" & sPath & ";" & Chr(34)
           
'cnExcel.ConnectionString = m_strConnectionString
' Set the Cursor location to Client Side
cnExcel.CursorLocation = 3 'adUseClient

On Error Resume Next
cnExcel.Open "DRIVER=Microsoft Excel Driver (*.xls);DBQ=" & sPath
On Error GoTo 0
if cnExcel.State = 0 then
	sError = "Error opening the Excel file. Please confirm the excel file is valid by opening the file in Microsoft Excel."
	Response.redirect "CreatePlatesFromExcel.asp?errmsg=" & sError
end if

Set objCat = CreateObject("ADOX.Catalog")
Set objCat.ActiveConnection = cnExcel
i = 0
For Each tbl In objCat.Tables
    if i = 0 then sSheetName = tbl.Name end if
    i = i + 1
Next
Set objCat = Nothing
%>

<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Excel</title>
<script language=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script language=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<br><br><br><br><br><br>
<%
Response.Write "<div align=""center"" ID=""processingImage""><img src=""/cfserverasp/source/graphics/processing_Ybvl_Ysh_grey.gif"" WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush

' Obtain the recordset by executing SQL statement with sheet name
Set rsData = cnExcel.Execute("SELECT * FROM [" & sSheetName & "]")
' If records were returned the save the file
If rsData.RecordCount > 0 Then
	'use DOM doc to get around file already exists error
    Set oTransforms = server.CreateObject("MSXML2.DOMDOCUMENT.4.0")
    'rsData.Save sPath&".xml", adPersistXML
    'Response.End
    rsData.Save oTransforms, 1 'adPersistXML
    if bDebugPrint then oTransforms.save(sPath&".xml")
    'oTransforms.save(sPath&".xml")
    'Response.End
	oTransforms.setProperty "SelectionLanguage","XPath"
	oTransforms.setProperty "SelectionNamespaces","xmlns:s='uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882' xmlns:rs='urn:schemas-microsoft-com:rowset' xmlns:z='#RowsetSchema'"
	'get column names, the names could change if the column row is changed, but we select by position
	Set nlColumns = oTransforms.selectNodes("//s:AttributeType")
	for each oNode in nlColumns
		if oNode.getAttribute("rs:number") = 1 then LocationCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 2 then NDCCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 3 then AmountCol = oNode.getAttribute("name")
	next

	Set nlTransforms = oTransforms.selectNodes("//rs:data/z:row")
	'prepare command for lookups
	Call GetInvCommand("SQLText", &H0001) 'adCmdText
	ServerName = Request.ServerVariables("Server_Name")
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
	
	if nlTransforms.length = 0 then
	'Response.Write nlTransforms.length
	'Response.End
		sError = sError & "The data in the excel file appears to be incomplete or incorrect."
		bError = true	
		ExportToXML = False
	else
		for each oNode in nlTransforms
			bError = false
			Location = oNode.getAttribute(LOCATIONCol)
			NDC = oNode.getAttribute(NDCCol)
			Amount = oNode.getAttribute(AMOUNTCol)

			
				QueryString = "location=" & location
				QueryString = QueryString & "&ndc=" & NDC
				QueryString = QueryString & "&amount=" & Amount

				Response.Write QueryString & "<BR>"
				'Response.End
				httpResponse = CShttpRequest3("POST", ServerName, "/cheminv/custom/api/updateforecast.asp", "ChemInv", QueryString, 1*60*1000, 1*60*1000)
				out = trim(httpResponse)
			response.write out & "<BR>"
		next


'response.end	
   end if	

	
    Set oDoc = nothing
    ' Set to successful
    ExportToXML = True
Else
    ' NO records found advise user and return false
    sError = "No Data to Save."
    ExportToXML = False
End If

' Close the Recordset and connection
rsData.Close
cnExcel.Close
' Release objects
Set rsData = Nothing
Set cnExcel = Nothing

'Response.End
arrNewPlateIDs = split(newPlateIDs,"|")
%>
<script language="javascript">document.all.processingImage.style.display = 'none';</script>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td height=50 valign=middle align="left">
<%

						'Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & firstLocationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & arrNewPlateIDs(0) & ",1); opener.focus(); window.close();</SCRIPT>" 
						Response.Write "<P><center><a HREF=""3"" onclick=""history.go(-2); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"						
%>
		</TD>
	</TR>
</TABLE>
</Body>			
