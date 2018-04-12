<!--#INCLUDE VIRTUAL = "/docmanager/docmanager/ExternalLinks/ExternalLinksFunctions.asp"-->
<!--#INCLUDE VIRTUAL="/docmanager/docmanager/src/datetimefunc.asp"-->
<%
Server.ScriptTimeout = 1200

'escape "'" for PL/SQL
Function MyEscape(ByVal myStr)
	if myStr <> "" then
		myStr = replace(myStr, "'", "''")
	end if
	MyEscape = myStr
End Function

Const IN_CHUNK_SIZE = 10240

Dim dotBase, base
dotBase = 0
base = 0

%>
<html>
<head>
<title>Document submission</title>

<!--#include file="adovbs.inc"-->
<!--#INCLUDE FILE="upload.asp"-->

</head>

<body background="<%=Application("UserWindowBackground")%>">

	<!---JHS added 4/9/2003--->
	<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
		<!-- The table for the banner. -->
		<tr>

			<td valign="top" width="300">
				<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
			</td>

			<td>
					<font face="Arial" color="#0099FF" size="4"><i>
						Submitting Document
					</i></font>
			</td>
		</tr>
	</table>
	<!---JHS added 4/9/2003 end--->
	
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
						<td><font size="4">Storing and indexing document...</font></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</body>
</html>
		
<%
Response.Flush

Dim fileName, status
fileName = ""

Session("submitFeedback") = ""
Session("previewFeedback") = ""
Server.ScriptTimeout = 1000

'stop
RLSProjectID = request("RLSProjectID")
'SEPNum = request("SEPNum")
REGNum = request("REGNum")

if IsNull(RLSProjectID) or IsEmpty(RLSProjectID) or RLSProjectID = "" then
	RLSProjectID = 0
end if


'If Request.ServerVariables("REQUEST_METHOD") = "POST" Then 'submit button clicked
	status = SaveUploadToDB(Session("fileTempFolder"), Session("fileName"))
	if status = "" then
		Session("submitFeedback") = "<font color=#990000><b>" & session("fileName") & " (" & session("docSize") & " kb) was successfully submitted and indexed. You can submit another document below or <a href=/docmanager/inputtoggle.asp?formgroup=base_form_group&dbname=docmanager target=_top>search over documents</a>.</b></font>"
	else
		Session("submitFeedback") = status
		Response.Write status
		Response.end
	end if
	
'End If

