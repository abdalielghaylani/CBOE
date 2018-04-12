Attribute VB_Name = "Module1"
' This is a simple exe program meant to be executed from the command line
' to do a regular expression based replacement of text in a text file.
' Usage REreplace.exe filepath; replacement; REpattern;[U]
' The optional third paramater supports unicode files

Private Sub Main()
    Dim theString As String
    Dim theReplacement As String
    Dim theREPattern As String
    Dim ColFiles As Object
    Dim iFile As Integer
    Dim aTemp() As String
    Dim FolderPath, PathSpec, inExt, fnExt As String
    Dim inLength, inlastSlash, indot, fnLength, fndot As Integer
    Dim Rts, Wts As TextStream
    Dim fFormat As Integer
    
    aTemp = Split(Command, ";")

    PathSpec = Trim$(aTemp(0))
    theReplacement = Trim$(aTemp(1))
    theREPattern = Trim$(aTemp(2))
    
    
    fFormat = -2
    If UBound(aTemp) = 3 Then
        If Trim$(aTemp(3)) = "U" Then fFormat = -1
    End If
    
    inLength = Len(PathSpec)
    inlastSlash = InStr(1, StrReverse(PathSpec), "\")
    indot = InStr(1, PathSpec, ".")
    FolderPath = Left(PathSpec, inLength - inlastSlash + 1)
    inExt = Mid(PathSpec, indot, inLength - indot + 1)
    If inlastSlash > 0 Then
        inFileName = Mid(PathSpec, (inLength - inlastSlash) + 2, indot - (inLength - inlastSlash) + 2)
    Else
        iniFileName = PathSpec
    End If
    
    If InStr(1, inFileName, "*") > 0 Then
        Set ColFiles = GetFilesCollection(FolderPath)
        For Each File In ColFiles
            fnLenght = Len(File.Name)
            fndot = InStr(1, File.Name, ".")
            fnExt = Mid(File.Name, fndot, fnLenght - fndot + 1)
            If fnExt = inExt Then
                Set Rts = File.OpenAsTextStream(ForReading, fFormat)
                theString = Rts.ReadAll()
                Rts.Close
                Set Wts = File.OpenAsTextStream(ForWriting, fFormat)
                Wts.Write (REReplace(theString, theReplacement, theREPattern, True))
                Wts.Close
            End If
        Next
    Else
        Dim fso As FileSystemObject
        
        Set fso = New FileSystemObject
        
        Set Rts = fso.OpenTextFile(PathSpec, ForReading, False, fFormat)
        theString = Rts.ReadAll()
        Rts.Close
        
        Set Wts = fso.OpenTextFile(PathSpec, ForWriting, True, fFormat)
        Wts.Write (REReplace(theString, theReplacement, theREPattern, True))
        Wts.Close
        Set Rts = Nothing
        Set Wts = Nothing
        Set fso = Nothing
        
        'iFile = FreeFile
        'Open PathSpec For Input As #iFile
        'theString = Input(LOF(1), #iFile)
        'Close #iFile
    
        'Open PathSpec For Output As #iFile
        'Print #iFile, REReplace(theString, theReplacement, theREPattern, True)
    End If
End Sub

Private Function REReplace(pString As String, pReplacement As String, pREPattern As String, bIsGlobal)
    Dim oRE As RegExp
    
    Set oRE = New RegExp
    oRE.Global = bIsGlobal
    oRE.Pattern = pREPattern
    REReplace = oRE.Replace(pString, pReplacement)
End Function

Function GetFilesCollection(ByVal strFolderPath As String)
    Dim oFs As FileSystemObject
    Dim oFolder As Folder
    
    Set oFs = New FileSystemObject
    Set oFolder = oFs.GetFolder(strFolderPath)
    Set GetFilesCollection = oFolder.Files
    Set oFs = Nothing
    Set oFolder = Nothing
End Function
