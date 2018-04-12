VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Decode"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Private Declare Function Base64File Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Boolean, ByVal NetVariant As Boolean) As Long
Private Declare Function Base64 Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Boolean, ByVal NetVariant As Boolean) As Long



Public Function Decode(ByVal base64input As String, ByVal cdxOutPath As String) As Boolean
    Dim olen As Long
    olen = Base64(base64input, cdxOutPath, 0, False)
    If olen > 0 Then
        Decode = True
    Else
        Decode = False
    End If
End Function


Public Function Encode(ByVal cdxInputFile As String)
    Dim success As Boolean
    Dim olen As Long
    Dim DestFileName As String
    Dim myBase64Str As String
    Dim fs As Object
    Dim a As Object
    On Error GoTo errHandler
    
    DestFileName = Replace(cdxInputFile, ".cdx", ".txt")
    olen = Base64File(cdxInputFile, DestFileName, True, False)
    
    'open file and get as string
    Set fs = CreateObject("Scripting.FileSystemObject")
    Set a = fs.OpenTextFile(DestFileName)
    myBase64Str = a.ReadAll()
    a.Close
    success = fs.DeleteFile(DestFileName)
    Set a = Nothing
    Set fs = Nothing
    
    'return base64 string to caller.
    Encode = myBase64Str

Exit Function
errHandler:
    Encode = Err.Number & " " & Err.Description
    Exit Function

End Function




