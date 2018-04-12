Attribute VB_Name = "MCDUtils"
Option Explicit

' ##MODULE_SUMMARY A set of utilities for managing data written to temporary files.

Public Function GetTempPath() As String
    Dim fso As New FileSystemObject
    GetTempPath = fso.GetSpecialFolder(TemporaryFolder).Path
End Function

Public Sub WriteBytes(FilePath As String, destData As Variant)
    Dim NewData() As Byte
    
    NewData = destData
    WriteByteArray FilePath, NewData
End Sub

Public Function GetTempFile(ByVal theExt As String) As String
    Dim rndInt As Long
    Dim firstCInt As Long
    Dim fileName As String
    Dim fso As New FileSystemObject
    Dim thePath As String
    
    If Left(theExt, 1) <> "." Then
        theExt = "." & theExt
    End If
    
    thePath = fso.GetSpecialFolder(TemporaryFolder).Path
    If Right(thePath, 1) <> "\" Then
        thePath = thePath & "\"
    End If
    
    Do
        rndInt = Int((100001) * Rnd)
        firstCInt = Int((89 - 64 + 1) * Rnd + 64)
        
        fileName = thePath & Chr$(firstCInt) & Format(rndInt, "000000") & theExt
    Loop Until fso.FileExists(fileName) = False
    
    Set fso = Nothing
    GetTempFile = fileName
End Function