Function SaveUploadToDB(fileTempFolder, fileName)
	
	Dim fso
	dim fileFullPath
	dim nRound
	dim leftOver
	dim f, fSize, fType
	dim doc_uid
	dim bytes
	dim i
	dim parseResult, status
	dim insertRet
	dim percentageDone
	Dim textFoundArr
		
		
	'JHS 11/8/2007 put the reg check here	
	if REGNum <> "" then
		Set connRegNumber = Server.CreateObject("ADODB.Connection")
		'11/9/2009 - REG now longer grants specific access to end users. 
		'So instead connect as DOCMGR and make sure that DOCMGR is granted select on the correct table/view
		'connRegNumber.Open(connStr)
        connRegNumber.Open(Application("cnnStr"))
        
		Set checkRegNumberExists = Server.CreateObject("ADODB.Recordset")
		'Set checkRegNumberExists = connRegNumber.Execute("Select count(*) as matches From REGDB.REG_NUMBERS WHERE ROOT_NUMBER = '" & SEPNum & "'")
		
		'Now put in code to use the reg view for 11 or the 10/9 reg numbers table
		if Application("DOCMGR_USE_REG") = "" then
			'really you only get here if you have reg permissions
			'so now try and connect to the 11 table to see if it exists
			on error resume next
			Set checkRegNumberExists = connRegNumber.Execute("select 1 from REGDB.vw_registrynumber")
			if err.number <> 0 then
				Application("DOCMGR_USE_REG") = 10
				err.Clear()
			else
				Application("DOCMGR_USE_REG") = 11
			end if
		
			
		end if
		
			Set cmdRegCheck = Server.CreateObject("ADODB.Command")
			cmdRegCheck.ActiveConnection = connRegNumber

		
		if Application("DOCMGR_USE_REG") = 11 then
			selectstatement = "select 1 from REGDB.vw_registrynumber where regnumber = ?"
		else
			selectstatement = "Select 1 From REGDB.REG_NUMBERS WHERE ROOT_NUMBER = ?"
		end if
		
			cmdRegCheck.CommandText = selectstatement

			Set param = cmdRegCheck.CreateParameter ("REGNum", adVarChar, adParamInput)
				param.Size = Len(REGNum)
				param.Value = REGNum
				cmdRegCheck.Parameters.Append param
		
		Set checkRegNumberExists = cmdRegCheck.Execute
		
		'This should not occur since the field is only shown for those who can see the reg table.
		if err.number > 0 then
		 Response.Write "You do not have permissions to confirm that this is a valid registration number.  Please submit the document again with a valid number"
		 Response.End
		end if
								
		'regNumberExists = cint(checkRegNumberExists("matches"))
		
		'regNumberExists = checkRegNumberExists.RecordCount
		
		
		'A reg number was submitted but it wasn't valid	
		'if regNumberExists < 1 then
		if checkRegNumberExists.EOF and checkRegNumberExists.BOF then
			Response.Write "You have submitted a document with an invalid registration number."
			Response.Write "Please submit the document again with valid information."
			Response.End	
		end if
			
		checkRegNumberExists.Close	
		connRegNumber.Close

	end if	
		
		
		
	
		
	fileFullPath = fileTempFolder & "\" & fileName
	
	If fileFullPath <> "" Then
			
		Set fso = CreateObject("Scripting.FileSystemObject")

		if not fso.FileExists(fileFullPath) then
			SaveUploadToDB = "file not found: " & fileFullPath
		end if
				
		dim title, author, submitter, submitter_comments
		
		title = request("title")
		if title = "" or title = " " then
			title = "No Title"
		end if
		
		author = request("author")
		
		submitter = Session("UserName" & Application("appkey"))
		submitter_comments = request("comments")
		
		'1. Insert document attributes to DOCMGR_DOCUMENTS" table, get unique id.
		dim cnn, rst, sql, cmd
		Set cnn = Server.CreateObject("ADODB.Connection")
		
		Dim fileTempFolderSubFolder, start
		start = InStr(fileTempFolder, "session")
		fileTempFolderSubFolder = Replace(Right(fileTempFolder, Len(fileTempFolder) - start + 1), "\", "/")
		
		Set f = fso.GetFile(fileFullPath)
		fSize = f.size
		
		fType = GetFileType(fileName)
		
		'rst.CursorLocation = adUseClient 
			
		with cnn
			.ConnectionString = Application("cnnStr")
		end with
		cnn.Open
		
		'JHS 1/15/2008 - THis is doing the exact opposite of what it looks like it is doing
		'Basically if the date being passed is a dd/mm/yyyy then you need to flip the dd and mm for English Oracle
		'The call below tricks the function because it takes 03/01/2008 and thinks it is march 1, instead of jan 1
		'when in european format
		if Application("DATE_FORMAT")=9 and  request("DOCMGR_DOCUMENTS.DOCUMENT_DATE") <> "" then
			dt = fmtDateTime(CDATE(request("DOCMGR_DOCUMENTS.DOCUMENT_DATE")),"dd/mm/yyyy")
		else 
			dt = request("DOCMGR_DOCUMENTS.DOCUMENT_DATE")
		end if
		
		'stop

		if cnn.State = 1 then 'adStateOpen
			insertRet = InsertDoc(cnn, _
						"DOCMGR.INSERTDOC", _
						MyEscape(Session("fileFullPath")) & "\", _
						MyEscape(Session("fileName")), _
						fSize, _
						fType, _
						MyEscape(title), _
						MyEscape(author), _
						MyEscape(submitter), _
						MyEscape(submitter_comments), _
						MyEscape(CLng(RLSProjectID)), _
						MyEscape(request("REPORT_NUMBER")), _
						MyEscape(request("MAIN_AUTHOR")), _
						MyEscape(request("DOCMGR_DOCUMENTS.STATUS")), _
						MyEscape(request("WRITER")), _
						MyEscape(request("ABSTRACT")), _
						MyEscape(dt), _
						MyEscape(request("DOCMGR_DOCUMENTS.DOCUMENT_CLASS")), _
						MyEscape(request("DOCMGR_DOCUMENTS.SEC_DOC_CAT")))
						
			if Instr(insertRet, "ERROR") > 0 then
				status = insertRet
			else	
				doc_uid = CInt(insertRet)	
				
				'SYAN added 10/25/2004 to fix CSBR-48313
				Session("doc_uid") = doc_uid
				'End of SYAN modification
				
				'2. Wtite chunks into BLOB field.
				nRound = Int(fSize / IN_CHUNK_SIZE)
				
				leftOver = fSize Mod IN_CHUNK_SIZE
								
				For i = 0 To (nRound - 1)
					'Write out progress
					percentageDone = CDbl(i/nRound*100)
					WriteOutProgress percentageDone
					
					on error resume next
					bytes = ReadFile(fileFullPath, i * IN_CHUNK_SIZE, IN_CHUNK_SIZE)
					if typename(bytes) = "String" then
						if Instr(bytes, "ERROR: ") > 0 then
							Response.Write bytes
							Response.end
						end if
					else
						AppendChunk cnn, "DOCMGR.BLOB_IN", doc_uid, UBound(bytes) + 1, bytes
					end if
				Next
				            
				on error resume next
				bytes = ReadFile(fileFullPath, nRound * IN_CHUNK_SIZE, leftOver)
				if typename(bytes) = "String" then
					if Instr(bytes, "ERROR: ") > 0 then
						Response.Write bytes
						Response.end
					end if
				else
					AppendChunk cnn, "DOCMGR.BLOB_IN", doc_uid, UBound(bytes) + 1, bytes
				end if
		
				'3. Index structures
				'don't bother to do following with txt files.
				if Right(fileName, 4) <> ".txt" then
					parseResult = ParseStructures(cnn, CLng(doc_uid), Session("fileFullPath"), Application("cfwMstPath"))
					
					if UCase(parseResult) <> "SUCCEED" then
						status = "ERROR PARSING STRUCTURES -- " & parseResult
					else
						status = ""
					End if
				end if 'Right(fileName, 4) <> ".txt"
		
				'4. Write textFound to database
				'if Session("textFound") <> "" then
				'	textFoundArr = Split(Session("textFound"), "|")
				'	for i = 0 to Ubound(textFoundArr)
				'		'sql = "INSERT INTO DOCMGR_EXTERNAL_LINKS (APPNAME, LINKTYPE, LINKID, DOCID, LINKFIELDNAME, SUBMITTER) " & _
				'		'	"VALUES ('CHEMREG', 'CHEMREGREGNUMBER', '" & textFoundArr(i) & "', " & doc_uid & ", 'REG_NUMBER', '" & submitter & "')"
				'		addLinkToTable textFoundArr(i), "CHEM_REG", "CHEMREGREGNUMBER", "REG_NUMBER", doc_uid
				'	next
				'end if

				'4. Write manually defined link to database
				'stop
				
				'if SEPNum <> "" then
				if REGNum <> "" then
					'JHS 11/8/2007			
					'Now that the document has been added we can create an external link to the valid doc id
						addLinkResult = AddExternalLink (cnn, _
							doc_uid, _
							REGNum, _
							MyEscape(submitter))

				end if
				
				'Clean up
				cnn.Close
				Set cnn = Nothing
				Set fso = nothing
			End If 'InsertDoc error checking
		
		Else 'cnn.State <> 1 then
			status = "ERROR: Cannot connect to database."
		End if
				
	Else 'fileFullPath = ""
	
		status = "ERROR: File path empty."
			
	End If
	
	SaveUploadToDB = status
End Function

Sub WriteOutProgress(percent)
	Dim delta
	
	delta = 10
	
	if CDbl(percent) = 0 then
		Response.Write "0%"
	end if
	
	if (Int(percent) - Int(dotBase)) >= 1 then
		Response.Write "<b>.</b>"
		if (Int(percent) - Int(dotBase)) >= 2 then
			Response.Write "<b>.</b>"
			if (Int(percent) - Int(dotBase)) >= 3 then
				Response.Write "<b>.</b>"
				if (Int(percent) - Int(dotBase)) >= 4 then
					Response.Write "<b>.</b>"
					if (Int(percent) - Int(dotBase)) >= 5 then
						Response.Write "<b>.</b>"
						if (Int(percent) - Int(dotBase)) >= 6 then
							Response.Write "<b>.</b>"
							if (Int(percent) - Int(dotBase)) >= 7 then
								Response.Write "<b>.</b>"
								if (Int(percent) - Int(dotBase)) >= 8 then
									Response.Write "<b>.</b>"
									if (Int(percent) - Int(dotBase)) >= 9 then
										Response.Write "<b>.</b>"
										if (Int(percent) - Int(dotBase)) >= 10 then
											Response.Write "<b>.</b>"
										end if
									end if
								end if
							end if
						end if
					end if
				end if
			end if
		end if
	end if

	dotBase = percent
					
	if (Int(percent) - Int(base)) >= delta then
		Response.Write Int(percent) & "%"
		base = percent
	end if
	Response.Flush
End Sub

Function ReadFile(filespec, pos, length)
	dim strm
	Set strm = CreateObject("ADODB.Stream")
	strm.Open
	strm.Type = 1
	
	on error resume next
	strm.LoadFromFile fileSpec
	if err.number <> 0 then
		ReadFile = "ERROR: Error read file. File can be corrupted."
	else
		strm.Position = pos
		ReadFile = strm.Read(length)
		Set strm = Nothing
	end if
End Function

Function InsertDoc(conn, sp_name, fileLocation, fileName, fileSize, fileType, title, author, submitter, submitter_comments, projectID, REPORT_NUMBER,MAIN_AUTHOR,STATUS,WRITER,ABSTRACT,DOCUMENT_DATE,DOCUMENT_CLASS,SEC_DOC_CAT)
	
	dim cmd, param

	Set cmd = Server.CreateObject("ADODB.Command")
	with cmd
		.ActiveConnection = conn
		.CommandText = sp_name
			
		Set param = .CreateParameter ("u_id", adInteger, adParamOutput)
		.Parameters.Append param
			
		Set param = .CreateParameter ("fileLocation", adVarChar, adParamInput)
		param.Size = Len(fileLocation)
		param.Value = fileLocation
		.Parameters.Append param
			
		Set param = .CreateParameter ("fileName", adVarChar, adParamInput)
		param.Size = Len(fileName)
		param.Value = fileName 
		.Parameters.Append param

		Set param = .CreateParameter ("fileSize", adInteger, adParamInput)
		param.Value = fileSize
		.Parameters.Append param

		Set param = .CreateParameter ("fileType", adVarChar, adParamInput)
		param.Size = Len(fileType)
		param.Value = fileType
		.Parameters.Append param

		Set param = .CreateParameter ("title", adVarChar, adParamInput)
		param.Size = Len(title)
		param.Value = title
		.Parameters.Append param

		Set param = .CreateParameter ("author", adVarChar, adParamInput)
		if Len(author) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(author)
			param.Value = MyEscape(author) 
		end if
		.Parameters.Append param

		Set param = .CreateParameter ("submitter", adVarChar, adParamInput)
		param.Size = Len(submitter)
		param.Value = submitter
		.Parameters.Append param

		Set param = .CreateParameter ("submitter_comments", adVarChar, adParamInput)
		if Len(submitter_comments) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(submitter_comments)
			param.Value = submitter_comments
		end if
		.Parameters.Append param
			
		Set param = .CreateParameter ("projectID", adInteger, adParamInput)
		param.Value = projectID
		.Parameters.Append param

		.CommandType = adCmdStoredProc
		
		Set param = .CreateParameter ("REPORT_NUMBER", adVarChar, adParamInput)
		if Len(REPORT_NUMBER) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(REPORT_NUMBER)
			param.Value = REPORT_NUMBER
		end if
		.Parameters.Append param

		Set param = .CreateParameter ("MAIN_AUTHOR", adVarChar, adParamInput)
		if Len(MAIN_AUTHOR) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(MAIN_AUTHOR)
			param.Value = MAIN_AUTHOR
		end if
		.Parameters.Append param

		Set param = .CreateParameter ("STATUS", adVarChar, adParamInput)
		if Len(STATUS) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(STATUS)
			param.Value = STATUS
		end if
		.Parameters.Append param
		
		Set param = .CreateParameter ("WRITER", adVarChar, adParamInput)
		if Len(WRITER) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(WRITER)
			param.Value = WRITER
		end if
		.Parameters.Append param				
	
		Set param = .CreateParameter ("ABSTRACT", adVarChar, adParamInput)
		if Len(ABSTRACT) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(ABSTRACT)
			param.Value = ABSTRACT
		end if
		.Parameters.Append param
		
		Set param = .CreateParameter ("DOCUMENT_DATE", adDBDate, adParamInput)
		if Len(DOCUMENT_DATE) = 0 or isDate(DOCUMENT_DATE) = false then
			param.Size = 1
			param.Value = NULL
		else
			param.Size = Len(CDATE(DOCUMENT_DATE))
			param.Value = CDATE(DOCUMENT_DATE)
		end if
		.Parameters.Append param

		Set param = .CreateParameter ("DOCUMENT_CLASS", adVarChar, adParamInput)
		if Len(DOCUMENT_CLASS) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(DOCUMENT_CLASS)
			param.Value = DOCUMENT_CLASS
		end if
		.Parameters.Append param

		Set param = .CreateParameter ("SEC_DOC_CAT", adVarChar, adParamInput)
		if Len(SEC_DOC_CAT) = 0 then
			param.Size = 1
			param.Value = ""
		else
			param.Size = Len(SEC_DOC_CAT)
			param.Value = SEC_DOC_CAT
		end if
		.Parameters.Append param
			
	
	
	end with
		
	on error resume next
	cmd.Execute()
	
	if err.number <> 0 then
		InsertDoc = "ERROR: " & err.number & " -- " & err.Description
	else
		InsertDoc = cmd.Parameters("u_id")
	end if
	
	Set cmd = nothing
End Function

Function AppendChunk(conn, sp_name, doc_uid, amount, buffer)

	dim cmd, param

	Set cmd = Server.CreateObject("ADODB.Command")
	with cmd
		.ActiveConnection = conn
		.CommandText = sp_name
		
		Set param = .CreateParameter("doc_pId", adInteger, adParamInput)
		param.Value = CInt(doc_uid)
		.Parameters.Append param
				
		Set param = .CreateParameter("amount", adInteger, adParamInput)
		param.Value = CInt(amount)
		.Parameters.Append param
		
		Set param = .CreateParameter("doc_buffer", adLongVarBinary, adParamInput)
		on error resume next
		param.Attributes = adParamLong
		on error resume next
		param.Size = UBound(buffer) + 1
		on error resume next
		on error resume next
		param.AppendChunk buffer
		on error resume next
		.Parameters.Append param		
				
		.CommandType = adCmdStoredProc
	
	end with
		
	on error resume next
	cmd.Execute()
	Set cmd = nothing
End Function

Function ParseStructures(conn, doc_uid, fileFullPath, mstPath)
	Dim cmd
	Dim myMolParser 
	Dim returnArray
	Dim numOfDocs, i, j, docid, molid
	Dim docidMolidString, docidMolidArray
	Dim retVal
	
	Response.Write "Parsing structures..."
	Response.Flush
	
	Set cmd = Server.CreateObject("ADODB.Command")
	cmd.ActiveConnection = conn
	
	on error resume next
	Set myMolParser = Server.CreateObject("MolParser.Parser")



    'Add support of docx by downsaving
    if LCase(Right(fileFullPath, 5)) = ".docx" then
        fileFullPath2 = replace(LCase(fileFullPath), ".docx", ".doc")
        
        Dim wordAppCreated
		wordAppCreated = false
		
		'Often times Word App does not get created correctly on app start
		if UCase(TypeName(Application("wordApp"))) = "APPLICATION" OR UCase(TypeName(Application("wordApp"))) = "STRING" then
			wordAppCreated = true
		else
			returnStr = "ERROR: Office automation could not create Word.Application. " & err.number & " -- " & err.Description & "<br>Check DCOMCNFG settings. camsoft_admin, Interactive, IUSR, IWAM need to be granted Access and Launch permission for Word Document and in Default Security.  Then restart IIS."
		end if
		
	    if wordAppCreated = true then
			Dim wordDoc
			'Give a bogus password to catch error for protected files
			on error resume next
			Set wordDoc = Application("wordApp").Documents.Open(fileFullPath, false, false, false, "bogusPassword")

			If err.number = 0 then
				
				'Session("textFound") = ""
				'on error resume next
				'Session("textFound") = ParseDocument(wordDoc, "WORD")
				
				on error resume next		
				wordDoc.SaveAs fileFullPath2, 0 'word document format
				fileFullPath = 	fileFullPath2			
			Elseif err.number = 5408 then
				returnStr = "ERROR: The file is password-protected. Please unprotect the file and try again."
			Else
				returnStr = "ERROR: " & err.number & " -- " & err.Description
			End if

			wordDoc.Close
						
			Set wordDoc = Nothing
            end if
        'add suport for xlsx by downsaving
        elseif LCase(Right(fileFullPath, 5)) = ".xlsx" then
            fileFullPath2 = replace(LCase(fileFullPath), ".xlsx", ".xls")
        
        
            Dim xlWorkBook
    							
		    'To fix CSBR-36657
		    'Second parameter is UpdateLinks. Set to 0 to not update any links
		    'bogusPassword is set to catch error for password-protected files.
		    'IgnoreReadOnlyRecommended set to true
		    on error resume next
		    Set xlWorkBook = Application("xlApp").Workbooks.Open (fileFullPath, 0, false, , "bogusPassword", , True)
    		
		    If err.number = 0 then
			    'Session("textFound") = ""
			    'on error resume next
			    'Session("textFound") = ParseDocument(xlWorkBook, "EXCEL")

			    on error resume next		
			    xlWorkBook.SaveAs fileFullPath2,-4143 'xls file
			    fileFullPath = fileFullPath2
		    Elseif err.number = 1004 then
			    'this has not been tested yet
			    returnStr = "ERROR: The file is password protected. Please unprotect the file and try again." & "<br>" & _
						    "Another possible cause of the error: Couldn't find the file on server. Check the temp file path."
		    End if
    		
		    xlWorkBook.Close
    			
		    Set xlWorkBook = Nothing
    		        
        elseif LCase(Right(fileFullPath, 5)) = ".pptx" then
			 fileFullPath2 = replace(LCase(fileFullPath), ".pptx", ".ppt")	

		    Dim pptApp, pptPresentation
    					
		    Set pptApp = CreateObject("Powerpoint.Application")
		    pptApp.Visible = true
    		
		    On Error Resume Next
		    Set pptPresentation = pptApp.Presentations.Open(fileFullPath)

		    If err.number = 0 then

			    pptPresentation.SaveAs fileFullPath2
			    fileFullPath = fileFullPath2
		    Else
			    returnStr = "ERROR: PowerPoint could not open this file. The file could be corrupted. Please correct it and retry."
		    End if
    		
    					
		    pptPresentation.Close
		    pptApp.Quit
    					
		    'Delete the file in temp dir
		    'fso.DeleteFolder(Session("sessionTempFolder"), true)
    				
		    Set pptPresentation = Nothing
		    Set pptApp = Nothing    
        
        end if


	if UCase(Typename(myMolParser)) = "PARSER" then
	
		on error resume next
		retVal = myMolParser.ParseDoc(Application("cnnStr"), _
											CLng(doc_uid), _
											CStr(fileFullPath), _
											CStr(mstPath), _
											CStr(Session("fileTempFolder") & "\"), _
											Application("Struct_Engine"))
																
		Set cmd = nothing
	else
		retVal = "ERROR: Cannot create MolParser.Parser. Re-register molparser.dll and recreate MolParser COM+ application and try again."
	end if
	
	ParseStructures = retVal
End Function

Function GetFileType(s)
	Dim lastDotPos
	
	lastDotPos = InstrRev(s, ".")
	If lastDotPos > 0 then
		GetFileType = Right(s, Len(s) - lastDotPos)
	Else
		GetFileType = ""
	End if
End Function

'Function AddExternalLink(conn, docid, SEPNumber, submitter)
Function AddExternalLink(conn, docid, REGNumber, submitter)
	'stop
	dim cmd, param
	
	sql = "INSERT INTO DOCMGR_EXTERNAL_LINKS (APPNAME, LINKTYPE, LINKID, DOCID, LINKFIELDNAME, SUBMITTER, DATE_SUBMITTED) " & _
	"VALUES ('CHEMREG', 'CHEMREGREGNUMBER', '" & UCase(REGNumber) & "', " & docid & ", 'REG_NUMBER', '" & submitter & "', SYSDATE)"
	'"VALUES ('CHEMREG', 'CHEMREGREGNUMBER', '" & SEPNumber & "', " & docid & ", 'REG_NUMBER', '" & submitter & "', SYSDATE)"

	Set cmd = Server.CreateObject("ADODB.Command")
	with cmd
		.ActiveConnection = conn
		.CommandText = sql	
	end with
		
	on error resume next
	cmd.Execute()
	
	if err.number <> 0 then
		retVal = "ERROR: " & err.number & " -- " & err.Description
	else 
		retVal = ""
	end if
	
	Set cmd = nothing
	
	AddExternalLink = retVal
End Function
%>



<script language="javascript">
	//SYAN modifid on 10/25/2004 to fix CSBR-48313
	location.replace('locateDocs.asp?docid=' + '<%=session("doc_uid")%>');
</script>