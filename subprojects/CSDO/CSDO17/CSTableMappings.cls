VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSChemLinks"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Private moCol As Collection

Public Property Get Item(ByVal avIndexKey As Variant) As CSChemLink
    Set Item = moCol(avIndexKey)
End Property

Public Property Get NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = moCol.[_NewEnum]
End Property

Public Property Get count() As Long
    count = moCol.count
End Property

Private Sub Class_Initialize()
    'creates the collection when this class is created
    Set moCol = New Collection
End Sub

Private Sub Class_Terminate()
    'destroy class objects when this class is terminated
    Set moCol = Nothing
End Sub

Public Sub Clear()
    Dim i As Long
    Dim count As Long
    count = moCol.count
    For i = 1 To count
        moCol.Remove 1
    Next
End Sub

Public Sub ClearFields()
    Dim cl As CSChemLink
    For Each cl In Me
        cl.QueryFields.Clear
        cl.ReturnFields.Clear
    Next cl
End Sub

Private Function Append(newLink As CSChemLink) As CSChemLink
    ' If the add fails, then it may be because this link already exists in the table.
    ' If this is true, then do not do anything.
    On Error Resume Next
    moCol.Add newLink, newLink.RelTable
    If (Err.Number > 0) Then
        Dim dest As CSChemLink
        Dim f As CSField
        
        Set dest = moCol(newLink.RelTable)
        For Each f In newLink.QueryFields
            dest.QueryFields.AddFromExisting f
        Next
        For Each f In newLink.ReturnFields
            dest.ReturnFields.AddFromExisting f
        Next
        Err.Clear
        
    End If
    Set Append = moCol(newLink.RelTable)
End Function

Public Function Add(ByVal RelTable As String, _
                    ByVal RelField As String, _
                    ByVal MolConnString As String, _
                    ByVal MolPassword As String, _
                    ByVal MolOptions As Long) As CSChemLink
    ' Add a new project to the collection
    Dim toNewMember As CSChemLink
        
    ' Add a new key to the collection
    Set toNewMember = New CSChemLink
    With toNewMember
        .RelTable = RelTable
        .RelField = RelField
        .MolConnString = MolConnString
        .MolPassword = MolPassword
        .MolOptions = MolOptions
    End With
    
    Set Add = Append(toNewMember)
End Function

Friend Function AddFromXML(strDoc As String) As CSChemLink
    Set AddFromXML = Add(GetNextTaggedItem(strDoc, "RelTable"), GetNextTaggedItem(strDoc, "RelField"), _
        GetNextTaggedItem(strDoc, "MolConnString"), GetNextTaggedItem(strDoc, "MolPassword"), _
        CLng(GetNextTaggedItem(strDoc, "MolOptions")))
End Function







