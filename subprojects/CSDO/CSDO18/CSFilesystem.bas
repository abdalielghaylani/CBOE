Attribute VB_Name = "Filesystem"
Option Explicit

' CS Common routines for temporary directories, text files

Declare Function GetTempPath Lib "kernel32" Alias _
"GetTempPathA" (ByVal nBufferLength As Long, ByVal _
lpBuffer As String) As Long

Public Const MAX_PATH = 1024

Public Function GetTmpPath()
Dim strFolder As String
Dim lngResult As Long

strFolder = String(MAX_PATH, 0)
lngResult = GetTempPath(MAX_PATH, strFolder)
If lngResult <> 0 Then
  GetTmpPath = Left(strFolder, InStr(strFolder, _
  Chr(0)) - 1)
Else
  GetTmpPath = ""
End If

End Function


Public Function ReadTextFromFile(fileName As String) As String
    ' read contents of file into var
    Dim fileNum As Integer
    fileNum = FreeFile
    Open fileName For Input As #fileNum
    ReadTextFromFile = Input(LOF(1), #fileNum)
    Close #fileNum
End Function

Public Function SaveTextToFile(fileName As String, contents As String) As Boolean
    Dim fileNum As Integer
    fileNum = FreeFile
    Open fileName For Output As #fileNum
    Print #fileNum, contents
    Close #fileNum
    SaveTextToFile = True
End Function

Public Function DeleteFile(fileName As String) As Boolean
    Kill fileName
    DeleteFile = True
End Function

Public Function FileExists(fileName As String) As Boolean
    If Dir$(fileName) Then
        FileExists = True
    Else
        FileExists = False
    End If
End Function

