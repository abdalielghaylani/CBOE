<%

'************************************************************************
'*PURPOSE	-- Write binary as file on disk. The binary could be		*
'*				upload JScriptTypeInfo object or binary written out 	*
'*				from Oracle BLOB										*
'*INPUT		-- Binary, fileName											*
'*OUTPUT	-- Full path include file name on disk						*
'************************************************************************
Function WriteBinaryToFile(bin, fileName)
	Dim fileTempFolder, subFolder, fileFullPath, htmlFullPath, fso
	
	Set fso = CreateObject("Scripting.FileSystemObject")
	if not fso.FolderExists(Session("sessionTempFolder")) then
		fso.CreateFolder(Session("sessionTempFolder"))
	end if
			
	subFolder = "at" & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)
	fileTempFolder = fso.BuildPath(Session("sessionTempFolder"), subFolder)

	fso.CreateFolder(fileTempFolder)
	fileFullPath = fileTempFolder & "\" & fileName	
	
	if typename(bin) = "JScriptTypeInfo" then 'when bin is the Field of Upload object
		bin.Value.SaveAs fileFullPath 
	else
	end if
	

	'this a few lines may be moved to somewhere else
	Session("fileTempFolder") = fileTempFolder
	Session("fileFullPath") = fileFullPath
	Session("fileName") = fileName
	Set Session("fileHexString") = bin.Value 'bin.Value is a byte()
	'----
	
	Set fso = nothing

	WriteBinaryToFile = fileFullPath

End Function

%>

