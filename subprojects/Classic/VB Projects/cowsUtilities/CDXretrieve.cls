VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Objretrieve"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Public Function ObjWrite(ByVal inputval, ByVal outpath) As Boolean
    On Error Resume Next
    Dim InputObject() As Byte
    InputObject = inputval
    Open outpath For Binary Access Write As #1
        Put #1, , InputObject
        Close #1
    If Err.Number > 0 Then
        ObjWrite = False
    Else
        ObjWrite = True
    End If
End Function
