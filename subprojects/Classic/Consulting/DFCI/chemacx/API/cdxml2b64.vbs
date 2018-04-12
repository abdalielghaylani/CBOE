    Dim oFS
    inPath = "CDXML\"
    outpath = "B64CDX\" 	
    
    Set oFs = CreateObject("Scripting.FileSystemObject")
    Set oCdax = CreateObject("ChemDrawControl8.ChemDrawCtl")
    
    Set ColFiles = GetFilesCollection(inPath)
    i = 0
    For Each File In ColFiles
        fnLenght = Len(File.Name)
        fndot = InStr(1, File.Name, ".")
        fnExt = Mid(File.Name, fndot, fnLenght - fndot + 1)
        fileName = Left(File.Name, fndot - 1)
        If fnExt = ".cdxml" Then
            oCdax.open inPath & file.name, true
            oCdax.DataEncoded = true
	    b64 = oCdax.Data("chemical/x-cdx")		
            Set f = oFs.OpenTextFile(outpath & fileName & ".xml", 2, True)
	    f.writeline "<cdtemplate id=""1""><p>" & replace(b64, vbcrlf, "\n") & "</p></cdtemplate>"		
    	End if
    i=i+1
    Next
    
    msgbox  i & " structures where converted"



Function GetFilesCollection(ByVal strFolderPath)
    Dim oFs
    Set oFs = CreateObject("Scripting.FileSystemObject")
    Set oFolder = oFs.GetFolder(strFolderPath)
    Set GetFilesCollection = oFolder.Files
    Set oFs = Nothing
    Set oFolder = Nothing
End Function
