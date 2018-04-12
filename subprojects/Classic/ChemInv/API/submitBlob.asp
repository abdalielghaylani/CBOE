<SCRIPT RUNAT="Server" Language="VbScript">
Server.ScriptTimeout = 1200

Const IN_CHUNK_SIZE = 10240

'escape "'" for PL/SQL
Function MyEscape(ByVal myStr)
	if myStr <> "" then
		myStr = replace(myStr, "'", "''")
	end if
	MyEscape = myStr
End Function

Function SaveUploadToDB(fileFullPath, requestId)
	
	Dim fso
	dim nRound
	dim leftOver
	dim f
	dim bytes
	dim i
	Dim textFoundArr
	
	If fileFullPath <> "" Then

		Set fso = CreateObject("Scripting.FileSystemObject")

		if not fso.FileExists(fileFullPath) then
			SaveUploadToDB = "file not found: " & fileFullPath
		end if
		
        
        		
		dim rst, sql
		
        on error resume next		
		Set f = fso.GetFile(fileFullPath)		
		'rst.CursorLocation = adUseClient     	
    	
        if err.number > 0 then
             cfwlogaction "errornum= " & err.number	
             cfwlogaction "errordescription= " & err.number
        end if

	    nRound = Int(fSize / IN_CHUNK_SIZE)
    	
	    leftOver = fSize Mod IN_CHUNK_SIZE
    					
	    For i = 0 To (nRound - 1)

		    'Write out progress
		    percentageDone = CDbl(i/nRound*100)
    		
		    on error resume next
		    bytes = ReadFile(fileFullPath, i * IN_CHUNK_SIZE, IN_CHUNK_SIZE)


		    if typename(bytes) = "String" then
			    if Instr(bytes, "ERROR: ") > 0 then
				    'Response.Write bytes
				    'Response.end
                    cfwlogaction "bytes= " & bytes
			    end if
		    else
			    AppendChunk requestId, UBound(bytes) + 1, bytes
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
		    AppendChunk requestId, UBound(bytes) + 1, bytes
	    end if
    	Set cmd = Nothing
	    'Clean up
	    cnn.Close
	    Set cnn = Nothing
	    Set fso = nothing
				
	Else 'fileFullPath = ""
	
		status = "ERROR: File path empty."
			
	End If
	
	SaveUploadToDB = status
End Function


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

Function AppendChunk(requestId, amount, buffer)


    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.UpdateBlob", adCmdStoredProc)

	With Cmd	
        
		Set param = .CreateParameter("p_requestId", adInteger, adParamInput)
		param.Value = Clng(requestId)
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
	end with
		
	on error resume next
	cmd.Execute()

	Set cmd = nothing
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
</SCRIPT>