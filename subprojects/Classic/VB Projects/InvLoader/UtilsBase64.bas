Attribute VB_Name = "UtilsBase64"
Option Explicit

Private Declare Function Base64File Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Boolean, ByVal NetVariant As Boolean) As Long
Private Declare Function Base64 Lib "Base64" (ByVal iStr As String, ByVal outFile As String, ByVal iLen As Boolean, ByVal NetVariant As Boolean) As Long

' ##MODULE_SUMMARY A set of utilities for manipulating Base64 encoded data.

'Public Function Base64Decode(ByVal base64Input As String) As Byte()
'    Dim codec As Object
'    Set codec = CreateObject("Base64Decode.Decode")
'    Dim dataV As Variant
'
'    codec.Decode base64Input, dataV
'    Base64Decode = dataV
'End Function
'
'Public Function Base64Encode(cdxInput() As Byte) As String
'    Dim codec As Object
'    Set codec = CreateObject("Base64Decode.Decode")
'    Dim dataV As Variant
'
'    codec.Encode cdxInput, dataV
'    Base64Encode = dataV
'End Function

Public Sub Base64DecodeToFile(ByVal fName As String, ByVal base64input As String)
    Dim codec As Object
    Set codec = CreateObject("Base64Decode.Decode")
    codec.Decode base64input, fName
'    Dim fData() As Byte
'
'    fData = Base64Decode(base64Input)
'    WriteByteArray fName, fData
End Sub

Public Function Base64EncodeFile(ByVal Path As String) As String
'    Dim fso As New FileSystemObject
'    Dim fSize As Long
'    Dim fData() As Byte
    Dim codec As Object
    Set codec = CreateObject("Base64Decode.Decode")
    
    Base64EncodeFile = codec.Encode(Path)
    
'    Dim dataV As Variant
'
'    fSize = fso.GetFile(Path).Size
'    ReDim fData(0 To fSize - 1)
'
'    ReadByteArray Path, fData, fSize
'    codec.Encode fData, dataV
'    Base64EncodeFile = dataV
End Function

Public Function CheckBase64() As Boolean
    Dim FilePath As String
    Dim EmptyStruct As String
    Dim olen As Long
    
    CheckBase64 = True
    
    FilePath = Filesystem.GetTmpPath() + "blank.cdx"
    EmptyStruct = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAEAAAAENoZW1EcmF3IDYuMC4xCAAMAAAAbXl0ZXN0LmNkeAADMgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////AAD//wEJCAAAAFkAAAAEAAIJCAAAAKcCAAAXAgIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIAeAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMAYAC0AAMACwgIAAQAAADwAAMADQgAAAAIeAAAAwAAAAEAAQAAAAAACwARAAAAAAALABEDZQf4BSgAAgAAAAEAAQAAAAAACwARAAEAZABkAAAAAQABAQEABwABJw8AAQABAAAAAAAAAAAAAAAAAAIAGQGQAAAAAAJAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAEAhAAAAD+/wAA/v8AAAIAAAACAAABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgAEAAAAEAhAAAAD+/wAA/v8AAAIAAAACAA8IAgABABAIAgABABYIBAAAACQAGAgEAAAAJAAAAAAA"
    On Error GoTo errHandler
    olen = Base64(EmptyStruct, FilePath, 0, False)
    Kill (FilePath)
    
    Exit Function

errHandler:
    If LCase(Err.Description) = "file not found: base64" Then
        CheckBase64 = False
    End If
    Exit Function

End Function
