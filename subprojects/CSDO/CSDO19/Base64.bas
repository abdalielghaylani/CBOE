Attribute VB_Name = "Base64"
Option Explicit

Private Declare Function Base64File Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Boolean, ByVal NetVariant As Boolean) As Long
Private Declare Function Base64 Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Long, ByVal NetVariant As Boolean) As Long

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
    Dim olen As Long
    Dim DestFileName As String
    Dim myBase64Str As String
On Error GoTo errHandler

    DestFileName = Replace(cdxInputFile, ".cdx", ".txt")
    olen = Base64File(cdxInputFile, DestFileName, True, False)

    'open file and get as string
    myBase64Str = ReadTextFromFile(DestFileName)
    DeleteFile DestFileName

    'return base64 string to caller.
    Encode = myBase64Str

Exit Function
errHandler:
    Encode = Err.Number & " " & Err.Description
    Exit Function

End Function



