Attribute VB_Name = "Base64Utils"
Option Explicit

' ##MODULE_SUMMARY A set of utilities for manipulating Base64 encoded data.

Public Function Base64Decode(ByVal base64input As String) As Byte()
    Dim codec As New Base64Codec
    Dim dataV As Variant
    
    codec.Decode base64input, dataV
    Base64Decode = dataV
End Function

Public Function ReadTextFromFile(FileName As String) As String
    ' read contents of file into var
    Dim fileNum As Integer
    fileNum = FreeFile
    Open FileName For Input As #fileNum
    ReadTextFromFile = Input(LOF(1), #fileNum)
    Close #fileNum
End Function

Public Function Base64Encode(cdxInput() As Byte) As String
    Dim codec As New Base64Codec
    Dim dataV As Variant
    
    codec.Encode cdxInput, dataV
    Base64Encode = dataV
End Function

Public Function Base64EncodeFile(ByVal Path As String) As String
    Dim fso As New FileSystemObject
    Dim fSize As Long
    Dim fData() As Byte
    Dim codec As New Base64Codec
    Dim dataV As Variant
    
    fSize = fso.GetFile(Path).Size
    ReDim fData(0 To fSize - 1)
    
    ReadByteArray Path, fData, fSize
    codec.Encode fData, dataV
    Base64EncodeFile = dataV
End Function




